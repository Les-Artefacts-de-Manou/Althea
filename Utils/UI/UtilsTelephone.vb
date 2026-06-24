' -------------------------------------------------------------------------------------------------
' Module      : UtilsTelephone
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 13/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Fournit une brique réutilisable de gestion des numéros de téléphone : déduction du pays à partir
' d'un libellé (champ « Pays »), normalisation au format international canonique E.164, formatage
' lisible avec espacement, et validation par pays (nombre de chiffres attendu).
'
' Responsabilités :
' - Déduire le pays d'un numéro à partir du libellé du champ Pays (DeduirePays)
' - Normaliser un numéro saisi en format canonique E.164 pour le stockage (NormaliserE164)
' - Formater un numéro pour un affichage lisible et homogène (FormaterAffichage)
' - Valider la plausibilité d'un numéro selon les règles du pays (Valider)
'
' Pays pris en charge (cabinet en Belgique + pays limitrophes) :
' - Belgique (+32), France (+33), Luxembourg (+352), Allemagne (+49), Pays-Bas (+31)
'
' Remarques   :
' - Module statique (fonctions partagées), aucune dépendance UI : réutilisable dans tout écran.
' - Stockage canonique recommandé : E.164 (ex. « +32475123456 »).
' - Affichage international lisible (ex. « +32 475 12 34 56 »).
' - Si le pays est inconnu, repli souple : le numéro n'est pas dénaturé et la validation délègue
'   à UtilsValidation.IsValidTelephone (validation souple historique).
' - L'indicatif présent dans la saisie (+33…, 0033…) est prioritaire sur la déduction du champ Pays.
'
' Dépendances :
' - UtilsValidation (repli souple quand le pays est inconnu)
'
' Imports :
' - System.Globalization
' - System.Linq
' - System.Text
' -------------------------------------------------------------------------------------------------
Option Strict On
Option Explicit On

Imports System.Globalization
Imports System.Linq
Imports System.Text

Public Module UtilsTelephone

#Region "Type interne : règle de pays"

    ' -------------------------------------------------------------------------------------------------
    ' Classe    : ReglePays
    ' Rôle      : Décrit les règles de normalisation, de formatage et de validation d'un pays.
    '
    ' Propriétés :
    ' - Code             : Code court interne (« BE », « FR », « LU », « DE », « NL »)
    ' - Libelle          : Libellé humain pour les messages (« la Belgique », « la France »…)
    ' - LibelleCanonique : Libellé de saisie normalisé (« Belgique », « France »…) pour la liste déroulante
    ' - Indicatif        : Indicatif international sans « + » (« 32 », « 33 », « 352 »…)
    ' - MinNat / MaxNat  : Nombre de chiffres nationaux attendus (hors indicatif et trunk « 0 »)
    ' - UtiliseTrunkZero : True si le numéro national s'écrit avec un « 0 » initial à retirer
    ' - Variantes        : Libellés acceptés (majuscules, sans accents) pour déduire le pays
    ' -------------------------------------------------------------------------------------------------
    Private NotInheritable Class ReglePays

        Public ReadOnly Property Code As String
        Public ReadOnly Property Libelle As String
        Public ReadOnly Property LibelleCanonique As String
        Public ReadOnly Property Indicatif As String
        Public ReadOnly Property MinNat As Integer
        Public ReadOnly Property MaxNat As Integer
        Public ReadOnly Property UtiliseTrunkZero As Boolean
        Public ReadOnly Property Variantes As String()

        Public Sub New(code As String,
                       libelle As String,
                       libelleCanonique As String,
                       indicatif As String,
                       minNat As Integer,
                       maxNat As Integer,
                       utiliseTrunkZero As Boolean,
                       variantes As String())

            Me.Code = code
            Me.Libelle = libelle
            Me.LibelleCanonique = libelleCanonique
            Me.Indicatif = indicatif
            Me.MinNat = minNat
            Me.MaxNat = maxNat
            Me.UtiliseTrunkZero = utiliseTrunkZero
            Me.Variantes = variantes

        End Sub

    End Class

#End Region

