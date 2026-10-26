' -------------------------------------------------------------------------------------------------
' Enum / Classes : NiveauEtat, LigneEtat, SectionEtat
' Projet         : Althéa
' Version        : V1.0.0
' Date           : 13/10/2026
' Auteur         : Joëlle (Manou) / Projet Althéa
'
' Rôle           :
' Modèles de l'état du système (GestionEtatSysteme → écran État du système).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

''' <summary>Appréciation d'une ligne de l'état du système.</summary>
Public Enum NiveauEtat
    ''' <summary>Simple information, sans appréciation.</summary>
    Information = 0
    Ok = 1
    ''' <summary>À vérifier, sans empêcher le fonctionnement.</summary>
    Avertissement = 2
    ''' <summary>Problème qui empêche une fonction de marcher.</summary>
    Probleme = 3
End Enum

Public Class LigneEtat
    Public Property Libelle As String
    Public Property Valeur As String
    Public Property Niveau As NiveauEtat
End Class

Public Class SectionEtat
    Public Property Titre As String
    Public Property Lignes As New List(Of LigneEtat)

    Public Sub Ajouter(libelle As String, valeur As String, Optional niveau As NiveauEtat = NiveauEtat.Information)
        Lignes.Add(New LigneEtat With {.Libelle = libelle, .Valeur = If(valeur, String.Empty), .Niveau = niveau})
    End Sub
End Class
