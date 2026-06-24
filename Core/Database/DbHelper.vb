' -------------------------------------------------------------------------------------------------
' Module      : DbHelper
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 21/07/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Fonctions utilitaires partagées d'accès aux données :
' - Lecture typée d'une colonne de MySqlDataReader avec gestion des valeurs NULL
' - Conversion de valeurs métier vers des paramètres SQL (DBNull si absent/vide)
'
' Responsabilités :
' - Centraliser les helpers Lirexxx / Valeurxxx auparavant dupliqués dans chaque module Gestion*
' - Garantir un comportement homogène face aux NULL sur toute la couche métier
'
' Important   :
' - Aucun accès UI direct
' - Module global (namespace racine) : fonctions appelables sans qualification
'
' Dépendances :
' - MySqlConnector (MySqlDataReader)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module DbHelper

#Region "Lecture (DataReader vers valeur métier)"

    ' Retourne la valeur d'une colonne texte, ou Nothing si NULL
    Public Function LireString(reader As MySqlDataReader, colonne As String) As String

        If reader(colonne) Is DBNull.Value Then
            Return Nothing
        End If

        Return reader(colonne).ToString()

    End Function

    ' Retourne la valeur d'une colonne entière (NULL traité comme 0)
    Public Function LireInt(reader As MySqlDataReader, colonne As String) As Integer

        If reader(colonne) Is DBNull.Value Then
            Return 0
        End If

        Return Convert.ToInt32(reader(colonne))

    End Function

    ' Retourne la valeur d'une colonne booléenne (tinyint(1)), NULL traité comme False
    Public Function LireBool(reader As MySqlDataReader, colonne As String) As Boolean

        If reader(colonne) Is DBNull.Value Then
            Return False
        End If

        Return Convert.ToBoolean(reader(colonne))

    End Function

    ' Retourne la valeur d'une colonne date, ou Nothing si NULL
    Public Function LireDateNullable(reader As MySqlDataReader, colonne As String) As Date?

        If reader(colonne) Is DBNull.Value Then
            Return Nothing
        End If

        Return CDate(reader(colonne))

    End Function

    ' Retourne la valeur d'une colonne entière longue, ou Nothing si NULL
    Public Function LireLongNullable(reader As MySqlDataReader, colonne As String) As Long?

        If reader(colonne) Is DBNull.Value Then
            Return Nothing
        End If

        Return Convert.ToInt64(reader(colonne))

    End Function

    ' Retourne la valeur d'une colonne entière non signée longue, ou Nothing si NULL
    Public Function LireULongNullable(reader As MySqlDataReader, colonne As String) As ULong?

        If reader(colonne) Is DBNull.Value Then
            Return Nothing
        End If

        Return Convert.ToUInt64(reader(colonne))

    End Function

#End Region

#Region "Écriture (valeur métier vers paramètre SQL)"

    ' Convertit une chaîne en valeur SQL : NULL si vide/blanc, sinon la chaîne nettoyée
    Public Function ValeurOuDBNull(valeur As String) As Object

        If String.IsNullOrWhiteSpace(valeur) Then
            Return DBNull.Value
        End If

        Return valeur.Trim()

    End Function

    ' Convertit une date nullable en valeur SQL : NULL si absente, sinon la date
    Public Function ValeurDateOuDBNull(valeur As Date?) As Object

        If Not valeur.HasValue Then
            Return DBNull.Value
        End If

        Return valeur.Value

    End Function

    ' Convertit un entier long nullable en valeur SQL : NULL si absent, sinon la valeur
    Public Function ValeurLongOuDBNull(valeur As Long?) As Object

        If Not valeur.HasValue Then
            Return DBNull.Value
        End If

        Return valeur.Value

    End Function

    ' Convertit un identifiant non signé nullable en valeur SQL : NULL si absent, sinon la valeur
    Public Function ValeurULongOuDBNull(valeur As ULong?) As Object

        If Not valeur.HasValue Then
            Return DBNull.Value
        End If

        Return valeur.Value

    End Function

#End Region

End Module
