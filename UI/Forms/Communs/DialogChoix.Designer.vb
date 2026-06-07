<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DialogChoix
    Inherits System.Windows.Forms.Form

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DialogChoix))
        pnlMain = New Panel()
        picIcone = New PictureBox()
        pnlContenu = New Panel()
        lblMessage = New Label()
        pnlIcone = New Panel()
        pnlBoutons = New Panel()
        btn3 = New Button()
        btn2 = New Button()
        btn1 = New Button()
        pnlMain.SuspendLayout()
        CType(picIcone, ComponentModel.ISupportInitialize).BeginInit()
        pnlContenu.SuspendLayout()
        pnlBoutons.SuspendLayout()
        SuspendLayout()
        ' 
        ' pnlMain
        ' 
        pnlMain.BackgroundImage = My.Resources.Resources.Fond_480x240
        pnlMain.BackgroundImageLayout = ImageLayout.Center
        pnlMain.Controls.Add(picIcone)
        pnlMain.Controls.Add(pnlIcone)
        pnlMain.Controls.Add(pnlContenu)
        pnlMain.Controls.Add(pnlBoutons)
        pnlMain.Dock = DockStyle.Fill
        pnlMain.Location = New Point(0, 0)
        pnlMain.Name = "pnlMain"
        pnlMain.Size = New Size(464, 212)
        pnlMain.TabIndex = 0
        ' 
        ' picIcone
        ' 
        picIcone.Anchor = AnchorStyles.None
        picIcone.Location = New Point(12, 48)
        picIcone.Name = "picIcone"
        picIcone.Size = New Size(64, 64)
        picIcone.SizeMode = PictureBoxSizeMode.Zoom
        picIcone.TabIndex = 0
        picIcone.TabStop = False
        ' 
        ' pnlContenu
        ' 
        pnlContenu.BackColor = Color.Transparent
        pnlContenu.Controls.Add(lblMessage)
        pnlContenu.Dock = DockStyle.Right
        pnlContenu.Location = New Point(93, 0)
        pnlContenu.Name = "pnlContenu"
        pnlContenu.Padding = New Padding(13, 19, 13, 9)
        pnlContenu.Size = New Size(371, 156)
        pnlContenu.TabIndex = 0
        ' 
        ' lblMessage
        ' 
        lblMessage.BackColor = Color.Transparent
        lblMessage.Font = New Font("Segoe UI", 10F)
        lblMessage.Location = New Point(0, 17)
        lblMessage.Name = "lblMessage"
        lblMessage.Size = New Size(359, 120)
        lblMessage.TabIndex = 0
        lblMessage.Text = "Message de confirmation"
        lblMessage.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' pnlIcone
        ' 
        pnlIcone.BackColor = Color.Transparent
        pnlIcone.Location = New Point(12, 140)
        pnlIcone.Name = "pnlIcone"
        pnlIcone.Size = New Size(440, 10)
        pnlIcone.TabIndex = 1
        ' 
        ' pnlBoutons
        ' 
        pnlBoutons.BackColor = Color.Transparent
        pnlBoutons.Controls.Add(btn3)
        pnlBoutons.Controls.Add(btn2)
        pnlBoutons.Controls.Add(btn1)
        pnlBoutons.Dock = DockStyle.Bottom
        pnlBoutons.Location = New Point(0, 156)
        pnlBoutons.Name = "pnlBoutons"
        pnlBoutons.Padding = New Padding(13, 9, 13, 14)
        pnlBoutons.Size = New Size(464, 56)
        pnlBoutons.TabIndex = 2
        ' 
        ' btn3
        ' 
        btn3.Anchor = AnchorStyles.Right
        btn3.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btn3.BackgroundImageLayout = ImageLayout.None
        btn3.DialogResult = DialogResult.Cancel
        btn3.FlatStyle = FlatStyle.Flat
        btn3.ForeColor = Color.White
        btn3.Image = CType(resources.GetObject("btn3.Image"), Image)
        btn3.ImageAlign = ContentAlignment.MiddleLeft
        btn3.Location = New Point(259, 7)
        btn3.Name = "btn3"
        btn3.Size = New Size(90, 42)
        btn3.TabIndex = 2
        btn3.Tag = "annuler_normal"
        btn3.Text = "Annuler"
        btn3.TextAlign = ContentAlignment.MiddleLeft
        btn3.TextImageRelation = TextImageRelation.ImageBeforeText
        btn3.UseVisualStyleBackColor = False
        ' 
        ' btn2
        ' 
        btn2.Anchor = AnchorStyles.Right
        btn2.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btn2.FlatStyle = FlatStyle.Flat
        btn2.ForeColor = Color.White
        btn2.Image = CType(resources.GetObject("btn2.Image"), Image)
        btn2.ImageAlign = ContentAlignment.MiddleLeft
        btn2.Location = New Point(182, 7)
        btn2.Name = "btn2"
        btn2.Size = New Size(73, 42)
        btn2.TabIndex = 1
        btn2.Tag = "no_normal"
        btn2.Text = "Non"
        btn2.TextAlign = ContentAlignment.MiddleLeft
        btn2.TextImageRelation = TextImageRelation.ImageBeforeText
        btn2.UseVisualStyleBackColor = False
        ' 
        ' btn1
        ' 
        btn1.Anchor = AnchorStyles.Right
        btn1.BackColor = Color.FromArgb(CByte(122), CByte(155), CByte(135))
        btn1.FlatStyle = FlatStyle.Flat
        btn1.ForeColor = Color.White
        btn1.Image = CType(resources.GetObject("btn1.Image"), Image)
        btn1.ImageAlign = ContentAlignment.MiddleLeft
        btn1.Location = New Point(107, 7)
        btn1.Name = "btn1"
        btn1.Size = New Size(71, 42)
        btn1.TabIndex = 0
        btn1.Tag = "yes_normal"
        btn1.Text = "Oui"
        btn1.TextAlign = ContentAlignment.MiddleLeft
        btn1.TextImageRelation = TextImageRelation.ImageBeforeText
        btn1.UseVisualStyleBackColor = False
        ' 
        ' DialogChoix
        ' 
        AutoScaleDimensions = New SizeF(6F, 14F)
        AutoScaleMode = AutoScaleMode.Font
        CancelButton = btn3
        ClientSize = New Size(464, 212)
        Controls.Add(pnlMain)
        Font = New Font("Calibri", 9F)
        FormBorderStyle = FormBorderStyle.FixedDialog
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "DialogChoix"
        ShowIcon = False
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Confirmation"
        pnlMain.ResumeLayout(False)
        CType(picIcone, ComponentModel.ISupportInitialize).EndInit()
        pnlContenu.ResumeLayout(False)
        pnlBoutons.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlMain As Panel
    Friend WithEvents pnlContenu As Panel
    Friend WithEvents lblMessage As Label
    Friend WithEvents pnlIcone As Panel
    Friend WithEvents picIcone As PictureBox
    Friend WithEvents pnlBoutons As Panel
    Friend WithEvents btn1 As Button
    Friend WithEvents btn2 As Button
    Friend WithEvents btn3 As Button

End Class
