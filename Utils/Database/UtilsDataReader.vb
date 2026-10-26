' -------------------------------------------------------------------------------------------------
' Module   : UtilsDataReader
' Projet   : Althéa
' Version  : V1.0.1
' Date     : 25/10/2026
'
' Rôle :
' Fonctions utilitaires de lecture tolérante d'un DbDataReader (colonnes optionnelles).
'
' Remarques :
' - Permet de partager un même mapping entre plusieurs requêtes dont certaines
'   ne sélectionnent pas une colonne optionnelle (ex. est_systeme).
' - V1.0.1 (25/10/2026, audit D-1) : Option Strict On.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Data.Common

Public Module UtilsDataReader

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LireBooleenOptionnel
    ' Rôle     : Retourne la valeur booléenne d'une colonne si elle est présente et non NULL,
    '            sinon la valeur par défaut.
    ' -------------------------------------------------------------------------------------------------
    Public Function LireBooleenOptionnel(reader As DbDataReader, nomColonne As String,
                                         Optional valeurParDefaut As Boolean = False) As Boolean

        For i As Integer = 0 To reader.FieldCount - 1
            If String.Equals(reader.GetName(i), nomColonne, StringComparison.OrdinalIgnoreCase) Then
                If reader.IsDBNull(i) Then Return valeurParDefaut
                Return Convert.ToBoolean(reader.GetValue(i))
            End If
        Next

        Return valeurParDefaut

    End Function

End Module
