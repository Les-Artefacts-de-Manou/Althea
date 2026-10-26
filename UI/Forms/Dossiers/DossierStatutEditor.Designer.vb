' -------------------------------------------------------------------------------------------------
' Fichier     : DossierStatutEditor.Designer.vb
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 14/07/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Code généré (layout) de la petite modale DossierStatutEditor : bandeau titre, statut courant,
' choix de la transition (combo des statuts cibles légaux), date de clôture (activée si besoin),
' et panneau d'actions (Valider / Annuler).
'
' Remarques   :
' - Brique B6 : changement de statut du dossier via une modale dédiée (cycle de vie).
' - Boutons colorés cohérents avec la charte → pas de ressource image, pas de .resx.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DossierStatutEditor
    Inherits System.Windows.Forms.Form

    ' Form remplace Dispose pour nettoyer la liste des composants.
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

    ' Requis par le concepteur Windows Forms.
    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DossierStatutEditor))
        enTete = New AltheaEnTete()
        carteStatut = New AltheaCarte()
        lblStatutCourantLibelle = New Label()
        lblStatutCourantValeur = New Label()
        lblNouveauStatut = New Label()
        cboStatutCible = New AltheaListe()
        lblDateCloture = New Label()
        dtpDateCloture = New AltheaDate()
        pnlActions = New AltheaBarreActions()
        btnValider = New AltheaBouton()
        btnAnnuler = New AltheaBouton()
        pnlActions.SuspendLayout()
        carteStatut.SuspendLayout()
        SuspendLayout()
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(440, 57)
        enTete.TabIndex = 0
        enTete.SousTitre = "Changer le statut"
        enTete.Text = "Dossier"
        ' 
        ' carteStatut
        ' 
        carteStatut.Controls.Add(lblStatutCourantLibelle)
        carteStatut.Controls.Add(lblStatutCourantValeur)
        carteStatut.Controls.Add(lblNouveauStatut)
        carteStatut.Controls.Add(cboStatutCible)
        carteStatut.Controls.Add(lblDateCloture)
        carteStatut.Controls.Add(dtpDateCloture)
        carteStatut.Location = New Point(12, 64)
        carteStatut.Name = "carteStatut"
        carteStatut.Padding = New Padding(12, 8, 12, 10)
        carteStatut.Size = New Size(416, 140)
        carteStatut.TabIndex = 1
        carteStatut.TabStop = False
        carteStatut.Titre = "Statut du dossier"
        ' 
        ' lblStatutCourantLibelle
        ' 
        lblStatutCourantLibelle.AutoSize = True
        lblStatutCourantLibelle.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblStatutCourantLibelle.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblStatutCourantLibelle.Location = New Point(14, 44)
        lblStatutCourantLibelle.Name = "lblStatutCourantLibelle"
        lblStatutCourantLibelle.Size = New Size(88, 17)
        lblStatutCourantLibelle.TabIndex = 1
        lblStatutCourantLibelle.Text = "Statut actuel :"
        ' 
        ' lblStatutCourantValeur
        ' 
        lblStatutCourantValeur.AutoSize = True
        lblStatutCourantValeur.Font = New Font("Calibri", 11F, FontStyle.Bold)
        lblStatutCourantValeur.ForeColor = Color.FromArgb(CByte(90), CByte(75), CByte(65))
        lblStatutCourantValeur.Location = New Point(150, 44)
        lblStatutCourantValeur.Name = "lblStatutCourantValeur"
        lblStatutCourantValeur.Size = New Size(21, 17)
        lblStatutCourantValeur.TabIndex = 2
        lblStatutCourantValeur.Text = "—"
        ' 
        ' lblNouveauStatut
        ' 
        lblNouveauStatut.AutoSize = True
        lblNouveauStatut.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblNouveauStatut.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblNouveauStatut.Location = New Point(14, 76)
        lblNouveauStatut.Name = "lblNouveauStatut"
        lblNouveauStatut.Size = New Size(102, 17)
        lblNouveauStatut.TabIndex = 3
        lblNouveauStatut.Text = "Nouveau statut :"
        ' 
        ' cboStatutCible
        ' 
        cboStatutCible.Font = New Font("Calibri", 11F)
        cboStatutCible.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboStatutCible.ItemHeight = 20
        cboStatutCible.Location = New Point(150, 72)
        cboStatutCible.Name = "cboStatutCible"
        cboStatutCible.Size = New Size(250, 26)
        cboStatutCible.TabIndex = 0
        ' 
        ' lblDateCloture
        ' 
        lblDateCloture.AutoSize = True
        lblDateCloture.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblDateCloture.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDateCloture.Location = New Point(14, 108)
        lblDateCloture.Name = "lblDateCloture"
        lblDateCloture.Size = New Size(103, 17)
        lblDateCloture.TabIndex = 4
        lblDateCloture.Text = "Date de clôture :"
        ' 
        ' dtpDateCloture
        ' 
        dtpDateCloture.Font = New Font("Calibri", 11F)
        dtpDateCloture.Location = New Point(150, 104)
        dtpDateCloture.Name = "dtpDateCloture"
        dtpDateCloture.Size = New Size(160, 26)
        dtpDateCloture.TabIndex = 1
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnValider)
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(0, 212)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(440, 52)
        pnlActions.TabIndex = 5
        ' 
        ' btnValider
        ' 
        btnValider.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnValider.Image = CType(resources.GetObject("btnValider.Image"), Image)
        btnValider.Location = New Point(189, 7)
        btnValider.Name = "btnValider"
        btnValider.Size = New Size(108, 38)
        btnValider.TabIndex = 0
        btnValider.Text = "Valider"
        ' 
        ' btnAnnuler
        ' 
        btnAnnuler.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.Location = New Point(307, 7)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(114, 38)
        btnAnnuler.TabIndex = 1
        btnAnnuler.Text = "Annuler"
        btnAnnuler.Variante = VarianteBouton.Clair
        ' 
        ' DossierStatutEditor
        ' 
        AutoScaleDimensions = New SizeF(8F, 18F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        ClientSize = New Size(440, 264)
        Controls.Add(carteStatut)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        DoubleBuffered = True
        Font = New Font("Calibri", 11F)
        FormBorderStyle = FormBorderStyle.FixedDialog
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "DossierStatutEditor"
        StartPosition = FormStartPosition.CenterParent
        Text = "Changer le statut du dossier"
        pnlActions.ResumeLayout(False)
        carteStatut.ResumeLayout(False)
        carteStatut.PerformLayout()
        ResumeLayout(False)
        PerformLayout()

    End Sub

    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteStatut As AltheaCarte
    Friend WithEvents lblStatutCourantLibelle As Label
    Friend WithEvents lblStatutCourantValeur As Label
    Friend WithEvents lblNouveauStatut As Label
    Friend WithEvents cboStatutCible As AltheaListe
    Friend WithEvents lblDateCloture As Label
    Friend WithEvents dtpDateCloture As AltheaDate
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnValider As AltheaBouton
    Friend WithEvents btnAnnuler As AltheaBouton

End Class
