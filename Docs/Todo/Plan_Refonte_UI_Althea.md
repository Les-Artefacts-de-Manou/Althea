# 📌 Althéa — Plan de refonte de l'interface (chantier UI)

> Chantier transverse, mené en parallèle des lots métier (27 à 33) sans en modifier la numérotation.
> Maquettes de référence : artifact **« Althéa – Kit d'interface »** (kit, hubs, liste, dialogue, Domaines, Cabinet).
> Charte couleurs et contrôles : `../Rules/Charte_Graphique.md`.
*Dernière mise à jour : 19/10/2026 — `UC_Pagination` dans le thème, éditeur de texte refondu (temps 1, ADR-42)*

<details>
<summary>Historique des mises à jour</summary>

- 16/10/2026
  - lots UI-0 à UI-6 livrés
  - UI-7 en cours (`DialogChoix` V2, revu en petite fenêtre du kit le 16/10 (V2.1, maquette C), champ de date du kit, fiches Patient et Dossier, fenêtres Contact, Intervenant, Thérapeute, Utilisateur, Séance, Paiement, dialogues du dossier, connexion, catégories de quota, zone clinique, restauration, compte praticien —
    - toutes les fenêtres sont reprises ;
    - règle « fenêtres en cartes » ;
    - barre de titre du kit ;
    - panneau Détail des référentiels et écran Paramètres sur une page le 16/10)

</details>

---

## 1. Objectif

Remplacer l'habillage par **images de fond étirées** par un jeu de **contrôles dessinés par le code**, réglables dans le Designer, cohérents sur tous les écrans.

**Pourquoi** : une image de bouton ou de panneau étirée change d'épaisseur de bord, d'arrondi et d'ombre selon la taille du contrôle ; elle doit être posée contrôle par contrôle ; et toute retouche de style oblige à repasser partout.

**Résultat visé** : une couleur, un arrondi ou une ombre se changent **à un seul endroit** et tous les écrans suivent.

### Principes

1. **Aucune régression fonctionnelle.** Le placement des composants et le comportement métier restent ceux d'aujourd'hui. Les réorganisations de mise en page sont signalées et validées séparément.
2. **Designer d'abord** (règle du 06/10/2026). Les contrôles exposent leurs réglages dans la catégorie « Althéa » du Designer. Le code ne réapplique pas de thème au runtime.
3. **Couleurs de la charte uniquement**, via `UITheme`. Aucune couleur codée en dur dans un écran.
4. **Un lot = un état cohérent, testé, documenté**, avant de passer au suivant.
5. **Réversibilité.** Chaque lot doit pouvoir être annulé par `git revert` sans casser les autres.

---

## 2. État des lieux (10/10/2026)

| Élément | Nombre | Remarque |
| --- | ---: | --- |
| Forms | 21 | hors `UI/Forms/Test` |
| UserControls avec Designer | 16 | |
| Référentiels héritant de `UC_ReferentielBase` | 11 | **sans Designer propre : traités d'un coup via la base** |
| Boutons (`Button`) | 203 | à migrer vers `AltheaBouton` / `AltheaTuile` |
| `TabControl` | 6 | `UC_Cabinet`, `UC_Parametres`, `UC_DomainesHome`, `UC_DossierFiche`, `UC_PatientFiche`, `SeanceEditor` |
| `TabPage` | 21 | |
| `GroupBox` | 22 | à remplacer par `AltheaTitreSection` |
| `DataGridView` | 17 | déjà centralisés par `UtilsDataGrid` |
| `BackgroundImage` dans les Designers | 153 | dont 31 `picTitre` (icônes, à conserver), 22 `pnlActions`, 17 `pnlForm` |

**Images embarquées dans les `.resx`** (à retirer au fur et à mesure) : `UC_ReferentielHome` 21, `UC_AdminHome` 11, `UC_ReferentielBase` 9, `UC_Cabinet` 7.

---

## 3. Les contrôles du kit

### 3.1 Déjà créés — 09/10 et 10/10/2026

Répertoire `UI/Controls/Theme/`. **Statut : compilés (UI-0) et posés dans les écrans des lots livrés.**

| Contrôle | Hérite de | Rôle | État |
| --- | --- | --- | --- |
| `AltheaDessin` | *(Module)* | Outils de dessin partagés : chemins arrondis, ombres, teintes d'état, titre de section. | ✅ Compilé |
| `AltheaBouton` | `Button` | Bouton arrondi, variantes **Sauge / Ocre / Terre / Clair**, états survol / enfoncé / désactivé. | ✅ Compilé |
| `AltheaTuile` | `AltheaBouton` | Tuile de hub : icône en pastille, titre + sous-titre (`Text` sur deux lignes). | ✅ Compilé |
| `AltheaCarte` | `Panel` | Panneau translucide arrondi, ombre douce, titre de section facultatif. | ✅ Compilé |
| `AltheaBarreActions` | `AltheaCarte` | Barre d'actions du bas, vague dessinée (remplace `Althea_Bandeau_Haut_Trans.png`). | ✅ Compilé |
| `AltheaTitreSection` | `Label` | Titre de groupe + trait fin (remplace les `GroupBox`). | ✅ Compilé |
| `AltheaOnglets` | `TabControl` | Barre d'onglets soulignés, fond transparent possible (`CouleurFond`), police héritée du parent. Voir § 4 et ADR-32. | ✅ Posé (UI-2) |
| `AltheaEnTete` | `Panel` | En-tête d'écran : icône, titre Mauve foncé, sous-titre Sauge foncé **sous** le titre, trait fin. Remplace le trio `picTitre` / `lblTitreForm` / `lblTop`. | ✅ Posé (UI-3) |
| `AltheaRecherche` / `AltheaListe` / `AltheaCase` | `UserControl` / `ComboBox` / `CheckBox` | Champs des bandeaux de recherche (ADR-33), réutilisés au lot UI-7. | ✅ Posés (UI-4) |
| `AltheaBarreOutils` | `Panel` | Barre locale d'une liste, en haut de sa carte (ADR-34). | ✅ Posée (11/10) |
| `AltheaJauge` | `Control` | Jauge Sauge / Jaune modéré / Rouge moyen clair (quota du récapitulatif patient). | ✅ Posée (UI-5) |
| `AltheaPastille` | `Label` | Libellé court sur fond arrondi (code, organisme, natures, statut patient). | ✅ Posée (UI-5) |
| `AltheaBoutonMenu` / `AltheaFilAriane` / `AltheaPuceUtilisateur` | `Button` / `Label` / `Control` | Menu, fil d'Ariane et utilisateur du bandeau de la Home (maquette « Home A »). | ✅ Posés (11/10) |
| `AltheaLigne` | `Button` | Ligne cliquable : icône, titre, aperçu, « Ouvrir » (zones du Suivi clinique). | ✅ Posée (UI-5) |
| `TestKitAlthea` | `Form` | Vitrine du kit (`UI/Forms/Test`) : tous les contrôles dans leurs états (V2.0.0, 16/10/2026). | ✅ Conservée : banc de vérification et source des captures de `Reference_UI.md` |

