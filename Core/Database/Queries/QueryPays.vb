' -------------------------------------------------------------------------------------------------
' Module      : QueryPays
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Requêtes SQL du référentiel ref_pays (aucun accès base ici).
'
' Remarques   :
' - Garde-fou côté base : une ligne système conserve son code et reste active, même si
'   l'interface venait à transmettre d'autres valeurs.
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : lignes système protégées dans la requête elle-même
'   (code inchangé et ligne active pour la modifier, désactivation exclue) ; le métier refuse si
'   aucune ligne n'est concernée.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryPays

    Private Const COLONNES As String = "
    id_pays,
    code_pays,
    libelle_pays,
    actif,
    est_systeme,
    ordre_affichage"

    Public ReadOnly Property SelectPaysActifs As String
        Get
            Return "SELECT" & COLONNES & "
FROM ref_pays
WHERE actif = 1
ORDER BY ordre_affichage, libelle_pays;"
        End Get
    End Property

    Public ReadOnly Property SelectPaysTous As String
        Get
            Return "SELECT" & COLONNES & "
FROM ref_pays
ORDER BY ordre_affichage, libelle_pays;"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodePays As String
        Get
            Return "
SELECT COUNT(*) FROM ref_pays
WHERE code_pays = @code AND id_pays <> @id;"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibellePays As String
        Get
            Return "
SELECT COUNT(*) FROM ref_pays
WHERE libelle_pays = @libelle AND id_pays <> @id;"
        End Get
    End Property

    Public ReadOnly Property UpdatePays As String
        Get
            Return "
UPDATE ref_pays
SET code_pays = @code,
    libelle_pays = @libelle,
    actif = @actif,
    ordre_affichage = @ordre
WHERE id_pays = @id
  AND (est_systeme = 0 OR (BINARY code_pays = BINARY @code AND @actif = 1));"
        End Get
    End Property

    Public ReadOnly Property DesactiverPays As String
        Get
            Return "
UPDATE ref_pays SET actif = 0
WHERE id_pays = @id AND est_systeme = 0;"
        End Get
    End Property

    Public ReadOnly Property InsertPays As String
        Get
            Return "
INSERT INTO ref_pays
    (code_pays, libelle_pays, actif, ordre_affichage)
VALUES
    (@code, @libelle, @actif, @ordre);"
        End Get
    End Property

End Module
