' -------------------------------------------------------------------------------------------------
' UserControl : UC_DomainesHome
' Projet      : Althéa
' Version     : V1.13.2
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Espace dédié à la gestion des domaines, ouvert depuis le menu « Domaines » de l'accueil.
' Un domaine représentant une convention (tarif + organisme + quotas), tout ce qui s'y rattache
' est regroupé ici : identité et comportement, natures de séance, tarifs, quotas, synthèse.
'
' Responsabilités :
' - Lister les domaines avec leur état de complétude (Complet / À compléter / Inactif)
' - Créer et modifier un domaine (onglet Général), saisie jugée par le métier (GestionDomaines.ValiderDomaine)
' - Héberger les panneaux existants UC_LiaisonsNaturesDomaines et UC_LiaisonsPrixDomaines
' - Afficher les règles de quota du domaine
' - Guider l'utilisateur par les étapes et la synthèse (ce qu'il reste à faire)
'
' Remarques   :
' - N'hérite PAS de UC_ReferentielBase : écran à étapes propre. L'ancien UC_Domaines, qui en
'   héritait, a été supprimé le 18/10/2026 (UI-8).
' - UI-6 (14/10/2026, maquette « Domaines — A ») : les étapes (AltheaEtapes) remplacent la bande
'   d'onglets ; chaque étape montre son état (fait, à compléter, non nécessaire), calculé à partir
'   des règles de complétude du métier (GestionDomaines.CalculerManques). L'onglet Général est
'   présenté en deux cartes.
' - V1.5.0 (14/10/2026, UI-6 6d) : infobulle de « Nouvelle règle » alignée sur ADR-40 (une règle qui
'   n'a pas servi se corrige) ; synthèse en carte (Designer). Périodes sans règle : jamais comblées
'   automatiquement (décision de Joëlle) mais affichées à leur place dans la grille sur une ligne
'   d'alerte, proposées par « Nouvelle règle » et annoncées avant toute action qui en créerait une.
' - V1.6.0 (14/10/2026, UI-7) : dialogues des règles de quota au format DialogChoix V2 (récapitulatif
'   en détails, encadré d'attention, boutons « Enregistrer », « Clôturer », « Supprimer »).
' - V1.7.0 (18/10/2026, UI-9.4, maquette « Grilles — états lisibles ») : état des domaines et des
'   règles de quota en pastilles (Complet sauge, À compléter ocre, Inactif gris ; règles :
'   QuotaInamiRegle.Etat ; période sans règle « à surveiller ») ; domaine et règle inactifs
'   atténués ; catégorie inactive en pastille grise ; alerte et plafond à droite.
' - V1.8.0 (19/10/2026) : modifications non enregistrées — quitter l'écran (IUnsavedChangesAware)
'   pendant la saisie d'un domaine ou d'une règle de quota propose Enregistrer / Quitter sans
'   enregistrer / Rester ; Annuler demande « Abandonner / Continuer la saisie » (GardeModifications ;
'   ADR-43). Les rattachements tarifs / natures (panneaux de liaison) restent hors de cette garde.
' - V1.9.0 (19/10/2026) : « Modifier » n'est proposé que sur l'étape 1 (Général), comme sur la fiche
'   d'un dossier ; les étapes 2 à 4 annoncent qu'elles se modifient directement (ADR-40, § 5.7).
' - V1.10.0 (20/10/2026, v2.45.0) : dates des règles de quota en AltheaDate (champ du kit, au lieu des
'   DateTimePicker Windows) ; logo facultatif du domaine (carte « Logo du domaine », étape 1) :
'   choisi ou retiré en saisie, écrit avec le domaine, compté dans les modifications non
'   enregistrées (demande de Joëlle).
' - V1.10.1 (20/10/2026) : PeutModifier refuse par défaut quand le contexte manque (audit B10).
' - V1.11.0 (21/10/2026, audit B.2) : nouvelle règle de quota — arrêt de la règle précédente et ajout
'   en une transaction (GestionQuotasInami.AjouterRegleEnChaine) ; plus de rétablissement à la main ;
'   cohérence de la saisie contrôlée avant le récapitulatif.
' - V1.12.0 (22/10/2026, audit C.1, étape 3) : écritures (domaines, logo, règles de quota,
'   rattachements, catégories) transmises avec le ContexteOperation, vérifiées par le métier.
' - V1.13.0 (23/10/2026, audit C.2 étape 3, ADR-51) : la saisie du domaine est jugée par
'   GestionDomaines.ValiderDomaine (règles sorties de ValiderSaisie, appliquées aussi à l'enregistrement) ;
'   toutes les erreurs d'un coup, sur leurs champs et dans une fenêtre récapitulative
'   (AffichageErreursSaisie) au lieu d'une fenêtre par erreur ; refus du métier affiché. Complétude
'   et règles de quota du domaine lues dans le métier (GestionDomaines.CalculerManques,
'   GestionQuotasInami.GetQuotasParDomaine).
' - V1.13.1 (25/10/2026, audit C.4, ADR-55) : la fenêtre principale est lue dans le contexte
'   (_context.FenetrePrincipale, IFenetrePrincipale) ; plus aucune référence à Home.
' - V1.13.2 (25/10/2026, audit D-1) : EnregistrerLogo rend False explicitement après un refus de
'   droit (avertissement du compilateur, comportement inchangé).
' - Aucun accès direct à la base : tout passe par les modules Gestion*.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_DomainesHome
    Implements IContextAwareUserControl, IUnsavedChangesAware

#Region "Variables privées"

    ' Photos de la fiche du domaine (onglet Général) et du formulaire de règle de quota, en saisie
    ' (créées au chargement, ADR-43).
    Private _gardeDomaine As GardeModifications
    Private _gardeQuota As GardeModifications

    ' Emplacements de Modifier et Désactiver dans la barre (relevés au chargement) : Désactiver prend
    ' la place de Modifier quand celui-ci est masqué, pour ne pas laisser de trou dans la barre.
    Private _gaucheModifier As Integer
    Private _gaucheDesactiver As Integer

    ' Logo de l'étape 1 en saisie : image choisie (déjà réduite) ou retrait demandé ; écrit avec le
    ' domaine (Enregistrer), oublié en revenant en consultation. _versionLogo change à chaque
    ' choix : la garde des modifications voit un nouveau logo (GardeModifications.AjouterValeur).
    Private _logoChoisi As Image
    Private _logoRetire As Boolean
    Private _versionLogo As Integer

    Private Enum ModeEcran
        Consultation
        Creation
        Modification
    End Enum

    Private _context As UserControlContext
    Private _mode As ModeEcran = ModeEcran.Consultation
    Private _domaines As New List(Of Domaine)
    Private WithEvents _pnlNatures As UC_LiaisonsNaturesDomaines
    Private WithEvents _pnlTarifs As UC_LiaisonsPrixDomaines
    Private _chargementEnCours As Boolean
    Private _quotas As New List(Of QuotaInamiRegle)
    Private _modeQuota As ModeQuota = ModeQuota.Consultation

    ' Ligne « période sans règle » de la grille des quotas : ocre sur fond jaune pâle, en gras.
    Private Shared ReadOnly CouleurTexteSansRegle As Color = AltheaDessin.Assombrir(UITheme.ColorOrangeMoyenFonce, 0.3F)
    Private Shared ReadOnly CouleurFondSansRegle As Color = AltheaDessin.Melanger(UITheme.ColorJauneModere, Color.White, 0.75F)
    Private Shared ReadOnly CouleurSelectionSansRegle As Color = AltheaDessin.Melanger(UITheme.ColorJauneModere, Color.White, 0.45F)
    Private ReadOnly _policeSansRegle As New Font("Calibri", 10.0F, FontStyle.Bold)

    ' Période qu'aucune règle active ne couvre, portée par sa ligne dans la grille des quotas (Tag).
    Private NotInheritable Class PeriodeSansRegle
        Public ReadOnly Property Periode As (Debut As Date, Fin As Date?)
        Public Sub New(periode As (Debut As Date, Fin As Date?))
            Me.Periode = periode
        End Sub
        Public Function Contient(jour As Date) As Boolean
            Return Periode.Debut <= jour AndAlso (Not Periode.Fin.HasValue OrElse Periode.Fin.Value >= jour)
        End Function
    End Class

#End Region

#Region "Contexte UI"

    Public Sub SetContext(context As UserControlContext) _
        Implements IContextAwareUserControl.SetContext

        _context = context

    End Sub

#End Region

