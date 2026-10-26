' -------------------------------------------------------------------------------------------------
' Module      : RichTextEditorHelper
' Projet      : Althéa
' Version     : V2.3.0
' Date        : 19/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les opérations techniques du RichTextBox de l'éditeur riche (UC_RichTextEditor) :
' configuration, mise en forme, listes, recherche, impression, export.
'
' Responsabilités :
' - Configurer le RichTextBox (marges intérieures, typographie avancée pour le texte justifié).
' - Mettre en forme les caractères (gras, italique, souligné, barré, couleurs) et les paragraphes
'   (styles, alignements, listes à puces ou numérotées, retraits).
' - Rechercher, compter et remplacer du texte.
' - Imprimer et prévisualiser le contenu RTF (EM_FORMATRANGE), avec la mise en page de la session.
' - Exporter en PDF ou en Word (Syncfusion DocIO).
' - Poser l'en-tête et le pied de page des documents (EnTeteDocument) à l'impression, à l'aperçu
'   et dans les exports.
'
' Important   :
' - Module technique : aucune règle métier, aucun accès à la base, aucun message à l'utilisateur.
'   Une opération qui échoue est journalisée et renvoie False ; c'est l'éditeur qui informe
'   l'utilisateur (DialogChoix).
' - Rien du contenu (texte des notes, chemins de fichiers patients) n'est écrit dans les journaux.
' - Les appels Win32 et la mémoire non managée sont toujours libérés dans le même flux.
'
' Historique :
' - V2.3.0 (20/10/2026, temps 2, chantier 4) : correcteur d'orthographe de Windows
'   (DefinirCorrecteurOrthographe, ADR-45) et langue du texte (DefinirLangueTexte).
' - V2.2.0 (19/10/2026, temps 2) : InsererRtf (textes types).
' - V2.1.0 (19/10/2026, temps 2) : en-tête (cabinet, praticien, titre, patient) et pied de page
'   (mention, date d'édition, page n / N) à l'impression, à l'aperçu, en PDF et en Word ; pages
'   découpées d'avance pour connaître leur nombre.
' - V2.0.0 (19/10/2026) : refonte de l'éditeur. Plus de MessageBox (journal + résultat) ;
'   P/Invoke regroupés ici (marges, paragraphe, typographie) ; styles de paragraphe, texte
'   justifié, listes numérotées, collage sans mise en forme ; recherche et remplacement sans
'   risque de boucle ; impression et aperçu sur la mise en page choisie (elle était ignorée :
'   chaque impression recréait un document), état d'impression propre à chaque document ;
'   RTF illisible au chargement journalisé ; police et taille libres retirées (styles).
' - V1.1.1 (18/10/2026) : licence Syncfusion lue depuis Paramètres.
'
' Dépendances :
' - System.Drawing.Printing (impression), user32.dll (rendu RTF natif, paragraphes).
' - Syncfusion.DocIO, Syncfusion.DocToPDFConverter, Syncfusion.Pdf (export).
' - GestionLog (journal), UITheme (couleurs des styles), EnTeteDocument (contenu de l'en-tête,
'   rassemblé par l'éditeur : ce module n'accède pas à la base).
' - UC_RichTextEditor (consommateur).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On
Imports System.Drawing.Printing
Imports System.Runtime.InteropServices
Imports DocIO = Syncfusion.DocIO.DLS

#Region "Énumérations"

' -------------------------------------------------------------------------------------------------
' Enum        : StyleParagraphe
' Version     : V1.0.0
' Date        : 19/10/2026
'
' Rôle        : Styles proposés par l'éditeur à la place du choix libre de police et de taille
'               (maquette « Éditeurs de texte — refonte », validée le 19/10/2026) : une note garde
'               la police d'Althéa.
' -------------------------------------------------------------------------------------------------
Public Enum StyleParagraphe
    Normal = 0
    Titre = 1
    SousTitre = 2
    Citation = 3
    Petit = 4
End Enum

' -------------------------------------------------------------------------------------------------
' Enum        : AlignementParagraphe
' Version     : V1.0.0
' Date        : 19/10/2026
'
' Rôle        : Alignements d'un paragraphe, justifié compris (HorizontalAlignment ne le connaît pas).
' Remarques   : Les valeurs sont celles de Windows (PFA_LEFT, PFA_RIGHT, PFA_CENTER, PFA_JUSTIFY).
' -------------------------------------------------------------------------------------------------
Public Enum AlignementParagraphe
    Gauche = 1
    Droite = 2
    Centre = 3
    Justifie = 4
End Enum

' -------------------------------------------------------------------------------------------------
' Enum        : TypeListe
' Version     : V1.0.0
' Date        : 19/10/2026
'
' Rôle        : Liste appliquée à un paragraphe.
' -------------------------------------------------------------------------------------------------
Public Enum TypeListe
    Aucune = 0
    Puces = 1
    Numeros = 2
End Enum

#End Region

' -------------------------------------------------------------------------------------------------
' Classe      : DefinitionStyleParagraphe
' Version     : V1.0.0
' Date        : 19/10/2026
'
' Rôle        : Rendu d'un style de paragraphe, calculé à partir de la police de base de l'éditeur
'               (les tailles sont relatives : changer la police de base décale tous les styles).
' -------------------------------------------------------------------------------------------------
Public NotInheritable Class DefinitionStyleParagraphe

    Public ReadOnly Property Style As StyleParagraphe
    Public ReadOnly Property Libelle As String
    Public ReadOnly Property EcartTaille As Single
    Public ReadOnly Property StylePolice As FontStyle
    Public ReadOnly Property Couleur As Color
    Public ReadOnly Property Retrait As Integer

    Public Sub New(style As StyleParagraphe, libelle As String, ecartTaille As Single,
                   stylePolice As FontStyle, couleur As Color, retrait As Integer)
        Me.Style = style
        Me.Libelle = libelle
        Me.EcartTaille = ecartTaille
        Me.StylePolice = stylePolice
        Me.Couleur = couleur
        Me.Retrait = retrait
    End Sub

    ' Taille du style pour une police de base donnée.
    Public Function Taille(tailleBase As Single) As Single
        Return Math.Max(6.0F, tailleBase + EcartTaille)
    End Function

End Class

Public Module RichTextEditorHelper

