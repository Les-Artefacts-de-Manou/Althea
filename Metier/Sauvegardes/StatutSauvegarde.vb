' -------------------------------------------------------------------------------------------------
' Enum / Module : StatutSauvegarde / StatutsSauvegarde
' Projet        : Althéa
' Version       : V1.0.0
' Date          : 12/10/2026
' Auteur        : Joëlle (Manou) / Projet Althéa
'
' Rôle          :
' Statut d'une ligne du journal tec_sauvegardes et correspondance avec le code stocké
' (contrainte CHECK chk_tec_sauvegardes_statut, migration v2.38.0).
'
' Valeurs       :
' - EnCours : ligne créée au lancement ; reste EN_COURS si l'application s'est arrêtée pendant
'             la sauvegarde (affichée comme interrompue).
' - Reussie : archive complète, renommée à son nom définitif.
' - Echec   : sauvegarde abandonnée, archive provisoire supprimée.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Enum StatutSauvegarde
    EnCours = 0
    Reussie = 1
    Echec = 2
End Enum

Public Module StatutsSauvegarde

    Public Const CodeEnCours As String = "EN_COURS"
    Public Const CodeReussie As String = "REUSSIE"
    Public Const CodeEchec As String = "ECHEC"

    Public Function VersCode(statut As StatutSauvegarde) As String
        Select Case statut
            Case StatutSauvegarde.Reussie : Return CodeReussie
            Case StatutSauvegarde.Echec : Return CodeEchec
            Case Else : Return CodeEnCours
        End Select
    End Function

    Public Function DepuisCode(code As String) As StatutSauvegarde
        If String.Equals(code, CodeReussie, StringComparison.OrdinalIgnoreCase) Then Return StatutSauvegarde.Reussie
        If String.Equals(code, CodeEchec, StringComparison.OrdinalIgnoreCase) Then Return StatutSauvegarde.Echec
        Return StatutSauvegarde.EnCours
    End Function

    Public Function Libelle(statut As StatutSauvegarde) As String
        Select Case statut
            Case StatutSauvegarde.Reussie : Return "Réussie"
            Case StatutSauvegarde.Echec : Return "Échec"
            Case Else : Return "Interrompue"
        End Select
    End Function

End Module
