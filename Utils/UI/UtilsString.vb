' -------------------------------------------------------------------------------------------------
' Module      : UtilsString
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 02/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Fonctions utilitaires de normalisation de chaînes de caractères.
'
' Responsabilités :
' - Supprimer les signes diacritiques (accents, cédilles, etc.) - RemoveDiacritics
' - Normaliser les codes techniques (MAJUSCULES, sans accents, espaces remplacés par _) - NormalizeTechnicalCode
'
' Remarques   :
' - Module statique (fonctions partagées)
' - Utilise System.Globalization pour la normalisation Unicode
'
' Dépendances :
' - Utilisé pour générer les codes techniques (CodeParametre, CodeUtilisateur, etc.)

' Imports :
' - System.Globalization (pour UnicodeCategory)
' - System.Text (pour StringBuilder)
' -------------------------------------------------------------------------------------------------

Imports System.Globalization
Imports System.Text

Public Module UtilsString

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : RemoveDiacritics (Public)
    ' Version    : V1.0.0
    ' Date       : 02/05/2026
    '
    ' Rôle       :
    ' Supprime les signes diacritiques (accents, cédilles, etc.) d'une chaîne.
    '
    ' Paramètres :
    ' - value : Chaîne source
    '
    ' Retour     :
    ' - String : Chaîne sans accents (ex: "é" → "e", "ç" → "c")
    '
    ' Remarques  :
    ' - Utilise la normalisation Unicode FormD puis FormC
    ' - Retourne String.Empty si value est Nothing ou vide
    ' -------------------------------------------------------------------------------------------------
    Public Function RemoveDiacritics(value As String) As String

        If String.IsNullOrWhiteSpace(value) Then
            Return String.Empty
        End If

        Dim normalized As String = value.Normalize(NormalizationForm.FormD)
        Dim sb As New StringBuilder()

        For Each c As Char In normalized
            If CharUnicodeInfo.GetUnicodeCategory(c) <> UnicodeCategory.NonSpacingMark Then
                sb.Append(c)
            End If
        Next

        Return sb.ToString().Normalize(NormalizationForm.FormC)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : NormalizeTechnicalCode (Public)
    ' Version    : V1.0.0
    ' Date       : 02/05/2026
    '
    ' Rôle       :
    ' Normalise une chaîne pour générer un code technique valide.
    '
    ' Paramètres :
    ' - value : Chaîne source
    '
    ' Retour     :
    ' - String : Code technique normalisé (MAJUSCULES, sans accents, espaces remplacés par _)
    '
    ' Exemple    :
    ' - "Joëlle Paramètre" → "JOELLE_PARAMETRE"
    ' - "Mot de passe" → "MOT_DE_PASSE"
    '
    ' Remarques  :
    ' - Utilise RemoveDiacritics puis Trim, ToUpperInvariant, Replace(" ", "_")
    ' - Retourne String.Empty si value est Nothing ou vide
    ' -------------------------------------------------------------------------------------------------
    Public Function NormalizeTechnicalCode(value As String) As String

        If String.IsNullOrWhiteSpace(value) Then
            Return String.Empty
        End If

        Dim result As String = RemoveDiacritics(value)

        result = result.Trim()
        result = result.ToUpperInvariant()
        result = result.Replace(" ", "_")

        Return result

    End Function

End Module
