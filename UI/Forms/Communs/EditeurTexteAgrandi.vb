' -------------------------------------------------------------------------------------------------
' Formulaire : EditeurTexteAgrandi
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 19/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Fenêtre « Ouvrir en grand » d'un éditeur au profil Compact (notes de séance, alerte,
' commentaires) : le même texte dans un grand éditeur au profil Complet.
'
' Fonctionnement :
' - Le constructeur reçoit le titre, le contenu (RTF et texte brut), l'auteur de l'horodatage et
'   l'icône de la fenêtre d'origine (facultative).
' - Valider : DialogResult.OK si le texte a changé ; l'appelant relit RtfContent / TextContent et
'   les reporte dans son champ. Rien n'est enregistré en base ici : le champ l'est avec sa fiche.
' - Annuler, croix, Échap après une modification : Enregistrer (= Valider) / Quitter sans
'   enregistrer / Rester, comme partout (GardeModifications, ADR-43).
'
' Remarques  :
' - Aperçu, impression et export masqués (AfficherSorties = False) : le texte n'est pas un document
'   et n'a pas d'en-tête de document ; ces sorties restent celles des éditeurs au profil Complet.
' - Ouverte seulement en édition (le profil Compact masque sa barre en consultation).
'
' Dépendances :
' - UC_RichTextEditor, GardeModifications, AltheaFondFenetre, AltheaEnTete, AltheaCarte,
'   AltheaBarreActions, AltheaBouton
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class EditeurTexteAgrandi

#Region "Constantes"

    ' Part de la zone de travail de l'écran occupée au plus par la fenêtre à l'ouverture.
    Private Const PART_ECRAN_MAX As Double = 0.9

#End Region

#Region "Variables privées"

    Private ReadOnly _titre As String
    Private ReadOnly _rtfInitial As String
    Private ReadOnly _txtInitial As String
    Private ReadOnly _nomAuteur As String
    Private ReadOnly _icone As Image

#End Region

#Region "Construction"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur
    ' Paramètres :
    ' - titre     : Titre du texte (TitreDocument de l'éditeur d'origine : « Notes de séance »…).
    ' - rtf, txt  : Contenu à ouvrir (RTF, texte brut de repli).
    ' - nomAuteur : Nom signé par l'horodatage (vide : horodatage signé non proposé).
    ' - icone     : Icône de l'en-tête de la fenêtre d'origine (Nothing : pas d'icône).
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(titre As String, rtf As String, txt As String, nomAuteur As String, icone As Image)

        InitializeComponent()

        _titre = titre
        _rtfInitial = rtf
        _txtInitial = txt
        _nomAuteur = nomAuteur
        _icone = icone

    End Sub

#End Region

#Region "Résultat"

    ' Contenu à reporter dans le champ d'origine (après Valider).
    Public ReadOnly Property RtfContent As String
        Get
            Return rteTexte.RtfContent
        End Get
    End Property

    Public ReadOnly Property TextContent As String
        Get
            Return rteTexte.TextContent
        End Get
    End Property

#End Region

#Region "Cycle de vie"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : EditeurTexteAgrandi_Load
    ' Version   : V1.0.0
    ' Date      : 19/10/2026
    '
    ' Rôle      : Garde des modifications, fond du kit, titres, taille adaptée à l'écran, contenu.
    ' -------------------------------------------------------------------------------------------------
    Private Sub EditeurTexteAgrandi_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        GardeModifications.SurveillerFenetre(Me, btnValider, "du texte")

        AltheaFondFenetre.Appliquer(Me)

        enTete.Text = _titre
        enTete.Icone = _icone
        Text = $"{_titre} — en grand"

        AjusterAEcran()

        rteTexte.AfficherSorties = False
        rteTexte.TitreDocument = _titre
        rteTexte.NomAuteur = _nomAuteur
        rteTexte.ChargerContenu(_rtfInitial, _txtInitial)

    End Sub

    Private Sub EditeurTexteAgrandi_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        rteTexte.Focus()
    End Sub

    ' Taille du Designer, ramenée à la zone de travail de l'écran si elle est plus petite.
    Private Sub AjusterAEcran()

        Dim zone As Rectangle = Screen.FromControl(If(Owner, CType(Me, Control))).WorkingArea

        Dim largeur As Integer = Math.Min(Width, CInt(zone.Width * PART_ECRAN_MAX))
        Dim hauteur As Integer = Math.Min(Height, CInt(zone.Height * PART_ECRAN_MAX))
        If largeur = Width AndAlso hauteur = Height Then Return

        Size = New Size(largeur, hauteur)
        If Owner IsNot Nothing Then CenterToParent() Else CenterToScreen()

    End Sub

#End Region

#Region "Actions"

    ' Reporte le texte dans le champ d'origine, s'il a changé ; sinon, ferme simplement.
    Private Sub btnValider_Click(sender As Object, e As EventArgs) Handles btnValider.Click

        DialogResult = If(rteTexte.EstModifie, DialogResult.OK, DialogResult.Cancel)
        Close()

    End Sub

    Private Sub btnAnnuler_Click(sender As Object, e As EventArgs) Handles btnAnnuler.Click

        DialogResult = DialogResult.Cancel
        Close()

    End Sub

#End Region

End Class
