<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UC_Pagination
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
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UC_Pagination))
        btnPremiere = New AltheaBouton()
        btnPrecedente = New AltheaBouton()
        lblPage = New Label()
        btnSuivante = New AltheaBouton()
        btnDerniere = New AltheaBouton()
        lblResume = New Label()
        infobulle = New ToolTip(components)
        SuspendLayout()
        '
        ' btnPremiere
        '
        btnPremiere.Image = CType(resources.GetObject("btnPremiere.Image"), Image)
        btnPremiere.Location = New Point(0, 1)
        btnPremiere.Name = "btnPremiere"
        btnPremiere.Size = New Size(30, 30)
        btnPremiere.TabIndex = 0
        btnPremiere.Tag = "pagePremiere_24_normal"
        infobulle.SetToolTip(btnPremiere, "Première page")
        btnPremiere.Variante = VarianteBouton.Outil
        '
        ' btnPrecedente
        '
        btnPrecedente.Image = CType(resources.GetObject("btnPrecedente.Image"), Image)
        btnPrecedente.Location = New Point(32, 1)
        btnPrecedente.Name = "btnPrecedente"
        btnPrecedente.Size = New Size(30, 30)
        btnPrecedente.TabIndex = 1
        btnPrecedente.Tag = "pagePrecedente_24_normal"
        infobulle.SetToolTip(btnPrecedente, "Page précédente")
        btnPrecedente.Variante = VarianteBouton.Outil
        '
        ' lblPage
        '
        lblPage.Font = New Font("Calibri", 10F)
        lblPage.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblPage.Location = New Point(64, 1)
        lblPage.Name = "lblPage"
        lblPage.Size = New Size(96, 30)
        lblPage.TabIndex = 2
        lblPage.Text = "Page 1 / 1"
        lblPage.TextAlign = ContentAlignment.MiddleCenter
        '
        ' btnSuivante
        '
        btnSuivante.Image = CType(resources.GetObject("btnSuivante.Image"), Image)
        btnSuivante.Location = New Point(162, 1)
        btnSuivante.Name = "btnSuivante"
        btnSuivante.Size = New Size(30, 30)
        btnSuivante.TabIndex = 3
        btnSuivante.Tag = "pageSuivante_24_normal"
        infobulle.SetToolTip(btnSuivante, "Page suivante")
        btnSuivante.Variante = VarianteBouton.Outil
        '
        ' btnDerniere
        '
        btnDerniere.Image = CType(resources.GetObject("btnDerniere.Image"), Image)
        btnDerniere.Location = New Point(194, 1)
        btnDerniere.Name = "btnDerniere"
        btnDerniere.Size = New Size(30, 30)
        btnDerniere.TabIndex = 4
        btnDerniere.Tag = "pageDerniere_24_normal"
        infobulle.SetToolTip(btnDerniere, "Dernière page")
        btnDerniere.Variante = VarianteBouton.Outil
        '
        ' lblResume
        '
        lblResume.Dock = DockStyle.Right
        lblResume.Font = New Font("Calibri", 10F)
        lblResume.ForeColor = Color.FromArgb(CByte(118), CByte(118), CByte(118))
        lblResume.Location = New Point(400, 0)
        lblResume.Name = "lblResume"
        lblResume.Size = New Size(240, 32)
        lblResume.TabIndex = 5
        lblResume.Text = "Aucun élément"
        lblResume.TextAlign = ContentAlignment.MiddleRight
        '
        ' UC_Pagination
        '
        AutoScaleDimensions = New SizeF(6F, 14F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Transparent
        Controls.Add(lblResume)
        Controls.Add(btnDerniere)
        Controls.Add(btnSuivante)
        Controls.Add(lblPage)
        Controls.Add(btnPrecedente)
        Controls.Add(btnPremiere)
        Font = New Font("Calibri", 9F)
        Name = "UC_Pagination"
        Size = New Size(640, 32)
        ResumeLayout(False)

    End Sub

    Friend WithEvents btnPremiere As AltheaBouton
    Friend WithEvents btnPrecedente As AltheaBouton
    Friend WithEvents lblPage As Label
    Friend WithEvents btnSuivante As AltheaBouton
    Friend WithEvents btnDerniere As AltheaBouton
    Friend WithEvents lblResume As Label
    Friend WithEvents infobulle As ToolTip

End Class
