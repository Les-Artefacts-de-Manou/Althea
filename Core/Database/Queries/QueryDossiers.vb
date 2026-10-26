' -------------------------------------------------------------------------------------------------
' Module      : QueryDossiers
' Projet      : Althéa
' Version     : V1.4.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table dossiers.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL de lecture des dossiers
' - Sélectionner la liste des dossiers d'un patient (avec libellés domaine/statut/référent)
' - Sélectionner un dossier complet par identifiant (avec les 7 zones cliniques RTF/TXT)
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_dossier et code_dossier sont générés par la base : jamais fournis par l'application
' - Les jointures sont en LEFT JOIN pour rester robustes même si un libellé venait à manquer
' - Périmètre B0 : lecture seule ; les requêtes INSERT/UPDATE arriveront en briques B3/B4
'
' - V1.1.0 (20/10/2026) : SelectDossierById lit le logo du domaine (bandeau).
' - V1.2.0 (21/10/2026, audit B.1) : SelectStatutDossierPourTransition ; UpdateStatutDossier n'écrit
'   que si le statut en base est encore celui qui a été validé.
' - V1.3.0 (21/10/2026, audit B.4, v2.49.0) : SelectIdDossierDominant (vue v_dossier_dominant).
' - V1.4.0 (22/10/2026, audit C.1) : SelectStatutDossierParSeance (dossier figé refusé par le métier).
'
' Dépendances :
' - Utilisé par GestionDossiers pour les opérations de lecture sur dossiers
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryDossiers

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectDossiersParPatient
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour charger la liste des dossiers d'un patient (onglet Dossiers)
    '
    ' Rôle       :
    ' Retourne les dossiers rattachés à un patient avec les libellés du domaine et du statut,
    ' ainsi que le nom du thérapeute référent, triés par date d'ouverture décroissante.
    '
    ' Paramètres SQL :
    ' - @id_patient : Identifiant du patient
    '
    ' Utilisé par :
    ' - GestionDossiers.GetDossiersParPatient
    '
    ' Remarques  :
    ' - Vue allégée (DossierListeItem) : n'inclut pas les colonnes RTF des zones cliniques
    ' - CONCAT_WS ignore les composants NULL du nom du référent
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectDossiersParPatient As String
        Get
            Return "
SELECT
    d.id_dossier,
    d.code_dossier,
    d.id_patient,
    d.id_domaine,
    dom.libelle_domaine,
    d.id_statut_dossier,
    sd.code_statut_dossier,
    sd.libelle_statut_dossier,
    d.date_ouverture,
    d.date_cloture,
    CONCAT_WS(' ', t.prenom, t.nom) AS therapeute_traitant,
    d.motif_suivi_txt,
    d.date_modification
FROM dossiers d
LEFT JOIN ref_domaines dom ON dom.id_domaine = d.id_domaine
LEFT JOIN ref_statuts_dossier sd ON sd.id_statut_dossier = d.id_statut_dossier
LEFT JOIN therapeutes t ON t.id_therapeute = d.id_therapeute_traitant
WHERE d.id_patient = @id_patient
ORDER BY d.date_ouverture DESC, d.id_dossier DESC;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectDossierById
    ' Version    : V1.1.0
    ' Date       : 20/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour charger un dossier complet depuis son identifiant
    '
    ' Rôle       :
    ' Retourne toutes les colonnes d'un dossier (identité + 7 zones cliniques RTF/TXT) pour
    ' l'affichage/édition du hub dossier, enrichies des libellés domaine/statut et du référent.
    '
    ' Paramètres SQL :
    ' - @id_dossier : Identifiant du dossier à charger
    '
    ' Utilisé par :
    ' - GestionDossiers.GetDossier
    '
    ' Remarques  :
    ' - LIMIT 1 pour garantir un seul résultat
    ' - V1.1.0 (20/10/2026) : logo du domaine (logo_domaine).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectDossierById As String
        Get
            Return "
