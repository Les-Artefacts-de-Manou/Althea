<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_RichTextEditorSimple
    Inherits System.Windows.Forms.UserControl

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_RichTextEditorSimple))
        toolStrip = New ToolStrip()
        btnBold = New ToolStripButton()
        btnItalic = New ToolStripButton()
        btnUnderline = New ToolStripButton()
        btnTextColor = New ToolStripButton()
        sep1 = New ToolStripSeparator()
        btnUndo = New ToolStripButton()
        btnRedo = New ToolStripButton()
        btnClearFormatting = New ToolStripButton()
        sep2 = New ToolStripSeparator()
        btnInsertDateTime = New ToolStripButton()
        rtbEditor = New RichTextBox()
        toolStrip.SuspendLayout()
        SuspendLayout()
        ' 
        ' toolStrip
        ' 
        toolStrip.BackColor = Color.FromArgb(CByte(178), CByte(197), CByte(186))
        toolStrip.Font = New Font("Calibri", 10F)
        toolStrip.GripStyle = ToolStripGripStyle.Hidden
        toolStrip.Items.AddRange(New ToolStripItem() {btnBold, btnItalic, btnUnderline, btnTextColor, sep1, btnUndo, btnRedo, btnClearFormatting, sep2, btnInsertDateTime})
        toolStrip.Location = New Point(0, 0)
        toolStrip.Name = "toolStrip"
        toolStrip.Size = New Size(500, 25)
        toolStrip.TabIndex = 0
        toolStrip.Text = "Barre d'outils"
        ' 
        ' btnBold
        ' 
        btnBold.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnBold.Font = New Font("Calibri", 10F, FontStyle.Bold)
        btnBold.ForeColor = Color.FromArgb(CByte(24), CByte(24), CByte(24))
        btnBold.Name = "btnBold"
        btnBold.Size = New Size(23, 22)
        btnBold.Text = "B"
        btnBold.ToolTipText = "Gras (Ctrl+B)"
        ' 
        ' btnItalic
        ' 
        btnItalic.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnItalic.Font = New Font("Calibri", 10F, FontStyle.Italic)
        btnItalic.ForeColor = Color.FromArgb(CByte(24), CByte(24), CByte(24))
        btnItalic.Name = "btnItalic"
        btnItalic.Size = New Size(23, 22)
        btnItalic.Text = "I"
        btnItalic.ToolTipText = "Italique (Ctrl+I)"
        ' 
        ' btnUnderline
        ' 
        btnUnderline.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnUnderline.Font = New Font("Calibri", 10F, FontStyle.Underline)
        btnUnderline.ForeColor = Color.FromArgb(CByte(24), CByte(24), CByte(24))
        btnUnderline.Name = "btnUnderline"
        btnUnderline.Size = New Size(23, 22)
        btnUnderline.Text = "S"
        btnUnderline.ToolTipText = "Souligné (Ctrl+U)"
        ' 
        ' btnTextColor
        ' 
        btnTextColor.BackgroundImageLayout = ImageLayout.Center
        btnTextColor.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnTextColor.ForeColor = Color.Red
        btnTextColor.ImageTransparentColor = Color.Magenta
        btnTextColor.Name = "btnTextColor"
        btnTextColor.Size = New Size(23, 22)
        btnTextColor.Text = "A"
        btnTextColor.ToolTipText = "Couleur du texte"
        ' 
        ' sep1
        ' 
        sep1.Name = "sep1"
        sep1.Size = New Size(6, 25)
        ' 
        ' btnUndo
        ' 
        btnUndo.BackgroundImage = CType(resources.GetObject("btnUndo.BackgroundImage"), Image)
        btnUndo.BackgroundImageLayout = ImageLayout.Center
        btnUndo.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnUndo.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnUndo.Name = "btnUndo"
        btnUndo.Size = New Size(23, 22)
        btnUndo.ToolTipText = "Annuler (Ctrl+Z)"
        ' 
        ' btnRedo
        ' 
        btnRedo.BackgroundImage = CType(resources.GetObject("btnRedo.BackgroundImage"), Image)
        btnRedo.BackgroundImageLayout = ImageLayout.Center
        btnRedo.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnRedo.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnRedo.Name = "btnRedo"
        btnRedo.Size = New Size(23, 22)
        btnRedo.ToolTipText = "Rétablir (Ctrl+Y)"
        ' 
        ' btnClearFormatting
        ' 
        btnClearFormatting.BackgroundImage = CType(resources.GetObject("btnClearFormatting.BackgroundImage"), Image)
        btnClearFormatting.BackgroundImageLayout = ImageLayout.Center
        btnClearFormatting.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnClearFormatting.Font = New Font("Calibri", 10F)
        btnClearFormatting.Name = "btnClearFormatting"
        btnClearFormatting.Size = New Size(23, 22)
        btnClearFormatting.ToolTipText = "Effacer le formatage"
        ' 
        ' sep2
        ' 
        sep2.Name = "sep2"
        sep2.Size = New Size(6, 25)
        ' 
        ' btnInsertDateTime
        ' 
        btnInsertDateTime.BackgroundImage = CType(resources.GetObject("btnInsertDateTime.BackgroundImage"), Image)
        btnInsertDateTime.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnInsertDateTime.Font = New Font("Calibri", 10F)
        btnInsertDateTime.Name = "btnInsertDateTime"
        btnInsertDateTime.Size = New Size(23, 22)
        btnInsertDateTime.ToolTipText = "Insérer date/heure courante"
        ' 
        ' rtbEditor
        ' 
        rtbEditor.AcceptsTab = True
        rtbEditor.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        rtbEditor.Dock = DockStyle.Fill
        rtbEditor.EnableAutoDragDrop = True
        rtbEditor.Font = New Font("Calibri", 11F)
        rtbEditor.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        rtbEditor.HideSelection = False
        rtbEditor.Location = New Point(0, 25)
        rtbEditor.Margin = New Padding(6, 5, 6, 5)
        rtbEditor.Name = "rtbEditor"
        rtbEditor.ScrollBars = RichTextBoxScrollBars.Vertical
        rtbEditor.Size = New Size(500, 175)
        rtbEditor.TabIndex = 1
        rtbEditor.Text = ""
        ' 
        ' UC_RichTextEditorSimple
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Transparent
        Controls.Add(rtbEditor)
        Controls.Add(toolStrip)
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_RichTextEditorSimple"
        Size = New Size(500, 200)
        toolStrip.ResumeLayout(False)
        toolStrip.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents toolStrip As ToolStrip
    Friend WithEvents btnBold As ToolStripButton
    Friend WithEvents btnItalic As ToolStripButton
    Friend WithEvents btnUnderline As ToolStripButton
    Friend WithEvents sep1 As ToolStripSeparator
    Friend WithEvents btnUndo As ToolStripButton
    Friend WithEvents btnRedo As ToolStripButton
    Friend WithEvents btnClearFormatting As ToolStripButton
    Friend WithEvents sep2 As ToolStripSeparator
    Friend WithEvents btnInsertDateTime As ToolStripButton
    Friend WithEvents rtbEditor As RichTextBox
    Friend WithEvents btnTextColor As ToolStripButton

End Class
