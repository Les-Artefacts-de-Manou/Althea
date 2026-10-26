' -------------------------------------------------------------------------------------------------
' Module   : GestionDomaines
' Projet   : Althéa
' Version  : V1.3.1
' Date     : 25/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Gère les domaines de prise en charge (lecture depuis la base, écriture, vérifications),
' à partir de la table référentielle ref_domaines.
'
' Remarques :
' - Utilise DatabaseManager pour l'accès DB
' - Utilise QueryDomaines pour les requêtes SQL
' - Retourne des objets Domaine
' - Suppression physique réservée aux domaines non utilisés ; sinon soft-delete (Actif = 0)
' - V1.1.0 (20/10/2026, v2.45.0) : logo facultatif (lecture, DefinirLogo) ; le dossier du domaine
'   est retiré avec lui (SupprimerDomaine).
' - V1.2.0 (22/10/2026, audit C.1, étape 3) : chaque écriture reçoit le ContexteOperation et
'   commence par ReglesReferentiels.ExigerDroitEcriture (SuperUser) ; SupprimerDomaine, plus
'   appelée, supprimée.
' - V1.3.0 (23/10/2026, audit C.2 étape 3, ADR-51) : ValiderDomaine (règles sorties de l'écran Domaines),
'   appliqué par InsertDomaine et UpdateDomaine, qui n'en vérifiaient aucune ; règles de complétude
'   (CalculerManques, sorties de UC_SyntheseDomaine).
' - V1.3.1 (25/10/2026, audit D-1) : Option Strict On ; identifiant lu par GetUInt64 (colonne BIGINT
'   UNSIGNED, propriété ULong).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module GestionDomaines

#Region "Lecture"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetDomainesActifs
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Retourne la liste des domaines actifs depuis la base de données.
    '
    ' Retour     :
    ' - List(Of Domaine) : Liste des domaines actifs (actif = 1)
    '
    ' Remarques  :
    ' - Utilise QueryDomaines.SelectDomainesActifs
    ' - Utilisé notamment pour alimenter les listes déroulantes métier
    ' -------------------------------------------------------------------------------------------------
    Public Function GetDomainesActifs() As List(Of Domaine)

        Return GetDomainesDepuisRequete(QueryDomaines.SelectDomainesActifs)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetDomaines
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Charge les domaines selon l'option d'affichage des inactifs.
    '
    ' Paramètres :
    ' - afficherInactifs : True pour afficher actifs + désactivés, False pour actifs uniquement
    '
    ' Retour     :
    ' - List(Of Domaine) : Liste des domaines
    '
    ' Remarques  :
    ' - Si afficherInactifs = True, utilise SelectDomainesTous
    ' - Si afficherInactifs = False, utilise SelectDomainesActifs
    ' -------------------------------------------------------------------------------------------------
    Public Function GetDomaines(afficherInactifs As Boolean) As List(Of Domaine)

        If afficherInactifs Then
            Return GetDomainesDepuisRequete(QueryDomaines.SelectDomainesTous)
        End If

        Return GetDomainesActifs()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetDomainesDepuisRequete
    ' Version    : V1.1.1
    ' Date       : 25/10/2026
    '
    ' Rôle       :
    ' Exécute une requête SQL pour récupérer les domaines et les mappe en objets métier.
    '
    ' Paramètres :
    ' - query : Requête SQL à exécuter (SelectDomainesActifs ou SelectDomainesTous)
    '
    ' Retour     :
    ' - List(Of Domaine) : Liste des domaines mappés
    '
    ' Remarques  :
    ' - Méthode privée, appelée par GetDomaines et GetDomainesActifs
    ' - V1.1.0 (20/10/2026) : logo_fichier lu (LogoFichier).
    ' - V1.1.1 (25/10/2026, audit D-1) : identifiant lu par GetUInt64 (colonne BIGINT UNSIGNED,
    '   propriété ULong).
    ' -------------------------------------------------------------------------------------------------
    Private Function GetDomainesDepuisRequete(query As String) As List(Of Domaine)

        Dim result As New List(Of Domaine)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(query, conn)
                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        Dim dom As New Domaine With {
                            .IdDomaine = reader.GetUInt64("id_domaine"),
                            .CodeDomaine = reader.GetString("code_domaine"),
                            .LibelleDomaine = reader.GetString("libelle_domaine"),
                            .TarifConvention = reader.GetBoolean("tarif_convention"),
                            .NoShowFacturable = reader.GetBoolean("no_show_facturable"),
                            .SoumisQuota = reader.GetBoolean("soumis_quota"),
                            .IdOrganisme = If(reader.IsDBNull(reader.GetOrdinal("id_organisme")),
                                              CType(Nothing, ULong?),
                                              reader.GetUInt64("id_organisme")),
                            .LibelleOrganisme = If(reader.IsDBNull(reader.GetOrdinal("libelle_organisme")),
                                                   String.Empty,
                                                   reader.GetString("libelle_organisme")),
                            .Actif = reader.GetBoolean("actif"),
                            .OrdreAffichage = reader.GetInt32("ordre_affichage"),
                            .LogoFichier = If(reader.IsDBNull(reader.GetOrdinal("logo_fichier")),
                                              String.Empty,
                                              reader.GetString("logo_fichier"))
                        }

                        result.Add(dom)

                    End While

                End Using
            End Using
        End Using

        Return result

    End Function

