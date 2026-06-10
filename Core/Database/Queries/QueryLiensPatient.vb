' -------------------------------------------------------------------------------------------------
' Module      : QueryLiensPatient
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table référentielle ref_liens_patient.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les liens patient actifs ou tous les liens patient
' - Vérifier l'unicité du code et du libellé d'un lien patient
' - Vérifier si un lien patient est utilisé (contacts de la famille du patient)
' - Mettre à jour un lien patient existant
' - Désactiver un lien patient (soft-delete)
' - Supprimer physiquement un lien patient non utilisé
' - Insérer un nouveau lien patient
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_lien_patient est généré par la base (AUTO_INCREMENT) : jamais fourni par l'application
' - code_lien_patient est saisi par l'utilisateur (majuscules, espaces remplacés par _), unique
'
' Dépendances :
' - Utilisé par GestionLiensPatient pour les opérations CRUD sur ref_liens_patient
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryLiensPatient

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectLiensPatientActifs
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour sélectionner les liens patient actifs de la table ref_liens_patient
    '
    ' Rôle       :
    ' Retourne tous les liens patient avec actif = 1, triés par ordre d'affichage et libellé.
    '
    ' Paramètres SQL :
    ' - Aucun
    '
    ' Utilisé par :
    ' - GestionLiensPatient.GetLiensPatientActifs
    '
    ' Remarques  :
    ' - Les liens patient inactifs ne sont pas retournés
    ' - Utilisé notamment pour alimenter les listes déroulantes métier
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectLiensPatientActifs As String
        Get
            Return "
SELECT
    id_lien_patient,
    code_lien_patient,
    libelle_lien_patient,
    actif,
    ordre_affichage
FROM ref_liens_patient
WHERE actif = 1
ORDER BY ordre_affichage, libelle_lien_patient;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectLiensPatientTous
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour sélectionner tous les liens patient de la table ref_liens_patient
    '
    ' Rôle       :
    ' Retourne tous les liens patient, actifs et inactifs, triés par ordre d'affichage et libellé.
    '
    ' Paramètres SQL :
    ' - Aucun
    '
    ' Utilisé par :
    ' - GestionLiensPatient.GetLiensPatient
    '
    ' Remarques  :
    ' - Inclut les liens patient inactifs (actif = 0)
    ' - Utilisé par l'écran de gestion du référentiel
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectLiensPatientTous As String
        Get
            Return "
SELECT
    id_lien_patient,
    code_lien_patient,
    libelle_lien_patient,
    actif,
    ordre_affichage
FROM ref_liens_patient
ORDER BY ordre_affichage, libelle_lien_patient;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountCodeLienPatient
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour vérifier l'existence d'un code de lien patient
    '
    ' Rôle       :
    ' Vérifie l'unicité d'un code de lien patient, en excluant un identifiant donné.
    '
    ' Paramètres SQL :
    ' - @code : Code de lien patient à vérifier
    ' - @id   : Identifiant du lien patient à exclure (utile lors de la mise à jour)
    '
    ' Utilisé par :
    ' - GestionLiensPatient.CodeLienPatientExiste
    '
    ' Remarques  :
    ' - Permet de vérifier l'unicité du code lors de la création ou modification
    ' - Retourne un COUNT(*), 0 = code disponible, >0 = code déjà utilisé
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountCodeLienPatient As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_liens_patient
WHERE code_lien_patient = @code
AND id_lien_patient <> @id;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountLibelleLienPatient
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour vérifier l'existence d'un libellé de lien patient
    '
    ' Rôle       :
    ' Vérifie l'unicité d'un libellé de lien patient, en excluant un identifiant donné.
    '
    ' Paramètres SQL :
    ' - @libelle : Libellé de lien patient à vérifier
    ' - @id      : Identifiant du lien patient à exclure (utile lors de la mise à jour)
    '
    ' Utilisé par :
    ' - GestionLiensPatient.LibelleLienPatientExiste
    '
    ' Remarques  :
    ' - Permet de vérifier l'unicité du libellé lors de la création ou modification
    ' - Retourne un COUNT(*), 0 = libellé disponible, >0 = libellé déjà utilisé
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountLibelleLienPatient As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_liens_patient
WHERE libelle_lien_patient = @libelle
AND id_lien_patient <> @id;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountUsageLienPatient
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour vérifier si un lien patient est référencé ailleurs
    '
    ' Rôle       :
    ' Compte le nombre total de références à un lien patient dans les tables qui en dépendent
    ' (famille_contacts).
    '
    ' Paramètres SQL :
    ' - @id : Identifiant du lien patient à vérifier
    '
    ' Utilisé par :
    ' - GestionLiensPatient.LienPatientEstUtilise
    '
    ' Remarques  :
    ' - Retourne un COUNT total, 0 = lien patient non utilisé (suppression physique possible),
    '   >0 = lien patient utilisé (privilégier la désactivation / soft-delete)
    ' - Doit lister toutes les tables possédant une FK vers ref_liens_patient(id_lien_patient)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountUsageLienPatient As String
        Get
            Return "
