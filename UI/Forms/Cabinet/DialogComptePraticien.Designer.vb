' -------------------------------------------------------------------------------------------------
' Fichier     : DialogComptePraticien.Designer.vb
' Projet      : Althéa
'
' Rôle        :
' Fichier de conception de DialogComptePraticien. Porte la disposition et l'apparence des
' contrôles (Designer obligatoire). Les valeurs dynamiques restent dans le code-behind.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DialogComptePraticien
    Inherits Form

    Private components As System.ComponentModel.IContainer

    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DialogComptePraticien))
        enTete = New AltheaEnTete()
        carteCompte = New AltheaCarte()
        lblInfo = New Label()
        lblLogin = New Label()
        txtLogin = New TextBox()
        lblNomAffichage = New Label()
        txtNomAffichage = New TextBox()
        lblCompte = New Label()
        cboCompte = New AltheaListe()
        lblErreur = New Label()
        pnlActions = New AltheaBarreActions()
        btnOk = New AltheaBouton()
        btnAnnuler = New AltheaBouton()
        carteCompte.SuspendLayout()
        pnlActions.SuspendLayout()
        SuspendLayout()
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(520, 62)
        enTete.TabIndex = 2
        enTete.SousTitre = "Compte de travail du praticien"
        enTete.Text = "Compte praticien"
        ' 
        ' carteCompte
        ' 
        carteCompte.Controls.Add(lblInfo)
        carteCompte.Controls.Add(lblLogin)
        carteCompte.Controls.Add(txtLogin)
        carteCompte.Controls.Add(lblNomAffichage)
        carteCompte.Controls.Add(txtNomAffichage)
        carteCompte.Controls.Add(lblCompte)
        carteCompte.Controls.Add(cboCompte)
        carteCompte.Controls.Add(lblErreur)
        carteCompte.Location = New Point(12, 70)
        carteCompte.Name = "carteCompte"
        carteCompte.Padding = New Padding(12, 8, 12, 10)
        carteCompte.Size = New Size(496, 232)
        carteCompte.TabIndex = 0
        carteCompte.TabStop = False
        carteCompte.Titre = "Compte de travail"
        ' 
        ' lblInfo
        ' 
        lblInfo.Font = New Font("Calibri", 10.5F)
        lblInfo.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblInfo.Location = New Point(14, 40)
        lblInfo.Name = "lblInfo"
        lblInfo.Size = New Size(468, 70)
        lblInfo.TabIndex = 0
        ' 
        ' lblLogin
        ' 
        lblLogin.AutoSize = True
        lblLogin.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblLogin.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblLogin.Location = New Point(14, 118)
        lblLogin.Name = "lblLogin"
        lblLogin.Size = New Size(125, 17)
        lblLogin.TabIndex = 1
        lblLogin.Text = "Identifiant (login) *"
        ' 
        ' txtLogin
        ' 
        txtLogin.BackColor = Color.White
        txtLogin.Font = New Font("Calibri", 11F)
        txtLogin.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtLogin.Location = New Point(190, 114)
        txtLogin.MaxLength = 50
        txtLogin.Name = "txtLogin"
        txtLogin.Size = New Size(290, 25)
        txtLogin.TabIndex = 2
        ' 
        ' lblNomAffichage
        ' 
        lblNomAffichage.AutoSize = True
        lblNomAffichage.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblNomAffichage.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNomAffichage.Location = New Point(14, 152)
        lblNomAffichage.Name = "lblNomAffichage"
        lblNomAffichage.Size = New Size(112, 17)
        lblNomAffichage.TabIndex = 3
        lblNomAffichage.Text = "Nom d'affichage *"
        ' 
        ' txtNomAffichage
        ' 
        txtNomAffichage.BackColor = Color.White
        txtNomAffichage.Font = New Font("Calibri", 11F)
        txtNomAffichage.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtNomAffichage.Location = New Point(190, 148)
        txtNomAffichage.MaxLength = 100
        txtNomAffichage.Name = "txtNomAffichage"
        txtNomAffichage.Size = New Size(290, 25)
        txtNomAffichage.TabIndex = 4
        ' 
        ' lblCompte
        ' 
        lblCompte.AutoSize = True
        lblCompte.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblCompte.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblCompte.Location = New Point(14, 118)
        lblCompte.Name = "lblCompte"
        lblCompte.Size = New Size(165, 17)
        lblCompte.TabIndex = 5
        lblCompte.Text = "Compte SuperUser à lier *"
        lblCompte.Visible = False
        ' 
        ' cboCompte
        ' 
        cboCompte.Font = New Font("Calibri", 11F)
        cboCompte.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboCompte.ItemHeight = 20
        cboCompte.Location = New Point(190, 114)
        cboCompte.Name = "cboCompte"
        cboCompte.Size = New Size(290, 26)
        cboCompte.TabIndex = 6
        cboCompte.Visible = False
        ' 
        ' lblErreur
        ' 
        lblErreur.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblErreur.Location = New Point(14, 182)
        lblErreur.Name = "lblErreur"
        lblErreur.Size = New Size(468, 40)
        lblErreur.TabIndex = 7
        lblErreur.Visible = False
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Controls.Add(btnOk)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(0, 310)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(520, 56)
        pnlActions.TabIndex = 1
        ' 
        ' btnOk
        ' 
        btnOk.Location = New Point(240, 8)
        btnOk.Name = "btnOk"
        btnOk.Size = New Size(140, 40)
        btnOk.TabIndex = 0
        btnOk.Text = "Valider"
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.DialogResult = DialogResult.Cancel
        btnAnnuler.Location = New Point(390, 8)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(114, 40)
        btnAnnuler.TabIndex = 1
        btnAnnuler.Text = "Annuler"
        btnAnnuler.Variante = VarianteBouton.Clair
        ' 
        ' DialogComptePraticien
        ' 
        AcceptButton = btnOk
        AutoScaleMode = AutoScaleMode.None
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        CancelButton = btnAnnuler
        ClientSize = New Size(520, 366)
        Controls.Add(carteCompte)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        DoubleBuffered = True
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "DialogComptePraticien"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Compte de travail du praticien"
        carteCompte.ResumeLayout(False)
        carteCompte.PerformLayout()
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteCompte As AltheaCarte
    Friend WithEvents lblInfo As Label
    Friend WithEvents lblLogin As Label
    Friend WithEvents txtLogin As TextBox
    Friend WithEvents lblNomAffichage As Label
    Friend WithEvents txtNomAffichage As TextBox
    Friend WithEvents lblCompte As Label
    Friend WithEvents cboCompte As AltheaListe
    Friend WithEvents lblErreur As Label
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnOk As AltheaBouton
    Friend WithEvents btnAnnuler As AltheaBouton

End Class
