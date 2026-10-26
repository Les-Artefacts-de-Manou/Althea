' -------------------------------------------------------------------------------------------------
' Fichier     : DossierZoneEditor.Designer.vb
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 11/07/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Code généré (layout) de la modale DossierZoneEditor : bandeau titre (libellé de la zone),
' éditeur riche complet (UC_RichTextEditor) et panneau d'actions (Enregistrer / Fermer).
'
' Remarques   :
' - Brique B4 : modale réutilisable pour éditer une zone clinique unique d'un dossier.
' - Aucune ressource image utilisée ici (boutons colorés, cohérents avec la charte) → pas de .resx.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DossierZoneEditor
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DossierZoneEditor))
        enTete = New AltheaEnTete()
        carteZone = New AltheaCarte()
        rteZone = New UC_RichTextEditor()
        pnlActions = New AltheaBarreActions()
        btnEnregistrer = New AltheaBouton()
        btnFermer = New AltheaBouton()
        carteZone.SuspendLayout()
        pnlActions.SuspendLayout()
        SuspendLayout()
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Icone = CType(resources.GetObject("enTete.Icone"), Image)
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(1061, 62)
        enTete.TabIndex = 0
        enTete.SousTitre = "Zone clinique"
        enTete.Text = "Dossier"
        ' 
        ' carteZone
        ' 
        carteZone.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        carteZone.Controls.Add(rteZone)
        carteZone.Location = New Point(12, 70)
        carteZone.Name = "carteZone"
        carteZone.Padding = New Padding(12, 8, 12, 10)
        carteZone.Size = New Size(1037, 410)
        carteZone.TabIndex = 1
        carteZone.TabStop = False
        carteZone.Titre = "Zone clinique"
        ' 
        ' rteZone
        ' 
        rteZone.BackColor = Color.Transparent
        rteZone.Dock = DockStyle.Fill
        rteZone.Location = New Point(12, 45)
        rteZone.Margin = New Padding(4, 3, 4, 3)
        rteZone.Name = "rteZone"
        rteZone.Padding = New Padding(0)
        rteZone.ReadOnlyMode = False
        rteZone.ShowToolbar = True
        rteZone.Size = New Size(1013, 355)
        rteZone.TabIndex = 0
        ' 
        ' pnlActions
        ' 
        pnlActions.Controls.Add(btnEnregistrer)
        pnlActions.Controls.Add(btnFermer)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(0, 488)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(1061, 52)
        pnlActions.TabIndex = 2
        ' 
        ' btnEnregistrer
        ' 
        btnEnregistrer.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnEnregistrer.Image = CType(resources.GetObject("btnEnregistrer.Image"), Image)
        btnEnregistrer.Location = New Point(791, 7)
        btnEnregistrer.Name = "btnEnregistrer"
        btnEnregistrer.Size = New Size(132, 38)
        btnEnregistrer.TabIndex = 0
        btnEnregistrer.Tag = "enregistrer_normal"
        btnEnregistrer.Text = "Enregistrer"
        btnEnregistrer.Variante = VarianteBouton.Ocre
        ' 
        ' btnFermer
        ' 
        btnFermer.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnFermer.Image = CType(resources.GetObject("btnFermer.Image"), Image)
        btnFermer.Location = New Point(933, 7)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(109, 38)
        btnFermer.TabIndex = 1
        btnFermer.Tag = "fermer_normal"
        btnFermer.Text = "Fermer"
        btnFermer.Variante = VarianteBouton.Clair
        ' 
        ' DossierZoneEditor
        ' 
        AutoScaleDimensions = New SizeF(8F, 18F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        ClientSize = New Size(1061, 540)
        Controls.Add(carteZone)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        DoubleBuffered = True
        Font = New Font("Calibri", 11F)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        MinimumSize = New Size(560, 400)
        Name = "DossierZoneEditor"
        StartPosition = FormStartPosition.CenterParent
        Text = "Édition d'une zone clinique"
        carteZone.ResumeLayout(False)
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)

    End Sub

    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteZone As AltheaCarte
    Friend WithEvents rteZone As UC_RichTextEditor
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnEnregistrer As AltheaBouton
    Friend WithEvents btnFermer As AltheaBouton

End Class