**Également livré le 09/10/2026, en production :** fond fixe de la Home (2560x1440, `Layout = None`), logo séparé, feuillage dessiné à taille réelle, et **« feuille coupée » automatique** — la Home redessine sur chaque UserControl hébergé la portion de feuillage qu'il recouvre. ✅ Vérifié à l'exécution.

### 3.2 À créer

| Contrôle | Rôle | Lot |
| --- | --- | --- |
| ~~*Bandeau(x)*~~ | ✅ Bandeau de recherche (UI-4) et récapitulatif patient de la fiche Dossier (UI-5) : maquettes « Althéa – Bandeaux », réalisés sans contrôle dédié (carte + champs du kit). | UI-4, UI-5 |
| `AltheaEtapes` | Étapes numérotées de l'assistant Domaines (pastille pleine / coche / cercle vide). | UI-6 ✅ (14/10/2026) |
| `AltheaDate` / `AltheaCalendrier` | Champ de date du kit (saisie, date facultative, lecture seule) et calendrier dessiné. | UI-7 ✅ (14/10/2026) |
| `AltheaFondFenetre` | Fond des fenêtres : beige clair uni, petite feuille dessinée dans le coin haut droit (remplace `Fond_1000x800_Feuille4`). | UI-7 ✅ (14/10/2026) |
| `AltheaBarreTitre` | Barre de titre Windows aux couleurs du kit (beige clair, titre sauge très foncé, bordure sauge clair) au lieu de la couleur d'accentuation Windows ; Windows 11 (décision du 15/10/2026). | UI-7 ✅ (15/10/2026) |
| `AltheaIconeEtat` | Icônes d'état dessinées (DialogChoix, encadrés d'attention). | UI-7 ✅ (14/10/2026) |
| `AltheaChamp` | Habillage commun des champs de saisie des fiches (`TextBox`, sélecteur de date, `NumericUpDown`) : bordure beige, arrondi, focus sauge, lecture seule teintée. Part des contrôles déjà posés en UI-4. | UI-7 |

---

## 4. La question des onglets

Le `TabControl` natif est pratique mais peu personnalisable : les onglets sont dessinés par le thème Windows et la zone de contenu est entourée d'un cadre 3D qui ne suit pas la charte.

### Trois voies possibles

| | **A — Contrôle maison complet** | **B — `TabControl` hérité, dessiné** | **C — Syncfusion `TabControlAdv`** |
| --- | --- | --- | --- |
| Principe | Barre d'onglets + panneaux dont on bascule la visibilité. | `Inherits TabControl`, `DrawMode = OwnerDrawFixed`, cadre repeint via `WndProc`. | Contrôle tiers paramétrable (styles, couleurs, bordures). |
| Migration | **Lourde** : les 21 `TabPage` deviennent des `Panel`, chaque Designer est réécrit. | **2 lignes par écran** : le type déclaré change, le contenu des `TabPage` est intact. | **Lourde** : `TabPage` → `TabPageAdv`, Designers réécrits. |
| Maîtrise du rendu | Totale | Bonne (limite : le cadre de la zone de contenu demande un contournement) | Très bonne, sans code de dessin |
| Dépendance | Aucune | Aucune | `Syncfusion.Tools.WinForms` à ajouter |
| Réversibilité | Faible | **Forte** — on remet `New TabControl()` et tout refonctionne | Faible |
| Risque | Élevé sur `UC_DossierFiche` et `UC_PatientFiche` | Faible | Moyen |

### Recommandation

**Commencer par B**, sur un seul écran (`SeanceEditor`, 2 onglets, le plus simple), en **prototype d'une demi-journée**.

Raison principale : la migration coûte deux lignes par écran et s'annule en deux lignes. On juge sur pièce, sans engager les 21 `TabPage`.

**Repli C si le rendu ne convient pas**, notamment à 150 % de mise à l'échelle Windows. Syncfusion est déjà dans le projet (licence enregistrée, packages `DocIO`) : il ne manque que le paquet des contrôles. Le prototype B/C se compare sur le même écran avant de décider.

**A n'est retenu que si B et C échouent tous les deux.**

> **Décision (10/10/2026)** : voie **B** retenue et appliquée aux sept zones d'onglets ; Syncfusion n'est pas ajouté. Voir **ADR-32**.

---

## 5. Les lots

Chaque lot se termine par : build réussi, écran vérifié à l'exécution, documentation mise à jour, commit.

### 5.0 Pourquoi cet ordre

1. **UI-0 avant tout** : rien ne peut être posé tant que les contrôles ne compilent pas.
2. **Les hubs ensuite** (UI-1) : peu de contrôles, beaucoup d'effet, et ce sont les écrans déjà maquettés et validés. Ils servent de référence visuelle pour la suite.
3. **Les onglets tôt** (UI-2) : c'est le point d'incertitude technique du chantier. Mieux vaut le lever avant d'avoir habillé 30 écrans que de découvrir un blocage à la fin.
4. **L'en-tête ensuite** (UI-3) : il touche 31 écrans. Une fois en place, chaque lot suivant en hérite gratuitement.
5. **Les listes avant les fiches** (UI-4 puis UI-5) : `UC_ReferentielBase` habille 11 référentiels d'un coup — le meilleur rapport effort/résultat du chantier.
6. **Domaines et dialogues en fin** (UI-6, UI-7) : ce sont les écrans les plus spécifiques, ils bénéficient de tout ce qui précède.
7. **Nettoyage en dernier** (UI-8) : on ne supprime un asset qu'une fois certain que plus rien ne l'utilise.

### UI-0 — Compiler et valider le kit  ✅ *10/10/2026*

- Générer la solution ; corriger les erreurs de compilation des six nouveaux fichiers.
- Ouvrir `TestKitAlthea` et vérifier : coins arrondis sans rectangle opaque, transparence réelle des cartes sur le fond de la Home, lisibilité du texte blanc sur Sauge, état désactivé, repère de focus clavier.
- Vérifier le comportement à **125 % et 150 %** de mise à l'échelle Windows.
- **Critère de fin** : la fenêtre de démonstration est conforme aux maquettes.

### UI-1 — Les deux hubs  ✅ *10/10/2026*