#Region "API Windows"

    Private Const WM_USER As Integer = &H400

    ' Rendu (ou libération du cache) d'une plage de texte sur un contexte graphique.
    Private Const EM_FORMATRANGE As Integer = WM_USER + 57
    ' Lecture / écriture de la mise en forme du paragraphe (alignement, listes).
    Private Const EM_GETPARAFORMAT As Integer = WM_USER + 61
    Private Const EM_SETPARAFORMAT As Integer = WM_USER + 71
    ' Typographie avancée : nécessaire pour que le texte justifié s'affiche justifié.
    Private Const EM_SETTYPOGRAPHYOPTIONS As Integer = WM_USER + 202
    Private Const TO_ADVANCEDTYPOGRAPHY As Integer = &H1
    ' Marges intérieures gauche et droite (en pixels).
    Private Const EM_SETMARGINS As Integer = &HD3
    Private Const EC_LEFTMARGIN As Integer = &H1
    Private Const EC_RIGHTMARGIN As Integer = &H2

    ' Correcteur d'orthographe de Windows (RichEdit 8, Windows 8 et suivants) : options de langue et
    ' services de texte (TSF) qu'il demande.
    Private Const EM_SETLANGOPTIONS As Integer = WM_USER + 120
    Private Const EM_GETLANGOPTIONS As Integer = WM_USER + 121
    Private Const EM_SETEDITSTYLE As Integer = WM_USER + 204
    Private Const IMF_SPELLCHECKING As Integer = &H800
    Private Const SES_USECTF As Integer = &H10000
    Private Const SES_CTFALLOWEMBED As Integer = &H200000
    Private Const SES_CTFALLOWSMARTTAG As Integer = &H400000
    Private Const SES_CTFALLOWPROOFING As Integer = &H800000

    ' Langue des caractères (CHARFORMAT2.lcid) appliquée à tout le texte.
    Private Const EM_SETCHARFORMAT As Integer = WM_USER + 68
    Private Const SCF_ALL As Integer = &H4
    Private Const CFM_LCID As Integer = &H2000000

    ' Champs de PARAFORMAT2 utilisés.
    Private Const PFM_OFFSET As Integer = &H4
    Private Const PFM_ALIGNMENT As Integer = &H8
    Private Const PFM_NUMBERING As Integer = &H20
    Private Const PFM_NUMBERINGSTYLE As Integer = &H2000
    Private Const PFM_NUMBERINGTAB As Integer = &H4000
    Private Const PFM_NUMBERINGSTART As Integer = &H8000

    Private Const PFN_BULLET As Short = 1
    Private Const PFN_ARABIC As Short = 2
    Private Const PFNS_PERIOD As Short = &H200

    ' Retrait suspendu d'un élément de liste (en twips : 1 pouce = 1440 twips, ici 1/4 pouce).
    Private Const RETRAIT_LISTE_TWIPS As Integer = 360

    <StructLayout(LayoutKind.Sequential)>
    Private Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure CHARRANGE
        Public cpMin As Integer
        Public cpMax As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure FORMATRANGE
        Public hdc As IntPtr
        Public hdcTarget As IntPtr
        Public rc As RECT
        Public rcPage As RECT
        Public chrg As CHARRANGE
    End Structure

    ' Structure Windows PARAFORMAT2 (188 octets).
    <StructLayout(LayoutKind.Sequential, Pack:=4)>
    Private Structure PARAFORMAT2
        Public cbSize As Integer
        Public dwMask As Integer
        Public wNumbering As Short
        Public wEffects As Short
        Public dxStartIndent As Integer
        Public dxRightIndent As Integer
        Public dxOffset As Integer
        Public wAlignment As Short
        Public cTabCount As Short
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=32)>
        Public rgxTabs() As Integer
        Public dySpaceBefore As Integer
        Public dySpaceAfter As Integer
        Public dyLineSpacing As Integer
        Public sStyle As Short
        Public bLineSpacingRule As Byte
        Public bOutlineLevel As Byte
        Public wShadingWeight As Short
        Public wShadingStyle As Short
        Public wNumberingStart As Short
        Public wNumberingStyle As Short
        Public wNumberingTab As Short
        Public wBorderSpace As Short
        Public wBorderWidth As Short
        Public wBorders As Short
    End Structure

    ' Structure Windows CHARFORMAT2W (116 octets) : seule la langue (lcid) est utilisée.
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Private Structure CHARFORMAT2
        Public cbSize As Integer
        Public dwMask As Integer
        Public dwEffects As Integer
        Public yHeight As Integer
        Public yOffset As Integer
        Public crTextColor As Integer
        Public bCharSet As Byte
        Public bPitchAndFamily As Byte
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
        Public szFaceName As String
        Public wWeight As Short
        Public sSpacing As Short
        Public crBackColor As Integer
        Public lcid As Integer
        Public dwCookie As Integer
        Public sStyle As Short
        Public wKerning As Short
        Public bUnderlineType As Byte
        Public bAnimation As Byte
        Public bRevAuthor As Byte
        Public bUnderlineColor As Byte
    End Structure

    <DllImport("user32.dll", CharSet:=CharSet.Unicode, EntryPoint:="SendMessageW")>
    Private Function SendMessageCaractere(hWnd As IntPtr, msg As Integer, wParam As IntPtr, ByRef lParam As CHARFORMAT2) As IntPtr
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto, EntryPoint:="SendMessage")>
    Private Function SendMessageParagraphe(hWnd As IntPtr, msg As Integer, wParam As IntPtr, ByRef lParam As PARAFORMAT2) As IntPtr
    End Function

    ' Nouvelle structure PARAFORMAT2 prête à l'emploi (taille renseignée, tableau alloué).
    Private Function NouveauFormatParagraphe(masque As Integer) As PARAFORMAT2
        Dim pf As New PARAFORMAT2()
        pf.rgxTabs = New Integer(31) {}
        pf.cbSize = Marshal.SizeOf(GetType(PARAFORMAT2))
        pf.dwMask = masque
        Return pf
    End Function

    ' Lit la mise en forme du paragraphe sélectionné ; dwMask indique les champs communs à toute la
    ' sélection (un champ absent du masque a des valeurs différentes d'un paragraphe à l'autre).
    Private Function LireFormatParagraphe(rtb As RichTextBox) As PARAFORMAT2
        Dim pf As PARAFORMAT2 = NouveauFormatParagraphe(0)
        SendMessageParagraphe(rtb.Handle, EM_GETPARAFORMAT, IntPtr.Zero, pf)
        Return pf
    End Function

    Private Sub EcrireFormatParagraphe(rtb As RichTextBox, pf As PARAFORMAT2)
        SendMessageParagraphe(rtb.Handle, EM_SETPARAFORMAT, IntPtr.Zero, pf)
    End Sub

#End Region

#Region "Configuration"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ConfigurerRichTextBox
    ' Version    : V2.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       :
    ' Applique la configuration commune de l'éditeur riche.
    '
    ' Paramètres :
    ' - rtb   : Contrôle à configurer (sans effet s'il vaut Nothing).
    ' - marge : Marge intérieure gauche et droite, en pixels.
    '
    ' Remarques  :
    ' - Crée le handle si nécessaire : les réglages Win32 en ont besoin.
    ' - AcceptsTab : la touche Tab insère une tabulation dans le texte (habitude du traitement de
    '   texte) ; Ctrl+Tab passe au contrôle suivant.
    ' - V2.0.0 : marges et typographie avancée (texte justifié) ; les marges étaient posées par
    '   chacun des deux éditeurs, avec leur propre déclaration Win32.
    ' -------------------------------------------------------------------------------------------------
    Public Sub ConfigurerRichTextBox(rtb As RichTextBox, marge As Integer)

        If rtb Is Nothing Then Exit Sub

        rtb.AcceptsTab = True
        rtb.DetectUrls = True
        rtb.HideSelection = False
        rtb.WordWrap = True
        rtb.ScrollBars = RichTextBoxScrollBars.Vertical
        rtb.EnableAutoDragDrop = True

        If Not rtb.IsHandleCreated Then rtb.CreateControl()

        Dim marges As New IntPtr(marge Or (marge << 16))
        SendMessage(rtb.Handle, EM_SETMARGINS, New IntPtr(EC_LEFTMARGIN Or EC_RIGHTMARGIN), marges)
        SendMessage(rtb.Handle, EM_SETTYPOGRAPHYOPTIONS, New IntPtr(TO_ADVANCEDTYPOGRAPHY), New IntPtr(TO_ADVANCEDTYPOGRAPHY))

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DefinirCorrecteurOrthographe
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       : Active ou coupe le correcteur d'orthographe de Windows dans le contrôle : mots
    '              inconnus soulignés en rouge pendant la frappe.
    '
    ' Remarques  :
    ' - Chantier 4 du temps 2 : correcteur de Windows retenu après essai (Joëlle, 20/10/2026, ADR-45).
    ' - Windows vérifie chaque mot dans la langue qui lui est attachée : la langue du clavier pour
    '   le texte tapé ; pour un texte chargé, celle du RTF (DefinirLangueTexte la corrige).
    ' - Le soulignement est fait par Windows : rien n'est envoyé hors du poste.
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirCorrecteurOrthographe(rtb As RichTextBox, actif As Boolean)

        If rtb Is Nothing Then Exit Sub
        If Not rtb.IsHandleCreated Then rtb.CreateControl()

        If actif Then
            Dim services As Integer = SES_USECTF Or SES_CTFALLOWEMBED Or SES_CTFALLOWSMARTTAG Or SES_CTFALLOWPROOFING
            SendMessage(rtb.Handle, EM_SETEDITSTYLE, New IntPtr(services), New IntPtr(services))
        End If

        Dim options As Integer = SendMessage(rtb.Handle, EM_GETLANGOPTIONS, IntPtr.Zero, IntPtr.Zero).ToInt32()
        options = If(actif, options Or IMF_SPELLCHECKING, options And Not IMF_SPELLCHECKING)
        SendMessage(rtb.Handle, EM_SETLANGOPTIONS, IntPtr.Zero, New IntPtr(options))

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DefinirLangueTexte
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       : Attache une langue (identifiant Windows, ex. 2060 = français de Belgique) à tout le
    '              texte, pour que le correcteur le vérifie dans cette langue.
    ' Remarques  : Le RTF du contrôle marque le texte en anglais (relevé le 19/10/2026 à l'export
    '              Word) : sans cela, un texte chargé serait vérifié en anglais.
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirLangueTexte(rtb As RichTextBox, lcid As Integer)

        If rtb Is Nothing OrElse lcid <= 0 Then Exit Sub
        If Not rtb.IsHandleCreated Then rtb.CreateControl()

        Dim cf As New CHARFORMAT2 With {
            .cbSize = Marshal.SizeOf(GetType(CHARFORMAT2)),
            .dwMask = CFM_LCID,
            .lcid = lcid,
            .szFaceName = String.Empty
        }
        SendMessageCaractere(rtb.Handle, EM_SETCHARFORMAT, New IntPtr(SCF_ALL), cf)

    End Sub

