' -------------------------------------------------------------------------------------------------
' Module      : QueryCategoriesQuota
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 20/09/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table référentielle ref_categories_quota.
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Les catégories sont propres à un domaine : unicité du code et du libellé par domaine
' - Utilisé par GestionCategoriesQuota
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryCategoriesQuota

#Region "SELECT"

    Private Const COLONNES As String = "
    id_categorie_quota,
    id_domaine,
    code_categorie,
    libelle_categorie,
    actif,
    ordre_affichage"

    ' Catégories d'un domaine (@id_domaine), actives et inactives
    Public ReadOnly Property SelectCategoriesParDomaine As String
        Get
            Return "
SELECT" & COLONNES & "
FROM ref_categories_quota
WHERE id_domaine = @id_domaine
ORDER BY ordre_affichage, libelle_categorie;
"
        End Get
    End Property

    ' Toutes les catégories actives, tous domaines confondus (combo des natures de séance)
    Public ReadOnly Property SelectCategoriesActives As String
        Get
            Return "
SELECT" & COLONNES & "
FROM ref_categories_quota
WHERE actif = 1
ORDER BY id_domaine, ordre_affichage, libelle_categorie;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeCategorie As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_categories_quota
WHERE id_domaine = @id_domaine
  AND code_categorie = @valeur
  AND id_categorie_quota <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleCategorie As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_categories_quota
WHERE id_domaine = @id_domaine
  AND libelle_categorie = @valeur
  AND id_categorie_quota <> @id;
"
        End Get
    End Property

    ' Usages bloquant la suppression physique : rattachements nature/domaine et plafonds historisés
    Public ReadOnly Property SelectCountUsageCategorie As String
        Get
            Return "
SELECT
    (SELECT COUNT(*) FROM lia_natures_seance_domaines WHERE id_categorie_quota = @id)
  + (SELECT COUNT(*) FROM ref_quotas_plafonds WHERE id_categorie_quota = @id);
"
        End Get
    End Property

#End Region

#Region "INSERT / UPDATE / DELETE"

    Public ReadOnly Property InsertCategorie As String
        Get
            Return "
INSERT INTO ref_categories_quota
(id_domaine, code_categorie, libelle_categorie, actif, ordre_affichage)
VALUES
(@id_domaine, @code, @libelle, @actif, @ordre);"
        End Get
    End Property

    Public ReadOnly Property UpdateCategorie As String
        Get
            Return "
UPDATE ref_categories_quota
SET code_categorie = @code,
    libelle_categorie = @libelle,
    actif = @actif,
    ordre_affichage = @ordre
WHERE id_categorie_quota = @id;"
        End Get
    End Property

    Public ReadOnly Property SupprimerCategorie As String
        Get
            Return "
DELETE FROM ref_categories_quota
WHERE id_categorie_quota = @id;"
        End Get
    End Property

#End Region

End Module