#End Region

#Region "Ecriture"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : UpdateDomaine
    ' Version    : V1.2.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Met à jour un domaine dans la base de données.
    '
    ' Paramètres :
    ' - dom : Domaine avec toutes les propriétés remplies
    '
    ' Remarques  :
    ' - Utilise QueryDomaines.UpdateDomaine
    ' - Ne modifie PAS id_domaine
    ' - Refus si ValiderDomaine trouve une erreur (ErreurValidationException)
    ' - V1.1.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; droit d'écriture vérifié par le métier.
    ' - V1.2.0 (23/10/2026, audit C.2) : refus si ValiderDomaine trouve une erreur (ErreurValidationException).
    ' -------------------------------------------------------------------------------------------------
    Public Sub UpdateDomaine(contexte As ContexteOperation, dom As Domaine)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        ErreurValidationException.LeverSiErreurs(ValiderDomaine(dom))

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryDomaines.UpdateDomaine, conn)

                cmd.Parameters.AddWithValue("@id", dom.IdDomaine)
                cmd.Parameters.AddWithValue("@code", dom.CodeDomaine)
                cmd.Parameters.AddWithValue("@libelle", dom.LibelleDomaine)
                cmd.Parameters.AddWithValue("@tarifConvention", dom.TarifConvention)
                cmd.Parameters.AddWithValue("@noShowFacturable", dom.NoShowFacturable)
                cmd.Parameters.AddWithValue("@soumisQuota", dom.SoumisQuota)
                cmd.Parameters.AddWithValue("@idOrganisme", If(dom.IdOrganisme.HasValue, CObj(dom.IdOrganisme.Value), DBNull.Value))
                cmd.Parameters.AddWithValue("@actif", dom.Actif)
                cmd.Parameters.AddWithValue("@ordre", dom.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InsertDomaine
    ' Version    : V1.2.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Insère un nouveau domaine dans la base de données.
    '
    ' Paramètres :
    ' - dom : Domaine avec toutes les propriétés remplies
    '
    ' Remarques  :
    ' - Utilise QueryDomaines.InsertDomaine
    ' - id_domaine est généré automatiquement par la base (AUTO_INCREMENT)
    ' - Refus si ValiderDomaine trouve une erreur (ErreurValidationException)
    ' - V1.1.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; droit d'écriture vérifié par le métier.
    ' - V1.2.0 (23/10/2026, audit C.2) : refus si ValiderDomaine trouve une erreur (ErreurValidationException).
    ' -------------------------------------------------------------------------------------------------
    Public Sub InsertDomaine(contexte As ContexteOperation, dom As Domaine)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        ErreurValidationException.LeverSiErreurs(ValiderDomaine(dom))

        Using conn = DatabaseManager.OpenConnection()

            Using cmd As New MySqlCommand(QueryDomaines.InsertDomaine, conn)

                cmd.Parameters.AddWithValue("@code", dom.CodeDomaine)
                cmd.Parameters.AddWithValue("@libelle", dom.LibelleDomaine)
                cmd.Parameters.AddWithValue("@tarifConvention", dom.TarifConvention)
                cmd.Parameters.AddWithValue("@noShowFacturable", dom.NoShowFacturable)
                cmd.Parameters.AddWithValue("@soumisQuota", dom.SoumisQuota)
                cmd.Parameters.AddWithValue("@idOrganisme", If(dom.IdOrganisme.HasValue, CObj(dom.IdOrganisme.Value), DBNull.Value))
                cmd.Parameters.AddWithValue("@actif", dom.Actif)
                cmd.Parameters.AddWithValue("@ordre", dom.OrdreAffichage)

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DesactiverDomaine
    ' Version    : V1.1.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Désactive un domaine dans la base de données (soft-delete).
    '
    ' Paramètres :
    ' - idDomaine : Identifiant du domaine à désactiver (ULong)
    '
    ' Remarques  :
    ' - Utilise QueryDomaines.DesactiverDomaine
    ' - Passe actif à 0 sans supprimer physiquement la ligne
    ' - À privilégier lorsqu'un domaine est déjà utilisé (FK existantes)
    ' - V1.1.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; droit d'écriture vérifié par le métier.
    ' -------------------------------------------------------------------------------------------------
    Public Sub DesactiverDomaine(contexte As ContexteOperation, idDomaine As ULong)
        ReglesReferentiels.ExigerDroitEcriture(contexte)
        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryDomaines.DesactiverDomaine, conn)
                cmd.Parameters.AddWithValue("@id", idDomaine)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DefinirLogo
    ' Version    : V1.1.0
    ' Date       : 22/10/2026
    '
    ' Rôle       : Enregistre le nom du fichier logo d'un domaine (vide = pas de logo).
    ' Remarques  : Appelée une fois le fichier écrit (CheminsDomaineHelper.EnregistrerLogo) ou retiré.
    ' - V1.1.0 (22/10/2026, audit C.1) : reçoit le ContexteOperation ; droit d'écriture vérifié par le métier.
    ' -------------------------------------------------------------------------------------------------
    Public Sub DefinirLogo(contexte As ContexteOperation, idDomaine As ULong, nomFichier As String)
        ReglesReferentiels.ExigerDroitEcriture(contexte)

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryDomaines.UpdateLogoDomaine, conn)
                cmd.Parameters.AddWithValue("@id", idDomaine)
                cmd.Parameters.AddWithValue("@logo", If(String.IsNullOrWhiteSpace(nomFichier), CObj(DBNull.Value), nomFichier))
                cmd.ExecuteNonQuery()
            End Using
        End Using

        GestionLog.EcrireLog($"Logo du domaine ID={idDomaine} " & If(String.IsNullOrWhiteSpace(nomFichier), "retiré.", "enregistré."),
                             GestionLog.LogLevel.Rapide, GestionLog.LogCategory.Database)

    End Sub

