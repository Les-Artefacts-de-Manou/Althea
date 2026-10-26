' -------------------------------------------------------------------------------------------------
' Fichier     : PaiementEditor.Designer.vb
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 15/09/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Code généré (layout) de la modale PaiementEditor : bandeau titre, rappel de la séance concernée,
' champs du paiement (date, montant, mode de paiement, référence, commentaire), bouton [+] d'ajout
' de mode de paiement en contexte, et panneau d'actions (Enregistrer / Fermer).
'
' Remarques   :
' - Brique 6 (Lot 2) : modale de saisie d'un paiement affecté à une séance depuis le hub Dossier.
' - Boutons colorés cohérents avec la charte → pas de ressource image, pas de .resx.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class PaiementEditor
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PaiementEditor))
        enTete = New AltheaEnTete()
        carteEncaissement = New AltheaCarte()
        carteCommentaire = New AltheaCarte()
        lblSeance = New Label()
        lblSeanceValeur = New Label()
        lblDatePaiement = New Label()
        dtpDatePaiement = New AltheaDate()
        lblMontant = New Label()
        numMontant = New NumericUpDown()
        lblModePaiement = New Label()
        cboModePaiement = New AltheaListe()
        btnAjouterModePaiement = New AltheaBouton()
        lblReference = New Label()
        txtReference = New TextBox()
        txtCommentaire = New AltheaZoneTexte()
        lblErreur = New Label()
        btnEnregistrer = New AltheaBouton()
        btnFermer = New AltheaBouton()
        pnlActions = New AltheaBarreActions()
        CType(numMontant, ComponentModel.ISupportInitialize).BeginInit()
        pnlActions.SuspendLayout()
        carteEncaissement.SuspendLayout()
        carteCommentaire.SuspendLayout()
        SuspendLayout()
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(520, 57)
        enTete.TabIndex = 0
        enTete.SousTitre = "Encaissement d'une séance"
        enTete.Text = "Paiement"
        ' 
        ' carteEncaissement
        ' 
        carteEncaissement.Controls.Add(lblSeance)
        carteEncaissement.Controls.Add(lblSeanceValeur)
        carteEncaissement.Controls.Add(lblDatePaiement)
        carteEncaissement.Controls.Add(dtpDatePaiement)
        carteEncaissement.Controls.Add(lblMontant)
        carteEncaissement.Controls.Add(numMontant)
        carteEncaissement.Controls.Add(lblModePaiement)
        carteEncaissement.Controls.Add(cboModePaiement)
        carteEncaissement.Controls.Add(btnAjouterModePaiement)
        carteEncaissement.Controls.Add(lblReference)
        carteEncaissement.Controls.Add(txtReference)
        carteEncaissement.Controls.Add(lblErreur)
        carteEncaissement.Location = New Point(12, 64)
        carteEncaissement.Name = "carteEncaissement"
        carteEncaissement.Padding = New Padding(12, 8, 12, 10)
        carteEncaissement.Size = New Size(496, 244)
        carteEncaissement.TabIndex = 1
        carteEncaissement.TabStop = False
        carteEncaissement.Titre = "Encaissement"
        ' 
        ' carteCommentaire
        ' 
        carteCommentaire.Controls.Add(txtCommentaire)
        carteCommentaire.Location = New Point(12, 316)
        carteCommentaire.Name = "carteCommentaire"
        carteCommentaire.Padding = New Padding(12, 8, 12, 10)
        carteCommentaire.Size = New Size(496, 122)
        carteCommentaire.TabIndex = 2
        carteCommentaire.TabStop = False
        carteCommentaire.Titre = "Commentaire"
        ' 
        ' lblSeance
        ' 
        lblSeance.AutoSize = True
        lblSeance.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblSeance.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblSeance.Location = New Point(14, 44)
        lblSeance.Name = "lblSeance"
        lblSeance.Size = New Size(48, 17)
        lblSeance.TabIndex = 1
        lblSeance.Text = "Séance"
        ' 
        ' lblSeanceValeur
        ' 
        lblSeanceValeur.AutoSize = True
        lblSeanceValeur.Font = New Font("Calibri", 11F)
        lblSeanceValeur.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblSeanceValeur.Location = New Point(150, 44)
        lblSeanceValeur.Name = "lblSeanceValeur"
        lblSeanceValeur.Size = New Size(12, 17)
        lblSeanceValeur.TabIndex = 2
        lblSeanceValeur.Text = "-"
        ' 
        ' lblDatePaiement
        ' 
        lblDatePaiement.AutoSize = True
        lblDatePaiement.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblDatePaiement.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblDatePaiement.Location = New Point(14, 76)
        lblDatePaiement.Name = "lblDatePaiement"
        lblDatePaiement.Size = New Size(110, 17)
        lblDatePaiement.TabIndex = 3
        lblDatePaiement.Text = "Date du paiement"
        ' 
        ' dtpDatePaiement
        ' 
        dtpDatePaiement.Font = New Font("Calibri", 11F)
        dtpDatePaiement.Location = New Point(150, 72)
        dtpDatePaiement.Name = "dtpDatePaiement"
        dtpDatePaiement.Size = New Size(150, 26)
        dtpDatePaiement.TabIndex = 1
        ' 
        ' lblMontant
        ' 
        lblMontant.AutoSize = True
        lblMontant.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblMontant.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblMontant.Location = New Point(14, 108)
        lblMontant.Name = "lblMontant"
        lblMontant.Size = New Size(76, 17)
        lblMontant.TabIndex = 5
        lblMontant.Text = "Montant (€)"
        ' 
        ' numMontant
        ' 
        numMontant.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        numMontant.DecimalPlaces = 2
        numMontant.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        numMontant.Location = New Point(150, 104)
        numMontant.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
        numMontant.Name = "numMontant"
        numMontant.Size = New Size(150, 26)
        numMontant.TabIndex = 2
        ' 
        ' lblModePaiement
        ' 
        lblModePaiement.AutoSize = True
        lblModePaiement.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblModePaiement.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblModePaiement.Location = New Point(14, 140)
        lblModePaiement.Name = "lblModePaiement"
        lblModePaiement.Size = New Size(115, 17)
        lblModePaiement.TabIndex = 7
        lblModePaiement.Text = "Mode de paiement"
        ' 
        ' cboModePaiement
        ' 
        cboModePaiement.Font = New Font("Calibri", 11F)
        cboModePaiement.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        cboModePaiement.ItemHeight = 20
        cboModePaiement.Location = New Point(150, 136)
        cboModePaiement.Name = "cboModePaiement"
        cboModePaiement.Size = New Size(300, 26)
        cboModePaiement.TabIndex = 3
        ' 
        ' btnAjouterModePaiement
        ' 
        btnAjouterModePaiement.Image = CType(resources.GetObject("btnAjouterModePaiement.Image"), Image)
        btnAjouterModePaiement.Location = New Point(454, 134)
        btnAjouterModePaiement.Name = "btnAjouterModePaiement"
        btnAjouterModePaiement.Size = New Size(30, 30)
        btnAjouterModePaiement.TabIndex = 4
        btnAjouterModePaiement.Tag = "plus_24_normal"
        btnAjouterModePaiement.Variante = VarianteBouton.Outil
        ' 
        ' lblReference
        ' 
        lblReference.AutoSize = True
        lblReference.Font = New Font("Calibri", 10.5F, FontStyle.Bold)
        lblReference.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        lblReference.Location = New Point(14, 172)
        lblReference.Name = "lblReference"
        lblReference.Size = New Size(66, 17)
        lblReference.TabIndex = 10
        lblReference.Text = "Référence"
        ' 
        ' txtReference
        ' 
        txtReference.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtReference.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        txtReference.Location = New Point(150, 168)
        txtReference.MaxLength = 100
        txtReference.Name = "txtReference"
        txtReference.Size = New Size(300, 25)
        txtReference.TabIndex = 5
        ' 
        ' lblErreur
        ' 
        lblErreur.Font = New Font("Calibri", 10F, FontStyle.Bold)
        lblErreur.ForeColor = Color.Firebrick
        lblErreur.Location = New Point(14, 202)
        lblErreur.Name = "lblErreur"
        lblErreur.Size = New Size(468, 36)
        lblErreur.TabIndex = 7
        lblErreur.Visible = False
        ' 
        ' txtCommentaire
        ' 
        txtCommentaire.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        txtCommentaire.ForeColor = SystemColors.WindowFrame
        txtCommentaire.Location = New Point(14, 40)
        txtCommentaire.Multiline = True
        txtCommentaire.Name = "txtCommentaire"
        txtCommentaire.ScrollBars = RichTextBoxScrollBars.Vertical
        txtCommentaire.Size = New Size(468, 72)
        txtCommentaire.TabIndex = 0
        ' 
        ' btnEnregistrer
        ' 
        btnEnregistrer.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnEnregistrer.Image = CType(resources.GetObject("btnEnregistrer.Image"), Image)
        btnEnregistrer.Location = New Point(237, 7)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(132, 38)
        btnEnregistrer.TabIndex = 0
        btnEnregistrer.Tag = "Enregistrer"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.Variante = VarianteBouton.Ocre
        ' 
        ' btnFermer
        ' 
        btnFermer.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnFermer.Image = CType(resources.GetObject("btnFermer.Image"), Image)
        btnFermer.Location = New Point(379, 7)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(109, 38)
        btnFermer.TabIndex = 1
        btnFermer.Tag = "Fermer"
        btnFermer.Text = "Fermer"
        btnFermer.Variante = VarianteBouton.Clair
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnEnregistrer)
        pnlActions.Controls.Add(btnFermer)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(0, 446)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(520, 52)
        pnlActions.TabIndex = 14
        ' 
        ' PaiementEditor
        ' 
        AutoScaleDimensions = New SizeF(8F, 18F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        ClientSize = New Size(520, 498)
        Controls.Add(carteCommentaire)
        Controls.Add(carteEncaissement)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        DoubleBuffered = True
        Font = New Font("Calibri", 11F)
        FormBorderStyle = FormBorderStyle.FixedDialog
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "PaiementEditor"
        StartPosition = FormStartPosition.CenterParent
        Text = "Paiement"
        CType(numMontant, ComponentModel.ISupportInitialize).EndInit()
        pnlActions.ResumeLayout(False)
        carteEncaissement.ResumeLayout(False)
        carteEncaissement.PerformLayout()
        carteCommentaire.ResumeLayout(False)
        carteCommentaire.PerformLayout()
        ResumeLayout(False)
        PerformLayout()

    End Sub

    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteEncaissement As AltheaCarte
    Friend WithEvents carteCommentaire As AltheaCarte
    Friend WithEvents lblSeance As Label
    Friend WithEvents lblSeanceValeur As Label
    Friend WithEvents lblDatePaiement As Label
    Friend WithEvents dtpDatePaiement As AltheaDate
    Friend WithEvents lblMontant As Label
    Friend WithEvents numMontant As NumericUpDown
    Friend WithEvents lblModePaiement As Label
    Friend WithEvents cboModePaiement As AltheaListe
    Friend WithEvents btnAjouterModePaiement As AltheaBouton
    Friend WithEvents lblReference As Label
    Friend WithEvents txtReference As TextBox
    Friend WithEvents txtCommentaire As AltheaZoneTexte
    Friend WithEvents lblErreur As Label
    Friend WithEvents btnEnregistrer As AltheaBouton
    Friend WithEvents btnFermer As AltheaBouton
    Friend WithEvents pnlActions As AltheaBarreActions

End Class
