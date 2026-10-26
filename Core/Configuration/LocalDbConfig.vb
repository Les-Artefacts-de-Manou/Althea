' -------------------------------------------------------------------------------------------------
' Classe DTO      : LocalDbConfig
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 21/04/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Représente la configuration locale de connexion à la base de données MariaDB.
' Cette classe correspond au contenu du fichier JSON stocké localement (%APPDATA%\Althea).
'
' Table correspondante dans la base de données :    tec_parametres
'
' Responsabilités :
' - Contenir les paramètres nécessaires à la connexion DB
'
'
' Remarques   :
' - Le mot de passe est stocké uniquement sous forme chiffrée (EncryptedPassword)
' - La validité de la configuration est évaluée via la propriété IsConfigurationValid
'
' Dépendances :
' - None
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class LocalDbConfig

#Region "Champs privés"

    ' Champ privé pour stocker la valeur du port, avec validation dans la propriété publique
    Private _port As Integer

#End Region

#Region "Propriétés de connexion"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DatabaseName
    ' Type       : String
    ' Description : Nom de la base de données MariaDB à laquelle se connecter
    ' -------------------------------------------------------------------------------------------------
    Public Property DatabaseName As String

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UserName
    ' Type       : String
    ' Description : Nom d'utilisateur pour la connexion à la base de données
    ' -------------------------------------------------------------------------------------------------
    Public Property UserName As String

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EncryptedPassword
    ' Type       : String
    ' Description : Mot de passe chiffré pour la connexion à la base de données (DPAPI)
    ' -------------------------------------------------------------------------------------------------
    Public Property EncryptedPassword As String

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : AdditionalOptions
    ' Type       : String
    ' Description : Options supplémentaires pour la connexion (SSL, timeout, etc.)
    ' -------------------------------------------------------------------------------------------------
    Public Property AdditionalOptions As String

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : LastConnectionTestUtc
    ' Type       : DateTime? (Nullable)
    ' Description : Date du dernier test de connexion réussi (optionnel, en UTC)
    ' -------------------------------------------------------------------------------------------------
    Public Property LastConnectionTestUtc As DateTime?

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : Host
    ' Type       : String
    ' Description : Adresse du serveur MariaDB (IP ou nom d'hôte)
    ' -------------------------------------------------------------------------------------------------
    Public Property Host As String

#End Region

#Region "Propriétés calculées"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : Port
    ' Version     : V1.1.0
    ' Date       :  16/05/2026

    ' Type       : Integer

    ' Description: Le port de connexion au serveur MariaDB
    '
    ' Rôle       :
    ' - Validation stricte afin d'empêcher toute configuration invalide, même en cas de modification manuelle du JSON.
    '
    ' Retour     :
    ' - Un entier compris entre 1 et 65535, sinon une exception est levée.
    '
    ' Remarques  :
    ' - La validation est effectuée dans le setter de la propriété.
    ' -------------------------------------------------------------------------------------------------

    Public Property Port As Integer
        Get
            Return _port
        End Get
        Set(value As Integer)
            If value < 1 OrElse value > 65535 Then
                Throw New ArgumentOutOfRangeException(NameOf(Port), "Le port doit être compris entre 1 et 65535.")
            End If
            _port = value
        End Set
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : IsConfigurationValid
    ' Version     : V1.0.0
    ' Date        : 21/04/2026
    '
    ' Type : Boolean

    ' Description: validation de la configuration minimale pour une connexion DB exploitable
    '
    ' Rôle       : Indique si la configuration minimale est complète et exploitable
    '
    ' Retour     :
    ' - True  : configuration valide
    ' - False : configuration incomplète

    ' Remarques  : Cette propriété ne vérifie que la présence des informations minimales nécessaires pour établir une connexion à la base de données.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property IsConfigurationValid As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(Host) AndAlso
               Port >= 1 AndAlso
               Port <= 65535 AndAlso
               Not String.IsNullOrWhiteSpace(DatabaseName) AndAlso
               Not String.IsNullOrWhiteSpace(UserName) AndAlso
               Not String.IsNullOrWhiteSpace(EncryptedPassword)
        End Get
    End Property

#End Region





End Class
