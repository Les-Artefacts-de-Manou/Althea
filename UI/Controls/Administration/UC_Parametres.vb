' -------------------------------------------------------------------------------------------------
' UserControl : UC_Parametres
' Projet      : Althéa
' Version     : V2.3.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran des paramètres applicatifs : une seule page, une carte par groupe de paramètres, chaque
' paramètre sur une ligne (libellé, valeur, bornes, description en aide sous la valeur).
'
' Responsabilités :
' - Charger les paramètres depuis la couche métier (GestionParametres) selon le mode d'accès
'   (Admin : tous ; SuperUser : paramètres ouverts seulement, filtrés par GestionParametres)
' - Construire une carte par groupe (titre et ordre : tec_groupes_parametres, v2.40.0) et une
'   ligne par paramètre, avec le champ adapté au type de valeur (type_valeur)
' - Consultation lisible par défaut ; « Modifier » ouvre tous les champs de la page ;
'   « Enregistrer » écrit en une seule transaction les valeurs changées (GestionParametres.
'   UpdateValeursParametres) ; « Annuler » recharge la page
' - Valider les saisies : type de valeur (UtilsValidation), bornes des nombres (champ borné),
'   forme et existence des chemins (type PATH, bouton Parcourir, création proposée), dossier des
'   sauvegardes hors du dossier de données (GestionSauvegardes)
' - Après un enregistrement, rappeler la portée des nouvelles valeurs (AvertirPortee)
' - Afficher les paramètres désactivés en option (chkAfficherInactifs)
' - Bloquer la navigation tant que des modifications ne sont pas enregistrées (IUnsavedChangesAware)
' - Admin : carte « Licences — propre à ce poste » (clé Syncfusion), enregistrée par « Enregistrer »
'   avec le reste mais rangée sur le poste (GestionLicences), jamais en base
'
' Important   :
' - La définition d'un paramètre (clé, libellé, groupe, type, bornes, description, activation,
'   ouverture au SuperUser) vient de sa migration ; l'écran n'en modifie que la valeur (ADR-36).
' - Aucun accès direct à la base de données (tout passe par GestionParametres).
' - Un SuperUser ne reçoit que les paramètres qui lui sont ouverts : tout champ affiché est donc
'   modifiable dans le mode courant ; la requête d'écriture revérifie ce droit.
'
' Remarques   :
' - V2.0.0 (16/10/2026, UI-7, maquette « Paramètres — A ») : remplace les onglets par groupe avec
'   grille et panneau de détail (V1.x). La notion de « paramètre sélectionné », à l'origine du
'   bug du 12/10/2026, disparaît avec eux.
' - V2.1.0 (18/10/2026) : carte « Licences » (Admin). La clé est une ligne comme les autres
'   (lecture seule, « Modifier », suivi des modifications), mais hors de _cartes : l'écriture en
'   base ne la voit jamais. Elle n'est jamais affichée : le champ reste vide, une pastille donne
'   l'état (définie, absente, illisible sur ce compte Windows).
' - V2.2.0 (19/10/2026) : modifications non enregistrées — quitter l'écran propose Enregistrer /
'   Quitter sans enregistrer / Rester, Annuler demande « Abandonner / Continuer la saisie », comme
'   partout dans l'application (DialogChoix.DemanderAvantDepart, ConfirmerAbandon ; ADR-43).
' - Les cartes sont construites par le code (les paramètres viennent de la base) : création des
'   contrôles dans ConstruirePage, placement dans MettreEnPage (relancé si la largeur change).
' - La page défile (pnlPage.AutoScroll) ; elle est redessinée entière à chaque défilement (ascenseur,
'   molette, tabulation vers un champ caché), sinon Windows recopie des morceaux de cartes
'   translucides au mauvais endroit.
' - Une valeur n'est écrite que si elle diffère de ce que le champ affichait au chargement : un
'   paramètre non touché n'est jamais réécrit, même stocké dans un autre format.
' - La molette ne modifie pas les nombres (Nombre_MouseWheel) : au-dessus d'un nombre, elle fait
'   défiler la page.
' - Journaux : clés et codes des paramètres seulement, jamais de valeur (un chemin peut contenir
'   un nom de personne).
'
' Dépendances :
' - GestionParametres, ParametreApplication, ModeAccesParametres, ClesParametres
' - GestionSauvegardes (règle du dossier des sauvegardes)
' - GestionLicences, LicenceSyncfusion (clé Syncfusion du poste)
' - UtilsValidation, UtilsControls, UtilsAffichage, UtilsButtons, UITheme
' - Kit : AltheaCarte, AltheaCase, AltheaDate, AltheaPastille, AltheaBouton
' - UserControlContext (contexte UI injecté par Home), DialogChoix, GestionLog
'
' Interface   :
' - IContextAwareUserControl : injection du contexte UI partagé
' - IUnsavedChangesAware     : confirmation avant de quitter avec des modifications en cours
' - V2.3.0 (22/10/2026, audit C.1) : les opérations sensibles reçoivent le ContexteOperation de l'écran (utilisateur + rôle
'   courant), vérifié par le métier (Autorisation).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Globalization

Public Class UC_Parametres
    Implements IContextAwareUserControl
    Implements IUnsavedChangesAware

#Region "Constantes de mise en page"

    ' Règle des fiches (Reference_UI § 1.3) : libellés à x = 14, première ligne à y = 40 sous le
    ' titre de la carte, libellé décalé de 4 px pour s'aligner sur le texte du champ.
    Private Const MARGE_PAGE As Integer = 4
    Private Const ECART_CARTES As Integer = 8
    Private Const X_LIBELLE As Integer = 14
    Private Const Y_PREMIERE_LIGNE As Integer = 40
    Private Const DECALAGE_LIBELLE As Integer = 4
    Private Const ECART_LIBELLE_CHAMP As Integer = 16
    Private Const ECART_LIGNES As Integer = 12
    Private Const ECART_AIDE As Integer = 3
    Private Const ECART_APRES_CHAMP As Integer = 8
    Private Const MARGE_DROITE_CARTE As Integer = 14
    Private Const MARGE_BAS_CARTE As Integer = 10
    Private Const LARGEUR_CHAMP_TEXTE As Integer = 420
    Private Const LARGEUR_CHAMP_NOMBRE As Integer = 90
    Private Const LARGEUR_CHAMP_DATE As Integer = 130
    Private Const LARGEUR_AIDE_MAX As Integer = 640
    Private Const TAILLE_BOUTON_OUTIL As Integer = 30
    Private Const ICONE_PARCOURIR As String = "dossier_24_normal"
    Private Const PAS_MOLETTE As Integer = 40       ' défilement de la page par cran de molette sur un nombre

    ' Carte « Licences » (Admin) : textes de l'écran, la clé elle-même n'est jamais affichée.
    Private Const TITRE_CARTE_LICENCES As String = "Licences — propre à ce poste"
    Private Const LIBELLE_CLE_SYNCFUSION As String = "Clé Syncfusion"
    Private Const AIDE_CLE_SYNCFUSION As String =
        "Sert aux exports PDF et Word de l'éditeur de texte ; sans elle, ces exports portent l'avertissement " &
        "de licence de Syncfusion. Pour la définir ou la remplacer : « Modifier », coller la clé copiée depuis " &
        "le compte Syncfusion, « Enregistrer ». Chiffrée pour ce compte Windows, jamais en base : à saisir sur " &
        "chaque poste."

    Private Shared ReadOnly PADDING_CARTE As New Padding(12, 8, 12, 10)
    Private Shared ReadOnly POLICE_LIBELLE As New Font("Calibri", 10.5F, FontStyle.Bold)
    Private Shared ReadOnly POLICE_CHAMP As New Font("Calibri", 11.0F)
    Private Shared ReadOnly POLICE_AIDE As New Font("Calibri", 9.75F, FontStyle.Italic)
    Private Shared ReadOnly POLICE_BORNES As New Font("Calibri", 9.75F)
    Private Shared ReadOnly POLICE_PASTILLE As New Font("Calibri", 9.0F, FontStyle.Bold)

#End Region

#Region "Types internes"

    ' Une ligne de la page : le paramètre et les contrôles qui le représentent.
    Private NotInheritable Class LigneParametre
        Public Parametre As ParametreApplication
        Public Libelle As Label
        Public Champ As Control                     ' TextBox, NumericUpDown, AltheaCase ou AltheaDate
        Public BoutonParcourir As AltheaBouton      ' type PATH seulement
        Public Bornes As Label                      ' nombres bornés seulement
        Public ReadOnly Pastilles As New List(Of AltheaPastille)
        Public Aide As Label                        ' si le paramètre a une description
        Public ValeurInitiale As String             ' valeur affichée au chargement (format de stockage)
    End Class

    ' Une carte de la page : un groupe de paramètres.
    Private NotInheritable Class CarteGroupe
        Public Carte As AltheaCarte
        Public ReadOnly Lignes As New List(Of LigneParametre)
    End Class

#End Region

#Region "Variables privées"

    Private _context As UserControlContext
    Private _modeAcces As ModeAccesParametres = ModeAccesParametres.SuperUser

    Private _parametres As New List(Of ParametreApplication)
    Private ReadOnly _cartes As New List(Of CarteGroupe)

    Private _enModification As Boolean
    Private _isDirty As Boolean
    Private _chargementEnCours As Boolean

    Private _xChamp As Integer                      ' colonne des champs, commune à toutes les cartes

    ' Carte « Licences » (Admin seulement) : hors de _cartes, l'enregistrement en base l'ignore.
    Private _carteLicences As AltheaCarte
    Private _ligneCleSyncfusion As LigneParametre   ' Parametre = Nothing ; Champ = TextBox masquée
    Private _largeurMiseEnPage As Integer = -1

#End Region

#Region "Constructeurs"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version      : V1.0.0
    ' Date         : 14/05/2026
    '
    ' Rôle         :
    ' Constructeur par défaut requis par le Designer (mode SuperUser).
    ' -------------------------------------------------------------------------------------------------
    Public Sub New()

        InitializeComponent()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version      : V1.0.0
    ' Date         : 14/05/2026
    '
    ' Rôle         :
    ' Initialise l'écran avec le mode d'accès voulu (Admin ou SuperUser) ; utilisé par Home.
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(modeAcces As ModeAccesParametres)

        InitializeComponent()

        _modeAcces = modeAcces

    End Sub

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetContext
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Reçoit le contexte UI partagé de Home (barre d'état, infobulles, erreurs de saisie).
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) _
        Implements IContextAwareUserControl.SetContext

        _context = context

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InitializeToolTips
    ' Version    : V2.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Infobulles des contrôles fixes de l'écran ; celles des champs sont posées à leur création.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitializeToolTips()

        If _context Is Nothing Then Return

        _context.SetToolTip(chkAfficherInactifs, "Afficher aussi les paramètres désactivés (sans effet tant qu'ils le restent).")
        _context.SetToolTip(btnModifier, "Ouvrir tous les paramètres de la page à la modification.")
        _context.SetToolTip(btnEnregistrer, "Enregistrer toutes les valeurs modifiées.")
        _context.SetToolTip(btnAnnuler, "Abandonner les modifications et revenir aux valeurs enregistrées.")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PeutQuitter
    ' Version    : V1.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Appelée par la navigation avant de quitter l'écran : confirmation si des valeurs modifiées
    ' ne sont pas enregistrées.
    ' -------------------------------------------------------------------------------------------------
    Public Function PeutQuitter() As Boolean Implements IUnsavedChangesAware.PeutQuitter

        If Not _isDirty Then Return True

        ' Même question que les autres écrans (19/10/2026) : « Enregistrer » passe par le bouton, qui
        ' valide la saisie ; l'enregistrement réussi repasse en consultation et efface l'état modifié.
        Select Case DialogChoix.DemanderAvantDepart("des paramètres")
            Case ChoixAvantDepart.Enregistrer
                btnEnregistrer.PerformClick()
                Return Not _isDirty
            Case ChoixAvantDepart.QuitterSansEnregistrer
                Return True
            Case Else
                Return False
        End Select

    End Function

#End Region

#Region "Initialisation et chargement"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UC_Parametres_Load
    ' Version    : V2.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Prépare les boutons et les infobulles, puis charge la page en consultation.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_Parametres_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        UtilsButtons.InitStandardButton(btnModifier)
        UtilsButtons.InitStandardButton(btnEnregistrer)
        UtilsButtons.InitStandardButton(btnAnnuler)

        InitializeToolTips()

        GestionLog.EcrireLog("UI: ouverture de l'écran des paramètres applicatifs.",
                             GestionLog.LogLevel.Rapide, GestionLog.LogCategory.UI)

        ChargerParametres()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ChargerParametres
    ' Version    : V2.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Lit les paramètres depuis la couche métier et reconstruit la page, en consultation.
    '
    ' Remarques  :
    ' - Sert aussi après Enregistrer et Annuler : la page montre toujours les valeurs en base.
    ' - En cas d'erreur de lecture : message à la place des cartes, détail dans le journal.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerParametres()

        _context?.ClearAllErrors()

        Try
            _parametres = GestionParametres.GetParametres(_modeAcces, chkAfficherInactifs.Checked)
            ConstruirePage()
            UpdateContextInfo(If(chkAfficherInactifs.Checked,
                                 "Paramètres actifs et désactivés chargés.",
                                 "Paramètres chargés."))

        Catch ex As Exception
            _parametres = New List(Of ParametreApplication)
            ViderPage()
            AfficherMessagePage("Les paramètres n'ont pas pu être chargés. Le détail a été noté dans le journal.",
                                UITheme.ColorRoseFonce)
            UpdateContextInfo("Erreur lors du chargement des paramètres.")
            GestionLog.EcrireLog("UI: erreur lors du chargement des paramètres applicatifs.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
        End Try

        AppliquerMode(False)

    End Sub

#End Region

#Region "Construction de la page"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ConstruirePage
    ' Version    : V1.1.0
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Crée une carte par groupe (ordre et titre du groupe) et une ligne par paramètre, puis les place ;
    ' en mode Admin, ajoute la carte « Licences ».
    '
    ' Remarques  :
    ' - Affichage gelé pendant la construction : la page apparaît d'un coup (UtilsAffichage).
    ' - _chargementEnCours empêche les valeurs posées ici de compter comme des modifications.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ConstruirePage()

        _chargementEnCours = True
        UtilsAffichage.SuspendreDessin(pnlPage)
        pnlPage.SuspendLayout()

        Try
            ViderPage()

            If _parametres.Count = 0 Then
                AfficherMessagePage("Aucun paramètre à afficher.", UITheme.ColorGrisDiscret)
                Return
            End If

            Dim groupes = _parametres.
                GroupBy(Function(p) New With {Key p.OrdreGroupe, Key p.GroupeParametre, Key p.LibelleGroupe}).
                OrderBy(Function(g) g.Key.OrdreGroupe).
                ThenBy(Function(g) g.Key.GroupeParametre, StringComparer.Ordinal)

            For Each groupe In groupes

                Dim carteGroupe As New CarteGroupe With {
                    .Carte = New AltheaCarte With {
                        .Titre = groupe.Key.LibelleGroupe,
                        .Padding = PADDING_CARTE,
                        .TabStop = False
                    }
                }

                For Each parametre In groupe.OrderBy(Function(p) p.OrdreAffichage).ThenBy(Function(p) p.LibelleParametre)
                    carteGroupe.Lignes.Add(CreerLigne(carteGroupe.Carte, parametre))
                Next

                pnlPage.Controls.Add(carteGroupe.Carte)
                _cartes.Add(carteGroupe)

            Next

            If _modeAcces = ModeAccesParametres.Admin Then ConstruireCarteLicences()

            _xChamp = CalculerColonneChamps()
            _largeurMiseEnPage = -1
            MettreEnPage()

        Finally
            pnlPage.ResumeLayout(True)
            UtilsAffichage.ReprendreDessin(pnlPage)
            _chargementEnCours = False
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ConstruireCarteLicences
    ' Version    : V1.0.0
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Crée la carte « Licences — propre à ce poste » (Admin) : la ligne de la clé Syncfusion, sa
    ' pastille d'état et son aide.
    '
    ' Remarques  :
    ' - Le champ reste vide : la clé enregistrée n'est jamais relue pour être affichée ; une saisie
    '   la remplace à l'enregistrement de la page.
    ' - Positions calculées ensuite par MettreEnPage, comme les autres lignes.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ConstruireCarteLicences()

        _carteLicences = New AltheaCarte With {
            .Titre = TITRE_CARTE_LICENCES,
            .Padding = PADDING_CARTE,
            .TabStop = False
        }

        Dim ligne As New LigneParametre With {.ValeurInitiale = String.Empty}

        ligne.Libelle = New Label With {
            .AutoSize = True,
            .BackColor = Color.Transparent,
            .Font = POLICE_LIBELLE,
            .ForeColor = UITheme.ColorSaugeFonce,
            .Text = LIBELLE_CLE_SYNCFUSION,
            .UseMnemonic = False
        }
        _carteLicences.Controls.Add(ligne.Libelle)

        Dim zone As New TextBox With {
            .Font = POLICE_CHAMP,
            .ForeColor = UITheme.ColorGrisFonce,
            .PlaceholderText = "Coller ici la nouvelle clé",
            .UseSystemPasswordChar = True,
            .Width = LARGEUR_CHAMP_TEXTE
        }
        AddHandler zone.TextChanged, AddressOf Champ_Modifie
        AddHandler zone.Enter, AddressOf Champ_Enter
        ligne.Champ = zone
        _carteLicences.Controls.Add(zone)
        _context?.SetToolTip(zone, "La clé saisie reste masquée ; elle ne sera jamais réaffichée.")

        Dim etat = EtatCleSyncfusion()
        ligne.Pastilles.Add(CreerPastille(_carteLicences, etat.Texte, etat.Fond, etat.CouleurTexte, etat.Infobulle))

        ligne.Aide = New Label With {
            .AutoSize = False,
            .BackColor = Color.Transparent,
            .Font = POLICE_AIDE,
            .ForeColor = UITheme.ColorGrisDiscret,
            .Text = AIDE_CLE_SYNCFUSION,
            .UseMnemonic = False
        }
        _carteLicences.Controls.Add(ligne.Aide)

        _ligneCleSyncfusion = ligne
        pnlPage.Controls.Add(_carteLicences)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : EtatCleSyncfusion
    ' Rôle       : Texte, couleurs et infobulle de la pastille d'état de la clé Syncfusion.
    ' Remarques  : Lue comme au démarrage : une clé chiffrée par un autre compte Windows est
    '              « illisible » (elle ne sert pas). Jamais d'exception : l'écran doit s'ouvrir.
    ' -------------------------------------------------------------------------------------------------
    Private Function EtatCleSyncfusion() As (Texte As String, Fond As Color, CouleurTexte As Color, Infobulle As String)

        Try
            If String.IsNullOrEmpty(LicenceSyncfusion.Obtenir()) Then
                Return ("Absente", UITheme.StatutPauseBack, UITheme.StatutPauseFore,
                        "Aucune clé sur ce poste : les exports PDF et Word portent l'avertissement de licence.")
            End If

            Dim quand As DateTime? = LicenceSyncfusion.DateDefinition()
            Return (If(quand.HasValue, $"Définie le {quand.Value:dd/MM/yyyy}", "Définie"),
                    UITheme.ColorGrisVertClair, UITheme.ColorSaugeTresFonce,
                    "Clé enregistrée sur ce poste, pour ce compte Windows.")

        Catch ex As Exception
            GestionLog.EcrireLog("UI: clé de licence Syncfusion illisible sur ce compte Windows.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            Return ("Illisible", UITheme.StatutPauseBack, UITheme.StatutPauseFore,
                    "La clé a été enregistrée par un autre compte Windows : saisissez-la à nouveau.")
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ViderPage
    ' Rôle       : Retire et libère les cartes (et le message éventuel) de la page, avec leurs
    '              infobulles et l'icône des boutons Parcourir.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ViderPage()

        pnlPage.AutoScrollPosition = Point.Empty
        pnlPage.AutoScrollMinSize = Size.Empty

        ' L'infobulle partagée de Home garde une référence à chaque contrôle : on la retire avant de
        ' détruire les contrôles, sinon ils restent en mémoire à chaque rechargement.
        For Each carteGroupe In _cartes
            For Each enfant As Control In carteGroupe.Carte.Controls
                _context?.SetToolTip(enfant, Nothing)
            Next
            For Each ligne In carteGroupe.Lignes
                ligne.BoutonParcourir?.Image?.Dispose()
            Next
        Next
        If _carteLicences IsNot Nothing Then
            For Each enfant As Control In _carteLicences.Controls
                _context?.SetToolTip(enfant, Nothing)
            Next
        End If

        For Each ctrl As Control In pnlPage.Controls.Cast(Of Control)().ToList()
            ctrl.Dispose()
        Next
        pnlPage.Controls.Clear()

        _cartes.Clear()
        _carteLicences = Nothing
        _ligneCleSyncfusion = Nothing

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AfficherMessagePage
    ' Rôle       : Message à la place des cartes (aucun paramètre, erreur de chargement).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherMessagePage(message As String, couleur As Color)

        Dim lblMessage As New Label With {
            .AutoSize = True,
            .BackColor = Color.Transparent,
            .Font = POLICE_CHAMP,
            .ForeColor = couleur,
            .Location = New Point(MARGE_PAGE + X_LIBELLE, MARGE_PAGE + 8),
            .Text = message,
            .UseMnemonic = False
        }
        pnlPage.Controls.Add(lblMessage)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CreerLigne
    ' Version    : V1.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Crée les contrôles d'un paramètre dans sa carte : libellé, champ selon le type, bouton
    ' Parcourir (PATH), bornes (nombres), pastilles (Admin, Désactivé) et aide (description).
    '
    ' Remarques  :
    ' - Les positions sont calculées ensuite par MettreEnPage.
    ' - En mode Admin, l'infobulle du libellé donne la clé technique (maintenance).
    ' -------------------------------------------------------------------------------------------------
    Private Function CreerLigne(carte As AltheaCarte, parametre As ParametreApplication) As LigneParametre

        Dim ligne As New LigneParametre With {.Parametre = parametre}

        ligne.Libelle = New Label With {
            .AutoSize = True,
            .BackColor = Color.Transparent,
            .Font = POLICE_LIBELLE,
            .ForeColor = UITheme.ColorSaugeFonce,
            .Text = parametre.LibelleParametre,
            .UseMnemonic = False
        }
        carte.Controls.Add(ligne.Libelle)
        If _modeAcces = ModeAccesParametres.Admin Then
            _context?.SetToolTip(ligne.Libelle, $"Clé technique : {parametre.CleParametre}")
        End If

        ligne.Champ = CreerChamp(parametre)
        carte.Controls.Add(ligne.Champ)
        ' Référence de comparaison à l'enregistrement : la valeur telle que le champ l'affiche, et non
        ' la valeur brute en base (format différent, nombre ramené dans ses bornes, date illisible…),
        ' pour ne jamais réécrire un paramètre que l'utilisateur n'a pas touché.
        ligne.ValeurInitiale = LireValeur(ligne)
        AddHandler ligne.Champ.Enter, AddressOf Champ_Enter

        If TypeValeur(parametre) = "PATH" Then
            ligne.BoutonParcourir = CreerBoutonParcourir(parametre, DirectCast(ligne.Champ, TextBox))
            carte.Controls.Add(ligne.BoutonParcourir)
        End If

        Dim texteBornes As String = TexteDesBornes(parametre)
        If texteBornes IsNot Nothing Then
            ligne.Bornes = New Label With {
                .AutoSize = True,
                .BackColor = Color.Transparent,
                .Font = POLICE_BORNES,
                .ForeColor = UITheme.ColorGrisMoyenFonce,
                .Text = texteBornes,
                .UseMnemonic = False
            }
            carte.Controls.Add(ligne.Bornes)
        End If

        If _modeAcces = ModeAccesParametres.Admin AndAlso Not parametre.ModifiableUtilisateur Then
            ligne.Pastilles.Add(CreerPastille(carte, "Admin", UITheme.ColorMauveTresClair, UITheme.ColorMauveFonce,
                                              "Réservé à l'administrateur : le SuperUser ne voit pas ce paramètre."))
        End If
        If Not parametre.Actif Then
            ligne.Pastilles.Add(CreerPastille(carte, "Désactivé", UITheme.ColorGrisClair, UITheme.ColorGrisDiscret,
                                              "Paramètre désactivé : l'application utilise sa valeur de secours."))
        End If

        If Not String.IsNullOrWhiteSpace(parametre.DescriptionParametre) Then
            ligne.Aide = New Label With {
                .AutoSize = False,
                .BackColor = Color.Transparent,
                .Font = POLICE_AIDE,
                .ForeColor = UITheme.ColorGrisDiscret,
                .Text = parametre.DescriptionParametre.Trim(),
                .UseMnemonic = False
            }
            carte.Controls.Add(ligne.Aide)
        End If

        Return ligne

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CreerChamp
    ' Version    : V1.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Crée le champ de saisie adapté au type de valeur, rempli avec la valeur enregistrée, et
    ' branché sur le suivi des modifications.
    '
    ' Remarques  :
    ' - BOOL : case du kit ; INT / DECIMAL : nombre borné (valeur_min / valeur_max) ; DATE : champ
    '   de date du kit ; PATH et autres types : zone de texte.
    ' - Un nombre enregistré hors bornes est affiché ramené dans les bornes (comme à la lecture par
    '   GestionParametres) ; il n'est réécrit que si l'utilisateur change ce champ.
    ' -------------------------------------------------------------------------------------------------
    Private Function CreerChamp(parametre As ParametreApplication) As Control

        Dim valeur As String = If(parametre.ValeurParametre, String.Empty)

        Select Case TypeValeur(parametre)

            Case "BOOL"
                Dim caseValeur As New AltheaCase With {
                    .AutoSize = True,
                    .Checked = valeur = "1" OrElse valeur.Equals("TRUE", StringComparison.OrdinalIgnoreCase),
                    .Text = String.Empty
                }
                AddHandler caseValeur.CheckedChanged, AddressOf Champ_Modifie
                Return caseValeur

            Case "INT", "DECIMAL"
                Dim decimales As Integer = If(TypeValeur(parametre) = "DECIMAL", 2, 0)
                Dim nombre As New NumericUpDown With {
                    .DecimalPlaces = decimales,
                    .Font = POLICE_CHAMP,
                    .ForeColor = UITheme.ColorGrisFonce,
                    .Maximum = 1000000000D,
                    .Minimum = -1000000000D,
                    .TextAlign = HorizontalAlignment.Right,
                    .Width = LARGEUR_CHAMP_NOMBRE
                }
                AppliquerBornes(nombre, parametre)
                AddHandler nombre.MouseWheel, AddressOf Nombre_MouseWheel
                Dim lu As Decimal
                If Decimal.TryParse(valeur, NumberStyles.Number, CultureInfo.InvariantCulture, lu) Then
                    nombre.Value = Math.Max(nombre.Minimum, Math.Min(nombre.Maximum, lu))
                End If
                AddHandler nombre.ValueChanged, AddressOf Champ_Modifie
                ' ValueChanged n'arrive qu'en quittant le champ : la frappe est suivie par TextChanged.
                AddHandler nombre.TextChanged, AddressOf Champ_Modifie
                Return nombre

            Case "DATE"
                Dim champDate As New AltheaDate With {
                    .Facultative = True,
                    .Font = POLICE_CHAMP,
                    .Width = LARGEUR_CHAMP_DATE
                }
                Dim lue As Date
                If Date.TryParseExact(valeur, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, lue) Then
                    champDate.Valeur = lue
                End If
                AddHandler champDate.ValeurChangee, AddressOf Champ_Modifie
                Return champDate

            Case Else
                Dim zone As New TextBox With {
                    .Font = POLICE_CHAMP,
                    .ForeColor = UITheme.ColorGrisFonce,
                    .Text = valeur,
                    .Width = LARGEUR_CHAMP_TEXTE
                }
                AddHandler zone.TextChanged, AddressOf Champ_Modifie
                Return zone

        End Select

    End Function

    ' La molette ne change jamais un nombre : en faisant défiler la page, la souris passe sur les
    ' champs, et un NumericUpDown réagit à la molette même en consultation (ReadOnly).
    ' Le cran de molette, intercepté, fait défiler la page à la place.
    Private Sub Nombre_MouseWheel(sender As Object, e As MouseEventArgs)

        Dim evenement = TryCast(e, HandledMouseEventArgs)
        If evenement Is Nothing Then Return
        evenement.Handled = True

        Dim position As Point = pnlPage.AutoScrollPosition
        pnlPage.AutoScrollPosition = New Point(-position.X, -position.Y - (e.Delta \ 120) * PAS_MOLETTE)
        pnlPage.Invalidate(True)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CreerBoutonParcourir
    ' Version    : V1.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Petit bouton Outil (icône dossier) à droite d'un chemin : choix du dossier dans Windows.
    ' Visible seulement en modification.
    ' -------------------------------------------------------------------------------------------------
    Private Function CreerBoutonParcourir(parametre As ParametreApplication, zone As TextBox) As AltheaBouton

        Dim bouton As New AltheaBouton With {
            .Image = UtilsButtons.LoadImageFromAsset(UITheme.AssetsStandardButtonDiversPath, ICONE_PARCOURIR),
            .Size = New Size(TAILLE_BOUTON_OUTIL, TAILLE_BOUTON_OUTIL),
            .Variante = VarianteBouton.Outil,
            .Visible = False
        }

        AddHandler bouton.Click,
            Sub()
                Using dlg As New FolderBrowserDialog With {
                    .Description = parametre.LibelleParametre,
                    .UseDescriptionForTitle = True,
                    .ShowNewFolderButton = True
                }
                    If IO.Directory.Exists(zone.Text.Trim()) Then dlg.InitialDirectory = zone.Text.Trim()
                    If dlg.ShowDialog(FindForm()) = DialogResult.OK Then zone.Text = dlg.SelectedPath
                End Using
            End Sub

        _context?.SetToolTip(bouton, "Choisir le dossier…")

        Return bouton

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CreerPastille
    ' Rôle       : Petite étiquette à droite du champ (« Admin », « Désactivé »), avec son infobulle.
    ' -------------------------------------------------------------------------------------------------
    Private Function CreerPastille(carte As AltheaCarte, texte As String, fond As Color, couleurTexte As Color,
                                   infobulle As String) As AltheaPastille

        Dim pastille As New AltheaPastille With {
            .CouleurFond = fond,
            .Font = POLICE_PASTILLE,
            .ForeColor = couleurTexte,
            .Text = texte
        }
        carte.Controls.Add(pastille)
        _context?.SetToolTip(pastille, infobulle)

        Return pastille

    End Function

#End Region

#Region "Mise en page"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CalculerColonneChamps
    ' Rôle       : Colonne des champs commune à toutes les cartes : après le libellé le plus long.
    ' Remarques  : Les libellés viennent de la base : la colonne s'adapte au lieu d'être fixée.
    ' -------------------------------------------------------------------------------------------------
    Private Function CalculerColonneChamps() As Integer

        Dim largeurMax As Integer = 0
        For Each carteGroupe In _cartes
            For Each ligne In carteGroupe.Lignes
                largeurMax = Math.Max(largeurMax, TextRenderer.MeasureText(ligne.Libelle.Text, POLICE_LIBELLE).Width)
            Next
        Next
        If _ligneCleSyncfusion IsNot Nothing Then
            largeurMax = Math.Max(largeurMax, TextRenderer.MeasureText(_ligneCleSyncfusion.Libelle.Text, POLICE_LIBELLE).Width)
        End If

        Return X_LIBELLE + largeurMax + ECART_LIBELLE_CHAMP

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : MettreEnPage
    ' Version    : V1.1.0
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Place les cartes l'une sous l'autre sur toute la largeur de la page, et dans chaque carte une
    ' ligne par paramètre : libellé, champ, bouton, bornes, pastilles, puis l'aide sous le champ.
    '
    ' Remarques  :
    ' - Relancée quand la largeur de la page change (l'aide se replie autrement).
    ' - La largeur de l'ascenseur est toujours réservée : la page ne bouge pas quand il apparaît.
    ' -------------------------------------------------------------------------------------------------
    Private Sub MettreEnPage()

        If _cartes.Count = 0 Then Return

        Dim largeurCarte As Integer = pnlPage.ClientSize.Width - 2 * MARGE_PAGE
        If pnlPage.VerticalScroll.Visible = False Then largeurCarte -= SystemInformation.VerticalScrollBarWidth
        largeurCarte = Math.Max(largeurCarte, _xChamp + LARGEUR_CHAMP_TEXTE + MARGE_DROITE_CARTE)

        If largeurCarte = _largeurMiseEnPage Then Return
        _largeurMiseEnPage = largeurCarte

        Dim origine As Point = pnlPage.AutoScrollPosition
        Dim y As Integer = MARGE_PAGE

        For Each carteGroupe In _cartes

            Dim yLigne As Integer = Y_PREMIERE_LIGNE
            Dim largeurAide As Integer = Math.Min(LARGEUR_AIDE_MAX, largeurCarte - _xChamp - MARGE_DROITE_CARTE)

            For Each ligne In carteGroupe.Lignes
                yLigne = PlacerLigne(ligne, yLigne, largeurAide) + ECART_LIGNES
            Next

            Dim hauteurCarte As Integer = yLigne - ECART_LIGNES + MARGE_BAS_CARTE + PADDING_CARTE.Bottom
            carteGroupe.Carte.SetBounds(MARGE_PAGE + origine.X, y + origine.Y, largeurCarte, hauteurCarte)
            y += hauteurCarte + ECART_CARTES

        Next

        ' Carte « Licences » en dernier, même règle de placement que les autres.
        If _carteLicences IsNot Nothing Then
            Dim largeurAide As Integer = Math.Min(LARGEUR_AIDE_MAX, largeurCarte - _xChamp - MARGE_DROITE_CARTE)
            Dim basLigne As Integer = PlacerLigne(_ligneCleSyncfusion, Y_PREMIERE_LIGNE, largeurAide)
            Dim hauteurCarte As Integer = basLigne + MARGE_BAS_CARTE + PADDING_CARTE.Bottom
            _carteLicences.SetBounds(MARGE_PAGE + origine.X, y + origine.Y, largeurCarte, hauteurCarte)
            y += hauteurCarte + ECART_CARTES
        End If

        pnlPage.AutoScrollMinSize = New Size(0, y)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PlacerLigne
    ' Rôle       : Place les contrôles d'une ligne à partir de yLigne ; retourne le bas de la ligne.
    ' -------------------------------------------------------------------------------------------------
    Private Function PlacerLigne(ligne As LigneParametre, yLigne As Integer, largeurAide As Integer) As Integer

        ligne.Libelle.Location = New Point(X_LIBELLE, yLigne + DECALAGE_LIBELLE)

        ' Une case n'a pas la hauteur d'une zone de texte : elle s'aligne sur le libellé.
        Dim decalageChamp As Integer = If(TypeOf ligne.Champ Is CheckBox, DECALAGE_LIBELLE - 1, 0)
        ligne.Champ.Location = New Point(_xChamp, yLigne + decalageChamp)

        Dim x As Integer = ligne.Champ.Right + ECART_APRES_CHAMP
        Dim basLigne As Integer = Math.Max(ligne.Champ.Bottom, ligne.Libelle.Bottom)

        If ligne.BoutonParcourir IsNot Nothing Then
            ligne.BoutonParcourir.Location = New Point(ligne.Champ.Right + 4,
                                                       yLigne + (ligne.Champ.Height - TAILLE_BOUTON_OUTIL) \ 2)
            ' Place réservée même en consultation (bouton masqué) : les pastilles ne bougent pas.
            x = ligne.BoutonParcourir.Right + ECART_APRES_CHAMP
        End If

        If ligne.Bornes IsNot Nothing Then
            ligne.Bornes.Location = New Point(x, yLigne + (ligne.Champ.Height - ligne.Bornes.Height) \ 2)
            x = ligne.Bornes.Right + ECART_APRES_CHAMP
        End If

        For Each pastille In ligne.Pastilles
            pastille.Location = New Point(x, yLigne + (ligne.Champ.Height - pastille.Height) \ 2)
            x = pastille.Right + 6
        Next

        If ligne.Aide IsNot Nothing Then
            Dim taille As Size = TextRenderer.MeasureText(ligne.Aide.Text, POLICE_AIDE,
                                                          New Size(largeurAide, Integer.MaxValue),
                                                          TextFormatFlags.WordBreak)
            ligne.Aide.SetBounds(_xChamp, basLigne + ECART_AIDE, largeurAide, taille.Height)
            basLigne = ligne.Aide.Bottom
        End If

        Return basLigne

    End Function

    Private Sub pnlPage_Resize(sender As Object, e As EventArgs) Handles pnlPage.Resize
        MettreEnPage()
    End Sub

    ' Redessin complet au défilement : sans lui, Windows recopie des morceaux des cartes
    ' translucides (elles peignent le fond de l'écran derrière elles) au mauvais endroit.
    Private Sub pnlPage_Scroll(sender As Object, e As ScrollEventArgs) Handles pnlPage.Scroll
        pnlPage.Invalidate(True)
    End Sub

    Private Sub pnlPage_MouseWheel(sender As Object, e As MouseEventArgs) Handles pnlPage.MouseWheel
        pnlPage.Invalidate(True)
    End Sub

    ' La tabulation vers un champ hors de l'écran fait défiler la page sans événement Scroll.
    Private Sub Champ_Enter(sender As Object, e As EventArgs)
        pnlPage.Invalidate(True)
    End Sub

#End Region

#Region "Modes et boutons"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AppliquerMode
    ' Version    : V2.1.0
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Bascule toute la page entre consultation (champs lisibles, fond teinté) et modification
    ' (champs ouverts, boutons Parcourir visibles), et met les boutons d'action à jour.
    '
    ' Remarques  :
    ' - « Afficher les paramètres désactivés » est indisponible pendant la modification : recharger
    '   la page ferait perdre la saisie en cours.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerMode(edition As Boolean)

        _enModification = edition
        If Not edition Then _isDirty = False

        UtilsControls.DefinirLectureSeule(pnlPage, Not edition)

        ' Le champ de la clé n'a rien à montrer en consultation (la clé n'est jamais affichée) :
        ' masqué, sa place reste réservée et la pastille d'état ne bouge pas.
        If _ligneCleSyncfusion IsNot Nothing Then _ligneCleSyncfusion.Champ.Visible = edition

        For Each carteGroupe In _cartes
            For Each ligne In carteGroupe.Lignes
                If ligne.BoutonParcourir IsNot Nothing Then
                    ligne.BoutonParcourir.Visible = edition
                    ligne.BoutonParcourir.Enabled = edition
                End If
            Next
        Next

        chkAfficherInactifs.Enabled = Not edition
        UpdateButtonsState()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateButtonsState
    ' Rôle       : Modifier en consultation (s'il y a des paramètres) ; Enregistrer si une valeur a
    '              changé ; Annuler pendant la modification.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UpdateButtonsState()

        btnModifier.Enabled = Not _enModification AndAlso _cartes.Count > 0
        btnEnregistrer.Enabled = _enModification AndAlso _isDirty
        btnAnnuler.Enabled = _enModification

    End Sub

    ' Suivi des modifications : tout champ de la page signale ici un changement de valeur.
    Private Sub Champ_Modifie(sender As Object, e As EventArgs)

        If _chargementEnCours OrElse Not _enModification Then Return

        _context?.ClearError(TryCast(sender, Control))
        If Not _isDirty Then
            _isDirty = True
            UpdateButtonsState()
        End If

    End Sub

    Private Sub btnModifier_Click(sender As Object, e As EventArgs) Handles btnModifier.Click

        AppliquerMode(True)
        UpdateContextInfo("Modification des paramètres en cours.")
        GestionLog.EcrireLog("UI: modification des paramètres ouverte.",
                             GestionLog.LogLevel.Rapide, GestionLog.LogCategory.UI)

    End Sub

    Private Sub btnAnnuler_Click(sender As Object, e As EventArgs) Handles btnAnnuler.Click

        If _isDirty AndAlso Not DialogChoix.ConfirmerAbandon("des paramètres") Then
            Return
        End If

        ChargerParametres()
        UpdateContextInfo("Modifications annulées.")

    End Sub

    Private Sub chkAfficherInactifs_CheckedChanged(sender As Object, e As EventArgs) Handles chkAfficherInactifs.CheckedChanged

        GestionLog.EcrireLog($"UI: affichage des paramètres désactivés = {chkAfficherInactifs.Checked}.",
                             GestionLog.LogLevel.Rapide, GestionLog.LogCategory.UI)
        ChargerParametres()

    End Sub

#End Region

#Region "Enregistrement"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnEnregistrer_Click
    ' Version    : V2.1.0
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Valide les valeurs changées, puis les enregistre toutes ensemble (une transaction) ; recharge
    ' la page en consultation et rappelle la portée des nouvelles valeurs.
    '
    ' Remarques  :
    ' - Seuls les paramètres dont la valeur a changé (par rapport à l'affichage initial) sont écrits.
    ' - Au premier refus de validation, le champ est signalé, montré à l'écran, et rien n'est écrit.
    ' - Les dossiers absents ne sont proposés à la création qu'une fois toute la page validée.
    ' - Refus de la base (paramètre fermé ou disparu) : rien n'est écrit (tout ou rien).
    ' - Clé de licence (V2.1.0) : vérifiée avant toute écriture, enregistrée sur le poste après les
    '   paramètres, et seulement s'ils ont été acceptés.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEnregistrer_Click(sender As Object, e As EventArgs) Handles btnEnregistrer.Click

        _context?.ClearAllErrors()

        Dim modifies As New List(Of ParametreApplication)
        Dim dossiersACreer As New List(Of LigneParametre)

        ' 1. Validation de toutes les valeurs changées, sans rien écrire ni créer.
        For Each ligne In _cartes.SelectMany(Function(c) c.Lignes)

            Dim nouvelleValeur As String = LireValeur(ligne)
            If String.Equals(nouvelleValeur, ligne.ValeurInitiale, StringComparison.Ordinal) Then Continue For

            If Not ValiderValeur(ligne, nouvelleValeur) Then Return

            If TypeValeur(ligne.Parametre) = "PATH" AndAlso nouvelleValeur.Length > 0 AndAlso
               Not IO.Directory.Exists(nouvelleValeur) Then
                dossiersACreer.Add(ligne)
            End If

            modifies.Add(CopieAvecValeur(ligne.Parametre, nouvelleValeur))

        Next

        ' 2. Dossiers absents : création proposée seulement quand toute la page est valable.
        For Each ligne In dossiersACreer
            If Not CreerDossierPropose(ligne, LireValeur(ligne)) Then Return
        Next

        ' 3. Clé de licence (Admin) : vérifiée avec le reste, écrite seulement après les paramètres.
        Dim cleSaisie As String = If(_ligneCleSyncfusion Is Nothing, String.Empty, _ligneCleSyncfusion.Champ.Text)
        Dim cleModifiee As Boolean = Not String.IsNullOrWhiteSpace(cleSaisie)

        If cleModifiee AndAlso Not GestionLicences.CleSyncfusionValide(cleSaisie) Then
            MontrerLigne(_ligneCleSyncfusion)
            _context?.SetError(_ligneCleSyncfusion.Champ, "Clé incomplète ou mal copiée.")
            UpdateContextInfo("Enregistrement refusé : corrige la valeur signalée.")
            DialogChoix.Avertissement($"« {LIBELLE_CLE_SYNCFUSION} » : ce texte ne ressemble pas à une clé de licence. " &
                                      "Copiez-la en entier depuis le compte Syncfusion." &
                                      Environment.NewLine & Environment.NewLine & "Rien n'a été enregistré.",
                                      "Althéa - Validation")
            Return
        End If

        If modifies.Count = 0 AndAlso Not cleModifiee Then
            AppliquerMode(False)
            UpdateContextInfo("Aucune valeur n'a changé.")
            Return
        End If

        If modifies.Count > 0 Then

            UpdateContextInfo("Enregistrement en cours…")
            Dim cles As String = String.Join(", ", modifies.Select(Function(p) p.CleParametre))

            Try
                If Not GestionParametres.UpdateValeursParametres(modifies, _modeAcces) Then
                    UpdateContextInfo("Enregistrement refusé.")
                    DialogChoix.Avertissement(
                        "Les paramètres n'ont pas été enregistrés : l'un d'eux est réservé à l'administrateur " &
                        "ou n'existe plus. Aucune valeur n'a été modifiée.",
                        "Althéa")
                    Return
                End If

            Catch ex As Exception
                UpdateContextInfo("Erreur lors de l'enregistrement des paramètres.")
                GestionLog.EcrireLog($"UI: erreur lors de l'enregistrement des paramètres ({cles}).",
                                     GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
                DialogChoix.Erreur("Les paramètres n'ont pas pu être enregistrés ; aucune valeur n'a été modifiée. " &
                                   "Le détail a été noté dans le journal.", "Althéa")
                Return
            End Try

            GestionLog.EcrireLog($"UI: paramètres enregistrés ({cles}).",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)

        End If

        Dim cleEnregistree As Boolean = cleModifiee AndAlso EnregistrerCleSyncfusion(cleSaisie)

        ChargerParametres()

        Dim bilan As New List(Of String)
        If modifies.Count > 0 Then bilan.Add(If(modifies.Count = 1, "Paramètre enregistré.", $"{modifies.Count} paramètres enregistrés."))
        If cleEnregistree Then bilan.Add("Clé de licence enregistrée sur ce poste.")
        If bilan.Count > 0 Then UpdateContextInfo(String.Join(" ", bilan))

        If modifies.Count > 0 Then
            AvertirPortee(modifies)
        ElseIf cleEnregistree Then
            DialogChoix.Succes("Clé de licence Syncfusion enregistrée sur ce poste." & Environment.NewLine & Environment.NewLine &
                               "Les exports PDF et Word l'utilisent dès maintenant. Sur un autre poste ou un autre " &
                               "compte Windows, elle est à saisir à nouveau.",
                               "Althéa - Paramètres")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : EnregistrerCleSyncfusion
    ' Rôle       : Confie la clé saisie à GestionLicences (droits, forme, stockage chiffré) ; True si
    '              elle est enregistrée. Les refus et erreurs sont montrés ici ; la clé n'est jamais
    '              journalisée.
    ' -------------------------------------------------------------------------------------------------
    Private Function EnregistrerCleSyncfusion(cle As String) As Boolean

        Try
            GestionLicences.DefinirCleSyncfusion(_context?.ContexteOperation, cle)
            Return True

        Catch ex As UnauthorizedAccessException
            DialogChoix.Avertissement(ex.Message, "Accès refusé")
        Catch ex As ArgumentException
            DialogChoix.Avertissement(ex.Message, "Althéa - Validation")
        Catch ex As Exception
            GestionLog.EcrireLog("UI: enregistrement de la clé de licence Syncfusion impossible.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security, ex)
            DialogChoix.Erreur("La clé de licence n'a pas pu être enregistrée. Le détail a été noté dans le journal.",
                               "Althéa - Paramètres")
        End Try

        Return False

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : LireValeur
    ' Rôle       : Valeur saisie d'une ligne, au format de stockage (BOOL : 1 / 0 ; nombres : point
    '              décimal ; date : aaaa-mm-jj ; texte et chemins : sans espaces autour).
    ' -------------------------------------------------------------------------------------------------
    Private Function LireValeur(ligne As LigneParametre) As String

        Select Case True

            Case TypeOf ligne.Champ Is CheckBox
                Return If(DirectCast(ligne.Champ, CheckBox).Checked, "1", "0")

            Case TypeOf ligne.Champ Is NumericUpDown
                ' Arrondi et format au nombre de décimales du champ : un Decimal garde l'échelle de sa
                ' source (borne 10.0000 en base) et une saisie « 12,5 » dans un entier s'affiche « 13 ».
                Dim nombre As NumericUpDown = DirectCast(ligne.Champ, NumericUpDown)
                Return Decimal.Round(nombre.Value, nombre.DecimalPlaces, MidpointRounding.AwayFromZero).
                    ToString("F" & nombre.DecimalPlaces.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture)

            Case TypeOf ligne.Champ Is AltheaDate
                Dim valeur As Date? = DirectCast(ligne.Champ, AltheaDate).Valeur
                Return If(valeur.HasValue, valeur.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), String.Empty)

            Case Else
                Return ligne.Champ.Text.Trim()

        End Select

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ValiderValeur
    ' Version    : V2.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Contrôle une valeur changée : type de valeur, puis règles des chemins. En cas de refus, signale
    ' le champ, le montre à l'écran et explique pourquoi.
    ' -------------------------------------------------------------------------------------------------
    Private Function ValiderValeur(ligne As LigneParametre, valeur As String) As Boolean

        Dim messageType As String = String.Empty
        If Not UtilsValidation.IsValidValueForType(valeur, TypeValeur(ligne.Parametre), messageType) Then
            SignalerRefus(ligne, messageType)
            Return False
        End If

        Return VerifierDossierChemin(ligne, valeur)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : VerifierDossierChemin
    ' Version    : V2.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Règles des chemins (type PATH) : racine des données obligatoire ; dossier des sauvegardes
    ' accepté par GestionSauvegardes. L'existence du dossier est traitée à part (CreerDossierPropose).
    '
    ' Remarques  :
    ' - Reprise de la V1 (12/10/2026), appliquée maintenant à chaque chemin modifié de la page.
    ' -------------------------------------------------------------------------------------------------
    Private Function VerifierDossierChemin(ligne As LigneParametre, chemin As String) As Boolean

        Dim parametre As ParametreApplication = ligne.Parametre
        If TypeValeur(parametre) <> "PATH" Then Return True

        If chemin.Length = 0 AndAlso
           String.Equals(parametre.CleParametre, ClesParametres.CheminRacineDonnees, StringComparison.OrdinalIgnoreCase) Then
            SignalerRefus(ligne, "La racine du dossier de données est obligatoire.")
            Return False
        End If

        If chemin.Length > 0 AndAlso
           String.Equals(parametre.CleParametre, ClesParametres.SauvegardeDossier, StringComparison.OrdinalIgnoreCase) Then
            Dim refus As String = GestionSauvegardes.ValiderDossierSauvegarde(chemin)
            If refus IsNot Nothing Then
                SignalerRefus(ligne, refus)
                Return False
            End If
        End If

        Return True

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CreerDossierPropose
    ' Version    : V1.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Dossier d'un chemin modifié absent du disque : propose de le créer. Refus ou échec de la
    ' création = enregistrement arrêté, champ signalé.
    '
    ' Remarques  :
    ' - Appelée une fois toutes les valeurs de la page validées : un dossier n'est jamais créé pour
    '   une page qui serait ensuite refusée.
    ' -------------------------------------------------------------------------------------------------
    Private Function CreerDossierPropose(ligne As LigneParametre, chemin As String) As Boolean

        Dim parametre As ParametreApplication = ligne.Parametre

        MontrerLigne(ligne)
        If DialogChoix.Confirmer($"« {parametre.LibelleParametre} » : le dossier « {chemin} » n'existe pas." &
                                 Environment.NewLine & "Voulez-vous le créer ?",
                                 "Althéa - Dossier introuvable") <> DialogResult.Yes Then
            _context?.SetError(ligne.Champ, "Ce dossier n'existe pas.")
            UpdateContextInfo("Enregistrement arrêté : le dossier n'existe pas.")
            Return False
        End If

        Try
            IO.Directory.CreateDirectory(chemin)
            GestionLog.EcrireLog($"UI: dossier créé pour le paramètre {parametre.CleParametre}.",
                                 GestionLog.LogLevel.Rapide, GestionLog.LogCategory.UI)
            Return True

        Catch ex As Exception
            GestionLog.EcrireLog($"UI: création du dossier impossible pour le paramètre {parametre.CleParametre}.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            _context?.SetError(ligne.Champ, "Ce dossier n'a pas pu être créé.")
            DialogChoix.Erreur("Le dossier n'a pas pu être créé (droits insuffisants, disque absent…). " &
                               "Rien n'a été enregistré ; le détail a été noté dans le journal.",
                               "Althéa - Dossier introuvable")
            Return False
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SignalerRefus
    ' Rôle       : Champ refusé : erreur sur le champ, ligne amenée à l'écran, message expliquant
    '              le refus (avec le nom du paramètre, la page en comptant plusieurs).
    ' -------------------------------------------------------------------------------------------------
    Private Sub SignalerRefus(ligne As LigneParametre, message As String)

        MontrerLigne(ligne)
        _context?.SetError(ligne.Champ, message)
        UpdateContextInfo("Enregistrement refusé : corrige la valeur signalée.")
        GestionLog.EcrireLog($"UI: valeur refusée pour le paramètre {ligne.Parametre.CleParametre}.",
                             GestionLog.LogLevel.Rapide, GestionLog.LogCategory.UI)
        DialogChoix.Avertissement($"« {ligne.Parametre.LibelleParametre} » : {message}" &
                                  Environment.NewLine & Environment.NewLine & "Rien n'a été enregistré.",
                                  "Althéa - Validation")

    End Sub

    ' Amène la ligne à l'écran et place le curseur dans son champ.
    Private Sub MontrerLigne(ligne As LigneParametre)
        pnlPage.ScrollControlIntoView(ligne.Champ)
        pnlPage.Invalidate(True)
        ligne.Champ.Focus()
    End Sub

#End Region

#Region "Utilitaires"

    ' Type de valeur normalisé (MAJUSCULE, sans espaces) ; STRING si absent.
    Private Shared Function TypeValeur(parametre As ParametreApplication) As String
        Return If(parametre.TypeValeur, "STRING").Trim().ToUpperInvariant()
    End Function

    ' Copie du paramètre portant la nouvelle valeur : l'original garde la valeur enregistrée.
    Private Shared Function CopieAvecValeur(parametre As ParametreApplication, valeur As String) As ParametreApplication
        Return New ParametreApplication With {
            .IdParametre = parametre.IdParametre,
            .CodeParametre = parametre.CodeParametre,
            .CleParametre = parametre.CleParametre,
            .LibelleParametre = parametre.LibelleParametre,
            .GroupeParametre = parametre.GroupeParametre,
            .LibelleGroupe = parametre.LibelleGroupe,
            .OrdreGroupe = parametre.OrdreGroupe,
            .TypeValeur = parametre.TypeValeur,
            .ValeurParametre = valeur,
            .ValeurMin = parametre.ValeurMin,
            .ValeurMax = parametre.ValeurMax,
            .DescriptionParametre = parametre.DescriptionParametre,
            .ModifiableUtilisateur = parametre.ModifiableUtilisateur,
            .Actif = parametre.Actif,
            .OrdreAffichage = parametre.OrdreAffichage
        }
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TexteDesBornes
    ' Rôle       : « 10 à 240 », « 5 minimum » ou « 90 maximum » pour un nombre borné ; Nothing sinon.
    ' -------------------------------------------------------------------------------------------------
    Private Shared Function TexteDesBornes(parametre As ParametreApplication) As String

        Dim type As String = TypeValeur(parametre)
        If type <> "INT" AndAlso type <> "DECIMAL" Then Return Nothing

        Dim formatBorne = Function(valeur As Decimal) valeur.ToString("0.##", CultureInfo.CurrentCulture)

        If parametre.ValeurMin.HasValue AndAlso parametre.ValeurMax.HasValue Then
            Return $"{formatBorne(parametre.ValeurMin.Value)} à {formatBorne(parametre.ValeurMax.Value)}"
        ElseIf parametre.ValeurMin.HasValue Then
            Return $"{formatBorne(parametre.ValeurMin.Value)} minimum"
        ElseIf parametre.ValeurMax.HasValue Then
            Return $"{formatBorne(parametre.ValeurMax.Value)} maximum"
        End If

        Return Nothing

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AppliquerBornes
    ' Version    : V1.0.0
    ' Date       : 12/10/2026
    '
    ' Rôle       :
    ' Bornes du champ nombre d'après valeur_min / valeur_max ; bornes incohérentes (min > max)
    ' ignorées plutôt que de bloquer la saisie.
    ' -------------------------------------------------------------------------------------------------
    Private Shared Sub AppliquerBornes(nombre As NumericUpDown, parametre As ParametreApplication)

        If parametre.ValeurMin.HasValue AndAlso parametre.ValeurMax.HasValue AndAlso
           parametre.ValeurMin.Value > parametre.ValeurMax.Value Then Return

        If parametre.ValeurMin.HasValue Then nombre.Minimum = parametre.ValeurMin.Value
        If parametre.ValeurMax.HasValue Then nombre.Maximum = parametre.ValeurMax.Value

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AvertirPortee
    ' Version    : V2.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Après un enregistrement : rappelle que les nouvelles valeurs valent pour les prochaines
    ' opérations, avec la description de chaque paramètre modifié (quand le changer a des
    ' conséquences, la description le dit : fichiers non déplacés, prochain lancement…).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AvertirPortee(parametres As List(Of ParametreApplication))

        Dim message As New System.Text.StringBuilder()

        If parametres.Count = 1 Then
            message.Append($"« {parametres(0).LibelleParametre} » est enregistré.")
        Else
            message.Append($"{parametres.Count} paramètres sont enregistrés.")
        End If
        message.AppendLine().AppendLine()
        message.Append("Les nouvelles valeurs s'appliquent aux prochaines opérations ; " &
                       "les données déjà enregistrées ne sont pas modifiées.")

        For Each parametre In parametres
            If String.IsNullOrWhiteSpace(parametre.DescriptionParametre) Then Continue For
            message.AppendLine().AppendLine()
            If parametres.Count > 1 Then message.Append($"{parametre.LibelleParametre} : ")
            message.Append(parametre.DescriptionParametre.Trim())
        Next

        DialogChoix.Information(message.ToString(), "Althéa - Paramètres modifiés")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateContextInfo
    ' Rôle       : Fil d'Ariane (avec le mode d'accès) et, si fourni, message de la barre d'état.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UpdateContextInfo(Optional statusMessage As String = Nothing)

        If _context Is Nothing Then Return

        Dim mode As String = If(_modeAcces = ModeAccesParametres.Admin, "mode Admin", "mode SuperUser")
        _context.SetHeader($"Administration > Paramètres ({mode})")

        If Not String.IsNullOrWhiteSpace(statusMessage) Then _context.SetStatus(statusMessage)

    End Sub

#End Region

End Class
