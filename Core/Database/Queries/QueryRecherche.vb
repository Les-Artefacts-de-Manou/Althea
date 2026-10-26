' -------------------------------------------------------------------------------------------------
' Module      : QueryRecherche
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Requêtes SQL de la recherche transversale (ADR-52) : lecture seule, paramétrées.
'
' Remarques   :
' - Contient uniquement du SQL ; aucun accès à la base ici (GestionRecherche).
' - Chaque requête rend les colonnes communes des résultats (code, id_patient, code_patient,
'   nom_patient, prenom_patient, id_dossier, code_dossier…) plus ses colonnes propres : un même
'   mapping sert toutes les recherches.
' - R1 (recherche par code) : un élément par identifiant ; le code est généré par la base
'   (préfixe + identifiant sur 6 chiffres), l'identifiant suffit donc à le retrouver.
' - V1.1.0 (24/10/2026, R5) : réseau des patients (contacts, intervenants, thérapeutes), trois
'   lectures sans paramètre (pas d'UNION : collations différentes entre tables) ; thérapeute par son
'   identifiant (code TH…, décision de Joëlle).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryRecherche

#Region "Par code (R1)"

    Public ReadOnly Property SelectPatientParId As String
        Get
            Return "
SELECT p.id_patient, p.code_patient, p.nom AS nom_patient, p.prenom AS prenom_patient,
       p.date_naissance, p.localite, p.is_actif
FROM patients p
WHERE p.id_patient = @id;"
        End Get
    End Property

    Public ReadOnly Property SelectDossierParId As String
        Get
            Return "
SELECT d.id_dossier, d.code_dossier, d.date_ouverture, d.date_cloture,
       rd.libelle_domaine, rsd.libelle_statut_dossier,
       p.id_patient, p.code_patient, p.nom AS nom_patient, p.prenom AS prenom_patient
FROM dossiers d
JOIN patients p ON p.id_patient = d.id_patient
LEFT JOIN ref_domaines rd ON rd.id_domaine = d.id_domaine
LEFT JOIN ref_statuts_dossier rsd ON rsd.id_statut_dossier = d.id_statut_dossier
WHERE d.id_dossier = @id;"
        End Get
    End Property

    Public ReadOnly Property SelectSeanceParId As String
        Get
            Return "
SELECT s.id_seance, s.code_seance, s.date_seance, s.heure_debut, s.tarif_seance,
       rss.libelle_statut_seance, rns.libelle_nature_seance,
       d.id_dossier, d.code_dossier, rd.libelle_domaine,
       p.id_patient, p.code_patient, p.nom AS nom_patient, p.prenom AS prenom_patient
FROM seances s
JOIN dossiers d ON d.id_dossier = s.id_dossier
JOIN patients p ON p.id_patient = d.id_patient
LEFT JOIN ref_domaines rd ON rd.id_domaine = d.id_domaine
LEFT JOIN ref_statuts_seance rss ON rss.id_statut_seance = s.id_statut_seance
LEFT JOIN ref_natures_seance rns ON rns.id_nature_seance = s.id_nature_seance
WHERE s.id_seance = @id;"
        End Get
    End Property

    ' Un paiement est affecté à une ou plusieurs séances (lia_paiements_seances) : la première sert
    ' à le rattacher à son dossier et à son patient (un règlement ne couvre qu'un dossier).
    Public ReadOnly Property SelectPaiementParId As String
        Get
            Return "
SELECT pa.id_paiement, pa.code_paiement, pa.date_paiement, pa.montant, pa.statut_paiement,
       pa.date_annulation, pa.commentaire, rmp.libelle_mode_paiement,
       s.id_seance, s.code_seance, s.date_seance,
       d.id_dossier, d.code_dossier,
       p.id_patient, p.code_patient, p.nom AS nom_patient, p.prenom AS prenom_patient
FROM paiements pa
LEFT JOIN ref_modes_paiement rmp ON rmp.id_mode_paiement = pa.id_mode_paiement
LEFT JOIN (SELECT id_paiement, MIN(id_seance) AS id_seance
           FROM lia_paiements_seances
           GROUP BY id_paiement) l ON l.id_paiement = pa.id_paiement