#End Region

#Region "Chargement / Sauvegarde"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ChargerContenu
    ' Version    : V2.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       :
    ' Charge le RTF dans l'éditeur, avec repli sur le texte brut s'il est vide ou illisible.
    '
    ' Paramètres :
    ' - rtb         : Éditeur cible.
    ' - rtfContent  : Contenu RTF (peut être vide ou Nothing).
    ' - txtFallback : Texte brut affiché si le RTF est vide ou illisible.
    ' - origine     : Nom du contrôle, pour le journal (jamais le contenu).
    '
    ' Remarques  :
    ' - V2.0.0 : un RTF illisible est journalisé (il retombait en silence sur le texte brut, en
    '   perdant la mise en forme à l'enregistrement suivant sans que personne le sache).
    ' -------------------------------------------------------------------------------------------------
    Public Sub ChargerContenu(rtb As RichTextBox, rtfContent As String, txtFallback As String,
                              Optional origine As String = "")

        If rtb Is Nothing Then Exit Sub

        rtb.Clear()

        If String.IsNullOrWhiteSpace(rtfContent) Then
            rtb.Text = If(txtFallback, "")
            Exit Sub
        End If

        Try
            rtb.Rtf = rtfContent
        Catch ex As ArgumentException
            GestionLog.EcrireLog($"Contenu RTF illisible ({origine}) : affichage du texte brut, mise en forme perdue.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)
            rtb.Text = If(txtFallback, "")
        End Try

    End Sub

    Public Function ExtraireRtf(rtb As RichTextBox) As String

        If rtb Is Nothing Then Return ""
        If String.IsNullOrWhiteSpace(rtb.Text) Then Return ""

        Return rtb.Rtf

    End Function

    Public Function ExtraireTxt(rtb As RichTextBox) As String

        If rtb Is Nothing Then Return ""

        Return rtb.Text.Trim()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CompterMots
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Nombre de mots du texte (suites de caractères séparées par des blancs).
    ' Remarques  : Parcours simple, sans expression régulière : appelée à chaque frappe.
    ' -------------------------------------------------------------------------------------------------
    Public Function CompterMots(texte As String) As Integer

        If String.IsNullOrEmpty(texte) Then Return 0

        Dim nombre As Integer = 0
        Dim dansMot As Boolean = False

        For Each c As Char In texte
            If Char.IsWhiteSpace(c) Then
                dansMot = False
            ElseIf Not dansMot Then
                dansMot = True
                nombre += 1
            End If
        Next

        Return nombre

    End Function

#End Region

#Region "Formatage de caractères"

    Public Sub BasculerGras(rtb As RichTextBox)
        BasculerStyle(rtb, FontStyle.Bold)
    End Sub

    Public Sub BasculerItalique(rtb As RichTextBox)
        BasculerStyle(rtb, FontStyle.Italic)
    End Sub

    Public Sub BasculerSouligne(rtb As RichTextBox)
        BasculerStyle(rtb, FontStyle.Underline)
    End Sub

    Public Sub BasculerBarre(rtb As RichTextBox)
        BasculerStyle(rtb, FontStyle.Strikeout)
    End Sub

    ' Ajoute ou retire un style de caractère. Sans effet sur une sélection aux polices mélangées
    ' (SelectionFont vaut alors Nothing), comme dans la version précédente.
    Private Sub BasculerStyle(rtb As RichTextBox, style As FontStyle)

        If rtb Is Nothing OrElse rtb.SelectionFont Is Nothing Then Exit Sub

        Dim police As Font = rtb.SelectionFont
        Dim nouveauStyle As FontStyle = If(police.Style.HasFlag(style), police.Style And Not style, police.Style Or style)

        rtb.SelectionFont = New Font(police, nouveauStyle)

    End Sub

    Public Sub ChangerCouleurTexte(rtb As RichTextBox, couleur As Color)
        If rtb Is Nothing Then Exit Sub
        rtb.SelectionColor = couleur
    End Sub

    ' Surligne la sélection ; Color.Empty retire le surlignage (fond de l'éditeur).
    Public Sub ChangerCouleurFond(rtb As RichTextBox, couleur As Color)
        If rtb Is Nothing Then Exit Sub
        rtb.SelectionBackColor = If(couleur.IsEmpty, rtb.BackColor, couleur)
    End Sub

#End Region

#Region "Styles de paragraphe"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : DefinitionStyle
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Rendu d'un style (maquette validée le 19/10/2026).
    '
    ' Paramètres :
    ' - style         : Style demandé.
    ' - couleurTexte  : Couleur du texte normal de l'éditeur (son ForeColor).
    '
    ' Remarques  :
    ' - Titre et sous-titre en sauge, citation en italique gris discret avec un retrait, petit
    '   deux points sous la taille de base.
    ' -------------------------------------------------------------------------------------------------
    Public Function DefinitionStyle(style As StyleParagraphe, couleurTexte As Color) As DefinitionStyleParagraphe

        Select Case style
            Case StyleParagraphe.Titre
                Return New DefinitionStyleParagraphe(style, "Titre", 3.0F, FontStyle.Bold, UITheme.ColorSaugeTresFonce, 0)
            Case StyleParagraphe.SousTitre
                Return New DefinitionStyleParagraphe(style, "Sous-titre", 1.0F, FontStyle.Bold, UITheme.ColorSaugeFonce, 0)
            Case StyleParagraphe.Citation
                Return New DefinitionStyleParagraphe(style, "Citation", 0.0F, FontStyle.Italic, UITheme.ColorGrisDiscret, 20)
            Case StyleParagraphe.Petit
                Return New DefinitionStyleParagraphe(style, "Petit", -2.0F, FontStyle.Regular, couleurTexte, 0)
            Case Else
                Return New DefinitionStyleParagraphe(StyleParagraphe.Normal, "Normal", 0.0F, FontStyle.Regular, couleurTexte, 0)
        End Select

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AppliquerStyleParagraphe
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       :
    ' Applique un style aux paragraphes entiers touchés par la sélection, puis rétablit la sélection.
    '
    ' Remarques  :
    ' - Le style remplace la mise en forme des caractères du paragraphe (police d'Althéa, taille,
    '   gras / italique, couleur) : un titre est un titre d'un bout à l'autre.
    ' - Seule la citation pose un retrait ; les autres styles laissent le retrait tel quel (posé avec
    '   les boutons de retrait), sauf en quittant une citation.
    ' -------------------------------------------------------------------------------------------------
    Public Sub AppliquerStyleParagraphe(rtb As RichTextBox, style As StyleParagraphe)

        If rtb Is Nothing Then Exit Sub

        Dim debutSelection As Integer = rtb.SelectionStart
        Dim longueurSelection As Integer = rtb.SelectionLength
        Dim styleActuel As StyleParagraphe? = DetecterStyleParagraphe(rtb)
        Dim etaitCitation As Boolean = styleActuel.HasValue AndAlso styleActuel.Value = StyleParagraphe.Citation

        Dim debut As Integer
        Dim fin As Integer
        BornesParagraphes(rtb.Text, debutSelection, longueurSelection, debut, fin)

        Dim definition As DefinitionStyleParagraphe = DefinitionStyle(style, rtb.ForeColor)

        rtb.Select(debut, fin - debut)
        rtb.SelectionFont = New Font(rtb.Font.FontFamily, definition.Taille(rtb.Font.Size), definition.StylePolice)
        rtb.SelectionColor = definition.Couleur

        If style = StyleParagraphe.Citation Then
            rtb.SelectionIndent = definition.Retrait
        ElseIf etaitCitation Then
            rtb.SelectionIndent = 0
        End If

        rtb.Select(debutSelection, longueurSelection)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : DetecterStyleParagraphe
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Style reconnu à la position du curseur, pour l'afficher dans la barre.
    ' Retour     : Le style, ou Nothing si la sélection mélange plusieurs polices.
    ' -------------------------------------------------------------------------------------------------
    Public Function DetecterStyleParagraphe(rtb As RichTextBox) As StyleParagraphe?

        If rtb Is Nothing OrElse rtb.SelectionFont Is Nothing Then Return Nothing

        Dim police As Font = rtb.SelectionFont
        Dim ecart As Single = police.Size - rtb.Font.Size

        For Each style As StyleParagraphe In {StyleParagraphe.Titre, StyleParagraphe.SousTitre, StyleParagraphe.Petit}
            Dim definition As DefinitionStyleParagraphe = DefinitionStyle(style, rtb.ForeColor)
            If Math.Abs(ecart - definition.EcartTaille) < 0.3F AndAlso
               (definition.StylePolice = FontStyle.Regular OrElse police.Style.HasFlag(definition.StylePolice)) Then
                Return style
            End If
        Next

        If Math.Abs(ecart) < 0.3F AndAlso police.Style.HasFlag(FontStyle.Italic) AndAlso
           rtb.SelectionColor.ToArgb() = UITheme.ColorGrisDiscret.ToArgb() Then
            Return StyleParagraphe.Citation
        End If

        Return StyleParagraphe.Normal

    End Function

    ' Début du premier paragraphe et fin du dernier paragraphe touchés par une sélection.
    ' Les positions du RichTextBox comptent un caractère par fin de ligne, comme sa propriété Text.
    Private Sub BornesParagraphes(texte As String, debutSelection As Integer, longueurSelection As Integer,
                                  ByRef debut As Integer, ByRef fin As Integer)

        debut = If(debutSelection <= 0, 0, texte.LastIndexOf(ControlChars.Lf, debutSelection - 1) + 1)

        Dim finSelection As Integer = Math.Min(texte.Length, debutSelection + longueurSelection)
        Dim finLigne As Integer = texte.IndexOf(ControlChars.Lf, finSelection)
        fin = If(finLigne < 0, texte.Length, finLigne)

    End Sub

