' -------------------------------------------------------------------------------------------------
' Module     : UITheme
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 14/05/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Centralise les couleurs, chemins d'assets et constantes visuelles UI.
'
' Responsabilités :
' - Fournir les couleurs de la charte graphique
' - Fournir les couleurs des boutons Home
' - Fournir les couleurs des boutons standards
' - Fournir les chemins des assets UI
' - Fournir les suffixes normalisés des images
'
' Remarques :
' - Ce module ne contient aucune logique comportementale.
' - Il sert uniquement de référence visuelle centralisée.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module UITheme

#Region "Palette Althéa"

    ' -------------------------------------------------------------------------------------------------
    ' Propriétés de la palette de couleurs Althéa
    ' Version     : V1.0.0
    ' Date        : 14/05/2026
    ' Rôle        :
    ' Définition de la palette de couleurs utilisée dans l'application Althéa (Sauge, Beige, Gris, Blanc, Mauve, Rouge/Rose).
    ' -------------------------------------------------------------------------------------------------
    'Sauge
    Public ReadOnly ColorSauge As Color =
        Color.FromArgb(122, 155, 135)

    Public ReadOnly ColorSaugeFonce As Color =
        Color.FromArgb(95, 125, 110)

    Public ReadOnly ColorSaugeClair As Color =
        Color.FromArgb(178, 197, 186)

    Public ReadOnly ColorSaugeTresFonce As Color =
        Color.FromArgb(75, 105, 90)

    'Beige
    Public ReadOnly ColorBeigeClair As Color =
        Color.FromArgb(244, 239, 234)

    Public ReadOnly ColorBeige As Color =
        Color.FromArgb(235, 226, 217)

    Public ReadOnly ColorBeigeFonce As Color =
        Color.FromArgb(218, 201, 184)

    Public ReadOnly ColorRoseBeige As Color =
        Color.FromArgb(216, 180, 160)

    'Gris
    Public ReadOnly ColorGrisFonce As Color =
        Color.FromArgb(74, 74, 74)

    Public ReadOnly ColorGrisTresFonce As Color =
        Color.FromArgb(50, 50, 50)

    Public ReadOnly ColorGrisMoyenFonce As Color =
        Color.FromArgb(160, 160, 160)

    Public ReadOnly ColorGrisMoyen As Color =
        Color.FromArgb(210, 210, 210)

    Public ReadOnly ColorGrisTresClair As Color =
        Color.FromArgb(250, 250, 250)

    Public ReadOnly ColorGrisClair As Color =
        Color.FromArgb(236, 236, 236)

    Public ReadOnly ColorGrisVertClair As Color =
        Color.FromArgb(222, 230, 225)

    'Blanc
    Public ReadOnly ColorBlanc As Color =
        Color.White

    'Mauve
    Public ReadOnly ColorMauveTresClair As Color =
        Color.FromArgb(230, 222, 226)

    Public ReadOnly ColorMauveFonce As Color =
        Color.FromArgb(11, 95, 125)

    'Rouge, Rose
    Public ReadOnly ColorRougeMoyenFonce As Color =
        Color.FromArgb(120, 40, 40)

    Public ReadOnly ColorRoseFonce As Color =
        Color.FromArgb(194, 106, 118)

    'Orange
    Public ReadOnly ColorOrangeMoyenFonce As Color =
        Color.FromArgb(193, 132, 69)

    'Bleu
    Public ReadOnly ColorBleuMoyen As Color =
        Color.FromArgb(69, 130, 193)

#End Region

#Region "Boutons standards"

    ' -------------------------------------------------------------------------------------------------
    ' Propriétés des couleurs des boutons standards
    ' Version     : V1.0.0
    ' Date        : 14/05/2026
    ' Rôle        :
    ' Couleurs de fond et de texte pour les états Normal, Hover, Down et Disabled des boutons standards.
    ' -------------------------------------------------------------------------------------------------

    Public ReadOnly ButtonStandardBackNormal As Color =
        ColorSauge

    Public ReadOnly ButtonStandardBackHover As Color =
        ColorSaugeFonce

    Public ReadOnly ButtonStandardBackDown As Color =
        ColorSaugeTresFonce

    Public ReadOnly ButtonStandardBackDisabled As Color =
        ColorSaugeClair

    Public ReadOnly ButtonStandardForeNormal As Color =
        ColorBlanc

    Public ReadOnly ButtonStandardForeHover As Color =
        ColorBlanc

    Public ReadOnly ButtonStandardForeDown As Color =
        ColorBlanc

    Public ReadOnly ButtonStandardForeDisabled As Color =
        ColorGrisClair

#End Region

