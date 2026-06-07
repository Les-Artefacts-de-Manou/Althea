' -------------------------------------------------------------------------------------------------
' Classe      : AuthenticationResult
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Résultat d'une tentative d'authentification utilisateur.
'
' Responsabilités :
' - Transmettre le succès ou l'échec de l'authentification
' - Transmettre l'utilisateur authentifié (si succès)
' - Transmettre un message UI contrôlé (sans détails sensibles)
' - Transmettre le nombre d'essais restants avant verrouillage
' - Transmettre l'état de verrouillage du compte
'
' Remarques   :
' - Aucun mot de passe stocké
' - Aucun détail sensible exposé (messages génériques)
'
' Dépendances :
' - GestionAuthentification.AuthentifierUtilisateur
' - Utilisée par Login (formulaire de connexion)
' -------------------------------------------------------------------------------------------------

Public Class AuthenticationResult

#Region "Propriétés publiques"

    ' Indique si l'authentification a réussi
    Public Property Success As Boolean

    ' Utilisateur authentifié (Nothing si échec)
    Public Property Utilisateur As UtilisateurApplication

    ' Message à afficher dans l'interface utilisateur (message générique, sans détails sensibles)
    Public Property MessageUI As String

    ' Nombre de tentatives restantes avant verrouillage
    Public Property RemainingAttempts As Integer

    ' Indique si le compte est verrouillé
    Public Property IsLocked As Boolean

#End Region

End Class