#End Region

#Region "Validation"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ValiderDomaine
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Erreurs de saisie d'un domaine (liste vide s'il est valide) : code de 3 lettres et unique,
    ' libellé obligatoire et unique ; le quota ne se retire pas tant que des règles de quota sont
    ' actives.
    '
    ' Paramètres :
    ' - dom : Domaine à juger (IdDomaine = 0 en création)
    '
    ' Remarques  :
    ' - Sortie de UC_DomainesHome.ValiderSaisie (audit C.2, ADR-51) ; l'écran l'appelle pour ses
    '   champs, InsertDomaine et UpdateDomaine l'appliquent. Champs : NameOf de Domaine.
    ' - L'unicité n'est vérifiée que pour un code au bon format et un libellé renseigné.
    ' -------------------------------------------------------------------------------------------------
    Public Function ValiderDomaine(dom As Domaine) As List(Of ErreurSaisie)

        If dom Is Nothing Then Throw New ArgumentNullException(NameOf(dom))

        Dim erreurs As New List(Of ErreurSaisie)
        Dim code As String = If(dom.CodeDomaine, String.Empty).Trim()
        Dim libelle As String = If(dom.LibelleDomaine, String.Empty).Trim()

        If code.Length <> 3 OrElse Not code.All(AddressOf Char.IsLetter) Then
            erreurs.Add(New ErreurSaisie(NameOf(Domaine.CodeDomaine), "Le code doit comporter exactement 3 lettres."))
        ElseIf CodeDomaineExiste(code, dom.IdDomaine) Then
            erreurs.Add(New ErreurSaisie(NameOf(Domaine.CodeDomaine), "Ce code est déjà utilisé par un autre domaine."))
        End If

        If libelle.Length = 0 Then
            erreurs.Add(New ErreurSaisie(NameOf(Domaine.LibelleDomaine), "Le libellé est obligatoire."))
        ElseIf LibelleDomaineExiste(libelle, dom.IdDomaine) Then
            erreurs.Add(New ErreurSaisie(NameOf(Domaine.LibelleDomaine), "Ce libellé est déjà utilisé par un autre domaine."))
        End If

        If dom.IdDomaine <> 0UL AndAlso Not dom.SoumisQuota AndAlso DomainePossedeQuotasActifs(dom.IdDomaine) Then
            erreurs.Add(New ErreurSaisie(NameOf(Domaine.SoumisQuota),
                "Ce domaine possède encore des règles de quota actives : désactivez-les d'abord (étape 3) avant de retirer le quota."))
        End If

        Return erreurs

    End Function

