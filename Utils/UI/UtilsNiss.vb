' -------------------------------------------------------------------------------------------------
' Module      : UtilsNiss
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Fournit une brique réutilisable de validation des numéros d'identification de sécurité sociale
' selon le pays émetteur de l'identifiant (indépendant du pays de l'adresse).
'
' Types pris en charge :
' - Belgique   : NISS / numéro de registre national (11 chiffres, clé modulo 97, règle 2000+)
' - France     : NIR / numéro de sécurité sociale (13 caractères, Corse 2A/2B, + clé 97 facultative)
' - Luxembourg : matricule national (13 chiffres, contrôle Luhn + Verhoeff)
' - Autre      : aucune validation (saisie libre)
'
' Remarques   :
' - Les séparateurs usuels (espaces, points, tirets, barres) sont ignorés pour la validation.
' - Le type n'est pas stocké : il est déduit au chargement (DeduireType), Belgique par défaut ;
'   l'écran le transmet au métier (Patient.TypeIdentifiantNiss, audit C.2, décision de Joëlle).
' - Module statique sans dépendance UI.
' - V1.1.0 (23/10/2026, audit C.2, décision de Joëlle) : Normaliser (forme enregistrée : sans
'   séparateurs, majuscules — un même numéro tapé avec ou sans points n'est plus pris pour deux
'   numéros) et Formater (affichage lisible selon le type). France : 13 caractères acceptés sans la
'   clé (vérifiée quand elle est donnée), relevé de Joëlle.
' -------------------------------------------------------------------------------------------------
Option Strict On
Option Explicit On

Imports System.Linq
Imports System.Text.RegularExpressions

Public Module UtilsNiss

    Public Const TYPE_BE As String = "Belgique"
    Public Const TYPE_FR As String = "France"
    Public Const TYPE_LU As String = "Luxembourg"
    Public Const TYPE_AUTRE As String = "Autre"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : TypesIdentifiant
    ' Rôle     : Liste des types d'identifiant proposés (Belgique en tête, « Autre » en dernier).
    ' -------------------------------------------------------------------------------------------------
    Public Function TypesIdentifiant() As String()

        Return New String() {TYPE_BE, TYPE_FR, TYPE_LU, TYPE_AUTRE}

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : DeduireType
    ' Version  : V1.1.0
    ' Date     : 23/10/2026
    '
    ' Rôle     : Déduit le type d'identifiant d'un numéro existant (pour le chargement d'une fiche).
    '
    ' Retour   :
    ' - String : Type valide reconnu ; Belgique si vide ; « Autre » si aucun format ne correspond.
    '
    ' Remarques :
    ' - V1.1.0 (23/10/2026) : le numéro français sans clé (13 caractères) est essayé après le
    '   matricule luxembourgeois, de même longueur mais dont les chiffres de contrôle sont vérifiés.
    ' -------------------------------------------------------------------------------------------------
    Public Function DeduireType(saisie As String) As String

        If Nettoyer(saisie).Length = 0 Then
            Return TYPE_BE
        End If

        ' Ordre : les formats dont la clé est vérifiée d'abord. Un numéro français sans sa clé
        ' (13 caractères) a la longueur d'un matricule luxembourgeois : le Luxembourg (deux chiffres de
        ' contrôle) passe avant lui.
        Dim valeur As String = Nettoyer(saisie)
        Dim message As String = String.Empty

        If Valider(valeur, TYPE_BE, message) Then Return TYPE_BE
        If valeur.Length = 15 AndAlso Valider(valeur, TYPE_FR, message) Then Return TYPE_FR
        If Valider(valeur, TYPE_LU, message) Then Return TYPE_LU
        If Valider(valeur, TYPE_FR, message) Then Return TYPE_FR

        Return TYPE_AUTRE

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Valider
    ' Version  : V1.1.0
    ' Date     : 23/10/2026
    '
    ' Rôle     : Valide un identifiant selon son type (format + clé de contrôle).
    '
    ' Paramètres :
    ' - saisie        : Identifiant saisi.
    ' - typeId        : Type d'identifiant (TYPE_BE, TYPE_FR, TYPE_LU, TYPE_AUTRE).
    ' - messageErreur : Message explicatif en cas d'anomalie.
    '
    ' Retour   :
    ' - Boolean : True si vide, type « Autre » ou identifiant conforme ; False sinon.
    ' -------------------------------------------------------------------------------------------------
    Public Function Valider(saisie As String,
                            typeId As String,
                            ByRef messageErreur As String) As Boolean

        messageErreur = String.Empty

        Dim valeur As String = Nettoyer(saisie)

        If valeur.Length = 0 Then
            Return True
        End If

        Select Case typeId

            Case TYPE_BE
                If Not Regex.IsMatch(valeur, "^[0-9]{11}$") Then
                    messageErreur = "Le NISS belge doit comporter 11 chiffres (ex. 85.07.30-033.28)."
                    Return False
                End If
                If Not CleBelgeValide(valeur) Then
                    messageErreur = "Le NISS belge est invalide (clé de contrôle incorrecte)."
                    Return False
                End If

            Case TYPE_FR
                ' 13 caractères, suivis ou non de la clé (2 chiffres) : beaucoup de documents (attestation,
                ' relevé de retraite) donnent le numéro et la clé séparément (relevé de Joëlle, 23/10/2026).
                If Not Regex.IsMatch(valeur, "^[1-478][0-9]{4}(2A|2B|[0-9]{2})[0-9]{6}([0-9]{2})?$") Then
                    messageErreur = "Le numéro de sécurité sociale français comporte 13 caractères, suivis ou non de la clé (2 chiffres)."
                    Return False
                End If
                If valeur.Length = 15 AndAlso Not CleFrancaiseValide(valeur) Then
                    messageErreur = "Le numéro de sécurité sociale français est invalide (clé de contrôle incorrecte)."
                    Return False
                End If

            Case TYPE_LU
                If Not Regex.IsMatch(valeur, "^[0-9]{13}$") Then
                    messageErreur = "Le matricule luxembourgeois doit comporter 13 chiffres (AAAAMMJJXXXCC)."
                    Return False
                End If
                If Not ClesLuxembourgeoisesValides(valeur) Then
                    messageErreur = "Le matricule luxembourgeois est invalide (chiffres de contrôle incorrects)."
                    Return False
                End If

        End Select

        Return True

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Normaliser
    ' Version  : V1.0.0
    ' Date     : 23/10/2026
    '
    ' Rôle     : Forme enregistrée d'un identifiant : séparateurs usuels retirés, majuscules (Corse
    '            2A/2B) ; Nothing si vide. Une seule forme en base, pour que l'unicité (index
    '            uq_patients_niss) et la recherche jouent quel que soit la façon de le taper.
    ' -------------------------------------------------------------------------------------------------
    Public Function Normaliser(saisie As String) As String

        Dim valeur As String = Nettoyer(saisie)
        Return If(valeur.Length = 0, Nothing, valeur)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Formater
    ' Version  : V1.0.0
    ' Date     : 23/10/2026
    '
    ' Rôle     : Affichage lisible d'un identifiant valide selon son type :
    '            Belgique « 85.07.30-033.28 », France « 1 85 07 2A 033 033 28 »,
    '            Luxembourg « 1985 07 30 033 28 ». Un identifiant invalide ou de type « Autre » est
    '            rendu tel qu'il a été saisi (rien n'est perdu à l'affichage).
    ' -------------------------------------------------------------------------------------------------
    Public Function Formater(saisie As String, typeId As String) As String

        Dim valeur As String = Nettoyer(saisie)
        Dim message As String = String.Empty

        If valeur.Length = 0 OrElse Not Valider(valeur, typeId, message) Then
            Return If(saisie, String.Empty).Trim()
        End If

        Select Case typeId
            Case TYPE_BE
                Return $"{valeur.Substring(0, 2)}.{valeur.Substring(2, 2)}.{valeur.Substring(4, 2)}-{valeur.Substring(6, 3)}.{valeur.Substring(9, 2)}"
            Case TYPE_FR
                Dim sansCle As String = $"{valeur.Substring(0, 1)} {valeur.Substring(1, 2)} {valeur.Substring(3, 2)} {valeur.Substring(5, 2)} {valeur.Substring(7, 3)} {valeur.Substring(10, 3)}"
                Return If(valeur.Length = 15, sansCle & " " & valeur.Substring(13, 2), sansCle)
            Case TYPE_LU
                Return $"{valeur.Substring(0, 4)} {valeur.Substring(4, 2)} {valeur.Substring(6, 2)} {valeur.Substring(8, 3)} {valeur.Substring(11, 2)}"
            Case Else
                Return If(saisie, String.Empty).Trim()
        End Select

    End Function

#Region "Outils internes"

    ' Retire les séparateurs usuels et met en majuscules (Corse 2A/2B).
    Private Function Nettoyer(saisie As String) As String

        Return Regex.Replace(If(saisie, ""), "[\s\.\-/]", "").ToUpperInvariant()

    End Function

    ' NISS belge : clé = 97 - (9 premiers chiffres mod 97) ; pour les naissances 2000+, préfixe « 2 ».
    Private Function CleBelgeValide(valeur As String) As Boolean

        Dim base As Long = Long.Parse(valeur.Substring(0, 9))
        Dim cle As Integer = Integer.Parse(valeur.Substring(9, 2))

        If 97 - CInt(base Mod 97) = cle Then
            Return True
        End If

        Return 97 - CInt((2000000000L + base) Mod 97) = cle

    End Function

    ' NIR français : clé = 97 - (13 premiers caractères mod 97) ; Corse 2A → 19, 2B → 18.
    Private Function CleFrancaiseValide(valeur As String) As Boolean

        Dim corps As String = valeur.Substring(0, 13)
        Dim cle As Integer = Integer.Parse(valeur.Substring(13, 2))
        Dim nombre As Long

        If corps.Substring(5, 2) = "2A" Then
            nombre = Long.Parse(corps.Substring(0, 5) & "19" & corps.Substring(7))
        ElseIf corps.Substring(5, 2) = "2B" Then
            nombre = Long.Parse(corps.Substring(0, 5) & "18" & corps.Substring(7))
        Else
            nombre = Long.Parse(corps)
        End If

        Return 97 - CInt(nombre Mod 97) = cle

    End Function

    ' Matricule luxembourgeois : 12e chiffre = Luhn, 13e chiffre = Verhoeff (sur les 11 premiers).
    Private Function ClesLuxembourgeoisesValides(valeur As String) As Boolean

        Dim base As String = valeur.Substring(0, 11)

        Return CalculerLuhn(base) = CInt(Char.GetNumericValue(valeur(11))) AndAlso
               CalculerVerhoeff(base) = CInt(Char.GetNumericValue(valeur(12)))

    End Function

    Private Function CalculerLuhn(chiffres As String) As Integer

        Dim somme As Integer = 0
        Dim doubler As Boolean = True

        For i As Integer = chiffres.Length - 1 To 0 Step -1
            Dim d As Integer = CInt(Char.GetNumericValue(chiffres(i)))
            If doubler Then
                d *= 2
                If d > 9 Then d -= 9
            End If
            somme += d
            doubler = Not doubler
        Next

        Return (10 - (somme Mod 10)) Mod 10

    End Function

    Private ReadOnly _verhoeffD As Integer(,) = {
        {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}, {1, 2, 3, 4, 0, 6, 7, 8, 9, 5},
        {2, 3, 4, 0, 1, 7, 8, 9, 5, 6}, {3, 4, 0, 1, 2, 8, 9, 5, 6, 7},
        {4, 0, 1, 2, 3, 9, 5, 6, 7, 8}, {5, 9, 8, 7, 6, 0, 4, 3, 2, 1},
        {6, 5, 9, 8, 7, 1, 0, 4, 3, 2}, {7, 6, 5, 9, 8, 2, 1, 0, 4, 3},
        {8, 7, 6, 5, 9, 3, 2, 1, 0, 4}, {9, 8, 7, 6, 5, 4, 3, 2, 1, 0}}

    Private ReadOnly _verhoeffP As Integer(,) = {
        {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}, {1, 5, 7, 6, 2, 8, 3, 0, 9, 4},
        {5, 8, 0, 3, 7, 9, 6, 1, 4, 2}, {8, 9, 1, 6, 0, 4, 3, 5, 2, 7},
        {9, 4, 5, 3, 1, 2, 6, 8, 7, 0}, {4, 2, 8, 6, 5, 7, 3, 9, 0, 1},
        {2, 7, 9, 3, 8, 0, 6, 4, 1, 5}, {7, 0, 4, 6, 9, 1, 3, 2, 5, 8}}

    Private ReadOnly _verhoeffInv As Integer() = {0, 4, 3, 2, 1, 5, 6, 7, 8, 9}

    Private Function CalculerVerhoeff(chiffres As String) As Integer

        Dim c As Integer = 0
        Dim inverses As Char() = chiffres.Reverse().ToArray()

        For i As Integer = 0 To inverses.Length - 1
            Dim d As Integer = CInt(Char.GetNumericValue(inverses(i)))
            c = _verhoeffD(c, _verhoeffP((i + 1) Mod 8, d))
        Next

        Return _verhoeffInv(c)

    End Function

#End Region

End Module
