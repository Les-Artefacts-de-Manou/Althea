' -------------------------------------------------------------------------------------------------
' Module      : QuerySituationsJuridiques
' Projet      : Althéa
' Version     : V1.1.0
'
' Rôle        :
' Requêtes SQL du référentiel ref_situations_juridiques (aucun accès base ici).
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : lignes système protégées dans la requête elle-même
'   (code inchangé et ligne active pour la modifier, désactivation exclue) ; le métier refuse si
'   aucune ligne n'est concernée.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QuerySituationsJuridiques

    Private Const COLONNES As String = "
    id_situation_juridique,
    code_situation_juridique,
    libelle_situation_juridique,
    porteur_bce,
    actif,
    est_systeme,
    ordre_affichage"

    Public ReadOnly Property SelectSituationsJuridiquesActives As String
        Get
            Return "SELECT" & COLONNES & "
FROM ref_situations_juridiques
WHERE actif = 1
ORDER BY ordre_affichage, libelle_situation_juridique;"
        End Get
    End Property

    Public ReadOnly Property SelectSituationsJuridiquesToutes As String
        Get
            Return "SELECT" & COLONNES & "
FROM ref_situations_juridiques
ORDER BY ordre_affichage, libelle_situation_juridique;"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeSituationJuridique As String
        Get
            Return "
SELECT COUNT(*) FROM ref_situations_juridiques
WHERE code_situation_juridique = @code AND id_situation_juridique <> @id;"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleSituationJuridique As String
        Get
            Return "
SELECT COUNT(*) FROM ref_situations_juridiques
WHERE libelle_situation_juridique = @libelle AND id_situation_juridique <> @id;"
        End Get
    End Property

    Public ReadOnly Property UpdateSituationJuridique As String
        Get
            Return "
UPDATE ref_situations_juridiques
SET code_situation_juridique = @code,
    libelle_situation_juridique = @libelle,
    porteur_bce = @porteur,
    actif = @actif,
    ordre_affichage = @ordre
WHERE id_situation_juridique = @id
  AND (est_systeme = 0 OR (BINARY code_situation_juridique = BINARY @code AND @actif = 1));"
        End Get
    End Property

    Public ReadOnly Property DesactiverSituationJuridique As String
        Get
            Return "
UPDATE ref_situations_juridiques SET actif = 0
WHERE id_situation_juridique = @id AND est_systeme = 0;"
        End Get
    End Property

    Public ReadOnly Property InsertSituationJuridique As String
        Get
            Return "
INSERT INTO ref_situations_juridiques
    (code_situation_juridique, libelle_situation_juridique, porteur_bce, actif, ordre_affichage)
VALUES
    (@code, @libelle, @porteur, @actif, @ordre);"
        End Get
    End Property

End Module
