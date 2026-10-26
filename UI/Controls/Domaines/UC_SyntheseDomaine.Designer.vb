' -------------------------------------------------------------------------------------------------
' Designer    : UC_SyntheseDomaine
' Projet      : Althéa
' Version     : V2.0.0
' Date        : 11/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Structure visuelle de la synthèse d'un domaine-convention : ligne d'en-tête (libellé, pastille
' du code, pastille de l'organisme) et zone de blocs (règles, tarifs, natures, quota, complétude),
' ces blocs étant construits et disposés en code (UI-5, maquette « Fiche dossier » B).
' -------------------------------------------------------------------------------------------------

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_SyntheseDomaine
    Inherits System.Windows.Forms.UserControl

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        pnlEntete = New Panel()
        lblTitre = New Label()
        pstCode = New AltheaPastille()
        pstOrganisme = New AltheaPastille()
        pnlCorps = New Panel()
        pnlEntete.SuspendLayout()
        SuspendLayout()
        ' 
        ' pnlEntete
        ' 
        pnlEntete.BackColor = Color.Transparent
        pnlEntete.Controls.Add(lblTitre)
        pnlEntete.Controls.Add(pstCode)
        pnlEntete.Controls.Add(pstOrganisme)
        pnlEntete.Dock = DockStyle.Top
        pnlEntete.Location = New Point(0, 0)
        pnlEntete.Name = "pnlEntete"
        pnlEntete.Size = New Size(620, 44)
        pnlEntete.TabIndex = 0
        ' 
        ' lblTitre
        ' 
        lblTitre.AutoSize = True
        lblTitre.BackColor = Color.Transparent
        lblTitre.Font = New Font("Calibri", 13F, FontStyle.Bold)
        lblTitre.ForeColor = Color.FromArgb(CByte(75), CByte(105), CByte(90))
        lblTitre.Location = New Point(0, 6)
        lblTitre.Name = "lblTitre"
        lblTitre.Size = New Size(196, 22)
        lblTitre.TabIndex = 0
        lblTitre.Text = "Aucun domaine sélectionné"
        ' 
        ' pstCode
        ' 
        pstCode.Font = New Font("Calibri", 9.75F, FontStyle.Bold)
        pstCode.Location = New Point(204, 8)
        pstCode.Name = "pstCode"
        pstCode.Size = New Size(48, 20)
        pstCode.TabIndex = 1
        pstCode.Text = "CODE"
        pstCode.Visible = False
        ' 
        ' pstOrganisme
        ' 
        pstOrganisme.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        pstOrganisme.CouleurFond = Color.FromArgb(CByte(235), CByte(226), CByte(217))
        pstOrganisme.Font = New Font("Calibri", 9.75F, FontStyle.Italic)
        pstOrganisme.ForeColor = Color.FromArgb(CByte(95), CByte(125), CByte(110))
        pstOrganisme.Location = New Point(420, 8)
        pstOrganisme.Name = "pstOrganisme"
        pstOrganisme.Size = New Size(200, 20)
        pstOrganisme.TabIndex = 2
        pstOrganisme.Text = "Organisme"
        pstOrganisme.Visible = False
        ' 
        ' pnlCorps
        ' 
        pnlCorps.AutoScroll = True
        pnlCorps.BackColor = Color.Transparent
        pnlCorps.Dock = DockStyle.Fill
        pnlCorps.Location = New Point(0, 44)
        pnlCorps.Name = "pnlCorps"
        pnlCorps.Size = New Size(620, 276)
        pnlCorps.TabIndex = 1
        ' 
        ' UC_SyntheseDomaine
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Transparent
        Controls.Add(pnlCorps)
        Controls.Add(pnlEntete)
        Name = "UC_SyntheseDomaine"
        Size = New Size(620, 320)
        pnlEntete.ResumeLayout(False)
        pnlEntete.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlEntete As Panel
    Friend WithEvents lblTitre As Label
    Friend WithEvents pstCode As AltheaPastille
    Friend WithEvents pstOrganisme As AltheaPastille
    Friend WithEvents pnlCorps As Panel

End Class
