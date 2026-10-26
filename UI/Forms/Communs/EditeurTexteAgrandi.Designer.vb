' -------------------------------------------------------------------------------------------------
' Fichier     : EditeurTexteAgrandi.Designer.vb
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 19/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Code généré (layout) de la fenêtre « Ouvrir en grand » : en-tête, carte contenant l'éditeur au
' profil Complet, barre d'actions (Valider / Annuler).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class EditeurTexteAgrandi
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EditeurTexteAgrandi))
        enTete = New AltheaEnTete()
        carteTexte = New AltheaCarte()
        rteTexte = New UC_RichTextEditor()
        pnlActions = New AltheaBarreActions()
        btnValider = New AltheaBouton()
        btnAnnuler = New AltheaBouton()
        carteTexte.SuspendLayout()
        pnlActions.SuspendLayout()
        SuspendLayout()
        '
        ' enTete
        '
        enTete.Dock = DockStyle.Top
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(1100, 62)
        enTete.TabIndex = 0
        enTete.SousTitre = "Ouvert en grand — Valider reporte le texte dans le champ"
        enTete.Text = "Texte"
        '
        ' carteTexte
        '
        carteTexte.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        carteTexte.Controls.Add(rteTexte)
        carteTexte.Location = New Point(12, 70)
        carteTexte.Name = "carteTexte"
        carteTexte.Padding = New Padding(12, 10, 12, 10)
        carteTexte.Size = New Size(1076, 566)
        carteTexte.TabIndex = 1
        carteTexte.TabStop = False
        '
        ' rteTexte
        '
        rteTexte.BackColor = Color.Transparent
        rteTexte.Dock = DockStyle.Fill
        rteTexte.Location = New Point(12, 10)
        rteTexte.Margin = New Padding(4, 3, 4, 3)
        rteTexte.Name = "rteTexte"
        rteTexte.Padding = New Padding(0)
        rteTexte.Size = New Size(1052, 546)
        rteTexte.TabIndex = 0
        '
        ' pnlActions
        '
        pnlActions.Controls.Add(btnValider)
        pnlActions.Controls.Add(btnAnnuler)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(0, 644)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(1100, 52)
        pnlActions.TabIndex = 2
        '
        ' btnValider
        '
        btnValider.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnValider.Image = CType(resources.GetObject("btnValider.Image"), Image)
        btnValider.Location = New Point(838, 7)
        btnValider.Name = "btnValider"
        btnValider.Size = New Size(120, 38)
        btnValider.TabIndex = 0
        btnValider.Tag = "valider_normal"
        btnValider.Text = "Valider"
        btnValider.Variante = VarianteBouton.Ocre
        '
        ' btnAnnuler
        '
        btnAnnuler.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnAnnuler.Image = CType(resources.GetObject("btnAnnuler.Image"), Image)
        btnAnnuler.Location = New Point(968, 7)
        btnAnnuler.Name = "btnAnnuler"
        btnAnnuler.Size = New Size(120, 38)
        btnAnnuler.TabIndex = 1
        btnAnnuler.Tag = "annuler_normal"
        btnAnnuler.Text = "Annuler"
        btnAnnuler.Variante = VarianteBouton.Clair
        '
        ' EditeurTexteAgrandi
        '
        AutoScaleDimensions = New SizeF(8F, 18F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        CancelButton = btnAnnuler
        ClientSize = New Size(1100, 696)
        Controls.Add(carteTexte)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        DoubleBuffered = True
        Font = New Font("Calibri", 11F)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MinimizeBox = False
        MinimumSize = New Size(560, 400)
        Name = "EditeurTexteAgrandi"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Texte en grand"
        carteTexte.ResumeLayout(False)
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)

    End Sub

    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteTexte As AltheaCarte
    Friend WithEvents rteTexte As UC_RichTextEditor
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnValider As AltheaBouton
    Friend WithEvents btnAnnuler As AltheaBouton

End Class
