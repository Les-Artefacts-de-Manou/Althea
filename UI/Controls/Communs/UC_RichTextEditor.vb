' -------------------------------------------------------------------------------------------------
' UserControl : UC_RichTextEditor
' Projet      : Althéa
' Version     : V2.4.1
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Éditeur de texte riche d'Althéa (anamnèse, zones du dossier, notes de séance, alertes,
' commentaires), en deux profils : Complet et Compact.
'
' Responsabilités :
' - Barre d'outils dans le style des barres locales du kit (AltheaRenduBarre), selon le profil :
'   * Complet : historique · style de paragraphe · caractère · paragraphe · insertion · recherche,
'     et à droite aperçu, impression, export ;
'   * Compact : historique · G / I / S, couleurs · puces · date, et à droite « Ouvrir en grand ».
' - Consultation (ReadOnlyMode) : le profil Complet garde une barre de consultation (repère
'   « Lecture seule », rechercher, copier, aperçu, imprimer, exporter) ; le profil Compact masque
'   sa barre.
' - Barre de recherche et de remplacement (Ctrl+F, Ctrl+H, F3), menu du clic droit, ligne d'état
'   (mots, « Modifié », zoom) pour le profil Complet.
' - Exposer RtfContent / TextContent pour l'enregistrement (RTF pour l'affichage, texte pour la
'   recherche), signaler les modifications (ContentChanged), déléguer le chemin d'export au
'   conteneur (ExportRequested / ExportCompleted).
'
' Remarques   :
' - Refonte du 19/10/2026 (maquette « Éditeurs de texte — refonte » validée) : un seul contrôle à
'   deux profils remplace UC_RichTextEditor et UC_RichTextEditorSimple.
' - Les raccourcis sont traités avant le RichTextBox (ProcessCmdKey) : Ctrl+G ou Ctrl+B gras,
'   Ctrl+I italique (il insérait une tabulation), Ctrl+U souligné, Ctrl+L / E / R / J alignements,
'   Ctrl+Maj+V coller sans mise en forme, Ctrl+F / Ctrl+H recherche, F3 / Maj+F3, Ctrl+P.
'   En consultation, aucun raccourci ne modifie le texte.
' - Les actions de mise en forme lèvent ContentChanged : le RichTextBox ne signale que les
'   changements de texte, un mot passé en gras n'était pas vu comme une modification.
' - Les messages à l'utilisateur passent par DialogChoix ; les échecs sont journalisés par
'   RichTextEditorHelper, sans contenu ni chemin de fichier.
' - « Ouvrir en grand » (profil Compact) : le même texte dans une grande fenêtre au profil Complet
'   (EditeurTexteAgrandi) ; Valider le reporte dans le champ, qui sera enregistré avec la fiche.
' - Impression, aperçu et exports portent l'en-tête et le pied de page des documents
'   (EnTeteDocument, rassemblé au moment de la sortie : cabinet, praticien, TitreDocument,
'   PatientDocument ; mention, date d'édition, page n / N).
' - Textes types (temps 2) : bouton de la zone insertion, proposé quand l'écran a déclaré l'usage
'   de l'éditeur (UsageTextesTypes) ; liste lue à l'ouverture du menu (GestionTextesTypes).
' - Correcteur d'orthographe (temps 2) : celui de Windows (choix de Joëlle après essai, 20/10/2026),
'   actif en saisie si le réglage le permet ; il vérifie l'orthographe, pas la grammaire.
'
' Historique :
' - V2.4.1 (20/10/2026) : réglage, langue du clavier et lignes du clic droit du correcteur sortis
'   dans CorrecteurTexte, partagé avec les zones de texte simples (AltheaZoneTexte) ; comportement
'   inchangé.
' - V2.4.0 (20/10/2026, temps 2, chantier 4) : correcteur d'orthographe de Windows dans tous les
'   éditeurs, en saisie (CorrecteurOrthographe, réglage EDITEUR_CORRECTEUR_ORTHOGRAPHE) ; texte
'   attaché à la langue du clavier ; clic droit sur un mot souligné : suggestions, « Ajouter au
'   dictionnaire » (CorrecteurWindows). « Ignorer » essayé puis retiré : le contrôle soulignait de
'   nouveau le mot retapé, sans plus proposer de correction (relevé par Joëlle, 20/10/2026).
' - V2.3.0 (19/10/2026, temps 2) : textes types (btnTextesTypes, UsageTextesTypes).
' - V2.2.0 (19/10/2026, temps 2) : en-tête et pied de page à l'impression, à l'aperçu et dans les
'   exports (PatientDocument, EnTeteDocument).
' - V2.1.0 (19/10/2026, temps 2) : « Ouvrir en grand » pour le profil Compact (btnOuvrirEnGrand,
'   EditeurTexteAgrandi) ; propriétés AfficherSorties (aperçu, impression, export masquables) et
'   EstModifie.
' - V2.0.0 (19/10/2026) : refonte (profils, barre du kit, icônes redessinées, styles de paragraphe
'   à la place de police / taille, justifié, listes numérotées, couleurs de la charte, date et
'   horodatage signé, recherche / remplacement, aperçu, menu contextuel, ligne d'état, barre de
'   consultation) ; boucle des listes de police supprimée avec elles ; ContentSaved (jamais levé)
'   retiré ; MarquerEnregistre, NomAuteur, TitreDocument ajoutés.
' - V1.0 (16/05/2026) : création.
'
' Dépendances :
' - RichTextEditorHelper (opérations techniques), AltheaRenduBarre (rendu des barres)
' - EditeurTexteAgrandi (« Ouvrir en grand »), AltheaEnTete (icône reprise de la fenêtre)
' - EnTeteDocument (en-tête et pied de page des documents)
' - GestionTextesTypes, TexteType (textes types proposés selon l'usage de l'éditeur)
' - CorrecteurTexte (correcteur d'orthographe : réglage, langue du clavier, lignes du clic droit)
' - AltheaRecherche, AltheaBouton, AltheaCase (barre de recherche), DialogChoix (messages)
' - IContextAwareUserControl, UserControlContext (auteur de l'horodatage), UITheme
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel

' -------------------------------------------------------------------------------------------------
' Enum        : ProfilEditeur
' Version     : V1.0.0
' Date        : 19/10/2026
'
' Rôle        : Profil de l'éditeur : Complet (documents : anamnèse, zones du dossier) ou Compact
'               (notes et commentaires posés dans une fiche).
' -------------------------------------------------------------------------------------------------
Public Enum ProfilEditeur
    Complet = 0
    Compact = 1
End Enum

Public Class UC_RichTextEditor
    Implements IContextAwareUserControl

#Region "Constantes"

    ' Marge intérieure du texte (pixels).
    Private Const MARGE_TEXTE As Integer = 10

    ' Formats insérés par le bouton Date.
    Private Const FORMAT_DATE As String = "dd/MM/yyyy"
    Private Const FORMAT_DATE_HEURE As String = "dd/MM/yyyy HH:mm"

    ' Titre par défaut des impressions et des exports.
    Private Const TITRE_PAR_DEFAUT As String = "Notes Althéa"

    ' Taille des pastilles de couleur des menus (pixels).
    Private Const TAILLE_PASTILLE As Integer = 16

    ' Présence d'un élément de la barre selon le mode : édition Complet, édition Compact,
    ' consultation (profil Complet en lecture seule).
    <Flags>
    Private Enum Presence
        Aucune = 0
        Complet = 1
        Compact = 2
        Consultation = 4
    End Enum

#End Region

#Region "Variables privées"

    Private _context As UserControlContext
    Private _isLoading As Boolean = False
    Private _profil As ProfilEditeur = ProfilEditeur.Complet
    Private _barreDemandee As Boolean = True
    Private _nomAuteur As String = String.Empty
    Private _titreDocument As String = TITRE_PAR_DEFAUT
    Private _sortiesAffichees As Boolean = True
    Private _patientDocument As String = String.Empty
    Private _usageTextesTypes As String = String.Empty
    Private _correcteur As Boolean = True

    ' Réglage EDITEUR_CORRECTEUR_ORTHOGRAPHE, lu au chargement (False dans le Designer).
    Private _correcteurPermis As Boolean

    ' Lignes du correcteur au clic droit (mot souligné) ; libérées avec les menus.
    Private ReadOnly _lignesCorrecteur As CorrecteurTexte

    ' Présence de chaque élément de la barre, selon le mode.
    Private ReadOnly _presence As New Dictionary(Of ToolStripItem, Presence)()

    ' Polices et images créées pour les menus (styles, couleurs) : libérées avec le contrôle.
    Private ReadOnly _ressourcesMenus As New List(Of IDisposable)()

    ' Position du curseur à l'ouverture de la recherche (recherche au fil de la frappe).
    Private _debutRecherche As Integer

    ' Menu du bouton Date (textes recalculés à chaque ouverture).
    Private ReadOnly mnuDateJour As New ToolStripMenuItem()
    Private ReadOnly mnuDateHeure As New ToolStripMenuItem()
    Private ReadOnly mnuHorodatage As New ToolStripMenuItem()

