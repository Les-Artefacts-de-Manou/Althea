' -------------------------------------------------------------------------------------------------
' Module      : QueryPeriodicites
' Projet      : Althéa
' Version     : V1.1.0
'
' Rôle        :
' Requêtes SQL du référentiel ref_periodicites (aucun accès base ici).
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : lignes système protégées dans la requête elle-même
'   (code inchangé et ligne active pour la modifier, désactivation exclue) ; le métier refuse si
'   aucune ligne n'est concernée.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryPeriodicites

    Private Const COLONNES As String = "
    id_periodicite,
    code_periodicite,
    libelle_periodicite,
    nb_mois,
    actif,
    est_systeme,
    ordre_affichage"

    Public ReadOnly Property SelectPeriodicitesActives As String
        Get
            Return "SELECT" & COLONNES & "
FROM ref_periodicites
WHERE actif = 1
ORDER BY ordre_affichage, libelle_periodicite;"
        End Get
    End Property

    Public ReadOnly Property SelectPeriodicitesToutes As String
        Get
            Return "SELECT" & COLONNES & "
FROM ref_periodicites
ORDER BY ordre_affichage, libelle_periodicite;"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodePeriodicite As String
        Get
            Return "
SELECT COUNT(*) FROM ref_periodicites
WHERE code_periodicite = @code AND id_periodicite <> @id;"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibellePeriodicite As String
        Get
            Return "
SELECT COUNT(*) FROM ref_periodicites
WHERE libelle_periodicite = @libelle AND id_periodicite <> @id;"
        End Get
    End Property

    ' Nombre d'organismes rattachés à la périodicité (information avant désactivation)
    Public ReadOnly Property SelectCountUsagePeriodicite As String
        Get
            Return "
SELECT COUNT(*) FROM ref_organismes
WHERE id_periodicite = @id;"
        End Get
    End Property

    Public ReadOnly Property UpdatePeriodicite As String
        Get
            Return "
UPDATE ref_periodicites
SET code_periodicite = @code,
    libelle_periodicite = @libelle,
    nb_mois = @nbMois,
    actif = @actif,
    ordre_affichage = @ordre
WHERE id_periodicite = @id
  AND (est_systeme = 0 OR (BINARY code_periodicite = BINARY @code AND @actif = 1));"
        End Get
    End Property

    Public ReadOnly Property DesactiverPeriodicite As String
        Get
            Return "
UPDATE ref_periodicites SET actif = 0
WHERE id_periodicite = @id AND est_systeme = 0;"
        End Get
    End Property

    Public ReadOnly Property InsertPeriodicite As String
        Get
            Return "
INSERT INTO ref_periodicites
    (code_periodicite, libelle_periodicite, nb_mois, actif, ordre_affichage)
VALUES
    (@code, @libelle, @nbMois, @actif, @ordre);"
        End Get
    End Property

End Module
