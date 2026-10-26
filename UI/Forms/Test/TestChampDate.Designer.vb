' -------------------------------------------------------------------------------------------------
' Designer    : TestChampDate
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 14/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Fenêtre de démonstration du champ de date du kit (AltheaDate) et du calendrier seul
' (AltheaCalendrier), pour les tests et les captures.
' -------------------------------------------------------------------------------------------------

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TestChampDate
    Inherits System.Windows.Forms.Form

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
        enTete = New AltheaEnTete()
        carteChamps = New AltheaCarte()
        lblDateNormale = New Label()
        dateNormale = New AltheaDate()
        lblDateFacultative = New Label()
        dateFacultative = New AltheaDate()
        lblDateBornee = New Label()
        dateBornee = New AltheaDate()
        lblDateDu = New Label()
        dateDu = New AltheaDate()
        lblDateAu = New Label()
        dateAu = New AltheaDate()
        chkLectureSeule = New AltheaCase()
        lblJournal = New Label()
        carteCalendrier = New AltheaCarte()
        calendrierSeul = New AltheaCalendrier()
        barreActions = New AltheaBarreActions()
        btnFermer = New AltheaBouton()
        carteChamps.SuspendLayout()
        carteCalendrier.SuspendLayout()
        barreActions.SuspendLayout()
        SuspendLayout()
        ' 
        ' enTete
        ' 
        enTete.Dock = DockStyle.Top
        enTete.Location = New Point(0, 0)
        enTete.Name = "enTete"
        enTete.Size = New Size(900, 58)
        enTete.SousTitre = "Tapez une date (« 141026 », « 14/10 »), ouvrez le calendrier (icône, Alt+Bas, F4), Haut / Bas pour changer d'un jour."
        enTete.TabIndex = 0
        enTete.Text = "Démonstration du champ de date"
        ' 
        ' carteChamps
        ' 
        carteChamps.Controls.Add(lblDateNormale)
        carteChamps.Controls.Add(dateNormale)
        carteChamps.Controls.Add(lblDateFacultative)
        carteChamps.Controls.Add(dateFacultative)
        carteChamps.Controls.Add(lblDateBornee)
        carteChamps.Controls.Add(dateBornee)
        carteChamps.Controls.Add(lblDateDu)
        carteChamps.Controls.Add(dateDu)
        carteChamps.Controls.Add(lblDateAu)
        carteChamps.Controls.Add(dateAu)
        carteChamps.Controls.Add(chkLectureSeule)
        carteChamps.Controls.Add(lblJournal)
        carteChamps.Location = New Point(20, 74)
        carteChamps.Name = "carteChamps"
        carteChamps.Size = New Size(530, 400)
        carteChamps.TabIndex = 1
        carteChamps.Titre = "AltheaDate"
        ' 
        ' lblDateNormale
        ' 
        lblDateNormale.AutoSize = True
        lblDateNormale.BackColor = Color.Transparent
        lblDateNormale.Font = New Font("Calibri", 11.25F)
        lblDateNormale.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblDateNormale.Location = New Point(18, 56)
        lblDateNormale.Name = "lblDateNormale"
        lblDateNormale.Text = "Date (obligatoire)"
        ' 
        ' dateNormale
        ' 
        dateNormale.Location = New Point(300, 50)
        dateNormale.Name = "dateNormale"
        dateNormale.Size = New Size(150, 32)
        dateNormale.TabIndex = 0
        ' 
        ' lblDateFacultative
        ' 
        lblDateFacultative.AutoSize = True
        lblDateFacultative.BackColor = Color.Transparent
        lblDateFacultative.Font = New Font("Calibri", 11.25F)
        lblDateFacultative.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblDateFacultative.Location = New Point(18, 104)
        lblDateFacultative.Name = "lblDateFacultative"
        lblDateFacultative.Text = "Date facultative (vide + croix)"
        ' 
        ' dateFacultative
        ' 
        dateFacultative.Facultative = True
        dateFacultative.Location = New Point(300, 98)
        dateFacultative.Name = "dateFacultative"
        dateFacultative.Size = New Size(150, 32)
        dateFacultative.TabIndex = 1
        ' 
        ' lblDateBornee
        ' 
        lblDateBornee.AutoSize = True
        lblDateBornee.BackColor = Color.Transparent
        lblDateBornee.Font = New Font("Calibri", 11.25F)
        lblDateBornee.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblDateBornee.Location = New Point(18, 152)
        lblDateBornee.Name = "lblDateBornee"
        lblDateBornee.Text = "Bornée : d'aujourd'hui à + 1 an"
        ' 
        ' dateBornee
        ' 
        dateBornee.Location = New Point(300, 146)
        dateBornee.Name = "dateBornee"
        dateBornee.Size = New Size(150, 32)
        dateBornee.TabIndex = 2
        ' 
        ' lblDateDu
        ' 
        lblDateDu.AutoSize = True
        lblDateDu.BackColor = Color.Transparent
        lblDateDu.Font = New Font("Calibri", 11.25F)
        lblDateDu.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblDateDu.Location = New Point(18, 200)
        lblDateDu.Name = "lblDateDu"
        lblDateDu.Text = "Filtre : du"
        ' 
        ' dateDu
        ' 
        dateDu.Location = New Point(300, 194)
        dateDu.Name = "dateDu"
        dateDu.Size = New Size(150, 32)
        dateDu.TabIndex = 3
        ' 
        ' lblDateAu
        ' 
        lblDateAu.AutoSize = True
        lblDateAu.BackColor = Color.Transparent
        lblDateAu.Font = New Font("Calibri", 11.25F)
        lblDateAu.ForeColor = Color.FromArgb(CByte(74), CByte(74), CByte(74))
        lblDateAu.Location = New Point(18, 248)
        lblDateAu.Name = "lblDateAu"
        lblDateAu.Text = "Filtre : au"
        ' 
        ' dateAu
        ' 
        dateAu.Location = New Point(300, 242)
        dateAu.Name = "dateAu"
        dateAu.Size = New Size(150, 32)
        dateAu.TabIndex = 4
        ' 
        ' chkLectureSeule
        ' 
        chkLectureSeule.AutoSize = True
        chkLectureSeule.Font = New Font("Calibri", 11.25F)
        chkLectureSeule.Location = New Point(18, 298)
        chkLectureSeule.Name = "chkLectureSeule"
        chkLectureSeule.TabIndex = 5
        chkLectureSeule.Text = "Lecture seule (consultation) — via UtilsControls.DefinirLectureSeule"
        ' 
        ' lblJournal
        ' 
        lblJournal.BackColor = Color.Transparent
        lblJournal.Font = New Font("Calibri", 10.5F)
        lblJournal.ForeColor = Color.FromArgb(CByte(75), CByte(105), CByte(90))
        lblJournal.Location = New Point(18, 334)
        lblJournal.Name = "lblJournal"
        lblJournal.Size = New Size(494, 50)
        lblJournal.Text = "Dernier changement : —"
        ' 
        ' carteCalendrier
        ' 
        carteCalendrier.Controls.Add(calendrierSeul)
        carteCalendrier.Location = New Point(570, 74)
        carteCalendrier.Name = "carteCalendrier"
        carteCalendrier.Size = New Size(310, 400)
        carteCalendrier.TabIndex = 2
        carteCalendrier.Titre = "AltheaCalendrier seul"
        ' 
        ' calendrierSeul
        ' 
        calendrierSeul.Location = New Point(17, 46)
        calendrierSeul.Name = "calendrierSeul"
        calendrierSeul.Size = New Size(272, 292)
        calendrierSeul.TabIndex = 0
        ' 
        ' barreActions
        ' 
        barreActions.Controls.Add(btnFermer)
        barreActions.Dock = DockStyle.Bottom
        barreActions.Location = New Point(0, 490)
        barreActions.Name = "barreActions"
        barreActions.Size = New Size(900, 60)
        barreActions.TabIndex = 3
        ' 
        ' btnFermer
        ' 
        btnFermer.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnFermer.Font = New Font("Calibri", 11.25F)
        btnFermer.Location = New Point(770, 14)
        btnFermer.Name = "btnFermer"
        btnFermer.Size = New Size(110, 34)
        btnFermer.TabIndex = 0
        btnFermer.Text = "Fermer"
        btnFermer.Variante = VarianteBouton.Clair
        ' 
        ' TestChampDate
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(239), CByte(234))
        ClientSize = New Size(900, 550)
        Controls.Add(carteCalendrier)
        Controls.Add(carteChamps)
        Controls.Add(barreActions)
        Controls.Add(enTete)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "TestChampDate"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Démonstration du champ de date"
        carteChamps.ResumeLayout(False)
        carteChamps.PerformLayout()
        carteCalendrier.ResumeLayout(False)
        barreActions.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents enTete As AltheaEnTete
    Friend WithEvents carteChamps As AltheaCarte
    Friend WithEvents lblDateNormale As Label
    Friend WithEvents dateNormale As AltheaDate
    Friend WithEvents lblDateFacultative As Label
    Friend WithEvents dateFacultative As AltheaDate
    Friend WithEvents lblDateBornee As Label
    Friend WithEvents dateBornee As AltheaDate
    Friend WithEvents lblDateDu As Label
    Friend WithEvents dateDu As AltheaDate
    Friend WithEvents lblDateAu As Label
    Friend WithEvents dateAu As AltheaDate
    Friend WithEvents chkLectureSeule As AltheaCase
    Friend WithEvents lblJournal As Label
    Friend WithEvents carteCalendrier As AltheaCarte
    Friend WithEvents calendrierSeul As AltheaCalendrier
    Friend WithEvents barreActions As AltheaBarreActions
    Friend WithEvents btnFermer As AltheaBouton

End Class
