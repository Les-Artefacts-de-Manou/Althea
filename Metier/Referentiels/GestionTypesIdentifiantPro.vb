' -------------------------------------------------------------------------------------------------
' Module      : GestionTypesIdentifiantPro
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Couche métier du référentiel des types d'identifiant professionnel (ref_types_identifiant_pro) :
' lecture, écriture, vérifications.
'
' Remarques   :
' - SQL dans QueryTypesIdentifiantPro ; accès par DatabaseManager ; requêtes paramétrées.
' - Seule lecture du référentiel dans l'application : l'écran Cabinet l'utilise aussi (elle
'   remplace GestionCabinet.GetTypesIdentifiantActifs).
' - Pas de suppression physique : un type qui ne sert plus est désactivé ; les identifiants déjà
'   saisis avec ce type restent lisibles.
' - Lignes système (INAMI, BCE, TVA : contrôle écrit dans l'application, choisi par le code) :
'   code et activation verrouillés, y compris dans les requêtes.
' - Masque et expression de contrôle vides enregistrés en NULL.
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser).
'   Lignes système : refus explicite si la requête n'a concerné aucune ligne (ExecuterSurLigneModifiable).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionTypesIdentifiantPro

#Region "Lecture"

    Public Function GetTypesIdentifiant(afficherInactifs As Boolean) As List(Of TypeIdentifiantPro)

        Dim liste As New List(Of TypeIdentifiantPro)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(If(afficherInactifs, QueryTypesIdentifiantPro.SelectTypesIdentifiantTous,
                                             QueryTypesIdentifiantPro.SelectTypesIdentifiantActifs), conn)
                Using reader = cmd.ExecuteReader()
                    Dim ordMasque As Integer = reader.GetOrdinal("masque_saisie")
                    Dim ordRegex As Integer = reader.GetOrdinal("regex_validation")
                    While reader.Read()
                        liste.Add(New TypeIdentifiantPro With {
                            .IdTypeIdentifiant = reader.GetUInt64("id_type_identifiant"),
                            .CodeTypeIdentifiant = reader.GetString("code_type_identifiant"),
                            .LibelleTypeIdentifiant = reader.GetString("libelle_type_identifiant"),
                            .MasqueSaisie = If(reader.IsDBNull(ordMasque), String.Empty, reader.GetString(ordMasque)),
                            .RegexValidation = If(reader.IsDBNull(ordRegex), String.Empty, reader.GetString(ordRegex)),
                            .Cible = reader.GetString("cible"),
                            .EstSysteme = reader.GetBoolean("est_systeme"),
                            .Actif = reader.GetBoolean("actif"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        })
                    End While
                End Using
            End Using
        End Using

        Return liste

    End Function

#End Region

#Region "Écriture"

    Public Sub InsertTypeIdentifiant(contexte As ContexteOperation, t As TypeIdentifiantPro)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Executer(QueryTypesIdentifiantPro.InsertTypeIdentifiant, t, False)
    End Sub

    Public Sub UpdateTypeIdentifiant(contexte As ContexteOperation, t As TypeIdentifiantPro)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Executer(QueryTypesIdentifiantPro.UpdateTypeIdentifiant, t, True)
    End Sub

    Private Sub Executer(query As String, t As TypeIdentifiantPro, avecId As Boolean)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                If avecId Then cmd.Parameters.AddWithValue("@id", t.IdTypeIdentifiant)
                cmd.Parameters.AddWithValue("@code", t.CodeTypeIdentifiant)
                cmd.Parameters.AddWithValue("@libelle", t.LibelleTypeIdentifiant)
                cmd.Parameters.AddWithValue("@masque", TexteOuNull(t.MasqueSaisie))
                cmd.Parameters.AddWithValue("@regex", TexteOuNull(t.RegexValidation))
                cmd.Parameters.AddWithValue("@cible", t.Cible)
                cmd.Parameters.AddWithValue("@actif", t.Actif)
                cmd.Parameters.AddWithValue("@ordre", t.OrdreAffichage)
                If avecId Then
                    ' Modification : la requête exclut ce qu'une ligne système interdit (audit C.1).
                    ReglesReferentiels.ExecuterSurLigneModifiable(cmd)
                Else
                    cmd.ExecuteNonQuery()
                End If
            End Using
        End Using
    End Sub

    ' Active ou désactive (jamais de suppression physique) ; sans effet pour désactiver une ligne système.
    Public Sub DefinirActivation(contexte As ContexteOperation, id As ULong, actif As Boolean)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryTypesIdentifiantPro.DefinirActivationTypeIdentifiant, conn)
                cmd.Parameters.AddWithValue("@id", id)
                cmd.Parameters.AddWithValue("@actif", actif)
                ReglesReferentiels.ExecuterSurLigneModifiable(cmd)
            End Using
        End Using
    End Sub

    Private Function TexteOuNull(valeur As String) As Object
        Return If(String.IsNullOrWhiteSpace(valeur), CObj(DBNull.Value), valeur.Trim())
    End Function

#End Region

#Region "Vérifications"

    Public Function CodeTypeIdentifiantExiste(code As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QueryTypesIdentifiantPro.SelectCountCodeTypeIdentifiant, "@code", code, idExclu) > 0
    End Function

    Public Function LibelleTypeIdentifiantExiste(libelle As String, Optional idExclu As ULong = 0) As Boolean
        Return Compter(QueryTypesIdentifiantPro.SelectCountLibelleTypeIdentifiant, "@libelle", libelle, idExclu) > 0
    End Function

    Private Function Compter(query As String, nomParam As String, valeur As String, id As ULong) As Integer
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue(nomParam, valeur)
                cmd.Parameters.AddWithValue("@id", id)
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

#End Region

End Module
