' -------------------------------------------------------------------------------------------------
' Classe   : BaseDeTests
' Projet   : Althéa (tests automatisés)
' Version  : V1.0.0
' Date     : 26/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Prépare la base vide dédiée aux tests (althea_tests) pour le test de fumée des requêtes : la
' structure y est recréée, une fois par série de tests, depuis Docs/Database/backup_NoData_althea.sql.
'
' Remarques (ADR-58) :
' - Chaîne de connexion lue dans la variable d'environnement ALTHEA_TESTS_CONNEXION du poste,
'   jamais dans le dépôt ; si elle manque, Disponible = False et les tests qui en dépendent sont
'   ignorés (poste sans base, intégration continue).
' - Garde-fou : la chaîne doit viser la base « althea_tests », sinon refus — la préparation supprime
'   toutes les tables de la base visée.
' - Aucune donnée : la sauvegarde ne contient que la structure. Vérifie au passage qu'elle se
'   recharge sans erreur.
' - Le script est adapté à la base de test : valeurs par défaut citant les séquences de la base
'   `althea` ramenées à la base courante ; clause DEFINER (compte root) retirée.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports MySqlConnector

Public NotInheritable Class BaseDeTests

    Public Const VARIABLE_CONNEXION As String = "ALTHEA_TESTS_CONNEXION"
    Public Const NOM_BASE As String = "althea_tests"

    ' True si la base de test est configurée sur le poste et sa structure recréée.
    Public ReadOnly Property Disponible As Boolean

    ' Chaîne de connexion à la base de test (variables utilisateur permises pour le script).
    Public ReadOnly Property ChaineConnexion As String

    Public Shared ReadOnly Property EstConfiguree As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(VARIABLE_CONNEXION))
        End Get
    End Property

    Public Sub New()

        If Not EstConfiguree Then Return

        Dim csb As New MySqlConnectionStringBuilder(Environment.GetEnvironmentVariable(VARIABLE_CONNEXION)) With {
            .AllowUserVariables = True
        }

        If Not String.Equals(csb.Database, NOM_BASE, StringComparison.OrdinalIgnoreCase) Then
            Throw New InvalidOperationException(
                $"{VARIABLE_CONNEXION} doit viser la base « {NOM_BASE} » (et non « {csb.Database} ») : " &
                "la préparation des tests y supprime puis recrée toutes les tables.")
        End If

        ChaineConnexion = csb.ConnectionString
        RecreerStructure()
        Disponible = True

    End Sub

    ' Supprime vues, tables et séquences de la base de test, puis rejoue la sauvegarde de structure.
    Private Sub RecreerStructure()

        Dim script As String = File.ReadAllText(CheminSauvegardeStructure(), Encoding.UTF8)
        script = script.Replace("`althea`.", String.Empty)
        script = Regex.Replace(script, "DEFINER=`[^`]*`@`[^`]*`\s*", String.Empty)

        Using conn As New MySqlConnection(ChaineConnexion)

            conn.Open()
            Executer(conn, "SET FOREIGN_KEY_CHECKS = 0;")

            Dim objets As New List(Of (Genre As String, Nom As String))
            Using cmd As New MySqlCommand(
                "SELECT table_type, table_name FROM information_schema.tables WHERE table_schema = DATABASE() " &
                "ORDER BY table_type = 'VIEW' DESC, table_name;", conn)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        objets.Add((GenreObjet(reader.GetString(0)), reader.GetString(1)))
                    End While
                End Using
            End Using

            For Each objet In objets
                Executer(conn, $"DROP {objet.Genre} IF EXISTS `{objet.Nom}`;")
            Next

            Executer(conn, script)
            Executer(conn, "SET FOREIGN_KEY_CHECKS = 1;")

        End Using

    End Sub

    ' Genre d'objet SQL d'après information_schema.tables.table_type (« BASE TABLE », « VIEW », « SEQUENCE »).
    Private Shared Function GenreObjet(typeTable As String) As String
        Select Case typeTable
            Case "VIEW" : Return "VIEW"
            Case "SEQUENCE" : Return "SEQUENCE"
            Case Else : Return "TABLE"
        End Select
    End Function

    Private Shared Sub Executer(conn As MySqlConnection, sql As String)
        Using cmd As New MySqlCommand(sql, conn) With {.CommandTimeout = 120}
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ' Docs/Database/backup_NoData_althea.sql, trouvé en remontant jusqu'à la racine du dépôt (Althea.slnx).
    Friend Shared Function CheminSauvegardeStructure() As String

        Dim dossier As DirectoryInfo = New DirectoryInfo(AppContext.BaseDirectory)

        While dossier IsNot Nothing AndAlso Not File.Exists(Path.Combine(dossier.FullName, "Althea.slnx"))
            dossier = dossier.Parent
        End While

        If dossier Is Nothing Then
            Throw New FileNotFoundException("Racine du dépôt (Althea.slnx) introuvable depuis le dossier des tests.")
        End If

        Return Path.Combine(dossier.FullName, "Docs", "Database", "backup_NoData_althea.sql")

    End Function

End Class