SELECT
    d.id_dossier,
    d.code_dossier,
    d.id_patient,
    d.id_domaine,
    dom.libelle_domaine,
    dom.logo_fichier AS logo_domaine,
    d.id_statut_dossier,
    sd.code_statut_dossier,
    sd.libelle_statut_dossier,
    d.date_ouverture,
    d.date_cloture,
    d.date_archivage,
    d.modalites_paiement,
    d.id_therapeute_traitant,
    CONCAT_WS(' ', t.prenom, t.nom) AS therapeute_traitant,
    d.motif_suivi_rtf,
    d.motif_suivi_txt,
    d.historique_rtf,
    d.historique_txt,
    d.antecedents_rtf,
    d.antecedents_txt,
    d.diagnostic_rtf,
    d.diagnostic_txt,
    d.traitements_en_cours_rtf,
    d.traitements_en_cours_txt,
    d.objectifs_suivi_rtf,
    d.objectifs_suivi_txt,
    d.historique_archive_rtf,
    d.historique_archive_txt,
    d.date_creation,
    d.date_modification
FROM dossiers d
LEFT JOIN ref_domaines dom ON dom.id_domaine = d.id_domaine
LEFT JOIN ref_statuts_dossier sd ON sd.id_statut_dossier = d.id_statut_dossier
LEFT JOIN therapeutes t ON t.id_therapeute = d.id_therapeute_traitant
WHERE d.id_dossier = @id_dossier
LIMIT 1;
"
        End Get
    End Property

#End Region

#Region "INSERT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : InsertDossier
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour insérer un nouveau dossier
    '
    ' Rôle       :
    ' Insère une nouvelle ligne dans dossiers ; id_dossier et code_dossier sont générés par la base
    ' (seq_dossiers + colonne GENERATED). Le statut initial est fourni par l'appelant (forcé Actif).
    '
    ' Paramètres SQL :
    ' - @id_patient, @id_domaine, @id_statut_dossier, @date_ouverture, @modalites_paiement
    '
    ' Utilisé par :
    ' - GestionDossiers.CreateDossier
    '
    ' Remarques  :
    ' - id_dossier et code_dossier sont générés par la base : jamais fournis par l'application
    ' - id_therapeute_traitant volontairement non renseigné (reste NULL) — retiré de l'UI (D-DOS)
    ' - date_creation et date_modification sont gérées par la base (DEFAULT current_timestamp())
    ' - L'ID généré est récupéré séparément via SELECT LASTVAL(seq_dossiers)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertDossier As String
        Get
            Return "
INSERT INTO dossiers (
    id_patient,
    id_domaine,
    id_statut_dossier,
    date_ouverture,
    modalites_paiement
) VALUES (
    @id_patient,
    @id_domaine,
    @id_statut_dossier,
    @date_ouverture,
    @modalites_paiement
);
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateDossier
    ' Version    : V1.0.0
    ' Date       : 21/07/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour mettre à jour l'identité d'un dossier existant
    '
    ' Rôle       :
    ' Met à jour les colonnes éditables d'identité (domaine, date d'ouverture, modalités) d'un
    ' dossier identifié par @id_dossier.
    '
    ' Paramètres SQL :
    ' - @id_domaine, @date_ouverture, @modalites_paiement, @id_dossier
    '
    ' Utilisé par :
    ' - GestionDossiers.UpdateDossier
    '
    ' Remarques  :
    ' - Ne modifie ni id_dossier, ni code_dossier, ni id_patient
    ' - Le statut n'est pas modifié ici : les transitions de cycle de vie relèvent de la brique B6
    ' - date_modification est mise à jour explicitement (la table n'a pas de ON UPDATE)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateDossier As String
        Get
            Return "
UPDATE dossiers
SET
    id_domaine = @id_domaine,
    date_ouverture = @date_ouverture,
    modalites_paiement = @modalites_paiement,
    date_modification = current_timestamp()