#Region "Table des pays pris en charge"

    ' Règles indexées par code court interne.
    Private ReadOnly _regles As Dictionary(Of String, ReglePays) = ConstruireRegles()

    ' Règles triées par longueur d'indicatif décroissante (pour reconnaître « 352 » avant « 32 »).
    Private ReadOnly _reglesParIndicatif As List(Of ReglePays) =
        _regles.Values.OrderByDescending(Function(r) r.Indicatif.Length).ToList()

    ' Ordre d'affichage des pays dans la liste déroulante (cabinet en Belgique → Belgique en tête).
    Private ReadOnly _ordreAffichage As String() = {"BE", "FR", "LU", "DE", "NL"}

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ConstruireRegles
    ' Rôle     : Construit la table des règles des pays pris en charge.
    '
    ' Remarques :
    ' - Fourchettes de chiffres nationaux (hors indicatif, hors « 0 » de trunk) :
    '   BE 8-9 ; FR 9 ; LU 6-9 ; DE 6-11 ; NL 9.
    ' - Le Luxembourg n'utilise pas de « 0 » de trunk (UtiliseTrunkZero = False).
    ' - Variantes en MAJUSCULES et sans accents (comparées après normalisation).
    ' -------------------------------------------------------------------------------------------------
    Private Function ConstruireRegles() As Dictionary(Of String, ReglePays)

        Dim regles As New Dictionary(Of String, ReglePays)(StringComparer.Ordinal)

        regles.Add("BE", New ReglePays(
            "BE", "la Belgique", "Belgique", "32", 8, 9, True,
            New String() {"BELGIQUE", "BELGIE", "BELGIUM", "BE"}))

        regles.Add("FR", New ReglePays(
            "FR", "la France", "France", "33", 9, 9, True,
            New String() {"FRANCE", "FR"}))

        regles.Add("LU", New ReglePays(
            "LU", "le Luxembourg", "Luxembourg", "352", 6, 9, False,
            New String() {"LUXEMBOURG", "LUX", "LU"}))

        regles.Add("DE", New ReglePays(
            "DE", "l'Allemagne", "Allemagne", "49", 6, 11, True,
            New String() {"ALLEMAGNE", "DEUTSCHLAND", "GERMANY", "DE"}))

        regles.Add("NL", New ReglePays(
            "NL", "les Pays-Bas", "Pays-Bas", "31", 9, 9, True,
            New String() {"PAYS-BAS", "PAYS BAS", "PAYSBAS", "NEDERLAND", "NETHERLANDS", "HOLLANDE", "NL"}))

        Return regles

    End Function

#End Region

#Region "Déduction du pays"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : DeduirePays
    ' Version  : V1.0.0
    ' Date     : 13/06/2026
    '
    ' Rôle     : Déduit le code pays interne à partir du libellé du champ « Pays ».
    '
    ' Paramètres :
    ' - libellePays : Texte saisi dans le champ Pays (ex. « Belgique », « France »…)
    '
    ' Retour   :
    ' - String : Code pays interne (« BE », « FR », « LU », « DE », « NL ») ou Nothing si inconnu.
    '
    ' Remarques :
    ' - Comparaison tolérante : insensible à la casse et aux accents, gère quelques variantes courantes.
    ' -------------------------------------------------------------------------------------------------
    Public Function DeduirePays(libellePays As String) As String

        Dim regle As ReglePays = TrouverRegleParLibelle(libellePays)
        Return regle?.Code

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LibellesPays
    ' Version  : V1.0.0
    ' Date     : 13/06/2026
    '
    ' Rôle     : Retourne la liste des libellés de pays pris en charge, dans l'ordre d'affichage,
    '            pour alimenter une liste déroulante de saisie (garantit une valeur normalisée).
    '
    ' Retour   :
    ' - List(Of String) : Libellés canoniques (« Belgique », « France », « Luxembourg »,
    '                     « Allemagne », « Pays-Bas »), Belgique en tête.
    ' -------------------------------------------------------------------------------------------------
    Public Function LibellesPays() As List(Of String)

        Return _ordreAffichage.Select(Function(code) _regles(code).LibelleCanonique).ToList()

    End Function

    ' Libellé canonique du pays proposé par défaut (cabinet situé en Belgique).
    Public ReadOnly Property LibellePaysParDefaut As String
        Get
            Return _regles("BE").LibelleCanonique
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : NormaliserLibellePays
    ' Version  : V1.0.0
    ' Date     : 13/06/2026
    '
    ' Rôle     : Convertit un libellé de pays éventuellement ancien ou localisé en libellé canonique.
    '            (ex. « Belgium » / « België » / « BE » → « Belgique »).
    '
    ' Paramètres :
    ' - libellePays : Libellé saisi ou stocké (variantes acceptées).
    '
    ' Retour   :
    ' - String : Libellé canonique (« Belgique », « France »…) ou Nothing si le pays est inconnu.
    ' -------------------------------------------------------------------------------------------------
    Public Function NormaliserLibellePays(libellePays As String) As String

        Dim regle As ReglePays = TrouverRegleParLibelle(libellePays)
        Return regle?.LibelleCanonique

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : TrouverRegleParLibelle
    ' Rôle     : Recherche la règle d'un pays à partir d'un libellé (normalisé majuscules sans accents).
    ' -------------------------------------------------------------------------------------------------
    Private Function TrouverRegleParLibelle(libellePays As String) As ReglePays

        Dim cle As String = SupprimerAccents(If(libellePays, "").Trim().ToUpperInvariant())

        If cle.Length = 0 Then
            Return Nothing
        End If

        For Each regle As ReglePays In _regles.Values
            If regle.Variantes.Contains(cle) Then
                Return regle
            End If
        Next

        Return Nothing

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : TrouverRegleParIndicatif
    ' Rôle     : Recherche la règle d'un pays à partir des chiffres internationaux (sans « + »).
    '
    ' Remarques :
    ' - Les indicatifs sont testés du plus long au plus court pour lever toute ambiguïté (« 352 »).
    ' -------------------------------------------------------------------------------------------------
    Private Function TrouverRegleParIndicatif(chiffres As String) As ReglePays

        If String.IsNullOrEmpty(chiffres) Then
            Return Nothing
        End If

        For Each regle As ReglePays In _reglesParIndicatif
            If chiffres.StartsWith(regle.Indicatif, StringComparison.Ordinal) Then
                Return regle
            End If
        Next

        Return Nothing

    End Function

