' -------------------------------------------------------------------------------------------------
' Module     : UITheme
' Projet     : Althéa
' Version    : V1.5.0
' Date       : 19/10/2026
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
' - V1.3.0 (16/10/2026) : retrait de DynamicTabBack, DynamicPanelDetailBack, DynamicLabelFore et
'   DynamicTextDisabledFore, qui ne servaient qu'aux onglets de l'ancien écran Paramètres.
' - V1.4.0 (17/10/2026) : StatutPause (ocre, distinct de l'orange des absences), StatutAujourdhui
'   (rendez-vous du jour) et LigneAujourdhuiBack (grille Patients).
' - V1.5.0 (19/10/2026) : teintes de l'éditeur de texte (EditeurTexteTerre, EditeurTexteBleu,
'   EditeurSurligneurJaune) pour les couleurs du texte et du surligneur.
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

    ' Fond d'un champ en lecture seule (consultation, 14/10/2026) : seul le fond se teinte, le texte
    ' garde sa couleur normale pour rester lisible sans effort. Un champ modifiable a un fond blanc.
    Public ReadOnly ColorChampLectureSeule As Color =
        Color.FromArgb(241, 236, 230)

    'Gris
    Public ReadOnly ColorGrisFonce As Color =
        Color.FromArgb(74, 74, 74)

    Public ReadOnly ColorGrisTresFonce As Color =
        Color.FromArgb(50, 50, 50)

    Public ReadOnly ColorGrisMoyenFonce As Color =
        Color.FromArgb(160, 160, 160)

    ' Texte secondaire (périodes, mentions « aucun », précisions) : assez foncé pour rester lisible
    ' sur les fonds clairs (contraste ≥ 4,5:1), contrairement à ColorGrisMoyenFonce.
    Public ReadOnly ColorGrisDiscret As Color =
        Color.FromArgb(118, 118, 118)

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
        Color.FromArgb(126, 95, 112)

    'Rouge, Rose
    Public ReadOnly ColorRougeMoyenFonce As Color =
        Color.FromArgb(120, 40, 40)

    'Rouge, Rose
    Public ReadOnly ColorRougeMoyenClair As Color =
        Color.FromArgb(168, 120, 110)

    Public ReadOnly ColorRoseFonce As Color =
        Color.FromArgb(194, 106, 118)

    'Orange/jaune
    Public ReadOnly ColorOrangeMoyenFonce As Color =
        Color.FromArgb(193, 132, 69)

    Public ReadOnly ColorJauneModere As Color =
        Color.FromArgb(200, 160, 90)

    Public ReadOnly ColorBoutonEnregistrer As Color =
        Color.FromArgb(200, 160, 90)

    Public ReadOnly ColorBoutonSupprimer As Color =
        Color.FromArgb(168, 120, 110)

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

    ' Gris-vert clair depuis la refonte du menu (maquette « Home A », 11/10/2026).
    Public ReadOnly ButtonHomeBackHover As Color =
        ColorGrisVertClair

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

    ' -------------------------------------------------------------------------------------------------
    ' Couleurs des grilles SECONDAIRES (données rattachées à la ligne de la grille principale).
    ' Volontairement plus sombres / beiges afin que l'utilisateur distingue immédiatement la grille
    ' d'édition (en haut) de la grille de rattachement (en bas), qui n'est pas pilotée par les
    ' boutons Nouveau / Modifier / Désactiver.
    ' -------------------------------------------------------------------------------------------------

    Public ReadOnly DataGridSecondaireBack As Color =
        ColorBeige

    Public ReadOnly DataGridSecondaireHeaderBack As Color =
        ColorSaugeTresFonce

    Public ReadOnly DataGridSecondaireHeaderFore As Color =
        ColorBlanc

    Public ReadOnly DataGridSecondaireCellBack As Color =
        ColorBeigeClair

    Public ReadOnly DataGridSecondaireAlternatingBack As Color =
        ColorBeige

#End Region

#Region "Statuts financiers"

    ' -------------------------------------------------------------------------------------------------
    ' Propriétés des couleurs de statut financier
    ' Version     : V1.0.0
    ' Date        : 17/09/2026
    ' Rôle        :
    ' Palette sémantique commune aux grilles Patients, Séances et Paiements, afin qu'une même réalité
    ' métier (dû, réglé, annulé, absence) soit toujours signalée par la même couleur, quel que soit
    ' l'écran consulté. Les teintes sont volontairement pastel pour rester lisibles en fond de cellule.
    '
    ' Sémantique :
    ' - Regle   : encaissé, plus rien n'est dû (vert)
    ' - Du      : somme exigible, séance passée et non payée (rouge)
    ' - Previsionnel : séance planifiée non encore due (bleu, simple information)
    ' - Annule  : ligne neutralisée, montant ramené à zéro (gris)
    ' - NoShow  : absence patient non prévenue, due et signalée comme alerte (orange)
    ' -------------------------------------------------------------------------------------------------

    Public ReadOnly StatutRegleBack As Color =
        Color.FromArgb(214, 236, 218)

    Public ReadOnly StatutRegleFore As Color =
        Color.FromArgb(30, 85, 45)

    Public ReadOnly StatutDuBack As Color =
        Color.FromArgb(248, 215, 215)

    Public ReadOnly StatutDuFore As Color =
        Color.FromArgb(125, 30, 30)

    Public ReadOnly StatutPrevisionnelBack As Color =
        Color.FromArgb(219, 232, 244)

    Public ReadOnly StatutPrevisionnelFore As Color =
        Color.FromArgb(35, 70, 110)

    Public ReadOnly StatutAnnuleBack As Color =
        Color.FromArgb(230, 230, 230)

    Public ReadOnly StatutAnnuleFore As Color =
        Color.FromArgb(110, 110, 110)

    Public ReadOnly StatutNoShowBack As Color =
        Color.FromArgb(252, 231, 205)

    Public ReadOnly StatutNoShowFore As Color =
        Color.FromArgb(140, 75, 10)

    ' Dossier en pause : ocre tiré de ColorJauneModere, distinct de l'orange des absences
    ' (décision du 17/10/2026, maquette « Patients — proposition »).
    Public ReadOnly StatutPauseBack As Color =
        Color.FromArgb(245, 236, 217)

    Public ReadOnly StatutPauseFore As Color =
        Color.FromArgb(122, 90, 30)

    ' Rendez-vous du jour : le bleu « à venir » (StatutPrevisionnel) en plein, texte blanc.
    Public ReadOnly StatutAujourdhuiBack As Color =
        Color.FromArgb(35, 70, 110)

    Public ReadOnly StatutAujourdhuiFore As Color =
        Color.White

    ' Fond très léger des lignes de la grille Patients dont le patient vient aujourd'hui.
    Public ReadOnly LigneAujourdhuiBack As Color =
        Color.FromArgb(243, 247, 251)

#End Region

#Region "Contrôles dynamiques"

    ' -------------------------------------------------------------------------------------------------
    ' Propriétés des couleurs pour contrôles dynamiques
    ' Version     : V1.0.0
    ' Date        : 14/05/2026
    ' Rôle        :
    ' Couleurs de texte pour les contrôles créés dynamiquement (texte, erreurs).
    ' -------------------------------------------------------------------------------------------------

    '(74, 74, 74)
    Public ReadOnly DynamicTextFore As Color =
        ColorGrisFonce

    Public ReadOnly DynamicErrorFore As Color =
        ColorRougeMoyenFonce

#End Region

#Region "Éditeur de texte"

    ' -------------------------------------------------------------------------------------------------
    ' Teintes de l'éditeur de texte riche
    ' Version     : V1.0.0
    ' Date        : 19/10/2026
    ' Rôle        :
    ' Couleurs du texte et du surligneur proposées par UC_RichTextEditor (maquette « Éditeurs de
    ' texte — refonte », validée le 19/10/2026). Les autres viennent de la palette : texte en Sauge
    ' très foncé, Mauve foncé, Orange moyen foncé (ocre) ; surligneur Gris-vert clair, Mauve très
    ' clair. Teintes foncées pour le texte, pâles pour le surligneur : lisibles sur le fond beige.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly EditeurTexteTerre As Color =
        Color.FromArgb(168, 91, 75)
    Public ReadOnly EditeurTexteBleu As Color =
        Color.FromArgb(79, 111, 149)
    Public ReadOnly EditeurSurligneurJaune As Color =
        Color.FromArgb(243, 234, 185)

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

    Public Const AssetsStandardButtonDiversPath As String =
        "Assets\ico_divers"

    Public Const AssetsHomeButtonPath As String =
        "Assets\Boutons_Home"

    Public Const AssetsBoutonsSuiviPath As String =
        "Assets\Boutons_suivi"

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
