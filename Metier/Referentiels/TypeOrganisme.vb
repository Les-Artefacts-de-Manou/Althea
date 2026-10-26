' -------------------------------------------------------------------------------------------------
' Fichier  : TypeOrganisme
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 11/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Type d'un organisme du référentiel ref_organismes (colonne type_organisme) et conversions
' entre l'énumération, le code stocké en base et le libellé affiché.
'
' Remarques :
' - Le type pilote un comportement de l'application (organismes proposés aux domaines ou aux
'   patients) : c'est pourquoi il s'agit d'une énumération et non d'un référentiel paramétrable.
' - Les codes doivent rester alignés sur la contrainte chk_ref_organismes_type (migration v2.34.0).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

' -------------------------------------------------------------------------------------------------
' Énumération : TypeOrganisme
' Rôle        : Nature d'un organisme.
' - Remboursement : organisme de remboursement rattaché aux domaines (ex. INAMI).
' - Mutualite     : organisme assureur (mutualité) rattaché aux patients.
' -------------------------------------------------------------------------------------------------
Public Enum TypeOrganisme
    Remboursement = 0
    Mutualite = 1
End Enum

Public Module TypesOrganisme

    ' Codes stockés dans ref_organismes.type_organisme
    Public Const CodeRemboursement As String = "REMBOURSEMENT"
    Public Const CodeMutualite As String = "MUTUALITE"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : VersCode
    ' Rôle     : Retourne le code base de données correspondant au type.
    ' -------------------------------------------------------------------------------------------------
    Public Function VersCode(type As TypeOrganisme) As String
        Return If(type = TypeOrganisme.Mutualite, CodeMutualite, CodeRemboursement)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : DepuisCode
    ' Rôle     : Convertit le code lu en base en type.
    ' Remarque : La contrainte CHECK garantit un code connu ; par prudence, un code inattendu est
    '            traité comme Remboursement (comportement historique de la table).
    ' -------------------------------------------------------------------------------------------------
    Public Function DepuisCode(code As String) As TypeOrganisme
        Return If(String.Equals(code, CodeMutualite, StringComparison.OrdinalIgnoreCase),
                  TypeOrganisme.Mutualite, TypeOrganisme.Remboursement)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LibelleType
    ' Rôle     : Libellé affiché du type (listes, grille).
    ' -------------------------------------------------------------------------------------------------
    Public Function LibelleType(type As TypeOrganisme) As String
        Return If(type = TypeOrganisme.Mutualite, "Mutualité", "Remboursement")
    End Function

End Module
