' -------------------------------------------------------------------------------------------------
' Module      : QueryLateralites
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Requêtes SQL du référentiel des latéralités (ref_lateralites, migration v2.46.0).
'
' Remarques   :
' - Contient uniquement du SQL ; aucun accès à la base ici (GestionLateralites).
' - Pas de suppression physique : une latéralité qui ne sert plus est désactivée.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryLateralites

    Private Const SELECT_LATERALITES As String = "
SELECT id_lateralite, code_lateralite, libelle_lateralite, actif, ordre_affichage
FROM ref_lateralites
"

    Public ReadOnly Property SelectLateralitesActives As String
        Get
            Return SELECT_LATERALITES & "
WHERE actif = 1
ORDER BY ordre_affichage, libelle_lateralite;"
        End Get
    End Property

    Public ReadOnly Property SelectLateralitesToutes As String
        Get
            Return SELECT_LATERALITES & "
ORDER BY ordre_affichage, libelle_lateralite;"
        End Get
    End Property

    Public ReadOnly Property InsertLateralite As String
        Get
            Return "
INSERT INTO ref_lateralites (code_lateralite, libelle_lateralite, actif, ordre_affichage)
VALUES (@code, @libelle, @actif, @ordre);"
        End Get
    End Property

    Public ReadOnly Property UpdateLateralite As String
        Get
            Return "
UPDATE ref_lateralites
SET code_lateralite = @code,
    libelle_lateralite = @libelle,
    actif = @actif,
    ordre_affichage = @ordre
WHERE id_lateralite = @id;"
        End Get
    End Property

    Public ReadOnly Property DefinirActivationLateralite As String
        Get
            Return "
UPDATE ref_lateralites
SET actif = @actif
WHERE id_lateralite = @id;"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeLateralite As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_lateralites
WHERE code_lateralite = @code
  AND id_lateralite <> @id;"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleLateralite As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_lateralites
WHERE libelle_lateralite = @libelle
  AND id_lateralite <> @id;"
        End Get
    End Property

End Module
