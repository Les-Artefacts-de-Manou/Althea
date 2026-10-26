' -------------------------------------------------------------------------------------------------
' Formulaire : PaiementEditor
' Projet     : Althéa
' Version    : V2.2.0
' Date       : 25/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Form modale permettant la saisie d'un paiement affecté à une séance donnée (tables paiements +
' lia_paiements_seances), depuis l'onglet « Paiements » du hub dossier (UC_DossierFiche).
'
' Responsabilités :
' - Saisir la date, le montant, le mode de paiement (référentiel ref_modes_paiement), une référence
'   et un commentaire
' - Permettre l'ajout « en contexte » d'un mode de paiement manquant via un bouton [+] (D-Q17)
' - Pré-remplir le montant avec le reste dû de la séance en création
' - Persister le paiement via GestionPaiements (création transactionnelle paiement + liaison)
'
' Fonctionnement :
' - Mode Création uniquement en V1 : un paiement affecté à une séance (D-PAY-1)
'
' Important   :
' - Retour DialogResult.OK après enregistrement réussi ; DialogResult.Cancel sinon
' - La séance doit être déjà enregistrée (idSeance > 0)
' - Aucun accès base direct : délègue à GestionPaiements / GestionModesPaiement
' - Le reste dû est calculé par la couche métier à partir des seuls paiements encaissés.
' - V1.2.0 (15/10/2026, UI-7) : cartes « Encaissement » et « Commentaire », fond AltheaFondFenetre,
'   listes AltheaListe, date AltheaDate, [+] en AltheaBouton Outil à icône seule, consultation lisible.
' - V1.3.0 (17/10/2026) : une saisie refusée est expliquée dans la fenêtre (phrase rose sous les
'   champs, montant teinté), et plus seulement dans la barre d'état, peu visible depuis une fenêtre.
' - V1.4.0 (17/10/2026) : l'en-tête nomme la séance par son code (« Encaissement de la séance
'   SE000074 »), y compris quand elle est choisie dans la liste ; le rappel commence par le code.
' - V1.5.0 (19/10/2026) : modifications non enregistrées — fermer la fenêtre (Annuler, croix, Échap)
'   après une modification propose Enregistrer / Quitter sans enregistrer / Rester (GardeModifications).
' - V1.6.0 (20/10/2026) : commentaire dans une AltheaZoneTexte : correcteur d'orthographe de Windows ;
'   il reste en texte simple (Althéa y ajoutait ses mentions jusqu'au 22/10/2026 : annulation,
'   utilisation d'un avoir ; elles ont désormais leurs colonnes, ADR-49).
' - V1.7.0 (21/10/2026, audit B.5) : le mode choisi est transmis par son identifiant (il l'était par
'   son libellé) ; LireLibelleModeSelectionne → LireIdModeSelectionne.
' - V1.8.0 (22/10/2026, audit C.1) : refus du métier (dossier clôturé ou archivé) et échec de
'   l'enregistrement affichés dans une fenêtre (ils ne restaient que dans la barre d'état).
' - V1.9.0 (22/10/2026, audit C.2) : la saisie est jugée par GestionPaiements.ValiderEncaissement,
'   la règle que CreatePaiement applique aussi ; la fenêtre n'en garde plus de copie.
' - V2.0.0 (22/10/2026, décision de Joëlle) : la séance est toujours imposée par l'appelant (bouton
'   « Encaisser séance ») ; le mode « choisir la séance dans la liste des séances dues », qui ne
'   servait qu'au bouton « Nouveau paiement » retiré de l'onglet Paiements, est supprimé (liste
'   cboSeance, paramètre idDossier, InitialiserSeance, AppliquerSeanceSelectionnee).
'
' Dépendances :
' - GestionPaiements / Paiement (persistance et modèle)
' - GestionModesPaiement / ModePaiement (combo Mode de paiement)
' - IFenetrePrincipale.OuvrirReferentielModal / UC_ModesPaiement (ajout de référentiel en contexte)
' - UserControlContext / IContextAwareForm (contexte UI partagé)
' - GestionLog (journalisation)
' - V2.1.0 (23/10/2026, relevé de Joëlle) : la barre d'état annonce le code du paiement enregistré
'   (« Paiement PAI000123 enregistré. »).
' - V2.2.0 (25/10/2026, audit C.4, ADR-55) : fenêtre principale reçue comme IFenetrePrincipale (et non
'   plus Home) ; boutons [+] inchangés.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class PaiementEditor
    Implements IContextAwareForm

#Region "Variables privées"

    ' Contexte UI partagé fourni par l'appelant (barre de statut, infobulles). Peut être Nothing.
    Private _context As UserControlContext

    ' Identifiant de la séance à laquelle le paiement est affecté (imposée par l'appelant, > 0).
    Private ReadOnly _idSeance As Long

    ' Code technique de la séance (affichage rappel).
    Private ReadOnly _codeSeance As String

    ' Libellé de rappel de la séance (« jj/mm/aaaa (Nature) »), plus parlant que le seul code.
    Private ReadOnly _libelleSeance As String

    ' Reste dû de la séance au moment de l'ouverture (pré-remplissage du montant, contrôle).
    Private ReadOnly _resteDu As Decimal

    ' Force la lecture seule quel que soit le mode (ex. dossier archivé).
    Private ReadOnly _lectureSeuleForcee As Boolean

    ' Fenêtre principale, nécessaire pour ouvrir un référentiel en contexte ([+]). Peut être Nothing.
    Private ReadOnly _fenetrePrincipale As IFenetrePrincipale

    ' Empêche des traitements pendant le chargement initial.
    Private _chargementEnCours As Boolean = False

    ' Fond d'origine du montant, rétabli quand l'erreur signalée est corrigée.
    Private _fondMontantNormal As Color

#End Region

#Region "Constructeur"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version       : V2.2.0
    ' Date          : 25/10/2026
    '
    ' Rôle          :
    ' Initialise la modale pour la saisie d'un paiement affecté à une séance.
    '
    ' Paramètres    :
    ' - idSeance           : Identifiant de la séance à encaisser (obligatoire, > 0)
    ' - codeSeance         : Code technique de la séance (affichage rappel), optionnel
    ' - resteDu            : Reste dû de la séance, pour pré-remplir le montant, optionnel
    ' - lectureSeuleForcee : Force la lecture seule (dossier archivé), optionnel
    ' - fenetrePrincipale : Fenêtre principale, requise pour activer le bouton [+], optionnel
    ' - libelleSeance      : Rappel lisible de la séance (« jj/mm/aaaa (Nature) »), optionnel
    '
    ' Remarques     :
    ' - V2.0.0 (22/10/2026) : paramètre idDossier retiré (plus de choix de la séance dans la fenêtre).
    ' - V2.2.0 (25/10/2026, audit C.4, ADR-55) : reçoit la fenêtre principale (IFenetrePrincipale)
    '   au lieu de Home.
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(idSeance As Long,
                   Optional codeSeance As String = "",
                   Optional resteDu As Decimal = 0D,
                   Optional lectureSeuleForcee As Boolean = False,
                   Optional fenetrePrincipale As IFenetrePrincipale = Nothing,
                   Optional libelleSeance As String = "")

        If idSeance <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(idSeance))

        InitializeComponent()

        ' Fond du kit : beige clair et petite feuille dessinée.
        AltheaFondFenetre.Appliquer(Me)

        ' Erreur de saisie : teintes de la charte (même rose que les messages d'erreur DialogChoix).
        lblErreur.ForeColor = AltheaIconeEtat.Teintes(EtatIcone.Erreur).Foncee
        _fondMontantNormal = numMontant.BackColor

        _idSeance = idSeance
        _codeSeance = codeSeance
        _libelleSeance = libelleSeance
        _resteDu = resteDu
        _lectureSeuleForcee = lectureSeuleForcee
        _fenetrePrincipale = fenetrePrincipale

    End Sub

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SetContext
    ' Version   : V1.0.0
    ' Date      : 15/09/2026
    '
    ' Rôle      :
    ' Reçoit le contexte UI partagé fourni par l'appelant. Doit être appelée AVANT ShowDialog.
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) Implements IContextAwareForm.SetContext

        _context = context

    End Sub

