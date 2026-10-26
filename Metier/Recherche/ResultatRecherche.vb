' -------------------------------------------------------------------------------------------------
' Classe      : ResultatRecherche
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Un élément trouvé par la recherche transversale (ADR-52), quelle que soit la recherche : ce que
' c'est (nature), son code, une description lisible et de quoi y aller (patient, dossier, séance,
' paiement…).
'
' Remarques   :
' - Même forme pour toutes les recherches : chaque écran de recherche choisit ses colonnes, une seule
'   routine d'écran décide où aller selon la nature (OuvertureResultat).
' - Aucun accès base de données, aucune logique UI.
' - V1.1.0 (24/10/2026, décision de Joëlle) : nature Thérapeute (code TH…, recherche par code),
'   IdTherapeute et NomElement (nom d'un élément qui n'est pas un patient).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

' Nature d'un élément trouvé ; décide de l'écran où aller.
Public Enum NatureResultat
    Patient
    Dossier
    Seance
    Paiement
    Contact
    Intervenant
    Therapeute
End Enum

Public Class ResultatRecherche

#Region "Propriétés"

    Public Property Nature As NatureResultat

    ' Code de l'élément (PA000005, DO000012, SE000083…)
    Public Property Code As String = String.Empty

    ' Description lisible (date, statut, domaine, montant…), composée par GestionRecherche
    Public Property Description As String = String.Empty

    ' Date de l'élément (naissance, ouverture, séance, paiement…) ; Nothing si sans objet
    Public Property DateElement As Date?

    ' Patient de l'élément (toujours renseigné quand l'élément appartient à un patient)
    Public Property IdPatient As Long
    Public Property CodePatient As String = String.Empty
    Public Property NomPatient As String = String.Empty

    ' Dossier, séance, paiement de l'élément (Nothing si sans objet)
    Public Property IdDossier As Long?
    Public Property CodeDossier As String = String.Empty
    Public Property IdSeance As Long?
    Public Property IdPaiement As Long?

    ' Thérapeute du référentiel (nature Thérapeute) : identifiant et nom (« NOM Prénom »)
    Public Property IdTherapeute As Long?
    Public Property NomElement As String = String.Empty

#End Region

#Region "Propriétés calculées"

    ' Libellé de la nature pour la grille.
    Public ReadOnly Property LibelleNature As String
        Get
            Return LibelleDeNature(Nature)
        End Get
    End Property

    ' Libellé d'une nature (grille, aide des codes).
    Public Shared Function LibelleDeNature(nature As NatureResultat) As String
        Select Case nature
            Case NatureResultat.Patient : Return "Patient"
            Case NatureResultat.Dossier : Return "Dossier"
            Case NatureResultat.Seance : Return "Séance"
            Case NatureResultat.Paiement : Return "Paiement"
            Case NatureResultat.Contact : Return "Contact"
            Case NatureResultat.Intervenant : Return "Intervenant"
            Case NatureResultat.Therapeute : Return "Thérapeute"
            Case Else : Return String.Empty
        End Select
    End Function

    ' Patient pour la grille : « PA000005 · NOM Prénom » (vide si l'élément n'a pas de patient).
    Public ReadOnly Property PatientAffichage As String
        Get
            If IdPatient <= 0 Then Return String.Empty
            Return $"{CodePatient} · {NomPatient}".Trim()
        End Get
    End Property

#End Region

End Class
