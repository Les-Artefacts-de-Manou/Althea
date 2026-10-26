' -------------------------------------------------------------------------------------------------
' Module      : QueryCabinet
' Projet      : Althéa
' Version     : V1.4.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL de la fondation Cabinet (cab_cabinet, cab_praticiens,
' cab_praticien_identifiants, cab_praticien_qualifications).
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Utilisé par GestionCabinet
' - V1.2.0 (20/10/2026) : lecture des types d'identifiant déplacée dans QueryTypesIdentifiantPro
'   (référentiel désormais modifiable depuis Administration › Types d'identifiant).
' - V1.3.0 (20/10/2026) : UpdateLogoCabinet, UpdateImagesPraticien (images écrites après la fiche).
' - V1.4.0 (24/10/2026) : la lecture des praticiens (compte lié) et SelectUtilisateursLiables rendent
'   aussi code_utilisateur (code du compte visible, Rules § 15).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryCabinet

#Region "Cabinet"

    Public ReadOnly Property SelectCabinetActif As String
        Get
            Return "
SELECT id_cabinet, nom_cabinet, id_situation_juridique, adresse_rue, adresse_numero, code_postal, localite, id_pays,
       (SELECT rp.code_pays FROM ref_pays rp WHERE rp.id_pays = cab_cabinet.id_pays) AS code_pays,
       (SELECT rp.libelle_pays FROM ref_pays rp WHERE rp.id_pays = cab_cabinet.id_pays) AS libelle_pays,
       telephone, email, site_web, numero_bce, numero_tva, iban, bic, chemin_logo, actif
FROM cab_cabinet
WHERE actif = 1
ORDER BY id_cabinet
LIMIT 1;
"
        End Get
    End Property

    Public ReadOnly Property InsertCabinet As String
        Get
            Return "
INSERT INTO cab_cabinet (nom_cabinet, id_situation_juridique, adresse_rue, adresse_numero, code_postal, localite, id_pays,
    telephone, email, site_web, numero_bce, numero_tva, iban, bic, chemin_logo, actif)
VALUES (@nom, @situation, @rue, @numero, @cp, @localite, @idPays,
    @telephone, @email, @site, @bce, @tva, @iban, @bic, @logo, 1);
SELECT LAST_INSERT_ID();
"
        End Get
    End Property

    Public ReadOnly Property UpdateCabinet As String
        Get
            Return "
UPDATE cab_cabinet SET
    nom_cabinet = @nom, id_situation_juridique = @situation, adresse_rue = @rue, adresse_numero = @numero, code_postal = @cp,
    localite = @localite, id_pays = @idPays, telephone = @telephone, email = @email, site_web = @site,
    numero_bce = @bce, numero_tva = @tva, iban = @iban, bic = @bic, chemin_logo = @logo
WHERE id_cabinet = @id;
"
        End Get
    End Property

    Public ReadOnly Property UpdateLogoCabinet As String
        Get
            Return "
UPDATE cab_cabinet SET chemin_logo = @logo
WHERE id_cabinet = @id;
"
        End Get
    End Property

#End Region

#Region "Praticiens"

    Public ReadOnly Property SelectPraticiensCabinet As String
        Get
            Return "
SELECT p.id_praticien, p.id_cabinet, p.id_utilisateur, p.id_civilite, p.nom, p.prenom, p.titre_professionnel, p.telephone, p.email,
       p.site_web, p.adresse_rue, p.adresse_numero, p.code_postal, p.localite, p.id_pays,
       (SELECT rp.code_pays FROM ref_pays rp WHERE rp.id_pays = p.id_pays) AS code_pays,
       (SELECT rp.libelle_pays FROM ref_pays rp WHERE rp.id_pays = p.id_pays) AS libelle_pays,
       p.chemin_photo, p.chemin_cachet, p.chemin_signature, p.actif,
       u.code_utilisateur, u.login_utilisateur, u.nom_affichage, u.role_utilisateur, u.actif AS utilisateur_actif
FROM cab_praticiens p
LEFT JOIN sec_utilisateurs u ON u.id_utilisateur = p.id_utilisateur
WHERE p.id_cabinet = @idCabinet AND (p.actif = 1 OR @inactifs = 1)
ORDER BY p.nom, p.prenom;
"
        End Get
    End Property

    ' Le lien vers le compte est volontairement hors de UpdatePraticien : il se modifie
    ' uniquement par l'action explicite « Créer / Lier un compte ».
    Public ReadOnly Property LierUtilisateurPraticien As String
        Get
            Return "UPDATE cab_praticiens SET id_utilisateur = @idUtilisateur WHERE id_praticien = @id;"
        End Get
    End Property

    ' Comptes SuperUser actifs non encore liés à un praticien (candidats à la liaison).
    Public ReadOnly Property SelectUtilisateursLiables As String
        Get
            Return "