#End Region

#Region "Cycle de vie"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : PaiementEditor_Load
    ' Version   : V1.0.0
    ' Date      : 15/09/2026
    '
    ' Rôle      :
    ' Initialise le bouton [+], le combo des modes de paiement, applique le rappel de séance et
    ' les valeurs par défaut (date du jour, montant = reste dû).
    ' -------------------------------------------------------------------------------------------------
    Private Sub PaiementEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        GardeModifications.SurveillerFenetre(Me, btnEnregistrer, "du paiement")

        _chargementEnCours = True

        InitialiserBoutonReferentiel()
        InitialiserCombo()
        AppliquerDefautsCreation()
        AppliquerEtatSaisie()

        _chargementEnCours = False

    End Sub

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserBoutonReferentiel
    ' Version   : V1.1.0
    ' Date      : 15/10/2026
    '
    ' Rôle      :
    ' Applique l'habillage standard au bouton [+] d'ajout de mode de paiement en contexte et le
    ' masque lorsque la fenêtre principale n'est pas disponible.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserBoutonReferentiel()

        ' [+] : AltheaBouton Outil à icône seule, sans UtilsButtons (ses icônes de survol, claires,
        ' disparaîtraient sur le survol vert pâle du kit) : l'icône du Designer reste.
        btnAjouterModePaiement.Visible = _fenetrePrincipale IsNot Nothing

        If _context IsNot Nothing Then
            _context.SetToolTip(btnAjouterModePaiement, "Ajouter un mode de paiement manquant au référentiel.")
            _context.SetToolTip(btnEnregistrer, "Enregistrer le paiement.")
            _context.SetToolTip(btnFermer, "Fermer la fenêtre sans enregistrer.")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserCombo
    ' Version   : V1.0.0
    ' Date      : 15/09/2026
    '
    ' Rôle      :
    ' Alimente le combo Mode de paiement depuis le référentiel actif.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserCombo()

        Dim modes As List(Of ModePaiement) = GestionModesPaiement.GetModesPaiementSaisie()
        cboModePaiement.DataSource = modes
        cboModePaiement.DisplayMember = NameOf(ModePaiement.LibelleModePaiement)
        cboModePaiement.ValueMember = NameOf(ModePaiement.IdModePaiement)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerDefautsCreation
    ' Version   : V1.0.0
    ' Date      : 15/09/2026
    '
    ' Rôle      :
    ' Applique les valeurs par défaut d'un nouveau paiement : rappel de séance, date du jour et
    ' montant présélectionné sur le reste dû (borné au maximum du contrôle).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerDefautsCreation()

        lblSeanceValeur.Text = LibelleRappelSeance()

        NommerSeanceDansEnTete()

        dtpDatePaiement.Valeur = Date.Today

        AppliquerMontantParDefaut()

    End Sub

    ' En-tête : « Encaissement de la séance SE000074 » dès que la séance est connue (les dates seules
    ' ne suffisent pas à distinguer deux séances).
    Private Sub NommerSeanceDansEnTete()
        enTete.SousTitre = If(String.IsNullOrWhiteSpace(_codeSeance),
                              "Encaissement d'une séance",
                              $"Encaissement de la séance {_codeSeance}")
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerMontantParDefaut
    ' Version   : V1.0.0
    ' Date      : 20/09/2026
    '
    ' Rôle      :
    ' Propose par défaut le reste dû de la séance ciblée, borné aux limites du contrôle.
    ' Le montant reste modifiable afin d'autoriser les règlements partiels.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerMontantParDefaut()

        Dim montantDefaut As Decimal = _resteDu
        If montantDefaut < numMontant.Minimum Then
            montantDefaut = numMontant.Minimum
        ElseIf montantDefaut > numMontant.Maximum Then
            montantDefaut = numMontant.Maximum
        End If
        numMontant.Value = montantDefaut

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerEtatSaisie
    ' Version   : V2.0.0
    ' Date      : 15/10/2026
    '
    ' Rôle      :
    ' Applique l'état de saisie des deux cartes : saisie (zones blanches) ou lecture seule forcée
    ' (dossier archivé : tout en consultation lisible, Enregistrer masqué).
    '
    ' Remarques :
    ' - V2.0.0 (UI-7) : remplace AppliquerLectureSeule ; règle « Consultation lisible »
    '   (UtilsControls.DefinirLectureSeule) au lieu de champs désactivés un par un.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerEtatSaisie()

        For Each carte As Control In {carteEncaissement, carteCommentaire}
            UtilsControls.DefinirLectureSeule(carte, _lectureSeuleForcee)
        Next

        btnEnregistrer.Visible = Not _lectureSeuleForcee

    End Sub

