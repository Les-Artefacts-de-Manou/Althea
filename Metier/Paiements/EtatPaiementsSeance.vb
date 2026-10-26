' -------------------------------------------------------------------------------------------------
' Classe      : EtatPaiementsSeance
' Projet      : Althéa
' Version     : V1.1.1
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Décompte des paiements d'une séance par état. Sert à décider, côté métier, s'il faut créer,
' réactiver ou annuler le dû d'une séance lors d'un changement de statut, sans avoir à charger
' l'intégralité des lignes de paiement.
'
' Important   :
' - DTO de synthèse, sans accès base de données ni règle de persistance.
' - Les statuts sont interprétés par la couche métier ; cette classe ne les modifie pas.
'
' Dépendances :
' - Alimenté par GestionPaiements et utilisé lors des transitions de séance.
'
' Historique :
' - V1.1.1 (25/10/2026, audit D-1) : Option Strict On.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class EtatPaiementsSeance

#Region "Propriétés"

    ' Nombre de paiements encaissés (statut 'PAYE')
    Public Property NbPayes As Integer

    ' Nombre de paiements dus non encaissés (statut 'EN_ATTENTE')
    Public Property NbEnAttente As Integer

    ' Nombre de paiements annulés (statut 'ANNULE')
    Public Property NbAnnules As Integer

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : ADejaEncaisse
    ' Version    : V1.1.0
    ' Date       : 17/09/2026
    '
    ' Rôle       :
    ' Indique qu'au moins un encaissement a eu lieu ; le dû ne doit alors jamais être écrasé.
    '
    ' Retour     :
    ' - Boolean : True lorsqu'au moins un paiement est encaissé.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property ADejaEncaisse As Boolean
        Get
            Return NbPayes > 0
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstVide
    ' Version    : V1.1.0
    ' Date       : 17/09/2026
    '
    ' Rôle       :
    ' Indique qu'aucune ligne de paiement n'existe encore pour la séance.
    '
    ' Retour     :
    ' - Boolean : True si tous les compteurs sont à zéro.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstVide As Boolean
        Get
            Return NbPayes = 0 AndAlso NbEnAttente = 0 AndAlso NbAnnules = 0
        End Get
    End Property

#End Region

End Class
