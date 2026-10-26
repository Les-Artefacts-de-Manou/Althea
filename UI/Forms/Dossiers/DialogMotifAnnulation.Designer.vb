' -------------------------------------------------------------------------------------------------
' Fichier     : DialogMotifAnnulation.Designer.vb
' Projet      : Althéa
'
' Rôle        :
' Fichier de conception de DialogMotifAnnulation. Porte la disposition et l'apparence des
' contrôles (Designer obligatoire). Les valeurs dynamiques restent dans le code-behind.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DialogMotifAnnulation
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DialogMotifAnnulation))
        lblInfo = New Label()
        lblMotif = New Label()
        txtMotif = New AltheaZoneTexte()
        lblErreur = New Label()
        btnOk = New AltheaBouton()
        btnAnnuler = New AltheaBouton()
        enTete = New AltheaEnTete()
        carteMotif = New AltheaCarte()
        pnlActions = New AltheaBarreActions()
        pnlActions.SuspendLayout()
        carteMotif.SuspendLayout()
        SuspendLayout()
        ' 
        ' lblInfo
        ' 
        lblInfo.BackColor = Color.Transparent
        lblInfo.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblInfo.Font = New Font("Calibri", 11F)
        lblInfo.Location = New Point(14, 40)
        lblInfo.Name = "lblInfo"
        lblInfo.Size = New Size(486, 82)
        lblInfo.TabIndex = 0
        ' 
        ' lblMotif
        ' 
        lblMotif.AutoSize = True
        lblMotif.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblMotif.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblMotif.Location = New Point(14, 128)
        lblMotif.Name = "lblMotif"
        lblMotif.Size = New Size(214, 17)
        lblMotif.TabIndex = 1
        lblMotif.Text = "Motif de l'annulation (obligatoire) :"
        ' 
        ' txtMotif
        ' 
        txtMotif.BackColor = Color.White
        txtMotif.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtMotif.Font = New Font("Calibri", 11F)
        txtMotif.Location = New Point(14, 152)
        txtMotif.Multiline = True
        txtMotif.Name = "txtMotif"
        txtMotif.Size = New Size(486, 64)
        txtMotif.TabIndex = 0
        ' 
        ' lblErreur
        ' 
        lblErreur.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblErreur.ForeColor = Color.Firebrick
        lblErreur.Location = New Point(14, 220)
        lblErreur.Name = "lblErreur"
        lblErreur.Size = New Size(486, 36)
        lblErreur.TabIndex = 3
        lblErreur.Text = "⚠ Le motif est obligatoire : saisissez la raison de l'annulation ci-dessus."
        lblErreur.Visible = False
        ' 
        ' btnOk
        ' 
        btnOk.Image = CType(resources.GetObject("btnOk.Image"), Image)
        btnOk.Location = New Point(111, 7)
        btnOk.Name = "btnOk"
        btnOk.Size = New Size(210, 38)
        btnOk.TabIndex = 4
        btnOk.Tag = "annuler"
        btnOk.Text = "Annuler l'encaissement"
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.DialogResult = DialogResult.Cancel
        btnAnnuler.Location = New Point(331, 7)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(109, 38)
        btnAnnuler.TabIndex = 5
        btnAnnuler.Tag = "fermer"
        btnAnnuler.Text = "Fermer"
        btnAnnuler.Variante = VarianteBouton.Clair
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(538, 52)
        enTete.TabIndex = 11
        enTete.SousTitre = "Annulez l'encaissement"
        enTete.Text = "Paiement"
        ' 
        ' carteMotif
        ' 
        carteMotif.Controls.Add(lblInfo)
        carteMotif.Controls.Add(lblMotif)
        carteMotif.Controls.Add(txtMotif)
        carteMotif.Controls.Add(lblErreur)
        carteMotif.Location = New Point(12, 64)
        carteMotif.Name = "carteMotif"
        carteMotif.Padding = New Padding(12, 8, 12, 10)
        carteMotif.Size = New Size(514, 266)
        carteMotif.TabIndex = 1
        carteMotif.TabStop = False
        carteMotif.Titre = "Motif de l'annulation"
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Controls.Add(btnOk)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(0, 338)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(538, 52)
        pnlActions.TabIndex = 12
        ' 
        ' DialogMotifAnnulation
        ' 
        AcceptButton = btnOk
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        CancelButton = btnAnnuler
        ClientSize = New Size(538, 390)
        Controls.Add(carteMotif)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        DoubleBuffered = True
        Font = New Font("Calibri", 10F)
        FormBorderStyle = FormBorderStyle.FixedDialog
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "DialogMotifAnnulation"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Annuler l'encaissement"
        pnlActions.ResumeLayout(False)
        carteMotif.ResumeLayout(False)
        carteMotif.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents lblInfo As Label
    Friend WithEvents lblMotif As Label
    Friend WithEvents txtMotif As AltheaZoneTexte
    Friend WithEvents lblErreur As Label
    Friend WithEvents btnOk As AltheaBouton
    Friend WithEvents btnAnnuler As AltheaBouton
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteMotif As AltheaCarte
    Friend WithEvents pnlActions As AltheaBarreActions

End Class
