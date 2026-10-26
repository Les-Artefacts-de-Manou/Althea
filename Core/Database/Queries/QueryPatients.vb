' -------------------------------------------------------------------------------------------------
' Module      : QueryPatients
' Projet      : Althéa
' Version     : V1.11.1
' Date        : 24/10/2026
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
'
' Historique  :
' - V1.2.0 (17/10/2026) : montant_avoir compte aussi l'encaissement resté sur une séance reportée.
' - V1.3.0 (17/10/2026) : montant_avoir compte aussi le trop-perçu d'une séance encore due.
' - V1.4.0 (17/10/2026) : montant_du suit la règle des grilles et du bandeau (séance due et passée,
'   heure de fin comprise).
' - V1.5.0 (17/10/2026) : la liste des patients donne aussi le prochain rendez-vous, la dernière
'   séance où le patient a été vu, le nombre de séances du jour et le nombre de dossiers au statut
'   Actif (grille Patients, hub de la journée — étape 1).
' - V1.7.0 (20/10/2026, v2.46.0) : id_lateralite (référentiel) à la place de la colonne texte lateralite.
' - V1.8.0 (21/10/2026, audit B.4, v2.49.0) : SelectPatientsListe lit soldes, « passée » et dossier
'   dominant dans les vues v_seances_solde et v_dossier_dominant.
' - V1.9.0 (22/10/2026, audit B.6, v2.53.0) : absences et « dernière séance » reconnues par la case
'   ref_statuts_seance.est_absence au lieu du code 'ABSENTE'.
' - V1.10.0 (23/10/2026, relevé de Joëlle) : SelectCountDoublon tient compte d'une date de naissance
'   inconnue.
' - V1.11.0 (24/10/2026, recherche R4) : la liste des patients donne aussi code postal, localité
'   et pays (recherche des patients par lieu).
' - V1.6.0 (17/10/2026) : identifiant, code et dossier du prochain rendez-vous et de la dernière
'   séance (ouverture directe de la séance depuis la grille Patients).
' - V1.11.1 (24/10/2026, audit C.3) : statuts de dossier écrits dans le SQL depuis
'   CodesSysteme.StatutsDossier, plus en clair.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryPatients

    ' Codes écrits dans le SQL : ceux de CodesSysteme (audit C.3), jamais une saisie — la
    ' chaîne est bâtie sur une constante du programme.
    Private Const DOSSIER_CLOTURE As String = CodesSysteme.StatutsDossier.Cloture
    Private Const DOSSIER_ARCHIVE As String = CodesSysteme.StatutsDossier.Archive
    Private Const DOSSIER_ACTIF As String = CodesSysteme.StatutsDossier.Actif

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectPatientsListe
    ' Version    : V2.7.0
    ' Date       : 24/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour charger la liste des patients (vue d'accueil / recherche)
    '
    ' Rôle       :
    ' Retourne les patients avec leur nombre total de dossiers et de dossiers actifs, ainsi que deux
    ' indicateurs de suivi financier affichés en grille : le montant réellement exigible et le nombre
    ' d'absences non prévenues (no show), qui sert d'alerte comportementale sur le patient.
    '
    ' Paramètres SQL :
    ' - Aucun
    '
    ' Utilisé par :
    ' - GestionPatients.GetPatients
    '
    ' Remarques  :
    ' - Le comptage des dossiers actifs s'appuie sur ref_statuts_dossier.code_statut_dossier <> 'ARCHIVE'
    '   et <> 'CLOTURE' (un dossier « vivant ») : codes de CodesSysteme.StatutsDossier, insérés dans
    '   le texte SQL (audit C.3).
    ' - LEFT JOIN pour inclure les patients sans dossier
    ' - montant_du ne retient que les séances au statut dû ET passées : une séance à venir n'est pas
    '   encore une créance. « Passée » suit SeanceListeItem.EstPassee (V2.3.0) : heure de fin, sinon
    '   heure de début, dépassée ; sans heure, la séance compte à partir du lendemain. Même règle que
    '   MontantExigible (grilles, bandeau du dossier).
    ' - Tous les dossiers du patient sont comptés (montant_du, montant_avoir, nb_no_show).
    ' - GREATEST(..., 0) neutralise un éventuel trop-perçu pour qu'il ne vienne pas masquer une dette
    '   sur une autre séance
    ' - code_domaine_dominant identifie le dossier « courant » du patient : on privilégie un dossier
    '   vivant, puis celui dont la séance est la plus proche de la date du jour (passée ou à venir),
    '   et à défaut le dernier dossier ouvert. Affiché en grille, il évite d'ouvrir la fiche pour
    '   savoir dans quel domaine le patient est suivi.
    ' - montant_avoir : par séance, tout le réglé d'une séance non due (reportées comprises, V2.1.0)
    '   ou le trop-perçu d'une séance due (réglé − montant, V2.2.0) — même définition que
    '   SeanceListeItem.AvoirSeance et que les avoirs réutilisables (QueryPaiements).
    ' - V2.4.0 — rendez-vous (séances au statut dû, tous dossiers du patient, même règle « passée »
    '   que montant_du) :
    '   · prochaine_seance_* : la première séance pas encore passée (date, puis heure de début) ;
    '   · derniere_seance_* : la dernière séance passée où le patient a été vu — une absence non
    '     excusée (ABSENTE) n'en est pas une, elle a sa propre colonne (décision du 17/10/2026) ;
    '   · nb_seances_jour : séances du jour, passées ou non (absence comprise : le patient était attendu) ;
    '   · nb_dossiers_statut_actif : dossiers au statut Actif seulement (un dossier en pause n'appelle
    '     pas de rendez-vous) — sert à signaler un suivi actif sans rendez-vous à venir.
    '   Une table dérivée par indicateur, une ligne par patient (ROW_NUMBER), plutôt qu'une
    '   sous-requête corrélée par colonne.
    ' - V2.5.0 : *_seance_id, *_seance_code et *_seance_dossier désignent ces deux séances, pour les
    '   ouvrir d'un double-clic depuis la grille Patients.
    ' - V2.6.0 (21/10/2026, audit B.4, migration v2.49.0) : une seule définition, en base :
    '   · montant_du, montant_avoir et « passée » lus dans la vue v_seances_solde (reste_du,
    '     montant_avoir, est_echue), une table dérivée par patient ;
    '   · statut et domaine du dossier dominant lus dans la vue v_dossier_dominant (décision de
    '     Joëlle : dossier vivant, puis séance la plus proche d'aujourd'hui) — ils pouvaient venir de
    '     deux dossiers différents.
    ' - V2.7.0 (24/10/2026, recherche R4) : code_postal, localite et code_pays (patients par lieu).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectPatientsListe As String
        Get
            Return $"