#End Region

#Region "Normalisation E.164"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : NormaliserE164
    ' Version  : V1.0.0
    ' Date     : 13/06/2026
    '
    ' Rôle     : Normalise un numéro saisi vers le format international canonique E.164 (« +CCNSN »).
    '
    ' Paramètres :
    ' - saisie      : Numéro saisi par l'utilisateur (formats variés autorisés).
    ' - libellePays : Libellé du champ Pays, utilisé si aucun indicatif n'est présent dans la saisie.
    '
    ' Retour   :
    ' - String : Numéro au format E.164 (« +32475123456 »), ou la saisie nettoyée si le pays
    '            ne peut être déterminé (repli souple).
    '
    ' Remarques :
    ' - Un indicatif présent dans la saisie (« + » ou « 00 ») est prioritaire sur le champ Pays.
    ' - Le « 0 » de trunk national est retiré pour les pays concernés (BE, FR, DE, NL).
    ' -------------------------------------------------------------------------------------------------
    Public Function NormaliserE164(saisie As String, libellePays As String) As String

        Dim brut As String = If(saisie, "").Trim()

        If brut.Length = 0 Then
            Return String.Empty
        End If

        Dim commenceParPlus As Boolean = brut.StartsWith("+", StringComparison.Ordinal)
        Dim chiffres As String = New String(brut.Where(AddressOf Char.IsDigit).ToArray())

        If chiffres.Length = 0 Then
            Return brut
        End If

        ' Détection d'un indicatif international explicite (« + » ou « 00 » en tête).
        Dim aIndicatifInternational As Boolean = commenceParPlus

        If Not aIndicatifInternational AndAlso chiffres.StartsWith("00", StringComparison.Ordinal) Then
            aIndicatifInternational = True
            chiffres = chiffres.Substring(2)
        End If

        If aIndicatifInternational Then

            Dim regleInternationale As ReglePays = TrouverRegleParIndicatif(chiffres)

            If regleInternationale IsNot Nothing Then
                Dim nationalI As String = chiffres.Substring(regleInternationale.Indicatif.Length)
                nationalI = RetirerTrunkZero(regleInternationale, nationalI)
                Return "+" & regleInternationale.Indicatif & nationalI
            End If

            ' Indicatif international inconnu : on conserve la forme « +chiffres » sans dénaturer.
            Return "+" & chiffres

        End If

        ' Aucun indicatif dans la saisie : on déduit le pays depuis le champ Pays.
        Dim regle As ReglePays = TrouverRegleParLibelle(libellePays)

        If regle Is Nothing Then
            Return brut
        End If

        Dim national As String = RetirerTrunkZero(regle, chiffres)
        Return "+" & regle.Indicatif & national

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : RetirerTrunkZero
    ' Rôle     : Retire le « 0 » national initial pour les pays qui en utilisent un (trunk).
    ' -------------------------------------------------------------------------------------------------
    Private Function RetirerTrunkZero(regle As ReglePays, national As String) As String

        If regle.UtiliseTrunkZero AndAlso national.StartsWith("0", StringComparison.Ordinal) Then
            Return national.Substring(1)
        End If

        Return national

    End Function

