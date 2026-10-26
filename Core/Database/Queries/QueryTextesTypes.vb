' -------------------------------------------------------------------------------------------------
' Module      : QueryTextesTypes
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Requêtes SQL des textes types (ref_textes_types), de leurs usages (ref_usages_textes_types) et
' de la liaison entre les deux (lia_textes_types_usages).
'
' Remarques   :
' - Contient uniquement du SQL ; aucun accès à la base ici (GestionTextesTypes).
' - id_texte_type est généré par la base (AUTO_INCREMENT) ; code_texte_type est saisi
'   (majuscules, espaces remplacés par _), unique, comme le libellé.
' - Pas de suppression physique d'un texte : on le désactive.
' - Un texte sans liaison est proposé dans tous les éditeurs.
' - V1.1.0 (19/10/2026, v2.43.0) : plusieurs usages par texte (liaison) ; requêtes de l'écran des
'   usages (libellé, ordre, activation).
' - V1.2.0 (22/10/2026, audit C.1, étape 3) : UpdateUsage — un usage système reste actif (condition
'   dans la requête).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryTextesTypes

#Region "Textes types : lecture"

    ' Colonnes communes ; usages agrégés dans l'ordre d'affichage (identifiants et libellés), en texte
    ' (CAST : GROUP_CONCAT peut sinon être lu comme binaire).
    Private Const SELECT_TEXTES As String = "
SELECT
    t.id_texte_type,
    t.code_texte_type,
    t.libelle_texte_type,
    (SELECT CAST(GROUP_CONCAT(u.id_usage_texte_type ORDER BY u.ordre_affichage SEPARATOR ',') AS CHAR)
       FROM lia_textes_types_usages l
       JOIN ref_usages_textes_types u ON u.id_usage_texte_type = l.id_usage_texte_type
      WHERE l.id_texte_type = t.id_texte_type) AS ids_usages,
    (SELECT CAST(GROUP_CONCAT(u.libelle_usage ORDER BY u.ordre_affichage SEPARATOR ', ') AS CHAR)
       FROM lia_textes_types_usages l
       JOIN ref_usages_textes_types u ON u.id_usage_texte_type = l.id_usage_texte_type
      WHERE l.id_texte_type = t.id_texte_type) AS libelles_usages,
    t.contenu_rtf,
    t.contenu_txt,
    t.actif,
    t.ordre_affichage
FROM ref_textes_types t
"

    Public ReadOnly Property SelectTextesTypesActifs As String
        Get
            Return SELECT_TEXTES & "
WHERE t.actif = 1
ORDER BY t.ordre_affichage, t.libelle_texte_type;"
        End Get
    End Property

    Public ReadOnly Property SelectTextesTypesTous As String
        Get
            Return SELECT_TEXTES & "
ORDER BY t.ordre_affichage, t.libelle_texte_type;"
        End Get
    End Property

    ' Textes proposés dans un éditeur : actifs, sans usage (partout) ou liés à son usage (actif).
    Public ReadOnly Property SelectTextesTypesPourUsage As String
        Get
            Return SELECT_TEXTES & "
WHERE t.actif = 1
  AND (NOT EXISTS (SELECT 1 FROM lia_textes_types_usages l WHERE l.id_texte_type = t.id_texte_type)
       OR EXISTS (SELECT 1
                    FROM lia_textes_types_usages l
                    JOIN ref_usages_textes_types u ON u.id_usage_texte_type = l.id_usage_texte_type
                   WHERE l.id_texte_type = t.id_texte_type
                     AND u.code_usage = @codeUsage
                     AND u.actif = 1))
ORDER BY t.ordre_affichage, t.libelle_texte_type;"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeTexteType As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_textes_types
WHERE code_texte_type = @code
  AND id_texte_type <> @id;"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleTexteType As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_textes_types
WHERE libelle_texte_type = @libelle
  AND id_texte_type <> @id;"
        End Get
    End Property

#End Region

#Region "Textes types : écriture"

    Public ReadOnly Property InsertTexteType As String
        Get
            Return "
INSERT INTO ref_textes_types
    (code_texte_type, libelle_texte_type, contenu_rtf, contenu_txt, actif, ordre_affichage)
VALUES
    (@code, @libelle, @rtf, @txt, @actif, @ordre);"
        End Get
    End Property

    Public ReadOnly Property UpdateTexteType As String
        Get
            Return "
UPDATE ref_textes_types
SET code_texte_type = @code,
    libelle_texte_type = @libelle,
    contenu_rtf = @rtf,
    contenu_txt = @txt,
    actif = @actif,
    ordre_affichage = @ordre
WHERE id_texte_type = @id;"
        End Get
    End Property

    Public ReadOnly Property DefinirActivationTexteType As String
        Get
            Return "
UPDATE ref_textes_types
SET actif = @actif
WHERE id_texte_type = @id;"
        End Get
    End Property

    Public ReadOnly Property SupprimerUsagesTexteType As String
        Get
            Return "
DELETE FROM lia_textes_types_usages
WHERE id_texte_type = @id;"
        End Get
    End Property

    Public ReadOnly Property InsertUsageTexteType As String
        Get
            Return "
INSERT INTO lia_textes_types_usages (id_texte_type, id_usage_texte_type)
VALUES (@id, @idUsage);"
        End Get
    End Property

#End Region

#Region "Usages"

    Private Const SELECT_USAGES As String = "
SELECT id_usage_texte_type, code_usage, libelle_usage, actif, est_systeme, ordre_affichage
FROM ref_usages_textes_types
"

    Public ReadOnly Property SelectUsagesActifs As String
        Get
            Return SELECT_USAGES & "
WHERE actif = 1
ORDER BY ordre_affichage, libelle_usage;"
        End Get
    End Property

    Public ReadOnly Property SelectUsagesTous As String
        Get
            Return SELECT_USAGES & "
ORDER BY ordre_affichage, libelle_usage;"
        End Get
    End Property

    ' Les usages naissent par migration (lignes système) : seuls libellé, ordre et activation changent.
    Public ReadOnly Property UpdateUsage As String
        Get
            Return "
UPDATE ref_usages_textes_types
SET libelle_usage = @libelle,
    ordre_affichage = @ordre,
    actif = @actif
WHERE id_usage_texte_type = @id
  AND (est_systeme = 0 OR @actif = 1);"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeUsage As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_usages_textes_types
WHERE code_usage = @code
  AND id_usage_texte_type <> @id;"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleUsage As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_usages_textes_types
WHERE libelle_usage = @libelle
  AND id_usage_texte_type <> @id;"
        End Get
    End Property

#End Region

End Module