- `UC_ReferentielHome` (18 boutons) et `UC_AdminHome` (8 boutons).
- Remplacer : tuiles → `AltheaTuile`, panneau central → `AltheaCarte`, titres de groupe → `AltheaTitreSection`, bandeau du haut → `AltheaEnTete` (ou provisoirement conservé), barre du bas → `AltheaBarreActions`.
- **Retirer les images de fond** des `.resx` (21 + 11 images embarquées) et les `BackgroundImage` associés.
- **Critère de fin** : les deux hubs correspondent aux maquettes ; les `.resx` sont allégés.

### UI-2 — Les onglets (prototype puis décision)  ✅ *10/10/2026*

- Prototype B sur `SeanceEditor` : validé, la comparaison avec C n'a pas été nécessaire. Décision consignée en **ADR-32**.
- Appliqué aux six `TabControl` des Designers (`SeanceEditor`, `UC_DossierFiche`, `UC_PatientFiche`, `UC_Cabinet`, `UC_Parametres`, `UC_DomainesHome`) et à la zone « données liées » créée par `UC_ReferentielBase`.
- `CouleurFond = Transparent` partout sauf `SeanceEditor` (fond beige clair) : le feuillage reste visible derrière la bande et ne scintille plus dans l'onglet Séances de `UC_DossierFiche`.
- `UC_Cabinet` et `UC_DomainesHome` : le gras de l'ancienne police d'onglets est reporté sur les `TabPage` (contenu inchangé). **Décision : régulariser en normal** pour que tous les écrans se ressemblent — au lot UI-5.
- **Critère de fin** : les six écrans à onglets ont la barre soulignée de la maquette ; décision documentée.

### UI-3 — En-tête d'écran commun  ✅ *10/10/2026*