WHERE id_dossier = @id_dossier;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : SelectIdDossierDominant
    ' Version    : V1.0.0
    ' Date       : 21/10/2026
    '
    ' Type       : String
    '
    ' Description : Dossier dominant d'un patient (celui que l'on montre comme son suivi).
    '
    ' Rôle       :
    ' Lit la vue v_dossier_dominant (migration v2.49.0) : dossier vivant (actif ou en pause) d'abord,
    ' puis celui dont une séance est la plus proche d'aujourd'hui, puis le plus récemment ouvert
    ' (décision de Joëlle, 21/10/2026). Même règle que la grille Patients.
    '
    ' Paramètres SQL :
    ' - @id_patient : Identifiant du patient
    '
    ' Utilisé par :
    ' - GestionDossiers.ChoisirDossierDominant
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectIdDossierDominant As String
        Get
            Return "
SELECT id_dossier
FROM v_dossier_dominant
WHERE id_patient = @id_patient;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : SelectStatutDossierPourTransition
    ' Version    : V1.0.0
    ' Date       : 21/10/2026
    '
    ' Type       : String
    '
    ' Description : Statut actuel d'un dossier, relu avant une transition.
    '
    ' Rôle       :
    ' Fournit le statut réellement en base (identifiant et code) et la date de clôture : la transition
    ' est validée sur ce statut, et non sur celui affiché par l'écran, qui peut être périmé.
    '
    ' Paramètres SQL :
    ' - @id_dossier : Identifiant du dossier concerné
    '
    ' Utilisé par :
    ' - GestionDossiers.ChangerStatutDossier
    ' - GestionDossiers.ExigerDossierModifiable (statut relu avant une écriture, audit C.1)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectStatutDossierPourTransition As String
        Get
            Return "
SELECT
    d.id_statut_dossier,
    sd.code_statut_dossier,
    d.date_cloture
FROM dossiers d
INNER JOIN ref_statuts_dossier sd ON sd.id_statut_dossier = d.id_statut_dossier
WHERE d.id_dossier = @id_dossier;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : SelectStatutDossierParSeance
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Type       : String
    '
    ' Description : Code du statut du dossier auquel appartient une séance.
    '
    ' Rôle       :
    ' Permet au métier de refuser l'écriture d'une séance ou d'un paiement dans un dossier clôturé ou
    ' archivé, d'après le statut réellement en base (audit C.1).
    '
    ' Paramètres SQL :
    ' - @id_seance : Identifiant de la séance concernée
    '
    ' Utilisé par :
    ' - GestionDossiers.ExigerSeanceModifiable
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectStatutDossierParSeance As String
        Get
            Return "
SELECT sd.code_statut_dossier
FROM seances s
INNER JOIN dossiers d ON d.id_dossier = s.id_dossier
INNER JOIN ref_statuts_dossier sd ON sd.id_statut_dossier = d.id_statut_dossier
WHERE s.id_seance = @id_seance;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : UpdateStatutDossier
    ' Version    : V1.1.0
    ' Date       : 21/10/2026
    '
    ' Type       : String
    '
    ' Description : Requête SQL de transition du statut d'un dossier (brique B6).
    '
    ' Rôle       :
    ' Met à jour le statut courant du dossier ainsi que les dates de clôture et d'archivage en une
    ' seule passe, sans toucher aux autres colonnes (identité, zones cliniques).
    '
    ' Paramètres SQL :
    ' - @id_statut_dossier : Identifiant du statut cible (résolu depuis ref_statuts_dossier)
    ' - @date_cloture      : Date de clôture (NULL hors état Clôturé)
    ' - @date_archivage    : Date d'archivage (NULL hors état Archivé)
    ' - @id_dossier        : Identifiant du dossier concerné
    ' - @id_statut_courant : Statut relu et validé avant l'écriture
    '
    ' Utilisé par :
    ' - GestionDossiers.ChangerStatutDossier
    '
    ' Remarques  :
    ' - date_modification est mise à jour explicitement (la table dossiers n'a pas de ON UPDATE).
    ' - La cohérence des transitions est garantie côté métier (whitelist des transitions).
    ' - V1.1.0 : écriture conditionnelle (id_statut_dossier = @id_statut_courant). Si le statut a changé
    '   entre la relecture et l'écriture, aucune ligne n'est modifiée et le métier refuse.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateStatutDossier As String
        Get
            Return "
