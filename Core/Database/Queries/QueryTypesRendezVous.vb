' -------------------------------------------------------------------------------------------------
' Module      : QueryTypesRendezVous
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table référentielle ref_types_rendez_vous.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les types de rendez-vous actifs ou tous les types de rendez-vous
' - Vérifier l'unicité du code et du libellé d'un type de rendez-vous
' - Vérifier si un type de rendez-vous est utilisé (rendez_vous)
' - Mettre à jour un type de rendez-vous existant
' - Désactiver un type de rendez-vous (soft-delete)
' - Supprimer physiquement un type de rendez-vous non utilisé
' - Insérer un nouveau type de rendez-vous
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_type_rendez_vous est généré par la base (AUTO_INCREMENT) : jamais fourni par l'application
' - code_type_rendez_vous est saisi par l'utilisateur (majuscules, espaces remplacés par _), unique
'
' Dépendances :
' - Utilisé par GestionTypesRendezVous pour les opérations CRUD sur ref_types_rendez_vous
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryTypesRendezVous

#Region "SELECT"

    Public ReadOnly Property SelectTypesRendezVousActifs As String
        Get
            Return "
SELECT
    id_type_rendez_vous,
    code_type_rendez_vous,
    libelle_type_rendez_vous,
    actif,
    ordre_affichage
FROM ref_types_rendez_vous
WHERE actif = 1
ORDER BY ordre_affichage, libelle_type_rendez_vous;
"
        End Get
    End Property

    Public ReadOnly Property SelectTypesRendezVousTous As String
        Get
            Return "
SELECT
    id_type_rendez_vous,
    code_type_rendez_vous,
    libelle_type_rendez_vous,
    actif,
    ordre_affichage
FROM ref_types_rendez_vous
ORDER BY ordre_affichage, libelle_type_rendez_vous;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeTypeRendezVous As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_types_rendez_vous
WHERE code_type_rendez_vous = @code
AND id_type_rendez_vous <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleTypeRendezVous As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_types_rendez_vous
WHERE libelle_type_rendez_vous = @libelle
AND id_type_rendez_vous <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountUsageTypeRendezVous As String
        Get
            Return "
SELECT
    (SELECT COUNT(*) FROM rendez_vous WHERE id_type_rendez_vous = @id) AS nb_usages;
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    Public ReadOnly Property UpdateTypeRendezVous As String
        Get
            Return "
        UPDATE ref_types_rendez_vous
        SET code_type_rendez_vous = @code,
            libelle_type_rendez_vous = @libelle,
            actif = @actif,
            ordre_affichage = @ordre
        WHERE id_type_rendez_vous = @id"
        End Get
    End Property

    Public ReadOnly Property DesactiverTypeRendezVous As String
        Get
            Return "
        UPDATE ref_types_rendez_vous
        SET actif = 0
        WHERE id_type_rendez_vous = @id"
        End Get
    End Property

#End Region

#Region "DELETE"

    Public ReadOnly Property SupprimerTypeRendezVous As String
        Get
            Return "
        DELETE FROM ref_types_rendez_vous
        WHERE id_type_rendez_vous = @id"
        End Get
    End Property

#End Region

#Region "INSERT"

    Public ReadOnly Property InsertTypeRendezVous As String
        Get
            Return "
INSERT INTO ref_types_rendez_vous
(
    code_type_rendez_vous,
    libelle_type_rendez_vous,
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
