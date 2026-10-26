<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UC_Parametres
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_Parametres))
        pnlForm = New Panel()
        pnlCenter = New Panel()
        pnlPage = New Panel()
        pnlActions = New AltheaBarreActions()
        btnAnnuler = New AltheaBouton()
        btnEnregistrer = New AltheaBouton()
        btnModifier = New AltheaBouton()
        pnlTop = New Panel()
        chkAfficherInactifs = New AltheaCase()
        enTete = New AltheaEnTete()
        pnlForm.SuspendLayout()
        pnlCenter.SuspendLayout()
        pnlActions.SuspendLayout()
        pnlTop.SuspendLayout()
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
        pnlForm.TabIndex = 1
        ' 
        ' pnlCenter
        ' 
        pnlCenter.BackColor = Color.Transparent
        pnlCenter.Controls.Add(pnlPage)
        pnlCenter.Controls.Add(pnlActions)
        pnlCenter.Controls.Add(pnlTop)
        pnlCenter.Dock = DockStyle.Fill
        pnlCenter.Location = New Point(16, 74)
        pnlCenter.Name = "pnlCenter"
        pnlCenter.Padding = New Padding(16)
        pnlCenter.Size = New Size(965, 681)
        pnlCenter.TabIndex = 5
        ' 
        ' pnlPage
        ' 
        pnlPage.AutoScroll = True
        pnlPage.BackColor = Color.Transparent
        pnlPage.Dock = DockStyle.Fill
        pnlPage.Location = New Point(16, 58)
        pnlPage.Name = "pnlPage"
        pnlPage.Size = New Size(933, 553)
        pnlPage.TabIndex = 20
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Controls.Add(btnEnregistrer)
        pnlActions.Controls.Add(btnModifier)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Font = New Font("Calibri", 10F)
        pnlActions.Location = New Point(16, 611)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(933, 54)
        pnlActions.TabIndex = 19
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.Location = New Point(593, 7)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(114, 40)
        btnAnnuler.TabIndex = 10
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.Variante = VarianteBouton.Clair
        ' 
        ' btnEnregistrer
        ' 
        btnEnregistrer.Image = CType(resources.GetObject("btnEnregistrer.Image"), Image)
        btnEnregistrer.Location = New Point(451, 7)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(132, 40)
        btnEnregistrer.TabIndex = 9
        btnEnregistrer.Tag = "enregistrer_normal"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.Variante = VarianteBouton.Ocre
        ' 
        ' btnModifier
        ' 
        btnModifier.Image = CType(resources.GetObject("btnModifier.Image"), Image)
        btnModifier.Location = New Point(322, 7)
        btnModifier.Name = "btnModifier"
        btnModifier.Size = New Size(119, 40)
        btnModifier.TabIndex = 7
        btnModifier.Tag = "modifier_normal"
        btnModifier.Text = "Modifier"
        ' 
        ' pnlTop
        ' 
        pnlTop.BackColor = Color.Transparent
        pnlTop.Controls.Add(chkAfficherInactifs)
        pnlTop.Dock = DockStyle.Top
        pnlTop.Font = New Font("Calibri", 11F)
        pnlTop.Location = New Point(16, 16)
        pnlTop.Name = "pnlTop"
        pnlTop.Padding = New Padding(8)
        pnlTop.Size = New Size(933, 42)
        pnlTop.TabIndex = 6
        ' 
        ' chkAfficherInactifs
        ' 
        chkAfficherInactifs.AutoSize = False
        chkAfficherInactifs.Location = New Point(636, 11)
        chkAfficherInactifs.Name = "chkAfficherInactifs"
        chkAfficherInactifs.Size = New Size(270, 24)
        chkAfficherInactifs.TabIndex = 1
        chkAfficherInactifs.Text = "Afficher les paramètres désactivés"
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(16, 16)
        enTete.Name = "enTete"
        enTete.Size = New Size(965, 58)
        enTete.TabIndex = 4
        enTete.SousTitre = "Réglages de l'application : dossiers, séances, sauvegardes, sécurité"
        enTete.Text = "Paramètres"
        ' 
        ' UC_Parametres
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        BackgroundImageLayout = ImageLayout.Center
        Controls.Add(pnlForm)
        Font = New Font("Calibri", 10F)
        Margin = New Padding(4, 3, 4, 3)
        Name = "UC_Parametres"
        Size = New Size(997, 771)
        pnlForm.ResumeLayout(False)
        pnlCenter.ResumeLayout(False)
        pnlActions.ResumeLayout(False)
        pnlTop.ResumeLayout(False)
        pnlTop.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlForm As Panel
    Friend WithEvents pnlCenter As Panel
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents pnlTop As Panel
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnModifier As AltheaBouton
    Friend WithEvents pnlPage As Panel
    Friend WithEvents btnAnnuler As AltheaBouton
    Friend WithEvents btnEnregistrer As AltheaBouton
    Friend WithEvents chkAfficherInactifs As AltheaCase

End Class
