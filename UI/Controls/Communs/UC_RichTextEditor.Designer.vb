<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_RichTextEditor
    Inherits System.Windows.Forms.UserControl

    'UserControl remplace la méthode Dispose pour nettoyer la liste des composants.
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

    'Requise par le Concepteur Windows Form
    Private components As System.ComponentModel.IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        toolStrip = New ToolStrip()
        btnCut = New ToolStripButton()
        btnCopy = New ToolStripButton()
        btnPaste = New ToolStripButton()
        sep1 = New ToolStripSeparator()
        btnBold = New ToolStripButton()
        btnItalic = New ToolStripButton()
        btnUnderline = New ToolStripButton()
        btnStrikeout = New ToolStripButton()
        btnTextColor = New ToolStripButton()
        btnHighlightColor = New ToolStripButton()
        sep2 = New ToolStripSeparator()
        lblFont = New ToolStripLabel()
        cmbFontFamily = New ToolStripComboBox()
        lblSize = New ToolStripLabel()
        cmbFontSize = New ToolStripComboBox()
        sep3 = New ToolStripSeparator()
        btnAlignLeft = New ToolStripButton()
        btnAlignCenter = New ToolStripButton()
        btnAlignRight = New ToolStripButton()
        btnBullets = New ToolStripButton()
        btnIncreaseIndent = New ToolStripButton()
        btnDecreaseIndent = New ToolStripButton()
        sep4 = New ToolStripSeparator()
        btnUndo = New ToolStripButton()
        btnRedo = New ToolStripButton()
        btnClearFormatting = New ToolStripButton()
        sep5 = New ToolStripSeparator()
        btnInsertDateTime = New ToolStripButton()
        sep6 = New ToolStripSeparator()
        btnPageSetup = New ToolStripButton()
        btnPrint = New ToolStripButton()
        btnExportPDF = New ToolStripButton()
        btnExportWord = New ToolStripButton()
        rtbEditor = New RichTextBox()
        toolStrip.SuspendLayout()
        SuspendLayout()
        ' 
        ' toolStrip
        ' 
        toolStrip.BackColor = Color.FromArgb(CByte(178), CByte(197), CByte(186))
        toolStrip.Font = New Font("Calibri", 10.0F)
        toolStrip.GripStyle = ToolStripGripStyle.Hidden
        toolStrip.Items.AddRange(New ToolStripItem() {btnCut, btnCopy, btnPaste, sep1, btnBold, btnItalic, btnUnderline, btnStrikeout, btnTextColor, btnHighlightColor, sep2, lblFont, cmbFontFamily, lblSize, cmbFontSize, sep3, btnAlignLeft, btnAlignCenter, btnAlignRight, btnBullets, btnIncreaseIndent, btnDecreaseIndent, sep4, btnUndo, btnRedo, btnClearFormatting, sep5, btnInsertDateTime, sep6, btnPageSetup, btnPrint, btnExportPDF, btnExportWord})
        toolStrip.Location = New Point(0, 0)
        toolStrip.Name = "toolStrip"
        toolStrip.Size = New Size(933, 26)
        toolStrip.TabIndex = 0
        toolStrip.Text = "Barre d'outils"
        ' 
        ' btnCut
        ' 
        btnCut.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnCut.Font = New Font("Calibri", 10.0F)
        btnCut.Name = "btnCut"
        btnCut.Size = New Size(26, 23)
        btnCut.Text = "✂"
        btnCut.ToolTipText = "Couper (Ctrl+X)"
        ' 
        ' btnCopy
        ' 
        btnCopy.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnCopy.Font = New Font("Calibri", 10.0F)
        btnCopy.Name = "btnCopy"
        btnCopy.Size = New Size(26, 23)
        btnCopy.Text = "📋"
        btnCopy.ToolTipText = "Copier (Ctrl+C)"
        ' 
        ' btnPaste
        ' 
        btnPaste.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnPaste.Font = New Font("Calibri", 10.0F)
        btnPaste.Name = "btnPaste"
        btnPaste.Size = New Size(26, 23)
        btnPaste.Text = "📄"
        btnPaste.ToolTipText = "Coller (Ctrl+V)"
        ' 
        ' sep1
        ' 
        sep1.Name = "sep1"
        sep1.Size = New Size(6, 26)
        ' 
        ' btnBold
        ' 
        btnBold.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnBold.Font = New Font("Calibri", 10.0F, FontStyle.Bold)
        btnBold.Name = "btnBold"
        btnBold.Size = New Size(23, 23)
        btnBold.Text = "B"
        btnBold.ToolTipText = "Gras (Ctrl+B)"
        ' 
        ' btnItalic
        ' 
        btnItalic.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnItalic.Font = New Font("Calibri", 10.0F, FontStyle.Italic)
        btnItalic.Name = "btnItalic"
        btnItalic.Size = New Size(23, 23)
        btnItalic.Text = "I"
        btnItalic.ToolTipText = "Italique (Ctrl+I)"
        ' 
        ' btnUnderline
        ' 
        btnUnderline.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnUnderline.Font = New Font("Calibri", 10.0F, FontStyle.Underline)
        btnUnderline.Name = "btnUnderline"
        btnUnderline.Size = New Size(23, 23)
        btnUnderline.Text = "S"
        btnUnderline.ToolTipText = "Souligné (Ctrl+U)"
        ' 
        ' btnStrikeout
        ' 
        btnStrikeout.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnStrikeout.Font = New Font("Calibri", 10.0F, FontStyle.Strikeout)
        btnStrikeout.Name = "btnStrikeout"
        btnStrikeout.Size = New Size(23, 23)
        btnStrikeout.Text = "S"
        btnStrikeout.ToolTipText = "Barré"
        ' 
        ' btnTextColor
        ' 
        btnTextColor.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnTextColor.Font = New Font("Calibri", 10.0F)
        btnTextColor.ForeColor = Color.Red
        btnTextColor.Name = "btnTextColor"
        btnTextColor.Size = New Size(23, 23)
        btnTextColor.Text = "A"
        btnTextColor.ToolTipText = "Couleur du texte"
        ' 
        ' btnHighlightColor
        ' 
        btnHighlightColor.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnHighlightColor.Font = New Font("Calibri", 10.0F)
        btnHighlightColor.Name = "btnHighlightColor"
        btnHighlightColor.Size = New Size(26, 23)
        btnHighlightColor.Text = "🖍"
        btnHighlightColor.ToolTipText = "Couleur de surbrillance"
        ' 
        ' sep2
        ' 
        sep2.Name = "sep2"
        sep2.Size = New Size(6, 26)
        ' 
        ' lblFont
        ' 
        lblFont.Name = "lblFont"
        lblFont.Size = New Size(48, 23)
        lblFont.Text = "Police :"
        ' 
        ' cmbFontFamily
        ' 
        cmbFontFamily.DropDownStyle = ComboBoxStyle.DropDownList
        cmbFontFamily.Name = "cmbFontFamily"
        cmbFontFamily.Size = New Size(140, 26)
        cmbFontFamily.ToolTipText = "Famille de police"
        ' 
        ' lblSize
        ' 
        lblSize.Name = "lblSize"
        lblSize.Size = New Size(44, 23)
        lblSize.Text = "Taille :"
        ' 
        ' cmbFontSize
        ' 
        cmbFontSize.DropDownStyle = ComboBoxStyle.DropDownList
        cmbFontSize.Name = "cmbFontSize"
        cmbFontSize.Size = New Size(87, 26)
        cmbFontSize.ToolTipText = "Taille de police"
        ' 
        ' sep3
        ' 
        sep3.Name = "sep3"
        sep3.Size = New Size(6, 26)
        ' 
        ' btnAlignLeft
        ' 
        btnAlignLeft.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnAlignLeft.Font = New Font("Calibri", 10.0F, FontStyle.Bold)
        btnAlignLeft.Name = "btnAlignLeft"
        btnAlignLeft.Size = New Size(23, 23)
        btnAlignLeft.Text = "◀"
        btnAlignLeft.ToolTipText = "Aligner à gauche"
        ' 
        ' btnAlignCenter
        ' 
        btnAlignCenter.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnAlignCenter.Font = New Font("Calibri", 10.0F, FontStyle.Bold)
        btnAlignCenter.Name = "btnAlignCenter"
        btnAlignCenter.Size = New Size(23, 23)
        btnAlignCenter.Text = "■"
        btnAlignCenter.ToolTipText = "Centrer"
        ' 
        ' btnAlignRight
        ' 
        btnAlignRight.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnAlignRight.Font = New Font("Calibri", 10.0F, FontStyle.Bold)
        btnAlignRight.Name = "btnAlignRight"
        btnAlignRight.Size = New Size(23, 23)
        btnAlignRight.Text = "▶"
        btnAlignRight.ToolTipText = "Aligner à droite"
        ' 
        ' btnBullets
        ' 
        btnBullets.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnBullets.Font = New Font("Calibri", 10.0F, FontStyle.Bold)
        btnBullets.Name = "btnBullets"
        btnBullets.Size = New Size(23, 23)
        btnBullets.Text = "•"
        btnBullets.ToolTipText = "Puces"
        ' 
        ' btnIncreaseIndent
        ' 
        btnIncreaseIndent.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnIncreaseIndent.Font = New Font("Calibri", 10.0F, FontStyle.Bold)
        btnIncreaseIndent.Name = "btnIncreaseIndent"
        btnIncreaseIndent.Size = New Size(27, 23)
        btnIncreaseIndent.Text = "-->"
        btnIncreaseIndent.ToolTipText = "Augmenter le retrait"
        ' 
        ' btnDecreaseIndent
        ' 
        btnDecreaseIndent.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnDecreaseIndent.Font = New Font("Calibri", 10.0F, FontStyle.Bold)
        btnDecreaseIndent.Name = "btnDecreaseIndent"
        btnDecreaseIndent.Size = New Size(27, 23)
        btnDecreaseIndent.Text = "<--"
        btnDecreaseIndent.ToolTipText = "Diminuer le retrait"
        ' 
        ' sep4
        ' 
        sep4.Name = "sep4"
        sep4.Size = New Size(6, 26)
        ' 
        ' btnUndo
        ' 
        btnUndo.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnUndo.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnUndo.Name = "btnUndo"
        btnUndo.Size = New Size(24, 23)
        btnUndo.Text = "↶"
        btnUndo.ToolTipText = "Annuler (Ctrl+Z)"
        ' 
        ' btnRedo
        ' 
        btnRedo.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnRedo.Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        btnRedo.Name = "btnRedo"
        btnRedo.Size = New Size(24, 23)
        btnRedo.Text = "↷"
        btnRedo.ToolTipText = "Rétablir (Ctrl+Y)"
        ' 
        ' btnClearFormatting
        ' 
        btnClearFormatting.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnClearFormatting.Font = New Font("Calibri", 10.0F)
        btnClearFormatting.Name = "btnClearFormatting"
        btnClearFormatting.Size = New Size(24, 23)
        btnClearFormatting.Text = "Tx"
        btnClearFormatting.ToolTipText = "Effacer le formatage"
        ' 
        ' sep5
        ' 
        sep5.Name = "sep5"
        sep5.Size = New Size(6, 26)
        ' 
        ' btnInsertDateTime
        ' 
        btnInsertDateTime.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnInsertDateTime.Font = New Font("Calibri", 10.0F)
        btnInsertDateTime.Name = "btnInsertDateTime"
        btnInsertDateTime.Size = New Size(26, 23)
        btnInsertDateTime.Text = "📅"
        btnInsertDateTime.ToolTipText = "Insérer date/heure"
        ' 
        ' sep6
        ' 
        sep6.Name = "sep6"
        sep6.Size = New Size(6, 26)
        ' 
        ' btnPageSetup
        ' 
        btnPageSetup.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnPageSetup.Font = New Font("Calibri", 10.0F)
        btnPageSetup.Name = "btnPageSetup"
        btnPageSetup.Size = New Size(26, 23)
        btnPageSetup.Text = "📄"
        btnPageSetup.ToolTipText = "Mise en page (marges, format, orientation)"
        ' 
        ' btnPrint
        ' 
        btnPrint.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnPrint.Font = New Font("Calibri", 10.0F)
        btnPrint.Name = "btnPrint"
        btnPrint.Size = New Size(26, 23)
        btnPrint.Text = "🖨"
        btnPrint.ToolTipText = "Imprimer (Ctrl+P)"
        ' 
        ' btnExportPDF
        ' 
        btnExportPDF.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnExportPDF.Font = New Font("Calibri", 10.0F)
        btnExportPDF.Name = "btnExportPDF"
        btnExportPDF.Size = New Size(26, 21)
        btnExportPDF.Text = "📑"
        btnExportPDF.ToolTipText = "Exporter en PDF"
        ' 
        ' btnExportWord
        ' 
        btnExportWord.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnExportWord.Font = New Font("Calibri", 10.0F)
        btnExportWord.Name = "btnExportWord"
        btnExportWord.Size = New Size(26, 21)
        btnExportWord.Text = "📝"
        btnExportWord.ToolTipText = "Exporter en Word (.docx)"
        ' 
        ' rtbEditor
        ' 
        rtbEditor.AcceptsTab = True
        rtbEditor.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        rtbEditor.Dock = DockStyle.Fill
        rtbEditor.EnableAutoDragDrop = True
        rtbEditor.Font = New Font("Calibri", 11.0F)
        rtbEditor.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        rtbEditor.HideSelection = False
        rtbEditor.Location = New Point(0, 26)
        rtbEditor.Margin = New Padding(6, 5, 6, 5)
        rtbEditor.Name = "rtbEditor"
        rtbEditor.ScrollBars = RichTextBoxScrollBars.Vertical
        rtbEditor.Size = New Size(933, 436)
        rtbEditor.TabIndex = 1
        rtbEditor.Text = ""
        ' 
        ' UC_RichTextEditor
        ' 
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Transparent
        Controls.Add(rtbEditor)
        Controls.Add(toolStrip)
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_RichTextEditor"
        Size = New Size(933, 462)
        toolStrip.ResumeLayout(False)
        toolStrip.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents toolStrip As ToolStrip
    Friend WithEvents btnCut As ToolStripButton
    Friend WithEvents btnCopy As ToolStripButton
    Friend WithEvents btnPaste As ToolStripButton
    Friend WithEvents sep1 As ToolStripSeparator
    Friend WithEvents btnBold As ToolStripButton
    Friend WithEvents btnItalic As ToolStripButton
    Friend WithEvents btnUnderline As ToolStripButton
    Friend WithEvents btnStrikeout As ToolStripButton
    Friend WithEvents btnTextColor As ToolStripButton
    Friend WithEvents btnHighlightColor As ToolStripButton
    Friend WithEvents sep2 As ToolStripSeparator
    Friend WithEvents lblFont As ToolStripLabel
    Friend WithEvents cmbFontFamily As ToolStripComboBox
    Friend WithEvents lblSize As ToolStripLabel
    Friend WithEvents cmbFontSize As ToolStripComboBox
    Friend WithEvents sep3 As ToolStripSeparator
    Friend WithEvents btnAlignLeft As ToolStripButton
    Friend WithEvents btnAlignCenter As ToolStripButton
    Friend WithEvents btnAlignRight As ToolStripButton
    Friend WithEvents btnBullets As ToolStripButton
    Friend WithEvents btnIncreaseIndent As ToolStripButton
    Friend WithEvents btnDecreaseIndent As ToolStripButton
    Friend WithEvents sep4 As ToolStripSeparator
    Friend WithEvents btnUndo As ToolStripButton
    Friend WithEvents btnRedo As ToolStripButton
    Friend WithEvents btnClearFormatting As ToolStripButton
    Friend WithEvents sep5 As ToolStripSeparator
    Friend WithEvents btnInsertDateTime As ToolStripButton
    Friend WithEvents sep6 As ToolStripSeparator
    Friend WithEvents btnPageSetup As ToolStripButton
    Friend WithEvents btnPrint As ToolStripButton
    Friend WithEvents btnExportPDF As ToolStripButton
    Friend WithEvents btnExportWord As ToolStripButton
    Friend WithEvents rtbEditor As RichTextBox

End Class
