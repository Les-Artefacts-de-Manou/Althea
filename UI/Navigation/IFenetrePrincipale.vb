' -------------------------------------------------------------------------------------------------
' Interface  : IFenetrePrincipale
' Projet     : Althéa
' Version    : V1.1.0
' Date       : 25/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Ce que la fenêtre principale (Home) offre aux écrans et aux fenêtres : naviguer, revenir, ouvrir un
' référentiel ou la création d'un thérapeute en fenêtre (droits et élévation compris), tenir le fil
' d'Ariane et rafraîchir le bandeau.
'
' Remarques  :
' - Audit C.4 (ADR-55, décision de Joëlle du 25/10/2026) : les écrans ne connaissent plus la classe
'   Home (« TryCast(FindForm(), Home) », « homeForm As Home ») mais ce contrat. Un écran le trouve
'   dans son contexte (UserControlContext.FenetrePrincipale) ; une fenêtre le reçoit en paramètre.
' - Nothing est une valeur normale : écran hébergé dans une fenêtre de référentiel
'   (ReferentielModalHost) ou fenêtre ouverte hors de la navigation principale. L'appelant masque
'   alors ce qui en dépend (boutons [+]), comme il le faisait sans Home.
' - Seul Home l'implémente aujourd'hui ; un autre hôte (tests, futur client) pourrait le faire.
'
' - V1.1.0 (25/10/2026, audit C.4 étape 3) : DemanderElevation (élévation réunie, décision de Joëlle).
'
' Dépendances :
' - Home (implémentation), UserControlContext (accès des écrans), NavigationEntry (pile de retour)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Interface IFenetrePrincipale

#Region "Navigation"

    ' Écrans du menu principal.
    Sub NavigateToAccueil()
    Sub NavigateToPatients()
    Sub NavigateToRecherche()

    ' Écran d'administration ou de référentiel, avec son fil d'Ariane (« Administration > … »).
    Sub NavigateToAdminView(view As UserControl, statusText As String)
    Sub NavigateToReferentielView(view As UserControl, statusText As String)

    ' Ouverture d'une fiche en gardant l'écran courant dans la pile de retour (creerEcranCourant le
    ' reconstruit au retour, critères compris).
    Sub NavigateToPatientFiche(fiche As UserControl, contexte As String, creerEcranCourant As Func(Of UserControl))
    Sub NavigateToDossierFiche(hub As UserControl, contexte As String, creerEcranCourant As Func(Of UserControl))
    Sub NavigateDepuisRecherche(vue As UserControl, contexte As String, creerEcranCourant As Func(Of UserControl),
                                Optional contexteRetour As String = OuvertureResultat.CONTEXTE_RECHERCHE)

    ' Retour à l'écran précédent ; False s'il n'y en a pas.
    Function NavigateRetour() As Boolean

#End Region

#Region "Fenêtres (droits et élévation gérés par la fenêtre principale)"

    ' Référentiel ouvert en fenêtre depuis un bouton [+] ; False si l'accès a été refusé.
    Function OuvrirReferentielModal(vueReferentiel As UserControl, titre As String) As Boolean

    ' Création d'un thérapeute ; Nothing si elle a été annulée ou refusée.
    Function OuvrirCreationTherapeuteModal() As Therapeute

    ' Élévation d'accès demandée par un écran (bouton des hubs) ; contexteTemporaire = fil d'Ariane
    ' pendant la fenêtre. True si elle est accordée (bandeau rafraîchi).
    Function DemanderElevation(contexteTemporaire As String) As Boolean

#End Region

#Region "Fil d'Ariane et bandeau"

    Function BuildAdminContexte(sousContexte As String) As String
    Function BuildReferentielContexte(sousContexte As String) As String
    Sub SetContexte(contexte As String)
    Function PushContexteTemporaire(contexteTemporaire As String) As String

    ' Bandeau : utilisateur et rôle courant, cabinet, photo de l'utilisateur.
    Sub UpdateConnectedUserDisplay()
    Sub RafraichirBandeauCabinet()
    Sub RafraichirPhotoUtilisateur()

#End Region

End Interface
