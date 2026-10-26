' -------------------------------------------------------------------------------------------------
' Classe      : ContexteOperation
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Dit QUI fait une opération : l'utilisateur authentifié et son rôle COURANT (élévation comprise).
' C'est la seule chose qu'un écran transmet à une méthode métier qui vérifie un droit ou trace un
' auteur (audit C.1, décision de Joëlle du 22/10/2026).
'
' Responsabilités :
' - Porter l'utilisateur (identifiant, login) et le rôle courant de la session
' - Rester immuable : un contexte décrit l'instant où l'opération est demandée
'
' Important   :
' - Construit par l'écran depuis son contexte (UserControlContext.ContexteOperation) ; jamais par le
'   métier lui-même.
' - Les droits se jugent sur RoleCourant, élévation comprise (décision de Joëlle, 20/10/2026, A.3).
' - Le contrôle lui-même est fait par Autorisation (Exiger, ExigerUtilisateur).
' - Remplace les paires de paramètres (roleCourant, idUtilisateur / utilisateur) introduites aux lots
'   A et B ; prêt pour une version web, où le contexte viendra de la requête.
'
' Dépendances :
' - UtilisateurApplication (Metier/Security), AppRole (Core/Security)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public NotInheritable Class ContexteOperation

    ' utilisateur : utilisateur authentifié (Nothing si aucun) ; roleCourant : rôle courant de la
    ' session, élévation comprise.
    Public Sub New(utilisateur As UtilisateurApplication, roleCourant As AppRole)
        Me.Utilisateur = utilisateur
        Me.RoleCourant = roleCourant
    End Sub

    ' Utilisateur authentifié (Nothing hors session).
    Public ReadOnly Property Utilisateur As UtilisateurApplication

    ' Rôle courant de la session, élévation comprise.
    Public ReadOnly Property RoleCourant As AppRole

    ' Identifiant de l'utilisateur (0 si aucun) : c'est lui qui est enregistré comme auteur.
    Public ReadOnly Property IdUtilisateur As Long
        Get
            Return If(Utilisateur Is Nothing, 0L, Utilisateur.IdUtilisateur)
        End Get
    End Property

    ' Login de l'utilisateur (vide si aucun) : traces techniques existantes (journal des sauvegardes).
    Public ReadOnly Property LoginUtilisateur As String
        Get
            Return If(Utilisateur?.LoginUtilisateur, String.Empty)
        End Get
    End Property

End Class
