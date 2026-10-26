' -------------------------------------------------------------------------------------------------
' Classe      : LieuPatients
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Un lieu d'adresse (code postal, localité, pays) et les patients qui y habitent, pour la recherche
' des patients par lieu (ADR-52, R4).
'
' Remarques   :
' - Produit par GestionRecherche.GrouperParLieu ; aucun accès base de données, aucune
'   logique UI.
' - Deux écritures d'une même localité (accents, majuscules, traits d'union) forment un seul lieu
'   (UtilsCodePostal.CleLocalite) ; la localité affichée est l'écriture la plus fréquente.
' - Un lieu particulier réunit les patients sans adresse (ni code postal ni localité) ; un autre,
'   « tous les lieux trouvés » (Tous), réunit les patients d'une liste de lieux.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class LieuPatients

    Public Property CodePostal As String = String.Empty
    Public Property Localite As String = String.Empty

    ' Code ISO du pays de l'adresse ; une adresse sans pays est lue comme étant du pays du cabinet.
    Public Property CodePays As String = String.Empty

    ' Pays du cabinet : il n'est pas répété dans le libellé du lieu.
    Public Property CodePaysCabinet As String = String.Empty

    ' Lieu des patients sans adresse (ni code postal ni localité).
    Public Property EstSansAdresse As Boolean

    ' Réunion de plusieurs lieux (Tous) : « tous les lieux trouvés ».
    Public Property EstTous As Boolean

    ' Patients du lieu, par nom puis prénom.
    Public Property Patients As New List(Of PatientListeItem)

    ' Clé du lieu (regroupement, resélection au retour) : pays, code postal, localité normalisés ;
    ' chaîne vide pour « Sans adresse », Nothing pour « Tous ».
    Public ReadOnly Property Cle As String
        Get
            If EstTous Then Return Nothing
            If EstSansAdresse Then Return String.Empty
            Return CleDe(CodePays, CodePostal, Localite, CodePaysCabinet)
        End Get
    End Property

    Public ReadOnly Property NbPatients As Integer
        Get
            Return Patients.Count
        End Get
    End Property

    ' Lieu lisible : « 4317 Faimes », « 75001 Paris (FR) », « Sans adresse ».
    Public ReadOnly Property Libelle As String
        Get
            If EstTous Then Return "Tous les lieux trouvés"
            If EstSansAdresse Then Return "Sans adresse"
            Dim lieu As String = $"{CodePostal} {Localite}".Trim()
            If Not String.IsNullOrEmpty(CodePays) AndAlso Not String.Equals(CodePays, CodePaysCabinet, StringComparison.OrdinalIgnoreCase) Then
                lieu &= $" ({CodePays})"
            End If
            Return lieu
        End Get
    End Property

    ' Tous les lieux d'une liste réunis : leurs patients dans l'ordre des lieux.
    Public Shared Function Tous(lieux As IEnumerable(Of LieuPatients)) As LieuPatients
        Return New LieuPatients With {
            .EstTous = True,
            .Patients = If(lieux, Enumerable.Empty(Of LieuPatients)()).SelectMany(Function(l) l.Patients).ToList()
        }
    End Function

    ' Clé de regroupement d'une adresse ; chaîne vide pour une adresse sans code postal ni localité.
    ' Une adresse sans pays compte comme une adresse du pays du cabinet.
    Public Shared Function CleDe(codePays As String, codePostal As String, localite As String, codePaysCabinet As String) As String

        Dim cp As String = UtilsCodePostal.CleCodePostal(codePostal)
        Dim loc As String = UtilsCodePostal.CleLocalite(localite)
        If cp.Length = 0 AndAlso loc.Length = 0 Then Return String.Empty

        Dim pays As String = If(String.IsNullOrWhiteSpace(codePays), codePaysCabinet, codePays)
        Return $"{If(pays, String.Empty).Trim().ToUpperInvariant()}|{cp}|{loc}"

    End Function

End Class
