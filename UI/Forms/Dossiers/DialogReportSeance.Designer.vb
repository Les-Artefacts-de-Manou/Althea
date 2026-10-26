' -------------------------------------------------------------------------------------------------
' Fichier     : DialogReportSeance.Designer.vb
' Projet      : Althéa
'
' Rôle        :
' Fichier de conception de DialogReportSeance. Porte la disposition et l'apparence des
' contrôles (Designer obligatoire). Les valeurs dynamiques restent dans le code-behind.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DialogReportSeance
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DialogReportSeance))
        lblDate = New Label()
        dtpDate = New AltheaDate()
        chkHoraires = New AltheaCase()
        lblDebut = New Label()
        hreDebut = New AltheaHeure()
        lblFin = New Label()
        hreFin = New AltheaHeure()
        btnOk = New AltheaBouton()
        btnAnnuler = New AltheaBouton()
        enTete = New AltheaEnTete()
        carteReport = New AltheaCarte()
        pnlActions = New AltheaBarreActions()
        pnlActions.SuspendLayout()
        carteReport.SuspendLayout()
        SuspendLayout()
        ' 
        ' lblDate
        ' 
        lblDate.AutoSize = True
        lblDate.BackColor = Color.Transparent
        lblDate.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDate.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblDate.Location = New Point(14, 44)
        lblDate.Name = "lblDate"
        lblDate.Size = New Size(86, 17)
        lblDate.TabIndex = 1
        lblDate.Text = "Nouvelle date"
        ' 
        ' dtpDate
        ' 
        dtpDate.Font = New Font("Calibri", 11F)
        dtpDate.Location = New Point(150, 40)
        dtpDate.Name = "dtpDate"
        dtpDate.Size = New Size(150, 26)
        dtpDate.TabIndex = 0
        ' 
        ' chkHoraires
        ' 
        chkHoraires.AutoSize = True
        chkHoraires.Font = New Font("Calibri", 11F)
        chkHoraires.Location = New Point(14, 73)
        chkHoraires.Name = "chkHoraires"
        chkHoraires.Size = New Size(139, 21)
        chkHoraires.TabIndex = 1
        chkHoraires.Text = "Préciser les horaires"
        ' 
        ' lblDebut
        ' 
        lblDebut.AutoSize = True
        lblDebut.BackColor = Color.Transparent
        lblDebut.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDebut.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblDebut.Location = New Point(14, 108)
        lblDebut.Name = "lblDebut"
        lblDebut.Size = New Size(43, 17)
        lblDebut.TabIndex = 4
        lblDebut.Text = "Début"
        ' 
        ' hreDebut
        ' 
        hreDebut.Enabled = False
        hreDebut.Location = New Point(150, 104)
        hreDebut.Name = "hreDebut"
        hreDebut.Size = New Size(100, 26)
        hreDebut.TabIndex = 2
        ' 
        ' lblFin
        ' 
        lblFin.AutoSize = True
        lblFin.BackColor = Color.Transparent
        lblFin.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblFin.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblFin.Location = New Point(260, 108)
        lblFin.Name = "lblFin"
        lblFin.Size = New Size(24, 17)
        lblFin.TabIndex = 6
        lblFin.Text = "Fin"
        ' 
        ' hreFin
        ' 
        hreFin.Enabled = False
        hreFin.Location = New Point(300, 104)
        hreFin.Name = "hreFin"
        hreFin.Size = New Size(100, 26)
        hreFin.TabIndex = 3
        ' 
        ' btnOk
        ' 
        btnOk.Image = CType(resources.GetObject("btnOk.Image"), Image)
        btnOk.DialogResult = DialogResult.OK
        btnOk.Location = New Point(190, 7)
        btnOk.Name = "btnOk"
        btnOk.Size = New Size(120, 38)
        btnOk.TabIndex = 8
        btnOk.Tag = "reporterSeance"
        btnOk.Text = "Reporter"
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.DialogResult = DialogResult.Cancel
        btnAnnuler.Location = New Point(320, 7)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(114, 38)
        btnAnnuler.TabIndex = 9
        btnAnnuler.Tag = "annuler"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.Variante = VarianteBouton.Clair
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(450, 57)
        enTete.TabIndex = 10
        enTete.SousTitre = "Choisissez une nouvelle date de séance"
        enTete.Text = "Séance"
        ' 
        ' carteReport
        ' 
        carteReport.Controls.Add(lblDate)
        carteReport.Controls.Add(dtpDate)
        carteReport.Controls.Add(chkHoraires)
        carteReport.Controls.Add(lblDebut)
        carteReport.Controls.Add(hreDebut)
        carteReport.Controls.Add(lblFin)
        carteReport.Controls.Add(hreFin)
        carteReport.Location = New Point(12, 64)
        carteReport.Name = "carteReport"
        carteReport.Padding = New Padding(12, 8, 12, 10)
        carteReport.Size = New Size(426, 140)
        carteReport.TabIndex = 1
        carteReport.TabStop = False
        carteReport.Titre = "Nouvelle date de la séance"
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnOk)
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(0, 212)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(450, 52)
        pnlActions.TabIndex = 11
        ' 
        ' DialogReportSeance
        ' 
        AcceptButton = btnOk
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        CancelButton = btnAnnuler
        ClientSize = New Size(450, 264)
        Controls.Add(carteReport)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        DoubleBuffered = True
        Font = New Font("Calibri", 10F)
        ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        FormBorderStyle = FormBorderStyle.FixedDialog
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "DialogReportSeance"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Reporter la séance"
        pnlActions.ResumeLayout(False)
        carteReport.ResumeLayout(False)
        carteReport.PerformLayout()
        ResumeLayout(False)
        PerformLayout()

    End Sub
    Friend WithEvents lblDate As Label
    Friend WithEvents dtpDate As AltheaDate
    Friend WithEvents chkHoraires As AltheaCase
    Friend WithEvents lblDebut As Label
    Friend WithEvents hreDebut As AltheaHeure
    Friend WithEvents lblFin As Label
    Friend WithEvents hreFin As AltheaHeure
    Friend WithEvents btnOk As AltheaBouton
    Friend WithEvents btnAnnuler As AltheaBouton
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteReport As AltheaCarte
    Friend WithEvents pnlActions As AltheaBarreActions

End Class
