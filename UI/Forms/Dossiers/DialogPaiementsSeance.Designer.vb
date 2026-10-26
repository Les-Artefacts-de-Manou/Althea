' -------------------------------------------------------------------------------------------------
' Fichier     : DialogPaiementsSeance.Designer.vb
' Projet      : Althéa
'
' Rôle        :
' Fichier de conception de DialogPaiementsSeance (disposition des contrôles).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DialogPaiementsSeance
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DialogPaiementsSeance))
        lblResume = New Label()
        dgvPaiements = New DataGridView()
        colDate = New DataGridViewTextBoxColumn()
        colCode = New DataGridViewTextBoxColumn()
        colMouvement = New DataGridViewTextBoxColumn()
        colMontant = New DataGridViewTextBoxColumn()
        colAffecte = New DataGridViewTextBoxColumn()
        colMode = New DataGridViewTextBoxColumn()
        colStatut = New DataGridViewTextBoxColumn()
        btnFermer = New AltheaBouton()
        enTete = New AltheaEnTete()
        carteSeance = New AltheaCarte()
        pnlActions = New AltheaBarreActions()
        CType(dgvPaiements, System.ComponentModel.ISupportInitialize).BeginInit()
        pnlActions.SuspendLayout()
        carteSeance.SuspendLayout()
        SuspendLayout()
        ' 
        ' lblResume
        ' 
        lblResume.BackColor = Color.Transparent
        lblResume.ForeColor = Color.FromArgb(74, 74, 74)
        lblResume.Font = New Font("Calibri", 11F)
        lblResume.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblResume.Location = New Point(14, 40)
        lblResume.Name = "lblResume"
        lblResume.Size = New Size(740, 44)
        lblResume.TabIndex = 1
        lblResume.Text = "-"
        ' 
        ' dgvPaiements
        ' 
        dgvPaiements.AllowUserToAddRows = False
        dgvPaiements.AllowUserToDeleteRows = False
        dgvPaiements.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        dgvPaiements.BackgroundColor = Color.White
        dgvPaiements.Columns.AddRange(New DataGridViewColumn() {colDate, colCode, colMouvement, colMontant, colAffecte, colMode, colStatut})
        dgvPaiements.Location = New Point(14, 88)
        dgvPaiements.MultiSelect = False
        dgvPaiements.Name = "dgvPaiements"
        dgvPaiements.ReadOnly = True
        dgvPaiements.RowHeadersVisible = False
        dgvPaiements.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPaiements.Size = New Size(740, 283)
        dgvPaiements.TabIndex = 0
        ' 
        ' colDate
        ' 
        colDate.DataPropertyName = "DatePaiement"
        colDate.DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "dd/MM/yyyy"}
        colDate.HeaderText = "Date"
        colDate.Name = "colDate"
        colDate.ReadOnly = True
        colDate.Width = 90
        ' 
        ' colCode
        ' 
        colCode.DataPropertyName = "CodePaiement"
        colCode.HeaderText = "Code"
        colCode.Name = "colCode"
        colCode.ReadOnly = True
        colCode.Width = 90
        ' 
        ' colMouvement
        ' 
        colMouvement.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colMouvement.DataPropertyName = "MouvementAffichage"
        colMouvement.HeaderText = "Mouvement"
        colMouvement.Name = "colMouvement"
        colMouvement.ReadOnly = True
        ' 
        ' colMontant
        ' 
        colMontant.DataPropertyName = "Montant"
        colMontant.DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "N2", .Alignment = DataGridViewContentAlignment.MiddleRight}
        colMontant.HeaderText = "Montant"
        colMontant.Name = "colMontant"
        colMontant.ReadOnly = True
        colMontant.Width = 80
        ' 
        ' colAffecte
        ' 
        colAffecte.DataPropertyName = "MontantAffecte"
        colAffecte.DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "N2", .Alignment = DataGridViewContentAlignment.MiddleRight}
        colAffecte.HeaderText = "Affecté séance"
        colAffecte.Name = "colAffecte"
        colAffecte.ReadOnly = True
        colAffecte.Width = 100
        ' 
        ' colMode
        ' 
        colMode.DataPropertyName = "ModePaiement"
        colMode.HeaderText = "Mode"
        colMode.Name = "colMode"
        colMode.ReadOnly = True
        colMode.Width = 100
        ' 
        ' colStatut
        ' 
        colStatut.DataPropertyName = "LibelleStatutPaiement"
        colStatut.HeaderText = "État"
        colStatut.Name = "colStatut"
        colStatut.ReadOnly = True
        colStatut.Width = 200
        ' 
        ' btnFermer
        ' 
        btnFermer.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnFermer.Image = CType(resources.GetObject("btnFermer.Image"), Image)
        btnFermer.DialogResult = DialogResult.Cancel
        btnFermer.Location = New Point(657, 7)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(119, 38)
        btnFermer.TabIndex = 3
        btnFermer.Tag = "fermer"
        btnFermer.Text = "Fermer"
        btnFermer.Variante = VarianteBouton.Clair
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(792, 56)
        enTete.TabIndex = 8
        enTete.SousTitre = "Détail des paiements d'une séance"
        enTete.Text = "Paiement"
        ' 
        ' carteSeance
        ' 
        carteSeance.Controls.Add(lblResume)
        carteSeance.Controls.Add(dgvPaiements)
        carteSeance.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        carteSeance.Location = New Point(12, 64)
        carteSeance.Name = "carteSeance"
        carteSeance.Padding = New Padding(12, 8, 12, 10)
        carteSeance.Size = New Size(768, 381)
        carteSeance.TabIndex = 1
        carteSeance.TabStop = False
        carteSeance.Titre = "Paiements de la séance"
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnFermer)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(0, 453)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(792, 52)
        pnlActions.TabIndex = 9
        ' 
        ' DialogPaiementsSeance
        ' 
        BackColor = Color.FromArgb(244, 239, 234)
        CancelButton = btnFermer
        ClientSize = New Size(792, 505)
        Controls.Add(carteSeance)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        DoubleBuffered = True
        Font = New Font("Calibri", 10F)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MinimizeBox = False
        MinimumSize = New Size(600, 300)
        Name = "DialogPaiementsSeance"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Détail des paiements de la séance"
        CType(dgvPaiements, System.ComponentModel.ISupportInitialize).EndInit()
        pnlActions.ResumeLayout(False)
        carteSeance.ResumeLayout(False)
        carteSeance.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents lblResume As Label
    Friend WithEvents dgvPaiements As DataGridView
    Friend WithEvents colDate As DataGridViewTextBoxColumn
    Friend WithEvents colCode As DataGridViewTextBoxColumn
    Friend WithEvents colMouvement As DataGridViewTextBoxColumn
    Friend WithEvents colMontant As DataGridViewTextBoxColumn
    Friend WithEvents colAffecte As DataGridViewTextBoxColumn
    Friend WithEvents colMode As DataGridViewTextBoxColumn
    Friend WithEvents colStatut As DataGridViewTextBoxColumn
    Friend WithEvents btnFermer As AltheaBouton
    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteSeance As AltheaCarte
    Friend WithEvents pnlActions As AltheaBarreActions

End Class