SELECT u.id_utilisateur, u.code_utilisateur, u.login_utilisateur, u.nom_affichage, u.role_utilisateur
FROM sec_utilisateurs u
WHERE u.actif = 1 AND u.role_utilisateur = 'SuperUser'
  AND NOT EXISTS (SELECT 1 FROM cab_praticiens p WHERE p.id_utilisateur = u.id_utilisateur)
ORDER BY u.nom_affichage;
"
        End Get
    End Property

    Public ReadOnly Property SelectPraticienParUtilisateur As String
        Get
            Return "SELECT id_praticien, nom, prenom, titre_professionnel, chemin_photo FROM cab_praticiens WHERE id_utilisateur = @idUtilisateur LIMIT 1;"
        End Get
    End Property

    Public ReadOnly Property InsertPraticien As String
        Get
            Return "
INSERT INTO cab_praticiens (id_cabinet, id_civilite, nom, prenom, titre_professionnel, telephone, email,
    site_web, adresse_rue, adresse_numero, code_postal, localite, id_pays,
    chemin_photo, chemin_cachet, chemin_signature, actif)
VALUES (@idCabinet, @idCivilite, @nom, @prenom, @titre, @telephone, @email,
    @site, @rue, @numero, @cp, @localite, @idPays,
    @photo, @cachet, @signature, @actif);
SELECT LAST_INSERT_ID();
"
        End Get
    End Property

    Public ReadOnly Property UpdatePraticien As String
        Get
            Return "
UPDATE cab_praticiens SET
    id_civilite = @idCivilite, nom = @nom, prenom = @prenom, titre_professionnel = @titre,
    telephone = @telephone, email = @email, site_web = @site, adresse_rue = @rue,
    adresse_numero = @numero, code_postal = @cp, localite = @localite, id_pays = @idPays,
    chemin_photo = @photo, chemin_cachet = @cachet, chemin_signature = @signature,
    actif = @actif
WHERE id_praticien = @id;
"
        End Get
    End Property

    Public ReadOnly Property UpdateImagesPraticien As String
        Get
            Return "
UPDATE cab_praticiens SET
    chemin_photo = @photo, chemin_cachet = @cachet, chemin_signature = @signature
WHERE id_praticien = @id;
"
        End Get
    End Property

#End Region

#Region "Identifiants"

    Public ReadOnly Property SelectIdentifiantsPraticien As String
        Get
            Return "
SELECT i.id_identifiant, i.id_praticien, i.id_type_identifiant, t.libelle_type_identifiant, t.code_type_identifiant,
       i.valeur, i.date_debut, i.date_fin, i.afficher_sur_cachet
FROM cab_praticien_identifiants i
INNER JOIN ref_types_identifiant_pro t ON t.id_type_identifiant = i.id_type_identifiant
WHERE i.id_praticien = @idPraticien
ORDER BY t.ordre_affichage, i.date_debut;
"
        End Get
    End Property

    Public ReadOnly Property InsertIdentifiant As String
        Get
            Return "
INSERT INTO cab_praticien_identifiants (id_praticien, id_type_identifiant, valeur, date_debut, date_fin, afficher_sur_cachet)
VALUES (@idPraticien, @idType, @valeur, @debut, @fin, @cachet);
"
        End Get
    End Property

    Public ReadOnly Property UpdateIdentifiant As String
        Get
            Return "
UPDATE cab_praticien_identifiants SET
    id_type_identifiant = @idType, valeur = @valeur, date_debut = @debut, date_fin = @fin,
    afficher_sur_cachet = @cachet
WHERE id_identifiant = @id;
"
        End Get
    End Property

    Public ReadOnly Property DeleteIdentifiant As String
        Get
            Return "DELETE FROM cab_praticien_identifiants WHERE id_identifiant = @id;"
        End Get
    End Property

#End Region

#Region "Qualifications"

    Public ReadOnly Property SelectQualificationsPraticien As String
        Get
            Return "
SELECT id_qualification, id_praticien, libelle, etablissement, annee_obtention,
       afficher_sur_cachet, ordre_affichage
FROM cab_praticien_qualifications
WHERE id_praticien = @idPraticien
ORDER BY ordre_affichage, libelle;
"
        End Get
    End Property

    Public ReadOnly Property InsertQualification As String
        Get
            Return "
INSERT INTO cab_praticien_qualifications (id_praticien, libelle, etablissement, annee_obtention, afficher_sur_cachet, ordre_affichage)
VALUES (@idPraticien, @libelle, @etablissement, @annee, @cachet, @ordre);
"
        End Get
    End Property

    Public ReadOnly Property UpdateQualification As String
        Get
            Return "
UPDATE cab_praticien_qualifications SET
    libelle = @libelle, etablissement = @etablissement, annee_obtention = @annee,
    afficher_sur_cachet = @cachet, ordre_affichage = @ordre
WHERE id_qualification = @id;
"
        End Get
    End Property

    Public ReadOnly Property DeleteQualification As String
        Get
            Return "DELETE FROM cab_praticien_qualifications WHERE id_qualification = @id;"
        End Get
    End Property

#End Region

End Module
