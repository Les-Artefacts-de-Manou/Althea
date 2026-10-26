' -------------------------------------------------------------------------------------------------
' Module   : CatalogueRequetes
' Projet   : Althéa (tests automatisés)
' Version  : V1.0.0
' Date     : 26/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Recense toutes les requêtes SQL des modules Query* d'Althéa (propriétés et fonctions sans
' paramètre qui rendent un texte), plus les requêtes construites pour chaque zone clinique, et les
' découpe en instructions préparables par MariaDB.
'
' Remarques :
' - Recensement par réflexion : une requête ajoutée à un module Query* est testée sans rien écrire.
' - Forme de PREPARE : un paramètre « @nom » devient « ? » (LIMIT @n serait refusé tel quel), une
'   requête à plusieurs instructions est découpée sur « ; » hors des chaînes et des commentaires.
'   Les variables système « @@… » sont laissées telles quelles.
' - Modèles de requête (repère remplacé à l'exécution, ex. « {IDS} ») : déclarés un par un dans
'   _reperes, jamais acceptés en bloc — un « {CONSTANTE} » oublié sans « $ » doit rester détecté.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Reflection
Imports System.Text

Public Module CatalogueRequetes

    Private ReadOnly _toutes As New Lazy(Of SortedDictionary(Of String, String))(AddressOf Recenser)

    ' Requête modèle → repère remplacé à l'exécution par le code appelant ; préparé ici comme un paramètre.
    Private ReadOnly _reperes As New Dictionary(Of String, String) From {
        {"QueryQuotasInami.SelectPlafondsParQuotas", "{IDS}"}   ' identifiants de règles (nombres), GestionQuotasInami.ChargerPlafonds
    }

    ' Nom lisible (« QueryPatients.SelectPatientsListe ») → texte SQL.
    Public ReadOnly Property Toutes As SortedDictionary(Of String, String)
        Get
            Return _toutes.Value
        End Get
    End Property

    Private Function Recenser() As SortedDictionary(Of String, String)

        Dim result As New SortedDictionary(Of String, String)(StringComparer.Ordinal)
        Dim espace As String = GetType(QueryDossiers).Namespace

        For Each t In GetType(QueryDossiers).Assembly.GetTypes().
            Where(Function(x) x.IsClass AndAlso x.Namespace = espace AndAlso x.Name.StartsWith("Query", StringComparison.Ordinal))

            For Each p In t.GetProperties(BindingFlags.Public Or BindingFlags.Static).
                Where(Function(x) x.PropertyType Is GetType(String) AndAlso x.GetIndexParameters().Length = 0)
                result($"{t.Name}.{p.Name}") = CStr(p.GetValue(Nothing))
            Next

            For Each m In t.GetMethods(BindingFlags.Public Or BindingFlags.Static Or BindingFlags.DeclaredOnly).
                Where(Function(x) x.ReturnType Is GetType(String) AndAlso x.GetParameters().Length = 0 AndAlso Not x.IsSpecialName)
                result($"{t.Name}.{m.Name}()") = CStr(m.Invoke(Nothing, Nothing))
            Next

        Next

        ' Requêtes construites pour une zone clinique : noms de colonnes du registre ZonesDossier.
        For Each zone As ZoneDossier In [Enum].GetValues(Of ZoneDossier)()
            Dim info As ZoneDossierInfo = ZonesDossier.GetInfo(zone)
            result($"QueryDossiers.SelectZoneDossier({zone})") = QueryDossiers.SelectZoneDossier(info)
            result($"QueryDossiers.UpdateZoneDossier({zone})") = QueryDossiers.UpdateZoneDossier(info)
        Next

        For Each repere In _reperes
            result(repere.Key) = result(repere.Key).Replace(repere.Value, "@liste_modele")
        Next

        Return result

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Instructions
    ' Rôle     : Découpe une requête en instructions (« ; » hors chaînes et commentaires), paramètres
    '            « @nom » remplacés par « ? » ; instructions vides ou faites de commentaires écartées.
    ' -------------------------------------------------------------------------------------------------
    Public Function Instructions(sql As String) As List(Of String)

        Dim result As New List(Of String)
        Dim courante As New StringBuilder()
        Dim contenuUtile As Boolean = False
        Dim i As Integer = 0
        Dim texte As String = If(sql, String.Empty)

        While i < texte.Length

            Dim c As Char = texte(i)
            Dim suivant As Char = If(i + 1 < texte.Length, texte(i + 1), ControlChars.NullChar)

            If c = "'"c OrElse c = """"c OrElse c = "`"c Then
                ' Chaîne ou identifiant cité : recopié tel quel jusqu'au délimiteur fermant (doublé = échappé).
                Dim fin As Integer = i + 1
                While fin < texte.Length
                    If texte(fin) = "\"c AndAlso c <> "`"c Then
                        fin += 2
                        Continue While
                    End If
                    If texte(fin) = c Then
                        If fin + 1 < texte.Length AndAlso texte(fin + 1) = c Then
                            fin += 2
                            Continue While
                        End If
                        Exit While
                    End If
                    fin += 1
                End While
                fin = Math.Min(fin, texte.Length - 1)
                courante.Append(texte, i, fin - i + 1)
                contenuUtile = True
                i = fin + 1

            ElseIf c = "-"c AndAlso suivant = "-"c Then
                Dim fin As Integer = texte.IndexOf(ControlChars.Lf, i)
                If fin < 0 Then fin = texte.Length - 1
                courante.Append(texte, i, fin - i + 1)
                i = fin + 1

            ElseIf c = "/"c AndAlso suivant = "*"c Then
                Dim fin As Integer = texte.IndexOf("*/", i + 2, StringComparison.Ordinal)
                fin = If(fin < 0, texte.Length - 1, fin + 1)
                courante.Append(texte, i, fin - i + 1)
                i = fin + 1

            ElseIf c = "@"c AndAlso suivant <> "@"c AndAlso (Char.IsLetter(suivant) OrElse suivant = "_"c) AndAlso
                   (i = 0 OrElse texte(i - 1) <> "@"c) Then
                ' Paramètre « @nom » → « ? ».
                Dim fin As Integer = i + 1
                While fin < texte.Length AndAlso (Char.IsLetterOrDigit(texte(fin)) OrElse texte(fin) = "_"c)
                    fin += 1
                End While
                courante.Append("?"c)
                contenuUtile = True
                i = fin

            ElseIf c = ";"c Then
                If contenuUtile Then result.Add(courante.ToString().Trim())
                courante.Clear()
                contenuUtile = False
                i += 1

            Else
                courante.Append(c)
                If Not Char.IsWhiteSpace(c) Then contenuUtile = True
                i += 1
            End If

        End While

        If contenuUtile Then result.Add(courante.ToString().Trim())

        Return result

    End Function

End Module