#Region "Chargement"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : UC_DomainesHome_Load
    ' Rôle      : Initialise les panneaux liés, les boutons et charge la liste.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_DomainesHome_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If DesignMode Then Exit Sub

        _gardeDomaine = New GardeModifications(tabGeneral)
        _gardeDomaine.AjouterValeur("logo", AddressOf EtatLogo)
        _gardeQuota = New GardeModifications(pnlQuotaEdition)

        _gaucheModifier = btnModifier.Left
        _gaucheDesactiver = btnActiverDesactiver.Left

        UtilsButtons.InitStandardButton(btnNouveau)
        UtilsButtons.InitStandardButton(btnModifier)
        UtilsButtons.InitStandardButton(btnActiverDesactiver)
        UtilsButtons.InitStandardButton(btnEnregistrer)
        UtilsButtons.InitStandardButton(btnAnnuler)
        UtilsButtons.InitStandardButton(btnQuotaAjouter)
        UtilsButtons.InitStandardButton(btnQuotaCloturer)
        UtilsButtons.InitStandardButton(btnQuotaActiver)
        UtilsButtons.InitStandardButton(btnQuotaSupprimer)
        UtilsButtons.InitStandardButton(btnQuotaCategories)
        UtilsButtons.InitStandardButton(btnQuotaModifier)
        UtilsButtons.InitStandardButton(btnNouvelOrganisme)
        UtilsButtons.InitStandardButton(btnLogoImporter)
        UtilsButtons.InitStandardButton(btnLogoRetirer)

        UtilsDataGrid.InitDataGridBasique(dgvDomaines)
        UtilsDataGrid.InitDataGridSecondaire(dgvQuotas)

        ' Grille de saisie des plafonds : thème Althéa, mais la colonne Plafond reste éditable.
        UtilsDataGrid.InitDataGridSecondaire(dgvQuotaPlafonds)
        dgvQuotaPlafonds.ReadOnly = False
        colPlafondCategorie.ReadOnly = True
        colPlafondValeur.ReadOnly = False
        dgvQuotaPlafonds.SelectionMode = DataGridViewSelectionMode.CellSelect
        dgvQuotaPlafonds.EditMode = DataGridViewEditMode.EditOnEnter
        colPlafondValeur.DefaultCellStyle.BackColor = UITheme.ColorBeigeClair
        UtilsDataGrid.AlignerDroite(colQuotaAlerte, colPlafondValeur)

        ' Panneaux de rattachement partagés avec les référentiels (carte + barre locale, ADR-34) ;
        ' l'action de création, à droite de leur barre, ouvre le référentiel sans quitter l'écran.
        _pnlNatures = New UC_LiaisonsNaturesDomaines With {
            .Mode = UC_LiaisonsNaturesDomaines.SensLiaison.ParDomaine,
            .TexteCreation = "Créer une nouvelle nature…",
            .FournisseurContexte = Function() _context?.ContexteOperation
        }
        tabNatures.Controls.Add(_pnlNatures)

        _pnlTarifs = New UC_LiaisonsPrixDomaines With {
            .Mode = UC_LiaisonsPrixDomaines.SensLiaison.ParDomaine,
            .TexteCreation = "Créer un nouveau tarif…",
            .FournisseurContexte = Function() _context?.ContexteOperation
        }
        tabTarifs.Controls.Add(_pnlTarifs)

        dtpQuotaDebut.Valeur = Date.Today
        dtpQuotaFin.Valeur = Date.Today

        If _context IsNot Nothing Then
            _context.SetHeader("Domaines")
            _context.SetToolTip(chkTarifConvention,
                "Le tarif des séances provient d'un tarif rattaché au domaine (étape 2. Tarifs), " &
                "selon la date de la séance.")
            _context.SetToolTip(chkNoShowFacturable,
                "Une séance non honorée et non excusée reste facturée au patient.")
            _context.SetToolTip(chkSoumisQuota,
                "Les séances de ce domaine consomment un quota défini à l'étape 3. Catégories & quotas.")
            _context.SetToolTip(btnNouvelOrganisme,
                "Ouvre le référentiel des organismes sans quitter cet écran." & Environment.NewLine &
                "Le nouvel organisme sera ensuite proposé dans la liste.")
            _context.SetToolTip(btnLogoImporter,
                "Choisit une image (PNG, JPG…) ; elle est enregistrée avec le domaine (Enregistrer).")
            _context.SetToolTip(btnLogoRetirer,
                "Retire le logo du domaine ; effectif à l'enregistrement.")
            _context.SetToolTip(numQuotaAlerte,
                "Nombre total de séances à partir duquel un avertissement est affiché (0 = pas d'alerte).")
            _context.SetToolTip(btnQuotaCloturer,
                "Fixe le dernier jour d'application de la règle sélectionnée." & Environment.NewLine &
                "Pour faire évoluer un quota, « Nouvelle règle » suffit : la règle en cours s'arrête la veille.")
            _context.SetToolTip(btnQuotaModifier,
                "Corrige la règle sélectionnée (plafonds, période, dates)." & Environment.NewLine &
                "Seulement tant qu'aucune séance n'est concernée par cette règle.")
            _context.SetToolTip(btnQuotaAjouter,
                "Saisit une nouvelle règle (période glissante, alerte, plafond par catégorie)." & Environment.NewLine &
                "La règle en vigueur s'arrête automatiquement la veille de son début.")
            _context.SetToolTip(btnQuotaSupprimer,
                "Réservé aux erreurs de saisie, et seulement si la règle n'a pas encore servi au décompte de séances." & Environment.NewLine &
                "La règle précédente qu'elle avait arrêtée est rouverte.")
        End If

        PasserEnMode(ModeEcran.Consultation)
        ChargerListe()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerListe
    ' Rôle      : Recharge les domaines et réapplique le filtre, en conservant la sélection.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerListe(Optional idASelectionner As ULong = 0)

        Try
            _domaines = GestionDomaines.GetDomaines(chkAfficherInactifs.Checked)
            AppliquerFiltre(idASelectionner)
        Catch ex As Exception
            GestionLog.EcrireLog("UC_DomainesHome : erreur chargement domaines.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            DialogChoix.Erreur("Erreur lors du chargement des domaines.", "Erreur")
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerFiltre
    ' Rôle      : Remplit la grille selon la recherche (code + libellé).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerFiltre(Optional idASelectionner As ULong = 0)

        If idASelectionner = 0 Then idASelectionner = IdSelectionne()

        Dim filtre As String = txtRecherche.Text.Trim()

        _chargementEnCours = True
        dgvDomaines.Rows.Clear()

        For Each d In _domaines.Where(
            Function(x) filtre = String.Empty OrElse
                        x.CodeDomaine.Contains(filtre, StringComparison.OrdinalIgnoreCase) OrElse
                        x.LibelleDomaine.Contains(filtre, StringComparison.OrdinalIgnoreCase))

            Dim index As Integer = dgvDomaines.Rows.Add(d.IdDomaine, d.CodeDomaine, d.LibelleDomaine, String.Empty)
            AfficherEtatLigne(dgvDomaines.Rows(index), d)
        Next

        _chargementEnCours = False

        dgvDomaines.ClearSelection()
        For Each row As DataGridViewRow In dgvDomaines.Rows
            If CULng(row.Cells(colId.Index).Value) = idASelectionner Then
                row.Selected = True
                dgvDomaines.CurrentCell = row.Cells(colCode.Index)
                Exit For
            End If
        Next

        If dgvDomaines.SelectedRows.Count = 0 AndAlso dgvDomaines.Rows.Count > 0 Then
            dgvDomaines.Rows(0).Selected = True
            dgvDomaines.CurrentCell = dgvDomaines.Rows(0).Cells(colCode.Index)
        End If

        AfficherSelection()

    End Sub

#End Region

#Region "Sélection et affichage"

    ' État de complétude d'un domaine dans la liste (Complet / À compléter / Inactif). Le ton de la
    ' pastille est posé dans le Tag de la cellule, lu par dgvDomaines_CellPainting (V1.7.0).
    Private Sub AfficherEtatLigne(row As DataGridViewRow, d As Domaine)

        Dim manques As Integer = GestionDomaines.CalculerManques(d).Count
        Dim cellule As DataGridViewCell = row.Cells(colEtat.Index)
        cellule.Value = If(Not d.Actif, "Inactif", If(manques = 0, "Complet", "À compléter"))
        cellule.Tag = If(Not d.Actif, TonEtat.Inactif, If(manques = 0, TonEtat.Ok, TonEtat.ASurveiller))

        row.DefaultCellStyle.ForeColor = If(d.Actif, Color.Empty, UITheme.ColorGrisMoyenFonce)
        row.DefaultCellStyle.SelectionForeColor = If(d.Actif, Color.Empty, UITheme.ColorGrisDiscret)

    End Sub

    Private Sub dgvDomaines_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvDomaines.CellPainting
        If e.RowIndex < 0 OrElse e.ColumnIndex <> colEtat.Index Then Return
        Dim cellule As DataGridViewCell = dgvDomaines.Rows(e.RowIndex).Cells(e.ColumnIndex)
        If Not TypeOf cellule.Tag Is TonEtat Then Return
        UtilsDataGrid.DessinerPastilleCellule(dgvDomaines, e, CStr(cellule.Value), UtilsPastilles.Teinte(CType(cellule.Tag, TonEtat)))
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RafraichirApresRattachement
    ' Rôle      : Un tarif ou une nature a été rattaché, retiré ou recatégorisé : l'état du domaine
    '             (étapes, pastille, liste, synthèse) est recalculé sans recharger les panneaux.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RafraichirApresRattachement(sender As Object, e As EventArgs) _
        Handles _pnlNatures.RattachementsModifies, _pnlTarifs.RattachementsModifies

        Dim dom As Domaine = DomaineSelectionne()
        If dom Is Nothing Then Return

        AfficherSynthese(dom)
        ActualiserAssistant(dom)

        If dgvDomaines.SelectedRows.Count > 0 Then AfficherEtatLigne(dgvDomaines.SelectedRows(0), dom)

    End Sub

    Private Function IdSelectionne() As ULong

        If dgvDomaines.SelectedRows.Count = 0 Then Return 0UL
        Return CULng(dgvDomaines.SelectedRows(0).Cells(colId.Index).Value)

    End Function

    Private Function DomaineSelectionne() As Domaine

        Dim id As ULong = IdSelectionne()
        Return _domaines.FirstOrDefault(Function(d) d.IdDomaine = id)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherSelection
    ' Rôle      : Affiche le domaine sélectionné dans tous les onglets.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherSelection()

        If _chargementEnCours OrElse _mode <> ModeEcran.Consultation Then Return

        Dim dom As Domaine = DomaineSelectionne()

        If dom Is Nothing Then
            ViderGeneral()
        Else
            txtCode.Text = dom.CodeDomaine
            txtLibelle.Text = dom.LibelleDomaine
            numOrdre.Value = Math.Max(numOrdre.Minimum, Math.Min(numOrdre.Maximum, dom.OrdreAffichage))
            chkActif.Checked = dom.Actif
            chkTarifConvention.Checked = dom.TarifConvention
            chkNoShowFacturable.Checked = dom.NoShowFacturable
            chkSoumisQuota.Checked = dom.SoumisQuota
            ChargerOrganismes(dom.IdOrganisme)
            AfficherLogo(dom)
        End If

        Dim id As ULong = If(dom Is Nothing, 0UL, dom.IdDomaine)
        _pnlNatures?.ChargerPour(id)
        _pnlTarifs?.ChargerPour(id)
        ChargerQuotas(dom)
        AfficherSynthese(dom)
        ActualiserBoutons()
        AfficherDomaineCourant()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherDomaineCourant
    ' Rôle      : Rappelle en bandeau le domaine sur lequel on travaille, et l'action en cours dans
    '             l'en-tête et la barre d'état de l'application.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherDomaineCourant()

        Dim dom As Domaine = DomaineSelectionne()
        Dim onglet As String = etapes.LibelleEtape(tabDomaine.SelectedIndex)
        Dim texte As String
        Dim statut As String

        Select Case _mode
            Case ModeEcran.Creation
                texte = "Nouveau domaine (en cours de création)"
                statut = "Création d'un domaine : complétez l'identité puis cliquez sur Enregistrer."
            Case ModeEcran.Modification
                texte = If(dom Is Nothing, "Domaine", $"Modification : {dom.CodeDomaine} — {dom.LibelleDomaine}")
                statut = If(dom Is Nothing, String.Empty, $"Modification du domaine {dom.LibelleDomaine} : Enregistrer ou Annuler pour terminer.")
            Case Else
                If dom Is Nothing Then
                    texte = "Aucun domaine sélectionné"
                    statut = "Sélectionnez un domaine dans la liste ou cliquez sur Nouveau."
                Else
                    texte = $"Domaine : {dom.CodeDomaine} — {dom.LibelleDomaine}" & If(dom.Actif, String.Empty, " (inactif)")
                    statut = $"Domaine {dom.LibelleDomaine} — étape {onglet}."
                End If
        End Select

        lblDomaineCourant.Text = texte
        ActualiserAssistant(dom)

        If _context IsNot Nothing Then
            _context.SetHeader(If(dom Is Nothing OrElse _mode = ModeEcran.Creation, "Domaines", $"Domaines › {dom.LibelleDomaine} › {onglet}"))
            If statut <> String.Empty Then _context.SetStatus(statut)
        End If

    End Sub

    ' Ouvre la gestion des catégories du domaine sélectionné, puis rafraîchit les plafonds.
    Private Sub btnQuotaCategories_Click(sender As Object, e As EventArgs) Handles btnQuotaCategories.Click

        Dim dom As Domaine = DomaineSelectionne()
        If dom Is Nothing Then Return

        Using dlg As New CategoriesQuotaEditor(dom.IdDomaine, dom.LibelleDomaine, _context?.ContexteOperation)
            dlg.ShowDialog(FindForm())
            If dlg.AModifie Then
                RafraichirApresQuota()
                _pnlNatures?.ChargerPour(dom.IdDomaine)
                _context?.SetStatus($"Catégories de quota du domaine {dom.LibelleDomaine} mises à jour.")
            End If
        End Using

    End Sub

    Private Sub ViderGeneral()

        txtCode.Clear()
        txtLibelle.Clear()
        numOrdre.Value = 0
        chkActif.Checked = True
        chkTarifConvention.Checked = False
        chkNoShowFacturable.Checked = True
        chkSoumisQuota.Checked = False
        ChargerOrganismes(Nothing)
        AfficherLogo(Nothing)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerOrganismes
    ' Rôle      : Alimente la liste des organismes de remboursement (actifs + celui déjà rattaché)
    '             et sélectionne l'organisme courant. La première entrée « Aucun organisme »
    '             correspond à NULL. Les mutualités (organismes des patients) ne sont pas proposées.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerOrganismes(idCourant As ULong?)

        Dim items As New List(Of KeyValuePair(Of ULong, String)) From {
            New KeyValuePair(Of ULong, String)(0UL, "(Aucun organisme)")
        }

        Try
            For Each o In GestionOrganismes.GetOrganismes(True).
                Where(Function(x) (x.Actif AndAlso x.TypeOrganisme = TypeOrganisme.Remboursement) OrElse
                                  (idCourant.HasValue AndAlso x.IdOrganisme = idCourant.Value))
                items.Add(New KeyValuePair(Of ULong, String)(o.IdOrganisme,
                    If(o.Actif, o.LibelleOrganisme, o.LibelleOrganisme & " (inactif)")))
            Next
        Catch ex As Exception
            GestionLog.EcrireLog("UC_DomainesHome : erreur chargement organismes.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
        End Try

        cboOrganisme.DisplayMember = "Value"
        cboOrganisme.ValueMember = "Key"
        cboOrganisme.DataSource = items
        cboOrganisme.SelectedValue = If(idCourant, 0UL)
        If cboOrganisme.SelectedIndex < 0 Then cboOrganisme.SelectedIndex = 0

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : OrganismeSaisi
    ' Rôle     : Retourne l'organisme choisi, ou Nothing si « Aucun organisme ».
    ' -------------------------------------------------------------------------------------------------
    Private Function OrganismeSaisi() As ULong?

        If cboOrganisme.SelectedValue Is Nothing Then Return Nothing
        Dim id As ULong = CULng(cboOrganisme.SelectedValue)
        Return If(id = 0UL, CType(Nothing, ULong?), id)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerQuotas
    ' Version   : V2.0.0
    ' Date      : 14/10/2026
    '
    ' Rôle      :
    ' Étape 3 : catégories du domaine (pastilles), règles de quota et détail de la règle choisie.
    ' Un domaine non soumis à quota n'affiche qu'une carte « étape non nécessaire ».
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerQuotas(dom As Domaine)

        _modeQuota = ModeQuota.Consultation
        dgvQuotas.Rows.Clear()
        _quotas = New List(Of QuotaInamiRegle)

        Dim soumis As Boolean = dom IsNot Nothing AndAlso dom.SoumisQuota
        carteCategories.Visible = soumis
        pnlEspaceQuotas.Visible = soumis
        carteRegles.Visible = soumis
        carteQuotaNonNecessaire.Visible = Not soumis

        If dom Is Nothing Then
            lblQuotaNonNecessaire.Text = "Sélectionnez un domaine."
            lblAideQuotas.Text = String.Empty
            ActualiserBoutonsQuota()
            Return
        End If

        If Not soumis Then
            lblQuotaNonNecessaire.Text = "Étape non nécessaire : ce domaine n'est pas soumis à quota (voir l'étape 1. Général)."
            lblAideQuotas.Text = String.Empty
            ActualiserBoutonsQuota()
            Return
        End If

        AfficherCategoriesQuota(dom.IdDomaine)

        _quotas = GestionQuotasInami.GetQuotasParDomaine(dom.IdDomaine)
        AfficherLignesQuotas()

        ' La règle en vigueur est sélectionnée d'office (son détail s'affiche sous la grille) ; à défaut,
        ' la période sans règle qui contient aujourd'hui.
        dgvQuotas.ClearSelection()
        For Each row As DataGridViewRow In dgvQuotas.Rows
            Dim r As QuotaInamiRegle = TryCast(row.Tag, QuotaInamiRegle)
            Dim periode As PeriodeSansRegle = TryCast(row.Tag, PeriodeSansRegle)
            If (r IsNot Nothing AndAlso r.EstEnVigueur) OrElse (periode IsNot Nothing AndAlso periode.Contient(Date.Today)) Then
                row.Selected = True
                Exit For
            End If
        Next

        AfficherZoneQuota()
        ActualiserBoutonsQuota()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherLignesQuotas
    ' Rôle      : Remplit la grille des règles, la plus récente en haut. Chaque période qu'aucune règle
    '             active ne couvre y figure aussi, à sa place, sur une ligne d'alerte : décision du
    '             14/10/2026, un trou ne doit jamais passer inaperçu et n'est jamais comblé
    '             automatiquement (déplacer une date de fin en douce peut fausser les périodes).
    '             Chaque ligne porte sa règle ou sa période dans Tag.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherLignesQuotas()

        dgvQuotas.Rows.Clear()

        Dim lignes As New List(Of (DateTri As Date, Contenu As Object))
        For Each r In _quotas
            lignes.Add((r.DateDebutValidite.Date, CObj(r)))
        Next
        For Each trou In GestionQuotasInami.PeriodesSansRegle(_quotas)
            lignes.Add((trou.Debut, CObj(New PeriodeSansRegle(trou))))
        Next

        For Each ligne In lignes.OrderByDescending(Function(l) l.DateTri)

            Dim regle As QuotaInamiRegle = TryCast(ligne.Contenu, QuotaInamiRegle)
            Dim index As Integer

            If regle IsNot Nothing Then
                index = dgvQuotas.Rows.Add(regle.LibellePeriodeValidite, regle.LibellePlafonds, regle.SeuilAlerteTotal, regle.LibelleEtat)
                If Not regle.Actif Then
                    dgvQuotas.Rows(index).DefaultCellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
                    dgvQuotas.Rows(index).DefaultCellStyle.SelectionForeColor = UITheme.ColorGrisDiscret
                End If
            Else
                Dim periode As PeriodeSansRegle = DirectCast(ligne.Contenu, PeriodeSansRegle)
                index = dgvQuotas.Rows.Add("Aucune règle " & LibellePeriode(periode.Periode),
                                           "Séances non soumises au quota", String.Empty, "Sans règle")
                With dgvQuotas.Rows(index).DefaultCellStyle
                    .BackColor = CouleurFondSansRegle
                    .ForeColor = CouleurTexteSansRegle
                    .SelectionBackColor = CouleurSelectionSansRegle
                    .SelectionForeColor = CouleurTexteSansRegle
                    .Font = _policeSansRegle
                End With
            End If

            dgvQuotas.Rows(index).Tag = ligne.Contenu

        Next

    End Sub

    ' Période sans règle sélectionnée dans la grille (Nothing si la ligne est une règle).
    Private Function PeriodeSelectionnee() As PeriodeSansRegle
        If dgvQuotas.SelectedRows.Count = 0 Then Return Nothing
        Return TryCast(dgvQuotas.SelectedRows(0).Tag, PeriodeSansRegle)
    End Function

    ' Période qu'une nouvelle règle doit couvrir : celle sélectionnée, sinon celle qui contient aujourd'hui.
    Private Function TrouAProposer() As (Debut As Date, Fin As Date?)?
        Dim periode As PeriodeSansRegle = PeriodeSelectionnee()
        If periode IsNot Nothing Then Return periode.Periode
        Return TrouDuJour()
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : AlerteTrousCrees
    ' Rôle     : Texte de l'encadré d'attention si une action laisserait des périodes sans règle qui
    '            n'existent pas aujourd'hui (vide sinon). Les règles simulées ne servent qu'à ce calcul.
    '            « apres » commence la phrase (« Après la clôture », « Avec cette règle »…).
    ' -------------------------------------------------------------------------------------------------
    Private Function AlerteTrousCrees(simulees As IEnumerable(Of QuotaInamiRegle), apres As String) As String

        Dim actuels = GestionQuotasInami.PeriodesSansRegle(_quotas)
        Dim nouveaux = GestionQuotasInami.PeriodesSansRegle(simulees).Where(Function(t) Not actuels.Contains(t)).ToList()
        If nouveaux.Count = 0 Then Return String.Empty

        Return $"{apres}, aucune règle ne couvrira " & String.Join(", ", nouveaux.Select(Function(t) LibellePeriode(t))) &
               ". Les séances de cette période ne seront soumises à aucun quota ; pour les y soumettre, " &
               "créez la règle qui manque (« Nouvelle règle »)." 

    End Function

    ' Copie limitée aux dates et à l'état, pour simuler l'effet d'une action sur les périodes.
    Private Shared Function CopiePourSimulation(r As QuotaInamiRegle, fin As Date?) As QuotaInamiRegle
        Return New QuotaInamiRegle With {.IdQuotaInami = r.IdQuotaInami, .IdDomaine = r.IdDomaine,
                                         .DateDebutValidite = r.DateDebutValidite, .DateFinValidite = fin, .Actif = r.Actif}
    End Function

    ' Période sans règle qui contient aujourd'hui (Nothing s'il n'y en a pas).
    Private Function TrouDuJour() As (Debut As Date, Fin As Date?)?

        For Each trou In GestionQuotasInami.PeriodesSansRegle(_quotas)
            If trou.Debut <= Date.Today AndAlso (Not trou.Fin.HasValue OrElse trou.Fin.Value >= Date.Today) Then Return trou
        Next

        Return Nothing

    End Function

    Private Shared Function LibellePeriode(trou As (Debut As Date, Fin As Date?)) As String
        Return If(trou.Fin.HasValue, $"du {trou.Debut:dd/MM/yyyy} au {trou.Fin.Value:dd/MM/yyyy}", $"depuis le {trou.Debut:dd/MM/yyyy}")
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherCategoriesQuota
    ' Rôle      : Une pastille par catégorie du domaine (les inactives sont marquées).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherCategoriesQuota(idDomaine As ULong)

        flpCategories.SuspendLayout()

        For Each ctrl As Control In flpCategories.Controls.Cast(Of Control)().ToList()
            ctrl.Dispose()
        Next
        flpCategories.Controls.Clear()

        Dim categories As List(Of CategorieQuota) = GestionCategoriesQuota.GetCategoriesParDomaine(idDomaine, True)

        If categories.Count = 0 Then
            flpCategories.Controls.Add(New Label With {
                .AutoSize = True,
                .BackColor = Color.Transparent,
                .ForeColor = AltheaDessin.Assombrir(UITheme.ColorOrangeMoyenFonce, 0.3F),
                .Margin = New Padding(0, 2, 0, 0),
                .Text = "Aucune catégorie : créez-les avec « Gérer les catégories… » (ex. Soutien, Traitement)."})
        Else
            For Each c As CategorieQuota In categories
                ' Catégorie active : étiquette neutre ; inactive : pastille du ton « inactif » (V1.7.0).
                Dim inactif As UtilsPastilles.TeintePastille = UtilsPastilles.Teinte(TonEtat.Inactif)
                flpCategories.Controls.Add(New AltheaPastille With {
                    .Text = If(c.Actif, c.LibelleCategorie, c.LibelleCategorie & " (inactive)"),
                    .CouleurFond = If(c.Actif, Color.White, inactif.Fond),
                    .CouleurBordure = If(c.Actif, UITheme.ColorBeigeFonce, inactif.Texte),
                    .ForeColor = If(c.Actif, UITheme.ColorGrisFonce, inactif.Texte),
                    .Margin = New Padding(0, 0, 6, 0)})
            Next
        End If

        flpCategories.ResumeLayout(True)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherZoneQuota
    ' Version   : V1.0.0
    ' Date      : 14/10/2026
    '
    ' Rôle      :
    ' Zone sous la grille, selon le mode : détail de la règle sélectionnée (lecture seule),
    ' saisie d'une nouvelle règle, ou dernier jour d'une clôture.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherZoneQuota()

        Dim regle As QuotaInamiRegle = QuotaSelectionne()
        Dim saisie As Boolean = _modeQuota <> ModeQuota.Consultation

        ' Couleur normale de la note ; la consultation la passe en ocre s'il existe une période sans règle.
        lblAideQuotas.ForeColor = UITheme.ColorSaugeTresFonce

        btnQuotaValider.Visible = saisie
        btnQuotaAnnuler.Visible = saisie
        dgvQuotas.Enabled = Not saisie

        Select Case _modeQuota

            Case ModeQuota.NouvelleRegle
                lblQuotaZone.Text = "Nouvelle règle (à compléter)"
                btnQuotaValider.Text = "Ajouter la règle"
                ' Modèle : la règle en vigueur, sinon la plus récente.
                Dim modele As QuotaInamiRegle = If(_quotas.FirstOrDefault(Function(r) r.EstEnVigueur),
                                                   _quotas.Where(Function(r) r.Actif).OrderByDescending(Function(r) r.DateDebutValidite).FirstOrDefault())
                ChargerGrillePlafonds(DomaineSelectionne().IdDomaine, modele)
                numQuotaDuree.Value = BornerNombre(numQuotaDuree, If(modele Is Nothing, 12, modele.DureePeriodeMois))
                numQuotaAlerte.Value = BornerNombre(numQuotaAlerte, If(modele Is Nothing, 0, modele.SeuilAlerteTotal))

                ' Période sans règle sélectionnée, ou aujourd'hui sans règle : les dates proposées la comblent.
                Dim trou = TrouAProposer()
                If trou.HasValue Then
                    dtpQuotaDebut.Valeur = trou.Value.Debut
                    chkQuotaFin.Checked = trou.Value.Fin.HasValue
                    dtpQuotaFin.Valeur = If(trou.Value.Fin, Date.Today)
                    lblAideQuotas.Text = $"Dates proposées pour couvrir la période sans règle ({LibellePeriode(trou.Value)}) ; modifiez-les si besoin. " &
                                         "Puis plafonds, période glissante et alerte, et « Ajouter la règle »."
                Else
                    dtpQuotaDebut.Valeur = Date.Today
                    chkQuotaFin.Checked = False
                    dtpQuotaFin.Valeur = Date.Today
                    lblAideQuotas.Text = "1. Plafond de chaque catégorie (colonne Plafond).  2. Période glissante et alerte.  " &
                                         "3. Date de début (passée si besoin).  4. « Ajouter la règle »."
                End If

                UtilsControls.DefinirLectureSeule(pnlQuotaEdition, False)
                dgvQuotaPlafonds.ReadOnly = False
                colPlafondCategorie.ReadOnly = True
                colPlafondValeur.DefaultCellStyle.BackColor = Color.White
                dtpQuotaFin.Enabled = chkQuotaFin.Checked

            Case ModeQuota.Modification
                lblQuotaZone.Text = "Correction de la règle (pas encore utilisée)"
                btnQuotaValider.Text = "Enregistrer la correction"
                lblAideQuotas.Text = "Corrigez plafonds, période glissante, alerte ou dates, puis « Enregistrer la correction ». " &
                                     "Possible tant qu'aucune séance n'est concernée par cette règle."
                ChargerGrillePlafonds(DomaineSelectionne().IdDomaine, regle)
                numQuotaDuree.Value = BornerNombre(numQuotaDuree, regle.DureePeriodeMois)
                numQuotaAlerte.Value = BornerNombre(numQuotaAlerte, regle.SeuilAlerteTotal)
                dtpQuotaDebut.Valeur = regle.DateDebutValidite.Date
                chkQuotaFin.Checked = regle.DateFinValidite.HasValue
                dtpQuotaFin.Valeur = If(regle.DateFinValidite, regle.DateDebutValidite).Date
                UtilsControls.DefinirLectureSeule(pnlQuotaEdition, False)
                dgvQuotaPlafonds.ReadOnly = False
                colPlafondCategorie.ReadOnly = True
                colPlafondValeur.DefaultCellStyle.BackColor = Color.White
                dtpQuotaFin.Enabled = chkQuotaFin.Checked

            Case ModeQuota.Cloture
                lblQuotaZone.Text = "Clôture de la règle sélectionnée"
                lblAideQuotas.Text = $"Indiquez le dernier jour d'application de la règle ({regle?.LibellePeriodeValidite}), puis « Clôturer »."
                btnQuotaValider.Text = "Clôturer"
                AfficherRegle(regle)
                UtilsControls.DefinirLectureSeule(pnlQuotaEdition, True)
                ' Seule la date de fin est saisissable ; les boutons de la zone restent actifs.
                chkQuotaFin.Checked = True
                dtpQuotaFin.Valeur = If(regle IsNot Nothing AndAlso regle.DateDebutValidite.Date > Date.Today,
                                       regle.DateDebutValidite.Date, Date.Today)
                dtpQuotaFin.Enabled = True
                btnQuotaValider.Enabled = True
                btnQuotaAnnuler.Enabled = True

            Case Else
                lblQuotaZone.Text = If(PeriodeSelectionnee() IsNot Nothing, "Période sans règle : à couvrir par « Nouvelle règle »",
                                    If(regle Is Nothing,
                                       If(TrouDuJour().HasValue, "Aucune règle en vigueur aujourd'hui", "Sélectionnez une règle pour voir son détail."),
                                       "Règle sélectionnée"))
                AfficherAideQuotasConsultation()
                AfficherRegle(regle)
                UtilsControls.DefinirLectureSeule(pnlQuotaEdition, True)
                dgvQuotaPlafonds.ReadOnly = True
                colPlafondValeur.DefaultCellStyle.BackColor = UITheme.ColorChampLectureSeule

        End Select

        lblQuotaZone.Width = TextRenderer.MeasureText(lblQuotaZone.Text, lblQuotaZone.Font).Width + 6
        AjusterLargeurBouton(btnQuotaValider)
        ActualiserTotalPlafonds()

    End Sub

    ' Détail d'une règle existante (plafonds compris) ; Nothing = zone vide.
    Private Sub AfficherRegle(regle As QuotaInamiRegle)

        dgvQuotaPlafonds.Rows.Clear()
        If regle Is Nothing Then Return

        numQuotaDuree.Value = BornerNombre(numQuotaDuree, regle.DureePeriodeMois)
        numQuotaAlerte.Value = BornerNombre(numQuotaAlerte, regle.SeuilAlerteTotal)
        dtpQuotaDebut.Valeur = regle.DateDebutValidite.Date
        chkQuotaFin.Checked = regle.DateFinValidite.HasValue
        dtpQuotaFin.Valeur = If(regle.DateFinValidite, regle.DateDebutValidite).Date

        For Each p As QuotaPlafond In regle.Plafonds
            dgvQuotaPlafonds.Rows.Add(p.LibelleCategorie, p.Plafond)
        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherAideQuotasConsultation
    ' Rôle      : Note sous la carte en consultation. Une période sans règle passe en tête, en ocre :
    '             les séances qui y tombent ne sont soumises à aucun quota.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherAideQuotasConsultation()

        Dim trous = GestionQuotasInami.PeriodesSansRegle(_quotas)

        If trous.Count > 0 Then
            lblAideQuotas.ForeColor = AltheaDessin.Assombrir(UITheme.ColorOrangeMoyenFonce, 0.3F)
            lblAideQuotas.Text = "Période sans règle : " & String.Join(", ", trous.Select(Function(t) LibellePeriode(t))) &
                                 ". Les séances de cette période ne sont soumises à aucun quota : sélectionnez sa ligne (en ocre), puis « Nouvelle règle »."
        Else
            lblAideQuotas.ForeColor = UITheme.ColorSaugeTresFonce
            lblAideQuotas.Text = "Une règle pas encore utilisée se corrige avec « Modifier la règle ». Une règle qui a servi ne se modifie plus : " &
                                 "« Nouvelle règle » pour changer un plafond à partir d'une date (la règle en cours s'arrête la veille), « Clôturer la règle » pour l'arrêter."
        End If

    End Sub

    Private Shared Function BornerNombre(nombre As NumericUpDown, valeur As Integer) As Decimal
        Return Math.Max(nombre.Minimum, Math.Min(nombre.Maximum, CDec(valeur)))
    End Function

    ' Total calculé des plafonds affichés (jamais stocké).
    Private Sub ActualiserTotalPlafonds()

        Dim total As Integer = 0
        For Each row As DataGridViewRow In dgvQuotaPlafonds.Rows
            Dim valeur As Integer
            If Integer.TryParse(Convert.ToString(row.Cells(colPlafondValeur.Index).Value), valeur) Then total += valeur
        Next

        lblQuotaTotal.Text = $"Total (calculé) : {total}"

    End Sub

    Private Sub dgvQuotaPlafonds_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvQuotaPlafonds.CellValueChanged
        ActualiserTotalPlafonds()
    End Sub

    ' Largeur au plus juste d'un bouton Outil dont le texte change.
    Private Shared Sub AjusterLargeurBouton(btn As AltheaBouton)
        Dim icone As Integer = If(btn.Image Is Nothing, 0, 26)
        btn.Width = TextRenderer.MeasureText(btn.Text, btn.Font).Width + icone + 26
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerGrillePlafonds
    ' Rôle      : Nouvelle règle : une ligne de plafond par catégorie active du domaine ; la valeur
    '             initiale reprend la règle en vigueur pour faciliter la saisie d'une nouvelle version.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerGrillePlafonds(idDomaine As ULong, enVigueur As QuotaInamiRegle)

        dgvQuotaPlafonds.Rows.Clear()

        For Each c As CategorieQuota In GestionCategoriesQuota.GetCategoriesParDomaine(idDomaine, False).Where(Function(x) x.Actif)
            Dim valeur As Integer = If(enVigueur IsNot Nothing, enVigueur.PlafondDe(c.IdCategorieQuota), 0)
            Dim index As Integer = dgvQuotaPlafonds.Rows.Add(c.LibelleCategorie, valeur)
            dgvQuotaPlafonds.Rows(index).Tag = c
        Next

    End Sub

    ' Plafonds saisis dans la grille ; Nothing si une valeur n'est pas un entier positif.
    Private Function PlafondsSaisis() As List(Of QuotaPlafond)

        Dim liste As New List(Of QuotaPlafond)

        For Each row As DataGridViewRow In dgvQuotaPlafonds.Rows
            Dim c As CategorieQuota = TryCast(row.Tag, CategorieQuota)
            If c Is Nothing Then Continue For

            Dim valeur As Integer
            If Not Integer.TryParse(Convert.ToString(row.Cells(colPlafondValeur.Index).Value), valeur) OrElse valeur < 0 Then
                Return Nothing
            End If

            liste.Add(New QuotaPlafond With {
                .IdCategorieQuota = c.IdCategorieQuota,
                .CodeCategorie = c.CodeCategorie,
                .LibelleCategorie = c.LibelleCategorie,
                .Plafond = valeur
            })
        Next

        Return liste

    End Function

    Private Function QuotaSelectionne() As QuotaInamiRegle

        If dgvQuotas.SelectedRows.Count = 0 Then Return Nothing
        Return TryCast(dgvQuotas.SelectedRows(0).Tag, QuotaInamiRegle)

    End Function

    Private Sub ActualiserBoutonsQuota()

        Dim dom As Domaine = DomaineSelectionne()
        Dim ecriture As Boolean = _mode = ModeEcran.Consultation AndAlso PeutModifier() AndAlso
                                  dom IsNot Nothing AndAlso dom.SoumisQuota AndAlso
                                  _modeQuota = ModeQuota.Consultation
        Dim regle As QuotaInamiRegle = QuotaSelectionne()

        btnQuotaAjouter.Enabled = ecriture
        btnQuotaModifier.Enabled = ecriture AndAlso regle IsNot Nothing AndAlso regle.Actif
        btnQuotaCloturer.Enabled = ecriture AndAlso regle IsNot Nothing AndAlso regle.Actif AndAlso
                                   Not regle.DateFinValidite.HasValue
        btnQuotaActiver.Enabled = ecriture AndAlso regle IsNot Nothing
        btnQuotaSupprimer.Enabled = ecriture AndAlso regle IsNot Nothing
        btnQuotaActiver.Text = If(regle IsNot Nothing AndAlso Not regle.Actif, "Réactiver", "Désactiver")

    End Sub

#End Region

#Region "Synthèse / complétude"

    Private Sub AfficherSynthese(dom As Domaine)
        ucSynthese.AfficherDomaine(dom, inclureCompletude:=True)
    End Sub

#End Region

#Region "Logo du domaine"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherLogo
    ' Version   : V1.0.0
    ' Date      : 20/10/2026
    '
    ' Rôle      : Montre le logo enregistré du domaine (ou rien) dans la carte « Logo du domaine ».
    ' Remarques : Copie propre à l'écran (libérée au remplacement) : pas la miniature partagée de
    '             LogosDomaines. Un logo illisible est journalisé et laisse la carte vide.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherLogo(dom As Domaine)

        Dim logo As Image = Nothing

        If dom IsNot Nothing Then
            Try
                logo = CheminsDomaineHelper.ChargerLogo(dom.IdDomaine, dom.LogoFichier)
            Catch ex As Exception
                GestionLog.EcrireException($"UC_DomainesHome : logo du domaine {dom.IdDomaine} illisible.", ex,
                                           GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)
            End Try
        End If

        UtilsImages.RemplacerImage(picLogo, logo)

    End Sub

    ' État du logo en saisie, pour la garde des modifications.
    Private Function EtatLogo() As String
        If _logoChoisi IsNot Nothing Then Return $"choisi:{_versionLogo}"
        Return If(_logoRetire, "retire", "inchange")
    End Function

    ' Retour en consultation (enregistré ou abandonné) : rien n'est plus en attente.
    Private Sub OublierLogoChoisi()
        _logoChoisi?.Dispose()
        _logoChoisi = Nothing
        _logoRetire = False
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnLogoImporter_Click
    ' Version   : V1.0.1
    ' Date      : 20/10/2026
    '
    ' Rôle      : Choisit une image (dialogue commun, fichier contrôlé), la réduit et la montre ; elle
    '             sera écrite avec le domaine.
    ' Remarques :
    ' - Pas de confirmation de remplacement (UtilsImages.ConfirmerRemplacement) : rien n'est
    '   définitif avant Enregistrer, Annuler rend l'ancien logo. Même règle que la fiche Cabinet.
    ' - V1.0.1 (20/10/2026) : remarque mise à jour (le Cabinet diffère aussi ses images).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnLogoImporter_Click(sender As Object, e As EventArgs) Handles btnLogoImporter.Click

        Dim source As String = UtilsImages.ChoisirFichierImage(Me, "Logo du domaine")
        If source Is Nothing Then Return

        Try
            Using image As Image = ImagesHelper.ChargerSansVerrou(source)
                If image Is Nothing Then
                    DialogChoix.Erreur("Cette image n'a pas pu être lue.", "Logo du domaine")
                    Return
                End If

                _logoChoisi?.Dispose()
                _logoChoisi = ImagesHelper.Reduire(image, CheminsDomaineHelper.TailleMaxLogo)
            End Using

            _logoRetire = False
            _versionLogo += 1
            UtilsImages.RemplacerImage(picLogo, New Bitmap(_logoChoisi))
            ActualiserBoutons()
            _context?.SetStatus("Logo choisi : il sera enregistré avec le domaine.")

        Catch ex As Exception
            GestionLog.EcrireException("UC_DomainesHome : import du logo impossible.", ex,
                                       GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)
            DialogChoix.Erreur("Cette image n'a pas pu être préparée. Le détail est dans le journal.", "Logo du domaine")
        End Try

    End Sub

    ' Retire le logo affiché : un logo choisi mais pas enregistré est simplement oublié ; un logo
    ' enregistré sera retiré à l'enregistrement.
    Private Sub btnLogoRetirer_Click(sender As Object, e As EventArgs) Handles btnLogoRetirer.Click

        Dim dom As Domaine = If(_mode = ModeEcran.Modification, DomaineSelectionne(), Nothing)

        _logoChoisi?.Dispose()
        _logoChoisi = Nothing
        _logoRetire = dom IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(dom.LogoFichier)
        _versionLogo += 1
        UtilsImages.RemplacerImage(picLogo, Nothing)
        ActualiserBoutons()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : EnregistrerLogo
    ' Version   : V1.0.1
    ' Date      : 25/10/2026
    '
    ' Rôle      : Écrit le logo choisi (fichier puis nom en base) ou applique le retrait (base puis
    '             fichier) ; False en cas d'échec (journalisé), le domaine restant enregistré.
    ' Remarques : Dans cet ordre, la base ne cite jamais un fichier absent ; un fichier resté après
    '             un retrait est ignoré.
    '
    ' Historique :
    ' - V1.0.1 (25/10/2026, audit D-1) : Return False explicite après un refus de droit
    '   (avertissement BC42353 ; le résultat était déjà False).
    ' -------------------------------------------------------------------------------------------------
    Private Function EnregistrerLogo(idDomaine As ULong) As Boolean

        If idDomaine = 0UL OrElse (_logoChoisi Is Nothing AndAlso Not _logoRetire) Then Return True

        Try
            If _logoChoisi IsNot Nothing Then
                GestionDomaines.DefinirLogo(_context?.ContexteOperation, idDomaine, CheminsDomaineHelper.EnregistrerLogo(idDomaine, _logoChoisi))
            Else
                GestionDomaines.DefinirLogo(_context?.ContexteOperation, idDomaine, Nothing)
                CheminsDomaineHelper.SupprimerLogo(idDomaine)
            End If
            Return True
        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
            Return False
        Catch ex As Exception
            GestionLog.EcrireException($"UC_DomainesHome : logo du domaine {idDomaine} non enregistré.", ex,
                                       GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)
            Return False
        Finally
            LogosDomaines.Oublier(idDomaine)
        End Try

    End Function

#End Region

#Region "Modes et droits"

    ' Droit d'écriture (SuperUser). Sans contexte, rien n'est permis : refuser par défaut (audit B10 :
    ' l'écran donnait tous les droits si le contexte manquait).
    Private Function PeutModifier() As Boolean

        Return _context IsNot Nothing AndAlso _context.HasRole(AppRole.SuperUser)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : PasserEnMode
    ' Version   : V1.1.0
    ' Date      : 20/10/2026
    ' Rôle      : Verrouille / déverrouille l'écran selon le mode.
    ' Remarques : Pendant l'édition, la liste et les onglets liés sont bloqués pour éviter
    '             de rattacher des éléments à un domaine non encore enregistré.
    ' - V1.1.0 (20/10/2026) : retour en consultation : le logo choisi non enregistré est oublié.
    ' -------------------------------------------------------------------------------------------------
    Private Sub PasserEnMode(mode As ModeEcran)

        _mode = mode
        Dim enEdition As Boolean = mode <> ModeEcran.Consultation

        ' Consultation : les champs restent lisibles (lecture seule), seuls les boutons sont indisponibles.
        UtilsControls.DefinirLectureSeule(carteIdentite, Not enEdition)
        UtilsControls.DefinirLectureSeule(carteComportement, Not enEdition)
        splMain.Panel1.Enabled = Not enEdition

        For Each page As TabPage In {tabNatures, tabTarifs, tabQuotas, tabSynthese}
            page.Enabled = Not enEdition
        Next

        Dim lectureSeuleLiaisons As Boolean = enEdition OrElse Not PeutModifier()
        _pnlNatures?.DefinirLectureSeule(lectureSeuleLiaisons)
        _pnlTarifs?.DefinirLectureSeule(lectureSeuleLiaisons)

        If enEdition Then tabDomaine.SelectedTab = tabGeneral

        btnQuotaCategories.Enabled = Not enEdition AndAlso PeutModifier()

        ActualiserBoutons()
        ActualiserBoutonsQuota()
        AfficherDomaineCourant()

        If Not enEdition Then OublierLogoChoisi()

        ' Création / modification : photo de la fiche une fois remplie ; consultation : rien à perdre.
        If enEdition Then
            _gardeDomaine?.PhotographierApresChargement()
        Else
            _gardeDomaine?.Oublier()
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ActualiserBoutons
    ' Version   : V1.1.0
    ' Date      : 19/10/2026
    '
    ' Rôle      :
    ' Active les boutons de la barre selon le mode, les droits et le domaine sélectionné.
    '
    ' Remarques :
    ' - « Modifier » ne concerne que l'étape 1 (Général) : il n'est montré que sur cette étape, comme
    '   sur la fiche d'un dossier (UC_DossierFiche.AfficherBoutonModifier). Les étapes 2 à 4 se
    '   modifient en consultation, action par action (ADR-40, Reference_UI § 5.7).
    ' - V1.1.0 (19/10/2026) : Modifier masqué hors de l'étape 1 ; Désactiver prend alors sa place.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ActualiserBoutons()

        Dim enEdition As Boolean = _mode <> ModeEcran.Consultation
        Dim droits As Boolean = PeutModifier()
        Dim dom As Domaine = DomaineSelectionne()

        btnNouveau.Enabled = Not enEdition AndAlso droits
        btnModifier.Visible = tabDomaine.SelectedTab Is tabGeneral
        btnModifier.Enabled = Not enEdition AndAlso droits AndAlso dom IsNot Nothing
        btnActiverDesactiver.Left = If(btnModifier.Visible, _gaucheDesactiver, _gaucheModifier)
        btnActiverDesactiver.Enabled = Not enEdition AndAlso droits AndAlso dom IsNot Nothing
        btnActiverDesactiver.Text = If(dom IsNot Nothing AndAlso Not dom.Actif, "Activer", "Désactiver")
        btnEnregistrer.Enabled = enEdition
        btnAnnuler.Enabled = enEdition
        btnLogoImporter.Enabled = enEdition
        btnLogoRetirer.Enabled = enEdition AndAlso picLogo.Image IsNot Nothing

    End Sub

#End Region

#Region "Actions"

    Private Sub btnNouveau_Click(sender As Object, e As EventArgs) Handles btnNouveau.Click

        dgvDomaines.ClearSelection()
        ViderGeneral()
        numOrdre.Value = Math.Min(numOrdre.Maximum, If(_domaines.Count = 0, 1, _domaines.Max(Function(d) d.OrdreAffichage) + 1))
        PasserEnMode(ModeEcran.Creation)
        txtCode.Focus()

    End Sub

    Private Sub btnModifier_Click(sender As Object, e As EventArgs) Handles btnModifier.Click

        If DomaineSelectionne() Is Nothing Then Return
        PasserEnMode(ModeEcran.Modification)
        txtLibelle.Focus()

    End Sub

    Private Sub btnAnnuler_Click(sender As Object, e As EventArgs) Handles btnAnnuler.Click

        If _gardeDomaine IsNot Nothing AndAlso Not _gardeDomaine.ConfirmerAnnulation(DeQuoiDomaine()) Then Return

        _context?.ClearAllErrors()
        PasserEnMode(ModeEcran.Consultation)
        AfficherSelection()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnEnregistrer_Click
    ' Version   : V1.2.0
    ' Date      : 23/10/2026
    ' Rôle      : Valide et enregistre le domaine, puis guide vers l'étape suivante.
    ' - V1.1.0 (20/10/2026) : logo écrit avec le domaine (EnregistrerLogo) ; avertissement s'il n'a pas
    '   pu l'être.
    ' - V1.2.0 (23/10/2026, audit C.2) : domaine construit par DomaineSaisi et jugé par le métier ; refus
    '   du métier affiché.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEnregistrer_Click(sender As Object, e As EventArgs) Handles btnEnregistrer.Click

        Dim dom As Domaine = DomaineSaisi(If(_mode = ModeEcran.Modification, IdSelectionne(), 0UL))
        If Not ValiderSaisie(dom) Then Return

        Try
            Dim creation As Boolean = _mode = ModeEcran.Creation

            If creation Then
                GestionDomaines.InsertDomaine(_context?.ContexteOperation, dom)
            Else
                GestionDomaines.UpdateDomaine(_context?.ContexteOperation, dom)
            End If

            ' Identifiant du domaine (créé : retrouvé par son code, unique) pour ranger son logo.
            Dim enregistre As Domaine = GestionDomaines.GetDomaines(True).
                FirstOrDefault(Function(d) d.CodeDomaine = dom.CodeDomaine)
            Dim idDomaine As ULong = If(enregistre Is Nothing, 0UL, enregistre.IdDomaine)
            Dim logoEnregistre As Boolean = EnregistrerLogo(idDomaine)

            PasserEnMode(ModeEcran.Consultation)
            ChargerListe(idDomaine)

            _context?.SetStatus($"Domaine « {dom.LibelleDomaine} » enregistré.")

            If Not logoEnregistre Then
                DialogChoix.Avertissement(
                    "Le domaine est enregistré, mais son logo n'a pas pu l'être (dossier de données inaccessible ?)." &
                    Environment.NewLine & "Le détail est dans le journal ; vous pouvez réessayer avec Modifier.",
                    "Logo non enregistré")
            End If

            If creation Then
                DialogChoix.Information(
                    "Le domaine est créé." & Environment.NewLine & Environment.NewLine &
                    "Suivez maintenant les étapes affichées en haut : celles qui restent à compléter sont " &
                    "marquées d'un « ! », celles qui ne sont pas nécessaires pour ce domaine d'un tiret.",
                    "Domaine créé")
                tabDomaine.SelectedTab = PremiereEtapeACompleter(DomaineSelectionne())
            End If

        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
        Catch exRefus As InvalidOperationException
            ' Refus du métier (domaine changé entre-temps sur un autre poste…) : la saisie reste ouverte.
            DialogChoix.Information(exRefus.Message, "Enregistrement impossible")
        Catch ex As Exception
            GestionLog.EcrireLog("UC_DomainesHome : erreur enregistrement domaine.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            DialogChoix.Erreur("Erreur lors de l'enregistrement du domaine.", "Erreur")
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : DomaineSaisi
    ' Version  : V1.0.0
    ' Date     : 23/10/2026
    '
    ' Rôle     :
    ' Domaine tel qu'il est saisi à l'étape 1 (sans l'enregistrer) : sert à la validation comme à
    ' l'enregistrement, pour que le métier juge exactement ce qui sera écrit.
    '
    ' Paramètres :
    ' - idDomaine : domaine modifié, ou 0 en création
    ' -------------------------------------------------------------------------------------------------
    Private Function DomaineSaisi(idDomaine As ULong) As Domaine

        Return New Domaine With {
            .IdDomaine = idDomaine,
            .CodeDomaine = txtCode.Text.Trim().ToUpperInvariant(),
            .LibelleDomaine = txtLibelle.Text.Trim(),
            .OrdreAffichage = CInt(numOrdre.Value),
            .Actif = chkActif.Checked,
            .TarifConvention = chkTarifConvention.Checked,
            .NoShowFacturable = chkNoShowFacturable.Checked,
            .SoumisQuota = chkSoumisQuota.Checked,
            .IdOrganisme = OrganismeSaisi()
        }

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ValiderSaisie
    ' Version  : V2.0.0
    ' Date     : 23/10/2026
    '
    ' Rôle     :
    ' Fait juger la saisie par le métier (GestionDomaines.ValiderDomaine : code de 3 lettres et unique,
    ' libellé obligatoire et unique, quota pas retiré tant que des règles sont actives) et montre les
    ' erreurs.
    '
    ' Retour   :
    ' - Boolean : True si la saisie est valide, False sinon
    '
    ' Remarques :
    ' - V2.0.0 (23/10/2026, audit C.2, ADR-51) : plus de copie des règles ici ; toutes les erreurs
    '   d'un coup (une fenêtre par erreur auparavant, et seulement la première) : icône sur chaque
    '   champ, fenêtre récapitulative, focus sur le premier (AffichageErreursSaisie).
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderSaisie(dom As Domaine) As Boolean

        _context?.ClearAllErrors()

        Dim erreurs As List(Of ErreurSaisie) = GestionDomaines.ValiderDomaine(dom)

        Return Not AffichageErreursSaisie.Signaler(erreurs, ChampsDuDomaine(),
                                                   Sub(c, m) _context?.SetError(c, m),
                                                   "Domaine")

    End Function

    ' Contrôle de chaque propriété du domaine jugée par ValiderDomaine (ErreurSaisie.Champ).
    Private Function ChampsDuDomaine() As Dictionary(Of String, Control)
        Return New Dictionary(Of String, Control) From {
            {NameOf(Domaine.CodeDomaine), txtCode},
            {NameOf(Domaine.LibelleDomaine), txtLibelle},
            {NameOf(Domaine.SoumisQuota), chkSoumisQuota}
        }
    End Function

    Private Sub btnActiverDesactiver_Click(sender As Object, e As EventArgs) Handles btnActiverDesactiver.Click

        Dim dom As Domaine = DomaineSelectionne()
        If dom Is Nothing Then Return

        Try
            If dom.Actif Then
                If GestionDomaines.DomaineEstUtilise(dom.IdDomaine) AndAlso
                   DialogChoix.Confirmer(
                       "Ce domaine est utilisé par des dossiers existants." & Environment.NewLine &
                       "Il ne sera plus proposé pour de nouveaux dossiers. Continuer ?",
                       "Désactivation") <> DialogResult.Yes Then
                    Return
                End If
                GestionDomaines.DesactiverDomaine(_context?.ContexteOperation, dom.IdDomaine)
            Else
                dom.Actif = True
                GestionDomaines.UpdateDomaine(_context?.ContexteOperation, dom)
            End If

            ChargerListe(dom.IdDomaine)

        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
        Catch exRefus As InvalidOperationException
            ' Refus du métier (réactivation d'un domaine dont la fiche n'est plus valide).
            DialogChoix.Information(exRefus.Message, "Réactivation impossible")
        Catch ex As Exception
            GestionLog.EcrireLog("UC_DomainesHome : erreur activation domaine.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            DialogChoix.Erreur("Erreur lors du changement d'état du domaine.", "Erreur")
        End Try

    End Sub

#End Region

#Region "Création en contexte"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirReferentiel
    ' Rôle      : Ouvre un référentiel existant en fenêtre modale (sans quitter l'écran),
    '             puis recharge les listes de rattachement pour proposer le nouvel élément.
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirReferentiel(vue As UserControl, titre As String)

        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale
        If fenetre Is Nothing Then Return

        If fenetre.OuvrirReferentielModal(vue, titre) Then
            AfficherSelection()
        End If

    End Sub

    ' Création d'une nature ou d'un tarif depuis la barre des panneaux de rattachement : le
    ' référentiel s'ouvre en fenêtre, puis les listes sont rechargées.
    Private Sub _pnlNatures_CreationDemandee(sender As Object, e As EventArgs) Handles _pnlNatures.CreationDemandee
        OuvrirReferentiel(New UC_NaturesSeance(), "Natures de séance")
    End Sub

    ' Création d'un organisme sans quitter l'écran : la saisie en cours du domaine est conservée,
    ' seule la liste des organismes est rechargée (le nouvel organisme est sélectionné s'il est unique).
    Private Sub btnNouvelOrganisme_Click(sender As Object, e As EventArgs) Handles btnNouvelOrganisme.Click

        Dim fenetre As IFenetrePrincipale = _context?.FenetrePrincipale
        If fenetre Is Nothing Then Return

        Dim idAvant As ULong? = OrganismeSaisi()
        Dim existants As New HashSet(Of ULong)(
            cboOrganisme.Items.Cast(Of KeyValuePair(Of ULong, String))().Select(Function(kv) kv.Key))

        fenetre.OuvrirReferentielModal(New UC_Organismes(TypeOrganisme.Remboursement), "Organismes")

        ChargerOrganismes(idAvant)

        Dim nouveaux = cboOrganisme.Items.Cast(Of KeyValuePair(Of ULong, String))().
            Where(Function(kv) Not existants.Contains(kv.Key)).ToList()
        If nouveaux.Count = 1 Then cboOrganisme.SelectedValue = nouveaux(0).Key

    End Sub

    Private Sub _pnlTarifs_CreationDemandee(sender As Object, e As EventArgs) Handles _pnlTarifs.CreationDemandee
        OuvrirReferentiel(New UC_PrixSeance(), "Tarifs de séance")
    End Sub

#End Region

#Region "Quotas"

    Private Enum ModeQuota
        Consultation
        NouvelleRegle
        Modification
        Cloture
    End Enum

    Private Sub chkQuotaFin_CheckedChanged(sender As Object, e As EventArgs) Handles chkQuotaFin.CheckedChanged
        If _modeQuota = ModeQuota.NouvelleRegle Then dtpQuotaFin.Enabled = chkQuotaFin.Checked
    End Sub

    Private Sub UC_DomainesHome_Disposed(sender As Object, e As EventArgs) Handles MyBase.Disposed
        _policeSansRegle.Dispose()
        _logoChoisi?.Dispose()
        UtilsImages.RemplacerImage(picLogo, Nothing)
    End Sub

    ' Pastille « État » : celle de la règle, ou « Sans règle » à surveiller sur une ligne de période (V1.7.0).
    Private Sub dgvQuotas_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvQuotas.CellPainting
        If e.RowIndex < 0 OrElse e.ColumnIndex <> colQuotaEtat.Index Then Return
        Dim contenu As Object = dgvQuotas.Rows(e.RowIndex).Tag
        Dim regle As QuotaInamiRegle = TryCast(contenu, QuotaInamiRegle)
        If regle IsNot Nothing Then
            UtilsDataGrid.DessinerPastilleCellule(dgvQuotas, e, regle.LibelleEtat, UtilsPastilles.Teinte(regle.Etat))
        ElseIf TypeOf contenu Is PeriodeSansRegle Then
            UtilsDataGrid.DessinerPastilleCellule(dgvQuotas, e, CStr(dgvQuotas.Rows(e.RowIndex).Cells(e.ColumnIndex).Value),
                                                  UtilsPastilles.Teinte(TonEtat.ASurveiller))
        End If
    End Sub

    Private Sub dgvQuotas_SelectionChanged(sender As Object, e As EventArgs) Handles dgvQuotas.SelectionChanged
        If _modeQuota = ModeQuota.Consultation Then AfficherZoneQuota()
        ActualiserBoutonsQuota()
    End Sub

    ' Après une modification de quota : rafraîchit l'étape, la synthèse et l'état dans la liste.
    Private Sub RafraichirApresQuota()

        Dim dom As Domaine = DomaineSelectionne()
        ChargerQuotas(dom)
        AfficherSynthese(dom)
        If dom IsNot Nothing Then AppliquerFiltre(dom.IdDomaine)

    End Sub

    ' Ouvre la zone sous la grille dans un mode de saisie, ou la ramène en consultation.
    Private Sub PasserEnModeQuota(mode As ModeQuota)

        _modeQuota = mode
        AfficherZoneQuota()
        ActualiserBoutonsQuota()

        If mode = ModeQuota.Consultation Then
            _gardeQuota?.Oublier()
        Else
            _gardeQuota?.PhotographierApresChargement()
        End If

    End Sub

    Private Sub btnQuotaAjouter_Click(sender As Object, e As EventArgs) Handles btnQuotaAjouter.Click

        If DomaineSelectionne() Is Nothing Then Return
        PasserEnModeQuota(ModeQuota.NouvelleRegle)
        CommencerSaisiePlafonds()

    End Sub

    Private Sub btnQuotaCloturer_Click(sender As Object, e As EventArgs) Handles btnQuotaCloturer.Click

        If QuotaSelectionne() Is Nothing Then Return
        PasserEnModeQuota(ModeQuota.Cloture)
        dtpQuotaFin.Focus()

    End Sub

    Private Sub btnQuotaAnnuler_Click(sender As Object, e As EventArgs) Handles btnQuotaAnnuler.Click
        If _gardeQuota IsNot Nothing AndAlso Not _gardeQuota.ConfirmerAnnulation("de la règle de quota") Then Return
        PasserEnModeQuota(ModeQuota.Consultation)
    End Sub

    Private Sub btnQuotaValider_Click(sender As Object, e As EventArgs) Handles btnQuotaValider.Click

        Select Case _modeQuota
            Case ModeQuota.NouvelleRegle : AjouterRegle()
            Case ModeQuota.Modification : CorrigerRegle()
            Case ModeQuota.Cloture : CloturerRegle()
        End Select

    End Sub

    ' Correction d'une règle : seulement si aucune séance n'est encore concernée (refus expliqué sinon).
    Private Sub btnQuotaModifier_Click(sender As Object, e As EventArgs) Handles btnQuotaModifier.Click

        Dim regle As QuotaInamiRegle = QuotaSelectionne()
        If regle Is Nothing Then Return

        Try
            Dim refus As String = GestionQuotasInami.ControlerModification(regle)
            If refus <> String.Empty Then
                DialogChoix.Information(refus, "Correction impossible")
                Return
            End If
        Catch ex As Exception
            GestionLog.EcrireLog("UC_DomainesHome : erreur contrôle modification quota.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            DialogChoix.Erreur("Impossible de vérifier si la règle a déjà servi.", "Erreur")
            Return
        End Try

        PasserEnModeQuota(ModeQuota.Modification)
        CommencerSaisiePlafonds()

    End Sub

    ' La saisie commence par le premier plafond : c'est ce que l'on vient le plus souvent changer.
    Private Sub CommencerSaisiePlafonds()

        If dgvQuotaPlafonds.Rows.Count > 0 Then
            dgvQuotaPlafonds.Focus()
            dgvQuotaPlafonds.CurrentCell = dgvQuotaPlafonds.Rows(0).Cells(colPlafondValeur.Index)
            dgvQuotaPlafonds.BeginEdit(True)
        Else
            numQuotaDuree.Focus()
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ConfirmerRegle
    ' Rôle     : Récapitulatif à confirmer avant d'enregistrer une règle nouvelle ou corrigée : c'est le
    '            dernier moment pour relire ; une règle ne se corrige plus dès qu'une séance est concernée.
    '            Présenté avec les parties structurées de DialogChoix V2 (détails, encadré d'attention).
    ' Retour   : True si l'utilisateur confirme l'enregistrement.
    ' -------------------------------------------------------------------------------------------------
    Private Shared Function ConfirmerRegle(regle As QuotaInamiRegle, aRemplacer As QuotaInamiRegle, correction As Boolean,
                                           alerteTrous As String) As Boolean

        Dim complements As New List(Of String)

        If aRemplacer IsNot Nothing Then
            complements.Add($"La règle actuelle ({aRemplacer.LibellePlafonds}) s'arrêtera le {regle.DateDebutValidite.AddDays(-1):dd/MM/yyyy}.")
        End If

        ' Date de début passée (règle édictée au 1er janvier, saisie le 5…) : c'est permis, le décompte
        ' étant recalculé à la date de chaque séance ; l'utilisateur est simplement prévenu.
        If regle.DateDebutValidite.Date < Date.Today Then
            complements.Add($"Date de début passée : les séances enregistrées depuis le {regle.DateDebutValidite:dd/MM/yyyy} " &
                            "seront décomptées selon cette règle.")
        End If

        Using dlg As New DialogChoix()
            dlg.Titre = If(correction, "Correction de la règle", "Nouvelle règle de quota")
            dlg.TypeDialogue = TypeDialogue.Question
            dlg.Message = If(correction, "Vérifiez la règle corrigée :", "Vérifiez la nouvelle règle avant de l'enregistrer :")
            dlg.AjouterDetail("Plafonds", regle.LibellePlafonds)
            dlg.AjouterDetail("Alerte", If(regle.SeuilAlerteTotal > 0, $"à {regle.SeuilAlerteTotal} séances", "aucune"))
            dlg.AjouterDetail("Validité", $"à partir du {regle.DateDebutValidite:dd/MM/yyyy}" &
                              If(regle.DateFinValidite.HasValue, $" jusqu'au {regle.DateFinValidite.Value:dd/MM/yyyy}", String.Empty))
            dlg.Complement = String.Join(Environment.NewLine, complements)
            dlg.Attention = alerteTrous
            dlg.Remarque = "La règle restera corrigeable tant qu'aucune séance n'est concernée ; ensuite, elle ne se modifie plus."
            dlg.QuestionFinale = If(correction, "Enregistrer la correction ?", "Enregistrer cette règle ?")
            dlg.VarianteAction = VarianteBouton.Ocre
            dlg.SetBoutons("Enregistrer", "Revenir à la saisie")
            Return dlg.ShowDialog() = DialogResult.Yes
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : CorrigerRegle
    ' Rôle      : Enregistre la correction d'une règle pas encore utilisée (contrôlée par la couche
    '             métier : utilisation, chevauchement, plafonds). Pas d'arrêt automatique d'une autre
    '             règle : un chevauchement est refusé avec son explication.
    ' -------------------------------------------------------------------------------------------------
    Private Sub CorrigerRegle()

        Dim originale As QuotaInamiRegle = QuotaSelectionne()
        If originale Is Nothing Then
            PasserEnModeQuota(ModeQuota.Consultation)
            Return
        End If

        Dim plafonds As List(Of QuotaPlafond) = PlafondsSaisis()
        If plafonds Is Nothing Then
            DialogChoix.Avertissement("Chaque plafond doit être un nombre entier positif ou nul.", "Règle de quota")
            Return
        End If

        Dim corrigee As New QuotaInamiRegle With {
            .IdQuotaInami = originale.IdQuotaInami,
            .IdDomaine = originale.IdDomaine,
            .Plafonds = plafonds,
            .DureePeriodeMois = CInt(numQuotaDuree.Value),
            .SeuilAlerteTotal = CInt(numQuotaAlerte.Value),
            .DateDebutValidite = dtpQuotaDebut.Valeur.Value.Date,
            .DateFinValidite = If(chkQuotaFin.Checked, dtpQuotaFin.Valeur.Value.Date, CType(Nothing, Date?)),
            .Actif = originale.Actif
        }

        Dim erreur As String = GestionQuotasInami.ValiderRegle(corrigee)
        If erreur <> String.Empty Then
            DialogChoix.Avertissement(erreur, "Règle de quota")
            Return
        End If

        ' Corriger une date ne déplace jamais la fin d'une autre règle (décision du 14/10/2026) : un
        ' écart qui en résulterait est annoncé, puis signalé dans la grille et dans la complétude.
        Dim simulees = _quotas.Select(Function(r) If(r.IdQuotaInami = corrigee.IdQuotaInami, corrigee, r))
        Dim alerte As String = AlerteTrousCrees(simulees, "Après cette correction")

        If Not ConfirmerRegle(corrigee, Nothing, True, alerte) Then Return

        Try
            GestionQuotasInami.ModifierRegleInutilisee(_context?.ContexteOperation, originale, corrigee)
            _context?.SetStatus("Règle de quota corrigée.")
            RafraichirApresQuota()
        Catch ex As InvalidOperationException
            DialogChoix.Information(ex.Message, "Correction impossible")
        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
        Catch ex As Exception
            GestionLog.EcrireLog("UC_DomainesHome : erreur correction quota.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            DialogChoix.Erreur("La règle n'a pas pu être corrigée. Rien n'a été modifié.", "Erreur")
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AjouterRegle
    ' Rôle      : Enregistre la nouvelle règle saisie ; la règle ouverte qui la précède est clôturée
    '             la veille, après une seule confirmation, dans la même transaction que l'ajout
    '             (GestionQuotasInami.AjouterRegleEnChaine, 21/10/2026 : tout ou rien).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AjouterRegle()

        Dim dom As Domaine = DomaineSelectionne()
        If dom Is Nothing Then Return

        Dim plafonds As List(Of QuotaPlafond) = PlafondsSaisis()
        If plafonds Is Nothing Then
            DialogChoix.Avertissement("Chaque plafond doit être un nombre entier positif ou nul.", "Règle de quota")
            Return
        End If

        If plafonds.Count = 0 Then
            DialogChoix.Avertissement(
                "Aucune catégorie de quota active n'est définie pour ce domaine." & Environment.NewLine &
                "Utilisez « Gérer les catégories… » pour les créer (par exemple Soutien, Traitement).", "Règle de quota")
            Return
        End If

        Dim regle As New QuotaInamiRegle With {
            .IdDomaine = dom.IdDomaine,
            .Plafonds = plafonds,
            .DureePeriodeMois = CInt(numQuotaDuree.Value),
            .SeuilAlerteTotal = CInt(numQuotaAlerte.Value),
            .DateDebutValidite = dtpQuotaDebut.Valeur.Value.Date,
            .DateFinValidite = If(chkQuotaFin.Checked, dtpQuotaFin.Valeur.Value.Date, CType(Nothing, Date?)),
            .Actif = True
        }

        ' Enchaînement simple : si une règle ouverte (sans fin) a commencé avant la nouvelle, elle est
        ' clôturée automatiquement la veille du début de la nouvelle règle, après une seule confirmation.
        Dim aRemplacer As QuotaInamiRegle = _quotas.FirstOrDefault(
            Function(r) r.Actif AndAlso Not r.DateFinValidite.HasValue AndAlso
                        r.DateDebutValidite.Date < regle.DateDebutValidite)

        ' Cohérence de la saisie (plafonds, seuil, dates) avant le récapitulatif : inutile de faire
        ' confirmer une règle qui sera refusée. Le chevauchement est contrôlé à l'enregistrement.
        Dim incoherence As String = GestionQuotasInami.ValiderCoherence(regle)
        If incoherence <> String.Empty Then
            DialogChoix.Avertissement(incoherence, "Règle de quota")
            Return
        End If

        Dim simulees As New List(Of QuotaInamiRegle)(
            _quotas.Select(Function(r) If(r Is aRemplacer, CopiePourSimulation(r, regle.DateDebutValidite.AddDays(-1)), r)))
        simulees.Add(regle)
        Dim alerte As String = AlerteTrousCrees(simulees, "Avec cette règle")

        If Not ConfirmerRegle(regle, aRemplacer, False, alerte) Then Return

        Try
            GestionQuotasInami.AjouterRegleEnChaine(_context?.ContexteOperation, regle, aRemplacer)
            _context?.SetStatus(If(aRemplacer Is Nothing, "Règle de quota ajoutée.",
                                   "Règle de quota ajoutée ; la règle précédente est arrêtée la veille."))
            RafraichirApresQuota()
        Catch ex As InvalidOperationException
            ' Refus (chevauchement, règle précédente modifiée entre-temps…) : rien n'a été enregistré.
            DialogChoix.Avertissement(ex.Message, "Règle de quota")
        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
        Catch ex As Exception
            GestionLog.EcrireLog("UC_DomainesHome : erreur ajout quota.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            DialogChoix.Erreur("La règle n'a pas pu être enregistrée. Rien n'a été modifié (la règle actuelle reste en vigueur).", "Erreur")
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : CloturerRegle
    ' Rôle      : Clôture la règle sélectionnée au dernier jour saisi (refusé avant son début).
    ' -------------------------------------------------------------------------------------------------
    Private Sub CloturerRegle()

        Dim regle As QuotaInamiRegle = QuotaSelectionne()
        If regle Is Nothing Then
            PasserEnModeQuota(ModeQuota.Consultation)
            Return
        End If

        Dim dateFin As Date = dtpQuotaFin.Valeur.Value.Date

        If dateFin < regle.DateDebutValidite.Date Then
            DialogChoix.Avertissement(
                $"La règle sélectionnée commence le {regle.DateDebutValidite:dd/MM/yyyy} : " &
                $"elle ne peut pas être clôturée au {dateFin:dd/MM/yyyy}.", "Clôture")
            Return
        End If

        Dim simulees = _quotas.Select(Function(r) If(r.IdQuotaInami = regle.IdQuotaInami, CopiePourSimulation(r, dateFin), r))
        If DialogChoix.Confirmer($"Clôturer cette règle au {dateFin:dd/MM/yyyy} ?", "Clôture de la règle", "Clôturer", "Annuler",
                                 attention:=AlerteTrousCrees(simulees, "Après la clôture")) <> DialogResult.Yes Then Return

        Try
            regle.DateFinValidite = dateFin
            GestionQuotasInami.UpdateQuotaInami(_context?.ContexteOperation, regle)
            RafraichirApresQuota()
        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
        Catch ex As Exception
            GestionLog.EcrireLog("UC_DomainesHome : erreur clôture quota.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            DialogChoix.Erreur("Erreur lors de la clôture de la règle.", "Erreur")
        End Try

    End Sub

    Private Sub btnQuotaActiver_Click(sender As Object, e As EventArgs) Handles btnQuotaActiver.Click

        Dim regle As QuotaInamiRegle = QuotaSelectionne()
        If regle Is Nothing Then Return

        Try
            If Not regle.Actif Then
                regle.Actif = True
                Dim erreur As String = GestionQuotasInami.ValiderRegle(regle)
                If erreur <> String.Empty Then
                    regle.Actif = False
                    DialogChoix.Avertissement(erreur, "Réactivation impossible")
                    Return
                End If
            End If

            GestionQuotasInami.DefinirActivationQuotaInami(_context?.ContexteOperation, regle.IdQuotaInami, regle.Actif)
            RafraichirApresQuota()
        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
        Catch ex As Exception
            GestionLog.EcrireLog("UC_DomainesHome : erreur activation quota.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            DialogChoix.Erreur("Erreur lors du changement d'état de la règle.", "Erreur")
        End Try

    End Sub

    Private Sub btnQuotaSupprimer_Click(sender As Object, e As EventArgs) Handles btnQuotaSupprimer.Click

        Dim regle As QuotaInamiRegle = QuotaSelectionne()
        If regle Is Nothing Then Return

        Try
            ' Une règle qui a servi au décompte reste (historique des quotas) : explication, pas d'erreur.
            Dim refus As String = GestionQuotasInami.ControlerSuppression(regle)
            If refus <> String.Empty Then
                DialogChoix.Information(refus, "Suppression impossible")
                Return
            End If

            ' Si cette règle avait arrêté la précédente, la suppression la rouvre (période sans trou).
            Dim precedente As QuotaInamiRegle = GestionQuotasInami.ReglePrecedenteArretee(regle, _quotas)

            Dim message As String = "Cette règle (saisie erronée) sera supprimée définitivement."
            If precedente IsNot Nothing Then
                message &= Environment.NewLine & Environment.NewLine &
                           $"La règle précédente ({precedente.LibellePlafonds}) avait été arrêtée le " &
                           $"{precedente.DateFinValidite.Value:dd/MM/yyyy} à cause de celle-ci : elle sera rouverte" &
                           If(regle.DateFinValidite.HasValue, $" jusqu'au {regle.DateFinValidite.Value:dd/MM/yyyy}.", ", sans date de fin.")
            End If

            ' Simulation : règles restantes (la précédente éventuellement rouverte) → période sans règle ?
            Dim restantes As New List(Of QuotaInamiRegle)
            For Each r In _quotas.Where(Function(x) x.IdQuotaInami <> regle.IdQuotaInami)
                If precedente IsNot Nothing AndAlso r.IdQuotaInami = precedente.IdQuotaInami Then
                    restantes.Add(New QuotaInamiRegle With {.IdQuotaInami = r.IdQuotaInami, .IdDomaine = r.IdDomaine,
                                                            .DateDebutValidite = r.DateDebutValidite,
                                                            .DateFinValidite = regle.DateFinValidite, .Actif = r.Actif})
                Else
                    restantes.Add(r)
                End If
            Next

            Dim trous = GestionQuotasInami.PeriodesSansRegle(restantes)
            Dim attention As String = String.Empty
            If trous.Count > 0 Then
                attention = "Après la suppression, aucune règle ne couvrira " &
                            String.Join(", ", trous.Select(Function(t) LibellePeriode(t))) &
                            ". Pensez à la couvrir par une « Nouvelle règle »."
            End If

            If DialogChoix.ConfirmerSuppression(message, "Supprimer la règle ?", attention) <> DialogResult.Yes Then Return

            GestionQuotasInami.SupprimerRegleErronee(_context?.ContexteOperation, regle, precedente)
            _context?.SetStatus(If(precedente Is Nothing, "Règle de quota supprimée.",
                                   "Règle de quota supprimée ; la règle précédente est rouverte."))
            RafraichirApresQuota()

        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
        Catch ex As Exception
            GestionLog.EcrireLog("UC_DomainesHome : erreur suppression quota.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            DialogChoix.Erreur("La règle n'a pas pu être supprimée. Rien n'a été modifié.", "Erreur")
        End Try

    End Sub

#End Region

#Region "Assistant pas à pas"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ActualiserAssistant
    ' Version   : V2.1.0
    ' Date      : 19/10/2026
    '
    ' Rôle      :
    ' Met à jour les étapes (état de chacune, étape affichée), la consigne de l'étape affichée et la
    ' pastille d'état du domaine dans le bandeau. Précédent / Suivant parcourent les étapes.
    '
    ' Remarques :
    ' - Étape = page de tabDomaine (même index) ; la bande d'onglets est masquée (MasquerOnglets).
    ' - Les états viennent de GestionDomaines.CalculerManques, source unique des règles
    '   de complétude : l'écran ne refait aucun calcul de son côté.
    ' - V2.1.0 (19/10/2026) : étapes 2 à 4 : « se modifie directement… sans passer par Modifier ».
    ' -------------------------------------------------------------------------------------------------
    Private Sub ActualiserAssistant(dom As Domaine)

        Dim index As Integer = Math.Max(0, tabDomaine.SelectedIndex)
        Dim total As Integer = tabDomaine.TabPages.Count
        Dim consultation As Boolean = _mode = ModeEcran.Consultation

        etapes.IndexCourant = index
        etapes.Enabled = consultation AndAlso dom IsNot Nothing
        btnAssistantPrecedent.Enabled = etapes.Enabled AndAlso index > 0
        btnAssistantSuivant.Enabled = etapes.Enabled AndAlso index < total - 1

        If Not consultation Then
            ' Règle d'ADR-34 : « Modifier » concerne les champs de l'étape 1 ; les étapes 2 à 4 se
            ' modifient en consultation, action par action (enregistrement immédiat).
            etapes.ReinitialiserEtats()
            lblAssistantEtape.Text = If(_mode = ModeEcran.Creation,
                "Complétez l'identité et le comportement, puis Enregistrer : les étapes 2 à 4 s'ouvriront ensuite.",
                "Modification de l'identité et du comportement. Enregistrez ou annulez pour retrouver les étapes 2 à 4, " &
                "qui se modifient directement (chaque action y est enregistrée immédiatement).")
            AfficherEtatDomaine(If(_mode = ModeEcran.Creation, "Nouveau domaine", "Modification en cours"), Nothing)
            Return
        End If

        If dom Is Nothing Then
            etapes.ReinitialiserEtats()
            lblAssistantEtape.Text = "Sélectionnez un domaine, ou cliquez sur Nouveau pour en créer un pas à pas."
            AfficherEtatDomaine(String.Empty, Nothing)
            Return
        End If

        Dim manques As List(Of ManqueDomaine) = GestionDomaines.CalculerManques(dom)
        DefinirEtatsEtapes(dom, manques)

        If Not dom.Actif Then
            AfficherEtatDomaine("Inactif", Nothing)
        ElseIf manques.Count = 0 Then
            AfficherEtatDomaine("Domaine complet", True)
        Else
            AfficherEtatDomaine($"À compléter : {manques.Count} point(s)", False)
        End If

        Dim page As TabPage = tabDomaine.SelectedTab
        Dim consigne As String

        If page Is tabGeneral Then
            consigne = "Identité et comportement du domaine (Modifier pour changer). Le comportement décide " &
                       "des étapes utiles : sans convention, l'étape Tarifs n'est pas nécessaire ; sans quota, l'étape Quotas non plus."
        ElseIf page Is tabTarifs Then
            consigne = If(dom.TarifConvention,
                "Rattachez au moins un tarif en vigueur. « Créer un nouveau tarif… » ouvre le catalogue si besoin.",
                "Étape non nécessaire : le domaine n'est pas conventionné, le tarif vient du dossier du patient.")
        ElseIf page Is tabQuotas Then
            consigne = If(dom.SoumisQuota,
                "1) « Gérer les catégories… » : créez les compteurs (ex. Soutien). " &
                "2) Ajoutez une règle : période + plafond par catégorie.",
                "Étape non nécessaire : le domaine n'est pas soumis à quota.")
        ElseIf page Is tabNatures Then
            consigne = "Rattachez les natures de séance proposées à la saisie" &
                If(dom.SoumisQuota, ", puis choisissez pour chacune sa catégorie de quota (ou Hors quota).", ".")
        Else
            consigne = If(manques.Count = 0,
                "Tout est en ordre : le domaine peut être utilisé pour ouvrir des dossiers.",
                $"{manques.Count} point(s) à compléter, listé(s) ci-dessous avec l'étape concernée.")
        End If

        ' Sur les étapes de rattachement, rappeler qu'il n'y a ni « Modifier » ni « Enregistrer ».
        If PeutModifier() AndAlso (page Is tabTarifs OrElse page Is tabQuotas OrElse page Is tabNatures) AndAlso
           Not consigne.StartsWith("Étape non nécessaire") Then
            consigne &= " Cette étape se modifie directement : chaque action est enregistrée aussitôt, sans passer par Modifier."
        End If

        lblAssistantEtape.Text = consigne

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirEtatsEtapes
    ' Version   : V1.0.0
    ' Date      : 14/10/2026
    ' Rôle      : Traduit le comportement du domaine et ses manques en états d'étapes.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DefinirEtatsEtapes(dom As Domaine, manques As List(Of ManqueDomaine))

        Dim aCompleter As Func(Of EtapeDomaine, Boolean) =
            Function(etape As EtapeDomaine) manques.Any(Function(m) m.Etape = etape)

        etapes.DefinirEtat(tabDomaine.TabPages.IndexOf(tabGeneral),
                           If(aCompleter(EtapeDomaine.General), EtatEtape.ACompleter, EtatEtape.Fait))

        etapes.DefinirEtat(tabDomaine.TabPages.IndexOf(tabTarifs),
                           If(Not dom.TarifConvention, EtatEtape.NonNecessaire,
                              If(aCompleter(EtapeDomaine.Tarifs), EtatEtape.ACompleter, EtatEtape.Fait)))

        etapes.DefinirEtat(tabDomaine.TabPages.IndexOf(tabQuotas),
                           If(Not dom.SoumisQuota, EtatEtape.NonNecessaire,
                              If(aCompleter(EtapeDomaine.Quotas), EtatEtape.ACompleter, EtatEtape.Fait)))

        etapes.DefinirEtat(tabDomaine.TabPages.IndexOf(tabNatures),
                           If(aCompleter(EtapeDomaine.Natures), EtatEtape.ACompleter, EtatEtape.Fait))

        If manques.Count = 0 Then
            etapes.DefinirEtat(tabDomaine.TabPages.IndexOf(tabSynthese), EtatEtape.Fait, "Utilisable")
        Else
            etapes.DefinirEtat(tabDomaine.TabPages.IndexOf(tabSynthese), EtatEtape.ACompleter, $"{manques.Count} point(s)")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherEtatDomaine
    ' Rôle      : Pastille d'état à droite du bandeau (vide = masquée).
    ' Paramètres :
    ' - complet : True = vert (complet), False = ocre (à compléter), Nothing = neutre
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherEtatDomaine(texte As String, complet As Boolean?)

        pstEtatDomaine.Visible = texte.Length > 0
        If texte.Length = 0 Then Return

        pstEtatDomaine.Text = texte

        If complet.HasValue AndAlso Not complet.Value Then
            pstEtatDomaine.CouleurFond = AltheaDessin.Melanger(Color.White, UITheme.ColorJauneModere, 0.3F)
            pstEtatDomaine.ForeColor = AltheaDessin.Assombrir(UITheme.ColorOrangeMoyenFonce, 0.3F)
        Else
            pstEtatDomaine.CouleurFond = UITheme.ColorGrisVertClair
            pstEtatDomaine.ForeColor = UITheme.ColorSaugeTresFonce
        End If

        PlacerPastilleEtat()

    End Sub

    ' La pastille suit la taille de son texte : elle est recalée à droite du bandeau.
    Private Sub PlacerPastilleEtat()
        pstEtatDomaine.Left = lblDomaineCourant.ClientSize.Width - pstEtatDomaine.Width - 10
        pstEtatDomaine.Top = (lblDomaineCourant.ClientSize.Height - pstEtatDomaine.Height) \ 2
    End Sub

    Private Sub lblDomaineCourant_Resize(sender As Object, e As EventArgs) Handles lblDomaineCourant.Resize
        If pstEtatDomaine IsNot Nothing Then PlacerPastilleEtat()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : PremiereEtapeACompleter
    ' Rôle     : Page de la première étape qui reste à compléter (Synthèse si tout est en ordre) ;
    '            sert après la création d'un domaine pour enchaîner sur la bonne étape.
    ' -------------------------------------------------------------------------------------------------
    Private Function PremiereEtapeACompleter(dom As Domaine) As TabPage

        Dim premier As ManqueDomaine = GestionDomaines.CalculerManques(dom).
            OrderBy(Function(m) m.Etape).FirstOrDefault()

        If premier Is Nothing Then Return tabSynthese

        Select Case premier.Etape
            Case EtapeDomaine.Tarifs : Return tabTarifs
            Case EtapeDomaine.Quotas : Return tabQuotas
            Case EtapeDomaine.Natures : Return tabNatures
            Case Else : Return tabGeneral
        End Select

    End Function

    ' Choix d'une étape (clic ou flèches) : seulement en consultation, avec un domaine sélectionné.
    Private Sub etapes_EtapeChoisie(sender As Object, index As Integer) Handles etapes.EtapeChoisie
        If _mode = ModeEcran.Consultation AndAlso DomaineSelectionne() IsNot Nothing Then
            tabDomaine.SelectedIndex = index
        End If
    End Sub

    Private Sub btnAssistantPrecedent_Click(sender As Object, e As EventArgs) Handles btnAssistantPrecedent.Click
        If tabDomaine.SelectedIndex > 0 Then tabDomaine.SelectedIndex -= 1
    End Sub

    Private Sub btnAssistantSuivant_Click(sender As Object, e As EventArgs) Handles btnAssistantSuivant.Click
        If tabDomaine.SelectedIndex < tabDomaine.TabPages.Count - 1 Then tabDomaine.SelectedIndex += 1
    End Sub

#End Region

#Region "Événements liste / onglets"

    Private Sub dgvDomaines_SelectionChanged(sender As Object, e As EventArgs) Handles dgvDomaines.SelectionChanged
        AfficherSelection()
    End Sub

    Private Sub txtRecherche_TextChanged(sender As Object, e As EventArgs) Handles txtRecherche.TextChanged
        AppliquerFiltre()
    End Sub

    Private Sub chkAfficherInactifs_CheckedChanged(sender As Object, e As EventArgs) Handles chkAfficherInactifs.CheckedChanged
        ChargerListe()
    End Sub

    ' Les rattachements ont pu changer dans les onglets : on rafraîchit la synthèse et l'état.
    ' Modifier ne se montre que sur l'étape 1 (ActualiserBoutons).
    Private Sub tabDomaine_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tabDomaine.SelectedIndexChanged

        If _mode <> ModeEcran.Consultation Then Return

        ActualiserBoutons()
        AfficherDomaineCourant()

        If tabDomaine.SelectedTab Is tabSynthese
            Dim dom As Domaine = DomaineSelectionne()
            ChargerQuotas(dom)
            AfficherSynthese(dom)
            If tabDomaine.SelectedTab Is tabSynthese Then AppliquerFiltre()
        End If

    End Sub

#End Region

#Region "Modifications non enregistrées"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PeutQuitter
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Avant de quitter l'écran : la fiche du domaine en saisie, puis la règle de quota en
    '              saisie — Enregistrer / Quitter sans enregistrer / Rester, comme partout (ADR-43).
    ' Remarques  : « Enregistrer » passe par le bouton de la saisie (validation, messages) ; il a abouti
    '              si la saisie est revenue en consultation.
    ' -------------------------------------------------------------------------------------------------
    Public Function PeutQuitter() As Boolean Implements IUnsavedChangesAware.PeutQuitter

        If _gardeDomaine IsNot Nothing AndAlso
           Not _gardeDomaine.PeutQuitter(DeQuoiDomaine(), Function()
                                                               btnEnregistrer.PerformClick()
                                                               Return _mode = ModeEcran.Consultation
                                                           End Function) Then
            Return False
        End If

        Return _gardeQuota Is Nothing OrElse
               _gardeQuota.PeutQuitter("de la règle de quota", Function()
                                                                  btnQuotaValider.PerformClick()
                                                                  Return _modeQuota = ModeQuota.Consultation
                                                              End Function)

    End Function

    ' Désignation de la fiche du domaine dans les questions.
    Private Function DeQuoiDomaine() As String
        Return If(_mode = ModeEcran.Creation, "du nouveau domaine", "du domaine")
    End Function

#End Region

End Class
