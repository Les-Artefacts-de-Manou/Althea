' -------------------------------------------------------------------------------------------------
' Module      : QueryTherapeutes
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 16/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table therapeutes (référentiel des thérapeutes).
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les thérapeutes actifs ou tous les thérapeutes, et un thérapeute par identifiant
' - Vérifier l'unicité du couple nom + prénom d'un thérapeute
' - Vérifier si un thérapeute est référencé ailleurs (dossiers, autres_suivis_patient)
' - Insérer, mettre à jour, désactiver (soft-delete) et supprimer physiquement un thérapeute
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_therapeute et code_therapeute sont générés par la base : jamais fournis par l'application
' - Le commentaire est stocké en texte simple (colonne commentaire)
' - La colonne actif est ajoutée manuellement à la table (pas de migration) — D-Therapeutes
'
' TODO (évolutions prévues — voir Docs\Todo\ToDo.md § Thérapeutes) :
' - Commentaire RTF : ajouter la colonne commentaire_rtf (et conserver le texte brut pour la recherche),
'   puis adapter les requêtes INSERT/UPDATE/SELECT ci-dessous (UC_RichTextEditorSimple côté UI).
' - Spécialité en table (idée à creuser, décision à prendre) : remplacer la colonne texte specialite
'   par une FK id_specialite vers un nouveau référentiel, et adapter les requêtes en conséquence.
'
' Dépendances :
' - Utilisé par GestionTherapeutes pour les opérations CRUD sur therapeutes
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryTherapeutes

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectTherapeutesActifs
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour sélectionner les thérapeutes actifs de la table therapeutes
    '
    ' Rôle       :
    ' Retourne tous les thérapeutes avec actif = 1, triés par nom puis prénom.
    '
    ' Paramètres SQL :
    ' - Aucun
    '
    ' Utilisé par :
    ' - GestionTherapeutes.GetTherapeutesActifs
    '
    ' Remarques  :
    ' - Les thérapeutes inactifs ne sont pas retournés
    ' - Utilisé notamment pour alimenter les listes déroulantes métier (combo Intervenants)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectTherapeutesActifs As String
        Get
            Return "
SELECT
    id_therapeute,
    code_therapeute,
    nom,
    prenom,
    specialite,
    telephone,
    email,
    adresse_ligne1,
    adresse_ligne2,
    code_postal,
    localite,
    pays,
    commentaire_rtf,
    commentaire_txt,
    actif,
    date_creation,
    date_modification
FROM therapeutes
WHERE actif = 1
ORDER BY nom, prenom;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectTherapeutesTous
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour sélectionner tous les thérapeutes de la table therapeutes
    '
    ' Rôle       :
    ' Retourne tous les thérapeutes, actifs et inactifs, triés par nom puis prénom.
    '
    ' Paramètres SQL :
    ' - Aucun
    '
    ' Utilisé par :
    ' - GestionTherapeutes.GetTherapeutes
    '
    ' Remarques  :
    ' - Inclut les thérapeutes inactifs (actif = 0)
    ' - Utilisé par l'écran de gestion du référentiel
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectTherapeutesTous As String
        Get
            Return "
SELECT
    id_therapeute,
    code_therapeute,
    nom,
    prenom,
    specialite,
    telephone,
    email,
    adresse_ligne1,
    adresse_ligne2,
    code_postal,
    localite,
    pays,
    commentaire_rtf,
    commentaire_txt,
    actif,
    date_creation,
    date_modification
FROM therapeutes
ORDER BY nom, prenom;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectTherapeuteById
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour charger un thérapeute par son identifiant
    '
    ' Rôle       :
    ' Retourne un thérapeute complet identifié par @id_therapeute.
    '
    ' Paramètres SQL :
    ' - @id_therapeute : Identifiant du thérapeute
    '
    ' Utilisé par :
    ' - GestionTherapeutes.GetTherapeuteById
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectTherapeuteById As String
        Get
            Return "
SELECT
    id_therapeute,
    code_therapeute,
    nom,
    prenom,
    specialite,
    telephone,
    email,
    adresse_ligne1,
    adresse_ligne2,
    code_postal,
    localite,
    pays,
    commentaire_rtf,
    commentaire_txt,
    actif,
    date_creation,
    date_modification
FROM therapeutes
WHERE id_therapeute = @id_therapeute
LIMIT 1;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountNomPrenom
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour vérifier l'existence d'un thérapeute par nom + prénom
    '
    ' Rôle       :
    ' Vérifie l'unicité du couple nom + prénom d'un thérapeute, en excluant un identifiant donné.
    '
    ' Paramètres SQL :
    ' - @nom    : Nom du thérapeute à vérifier
    ' - @prenom : Prénom du thérapeute à vérifier
    ' - @id     : Identifiant du thérapeute à exclure (utile lors de la mise à jour)
    '
    ' Utilisé par :
    ' - GestionTherapeutes.TherapeuteExiste
    '
    ' Remarques  :
    ' - Retourne un COUNT(*), 0 = couple disponible, >0 = couple déjà utilisé
    ' - La comparaison du prénom utilise COALESCE pour gérer les prénoms NULL
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountNomPrenom As String
        Get
            Return "
