<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UC_Accueil
    Inherits System.Windows.Forms.UserControl

    'UserControl remplace la méthode Dispose pour nettoyer la liste des composants.
    <System.Diagnostics.DebuggerNonUserCode()> _
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

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_Accueil))
        pnlForm = New Panel()
        pnlCenter = New Panel()
        lblMessage = New Label()
        enTete = New AltheaEnTete()
        carteTests = New AltheaCarte()
        btnTestHash = New AltheaBouton()
        btnTestDialogChoix = New AltheaBouton()
        btnTestRichTextEditor = New AltheaBouton()
        btnTestChampDate = New AltheaBouton()
        btnTestVitrineKit = New AltheaBouton()
        btnTestErreur = New AltheaBouton()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        carteTests.SuspendLayout()
        SuspendLayout()
        ' 
        ' pnlForm
        ' 
        pnlForm.Controls.Add(pnlCenter)
        pnlForm.Controls.Add(enTete)
        pnlForm.Dock = DockStyle.Fill
        pnlForm.Location = New Point(0, 0)
        pnlForm.Name = "pnlForm"
        pnlForm.Padding = New Padding(16)
        pnlForm.Size = New Size(997, 771)
        pnlForm.TabIndex = 0
        ' 
        ' pnlCenter
        ' 
        pnlCenter.BackColor = Color.Transparent
        pnlCenter.Controls.Add(lblMessage)
        pnlCenter.Controls.Add(carteTests)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(16, 74)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(16)
        pnlCenter.Size = New Size(965, 681)
        pnlCenter.TabIndex = 5
        ' 
        ' lblMessage
        ' 
        lblMessage.AutoSize = True
        lblMessage.Font = New Font("Calibri", 12F)
        lblMessage.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblMessage.Location = New Point(16, 16)
        lblMessage.Name = "lblMessage"
        lblMessage.Size = New Size(158, 19)
        lblMessage.TabIndex = 0
        lblMessage.Text = "Bienvenue dans Althéa"
        ' 
        ' carteTests
        ' 
        carteTests.Controls.Add(btnTestHash)
        carteTests.Controls.Add(btnTestDialogChoix)
        carteTests.Controls.Add(btnTestRichTextEditor)
        carteTests.Controls.Add(btnTestChampDate)
        carteTests.Controls.Add(btnTestVitrineKit)
        carteTests.Controls.Add(btnTestErreur)
        carteTests.Location = New Point(16, 56)
        carteTests.Name = "carteTests"
        carteTests.Padding = New Padding(12, 8, 12, 10)
        carteTests.Size = New Size(808, 136)
        carteTests.TabIndex = 1
        carteTests.Titre = "Outils de test (provisoire)"
        ' 
        ' btnTestHash
        ' 
        btnTestHash.Location = New Point(12, 48)
        btnTestHash.Name = "btnTestHash"
        btnTestHash.Size = New Size(150, 32)
        btnTestHash.TabIndex = 0
        btnTestHash.Text = "Hash mot de passe"
        btnTestHash.Variante = VarianteBouton.Clair
        ' 
        ' btnTestDialogChoix
        ' 
        btnTestDialogChoix.Location = New Point(170, 48)
        btnTestDialogChoix.Name = "btnTestDialogChoix"
        btnTestDialogChoix.Size = New Size(140, 32)
        btnTestDialogChoix.TabIndex = 1
        btnTestDialogChoix.Text = "Test DialogChoix"
        btnTestDialogChoix.Variante = VarianteBouton.Clair
        ' 
        ' btnTestRichTextEditor
        ' 
        btnTestRichTextEditor.Location = New Point(318, 48)
        btnTestRichTextEditor.Name = "btnTestRichTextEditor"
        btnTestRichTextEditor.Size = New Size(160, 32)
        btnTestRichTextEditor.TabIndex = 2
        btnTestRichTextEditor.Text = "Test RichTextEditor"
        btnTestRichTextEditor.Variante = VarianteBouton.Clair
        ' 
        ' btnTestChampDate
        ' 
        btnTestChampDate.Location = New Point(486, 48)
        btnTestChampDate.Name = "btnTestChampDate"
        btnTestChampDate.Size = New Size(140, 32)
        btnTestChampDate.TabIndex = 3
        btnTestChampDate.Text = "Champ de date"
        btnTestChampDate.Variante = VarianteBouton.Clair
        ' 
        ' btnTestVitrineKit
        ' 
        btnTestVitrineKit.Location = New Point(634, 48)
        btnTestVitrineKit.Name = "btnTestVitrineKit"
        btnTestVitrineKit.Size = New Size(150, 32)
        btnTestVitrineKit.TabIndex = 4
        btnTestVitrineKit.Text = "Vitrine du kit"
        btnTestVitrineKit.Variante = VarianteBouton.Clair
        ' 
        ' btnTestErreur
        ' 
        btnTestErreur.Location = New Point(12, 88)
        btnTestErreur.Name = "btnTestErreur"
        btnTestErreur.Size = New Size(170, 32)
        btnTestErreur.TabIndex = 5
        btnTestErreur.Text = "Provoquer une erreur"
        btnTestErreur.Variante = VarianteBouton.Clair
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(16, 16)
        enTete.Name = "enTete"
        enTete.Size = New Size(965, 58)
        enTete.TabIndex = 4
        enTete.SousTitre = "Bienvenue dans Althéa"
        enTete.Text = "Accueil"
        ' 
        ' UC_Accueil
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_Accueil"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        carteTests.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        pnlCenter.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents lblMessage As Label
    Friend WithEvents carteTests As AltheaCarte
    Friend WithEvents btnTestHash As AltheaBouton
    Friend WithEvents btnTestChampDate As AltheaBouton
    Friend WithEvents btnTestVitrineKit As AltheaBouton
    Friend WithEvents btnTestDialogChoix As AltheaBouton
    Friend WithEvents btnTestRichTextEditor As AltheaBouton
    Friend WithEvents btnTestErreur As AltheaBouton

End Class
