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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_RichTextEditor))
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
        toolStrip.AutoSize = False
        toolStrip.BackColor = Color.FromArgb(CByte(178), CByte(197), CByte(186))
        toolStrip.Font = New Font("Calibri", 10F)
        toolStrip.GripStyle = ToolStripGripStyle.Hidden
        toolStrip.Items.AddRange(New ToolStripItem() {btnCut, btnCopy, btnPaste, sep1, btnBold, btnItalic, btnUnderline, btnStrikeout, btnTextColor, btnHighlightColor, sep2, lblFont, cmbFontFamily, lblSize, cmbFontSize, sep3, btnAlignLeft, btnAlignCenter, btnAlignRight, btnBullets, btnIncreaseIndent, btnDecreaseIndent, sep4, btnUndo, btnRedo, btnClearFormatting, sep5, btnInsertDateTime, sep6, btnPageSetup, btnPrint, btnExportPDF, btnExportWord})
        toolStrip.Location = New Point(0, 0)
        toolStrip.Name = "toolStrip"
        toolStrip.Size = New Size(1030, 32)
        toolStrip.TabIndex = 0
        toolStrip.Text = "Barre d'outils"
        ' 
        ' btnCut
        ' 
        btnCut.AutoSize = False
        btnCut.BackgroundImage = CType(resources.GetObject("btnCut.BackgroundImage"), Image)
        btnCut.BackgroundImageLayout = ImageLayout.Center
        btnCut.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnCut.Font = New Font("Calibri", 10F)
        btnCut.MergeAction = MergeAction.Insert
        btnCut.Name = "btnCut"
        btnCut.Size = New Size(28, 30)
        btnCut.ToolTipText = "Couper (Ctrl+X)"
        ' 
        ' btnCopy
        ' 
        btnCopy.AutoSize = False
        btnCopy.BackgroundImage = CType(resources.GetObject("btnCopy.BackgroundImage"), Image)
        btnCopy.BackgroundImageLayout = ImageLayout.Center
        btnCopy.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnCopy.Font = New Font("Calibri", 10F)
        btnCopy.Name = "btnCopy"
        btnCopy.Size = New Size(28, 30)
        btnCopy.ToolTipText = "Copier (Ctrl+C)"
        ' 
        ' btnPaste
        ' 
        btnPaste.AutoSize = False
        btnPaste.BackgroundImage = CType(resources.GetObject("btnPaste.BackgroundImage"), Image)
        btnPaste.BackgroundImageLayout = ImageLayout.Center
        btnPaste.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnPaste.Font = New Font("Calibri", 10F)
        btnPaste.Name = "btnPaste"
        btnPaste.Size = New Size(28, 30)
        btnPaste.ToolTipText = "Coller (Ctrl+V)"
        ' 
        ' sep1
        ' 
        sep1.Name = "sep1"
        sep1.Size = New Size(6, 32)
        ' 
        ' btnBold
        ' 
        btnBold.AutoSize = False
        btnBold.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnBold.Font = New Font("Calibri", 13F)
        btnBold.ForeColor = Color.FromArgb(CByte(24), CByte(24), CByte(24))
        btnBold.Name = "btnBold"
        btnBold.Size = New Size(28, 30)
        btnBold.Text = "B"
        btnBold.ToolTipText = "Gras (Ctrl+B)"
        ' 
        ' btnItalic
        ' 
        btnItalic.AutoSize = False
        btnItalic.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnItalic.Font = New Font("Calibri", 13F, FontStyle.Italic)
        btnItalic.ForeColor = Color.FromArgb(CByte(24), CByte(24), CByte(24))
        btnItalic.Name = "btnItalic"
        btnItalic.Size = New Size(28, 30)
        btnItalic.Text = "I"
        btnItalic.ToolTipText = "Italique (Ctrl+I)"
        ' 
        ' btnUnderline
        ' 
        btnUnderline.AutoSize = False
        btnUnderline.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnUnderline.Font = New Font("Calibri", 13F, FontStyle.Underline)
        btnUnderline.ForeColor = Color.FromArgb(CByte(24), CByte(24), CByte(24))
        btnUnderline.Name = "btnUnderline"
        btnUnderline.Size = New Size(28, 30)
        btnUnderline.Text = "S"
        btnUnderline.ToolTipText = "Souligné (Ctrl+U)"
        ' 
        ' btnStrikeout
        ' 
        btnStrikeout.AutoSize = False
        btnStrikeout.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnStrikeout.Font = New Font("Calibri", 13F, FontStyle.Strikeout)
        btnStrikeout.ForeColor = Color.FromArgb(CByte(24), CByte(24), CByte(24))
        btnStrikeout.Name = "btnStrikeout"
        btnStrikeout.Size = New Size(28, 30)
        btnStrikeout.Text = "S"
        btnStrikeout.ToolTipText = "Barré"
        ' 
        ' btnTextColor
        ' 
        btnTextColor.AutoSize = False
        btnTextColor.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnTextColor.Font = New Font("Calibri", 13F)
        btnTextColor.ForeColor = Color.Red
        btnTextColor.Name = "btnTextColor"
        btnTextColor.Size = New Size(28, 30)
        btnTextColor.Text = "A"
        btnTextColor.ToolTipText = "Couleur du texte"
        ' 
        ' btnHighlightColor
        ' 
        btnHighlightColor.AutoSize = False
        btnHighlightColor.BackgroundImage = CType(resources.GetObject("btnHighlightColor.BackgroundImage"), Image)
        btnHighlightColor.BackgroundImageLayout = ImageLayout.Center
        btnHighlightColor.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnHighlightColor.Font = New Font("Calibri", 10F)
        btnHighlightColor.Name = "btnHighlightColor"
        btnHighlightColor.Size = New Size(28, 30)
        btnHighlightColor.ToolTipText = "Couleur de surbrillance"
        ' 
        ' sep2
        ' 
        sep2.Name = "sep2"
        sep2.Size = New Size(6, 32)
        ' 
        ' lblFont
        ' 
        lblFont.Name = "lblFont"
        lblFont.Size = New Size(48, 29)
        lblFont.Text = "Police :"
        ' 
        ' cmbFontFamily
        ' 
        cmbFontFamily.DropDownStyle = ComboBoxStyle.DropDownList
        cmbFontFamily.Name = "cmbFontFamily"
        cmbFontFamily.Size = New Size(140, 32)
        cmbFontFamily.ToolTipText = "Famille de police"
        ' 
        ' lblSize
        ' 
        lblSize.Name = "lblSize"
        lblSize.Size = New Size(44, 29)
        lblSize.Text = "Taille :"
        ' 
        ' cmbFontSize
        ' 
        cmbFontSize.DropDownStyle = ComboBoxStyle.DropDownList
        cmbFontSize.Name = "cmbFontSize"
        cmbFontSize.Size = New Size(87, 32)
        cmbFontSize.ToolTipText = "Taille de police"
        ' 
        ' sep3
        ' 
        sep3.Name = "sep3"
        sep3.Size = New Size(6, 32)
        ' 
        ' btnAlignLeft
        ' 
        btnAlignLeft.AutoSize = False
        btnAlignLeft.BackgroundImage = CType(resources.GetObject("btnAlignLeft.BackgroundImage"), Image)
        btnAlignLeft.BackgroundImageLayout = ImageLayout.Center
        btnAlignLeft.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnAlignLeft.Font = New Font("Calibri", 10F, FontStyle.Bold)
        btnAlignLeft.Name = "btnAlignLeft"
        btnAlignLeft.Size = New Size(28, 30)
        btnAlignLeft.ToolTipText = "Aligner à gauche"
        ' 
        ' btnAlignCenter
        ' 
        btnAlignCenter.AutoSize = False
        btnAlignCenter.BackgroundImage = CType(resources.GetObject("btnAlignCenter.BackgroundImage"), Image)
        btnAlignCenter.BackgroundImageLayout = ImageLayout.Center
        btnAlignCenter.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnAlignCenter.Font = New Font("Calibri", 10F, FontStyle.Bold)
        btnAlignCenter.Name = "btnAlignCenter"
        btnAlignCenter.Size = New Size(28, 30)
        btnAlignCenter.ToolTipText = "Centrer"
        ' 
        ' btnAlignRight
        ' 
        btnAlignRight.AutoSize = False
        btnAlignRight.BackgroundImage = CType(resources.GetObject("btnAlignRight.BackgroundImage"), Image)
        btnAlignRight.BackgroundImageLayout = ImageLayout.Center
        btnAlignRight.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnAlignRight.Font = New Font("Calibri", 10F, FontStyle.Bold)
        btnAlignRight.Name = "btnAlignRight"
        btnAlignRight.Size = New Size(28, 30)
        btnAlignRight.ToolTipText = "Aligner à droite"
        ' 
        ' btnBullets
        ' 
        btnBullets.AutoSize = False
        btnBullets.BackgroundImage = CType(resources.GetObject("btnBullets.BackgroundImage"), Image)
        btnBullets.BackgroundImageLayout = ImageLayout.Center
        btnBullets.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnBullets.Font = New Font("Calibri", 10F, FontStyle.Bold)
        btnBullets.Name = "btnBullets"
        btnBullets.Size = New Size(28, 30)
        btnBullets.ToolTipText = "Puces"
        ' 
        ' btnIncreaseIndent
        ' 
        btnIncreaseIndent.AutoSize = False
        btnIncreaseIndent.BackgroundImage = CType(resources.GetObject("btnIncreaseIndent.BackgroundImage"), Image)
        btnIncreaseIndent.BackgroundImageLayout = ImageLayout.Center
        btnIncreaseIndent.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnIncreaseIndent.Font = New Font("Calibri", 10F, FontStyle.Bold)
        btnIncreaseIndent.Name = "btnIncreaseIndent"
        btnIncreaseIndent.Size = New Size(28, 30)
        btnIncreaseIndent.ToolTipText = "Augmenter le retrait"
        ' 
        ' btnDecreaseIndent
        ' 
        btnDecreaseIndent.AutoSize = False
        btnDecreaseIndent.BackgroundImage = CType(resources.GetObject("btnDecreaseIndent.BackgroundImage"), Image)
        btnDecreaseIndent.BackgroundImageLayout = ImageLayout.Center
        btnDecreaseIndent.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnDecreaseIndent.Font = New Font("Calibri", 10F, FontStyle.Bold)
        btnDecreaseIndent.Name = "btnDecreaseIndent"
        btnDecreaseIndent.Size = New Size(28, 30)
        btnDecreaseIndent.ToolTipText = "Diminuer le retrait"
        ' 
        ' sep4
        ' 
        sep4.Name = "sep4"
        sep4.Size = New Size(6, 32)
        ' 
        ' btnUndo
        ' 
        btnUndo.AutoSize = False
        btnUndo.BackgroundImage = CType(resources.GetObject("btnUndo.BackgroundImage"), Image)
        btnUndo.BackgroundImageLayout = ImageLayout.Center
        btnUndo.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnUndo.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnUndo.Name = "btnUndo"
        btnUndo.Size = New Size(28, 30)
        btnUndo.ToolTipText = "Annuler (Ctrl+Z)"
        ' 
        ' btnRedo
        ' 
        btnRedo.AutoSize = False
        btnRedo.BackgroundImage = CType(resources.GetObject("btnRedo.BackgroundImage"), Image)
        btnRedo.BackgroundImageLayout = ImageLayout.Center
        btnRedo.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnRedo.Font = New Font("Calibri", 12F, FontStyle.Bold)
        btnRedo.ImageScaling = ToolStripItemImageScaling.None
        btnRedo.ImageTransparentColor = Color.Black
        btnRedo.Name = "btnRedo"
        btnRedo.Size = New Size(28, 30)
        btnRedo.TextImageRelation = TextImageRelation.Overlay
        btnRedo.ToolTipText = "Rétablir (Ctrl+Y)"
        ' 
        ' btnClearFormatting
        ' 
        btnClearFormatting.AutoSize = False
        btnClearFormatting.BackgroundImage = CType(resources.GetObject("btnClearFormatting.BackgroundImage"), Image)
        btnClearFormatting.BackgroundImageLayout = ImageLayout.Center
        btnClearFormatting.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnClearFormatting.Font = New Font("Calibri", 10F)
        btnClearFormatting.Name = "btnClearFormatting"
        btnClearFormatting.Size = New Size(28, 29)
        btnClearFormatting.ToolTipText = "Effacer le formatage"
        ' 
        ' sep5
        ' 
        sep5.Name = "sep5"
        sep5.Size = New Size(6, 32)
        ' 
        ' btnInsertDateTime
        ' 
        btnInsertDateTime.AutoSize = False
        btnInsertDateTime.BackgroundImage = CType(resources.GetObject("btnInsertDateTime.BackgroundImage"), Image)
        btnInsertDateTime.BackgroundImageLayout = ImageLayout.Center
        btnInsertDateTime.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnInsertDateTime.Font = New Font("Calibri", 10F)
        btnInsertDateTime.Name = "btnInsertDateTime"
        btnInsertDateTime.Size = New Size(28, 30)
        btnInsertDateTime.ToolTipText = "Insérer date/heure"
        ' 
        ' sep6
        ' 
        sep6.Name = "sep6"
        sep6.Size = New Size(6, 32)
        ' 
        ' btnPageSetup
        ' 
        btnPageSetup.AutoSize = False
        btnPageSetup.BackgroundImage = CType(resources.GetObject("btnPageSetup.BackgroundImage"), Image)
        btnPageSetup.BackgroundImageLayout = ImageLayout.Center
        btnPageSetup.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnPageSetup.Font = New Font("Calibri", 10F)
        btnPageSetup.Name = "btnPageSetup"
        btnPageSetup.Size = New Size(28, 30)
        btnPageSetup.Text = "📄"
        btnPageSetup.ToolTipText = "Mise en page (marges, format, orientation)"
        ' 
        ' btnPrint
        ' 
        btnPrint.AutoSize = False
        btnPrint.BackgroundImage = CType(resources.GetObject("btnPrint.BackgroundImage"), Image)
        btnPrint.BackgroundImageLayout = ImageLayout.Center
        btnPrint.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnPrint.Font = New Font("Calibri", 10F)
        btnPrint.Name = "btnPrint"
        btnPrint.Size = New Size(28, 30)
        btnPrint.ToolTipText = "Imprimer (Ctrl+P)"
        ' 
        ' btnExportPDF
        ' 
        btnExportPDF.AutoSize = False
        btnExportPDF.BackgroundImage = CType(resources.GetObject("btnExportPDF.BackgroundImage"), Image)
        btnExportPDF.BackgroundImageLayout = ImageLayout.Center
        btnExportPDF.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnExportPDF.Font = New Font("Calibri", 10F)
        btnExportPDF.Name = "btnExportPDF"
        btnExportPDF.Size = New Size(28, 30)
        btnExportPDF.ToolTipText = "Exporter en PDF"
        ' 
        ' btnExportWord
        ' 
        btnExportWord.AutoSize = False
        btnExportWord.BackgroundImage = CType(resources.GetObject("btnExportWord.BackgroundImage"), Image)
        btnExportWord.BackgroundImageLayout = ImageLayout.Center
        btnExportWord.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnExportWord.Font = New Font("Calibri", 10F)
        btnExportWord.Name = "btnExportWord"
        btnExportWord.Size = New Size(28, 30)
        btnExportWord.ToolTipText = "Exporter en Word (.docx)"
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
        rtbEditor.Location = New Point(0, 32)
        rtbEditor.Margin = New Padding(6, 5, 6, 5)
        rtbEditor.Name = "rtbEditor"
        rtbEditor.ScrollBars = RichTextBoxScrollBars.Vertical
        rtbEditor.Size = New Size(1030, 430)
        rtbEditor.TabIndex = 1
        rtbEditor.Text = ""
        ' 
        ' UC_RichTextEditor
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Transparent
        Controls.Add(rtbEditor)
        Controls.Add(toolStrip)
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_RichTextEditor"
        Size = New Size(1030, 462)
        toolStrip.ResumeLayout(False)
        toolStrip.PerformLayout()
        ResumeLayout(False)
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
