' -------------------------------------------------------------------------------------------------
' Module   : GestionCabinet
' Projet   : Althéa
' Version  : V1.7.1
' Date     : 25/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Accès métier à la fondation Cabinet (lot 27) : cabinet, praticiens, identifiants
' professionnels et qualifications.
'
' Remarques :
' - V1 : un seul cabinet actif et un seul praticien (contrôle applicatif).
' - Chaînes vides enregistrées en NULL.
' - V1.2.0 (20/10/2026) : lecture des types d'identifiant reprise par GestionTypesIdentifiantPro
'   (une seule lecture du référentiel, écran Administration › Types d'identifiant).
' - V1.3.0 (20/10/2026) : la fiche (cabinet, praticien, identifiants, qualifications) s'enregistre
'   en UNE transaction (EnregistrerFiche) : tout ou rien. Les images s'écrivent ensuite
'   (EnregistrerImages, noms de fichier seuls) ; les écritures unitaires deviennent privées.
' - V1.4.0 (20/10/2026) : CreerCompteSuperUser et LierUtilisateur exigent un administrateur
'   (rôle courant, élévation comprise ; audit A.3) : un SuperUser pouvait créer un compte dont
'   l'élévation va jusqu'à Admin, puis s'en servir.
' - V1.5.0 (21/10/2026, audit B.2) : CreerCompteSuperUser crée le compte et le lie au praticien dans
'   UNE transaction (avant : compte orphelin et mot de passe perdu si la liaison échouait) ; le
'   praticien doit être enregistré avant la création ; journal sans nom (identifiants seulement).
' - V1.6.0 (22/10/2026, audit C.1) : LierUtilisateur et CreerCompteSuperUser reçoivent un
'   ContexteOperation et vérifient le droit par Autorisation.Exiger.
' - V1.7.0 (24/10/2026) : GetPraticiens (compte lié) et GetUtilisateursLiables lisent le code du
'   compte (USR…).
' - V1.7.1 (25/10/2026, audit D-1) : Option Strict On.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionCabinet

#Region "Outils"

    Private Function DbVal(valeur As String) As Object
        If String.IsNullOrWhiteSpace(valeur) Then Return DBNull.Value
        Return valeur.Trim()
    End Function

    Private Function DbVal(valeur As Date?) As Object
        If valeur.HasValue Then Return valeur.Value.Date
        Return DBNull.Value
    End Function

    Private Function DbVal(valeur As Integer?) As Object
        If valeur.HasValue Then Return valeur.Value
        Return DBNull.Value
    End Function

    ' Identifiant de référentiel facultatif : 0 = non renseigné → NULL.
    Private Function DbId(id As ULong) As Object
        Return If(id = 0UL, CObj(DBNull.Value), CObj(id))
    End Function

    Private Function LireTexte(reader As MySqlDataReader, colonne As String) As String
        Dim i = reader.GetOrdinal(colonne)
        Return If(reader.IsDBNull(i), String.Empty, reader.GetString(i))
    End Function

    Private Function LireDate(reader As MySqlDataReader, colonne As String) As Date?
        Dim i = reader.GetOrdinal(colonne)
        Return If(reader.IsDBNull(i), CType(Nothing, Date?), reader.GetDateTime(i))
    End Function

#End Region

