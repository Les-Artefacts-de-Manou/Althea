' -------------------------------------------------------------------------------------------------
' Classe      : AvoirDisponible
' Projet      : Althéa
' Version     : V1.0.2
' Date        : 17/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Source d'avoir réutilisable d'un patient, tous dossiers confondus : affectation encaissée (PAYE)
' portée par une séance devenue non due (séance reportée comprise), ou trop-perçu d'une séance
' encore due (Montant = part disponible, plafonnée au trop-perçu) — depuis le 17/10/2026.
'
' Important   :
' - DTO de lecture seule : aucune règle métier n'est portée ici.
' - Alimenté par GestionPaiements.GetAvoirsDisponiblesPourSeance.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class AvoirDisponible

    ' Identifiant de la ligne de liaison source (lia_paiements_seances.id_lia_paiements_seances)
    Public Property IdLiaison As Long

    ' Paiement encaissé d'origine
    Public Property IdPaiement As Long

    ' Code du paiement d'origine (ex. 'PAI000012')
    Public Property CodePaiement As String

    ' Séance non due portant l'avoir
    Public Property IdSeance As Long

    ' Code de la séance portant l'avoir (ex. 'SE000034')
    Public Property CodeSeance As String

    ' Montant d'avoir encore disponible sur cette source
    Public Property Montant As Decimal

End Class
