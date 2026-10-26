' -------------------------------------------------------------------------------------------------
' Module      : QueryTypesIdentifiantPro
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Requêtes SQL du référentiel des types d'identifiant professionnel (ref_types_identifiant_pro).
'
' Remarques   :
' - Contient uniquement du SQL ; aucun accès à la base ici (GestionTypesIdentifiantPro).
' - Reprend la lecture des types qui se trouvait dans QueryCabinet (SelectTypesIdentifiantActifs).
' - Garde-fou en base pour les lignes système (INAMI, BCE, TVA) : leur code n'est jamais modifié
'   et elles restent actives, même si l'écran était contourné.
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : lignes système protégées dans la requête elle-même
'   (code inchangé et ligne active pour la modifier, désactivation exclue) ; le métier refuse si
'   aucune ligne n'est concernée.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryTypesIdentifiantPro

    Private Const SELECT_TYPES As String = "
SELECT id_type_identifiant, code_type_identifiant, libelle_type_identifiant, masque_saisie,
       regex_validation, cible, est_systeme, actif, ordre_affichage
FROM ref_types_identifiant_pro
"

    Public ReadOnly Property SelectTypesIdentifiantActifs As String
        Get
            Return SELECT_TYPES & "
WHERE actif = 1
ORDER BY ordre_affichage, libelle_type_identifiant;"
        End Get
    End Property

    Public ReadOnly Property SelectTypesIdentifiantTous As String
        Get
            Return SELECT_TYPES & "
ORDER BY ordre_affichage, libelle_type_identifiant;"
        End Get
    End Property

    Public ReadOnly Property InsertTypeIdentifiant As String
        Get
            Return "
INSERT INTO ref_types_identifiant_pro
    (code_type_identifiant, libelle_type_identifiant, masque_saisie, regex_validation, cible,
     est_systeme, actif, ordre_affichage)
VALUES (@code, @libelle, @masque, @regex, @cible, 0, @actif, @ordre);"
        End Get
    End Property

    Public ReadOnly Property UpdateTypeIdentifiant As String
        Get
            Return "
UPDATE ref_types_identifiant_pro
SET code_type_identifiant = @code,
    libelle_type_identifiant = @libelle,
    masque_saisie = @masque,
    regex_validation = @regex,
    cible = @cible,
    actif = @actif,
    ordre_affichage = @ordre
WHERE id_type_identifiant = @id
  AND (est_systeme = 0 OR (BINARY code_type_identifiant = BINARY @code AND @actif = 1));"
        End Get
    End Property

    Public ReadOnly Property DefinirActivationTypeIdentifiant As String
        Get
            Return "
UPDATE ref_types_identifiant_pro
SET actif = @actif
WHERE id_type_identifiant = @id
  AND (est_systeme = 0 OR @actif = 1);"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeTypeIdentifiant As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_types_identifiant_pro
WHERE code_type_identifiant = @code
  AND id_type_identifiant <> @id;"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleTypeIdentifiant As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_types_identifiant_pro
WHERE libelle_type_identifiant = @libelle
  AND id_type_identifiant <> @id;"
        End Get
    End Property

End Module
