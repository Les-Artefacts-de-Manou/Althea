# 🗓️ Althéa — Planning d'actions V1 (roadmap consolidée)

> Planning macro par lots, synchronisé avec :
> - `Plan_Conception_Metier_Althea.md` (plan directeur de la phase métier, source de vérité)
> - `Checklist_projet_V1.md` (référence détaillée)
> - `ToDo.md` (actions opérationnelles)
>
> Dernière mise à jour : 23/10/2026 — audit du 18/10/2026 : lots A, B, C.1 et C.2 terminés ; recherche transversale R1 à R3 ; éditeur de texte refondu ; UI-8 et UI-9 terminés
>
> ℹ️ **Rythme de travail** : développement solo, **sans cadence imposée**. La progression se mesure « fait / pas fait » par lot, chaque lot étant clôturé par sa documentation. Aucune estimation en semaines ni jalon daté pour les travaux à venir.

<details>
<summary>Historique des mises à jour</summary>

- 16/10/2026
  - UI-6 Domaines terminé
  - UI-7 en cours : `DialogChoix` revu en petite fenêtre du kit le 16/10, toutes les fenêtres reprises le 15/10, barre de titre du kit, Cabinet onglets Cabinet et Praticien
  - panneau Détail des référentiels et écran Paramètres sur une page le 16/10
  - restent champ d'heure, grille des Paiements

</details>

---

## Légende

- ✅ Réalisé
- 🟡 En cours
- 🔜 À lancer

---

## Phase 0 — Socle technique et sécurité

**Statut : ✅ Réalisé (complet)**

- Architecture du projet structurée
- Démarrage applicatif orchestré
- Configuration locale + sécurité secrets
- Connexion DB centralisée
- Logging centralisé
- Authentification/rôles/session/élévation

**Date de clôture : 17/05/2026**

---

## Phase 1 — Coque UI et administration

**Statut : ✅ Réalisé (complet)**

- `Home`, navigation centralisée, contexte UI : ✅
- `UC_Accueil`, `UC_AdminHome`, `UC_Parametres` : ✅
- `UC_Utilisateurs` + `UtilisateurEdition` : ✅
- `DialogChoix` personnalisé : ✅
- `UtilsIcons` centralisé : ✅
- Documentation exhaustive : ✅

**Objectif de sortie de phase :**
- Administration utilisable de bout en bout (paramètres + utilisateurs) ✅ **ATTEINT**

**Date de clôture : 07/06/2026**

**Livrables :**
- Module administration complet et opérationnel
- UI cohérente avec DialogChoix et icônes centralisées
- Documentation technique complète et à jour
- Plan de tests et validation effectuée

---

## Phase 2 — Lot 0 : socle métier (migration de schéma + UC transverses)

**Statut : ✅ Réalisé (complet)**