#End Region

#Region "Formatage de paragraphe"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ChangerAlignement
    ' Version    : V2.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Aligne les paragraphes sélectionnés (gauche, centre, droite ou justifié).
    ' Remarques  : V2.0.0 : passe par Windows, seul moyen de justifier (HorizontalAlignment s'arrête
    '              à gauche / centre / droite).
    ' -------------------------------------------------------------------------------------------------
    Public Sub ChangerAlignement(rtb As RichTextBox, alignement As AlignementParagraphe)

        If rtb Is Nothing Then Exit Sub

        Dim pf As PARAFORMAT2 = NouveauFormatParagraphe(PFM_ALIGNMENT)
        pf.wAlignment = CShort(alignement)
        EcrireFormatParagraphe(rtb, pf)

    End Sub

    ' Alignement de la sélection ; Nothing si les paragraphes sélectionnés diffèrent.
    Public Function LireAlignement(rtb As RichTextBox) As AlignementParagraphe?

        If rtb Is Nothing Then Return Nothing

        Dim pf As PARAFORMAT2 = LireFormatParagraphe(rtb)
        If (pf.dwMask And PFM_ALIGNMENT) = 0 Then Return Nothing

        Select Case CInt(pf.wAlignment)
            Case AlignementParagraphe.Centre, AlignementParagraphe.Droite, AlignementParagraphe.Justifie
                Return CType(CInt(pf.wAlignment), AlignementParagraphe)
            Case Else
                Return AlignementParagraphe.Gauche
        End Select

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DefinirListe
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Met les paragraphes sélectionnés en liste à puces, en liste numérotée (1. 2. 3.)
    '              ou les sort de la liste.
    ' Remarques  : Retrait suspendu d'un quart de pouce, pour que les lignes suivantes d'un même
    '              élément s'alignent sur le texte et non sur la puce.
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirListe(rtb As RichTextBox, liste As TypeListe)

        If rtb Is Nothing Then Exit Sub

        Dim pf As PARAFORMAT2 = NouveauFormatParagraphe(PFM_NUMBERING Or PFM_OFFSET)

        Select Case liste
            Case TypeListe.Puces
                pf.wNumbering = PFN_BULLET
                pf.dxOffset = RETRAIT_LISTE_TWIPS
            Case TypeListe.Numeros
                pf.dwMask = pf.dwMask Or PFM_NUMBERINGSTYLE Or PFM_NUMBERINGSTART Or PFM_NUMBERINGTAB
                pf.wNumbering = PFN_ARABIC
                pf.wNumberingStyle = PFNS_PERIOD
                pf.wNumberingStart = 1
                pf.wNumberingTab = CShort(RETRAIT_LISTE_TWIPS)
                pf.dxOffset = RETRAIT_LISTE_TWIPS
            Case Else
                pf.wNumbering = 0
                pf.dxOffset = 0
        End Select

        EcrireFormatParagraphe(rtb, pf)

    End Sub

    ' Liste du paragraphe courant (Aucune si la sélection mélange plusieurs types).
    Public Function LireListe(rtb As RichTextBox) As TypeListe

        If rtb Is Nothing Then Return TypeListe.Aucune

        Dim pf As PARAFORMAT2 = LireFormatParagraphe(rtb)
        If (pf.dwMask And PFM_NUMBERING) = 0 Then Return TypeListe.Aucune

        If pf.wNumbering = PFN_BULLET Then Return TypeListe.Puces
        If pf.wNumbering >= PFN_ARABIC Then Return TypeListe.Numeros
        Return TypeListe.Aucune

    End Function

    ' Passe du type demandé à « aucune liste » s'il est déjà appliqué, sinon l'applique.
    Public Sub BasculerListe(rtb As RichTextBox, liste As TypeListe)
        If rtb Is Nothing Then Exit Sub
        DefinirListe(rtb, If(LireListe(rtb) = liste, TypeListe.Aucune, liste))
    End Sub

    Public Sub AugmenterRetrait(rtb As RichTextBox)

        If rtb Is Nothing Then Exit Sub

        Dim nouveauRetrait As Integer = rtb.SelectionIndent + 20
        If nouveauRetrait <= 200 Then rtb.SelectionIndent = nouveauRetrait

    End Sub

    Public Sub DiminuerRetrait(rtb As RichTextBox)

        If rtb Is Nothing Then Exit Sub

        rtb.SelectionIndent = Math.Max(0, rtb.SelectionIndent - 20)

    End Sub

#End Region

#Region "Presse-papiers"

    Public Sub Couper(rtb As RichTextBox)
        If rtb Is Nothing OrElse rtb.SelectionLength = 0 Then Exit Sub
        rtb.Cut()
    End Sub

    Public Sub Copier(rtb As RichTextBox)
        If rtb Is Nothing OrElse rtb.SelectionLength = 0 Then Exit Sub
        rtb.Copy()
    End Sub

    Public Sub Coller(rtb As RichTextBox)
        If rtb Is Nothing Then Exit Sub
        rtb.Paste()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : CollerSansMiseEnForme
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Colle le texte du presse-papiers sans sa mise en forme : il prend celle du point
    '              d'insertion (utile depuis un courriel ou une page web).
    ' Remarques  : Le presse-papiers peut être verrouillé par une autre application : l'échec est
    '              journalisé et rien n'est collé.
    ' -------------------------------------------------------------------------------------------------
    Public Sub CollerSansMiseEnForme(rtb As RichTextBox)

        If rtb Is Nothing Then Exit Sub

        Try
            If Clipboard.ContainsText() Then
                rtb.SelectedText = Clipboard.GetText()
            End If
        Catch ex As ExternalException
            GestionLog.EcrireException("Presse-papiers indisponible (coller sans mise en forme).", ex,
                                       GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)
        End Try

    End Sub

#End Region