#Region "Cabinet"

    ''' <summary>Retourne le cabinet actif, ou Nothing si aucun n'est encore paramétré.</summary>
    Public Function GetCabinetActif() As Cabinet
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryCabinet.SelectCabinetActif, conn)
                Using reader = cmd.ExecuteReader()
                    If Not reader.Read() Then Return Nothing
                    Return New Cabinet With {
                        .IdCabinet = reader.GetUInt64("id_cabinet"),
                        .NomCabinet = reader.GetString("nom_cabinet"),
                        .IdSituationJuridique = If(reader.IsDBNull(reader.GetOrdinal("id_situation_juridique")), 0UL, reader.GetUInt64("id_situation_juridique")),
                        .AdresseRue = LireTexte(reader, "adresse_rue"),
                        .AdresseNumero = LireTexte(reader, "adresse_numero"),
                        .CodePostal = LireTexte(reader, "code_postal"),
                        .Localite = LireTexte(reader, "localite"),
                        .IdPays = If(reader.IsDBNull(reader.GetOrdinal("id_pays")), 0UL, reader.GetUInt64("id_pays")),
                        .CodePays = LireTexte(reader, "code_pays"),
                        .LibellePays = LireTexte(reader, "libelle_pays"),
                        .Telephone = LireTexte(reader, "telephone"),
                        .Email = LireTexte(reader, "email"),
                        .SiteWeb = LireTexte(reader, "site_web"),
                        .NumeroBce = LireTexte(reader, "numero_bce"),
                        .NumeroTva = LireTexte(reader, "numero_tva"),
                        .Iban = LireTexte(reader, "iban"),
                        .Bic = LireTexte(reader, "bic"),
                        .CheminLogo = LireTexte(reader, "chemin_logo"),
                        .Actif = reader.GetBoolean("actif")
                    }
                End Using
            End Using
        End Using
    End Function

    ' Crée ou met à jour le cabinet dans la transaction de EnregistrerFiche (IdCabinet renseigné).
    Private Sub EcrireCabinet(cab As Cabinet, conn As MySqlConnection, tx As MySqlTransaction)
        Dim estNouveau = cab.IdCabinet = 0UL
        Using cmd As New MySqlCommand(If(estNouveau, QueryCabinet.InsertCabinet, QueryCabinet.UpdateCabinet), conn, tx)
            cmd.Parameters.AddWithValue("@nom", cab.NomCabinet.Trim())
            cmd.Parameters.AddWithValue("@situation", If(cab.IdSituationJuridique = 0UL, CObj(DBNull.Value), CObj(cab.IdSituationJuridique)))
            cmd.Parameters.AddWithValue("@rue", DbVal(cab.AdresseRue))
            cmd.Parameters.AddWithValue("@numero", DbVal(cab.AdresseNumero))
            cmd.Parameters.AddWithValue("@cp", DbVal(cab.CodePostal))
            cmd.Parameters.AddWithValue("@localite", DbVal(cab.Localite))
            cmd.Parameters.AddWithValue("@idPays", DbId(cab.IdPays))
            cmd.Parameters.AddWithValue("@telephone", DbVal(cab.Telephone))
            cmd.Parameters.AddWithValue("@email", DbVal(cab.Email))
            cmd.Parameters.AddWithValue("@site", DbVal(cab.SiteWeb))
            cmd.Parameters.AddWithValue("@bce", DbVal(cab.NumeroBce))
            cmd.Parameters.AddWithValue("@tva", DbVal(cab.NumeroTva))
            cmd.Parameters.AddWithValue("@iban", DbVal(cab.Iban))
            cmd.Parameters.AddWithValue("@bic", DbVal(cab.Bic))
            cmd.Parameters.AddWithValue("@logo", DbVal(cab.CheminLogo))

            If estNouveau Then
                cab.IdCabinet = Convert.ToUInt64(cmd.ExecuteScalar())
            Else
                cmd.Parameters.AddWithValue("@id", cab.IdCabinet)
                cmd.ExecuteNonQuery()
            End If
        End Using
    End Sub

#End Region

#Region "Praticiens"

    Public Function GetPraticiens(idCabinet As ULong, afficherInactifs As Boolean) As List(Of Praticien)
        Dim result As New List(Of Praticien)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryCabinet.SelectPraticiensCabinet, conn)
                cmd.Parameters.AddWithValue("@idCabinet", idCabinet)
                cmd.Parameters.AddWithValue("@inactifs", If(afficherInactifs, 1, 0))
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        result.Add(New Praticien With {
                            .IdPraticien = reader.GetUInt64("id_praticien"),
                            .IdCabinet = reader.GetUInt64("id_cabinet"),
                            .IdCivilite = If(reader.IsDBNull(reader.GetOrdinal("id_civilite")), 0UL, reader.GetUInt64("id_civilite")),
                            .Nom = reader.GetString("nom"),
                            .Prenom = reader.GetString("prenom"),
                            .TitreProfessionnel = LireTexte(reader, "titre_professionnel"),
                            .Telephone = LireTexte(reader, "telephone"),
                            .Email = LireTexte(reader, "email"),
                            .SiteWeb = LireTexte(reader, "site_web"),
                            .AdresseRue = LireTexte(reader, "adresse_rue"),
                            .AdresseNumero = LireTexte(reader, "adresse_numero"),
                            .CodePostal = LireTexte(reader, "code_postal"),
                            .Localite = LireTexte(reader, "localite"),
                            .IdPays = If(reader.IsDBNull(reader.GetOrdinal("id_pays")), 0UL, reader.GetUInt64("id_pays")),
                            .CodePays = LireTexte(reader, "code_pays"),
                            .LibellePays = LireTexte(reader, "libelle_pays"),
                            .CheminPhoto = LireTexte(reader, "chemin_photo"),
                            .CheminCachet = LireTexte(reader, "chemin_cachet"),
                            .CheminSignature = LireTexte(reader, "chemin_signature"),
                            .Actif = reader.GetBoolean("actif"),
                            .IdUtilisateur = If(reader.IsDBNull(reader.GetOrdinal("id_utilisateur")), 0UL, reader.GetUInt64("id_utilisateur")),
                            .UtilisateurCode = LireTexte(reader, "code_utilisateur"),
                            .UtilisateurLogin = LireTexte(reader, "login_utilisateur"),
                            .UtilisateurNomAffichage = LireTexte(reader, "nom_affichage"),
                            .UtilisateurRole = LireTexte(reader, "role_utilisateur"),
                            .UtilisateurActif = Not reader.IsDBNull(reader.GetOrdinal("utilisateur_actif")) AndAlso reader.GetBoolean("utilisateur_actif")
                        })
                    End While
                End Using
            End Using
        End Using
        Return result
    End Function

    ''' <summary>Praticien principal du cabinet actif (V1 : l'unique praticien actif), ou Nothing.</summary>
    Public Function GetPraticienPrincipal() As Praticien
        Dim cab = GetCabinetActif()
        If cab Is Nothing Then Return Nothing
        Return GetPraticiens(cab.IdCabinet, False).FirstOrDefault()
    End Function

    ' Crée ou met à jour le praticien dans la transaction de EnregistrerFiche (IdPraticien renseigné).
    Private Sub EcrirePraticien(prat As Praticien, conn As MySqlConnection, tx As MySqlTransaction)
        Dim estNouveau = prat.IdPraticien = 0UL
        Using cmd As New MySqlCommand(If(estNouveau, QueryCabinet.InsertPraticien, QueryCabinet.UpdatePraticien), conn, tx)
            cmd.Parameters.AddWithValue("@idCabinet", prat.IdCabinet)
            cmd.Parameters.AddWithValue("@idCivilite", DbId(prat.IdCivilite))
            cmd.Parameters.AddWithValue("@nom", prat.Nom.Trim())
            cmd.Parameters.AddWithValue("@prenom", prat.Prenom.Trim())
            cmd.Parameters.AddWithValue("@titre", DbVal(prat.TitreProfessionnel))
            cmd.Parameters.AddWithValue("@telephone", DbVal(prat.Telephone))
            cmd.Parameters.AddWithValue("@email", DbVal(prat.Email))
            cmd.Parameters.AddWithValue("@site", DbVal(prat.SiteWeb))
            cmd.Parameters.AddWithValue("@rue", DbVal(prat.AdresseRue))
            cmd.Parameters.AddWithValue("@numero", DbVal(prat.AdresseNumero))
            cmd.Parameters.AddWithValue("@cp", DbVal(prat.CodePostal))
            cmd.Parameters.AddWithValue("@localite", DbVal(prat.Localite))
            cmd.Parameters.AddWithValue("@idPays", DbId(prat.IdPays))
            cmd.Parameters.AddWithValue("@photo", DbVal(prat.CheminPhoto))
            cmd.Parameters.AddWithValue("@cachet", DbVal(prat.CheminCachet))
            cmd.Parameters.AddWithValue("@signature", DbVal(prat.CheminSignature))
            cmd.Parameters.AddWithValue("@actif", prat.Actif)

            If estNouveau Then
                prat.IdPraticien = Convert.ToUInt64(cmd.ExecuteScalar())
            Else
                cmd.Parameters.AddWithValue("@id", prat.IdPraticien)
                cmd.ExecuteNonQuery()
            End If
        End Using
    End Sub

    ''' <summary>Comptes SuperUser actifs non liés à un praticien (candidats à la liaison).</summary>
    Public Function GetUtilisateursLiables() As List(Of UtilisateurLiable)
        Dim result As New List(Of UtilisateurLiable)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryCabinet.SelectUtilisateursLiables, conn)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        result.Add(New UtilisateurLiable With {
                            .IdUtilisateur = reader.GetUInt64("id_utilisateur"),
                            .CodeUtilisateur = reader.GetString("code_utilisateur"),
                            .Login = reader.GetString("login_utilisateur"),
                            .NomAffichage = reader.GetString("nom_affichage"),
                            .Role = reader.GetString("role_utilisateur")
                        })
                    End While
                End Using
            End Using
        End Using
        Return result
    End Function

    ''' <summary>
    ''' Lie (ou délie si idUtilisateur = 0) un compte au praticien. Opération volontairement séparée
    ''' de EnregistrerPraticien : le lien ne change que par une action explicite.
    ''' Le compte Admin technique n'est jamais lié à un praticien.
    ''' Réservé à l'administrateur (rôle courant, élévation comprise).
    ''' </summary>
    Public Sub LierUtilisateur(contexte As ContexteOperation, prat As Praticien, idUtilisateur As ULong)
        Autorisation.Exiger(contexte, AppRole.Admin, "Seul un administrateur peut lier ou délier le compte du praticien.")
        If prat.IdPraticien = 0UL Then
            Throw New InvalidOperationException("Le praticien doit être enregistré avant d'être lié à un compte.")
        End If
        If idUtilisateur > 0UL Then
            Dim deja = GetPraticienParUtilisateur(idUtilisateur)
            If deja IsNot Nothing AndAlso deja.IdPraticien <> prat.IdPraticien Then
                Throw New InvalidOperationException($"Ce compte est déjà lié au praticien {deja.NomComplet}.")
            End If
        End If
        DatabaseManager.ExecuterEnTransaction(Sub(conn, tx) EcrireLienUtilisateur(prat.IdPraticien, idUtilisateur, conn, tx))
        prat.IdUtilisateur = idUtilisateur
        JournaliserLien(prat.IdPraticien, idUtilisateur)
    End Sub

    ' Lien praticien ↔ compte (0 = délier), dans la transaction de l'appelant.
    Private Sub EcrireLienUtilisateur(idPraticien As ULong, idUtilisateur As ULong, conn As MySqlConnection, tx As MySqlTransaction)
        Using cmd As New MySqlCommand(QueryCabinet.LierUtilisateurPraticien, conn, tx)
            cmd.Parameters.AddWithValue("@idUtilisateur", If(idUtilisateur > 0UL, CObj(idUtilisateur), DBNull.Value))
            cmd.Parameters.AddWithValue("@id", idPraticien)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ' Identifiants seulement dans le journal (pas de nom de personne).
    Private Sub JournaliserLien(idPraticien As ULong, idUtilisateur As ULong)
        GestionLog.EcrireLog(
            If(idUtilisateur > 0UL,
               $"Praticien ID={idPraticien} lié au compte utilisateur #{idUtilisateur}.",
               $"Praticien ID={idPraticien} délié de son compte utilisateur."),
            GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security)
    End Sub

    ''' <summary>Praticien lié à un compte, ou Nothing (utilisé par la gestion des utilisateurs).</summary>
    Public Function GetPraticienParUtilisateur(idUtilisateur As ULong) As Praticien
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryCabinet.SelectPraticienParUtilisateur, conn)
                cmd.Parameters.AddWithValue("@idUtilisateur", idUtilisateur)
                Using reader = cmd.ExecuteReader()
                    If reader.Read() Then
                        Return New Praticien With {
                            .IdPraticien = reader.GetUInt64("id_praticien"),
                            .Nom = reader.GetString("nom"),
                            .Prenom = reader.GetString("prenom"),
                            .TitreProfessionnel = LireTexte(reader, "titre_professionnel"),
                            .CheminPhoto = LireTexte(reader, "chemin_photo"),
                            .IdUtilisateur = idUtilisateur
                        }
                    End If
                End Using
            End Using
        End Using
        Return Nothing
    End Function

    ''' <summary>
    ''' Crée le compte de travail du praticien (SuperUser, élévation Admin possible, mot de passe
    ''' temporaire à changer à la première connexion) et le lie au praticien.
    ''' Retourne le mot de passe temporaire à communiquer une seule fois.
    ''' Réservé à l'administrateur (rôle courant, élévation comprise).
    ''' V1.1.0 (21/10/2026, audit B.2) : compte et lien dans une seule transaction — un échec de la
    ''' liaison ne laisse plus de compte orphelin dont le mot de passe temporaire serait perdu.
    ''' </summary>
    Public Function CreerCompteSuperUser(contexte As ContexteOperation, prat As Praticien, login As String, nomAffichage As String) As String
        Autorisation.Exiger(contexte, AppRole.Admin, "Seul un administrateur peut créer le compte du praticien.")
        If prat.IdPraticien = 0UL Then
            Throw New InvalidOperationException("Le praticien doit être enregistré avant la création de son compte.")
        End If
        If prat.ACompteLie Then
            Throw New InvalidOperationException("Ce praticien est déjà lié à un compte.")
        End If

        Dim motDePasse = GestionUtilisateurs.GenererMotDePasseTemporaire()

        Dim idUtilisateur As Long = DatabaseManager.ExecuterEnTransaction(Of Long)(
            Function(conn, tx)
                Dim id As Long = GestionUtilisateurs.CreateUtilisateur(
                    contexte, login, nomAffichage, motDePasse, AppRole.SuperUser, AppRole.Admin,
                    actif:=True, mustChangePassword:=True, conn:=conn, tx:=tx)
                If id <= 0 Then
                    Throw New InvalidOperationException("Création du compte refusée (login déjà utilisé ou données invalides).")
                End If
                EcrireLienUtilisateur(prat.IdPraticien, CULng(id), conn, tx)
                Return id
            End Function)

        prat.IdUtilisateur = CULng(idUtilisateur)
        JournaliserLien(prat.IdPraticien, prat.IdUtilisateur)
        Return motDePasse
    End Function

#End Region

#Region "Identifiants"

    Public Function GetIdentifiants(idPraticien As ULong) As List(Of PraticienIdentifiant)
        Dim result As New List(Of PraticienIdentifiant)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryCabinet.SelectIdentifiantsPraticien, conn)
                cmd.Parameters.AddWithValue("@idPraticien", idPraticien)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        result.Add(New PraticienIdentifiant With {
                            .IdIdentifiant = reader.GetUInt64("id_identifiant"),
                            .IdPraticien = reader.GetUInt64("id_praticien"),
                            .IdTypeIdentifiant = reader.GetUInt64("id_type_identifiant"),
                            .LibelleType = reader.GetString("libelle_type_identifiant"),
                            .CodeType = reader.GetString("code_type_identifiant"),
                            .Valeur = reader.GetString("valeur"),
                            .DateDebut = LireDate(reader, "date_debut"),
                            .DateFin = LireDate(reader, "date_fin"),
                            .AfficherSurCachet = reader.GetBoolean("afficher_sur_cachet")
                        })
                    End While
                End Using
            End Using
        End Using
        Return result
    End Function

    Private Sub EcrireIdentifiant(ident As PraticienIdentifiant, conn As MySqlConnection, tx As MySqlTransaction)
        Dim estNouveau = ident.IdIdentifiant = 0UL
        Using cmd As New MySqlCommand(If(estNouveau, QueryCabinet.InsertIdentifiant, QueryCabinet.UpdateIdentifiant), conn, tx)
            cmd.Parameters.AddWithValue("@idPraticien", ident.IdPraticien)
            cmd.Parameters.AddWithValue("@idType", ident.IdTypeIdentifiant)
            cmd.Parameters.AddWithValue("@valeur", ident.Valeur.Trim())
            cmd.Parameters.AddWithValue("@debut", DbVal(ident.DateDebut))
            cmd.Parameters.AddWithValue("@fin", DbVal(ident.DateFin))
            cmd.Parameters.AddWithValue("@cachet", ident.AfficherSurCachet)
            If Not estNouveau Then cmd.Parameters.AddWithValue("@id", ident.IdIdentifiant)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

