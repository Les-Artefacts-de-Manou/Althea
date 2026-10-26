' -------------------------------------------------------------------------------------------------
' Module      : UtilsControls
' Projet      : Althéa
' Version     : V1.4.1
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les comportements UI communs aux contrôles WinForms (ComboBox, etc.).
'
' Responsabilités :
' - Initialiser le thème des ComboBox (InitComboBoxTheme)
' - Gérer le dessin personnalisé des items (OwnerDrawFixed)
' - Gérer le focus et la sélection dans les ComboBox
' - Appliquer les couleurs de UITheme
' - Basculer les champs d'une zone, ou un champ seul, en lecture seule lisible (DefinirLectureSeule,
'   DefinirChampLectureSeule)
'
' Remarques   :
' - Les couleurs viennent de UITheme
' - Module statique (méthodes partagées)
'
' Dépendances :
' - UITheme (couleurs)
'
' Historique :
' - V1.4.1 (25/10/2026, audit D-1) : Option Strict On.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module UtilsControls

#Region "Initialisation des contrôles"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InitComboBoxTheme
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Initialise le thème visuel d'un ComboBox selon la charte Althéa.
    '
    ' Paramètres :
    ' - cmb : Le ComboBox à thémer
    '
    ' Remarques  :
    ' - Configure BackColor, ForeColor, FlatStyle, DrawMode (OwnerDrawFixed), ItemHeight
    ' - Attache les handlers DrawItem, GotFocus, MouseUp
    ' - Utilise les couleurs de UITheme
    ' -------------------------------------------------------------------------------------------------
    Public Sub InitComboBoxTheme(
        cmb As ComboBox
    )

        If cmb Is Nothing Then Return

        cmb.BackColor = UITheme.ColorBeigeClair
        cmb.ForeColor = UITheme.DynamicTextFore
        cmb.FlatStyle = FlatStyle.Flat
        cmb.DrawMode = DrawMode.OwnerDrawFixed
        cmb.ItemHeight = cmb.Font.Height + 6

        RemoveHandler cmb.DrawItem, AddressOf ComboBox_DrawItem
        AddHandler cmb.DrawItem, AddressOf ComboBox_DrawItem

        RemoveHandler cmb.GotFocus, AddressOf ComboBox_GotFocus
        AddHandler cmb.GotFocus, AddressOf ComboBox_GotFocus

        RemoveHandler cmb.MouseUp, AddressOf ComboBox_MouseUp
        AddHandler cmb.MouseUp, AddressOf ComboBox_MouseUp

    End Sub

#End Region

#Region "Gestionnaires d'événements ComboBox"
    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ComboBox_DrawItem
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Handler DrawItem pour le dessin personnalisé des items de ComboBox.
    '
    ' Paramètres :
    ' - sender : Le ComboBox source
    ' - e      : DrawItemEventArgs
    '
    ' Remarques  :
    ' - Dessine le fond selon l'état sélectionné (ColorSaugeClair si sélectionné, ColorBeigeClair sinon)
    ' - Utilise TextRenderer pour un rendu clair avec ClearTypeGridFit
    ' - Dessine le rectangle de focus
    ' -------------------------------------------------------------------------------------------------
    Private Sub ComboBox_DrawItem(
        sender As Object,
        e As DrawItemEventArgs
    )

        If e.Index < 0 Then Return

        e.Graphics.TextRenderingHint =
    Drawing.Text.TextRenderingHint.ClearTypeGridFit

        Dim cmb As ComboBox =
            DirectCast(sender, ComboBox)

        Dim isSelected As Boolean =
            (e.State And DrawItemState.Selected) =
            DrawItemState.Selected

        Dim backColor As Color =
            If(isSelected, UITheme.ColorSaugeClair, UITheme.ColorBeigeClair)

        Dim foreColor As Color =
            UITheme.DynamicTextFore

        Using backBrush As New SolidBrush(backColor)
            e.Graphics.FillRectangle(backBrush, e.Bounds)
        End Using

        Dim textRect As New Rectangle(
    e.Bounds.X + 4,
    e.Bounds.Y,
    e.Bounds.Width - 8,
    e.Bounds.Height
)

        TextRenderer.DrawText(
    e.Graphics,
    cmb.Items(e.Index).ToString(),
    cmb.Font,
    textRect,
    foreColor,
    TextFormatFlags.Left Or
    TextFormatFlags.VerticalCenter Or
    TextFormatFlags.SingleLine
)

        e.DrawFocusRectangle()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ComboBox_GotFocus
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Handler GotFocus pour positionner le curseur à la fin du texte dans les ComboBox éditables.
    '
    ' Paramètres :
    ' - sender : Le ComboBox source
    ' - e      : EventArgs
    '
    ' Remarques  :
    ' - Ne fait rien si DropDownStyle n'est pas DropDown (ComboBox non éditable)
    ' - Utilise BeginInvoke pour éviter les conflits de sélection
    ' - Positionne SelectionStart à Text.Length et SelectionLength à 0
    ' -------------------------------------------------------------------------------------------------
    Private Sub ComboBox_GotFocus(
    sender As Object,
    e As EventArgs
)

        Dim cmb As ComboBox =
            TryCast(sender, ComboBox)

        If cmb Is Nothing Then Return

        If cmb.DropDownStyle <> ComboBoxStyle.DropDown Then Return

        cmb.BeginInvoke(
            Sub()
                cmb.SelectionStart = cmb.Text.Length
                cmb.SelectionLength = 0
            End Sub
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ComboBox_MouseUp
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Handler MouseUp pour repositionner le curseur à la fin du texte dans les ComboBox éditables.
    '
    ' Paramètres :
    ' - sender : Le ComboBox source
    ' - e      : MouseEventArgs
    '
    ' Remarques  :
    ' - Ne fait rien si DropDownStyle n'est pas DropDown (ComboBox non éditable)
    ' - Utilise BeginInvoke pour éviter les conflits de sélection
    ' - Positionne SelectionStart à Text.Length et SelectionLength à 0
    ' -------------------------------------------------------------------------------------------------
    Private Sub ComboBox_MouseUp(
    sender As Object,
    e As MouseEventArgs
)

        Dim cmb As ComboBox =
        TryCast(sender, ComboBox)

        If cmb Is Nothing Then Return

        If cmb.DropDownStyle <> ComboBoxStyle.DropDown Then Return

        cmb.BeginInvoke(
        Sub()
            cmb.SelectionStart = cmb.Text.Length
            cmb.SelectionLength = 0
        End Sub
    )

    End Sub

#End Region

#Region "Lecture seule"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DefinirLectureSeule
    ' Version    : V1.2.0
    ' Date       : 15/10/2026
    '
    ' Rôle       :
    ' Bascule les champs d'une zone (carte, panneau…) entre consultation et modification, sans les
    ' rendre illisibles : en consultation, le texte garde sa couleur normale et seul le fond se teinte.
    '
    ' Paramètres :
    ' - conteneur    : zone dont les champs sont concernés (parcourue récursivement)
    ' - lectureSeule : True = consultation, False = modification
    '
    ' Remarques  :
    ' - Décision du 14/10/2026 : ne plus désactiver une carte entière pour la mettre en consultation
    '   (Enabled = False grise aussi les libellés et le texte des champs).
    ' - Chaque champ est traité par DefinirChampLectureSeule (V1.2.0 : extrait pour servir aussi à un
    '   champ isolé). Libellés jamais modifiés.
    ' - Les UserControl imbriqués (éditeurs riches…) et les grilles (ReadOnly propre, contrôle
    '   d'édition interne) gardent leur propre mécanisme : ignorés ici.
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirLectureSeule(conteneur As Control, lectureSeule As Boolean)

        If conteneur Is Nothing Then Return

        For Each ctrl As Control In conteneur.Controls

            If EstChampDeSaisie(ctrl) Then
                DefinirChampLectureSeule(ctrl, lectureSeule)

            ElseIf TypeOf ctrl Is Label OrElse TypeOf ctrl Is UserControl OrElse TypeOf ctrl Is DataGridView Then
                Continue For

            ElseIf ctrl.HasChildren Then
                DefinirLectureSeule(ctrl, lectureSeule)

            End If

        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DefinirChampLectureSeule
    ' Version    : V1.0.0
    ' Date       : 15/10/2026
    '
    ' Rôle       :
    ' Bascule un seul champ entre consultation et modification, selon la règle « Consultation
    ' lisible » (texte normal, fond teinté en consultation, fond blanc en saisie).
    '
    ' Paramètres :
    ' - champ        : champ concerné (sans effet si ce n'est pas un champ de saisie)
    ' - lectureSeule : True = consultation, False = modification
    '
    ' Remarques  :
    ' - AltheaDate et AltheaHeure (V1.4.0, 16/10/2026) : propriété LectureSeule. Zones de texte : ReadOnly + fond. NumericUpDown :
    '   ReadOnly, flèches et touches Haut / Bas désactivées, fond.
    ' - Listes, cases, dates Windows et boutons : Enabled. Les contrôles du kit (AltheaListe,
    '   AltheaCase) se dessinent alors lisibles ; les ComboBox, CheckBox et DateTimePicker Windows
    '   restent grisés tant qu'ils ne sont pas remplacés par ceux du kit (lot UI-7).
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirChampLectureSeule(champ As Control, lectureSeule As Boolean)

        If champ Is Nothing Then Return

        Dim fondChamp As Color = If(lectureSeule, UITheme.ColorChampLectureSeule, Color.White)

        If TypeOf champ Is AltheaDate Then
            DirectCast(champ, AltheaDate).LectureSeule = lectureSeule

        ElseIf TypeOf champ Is AltheaHeure Then
            DirectCast(champ, AltheaHeure).LectureSeule = lectureSeule

        ElseIf TypeOf champ Is TextBoxBase Then
            Dim zone As TextBoxBase = DirectCast(champ, TextBoxBase)
            zone.ReadOnly = lectureSeule
            zone.BackColor = fondChamp

        ElseIf TypeOf champ Is NumericUpDown Then
            Dim nombre As NumericUpDown = DirectCast(champ, NumericUpDown)
            nombre.ReadOnly = lectureSeule
            nombre.InterceptArrowKeys = Not lectureSeule
            nombre.BackColor = fondChamp
            ' Les flèches restent actives malgré ReadOnly : elles sont désactivées à part.
            For Each enfant As Control In nombre.Controls
                If Not TypeOf enfant Is TextBoxBase Then enfant.Enabled = Not lectureSeule
            Next

        ElseIf TypeOf champ Is ComboBox OrElse TypeOf champ Is ButtonBase OrElse
               TypeOf champ Is DateTimePicker Then
            ' ButtonBase couvre aussi CheckBox / RadioButton (et AltheaCase).
            champ.Enabled = Not lectureSeule

        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : EstChampDeSaisie
    ' Rôle       : Contrôles traités par DefinirChampLectureSeule (les autres sont ignorés ou parcourus).
    ' -------------------------------------------------------------------------------------------------
    Private Function EstChampDeSaisie(ctrl As Control) As Boolean

        Return TypeOf ctrl Is AltheaDate OrElse
               TypeOf ctrl Is AltheaHeure OrElse
               TypeOf ctrl Is TextBoxBase OrElse
               TypeOf ctrl Is NumericUpDown OrElse
               TypeOf ctrl Is ComboBox OrElse
               TypeOf ctrl Is ButtonBase OrElse
               TypeOf ctrl Is DateTimePicker

    End Function

#End Region

End Module