- `AltheaEnTete` créé, validé sur trois pilotes (Paramètres, Patients, SeanceEditor), puis posé sur **tous** les écrans : 12 UC et 19 Forms, plus les 15 référentiels via `UC_ReferentielBase`.
- **Règle (décision du 10/10)** : tous les UC et toutes les Forms ont cet en-tête ; un élément manquant (icône, titre, sous-titre) est rétabli. Rétabli sur `DialogComptePraticien`.
- Sans en-tête, volontairement : `Home` (logo), `ReferentielModalHost` (l'UC hébergé porte le sien), composants intégrés (liaisons, synthèse, pagination, éditeurs de texte).
- Conversion par script : le script refuse tout en-tête contenant d'autres contrôles que le trio, les cas particuliers (`UC_DossierFiche`, Login…) sont traités explicitement.
- `UC_DossierFiche` : titre = code du dossier, sous-titre = « Patient : nom (code) ».
- **Critère de fin** : un seul contrôle porte l'en-tête ; l'icône reste réglable par écran.

### UI-4 — Écrans de liste  ✅ *10/10/2026*

- `UC_PatientHome`, `UC_Therapeutes`, `UC_Utilisateurs`, `UC_ReferentielBase` *(→ les 11 référentiels d'un coup)*.
- **Bandeau de recherche redessiné** (décision du 10/10) : un même modèle pour tous les écrans, avec plus ou moins de champs selon le type.
  - ✅ *10/10/2026* — maquette validée, prototype sur Patients puis Thérapeutes, Utilisateurs, `UC_ReferentielBase` (15 référentiels) et liste des Domaines.
  - Champs du kit avancés de UI-7 : `AltheaRecherche`, `AltheaListe`, `AltheaCase` (ADR-33).
- Barre d'actions ✅ — `AltheaBarreActions` + `AltheaBouton` (couleur selon le rôle, largeur ajustée au texte).
  - **Étendue le 10/10, à la demande de Joëlle, à toutes les barres d'actions** (fiches et fenêtres comprises, 28 écrans) pour ne pas mélanger anciens et nouveaux boutons ;
  - le reste de l'habillage des fiches et fenêtres reste à UI-5 / UI-7.
- ✅ *11/10/2026* — **Décision du 10/10** : les barres de boutons **locales** (actions d'une grille ou d'une section : Ajouter / Modifier / Supprimer un contact, un intervenant, une séance…) ne doivent pas ressembler à la barre d'actions de l'écran, pour que l'utilisateur ne les confonde pas.
  - Elles recevront un habillage propre (barre et boutons plus légers, posés au plus près de la grille qu'ils pilotent), **sur maquette d'abord**.
- Panneau de détail des référentiels ✅ — `AltheaCarte` avec titre intégré (l'ancien `lblTitreEdition` disparaît) ; ses champs suivront au lot UI-7.
- **Critère de fin** : les écrans de liste sont homogènes ; `UC_ReferentielBase` propage le style à ses 11 descendants.

### UI-5 — Fiches et écrans complexes  ✅ *11/10/2026*

**Avancement** :
- ✅ *11/10* — **Récapitulatif patient** de `UC_DossierFiche` (maquette « Récapitulatif A ») : quatre blocs Patient / Dossier / Finances / Quotas ; bloc Quotas titré par l'organisme du domaine (« QUOTAS — INAMI »), « Domaine sans quota » sinon. Nouveau contrôle `AltheaJauge`.
- ✅ *11/10* — **Onglet Fiche dossier** (maquette « Fiche dossier B ») : cartes « Paramètres du dossier » et « Convention du domaine » ; `UC_SyntheseDomaine` V2 (blocs du kit, colonnes ou empilés), donc aussi l'onglet Synthèse des Domaines. Nouveau contrôle `AltheaPastille`. Miniature du logo de domaine prévue (Suite lot 27).
- ✅ *11/10* — **Fiche Patient** (maquette « Fiche patient ») : bandeau identité en carte, onglet Identité en cartes (Alerte / Notes placée sous Coordonnées faute de hauteur en pleine largeur), cadre redondant de l'Anamnèse retiré.
- ✅ *11/10* — **Fiche Dossier** : Suivi clinique (maquette « Suivi clinique A », nouveau contrôle `AltheaLigne`), Documents en carte.
- ✅ *11/10* — **Bandeaux de recherche des onglets** : Famille, Intervenants, Séances, Paiements (modèle UI-4).
- ✅ *11/10* — **Cabinet** : les 4 `GroupBox` (Logo, Photo, Signature, Compte) deviennent des cartes, leurs boutons passent en variantes Outil ; l'image de panneau étirée derrière la grille de l'onglet Cabinet est remplacée par une carte. **Gras retiré** des pages de Cabinet et de Domaines.
- ✅ *11/10* — **Paramètres** : case « Afficher les paramètres désactivés » du kit ; les champs des onglets, construits par le code, suivront l'habillage des champs (UI-7).
- **Reporté (décision du 11/10)** : les 2 `GroupBox` de `UC_DomainesHome` → UI-6 (refonte complète) ; les 11 des fenêtres `ContactEdition`, `IntervenantEdition`, `TherapeuteEdition`, `CategoriesQuotaEditor` → UI-7 ; `UC_Accueil` → lot 31 (widgets).


- `UC_PatientFiche` (23 boutons), `UC_DossierFiche`, `UC_Cabinet`, `UC_Parametres`, `UC_Accueil`.
- **Bandeau récapitulatif du patient** de `UC_DossierFiche` redessiné (décision du 10/10) : il reste visible pendant la navigation dans le dossier et rappelle les renseignements principaux du patient. **Maquette à valider avant de coder.**
- **Bandeau identité de la fiche Patient** (photo, nom, code, statut, Photo… / Désactiver, note d'alerte) : redessiné en carte, en écho au récapitulatif patient de la fiche Dossier (signalé le 11/10).
- **Onglets sans grille** (formulaires) : Identité (fiche Patient), Fiche dossier, **Suivi clinique** (boutons de zones générés par le code), Documents (fiche Dossier) — cartes, titres de section, boutons du kit (signalé le 11/10).
- Remplacement des `GroupBox` des fiches par des cartes du kit (`AltheaCarte` à titre intégré) plutôt que par `AltheaTitreSection`, pour garder le regroupement visuel.
- Texte des pages de `UC_Cabinet` et `UC_DomainesHome` repassé en normal (décision du 10/10 : tout doit se ressembler).
- **Critère de fin** : plus aucun `GroupBox` dans les fiches ; les fiches suivent la charte. ✅

### UI-6 — Domaines et assistant  ✅ *terminé et validé le 14/10/2026*

**Déjà fait dans les lots précédents** : ✅ en-tête (UI-3) ; ✅ bandeau de recherche de la liste (UI-4) ; ✅ barre d'actions (UI-4) ; ✅ onglets (UI-2) ; ✅ `UC_SyntheseDomaine` V2 (UI-5) ; ✅ texte des pages en normal (UI-5).
**Reste** (état au 11/10/2026) : 10 `Button` natifs, 2 `GroupBox`, champs au style Windows (2 `TextBox`, 1 `ComboBox`, 2 `DateTimePicker`, 5 `CheckBox`), panneaux de liaison construits par le code.

- `UC_DomainesHome` : **refonte complète du design** (rappel de Joëlle, 11/10) — `AltheaEtapes`, panneau de détail avec bandeau Sauge foncé, note d'aide gris-vert, onglets.
- Barres locales des onglets Tarifs et Natures : les panneaux de liaison construits par le code (`UC_LiaisonsNaturesDomaines`, `UC_LiaisonsPrixDomaines`, partagés avec les référentiels) passent au modèle carte + `AltheaBarreOutils` (ADR-34).
- `UC_SyntheseDomaine` : redessinée en UI-5 (V2) ; reste sa mise en carte dans l'onglet Synthèse.
- Les 2 `GroupBox` de l'onglet Général (`grpIdentite`, `grpComportement`), reportés de UI-5.
- **Critère de fin** : l'assistant montre clairement l'étape en cours et les étapes faites.

**Réalisé le 14/10/2026** (maquettes « Domaines — A », « liaisons », « quotas ») :
- 6a :
  - `AltheaEtapes` (nouveau contrôle) à la place de la bande d'onglets (`AltheaOnglets.MasquerOnglets`),
  - pastille d'état du domaine,
  - Général en deux cartes (les 2 `GroupBox` supprimés),
  - `cboOrganisme` en `AltheaListe` ;
  - règle « Consultation lisible » (`UtilsControls.DefinirLectureSeule`, `ColorChampLectureSeule`, `AltheaCase` / `AltheaListe` lisibles désactivées).
- 6b : `UC_LiaisonsPrixDomaines` et `UC_LiaisonsNaturesDomaines` V2 (carte, barre locale, formulaire au pied de la carte, catégorie de quota dans la grille, `DialogChoix`).
- 6c : étape Catégories & quotas en deux cartes, nouvelle règle guidée et récapitulée, règles corrigeables tant qu'elles n'ont pas servi (ADR-40) ; `DialogChoix` V1.1.0 (taille adaptée au texte).
- 6d : Synthèse dans une carte « Convention du domaine » (quota en bande pleine largeur, `UC_SyntheseDomaine` V2.3.0) ; périodes sans règle mises en évidence (jamais comblées automatiquement) ; guide utilisateur, plan de tests et documentation mis à jour.
- Reste hors UI-6 : logo de domaine (Suite lot 27), champs Windows restants (`TextBox`, `DateTimePicker`, `NumericUpDown`) avec `AltheaChamp` (UI-7), captures d'écran à refaire.

### UI-7 — Fenêtres et dialogues  🔄 *partiellement avancé*

**Déjà fait dans les lots précédents** : ✅ en-tête `AltheaEnTete` sur toutes les fenêtres (UI-3) ; ✅ barres d'actions du kit (UI-4) ; ✅ onglets de `SeanceEditor` (UI-2) ; ✅ champs du kit créés (`AltheaRecherche`, `AltheaListe`, `AltheaCase`, UI-4) — prêts à être réutilisés.

**Ordre décidé le 14/10/2026** :

1) `DialogChoix` (maquette, puis mise en page extensible et icônes dessinées)
   - ✅ 14/10/2026 : V2.0.0 (maquette B, icônes style 2, `AltheaIconeEtat`, démonstration `TestDialogChoix`) ;
   - ✅ 16/10/2026 : V2.1.0 (maquette C : petite fenêtre du kit — barre de titre, `AltheaEnTete`, carte, `AltheaBarreActions`, boutons dans l'ordre habituel ; la V2.0 jurait avec les fenêtres en cartes) ;
2) champ de date du kit
   - ✅ 14/10/2026 : `AltheaDate` (composé, saisie au clavier, date facultative « vide + croix », lecture seule lisible) et `AltheaCalendrier` (calendrier A dessiné), démonstration `TestChampDate` ;
3) écrans un par un, des plus utilisés aux fenêtres (
   - fiche Patient — ✅ onglet Identité le 14/10/2026 —,
   - fiche Dossier — ✅ 14/10/2026 : domaine, date d'ouverture, filtres du / au des onglets Séances et Paiements, carte Paramètres en consultation lisible —,
   - puis fenêtres — ✅ 14/10/2026 : `ContactEdition`, `IntervenantEdition`, `TherapeuteEdition`, `UtilisateurEdition` ; ✅ 15/10/2026 : `SeanceEditor`, `PaiementEditor`, `DossierStatutEditor`, `DialogReportSeance`, `DialogMotifAnnulation`, `DialogPaiementReaffectation`, `DialogPaiementsSeance`, `PlanningJourViewer`),
   - en appliquant à chacun les champs du kit **et** la règle « Consultation lisible » (`DefinirLectureSeule`) — le tour lecture seule n'est pas fait à part.

**Reste : panneaux centraux, boutons hors barre d'actions, cadres et champs** — inventaire au 11/10/2026 (comptage dans les Designers) :

| Fenêtre | `Button` natifs | `GroupBox` | Fond image | Champs Windows (texte / liste / date / case) |
| --- | --- | --- | --- | --- |
| `ContactEdition` | 3 | 4 | 1 | 8 / 3 / 1 / 0 — ✅ 14/10 : 4 cartes, 4 listes et la date du kit, consultation lisible, fond du kit, 3 boutons en `AltheaBouton` Outil |
| `IntervenantEdition` | 3 | 3 | 1 | 4 / 2 / 2 / 0 — ✅ 14/10 : 3 cartes, 2 listes et 2 dates du kit, consultation lisible, fond du kit, 3 boutons en `AltheaBouton` Outil |
| `TherapeuteEdition` | 0 | 3 | 1 | 9 / 1 / 0 / 1 — ✅ 14/10 : 3 cartes, 2 listes et la case du kit, fond du kit |
| `UtilisateurEdition` | 0 | 0 | 1 | 8 / 2 / 0 / 3 — ✅ panneaux et titres remplacés par 4 `AltheaCarte` le 12/10 ; ✅ 14/10 : fond du kit, 2 listes et 3 cases du kit, consultation lisible |
| `SeanceEditor` | 5 | 0 | 1 | 0 / 5 / 1 / 0 — ✅ 15/10 : fond du kit, deux cartes (Prestation, Rendez-vous), 3 listes et la date du kit, [+] et cadenas en Outil icône seule, planning du jour en Clair, `DefinirSaisieActive` ; restent les 2 listes d'heures (champ d'heure à venir) |
| `PaiementEditor` | 1 | 0 | 1 | 2 / 2 / 1 / 0 — ✅ 15/10 : cartes Encaissement / Commentaire, fond du kit, 2 listes et la date du kit, [+] en Outil icône seule, consultation lisible |
| `CategoriesQuotaEditor` | 3 | 1 | 1 | 2 / 0 / 0 / 0 — ✅ 15/10 : 2 cartes (barre locale comme l'écran Domaines), fond du kit, boutons du kit |
| `DossierStatutEditor` | 0 | 0 | 1 | 0 / 1 / 1 / 0 — ✅ 15/10 : carte, fond du kit, liste et date du kit |
| `DossierZoneEditor` | 0 | 0 | 1 | — (éditeur riche) — ✅ 15/10 : carte titrée par la zone, fond du kit |
| `DialogReportSeance` | 0 | 0 | 1 | 0 / 0 / 3 / 1 — ✅ 15/10 : carte, fond du kit, date et case du kit ; restent les 2 heures (champ d'heure à venir) |
| `DialogMotifAnnulation` | 0 | 0 | 1 | 1 / 0 / 0 / 0 — ✅ 15/10 : carte, fond du kit, couleurs d'erreur de la charte |
| `DialogPaiementReaffectation` | 0 | 0 | 1 | 0 / 2 / 0 / 0 — ✅ 15/10 : carte, fond du kit, 2 listes du kit |
| `DialogPaiementsSeance` | 0 | 0 | 1 | — (grille) — ✅ 15/10 : carte titrée par la séance, fond du kit |
| `DialogRestauration` | 0 | 0 | 1 | 4 / 1 / 0 / 1 — ✅ 15/10 : 4 cartes (le `TableLayoutPanel` disparaît), liste et case du kit, yeux en Outil icône seule, fond du kit |
| `DialogComptePraticien` | 0 | 0 | 0 | 2 / 1 / 0 / 0 — ✅ 15/10 : carte, fond du kit, liste du kit, erreur aux couleurs de la charte |
| `PlanningJourViewer` | 0 | 0 | 1 | 0 / 0 / 1 / 0 — ✅ 15/10 : carte, fond du kit, date du kit |
| `DialogChoix` | 3 | 0 | 2 | — (boîte de message) |
| `Login` | 1 | 0 | 1 | 2 / 0 / 0 / 0 — ✅ 15/10 : carte, fond du kit, œil en Outil |
| `ChangePassword` | 3 | 0 | 1 | 3 / 0 / 0 / 0 — ✅ 15/10 : carte, fond du kit, 3 yeux en Outil |
| `ElevationAcces` | 1 | 0 | 1 | 1 / 1 / 0 / 0 — ✅ 15/10 : carte, fond du kit, liste du kit, œil en Outil |
| `ConfigurationConnexion` | 2 | 0 | 1 | 5 / 0 / 0 / 0 — ✅ 15/10 : 2 cartes, fond du kit, œil en Outil, « Modifier » du kit ; port encore Windows |

**Cachet (`UC_Cabinet`)** : aperçu sur fond « papier » blanc, pour qu'il se lise comme un aperçu de document et non comme un élément de l'interface — le cachet garde son encre de tampon (décision du 11/10).

**UserControls à reprendre au même lot** (champs) :

- `UC_Cabinet` (24 champs texte —
  - onglets Cabinet et Praticien ✅ 15/10/2026 : 4 + 3 cartes, 4 listes du kit, consultation lisible, icônes des boutons Outil redessinées ;
  - onglets Identifiants et Cachet ✅ 16/10/2026 : deux cartes titrées avec consigne, consigne du cachet dans sa carte, icône tampon `cachet_24_normal`),
- `UC_PatientFiche` (10 texte, 5 listes, 1 date, et le bouton « + » de la situation familiale),
- ~~`UC_DossierFiche` (dates)~~ ✅ 14/10/2026,
- ~~`UC_ReferentielBase` (panneau Détail)~~ ✅ 16/10/2026 (règle des fenêtres, `AjouterLigneDetail`, consultation lisible),
- ~~`UC_Parametres` (champs construits par le code)~~ ✅ 16/10/2026 (V2.0.0, page unique).

Cas particuliers :

- ~~`UC_Pagination` (4 boutons natifs)~~ ✅ 19/10/2026 (V1.1.0 : boutons Outil du kit, résumé, Séances et Paiements du dossier paginés),
- ~~éditeurs de texte riche (barre d'outils en images)~~ ✅ 19/10/2026 (temps 1 : un seul `UC_RichTextEditor` à deux profils, barre du kit `AltheaRenduBarre`, icônes redessinées ; ADR-42 ; temps 2 au ToDo G quater).


- ✅ **Toutes reprises le 15/10/2026** (restent hors fenêtres : ~~champ d'heure du kit~~ ✅ 16/10/2026 — `AltheaHeure`, liste A, fenêtre Séance et report —, `NumericUpDown`, port MariaDB, trois dates Windows : Utilisateurs, règles de quota, tarifs liés).
  - Les 14 Forms d'alors : `SeanceEditor`, `PaiementEditor`, `ContactEdition`, `IntervenantEdition`, `TherapeuteEdition`, `UtilisateurEdition`, `CategoriesQuotaEditor`, `DossierStatutEditor`, `DossierZoneEditor`, `DialogChoix`, `DialogComptePraticien`, `DialogMotifAnnulation`, `DialogPaiementReaffectation`, `DialogPaiementsSeance`, `DialogReportSeance`, `PlanningJourViewer`, `Login`, `ChangePassword`, `ElevationAcces`, `ConfigurationConnexion`.
- Les 11 `GroupBox` de `ContactEdition`, `IntervenantEdition`, `TherapeuteEdition` et `CategoriesQuotaEditor`, reportés de UI-5.
- Fonds `Fond_1000x800_Feuille4` et `Fond_480x240` remplacés — **décidé le 14/10/2026** : beige clair uni et petite feuille dessinée dans le coin (`AltheaFondFenetre.Appliquer(Me)`), posé fenêtre par fenêtre.
- **Panneau racine `pnlForm`** (accord de Joëlle, 14/10/2026) : supprimé au passage de chaque fenêtre qui en a un (contenu posé directement sur la fenêtre, comme les fenêtres récentes)
  - fait pour `UtilisateurEdition`, puis pour `Login`, `ChangePassword`, `ElevationAcces` et `ConfigurationConnexion` (15/10/2026) : plus aucune fenêtre n'en a.
  - Ne pas ajouter de police à la fenêtre en le retirant : `AutoScaleDimensions` suit la police par défaut.
- **Champs des fenêtres en cartes** — **décidé le 15/10/2026 (Joëlle) : règle pour TOUTES les fenêtres** (Rules § 15, « Fenêtres : même schéma que les fiches ») :
  - comme dans les fiches et `UtilisateurEdition`, les champs d'une fenêtre sont regroupés dans des `AltheaCarte` titrées.
  - Essai validé sur `SeanceEditor` (Prestation / Rendez-vous).
  - Les maquettes de fenêtres du 09/10 sont antérieures aux cartes.
- **Petits boutons à côté d'un champ** ([+], loupe, copie d'adresse) — **décidé le 14/10/2026** : `AltheaBouton` variante Outil, icône seule, 30 × 30 (fiche Patient et fenêtres Contact / Intervenant faites).
- **Habillage de tous les contrôles de saisie** (décidé le 10/10) : `AltheaChamp` pour `TextBox` et `ComboBox` (bordure beige, arrondi, focus sauge, lecture seule teintée), cases à cocher sauge, `DateTimePicker` et `NumericUpDown` alignés, grilles harmonisées via `UtilsDataGrid`.
- **`UC_Parametres`** (question du 12/10, retenu pour UI-7) : les onglets avec grille et panneau de détail sont lourds pour une dizaine de paramètres répartis en groupes de 1 à 4.
  - Piste : une seule page, une `AltheaCarte` par groupe, chaque paramètre sur une ligne (libellé, valeur modifiable sur place, description en aide).
  - Cela supprime aussi la notion de « paramètre sélectionné », à l'origine du bug du 12/10.
  - ✅ **16/10/2026** : maquettes A (aide sous la valeur) et B (deux colonnes, aide en infobulle) ;
  - **A retenue** par Joëlle, avec les titres et l'ordre des groupes **en base** (migration v2.40.0, `tec_groupes_parametres`) et **un seul Modifier pour la page** (enregistrement en une transaction, `GestionParametres.UpdateValeursParametres`).
  - `UC_Parametres` V2.0.0 ; nouvelle icône Outil `dossier_24_normal` (Parcourir).
- **Cadre image du kit** (proposé le 12/10) : un contrôle unique « image + bouton Importer… + texte d'aide » (nom provisoire `AltheaCadreImage`) pour remplacer les assemblages refaits à la main dans la fiche Patient (photo), `UC_Cabinet` (logo, photo, signature, cachet) et `UtilisateurEdition` (photo).
  - Il s'appuie sur `UtilsImages` / `ImagesHelper`, sans rien changer à l'import.
- **Critère de fin** : aucune fenêtre ne détonne ; plus aucun contrôle de saisie au style Windows par défaut.

### UI-8 — Nettoyage et clôture

**Exigence de Joëlle (10/10/2026)** : à l'issue du chantier, la solution ne doit plus contenir **aucun fichier ni asset inutilisé**, ni en local ni sur GitHub. Sauvegarde externe confirmée : la suppression est assumée.

**8a. Audit automatisé.** Script de recensement produisant trois listes :
- assets de `Assets/` cités dans aucun `.vb`, `.Designer.vb`, `.resx` ni `.vbproj` ;
- fichiers `.vb` orphelins (aucune référence au type qu'ils déclarent) ;
- images embarquées en base64 dans les `.resx` devenues inutiles.

**8b. Suppressions attendues** (à confirmer par l'audit) :
- `Assets/ui-backgrounds/` — l'essai d'habillage par images du 09/10 ;
- anciens fonds étirés : `Fond_1000x800_Feuille4`, `Fond_1000x770_FeuilleCoupee1`, `Fond_500x400_Feuille4`, `Fond_480x240`, `Althea_Bandeau_Haut_Trans` ;
- `Fond_1000x770_FeuilleCoupee1` retiré des Designers des UserControls hébergés (déjà ignoré au runtime depuis le 09/10) ; ✅ 18/10/2026
- images embarquées dans les `.resx` (21 dans `UC_ReferentielHome`, 11 dans `UC_AdminHome`, 9 dans `UC_ReferentielBase`, 7 dans `UC_Cabinet`) ; → **gardées** (18/10/2026 : aperçu dans le Designer) ;
- icônes `_hover` / `_disabled` devenues inutiles si les états sont désormais dessinés ; → **gardées** (18/10/2026 : familles complètes) ;
- entrées `<None Update=...>` correspondantes dans `Althea.vbproj` ;
- `UI/Forms/Test/UC_Domaines` (déjà exclu de la compilation) ✅ 18/10/2026 ; `TestKitAlthea` reste (vitrine du kit, décision du 15/10/2026) ;
- documents de travail obsolètes dans `Docs/` (`_tmp_*.md`, `_tmp_*.ps1`, `roles_tmp.ps1`).

**8c. Méthode de suppression — prudence.**
- Un commit de nettoyage **séparé par catégorie** (assets / resx / code / docs), jamais un commit fourre-tout : un `git revert` ciblé reste possible.
- Build **et** exécution complète après chaque catégorie : une image manquante ne casse pas la compilation, elle casse l'affichage à l'exécution.
- `UtilsButtons.LoadImageFromAsset` charge des images **par leur nom, au runtime** : une recherche de texte ne les voit pas toutes. Vérifier chaque dossier d'icônes à la main avant suppression.
- **Leçon du 17/10/2026** : un `Tag` mal orthographié fait paraître « inutilisé » le fichier qu'il devait viser (`naturesSeance_normal` contre `natureSeance_*`).
  - Avant toute suppression :
    - (1) relever chaque `Tag` dont le fichier `_normal` / `_hover` / `_disabled` manque ;
    - (2) comparer **au pixel** chaque image candidate aux images intégrées dans les `.resx` des écrans — une correspondance en fait une source à garder.

**8d. Mesures de fin.** Taille du dépôt avant/après, nombre de fichiers supprimés, nombre d'images restantes.

**Avancement.**
- **Catégorie « assets »** ✅ 17/10/2026 (validée, commit d780ac47) :
  - trois chemins de chargement relevés — par nom sur le disque (`Tag` des boutons, enum `ZoneDossier`), par `My.Resources`, et images **intégrées par le Designer** dans le `.resx` de chaque écran, dont les PNG de `Titre_Ico`, `Editor_Ico`, `Splash` et `Appli_Ico` restent les sources (conservés).
  - Supprimés :
    - `ui-backgrounds`, `Fond_1000x800_Feuille4`, `Fond_500x400_Feuille4`, `Fond_480x240`, `Althea_Bandeau_Haut_Trans`, `Fond_icone_Transp`, GIF, PNG de statut, icônes jamais branchées ;
    - 20 ressources ;
    - 146 entrées `<None Update>` (106 visaient des dossiers disparus).
  - Photo de la praticienne retirée du suivi Git.
  - Quatre `Tag` de tuiles corrigés (Cabinet, Utilisateurs, Natures séance, Prix séance : leur fichier n'existait pas sous ce nom, d'où l'absence de survol).
  - `Assets` : 356 → 305 fichiers suivis, 4,8 → 4,0 Mo.
- **Catégorie « resx »** ✅ 18/10/2026 (validée, commit 449e705d) :
  - `Fond_1000x770_FeuilleCoupee1` retiré des Designers de 11 UserControls (la Home le retirait déjà à l'exécution), puis ressource et fichier supprimés ;
  - la fenêtre d'ajout « en passant » (`ReferentielModalHost`), où il s'affichait encore, reçoit le fond du kit.
  - **Gardés, décision de Joëlle** :
    - les 102 icônes de boutons intégrées en double dans les `.resx` (77 Ko ; aperçu dans le Designer, alors que `UtilsButtons` les recharge depuis le `Tag`)
    - et les familles complètes `_normal` / `_hover` / `_disabled` (chargées par plusieurs mécanismes : `Tag`, noms logiques de la Home, `PreparerBouton`, vitrine du kit).
  - `Assets` : 305 → 304 fichiers suivis.
- **Catégorie « code »** ✅ 18/10/2026 (validée) :
  - audit des types déclarés par chaque `.vb` et de leurs usages (fonctions de module comprises, appelables sans préfixe en VB) ;
  - supprimés `UC_Domaines` (+ `.resx`), `ApplicationEvents.vb`, `UtilsString.vb`.
  - Gardées, décision de Joëlle : les fenêtres de test de l'Accueil (Debug seulement).
  - Relevé hors nettoyage : clé de licence Syncfusion en clair dans `Program.vb` (ToDo, sécurité).
- **Catégorie « docs »** ✅ 18/10/2026 (validée) :
  - plus aucun document de travail `_tmp_*` suivi ;
  - liens vers des documents déplacés corrigés (14) ;
  - 51 diagrammes de base périmés supprimés ;
  - `desktop.ini` et `settings.VisualStudio.json` (propres au poste) retirés du suivi et ignorés.
  - **Gardés, décision de Joëlle** : les 79 liens vers les captures supprimées le 16/10, en attente des captures de la refonte (ToDo).
- **Mesures (8d)** : fichiers retirés du dépôt — 51 assets, 1 fond, 4 fichiers de code, 51 diagrammes, 2 fichiers propres au poste, plus la photo de la praticienne (gardée sur le disque, ignorée) ;
  - 21 ressources de `Resources.resx` ;
  - 151 entrées du `.vbproj`.
  - `Assets` : 356 → 304 fichiers suivis (4,8 → 4,0 Mo).
  - Relevé de sécurité hors nettoyage : clé Syncfusion retirée du code (ADR-41).

**Critère de fin** : plus aucune image de fond étirée ; `git status` propre ; application vérifiée écran par écran après le nettoyage.

### UI-9 — Grilles : états lisibles partout

**Origine** : revue de toutes les grilles demandée par Joëlle à la clôture d'UI-8 (18/10/2026). Le socle (`InitDataGridBasique` / `InitDataGridSecondaire`) est homogène, mais hors des écrans repris le 17/10 (Patients, séances, paiements, aperçu), un état se lit de trois façons : texte coloré ou ligne grisée, case « Actif », icône PNG.

**Principe** :

- la règle des pastilles (`Reference_UI`, grille des séances) partout ;
- des **tons sémantiques** pris dans les teintes existantes — OK (sauge), À surveiller (ocre), Problème (rose), Inactif (gris), Information (bleu), Rôle (mauve) — que la couche métier choisit par une famille, jamais par une liste de codes.

Maquette « Grilles — états lisibles » avant l'étape 1.

**Maquette** ✅ 18/10/2026 (validée ; `Reference_UI` § 2.4, image `grilles_etats.png`).

**Étapes** :

- 9.1 socle (tons, double tampon, polices partagées, alignements, fin contour sur toute pastille pleine — ✅ 18/10/2026, validé) ;
- 9.2 patient et dossier (pastilles du planning, des paiements d'une séance et des dossiers, créneaux libres du jour — ✅ 18/10/2026, validé) ;
- 9.3 administration (pastilles et repères des Utilisateurs, de l'État du système, des Journaux et des historiques — ✅ 18/10/2026, validé) ;
- 9.4 référentiels et Domaines, puis retrait des PNG `Tech_Ico` devenus inutiles (✅ 18/10/2026, validé).

**UI-9 terminé le 18/10/2026.** Détail : ToDo, section H bis.

## 6. Documentation à mettre à jour

Mise à jour **au fil des lots**, pas en bloc à la fin.

| Fichier | Nature de la mise à jour | Lot |
| --- | --- | --- |
| `Rules/Charte_Graphique.md` | Section « Kit d'interface » ✅ créée le 09/10. À compléter : onglets, en-tête, étapes, champs ; section `ui-backgrounds` ✅ retirée le 17/10/2026 (UI-8). | tous |
| `Rules/Reference_UI.md` | Construire un écran (familles d'écrans illustrées par les maquettes, recettes fenêtre et UserControl, comportements communs) et catalogue des contrôles du kit (propriétés, valeurs par défaut et d'usage). ✅ 15/10/2026 : fusion de `Reference_UI_Proprietes.md` et `Reference_UI_Guide_Utilisation.md` ; à compléter à chaque création de contrôle ; captures réelles à venir (vitrine du kit). | chaque création |
| `Rules/Rules.md` | Remplacer la règle « Habillage par images de fond » par « Habillage par contrôles dessinés ». Interdire tout nouveau `Button` / `GroupBox` natif dans un écran. | UI-1 |
| `Rules/ARCHITECTURE_DECISIONS.md` | **ADR-30** : contrôles dessinés plutôt qu'images de fond. **ADR-31** : place d'une tuile dans un hub. **ADR-32** : stratégie onglets ✅. | UI-1, UI-2 |
| `Architecture/Documentation_technique_UI_Althea.md` | 8 492 lignes — le gros morceau. Section par écran à reprendre ; ajouter une section « Contrôles du thème ». | chaque lot |
| `Architecture/Interface_Utilisateur.md` | Vue d'ensemble de l'interface : décrire le kit et les principes visuels. | UI-1 |
| `Architecture/Structure_Solution_Althea.md` | Nouveau répertoire `UI/Controls/Theme/`. | UI-0 |
| `Architecture/Utilitaires_Transverses.md` | `AltheaDessin` ; évolution du rôle d'`UtilsButtons` (il ne pose plus de `BackColor` sur les boutons habillés). | UI-0 |
| `Architecture/Architecture_Technique.md` | Mention du kit dans la couche présentation. | UI-1 |
| `Architecture/Bibliotheque_Documentation.md` | Référencer le présent plan. | aujourd'hui |
| `Todo/ToDo.md` | Nouvelle section « H bis. Chantier UI » avec les lots UI-0 à UI-8 cochables. | aujourd'hui |
| `Todo/Planning_actions_Althéa.md` | Insérer le chantier UI comme travail transverse ; préciser qu'il n'interrompt pas les lots 30 à 33. | aujourd'hui |
| `Todo/ETAT_DU_PROJET.md` | État d'avancement du chantier. | chaque lot |
| `Todo/Checklist_projet_V1.md` | Points de recette visuelle. | UI-8 |
| `CHANGELOG.md` | Une entrée par lot livré. | chaque lot |
| **Plans de tests** | Nouveau `Docs/Tests/PLAN_TESTS_UI_KIT.md` : rendu, états, mise à l'échelle 100/125/150 %, navigation clavier, lisibilité. | UI-0 |

---

## 7. Points d'attention

1. **Visual Studio écrase les fichiers ouverts.** Le 09/10, `Home.vb` était ouvert dans l'IDE : la génération a réécrit le fichier par-dessus une modification externe, supprimant le code du feuillage. **Avant chaque session : fermer les fichiers concernés dans Visual Studio, ou accepter « Recharger » quand il le propose.**
2. **Clé de licence Syncfusion en clair** dans `Program.vb` (ligne 42), alors que le projet possède un dépôt **public** (`Les-Artefacts-de-Manou/Althea`) et que les règles du projet interdisent tout secret dans le dépôt. À déplacer vers la configuration ou une variable d'environnement. ⚠️ *Hors périmètre UI, mais à traiter.*
3. **Mise à l'échelle Windows (125 %, 150 %).** `AutoScaleMode = Font` déplace les contrôles mais pas les images de fond. C'est la raison pour laquelle le fond de la Home ne contient plus ni logo ni élément devant tomber sous un contrôle. Tout nouveau contrôle doit être vérifié aux trois échelles.
4. **Transparence WinForms.** `Color.Transparent` ne fait que peindre le fond du parent : un contrôle translucide posé sur un autre contrôle opaque ne verra pas le fond de la Home. Les panneaux intermédiaires doivent rester transparents.
5. **Scintillement.** Les contrôles dessinés utilisent le double tampon. Si un écran chargé scintille au redimensionnement, activer `WS_EX_COMPOSITED` sur le conteneur plutôt que multiplier les `Invalidate`. Un contrôle **natif** qui laisse voir le feuillage (la bande d'onglets Windows, par exemple) le repeint directement à l'écran, hors double tampon : le remplacer par un contrôle du kit.
6. **Lisibilité.** Le texte blanc sur Sauge (#7a9b87) est juste au regard des recommandations d'accessibilité. Compensé par le gras et une taille un peu plus grande sur les tuiles. À ne pas dégrader.
7. **Les 11 référentiels sans Designer** héritent de `UC_ReferentielBase` : une seule modification les habille tous. À faire tôt (UI-4) pour un gain rapide.

---

## 8. Décisions actées (10/10/2026)

| Question | Décision |
| --- | --- |
| Onglets | **Prototype B** (`TabControl` hérité et dessiné) avant d'envisager Syncfusion. → Prototype validé, voie B actée (**ADR-32**). |
| Ordre des lots | Laissé à mon appréciation → ordre retenu : **UI-0 → UI-1 → UI-2 → UI-3 → UI-4 → UI-5 → UI-6 → UI-7 → UI-8** (justification § 5.0). |
| `AltheaEnTete` | **Oui**, le trio `picTitre` / `lblTitreForm` / `lblTop` est remplacé par un seul contrôle. |
| Champs de saisie | **Oui**, tous les contrôles sont habillés : `TextBox`, `ComboBox`, `CheckBox`, `DateTimePicker`, `NumericUpDown`, `DataGridView`. |
| En-tête | **Tous** les UC et Forms portent `AltheaEnTete` ; éléments manquants rétablis. Sous-titre **sous** le titre. |
| Bandeaux | Maquettes validées le 10/10 (artifact « Althéa – Bandeaux »). **Recherche** : un modèle commun en carte translucide — recherche avec loupe, listes à étiquette intégrée, Réinitialiser / Rechercher en bout de ligne, ligne « Affiner » pour les critères secondaires (Utilisateurs), version allégée dans les onglets. **Récapitulatif patient** : version **A, blocs groupés** (Patient, Dossier, Finances, Quotas INAMI avec jauges), resserrée en hauteur ; ajoute date de naissance, âge et note d'alerte du patient. |
| Barres locales | Distinctes de la barre d'actions de l'écran (autre habillage), pour ne pas tromper l'utilisateur — maquette puis UI-5. |
| Nettoyage final | **Exigé** : aucun fichier ni asset inutilisé ne doit subsister, en local comme sur GitHub. Sauvegarde externe confirmée par Joëlle. |

---

# Contact

> ***Joëlle De Poorter - Les Artefacts de Manou***
>
> - GitHub privé : Althea — https://github.com/AngeljoNG/Althea
> - GitHub public : https://github.com/Les-Artefacts-de-Manou/Althea
