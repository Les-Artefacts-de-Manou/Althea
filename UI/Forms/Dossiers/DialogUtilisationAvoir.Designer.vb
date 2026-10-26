' -------------------------------------------------------------------------------------------------
' Fichier     : DialogUtilisationAvoir.Designer.vb
' Projet      : Althéa
'
' Rôle        :
' Fichier de conception généré pour DialogUtilisationAvoir (ex-DialogPaiementReaffectation, 21/10/2026). Porte la disposition et
' l'apparence des contrôles (Designer obligatoire — cf. Rules.md §9). Les valeurs dynamiques
' (données liées, message contextuel, activation conditionnelle) restent dans le code-behind.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.Windows.Forms

Partial Class DialogUtilisationAvoir
    Inherits Form

    ' Conteneur des composants du concepteur.
    Private components As System.ComponentModel.IContainer

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Dispose
    ' Rôle      : Libère les ressources utilisées par le formulaire.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitializeComponent
    ' Rôle      : Compose la fenêtre (message, montant de l'avoir, choix de la séance à régler et
    '             boutons d'action). Disposition et apparence uniquement.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DialogUtilisationAvoir))
        lblMessage = New Label()
        lblAvoir = New Label()
        lblMontantAvoir = New Label()
        lblSeance = New Label()
        cboSeances = New AltheaListe()
        btnUtiliser = New AltheaBouton()
        btnGarder = New AltheaBouton()
        enTete = New AltheaEnTete()
        carteReport = New AltheaCarte()
        pnlActions = New AltheaBarreActions()
        pnlActions.SuspendLayout()
        carteReport.SuspendLayout()
        SuspendLayout()
        ' 
        ' lblMessage
        ' 
        lblMessage.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblMessage.Font = New Font("Calibri", 11F)
        lblMessage.Location = New Point(14, 40)
        lblMessage.Name = "lblMessage"
        lblMessage.Size = New Size(508, 60)
        lblMessage.TabIndex = 0
        lblMessage.Text = "-"
        ' 
        ' lblAvoir
        ' 
        lblAvoir.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblAvoir.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblAvoir.Location = New Point(14, 112)
        lblAvoir.Name = "lblAvoir"
        lblAvoir.Size = New Size(150, 24)
        lblAvoir.TabIndex = 1
        lblAvoir.Text = "Avoir disponible :"
        ' 
        ' lblMontantAvoir
        ' 
        lblMontantAvoir.Font = New Font("Calibri", 11F, FontStyle.Bold)
        lblMontantAvoir.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblMontantAvoir.Location = New Point(176, 110)
        lblMontantAvoir.Name = "lblMontantAvoir"
        lblMontantAvoir.Size = New Size(344, 24)
        lblMontantAvoir.TabIndex = 2
        lblMontantAvoir.Text = "-"
        ' 
        ' lblSeance
        ' 
        lblSeance.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblSeance.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblSeance.Location = New Point(14, 144)
        lblSeance.Name = "lblSeance"
        lblSeance.Size = New Size(150, 24)
        lblSeance.TabIndex = 3
        lblSeance.Text = "Séance à régler :"
        ' 
        ' cboSeances
        ' 
        cboSeances.DisplayMember = "LibelleChoixSeance"
        cboSeances.Font = New Font("Calibri", 11F)
        cboSeances.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboSeances.ItemHeight = 20
        cboSeances.Location = New Point(176, 140)
        cboSeances.Name = "cboSeances"
        cboSeances.Size = New Size(344, 26)
        cboSeances.TabIndex = 0
        ' 
        ' btnUtiliser
        ' 
        btnUtiliser.Image = CType(resources.GetObject("btnUtiliser.Image"), Image)
        btnUtiliser.DialogResult = DialogResult.OK
        btnUtiliser.Location = New Point(254, 7)
        btnUtiliser.Name = "btnUtiliser"
        btnUtiliser.Size = New Size(150, 38)
        btnUtiliser.TabIndex = 5
        btnUtiliser.Tag = "reporterSeance"
        btnUtiliser.Text = "Utiliser l'avoir"
        ' 
        ' btnGarder
        ' 
        btnGarder.Image = CType(resources.GetObject("btnGarder.Image"), Image)
        btnGarder.DialogResult = DialogResult.Cancel
        btnGarder.Location = New Point(414, 7)
        btnGarder.Name = "btnGarder"
        btnGarder.Size = New Size(130, 38)
        btnGarder.TabIndex = 6
        btnGarder.Tag = "retourRole"
        btnGarder.Text = "Garder l'avoir"
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(560, 60)
        enTete.TabIndex = 7
        enTete.SousTitre = "Avoir à utiliser"
        enTete.Text = "Avoir"
        ' 
        ' carteReport
        ' 
        carteReport.Controls.Add(lblMessage)
        carteReport.Controls.Add(lblAvoir)
        carteReport.Controls.Add(lblMontantAvoir)
        carteReport.Controls.Add(lblSeance)
        carteReport.Controls.Add(cboSeances)
        carteReport.Location = New Point(12, 64)
        carteReport.Name = "carteReport"
        carteReport.Padding = New Padding(12, 8, 12, 10)
        carteReport.Size = New Size(536, 176)
        carteReport.TabIndex = 1
        carteReport.TabStop = False
        carteReport.Titre = "Utilisation de l'avoir"
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnGarder)
        pnlActions.Controls.Add(btnUtiliser)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(0, 248)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(560, 52)
        pnlActions.TabIndex = 8
        ' 
        ' DialogUtilisationAvoir
        ' 
        AcceptButton = btnUtiliser
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        CancelButton = btnGarder
        ClientSize = New Size(560, 300)
        Controls.Add(carteReport)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        DoubleBuffered = True
        Font = New Font("Calibri", 10F)
        FormBorderStyle = FormBorderStyle.FixedDialog
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "DialogUtilisationAvoir"
        StartPosition = FormStartPosition.CenterParent
        Text = "Avoir à utiliser"
        pnlActions.ResumeLayout(False)
        carteReport.ResumeLayout(False)
        carteReport.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents lblMessage As Label
    Friend WithEvents lblAvoir As Label
    Friend WithEvents lblMontantAvoir As Label
    Friend WithEvents lblSeance As Label
    Friend WithEvents cboSeances As AltheaListe
    Friend WithEvents btnUtiliser As AltheaBouton
    Friend WithEvents btnGarder As AltheaBouton
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteReport As AltheaCarte
    Friend WithEvents pnlActions As AltheaBarreActions

End Class