#End Region

#Region "Formatage pour affichage"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : FormaterAffichage
    ' Version  : V1.0.0
    ' Date     : 13/06/2026
    '
    ' Rôle     : Met en forme un numéro pour un affichage lisible et homogène.
    '
    ' Paramètres :
    ' - saisie      : Numéro saisi (formats variés autorisés).
    ' - libellePays : Libellé du champ Pays (utilisé si aucun indicatif n'est présent dans la saisie).
    '
    ' Retour   :
    ' - String : Numéro formaté (« +32 475 12 34 56 »), ou la saisie nettoyée si le pays est inconnu.
    ' -------------------------------------------------------------------------------------------------
    Public Function FormaterAffichage(saisie As String, libellePays As String) As String

        Dim e164 As String = NormaliserE164(saisie, libellePays)

        If e164.Length = 0 Then
            Return String.Empty
        End If

        If Not e164.StartsWith("+", StringComparison.Ordinal) Then
            Return e164
        End If

        Dim chiffres As String = e164.Substring(1)
        Dim regle As ReglePays = TrouverRegleParIndicatif(chiffres)

        If regle Is Nothing Then
            Return e164
        End If

        Dim national As String = chiffres.Substring(regle.Indicatif.Length)
        Dim nationalFormate As String = FormaterNational(regle.Code, national)

        Return "+" & regle.Indicatif & " " & nationalFormate

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : FormaterNational
    ' Rôle     : Regroupe les chiffres nationaux selon un découpage lisible propre à chaque pays.
    '
    ' Remarques :
    ' - Le cas « mobile » (le plus courant pour des patients) est traité finement ;
    '   les autres cas utilisent un regroupement régulier pragmatique.
    ' -------------------------------------------------------------------------------------------------
    Private Function FormaterNational(code As String, national As String) As String

        Select Case code

            Case "BE"
                If national.Length = 9 AndAlso national.StartsWith("4", StringComparison.Ordinal) Then
                    Return Grouper(national, {3, 2, 2, 2})
                ElseIf national.Length = 8 Then
                    If "2349".IndexOf(national(0)) >= 0 Then
                        Return Grouper(national, {1, 3, 2, 2})
                    End If
                    Return Grouper(national, {2, 2, 2, 2})
                End If

            Case "FR"
                If national.Length = 9 Then
                    Return Grouper(national, {1, 2, 2, 2, 2})
                End If

            Case "LU"
                If national.Length = 9 Then
                    Return Grouper(national, {3, 3, 3})
                ElseIf national.Length = 8 Then
                    Return Grouper(national, {3, 3, 2})
                ElseIf national.Length = 6 Then
                    Return Grouper(national, {2, 2, 2})
                End If

            Case "DE"
                If national.Length >= 7 AndAlso national.StartsWith("1", StringComparison.Ordinal) Then
                    Return national.Substring(0, 3) & " " & GrouperRegulier(national.Substring(3), 4)
                End If

            Case "NL"
                If national.Length = 9 AndAlso national.StartsWith("6", StringComparison.Ordinal) Then
                    Return Grouper(national, {1, 4, 4})
                End If

        End Select

        Return GrouperRegulier(national, 2)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Grouper
    ' Rôle     : Découpe une chaîne de chiffres selon un motif de tailles de groupes.
    '
    ' Remarques :
    ' - Si la longueur ne correspond pas au motif, repli sur un regroupement régulier par paquets de 2.
    ' -------------------------------------------------------------------------------------------------
    Private Function Grouper(chiffres As String, motif As Integer()) As String

        If chiffres.Length <> motif.Sum() Then
            Return GrouperRegulier(chiffres, 2)
        End If

        Dim sb As New StringBuilder()
        Dim index As Integer = 0

        For i As Integer = 0 To motif.Length - 1
            If i > 0 Then sb.Append(" "c)
            sb.Append(chiffres.Substring(index, motif(i)))
            index += motif(i)
        Next

        Return sb.ToString()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : GrouperRegulier
    ' Rôle     : Découpe une chaîne de chiffres en paquets réguliers d'une taille donnée.
    ' -------------------------------------------------------------------------------------------------
    Private Function GrouperRegulier(chiffres As String, taille As Integer) As String

        If taille <= 0 Then
            Return chiffres
        End If

        Dim sb As New StringBuilder()
        Dim i As Integer = 0

        While i < chiffres.Length
            If i > 0 Then sb.Append(" "c)
            Dim longueur As Integer = Math.Min(taille, chiffres.Length - i)
            sb.Append(chiffres.Substring(i, longueur))
            i += taille
        End While

        Return sb.ToString()

    End Function

