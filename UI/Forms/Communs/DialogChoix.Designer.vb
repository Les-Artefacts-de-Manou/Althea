' -------------------------------------------------------------------------------------------------
' Designer    : DialogChoix
' Projet      : Althéa
' Version     : V2.1.0
' Date        : 16/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Structure visuelle de la boîte de dialogue (maquette « DialogChoix — C », retenue le
' 16/10/2026) : petite fenêtre du kit, avec barre de titre Windows, en-tête AltheaEnTete (icône
' d'état + titre), message dans une AltheaCarte (message, liste de détails, complément, encadré
' d'attention, remarque, question finale) et boutons dans une AltheaBarreActions.
' Les positions et tailles définitives sont calculées au chargement selon le texte.
' -------------------------------------------------------------------------------------------------

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
        enTete = New AltheaEnTete()
        carteMessage = New AltheaCarte()
        pnlCorps = New Panel()
        lblQuestionFinale = New Label()
        lblRemarque = New Label()
        carteAttention = New AltheaCarte()
        lblAttention = New Label()
        iconeAttention = New AltheaIconeEtat()
        lblComplement = New Label()
        carteDetails = New AltheaCarte()
        lblMessage = New Label()
        pnlActions = New AltheaBarreActions()
        btn3 = New AltheaBouton()
        btn2 = New AltheaBouton()
        btn1 = New AltheaBouton()
        carteMessage.SuspendLayout()
        pnlCorps.SuspendLayout()
        carteAttention.SuspendLayout()
        pnlActions.SuspendLayout()
        SuspendLayout()
        '
        ' enTete
        '
        enTete.Dock = DockStyle.Top
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.PoliceTitre = New Font("Calibri", 15.75F, FontStyle.Bold)
        enTete.Size = New Size(480, 58)
        enTete.TabIndex = 0
        enTete.TailleIcone = 40
        enTete.Text = "Titre"
        '
        ' carteMessage
        '
        carteMessage.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        carteMessage.Controls.Add(pnlCorps)
        carteMessage.Location = New Point(12, 66)
        carteMessage.Name = "carteMessage"
        carteMessage.Padding = New Padding(16, 12, 16, 18)
        carteMessage.Size = New Size(456, 80)
        carteMessage.TabIndex = 1
        carteMessage.TabStop = False
        '
        ' pnlCorps
        '
        pnlCorps.BackColor = Color.Transparent
        pnlCorps.Controls.Add(lblQuestionFinale)
        pnlCorps.Controls.Add(lblRemarque)
        pnlCorps.Controls.Add(carteAttention)
        pnlCorps.Controls.Add(lblComplement)
        pnlCorps.Controls.Add(carteDetails)
        pnlCorps.Controls.Add(lblMessage)
        pnlCorps.Dock = DockStyle.Fill
        pnlCorps.Location = New Point(16, 12)
        pnlCorps.Name = "pnlCorps"
        pnlCorps.Size = New Size(424, 50)
        pnlCorps.TabIndex = 0
        '
        ' lblQuestionFinale
        '
        lblQuestionFinale.BackColor = Color.Transparent
        lblQuestionFinale.Font = New Font("Calibri", 11.25F, FontStyle.Bold)
        lblQuestionFinale.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblQuestionFinale.Location = New Point(0, 120)
        lblQuestionFinale.Name = "lblQuestionFinale"
        lblQuestionFinale.Size = New Size(424, 20)
        lblQuestionFinale.TabIndex = 5
        lblQuestionFinale.UseMnemonic = False
        lblQuestionFinale.Visible = False
        '
        ' lblRemarque
        '
        lblRemarque.BackColor = Color.Transparent
        lblRemarque.Font = New Font("Calibri", 9.75F)
        lblRemarque.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblRemarque.Location = New Point(0, 100)
        lblRemarque.Name = "lblRemarque"
        lblRemarque.Size = New Size(424, 18)
        lblRemarque.TabIndex = 4
        lblRemarque.UseMnemonic = False
        lblRemarque.Visible = False
        '
        ' carteAttention
        '
        carteAttention.CouleurBordure = Color.Transparent
        carteAttention.CouleurFond = Color.FromArgb(CByte(246), CByte(241), CByte(231))
        carteAttention.Controls.Add(lblAttention)
        carteAttention.Controls.Add(iconeAttention)
        carteAttention.Location = New Point(0, 54)
        carteAttention.Name = "carteAttention"
        carteAttention.Ombre = False
        carteAttention.OpaciteFond = 255
        carteAttention.Padding = New Padding(12, 10, 12, 10)
        carteAttention.Rayon = 8
        carteAttention.Size = New Size(424, 44)
        carteAttention.TabIndex = 3
        carteAttention.TabStop = False
        carteAttention.Visible = False
        '
        ' lblAttention
        '
        lblAttention.BackColor = Color.Transparent
        lblAttention.Font = New Font("Calibri", 11.25F)
        lblAttention.ForeColor = Color.FromArgb(CByte(140), CByte(112), CByte(63))
        lblAttention.Location = New Point(42, 10)
        lblAttention.Name = "lblAttention"
        lblAttention.Size = New Size(370, 24)
        lblAttention.TabIndex = 1
        lblAttention.UseMnemonic = False
        '
        ' iconeAttention
        '
        iconeAttention.Etat = EtatIcone.Avertissement
        iconeAttention.Location = New Point(12, 11)
        iconeAttention.Name = "iconeAttention"
        iconeAttention.Size = New Size(20, 20)
        iconeAttention.TabIndex = 0
        iconeAttention.TabStop = False
        '
        ' lblComplement
        '
        lblComplement.BackColor = Color.Transparent
        lblComplement.Font = New Font("Calibri", 11.25F)
        lblComplement.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblComplement.Location = New Point(0, 32)
        lblComplement.Name = "lblComplement"
        lblComplement.Size = New Size(424, 20)
        lblComplement.TabIndex = 2
        lblComplement.UseMnemonic = False
        lblComplement.Visible = False
        '
        ' carteDetails
        '
        carteDetails.CouleurBordure = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        carteDetails.CouleurFond = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        carteDetails.Location = New Point(0, 26)
        carteDetails.Name = "carteDetails"
        carteDetails.Ombre = False
        carteDetails.OpaciteFond = 255
        carteDetails.Padding = New Padding(14, 10, 14, 10)
        carteDetails.Rayon = 8
        carteDetails.Size = New Size(424, 40)
        carteDetails.TabIndex = 1
        carteDetails.TabStop = False
        carteDetails.Visible = False
        '
        ' lblMessage
        '
        lblMessage.BackColor = Color.Transparent
        lblMessage.Font = New Font("Calibri", 11.25F)
        lblMessage.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblMessage.Location = New Point(0, 0)
        lblMessage.Name = "lblMessage"
        lblMessage.Size = New Size(424, 22)
        lblMessage.TabIndex = 0
        lblMessage.Text = "Message"
        lblMessage.UseMnemonic = False
        '
        ' pnlActions
        '
        pnlActions.Controls.Add(btn3)
        pnlActions.Controls.Add(btn2)
        pnlActions.Controls.Add(btn1)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(0, 152)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(480, 54)
        pnlActions.TabIndex = 2
        '
        ' btn1
        '
        btn1.Font = New Font("Calibri", 11.25F, FontStyle.Bold)
        btn1.Location = New Point(132, 7)
        btn1.Name = "btn1"
        btn1.Size = New Size(100, 34)
        btn1.TabIndex = 0
        btn1.Text = "Oui"
        '
        ' btn2
        '
        btn2.Font = New Font("Calibri", 11.25F)
        btn2.Location = New Point(242, 7)
        btn2.Name = "btn2"
        btn2.Size = New Size(100, 34)
        btn2.TabIndex = 1
        btn2.Text = "Non"
        btn2.Variante = VarianteBouton.Clair
        '
        ' btn3
        '
        btn3.Font = New Font("Calibri", 11.25F)
        btn3.Location = New Point(352, 7)
        btn3.Name = "btn3"
        btn3.Size = New Size(100, 34)
        btn3.TabIndex = 2
        btn3.Text = "Annuler"
        btn3.Variante = VarianteBouton.Clair
        '
        ' DialogChoix
        '
        AutoScaleDimensions = New SizeF(6F, 14F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        ClientSize = New Size(480, 206)
        Controls.Add(carteMessage)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        Font = New Font("Calibri", 9F)
        FormBorderStyle = FormBorderStyle.FixedDialog
        KeyPreview = True
        MaximizeBox = False
        MinimizeBox = False
        Name = "DialogChoix"
        ShowIcon = False
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Confirmation"
        carteMessage.ResumeLayout(False)
        pnlCorps.ResumeLayout(False)
        carteAttention.ResumeLayout(False)
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteMessage As AltheaCarte
    Friend WithEvents pnlCorps As Panel
    Friend WithEvents lblMessage As Label
    Friend WithEvents carteDetails As AltheaCarte
    Friend WithEvents lblComplement As Label
    Friend WithEvents carteAttention As AltheaCarte
    Friend WithEvents iconeAttention As AltheaIconeEtat
    Friend WithEvents lblAttention As Label
    Friend WithEvents lblRemarque As Label
    Friend WithEvents lblQuestionFinale As Label
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btn1 As AltheaBouton
    Friend WithEvents btn2 As AltheaBouton
    Friend WithEvents btn3 As AltheaBouton

End Class
