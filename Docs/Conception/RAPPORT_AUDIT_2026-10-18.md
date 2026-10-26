# 🔍 Rapport d'audit complet — Althéa

**Date** : 18 octobre 2026
**Périmètre** : l'intégralité du code (Core, Metier, Utils, UI — 324 fichiers, ≈ 100 000 lignes) et la documentation, relus en six passes parallèles par couche, chaque constat recoupé avec la documentation (Rules, Reference_UI, ADR, Concepts métier).
**Méthode** : lecture seule d'une copie du dépôt (commit `5037dc4b`, fin d'UI-9). Aucune compilation ni exécution : tout constat est référencé par fichier et ligne, vérifiable dans le code. Les constats marqués ✔ ont été re-vérifiés à la main dans la copie auditée.
**Critères** : esprit professionnel, qualité, robustesse, évolutivité, réutilisabilité, sécurité (RGPD), convivialité ; avec en ligne de mire les trois modules à venir (agenda, documents liés à Google Drive / Agenda, IA) et un passage web ultérieur.

---

## 1. Synthèse

Althéa est une application saine dans son architecture :

- la séparation UI → Gestion\* → Query\* → DatabaseManager est respectée partout
- le SQL est intégralement paramétré
- aucun secret n'est versionné
- les mots de passe sont correctement hachés (PBKDF2-SHA256, 100 000 itérations, comparaison à temps constant)
- les journaux ne portent que des identifiants
- et le module de sauvegarde/restauration est d'une qualité remarquable (mot de passe jamais sur la ligne de commande, dump streamé dans l'archive chiffrée, retour arrière automatique).

Le kit graphique est techniquement solide (double tampon, ressources GDI libérées, navigation clavier), la documentation est abondante et vivante.

Les faiblesses sont concentrées sur trois axes, qui sont exactement ceux qui bloqueraient un passage web ou une exposition à des services externes :