#End Region

#Region "Référentiel en contexte"

    ' -------------------------------------------------------------------------------------------------
    ' Événement : btnAjouterModePaiement_Click
    ' Version    : V1.0.0
    ' Date       : 15/09/2026
    '
    ' Rôle       :
    ' Ouvre le référentiel des modes de paiement dans une fenêtre modale (via la fenêtre principale), puis recharge
    ' le combo en sélectionnant automatiquement la valeur nouvellement créée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAjouterModePaiement_Click(sender As Object, e As EventArgs) Handles btnAjouterModePaiement.Click

        If _fenetrePrincipale Is Nothing OrElse _lectureSeuleForcee Then
            Return
        End If

        ' Identifiants présents avant l'ouverture du référentiel (détection des ajouts).
        Dim idsAvant As New List(Of ULong)
        Dim listeAvant As List(Of ModePaiement) = TryCast(cboModePaiement.DataSource, List(Of ModePaiement))

        If listeAvant IsNot Nothing Then
            For Each mode As ModePaiement In listeAvant
                idsAvant.Add(mode.IdModePaiement)
            Next
        End If

        ' Valeur actuellement sélectionnée, à préserver si aucun ajout n'est effectué.
        Dim idSelectionCourante As Object = cboModePaiement.SelectedValue

        If Not _fenetrePrincipale.OuvrirReferentielModal(New UC_ModesPaiement(), "Modes de paiement") Then
            Return
        End If

        Try

            Dim modesApres As List(Of ModePaiement) = GestionModesPaiement.GetModesPaiementSaisie()

            ' Recherche d'un identifiant nouvellement créé (présent après, absent avant).
            Dim idNouveau As ULong? = Nothing
            For Each mode As ModePaiement In modesApres
                If Not idsAvant.Contains(mode.IdModePaiement) Then
                    idNouveau = mode.IdModePaiement
                    Exit For
                End If
            Next

            _chargementEnCours = True

            cboModePaiement.DataSource = modesApres
            cboModePaiement.DisplayMember = NameOf(ModePaiement.LibelleModePaiement)
            cboModePaiement.ValueMember = NameOf(ModePaiement.IdModePaiement)

            If idNouveau.HasValue Then
                cboModePaiement.SelectedValue = idNouveau.Value
            ElseIf idSelectionCourante IsNot Nothing Then
                cboModePaiement.SelectedValue = idSelectionCourante
            End If

            _chargementEnCours = False

        Catch ex As Exception

            _chargementEnCours = False

            GestionLog.EcrireLog(
                "Erreur rechargement des modes de paiement après ajout (PaiementEditor).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

#End Region

#Region "Actions"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnEnregistrer_Click
    ' Version   : V1.1.0
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Valide la saisie, persiste le paiement via la couche métier, puis ferme la modale (OK).
    ' - V1.0.1 (21/10/2026, audit B.5) : mode transmis par son identifiant.
    ' - V1.1.0 (22/10/2026, audit C.1) : refus et échec affichés dans une fenêtre ; la fenêtre reste
    '   ouverte, rien n'est enregistré.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEnregistrer_Click(sender As Object, e As EventArgs) Handles btnEnregistrer.Click

        If _lectureSeuleForcee Then
            Return
        End If

        If Not ValiderSaisie() Then
            Return
        End If

        Try

            Dim idCree As Long = GestionPaiements.CreatePaiement(LirePaiementSaisi())

            If idCree <= 0 Then
                ' Échec technique (aucune ligne insérée), pas un refus métier.
                Throw New System.Data.DataException($"Création du paiement : aucune ligne insérée (idSeance={_idSeance}).")
            End If

            ' Le code du paiement est annoncé (on le retrouve ensuite par la recherche, colonne Code).
            _context?.SetStatus($"Paiement {GestionRecherche.CodeElement(NatureResultat.Paiement, idCree)} enregistré.")

            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch exRefus As InvalidOperationException

            ' Refus métier (message rédigé pour l'utilisatrice).
            DialogChoix.Information(exRefus.Message, "Enregistrement impossible")
            _context?.SetStatus("Enregistrement du paiement refusé.")

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur enregistrement paiement (PaiementEditor, idSeance={_idSeance}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur(
                "Le paiement n'a pas pu être enregistré ; rien n'a été modifié." & Environment.NewLine &
                "Vérifiez la connexion à la base de données, puis cliquez à nouveau sur Enregistrer.",
                "Enregistrement impossible")
            _context?.SetStatus("Erreur lors de l'enregistrement du paiement.")

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnFermer_Click
    ' Version   : V1.0.0
    ' Date      : 15/09/2026
    '
    ' Rôle      :
    ' Ferme la modale sans enregistrer (DialogResult.Cancel).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnFermer_Click(sender As Object, e As EventArgs) Handles btnFermer.Click

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

#End Region

#Region "Validation & aides"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ValiderSaisie
    ' Version  : V2.0.0
    ' Date     : 22/10/2026
    '
    ' Rôle     :
    ' Fait juger la saisie par le métier (GestionPaiements.ValiderEncaissement : séance, montant
    ' strictement positif et au plus égal au reste dû, mode choisi) et explique la première erreur
    ' sur son champ.
    '
    ' Retour   :
    ' - Boolean : True si la saisie est valide, False sinon (refus expliqué dans la fenêtre).
    '
    ' Remarques :
    ' - V1.1.0 : le motif du refus s'affiche dans la fenêtre (SignalerErreur) en plus de la barre d'état.
    ' - V2.0.0 (22/10/2026, audit C.2) : règle déplacée dans le métier ; la fenêtre ne garde que
    '   l'affichage (une erreur à la fois, comme avant).
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderSaisie() As Boolean

        MasquerErreur()

        ' Date vide : rien à transmettre au métier (Paiement.DatePaiement n'est pas facultative).
        If Not dtpDatePaiement.Valeur.HasValue Then
            SignalerErreur("Indiquez la date du paiement.", dtpDatePaiement)
            Return False
        End If

        Dim erreurs As List(Of ErreurSaisie) = GestionPaiements.ValiderEncaissement(LirePaiementSaisi(), _resteDu)

        If erreurs.Count = 0 Then
            Return True
        End If

        SignalerErreur(erreurs(0).Message, ControleDuChamp(erreurs(0).Champ))
        Return False

    End Function

    ' Contrôle de la fenêtre qui saisit une propriété du paiement (ErreurSaisie.Champ) ; Nothing pour
    ' la séance, imposée par l'appelant.
    Private Function ControleDuChamp(champ As String) As Control

        Select Case champ
            Case NameOf(Paiement.Montant) : Return numMontant
            Case NameOf(Paiement.IdModePaiement) : Return cboModePaiement
            Case Else : Return Nothing
        End Select

    End Function

    ' Paiement tel que saisi dans la fenêtre.
    Private Function LirePaiementSaisi() As Paiement

        Return New Paiement With {
            .IdSeance = _idSeance,
            .DatePaiement = dtpDatePaiement.Valeur.Value.Date,
            .Montant = numMontant.Value,
            .MontantAffecte = numMontant.Value,
            .IdModePaiement = LireIdModeSelectionne(),
            .ReferencePaiement = txtReference.Text.Trim(),
            .Commentaire = txtCommentaire.Text.Trim()
        }

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SignalerErreur
    ' Version   : V1.0.1
    ' Date      : 22/10/2026
    '
    ' Rôle      :
    ' Explique un refus de saisie dans la fenêtre : phrase rose sous les champs (comme le motif
    ' manquant de DialogMotifAnnulation), montant teinté s'il est en cause, focus sur le champ ; le
    ' même message va dans la barre d'état.
    ' - V1.0.1 (22/10/2026) : champ facultatif (erreur qui porte sur la séance, imposée).
    ' -------------------------------------------------------------------------------------------------
    Private Sub SignalerErreur(message As String, champ As Control)

        lblErreur.Text = "⚠ " & message
        lblErreur.Visible = True

        If champ Is numMontant Then
            numMontant.BackColor = AltheaIconeEtat.Teintes(EtatIcone.Erreur).Pale
        End If

        _context?.SetStatus(message)
        champ?.Focus()

    End Sub

    ' Retire le signalement d'erreur (nouvelle validation, ou champ corrigé).
    Private Sub MasquerErreur()
        lblErreur.Visible = False
        numMontant.BackColor = _fondMontantNormal
    End Sub

    ' Dès que l'utilisatrice corrige le montant ou le mode, le signalement disparaît.
    Private Sub ChampCorrige(sender As Object, e As EventArgs) Handles numMontant.ValueChanged, cboModePaiement.SelectedIndexChanged
        If lblErreur.Visible Then MasquerErreur()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LibelleRappelSeance
    ' Version  : V1.0.0
    ' Date     : 20/09/2026
    '
    ' Rôle     :
    ' Retourne le rappel de la séance ciblée, en privilégiant la date et la nature (plus parlantes
    ' que le code technique) et en se repliant sur le code puis l'identifiant.
    '
    ' Retour   :
    ' - String : Libellé affichable dans l'en-tête de la modale.
    ' -------------------------------------------------------------------------------------------------
    Private Function LibelleRappelSeance() As String

        If Not String.IsNullOrWhiteSpace(_libelleSeance) Then
            Return _libelleSeance
        End If

        If Not String.IsNullOrWhiteSpace(_codeSeance) Then
            Return _codeSeance
        End If

        Return $"#{_idSeance}"

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LireIdModeSelectionne
    ' Version  : V2.0.0
    ' Date     : 21/10/2026
    '
    ' Rôle     :
    ' Retourne l'identifiant du mode de paiement sélectionné (paiements.id_mode_paiement).
    '
    ' Retour   :
    ' - ULong? : Identifiant du mode, ou Nothing si aucun élément sélectionné.
    '
    ' Remarques :
    ' - V2.0.0 (21/10/2026, audit B.5) : rendait le libellé (LireLibelleModeSelectionne), enregistré
    '   en clair ; renommer un mode coupait alors le lien avec l'historique.
    ' -------------------------------------------------------------------------------------------------
    Private Function LireIdModeSelectionne() As ULong?

        Dim mode As ModePaiement = TryCast(cboModePaiement.SelectedItem, ModePaiement)

        If mode Is Nothing Then
            Return Nothing
        End If

        Return mode.IdModePaiement

    End Function

#End Region

End Class
