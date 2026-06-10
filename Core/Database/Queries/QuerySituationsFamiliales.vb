' -------------------------------------------------------------------------------------------------
' Module      : QuerySituationsFamiliales
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table référentielle ref_situations_familiales.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les situations familiales actives ou toutes les situations familiales
' - Vérifier l'unicité du code et du libellé d'une situation familiale
' - Vérifier si une situation familiale est utilisée (patients)
' - Mettre à jour une situation familiale existante
' - Désactiver une situation familiale (soft-delete)
' - Supprimer physiquement une situation familiale non utilisée
' - Insérer une nouvelle situation familiale
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_situation_familiale est généré par la base (AUTO_INCREMENT) : jamais fourni par l'application
' - code_situation_familiale est saisi par l'utilisateur (majuscules, espaces remplacés par _), unique
'
' Dépendances :
' - Utilisé par GestionSituationsFamiliales pour les opérations CRUD sur ref_situations_familiales
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QuerySituationsFamiliales

#Region "SELECT"

    Public ReadOnly Property SelectSituationsFamilialesActives As String
        Get
            Return "
SELECT
    id_situation_familiale,
    code_situation_familiale,
    libelle_situation_familiale,
    actif,
    ordre_affichage
FROM ref_situations_familiales
WHERE actif = 1
ORDER BY ordre_affichage, libelle_situation_familiale;
"
        End Get
    End Property

    Public ReadOnly Property SelectSituationsFamilialesToutes As String
        Get
            Return "
SELECT
    id_situation_familiale,
    code_situation_familiale,
    libelle_situation_familiale,
    actif,
    ordre_affichage
FROM ref_situations_familiales
ORDER BY ordre_affichage, libelle_situation_familiale;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeSituationFamiliale As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_situations_familiales
WHERE code_situation_familiale = @code
AND id_situation_familiale <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleSituationFamiliale As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_situations_familiales
WHERE libelle_situation_familiale = @libelle
AND id_situation_familiale <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountUsageSituationFamiliale As String
        Get
            Return "
SELECT
    (SELECT COUNT(*) FROM patients WHERE id_situation_familiale = @id) AS nb_usages;
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    Public ReadOnly Property UpdateSituationFamiliale As String
        Get
            Return "
        UPDATE ref_situations_familiales
        SET code_situation_familiale = @code,
            libelle_situation_familiale = @libelle,
            actif = @actif,
            ordre_affichage = @ordre
        WHERE id_situation_familiale = @id"
        End Get
    End Property

    Public ReadOnly Property DesactiverSituationFamiliale As String
        Get
            Return "
        UPDATE ref_situations_familiales
        SET actif = 0
        WHERE id_situation_familiale = @id"
        End Get
    End Property

#End Region

#Region "DELETE"

    Public ReadOnly Property SupprimerSituationFamiliale As String
        Get
            Return "
        DELETE FROM ref_situations_familiales
        WHERE id_situation_familiale = @id"
        End Get
    End Property

#End Region

#Region "INSERT"

    Public ReadOnly Property InsertSituationFamiliale As String
        Get
            Return "
INSERT INTO ref_situations_familiales
(
    code_situation_familiale,
    libelle_situation_familiale,
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