SELECT
    p.id_patient,
    p.code_patient,
    p.nom,
    p.prenom,
    p.date_naissance,
    p.niss,
    p.telephone,
    p.email,
    p.code_postal,
    p.localite,
    rpays.code_pays,
    p.alerte_txt,
    p.photo_fichier,
    p.is_actif,
    p.date_activation,
    p.date_desactivation,
    p.date_creation,
    p.date_modification,
    COUNT(d.id_dossier) AS nb_dossiers,
    SUM(
        CASE
            WHEN sd.code_statut_dossier NOT IN ('{DOSSIER_CLOTURE}', '{DOSSIER_ARCHIVE}') THEN 1
            ELSE 0
        END
    ) AS nb_dossiers_actifs,
    sddom.code_statut_dossier AS code_statut_dernier_dossier,
    sddom.libelle_statut_dossier AS libelle_statut_dernier_dossier,
    domdom.code_domaine AS code_domaine_dominant,
    COALESCE(soldes.montant_du, 0) AS montant_du,
    COALESCE(soldes.montant_avoir, 0) AS montant_avoir,
    (
        SELECT COUNT(*)
        FROM seances s4
        INNER JOIN dossiers d4 ON d4.id_dossier = s4.id_dossier
        INNER JOIN ref_statuts_seance ss4 ON ss4.id_statut_seance = s4.id_statut_seance
        WHERE d4.id_patient = p.id_patient
          AND ss4.est_absence = 1
    ) AS nb_no_show,
    SUM(
        CASE
            WHEN sd.code_statut_dossier = '{DOSSIER_ACTIF}' THEN 1
            ELSE 0
        END
    ) AS nb_dossiers_statut_actif,
    sp.date_seance AS prochaine_seance_date,
    sp.heure_debut AS prochaine_seance_heure,
    sp.id_seance AS prochaine_seance_id,
    sp.code_seance AS prochaine_seance_code,
    sp.id_dossier AS prochaine_seance_dossier,
    sv.date_seance AS derniere_seance_date,
    sv.heure_debut AS derniere_seance_heure,
    sv.id_seance AS derniere_seance_id,
    sv.code_seance AS derniere_seance_code,
    sv.id_dossier AS derniere_seance_dossier,
    COALESCE(sj.nb_seances_jour, 0) AS nb_seances_jour
