' -------------------------------------------------------------------------------------------------
' Module      : UtilsControls
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 14/05/2026
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
'
' Remarques   :
' - Les couleurs viennent de UITheme
' - Module statique (méthodes partagées)
'
' Dépendances :
' - UITheme (couleurs)
' -------------------------------------------------------------------------------------------------

Public Module UtilsControls

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

End Module
