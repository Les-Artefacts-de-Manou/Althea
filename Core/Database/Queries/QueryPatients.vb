' -------------------------------------------------------------------------------------------------
' Module      : QueryPatients
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 11/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table patients.
'
' Responsabilités :
' - Fournir des propriétés/fonctions ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner la liste des patients (avec nombre de dossiers) et un patient par identifiant
' - Insérer, mettre à jour et supprimer un patient
' - Vérifier l'unicité du NISS et détecter les doublons (nom + prénom + date de naissance)
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_patient et code_patient sont générés par la base : jamais fournis par l'application
' - L'alerte est stockée en double format (alerte_rtf + alerte_txt) ; la recherche utilise alerte_txt
' - L'anamnèse est stockée en double format (anamnese_rtf + anamnese_txt) ; la recherche utilise anamnese_txt
'
' Dépendances :
' - Utilisé par GestionPatients pour les opérations CRUD sur patients
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryPatients

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectPatientsListe
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour charger la liste des patients (vue d'accueil / recherche)
    '
    ' Rôle       :
    ' Retourne les patients avec leur nombre total de dossiers et de dossiers actifs,
    ' triés par date de modification décroissante (les plus récemment touchés en premier).
    '
    ' Paramètres SQL :
    ' - Aucun
    '
    ' Utilisé par :
    ' - GestionPatients.GetPatients
    '
    ' Remarques  :
    ' - Le comptage des dossiers actifs s'appuie sur ref_statuts_dossier.code_statut_dossier <> 'ARCHIVE'
    '   et <> 'CLOTURE' (un dossier « vivant »)
    ' - LEFT JOIN pour inclure les patients sans dossier
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectPatientsListe As String
        Get
            Return "
SELECT
    p.id_patient,
    p.code_patient,
    p.nom,
    p.prenom,
    p.date_naissance,
    p.niss,
    p.telephone,
    p.email,
    p.alerte_txt,
    p.photo_fichier,
    p.suivi_en_cours,
    p.date_creation,
    p.date_modification,
    COUNT(d.id_dossier) AS nb_dossiers,
    SUM(
        CASE
            WHEN sd.code_statut_dossier NOT IN ('CLOTURE', 'ARCHIVE') THEN 1
            ELSE 0
        END
    ) AS nb_dossiers_actifs
FROM patients p
LEFT JOIN dossiers d ON d.id_patient = p.id_patient
LEFT JOIN ref_statuts_dossier sd ON sd.id_statut_dossier = d.id_statut_dossier
GROUP BY
    p.id_patient,
    p.code_patient,
    p.nom,
    p.prenom,
    p.date_naissance,
    p.niss,
    p.telephone,
    p.email,
    p.alerte_txt,
    p.photo_fichier,
    p.suivi_en_cours,
    p.date_creation,
    p.date_modification
ORDER BY p.date_modification DESC;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectPatientById
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour charger un patient complet depuis son identifiant
    '
    ' Rôle       :
    ' Retourne toutes les colonnes d'un patient pour l'affichage/édition de la fiche.
    '
    ' Paramètres SQL :
    ' - @id_patient : Identifiant du patient à charger
    '
    ' Utilisé par :
    ' - GestionPatients.GetPatientById
    '
    ' Remarques  :
    ' - LIMIT 1 pour garantir un seul résultat
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectPatientById As String
        Get
            Return "
SELECT
    id_patient,
    code_patient,
    nom,
    prenom,
    date_naissance,
    niss,
    lateralite,
    adresse_ligne1,
    adresse_ligne2,
    code_postal,
    localite,
    pays,
    telephone,
    email,
    mutualite,
    id_situation_familiale,
    photo_fichier,
    alerte_rtf,
    alerte_txt,
    anamnese_rtf,
    anamnese_txt,
    date_creation,
    date_modification
FROM patients
WHERE id_patient = @id_patient
LIMIT 1;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountNiss
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour vérifier l'unicité du NISS
    '
    ' Rôle       :
    ' Compte les patients possédant un NISS donné, en excluant un identifiant (utile en modification).
    '
    ' Paramètres SQL :
    ' - @niss       : NISS à vérifier
    ' - @id_patient : Identifiant du patient à exclure (0 en création)
    '
    ' Utilisé par :
    ' - GestionPatients.NissExiste
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountNiss As String
        Get
            Return "
SELECT COUNT(*)
FROM patients
WHERE niss = @niss
  AND id_patient <> @id_patient;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountDoublon
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour détecter un doublon potentiel de patient
    '
    ' Rôle       :
    ' Compte les patients ayant les mêmes nom, prénom et date de naissance, en excluant un identifiant.
    '
    ' Paramètres SQL :
    ' - @nom            : Nom à vérifier
    ' - @prenom         : Prénom à vérifier
    ' - @date_naissance : Date de naissance à vérifier
    ' - @id_patient     : Identifiant du patient à exclure (0 en création)
    '
    ' Utilisé par :
    ' - GestionPatients.DoublonExiste
    '
    ' Remarques  :
    ' - Comparaison insensible à la casse via la collation par défaut (utf8mb4_uca1400_ai_ci)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountDoublon As String
        Get
            Return "
SELECT COUNT(*)
FROM patients
WHERE nom = @nom
  AND prenom = @prenom
  AND date_naissance <=> @date_naissance
  AND id_patient <> @id_patient;
"
        End Get
    End Property

#End Region

#Region "INSERT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : InsertPatient
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour insérer un nouveau patient
    '
    ' Rôle       :
    ' Insère une nouvelle ligne dans patients ; id_patient et code_patient sont générés par la base.
    '
    ' Paramètres SQL :
    ' - @nom, @prenom, @date_naissance, @niss, @lateralite,
    '   @adresse_ligne1, @adresse_ligne2, @code_postal, @localite, @pays,
    '   @telephone, @email, @mutualite, @id_situation_familiale,
    '   @photo_fichier, @alerte_rtf, @alerte_txt, @anamnese_rtf, @anamnese_txt
    '
    ' Utilisé par :
    ' - GestionPatients.CreatePatient
    '
    ' Remarques  :
    ' - date_creation et date_modification sont gérées par la base (DEFAULT current_timestamp())
    ' - L'ID généré est récupéré séparément via SELECT LASTVAL(seq_patients)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertPatient As String
        Get
            Return "
INSERT INTO patients (
    nom,
    prenom,
    date_naissance,
    niss,
    lateralite,
    adresse_ligne1,
    adresse_ligne2,
    code_postal,
    localite,
    pays,
    telephone,
    email,
    mutualite,
    id_situation_familiale,
    photo_fichier,
    alerte_rtf,
    alerte_txt,
    anamnese_rtf,
    anamnese_txt
) VALUES (
    @nom,
    @prenom,
    @date_naissance,
    @niss,
    @lateralite,
    @adresse_ligne1,
    @adresse_ligne2,
    @code_postal,
    @localite,
    @pays,
    @telephone,
    @email,
    @mutualite,
    @id_situation_familiale,
    @photo_fichier,
    @alerte_rtf,
    @alerte_txt,
    @anamnese_rtf,
    @anamnese_txt
);
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdatePatient
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour mettre à jour un patient existant
    '
    ' Rôle       :
    ' Met à jour toutes les colonnes éditables d'un patient identifié par @id_patient.
    '
    ' Paramètres SQL :
    ' - Les mêmes que InsertPatient, plus @id_patient
    '
    ' Utilisé par :
    ' - GestionPatients.UpdatePatient
    '
    ' Remarques  :
    ' - Ne modifie ni id_patient ni code_patient
    ' - date_modification est gérée par la base (ON UPDATE current_timestamp())
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdatePatient As String
        Get
            Return "
UPDATE patients
SET
    nom = @nom,
    prenom = @prenom,
    date_naissance = @date_naissance,
    niss = @niss,
    lateralite = @lateralite,
    adresse_ligne1 = @adresse_ligne1,
    adresse_ligne2 = @adresse_ligne2,
    code_postal = @code_postal,
    localite = @localite,
    pays = @pays,
    telephone = @telephone,
    email = @email,
    mutualite = @mutualite,
    id_situation_familiale = @id_situation_familiale,
    photo_fichier = @photo_fichier,
    alerte_rtf = @alerte_rtf,
    alerte_txt = @alerte_txt,
    anamnese_rtf = @anamnese_rtf,
    anamnese_txt = @anamnese_txt
WHERE id_patient = @id_patient;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdatePhotoPatient
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour mettre à jour uniquement la photo d'un patient
    '
    ' Rôle       :
    ' Met à jour le nom du fichier photo (ou le met à NULL si la photo est retirée).
    '
    ' Paramètres SQL :
    ' - @photo_fichier : Nom du fichier photo (ou NULL)
    ' - @id_patient    : Identifiant du patient
    '
    ' Utilisé par :
    ' - GestionPatients.UpdatePhotoPatient
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdatePhotoPatient As String
        Get
            Return "
UPDATE patients
SET photo_fichier = @photo_fichier
WHERE id_patient = @id_patient;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateAnamnesePatient
    ' Version    : V1.0.0
    ' Date       : 18/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour mettre à jour uniquement l'anamnèse d'un patient
    '
    ' Rôle       :
    ' Met à jour les colonnes d'anamnèse (RTF + texte brut) sans toucher aux autres champs du patient,
    ' afin de permettre une édition/enregistrement localisé depuis l'onglet Anamnèse de la fiche.
    '
    ' Paramètres SQL :
    ' - @anamnese_rtf : Contenu RTF de l'anamnèse (ou NULL)
    ' - @anamnese_txt : Texte brut de l'anamnèse (ou NULL)
    ' - @id_patient   : Identifiant du patient
    '
    ' Utilisé par :
    ' - GestionPatients.UpdateAnamnesePatient
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateAnamnesePatient As String
        Get
            Return "
UPDATE patients
SET
    anamnese_rtf = @anamnese_rtf,
    anamnese_txt = @anamnese_txt
WHERE id_patient = @id_patient;
"
        End Get
    End Property

#End Region

#Region "DELETE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DeletePatient
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour supprimer physiquement un patient
    '
    ' Rôle       :
    ' Supprime un patient identifié par @id_patient.
    '
    ' Paramètres SQL :
    ' - @id_patient : Identifiant du patient à supprimer
    '
    ' Utilisé par :
    ' - GestionPatients.DeletePatient
    '
    ' Remarques  :
    ' - La suppression n'est autorisée que si le patient n'a aucune dépendance (dossiers, etc.) ;
    '   cette vérification est effectuée en amont par GestionPatients
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DeletePatient As String
        Get
            Return "
DELETE FROM patients
WHERE id_patient = @id_patient;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountDossiersDuPatient
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour compter les dossiers liés à un patient
    '
    ' Rôle       :
    ' Détermine si un patient possède des dossiers (sécurité avant suppression physique).
    '
    ' Paramètres SQL :
    ' - @id_patient : Identifiant du patient
    '
    ' Utilisé par :
    ' - GestionPatients.PeutSupprimerPatient
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountDossiersDuPatient As String
        Get
            Return "
SELECT COUNT(*)
FROM dossiers
WHERE id_patient = @id_patient;
"
        End Get
    End Property

#End Region

End Module