#End Region

#Region "Initialisation"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur
    ' Version    : V2.1.0
    ' Date       : 20/10/2026
    '
    ' Rôle       : Rendu du kit sur les barres et le menu contextuel, menus construits par le code
    '              (styles, couleurs, date), présence des éléments selon le mode.
    ' - V2.1.0 (20/10/2026) : lignes du correcteur au clic droit (CorrecteurTexte), libérées avec les menus.
    ' -------------------------------------------------------------------------------------------------
    Public Sub New()

        InitializeComponent()

        toolStrip.Renderer = New AltheaRenduBarre()
        menuContextuel.Renderer = New AltheaRenduBarre()

        ConstruireMenuStyles()
        ConstruireMenuCouleurs(btnCouleur, CouleursTexte(), AddressOf CouleurTexte_Click)
        ConstruireMenuCouleurs(btnSurligneur, CouleursSurligneur(), AddressOf Surligneur_Click)
        ConstruireMenuDate()
        ConstruireMenuTextesTypes()
        mnuAlignGauche.Tag = AlignementParagraphe.Gauche
        mnuAlignCentre.Tag = AlignementParagraphe.Centre
        mnuAlignDroite.Tag = AlignementParagraphe.Droite
        mnuAlignJustifier.Tag = AlignementParagraphe.Justifie
        DefinirPresences()

        _lignesCorrecteur = New CorrecteurTexte(rtbEditor, menuContextuel,
                                                Sub(texte) ExecuterMiseEnForme(Sub(r) r.SelectedText = texte))
        _ressourcesMenus.Add(_lignesCorrecteur)

        AddHandler Disposed, AddressOf LibererRessourcesMenus

        AppliquerPresentation()

    End Sub

    Private Sub UC_RichTextEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        RichTextEditorHelper.ConfigurerRichTextBox(rtbEditor, MARGE_TEXTE)

        If Not DesignMode AndAlso LicenseManager.UsageMode <> LicenseUsageMode.Designtime Then
            _correcteurPermis = CorrecteurTexte.ReglagePermis()
        End If
        AppliquerCorrecteur()
        MettreAJourBarre()
        MettreAJourLigneEtat()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DefinirPresences
    ' Version    : V1.1.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Dit, pour chaque élément de la barre, dans quels modes il apparaît (maquette).
    ' Remarques  : Couper / copier / coller passent au clic droit et aux raccourcis en édition ;
    '              « Copier » reste dans la barre en consultation (seule action possible sur le texte).
    ' - V1.1.0 (19/10/2026) : « Ouvrir en grand » en édition Compact ; textes types en édition.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DefinirPresences()

        Dim edition As Presence = Presence.Complet Or Presence.Compact
        Dim sortie As Presence = Presence.Complet Or Presence.Consultation

        _presence(lblLectureSeule) = Presence.Consultation
        _presence(sepLecture) = Presence.Consultation
        _presence(btnAnnuler) = edition
        _presence(btnRetablir) = edition
        _presence(sepHistorique) = edition
        _presence(btnStyle) = Presence.Complet
        _presence(sepStyle) = Presence.Complet
        _presence(btnGras) = edition
        _presence(btnItalique) = edition
        _presence(btnSouligne) = edition
        _presence(btnBarre) = Presence.Complet
        _presence(btnCouleur) = edition
        _presence(btnSurligneur) = edition
        _presence(btnEffacer) = edition
        _presence(sepCaractere) = edition
        _presence(btnPuces) = edition
        _presence(btnNumeros) = Presence.Complet
        _presence(btnRetraitMoins) = Presence.Complet
        _presence(btnRetraitPlus) = Presence.Complet
        _presence(btnAlignement) = Presence.Complet
        _presence(sepParagraphe) = edition
        _presence(btnDate) = edition
        _presence(btnTextesTypes) = edition
        _presence(sepInsertion) = Presence.Complet
        _presence(btnRechercher) = sortie
        _presence(btnCopier) = Presence.Consultation
        _presence(btnExporter) = sortie
        _presence(btnImprimer) = sortie
        _presence(btnApercu) = sortie
        _presence(btnOuvrirEnGrand) = Presence.Compact

    End Sub

    Private Sub LibererRessourcesMenus(sender As Object, e As EventArgs)

        For Each ressource As IDisposable In _ressourcesMenus
            ressource.Dispose()
        Next
        _ressourcesMenus.Clear()

    End Sub

#End Region