1. **Quelques bugs concrets** laissés par le développement (un test oublié qui empêche d'enregistrer une configuration de connexion, deux requêtes tronquées, un identifiant de document toujours à 0, un nom de patient qui fuit dans les journaux).
2. **Les droits et les règles métier vivent trop souvent dans l'écran.**
   - Le métier fait confiance à l'UI : création de compte, annulation d'encaissement, montant libre d'une séance, validation d'un patient, politique de mot de passe, protection des lignes système.
   - En WinForms mono-poste le risque réel est faible ; en web, c'est la seule barrière qui resterait.
3. **Des opérations multi-étapes non transactionnelles** (création d'une séance et de son dû, report, réaffectation d'un paiement, enregistrement du cabinet, enchaînement des règles de quota) et **deux défauts d'intégrité financière** possibles sur les avoirs.

Rien n'impose une refonte. Le plan proposé en § 6 tient en quatre lots progressifs, le premier (bugs et sécurité) se faisant en une journée.

---

## 2. Bugs confirmés (à corriger en premier)

| # | Où | Quoi | Effet | Correction |
|---|---|---|---|---|
| B1 ✔ | `UI/Forms/Database/ConfigurationConnexion.vb:497-503` | Bloc « TEST TEMPORAIRE DialogChoix » resté dans `btnTesterConnexion_Click` : affiche une erreur et `Return`. | « Tester la connexion » ne teste rien ; comme Enregistrer exige un test réussi, **aucune configuration ne peut plus être enregistrée** (y compris au premier démarrage). | Supprimer les lignes 497-503. |
| B2 ✔ | `ConfigurationConnexion.vb:400-409, 672-690` | La branche « mot de passe inchangé » teste `txtPassword.Text = "********"`, mais ce placeholder n'est jamais posé (grep : aucune affectation). | En mode « mot de passe conservé », modifier l'hôte et enregistrer **remplace le mot de passe stocké par une chaîne vide chiffrée** → plus de connexion au démarrage suivant. | Conditionner sur `_passwordMode = KeepExisting` (comme le test l. 528) et supprimer la logique `********`. |
| B3 ✔ | `UI/Forms/Home.vb:919-922` + `UC_PatientHome.vb:654, 1668` | `SelectHomeMenuButton` journalise le fil d'Ariane, qui contient `patient.NomComplet`. | **Nom du patient dans les journaux** à chaque navigation (ADR-39, RGPD). | Journaliser le `Tag` du bouton, jamais un texte d'affichage. |
| B4 ✔ | `Core/Database/Queries/QueryNaturesSeance.vb:61`, `QueryStatutsSeance.vb:71` | Les requêtes « toutes » se terminent par `ORDER BY` sans expression. | Erreur SQL dès qu'on coche « Afficher les inactifs » ou qu'on réactive une ligne dans Natures / Statuts de séance. | Compléter `ORDER BY ordre_affichage, libelle_…`. |
| B5 ✔ | `QueryDocuments.vb:128` | `SELECT LAST_INSERT_ID()` alors que `documents.id_document` vient de `seq_documents`. | `GestionDocuments.InsertDocument` **retourne toujours 0** ; latent (appelants ignorent le retour), bloquant pour le module Documents. | `SELECT LASTVAL(seq_documents)` sur la même connexion. |
| B6 ✔ | `GestionUtilisateurs.vb:373-576` / `QueryUtilisateurs.vb:106-124` | `MapUtilisateur` lit `password_hash` que la requête ne sélectionne pas. | `GetUtilisateurs()` / `HasUtilisateurs()` planteraient ; code mort **et** cassé. | Supprimer les trois méthodes (doublon de `MapUtilisateurPourAdministration`). |
| B7 ✔ | `UC_Utilisateurs.vb:330` | Filtre « Dernière connexion **depuis** » teste l'égalité de date. | Le filtre ne renvoie que les connexions du jour exact. | `>= dateFiltre`. |
| B8 | `UC_RichTextEditor.vb:782-835` | `SelectionChanged` pose `cmbFontSize.SelectedIndex`, ce qui déclenche `ChangerTaillePolice` → `SelectionFont` réécrit ; `CInt(10.5)` → 10. | **Déplacer le curseur modifie le texte** (10,5 pt → 10 pt), marque l'anamnèse « modifiée » et pollue l'historique Annuler. | Drapeau `_majBoutonsEnCours` testé dans les handlers des combos ; comparer en `Single`. |
| B9 | `GestionTarification.vb:61` | `CODE_STATUT_NO_SHOW = "NO_SHOW"` ; partout ailleurs le no-show est `ABSENTE`. | La règle « no-show non facturable → 0 » du résolveur de tarif est du code mort. | Constante unique (ou drapeau `est_no_show` en référentiel). |
| B10 | `UC_DomainesHome.vb:906` | `PeutModifier` retourne `True` si `_context Is Nothing`. | Fail-open. | Fail-closed. |
| B11 | `DossierZoneEditor.vb:240-247` | `Return` avant `_modifie = False` quand la zone est vide. | Fausse alerte « modifications non enregistrées » sur une zone vide. | `_modifie = False` avant le `Return`. |
| B12 | `IntervenantEdition.vb:424, 760` | Téléphone saisi mais `SuiviIntervenant` n'a pas la propriété. | Vide à la réouverture. | Ajouter la propriété ou retirer le champ. |

À retirer aussi, sans débat : le mot de passe en clair `"AltheaSuperUser1"` dans `UC_Accueil.vb:54` (outil Debug, mais présent dans un dépôt dont un miroir est public : le considérer comme compromis) ; les `[DEBUG]` de `GestionUtilisateurs.vb:930-959` et `UtilisateurEdition.vb:348-352` ; le bloc « TODO DEBUG joelle » `GestionUtilisateurs.vb:37-44`.

---

## 3. Sécurité et RGPD

### 3.1 Droits contrôlés uniquement dans l'écran

La documentation (`Securite_Utilisateurs.md:37`, Rules § 5) promet « les droits sont vérifiés dans la couche métier ». Dans les faits, hors Sauvegardes / Restaurations / Licences / Paramètres (qui le font bien, avec un `roleCourant`), le métier ne vérifie rien :

- `GestionUtilisateurs.CreateUtilisateur` (l. 825) : aucun appelant authentifié → un **SuperUser crée un compte SuperUser élevable Admin** via Cabinet › « Créer le compte » (`UC_Cabinet.vb:794`, `GestionCabinet.CreerCompteSuperUser:297`).
  - `ResetPasswordUtilisateurWithCustomPassword` (l. 1653) : « pas de vérification des droits : gérée par ChangePassword.vb ».
- `GestionPaiements.AnnulerPaiementEncaisse` (l. 544) : réservé SuperUser dans l'UI seulement ; l'`auteur` est une chaîne libre.
- `GestionSeances.UpdateSeance` : ni verrou « séance encaissée », ni montant libre réservé au SuperUser (le cadenas de `SeanceEditor.vb:1227` n'a d'ailleurs **aucun** contrôle de rôle).
- Tous les `Insert/Update/Supprimer` des 22 `Metier/Referentiels/Gestion*.vb` ; dossier clôturé/archivé jamais vérifié par `CreateSeance`, `UpdateDossier`, `CreatePaiement`…
- `UpdateUtilisateur` permet à un Admin de se rétrograder ou de se désactiver lui-même, et de laisser la base sans Admin.

**Deux « rôles courants » coexistent** : `UserSession.CurrentRole` (élévation comprise, utilisé par Sauvegardes) et `UtilisateurApplication.RoleUtilisateur` (rôle de base, utilisé par `GestionUtilisateurs:1055, 1260…`).

Un SuperUser élevé Admin peut restaurer la base mais pas déverrouiller un compte. `UserSession.HasAtLeastRole` n'est appelé nulle part.

### 3.2 Élévation et session

- `VerifierElevationUtilisateur` (l. 229-337) ne compte pas les échecs ni ne verrouille : **essais illimités** depuis une session ouverte.
- L'élévation n'a **aucune expiration** ; `LastActivityDateTime`, `Touch()`, `ElevationDateTime` existent dans `UserSession` et ne sont lus nulle part. Pas de verrouillage d'inactivité sur une application de données de santé.
- Politique de mot de passe (10 caractères, majuscule, minuscule, chiffre) uniquement dans `ChangePassword.vb:147-225` ; le métier accepte « a ». Réimplémentée pour les archives dans `GestionSauvegardes:765`.
- Mot de passe temporaire : repli constant `"TempPass2026!"` en cas d'erreur (`GestionUtilisateurs.vb:1210`), `RNGCryptoServiceProvider` obsolète, biais modulo ; affiché en clair et laissé dans le presse-papiers (`UC_Cabinet.vb:799-807`, `UtilisateurEdition.vb:358-369`).
- Hachage non versionné : impossible de relever les itérations (OWASP : 600 000) ou de passer à Argon2 sans invalider tous les comptes.

### 3.3 Autres

- `DatabaseManager.vb:88-94` : chaîne de connexion par concaténation → un mot de passe DB contenant `;` ou `"` casse la connexion ; pas de `SslMode` imposé (données patients potentiellement en clair si MariaDB est sur une autre machine). Utiliser `MySqlConnectionStringBuilder` (déjà employé l. 258).
- Pas de gestionnaire global d'exceptions (`Application.ThreadException` / `UnhandledException` : 0 occurrence) : une exception non interceptée affiche la pile .NET (tables, chemins) et n'est pas journalisée.
- `GestionLog.ReplaceKeyValueInsensitive` masque `Password=`/`Pwd=` mais pas `token`, `secret`, `key`, `Bearer`, ni le JSON : à étendre **avant** la première ligne OAuth Google.
- Journalisation de données de tiers : `GestionSuivisIntervenants.vb:197, 215, 227, 290` (nom du professionnel), `GestionPaiements.vb:595` (motif libre).
- Clé Syncfusion : toujours dans l'historique Git du miroir public → à renouveler (déjà dans le ToDo) et purger l'historique du miroir.
- Métadonnées d'annulation en texte libre `"[Annulé le … par …]"` re-parsées pour déduire l'état (`PaiementListeItem.vb:185-194, 531-574`) : non auditable, fragile.

---

## 4. Robustesse et intégrité des données

### 4.1 Intégrité financière (avoirs)

- **Double comptage possible à la réaffectation** (`GestionSeances.vb:841-913`) : `MontantReporte = paiement.Montant` (montant total) alors qu'une part a pu être consommée en avoir via `UtiliserAvoirPourSeance`.
  - Scénario : S1 payée 60 €, tarif baissé à 40 €, 20 € d'avoir utilisés sur S2, S1 annulée → 60 € réaffectés sur S3 : 80 € imputés pour 60 € reçus.
  - Utiliser `SUM(montant_affecte)` restant.
- **Annulation du paiement source d'un avoir déjà utilisé** (`GestionPaiements.vb:544-617`) : le paiement passe ANNULE mais l'avoir créé sur la séance cible reste PAYE. Refuser si `SUM(montant_affecte) < montant`.
- `ReactiverPaiementsAnnulesParSeance` remet **tous** les dûs annulés sans date : dû doublé après plusieurs cycles ; aucune garantie « une seule ligne EN_ATTENTE par séance ».
- `paiements.mode_paiement` stocke le **libellé** et non le code ; la détection « règlement par avoir » repose sur ce libellé. Renommer « Avoir » (autorisé par ADR-28) casse l'historique.

### 4.2 Opérations non atomiques (chaque étape sur sa propre connexion)

- Création de séance → `CreatePaiementEnAttente` **avec exception avalée** (`GestionSeances.vb:632-641`) : séance sans dû, utilisateur informé d'un succès. Même motif pour `ResynchroniserDuSeance` (l. 714-723, 1453-1472).
- `SeanceEditor.vb:1959-1974, 2035-2052` : `CreateSeance` → `UpdateNotesSeance` → `ChangerStatutSeance` orchestrés par l'UI.
- `ReporterSeance` (l. 1091-1177) : trois écritures, compensation partielle ; si elle échoue, la séance reste `REPORTEE` sans remplaçante, donc bloquée définitivement.
- `ChangerStatutDossier` valide la transition avec le statut **fourni par l'écran** (potentiellement périmé) et écrit sans condition SQL : un dossier archivé peut être réactivé.
- `UC_Cabinet.vb:496-522` (cabinet + praticien + identifiants), `UC_DomainesHome.vb:1433-1494` (clôture de la règle précédente + insertion, compensation manuelle), `GestionCabinet.CreerCompteSuperUser` (compte créé puis lié : compte orphelin et mot de passe perdu si la liaison échoue).

### 4.3 Un `SELECT` qui écrit

`GestionSeances.GetSeancesParDossier` (l. 202-207) exécute `BasculerPlanifieesEchues` : **charger un onglet modifie des statuts en base**, sans transaction avec le dû, sans trace, alors que `Concepts_Metier_Althea.md:101-103` affirme que le statut effectif est « calculé, non destructif ».

Conséquence directe pour l'agenda : l'information « RDV passé non pointé » vs « séance confirmée » est détruite au premier affichage. Décision à acter par ADR avant le module Agenda.

### 4.4 Divergences de règles

- **Reste dû défini trois fois** avec des résultats différents pour l'absence non facturable (`SeanceListeItem.ResteDu`, `QueryPatients.montant_du` vs `SelectSyntheseDuSeance`, `SelectSeancesNonSoldeesParDossier`, `SelectResteDuSeance`) : une absence non facturable garde une ligne EN_ATTENTE et est proposée « à régler ».
- **Deux algorithmes de quota** : `GetQuotaDossier` (période unique) vs `AppliquerRangsQuota` (renouvellement glissant) → l'alerte de `SeanceEditor` ne voit jamais le dépassement de la 2ᵉ période.
- **Trois définitions du « dossier dominant »** (`QueryPatients.vb:125-146, 147-163`, `GestionDossiers.ChoisirDossierDominant`).
- « Séance échue » écrite 5 fois en SQL avec 2 formules différentes ; sous-requête « encaissé PAYE d'une séance » copiée 8 fois.
- Horloge : `Date.Now` côté .NET (30 occurrences) et `NOW()` côté SQL — divergera dès qu'il y aura un serveur.
- Cache des règles de quota jamais invalidé quand un dossier change de domaine (`GestionQuotasInami.vb:140-186`).

### 4.5 Démarrage

- **Aucune vérification de version du schéma au démarrage** (`VersionSchema` comparé seulement dans État du système) : une base non migrée produit des erreurs SQL éparses.
- `TestConnection` avale la cause (`DatabaseManager.vb:216-231`) ; le test de connexion de `ConfigurationConnexion` **modifie l'état global** (`Initialize` avec les valeurs testées, même non enregistrées).
- Écriture des fichiers de configuration par `File.WriteAllText` direct : un plantage laisse un fichier vide → repart en configuration. Écrire en `.tmp` puis `Move`.
- `ImagesHelper.CopierSousNomFixe` supprime l'ancienne photo **avant** la copie.

---

## 5. Qualité, dette, convivialité

### 5.1 Qualité du code

- `<OptionStrict>Off</OptionStrict>` au niveau projet (`Althea.vbproj:10`) ; 36 fichiers sans `Option Strict On` (dont `GestionUtilisateurs`, `GestionAuthentification`, `GestionCabinet`, 20 référentiels) compilent en liaison tardive.
- **Codes métier en dur** malgré la règle « pilotage par les référentiels » :
  - `'CLOTURE'`, `'ARCHIVE'`, `'PAYE'`, `'EN_ATTENTE'`, `'REALISEE'`… ~40 occurrences dans les `Query*`
  - `"REPORTEE"`/`"PLANIFIEE"` littéraux dans `UC_DossierFiche` et `SeanceEditor` alors que `GestionSeances` a les constantes en `Private`.
  - Protection `est_systeme` en SQL dans 3 référentiels sur 12 seulement : un SuperUser peut renommer le code `PAYE` ou `CLOTURE` et casser silencieusement montants dus et compteurs.
- **Duplication** :
  - 16 gestionnaires de référentiels quasi identiques en deux « générations »
  - motif « [+] référentiel puis rechargement du combo » × 7 (~350 lignes)
  - bloc d'élévation × 4 dans `Home` et × 2 dans les hubs
  - triplet « recherche + filtre + pagination » × 5 (~900 lignes)
  - bandeau patient × 3
  - bloc « coordonnées » × 2-3
  - `AltheaHeure` ≈ `AltheaDate` (~450 lignes)
  - helpers de lecture DB réécrits dans 6 modules à côté de `DbHelper`
  - trois mappers `sec_utilisateurs`
  - `MotDePasseSauvegarde` ≡ `LicenceSyncfusion` à 95 %.
- **Fichiers trop gros** : `UC_DossierFiche` 4 126 l., `UC_PatientFiche` 3 886 l., `GestionSeances` 1 687 l., `Documentation_technique_UI` 8 277 l.
- Trois techniques pour l'identifiant généré (`LASTVAL` séparé, `LAST_INSERT_ID()` multi-instructions, requête dédiée) ; trois conventions de nommage des paramètres SQL ; `Long`/`ULong` mélangés avec conversions dispersées.
- Fuites : `UtilsDataGrid._etatsTri` ne retire jamais une grille disposée ; polices créées sans `Dispose` dans 10 contrôles du kit (`AltheaFilAriane` en crée une à chaque `OnPaint`) ; `_tabDonneesLiees`, `ToolTip` locaux hors `components`.
- Code mort :
  - `Seance.EstRealisee/EstPlanifiee/EstAnnulee`
  - `GestionDossiers.GetCodeStatutDernierDossier`
  - `QueryPaiements.SelectIdsPaiementsEnAttenteParSeance`
  - `QueryUtilisateurs.SelectUtilisateurPourAuthentification` et `ForcerChangementPasswordUtilisateur`
  - `UC_RichTextEditor.ContentSaved`
  - `Home.RestoreContexte`
  - `App.config` vide
  - membres inutilisés de `UserSession`
  - `TestRichTextEditor` (contredit le kit)
  - fenêtres de test compilées en Release.

### 5.2 Métier dans l'UI (frein direct au web)

- Validation du patient (nom, prénom, NISS unique) dans `UC_PatientFiche.vb:1347-1427` et nulle part dans `GestionPatients.CreatePatient`
- `SeanceReportable`/`SeanceEncaissable` dans `UC_DossierFiche`
- « dossier figé » dupliqué UI
- traçabilité documentaire avec codes magiques (`"ANAMNESE"`, `"LOCAL_DOCX"`, `"PDF"`) dans `UC_PatientFiche.vb:2061-2172` et `DossierZoneEditor.vb:448-493`
- complétude d'un domaine calculée par `UC_SyntheseDomaine`
- « code = 3 lettres » dans `UC_DomainesHome.vb:1055`
- statuts proposables décidés par `SeanceEditor.ChargerStatutsProposes`
- latéralité, modalité de paiement, pays en listes codées (`UtilsTelephone.vb:114-140` déduit le pays par **libellé**).

### 5.3 Convivialité

- Aucune garde « modifications non enregistrées » sur la fiche patient ni la fiche dossier (seule l'anamnèse en a une) ; pendant « Modifier », les onglets rattachés restent ouverts et la saisie peut être perdue silencieusement.
- Export/impression de l'anamnèse impossibles en consultation (il faut passer en « Modifier » pour imprimer).
- 12 fenêtres sur 20 sans `AcceptButton`/`CancelButton` (Échap ne ferme pas, Entrée ne valide pas).
- Erreurs d'enregistrement signalées dans la barre d'état de la Home, invisible sous une modale (`PaiementEditor`, `SeanceEditor`, `DossierZoneEditor`, `DossierStatutEditor`).
- `MessageBox.Show` encore présent : `UC_ReferentielBase` (classe de base des 16 référentiels), `SeanceEditor` ×4, `UtilisateurEdition`, `UC_PatientFiche`, 15 dans `RichTextEditorHelper` (dont `ex.Message` affiché).
- `Confirmer` Oui/Non au lieu de boutons qui disent l'action dans ~10 endroits ; suppressions physiques sans `ConfirmerSuppression`.
- Agenda et Documents : boutons du menu actifs qui ne changent pas l'écran.
- Performances d'ouverture : fiche dossier → 3 synthèses de domaine et 2 lectures DB inutiles (`cboDomaine` posé avant ses membres), tous les tarifs rechargés à chaque rafraîchissement du bandeau, dossier dominant relu à chaque changement d'onglet ; filtrage à chaque frappe sur séances/paiements.
- **DPI** : aucune politique (`dpiAware` commenté, pas de `SetHighDpiMode`), kit en pixels fixes → à 125-150 % textes tronqués, calendrier inutilisable.
- Accessibilité : aucun `AccessibleName` dans le kit ; `AltheaOnglets` sans repère de focus ; vue « années » du calendrier à la souris seulement.

### 5.4 Documentation et outillage

- **Aucun test automatisé, aucune CI** : `Althea.slnx` ne contient que le projet ; `STRATEGIE_TESTS_ALTHEA.md` ne mentionne ni test unitaire ni intégration continue.
- Aucun guide d'installation / mise à jour d'un poste ; migrations appliquées à la main sans runner ; `v2.29.0` seule migration sans ROLLBACK ; pas de SemVer d'application ; CHANGELOG de 3 928 lignes sans numéro de version.
- Incohérences doc ↔ code :
  - README « schéma v2.39.0 » (réel 2.40.0)
  - `UC_Domaines` supprimé le 18/10 mais décrit comme vivant dans Rules § 22, `Parametres_Referentiels.md:67`, doc UI ×3, PLAN_TESTS_REFERENTIELS
  - 119 liens vers `Docs/Illustrations` absents
  - « Réalisée calculée non destructive » vs persistée
  - « mode_paiement stocke le code » vs libellé
  - doc UI dit `UC_RichTextEditor` « hérite de `EventArgs` »
  - Rules § 9 / Reference_UI § 8 se contredisent sur `SplitContainer`.
- Prolifération : 5 documents de pilotage qui se désignent mutuellement source de vérité ; 3 notes sur un tarif patient archivé ; `Standards-Commentaires.md` impose « V1.0 » quand la pratique est « V1.0.0 » ; pas de `.editorconfig` (BOM mélangés).

---

## 6. Plan proposé

Quatre lots, du plus urgent au plus structurant. Chaque lot laisse le projet cohérent et testable.

### Lot A — Bugs et brèches (une journée)
1. Les 12 bugs du § 2, le mot de passe en clair de `UC_Accueil`, les `[DEBUG]`.
2. Renouveler la clé Syncfusion ; purger l'historique du miroir public.
3. `CreateUtilisateur`, `CreerCompteSuperUser`, `LierUtilisateur`, `AnnulerPaiementEncaisse`, `ResetPassword…` : paramètre `roleCourant` + `UnauthorizedAccessException` ; `UpdateUtilisateur` refuse l'auto-rétrogradation et la perte du dernier Admin.
4. Gestionnaire global d'exceptions ; `MySqlConnectionStringBuilder` ; vérification de `VersionSchema` au démarrage ; écriture `.tmp` + `Move` des fichiers de config.
5. Élévation : compter les échecs, verrouiller, expirer (paramètre `SECURITE_ELEVATION_MINUTES`).

### Lot B — Intégrité des données (quelques jours)
1. Les deux défauts avoirs (§ 4.1) ; `ChangerStatutDossier` relit et écrit sous condition SQL ; unicité du dû EN_ATTENTE.
2. Unité de travail transactionnelle sur `DatabaseManager` (surcharges `(conn, tx)`) ; `EnregistrerSeance`, `ReporterSeance`, `ReaffecterPaiement`, `EnregistrerFondation` (cabinet), `AjouterRegleEnChaine` (quota) deviennent atomiques et remontent un résultat typé.
3. ADR « vérité du statut de séance » : sortir `BasculerPlanifieesEchues` des lectures (pointage explicite ou calcul pur). **Prérequis de l'agenda.**
4. Vues SQL `v_seances_solde`, `v_patients_synthese` : une seule définition du dû, de « échue », du dossier dominant ; `QueryPatients`, `QueryPaiements`, `QuerySeances`, `SeanceListeItem` les consomment.
5. `mode_paiement` → code (migration) ; colonnes typées pour l'annulation et l'avoir (`date_annulation`, `annule_par`, `motif`) ou table `paiements_mouvements`.
6. Un seul algorithme de quota ; invalidation du cache ; constante unique pour le no-show (ou drapeau `est_no_show`).

### Lot C — Métier hors de l'UI, droits côté métier (le chantier « pré-web »)

> **Découpage validé par Joëlle le 22/10/2026** (ordre) :
>
> - **C.1** droits côté métier (`ContexteOperation`, décision de Joëlle)
> - **C.2** règles métier hors des écrans
> - **C.3** codes système centralisés
> - **C.4** découplage des écrans de `Home`
> - **C.5** découpage des gros fichiers (là où cela aide)
> - **C.6** (préparation Documents, point 4 ci-dessous) au démarrage du module Documents.
>
> La recherche transversale (ToDo) vient après C.2.
>
> **C.2, inventaire et découpage validés par Joëlle le 22/10/2026** (ADR-51) : valideur métier rendant la liste des erreurs par champ (`ErreurSaisie`), appliqué aussi par l'écriture ; chevauchement d'horaire refusé par le métier.
>
> - **Étape 1** séances et paiements (reportable, supprimable, encaissable, statuts proposés, chevauchement, encaissement)
> - **étape 2** patient, contacts, intervenants, thérapeutes (`ValiderPatient`…, `SuiviIntervenant.EstTerminee`)
> - **étape 3** domaines (`ValiderDomaine`, complétude `CalculerManques` sortie de `UC_SyntheseDomaine`)
> - **étape 4** comptes (`PolitiqueMotDePasse.Valider`, longueur du login).
>
> Hors C.2 :
>
> - dossier figé (🔁 C.1)
> - latéralité et modalité (✅ 20/10)
> - table des pays du téléphone (→ C.3)
> - traçabilité documentaire (→ C.6)
> - tarifs et natures filtrés par domaine restent une aide de saisie de l'écran.
1. `ContexteOperation` (utilisateur, rôle effectif, horloge) passé aux méthodes sensibles ; garde unique `Autorisation.Exiger` ; `est_systeme` en SQL sur tous les référentiels ; matrice rôle × opération dans `Securite_Utilisateurs.md`.
2. Validation dans `Gestion*` : `ValiderPatient`, `PolitiqueMotDePasse.Valider`, `ValiderDomaine`, `CalculerManques`, `StatutsProposes(creation, codeCourant)` ; propriétés `SeanceListeItem.EstReportable/EstEncaissable`, `Dossier.SeancesFigees`, `SuiviIntervenant.EstTerminee`.
3. Codes système en un seul module `CodesSysteme` (puis drapeaux de référentiel : `est_no_show`, `libere_creneau`, `par_defaut`, `proposable_creation`…).
4. `GestionDocuments` : `TracerExport`, lecture par patient/dossier/séance, `UNIQUE` sur `chemin_relatif`, empreinte SHA-256, `Enum` pour type/source/statut de synchro. **Prérequis du module Documents.**
5. Casser le couplage à `Home` (58 signatures `As Home`) : services dans `UserControlContext` ; garde unifiée `IEcranAvecSaisie` vérifiée par `NavigationManager`.
6. Découper `UC_PatientFiche` et `UC_DossierFiche` par onglet ; brique « liste filtrée paginée » ; `UC_BandeauPatient` ; `UC_Coordonnees` ; helper « [+] référentiel » ; `ReferentielSimple(Of T)` pour les 16 gestionnaires ; `AltheaChampCompose` pour Date/Heure.

### Lot D — Outillage et finition
1. `Althea.Tests` (xUnit) + CI GitHub Actions `windows-latest` ; premiers tests sur le métier pur (quotas, validation NISS, interpréteurs Date/Heure) et un **test de fumée des 44 modules Query** (`PREPARE` de chaque requête : aurait attrapé B4).
2. `Option Strict On` projet, `.editorconfig`, analyseurs .NET, SemVer, runner de migrations, guide « Installer / Mettre à jour un poste », ROLLBACK v2.29.0.
3. Politique DPI `PerMonitorV2` + facteur d'échelle dans `AltheaDessin` ; `AcceptButton`/`CancelButton` partout ; zéro `MessageBox` ; erreurs dans la fenêtre ; polices partagées du kit ; `AccessibleName`.
4. Documentation : un seul document de pilotage ; archiver les notes périmées ; scinder la doc UI par domaine ; corriger les mentions d'`UC_Domaines`, README, Rules § 9/§ 22 ; rétablir ou retirer les 119 liens.

---

## 7. Orientations pour les trois modules à venir

**Agenda.** `rendez_vous`, `ref_types_rendez_vous`, `documents.id_rendez_vous` et les paramètres `SEANCE_*` existent déjà.

- Décider par ADR si une séance est un rendez-vous réalisé (FK `seances.id_rendez_vous`) ou si `rendez_vous` ne sert qu'aux créneaux non cliniques ; sinon la synchro Google Agenda devra écouter deux tables.
- Prévoir par migration `google_event_id`, `etag`, `date_sync`, statut de conflit (symétrie avec `documents.statut_sync_google`).
- Trancher la stratégie temps (serveur SQL `NOW()` recommandé).
- Régler d'abord la « vérité du statut » (B.3) et la garde « saisie non enregistrée » (C.5).

**Documents / Google Drive.** La table `documents` est déjà prête (`google_file_id`, `google_mime_type`, `statut_sync_google`…) : garder la base comme source de vérité des métadonnées, le fichier local comme contenu, et un `IFournisseurStockage` (Local / Drive) sous `GestionDocuments`, le métier ne faisant aucun appel réseau.

- La synchro devient un job sur `statut_sync_google IN ('A_RESYNC','ERREUR_SYNC')`.
- Secrets OAuth : le refresh token est lié à un utilisateur Althéa, pas à un compte Windows → table `sec_jetons_externes` chiffrée par une clé applicative, `client_secret` hors base et hors dépôt ; étendre le masque des journaux avant la première ligne.
- RGPD : Drive est un second lieu d'hébergement de données de santé (hébergeur potentiellement hors UE) : chiffrement côté client ou espace dédié à décider avant la première synchro ; journaliser l'accès aux documents (qui, quand) sans contenu.

**IA.** Les zones cliniques (`*_txt`, `notes_seance_txt`, `anamnese_txt`) sont la donnée la plus sensible.

- Un `IServiceIA` dans Metier, jamais depuis l'UI
- un service `ExtraireContexteDossier(idDossier, options)` qui produit un texte **pseudonymisé** (codes `PA000001`/`DO…`, dates relatives)
- paramètres `IA_ACTIVE`/`IA_FOURNISSEUR`
- consentement patient tracé en base
- journal dédié des appels (horodatage, dossier, finalité, modèle, volume — jamais le contenu).
- Côté éditeur de notes : deux points d'extension `InsererTexte`/`RemplacerSelection` et un événement `TexteSelectionneChange`, après correction de B8.

**Web (plus tard).**

- Les trois dépendances Windows de Core (DPAPI, `%APPDATA%`, fichiers de log) derrière des interfaces
- `DatabaseManager`, `GestionLog`, `ConfigManager` en classes instanciées au démarrage plutôt qu'en `Module` statiques
- un exécuteur d'accès aux données unique (`DbExecutor.Lire/Scalar/NonQuery/DansTransaction`) qui absorbe les 308 `New MySqlCommand` et rend le métier testable
- harmoniser `Long`/`ULong`.
- Le kit WinForms ne se portera pas : n'y investir qu'en robustesse (DPI, Dispose, accessibilité).

---

## 8. Ce qui est sain et ne demande rien

- Sauvegarde / restauration (ADR-37/38)
- `GestionParametres`, `GestionEtatSysteme`, `GestionLicences`, `GestionFamilleContacts`, `GestionSuivisIntervenants`
- zones cliniques
- `DialogChoix`, `NavigationManager`
- `UC_Parametres`, `UC_Journaux`, `UC_EtatSysteme`
- logique de créneaux (`CreneauxProposes`, `GetPlanningJour`, `ExisteChevauchement` vérifiés sur les cas limites)
- DTO `Dossier`, `DossierListeItem`, `ControleClotureDossier`, `SoldeDossier`, `LignePlanningJour`
- et la discipline des en-têtes et du CHANGELOG.

---

## 9. Suivi des corrections

> Tenu à jour au fil des corrections (démarré le 20/10/2026, décision de Joëlle : passer tous les points en revue). État : ✅ corrigé · ⏳ en cours · ⬜ à faire · ➖ sans objet (code réécrit depuis) · 🔁 déjà traité ailleurs.

| Point | État | Date | Correction |
|---|---|---|---|
| B1 | ✅ | 20/10/2026 | Bloc de test retiré (`ConfigurationConnexion` V1.2.1). |
| B2 | ✅ | 20/10/2026 | Mot de passe conservé selon le mode `KeepExisting` ; logique `********` et `txtPassword_Enter` supprimées. |
| B7 | ✅ | 20/10/2026 | « Depuis » : date choisie et suivantes (`UC_Utilisateurs` V1.7.0), avec le passage du champ à `AltheaDate`. |
| B3 | ✅ | 20/10/2026 | Le journal de navigation note le nom du bouton du menu, plus le fil d'Ariane (`Home` V1.15.0). Même passe : noms des intervenants retirés du journal (`GestionSuivisIntervenants` V1.2.0). |
| B4 | ✅ | 20/10/2026 | `ORDER BY ordre_affichage, libelle_…` (`QueryNaturesSeance`, `QueryStatutsSeance` V1.0.1) ; aucune autre requête tronquée trouvée. |
| B5 | ✅ | 20/10/2026 | `InsertDocument` lit `LASTVAL(seq_documents)` sur la même connexion (`QueryDocuments.SelectDernierIdDocument`, `GestionDocuments` V1.0.1). |
| B6 | ✅ | 20/10/2026 | `GetUtilisateurs`, `HasUtilisateurs`, `MapUtilisateur` et `SelectUtilisateursListe` supprimés (code mort et cassé). |
| § 2 mot de passe en clair de `UC_Accueil`, `[DEBUG]`, bloc « TODO DEBUG joelle » | ✅ | 20/10/2026 | L'outil de hash demande le mot de passe (saisie masquée, décision de Joëlle : garder l'outil) ; trois traces `[DEBUG]` et le bloc TODO retirés. **À faire de ton côté** : si un compte utilise encore « AltheaSuperUser1 », changer son mot de passe (le dépôt l'a rendu public). |
| § 3.2 mot de passe temporaire : repli constant, générateur obsolète, biais | ✅ | 20/10/2026 | `GenererMotDePasseTemporaire` V2.0.0 : `RandomNumberGenerator.GetInt32`, plus de repli « TempPass2026! ». L'affichage en clair et le presse-papiers restent à décider avec le reste du § 3.2. |
| § 3.3 pas de gestionnaire global d'exceptions | ✅ | 20/10/2026 | `GestionErreursGlobales` (installé en tête de `Program.Main`) : journalisé, message clair sans détail technique ; interface → l'application continue, hors interface → fermeture annoncée. Bouton de test « Provoquer une erreur » (accueil, Debug). |
| Version du schéma non contrôlée au démarrage | ✅ | 20/10/2026 | `AppStartupManager.VerifierVersionBase` : base en retard → message et fermeture ; plus récente ou illisible → avertissement (décisions de Joëlle). Version de la base = plus haute migration (`GestionEtatSysteme.LireVersionSchemaBase`), plus le tri par date. |
| A.3 droits côté métier (`CreateUtilisateur`, `CreerCompteSuperUser`, `LierUtilisateur`, `AnnulerPaiementEncaisse`, réinitialisation…) ; deux « rôles courants » | ✅ | 20/10/2026 | Décision de Joëlle : le rôle courant (élévation comprise) fait foi partout (`UserControlContext.RoleCourant`). Les méthodes reçoivent `roleCourant` et lèvent `UnauthorizedAccessException`. `UpdateUtilisateur` : pas de modification de son propre rôle, élévation ou état ; jamais sans Admin actif (aussi à la désactivation). Compte du praticien réservé à l'Admin (un SuperUser pouvait créer un compte élevable en Admin). `ResetPasswordUtilisateur` (jamais appelée) supprimée. La garde unique `Autorisation.Exiger` reste au lot C. |
| § 4.5 fichiers de configuration écrits sans précaution | ✅ | 20/10/2026 | `ConfigManager.EcrireFichierConfiguration` (fichier `.tmp` puis remplacement) pour la connexion, la licence et le mot de passe des sauvegardes. |
| A.5 / § 3.2 élévation : essais illimités, aucune expiration | ✅ | 20/10/2026 | Décisions de Joëlle : mots de passe faux comptés comme à la connexion, compte verrouillé au seuil (`VerifierElevationUtilisateur` V2.0.0, compte relu en base) ; fin après inactivité, paramètre `SECURITE_ELEVATION_MINUTES` (migration v2.48.0), `SuiviActivite` + minuterie de la Home. Le verrouillage d'inactivité de toute la session (au-delà de l'élévation) reste à décider. |
| B9 | ✅ | 20/10/2026 | Le no-show est `ABSENTE` : codes des statuts de séance en constantes publiques uniques (`GestionSeances.CODE_STATUT_*`, comme les dossiers), plus aucune chaîne répétée dans le code VB. La règle « no-show non facturable → 0 » du résolveur s'applique enfin. Les requêtes SQL de `QueryPatients` gardent `'ABSENTE'` (commenté) ; le drapeau en référentiel reste au ToDo. |
| B10 | ✅ | 20/10/2026 | `UC_DomainesHome.PeutModifier` refuse sans contexte (V1.10.1) ; seul écran concerné. |
| B11 | ➖ | 19/10/2026 | Code réécrit : garde des modifications par photo prise à l'affichage (`GardeModifications.SurveillerFenetre`, ADR-43) ; une zone vide ne donne plus d'alerte. |
| B12 | ✅ | 20/10/2026 | Décision de Joëlle : téléphone **actuel** du thérapeute lié, affiché aussi à la réouverture (`IntervenantEdition` V1.5.0) ; pas de copie figée, pas de migration. |
| § 3.3 chaîne de connexion concaténée | ✅ | 20/10/2026 | `MySqlConnectionStringBuilder` (`DatabaseManager` V1.2.0). `SslMode` imposé : à décider avec l'installation chez Pearl (base sur le même poste ou non). |
| § 4.5 `TestConnection` avale la cause ; le test modifie l'état global | ✅ | 20/10/2026 | Cause journalisée et rendue (`TesterConfiguration`, `ResultatTestConnexion`) ; le test ne touche plus la connexion de l'application, appliquée à l'enregistrement. Messages d'aide et d'erreur dans `ConfigurationConnexion` (demande de Joëlle). |
| § 4.5 `CopierSousNomFixe` supprime l'ancienne image avant la copie | ✅ | 20/10/2026 | Nouvelle image écrite à part puis mise en place (`ImagesHelper.Remplacer`, V1.1.0), avec les logos des domaines. |
| § 5.4 README « schéma v2.39.0 » | ✅ | 20/10/2026 | v2.45.0. |
| § 5.2 latéralité et modalité de paiement en listes codées | ✅ | 20/10/2026 | Référentiel `ref_lateralites` (migration v2.46.0) ; modalité proposée en paramètre `DOSSIER_MODALITE_PAIEMENT_DEFAUT`. Le pays déduit par libellé (`UtilsTelephone`) reste au lot C. |
| B8 | ➖ | 19/10/2026 | Éditeur réécrit (V2.0.0, ADR-42) : plus de listes de police ni de taille. |
| § 3.3 motif d'annulation journalisé | ✅ | 20/10/2026 | Motif retiré du journal (`GestionPaiements` V1.2.0). |
| § 4.2 `UC_Cabinet` : cabinet + praticien + identifiants sans transaction | ✅ | 20/10/2026 | `GestionCabinet.EnregistrerFiche` : une transaction, tout ou rien ; images écrites ensuite (fichier puis nom) ; les écritures unitaires deviennent privées. Avec la « cohérence du Cabinet » du ToDo (images et cachet différés jusqu'à Enregistrer, cachet depuis la saisie). Restent au § 4.2 : `UC_DomainesHome` (règles de quota), `CreerCompteSuperUser`, séances. |
| § 5.3 garde « modifications non enregistrées », impression en consultation | 🔁 | 19/10/2026 | ADR-43 (`GardeModifications`, toutes les saisies) ; barre de consultation de l'éditeur. |
| § 4.1 réaffectation : double comptage d'une part utilisée en avoir | ✅ | 21/10/2026 | Seule la part encore portée par la séance d'origine est reportée (`PaiementEncaisse.MontantAffecte`, `ReaffecterPaiementVersSeance` V3.0.0). **Défaut voisin trouvé** : la nouvelle répartition effaçait les autres affectations d'un paiement déjà réparti sur plusieurs séances ; elles sont conservées (`GetAffectationsParPaiement`). Les séances touchées passent par la règle unique (le dû d'une séance couverte en partie n'était pas réaligné). — *Sans objet depuis le 21/10/2026 : réaffectation remplacée par l'avoir (lot B.2).* |
| § 4.1 annulation du paiement source d'un avoir déjà utilisé | ✅ | 21/10/2026 | Décision de Joëlle : refus, annoncé avant le motif (`GetMontantUtiliseEnAvoir`, `AnnulerPaiementEncaisse` V1.4.0, `UC_DossierFiche` V1.14.0). Annuler l'utilisation d'un avoir n'existe pas : à revoir avec les mouvements de paiement (lot B.5). |
| § 4.1 une seule ligne `EN_ATTENTE` par séance | ✅ | 21/10/2026 | `ResynchroniserDuSeance` V2.0.0 : la ligne de dû la plus récente est gardée (mise au reste dû ou réactivée), les autres lignes en attente sont annulées dans la même transaction. Les doublons déjà en base disparaissent à la première resynchronisation de leur séance (requête de contrôle : plan de tests Paiements P-63 ; aucune ligne sur la base de développement le 21/10/2026). |
| § 4.2 `ChangerStatutDossier` : statut de l'écran, écriture sans condition | ✅ | 21/10/2026 | Statut relu en base, transition validée sur lui, refus s'il diffère de l'écran ; `UPDATE … AND id_statut_dossier = @id_statut_courant` (`GestionDossiers` V1.4.0, `QueryDossiers` V1.2.0). **Défaut voisin trouvé** : l'archivage remettait la date de clôture à vide (le commentaire disait « conservée ») ; elle est conservée. Les dossiers déjà archivés ont perdu cette date. |
| § 5.1 code mort `QueryPaiements.SelectIdsPaiementsEnAttenteParSeance` | ✅ | 21/10/2026 | Supprimée (avec B.1). Le reste de la liste attend le lot D. |
| § 4.2 création de séance (dû avalé) ; `CreateSeance` → `UpdateNotesSeance` → `ChangerStatutSeance` orchestrés par l'écran | ✅ | 21/10/2026 | Unité de travail `DatabaseManager.ExecuterEnTransaction` (ADR-46) ; `GestionSeances.EnregistrerSeance` : identité, notes, statut et dû en une transaction ; `CreateSeance`, `UpdateSeance`, `ChangerStatutSeance` écrivent séance et dû ensemble ; la règle unique accepte la transaction de l'appelant. Restent au § 4.2 : réaffectation, règles de quota, `CreerCompteSuperUser` (lot B.2, étapes suivantes). |
| § 4.2 `ReporterSeance` : trois écritures, compensation qui peut échouer | ✅ | 21/10/2026 | Une transaction : origine reportée **si encore planifiée** (`UpdateStatutSeanceSiInchange`), séance de remplacement, dûs ; compensation supprimée (`ReporterSeance` V2.0.0, `UC_DossierFiche` V1.14.2). |
| § 4.2 `ReaffecterPaiement` non atomique | ✅ | 21/10/2026 | Décision de Joëlle : la réaffectation est **supprimée**, remplacée par l'utilisation de l'avoir de la séance (`GestionPaiements.UtiliserAvoirDeSeance`, une transaction ; `DialogUtilisationAvoir`). La correction B.1 de la réaffectation (part restante, liaisons conservées) devient sans objet ; `UtiliserAvoirPourSeance` passe aussi par `ExecuterEnTransaction`. Restent au § 4.2 : règles de quota, `CreerCompteSuperUser`. |
| § 4.2 `UC_DomainesHome` : clôture de la règle précédente + insertion, compensation manuelle | ✅ | 21/10/2026 | `GestionQuotasInami.AjouterRegleEnChaine` : arrêt (si encore ouverte, `ArreterRegleOuverte`), contrôle du chevauchement dans la transaction, insertion ; plus de rétablissement à la main ; cohérence contrôlée avant le récapitulatif (`ValiderCoherence`). Reste au § 4.2 : `CreerCompteSuperUser`. |
| § 4.2 `GestionCabinet.CreerCompteSuperUser` : compte créé puis lié (compte orphelin si la liaison échoue) | ✅ | 21/10/2026 | Compte et lien dans une transaction (`CreateUtilisateur` variante `(conn, tx)`, `EcrireLienUtilisateur`) ; praticien enregistré exigé avant ; journal par identifiants. **§ 4.2 entièrement traité** (lot B.2 terminé, ADR-46). |
| § 4.3 un `SELECT` qui écrit (`BasculerPlanifieesEchues`) ; B.3 ADR « vérité du statut de séance » | ✅ | 21/10/2026 | ADR-47, décision de Joëlle : calcul pur, plus aucune écriture à la lecture ; une planifiée passée reste « Planifiée » et s'affiche « À confirmer » (pastille ocre, filtre, résumé) ; pointage dans la fenêtre Séance en attendant l'Agenda. Séances déjà basculées laissées « Réalisée ». |
| § 4.4 reste dû défini plusieurs fois ; « échue » et « encaissé » recopiés ; trois dossiers dominants ; B.4 vues SQL | ✅ | 21/10/2026 | Décisions de Joëlle : vues SQL ; dossier dominant = vivant d'abord, puis séance la plus proche d'aujourd'hui. Migration v2.49.0 : `v_seances_solde` (réglé, facturable, reste dû, échue, avoir) et `v_dossier_dominant` (ADR-48) ; lues par `QuerySeances`, `QueryPaiements`, `QueryPatients`, `QueryDossiers` ; `SeanceListeItem` ne recalcule plus. Recensement : reste dû écrit 5 fois, « encaissé » 9 fois, « échue » 4 fois en SQL. L'absence non facturable ne doit plus rien partout (elle gardait un dû en attente et était proposée à l'encaissement) ; une ligne de paiement d'une séance non due ne montre plus de reste dû. `v_patients_synthese` prévue par le plan n'a pas été créée : la grille Patients somme la vue des séances, une seconde vue n'apportait rien. Restent au § 4.4 : quotas (deux algorithmes, cache) et drapeau no-show → B.6 ; horloge .NET / SQL ouverte. |
| § 4.1 `paiements.mode_paiement` stocke le libellé ; B.5 mode → code | ✅ | 21/10/2026 | Décision de Joëlle : **identifiant + clé étrangère** plutôt que le code (celui d'un mode non système reste modifiable). Migration v2.50.0 : `paiements.id_mode_paiement`, renseigné d'après le libellé ; libellés inconnus repris en modes inactifs ; colonne texte conservée, plus lue (ADR-49). Règlement par avoir reconnu par `est_avoir`. **Défaut voisin trouvé** : `ModePaiementEstUtilise` comparait le code au libellé (jamais d'usage trouvé) — supprimée, la clé étrangère protège. Restent au B.5 : annulation et utilisation d'avoir en colonnes typées (étapes 2 et 3). |
| B.5 colonnes typées pour l'annulation (`date_annulation`, `annule_par`, `motif`) | ✅ | 22/10/2026 | Migration v2.51.0 : `date_annulation`, `id_utilisateur_annulation` (FK `sec_utilisateurs`), `motif_annulation` ; plus de mention ajoutée au commentaire, les anciennes restent lues (décision de Joëlle). **Défauts voisins trouvés** : le nom affiché de l'utilisatrice était écrit dans le journal (« annulé (ID=…) par … ») — identifiant seulement ; la resynchronisation du dû se faisait après l'annulation, sur une autre connexion, et son échec n'était que journalisé — tout est désormais dans une transaction (ADR-46) ; une requête était écrite dans `GestionPaiements` (`SelectIdsSeancesParPaiement`). Reste au B.5 : utilisation d'avoir en liaison typée (étape 3). |
| B.5 utilisation d'un avoir tracée (table `paiements_mouvements` ou équivalent) ; § 4.1 « annuler l'utilisation d'un avoir n'existe pas » | ✅ | 22/10/2026 | Décision de Joëlle : table de liaison `lia_paiements_avoirs` (migration v2.52.0, ADR-49) — règlement par avoir ↔ chaque affectation encaissée diminuée, montant, date, utilisateur ; écrite dans la transaction de l'utilisation ; plus de mentions dans les commentaires (les anciennes restent lues). Le nom affiché ne va plus au journal. Annuler une utilisation d'avoir devient **possible** (la liaison dit quoi rendre à quelle source) : fonctionnalité non faite, à décider. **Lot B.5 terminé.** |
| § 4.4 deux algorithmes de quota ; cache des règles jamais invalidé au changement de domaine ; B.6 | ✅ | 22/10/2026 | Un seul calcul (`GestionQuotaInami.CalculerQuota`, renouvellement de date à date) pour l'onglet Séances, l'aperçu et l'alerte de la fenêtre Séance ; l'alerte simule la séance **à sa date** (**défaut voisin trouvé** : elle la comptait dans la période d'aujourd'hui). Requêtes de la première période supprimées. Règle lue d'après le domaine actuel du dossier, en mémoire par domaine (`GestionQuotasInami.GetRegleApplicableParDossier` V2.0.0). **Défaut voisin trouvé au test** : après un changement de domaine dans la fiche, le bloc Quotas du bandeau gardait l'ancien domaine — séances relues (`UC_DossierFiche` V1.17.5). Reste au B.6 : drapeau no-show (étape 2, décision de Joëlle : case dans le référentiel). |
| B.6 constante unique / drapeau `est_no_show` ; B9 (« le drapeau en référentiel reste au ToDo ») | ✅ | 22/10/2026 | Décision de Joëlle : case dans le référentiel. Migration v2.53.0 : `ref_statuts_seance.est_absence` (cochée pour ABSENTE), `v_seances_solde` recréée ; case « Absence non excusée (no-show) » dans Référentiels › Statuts de séance. Tarif, facturation, absences et « dernière séance » de la grille Patients, pastille et filtre de l'onglet Séances, aperçu : plus aucun `'ABSENTE'` dans les requêtes ni le calcul du tarif (`CODE_STATUT_ABSENTE` ne sert plus qu'à `Seance.EstAnnulee`, propriété inutilisée — lot D). **Lot B terminé.** |
| § 3.1 droits contrôlés dans l'écran ; C.1 `ContexteOperation`, garde unique `Autorisation.Exiger` (étape 1) | ✅ | 22/10/2026 | Décision de Joëlle : objet `ContexteOperation` (utilisateur + rôle courant) plutôt que des paramètres séparés (ADR-50). Garde unique `Autorisation` ; opérations déjà protégées migrées (comptes, compte du praticien, sauvegardes, restauration, licence, annulation, avoir) ; tableau rôle × opération dans `Securite_Utilisateurs.md`. Comportement inchangé ; **défaut voisin trouvé au test** : l'écran Utilisateurs réglait Nouveau / Modifier sur le rôle de base (élévation ignorée) — rôle courant. Reste au C.1 : référentiels (+ `est_systeme` en SQL), étape 3. |
| § 3.1 C.1 étape 2 : `UpdateSeance` sans verrou ni montant libre ; dossier clôturé / archivé jamais vérifié par le métier | ✅ | 22/10/2026 | Montant différent du tarif et séance réalisée et encaissée réservés au SuperUser dans `EnregistrerSeance` (décision de Joëlle : un montant enregistré laissé tel quel reste accepté) ; dossier figé refusé dans la transaction de chaque écriture (séances, paiements, avoir ; archivé : fiche et zones aussi, décision de Joëlle) ; `CreateSeance` et `ChangerStatutSeance`, inutilisées, supprimées. **Défauts voisins trouvés** : le cadenas du montant était ouvert à tous ; un montant libre était remplacé sans prévenir au changement de statut ; la fiche d'un dossier archivé restait modifiable ; fiche, paiement et zone n'annonçaient un échec que dans la barre d'état ; l'utilisation d'un avoir affichait le message technique d'une erreur. |
| § 3.1 / § 5 C.1 étape 3 : écritures des 22 (25) référentiels sans droit côté métier ; `est_systeme` en SQL dans 3 référentiels sur 12 | ✅ | 22/10/2026 | 82 écritures : `ContexteOperation` + `ReglesReferentiels.ExigerDroitEcriture` (SuperUser, règle actuelle des écrans ; ajout à la volée par un User : à cadrer, décision de Joëlle). Lignes système : condition dans les requêtes des 13 tables et refus explicite (les 4 protections silencieuses `IF(est_systeme…)` remplacées) ; `UseAffectedRows = False` explicite. 15 écritures inutilisées supprimées (décision de Joëlle). `UC_ReferentielBase` : rôle unique, `MessageBox` → `DialogChoix`, refus affichés. **Défaut voisin trouvé** : dans 10 des 13 tables système, le code n'était protégé que par l'écran (et l'activation dans 7) (« un SuperUser peut renommer `PAYE` ou `CLOTURE` »). **Lot C.1 terminé.** |
| § 5.2 C.2 étape 1 : séance reportable / encaissable et statuts proposés décidés par l'écran ; chevauchement contrôlé par la seule fenêtre ; `CreatePaiement` sans contrôle | ✅ | 22/10/2026 | Décisions de Joëlle : valideur métier rendant la liste des erreurs par champ (`ErreurSaisie`, `ErreurValidationException`, ADR-51) ; chevauchement refusé par le métier. Règles dans `GestionSeances` (`StatutPermetReport`, `StatutPermetSuppression`, `StatutPermetEncaissement`, `StatutEstProposable`, `StatutsProposes`, `ChercherChevauchement`) et `SeanceListeItem` (`EstReportable`, `EstSupprimable`, `EstEncaissable`, `RaisonNonEncaissable`), lues par les boutons et la fenêtre Séance ; `GestionPaiements.ValiderEncaissement`, lue par la fenêtre Paiement. **Défauts voisins trouvés** : `EnregistrerSeance` acceptait « Reportée » en modification (séance reportée sans remplaçante) et un créneau occupé ; `CreatePaiement` acceptait tout montant sur toute séance (non due, soldée, au-delà du reste dû) et réalignait le dû après coup, sur une autre connexion, échec seulement journalisé — contrôle et dû désormais dans sa transaction ; le **report** ne contrôlait pas le créneau de la nouvelle séance — refusé s'il est occupé ; une date de paiement vide provoquait une erreur technique. `ExisteChevauchement` et `ResynchroniserDuSeanceSansErreur` (sans appelant) supprimées. Reste au C.2 : étapes 2 (patient et fiches liées), 3 (domaines), 4 (comptes). |
| § 5.2 C.2 étape 2a : validation du patient dans `UC_PatientFiche` et nulle part dans `GestionPatients.CreatePatient` | ✅ | 23/10/2026 | Décisions de Joëlle : type d'identifiant transmis (`Patient.TypeIdentifiantNiss`, non stocké) ; NISS enregistré normalisé (migration v2.54.0) ; erreurs sur leurs champs et dans une fenêtre récapitulative (`AffichageErreursSaisie`). `GestionPatients.ValiderPatient` (nom, prénom, téléphone et code postal selon le pays, e-mail, NISS et unicité, CT1/CT2), appliqué par `CreatePatient` / `UpdatePatient`. **Défaut voisin trouvé** : le NISS était enregistré tel que tapé — deux écritures d'un même numéro contournaient l'unicité (index `uq_patients_niss` compris). Le doublon nom + prénom + naissance reste une confirmation ; relevés de Joëlle au test : homonyme sans date de naissance signalé, NIR français sans clé accepté, icône d'erreur effacée dès la correction. Reste au C.2 : étape 2b (contacts, intervenants, thérapeutes), 3 (domaines), 4 (comptes). |
| § 5.2 C.2 étape 2b : validation des contacts, intervenants et thérapeutes dans leurs fenêtres seulement ; homonyme de thérapeute refusé | ✅ | 23/10/2026 | `GestionFamilleContacts.ValiderContact`, `GestionSuivisIntervenants.ValiderSuivi`, `GestionTherapeutes.ValiderTherapeute`, appliqués par les `Create…` / `Update…` (aucun ne vérifiait rien) ; erreurs affichées par `AffichageErreursSaisie` dans les trois fenêtres. Décision de Joëlle : homonyme de thérapeute **confirmé**, plus refusé. Relevé de Joëlle : la fiche Intervenant montre aussi l'e-mail et l'adresse complète actuels du thérapeute. `SuiviIntervenant.EstTerminee` lu par la grille. Pas de migration. |
| § 5.2 C.2 étape 3 : validation du domaine dans `UC_DomainesHome` et nulle part dans `InsertDomaine` / `UpdateDomaine` ; complétude calculée par `UC_SyntheseDomaine` | ✅ | 23/10/2026 | `GestionDomaines.ValiderDomaine` (code de 3 lettres et unique, libellé obligatoire et unique, quota encore actif), appliqué par les deux écritures ; toutes les erreurs d'un coup (`AffichageErreursSaisie`, une fenêtre par erreur auparavant). `GestionDomaines.CalculerManques` (+ `ManqueDomaine`, `EtapeDomaine` en `Metier/Referentiels`), règles inchangées ; `GestionQuotasInami.GetQuotasParDomaine` remplace deux copies d'écran. Pas de migration. |
| § 5.2 C.2 étape 4 : politique de mot de passe dans `ChangePassword` seulement (le métier accepte « a »), recopiée pour les archives ; longueur du login différente selon l'écran | ✅ | 23/10/2026 | `PolitiqueMotDePasse` (règle unique, 10 pour les comptes, 12 pour les archives), appliquée par `ChangerMotDePasse`, la réinitialisation et `CreateUtilisateur` ; `GestionUtilisateurs.ValiderCompte` (login ≥ 3 et unique, nom, rôles) pour la fiche Utilisateur et le compte du praticien. **Défaut voisin trouvé** : un login déjà pris n'était signalé que par « Erreur lors de la création… » — message explicite. Lot C.2 terminé. |
| § 5.1 C.3 étape 1 : codes métier en dur (~40 dans les `Query*`, `"PAYE"`, `"ANAMNESE"`… dans le code), constantes dispersées (`GestionSeances`, `GestionDossiers`, aucune pour les paiements) | ✅ | 24/10/2026 | `Metier/Referentiels/CodesSysteme.vb` (décision de Joëlle : un point unique, ADR-54) : statuts de séance, de dossier et de paiement, types et sources de document. Le SQL est bâti sur ces constantes (`STATUT_PAYE`… dans `QueryPaiements`, `QuerySeances`, `QueryPatients`) ; aucun code de ces familles ne reste en clair dans le code. Code mort retiré : `Seance.EstRealisee`, `EstPlanifiee`, `EstAnnulee`. Sans migration ni changement de comportement (C3-01 à C3-06 validés). Reste `'PAYE'` dans la vue `v_seances_solde` (définie en base). |
| § 5.1 C.3 étape 2 : règles tirées d'un code de statut (« non tenue » en trois listes discordantes, statuts proposés à la création, lien et rôle « par défaut » d'un contact) | ✅ | 24/10/2026 | Migration v2.57.0 : `ref_statuts_seance.est_non_tenue` et `propose_creation`, `ref_liens_patient.par_defaut`, `ref_role_legal.par_defaut` ; lus par `GestionSeances` (créneau, chevauchement, statuts proposés) et `ContactEdition`. Valeurs de départ à revoir par Joëlle (Maladie libère désormais son créneau). Validé par Joëlle (C3-07 à C3-16). Lignes système devenues inutiles déverrouillées le 25/10/2026 (v2.58.0 : statuts Absence, Annulées, Maladie, lien Mère, rôle Autorité parentale). |
| § 5.2 C.4 : couplage à `Home` (≈ 60 accès `TryCast(FindForm(), Home)` / `homeForm As Home` dans 20 fichiers) ; élévation écrite 6 fois | ✅ | 25/10/2026 | Décisions de Joëlle (ADR-55) : contrat `IFenetrePrincipale` implémenté par `Home`, fourni par `UserControlContext.FenetrePrincipale` ; élévation réunie. Étape 1 (contrat, contexte, fenêtres Contact, Intervenant, Thérapeute, Séance, Paiement) faite et validée ; étape 2 (écrans Patients, fiche, dossier, Domaines, Cabinet, Utilisateurs, Thérapeutes) faite et validée ; étape 3 (recherches, hubs, élévation réunie, garde vérifiée) faite et validée — lot C.4 terminé (NAV-C4-01 à 15). |
| § 5.1 C.5 : fichiers trop gros (`UC_DossierFiche` 4 126 l., `UC_PatientFiche` 3 886 l. à l'audit) | ✅ | 25/10/2026 | Décisions de Joëlle (ADR-56) : fichier partiel par onglet lourd, les deux fiches seulement. Dossier 1 997 l. + Suivi clinique, Séances, Paiements ; patient 2 739 l. + Famille, Intervenants, Dossiers ; code déplacé à l'identique. Validé (C5-01 à C5-04) — lot C.5 terminé. |
| § 5.1 D-1 : `<OptionStrict>Off</OptionStrict>` au niveau du projet, 36 fichiers sans `Option Strict On` (38 au 25/10) ; pas de `.editorconfig` | ✅ | 25/10/2026 | Décisions de Joëlle (ADR-57). Étape 1 : inventaire par une compilation d'essai (12 erreurs BC30512, aucune dans les Designer) ; identifiants lus par `GetUInt64` ; `Option Strict On` dans les 38 fichiers et dans `Althea.vbproj` (D1-01 à D1-08 validés). Étape 2 : `.editorconfig` à la racine (réglages VS repris de `My Project`, `.vb` en UTF-8 avec BOM) ; analyseurs « Recommended », 26 règles rencontrées (3 écartées, 23 en suggestion), `NeutralLanguage` ; 0 avertissement (D1-09, D1-10 validés). |
| § 5.4 D-2 : aucun test automatisé | ✅ | 26/10/2026 | Décisions de Joëlle (ADR-58) : projet `Tests/Althea.Tests` en VB, xUnit v3, exclu de l'application ; `Tools\test.cmd`. Étape 1 : 154 tests (identifiants, sécurité, saisie), tous au vert (D2-01 à D2-03 validés). Étape 2 : interprètes Date/Heure (`Friend`), calcul du quota alimenté par une fonction ; 229 tests (D2-04 à D2-06 validés). Étape 3 : test de fumée sur la base vide `althea_tests` (structure recréée depuis la sauvegarde à chaque série), 389 requêtes acceptées ; 621 tests (D2-07, D2-08). Reste : intégration continue (D-3). |
| § 5.4 D-3 : aucune intégration continue | ⏳ | 26/10/2026 | Décision de Joëlle (ADR-59) : `.github/workflows/ci.yml`, compilation Release et tests sur Windows, chaque soir s'il y a eu un commit et à la demande ; premier passage au vert le 26/10/2026, 2 min 07 s (D3-01) ; passage du soir à constater (D3-02). |
| Lot A, point 2 : clé Syncfusion dans l'historique du miroir public | ⏳ | 23/10/2026 | Nouvelle clé mise en place par Joëlle (les anciennes ne servent plus). Reste : purger l'historique Git du miroir public (anciennes clés, ancien mot de passe). |

> **Parenthèse hors rapport (décision de Joëlle, 23/10/2026)** : après C.2, la recherche transversale (ToDo, ADR-52) est faite avant de reprendre le plan, puisqu'elle réutilise les règles que les lots B et C ont mises en un seul endroit.
>
> R1 recherche par code, R2 séances et R3 paiements d'une période : ✅ 23/10/2026 (`0508ab62`, `6ff24b32`, `0119b987`), sans migration.
>
> Elle a aussi retiré des doublons au sens du § 5.1 :
>
> - filtre, résumé et relevé des séances et des paiements (`FiltresSeances`, `FiltresPaiements`)
> - requête des listes de paiements (`QueryPaiements.SelectPaiementsBase`)
> - colonnes du planning (`QuerySeances.SelectSeancesPlanning`)
> - règle « séance passée » (`SeanceListeItem.EstMomentPasse`)
> - bornes d'une période (`AltheaDate.CorrigerBornes`).
>
> R4 patients par lieu : ✅ 24/10/2026, sans migration ; la liste des patients lit aussi l'adresse (`QueryPatients.SelectPatientsListe`) plutôt qu'une seconde requête, et la comparaison des localités est posée dans `UtilsCodePostal` (reprise par la validation code postal / localité).
>
> R5 réseau par nom : 24/10/2026 (validé), sans migration ; la comparaison sans accents ni majuscules est réunie dans `UtilsTexte` (`UtilsCodePostal.CleLocalite` la reprend).
>
> Validation code postal / localité : ✅ 24/10/2026 (v2.56.0, ADR-53) — référentiel `ref_codes_postaux_be` généré depuis bpost et le registre BeST, contrôle dans le métier (`GestionCodesPostauxBe`), aide de saisie dans les deux sens (`SaisieLocaliteBelge`). Le détour par la recherche transversale est terminé.
>
> **C.3, inventaire et découpage validés par Joëlle le 24/10/2026** (ADR-54) :
>
> - **Étape 1** codes système en un point unique (`CodesSysteme`), SQL bâti sur ces constantes, sans migration ni changement de comportement ;
> - **étape 2** drapeaux de référentiel (migration v2.57.0, Joëlle décidant pour chaque valeur) : séance « non tenue, libère le créneau » à la place des trois listes, statut « proposé à la création », lien et rôle légal « par défaut » d'un contact.
>
> **Suite du plan** : C.4 ✅ et C.5 ✅ 25/10/2026 (ADR-55, ADR-56) ; C.6 au démarrage du module Documents.
>
> **Lot D, ordre décidé par Joëlle le 25/10/2026** : l'installation, la finition UI et la documentation de fin de chantier (points 2 en partie, 3 et 4) attendent l'agenda Google, les documents (Google Docs, un peu d'IA) et l'accueil à widgets. Tout de suite :
>
> - point 4, grands documents scindés ✅ 25/10/2026 — doc UI par domaine (sommaire + `Docs/Architecture/UI/`, 5 parties), processus (sommaire + `Docs/Architecture/Processus/`, 3 parties), CHANGELOG limité au mois en cours (archive `Docs/Archives/CHANGELOG_2026.md`) ; règle `Rules.md` § 13 ;
> - D-1 (ADR-57) : `Option Strict On` pour tout le projet, `.editorconfig` et analyseurs .NET ✅ 25/10/2026 ;
> - D-2 tests automatisés (ADR-58) ✅ 26/10/2026 : socle, identifiants, sécurité, saisie, dates, heures, quota, test de fumée des requêtes (621 tests)  ; D-3 intégration continue (ADR-59) ⏳ 26/10/2026 : workflow en place, premier passage à valider.
>
> Point noté pour C.3 : `FiltresSeances` et `FiltresPaiements` sont en couche UI (listes et pastilles) ; leurs axes de filtre (métier) pourront en être séparés.