#Region "Actions"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : EffacerFormatage
    ' Version    : V2.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Rend à la sélection la mise en forme normale de l'éditeur (police, couleur, sans
    '              surlignage, sans liste, sans retrait, aligné à gauche).
    ' Remarques  : V2.0.0 : sans sélection, s'applique au point d'insertion (le texte tapé ensuite
    '              est normal) ; retire aussi la numérotation.
    ' -------------------------------------------------------------------------------------------------
    Public Sub EffacerFormatage(rtb As RichTextBox)

        If rtb Is Nothing Then Exit Sub

        rtb.SelectionFont = rtb.Font
        rtb.SelectionColor = rtb.ForeColor
        rtb.SelectionBackColor = rtb.BackColor
        rtb.SelectionIndent = 0
        DefinirListe(rtb, TypeListe.Aucune)
        ChangerAlignement(rtb, AlignementParagraphe.Gauche)

    End Sub

    ' Insère un texte au point d'insertion (remplace la sélection).
    Public Sub InsererTexte(rtb As RichTextBox, texte As String)
        If rtb Is Nothing OrElse String.IsNullOrEmpty(texte) Then Exit Sub
        rtb.SelectedText = texte
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InsererRtf
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Insère un contenu RTF (texte type) au point d'insertion, mise en forme comprise ;
    '              texte brut si le RTF est absent ou illisible.
    ' Remarques  : Un RTF illisible est journalisé sans son contenu.
    ' -------------------------------------------------------------------------------------------------
    Public Sub InsererRtf(rtb As RichTextBox, rtf As String, texteSecours As String)

        If rtb Is Nothing Then Exit Sub

        If String.IsNullOrWhiteSpace(rtf) Then
            InsererTexte(rtb, texteSecours)
            Exit Sub
        End If

        Try
            rtb.SelectedRtf = rtf
        Catch ex As ArgumentException
            GestionLog.EcrireLog("Texte type au RTF illisible : texte brut inséré, mise en forme perdue.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)
            InsererTexte(rtb, texteSecours)
        End Try

    End Sub

#End Region

#Region "Recherche"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : TrouverOccurrences
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Positions de toutes les occurrences d'un texte, dans l'ordre.
    ' Remarques  : Comparaison ordinale (avec ou sans casse) : la longueur trouvée est toujours celle
    '              du texte cherché, donc la sélection tombe juste.
    ' -------------------------------------------------------------------------------------------------
    Public Function TrouverOccurrences(rtb As RichTextBox, texteRecherche As String, sensibleCasse As Boolean) As List(Of Integer)

        Dim positions As New List(Of Integer)()
        If rtb Is Nothing OrElse String.IsNullOrEmpty(texteRecherche) Then Return positions

        Dim comparaison As StringComparison = If(sensibleCasse, StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase)
        Dim texte As String = rtb.Text
        Dim index As Integer = texte.IndexOf(texteRecherche, 0, comparaison)

        While index >= 0
            positions.Add(index)
            index = If(index + texteRecherche.Length >= texte.Length, -1,
                       texte.IndexOf(texteRecherche, index + texteRecherche.Length, comparaison))
        End While

        Return positions

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : SelectionnerOccurrence
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Sélectionne l'occurrence suivante (ou précédente) à partir de la sélection, en
    '              repartant du début (ou de la fin) du texte une fois au bout.
    ' Paramètres :
    ' - depuis : Position de départ imposée (recherche au fil de la frappe : la première occurrence
    '            à partir de là, celle qui commence au curseur comprise) ; -1 = après la sélection.
    ' Retour     : Rang de l'occurrence sélectionnée (à partir de 1), 0 si aucune.
    ' -------------------------------------------------------------------------------------------------
    Public Function SelectionnerOccurrence(rtb As RichTextBox, texteRecherche As String, sensibleCasse As Boolean,
                                           versLaFin As Boolean, Optional depuis As Integer = -1) As Integer

        Dim positions As List(Of Integer) = TrouverOccurrences(rtb, texteRecherche, sensibleCasse)
        If positions.Count = 0 Then Return 0

        Dim rang As Integer

        If versLaFin Then
            ' Après la sélection courante : une occurrence déjà sélectionnée n'est pas reprise.
            Dim depart As Integer = If(depuis >= 0, depuis, rtb.SelectionStart + Math.Max(1, rtb.SelectionLength))
            rang = positions.FindIndex(Function(p) p >= depart)
            If rang < 0 Then rang = 0
        Else
            Dim depart As Integer = rtb.SelectionStart
            rang = positions.FindLastIndex(Function(p) p < depart)
            If rang < 0 Then rang = positions.Count - 1
        End If

        rtb.Select(positions(rang), texteRecherche.Length)
        rtb.ScrollToCaret()

        Return rang + 1

    End Function

    ' Rang (à partir de 1) de l'occurrence sélectionnée, 0 si la sélection n'en est pas une.
    Public Function RangOccurrenceSelectionnee(rtb As RichTextBox, positions As List(Of Integer), longueur As Integer) As Integer
        If rtb Is Nothing OrElse rtb.SelectionLength <> longueur Then Return 0
        Return positions.IndexOf(rtb.SelectionStart) + 1
    End Function

    ' Remplace la sélection si elle correspond au texte cherché.
    Public Function RemplacerSelection(rtb As RichTextBox, texteRecherche As String, nouveauTexte As String,
                                       sensibleCasse As Boolean) As Boolean

        If rtb Is Nothing OrElse String.IsNullOrEmpty(texteRecherche) Then Return False

        Dim comparaison As StringComparison = If(sensibleCasse, StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase)
        If Not String.Equals(rtb.SelectedText, texteRecherche, comparaison) Then Return False

        rtb.SelectedText = If(nouveauTexte, "")
        Return True

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : RemplacerTout
    ' Version    : V2.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Remplace toutes les occurrences ; renvoie leur nombre.
    ' Remarques  : V2.0.0 : positions relevées une fois, remplacées de la dernière à la première
    '              (les positions restantes ne bougent pas) : aucune boucle possible, même quand le
    '              nouveau texte contient le texte cherché. Chaque remplacement garde la mise en
    '              forme de l'endroit remplacé.
    ' -------------------------------------------------------------------------------------------------
    Public Function RemplacerTout(rtb As RichTextBox, texteRecherche As String, nouveauTexte As String, sensibleCasse As Boolean) As Integer

        Dim positions As List(Of Integer) = TrouverOccurrences(rtb, texteRecherche, sensibleCasse)

        For i As Integer = positions.Count - 1 To 0 Step -1
            rtb.Select(positions(i), texteRecherche.Length)
            rtb.SelectedText = If(nouveauTexte, "")
        Next

        Return positions.Count

    End Function

#End Region

#Region "Impression"

    ' Mise en page choisie par l'utilisateur (format, orientation, marges) : gardée pour la session et
    ' reprise par chaque impression et chaque aperçu.
    Private _miseEnPage As PageSettings

    ' -------------------------------------------------------------------------------------------------
    ' Classe     : ImpressionRtf
    ' Version    : V1.1.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : État d'impression d'un document (pages, page en cours, en-tête) : propre à chaque
    '              impression, il était partagé par tout le module.
    ' Remarques  :
    ' - BeginPrint remet tout à zéro : l'aperçu peut imprimer après avoir affiché.
    ' - V1.1.0 (19/10/2026) : en-tête et pied de page (EnTeteDocument) ; les pages sont découpées
    '   d'avance, à la première page, pour imprimer « Page n / N ».
    ' -------------------------------------------------------------------------------------------------
    Private NotInheritable Class ImpressionRtf

        Private ReadOnly _rtb As RichTextBox
        Private ReadOnly _enTete As EnTeteDocument
        Private _debutsPages As List(Of Integer)
        Private _page As Integer

        Public Sub New(rtb As RichTextBox, enTete As EnTeteDocument)
            _rtb = rtb
            _enTete = enTete
        End Sub

        Public Sub Debuter(sender As Object, e As PrintEventArgs)
            _debutsPages = Nothing
            _page = 0
        End Sub

        Public Sub ImprimerPage(sender As Object, e As PrintPageEventArgs)

            Dim zone As Rectangle = ZoneTexte(e.Graphics, e.MarginBounds, _enTete)

            If _debutsPages Is Nothing Then _debutsPages = DecouperPages(_rtb, e.Graphics, zone, e.PageBounds)

            If _enTete IsNot Nothing Then
                DessinerEnTete(e.Graphics, e.MarginBounds, _enTete)
                DessinerPied(e.Graphics, e.MarginBounds, _enTete, _page + 1, _debutsPages.Count)
            End If

            FormaterPage(_rtb, _debutsPages(_page), _rtb.TextLength, e.Graphics, zone, e.PageBounds, True)

            _page += 1
            e.HasMorePages = _page < _debutsPages.Count

        End Sub

    End Class

    ' Document d'impression prêt : titre, mise en page de la session, pagination RTF, en-tête.
    Private Function CreerDocument(rtb As RichTextBox, titre As String, enTete As EnTeteDocument) As PrintDocument

        Dim document As New PrintDocument() With {.DocumentName = titre}

        If _miseEnPage IsNot Nothing Then
            document.DefaultPageSettings = CType(_miseEnPage.Clone(), PageSettings)
        End If

        Dim impression As New ImpressionRtf(rtb, enTete)
        AddHandler document.BeginPrint, AddressOf impression.Debuter
        AddHandler document.PrintPage, AddressOf impression.ImprimerPage

        Return document

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : Imprimer
    ' Version    : V2.1.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Choix de l'imprimante (dialogue Windows) puis impression.
    ' Paramètres : enTete : en-tête et pied de page (Nothing : aucun).
    ' Retour     : False si l'impression a échoué (journalisé) ; True sinon, annulation comprise.
    ' Remarques  : V2.0.0 : reprend la mise en page choisie ; plus de MessageBox.
    ' - V2.1.0 (19/10/2026) : en-tête et pied de page.
    ' -------------------------------------------------------------------------------------------------
    Public Function Imprimer(rtb As RichTextBox, titre As String, Optional enTete As EnTeteDocument = Nothing) As Boolean

        If rtb Is Nothing Then Return False

        Try
            Using document As PrintDocument = CreerDocument(rtb, titre, enTete)
                Using dialogue As New PrintDialog() With {
                    .Document = document,
                    .AllowSomePages = False,
                    .AllowSelection = False,
                    .AllowCurrentPage = False,
                    .UseEXDialog = True
                }
                    If dialogue.ShowDialog() = DialogResult.OK Then
                        document.Print()
                    End If
                End Using
            End Using
            Return True

        Catch ex As Exception
            GestionLog.EcrireException("Échec de l'impression (éditeur de texte).", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.UI)
            Return False
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : AfficherApercu
    ' Version    : V2.1.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Aperçu avant impression, d'où l'on peut imprimer.
    ' Paramètres : enTete : en-tête et pied de page (Nothing : aucun).
    ' Retour     : False si l'aperçu a échoué (journalisé).
    ' Remarques  : V2.0.0 : relié à la barre (il existait sans bouton) ; mise en page de la session.
    ' - V2.1.0 (19/10/2026) : en-tête et pied de page.
    ' -------------------------------------------------------------------------------------------------
    Public Function AfficherApercu(rtb As RichTextBox, titre As String, Optional enTete As EnTeteDocument = Nothing) As Boolean

        If rtb Is Nothing Then Return False

        Try
            Using document As PrintDocument = CreerDocument(rtb, titre, enTete)
                Using apercu As New PrintPreviewDialog() With {
                    .Document = document,
                    .Text = $"Aperçu avant impression — {titre}",
                    .WindowState = FormWindowState.Maximized,
                    .ShowIcon = False
                }
                    apercu.ShowDialog()
                End Using
            End Using
            Return True

        Catch ex As Exception
            GestionLog.EcrireException("Échec de l'aperçu avant impression (éditeur de texte).", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.UI)
            Return False
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ConfigurerMiseEnPage
    ' Version    : V2.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Dialogue Windows de mise en page (format, orientation, marges).
    ' Retour     : False si le dialogue a échoué (journalisé).
    ' Remarques  :
    ' - V2.0.0 : la mise en page est gardée pour la session et reprise à l'impression (elle était
    '   ignorée).
    ' - EnableMetric : marges saisies en millimètres et converties correctement (sans lui, Windows
    '   affiche des millimètres mais relit des centièmes de pouce : les marges rétrécissent).
    ' -------------------------------------------------------------------------------------------------
    Public Function ConfigurerMiseEnPage() As Boolean

        Try
            If _miseEnPage Is Nothing Then _miseEnPage = New PageSettings()

            Using dialogue As New PageSetupDialog() With {
                .PageSettings = _miseEnPage,
                .AllowMargins = True,
                .AllowOrientation = True,
                .AllowPaper = True,
                .AllowPrinter = True,
                .EnableMetric = True
            }
                dialogue.ShowDialog()
            End Using
            Return True

        Catch ex As Exception
            GestionLog.EcrireException("Échec de la mise en page (éditeur de texte).", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.UI)
            Return False
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : FormaterPage
    ' Version    : V2.1.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Dessine (ou mesure seulement) une page du contenu RTF (EM_FORMATRANGE), mise en
    '              forme comprise, dans la zone donnée (centièmes de pouce).
    ' Retour     : Position du premier caractère qui n'a pas tenu sur la page.
    ' Remarques  :
    ' - Le cache de mise en forme est libéré après chaque page (indispensable sur une imprimante).
    ' - V2.0.0 : fonction privée (c'était une méthode d'extension publique « Print »), mémoire et
    '   contexte graphique libérés même en cas d'erreur.
    ' - V2.1.0 (19/10/2026) : zone de texte fournie (place de l'en-tête et du pied) ; mesure sans
    '   dessin (rendre = False) pour découper les pages d'avance.
    ' -------------------------------------------------------------------------------------------------
    Private Function FormaterPage(rtb As RichTextBox, charFrom As Integer, charTo As Integer, g As Graphics,
                                  zone As Rectangle, page As Rectangle, rendre As Boolean) As Integer

        Dim fr As FORMATRANGE
        Dim hdc As IntPtr = g.GetHdc()
        Dim lParam As IntPtr = IntPtr.Zero

        Try
            fr.hdc = hdc
            fr.hdcTarget = hdc

            fr.rc.Left = CentiemesPouceVersTwips(zone.Left)
            fr.rc.Top = CentiemesPouceVersTwips(zone.Top)
            fr.rc.Right = CentiemesPouceVersTwips(zone.Right)
            fr.rc.Bottom = CentiemesPouceVersTwips(zone.Bottom)

            fr.rcPage.Left = CentiemesPouceVersTwips(page.Left)
            fr.rcPage.Top = CentiemesPouceVersTwips(page.Top)
            fr.rcPage.Right = CentiemesPouceVersTwips(page.Right)
            fr.rcPage.Bottom = CentiemesPouceVersTwips(page.Bottom)

            fr.chrg.cpMin = charFrom
            fr.chrg.cpMax = charTo

            lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fr))
            Marshal.StructureToPtr(fr, lParam, False)

            Return SendMessage(rtb.Handle, EM_FORMATRANGE, New IntPtr(If(rendre, 1, 0)), lParam).ToInt32()

        Finally
            If lParam <> IntPtr.Zero Then Marshal.FreeCoTaskMem(lParam)
            g.ReleaseHdc(hdc)
            SendMessage(rtb.Handle, EM_FORMATRANGE, IntPtr.Zero, IntPtr.Zero)
        End Try

    End Function

    ' Débuts de page (position du premier caractère de chaque page), mesurés sans dessiner.
    Private Function DecouperPages(rtb As RichTextBox, g As Graphics, zone As Rectangle, page As Rectangle) As List(Of Integer)

        Dim debuts As New List(Of Integer) From {0}
        Dim position As Integer = 0

        Do
            Dim suivant As Integer = FormaterPage(rtb, position, rtb.TextLength, g, zone, page, False)

            ' Garde-fou : si rien n'a tenu sur la page (marges impossibles), on s'arrête au lieu de
            ' compter des pages vides sans fin.
            If suivant <= position OrElse suivant >= rtb.TextLength Then Exit Do

            debuts.Add(suivant)
            position = suivant
        Loop

        Return debuts

    End Function

    Private Function CentiemesPouceVersTwips(n As Integer) As Integer
        Return CInt(n * 14.4)
    End Function