FROM patients p
LEFT JOIN ref_pays rpays ON rpays.id_pays = p.id_pays
LEFT JOIN dossiers d ON d.id_patient = p.id_patient
LEFT JOIN ref_statuts_dossier sd ON sd.id_statut_dossier = d.id_statut_dossier
LEFT JOIN v_dossier_dominant vdd ON vdd.id_patient = p.id_patient
LEFT JOIN dossiers ddom ON ddom.id_dossier = vdd.id_dossier
LEFT JOIN ref_statuts_dossier sddom ON sddom.id_statut_dossier = ddom.id_statut_dossier
LEFT JOIN ref_domaines domdom ON domdom.id_domaine = ddom.id_domaine
LEFT JOIN (
    SELECT d3.id_patient,
           SUM(CASE WHEN vs3.est_echue = 1 THEN vs3.reste_du ELSE 0 END) AS montant_du,
           SUM(vs3.montant_avoir) AS montant_avoir
    FROM v_seances_solde vs3
    INNER JOIN dossiers d3 ON d3.id_dossier = vs3.id_dossier
    GROUP BY d3.id_patient
) soldes ON soldes.id_patient = p.id_patient
LEFT JOIN (
    SELECT x7.id_patient, x7.date_seance, x7.heure_debut, x7.id_seance, x7.code_seance, x7.id_dossier
    FROM (
        SELECT d7.id_patient, s7.date_seance, s7.heure_debut, s7.id_seance, s7.code_seance, s7.id_dossier,
               ROW_NUMBER() OVER (PARTITION BY d7.id_patient
                                  ORDER BY s7.date_seance, s7.heure_debut, s7.id_seance) AS rang
        FROM seances s7
        INNER JOIN dossiers d7 ON d7.id_dossier = s7.id_dossier
        INNER JOIN ref_statuts_seance ss7 ON ss7.id_statut_seance = s7.id_statut_seance
        INNER JOIN v_seances_solde vs7 ON vs7.id_seance = s7.id_seance
        WHERE ss7.est_du = 1
          AND vs7.est_echue = 0
    ) x7
    WHERE x7.rang = 1
) sp ON sp.id_patient = p.id_patient
LEFT JOIN (
    SELECT x8.id_patient, x8.date_seance, x8.heure_debut, x8.id_seance, x8.code_seance, x8.id_dossier
    FROM (
        SELECT d8.id_patient, s8.date_seance, s8.heure_debut, s8.id_seance, s8.code_seance, s8.id_dossier,
               ROW_NUMBER() OVER (PARTITION BY d8.id_patient
                                  ORDER BY s8.date_seance DESC, s8.heure_debut DESC, s8.id_seance DESC) AS rang
        FROM seances s8
        INNER JOIN dossiers d8 ON d8.id_dossier = s8.id_dossier
        INNER JOIN ref_statuts_seance ss8 ON ss8.id_statut_seance = s8.id_statut_seance
        INNER JOIN v_seances_solde vs8 ON vs8.id_seance = s8.id_seance
        WHERE ss8.est_du = 1
          AND ss8.est_absence = 0
          AND vs8.est_echue = 1
    ) x8
    WHERE x8.rang = 1
) sv ON sv.id_patient = p.id_patient
LEFT JOIN (
    SELECT d9.id_patient, COUNT(*) AS nb_seances_jour
    FROM seances s9
    INNER JOIN dossiers d9 ON d9.id_dossier = s9.id_dossier
    INNER JOIN ref_statuts_seance ss9 ON ss9.id_statut_seance = s9.id_statut_seance
    WHERE ss9.est_du = 1
      AND s9.date_seance = CURDATE()
    GROUP BY d9.id_patient
) sj ON sj.id_patient = p.id_patient
GROUP BY
    p.id_patient,
    p.code_patient,
    p.nom,
    p.prenom,
    p.date_naissance,
    p.niss,
    p.telephone,
    p.email,
    p.code_postal,
    p.localite,
    rpays.code_pays,
    p.alerte_txt,
    p.photo_fichier,
    p.is_actif,
    p.date_activation,
    p.date_desactivation,
    p.date_creation,
    p.date_modification,
    sddom.code_statut_dossier,
    sddom.libelle_statut_dossier,
    domdom.code_domaine,
    soldes.montant_du,
    soldes.montant_avoir,
    sp.date_seance,
    sp.heure_debut,
    sp.id_seance,
    sp.code_seance,
    sp.id_dossier,
    sv.date_seance,
    sv.heure_debut,
    sv.id_seance,
    sv.code_seance,
    sv.id_dossier,
    sj.nb_seances_jour
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
    id_lateralite,
    adresse_ligne1,
    adresse_ligne2,
    code_postal,
    localite,
    id_pays,
    (SELECT rp.code_pays FROM ref_pays rp WHERE rp.id_pays = patients.id_pays) AS code_pays,
    id_civilite,
    telephone,
    email,
    id_organisme_mutualite,
    numero_affiliation,
    code_titulaire_1,
    code_titulaire_2,
    id_situation_familiale,
    photo_fichier,
    alerte_rtf,
    alerte_txt,
    anamnese_rtf,
    anamnese_txt,
    is_actif,
    date_activation,
    date_desactivation,
    id_prix_seance_defaut,
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
    ' Version    : V1.1.0
    ' Date       : 23/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour détecter un doublon potentiel de patient
    '
    ' Rôle       :
    ' Compte les patients ayant les mêmes nom et prénom et une date de naissance compatible (la même,
    ' ou inconnue d'un côté ou de l'autre), en excluant un identifiant.
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
    ' - V1.1.0 (23/10/2026, relevé de Joëlle) : une date de naissance non renseignée (d'un côté ou de
    '   l'autre) ne cache plus l'homonyme ; seules deux dates connues et différentes les distinguent.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountDoublon As String
        Get
            Return "