#End Region

#Region "Qualifications"

    Public Function GetQualifications(idPraticien As ULong) As List(Of PraticienQualification)
        Dim result As New List(Of PraticienQualification)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryCabinet.SelectQualificationsPraticien, conn)
                cmd.Parameters.AddWithValue("@idPraticien", idPraticien)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim iAnnee = reader.GetOrdinal("annee_obtention")
                        result.Add(New PraticienQualification With {
                            .IdQualification = reader.GetUInt64("id_qualification"),
                            .IdPraticien = reader.GetUInt64("id_praticien"),
                            .Libelle = reader.GetString("libelle"),
                            .Etablissement = LireTexte(reader, "etablissement"),
                            .AnneeObtention = If(reader.IsDBNull(iAnnee), CType(Nothing, Integer?), CInt(reader.GetInt16(iAnnee))),
                            .AfficherSurCachet = reader.GetBoolean("afficher_sur_cachet"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage")
                        })
                    End While
                End Using
            End Using
        End Using
        Return result
    End Function

    Private Sub EcrireQualification(qualif As PraticienQualification, conn As MySqlConnection, tx As MySqlTransaction)
        Dim estNouveau = qualif.IdQualification = 0UL
        Using cmd As New MySqlCommand(If(estNouveau, QueryCabinet.InsertQualification, QueryCabinet.UpdateQualification), conn, tx)
            cmd.Parameters.AddWithValue("@idPraticien", qualif.IdPraticien)
            cmd.Parameters.AddWithValue("@libelle", qualif.Libelle.Trim())
            cmd.Parameters.AddWithValue("@etablissement", DbVal(qualif.Etablissement))
            cmd.Parameters.AddWithValue("@annee", DbVal(qualif.AnneeObtention))
            cmd.Parameters.AddWithValue("@cachet", qualif.AfficherSurCachet)
            cmd.Parameters.AddWithValue("@ordre", qualif.OrdreAffichage)
            If Not estNouveau Then cmd.Parameters.AddWithValue("@id", qualif.IdQualification)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

#End Region

#Region "Enregistrement de la fiche"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : EnregistrerFiche
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       :
    ' Enregistre la fiche Cabinet en UNE transaction : cabinet, praticien, identifiants et
    ' qualifications (suppressions comprises). Tout ou rien.
    '
    ' Paramètres :
    ' - cab                      : cabinet (IdCabinet = 0 → création ; renseigné au retour)
    ' - prat                     : praticien, ou Nothing si aucun n'est saisi (identifiants et
    '                              qualifications ignorés alors)
    ' - identifiants, qualifications : lignes à créer (Id = 0) ou à mettre à jour
    ' - identifiantsSupprimes, qualificationsSupprimees : identifiants des lignes à supprimer
    '
    ' Remarques  :
    ' - Toutes les règles sont vérifiées AVANT d'ouvrir la transaction (ArgumentException,
    '   InvalidOperationException pour la règle V1 d'un seul praticien actif).
    ' - En cas d'échec : annulation, et les identifiants attribués pendant l'essai sont remis à
    '   leur valeur d'origine (l'écran peut réessayer sans créer de doublon).
    ' - Les images ne sont pas écrites ici : fichiers d'abord, puis EnregistrerImages.
    ' -------------------------------------------------------------------------------------------------
    Public Sub EnregistrerFiche(cab As Cabinet, prat As Praticien,
                                identifiants As IList(Of PraticienIdentifiant),
                                identifiantsSupprimes As IEnumerable(Of ULong),
                                qualifications As IList(Of PraticienQualification),
                                qualificationsSupprimees As IEnumerable(Of ULong))

        VerifierFiche(cab, prat, identifiants, qualifications)

        Dim idCabinetAvant As ULong = cab.IdCabinet
        Dim idPraticienAvant As ULong = If(prat?.IdPraticien, 0UL)

        Using conn = DatabaseManager.OpenConnection()
            Using tx = conn.BeginTransaction()
                Try
                    EcrireCabinet(cab, conn, tx)

                    If prat IsNot Nothing Then
                        prat.IdCabinet = cab.IdCabinet
                        EcrirePraticien(prat, conn, tx)

                        For Each id In identifiantsSupprimes
                            Supprimer(QueryCabinet.DeleteIdentifiant, id, conn, tx)
                        Next
                        For Each i In identifiants
                            i.IdPraticien = prat.IdPraticien
                            EcrireIdentifiant(i, conn, tx)
                        Next

                        For Each id In qualificationsSupprimees
                            Supprimer(QueryCabinet.DeleteQualification, id, conn, tx)
                        Next
                        For Each q In qualifications
                            q.IdPraticien = prat.IdPraticien
                            EcrireQualification(q, conn, tx)
                        Next
                    End If

                    tx.Commit()

                Catch
                    Try
                        tx.Rollback()
                    Catch exRollback As Exception
                        ' Connexion perdue : MariaDB annule de lui-même la transaction non validée.
                        GestionLog.EcrireException("Annulation de l'enregistrement du cabinet impossible.", exRollback,
                                                   GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database)
                    End Try
                    cab.IdCabinet = idCabinetAvant
                    If prat IsNot Nothing Then
                        prat.IdPraticien = idPraticienAvant
                        prat.IdCabinet = idCabinetAvant
                    End If
                    Throw
                End Try
            End Using
        End Using

    End Sub

    ' Règles de la fiche, vérifiées avant toute écriture.
    Private Sub VerifierFiche(cab As Cabinet, prat As Praticien,
                              identifiants As IEnumerable(Of PraticienIdentifiant),
                              qualifications As IEnumerable(Of PraticienQualification))

        If String.IsNullOrWhiteSpace(cab.NomCabinet) Then
            Throw New ArgumentException("Le nom du cabinet est obligatoire.")
        End If
        If prat Is Nothing Then Return

        If String.IsNullOrWhiteSpace(prat.Nom) OrElse String.IsNullOrWhiteSpace(prat.Prenom) Then
            Throw New ArgumentException("Le nom et le prénom du praticien sont obligatoires.")
        End If
        If prat.IdPraticien = 0UL AndAlso prat.Actif AndAlso cab.IdCabinet > 0UL AndAlso
           GetPraticiens(cab.IdCabinet, False).Count > 0 Then
            Throw New InvalidOperationException("Version 1 : un seul praticien actif par cabinet.")
        End If

        For Each ident In identifiants
            If String.IsNullOrWhiteSpace(ident.Valeur) Then
                Throw New ArgumentException("La valeur de l'identifiant est obligatoire.")
            End If
            If ident.DateDebut.HasValue AndAlso ident.DateFin.HasValue AndAlso ident.DateFin.Value < ident.DateDebut.Value Then
                Throw New ArgumentException("La date de fin doit être postérieure à la date de début.")
            End If
        Next
        For Each qualif In qualifications
            If String.IsNullOrWhiteSpace(qualif.Libelle) Then
                Throw New ArgumentException("Le libellé de la qualification est obligatoire.")
            End If
        Next

    End Sub

    Private Sub Supprimer(query As String, id As ULong, conn As MySqlConnection, tx As MySqlTransaction)
        Using cmd As New MySqlCommand(query, conn, tx)
            cmd.Parameters.AddWithValue("@id", id)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : EnregistrerImages
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       :
    ' Enregistre les noms de fichier des images (logo du cabinet ; photo, cachet et signature du
    ' praticien s'il est enregistré), en une transaction.
    '
    ' Remarques  :
    ' - Appelée après l'écriture des fichiers : la base ne cite pas un fichier qui n'existe pas.
    ' -------------------------------------------------------------------------------------------------
    Public Sub EnregistrerImages(cab As Cabinet, prat As Praticien)

        If cab Is Nothing OrElse cab.IdCabinet = 0UL Then
            Throw New ArgumentException("Le cabinet doit être enregistré avant ses images.")
        End If

        Using conn = DatabaseManager.OpenConnection()
            Using tx = conn.BeginTransaction()
                Using cmd As New MySqlCommand(QueryCabinet.UpdateLogoCabinet, conn, tx)
                    cmd.Parameters.AddWithValue("@logo", DbVal(cab.CheminLogo))
                    cmd.Parameters.AddWithValue("@id", cab.IdCabinet)
                    cmd.ExecuteNonQuery()
                End Using
                If prat IsNot Nothing AndAlso prat.IdPraticien > 0UL Then
                    Using cmd As New MySqlCommand(QueryCabinet.UpdateImagesPraticien, conn, tx)
                        cmd.Parameters.AddWithValue("@photo", DbVal(prat.CheminPhoto))
                        cmd.Parameters.AddWithValue("@cachet", DbVal(prat.CheminCachet))
                        cmd.Parameters.AddWithValue("@signature", DbVal(prat.CheminSignature))
                        cmd.Parameters.AddWithValue("@id", prat.IdPraticien)
                        cmd.ExecuteNonQuery()
                    End Using
                End If
                tx.Commit()
            End Using
        End Using

    End Sub

#End Region

End Module