#End Region

#Region "En-tête et pied de page"

    ' Police et tailles de l'en-tête et du pied (impression, PDF, Word).
    Private Const POLICE_EN_TETE As String = "Calibri"
    Private Const TAILLE_EN_TETE_TITRE As Single = 10.0F
    Private Const TAILLE_EN_TETE_TEXTE As Single = 8.5F

    ' Impression (centièmes de pouce) : espace entre le trait et le texte de la ligne voisine, et
    ' entre l'en-tête (ou le pied) et le texte du document.
    Private Const ECART_TRAIT As Single = 4.0F
    Private Const ECART_TEXTE As Integer = 15

    Private Const FORMAT_DATE_EDITION As String = "dd/MM/yyyy"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ZoneTexte
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Zone laissée au texte entre l'en-tête et le pied de page, à l'intérieur des marges ;
    '              les marges entières sans en-tête.
    ' -------------------------------------------------------------------------------------------------
    Private Function ZoneTexte(g As Graphics, marges As Rectangle, enTete As EnTeteDocument) As Rectangle

        If enTete Is Nothing Then Return marges

        Using titre As New Font(POLICE_EN_TETE, TAILLE_EN_TETE_TITRE, FontStyle.Bold),
              texte As New Font(POLICE_EN_TETE, TAILLE_EN_TETE_TEXTE)

            Dim haut As Integer = CInt(Math.Ceiling(titre.GetHeight(g) + texte.GetHeight(g) + ECART_TRAIT)) + ECART_TEXTE
            Dim bas As Integer = CInt(Math.Ceiling(texte.GetHeight(g) + ECART_TRAIT)) + ECART_TEXTE

            Return Rectangle.FromLTRB(marges.Left, marges.Top + haut, marges.Right,
                                      Math.Max(marges.Top + haut + 1, marges.Bottom - bas))
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerEnTete
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : En haut de la zone des marges : cabinet (sauge, gras) et titre (gras) sur la première
    '              ligne, praticien et patient sur la seconde, puis un trait fin.
    ' Remarques  : Chaque texte tient dans sa moitié de ligne (points de suspension sinon).
    ' -------------------------------------------------------------------------------------------------
    Private Sub DessinerEnTete(g As Graphics, marges As Rectangle, enTete As EnTeteDocument)

        Using titre As New Font(POLICE_EN_TETE, TAILLE_EN_TETE_TITRE, FontStyle.Bold),
              texte As New Font(POLICE_EN_TETE, TAILLE_EN_TETE_TEXTE),
              encreCabinet As New SolidBrush(UITheme.ColorSaugeTresFonce),
              encre As New SolidBrush(UITheme.ColorGrisFonce),
              trait As New Pen(UITheme.ColorGrisDiscret, 1.0F)

            Dim h1 As Single = titre.GetHeight(g)
            Dim h2 As Single = texte.GetHeight(g)
            Dim moitie As Single = marges.Width / 2.0F
            Dim y1 As Single = marges.Top
            Dim y2 As Single = y1 + h1

            EcrireLigne(g, enTete.Cabinet, titre, encreCabinet, New RectangleF(marges.Left, y1, moitie, h1), StringAlignment.Near)
            EcrireLigne(g, enTete.Titre, titre, encre, New RectangleF(marges.Left + moitie, y1, moitie, h1), StringAlignment.Far)
            EcrireLigne(g, enTete.Praticien, texte, encre, New RectangleF(marges.Left, y2, moitie, h2), StringAlignment.Near)
            EcrireLigne(g, enTete.Patient, texte, encre, New RectangleF(marges.Left + moitie, y2, moitie, h2), StringAlignment.Far)

            Dim yTrait As Single = y2 + h2 + ECART_TRAIT / 2.0F
            g.DrawLine(trait, marges.Left, yTrait, marges.Right, yTrait)

        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerPied
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : En bas de la zone des marges : un trait fin, puis la mention (à gauche), la date
    '              d'édition (au centre) et « Page n / N » (à droite).
    ' -------------------------------------------------------------------------------------------------
    Private Sub DessinerPied(g As Graphics, marges As Rectangle, enTete As EnTeteDocument, page As Integer, total As Integer)

        Using texte As New Font(POLICE_EN_TETE, TAILLE_EN_TETE_TEXTE),
              encre As New SolidBrush(UITheme.ColorGrisFonce),
              trait As New Pen(UITheme.ColorGrisDiscret, 1.0F)

            Dim h As Single = texte.GetHeight(g)
            Dim tiers As Single = marges.Width / 3.0F
            Dim y As Single = marges.Bottom - h

            g.DrawLine(trait, marges.Left, y - ECART_TRAIT / 2.0F, marges.Right, y - ECART_TRAIT / 2.0F)

            EcrireLigne(g, enTete.Mention, texte, encre, New RectangleF(marges.Left, y, tiers, h), StringAlignment.Near)
            EcrireLigne(g, TexteDateEdition(enTete), texte, encre, New RectangleF(marges.Left + tiers, y, tiers, h), StringAlignment.Center)
            EcrireLigne(g, $"Page {page} / {total}", texte, encre, New RectangleF(marges.Left + 2 * tiers, y, tiers, h), StringAlignment.Far)

        End Using

    End Sub

    ' Un texte sur une ligne, aligné dans sa zone, coupé par des points de suspension s'il déborde.
    Private Sub EcrireLigne(g As Graphics, texte As String, police As Font, encre As Brush, zone As RectangleF, alignement As StringAlignment)

        If String.IsNullOrEmpty(texte) Then Return

        Using format As New StringFormat(StringFormatFlags.NoWrap) With {
            .Alignment = alignement,
            .Trimming = StringTrimming.EllipsisCharacter
        }
            g.DrawString(texte, police, encre, zone, format)
        End Using

    End Sub

    Private Function TexteDateEdition(enTete As EnTeteDocument) As String
        Return $"Édité le {enTete.DateEdition.ToString(FORMAT_DATE_EDITION)}"
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AjouterEnTetePied
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Même en-tête et même pied de page dans le document Word (export Word et PDF) :
    '              deux lignes à tabulation droite et un trait sous l'en-tête ; pied avec la mention,
    '              la date d'édition et « Page n / N » (champs Word PAGE et NUMPAGES).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AjouterEnTetePied(document As DocIO.WordDocument, enTete As EnTeteDocument)

        For Each section As DocIO.WSection In document.Sections

            Dim largeur As Single = section.PageSetup.ClientWidth

            Dim ligne1 As DocIO.IWParagraph = section.HeadersFooters.Header.AddParagraph()
            ligne1.ParagraphFormat.Tabs.AddTab(largeur, DocIO.TabJustification.Right, DocIO.TabLeader.NoLeader)
            AjouterTexteWord(ligne1, enTete.Cabinet, True, UITheme.ColorSaugeTresFonce)
            AjouterTexteWord(ligne1, ControlChars.Tab & enTete.Titre, True, UITheme.ColorGrisFonce)

            Dim ligne2 As DocIO.IWParagraph = section.HeadersFooters.Header.AddParagraph()
            ligne2.ParagraphFormat.Tabs.AddTab(largeur, DocIO.TabJustification.Right, DocIO.TabLeader.NoLeader)
            AjouterTexteWord(ligne2, enTete.Praticien, False, UITheme.ColorGrisFonce)
            AjouterTexteWord(ligne2, ControlChars.Tab & enTete.Patient, False, UITheme.ColorGrisFonce)
            TracerBordureWord(ligne2.ParagraphFormat.Borders.Bottom)

            Dim pied As DocIO.IWParagraph = section.HeadersFooters.Footer.AddParagraph()
            pied.ParagraphFormat.Tabs.AddTab(largeur / 2.0F, DocIO.TabJustification.Centered, DocIO.TabLeader.NoLeader)
            pied.ParagraphFormat.Tabs.AddTab(largeur, DocIO.TabJustification.Right, DocIO.TabLeader.NoLeader)
            TracerBordureWord(pied.ParagraphFormat.Borders.Top)
            AjouterTexteWord(pied, enTete.Mention, False, UITheme.ColorGrisFonce)
            AjouterTexteWord(pied, ControlChars.Tab & TexteDateEdition(enTete), False, UITheme.ColorGrisFonce)
            AjouterTexteWord(pied, ControlChars.Tab & "Page ", False, UITheme.ColorGrisFonce)
            FormaterTexteWord(pied.AppendField("Page", Syncfusion.DocIO.FieldType.FieldPage), False, UITheme.ColorGrisFonce)
            AjouterTexteWord(pied, " / ", False, UITheme.ColorGrisFonce)
            FormaterTexteWord(pied.AppendField("NumPages", Syncfusion.DocIO.FieldType.FieldNumPages), False, UITheme.ColorGrisFonce)

        Next

    End Sub

    Private Sub AjouterTexteWord(paragraphe As DocIO.IWParagraph, texte As String, gras As Boolean, couleur As Color)
        If String.IsNullOrEmpty(texte) Then Return
        FormaterTexteWord(paragraphe.AppendText(texte), gras, couleur)
    End Sub

    Private Sub FormaterTexteWord(morceau As DocIO.IWTextRange, gras As Boolean, couleur As Color)

        If morceau Is Nothing Then Return

        morceau.CharacterFormat.FontName = POLICE_EN_TETE
        morceau.CharacterFormat.FontSize = If(gras, TAILLE_EN_TETE_TITRE, TAILLE_EN_TETE_TEXTE)
        morceau.CharacterFormat.Bold = gras
        morceau.CharacterFormat.TextColor = couleur

    End Sub

    Private Sub TracerBordureWord(bordure As DocIO.Border)
        bordure.BorderType = DocIO.BorderStyle.Single
        bordure.Color = UITheme.ColorGrisDiscret
        bordure.LineWidth = 0.5F
        bordure.Space = 2.0F
    End Sub

