' -------------------------------------------------------------------------------------------------
' Module      : QueryTypesSeance
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table référentielle ref_types_seance.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les types de séance actifs ou tous les types de séance
' - Vérifier l'unicité du code et du libellé d'un type de séance
' - Vérifier si un type de séance est utilisé (seances)
' - Mettre à jour un type de séance existant (avec tarif par défaut)
' - Désactiver un type de séance (soft-delete)
' - Supprimer physiquement un type de séance non utilisé
' - Insérer un nouveau type de séance (avec tarif par défaut)
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_type_seance est généré par la base (AUTO_INCREMENT) : jamais fourni par l'application
' - code_type_seance est saisi par l'utilisateur (majuscules, espaces remplacés par _), unique
' - PARTICULARITÉ : ref_types_seance possède un champ tarif_defaut decimal(10,2) NOT NULL
'
' Dépendances :
' - Utilisé par GestionTypesSeance pour les opérations CRUD sur ref_types_seance
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryTypesSeance

#Region "SELECT"

    Public ReadOnly Property SelectTypesSeanceActifs As String
        Get
            Return "
SELECT
    id_type_seance,
    code_type_seance,
    libelle_type_seance,
    tarif_defaut,
    actif,
    ordre_affichage
FROM ref_types_seance
WHERE actif = 1
ORDER BY ordre_affichage, libelle_type_seance;
"
        End Get
    End Property

    Public ReadOnly Property SelectTypesSeanceTous As String
        Get
            Return "
SELECT
    id_type_seance,
    code_type_seance,
    libelle_type_seance,
    tarif_defaut,
    actif,
    ordre_affichage
FROM ref_types_seance
ORDER BY ordre_affichage, libelle_type_seance;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeTypeSeance As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_types_seance
WHERE code_type_seance = @code
AND id_type_seance <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleTypeSeance As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_types_seance
WHERE libelle_type_seance = @libelle
AND id_type_seance <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountUsageTypeSeance As String
        Get
            Return "
SELECT
    (SELECT COUNT(*) FROM seances WHERE id_type_seance = @id) AS nb_usages;
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    Public ReadOnly Property UpdateTypeSeance As String
        Get
            Return "
        UPDATE ref_types_seance
        SET code_type_seance = @code,
            libelle_type_seance = @libelle,
            tarif_defaut = @tarif,
            actif = @actif,
            ordre_affichage = @ordre
        WHERE id_type_seance = @id"
        End Get
    End Property

    Public ReadOnly Property DesactiverTypeSeance As String
        Get
            Return "
        UPDATE ref_types_seance
        SET actif = 0
        WHERE id_type_seance = @id"
        End Get
    End Property

#End Region

#Region "DELETE"

    Public ReadOnly Property SupprimerTypeSeance As String
        Get
            Return "
        DELETE FROM ref_types_seance
        WHERE id_type_seance = @id"
        End Get
    End Property

#End Region

#Region "INSERT"

    Public ReadOnly Property InsertTypeSeance As String
        Get
            Return "
INSERT INTO ref_types_seance
(
    code_type_seance,
    libelle_type_seance,
    tarif_defaut,
    actif,
    ordre_affichage
)
VALUES
(
    @code,
    @libelle,
    @tarif,
    @actif,
    @ordre
);"
        End Get
    End Property

#End Region

End Module
