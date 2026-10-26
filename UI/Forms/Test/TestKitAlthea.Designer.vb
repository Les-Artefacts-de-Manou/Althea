<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TestKitAlthea
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
        onglets = New AltheaOnglets()
        tpPalette = New TabPage()
        tpBoutons = New TabPage()
        tpStructure = New TabPage()
        tpChamps = New TabPage()
        tpIndicateurs = New TabPage()
        tpGrilles = New TabPage()
        pnlActions = New AltheaBarreActions()
        btnFermer = New AltheaBouton()
        onglets.SuspendLayout()
        pnlActions.SuspendLayout()
        SuspendLayout()
        '
        ' enTete
        '
        enTete.Dock = DockStyle.Top
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(1084, 62)
        enTete.SousTitre = "Tous les contrôles du kit Althéa, dans leurs états (survol : passer la souris)"
        enTete.TabIndex = 0
        enTete.Text = "Vitrine du kit"
        '
        ' onglets
        '
        onglets.Controls.Add(tpPalette)
        onglets.Controls.Add(tpBoutons)
        onglets.Controls.Add(tpStructure)
        onglets.Controls.Add(tpChamps)
        onglets.Controls.Add(tpIndicateurs)
        onglets.Controls.Add(tpGrilles)
        onglets.Dock = DockStyle.Fill
        onglets.DrawMode = TabDrawMode.OwnerDrawFixed
        onglets.Font = New Font("Calibri", 10F)
        onglets.ItemSize = New Size(0, 30)
        onglets.Location = New Point(0, 62)
        onglets.Name = "onglets"
        onglets.SelectedIndex = 0
        onglets.Size = New Size(1084, 641)
        onglets.TabIndex = 1
        '
        ' tpPalette
        '
        tpPalette.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tpPalette.Location = New Point(4, 34)
        tpPalette.Name = "tpPalette"
        tpPalette.Size = New Size(1076, 603)
        tpPalette.TabIndex = 0
        tpPalette.Text = "Palette et polices"
        '
        ' tpBoutons
        '
        tpBoutons.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tpBoutons.Location = New Point(4, 34)
        tpBoutons.Name = "tpBoutons"
        tpBoutons.Size = New Size(1076, 603)
        tpBoutons.TabIndex = 1
        tpBoutons.Text = "Boutons"
        '
        ' tpStructure
        '
        tpStructure.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tpStructure.Location = New Point(4, 34)
        tpStructure.Name = "tpStructure"
        tpStructure.Size = New Size(1076, 603)
        tpStructure.TabIndex = 2
        tpStructure.Text = "Structure"
        '
        ' tpChamps
        '
        tpChamps.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tpChamps.Location = New Point(4, 34)
        tpChamps.Name = "tpChamps"
        tpChamps.Size = New Size(1076, 603)
        tpChamps.TabIndex = 3
        tpChamps.Text = "Champs"
        '
        ' tpIndicateurs
        '
        tpIndicateurs.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tpIndicateurs.Location = New Point(4, 34)
        tpIndicateurs.Name = "tpIndicateurs"
        tpIndicateurs.Size = New Size(1076, 603)
        tpIndicateurs.TabIndex = 4
        tpIndicateurs.Text = "Indicateurs"
        '
        ' tpGrilles
        '
        tpGrilles.BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        tpGrilles.Location = New Point(4, 34)
        tpGrilles.Name = "tpGrilles"
        tpGrilles.Size = New Size(1076, 603)
        tpGrilles.TabIndex = 5
        tpGrilles.Text = "Grilles"
        '
        ' pnlActions
        '
        pnlActions.Controls.Add(btnFermer)
        pnlActions.Dock = DockStyle.Bottom
        pnlActions.Location = New Point(0, 703)
        pnlActions.Name = "pnlActions"
        pnlActions.Size = New Size(1084, 57)
        pnlActions.TabIndex = 2
        '
        ' btnFermer
        '
        btnFermer.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnFermer.Location = New Point(950, 8)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(118, 40)
        btnFermer.TabIndex = 0
        btnFermer.Tag = "fermer_normal"
        btnFermer.Text = "Fermer"
        btnFermer.Variante = VarianteBouton.Clair
        '
        ' TestKitAlthea
        '
        AutoScaleDimensions = New SizeF(6F, 14F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        ClientSize = New Size(1084, 760)
        Controls.Add(onglets)
        Controls.Add(pnlActions)
        Controls.Add(enTete)
        Font = New Font("Calibri", 9F)
        MinimumSize = New Size(900, 600)
        Name = "TestKitAlthea"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Vitrine du kit Althéa"
        onglets.ResumeLayout(False)
        pnlActions.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents onglets As AltheaOnglets
    Friend WithEvents tpPalette As TabPage
    Friend WithEvents tpBoutons As TabPage
    Friend WithEvents tpStructure As TabPage
    Friend WithEvents tpChamps As TabPage
    Friend WithEvents tpIndicateurs As TabPage
    Friend WithEvents tpGrilles As TabPage
    Friend WithEvents pnlActions As AltheaBarreActions
    Friend WithEvents btnFermer As AltheaBouton

End Class