LEFT JOIN seances s ON s.id_seance = l.id_seance
LEFT JOIN dossiers d ON d.id_dossier = s.id_dossier
LEFT JOIN patients p ON p.id_patient = d.id_patient
WHERE pa.id_paiement = @id;"
        End Get
    End Property

    Public ReadOnly Property SelectContactParId As String
        Get
            Return "
SELECT fc.id_famille_contact, fc.code_famille_contact, fc.nom AS nom_contact, fc.prenom AS prenom_contact,
       rlp.libelle_lien_patient,
       p.id_patient, p.code_patient, p.nom AS nom_patient, p.prenom AS prenom_patient
FROM famille_contacts fc
JOIN patients p ON p.id_patient = fc.id_patient
LEFT JOIN ref_liens_patient rlp ON rlp.id_lien_patient = fc.id_lien_patient
WHERE fc.id_famille_contact = @id;"
        End Get
    End Property

    Public ReadOnly Property SelectIntervenantParId As String
        Get
            Return "
SELECT asp.id_autre_suivi_patient, asp.code_autre_suivi_patient, asp.nom_professionnel, asp.specialite,
       asp.date_debut, rri.libelle_role_intervenant,
       p.id_patient, p.code_patient, p.nom AS nom_patient, p.prenom AS prenom_patient
FROM autres_suivis_patient asp
JOIN patients p ON p.id_patient = asp.id_patient
LEFT JOIN ref_roles_intervenant rri ON rri.id_role_intervenant = asp.id_role_intervenant
WHERE asp.id_autre_suivi_patient = @id;"
        End Get
    End Property

    ' Thérapeute du référentiel (code TH…) ; pas de patient : colonnes communes à NULL.
    Public ReadOnly Property SelectTherapeuteParId As String
        Get
            Return "
SELECT t.id_therapeute, t.code_therapeute, t.nom, t.prenom, t.Actif, rs.libelle_specialite,
       NULL AS id_patient, NULL AS code_patient, NULL AS nom_patient, NULL AS prenom_patient
FROM therapeutes t
LEFT JOIN ref_specialites rs ON rs.id_specialite = t.id_specialite
WHERE t.id_therapeute = @id;"
        End Get
    End Property

#End Region

#Region "Réseau par nom (R5)"

    ' Contacts (famille / entourage) de tous les patients, avec leur lien et leur patient.
    Public ReadOnly Property SelectReseauContacts As String
        Get
            Return "
SELECT fc.id_famille_contact, fc.code_famille_contact, fc.nom AS nom_contact, fc.prenom AS prenom_contact,
       rlp.libelle_lien_patient,
       p.id_patient, p.code_patient, p.nom AS nom_patient, p.prenom AS prenom_patient, p.is_actif
FROM famille_contacts fc
JOIN patients p ON p.id_patient = fc.id_patient
LEFT JOIN ref_liens_patient rlp ON rlp.id_lien_patient = fc.id_lien_patient;"
        End Get
    End Property

    ' Intervenants de tous les patients : lié à un thérapeute du référentiel (id_therapeute) ou saisi
    ' dans la fiche (nom_professionnel), avec son rôle, sa période et son patient.
    Public ReadOnly Property SelectReseauIntervenants As String
        Get
            Return "
SELECT asp.id_autre_suivi_patient, asp.code_autre_suivi_patient, asp.id_therapeute,
       asp.nom_professionnel, asp.specialite, asp.date_debut, asp.date_fin,
       rri.libelle_role_intervenant,
       p.id_patient, p.code_patient, p.nom AS nom_patient, p.prenom AS prenom_patient, p.is_actif
FROM autres_suivis_patient asp
JOIN patients p ON p.id_patient = asp.id_patient
LEFT JOIN ref_roles_intervenant rri ON rri.id_role_intervenant = asp.id_role_intervenant;"
        End Get
    End Property

    ' Thérapeutes du référentiel, actifs ou non, avec leur spécialité.
    Public ReadOnly Property SelectReseauTherapeutes As String
        Get
            Return "
SELECT t.id_therapeute, t.code_therapeute, t.nom, t.prenom, t.Actif,
       rs.libelle_specialite
FROM therapeutes t
LEFT JOIN ref_specialites rs ON rs.id_specialite = t.id_specialite;"
        End Get
    End Property

#End Region

End Module