#End Region

#Region "Validation par pays"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Valider
    ' Version  : V1.0.0
    ' Date     : 13/06/2026
    '
    ' Rôle     : Valide la plausibilité d'un numéro selon les règles du pays (nombre de chiffres).
    '
    ' Paramètres :
    ' - saisie        : Numéro saisi.
    ' - libellePays   : Libellé du champ Pays.
    ' - messageErreur : Message explicatif en cas d'anomalie (avertissement, non bloquant côté appelant).
    '
    ' Retour   :
    ' - Boolean : True si le numéro est vide (optionnel) ou plausible, False sinon.
    '
    ' Remarques :
    ' - Si le pays est inconnu, repli souple sur UtilsValidation.IsValidTelephone.
    ' - La décision de bloquer ou non l'enregistrement appartient à l'appelant (avertissement).
    ' -------------------------------------------------------------------------------------------------
    Public Function Valider(saisie As String,
                            libellePays As String,
                            ByRef messageErreur As String) As Boolean

        messageErreur = String.Empty

        Dim brut As String = If(saisie, "").Trim()

        If brut.Length = 0 Then
            Return True
        End If

        Dim e164 As String = NormaliserE164(saisie, libellePays)
        Dim regle As ReglePays = Nothing

        If e164.StartsWith("+", StringComparison.Ordinal) Then
            regle = TrouverRegleParIndicatif(e164.Substring(1))
        End If

        ' Pays indéterminé : repli sur la validation souple historique.
        If regle Is Nothing Then
            Return UtilsValidation.IsValidTelephone(saisie, messageErreur)
        End If

        Dim national As String = e164.Substring(1 + regle.Indicatif.Length)
        Dim nbChiffres As Integer = national.Length

        If nbChiffres < regle.MinNat OrElse nbChiffres > regle.MaxNat Then
            messageErreur = $"Le numéro pour {regle.Libelle} (+{regle.Indicatif}) devrait comporter " &
                            DescriptionLongueur(regle) & " après l'indicatif." & Environment.NewLine &
                            $"Numéro interprété : {FormaterAffichage(saisie, libellePays)}."
            Return False
        End If

        Return True

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : DescriptionLongueur
    ' Rôle     : Produit une description lisible de la fourchette de chiffres attendue.
    ' -------------------------------------------------------------------------------------------------
    Private Function DescriptionLongueur(regle As ReglePays) As String

        If regle.MinNat = regle.MaxNat Then
            Return $"{regle.MinNat} chiffres"
        End If

        Return $"entre {regle.MinNat} et {regle.MaxNat} chiffres"

    End Function

#End Region

#Region "Outils internes"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : SupprimerAccents
    ' Rôle     : Retire les signes diacritiques d'une chaîne (pour comparer des libellés de pays).
    ' -------------------------------------------------------------------------------------------------
    Private Function SupprimerAccents(texte As String) As String

        If String.IsNullOrEmpty(texte) Then
            Return String.Empty
        End If

        Dim normalise As String = texte.Normalize(NormalizationForm.FormD)
        Dim sb As New StringBuilder()

        For Each c As Char In normalise
            If CharUnicodeInfo.GetUnicodeCategory(c) <> UnicodeCategory.NonSpacingMark Then
                sb.Append(c)
            End If
        Next

        Return sb.ToString().Normalize(NormalizationForm.FormC)

    End Function

#End Region

End Module
