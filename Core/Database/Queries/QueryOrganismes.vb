' -------------------------------------------------------------------------------------------------
' Module      : QueryOrganismes
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table référentielle ref_organismes.
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - id_organisme est généré par la base (AUTO_INCREMENT)
' - Utilisé par GestionOrganismes
' - Garde-fou côté base : une ligne système conserve son type (type_organisme)
' - V1.2.0 (22/10/2026, audit C.1, étape 3) : lignes système protégées dans la requête elle-même
'   (code inchangé et ligne active pour la modifier, désactivation exclue) ; le métier refuse si
'   aucune ligne n'est concernée ; SupprimerOrganisme, inutilisée(s), supprimée(s).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryOrganismes

#Region "SELECT"

    Private Const COLONNES As String = "
    o.id_organisme,
    o.code_organisme,
    o.libelle_organisme,
    o.type_organisme,
    o.adresse,
    o.telephone,
    o.telephone_2,
    o.email,
    o.email_2,
    o.site_web,
    o.url_plateforme,
    o.procedure_encodage,
    o.id_periodicite,
    p.libelle_periodicite,
    o.est_systeme,
    o.actif,
    o.ordre_affichage
FROM ref_organismes o
LEFT JOIN ref_periodicites p ON p.id_periodicite = o.id_periodicite"

    Public ReadOnly Property SelectOrganismesActifs As String
        Get
            Return "SELECT" & COLONNES & "
WHERE o.actif = 1
ORDER BY o.ordre_affichage, o.libelle_organisme;"
        End Get
    End Property

    Public ReadOnly Property SelectOrganismesTous As String
        Get
            Return "SELECT" & COLONNES & "
ORDER BY o.ordre_affichage, o.libelle_organisme;"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeOrganisme As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_organismes
WHERE code_organisme = @code
AND id_organisme <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleOrganisme As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_organismes
WHERE libelle_organisme = @libelle
AND id_organisme <> @id;
"
        End Get
    End Property

    ' Nombre de domaines rattachés à l'organisme (suppression physique interdite si > 0)
    Public ReadOnly Property SelectCountUsageOrganisme As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_domaines
WHERE id_organisme = @id;
"
        End Get
    End Property

#End Region

#Region "UPDATE / INSERT / DELETE"

    Public ReadOnly Property UpdateOrganisme As String
        Get
            Return "
UPDATE ref_organismes
SET code_organisme = @code,
    libelle_organisme = @libelle,
    type_organisme = @type,
    adresse = @adresse,
    telephone = @telephone,
    telephone_2 = @telephone2,
    email = @email,
    email_2 = @email2,
    site_web = @siteWeb,
    url_plateforme = @urlPlateforme,
    procedure_encodage = @procedure,
    id_periodicite = @idPeriodicite,
    actif = @actif,
    ordre_affichage = @ordre
WHERE id_organisme = @id
  AND (est_systeme = 0 OR (BINARY code_organisme = BINARY @code AND BINARY type_organisme = BINARY @type AND @actif = 1));"
        End Get
    End Property

    Public ReadOnly Property DesactiverOrganisme As String
        Get
            Return "
UPDATE ref_organismes
SET actif = 0
WHERE id_organisme = @id
AND est_systeme = 0;"
        End Get
    End Property

    Public ReadOnly Property InsertOrganisme As String
        Get
            Return "
INSERT INTO ref_organismes
(code_organisme, libelle_organisme, type_organisme, adresse, telephone, telephone_2, email, email_2,
 site_web, url_plateforme, procedure_encodage, id_periodicite, actif, ordre_affichage)
VALUES
(@code, @libelle, @type, @adresse, @telephone, @telephone2, @email, @email2,
 @siteWeb, @urlPlateforme, @procedure, @idPeriodicite, @actif, @ordre);"
        End Get
    End Property

#End Region

End Module
