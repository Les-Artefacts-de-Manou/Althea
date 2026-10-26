' -------------------------------------------------------------------------------------------------
' Enum / Module : StatutRestauration / StatutsRestauration
' Projet        : Althéa
' Version       : V1.0.0
' Date          : 13/10/2026
' Auteur        : Joëlle (Manou) / Projet Althéa
'
' Rôle          :
' Statut d'une ligne du journal tec_restaurations et correspondance avec le code stocké
' (contrainte CHECK chk_tec_restaurations_statut, migration v2.39.0).
'
' Valeurs       :
' - Reussie : base et dossier de données remplacés par ceux de l'archive (ligne écrite dans la
'             base restaurée).
' - Echec   : restauration arrêtée ; le message dit si la base actuelle a été conservée ou
'             remise dans son état d'avant.
'
' Remarques     :
' - Pas de statut « en cours » : une ligne écrite au début serait effacée par le rechargement.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Enum StatutRestauration
    Reussie = 1
    Echec = 2
End Enum

Public Module StatutsRestauration

    Public Const CodeReussie As String = "REUSSIE"
    Public Const CodeEchec As String = "ECHEC"

    Public Function VersCode(statut As StatutRestauration) As String
        Return If(statut = StatutRestauration.Reussie, CodeReussie, CodeEchec)
    End Function

    Public Function DepuisCode(code As String) As StatutRestauration
        Return If(String.Equals(code, CodeReussie, StringComparison.OrdinalIgnoreCase),
                  StatutRestauration.Reussie, StatutRestauration.Echec)
    End Function

    Public Function Libelle(statut As StatutRestauration) As String
        Return If(statut = StatutRestauration.Reussie, "Réussie", "Échec")
    End Function

End Module
