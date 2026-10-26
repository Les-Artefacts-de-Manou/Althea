' -------------------------------------------------------------------------------------------------
' Module      : UtilsCodePostal
' Projet      : Althéa
' Version     : V1.4.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Fournit une brique réutilisable de validation des codes postaux selon le pays de l'adresse.
'
' Pays validés (pour l'instant) :
' - Belgique   : 4 chiffres (1000 à 9999)
' - France     : 5 chiffres (01000 à 98999)
' - Luxembourg : 4 chiffres, préfixe « L- » ou « L » toléré
'
' Remarques   :
' - Format seulement : l'existence d'un code belge et sa localité sont contrôlées par
'   GestionCodesPostauxBe (référentiel ref_codes_postaux_be, v2.56.0), qui appelle Valider.
' - Tout autre pays (ou pays inconnu) : aucune validation (saisie libre).
' - Le pays transmis est le code ISO du référentiel ref_pays (un libellé reconnu reste accepté),
'   interprété par UtilsTelephone.DeduirePays (source unique des pays connus).
' - Module statique sans dépendance UI.
' - V1.2.0 (24/10/2026, recherche R4) : CleCodePostal et CleLocalite, formes de comparaison d'un
'   lieu (regroupement et recherche des patients par lieu ; serviront au contrôle code postal /
'   localité).
' - V1.3.0 (24/10/2026, R5) : CleLocalite reprend UtilsTexte.CleComparaison (même règle pour les
'   noms de la recherche du réseau).
' - V1.4.0 (24/10/2026, codes postaux belges) : CleLocalite lit aussi les points comme des espaces,
'   « œ » comme « oe », et confond « Saint / Sint / St » et « Sainte / Ste ».
' -------------------------------------------------------------------------------------------------
Option Strict On
Option Explicit On

Imports System.Text.RegularExpressions

Public Module UtilsCodePostal

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Valider
    ' Version  : V1.0.0
    ' Date     : 02/10/2026
    '
    ' Rôle     : Valide un code postal selon le pays de l'adresse.
    '
    ' Paramètres :
    ' - saisie        : Code postal saisi.
    ' - pays          : Code ISO du pays (ou libellé reconnu).
    ' - messageErreur : Message explicatif en cas d'anomalie.
    '
    ' Retour   :
    ' - Boolean : True si vide, pays non validé, ou code conforme ; False sinon.
    ' -------------------------------------------------------------------------------------------------
    Public Function Valider(saisie As String,
                            pays As String,
                            ByRef messageErreur As String) As Boolean

        messageErreur = String.Empty

        Dim brut As String = If(saisie, "").Trim()

        If brut.Length = 0 Then
            Return True
        End If

        Select Case UtilsTelephone.DeduirePays(pays)

            Case "BE"
                If Not Regex.IsMatch(brut, "^[1-9][0-9]{3}$") Then
                    messageErreur = "Le code postal belge doit comporter 4 chiffres (1000 à 9999)."
                    Return False
                End If

            Case "FR"
                If Not Regex.IsMatch(brut, "^[0-9]{5}$") OrElse
                   brut.StartsWith("00", StringComparison.Ordinal) OrElse
                   brut.StartsWith("99", StringComparison.Ordinal) Then
                    messageErreur = "Le code postal français doit comporter 5 chiffres (01000 à 98999)."
                    Return False
                End If

            Case "LU"
                If Not Regex.IsMatch(brut, "^(L-?)?[0-9]{4}$", RegexOptions.IgnoreCase) Then
                    messageErreur = "Le code postal luxembourgeois doit comporter 4 chiffres (ex. 1234 ou L-1234)."
                    Return False
                End If

        End Select

        Return True

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CleCodePostal
    ' Version  : V1.0.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : Forme de comparaison d'un code postal : sans espaces, en majuscules (« l-1234 » →
    '            « L-1234 ») ; chaîne vide si rien n'est saisi.
    ' -------------------------------------------------------------------------------------------------
    Public Function CleCodePostal(codePostal As String) As String
        Return If(codePostal, String.Empty).Replace(" ", String.Empty).Trim().ToUpperInvariant()
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CleLocalite
    ' Version  : V1.2.0
    ' Date     : 24/10/2026
    '
    ' Rôle     : Forme de comparaison d'une localité : sans accents ni majuscules, traits d'union,
    '            apostrophes et points lus comme des espaces, espaces multiples réduits, « œ » lu « oe »,
    '            « Saint », « Sint » et « St » confondus (de même « Sainte » et « Ste ») : « Grâce-Hollogne » et
    '            « grace hollogne », « St.-Truiden » et « Sint-Truiden » ont la même clé ; chaîne vide
    '            si rien n'est saisi.
    '
    ' Remarques :
    ' - Ne rapproche pas deux noms d'un même lieu (Bruxelles / Brussel) : la liste des codes postaux
    '   belges les accepte tous les deux (GestionCodesPostauxBe).
    ' - Même règle que le script qui produit cette liste (cle(), generer_codes_postaux_be.py) : les
    '   deux doivent rester d'accord.
    ' - V1.1.0 (24/10/2026, R5) : la règle est celle de UtilsTexte.CleComparaison (noms et lieux).
    ' - V1.2.0 (24/10/2026, codes postaux belges) : points, « œ », abréviations « St » / « Ste ».
    ' -------------------------------------------------------------------------------------------------
    Public Function CleLocalite(localite As String) As String

        Dim mots As String() = UtilsTexte.CleComparaison(If(localite, String.Empty).Replace("."c, " "c)).
            Replace("œ", "oe").Replace("æ", "ae").
            Split({" "c}, StringSplitOptions.RemoveEmptyEntries)

        For i As Integer = 0 To mots.Length - 1
            Select Case mots(i)
                Case "saint", "sint", "st" : mots(i) = "st"
                Case "sainte", "ste" : mots(i) = "ste"
            End Select
        Next

        Return String.Join(" ", mots)

    End Function

End Module