- Migration versionnée `volets` → `domaines` : ✅
- Migration versionnée `medecins` → `therapeutes` : ✅
- Liaison N-N `autres_suivis_patient` + référentiel `ref_roles_intervenant` : ✅
- Référentiel `ref_statuts_seance` (cycle de vie séance) : ✅
- Base UI commune `UC_ReferentielBase` (classe héritable + hooks champ supplémentaire) : ✅
- Hub `UC_ReferentielHome` (9 tuiles, droits, navigation) : ✅
- 9 UC référentiels concrets (Domaines, LiensPatient, RolesIntervenant, SituationsFamiliales, StatutsDossier, StatutsSeance, TypesDocuments, TypesRendezVous, TypesSeance) : ✅
- Composant `UC_RichTextEditor` (éditeur complet 30 boutons, impression, PDF/Word) : ✅
- Composant `UC_RichTextEditorSimple` (variante allégée 7 boutons, double format) : ✅ (remplacé le 19/10/2026 par le profil Compact d'`UC_RichTextEditor`, ADR-42)

**Objectif de sortie de phase :** ✅ **ATTEINT**
- Schéma et briques UI transverses alignés avec le plan de conception, prêts pour le codage métier

**Date de clôture : 10/06/2026**

**Livrables :**
- 9 piles applicatives référentiels complètes (Query / Modèle / Gestion / UC)
- Architecture générique extensible (`UC_ReferentielBase` + hooks)
- Cas spécial `UC_TypesSeance` avec `tarif_defaut` géré par hook sans modifier la base
- Deux éditeurs de texte riche réutilisables (`UC_RichTextEditor` + `UC_RichTextEditorSimple`)
- Build complet validé sans erreur

---

## Phase 3 — Lot 1 : cœur métier patients/dossiers

**Statut : ✅ Réalisé (complet)**

**Date de clôture : 21/07/2026**

**Périmètre :**
- Module métier `GestionPatients` + `QueryPatients`
- Recherche patients (filtres avancés)
- Fiche patient multi-onglets (identité, administratif, famille, suivi, documents, facturation, archive)
- Réseau d'intervenants externes (N-N via `autres_suivis_patient` + `ref_roles_intervenant`)
- Module métier `GestionDossiers` + `QueryDossiers`
- Gestion dossiers + statuts (actif/pause/clôturé/archivé)
- Transitions de statuts sécurisées, réouverture (même domaine) et transfert inter-domaines
- Le « suivi » du patient est **dérivé à la volée** de l'état du dernier dossier (priorité aux dossiers vivants ACTIF/PAUSE) : plus de colonne `patients.suivi_en_cours`. L'activation du patient est gérée séparément par `is_actif` (bascule manuelle, migration v2.8.0), exploitée par le filtre/pastille de `UC_PatientHome`

**Objectif de sortie de phase :**
- Parcours patient opérationnel minimal en production interne ✅ **ATTEINT**
- Gestion de base des dossiers avec cycle de vie complet ✅ **ATTEINT**

---

## Phase 4 — Lot 2 : suivi clinique et paiements

**Statut : 🟡 Largement réalisé (exports financiers et recette transversale restants)**

**Périmètre :**
- Paramétrage Domaine = convention (tarif + organisme + quotas par catégories, ADR-27) via `UC_DomainesHome` — réalisé 30/09–01/10/2026 (schéma 2.28.0)
- Module métier `GestionSeances`
- Séance créée dès la planification du rendez-vous lié, statut piloté par `ref_statuts_seance`
- Gestion des séances (saisie, notes, synthèse)
- Historique de suivi avec tri/filtrage
- Module métier `GestionPaiements` + `QueryPaiements`
- Gestion paiements liée aux séances
- Suivi financier (solde patient, paiements en attente)
- Exports de base (liste paiements, balance)
- Intégrité et règles revues par l'audit du 18/10/2026 (lots B et C, 21–23/10/2026) : opérations d'un seul tenant, solde défini dans une vue SQL, paiements tracés en base, droits et règles de saisie côté métier
- Liste des paiements d'une période avec totaux encaissés par mode : recherche transversale R3 (23/10/2026)

**Objectif de sortie de phase :**
- Parcours de suivi complet du patient (hors documents/agenda avancés)
- Gestion financière opérationnelle

---

## Chantier transverse — Refonte de l'interface (UI-0 à UI-9)

**Statut : 🚧 En cours** — plan validé le 10/10/2026, détail dans `Plan_Refonte_UI_Althea.md`. UI-0 à UI-6 ✅ (UI-6 le 14/10/2026) ; UI-8 nettoyage ✅ et UI-9 grilles en pastilles ✅ (18/10/2026) ; UI-7 (fenêtres et champs) 🟡 en cours.

Remplacement de l'habillage par images de fond étirées par un jeu de contrôles dessinés par le code (`UI/Controls/Theme/`). Mené **en parallèle** des lots métier 30 à 33, sans en modifier la numérotation ni le contenu. Se termine par un nettoyage complet des fichiers et assets inutilisés.

---

## Phase 4 bis — Habillage de la carcasse (fondation cabinet et paramétrage)

**Statut : 🔄 En cours** — décidé le 07/10/2026, avant Agenda et Documents.

- Lots 27 et 29 réalisés, lot 28 partiel, lots 30 à 32 à faire, lot 33 avec la Phase 6 (point du 11/10/2026).
- Point du 23/10/2026 : lots 27 à 30 réalisés (restent la vérification de l'import du logo cabinet et de la signature, et les tests de restauration 7.4, 7.5, 7.15) ; lots 31 et 32 à faire.

**Principe directeur** : Althéa doit pouvoir être installée dans **n'importe quel cabinet**.

Aucune donnée propre à un praticien (identité, numéros, cachet, photo) n'est codée en dur ni insérée par script SQL : tout est saisi et importé par l'interface.

Les images de `Assets/Praticien/` ne sont que des exemples de travail, gardés hors du dépôt (photo retirée du suivi Git et ignorée depuis le 17/10/2026, UI-8).

| Lot | Contenu | Migration |
| --- | --- | --- |
| 27 | **Fondation Cabinet** : cabinet, praticiens, identifiants professionnels (référentiel), qualifications, images (photo, signature, logo) ; **Cachet généré** par l'application depuis les données du praticien (modèle : `Assets/Praticien/Cachet_modele.svg`), avec possibilité d'importer un scan. ✅ **Réalisé (06/10/2026)**, complété par **v2.30.0** (suppression de `couleur_agenda` du praticien) et **v2.31.0** (`ref_situations_juridiques`, `cab_cabinet.id_situation_juridique`, porteur du BCE), contrôles IBAN/BIC, habillage Designer. Reste : vérifier l'import du logo cabinet. Logo des domaines ✅ 20/10/2026 (v2.45.0). | v2.29.0 → v2.31.0 |
| 28 | **Utilisateurs** : photo, lien praticien, préférences ; le SuperUser (élévation Admin via cadenas) est lié à la fiche praticien et reste prêt pour un futur mode multi-praticiens (V2/V3) sans refonte. ✅ **Lien praticien réalisé (08/10/2026)** : v2.32.0 (`cab_praticiens.id_utilisateur` nullable/unique), section « Compte de travail » dans `UC_Cabinet` (créer / lier / délier), `DialogComptePraticien` ; **le compte Admin n'est jamais modifié**. Reste : photo et préférences utilisateur. | v2.32.0 |
| 29 | **Organismes étoffés** : coordonnées (adresse, téléphones, e-mails, site), plateforme, périodicité (référentiel), procédure. ✅ **Réalisé (09/10/2026)** : périmètre minimal, nouveau référentiel `ref_periodicites` / `UC_Periodicites`. Reste : exécuter les tests §14.12 et §14.16 de `PLAN_TESTS_REFERENTIELS.md`. *(ligne reconstituée le 11/10/2026 : le texte d'origine était tronqué dans le fichier)* | v2.33.0 |
| 30 | **Paramètres enrichis** : séances, sécurité, chemins disque vérifiés. Détail à préciser avant de lancer le lot. *(ligne reconstituée le 11/10/2026 : le texte d'origine était tronqué dans le fichier)* | v2.37.0 prévue (v2.34.0 / v2.35.0 : référentiels de saisie ; v2.36.0 : photo utilisateur) |
| 31 | **Tableau de bord d'Accueil** : socle des widgets et premiers widgets sur données existantes, voir ci-dessous. *(ligne reconstituée le 11/10/2026 : le texte d'origine était tronqué dans le fichier)* — **reporté en toute fin de projet** (décision du 12/10/2026 : après Agenda et Documents, quand tous les processus existent), avec les préférences utilisateur. | au lancement |
| 32 | **Facturation** : encodage mensuel, mutualités, numérotation des documents (notes d'honoraires, reçus). *(ligne reconstituée le 11/10/2026 : le texte d'origine était tronqué dans le fichier)* | au lancement |
| 33 | **Exploitation** : assistant de premier démarrage (base → admin → cabinet → praticien → référentiels), export/import de la configuration, RGPD (export dossier patient, durée de conservation, anonymisation). | avec Phase 6 |

### Reste à faire — consolidé le 11/10/2026 (tableau ci-dessus + plans Copilot « Habillage de la carcasse — suite du lot 27 et lots 28-29 » et « Lot 29 »)

État vérifié dans le code et la base (dernière migration : v2.33.0). Regroupement proposé, à valider :

| Groupe | Contenu | État |
| --- | --- | --- |
| Suite lot 27 | Vérifier l'import du logo cabinet et de la signature ; **logo de domaine** avec miniatures (grille et fiche Domaines, synthèse, **choix du domaine dans la fiche Dossier**) — décision du 11/10 : fichier `Domaines/Domaine_{id}/Logo.png` dans le dossier de données (nommé par l'identifiant, pas par le libellé), nom de fichier seul dans une nouvelle colonne `ref_domaines.logo_fichier`, enregistrement à l'import ; miniature masquée sans logo | ✅ logo des domaines réalisé et testé le 20/10/2026 (v2.45.0 : étape 1 de l'écran Domaines, convention, bandeau et liste des domaines du dossier) ; reste : vérifier l'import du logo cabinet et de la signature |
| Suite lot 28 | **Photo utilisateur** (décision du 12/10 : un compte lié à un praticien affiche la photo du praticien ; photo propre sinon ; initiales à défaut) avec l'habillage UI-7 de `UtilisateurEdition` ; les **préférences utilisateur** passent avec le lot 31 ; le **journal d'audit** des actions SuperUser / Admin est reporté (cadrage préalable) | ✅ photo réalisée le 12/10 (v2.36.0, `PhotosUtilisateur`, `CheminsUtilisateurHelper`, pastille de la Home, `UtilisateurEdition` en cartes) — tests validés |
| Administration | **Consultation des logs** (tuile existante sans écran : choix du jour, filtres niveau / catégorie, recherche ; vérification que les actions importantes sont journalisées en entier, sans donnée sensible) ; **page « État du système »** (version de l'application et du schéma, migrations manquantes, chemins et espace disque, connexion) — ajoutés le 12/10/2026 | ✅ réalisé et validé le 13/10 (`UC_Journaux`, `UC_EtatSysteme`, ADR-39 : journaux sans données personnelles, `VersionSchema.Attendue`) |
| Référentiels de saisie | **Mutualités** (liste belge en seed, unions nationales à vérifier, FK patient + **numéro d'affilié du patient** (champ dédié dans la fiche) ; depuis la fiche Patient : **consulter la fiche complète** de la mutualité choisie et **en ajouter une nouvelle** sans quitter la fiche — demande du 11/10), **Pays** (Belgique + FR, LU, NL, DE), **Civilités** ; branchement des écrans concernés | 🔄 **R1 réalisé le 11/10** (v2.34.0 : `ref_pays` ISO — 5 lignes système, `ref_civilites`, mutualités = `ref_organismes.type_organisme = MUTUALITE` + 7 organismes assureurs, colonnes de rattachement, écrans `UC_Pays` / `UC_Civilites`, type dans `UC_Organismes`). **R2 réalisé le 12/10** (pays et civilité dans les écrans de saisie, listes fermées, contrôles par code ISO, pays par défaut = pays du cabinet). **R3 réalisé le 12/10** (mutualité, n° d'affiliation, CT1 / CT2 sur la fiche Patient ; fiche ou ajout de mutualité en fenêtre modale). **R4 réalisé le 12/10** (v2.35.0 : suppression des colonnes texte). ✅ Groupe terminé |
| Transverse | **Import d'image unifié** (patient, praticien, logos) ; **bandeau de la Home** (logo cabinet, praticien, domaine courant) | 🔄 Bandeau réalisé le 11/10 (maquette « Home A » : cabinet, praticien, fil d'Ariane avec pastille du domaine, utilisateur, menu) — reste la miniature du logo de domaine dans la pastille (Suite lot 27) ; import d'image unifié ✅ 11/10 (`ImagesHelper` + `UtilsImages` : formats communs, contrôle réel du fichier, confirmation de remplacement partout) |
| Lot 30 | Paramètres enrichis : séances, sécurité, chemins disque vérifiés, **couleur d'encre du cachet généré** (bleu nuit par défaut ou noir — aujourd'hui `GenerateurCachet.CouleurEncre` en dur, décision du 11/10) ; **écran Sauvegarde / restauration** (la tuile existe dans le hub Administration) | 🔄 découpé le 12/10 : **P1** paramètres enrichis ✅ (v2.37.0 : durée et grille des séances, sécurité, journaux, encre ; arborescence fixe du dossier de données, ADR-35 ; valeur unique sans dates de validité, écran limité à la valeur, ADR-36 — validé ; écran repensé en UI-7) ; **P2** sauvegarde ✅ (v2.38.0, ADR-37 : ZIP AES-256, dump `mariadb-dump --skip-add-locks` + dossier de données + manifeste, mot de passe DPAPI, rotation, journal, rappel, `UC_Sauvegardes` — validé) ; **P3** restauration ✅ (v2.39.0, ADR-38 : Admin, même version, compte d'administration MariaDB saisi, sauvegarde de sécurité, ancien dossier conservé, remise en état automatique, redémarrage — validé le 13/10 (sauf 7.4 / 7.5 à revérifier ; 7.15 reporté)) |
| Lot 31 | Tableau de bord d'Accueil (widgets) | ⬜ |
| Lot 32 | Facturation (s'appuie sur mutualités, situation juridique, périodicités) | ⬜ |
| Lot 33 | Assistant de premier démarrage, export de configuration, RGPD | avec la Phase 6 |


### Ordre de travail — décidé le 12/10/2026

1. Photo utilisateur + habillage de `UtilisateurEdition` (fin du lot 28) — ✅ réalisé et validé le 12/10
2. Lot 30 — Paramètres enrichis et **Sauvegarde / restauration** (base **et** dossier de données) — 🔄 P1 réalisé et validé le 12/10 ; P2 (sauvegarde) réalisé et validé le 12/10 ; P3 (restauration) réalisé et validé le 13/10 (sauf 7.4 / 7.5 à revérifier ; 7.15 reporté)
3. Administration — consultation des logs, page « État du système » — ✅ réalisé et validé le 13/10
4. **Fin de la refonte UI** :
   - UI-6 (Domaines) — ✅ réalisé et validé le 14/10 ;
   - UI-7 (fenêtres et champs) — 🔄 `DialogChoix` V2 et champ de date du kit faits le 14/10 ;
     - fiches patient (onglet Identité) et dossier faites ;
     - fenêtres Contact, Intervenant, Thérapeute, Utilisateur (14/10), puis **toutes les autres fenêtres le 15/10** (règle du 15/10 : champs des fenêtres en cartes) ;
     - barre de titre du kit ;
     - `UC_Cabinet` onglets Cabinet et Praticien (15/10), `DialogChoix` en petite fenêtre du kit et onglets Identifiants / Cachet, vitrine du kit, panneau Détail des référentiels (16/10), `UC_Parametres` sur une page avec groupes en base (16/10, v2.40.0), champ d'heure du kit `AltheaHeure` (16/10, à tester) ;
     - grille des Paiements en relevé chronologique (17/10) ;
     - plus aucun `DateTimePicker` Windows, boutons inactifs qui disent pourquoi (20/10)
5. Lot 32 — Facturation
6. Journal d'audit (cadrage puis réalisation)
7. UI-8 (nettoyage) — ✅ réalisé et validé le 18/10 — et revue de code générale — 🔄 audit du 18/10/2026 : lots A, B, C.1, C.2 ✅ (20–23/10)
8. Phase 5 — Agenda et Documents, avec la refonte du `RichTextEditor` (✅ faite en avance les 19–20/10, ADR-42 à ADR-45), branchés sur Patient / Dossier / Séance
9. Lot 31 — Tableau de bord d'Accueil (widgets) et préférences utilisateur, **en dernier**
10. Lot 33 — Exploitation (assistant de premier démarrage, export de configuration, RGPD), avec la Phase 6, précédé de la **préparation de l'installation** (inventaire des données de départ, scripts, choix du dossier de données — ajouté le 12/10)

> **Point du 23/10/2026** : la revue de code (point 7) a été avancée (décision de Joëlle du 20/10/2026 : passer tous les points de l'audit en revue).
>
> Fait : lots A, B, C.1, C.2 et, en parenthèse, la recherche transversale R1 à R3 (ADR-52).
>
> **Suite validée par Joëlle** :
>
> - R4 patients par lieu →
> - R5 réseau par nom (contact, thérapeute, intervenant et leurs patients) →
> - validation code postal / localité (codes postaux belges) →
> - audit C.3 (codes système centralisés, drapeaux de référentiel) →
> - C.4 (découplage des écrans de `Home`) →
> - C.5 (découpage des gros fichiers) ;
> - C.6 au démarrage du module Documents.
>
> Documents et Agenda restent à intégrer depuis les POC.

**Tableau de bord d'Accueil (`UC_Accueil`)** — « tableau de bord du praticien »
- Composé de **widgets** indépendants (UserControls héritant d'un `UC_WidgetBase`), déclarés dans un référentiel `ref_widgets` (code, libellé, rôle minimum, disponible ou non).
- **Configurable par utilisateur** (`usr_widgets_accueil` : widget, position, taille, visible) : chaque praticien choisit et ordonne ses widgets.
- **Disponibles dès le lot 31** (données existantes) :
  - Recherche rapide patient → ouverture directe de son dossier actif
  - Derniers patients / dossiers consultés
  - Séances passées non soldées (impayés)
  - Quotas proches du plafond
  - Séances à encoder ce mois (organismes conventionnés, ex. INAMI / Realism)
  - Anniversaires des patients du jour / de la semaine
  - Liens rapides paramétrables (site du cabinet, plateformes des organismes, favoris d'écrans)
  - Alertes et notifications (seuils paramétrables)
- **Après Agenda (Phase 5)** : rendez-vous du jour avec accès direct au dossier, création rapide d'un RV pour un patient, séances passées non validées.
- **Après Documents (Phase 5)** : Todo du jour (documents à produire, attestations, relevé mensuel de facturation), derniers documents.
- **Plus tard** : statistiques (patients actifs, séances du mois, chiffre d'affaires), Todo personnelle.

**Terminologie** : dans l'interface, la personne connectée est désignée comme **praticien** lorsqu'elle est liée à une fiche `cab_praticiens` ; « utilisateur » reste réservé à la gestion des comptes (connexion, rôles).

**Rythme** : un lot à la fois, chaque lot livré avec sa migration validée au préalable, son plan de test et sa documentation.

> **Rappel (10/10/2026)** : les lots restants de cette phase sont à terminer **avant** d'entamer la Phase 5. Le chantier UI se poursuit en parallèle ; une revue de code générale est prévue avant la Phase 5 (voir ToDo, section G quater).

---

## Phase 5 — Lot 3 : intégration documents et agenda (depuis POC)

**Statut : 🔜 À lancer**

**Périmètre documents :**
- Module métier `GestionDocuments`
- Stockage **hors base de données** (système de fichiers), chemin déterministe calculé
- Flux Word local **et** Google Docs, synchronisation à la sauvegarde, export PDF
- Catégorisation par type (rapport, synthèse, courrier)
- Liaison documents ↔ patients/dossiers
- Recherche documents
- Envoi par email + historique

**Périmètre agenda :**
- Module métier `GestionAgenda`
- Google Calendar comme **pilier** (synchronisation bidirectionnelle, reprise assistée)
- Gestion rendez-vous de base (date, heure, patient, type)
- Vue calendrier mensuelle/hebdomadaire (Scheduler)
- Création rapide depuis fiche patient
- Notifications/rappels
- Liaison rendez-vous ↔ séances
- Modèle préparé pour le multi-utilisateur / multi-agenda (activation différée)

**Précisions du 12/10/2026 (Joëlle)** — à revoir ensemble au lancement de la phase, alternatives bienvenues (y compris remettre en cause le flux Word) :
- Les POC existants, volontairement restreints, servent de marche à suivre.
- **Agenda** : Pearl utilise aujourd'hui Google Agenda. Il faudra **reprendre ses données existantes**, puis **inscrire les séances** dans l'agenda Google, avec une **consultation dans l'application par synchronisation**.
- **Documents** : une **copie locale** et une **copie sur Google Drive** (accès hors de l'application si nécessaire), avec à chaque fois une **copie PDF** pour une consultation rapide sans ouvrir Word.
- La **refonte du `RichTextEditor`** fait partie de cette phase. *(Faite en avance les 19–20/10/2026 : ADR-42 à ADR-45.)*
- Les deux processus sont branchés sur Patient / Dossier / Séance.

**Objectif de sortie de phase :**
- Couverture fonctionnelle globale V1 avec modules historiques prioritaires
- Intégration POC dans application principale

---

## Phase 6 — Exploitation, installation, stabilisation

**Statut : 🔜 À lancer**

**Périmètre :**
- Sauvegarde/restauration complète (DB + fichiers) — ✅ réalisée en avance au lot 30 (12–13/10/2026, ADR-37 / ADR-38)
  - Sauvegarde automatique quotidienne — non faite (rappel à l'ouverture en place ; à décider)
  - ✅ Sauvegarde manuelle à la demande
  - ✅ Localisation sécurisée
  - ✅ Tests de restauration (7.4, 7.5, 7.15 restants)
- Installation/configuration initiale
  - Assistant de configuration DB
  - Création utilisateur admin initial
  - Import/export paramètres
- Campagne de tests fonctionnels
  - Tests parcours complets (patients, dossiers, séances, paiements, documents, agenda)
  - Tests sécurité et permissions
  - Tests limites et erreurs
- Corrections UX finales
- Finalisation documentation utilisateur

**Objectif de sortie de phase :**
- Version V1 stabilisée, installable, maintenable
- Application prête pour déploiement production

---

## Jalons de pilotage

| Jalon | Description | Statut |
|-------|-------------|--------|
| **J0** | Socle technique complet | ✅ Atteint (17/05/2026) |
| **J1** | Administration complète | ✅ Atteint (07/06/2026) |
| **J2** | Cadrage de la phase métier (décisions actées) | ✅ Atteint (08/06/2026) |
| **J3** | Lot 0 : socle métier migré (schéma + UC transverses + référentiels) | ✅ Atteint (10/06/2026) |
| **J4** | Lot 1 : patient/dossier minimal viable | ✅ Atteint (21/07/2026) |
| **J5** | Lot 2 : suivi métier complet (séances + paiements) | ✅ Atteint (26/09/2026) |
| **J6** | Lot 3 : couverture transverse (documents + agenda) | 🔜 À lancer |
| **J7** | Go V1 interne | 🔜 À lancer |

---

## Indicateurs d'avancement

### Modules implémentés : 8/9 (89%)

- ✅ Socle technique (démarrage, config, DB, logs, sécurité)
- ✅ Navigation et shell UI
- ✅ Administration (paramètres, utilisateurs)
- ✅ UI cohérente (DialogChoix, UtilsIcons)
- ✅ Documentation exhaustive
- ✅ Lot 0 : socle métier (migration schéma + UC référentiels + éditeurs de texte)
- ✅ Lot 1 : Patients + Dossiers (cycle de vie complet)
- ✅ Séances + Paiements (tarification par domaine, quotas par catégories)
- 🔜 Documents + Agenda

### Documentation : 100%

- ✅ README, Rules, Process, Database, Standards
- ✅ Architecture Decisions (52 ADR)
- ✅ Plan de conception métier (décisions D-13 à D-19 actées)
- ✅ Documentation UI complète
- ✅ CHANGELOG détaillé
- ✅ Plans de pilotage (Checklist, ToDo, Planning, État du projet)

### Tests

- ✅ Tests administration (UC_Utilisateurs, sauvegardes, restauration, journaux et état du système validés)
- 🔄 Plans de tests par module métier (patients, dossiers, séances, paiements, domaines…) exécutés au fil des lots ; enrichis à chaque étape de l'audit (lots A à C) ; nouveau plan `PLAN_TESTS_RECHERCHE.md` (RCH-01 à RCH-39, 23/10/2026)
- 🔜 Tests documents + agenda
- 🔜 Tests transverses et stabilisation

---

## Notes de suivi

- **Phase 1 achevée avec succès** : Le module d'administration est complet, documenté et validé.
- **Cadrage métier acté (08/06/2026)** : décisions D-13 à D-19 figées dans le [plan de conception](../Conception/Plan_Conception_Metier_Althea.md), les ADR et Rules.
- **Lot 0 achevé (10/06/2026)** : schéma migré, 9 référentiels déployés (piles complètes), `UC_ReferentielBase` avec hooks extensibles, `UC_RichTextEditorSimple` ajouté. Build validé.
- **Priorité suivante** (23/10/2026) : recherche R4 et R5, validation code postal / localité, puis audit C.3 → C.4 → C.5 (voir le point du 23/10/2026 sous l'ordre de travail).
- **Priorité suivante** (14/10/2026, historique) : UI-7 — champ de date du kit, puis écrans un par un (champs du kit et lecture seule lisible) ; ancienne formulation : tour « lecture seule lisible » des écrans déjà faits, puis UI-7 (en commençant par la maquette de `DialogChoix`).
- **Rythme** : travail solo sans cadence ; chaque lot est clôturé par sa documentation avant de passer au suivant.
- **Contrainte connue** : Build/test .NET bloqués sur Linux pour ce projet WinForms cible Windows (à prendre en compte pour CI et validation).
- **Risque feature creep** : Gel du périmètre V1 recommandé avant de démarrer chaque lot métier.

---

## Réalisations Phase 2 – Lot 0 (09-10/06/2026)

### Référentiels complets ✅
- `UC_ReferentielBase` : base générique héritable avec hooks champ supplémentaire
- `UC_ReferentielHome` : hub 9 tuiles, droits, navigation
- 9 UC référentiels concrets déployés avec pile complète (Query/Modèle/Gestion/UC)
- Cas spécial `UC_TypesSeance` : `tarif_defaut` géré par hook sans modifier la base commune
- Build validé sans erreur

### Éditeur de texte simple ✅
- `UC_RichTextEditorSimple` : variante allégée (7 boutons), double format RTF+TXT, contexte optionnel

---

## Réalisations Phase 1 (23/05-07/06/2026)

### Gestion utilisateurs complète ✅
- UC_Utilisateurs : liste, recherche/filtres, actions admin
- UtilisateurEdition : 3 modes (création/modification/consultation)
- GestionUtilisateurs : CRUD complet, reset password, déverrouillage
- Gestion rôles Admin/SuperUser

### UI/UX cohérente ✅
- DialogChoix : remplacement complet MessageBox (34 occurrences)
- UtilsIcons : centralisation icônes d'état avec priorités
- Support icônes animées GIF

### Documentation exhaustive ✅
- Mise à jour complète de tous les documents de référence
- Nouvelle documentation UI (DialogChoix, UtilisateurEdition, UC_Utilisateurs)
- Plan de tests créé et validé

### Tests et validation ✅
- Tests manuels complets effectués
- Bugs identifiés et corrigés
- Workflows validés



# Contact

> ***Joëlle De Poorter - Les Artefacts de Manou***
>
> Projet réalisé pour soutenir une pratique de psychologie et de graphologie dans la gestion fiable et évolutive des patients et documents.
>
> - Site web Auteur/Les Artefacts de Manou/Joëlle De Poorter : https://lesartefactsdemanou.be
>
> - Site web Psychologue & Graphologue:  https://pearlnguyenduy.be/
> - mailto: `joelle@nguyen.eu` et `lesartefactsdemanou@gmail.com`
>
> - GitHub privé: Althea    https://github.com/AngeljoNG/Althea
> - GitHub public : https://github.com/Les-Artefacts-de-Manou/Althea
>
> ---



[TOC]