UPDATE dossiers
SET
    id_statut_dossier = @id_statut_dossier,
    date_cloture = @date_cloture,
    date_archivage = @date_archivage,
    date_modification = current_timestamp()
WHERE id_dossier = @id_dossier
  AND id_statut_dossier = @id_statut_courant;
"
        End Get
    End Property

#End Region

#Region "Zones cliniques (B4)"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : SelectZoneDossier
    ' Version    : V1.0.0
    ' Date       : 08/07/2026
    '
    ' Type       : String
    '
    ' Description : Construit la requête SQL de lecture d'UNE zone clinique d'un dossier.
    '
    ' Rôle       :
    ' Sélectionne les colonnes RTF et TXT d'une zone donnée pour un dossier identifié par @id_dossier.
    '
    ' Paramètres :
    ' - info : Métadonnées de la zone (issues de la whitelist ZonesDossier)
    '
    ' Paramètres SQL :
    ' - @id_dossier
    '
    ' Utilisé par :
    ' - GestionDossiers.GetZoneDossier
    '
    ' Remarques  :
    ' - Les noms de colonnes proviennent EXCLUSIVEMENT de la whitelist ZonesDossier (anti-injection).
    ' - Les colonnes sont aliasées en 'zone_rtf' / 'zone_txt' pour un mapping générique côté métier.
    ' -------------------------------------------------------------------------------------------------
    Public Function SelectZoneDossier(info As ZoneDossierInfo) As String

        If info Is Nothing Then
            Throw New ArgumentNullException(NameOf(info))
        End If

        Return $"
SELECT
    {info.ColonneRtf} AS zone_rtf,
    {info.ColonneTxt} AS zone_txt
FROM dossiers
WHERE id_dossier = @id_dossier
LIMIT 1;
"

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : UpdateZoneDossier
    ' Version    : V1.0.0
    ' Date       : 08/07/2026
    '
    ' Type       : String
    '
    ' Description : Construit la requête SQL de mise à jour ciblée d'UNE zone clinique d'un dossier.
    '
    ' Rôle       :
    ' Met à jour uniquement les colonnes RTF et TXT d'une zone donnée, sans réécrire les autres
    ' colonnes du dossier (enregistrement localisé, pattern UpdateAnamnesePatient).
    '
    ' Paramètres :
    ' - info : Métadonnées de la zone (issues de la whitelist ZonesDossier)
    '
    ' Paramètres SQL :
    ' - @zone_rtf, @zone_txt, @id_dossier
    '
    ' Utilisé par :
    ' - GestionDossiers.UpdateZoneDossier
    '
    ' Remarques  :
    ' - Les noms de colonnes proviennent EXCLUSIVEMENT de la whitelist ZonesDossier (anti-injection).
    ' - date_modification est mise à jour explicitement (la table dossiers n'a pas de ON UPDATE).
    ' -------------------------------------------------------------------------------------------------
    Public Function UpdateZoneDossier(info As ZoneDossierInfo) As String

        If info Is Nothing Then
            Throw New ArgumentNullException(NameOf(info))
        End If

        Return $"
UPDATE dossiers
SET
    {info.ColonneRtf} = @zone_rtf,
    {info.ColonneTxt} = @zone_txt,
    date_modification = current_timestamp()
WHERE id_dossier = @id_dossier;
"

    End Function

#End Region

End Module
