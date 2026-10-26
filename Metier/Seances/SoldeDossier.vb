' -------------------------------------------------------------------------------------------------
' Classe      : SoldeDossier
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Situation financière d'un dossier : montant à régler et avoir, calculés à partir de ses séances
' (mêmes règles que les grilles : SeanceListeItem.MontantExigible et AvoirSeance).
'
' Remarques   :
' - Produit par GestionSeances.CalculerSoldesPatient, une ligne par dossier du patient : le
'   bandeau du dossier montre ainsi le dossier ouvert et le total du patient (décision du
'   17/10/2026 — l'avoir appartient au patient et sert dans tous ses dossiers).
' - DTO de lecture : aucune règle ni accès base ici.
' - V1.1.0 (22/10/2026) : MontantPrevu (ligne « Prévu » du bandeau).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class SoldeDossier

    ' Identifiant du dossier
    Public Property IdDossier As Long

    ' Code du dossier (ex. 'DO000028'), pour le détail en infobulle
    Public Property CodeDossier As String

    ' Somme à régler maintenant : reste dû des séances dues et passées du dossier
    Public Property MontantExigible As Decimal

    ' Reste à payer des séances dues pas encore passées (SeanceListeItem.MontantPrevisionnel) : une
    ' prévision, pas un dû
    Public Property MontantPrevu As Decimal

    ' Avoir du dossier : encaissé sur des séances non dues, ou au-delà du montant d'une séance due
    Public Property Avoir As Decimal

End Class