#Region "Propriétés publiques"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : Profil
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Complet (barre entière, recherche, ligne d'état) ou Compact (barre réduite, pour
    '              les notes et commentaires posés dans une fiche).
    ' -------------------------------------------------------------------------------------------------
    <Category("Althéa")>
    <DefaultValue(GetType(ProfilEditeur), "Complet")>
    <Description("Complet : barre entière, recherche, ligne d'état. Compact : barre réduite (notes, commentaires).")>
    Public Property Profil As ProfilEditeur
        Get
            Return _profil
        End Get
        Set(value As ProfilEditeur)
            If _profil = value Then Return
            _profil = value
            AppliquerPresentation()
        End Set
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : RtfContent
    ' Version    : V2.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Contenu RTF (enregistrement en base) ; le setter accepte aussi du texte brut.
    ' Remarques  : V2.0.0 : non sérialisé par le Designer (il écrivait RtfContent = "").
    ' -------------------------------------------------------------------------------------------------
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property RtfContent As String
        Get
            Return RichTextEditorHelper.ExtraireRtf(rtbEditor)
        End Get
        Set(value As String)
            ChargerContenu(value, String.Empty)
        End Set
    End Property

    ' Texte brut (colonne _txt, pour la recherche).
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property TextContent As String
        Get
            Return RichTextEditorHelper.ExtraireTxt(rtbEditor)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : ReadOnlyMode
    ' Version    : V2.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Consultation : le texte reste sélectionnable et copiable.
    ' Remarques  : V2.0.0 : le profil Complet garde une barre de consultation (la barre entière
    '              disparaissait : il fallait passer en modification pour imprimer ou exporter).
    ' -------------------------------------------------------------------------------------------------
    <Category("Althéa")>
    <DefaultValue(False)>
    <Description("Consultation : texte non modifiable ; le profil Complet garde rechercher, copier, imprimer, exporter.")>
    Public Property ReadOnlyMode As Boolean
        Get
            Return rtbEditor.ReadOnly
        End Get
        Set(value As Boolean)
            rtbEditor.ReadOnly = value
            AppliquerPresentation()
            AppliquerCorrecteur()
        End Set
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : ShowToolbar
    ' Version    : V2.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Affiche ou masque la barre d'outils, quel que soit le mode.
    ' Remarques  : V2.0.0 : réellement respectée (ReadOnlyMode réaffichait la barre).
    ' -------------------------------------------------------------------------------------------------
    <Category("Althéa")>
    <DefaultValue(True)>
    <Description("Affiche la barre d'outils.")>
    Public Property ShowToolbar As Boolean
        Get
            Return _barreDemandee
        End Get
        Set(value As Boolean)
            _barreDemandee = value
            AppliquerPresentation()
        End Set
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : TitreDocument
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Titre des impressions et nom proposé pour les exports sans chemin fourni.
    ' -------------------------------------------------------------------------------------------------
    <Category("Althéa")>
    <DefaultValue(TITRE_PAR_DEFAUT)>
    <Description("Titre des impressions et nom proposé pour les exports.")>
    Public Property TitreDocument As String
        Get
            Return _titreDocument
        End Get
        Set(value As String)
            _titreDocument = If(String.IsNullOrWhiteSpace(value), TITRE_PAR_DEFAUT, value.Trim())
        End Set
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : NomAuteur
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Nom signé par l'horodatage. Par défaut, l'utilisateur connecté (contexte reçu par
    '              SetContext) ; sans contexte ni nom, l'horodatage signé n'est pas proposé.
    ' -------------------------------------------------------------------------------------------------
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property NomAuteur As String
        Get
            If Not String.IsNullOrWhiteSpace(_nomAuteur) Then Return _nomAuteur

            Dim utilisateur As UtilisateurApplication = _context?.AuthenticatedUser
            If utilisateur Is Nothing Then Return String.Empty

            Return If(String.IsNullOrWhiteSpace(utilisateur.NomAffichage), If(utilisateur.LoginUtilisateur, String.Empty), utilisateur.NomAffichage)
        End Get
        Set(value As String)
            _nomAuteur = If(value, String.Empty)
        End Set
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : PatientDocument
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Patient concerné par le texte, imprimé dans l'en-tête des documents (impression,
    '              aperçu, exports) : EnTeteDocument.DesignerPatient(patient). Vide : pas de ligne.
    ' -------------------------------------------------------------------------------------------------
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property PatientDocument As String
        Get
            Return _patientDocument
        End Get
        Set(value As String)
            _patientDocument = If(value, String.Empty).Trim()
        End Set
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : CorrecteurOrthographe
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       : Correcteur d'orthographe de Windows : mots inconnus soulignés pendant la frappe,
    '              suggestions au clic droit. Actif seulement en saisie et si le réglage de
    '              Paramètres le permet (EDITEUR_CORRECTEUR_ORTHOGRAPHE).
    ' Remarques  :
    ' - Vrai par défaut : tous les éditeurs en profitent (choix de Joëlle, 20/10/2026).
    ' - Le texte est attaché à la langue du clavier (le RTF le marque en anglais) : au chargement et
    '   à l'activation, sans compter comme une modification.
    ' -------------------------------------------------------------------------------------------------
    <Category("Althéa")>
    <DefaultValue(True)>
    <Description("Correcteur d'orthographe de Windows (soulignement pendant la frappe, langue du clavier).")>
    Public Property CorrecteurOrthographe As Boolean
        Get
            Return _correcteur
        End Get
        Set(value As Boolean)
            _correcteur = value
            AppliquerCorrecteur()
        End Set
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UsageTextesTypes
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Usage de l'éditeur (code de UsagesTexteType : ANAMNESE, NOTES_SEANCE…) ; le bouton
    '              Textes types propose les textes de cet usage et ceux proposés partout. Vide : pas
    '              de bouton.
    ' Remarques  : Renseigné par le code de l'écran (UsagesTexteType), jamais par le Designer.
    ' -------------------------------------------------------------------------------------------------
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property UsageTextesTypes As String
        Get
            Return _usageTextesTypes
        End Get
        Set(value As String)
            _usageTextesTypes = If(value, String.Empty).Trim()
            AppliquerPresentation()
        End Set
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : AfficherSorties
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Montre l'aperçu, l'impression et l'export (profil Complet). Masqués pour un texte
    '              qui n'est pas un document à sortir tel quel (fenêtre « Ouvrir en grand »).
    ' -------------------------------------------------------------------------------------------------
    <Category("Althéa")>
    <DefaultValue(True)>
    <Description("Affiche l'aperçu, l'impression et l'export (profil Complet).")>
    Public Property AfficherSorties As Boolean
        Get
            Return _sortiesAffichees
        End Get
        Set(value As Boolean)
            _sortiesAffichees = value
            AppliquerPresentation()
        End Set
    End Property

    ' Le contenu a changé depuis son chargement ou depuis MarquerEnregistre.
    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property EstModifie As Boolean
        Get
            Return rtbEditor.Modified
        End Get
    End Property

#End Region

#Region "Méthodes publiques"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ChargerContenu
    ' Version    : V2.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Charge le contenu depuis le RTF, avec repli sur le texte brut.
    ' Remarques  :
    ' - Crée le handle si nécessaire (un RichTextBox sans handle peut ignorer le RTF).
    ' - Le contenu chargé n'est pas une modification : « Modifié » et l'historique repartent à zéro.
    ' -------------------------------------------------------------------------------------------------
    Public Sub ChargerContenu(rtfContent As String, txtFallback As String)

        If Not rtbEditor.IsHandleCreated Then rtbEditor.CreateControl()

        _isLoading = True
        Try
            RichTextEditorHelper.ChargerContenu(rtbEditor, rtfContent, txtFallback, Name)
            If CorrecteurActif Then RichTextEditorHelper.DefinirLangueTexte(rtbEditor, CorrecteurTexte.LangueClavier())
            rtbEditor.ClearUndo()
            rtbEditor.Modified = False
        Finally
            _isLoading = False
        End Try

        MettreAJourBarre()
        MettreAJourLigneEtat()

    End Sub

    ' Correcteur réellement actif : demandé, permis par le réglage, et texte en saisie.
    Private ReadOnly Property CorrecteurActif As Boolean
        Get
            Return _correcteur AndAlso _correcteurPermis AndAlso Not rtbEditor.ReadOnly
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AppliquerCorrecteur
    ' Version    : V1.0.1
    ' Date       : 20/10/2026
    '
    ' Rôle       : Active ou coupe le soulignement selon CorrecteurActif ; à l'activation, le texte
    '              prend la langue du clavier, sans compter comme une modification.
    ' Remarques  : Rien avant le chargement (réglage pas encore lu) ni dans le Designer.
    ' - V1.0.1 (20/10/2026) : activation et langue par CorrecteurTexte.Appliquer.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerCorrecteur()

        If DesignMode OrElse Not rtbEditor.IsHandleCreated Then Return

        Dim modifie As Boolean = rtbEditor.Modified
        _isLoading = True
        Try
            CorrecteurTexte.Appliquer(rtbEditor, CorrecteurActif)
        Finally
            _isLoading = False
            rtbEditor.Modified = modifie
        End Try

    End Sub

    ' Indique que le contenu vient d'être enregistré : la ligne d'état n'affiche plus « Modifié ».
    Public Sub MarquerEnregistre()
        rtbEditor.Modified = False
        MettreAJourLigneEtat()
    End Sub

#End Region

#Region "Événements"

    ' Le contenu a changé (texte ou mise en forme), hors chargement.
    Public Event ContentChanged(sender As Object, e As EventArgs)

    ' -------------------------------------------------------------------------------------------------
    ' Événement  : ExportRequested
    ' Version    : V1.0
    ' Date       : 14/06/2026
    '
    ' Rôle       : Demande d'export (PDF ou Word) : le conteneur peut fournir le chemin (export
    '              contextuel, ex. dossier du patient) ; sinon l'éditeur propose un emplacement.
    ' -------------------------------------------------------------------------------------------------
    Public Event ExportRequested(sender As Object, e As ExportRequestedEventArgs)

    ' Fin d'un export contextuel (chemin fourni par le conteneur) : e.Success, e.DestinationPath.
    Public Event ExportCompleted(sender As Object, e As ExportCompletedEventArgs)

#End Region

#Region "Contexte UI"

    ' Contexte partagé (facultatif) : sert à signer l'horodatage du nom de l'utilisateur connecté.
    Public Sub SetContext(context As UserControlContext) Implements IContextAwareUserControl.SetContext
        _context = context
    End Sub

#End Region

#Region "Présentation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AppliquerPresentation
    ' Version    : V1.1.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Montre la barre, ses éléments, la recherche et la ligne d'état selon le profil,
    '              le mode (édition / consultation) et ShowToolbar.
    ' - V1.1.0 (19/10/2026) : aperçu, impression et export masqués si AfficherSorties = False ;
    '   textes types seulement si l'usage de l'éditeur est déclaré (ElementPermis).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerPresentation()

        Dim complet As Boolean = _profil = ProfilEditeur.Complet
        Dim mode As Presence

        If rtbEditor.ReadOnly Then
            mode = If(complet, Presence.Consultation, Presence.Aucune)
        Else
            mode = If(complet, Presence.Complet, Presence.Compact)
        End If

        toolStrip.Visible = _barreDemandee AndAlso mode <> Presence.Aucune

        For Each paire As KeyValuePair(Of ToolStripItem, Presence) In _presence
            paire.Key.Available = (paire.Value And mode) <> Presence.Aucune AndAlso ElementPermis(paire.Key)
        Next

        If Not complet Then FermerRecherche(False)
        If rtbEditor.ReadOnly Then pnlRemplacer.Visible = False
        btnAfficherRemplacer.Visible = Not rtbEditor.ReadOnly

        pnlEtat.Visible = complet

        MettreAJourLigneEtat()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : MettreAJourBarre
    ' Version    : V2.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Reflète la sélection dans la barre (styles cochés, liste, alignement, style du
    '              paragraphe, Annuler / Rétablir).
    ' Remarques  : V2.0.0 : ne touche plus à aucune liste déroulante ; la version précédente
    '              changeait les listes de police et de taille, ce qui réappliquait une taille
    '              arrondie à la sélection (10,5 devenait 10) et bouclait.
    ' -------------------------------------------------------------------------------------------------
    Private Sub MettreAJourBarre()

        If Not rtbEditor.IsHandleCreated Then Return

        Dim police As Font = rtbEditor.SelectionFont

        btnGras.Checked = police IsNot Nothing AndAlso police.Bold
        btnItalique.Checked = police IsNot Nothing AndAlso police.Italic
        btnSouligne.Checked = police IsNot Nothing AndAlso police.Underline
        btnBarre.Checked = police IsNot Nothing AndAlso police.Strikeout

        Dim liste As TypeListe = RichTextEditorHelper.LireListe(rtbEditor)
        btnPuces.Checked = liste = TypeListe.Puces
        btnNumeros.Checked = liste = TypeListe.Numeros

        Dim alignement As AlignementParagraphe? = RichTextEditorHelper.LireAlignement(rtbEditor)
        For Each element As ToolStripItem In btnAlignement.DropDownItems
            Dim ligne As ToolStripMenuItem = TryCast(element, ToolStripMenuItem)
            If ligne Is Nothing Then Continue For
            ligne.Checked = alignement.HasValue AndAlso CType(ligne.Tag, AlignementParagraphe) = alignement.Value
            If ligne.Checked Then btnAlignement.Image = ligne.Image
        Next

        Dim style As StyleParagraphe? = RichTextEditorHelper.DetecterStyleParagraphe(rtbEditor)
        btnStyle.Text = If(style.HasValue, RichTextEditorHelper.DefinitionStyle(style.Value, rtbEditor.ForeColor).Libelle, "—")
        For Each element As ToolStripItem In btnStyle.DropDownItems
            Dim ligne As ToolStripMenuItem = TryCast(element, ToolStripMenuItem)
            If ligne IsNot Nothing Then ligne.Checked = style.HasValue AndAlso CType(ligne.Tag, StyleParagraphe) = style.Value
        Next

        btnAnnuler.Enabled = rtbEditor.CanUndo
        btnRetablir.Enabled = rtbEditor.CanRedo

    End Sub

    ' Ligne d'état : nombre de mots, « Modifié » (en édition), zoom.
    Private Sub MettreAJourLigneEtat()

        If _profil <> ProfilEditeur.Complet Then Return

        Dim mots As Integer = RichTextEditorHelper.CompterMots(rtbEditor.Text)
        lblMots.Text = If(mots <= 1, $"{mots} mot", $"{mots} mots")
        lblModifie.Text = If(rtbEditor.Modified AndAlso Not rtbEditor.ReadOnly, "· Modifié", String.Empty)
        lblZoom.Text = $"Zoom {CInt(Math.Round(rtbEditor.ZoomFactor * 100))} %"

    End Sub

#End Region

    ' Aperçu, impression, export : la « sortie » du document.
    Private Function EstSortie(element As ToolStripItem) As Boolean
        Return element Is btnApercu OrElse element Is btnImprimer OrElse element Is btnExporter
    End Function

    ' Élément permis par la configuration de l'éditeur (sorties affichées, usage des textes types).
    Private Function ElementPermis(element As ToolStripItem) As Boolean
        If EstSortie(element) Then Return _sortiesAffichees
        If element Is btnTextesTypes Then Return _usageTextesTypes.Length > 0
        Return True
    End Function

#Region "Menus construits par le code"

    ' Styles de paragraphe, chacun montré dans son propre rendu (maquette).
    Private Sub ConstruireMenuStyles()

        Dim ordre As StyleParagraphe() = {StyleParagraphe.Titre, StyleParagraphe.SousTitre, StyleParagraphe.Normal,
                                          StyleParagraphe.Citation, StyleParagraphe.Petit}

        For Each style As StyleParagraphe In ordre

            Dim definition As DefinitionStyleParagraphe = RichTextEditorHelper.DefinitionStyle(style, rtbEditor.ForeColor)
            Dim police As New Font(rtbEditor.Font.FontFamily, definition.Taille(rtbEditor.Font.Size), definition.StylePolice)
            _ressourcesMenus.Add(police)

            Dim ligne As New ToolStripMenuItem(definition.Libelle) With {
                .Font = police,
                .ForeColor = definition.Couleur,
                .Tag = style
            }
            AddHandler ligne.Click, AddressOf Style_Click
            btnStyle.DropDownItems.Add(ligne)

        Next

    End Sub

    ' Couleurs du texte : teintes de la charte seulement (pas de rouge vif dans une note clinique).
    Private Function CouleursTexte() As IList(Of KeyValuePair(Of String, Color))
        Return {
            New KeyValuePair(Of String, Color)("Texte (par défaut)", rtbEditor.ForeColor),
            New KeyValuePair(Of String, Color)("Sauge", UITheme.ColorSaugeTresFonce),
            New KeyValuePair(Of String, Color)("Mauve", UITheme.ColorMauveFonce),
            New KeyValuePair(Of String, Color)("Ocre", UITheme.ColorOrangeMoyenFonce),
            New KeyValuePair(Of String, Color)("Terre", UITheme.EditeurTexteTerre),
            New KeyValuePair(Of String, Color)("Bleu", UITheme.EditeurTexteBleu)
        }
    End Function

    ' Surligneurs pâles ; Color.Empty retire le surlignage.
    Private Function CouleursSurligneur() As IList(Of KeyValuePair(Of String, Color))
        Return {
            New KeyValuePair(Of String, Color)("Jaune", UITheme.EditeurSurligneurJaune),
            New KeyValuePair(Of String, Color)("Sauge", UITheme.ColorGrisVertClair),
            New KeyValuePair(Of String, Color)("Mauve", UITheme.ColorMauveTresClair),
            New KeyValuePair(Of String, Color)("Aucun surlignage", Color.Empty)
        }
    End Function

    Private Sub ConstruireMenuCouleurs(bouton As ToolStripDropDownButton,
                                       couleurs As IList(Of KeyValuePair(Of String, Color)),
                                       gestionnaire As EventHandler)

        For Each couleur As KeyValuePair(Of String, Color) In couleurs

            Dim pastille As Bitmap = CreerPastille(couleur.Value)
            _ressourcesMenus.Add(pastille)

            Dim ligne As New ToolStripMenuItem(couleur.Key, pastille) With {.Tag = couleur.Value}
            AddHandler ligne.Click, gestionnaire
            bouton.DropDownItems.Add(ligne)

        Next

    End Sub

    ' Pastille ronde de la couleur, bord beige ; blanche barrée pour « aucune ».
    Private Shared Function CreerPastille(couleur As Color) As Bitmap

        Dim image As New Bitmap(TAILLE_PASTILLE, TAILLE_PASTILLE)

        Using g As Graphics = Graphics.FromImage(image)
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            Dim zone As New Rectangle(1, 1, TAILLE_PASTILLE - 3, TAILLE_PASTILLE - 3)

            Using pinceau As New SolidBrush(If(couleur.IsEmpty, Color.White, couleur))
                g.FillEllipse(pinceau, zone)
            End Using
            Using stylo As New Pen(UITheme.ColorBeigeFonce)
                g.DrawEllipse(stylo, zone)
                If couleur.IsEmpty Then g.DrawLine(stylo, zone.Left + 3, zone.Bottom - 3, zone.Right - 3, zone.Top + 3)
            End Using
        End Using

        Return image

    End Function

    Private Sub ConstruireMenuDate()

        AddHandler mnuDateJour.Click, Sub(s, e) InsererDate(FORMAT_DATE)
        AddHandler mnuDateHeure.Click, Sub(s, e) InsererDate(FORMAT_DATE_HEURE)
        AddHandler mnuHorodatage.Click, Sub(s, e) InsererHorodatage()

        btnDate.DropDownItems.AddRange(New ToolStripItem() {mnuDateJour, mnuDateHeure, mnuHorodatage})

    End Sub

    ' Textes du menu Date à l'heure qu'il est ; horodatage signé seulement si l'auteur est connu.
    Private Sub btnDate_DropDownOpening(sender As Object, e As EventArgs) Handles btnDate.DropDownOpening

        Dim maintenant As DateTime = DateTime.Now
        mnuDateJour.Text = maintenant.ToString(FORMAT_DATE)
        mnuDateHeure.Text = maintenant.ToString(FORMAT_DATE_HEURE)

        Dim initiales As String = InitialesAuteur()
        mnuHorodatage.Available = initiales.Length > 0
        mnuHorodatage.Text = $"{TexteHorodatage(maintenant, initiales)}   (horodatage signé)"

    End Sub

    ' Le menu des textes types est rempli à chaque ouverture ; une ligne d'attente le garde ouvrable.
    Private Sub ConstruireMenuTextesTypes()
        btnTextesTypes.DropDown.ShowItemToolTips = True
        AjouterLigneInactive(btnTextesTypes, "…")
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : btnTextesTypes_DropDownOpening
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       :
    ' Relit les textes types de l'usage de l'éditeur (et ceux proposés partout) à chaque ouverture :
    ' un texte ajouté dans Référentiels est proposé tout de suite. L'infobulle montre le début du texte.
    '
    ' Remarques  : Base injoignable : une ligne le dit (journalisé, sans contenu).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnTextesTypes_DropDownOpening(sender As Object, e As EventArgs) Handles btnTextesTypes.DropDownOpening

        btnTextesTypes.DropDownItems.Clear()

        Dim textes As List(Of TexteType)
        Try
            textes = GestionTextesTypes.GetTextesTypesPourUsage(_usageTextesTypes)
        Catch ex As Exception
            GestionLog.EcrireException("UC_RichTextEditor : textes types illisibles.", ex,
                                       category:=GestionLog.LogCategory.Database)
            AjouterLigneInactive(btnTextesTypes, "Textes types indisponibles pour le moment")
            Return
        End Try

        If textes.Count = 0 Then
            AjouterLigneInactive(btnTextesTypes, "Aucun texte type ici (Référentiels › Textes types)")
            Return
        End If

        For Each texte As TexteType In textes
            Dim ligne As New ToolStripMenuItem(texte.LibelleTexteType) With {
                .Tag = texte,
                .ToolTipText = DebutTexte(texte.ContenuTxt)
            }
            AddHandler ligne.Click, AddressOf TexteType_Click
            btnTextesTypes.DropDownItems.Add(ligne)
        Next

    End Sub

    Private Sub TexteType_Click(sender As Object, e As EventArgs)
        Dim texte As TexteType = DirectCast(DirectCast(sender, ToolStripItem).Tag, TexteType)
        ExecuterMiseEnForme(Sub(r) RichTextEditorHelper.InsererRtf(r, texte.ContenuRtf, texte.ContenuTxt))
    End Sub

    Private Shared Sub AjouterLigneInactive(bouton As ToolStripDropDownButton, texte As String)
        bouton.DropDownItems.Add(New ToolStripMenuItem(texte) With {.Enabled = False})
    End Sub

    ' Début d'un texte pour l'infobulle (une ligne, 120 caractères au plus).
    Private Shared Function DebutTexte(texte As String) As String

        Dim uneLigne As String = String.Join(" ", If(texte, String.Empty).Split({ControlChars.Cr, ControlChars.Lf}, StringSplitOptions.RemoveEmptyEntries)).Trim()
        Return If(uneLigne.Length <= 120, uneLigne, uneLigne.Substring(0, 117) & "…")

    End Function

#End Region

#Region "Barre : historique, style, caractère"

    Private Sub btnAnnuler_Click(sender As Object, e As EventArgs) Handles btnAnnuler.Click
        If rtbEditor.ReadOnly OrElse Not rtbEditor.CanUndo Then Return
        rtbEditor.Undo()
        SignalerModification()
        ApresAction()
    End Sub

    Private Sub btnRetablir_Click(sender As Object, e As EventArgs) Handles btnRetablir.Click
        If rtbEditor.ReadOnly OrElse Not rtbEditor.CanRedo Then Return
        rtbEditor.Redo()
        SignalerModification()
        ApresAction()
    End Sub

    Private Sub Style_Click(sender As Object, e As EventArgs)
        Dim style As StyleParagraphe = CType(CType(sender, ToolStripItem).Tag, StyleParagraphe)
        ExecuterMiseEnForme(Sub(r) RichTextEditorHelper.AppliquerStyleParagraphe(r, style))
    End Sub

    Private Sub btnGras_Click(sender As Object, e As EventArgs) Handles btnGras.Click
        ExecuterMiseEnForme(AddressOf RichTextEditorHelper.BasculerGras)
    End Sub

    Private Sub btnItalique_Click(sender As Object, e As EventArgs) Handles btnItalique.Click
        ExecuterMiseEnForme(AddressOf RichTextEditorHelper.BasculerItalique)
    End Sub

    Private Sub btnSouligne_Click(sender As Object, e As EventArgs) Handles btnSouligne.Click
        ExecuterMiseEnForme(AddressOf RichTextEditorHelper.BasculerSouligne)
    End Sub

    Private Sub btnBarre_Click(sender As Object, e As EventArgs) Handles btnBarre.Click
        ExecuterMiseEnForme(AddressOf RichTextEditorHelper.BasculerBarre)
    End Sub

    Private Sub CouleurTexte_Click(sender As Object, e As EventArgs)
        Dim couleur As Color = CType(CType(sender, ToolStripItem).Tag, Color)
        ExecuterMiseEnForme(Sub(r) RichTextEditorHelper.ChangerCouleurTexte(r, couleur))
    End Sub

    Private Sub Surligneur_Click(sender As Object, e As EventArgs)
        Dim couleur As Color = CType(CType(sender, ToolStripItem).Tag, Color)
        ExecuterMiseEnForme(Sub(r) RichTextEditorHelper.ChangerCouleurFond(r, couleur))
    End Sub

    Private Sub btnEffacer_Click(sender As Object, e As EventArgs) Handles btnEffacer.Click
        ExecuterMiseEnForme(AddressOf RichTextEditorHelper.EffacerFormatage)
    End Sub

#End Region

#Region "Barre : paragraphe, insertion"

    Private Sub btnPuces_Click(sender As Object, e As EventArgs) Handles btnPuces.Click
        ExecuterMiseEnForme(Sub(r) RichTextEditorHelper.BasculerListe(r, TypeListe.Puces))
    End Sub

    Private Sub btnNumeros_Click(sender As Object, e As EventArgs) Handles btnNumeros.Click
        ExecuterMiseEnForme(Sub(r) RichTextEditorHelper.BasculerListe(r, TypeListe.Numeros))
    End Sub

    Private Sub btnRetraitMoins_Click(sender As Object, e As EventArgs) Handles btnRetraitMoins.Click
        ExecuterMiseEnForme(AddressOf RichTextEditorHelper.DiminuerRetrait)
    End Sub

    Private Sub btnRetraitPlus_Click(sender As Object, e As EventArgs) Handles btnRetraitPlus.Click
        ExecuterMiseEnForme(AddressOf RichTextEditorHelper.AugmenterRetrait)
    End Sub

    Private Sub Alignement_Click(sender As Object, e As EventArgs) _
        Handles mnuAlignGauche.Click, mnuAlignCentre.Click, mnuAlignDroite.Click, mnuAlignJustifier.Click
        Dim alignement As AlignementParagraphe = CType(CType(sender, ToolStripItem).Tag, AlignementParagraphe)
        ExecuterMiseEnForme(Sub(r) RichTextEditorHelper.ChangerAlignement(r, alignement))
    End Sub

    Private Sub InsererDate(format As String)
        ExecuterMiseEnForme(Sub(r) RichTextEditorHelper.InsererTexte(r, DateTime.Now.ToString(format)))
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InsererHorodatage
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Insère « — 19/10/2026 14:32 · P. N. » sur sa propre ligne, en italique gris
    '              discret, puis reprend le texte normal à la ligne suivante.
    ' Remarques  : Repère d'auteur et de moment dans une note longue ; ce n'est pas une signature
    '              électronique (le texte reste modifiable).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InsererHorodatage()

        Dim initiales As String = InitialesAuteur()
        If initiales.Length = 0 Then Return

        ExecuterMiseEnForme(
            Sub(r)
                Dim position As Integer = r.SelectionStart
                If position > 0 AndAlso r.Text(position - 1) <> ControlChars.Lf Then r.SelectedText = ControlChars.Lf

                r.SelectionFont = New Font(r.Font, FontStyle.Italic)
                r.SelectionColor = UITheme.ColorGrisDiscret
                r.SelectedText = TexteHorodatage(DateTime.Now, initiales)

                r.SelectionFont = r.Font
                r.SelectionColor = r.ForeColor
                r.SelectedText = ControlChars.Lf
            End Sub)

    End Sub

    Private Shared Function TexteHorodatage(moment As DateTime, initiales As String) As String
        Return $"— {moment.ToString(FORMAT_DATE_HEURE)} · {initiales}"
    End Function

    ' Initiales de l'auteur (« Pearl Nguyen » → « P. N. ») ; vide si l'auteur n'est pas connu.
    Private Function InitialesAuteur() As String

        Dim nom As String = NomAuteur
        If String.IsNullOrWhiteSpace(nom) Then Return String.Empty

        Dim parties As String() = nom.Split({" "c, "-"c}, StringSplitOptions.RemoveEmptyEntries)
        Return String.Join(" ", parties.Select(Function(p) Char.ToUpper(p(0)) & "."))

    End Function

#End Region

#Region "Barre : recherche, consultation, sortie"

    Private Sub btnRechercher_Click(sender As Object, e As EventArgs) Handles btnRechercher.Click
        OuvrirRecherche(False)
    End Sub

    Private Sub btnCopier_Click(sender As Object, e As EventArgs) Handles btnCopier.Click
        RichTextEditorHelper.Copier(rtbEditor)
        rtbEditor.Focus()
    End Sub

    Private Sub btnApercu_Click(sender As Object, e As EventArgs) Handles btnApercu.Click

        If DocumentVide("à prévisualiser", "Aperçu") Then Return

        If Not RichTextEditorHelper.AfficherApercu(rtbEditor, _titreDocument, EnTeteDuDocument()) Then
            DialogChoix.Erreur("L'aperçu n'a pas pu s'afficher. Vérifiez qu'une imprimante est installée, puis réessayez.", "Aperçu")
        End If

    End Sub

    Private Sub btnImprimer_Click(sender As Object, e As EventArgs) Handles btnImprimer.Click
        Imprimer()
    End Sub

    Private Sub Imprimer()

        If DocumentVide("à imprimer", "Impression") Then Return

        If Not RichTextEditorHelper.Imprimer(rtbEditor, _titreDocument, EnTeteDuDocument()) Then
            DialogChoix.Erreur("L'impression n'a pas abouti. Vérifiez l'imprimante, puis réessayez.", "Impression")
        End If

        rtbEditor.Focus()

    End Sub

    Private Sub mnuMiseEnPage_Click(sender As Object, e As EventArgs) Handles mnuMiseEnPage.Click

        If Not RichTextEditorHelper.ConfigurerMiseEnPage() Then
            DialogChoix.Erreur("La mise en page n'a pas pu s'ouvrir. Vérifiez qu'une imprimante est installée.", "Mise en page")
        End If

    End Sub

    Private Sub mnuExportPdf_Click(sender As Object, e As EventArgs) Handles mnuExportPdf.Click
        DemanderExport(ExportFormat.Pdf)
    End Sub

    Private Sub mnuExportWord_Click(sender As Object, e As EventArgs) Handles mnuExportWord.Click
        DemanderExport(ExportFormat.Word)
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DemanderExport
    ' Version    : V2.1.0
    ' Date       : 19/10/2026
    '
    ' Rôle       :
    ' Lève ExportRequested pour que le conteneur fournisse le chemin (export contextuel) ; sans
    ' chemin, propose un emplacement (SaveFileDialog).
    '
    ' Remarques  :
    ' - Export contextuel : ExportCompleted est levé, réussi ou non ; le conteneur ouvre le fichier
    '   et l'annonce (pas de message en plus ici, sauf l'échec).
    ' - V2.0.0 : messages DialogChoix (le helper affichait des MessageBox, y compris un succès juste
    '   avant que le conteneur n'ouvre le fichier).
    ' - V2.1.0 (19/10/2026) : en-tête et pied de page du document transmis aux exports.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DemanderExport(format As ExportFormat)

        If DocumentVide("à exporter", "Export") Then Return

        Dim extension As String = If(format = ExportFormat.Word, ".docx", ".pdf")
        Dim args As New ExportRequestedEventArgs(format, NomFichierPropose() & extension)
        RaiseEvent ExportRequested(Me, args)

        Dim chemin As String = args.DestinationPath
        Dim contextuel As Boolean = Not String.IsNullOrWhiteSpace(chemin)

        If Not contextuel Then
            chemin = DemanderEmplacement(format, args.NomFichierInitial)
            If String.IsNullOrEmpty(chemin) Then Return
        End If

        Cursor = Cursors.WaitCursor
        Dim reussi As Boolean
        Try
            Dim enTete As EnTeteDocument = EnTeteDuDocument()
            reussi = If(format = ExportFormat.Word,
                        RichTextEditorHelper.ExporterWord(rtbEditor, chemin, enTete),
                        RichTextEditorHelper.ExporterPDF(rtbEditor, chemin, enTete))
        Finally
            Cursor = Cursors.Default
        End Try

        If contextuel Then RaiseEvent ExportCompleted(Me, New ExportCompletedEventArgs(format, chemin, reussi))

        If Not reussi Then
            DialogChoix.Erreur("L'export n'a pas abouti. Si le fichier est ouvert dans une autre application, fermez-le, puis réessayez.", "Export")
        ElseIf Not contextuel Then
            DialogChoix.Succes($"Le document a été exporté : {IO.Path.GetFileName(chemin)}.", "Export")
        End If

    End Sub

    ' Emplacement choisi par l'utilisateur ; vide s'il annule.
    Private Shared Function DemanderEmplacement(format As ExportFormat, nomPropose As String) As String

        Using dialogue As New SaveFileDialog() With {
            .Filter = If(format = ExportFormat.Word, "Documents Word (*.docx)|*.docx", "Fichiers PDF (*.pdf)|*.pdf"),
            .Title = If(format = ExportFormat.Word, "Exporter en Word", "Exporter en PDF"),
            .FileName = nomPropose,
            .AddExtension = True
        }
            Return If(dialogue.ShowDialog() = DialogResult.OK, dialogue.FileName, String.Empty)
        End Using

    End Function

    ' Nom de fichier tiré du titre du document (caractères interdits et espaces remplacés).
    Private Function NomFichierPropose() As String

        Dim nom As String = _titreDocument
        For Each interdit As Char In IO.Path.GetInvalidFileNameChars()
            nom = nom.Replace(interdit, "_"c)
        Next
        Return nom.Replace(" "c, "_"c)

    End Function

    ' En-tête et pied de page du document, lus au moment de la sortie (cabinet à jour).
    Private Function EnTeteDuDocument() As EnTeteDocument
        Return EnTeteDocument.Creer(_titreDocument, _patientDocument)
    End Function

    ' Prévient que le document est vide ; renvoie True dans ce cas.
    Private Function DocumentVide(action As String, titre As String) As Boolean

        If Not String.IsNullOrWhiteSpace(rtbEditor.Text) Then Return False

        DialogChoix.Information($"Le document est vide : il n'y a rien {action}.", titre)
        Return True

    End Function

#End Region

#Region "Ouvrir en grand"

    Private Sub btnOuvrirEnGrand_Click(sender As Object, e As EventArgs) Handles btnOuvrirEnGrand.Click
        OuvrirEnGrand()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OuvrirEnGrand
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       :
    ' Ouvre le texte dans une grande fenêtre au profil Complet (EditeurTexteAgrandi). Valider y
    ' reporte le texte, mise en forme comprise : c'est une modification du champ, enregistrée avec la
    ' fiche ou la fenêtre qui le contient.
    '
    ' Remarques  :
    ' - La fenêtre reprend le titre du document (TitreDocument), l'auteur de l'horodatage et
    '   l'icône de l'en-tête de la fenêtre qui contient le champ, s'il y en a une.
    ' - Rien n'est reporté si le texte n'a pas changé dans la grande fenêtre.
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirEnGrand()

        If rtbEditor.ReadOnly Then Return

        Using fenetre As New EditeurTexteAgrandi(_titreDocument, RtfContent, TextContent, NomAuteur, IconeDeLaFenetre())

            If fenetre.ShowDialog(FindForm()) <> DialogResult.OK Then
                rtbEditor.Focus()
                Return
            End If

            ExecuterMiseEnForme(Sub(r) RichTextEditorHelper.ChargerContenu(r, fenetre.RtfContent, fenetre.TextContent, Name))
            rtbEditor.Select(rtbEditor.TextLength, 0)

        End Using

    End Sub

    ' Icône de l'en-tête (AltheaEnTete) de la fenêtre qui contient l'éditeur ; Nothing s'il n'y en a pas.
    Private Function IconeDeLaFenetre() As Image

        Dim fenetre As Form = FindForm()
        If fenetre Is Nothing Then Return Nothing

        Dim enTete As AltheaEnTete = fenetre.Controls.OfType(Of AltheaEnTete)().FirstOrDefault()
        Return enTete?.Icone

    End Function

#End Region

#Region "Mise en forme : exécution commune"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ExecuterMiseEnForme
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Applique une action au texte (jamais en consultation), signale la modification,
    '              rafraîchit la barre et rend le focus au texte.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ExecuterMiseEnForme(action As Action(Of RichTextBox))

        If rtbEditor.ReadOnly Then Return

        action(rtbEditor)
        SignalerModification()
        ApresAction()

    End Sub

    Private Sub ApresAction()
        MettreAJourBarre()
        rtbEditor.Focus()
    End Sub

    ' Le RichTextBox ne signale que les changements de texte : une mise en forme est signalée ici.
    Private Sub SignalerModification()

        If _isLoading Then Return

        rtbEditor.Modified = True
        MettreAJourLigneEtat()
        RaiseEvent ContentChanged(Me, EventArgs.Empty)

    End Sub

#End Region

#Region "Recherche et remplacement"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OuvrirRecherche
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Ouvre la barre de recherche (profil Complet), reprend le mot sélectionné s'il tient
    '              sur une ligne, et la ligne de remplacement si demandée (hors consultation).
    ' -------------------------------------------------------------------------------------------------
    Private Sub OuvrirRecherche(avecRemplacement As Boolean)

        If _profil <> ProfilEditeur.Complet Then Return

        Dim selection As String = rtbEditor.SelectedText
        If selection.Length > 0 AndAlso selection.Length <= 80 AndAlso Not selection.Contains(ControlChars.Lf) Then
            txtRecherche.Text = selection
        End If

        _debutRecherche = rtbEditor.SelectionStart
        pnlRecherche.Visible = True
        If avecRemplacement AndAlso Not rtbEditor.ReadOnly Then pnlRemplacer.Visible = True

        txtRecherche.Focus()
        txtRecherche.SelectAll()
        MettreAJourOccurrences()

    End Sub

    Private Sub FermerRecherche(rendreFocus As Boolean)

        pnlRecherche.Visible = False
        pnlRemplacer.Visible = False
        If rendreFocus Then rtbEditor.Focus()

    End Sub

    ' Recherche au fil de la frappe : première occurrence à partir du curseur d'origine.
    Private Sub txtRecherche_TextChanged(sender As Object, e As EventArgs) Handles txtRecherche.TextChanged

        If txtRecherche.Text.Length > 0 Then
            RichTextEditorHelper.SelectionnerOccurrence(rtbEditor, txtRecherche.Text, chkCasse.Checked, True, _debutRecherche)
        End If
        MettreAJourOccurrences()

    End Sub

    Private Sub chkCasse_CheckedChanged(sender As Object, e As EventArgs) Handles chkCasse.CheckedChanged
        MettreAJourOccurrences()
    End Sub

    Private Sub btnOccSuivante_Click(sender As Object, e As EventArgs) Handles btnOccSuivante.Click
        AllerOccurrence(True)
    End Sub

    Private Sub btnOccPrecedente_Click(sender As Object, e As EventArgs) Handles btnOccPrecedente.Click
        AllerOccurrence(False)
    End Sub

    Private Sub AllerOccurrence(versLaFin As Boolean)
        RichTextEditorHelper.SelectionnerOccurrence(rtbEditor, txtRecherche.Text, chkCasse.Checked, versLaFin)
        MettreAJourOccurrences()
    End Sub

    ' « 2 sur 5 », « Aucun résultat », ou rien si le champ est vide.
    Private Sub MettreAJourOccurrences()

        Dim texte As String = txtRecherche.Text
        Dim positions As List(Of Integer) = RichTextEditorHelper.TrouverOccurrences(rtbEditor, texte, chkCasse.Checked)
        Dim rang As Integer = RichTextEditorHelper.RangOccurrenceSelectionnee(rtbEditor, positions, texte.Length)

        If texte.Length = 0 Then
            lblOccurrences.Text = String.Empty
        ElseIf positions.Count = 0 Then
            lblOccurrences.Text = "Aucun résultat"
        ElseIf rang > 0 Then
            lblOccurrences.Text = $"{rang} sur {positions.Count}"
        Else
            lblOccurrences.Text = If(positions.Count = 1, "1 résultat", $"{positions.Count} résultats")
        End If

        btnOccPrecedente.Enabled = positions.Count > 0
        btnOccSuivante.Enabled = positions.Count > 0
        btnRemplacer.Enabled = positions.Count > 0
        btnRemplacerTout.Enabled = positions.Count > 0

    End Sub

    Private Sub btnAfficherRemplacer_Click(sender As Object, e As EventArgs) Handles btnAfficherRemplacer.Click
        pnlRemplacer.Visible = Not pnlRemplacer.Visible AndAlso Not rtbEditor.ReadOnly
        If pnlRemplacer.Visible Then txtRemplacer.Focus()
    End Sub

    Private Sub btnRemplacer_Click(sender As Object, e As EventArgs) Handles btnRemplacer.Click
        Remplacer()
    End Sub

    ' Remplace l'occurrence sélectionnée (s'il y en a une), puis passe à la suivante.
    Private Sub Remplacer()

        If rtbEditor.ReadOnly OrElse txtRecherche.Text.Length = 0 Then Return

        If RichTextEditorHelper.RemplacerSelection(rtbEditor, txtRecherche.Text, txtRemplacer.Text, chkCasse.Checked) Then
            SignalerModification()
        End If

        AllerOccurrence(True)

    End Sub

    Private Sub btnRemplacerTout_Click(sender As Object, e As EventArgs) Handles btnRemplacerTout.Click

        If rtbEditor.ReadOnly OrElse txtRecherche.Text.Length = 0 Then Return

        Dim nombre As Integer = RichTextEditorHelper.RemplacerTout(rtbEditor, txtRecherche.Text, txtRemplacer.Text, chkCasse.Checked)
        If nombre > 0 Then SignalerModification()

        MettreAJourOccurrences()
        lblOccurrences.Text = If(nombre <= 1, $"{nombre} remplacement", $"{nombre} remplacements")

    End Sub

    Private Sub btnFermerRecherche_Click(sender As Object, e As EventArgs) Handles btnFermerRecherche.Click
        FermerRecherche(True)
    End Sub

#End Region

#Region "Menu contextuel"

    ' Couper / coller seulement en édition ; actions grisées sans sélection ou presse-papiers vide.
    Private Sub menuContextuel_Opening(sender As Object, e As CancelEventArgs) Handles menuContextuel.Opening

        Dim edition As Boolean = Not rtbEditor.ReadOnly
        Dim selection As Boolean = rtbEditor.SelectionLength > 0
        Dim presseTexte As Boolean = False

        Try
            presseTexte = Clipboard.ContainsText()
        Catch ex As Runtime.InteropServices.ExternalException
            ' Presse-papiers occupé par une autre application : Coller reste grisé.
        End Try

        mnuCouper.Available = edition
        mnuColler.Available = edition
        mnuCollerTexte.Available = edition

        mnuCouper.Enabled = selection
        mnuCopier.Enabled = selection
        mnuColler.Enabled = presseTexte OrElse SafeContientDonnees()
        mnuCollerTexte.Enabled = presseTexte

        _lignesCorrecteur.Preparer(CorrecteurActif)

    End Sub

    ' Le presse-papiers contient quelque chose que le RichTextBox sait coller (RTF, image…).
    Private Shared Function SafeContientDonnees() As Boolean
        Try
            Return Clipboard.GetDataObject() IsNot Nothing
        Catch ex As Runtime.InteropServices.ExternalException
            Return False
        End Try
    End Function

    Private Sub mnuCouper_Click(sender As Object, e As EventArgs) Handles mnuCouper.Click
        ExecuterMiseEnForme(AddressOf RichTextEditorHelper.Couper)
    End Sub

    Private Sub mnuCopier_Click(sender As Object, e As EventArgs) Handles mnuCopier.Click
        RichTextEditorHelper.Copier(rtbEditor)
    End Sub

    Private Sub mnuColler_Click(sender As Object, e As EventArgs) Handles mnuColler.Click
        ExecuterMiseEnForme(AddressOf RichTextEditorHelper.Coller)
    End Sub

    Private Sub mnuCollerTexte_Click(sender As Object, e As EventArgs) Handles mnuCollerTexte.Click
        ExecuterMiseEnForme(AddressOf RichTextEditorHelper.CollerSansMiseEnForme)
    End Sub

    Private Sub mnuToutSelectionner_Click(sender As Object, e As EventArgs) Handles mnuToutSelectionner.Click
        rtbEditor.SelectAll()
        rtbEditor.Focus()
    End Sub

#End Region

#Region "Gestionnaires d'événements RichTextBox"

    Private Sub rtbEditor_TextChanged(sender As Object, e As EventArgs) Handles rtbEditor.TextChanged

        If _isLoading Then Return

        MettreAJourLigneEtat()
        RaiseEvent ContentChanged(Me, EventArgs.Empty)

    End Sub

    Private Sub rtbEditor_SelectionChanged(sender As Object, e As EventArgs) Handles rtbEditor.SelectionChanged
        If Not _isLoading Then MettreAJourBarre()
    End Sub

    ' Ctrl + molette change le zoom sans événement : la ligne d'état est rafraîchie juste après.
    Private Sub rtbEditor_MouseWheel(sender As Object, e As MouseEventArgs) Handles rtbEditor.MouseWheel
        If (ModifierKeys And Keys.Control) = Keys.Control Then BeginInvoke(New Action(AddressOf MettreAJourLigneEtat))
    End Sub

    ' Clic sur le zoom : retour à 100 %.
    Private Sub lblZoom_Click(sender As Object, e As EventArgs) Handles lblZoom.Click
        rtbEditor.ZoomFactor = 1.0F
        MettreAJourLigneEtat()
    End Sub

    ' Traits fins qui séparent la recherche du texte et la ligne d'état du texte.
    Private Sub Bandeau_Paint(sender As Object, e As PaintEventArgs) Handles pnlRecherche.Paint, pnlRemplacer.Paint, pnlEtat.Paint

        Dim bandeau As Control = CType(sender, Control)
        Dim y As Integer = If(bandeau Is pnlEtat, 0, bandeau.Height - 1)

        Using stylo As New Pen(UITheme.ColorBeige)
            e.Graphics.DrawLine(stylo, 0, y, bandeau.Width, y)
        End Using

    End Sub

#End Region

#Region "Raccourcis clavier"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ProcessCmdKey
    ' Version    : V2.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Traite les raccourcis avant le RichTextBox et avant la fenêtre (Échap fermerait la
    '              fenêtre au lieu de la barre de recherche, Entrée validerait la fenêtre).
    ' Remarques  : V2.0.0 : remplace KeyDown, où e.Handled sans SuppressKeyPress laissait passer la
    '              touche (Ctrl+I insérait une tabulation).
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean

        If TraiterRaccourci(keyData) Then Return True
        Return MyBase.ProcessCmdKey(msg, keyData)

    End Function

    Private Function TraiterRaccourci(touches As Keys) As Boolean

        Dim dansRecherche As Boolean = pnlRecherche.ContainsFocus OrElse pnlRemplacer.ContainsFocus
        Dim dansTexte As Boolean = rtbEditor.Focused
        If Not dansTexte AndAlso Not dansRecherche Then Return False

        ' Raccourcis communs au texte et à la barre de recherche.
        Select Case touches
            Case Keys.Escape
                If Not pnlRecherche.Visible Then Return False
                FermerRecherche(True)
                Return True

            Case Keys.Enter, Keys.Shift Or Keys.Enter
                If txtRecherche.ContainsFocus Then
                    AllerOccurrence(touches = Keys.Enter)
                    Return True
                End If
                If txtRemplacer.Focused Then
                    Remplacer()
                    Return True
                End If
                Return False

            Case Keys.F3, Keys.Shift Or Keys.F3
                If _profil <> ProfilEditeur.Complet Then Return False
                If Not pnlRecherche.Visible OrElse txtRecherche.Text.Length = 0 Then
                    OuvrirRecherche(False)
                Else
                    AllerOccurrence(touches = Keys.F3)
                End If
                Return True

            Case Keys.Control Or Keys.F
                OuvrirRecherche(False)
                Return _profil = ProfilEditeur.Complet

            Case Keys.Control Or Keys.H
                OuvrirRecherche(True)
                Return _profil = ProfilEditeur.Complet

            Case Keys.Control Or Keys.P
                If _profil <> ProfilEditeur.Complet OrElse Not _sortiesAffichees Then Return False
                Imprimer()
                Return True
        End Select

        If Not dansTexte Then Return False

        ' Mise en forme : seulement dans le texte. En consultation, la touche est absorbée sans effet
        ' (le RichTextBox appliquerait lui-même certains raccourcis).
        Dim action As Action(Of RichTextBox) = Nothing

        Select Case touches
            Case Keys.Control Or Keys.B, Keys.Control Or Keys.G
                action = AddressOf RichTextEditorHelper.BasculerGras
            Case Keys.Control Or Keys.I
                action = AddressOf RichTextEditorHelper.BasculerItalique
            Case Keys.Control Or Keys.U
                action = AddressOf RichTextEditorHelper.BasculerSouligne
            Case Keys.Control Or Keys.Shift Or Keys.V
                action = AddressOf RichTextEditorHelper.CollerSansMiseEnForme
            Case Keys.Control Or Keys.L
                action = Sub(r) RichTextEditorHelper.ChangerAlignement(r, AlignementParagraphe.Gauche)
            Case Keys.Control Or Keys.E
                action = Sub(r) RichTextEditorHelper.ChangerAlignement(r, AlignementParagraphe.Centre)
            Case Keys.Control Or Keys.R
                action = Sub(r) RichTextEditorHelper.ChangerAlignement(r, AlignementParagraphe.Droite)
            Case Keys.Control Or Keys.J
                action = Sub(r) RichTextEditorHelper.ChangerAlignement(r, AlignementParagraphe.Justifie)
            Case Else
                Return False
        End Select

        ExecuterMiseEnForme(action)
        Return True

    End Function

#End Region

End Class

' -------------------------------------------------------------------------------------------------
' Enum        : ExportFormat
' Projet      : Althéa
' Version     : V1.0
' Date        : 14/06/2026
'
' Rôle        :
' Formats d'export proposés par UC_RichTextEditor (utilisés dans ExportRequestedEventArgs).
' -------------------------------------------------------------------------------------------------
Public Enum ExportFormat
    Pdf
    Word
End Enum

' -------------------------------------------------------------------------------------------------
' Classe      : ExportRequestedEventArgs
' Projet      : Althéa
' Version     : V1.0
' Date        : 14/06/2026
'
' Rôle        :
' Arguments de l'événement ExportRequested : transporte le format demandé et permet au conteneur
' de fournir, par délégation, le chemin de destination (export contextuel).
'
' Propriétés  :
' - Format            : Format d'export demandé (Pdf ou Word)
' - NomFichierInitial : Nom proposé par défaut si l'éditeur demande l'emplacement (lecture seule)
' - DestinationPath   : Chemin complet de destination, renseigné par le conteneur (Nothing = l'éditeur
'                       demande l'emplacement)
' -------------------------------------------------------------------------------------------------
Public Class ExportRequestedEventArgs
    Inherits EventArgs

    Public ReadOnly Property Format As ExportFormat
    Public ReadOnly Property NomFichierInitial As String
    Public Property DestinationPath As String

    Public Sub New(exportFormat As ExportFormat, nomFichierInitial As String)
        Me.Format = exportFormat
        Me.NomFichierInitial = nomFichierInitial
    End Sub

End Class

' -------------------------------------------------------------------------------------------------
' Classe      : ExportCompletedEventArgs
' Projet      : Althéa
' Version     : V1.0
' Date        : 14/06/2026
'
' Rôle        :
' Arguments de l'événement ExportCompleted : informe le conteneur de la fin d'un export contextuel.
'
' Propriétés  :
' - Format          : Format exporté (Pdf ou Word)
' - DestinationPath : Chemin complet du fichier écrit
' - Success         : Indique si l'export a réussi
' -------------------------------------------------------------------------------------------------
Public Class ExportCompletedEventArgs
    Inherits EventArgs

    Public ReadOnly Property Format As ExportFormat
    Public ReadOnly Property DestinationPath As String
    Public ReadOnly Property Success As Boolean

    Public Sub New(exportFormat As ExportFormat, destinationPath As String, success As Boolean)
        Me.Format = exportFormat
        Me.DestinationPath = destinationPath
        Me.Success = success
    End Sub

End Class