SELECT COUNT(*)
FROM patients
WHERE nom = @nom
  AND prenom = @prenom
  AND (date_naissance <=> @date_naissance OR date_naissance IS NULL OR @date_naissance IS NULL)
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
    ' - @nom, @prenom, @date_naissance, @niss, @id_lateralite,
    '   @adresse_ligne1, @adresse_ligne2, @code_postal, @localite, @id_pays, @id_civilite,
    '   @telephone, @email, @id_organisme_mutualite, @numero_affiliation,
    '   @code_titulaire_1, @code_titulaire_2, @id_situation_familiale,
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
    id_lateralite,
    adresse_ligne1,
    adresse_ligne2,
    code_postal,
    localite,
    id_pays,
    id_civilite,
    telephone,
    email,
    id_organisme_mutualite,
    numero_affiliation,
    code_titulaire_1,
    code_titulaire_2,
    id_situation_familiale,
    id_prix_seance_defaut,
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
    @id_lateralite,
    @adresse_ligne1,
    @adresse_ligne2,
    @code_postal,
    @localite,
    @id_pays,
    @id_civilite,
    @telephone,
    @email,
    @id_organisme_mutualite,
    @numero_affiliation,
    @code_titulaire_1,
    @code_titulaire_2,
    @id_situation_familiale,
    @id_prix_seance_defaut,
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
    id_lateralite = @id_lateralite,
    adresse_ligne1 = @adresse_ligne1,
    adresse_ligne2 = @adresse_ligne2,
    code_postal = @code_postal,
    localite = @localite,
    id_pays = @id_pays,
    id_civilite = @id_civilite,
    telephone = @telephone,
    email = @email,
    id_organisme_mutualite = @id_organisme_mutualite,
    numero_affiliation = @numero_affiliation,
    code_titulaire_1 = @code_titulaire_1,
    code_titulaire_2 = @code_titulaire_2,
    id_situation_familiale = @id_situation_familiale,
    id_prix_seance_defaut = @id_prix_seance_defaut,
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

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateStatutActifPatient
    ' Version    : V1.0.0
    ' Date       : 14/07/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Active ou désactive manuellement un patient (is_actif) avec traçage des dates
    '
    ' Rôle       :
    ' Met à jour le flag d'activation manuel du patient, indépendant du cycle de vie des dossiers :
    ' - is_actif = 1 : renseigne date_activation (aujourd'hui) et remet date_desactivation à NULL
    ' - is_actif = 0 : renseigne date_desactivation (aujourd'hui) ; conserve date_activation
    '
    ' Paramètres SQL :
    ' - @is_actif   : Nouvel état d'activation (1 = actif, 0 = inactif)
    ' - @id_patient : Identifiant du patient
    '
    ' Utilisé par :
    ' - GestionPatients.ChangerStatutActif
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateStatutActifPatient As String
        Get
            Return "
UPDATE patients
SET
    is_actif = @is_actif,
    date_activation = CASE WHEN @is_actif = 1 THEN CURRENT_DATE() ELSE date_activation END,
    date_desactivation = CASE WHEN @is_actif = 1 THEN NULL ELSE CURRENT_DATE() END
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
