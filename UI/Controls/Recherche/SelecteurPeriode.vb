' -------------------------------------------------------------------------------------------------
' Classe      : SelecteurPeriode
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Choix d'une période dans les recherches (ADR-52) : relie les champs « du … au … » et les boutons
' d'un écran (Aujourd'hui, Cette semaine, Ce mois, période précédente / suivante) et signale chaque
' nouvelle période par un seul évènement.
'
' Responsabilités :
' - Garder la période dans le bon ordre (AltheaDate.CorrigerBornes)
' - Raccourcis : aujourd'hui ; semaine en cours (lundi à dimanche) ; mois en cours
' - Période précédente / suivante : un mois entier passe au mois voisin entier, toute autre période
'   glisse de sa propre durée (un jour, une semaine, dix jours…)
' - Un seul PeriodeChangee par geste (les deux champs posés ensemble ne déclenchent qu'une recherche)
'
' Remarques   :
' - Sortie de UC_RechercheSeances (R2) pour servir aussi à la recherche des paiements (R3) : chaque
'   écran garde ses contrôles dans son Designer, la logique est ici.
' - Classe sans interface propre : elle ne crée aucun contrôle.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Friend NotInheritable Class SelecteurPeriode

    ' La période a changé (geste de l'utilisatrice ou Definir avec notification).
    Public Event PeriodeChangee As EventHandler

    Private ReadOnly _du As AltheaDate
    Private ReadOnly _au As AltheaDate
    Private ReadOnly _boutons As (Precedent As Control, Suivant As Control, Aujourdhui As Control, Semaine As Control, Mois As Control)

    ' Vrai pendant que la classe pose elle-même les deux dates.
    Private _miseAJour As Boolean

    Public Sub New(du As AltheaDate, au As AltheaDate, precedent As Control, suivant As Control,
                   aujourdhui As Control, semaine As Control, mois As Control)

        _du = du
        _au = au
        _boutons = (precedent, suivant, aujourdhui, semaine, mois)

        AddHandler _du.ValeurChangee, AddressOf Date_ValeurChangee
        AddHandler _au.ValeurChangee, AddressOf Date_ValeurChangee
        AddHandler precedent.Click, Sub(s, e) Decaler(-1)
        AddHandler suivant.Click, Sub(s, e) Decaler(1)
        AddHandler aujourdhui.Click, Sub(s, e) Definir(Date.Today, Date.Today)
        AddHandler semaine.Click,
            Sub(s, e)
                Dim lundi As Date = LundiDe(Date.Today)
                Definir(lundi, lundi.AddDays(6))
            End Sub
        AddHandler mois.Click,
            Sub(s, e)
                Dim premier As New Date(Date.Today.Year, Date.Today.Month, 1)
                Definir(premier, premier.AddMonths(1).AddDays(-1))
            End Sub

    End Sub

#Region "Période"

    Public ReadOnly Property Du As Date
        Get
            Return _du.Valeur.Value
        End Get
    End Property

    Public ReadOnly Property Au As Date
        Get
            Return _au.Valeur.Value
        End Get
    End Property

    ' La période est un seul jour (messages « ce jour-là »).
    Public ReadOnly Property EstUnJour As Boolean
        Get
            Return Du = Au
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Definir
    ' Rôle      : Pose la période (bornes remises dans l'ordre) ; notifier = False au chargement d'un
    '             écran, qui lance lui-même sa première recherche.
    ' -------------------------------------------------------------------------------------------------
    Public Sub Definir(du As Date, au As Date, Optional notifier As Boolean = True)

        If au < du Then au = du

        _miseAJour = True
        Try
            _du.Valeur = du
            _au.Valeur = au
        Finally
            _miseAJour = False
        End Try

        If notifier Then RaiseEvent PeriodeChangee(Me, EventArgs.Empty)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Decaler
    ' Rôle      : Période précédente (sens = -1) ou suivante (sens = +1) ; voir l'en-tête.
    ' -------------------------------------------------------------------------------------------------
    Public Sub Decaler(sens As Integer)

        Dim moisEntier As Boolean = Du.Day = 1 AndAlso Au = Du.AddMonths(1).AddDays(-1)

        If moisEntier Then
            Dim debut As Date = Du.AddMonths(sens)
            Definir(debut, debut.AddMonths(1).AddDays(-1))
        Else
            Dim duree As Integer = (Au - Du).Days + 1
            Definir(Du.AddDays(sens * duree), Au.AddDays(sens * duree))
        End If

    End Sub

    ' Lundi de la semaine d'une date.
    Private Shared Function LundiDe(jour As Date) As Date
        Dim ecart As Integer = (CInt(jour.DayOfWeek) + 6) Mod 7
        Return jour.Date.AddDays(-ecart)
    End Function

    Private Sub Date_ValeurChangee(sender As Object, e As EventArgs)
        If _miseAJour Then Return
        ' La borne corrigée relève ValeurChangee : une seule notification pour le geste.
        _miseAJour = True
        Try
            AltheaDate.CorrigerBornes(_du, _au, sender)
        Finally
            _miseAJour = False
        End Try
        RaiseEvent PeriodeChangee(Me, EventArgs.Empty)
    End Sub

#End Region

#Region "Infobulles"

    ' Infobulles des boutons de période, les mêmes dans toutes les recherches.
    Public Sub DefinirInfobulles(context As UserControlContext, elements As String)
        If context Is Nothing Then Return
        context.SetToolTip(_boutons.Precedent, "Période précédente (même durée ; mois précédent pour un mois entier).")
        context.SetToolTip(_boutons.Suivant, "Période suivante (même durée ; mois suivant pour un mois entier).")
        context.SetToolTip(_boutons.Aujourdhui, $"Les {elements} d'aujourd'hui.")
        context.SetToolTip(_boutons.Semaine, $"Les {elements} de la semaine en cours (lundi à dimanche).")
        context.SetToolTip(_boutons.Mois, $"Les {elements} du mois en cours.")
    End Sub

#End Region

End Class
