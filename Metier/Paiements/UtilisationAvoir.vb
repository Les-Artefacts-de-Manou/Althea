' -------------------------------------------------------------------------------------------------
' Classe      : UtilisationAvoir
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Une utilisation d'avoir, lue dans lia_paiements_avoirs (migration v2.52.0) : la part d'un
' encaissement (source) qui a servi à régler une autre séance par un paiement au mode « Avoir ».
'
' Important   :
' - DTO de lecture seule : aucune règle métier n'est portée ici, en dehors des phrases d'affichage.
' - Alimenté par GestionPaiements (GetPaiementsParDossier, GetPaiementsParSeance), qui le rattache
'   aux deux lignes concernées : l'encaissement source (PaiementListeItem.UtilisationsAvoirSource)
'   et le règlement par avoir (PaiementListeItem.OriginesAvoir).
' - Auteur : nom affiché de l'utilisateur, lu par jointure au moment de l'affichage (seul son
'   identifiant est enregistré).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UtilisationAvoir

    ' Règlement par avoir créé sur la séance réglée
    Public Property IdPaiementAvoir As Long

    ' Encaissement d'origine et séance qui portait l'avoir
    Public Property IdPaiementSource As Long
    Public Property CodePaiementSource As String
    Public Property IdSeanceSource As Long
    Public Property CodeSeanceSource As String

    ' Séance réglée avec cet avoir
    Public Property CodeSeanceReglee As String

    ' Part prise sur cette source
    Public Property Montant As Decimal

    ' Quand, et par qui (nom affiché ; vide s'il n'est pas connu)
    Public Property DateUtilisation As Date
    Public Property Auteur As String

    ' Phrase côté encaissement source : « Avoir 20,00 € utilisé le jj/mm/aaaa par X pour S000050 »
    ' (même forme que les mentions écrites dans le commentaire avant le 22/10/2026).
    Public ReadOnly Property MentionSource As String
        Get
            Dim parAuteur As String = If(String.IsNullOrWhiteSpace(Auteur), String.Empty, $" par {Auteur.Trim()}")
            Return $"Avoir {Montant:N2} € utilisé le {DateUtilisation:dd/MM/yyyy}{parAuteur} pour {CodeSeanceReglee}"
        End Get
    End Property

    ' Origine côté règlement par avoir : « PAI000012 (S000045) 20,00 € ».
    Public ReadOnly Property LibelleOrigine As String
        Get
            Return $"{CodePaiementSource} ({CodeSeanceSource}) {Montant:N2} €"
        End Get
    End Property

End Class
