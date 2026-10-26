' -------------------------------------------------------------------------------------------------
' Designer    : TestDialogChoix
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 14/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Fenêtre de démonstration de DialogChoix V2 : un bouton par cas, le résultat renvoyé, et une
' planche des icônes d'état (AltheaIconeEtat) en deux tailles, pour les tests et les captures.
' -------------------------------------------------------------------------------------------------

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TestDialogChoix
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
        enTete = New AltheaEnTete()
        carteCas = New AltheaCarte()
        btnInformation = New AltheaBouton()
        btnAvertissement = New AltheaBouton()
        btnErreur = New AltheaBouton()
        btnSucces = New AltheaBouton()
        btnConfirmerOuiNon = New AltheaBouton()
        btnQuestion = New AltheaBouton()
        btnConfirmerAction = New AltheaBouton()
        btnSuppression = New AltheaBouton()
        btnSuppressionAttention = New AltheaBouton()
        btnRecapitulatif = New AltheaBouton()
        btnQuotaDepasse = New AltheaBouton()
        btnTitreLong = New AltheaBouton()
        btnMessageTresLong = New AltheaBouton()
        btnAttente = New AltheaBouton()
        carteIcones = New AltheaCarte()
        iconeInformation48 = New AltheaIconeEtat()
        iconeInformation24 = New AltheaIconeEtat()
        lblIconeInformation = New Label()
        iconeAvertissement48 = New AltheaIconeEtat()
        iconeAvertissement24 = New AltheaIconeEtat()
        lblIconeAvertissement = New Label()
        iconeErreur48 = New AltheaIconeEtat()
        iconeErreur24 = New AltheaIconeEtat()
        lblIconeErreur = New Label()
        iconeQuestion48 = New AltheaIconeEtat()
        iconeQuestion24 = New AltheaIconeEtat()
        lblIconeQuestion = New Label()
        iconeSucces48 = New AltheaIconeEtat()
        iconeSucces24 = New AltheaIconeEtat()
        lblIconeSucces = New Label()
        iconeAttente48 = New AltheaIconeEtat()
        iconeAttente24 = New AltheaIconeEtat()
        lblIconeAttente = New Label()
        lblResultat = New Label()
        barreActions = New AltheaBarreActions()
        btnFermer = New AltheaBouton()
        carteCas.SuspendLayout()
        carteIcones.SuspendLayout()
        barreActions.SuspendLayout()
        SuspendLayout()
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(980, 58)
        enTete.SousTitre = "Un bouton par cas : testez Entrée, Échap, le déplacement par l'en-tête et la taille de la fenêtre."
        enTete.TabIndex = 0
        enTete.Text = "Démonstration de DialogChoix"
        ' 
        ' carteCas
        ' 
        carteCas.Controls.Add(btnInformation)
        carteCas.Controls.Add(btnAvertissement)
        carteCas.Controls.Add(btnErreur)
        carteCas.Controls.Add(btnSucces)
        carteCas.Controls.Add(btnConfirmerOuiNon)
        carteCas.Controls.Add(btnQuestion)
        carteCas.Controls.Add(btnConfirmerAction)
        carteCas.Controls.Add(btnSuppression)
        carteCas.Controls.Add(btnSuppressionAttention)
        carteCas.Controls.Add(btnRecapitulatif)
        carteCas.Controls.Add(btnQuotaDepasse)
        carteCas.Controls.Add(btnTitreLong)
        carteCas.Controls.Add(btnMessageTresLong)
        carteCas.Controls.Add(btnAttente)
        carteCas.Location = New Point(20, 74)
        carteCas.Name = "carteCas"
        carteCas.Size = New Size(600, 400)
        carteCas.TabIndex = 1
        carteCas.Titre = "Cas de dialogue"
        ' 
        ' btnInformation
        ' 
        btnInformation.Font = New Font("Calibri", 11.25F)
        btnInformation.Location = New Point(18, 46)
        btnInformation.Name = "btnInformation"
        btnInformation.Size = New Size(274, 36)
        btnInformation.TabIndex = 0
        btnInformation.Text = "Information"
        btnInformation.Variante = VarianteBouton.Clair
        ' 
        ' btnAvertissement
        ' 
        btnAvertissement.Font = New Font("Calibri", 11.25F)
        btnAvertissement.Location = New Point(18, 94)
        btnAvertissement.Name = "btnAvertissement"
        btnAvertissement.Size = New Size(274, 36)
        btnAvertissement.TabIndex = 1
        btnAvertissement.Text = "Avertissement"
        btnAvertissement.Variante = VarianteBouton.Clair
        ' 
        ' btnErreur
        ' 
        btnErreur.Font = New Font("Calibri", 11.25F)
        btnErreur.Location = New Point(18, 142)
        btnErreur.Name = "btnErreur"
        btnErreur.Size = New Size(274, 36)
        btnErreur.TabIndex = 2
        btnErreur.Text = "Erreur"
        btnErreur.Variante = VarianteBouton.Clair
        ' 
        ' btnSucces
        ' 
        btnSucces.Font = New Font("Calibri", 11.25F)
        btnSucces.Location = New Point(18, 190)
        btnSucces.Name = "btnSucces"
        btnSucces.Size = New Size(274, 36)
        btnSucces.TabIndex = 3
        btnSucces.Text = "Succès"
        btnSucces.Variante = VarianteBouton.Clair
        ' 
        ' btnConfirmerOuiNon
        ' 
        btnConfirmerOuiNon.Font = New Font("Calibri", 11.25F)
        btnConfirmerOuiNon.Location = New Point(18, 238)
        btnConfirmerOuiNon.Name = "btnConfirmerOuiNon"
        btnConfirmerOuiNon.Size = New Size(274, 36)
        btnConfirmerOuiNon.TabIndex = 4
        btnConfirmerOuiNon.Text = "Confirmer (Oui / Non)"
        btnConfirmerOuiNon.Variante = VarianteBouton.Clair
        ' 
        ' btnQuestion
        ' 
        btnQuestion.Font = New Font("Calibri", 11.25F)
        btnQuestion.Location = New Point(18, 286)
        btnQuestion.Name = "btnQuestion"
        btnQuestion.Size = New Size(274, 36)
        btnQuestion.TabIndex = 5
        btnQuestion.Text = "Question (Oui / Non / Annuler)"
        btnQuestion.Variante = VarianteBouton.Clair
        ' 
        ' btnConfirmerAction
        ' 
        btnConfirmerAction.Font = New Font("Calibri", 11.25F)
        btnConfirmerAction.Location = New Point(18, 334)
        btnConfirmerAction.Name = "btnConfirmerAction"
        btnConfirmerAction.Size = New Size(274, 36)
        btnConfirmerAction.TabIndex = 6
        btnConfirmerAction.Text = "Confirmer une action"
        btnConfirmerAction.Variante = VarianteBouton.Clair
        ' 
        ' btnSuppression
        ' 
        btnSuppression.Font = New Font("Calibri", 11.25F)
        btnSuppression.Location = New Point(304, 46)
        btnSuppression.Name = "btnSuppression"
        btnSuppression.Size = New Size(274, 36)
        btnSuppression.TabIndex = 7
        btnSuppression.Text = "Suppression (Entrée = Annuler)"
        btnSuppression.Variante = VarianteBouton.Clair
        ' 
        ' btnSuppressionAttention
        ' 
        btnSuppressionAttention.Font = New Font("Calibri", 11.25F)
        btnSuppressionAttention.Location = New Point(304, 94)
        btnSuppressionAttention.Name = "btnSuppressionAttention"
        btnSuppressionAttention.Size = New Size(274, 36)
        btnSuppressionAttention.TabIndex = 8
        btnSuppressionAttention.Text = "Suppression avec attention"
        btnSuppressionAttention.Variante = VarianteBouton.Clair
        ' 
        ' btnRecapitulatif
        ' 
        btnRecapitulatif.Font = New Font("Calibri", 11.25F)
        btnRecapitulatif.Location = New Point(304, 142)
        btnRecapitulatif.Name = "btnRecapitulatif"
        btnRecapitulatif.Size = New Size(274, 36)
        btnRecapitulatif.TabIndex = 9
        btnRecapitulatif.Text = "Récapitulatif complet"
        btnRecapitulatif.Variante = VarianteBouton.Clair
        ' 
        ' btnQuotaDepasse
        ' 
        btnQuotaDepasse.Font = New Font("Calibri", 11.25F)
        btnQuotaDepasse.Location = New Point(304, 190)
        btnQuotaDepasse.Name = "btnQuotaDepasse"
        btnQuotaDepasse.Size = New Size(274, 36)
        btnQuotaDepasse.TabIndex = 10
        btnQuotaDepasse.Text = "Quota dépassé (SeanceEditor)"
        btnQuotaDepasse.Variante = VarianteBouton.Clair
        ' 
        ' btnTitreLong
        ' 
        btnTitreLong.Font = New Font("Calibri", 11.25F)
        btnTitreLong.Location = New Point(304, 238)
        btnTitreLong.Name = "btnTitreLong"
        btnTitreLong.Size = New Size(274, 36)
        btnTitreLong.TabIndex = 11
        btnTitreLong.Text = "Titre et texte longs"
        btnTitreLong.Variante = VarianteBouton.Clair
        ' 
        ' btnMessageTresLong
        ' 
        btnMessageTresLong.Font = New Font("Calibri", 11.25F)
        btnMessageTresLong.Location = New Point(304, 286)
        btnMessageTresLong.Name = "btnMessageTresLong"
        btnMessageTresLong.Size = New Size(274, 36)
        btnMessageTresLong.TabIndex = 12
        btnMessageTresLong.Text = "Message très long (défilement)"
        btnMessageTresLong.Variante = VarianteBouton.Clair
        ' 
        ' btnAttente
        ' 
        btnAttente.Font = New Font("Calibri", 11.25F)
        btnAttente.Location = New Point(304, 334)
        btnAttente.Name = "btnAttente"
        btnAttente.Size = New Size(274, 36)
        btnAttente.TabIndex = 13
        btnAttente.Text = "Attente (icône animée)"
        btnAttente.Variante = VarianteBouton.Clair
        ' 
        ' carteIcones
        ' 
        carteIcones.Controls.Add(iconeInformation48)
        carteIcones.Controls.Add(iconeInformation24)
        carteIcones.Controls.Add(lblIconeInformation)
        carteIcones.Controls.Add(iconeAvertissement48)
        carteIcones.Controls.Add(iconeAvertissement24)
        carteIcones.Controls.Add(lblIconeAvertissement)
        carteIcones.Controls.Add(iconeErreur48)
        carteIcones.Controls.Add(iconeErreur24)
        carteIcones.Controls.Add(lblIconeErreur)
        carteIcones.Controls.Add(iconeQuestion48)
        carteIcones.Controls.Add(iconeQuestion24)
        carteIcones.Controls.Add(lblIconeQuestion)
        carteIcones.Controls.Add(iconeSucces48)
        carteIcones.Controls.Add(iconeSucces24)
        carteIcones.Controls.Add(lblIconeSucces)
        carteIcones.Controls.Add(iconeAttente48)
        carteIcones.Controls.Add(iconeAttente24)
        carteIcones.Controls.Add(lblIconeAttente)
        carteIcones.Location = New Point(640, 74)
        carteIcones.Name = "carteIcones"
        carteIcones.Size = New Size(320, 400)
        carteIcones.TabIndex = 2
        carteIcones.Titre = "Icônes d'état"
        ' 
        ' iconeInformation48
        ' 
        iconeInformation48.Etat = EtatIcone.Information
        iconeInformation48.Location = New Point(20, 46)
        iconeInformation48.Name = "iconeInformation48"
        iconeInformation48.Size = New Size(48, 48)
        iconeInformation48.TabStop = False
        ' 
        ' iconeInformation24
        ' 
        iconeInformation24.Etat = EtatIcone.Information
        iconeInformation24.Location = New Point(80, 58)
        iconeInformation24.Name = "iconeInformation24"
        iconeInformation24.Size = New Size(24, 24)
        iconeInformation24.TabStop = False
        ' 
        ' lblIconeInformation
        ' 
        lblIconeInformation.AutoSize = True
        lblIconeInformation.BackColor = Color.Transparent
        lblIconeInformation.Font = New Font("Calibri", 11.25F)
        lblIconeInformation.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblIconeInformation.Location = New Point(120, 59)
        lblIconeInformation.Name = "lblIconeInformation"
        lblIconeInformation.Text = "Information"
        ' 
        ' iconeAvertissement48
        ' 
        iconeAvertissement48.Etat = EtatIcone.Avertissement
        iconeAvertissement48.Location = New Point(20, 102)
        iconeAvertissement48.Name = "iconeAvertissement48"
        iconeAvertissement48.Size = New Size(48, 48)
        iconeAvertissement48.TabStop = False
        ' 
        ' iconeAvertissement24
        ' 
        iconeAvertissement24.Etat = EtatIcone.Avertissement
        iconeAvertissement24.Location = New Point(80, 114)
        iconeAvertissement24.Name = "iconeAvertissement24"
        iconeAvertissement24.Size = New Size(24, 24)
        iconeAvertissement24.TabStop = False
        ' 
        ' lblIconeAvertissement
        ' 
        lblIconeAvertissement.AutoSize = True
        lblIconeAvertissement.BackColor = Color.Transparent
        lblIconeAvertissement.Font = New Font("Calibri", 11.25F)
        lblIconeAvertissement.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblIconeAvertissement.Location = New Point(120, 115)
        lblIconeAvertissement.Name = "lblIconeAvertissement"
        lblIconeAvertissement.Text = "Avertissement"
        ' 
        ' iconeErreur48
        ' 
        iconeErreur48.Etat = EtatIcone.Erreur
        iconeErreur48.Location = New Point(20, 158)
        iconeErreur48.Name = "iconeErreur48"
        iconeErreur48.Size = New Size(48, 48)
        iconeErreur48.TabStop = False
        ' 
        ' iconeErreur24
        ' 
        iconeErreur24.Etat = EtatIcone.Erreur
        iconeErreur24.Location = New Point(80, 170)
        iconeErreur24.Name = "iconeErreur24"
        iconeErreur24.Size = New Size(24, 24)
        iconeErreur24.TabStop = False
        ' 
        ' lblIconeErreur
        ' 
        lblIconeErreur.AutoSize = True
        lblIconeErreur.BackColor = Color.Transparent
        lblIconeErreur.Font = New Font("Calibri", 11.25F)
        lblIconeErreur.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblIconeErreur.Location = New Point(120, 171)
        lblIconeErreur.Name = "lblIconeErreur"
        lblIconeErreur.Text = "Erreur"
        ' 
        ' iconeQuestion48
        ' 
        iconeQuestion48.Etat = EtatIcone.Question
        iconeQuestion48.Location = New Point(20, 214)
        iconeQuestion48.Name = "iconeQuestion48"
        iconeQuestion48.Size = New Size(48, 48)
        iconeQuestion48.TabStop = False
        ' 
        ' iconeQuestion24
        ' 
        iconeQuestion24.Etat = EtatIcone.Question
        iconeQuestion24.Location = New Point(80, 226)
        iconeQuestion24.Name = "iconeQuestion24"
        iconeQuestion24.Size = New Size(24, 24)
        iconeQuestion24.TabStop = False
        ' 
        ' lblIconeQuestion
        ' 
        lblIconeQuestion.AutoSize = True
        lblIconeQuestion.BackColor = Color.Transparent
        lblIconeQuestion.Font = New Font("Calibri", 11.25F)
        lblIconeQuestion.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblIconeQuestion.Location = New Point(120, 227)
        lblIconeQuestion.Name = "lblIconeQuestion"
        lblIconeQuestion.Text = "Question"
        ' 
        ' iconeSucces48
        ' 
        iconeSucces48.Etat = EtatIcone.Succes
        iconeSucces48.Location = New Point(20, 270)
        iconeSucces48.Name = "iconeSucces48"
        iconeSucces48.Size = New Size(48, 48)
        iconeSucces48.TabStop = False
        ' 
        ' iconeSucces24
        ' 
        iconeSucces24.Etat = EtatIcone.Succes
        iconeSucces24.Location = New Point(80, 282)
        iconeSucces24.Name = "iconeSucces24"
        iconeSucces24.Size = New Size(24, 24)
        iconeSucces24.TabStop = False
        ' 
        ' lblIconeSucces
        ' 
        lblIconeSucces.AutoSize = True
        lblIconeSucces.BackColor = Color.Transparent
        lblIconeSucces.Font = New Font("Calibri", 11.25F)
        lblIconeSucces.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblIconeSucces.Location = New Point(120, 283)
        lblIconeSucces.Name = "lblIconeSucces"
        lblIconeSucces.Text = "Succès"
        ' 
        ' iconeAttente48
        ' 
        iconeAttente48.Etat = EtatIcone.Attente
        iconeAttente48.Location = New Point(20, 326)
        iconeAttente48.Name = "iconeAttente48"
        iconeAttente48.Size = New Size(48, 48)
        iconeAttente48.TabStop = False
        ' 
        ' iconeAttente24
        ' 
        iconeAttente24.Etat = EtatIcone.Attente
        iconeAttente24.Location = New Point(80, 338)
        iconeAttente24.Name = "iconeAttente24"
        iconeAttente24.Size = New Size(24, 24)
        iconeAttente24.TabStop = False
        ' 
        ' lblIconeAttente
        ' 
        lblIconeAttente.AutoSize = True
        lblIconeAttente.BackColor = Color.Transparent
        lblIconeAttente.Font = New Font("Calibri", 11.25F)
        lblIconeAttente.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblIconeAttente.Location = New Point(120, 339)
        lblIconeAttente.Name = "lblIconeAttente"
        lblIconeAttente.Text = "Attente"
        ' 
        ' lblResultat
        ' 
        lblResultat.AutoSize = True
        lblResultat.BackColor = Color.Transparent
        lblResultat.Font = New Font("Calibri", 11.25F, FontStyle.Bold)
        lblResultat.ForeColor = Color.FromArgb(CByte(75), CByte(105), CByte(90))
        lblResultat.Location = New Point(24, 490)
        lblResultat.Name = "lblResultat"
        lblResultat.TabIndex = 3
        lblResultat.Text = "Dernier résultat : —"
        ' 
        ' barreActions
        ' 
        barreActions.Controls.Add(btnFermer)
        barreActions.Dock = DockStyle.Bottom
        barreActions.Location = New Point(0, 530)
        barreActions.Name = "barreActions"
        barreActions.Size = New Size(980, 60)
        barreActions.TabIndex = 4
        ' 
        ' btnFermer
        ' 
        btnFermer.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnFermer.Font = New Font("Calibri", 11.25F)
        btnFermer.Location = New Point(850, 14)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(110, 34)
        btnFermer.TabIndex = 0
        btnFermer.Text = "Fermer"
        btnFermer.Variante = VarianteBouton.Clair
        ' 
        ' TestDialogChoix
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        ClientSize = New Size(980, 590)
        Controls.Add(lblResultat)
        Controls.Add(carteIcones)
        Controls.Add(carteCas)
        Controls.Add(barreActions)
        Controls.Add(enTete)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "TestDialogChoix"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Démonstration de DialogChoix"
        carteCas.ResumeLayout(False)
        carteIcones.ResumeLayout(False)
        carteIcones.PerformLayout()
        barreActions.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteCas As AltheaCarte
    Friend WithEvents btnInformation As AltheaBouton
    Friend WithEvents btnAvertissement As AltheaBouton
    Friend WithEvents btnErreur As AltheaBouton
    Friend WithEvents btnSucces As AltheaBouton
    Friend WithEvents btnConfirmerOuiNon As AltheaBouton
    Friend WithEvents btnQuestion As AltheaBouton
    Friend WithEvents btnConfirmerAction As AltheaBouton
    Friend WithEvents btnSuppression As AltheaBouton
    Friend WithEvents btnSuppressionAttention As AltheaBouton
    Friend WithEvents btnRecapitulatif As AltheaBouton
    Friend WithEvents btnQuotaDepasse As AltheaBouton
    Friend WithEvents btnTitreLong As AltheaBouton
    Friend WithEvents btnMessageTresLong As AltheaBouton
    Friend WithEvents btnAttente As AltheaBouton
    Friend WithEvents carteIcones As AltheaCarte
    Friend WithEvents iconeInformation48 As AltheaIconeEtat
    Friend WithEvents iconeInformation24 As AltheaIconeEtat
    Friend WithEvents lblIconeInformation As Label
    Friend WithEvents iconeAvertissement48 As AltheaIconeEtat
    Friend WithEvents iconeAvertissement24 As AltheaIconeEtat
    Friend WithEvents lblIconeAvertissement As Label
    Friend WithEvents iconeErreur48 As AltheaIconeEtat
    Friend WithEvents iconeErreur24 As AltheaIconeEtat
    Friend WithEvents lblIconeErreur As Label
    Friend WithEvents iconeQuestion48 As AltheaIconeEtat
    Friend WithEvents iconeQuestion24 As AltheaIconeEtat
    Friend WithEvents lblIconeQuestion As Label
    Friend WithEvents iconeSucces48 As AltheaIconeEtat
    Friend WithEvents iconeSucces24 As AltheaIconeEtat
    Friend WithEvents lblIconeSucces As Label
    Friend WithEvents iconeAttente48 As AltheaIconeEtat
    Friend WithEvents iconeAttente24 As AltheaIconeEtat
    Friend WithEvents lblIconeAttente As Label
    Friend WithEvents lblResultat As Label
    Friend WithEvents barreActions As AltheaBarreActions
    Friend WithEvents btnFermer As AltheaBouton

End Class
