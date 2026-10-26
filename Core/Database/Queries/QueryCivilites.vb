' -------------------------------------------------------------------------------------------------
' Module      : QueryCivilites
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Requêtes SQL du référentiel ref_civilites (aucun accès base ici).
'
' Remarques   :
' - Garde-fou côté base : une ligne système conserve son code et reste active.
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : lignes système protégées dans la requête elle-même
'   (code inchangé et ligne active pour la modifier, désactivation exclue) ; le métier refuse si
'   aucune ligne n'est concernée.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryCivilites

    Private Const COLONNES As String = "
    id_civilite,
    code_civilite,
    libelle_civilite,
    abreviation_civilite,
    actif,
    est_systeme,
    ordre_affichage"

    Public ReadOnly Property SelectCivilitesActives As String
        Get
            Return "SELECT" & COLONNES & "
FROM ref_civilites
WHERE actif = 1
ORDER BY ordre_affichage, libelle_civilite;"
        End Get
    End Property

    Public ReadOnly Property SelectCivilitesToutes As String
        Get
            Return "SELECT" & COLONNES & "
FROM ref_civilites
ORDER BY ordre_affichage, libelle_civilite;"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeCivilite As String
        Get
            Return "
SELECT COUNT(*) FROM ref_civilites
WHERE code_civilite = @code AND id_civilite <> @id;"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleCivilite As String
        Get
            Return "
SELECT COUNT(*) FROM ref_civilites
WHERE libelle_civilite = @libelle AND id_civilite <> @id;"
        End Get
    End Property

    Public ReadOnly Property UpdateCivilite As String
        Get
            Return "
UPDATE ref_civilites
SET code_civilite = @code,
    libelle_civilite = @libelle,
    abreviation_civilite = @abreviation,
    actif = @actif,
    ordre_affichage = @ordre
WHERE id_civilite = @id
  AND (est_systeme = 0 OR (BINARY code_civilite = BINARY @code AND @actif = 1));"
        End Get
    End Property

    Public ReadOnly Property DesactiverCivilite As String
        Get
            Return "
UPDATE ref_civilites SET actif = 0
WHERE id_civilite = @id AND est_systeme = 0;"
        End Get
    End Property

    Public ReadOnly Property InsertCivilite As String
        Get
            Return "
INSERT INTO ref_civilites
    (code_civilite, libelle_civilite, abreviation_civilite, actif, ordre_affichage)
VALUES
    (@code, @libelle, @abreviation, @actif, @ordre);"
        End Get
    End Property

End Module