#End Region

#Region "Complétude"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CalculerManques
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Liste ce qu'il manque pour qu'un domaine soit utilisable sans surprise, chaque manque portant
    ' l'étape où le corriger ; liste vide = domaine complet.
    '
    ' Paramètres :
    ' - dom : Domaine à contrôler (rien à dire d'un domaine pas encore enregistré)
    '
    ' Remarques  :
    ' - Lit les tarifs, natures et règles de quota du domaine, puis applique les règles (surcharge
    '   ci-dessous).
    ' - En cas d'erreur de lecture, un manque unique rattaché à l'étape Général signale que la
    '   complétude n'a pas pu être vérifiée : le domaine n'est pas dit complet.
    ' - Sortie de UC_SyntheseDomaine.CalculerManquesParEtape (audit C.2 étape 3), règles inchangées.
    ' -------------------------------------------------------------------------------------------------
    Public Function CalculerManques(dom As Domaine) As List(Of ManqueDomaine)

        If dom Is Nothing OrElse dom.IdDomaine = 0UL Then Return New List(Of ManqueDomaine)

        Try
            Return CalculerManques(dom,
                                   GestionLiaisonsPrixDomaines.GetLiaisonsParDomaine(dom.IdDomaine),
                                   GestionLiaisonsNaturesDomaines.GetLiaisonsParDomaine(dom.IdDomaine),
                                   GestionQuotasInami.GetQuotasParDomaine(dom.IdDomaine))
        Catch ex As Exception
            GestionLog.EcrireLog($"GestionDomaines.CalculerManques : erreur de lecture (domaine {dom.IdDomaine}).",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database, ex)
            Return New List(Of ManqueDomaine) From {
                New ManqueDomaine With {.Etape = EtapeDomaine.General,
                                        .Message = "Impossible de vérifier la complétude (voir le journal)."}}
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CalculerManques (données déjà lues)
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Règles de complétude d'un domaine, sur ses tarifs, natures et règles de quota déjà lus (la
    ' synthèse les a chargés pour son affichage : pas de seconde lecture).
    '
    ' Remarques  :
    ' - Règles déduites des indicateurs du domaine et de ses rattachements, jamais de son code.
    ' - Période sans règle entre deux règles (décision de Joëlle, 14/10/2026) : jamais comblée
    '   automatiquement, elle est signalée ; une période qui contient aujourd'hui l'est déjà par
    '   « sans règle en vigueur aujourd'hui », une période ouverte après la dernière règle est un
    '   arrêt voulu.
    ' -------------------------------------------------------------------------------------------------
    Public Function CalculerManques(dom As Domaine,
                                    tarifs As IEnumerable(Of LiaisonPrixDomaine),
                                    natures As IEnumerable(Of LiaisonNatureDomaine),
                                    quotas As IEnumerable(Of QuotaInamiRegle)) As List(Of ManqueDomaine)

        Dim manques As New List(Of ManqueDomaine)

        If dom.TarifConvention AndAlso Not tarifs.Any(Function(l) l.EstEnVigueur) Then
            AjouterManque(manques, EtapeDomaine.Tarifs, "Domaine conventionné sans tarif en vigueur aujourd'hui (étape 2. Tarifs).")
        End If

        If dom.SoumisQuota Then

            If Not GestionCategoriesQuota.GetCategoriesParDomaine(dom.IdDomaine, False).Any() Then
                AjouterManque(manques, EtapeDomaine.Quotas, "Aucune catégorie de quota créée (étape 3, « Gérer les catégories… »).")
            End If

            If Not quotas.Any(Function(r) r.EstEnVigueur) Then
                AjouterManque(manques, EtapeDomaine.Quotas, "Domaine soumis à quota sans règle en vigueur aujourd'hui (étape 3. Catégories & quotas).")
            End If

            For Each trou In GestionQuotasInami.PeriodesSansRegle(quotas).
                             Where(Function(t) t.Fin.HasValue AndAlso Not (t.Debut <= Date.Today AndAlso t.Fin.Value >= Date.Today))
                AjouterManque(manques, EtapeDomaine.Quotas,
                              $"Aucune règle du {trou.Debut:dd/MM/yyyy} au {trou.Fin.Value:dd/MM/yyyy} : les séances de cette " &
                              "période ne sont soumises à aucun quota (étape 3, « Nouvelle règle »).")
            Next

        End If

        If Not natures.Any(Function(l) l.Actif) Then
            AjouterManque(manques, EtapeDomaine.Natures, "Aucune nature de séance active n'est rattachée (étape 4. Natures de séance).")
        ElseIf dom.SoumisQuota AndAlso Not natures.Any(Function(l) l.Actif AndAlso l.ConsommeQuota) Then
            AjouterManque(manques, EtapeDomaine.Natures, "Aucune nature ne consomme le quota : choisissez sa catégorie (étape 4).")
        End If

        Return manques

    End Function

    Private Sub AjouterManque(liste As List(Of ManqueDomaine), etape As EtapeDomaine, message As String)
        liste.Add(New ManqueDomaine With {.Etape = etape, .Message = message})
    End Sub