SELECT
    (SELECT COUNT(*) FROM famille_contacts WHERE id_lien_patient = @id) AS nb_usages;
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateLienPatient
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour mettre à jour un lien patient existant
    '
    ' Rôle       :
    ' Met à jour les champs modifiables d'un lien patient identifié par son id.
    '
    ' Paramètres SQL :
    ' - @id      : Identifiant du lien patient à mettre à jour
    ' - @code    : Nouveau code du lien patient (majuscules, espaces remplacés par _)
    ' - @libelle : Nouveau libellé du lien patient
    ' - @actif   : Indique si le lien patient est actif (1 ou 0)
    ' - @ordre   : Nouvel ordre d'affichage
    '
    ' Utilisé par :
    ' - GestionLiensPatient.UpdateLienPatient
    '
    ' Remarques :
    ' - id_lien_patient sert d'identifiant technique et n'est jamais modifié.
    ' - L'unicité du code et du libellé est vérifiée en amont dans la couche métier.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateLienPatient As String
        Get
            Return "
        UPDATE ref_liens_patient
        SET code_lien_patient = @code,
            libelle_lien_patient = @libelle,
            actif = @actif,
            ordre_affichage = @ordre
        WHERE id_lien_patient = @id"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DesactiverLienPatient
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour désactiver un lien patient (soft-delete)
    '
    ' Rôle       :
    ' Désactive un lien patient sans suppression physique en base de données.
    '
    ' Paramètres SQL :
    ' - @id : Identifiant du lien patient à désactiver
    '
    ' Utilisé par :
    ' - GestionLiensPatient.DesactiverLienPatient
    '
    ' Remarques  :
    ' - À privilégier lorsqu'un lien patient est déjà utilisé (FK existantes)
    ' - Le lien patient reste présent en base avec actif = 0
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DesactiverLienPatient As String
        Get
            Return "
        UPDATE ref_liens_patient
        SET actif = 0
        WHERE id_lien_patient = @id"
        End Get
    End Property

#End Region

#Region "DELETE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SupprimerLienPatient
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour supprimer physiquement un lien patient
    '
    ' Rôle       :
    ' Supprime définitivement un lien patient de la base de données.
    '
    ' Paramètres SQL :
    ' - @id : Identifiant du lien patient à supprimer
    '
    ' Utilisé par :
    ' - GestionLiensPatient.SupprimerLienPatient
    '
    ' Remarques  :
    ' - À n'utiliser que pour un lien patient NON utilisé (voir SelectCountUsageLienPatient)
    ' - Si le lien patient est référencé, privilégier DesactiverLienPatient (soft-delete)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SupprimerLienPatient As String
        Get
            Return "
        DELETE FROM ref_liens_patient
        WHERE id_lien_patient = @id"
        End Get
    End Property

#End Region

#Region "INSERT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : InsertLienPatient
    ' Version    : V1.0.0
    ' Date       : 09/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour insérer un nouveau lien patient
    '
    ' Rôle       :
    ' Insère un nouveau lien patient dans la table ref_liens_patient.
    '
    ' Paramètres SQL :
    ' - @code    : Code du lien patient (majuscules, espaces remplacés par _)
    ' - @libelle : Libellé du lien patient
    ' - @actif   : Indique si le lien patient est actif (1 ou 0)
    ' - @ordre   : Ordre d'affichage du lien patient
    '
    ' Utilisé par :
    ' - GestionLiensPatient.InsertLienPatient
    '
    ' Remarques :
    ' - id_lien_patient est généré par la base (AUTO_INCREMENT).
    ' - Aucune valeur id ne doit être fournie par l'application.
    ' - L'unicité du code et du libellé est vérifiée en amont dans la couche métier.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertLienPatient As String
        Get
            Return "
INSERT INTO ref_liens_patient
(
    code_lien_patient,
    libelle_lien_patient,
    actif,
    ordre_affichage
)
VALUES
(
    @code,
    @libelle,
    @actif,
    @ordre
);"
        End Get
    End Property

#End Region

End Module