#Region "Boutons Home"

    ' -------------------------------------------------------------------------------------------------
    ' Propriétés des couleurs des boutons Home
    ' Version     : V1.0.0
    ' Date        : 14/05/2026
    ' Rôle        :
    ' Couleurs de fond et de texte pour les états Normal, Hover, Selected et Disabled des boutons Home.
    ' -------------------------------------------------------------------------------------------------

    Public ReadOnly ButtonHomeBackNormal As Color =
        ColorBeige

    Public ReadOnly ButtonHomeBackHover As Color =
        ColorSaugeClair

    Public ReadOnly ButtonHomeBackSelected As Color =
        ColorSaugeFonce

    Public ReadOnly ButtonHomeBackDisabled As Color =
        ColorBeige

    Public ReadOnly ButtonHomeForeNormal As Color =
        ColorSaugeFonce

    Public ReadOnly ButtonHomeForeHover As Color =
        ColorSaugeFonce

    Public ReadOnly ButtonHomeForeSelected As Color =
        ColorBlanc

    Public ReadOnly ButtonHomeForeDisabled As Color =
        ColorGrisMoyen

#End Region

#Region "DataGridView"

    ' -------------------------------------------------------------------------------------------------
    ' Propriétés des couleurs pour DataGridView
    ' Version     : V1.0.0
    ' Date        : 14/05/2026
    ' Rôle        :
    ' Couleurs de fond, grille, en-têtes, cellules, sélection et alternance pour les DataGridView.
    ' -------------------------------------------------------------------------------------------------

    Public ReadOnly DataGridBack As Color =
        ColorBeigeClair

    Public ReadOnly DataGridGridColor As Color =
        ColorBeigeFonce

    Public ReadOnly DataGridHeaderBack As Color =
        ColorSaugeClair

    Public ReadOnly DataGridHeaderFore As Color =
        ColorGrisFonce

    Public ReadOnly DataGridCellBack As Color =
        ColorBlanc

    Public ReadOnly DataGridCellFore As Color =
        ColorGrisFonce

    Public ReadOnly DataGridCellSelectionBack As Color =
        ColorMauveTresClair

    Public ReadOnly DataGridCellSelectionFore As Color =
        ColorGrisTresFonce

    Public ReadOnly DataGridAlternatingBack As Color =
        ColorGrisVertClair

#End Region

#Region "Contrôles dynamiques"

    ' -------------------------------------------------------------------------------------------------
    ' Propriétés des couleurs pour contrôles dynamiques
    ' Version     : V1.0.0
    ' Date        : 14/05/2026
    ' Rôle        :
    ' Couleurs de fond et texte pour les contrôles créés dynamiquement (Tab, Panel, Label, TextBox, erreurs).
    ' -------------------------------------------------------------------------------------------------

    Public ReadOnly DynamicTabBack As Color =
        ColorBeigeClair

    Public ReadOnly DynamicPanelDetailBack As Color =
        ColorBeigeClair

    Public ReadOnly DynamicLabelFore As Color =
        ColorSaugeFonce

    '(74, 74, 74)
    Public ReadOnly DynamicTextFore As Color =
        ColorGrisFonce

    Public ReadOnly DynamicTextDisabledFore As Color =
        ColorGrisMoyenFonce


    Public ReadOnly DynamicErrorFore As Color =
        ColorRougeMoyenFonce

#End Region

#Region "Chemins Assets"

    ' -------------------------------------------------------------------------------------------------
    ' Propriétés des chemins vers les assets UI
    ' Version     : V1.0.0
    ' Date        : 14/05/2026
    ' Rôle        :
    ' Chemins relatifs vers les dossiers contenant les images des boutons (icônes 32px, 48px, Home).
    ' -------------------------------------------------------------------------------------------------

    Public Const AssetsStandardButton32Path As String =
        "Assets\Boutons_ico_32"

    Public Const AssetsStandardButton48Path As String =
        "Assets\Boutons_ico_48"

    Public Const AssetsHomeButtonPath As String =
        "Assets\Boutons_Home"

#End Region


#Region "Suffixes images"

    ' -------------------------------------------------------------------------------------------------
    ' Propriétés des suffixes normalisés pour les images
    ' Version     : V1.0.0
    ' Date        : 14/05/2026
    ' Rôle        :
    ' Suffixes normalisés pour construire les noms de fichiers d'images (normal, hover, disabled, selected) + extension.
    ' -------------------------------------------------------------------------------------------------

    Public Const ImageSuffixNormal As String =
        "_normal"

    Public Const ImageSuffixHover As String =
        "_hover"

    Public Const ImageSuffixDisabled As String =
        "_disabled"

    Public Const ImageSuffixSelected As String =
        "_selected"

    Public Const ImageExtension As String =
        ".png"

#End Region

End Module
