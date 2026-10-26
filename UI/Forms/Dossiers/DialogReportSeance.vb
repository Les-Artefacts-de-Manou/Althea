' -------------------------------------------------------------------------------------------------
' Formulaire  : DialogReportSeance
' Projet      : Althéa
' Version     : V1.5.0
' Date        : 17/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Petite boîte de dialogue modale permettant de saisir la nouvelle date (et les horaires
' optionnels) lors du report d'une séance.
'
' Responsabilités :
' - Proposer une date de report et, facultativement, des horaires cohérents.
' - Exposer les valeurs sélectionnées à l’appelant après DialogResult.OK.
'
' Important   :
' - Disposition et apparence portées par DialogReportSeance.Designer.vb (Designer obligatoire).
' - La persistance, le contrôle de chevauchement et le tarif restent délégués à GestionSeances.
' - V1.3.0 (15/10/2026, UI-7) : carte « Nouvelle date de la séance », fond AltheaFondFenetre, date
'   AltheaDate, case AltheaCase.
' - V1.4.0 (16/10/2026, UI-7) : heures en AltheaHeure (hh:mm, liste des créneaux paramétrés, durée
'   rappelée dans la liste des fins) à la place des DateTimePicker Windows.
' - V1.5.0 (17/10/2026) : la séance reportée est nommée par son code dans l'en-tête (paramètre codeSeance).
'
' Dépendances :
' - GestionSeances (durée standard et application du report).
' - SeanceEditor / UC_DossierFiche (appelants).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class DialogReportSeance

#Region "Constantes"

    ' Durée standard d'une séance, en minutes (paramètre lu une fois à l'ouverture).
    Private ReadOnly _dureeSeanceMinutes As Integer = GestionSeances.DureeSeanceParDefaut()

#End Region

#Region "Propriétés publiques"

    ' Nouvelle date choisie pour la séance reportée
    Public ReadOnly Property NouvelleDate As Date
        Get
            Return dtpDate.Valeur.Value.Date
        End Get
    End Property

    ' Heure de début choisie (Nothing si les horaires ne sont pas renseignés)
    Public ReadOnly Property NouvelleHeureDebut As TimeSpan?
        Get
            If Not chkHoraires.Checked Then
                Return Nothing
            End If
            Return hreDebut.Valeur
        End Get
    End Property

    ' Heure de fin choisie (Nothing si les horaires ne sont pas renseignés)
    Public ReadOnly Property NouvelleHeureFin As TimeSpan?
        Get
            If Not chkHoraires.Checked Then
                Return Nothing
            End If
            Return hreFin.Valeur
        End Get
    End Property

#End Region

#Region "Constructeur"

    ' codeSeance : séance reportée, nommée dans l'en-tête (facultatif).
    Public Sub New(dateSuggeree As Date, Optional heureDebutSuggeree As TimeSpan? = Nothing, Optional heureFinSuggeree As TimeSpan? = Nothing,
                   Optional codeSeance As String = "")

        InitializeComponent()

        ' Fond du kit : beige clair et petite feuille dessinée.
        AltheaFondFenetre.Appliquer(Me)

        If Not String.IsNullOrWhiteSpace(codeSeance) Then
            enTete.SousTitre = $"Nouvelle date pour la séance {codeSeance}"
        End If

        dtpDate.Valeur = dateSuggeree

        ' Reprise des horaires de la séance d'origine (heure identique), sinon 9:00.
        Dim debutInitial As TimeSpan = If(heureDebutSuggeree.HasValue, heureDebutSuggeree.Value, New TimeSpan(9, 0, 0))
        Dim finInitiale As TimeSpan = If(heureFinSuggeree.HasValue, heureFinSuggeree.Value, debutInitial.Add(New TimeSpan(0, _dureeSeanceMinutes, 0)))

        ' Mêmes heures proposées et même pas que la fiche séance.
        Dim creneaux As List(Of TimeSpan) = GestionSeances.CreneauxProposes()
        If creneaux.Count >= 2 Then
            Dim pas As Integer = CInt((creneaux(1) - creneaux(0)).TotalMinutes)
            hreDebut.Pas = pas
            hreFin.Pas = pas
        End If
        hreDebut.HeuresProposees = creneaux
        hreFin.HeuresProposees = creneaux

        hreDebut.Valeur = debutInitial
        hreFin.Valeur = finInitiale
        hreFin.HeureReference = debutInitial

        ' Si la séance d'origine avait des horaires, on les présente cochés par défaut.
        chkHoraires.Checked = heureDebutSuggeree.HasValue

    End Sub

#End Region

#Region "Événements"

    Private Sub ChkHoraires_CheckedChanged(sender As Object, e As EventArgs) Handles chkHoraires.CheckedChanged
        hreDebut.Enabled = chkHoraires.Checked
        hreFin.Enabled = chkHoraires.Checked
    End Sub

    ' Ajuste automatiquement l'heure de fin à début + la durée standard d'une séance
    ' (modifiable ensuite manuellement).
    Private Sub HreDebut_ValeurChangee(sender As Object, e As EventArgs) Handles hreDebut.ValeurChangee
        If Not hreDebut.Valeur.HasValue Then Return
        hreFin.HeureReference = hreDebut.Valeur
        hreFin.Valeur = hreDebut.Valeur.Value.Add(TimeSpan.FromMinutes(_dureeSeanceMinutes))
    End Sub

#End Region

End Class