#End Region

#Region "Vérifications"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetDomaineParId
    ' Version    : V1.0.0
    '
    ' Rôle       :
    ' Retourne un domaine précis (actif ou non) à partir de son identifiant.
    '
    ' Paramètres :
    ' - idDomaine : Identifiant du domaine recherché
    '
    ' Retour     :
    ' - Domaine : le domaine trouvé, ou Nothing si aucun domaine ne correspond
    '
    ' Remarques  :
    ' - S'appuie sur GetDomaines(True) afin de réutiliser le mapping unique des indicateurs
    '   de comportement (tarif_convention, no_show_facturable, soumis_quota).
    ' -------------------------------------------------------------------------------------------------
    Public Function GetDomaineParId(idDomaine As ULong) As Domaine

        Return GetDomaines(True).
            FirstOrDefault(Function(d) d.IdDomaine = idDomaine)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : DomainePossedeQuotasActifs
    ' Version    : V1.0.0
    '
    ' Rôle       :
    ' Indique si un domaine possède encore des règles de quota actives dans ref_quotas.
    '
    ' Paramètres :
    ' - idDomaine : Identifiant du domaine à contrôler
    '
    ' Retour     :
    ' - Boolean : True si au moins une règle de quota active existe
    '
    ' Remarques  :
    ' - Utilisé par ValiderDomaine pour empêcher de retirer l'indicateur « Soumis à quota » d'un
    '   domaine dont les règles de quota sont encore actives.
    ' -------------------------------------------------------------------------------------------------
    Public Function DomainePossedeQuotasActifs(idDomaine As ULong) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryDomaines.SelectCountQuotasActifsDomaine, conn)

                cmd.Parameters.AddWithValue("@id", idDomaine)

                Return Convert.ToInt32(cmd.ExecuteScalar()) > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : CodeDomaineExiste
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Vérifie si un code de domaine existe déjà dans la base de données.
    '
    ' Paramètres :
    ' - code    : Code de domaine à vérifier (String)
    ' - idExclu : Identifiant à exclure de la vérification (utile pour la mise à jour, optionnel)
    '
    ' Retour     :
    ' - Boolean : True si le code existe, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryDomaines.SelectCountCodeDomaine
    ' - Utilisé pour garantir l'unicité du code avant INSERT ou UPDATE
    ' -------------------------------------------------------------------------------------------------
    Public Function CodeDomaineExiste(code As String,
                                      Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryDomaines.SelectCountCodeDomaine, conn)

                cmd.Parameters.AddWithValue("@code", code)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : LibelleDomaineExiste
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Vérifie si un libellé de domaine existe déjà dans la base de données.
    '
    ' Paramètres :
    ' - libelle : Libellé de domaine à vérifier (String)
    ' - idExclu : Identifiant à exclure de la vérification (utile pour la mise à jour, optionnel)
    '
    ' Retour     :
    ' - Boolean : True si le libellé existe, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryDomaines.SelectCountLibelleDomaine
    ' - Utilisé pour garantir l'unicité du libellé avant INSERT ou UPDATE
    ' -------------------------------------------------------------------------------------------------
    Public Function LibelleDomaineExiste(libelle As String,
                                         Optional idExclu As ULong = 0) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryDomaines.SelectCountLibelleDomaine, conn)

                cmd.Parameters.AddWithValue("@libelle", libelle)
                cmd.Parameters.AddWithValue("@id", idExclu)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : DomaineEstUtilise
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Vérifie si un domaine est référencé dans les tables qui en dépendent
    ' (dossiers et modeles_documents).
    '
    ' Paramètres :
    ' - idDomaine : Identifiant du domaine à vérifier (ULong)
    '
    ' Retour     :
    ' - Boolean : True si le domaine est utilisé, False sinon
    '
    ' Remarques  :
    ' - Utilise QueryDomaines.SelectCountUsageDomaine
    ' - Utilisé pour décider entre suppression physique (False) et soft-delete (True)
    ' -------------------------------------------------------------------------------------------------
    Public Function DomaineEstUtilise(idDomaine As ULong) As Boolean

        Using conn = DatabaseManager.OpenConnection()
            Using cmd As New MySqlCommand(QueryDomaines.SelectCountUsageDomaine, conn)

                cmd.Parameters.AddWithValue("@id", idDomaine)

                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return count > 0

            End Using
        End Using

    End Function

#End Region

End Module