#End Region

#Region "Export"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ExporterPDF
    ' Version    : V2.1.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Écrit le contenu en PDF (Syncfusion : RTF → document Word → PDF), mise en forme
    '              comprise. Un fichier existant est remplacé.
    ' Retour     : True si le fichier est écrit ; False sinon (journalisé).
    ' Remarques  :
    ' - Requiert la licence Syncfusion du poste (Paramètres, GestionLicences).
    ' - V2.0.0 : plus de MessageBox (l'éditeur informe l'utilisateur) ; documents fermés même en
    '   cas d'erreur.
    ' - V2.1.0 (19/10/2026) : en-tête et pied de page (enTete ; Nothing : aucun).
    ' -------------------------------------------------------------------------------------------------
    Public Function ExporterPDF(rtb As RichTextBox, cheminFichier As String, Optional enTete As EnTeteDocument = Nothing) As Boolean

        If rtb Is Nothing OrElse String.IsNullOrWhiteSpace(cheminFichier) Then Return False

        Dim wordDoc As Syncfusion.DocIO.DLS.WordDocument = Nothing
        Dim pdfDoc As Syncfusion.Pdf.PdfDocument = Nothing

        Try
            Using flux As New System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(rtb.Rtf))
                wordDoc = New Syncfusion.DocIO.DLS.WordDocument(flux, Syncfusion.DocIO.FormatType.Rtf)
                If enTete IsNot Nothing Then AjouterEnTetePied(wordDoc, enTete)
                Dim convertisseur As New Syncfusion.DocToPDFConverter.DocToPDFConverter()
                pdfDoc = convertisseur.ConvertToPDF(wordDoc)
                pdfDoc.Save(cheminFichier)
            End Using
            Return True

        Catch ex As Exception
            JournaliserEchecExport("PDF", ex)
            Return False

        Finally
            pdfDoc?.Close(True)
            wordDoc?.Close()
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ExporterWord
    ' Version    : V2.1.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Écrit le contenu en document Word (.docx), mise en forme comprise. Un fichier
    '              existant est remplacé.
    ' Retour     : True si le fichier est écrit ; False sinon (journalisé).
    ' Remarques  : V2.0.0 : plus de MessageBox ; document fermé même en cas d'erreur.
    ' - V2.1.0 (19/10/2026) : en-tête et pied de page (enTete ; Nothing : aucun) ; langue de
    '   vérification de Word = langue de Windows (AppliquerLangue), le RTF arrivait en anglais.
    ' -------------------------------------------------------------------------------------------------
    Public Function ExporterWord(rtb As RichTextBox, cheminFichier As String, Optional enTete As EnTeteDocument = Nothing) As Boolean

        If rtb Is Nothing OrElse String.IsNullOrWhiteSpace(cheminFichier) Then Return False

        Dim wordDoc As Syncfusion.DocIO.DLS.WordDocument = Nothing

        Try
            Using flux As New System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(rtb.Rtf))
                wordDoc = New Syncfusion.DocIO.DLS.WordDocument(flux, Syncfusion.DocIO.FormatType.Rtf)
                If enTete IsNot Nothing Then AjouterEnTetePied(wordDoc, enTete)
                AppliquerLangue(wordDoc)
                wordDoc.Save(cheminFichier, Syncfusion.DocIO.FormatType.Docx)
            End Using
            Return True

        Catch ex As Exception
            JournaliserEchecExport("Word", ex)
            Return False

        Finally
            wordDoc?.Close()
        End Try

    End Function

    ' Identifiants de langue Windows sans langue réelle : invariante, personnalisée non précisée.
    Private Const LCID_INVARIANT As Integer = 127
    Private Const LCID_PERSONNALISE As Integer = 4096

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AppliquerLangue
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       :
    ' Donne au document Word la langue de Windows (format régional : français de Belgique chez
    ' Pearl), pour que Word vérifie l'orthographe en français.
    '
    ' Remarques  :
    ' - Relevé par Joëlle (19/10/2026) : le RTF du RichTextBox marque le texte en anglais, et Word
    '   soulignait tout. La langue n'est pas écrite en dur : c'est celle du poste.
    ' - Style Normal, puis chaque morceau de texte du corps, des en-têtes et des pieds (tableaux
    '   compris).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerLangue(document As DocIO.WordDocument)

        Dim lcid As Integer = Globalization.CultureInfo.CurrentCulture.LCID
        If lcid <= 0 OrElse lcid > Short.MaxValue OrElse lcid = LCID_INVARIANT OrElse lcid = LCID_PERSONNALISE Then Return

        Dim langue As Short = CShort(lcid)

        Dim normal As DocIO.WParagraphStyle = TryCast(document.Styles.FindByName("Normal"), DocIO.WParagraphStyle)
        If normal IsNot Nothing Then normal.CharacterFormat.LocaleIdASCII = langue

        For Each section As DocIO.WSection In document.Sections
            AppliquerLangueCorps(section.Body, langue)
            AppliquerLangueCorps(section.HeadersFooters.Header, langue)
            AppliquerLangueCorps(section.HeadersFooters.Footer, langue)
        Next

    End Sub

    Private Sub AppliquerLangueCorps(corps As DocIO.WTextBody, langue As Short)

        If corps Is Nothing Then Return

        For Each entite As DocIO.Entity In corps.ChildEntities

            Dim paragraphe As DocIO.WParagraph = TryCast(entite, DocIO.WParagraph)
            If paragraphe IsNot Nothing Then
                paragraphe.BreakCharacterFormat.LocaleIdASCII = langue
                For Each element As DocIO.Entity In paragraphe.ChildEntities
                    Dim morceau As DocIO.WTextRange = TryCast(element, DocIO.WTextRange)
                    If morceau IsNot Nothing Then morceau.CharacterFormat.LocaleIdASCII = langue
                Next
                Continue For
            End If

            Dim tableau As DocIO.WTable = TryCast(entite, DocIO.WTable)
            If tableau IsNot Nothing Then
                For Each ligne As DocIO.WTableRow In tableau.Rows
                    For Each cellule As DocIO.WTableCell In ligne.Cells
                        AppliquerLangueCorps(cellule, langue)
                    Next
                Next
            End If

        Next

    End Sub

    ' Journalise un export manqué sans écrire le chemin du fichier : il contient le code du patient.
    ' Les erreurs de fichier (accès, disque) ne gardent que leur type ; les autres (conversion,
    ' licence) gardent leur message pour le diagnostic.
    Private Sub JournaliserEchecExport(format As String, ex As Exception)

        If TypeOf ex Is System.IO.IOException OrElse TypeOf ex Is UnauthorizedAccessException Then
            GestionLog.EcrireLog($"Échec de l'export {format} (éditeur de texte) : {ex.GetType().Name}.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)
        Else
            GestionLog.EcrireException($"Échec de l'export {format} (éditeur de texte).", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.UI)
        End If

    End Sub

#End Region

End Module
