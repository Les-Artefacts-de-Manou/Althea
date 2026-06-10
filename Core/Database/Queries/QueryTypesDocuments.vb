' -------------------------------------------------------------------------------------------------
' Module      : QueryTypesDocuments
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table référentielle ref_types_documents.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les types de document actifs ou tous les types de document
' - Vérifier l'unicité du code et du libellé d'un type de document
' - Vérifier si un type de document est utilisé (documents, modeles_documents)
' - Mettre à jour un type de document existant
' - Désactiver un type de document (soft-delete)
' - Supprimer physiquement un type de document non utilisé
' - Insérer un nouveau type de document
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_type_document est généré par la base (AUTO_INCREMENT) : jamais fourni par l'application
' - code_type_document est saisi par l'utilisateur (majuscules, espaces remplacés par _), unique
'
' Dépendances :
' - Utilisé par GestionTypesDocuments pour les opérations CRUD sur ref_types_documents
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryTypesDocuments

#Region "SELECT"

    Public ReadOnly Property SelectTypesDocumentsActifs As String
        Get
            Return "
SELECT
    id_type_document,
    code_type_document,
    libelle_type_document,
    actif,
    ordre_affichage
FROM ref_types_documents
WHERE actif = 1
ORDER BY ordre_affichage, libelle_type_document;
"
        End Get
    End Property

    Public ReadOnly Property SelectTypesDocumentsTous As String
        Get
            Return "
SELECT
    id_type_document,
    code_type_document,
    libelle_type_document,
    actif,
    ordre_affichage
FROM ref_types_documents
ORDER BY ordre_affichage, libelle_type_document;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeTypeDocument As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_types_documents
WHERE code_type_document = @code
AND id_type_document <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleTypeDocument As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_types_documents
WHERE libelle_type_document = @libelle
AND id_type_document <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountUsageTypeDocument As String
        Get
            Return "
SELECT
    (SELECT COUNT(*) FROM documents WHERE id_type_document = @id)
    + (SELECT COUNT(*) FROM modeles_documents WHERE id_type_document = @id) AS nb_usages;
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    Public ReadOnly Property UpdateTypeDocument As String
        Get
            Return "
        UPDATE ref_types_documents
        SET code_type_document = @code,
            libelle_type_document = @libelle,
            actif = @actif,
            ordre_affichage = @ordre
        WHERE id_type_document = @id"
        End Get
    End Property

    Public ReadOnly Property DesactiverTypeDocument As String
        Get
            Return "
        UPDATE ref_types_documents
        SET actif = 0
        WHERE id_type_document = @id"
        End Get
    End Property

#End Region

#Region "DELETE"

    Public ReadOnly Property SupprimerTypeDocument As String
        Get
            Return "
        DELETE FROM ref_types_documents
        WHERE id_type_document = @id"
        End Get
    End Property

#End Region

#Region "INSERT"

    Public ReadOnly Property InsertTypeDocument As String
        Get
            Return "
INSERT INTO ref_types_documents
(
    code_type_document,
    libelle_type_document,
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