SELECT COUNT(*)
FROM therapeutes
WHERE nom = @nom
AND COALESCE(prenom, '') = COALESCE(@prenom, '')
AND id_therapeute <> @id;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountUsageTherapeute
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour vérifier si un thérapeute est référencé ailleurs
    '
    ' Rôle       :
    ' Compte le nombre total de références à un thérapeute dans les tables qui en dépendent
    ' (dossiers et autres_suivis_patient).
    '
    ' Paramètres SQL :
    ' - @id : Identifiant du thérapeute à vérifier
    '
    ' Utilisé par :
    ' - GestionTherapeutes.TherapeuteEstUtilise
    '
    ' Remarques  :
    ' - Retourne un COUNT total, 0 = thérapeute non utilisé (suppression physique possible),
    '   >0 = thérapeute utilisé (privilégier la désactivation / soft-delete)
    ' - Doit lister toutes les tables possédant une FK vers therapeutes(id_therapeute)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountUsageTherapeute As String
        Get
            Return "
SELECT
    (SELECT COUNT(*) FROM dossiers WHERE id_therapeute_traitant = @id)
    + (SELECT COUNT(*) FROM autres_suivis_patient WHERE id_therapeute = @id) AS nb_usages;
"
        End Get
    End Property

#End Region

#Region "INSERT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : InsertTherapeute
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour insérer un nouveau thérapeute
    '
    ' Rôle       :
    ' Insère une nouvelle ligne dans therapeutes ; id_therapeute et code_therapeute
    ' sont générés par la base.
    '
    ' Paramètres SQL :
    ' - @nom, @prenom, @specialite, @telephone, @email, @adresse_ligne1, @adresse_ligne2,
    '   @code_postal, @localite, @pays, @commentaire, @actif
    '
    ' Utilisé par :
    ' - GestionTherapeutes.CreateTherapeute
    '
    ' Remarques  :
    ' - date_creation et date_modification sont gérées par la base (DEFAULT current_timestamp())
    ' - L'ID généré est récupéré séparément via SELECT LASTVAL(seq_therapeutes)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertTherapeute As String
        Get
            Return "
INSERT INTO therapeutes (
    nom,
    prenom,
    specialite,
    telephone,
    email,
    adresse_ligne1,
    adresse_ligne2,
    code_postal,
    localite,
    pays,
    commentaire_rtf,
    commentaire_txt,
    actif
) VALUES (
    @nom,
    @prenom,
    @specialite,
    @telephone,
    @email,
    @adresse_ligne1,
    @adresse_ligne2,
    @code_postal,
    @localite,
    @pays,
    @commentaire_rtf,
    @commentaire_txt,
    @actif
);
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateTherapeute
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour mettre à jour un thérapeute existant
    '
    ' Rôle       :
    ' Met à jour toutes les colonnes éditables d'un thérapeute identifié par @id_therapeute.
    '
    ' Paramètres SQL :
    ' - Les mêmes que InsertTherapeute, plus @id_therapeute
    '
    ' Utilisé par :
    ' - GestionTherapeutes.UpdateTherapeute
    '
    ' Remarques  :
    ' - Ne modifie ni id_therapeute ni code_therapeute
    ' - date_modification est gérée par la base (ON UPDATE current_timestamp())
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateTherapeute As String
        Get
            Return "
UPDATE therapeutes
SET
    nom = @nom,
    prenom = @prenom,
    specialite = @specialite,
    telephone = @telephone,
    email = @email,
    adresse_ligne1 = @adresse_ligne1,
    adresse_ligne2 = @adresse_ligne2,
    code_postal = @code_postal,
    localite = @localite,
    pays = @pays,
    commentaire_rtf = @commentaire_rtf,
    commentaire_txt = @commentaire_txt,
    actif = @actif
WHERE id_therapeute = @id_therapeute;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DesactiverTherapeute
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour désactiver un thérapeute (soft-delete)
    '
    ' Rôle       :
    ' Désactive un thérapeute sans suppression physique en base de données.
    '
    ' Paramètres SQL :
    ' - @id : Identifiant du thérapeute à désactiver
    '
    ' Utilisé par :
    ' - GestionTherapeutes.DesactiverTherapeute
    '
    ' Remarques  :
    ' - À privilégier lorsqu'un thérapeute est déjà utilisé (FK existantes)
    ' - Le thérapeute reste présent en base avec actif = 0
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DesactiverTherapeute As String
        Get
            Return "
UPDATE therapeutes
SET actif = 0
WHERE id_therapeute = @id;
"
        End Get
    End Property

#End Region

#Region "DELETE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SupprimerTherapeute
    ' Version    : V1.0.0
    ' Date       : 16/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour supprimer physiquement un thérapeute
    '
    ' Rôle       :
    ' Supprime définitivement un thérapeute de la base de données.
    '
    ' Paramètres SQL :
    ' - @id : Identifiant du thérapeute à supprimer
    '
    ' Utilisé par :
    ' - GestionTherapeutes.SupprimerTherapeute
    '
    ' Remarques  :
    ' - À n'utiliser que pour un thérapeute NON utilisé (voir SelectCountUsageTherapeute)
    ' - Si le thérapeute est référencé, privilégier DesactiverTherapeute (soft-delete)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SupprimerTherapeute As String
        Get
            Return "
DELETE FROM therapeutes
WHERE id_therapeute = @id;
"
        End Get
    End Property

#End Region

End Module
