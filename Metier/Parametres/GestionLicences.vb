' -------------------------------------------------------------------------------------------------
' Module      : GestionLicences
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Licences des composants tiers propres au poste : aujourd'hui la clé Syncfusion (exports PDF et
' Word de l'éditeur de texte).
'
' Responsabilités :
' - Remettre la clé enregistrée à Syncfusion au démarrage (AppliquerLicenceSyncfusion)
' - Vérifier la forme d'une clé saisie (CleSyncfusionValide)
' - Enregistrer une nouvelle clé : droits (Admin), forme de la clé, remise immédiate à Syncfusion
'   (DefinirCleSyncfusion)
'
' Sécurité :
' - La clé n'est jamais journalisée ni affichée ; seul son état (définie / absente) l'est.
' - Stockage : LicenceSyncfusion (%APPDATA%, chiffrée par DPAPI), jamais en base ni dans le dépôt.
'
' Remarques   :
' - Sans clé, l'application fonctionne : seuls les exports PDF et Word portent l'avertissement de
'   licence de Syncfusion. Le démarrage ne doit donc jamais échouer à cause de la licence.
' - Une clé enregistrée en cours de session est remise à Syncfusion aussitôt : pas de redémarrage.
'
' Dépendances :
' - LicenceSyncfusion, Syncfusion.Licensing, GestionLog, AppRole, UtilisateurApplication
' - V1.1.0 (22/10/2026, audit C.1) : DefinirCleSyncfusion reçoit un ContexteOperation (Autorisation.Exiger).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports Syncfusion.Licensing

Public Module GestionLicences

    ' Une clé Syncfusion est un texte en base 64 d'environ 90 caractères : en dessous de cette
    ' longueur, la saisie est sûrement tronquée.
    Private Const LongueurMinCleSyncfusion As Integer = 40

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : AppliquerLicenceSyncfusion
    ' Version    : V1.0.0
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Remet à Syncfusion la clé enregistrée sur ce poste, avant toute utilisation de ses composants.
    '
    ' Remarques  :
    ' - Appelée au démarrage (AppStartupManager.Run). Ne lève jamais d'exception : une clé absente
    '   ou illisible (autre compte Windows) est seulement journalisée.
    ' -------------------------------------------------------------------------------------------------
    Public Sub AppliquerLicenceSyncfusion()

        Try
            Dim cle As String = LicenceSyncfusion.Obtenir()

            If String.IsNullOrEmpty(cle) Then
                GestionLog.EcrireLog("Licence Syncfusion absente sur ce poste : les exports PDF et Word porteront l'avertissement de licence.",
                                     GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Startup)
                Return
            End If

            SyncfusionLicenseProvider.RegisterLicense(cle)
            GestionLog.EcrireLog("Licence Syncfusion enregistrée.",
                                 GestionLog.LogLevel.Rapide, GestionLog.LogCategory.Startup)

        Catch ex As Exception
            GestionLog.EcrireLog("Licence Syncfusion illisible sur ce compte Windows : à ressaisir dans Paramètres.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Startup, ex)
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DefinirCleSyncfusion
    ' Version    : V1.0.0
    ' Date       : 18/10/2026
    '
    ' Rôle       :
    ' Enregistre (ou remplace) la clé Syncfusion de ce poste et la remet aussitôt à Syncfusion.
    '
    ' Paramètres :
    ' - contexte    : qui enregistre (Admin exigé, élévation comprise ; login au journal)
    ' - cle         : clé copiée depuis le compte Syncfusion (espaces et retours à la ligne ignorés)
    '
    ' Exceptions :
    ' - UnauthorizedAccessException : rôle insuffisant
    ' - ArgumentException           : clé vide ou d'une forme inattendue
    '
    ' Remarques  :
    ' - Journalisé sans jamais écrire la clé.
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirCleSyncfusion(contexte As ContexteOperation, cle As String)

        Autorisation.Exiger(contexte, AppRole.Admin, "Seul l'administrateur peut enregistrer la clé de licence.")

        Dim cleNormalisee As String = NormaliserCle(cle)
        If Not CleSyncfusionPlausible(cleNormalisee) Then
            Throw New ArgumentException("Ce texte ne ressemble pas à une clé de licence Syncfusion. " &
                                        "Copiez-la en entier depuis votre compte Syncfusion.")
        End If

        Dim remplacement As Boolean = LicenceSyncfusion.EstDefinie()
        LicenceSyncfusion.Enregistrer(cleNormalisee)
        SyncfusionLicenseProvider.RegisterLicense(cleNormalisee)

        GestionLog.EcrireLog($"Clé de licence Syncfusion {If(remplacement, "remplacée", "enregistrée")} par {If(String.IsNullOrEmpty(contexte.LoginUtilisateur), "?", contexte.LoginUtilisateur)}.",
                             GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security)

    End Sub

    ''' <summary>
    ''' Indique si un texte a la forme d'une clé Syncfusion (espaces et retours à la ligne ignorés).
    ''' Permet à l'écran de refuser une saisie avant d'enregistrer quoi que ce soit.
    ''' </summary>
    Public Function CleSyncfusionValide(cle As String) As Boolean
        Return CleSyncfusionPlausible(NormaliserCle(cle))
    End Function

    ' Une copie depuis une page web ou un courriel peut couper la clé en lignes ou l'entourer d'espaces.
    Private Function NormaliserCle(cle As String) As String
        If cle Is Nothing Then Return String.Empty
        Return New String(cle.Where(Function(c) Not Char.IsWhiteSpace(c)).ToArray())
    End Function

    ' Forme attendue : caractères de la base 64 seulement, longueur suffisante.
    Private Function CleSyncfusionPlausible(cle As String) As Boolean
        If cle.Length < LongueurMinCleSyncfusion Then Return False
        Return cle.All(Function(c) (c >= "A"c AndAlso c <= "Z"c) OrElse (c >= "a"c AndAlso c <= "z"c) OrElse
                                   (c >= "0"c AndAlso c <= "9"c) OrElse c = "+"c OrElse c = "/"c OrElse c = "="c)
    End Function

End Module
