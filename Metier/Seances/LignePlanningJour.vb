' -------------------------------------------------------------------------------------------------
' Classe      : LignePlanningJour
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Une ligne de la fenêtre « Planning du jour » : une séance, ou un créneau libre entre deux
' séances (UI-9.2, maquette « Grilles — états lisibles »).
'
' Remarques   :
' - Produite par GestionSeances.GetPlanningJour ; objet de lecture, sans accès base.
' - Les propriétés HeureDebut, HeureFin, NomCompletPatient, LibelleDomaine, CodeSeance,
'   LibelleStatutSeance et TarifSeance portent les noms des colonnes de la grille (DataPropertyName) : vides pour un
'   créneau libre, dessiné comme un repère sur toute la ligne.
' - V1.1.0 (24/10/2026) : CodeSeance, colonne Code de la grille (code toujours visible).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public NotInheritable Class LignePlanningJour

    ' Séance de la ligne ; Nothing pour un créneau libre
    Public ReadOnly Property Seance As SeancePlanningItem

    ' Début et fin d'un créneau libre (sans objet pour une séance)
    Public ReadOnly Property DebutLibre As TimeSpan
    Public ReadOnly Property FinLibre As TimeSpan

    Public Sub New(seance As SeancePlanningItem)
        Me.Seance = seance
    End Sub

    Private Sub New(debut As TimeSpan, fin As TimeSpan)
        DebutLibre = debut
        FinLibre = fin
    End Sub

    ''' <summary>Créneau libre de debut à fin.</summary>
    Public Shared Function CreneauLibre(debut As TimeSpan, fin As TimeSpan) As LignePlanningJour
        Return New LignePlanningJour(debut, fin)
    End Function

    Public ReadOnly Property EstLibre As Boolean
        Get
            Return Seance Is Nothing
        End Get
    End Property

    ' Titre du repère d'un créneau libre : « Libre · 10:20 – 14:00 »
    Public ReadOnly Property LibelleLibre As String
        Get
            Return $"Libre · {DebutLibre:hh\:mm} – {FinLibre:hh\:mm}"
        End Get
    End Property

    ' Ordre des lignes : par heure ; une séance sans horaire en tête
    Public ReadOnly Property CleTri As TimeSpan
        Get
            If EstLibre Then Return DebutLibre
            Return If(Seance.HeureDebut, TimeSpan.MinValue)
        End Get
    End Property

#Region "Colonnes de la grille"

    Public ReadOnly Property HeureDebut As TimeSpan?
        Get
            Return If(EstLibre, Nothing, Seance.HeureDebut)
        End Get
    End Property

    Public ReadOnly Property HeureFin As TimeSpan?
        Get
            Return If(EstLibre, Nothing, Seance.HeureFin)
        End Get
    End Property

    Public ReadOnly Property NomCompletPatient As String
        Get
            Return If(EstLibre, String.Empty, Seance.NomCompletPatient)
        End Get
    End Property

    Public ReadOnly Property LibelleDomaine As String
        Get
            Return If(EstLibre, String.Empty, Seance.LibelleDomaine)
        End Get
    End Property

    ' Code de la séance (SE…), toujours visible dans une grille de séances (Rules § 15)
    Public ReadOnly Property CodeSeance As String
        Get
            Return If(EstLibre, String.Empty, Seance.CodeSeance)
        End Get
    End Property

    ' Statut tel que la grille des séances l'affiche (« À confirmer » pour une séance planifiée passée)
    Public ReadOnly Property LibelleStatutSeance As String
        Get
            Return If(EstLibre, String.Empty, Seance.LibelleStatutAffichage)
        End Get
    End Property

    Public ReadOnly Property TarifSeance As Decimal?
        Get
            Return If(EstLibre, Nothing, CType(Seance.TarifSeance, Decimal?))
        End Get
    End Property

#End Region

End Class
