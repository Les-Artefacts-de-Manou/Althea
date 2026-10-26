' -------------------------------------------------------------------------------------------------
' Module      : QuerySpecialites
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Requêtes SQL du référentiel des spécialités des thérapeutes (ref_specialites, migration v2.55.0).
'
' Remarques   :
' - Contient uniquement du SQL ; aucun accès à la base ici (GestionSpecialites).
' - Pas de suppression physique : une spécialité qui ne sert plus est désactivée.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QuerySpecialites

    Private Const SELECT_SPECIALITES As String = "
SELECT id_specialite, code_specialite, libelle_specialite, actif, ordre_affichage
FROM ref_specialites
"

    Public ReadOnly Property SelectSpecialitesActives As String
        Get
            Return SELECT_SPECIALITES & "
WHERE actif = 1
ORDER BY ordre_affichage, libelle_specialite;"
        End Get
    End Property

    Public ReadOnly Property SelectSpecialitesToutes As String
        Get
            Return SELECT_SPECIALITES & "
ORDER BY ordre_affichage, libelle_specialite;"
        End Get
    End Property

    Public ReadOnly Property InsertSpecialite As String
        Get
            Return "
INSERT INTO ref_specialites (code_specialite, libelle_specialite, actif, ordre_affichage)
VALUES (@code, @libelle, @actif, @ordre);"
        End Get
    End Property

    Public ReadOnly Property UpdateSpecialite As String
        Get
            Return "
UPDATE ref_specialites
SET code_specialite = @code,
    libelle_specialite = @libelle,
    actif = @actif,
    ordre_affichage = @ordre
WHERE id_specialite = @id;"
        End Get
    End Property

    Public ReadOnly Property DefinirActivationSpecialite As String
        Get
            Return "
UPDATE ref_specialites
SET actif = @actif
WHERE id_specialite = @id;"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeSpecialite As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_specialites
WHERE code_specialite = @code
  AND id_specialite <> @id;"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleSpecialite As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_specialites
WHERE libelle_specialite = @libelle
  AND id_specialite <> @id;"
        End Get
    End Property

End Module
