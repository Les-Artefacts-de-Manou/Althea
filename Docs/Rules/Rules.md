# Rules – Althéa

> *Dernière mise à jour : 26/10/2026 — § 3 : tests automatisés (ADR-58)*

<details>
<summary>Historique des mises à jour</summary>

- 25/10/2026 — § 7 et § 22 : `Option Strict On` pour tout le projet, `.editorconfig` et analyseurs .NET (ADR-57)
- 25/10/2026 — § 13 : grands documents scindés (doc UI par domaine, processus, CHANGELOG par mois) ; où ranger une nouvelle fiche
- 25/10/2026 — § 9 : la fenêtre principale par son contrat `IFenetrePrincipale`, jamais par la classe `Home` (audit C.4)
- 25/10/2026 — § 3 : fiche à onglets répartie en fichiers partiels par onglet (audit C.5)
- 25/10/2026 — § 9 et § 15 : un écran ou une fenêtre n'atteint la fenêtre principale que par `IFenetrePrincipale` (ADR-55) ; l'élévation d'accès ne s'ouvre que par la fenêtre principale
- 24/10/2026
  - lignes système : les codes dont dépend le comportement sont dans `CodesSysteme`, ni le code ni le SQL ne les écrivent en clair ; une règle qui peut se dire par une case du référentiel y est lue (audit C.3, ADR-54)
  - § 15 : règle « nommer par le code » du 17/10 étendue — le code toujours visible (fenêtres, grilles, listes de choix, annonces d'enregistrement), demande de Joëlle
  - § 13 : lisibilité des documents (ligne de mise à jour courte, historique repliable, paragraphes courts)

</details>

## Objectif
Ce document fixe les règles de conception et de développement pour maintenir une application fiable, lisible et reprise facilement.

## Priorité des règles (ordre d'arbitrage)
Quand plusieurs règles entrent en tension, appliquer cet ordre :

1. **Sécurité** (protection des données, secrets, droits, traçabilité)
2. **Architecture** (séparation des responsabilités, couches, points d'accès uniques)
3. **Style & conventions** (noms, présentation, homogénéité)

**Règle** : Une règle de niveau inférieur ne doit jamais affaiblir une règle de niveau supérieur.

---

## 1. Philosophie générale
- Réfléchir avant de coder.
- Toute fonctionnalité est analysée, décrite et validée.
- Développement progressif, sans accumulation de code non stabilisé.
- Validation étape par étape obligatoire.

## 2. Approche de développement
- Refactorisation autorisée et encouragée si elle améliore clarté/maintenabilité.
- Pas de développement exploratoire sur des éléments structurants.
- Toute évolution doit rester traçable (documentation + changelog).

## 3. Qualité du code
### Clarté
- Noms explicites.
- Aucun code « magique ».
- Lisibilité immédiate.

### Structure
- Une classe = une responsabilité.
- Séparation claire des rôles.
- Aucune logique mélangée.
- **Fiche à onglets trop longue** (audit C.5, ADR-56) : un fichier partiel par onglet lourd, `<Classe>.<Onglet>.vb` (`Partial Class`, en-tête du kit), rangé sous la fiche dans `Althea.vbproj` (`DependentUpon`, `SubType` Code). Le fichier principal garde les champs, le chargement commun, les modes et la garde. On ne coupe pas un écran d'un seul tenant par « Initialisation / Actions ».

### Tests automatisés (ADR-58)
- Projet `Tests/Althea.Tests` (xUnit, VB), à côté de l'application et exclu de sa compilation ; `Tools\test.cmd` lance tous les tests (journal `obj\_tests.log`).
- Une fonction de calcul ou de validation sans écran ni base, nouvelle ou modifiée, reçoit ses tests ; un défaut corrigé dans une telle fonction reçoit son test de non-régression.
- Une classe de tests par module testé, rangée comme lui ; noms `Methode_Cas_Attendu` ; données fictives uniquement ; un résultat attendu qui est un calcul (clé de contrôle, empreinte) est calculé hors du code testé.
- Avant un commit qui touche le métier ou les utilitaires : `Tools\test.cmd` au vert.

## 4. Architecture applicative
### Structure globale
- `Core` → technique
- `Metier` → logique métier
- `UI` → interface
- `Utils` → utilitaires

### Principes
- Séparation stricte des couches.
- Aucun mélange UI / métier / accès DB.
- SQL centralisé dans les modules Query.
- Toute logique répétable est factorisée.
- Les règles métier variables sont portées par les référentiels, pas par le code (cf. § 6).
- **Une règle de saisie ou d'action se définit une fois, dans le métier** (ADR-51, audit C.2) :
  - un valideur `Gestion*.Valider…` rend la liste des erreurs (`ErreurSaisie` : propriété en cause par `NameOf`, message)
  - l'écran l'appelle pour afficher les erreurs sur ses champs, la méthode d'écriture l'applique et refuse (`ErreurValidationException`).
  - Ce qu'un élément permet (reporter, encaisser, statuts proposés…) est une fonction du métier lue par les boutons.
  - L'écran ne recopie jamais la règle.

## 5. Sécurité
- Aucun mot de passe en clair.
- Aucun secret dans les logs.
- Toute exception est gérée (aucune exception silencieuse).
- Toute action sensible est validée côté métier.

### Rôles
- Rôles actuels : `User`, `SuperUser`, `Admin`.
- Les droits ne sont jamais contrôlés uniquement par visibilité UI.
- L'élévation temporaire doit être explicite, tracée, et réversible.
- Une méthode métier sensible reçoit un `ContexteOperation` (utilisateur et rôle courant, élévation comprise, construit par `UserControlContext.ContexteOperation`) et commence par `Autorisation.Exiger` / `ExigerUtilisateur` (ADR-50)
  - jamais de paramètre `roleCourant` isolé.
  - Tout nouveau droit s'ajoute au tableau rôle × opération de [Securite_Utilisateurs.md](../Architecture/Securite_Utilisateurs.md).

### Gestion des utilisateurs
- **Modes d'édition** : `Creation`, `Modification`, `Consultation`.
- **Mode Consultation** : ouvert aux SuperUsers pour visualisation seule.
- **Mode Création** : génération automatique de mot de passe temporaire, copie dans le presse-papiers, flag `must_change_password = True`.
- **Mode Modification** : login en lecture seule (non modifiable), modification des autres champs selon droits.
- **Actions de maintenance** :
  - Réinitialisation de mot de passe : via `ChangePassword` en mode `AdminReset` ; Admin (rôle courant, élévation comprise), vérifié par le métier
  - Déverrouillage de compte : Admin, vérifié par le métier
  - Activation/désactivation de compte : Admin uniquement
- **Mot de passe** : règle unique `PolitiqueMotDePasse` (ADR-51), appliquée par le métier ; ne jamais la recopier dans un écran (le texte affiché vient de `PolitiqueMotDePasse.Description`).
- **Journalisation** : toutes les actions de sécurité sont loggées (création, modification, reset password, déverrouillage, activation/désactivation).

## 6. Base de données
### Principes
- Clé primaire numérique obligatoire (`id_xxx`).
- Clés : **séquences** (`seq_*`) pour les tables métier ; `AUTO_INCREMENT` admis pour les référentiels (`ref_*`), les liaisons de paramétrage et les tables techniques (`tec_*`) (décision rappelée par Joëlle le 13/10/2026).
- Codes métier lisibles.
- Pour MariaDB avec séquences : utiliser `LASTVAL(nom_sequence)` pour récupérer l'ID généré.
- **Version du schéma** : chaque migration met à jour, dans le même commit, `VersionSchema.Attendue` (`Core/Database/VersionSchema.vb`) — c'est la version que l'application attend (écran État du système ; contrôle au démarrage à venir).

### Référentiels
Structure normalisée :
- `id`
- `code`
- `libelle`
- `actif`
- `ordre_affichage`

### Pilotage par les référentiels (règle structurante)
**Toute règle métier variable doit être portée par un référentiel, jamais par le code.**

L'ajout d'une nouvelle valeur métier (domaine, convention, tarif, quota, nature de séance…)
doit pouvoir se faire **entièrement par paramétrage**, sans modification ni recompilation.

En pratique :
- Pas de constante de code métier en dur (`CODE_PRIX_INAMI = "INAMI"`) servant de clé de règle.
  Une telle constante n'est admise que comme **valeur de repli** documentée, jamais comme pivot unique.
- Pas de booléen générique gouvernant plusieurs comportements distincts. Un drapeau du type
  `est_inami` mélange circuit tarifaire, règle de no-show et quota : dès qu'un second cas
  apparaît avec des règles partiellement différentes, il devient inexprimable.
  Préférer des attributs séparés rattachés à l'entité concernée.
- Les paramètres susceptibles d'évoluer dans le temps (plafonds, tarifs, seuils) sont
  **historisés par dates de validité** et résolus à la date de l'événement métier, afin qu'une
  révision ne modifie pas rétroactivement des données déjà produites.
- Toute valeur dérivable (un total, par exemple) est **calculée et non stockée**.
- Tout référentiel paramétrable est accompagné de son écran d'administration.
- L'application doit rester fonctionnelle si le référentiel n'est pas encore alimenté :
  repli documenté + journalisation de l'anomalie, jamais d'échec silencieux.

Référence d'implémentation : `ref_quotas` + `ref_categories_quota` + `ref_quotas_plafonds`
(rattachement au domaine, dates de validité, catégories libres, plafond total calculé, repli journalisé).
Autre exemple : `ref_statuts_seance.consomme_quota` remplace une liste de statuts codée en dur ; de même `est_absence` remplace le code `ABSENTE` (22/10/2026).

### Domaine = convention (ADR-27)
- Un **domaine** représente une convention : tarif + organisme + quotas. Un nouvel organisme de
  remboursement se traduit par un **nouveau domaine**.
- L'organisme (`ref_organismes`) est facultatif et purement descriptif : aucun comportement n'est déduit de son code.
- Les quotas restent rattachés au **domaine** avec période de validité, jamais au dossier ni au patient.
- La catégorie de quota consommée par une nature est portée par la **liaison** nature–domaine
  (`lia_natures_seance_domaines.id_categorie_quota`), pas par la nature.
- **Une règle de quota qui a servi ne se modifie plus** (ADR-40, qui remplace la règle « jamais ») :
  dès qu'une séance du domaine tombe dans sa période, on ajoute une nouvelle règle (l'ancienne est
  clôturée automatiquement la veille) ou on la clôture ; ni modification ni suppression.
- Une règle **qui n'a pas servi** peut être corrigée ou supprimée ; la suppression rouvre la règle
  précédente qu'elle avait clôturée. Les contrôles sont dans `GestionQuotasInami`, jamais dans l'écran.
- Les compteurs de quota ne sont jamais stockés : une règle peut commencer dans le passé.
- Une période sans règle n'est **jamais comblée automatiquement** (aucune date d'une autre règle n'est
  déplacée d'office) : elle est montrée, annoncée avant l'action qui la crée, et doit être couverte
  par une règle intermédiaire.
- Une catégorie utilisée (liaison ou plafond) ne peut être que désactivée.
- Écran de référence : `UC_DomainesHome` (menu Home › Domaines). Guide : `Docs/Guide/Guide_Utilisateur_Domaines.md`.

### Lignes système (ADR-28)
- Une ligne `est_systeme = 1` (ex. organisme `INAMI`, statuts techniques) ne peut ni changer de code,
  ni être désactivée, ni être supprimée. Le contrôle est fait dans la couche `Gestion*`, pas seulement dans l'UI.
- Les codes de ces lignes dont dépend le comportement (statuts de séance, de dossier et de paiement, types et
  sources de document) sont définis **une seule fois**, dans `CodesSysteme` (ADR-54, 24/10/2026) :
  - jamais écrits en clair dans le code (`"PAYE"`) ni dans le SQL (`'PAYE'`) ; une requête bâtit son texte sur la
    constante (`Private Const STATUT_PAYE As String = CodesSysteme.StatutsPaiement.Paye`) ;
  - une règle qui peut se dire par une case du référentiel (`consomme_quota`, `est_absence`…) y est lue plutôt que
    tirée d'un code.

### Relations et séparation
- Table N-N : préfixe `lia_`.
- Données métier ≠ données techniques.
- Données UI ≠ données persistées.

### Tarification, natures et quotas
- Le domaine porte les comportements indépendants `tarif_convention`, `no_show_facturable` et `soumis_quota` ; ne pas recréer un booléen générique qui fusionne ces règles.
- Un tarif ou un quota évolutif est résolu à la date de l’événement à partir d’une période de validité ; le montant effectivement appliqué à une séance est figé dans la séance.
- Les liaisons N-N domaine–tarif et domaine–nature sont la source des choix autorisés ; ne pas filtrer une interface au moyen d’une liste de codes en dur.
- Les codes de prestation servent au décompte des quotas ; les libellés et codes de nature sont des données de présentation, pas des pivots de règle.
- Un total dérivable, tel que le plafond total ou le reste dû, est calculé. Il ne doit pas être persisté comme valeur autonome.

## 7. Environnement technique
- Framework : `net8.0-windows`.
- Packages compatibles uniquement.
- `MySqlConnector` obligatoire.
- `Option Strict On` réglé pour tout le projet (`Althea.vbproj`, ADR-57) : aucune conversion implicite restrictive ni liaison tardive, Designer compris.
- Un identifiant `BIGINT UNSIGNED` (propriété `ULong`) se lit par `reader.GetUInt64(...)`.
- `.editorconfig` à la racine : conventions d'écriture (indentation, `.vb` en UTF-8 avec BOM, préférences de style) et niveau de chaque règle des analyseurs ; un commentaire s'y écrit sur sa propre ligne.
- Analyseurs .NET, jeu « Recommended » : une règle déjà rencontrée est une suggestion ; on ne la durcit (`warning`) qu'après en avoir corrigé les cas. Le journal de compilation reste à 0 avertissement.

## 8. Démarrage application
- Démarrage via `Home`.
- Connexion DB obligatoire avant déverrouillage UI.

### Connexion DB
- `DatabaseManager` est le seul point d'accès DB.
- Pas de connexion directe.
- `OpenConnection()` doit retourner une connexion valide ou lever une exception.
- Utilisation systématique de `Using`.
- Ne pas dupliquer des vérifications de connexion hors `DatabaseManager` (`conn.State`, `conn IsNot Nothing`, etc.).
- Une opération qui écrit plusieurs fois passe par `DatabaseManager.ExecuterEnTransaction` : ses écritures reçoivent `(conn, tx)` et n'ouvrent pas leur propre connexion ; tout ou rien (ADR-46).

## 9. UI / Navigation
### Structure
- Une seule Form principale : `Home`.
- Pas de MDI.
- Navigation centralisée via `NavigationManager`.
- Un seul écran actif.

### Écrans
- Écrans principaux = UserControls.
- Forms réservées aux cas ponctuels/techniques/modaux.

### Interdictions
- Pas d'accès DB dans l'UI.
- Pas de logique métier dans l'UI.
- Pas de manipulation directe de `pnlContent` hors mécanisme prévu.
- **Jamais la classe `Home` dans un écran ou une fenêtre** (audit C.4, ADR-55) : pas de `TryCast(FindForm(), Home)` ni de paramètre `As Home`. Un écran lit `_context.FenetrePrincipale` (`IFenetrePrincipale`) ; une fenêtre la reçoit en paramètre (`fenetrePrincipale As IFenetrePrincipale`). Nothing est normal (écran dans une fenêtre de référentiel) : masquer ce qui en dépend. Une capacité nouvelle de la fenêtre principale s'ajoute au contrat. Seul l'outil de capture (Debug) connaît `Home`.
- **Élévation d'accès** : uniquement par la fenêtre principale (`DemanderElevation` pour un bouton d'écran ; les zones réservées la demandent elles-mêmes) ; jamais de `New ElevationAcces` dans un écran.
- Pas de `SplitContainer` (instabilités identifiées).

### Construction des écrans — Designer obligatoire (règle structurante)
- **Tout Form ou UserControl doit être construit via le concepteur WinForms** (fichier `*.Designer.vb` généré), **jamais en instanciant et positionnant les contrôles à la main dans le code** (pas de `New Label()` / `New Button()` / `Controls.Add(...)` / `.Location = ...` dispersés dans le `.vb`).
- **Motivation** : les écrans montés en code échappent à l'aperçu du concepteur et rendent **très coûteux** l'alignement sur le branding (couleurs, polices, marges, `UITheme`) lors des évolutions visuelles ; un changement de charte doit rester une opération centralisée et non un patch manuel sur chaque contrôle.
- **Séparation** : le `.Designer.vb` porte la **disposition et l'apparence** ; le `.vb` code-behind porte **uniquement le comportement** (chargement des données, validation, événements). Appliquer l'apparence via `UITheme` / styles partagés, jamais par des valeurs codées en dur au cas par cas.
- **Icône d'en-tête** : choisie dans le Designer (propriété `Icone` de `enTete`, image dans le `.resx`) depuis `Assets/Titre_Ico` (noir et vert, fond clair) ; jamais chargée par le code ni prise dans les icônes de tuiles (noir et blanc). Exception : référentiels via `IconeTitre`.
- **Exceptions admises** (à documenter) : contrôles réellement dynamiques (nombre variable connu seulement à l'exécution, p. ex. génération de lignes/onglets pilotée par les données) ; dans ce cas, factoriser le style via `UITheme` / helpers communs.
- **Dette existante à résorber** : les écrans encore montés en code doivent être **repassés par le concepteur** dès qu'ils sont retouchés.
  - Le repassage est sûr tant que les **noms des contrôles et la logique du code-behind sont conservés** : déplacer la construction des contrôles et leurs propriétés visuelles vers un `*.Designer.vb` avec un `InitializeComponent()`, sans changer les signatures publiques ni le comportement.
  - `DialogPaiementReaffectation` et `PlanningJourViewer` ont été traités de cette façon (références d'implémentation).
- **Écrans montés en code retenus comme exceptions** (branding déjà centralisé via `UITheme` / `UtilsButtons` / `UtilsDataGrid`, et/ou disposition réellement dynamique — conversion écartée car elle figerait les couleurs en dur ou casserait l'aperçu du concepteur) :
  - `UC_LiaisonsPrixDomaines` et `UC_LiaisonsNaturesDomaines` : colonnes et libellés de la grille **reconstruits dynamiquement** selon la propriété `Mode` (parent tarif ou parent domaine) ; styles entièrement pilotés par `UITheme`, `UtilsButtons.InitStandardButton` et `UtilsDataGrid.InitDataGridSecondaire`.
  - `ReferentielModalHost` : hôte modal **générique et volontairement minimal** (barre d'actions + bouton Fermer repositionné dynamiquement) hébergeant une vue variable en `DockStyle.Fill`.
  - `UC_SyntheseDomaine` (V2, 11/10/2026) : l'en-tête est dans le Designer, mais les **blocs** (règles, tarifs, natures, quota, complétude) sont construits en code car leur contenu dépend des données ; couleurs `UITheme`, polices partagées.
- **`TableLayoutPanel` dont les lignes sont en hauteur absolue** (lignes construites par le code) : ajouter une **dernière ligne en pourcentage (100 %)** vide ; sans elle, l'espace restant est attribué à la dernière ligne réelle, qui s'étire (constaté sur le Suivi clinique, 11/10/2026).
- **Panneau en `Dock = Fill` dans une cellule de `TableLayoutPanel`** (ou tout conteneur qui le redimensionne) : sa `Size` doit être **déclarée dans le Designer** et cohérente avec la position de ses enfants.
  - Sans elle, le panneau vaut 200 × 100 au moment où les enfants sont posés, et un enfant ancré à droite garde un écart négatif : il déborde ou disparaît une fois le panneau élargi (constaté sur le récapitulatif de la fiche Dossier, 11/10/2026).
  - Règle pour ces exceptions : conserver le style centralisé (aucune valeur `Color.FromArgb` en dur ; passer par `UITheme` / helpers) afin qu'un changement de charte reste une opération centralisée malgré l'absence de `*.Designer.vb`.

## 10. Cohérence UI
- Style uniforme obligatoire.
- Composants homogènes dans toute l'application.
- Aucune variation locale non validée.
- Référence UI (construire un écran, catalogue des contrôles) : [Reference_UI.md](./Reference_UI.md)

### Grilles métier denses et contextualisées
- Avant de supprimer une information ou d’ajouter une navigation, privilégier un affichage contextuel compact : en-têtes multi-lignes, colonnes courtes, recherche et filtres adaptés.
- Utiliser `UtilsDataGrid.ActiverEntetesMultiLignes` pour une grille dont les titres imposent un ascenseur horizontal, après vérification de la lisibilité réelle.
- Une grille de paiements doit afficher le contexte de la séance (date et nature) : le code technique seul ne suffit pas à expliquer une ligne métier.
- L’état d’un paiement et le solde d’une séance sont distincts. Dans la colonne « Reste dû » : `ANNULE` affiche « Annulé », un paiement non encaissé affiche « Prévision », « Soldé » est réservé à une séance effectivement réglée après encaissement, sinon afficher le montant restant.
- Toute couleur ou police appliquée au rendu d’une grille doit utiliser `UITheme` ou les styles existants ; elle doit compléter le texte, jamais le remplacer.

### Composition grille + panneau de détail (règle structurante)
- Une grille de consultation accompagnée d'un panneau « Détail » ne doit **pas** être en `Dock = Fill` avec un panneau en `Dock = Right` : cela produit un large vide central dès que les colonnes sont étroites.
- La grille est en `Dock = Left`, sa largeur est **calée sur la somme de ses colonnes visibles** ; le panneau est positionné au runtime à `grille.Right + 60 px` (≈ 2 cm), avec le même `Top` et la même hauteur.
- La logique est portée **une seule fois dans l'écran de base** (`UC_ReferentielBase.PositionnerPanneauDetail` / `AjusterHauteurPanneauDetail`), jamais dupliquée dans les écrans dérivés.
- Si le contenu du panneau dépasse (écran avec zone de données liées, p. ex. Tarifs de séance), le panneau déborde d'abord sous la grille, puis les champs sont remontés ; le contenu ne doit jamais être tronqué.
- L'espace libre à droite est laissé au fond décoratif de `pnlForm`.

### Titres de page responsives (règle structurante)
- Un titre ou sous-titre (`lblTop`) ne se centre **jamais** par une `Location` calculée : il se décalerait dès que la page s'élargit (fenêtre agrandie ou maximisée).
- Le label est **étiré** et le texte centré à l'intérieur : `AutoSize = False` + `TextAlign = MiddleCenter`, avec `Dock = Fill` s'il est seul dans son conteneur, sinon `Anchor = Top, Left, Right`, `Location.X = 0` et largeur égale à celle du panneau parent.
- Ce réglage est défini **dans le Designer**, jamais par code.

### Habillage des contrôles (règle structurante)
- Les contrôles sont **dessinés par le kit** (`AltheaBouton`, `AltheaCarte`, `AltheaFondFenetre`…) et réglés dans le Designer : aucune image de fond étirée. L'essai d'habillage par images (`/Assets/ui-backgrounds/`, 09/10/2026) est abandonné ; ses fichiers ont été supprimés au lot UI-8 (17/10/2026).
- Codes couleur sémantiques des boutons (`Variante` d'`AltheaBouton`) : **Sauge** = action courante, **Ocre** = Enregistrer, **Terre** = Supprimer. Toute nouvelle teinte doit être ajoutée à la charte avant usage.
- **Le code ne réécrit jamais le `BackColor` d'un contrôle possédant un `BackgroundImage`** : `UtilsButtons` garde cette protection (`HasBackgroundImage` / `SetStandardButtonBackColor`) ; les contrôles du kit forcent de toute façon leur `BackColor` à `Transparent`.
- Référence visuelle : [Charte_Graphique.md](./Charte_Graphique.md) § Kit d'interface Althéa.

## 11. Boutons
### Principes
- Aucun traitement métier dans les événements `Click`.
- Appel de méthodes dédiées.
- Initialisation via `UtilsButtons`.
- Cohérence graphique via `UITheme`.

### Standards
- `Tag` obligatoire : nom image `_normal` sans extension.
- États visuels centralisés (`normal`, `hover/selected`, `disabled`).
- Les alignements du Designer ne doivent pas être écrasés par `UtilsButtons`.
- L'état `Enabled` reste piloté par la logique écran.
- La couleur d'un bouton du kit vient de sa `Variante` (Sauge, Ocre pour **Enregistrer**, Terre pour **Supprimer**), jamais d'une image de fond.

### Initialisation
- Tout bouton est initialisé dans le `Load` de la Form/UserControl.

## 12. Gestion des erreurs & logs
- Tout `Catch` doit produire un log exploitable.
- Aucun log de mot de passe ni de chaîne de connexion sensible.
- Utiliser le mécanisme de masquage des secrets.
- Chaque session applicative produit un en-tête de log (date/heure, machine, utilisateur).
- Tout nouveau processus inclut logging structuré + catégorie adaptée (`Startup`, `Database`, `UI`, `Process`, `Security`).
- Une opération qui écrit des données (sauvegarde, export…) ne réussit jamais « à moitié » en silence : écriture sous un nom provisoire puis renommage, contrôle de ce qui a réellement été lu/écrit, échec explicite qui nomme l'élément fautif (ADR-37).
- Aucune donnée de patient en clair laissée sur le disque par un traitement technique (fichiers temporaires supprimés, dump directement chiffré).
- **Pas de donnée personnelle dans les journaux** (ADR-39) : on journalise des identifiants (`ID=…`, codes), jamais un nom, un prénom, un NISS, une adresse, un e-mail ou un contenu clinique.
  - Le masquage de `GestionLog` couvre les mots de passe et la valeur citée par MariaDB dans une erreur de doublon ; le reste est la responsabilité de chaque message.
- **Recherche** (ADR-52) : une recherche ne journalise que son type et le nombre de résultats, jamais ce qui a été cherché (code, nom, période) ; ses listes sont plafonnées (`GestionRecherche.LIMITE_RESULTATS`).
- Consultation : écran Journaux (Admin), lecture seule ; le format d'une ligne est défini une seule fois dans `GestionLog` (constantes partagées avec `LecteurJournaux`).

## 13. Documentation & traçabilité
- En-tête de fichier versionné.
- En-tête de méthode.
- `CHANGELOG.md` mis à jour.
- Documentation de décision maintenue.
- Résumé quotidien (OneNote) maintenu.
- Une mise à jour documentaire modifie en priorité le document de référence existant. Un document complémentaire n’est créé que pour un contenu autonome clairement identifié, puis il est référencé depuis le README.
- Avant de déclarer une brique « terminée », vérifier les noms, relations et comportements dans le code et les migrations concernés.
- **Lisibilité des documents** (décision de Joëlle, 24/10/2026) :
  - en tête, une ligne « Dernière mise à jour : JJ/MM/AAAA — ce qui a changé », courte ;
  - les mises à jour précédentes vont dans un bloc repliable, une date par puce, la plus récente en haut :
    `<details>` / `<summary>Historique des mises à jour</summary>` / liste / `</details>` (lignes vides autour de la liste) ;
  - pas de longs paragraphes compacts : une idée par ligne ou par puce, les énumérations en listes ; les tableaux restent des tableaux.
- **Grands documents scindés** (décision de Joëlle, 25/10/2026, audit lot D point 4) :
  - la documentation technique UI est un sommaire (`Docs/Architecture/Documentation_technique_UI_Althea.md`) et cinq parties par domaine dans `Docs/Architecture/UI/` (socle, administration, référentiels et domaines, patients et dossiers, recherche) ;
  - une nouvelle fiche de composant va dans la partie de son domaine, et sa ligne dans le tableau de vue d'ensemble du sommaire ;
  - les processus : sommaire `Docs/Architecture/Process_Althea.md` et trois parties dans `Docs/Architecture/Processus/` ; un nouveau processus prend le numéro suivant, va dans la partie de son domaine et dans le sommaire des processus ;
  - `CHANGELOG.md` ne garde que le mois en cours : au début de chaque mois, les entrées du mois précédent rejoignent l'archive de l'année (`Docs/Archives/CHANGELOG_AAAA.md`), sans être réécrites ;
  - les liens vers une fiche ou un processus pointent vers la partie qui le contient, pas vers le sommaire.

## 14. Paramètres applicatifs
- Paramètres stockés dans `tec_parametres`.
- **Un paramètre naît par migration** et s'ajoute en même temps dans `ClesParametres` (clé + valeur de secours) et dans `Docs/Database/Maintenance/reinit_tec_parametres.sql` ; l'écran n'en modifie que la valeur (ADR-36).
- Le code lit un paramètre **uniquement par sa clé de `ClesParametres`**, via `GestionParametres.GetEntier` / `GetBooleen` : valeur ramenée dans les bornes, valeur de secours journalisée si la valeur manque ou est invalide. Jamais de littéral de clé dispersé dans le code.
- Une seule valeur par paramètre, sans dates de validité ; elle s'applique aux prochaines opérations.
- Aucune suppression physique ; désactivation via `actif`.
- `cle_parametre` est technique et immuable après création.
- Clés/groupes normalisés : MAJUSCULE, sans accents, sans espaces.
- **Un groupe naît par migration** dans `tec_groupes_parametres` (code, titre de la carte, ordre ; v2.40.0) : un paramètre ne peut nommer qu'un groupe existant (clé étrangère). L'écran Paramètres affiche une carte par groupe, sans code à modifier.
- Type contrôlé via listes fermées quand applicable.

### Chemins
- Seule la racine du dossier de données (`PATH_GENERAL`) est paramétrable ; les sous-dossiers `Documents` / `Cabinet` / `Utilisateurs` sont fixes et obtenus par `ArborescenceDonnees` (ADR-35).
- Un chemin saisi (type PATH) est complet et vérifié ; un dossier de sauvegarde ne peut pas être dans le dossier de données.
- Stockage en segments logiques en base.
- Construction des chemins via `Path.Combine`.
- Variables système résolues côté code.

## 15. UI globale, contexte et modales

### Interfaces de contexte

L'application utilise deux interfaces pour l'injection du contexte UI :

- **`IContextAwareUserControl`** : pour les UserControls chargés dans `Home` via `NavigationManager`.
- **`IContextAwareForm`** : pour les Forms modales ouvertes depuis `Home` ou un UserControl.

**Implémentation obligatoire** :

```vb
' UserControl
Public Class UC_MonModule
	Implements IContextAwareUserControl

	Private _context As UserControlContext

	Public Sub SetContext(context As UserControlContext) _
		Implements IContextAwareUserControl.SetContext
		_context = context
	End Sub
End Class

' Form modale
Public Class MaFormModale
	Implements IContextAwareForm

	Private _context As UserControlContext

	Public Sub SetContext(context As UserControlContext) _
		Implements IContextAwareForm.SetContext
		_context = context
	End Sub
End Class
```

### UserControls dans `Home`
- Ne pas créer localement `StatusStrip`, `ToolTip`, `ErrorProvider`, label de contexte.
- Utiliser `UserControlContext` via `IContextAwareUserControl`.
- Interactions via méthodes de contexte (`SetStatus`, `SetHeader`/`SetContexte`, `SetToolTip`, `SetError`, `ClearError`).
- Fil d'Ariane : `SetHeader` reçoit le chemin complet, segments séparés par « > » ; le dernier est l'écran courant, les précisions vont entre parenthèses ou crochets (atténuées à l'affichage). Une précision qui doit ressortir (domaine du dossier) passe par le second paramètre `pastille`, jamais dans le texte. Un appel sans pastille l'efface.

### Forms autonomes avant `Home`
- Ex.: `Login`, `ChangePassword` depuis `Login`.
- Peuvent conserver leurs composants locaux.

### Forms modales ouvertes depuis `Home`
- Utiliser `IContextAwareForm`.
- Appeler `SetContext(...)` avant `ShowDialog(...)`.
- Restaurer le contexte précédent après contexte temporaire.
- **Statut et infobulles via `_context`** : router les messages de statut (`SetStatus`) et les infobulles (`SetToolTip`) vers le contexte partagé de `Home`, afin que les messages s'affichent toujours au même endroit. Prévoir un repli sur le `ToolTip` local si `_context` est `Nothing`.
- **`ErrorProvider` conservé local** : une Form modale `ShowDialog` garde son `ErrorProvider` local (son `ContainerControl` est la modale). Un provider partagé positionnerait les icônes d'erreur sur `Home` en arrière-plan. Exception assumée à la règle « pas de composants locaux ».
- **Init des infobulles dans `Load`** (et non le constructeur) : le contexte n'est injecté qu'après construction, via `SetContext` appelé avant `ShowDialog`.
- Référence : `ContactEdition`, `IntervenantEdition`, `TherapeuteEdition`, `UtilisateurEdition`.
- **`DialogResult` des boutons** : un bouton qui ferme une boîte de dialogue porte son `DialogResult` (OK, Cancel…) dans le Designer, ou un gestionnaire `Click` qui le pose.
  - `AcceptButton` ne le donne pas (seul `CancelButton` pose Cancel).
  - En remplaçant un bouton (ex. par `AltheaBouton`), **reporter cette propriété** : sa perte au lot UI-4 a rendu inopérants « Reporter » (report de séance) et la réaffectation d'un paiement (corrigé le 15/10/2026).

### Fenêtres : même schéma que les fiches (règle, 15/10/2026)
- **Toute fenêtre de saisie** (Form) suit le schéma des fiches : en-tête `AltheaEnTete`, **champs regroupés dans des `AltheaCarte` titrées** (par sens : ex. « Prestation » / « Rendez-vous » dans `SeanceEditor`), barre d'actions `AltheaBarreActions`. Aucun champ posé directement sur le fond.
- **Fond et barre de titre** : `AltheaFondFenetre.Appliquer(Me)` dans le constructeur (beige clair, petite feuille dessinée, barre de titre Windows aux couleurs du kit via `AltheaBarreTitre`) ; plus d'image de fond.
  - Une fenêtre sans ce fond (Home, connexion…) appelle `AltheaBarreTitre.Appliquer(Me)`.
  - Pas de panneau racine `pnlForm` : le contenu est posé sur la fenêtre (ne pas donner de police à la fenêtre en le retirant, `AutoScaleDimensions` suit la police par défaut).
- **Champs du kit** : `AltheaListe`, `AltheaDate`, `AltheaCase` ; libellés Calibri 10,5 gras, champs Calibri 11 ; petits boutons à côté d'un champ ([+], loupe, cadenas…) en `AltheaBouton` Outil à icône seule, sans `UtilsButtons`.
- **Consultation lisible** (ci-dessous) ; l'état de saisie est posé à un seul endroit (ex. `DefinirEtatChamps`, `DefinirSaisieActive`).
- Référence : `SeanceEditor`, `UtilisateurEdition`, `ContactEdition`.

### Le code toujours visible (règle du 17/10/2026, étendue le 24/10/2026)
Un élément qui porte un code généré par la base (PA, DO, SE, PAI, FC, AS, TH, USR…) le montre partout où on le désigne. Les noms, dates et libellés seuls ne suffisent pas à distinguer deux éléments.

- **Fenêtre ouverte sur un élément existant** (consultation ou modification) : le sous-titre de l'en-tête — ou le titre — le nomme par son code (« Modifier la séance SE000074 », « Consulter le contact — FC000004 »).
- **Grille qui liste des éléments codés** : une colonne **Code**, visible, jamais masquée.
- **Rappels et listes de choix** : le code en tête (« SE000074 · 12/10/2026 … »). Quand on choisit **par le nom** (thérapeute, compte), le code suit le nom (« Marie DUPONT (Logopède) · TH000003 »), pour que la frappe d'une lettre saute encore au bon nom (décision de Joëlle, 24/10/2026).
- **Annonces d'enregistrement** : barre d'état ou message citent le code (« Contact FC000012 créé. ») ; à la création, le code généré par la base est relu.
- **Exception** : un aperçu compact (panneau Aperçu patient) peut ne le porter que dans l'infobulle ; il apparaît dès que l'élément est ouvert.

Détail et mise en œuvre : Reference_UI § 5.6.

### Édition d'un élément existant — consultation d'abord
- Ouvrir un élément **existant** (double-clic en grille) en **consultation** (lecture seule) ; la **création** ouvre directement en saisie.
- En consultation : champs **et** actions annexes (`[+]`, copie d'adresse…) désactivés ; bouton **Modifier** pour basculer en édition, bouton **Fermer** pour sortir sans rien changer.
- Centraliser le basculement dans une méthode unique (`AppliquerMode` + `DefinirEtatChamps(editable)`) pilotée par un drapeau `ouvrirEnConsultation`.
- Référence : `ContactEdition`, `IntervenantEdition` (cohérent avec ADR-10).

### Consultation lisible — lecture seule sans effacement (règle structurante, 14/10/2026)
- Un champ **consulté** se lit sans effort : texte dans sa couleur normale, seul le **fond** se teinte (`UITheme.ColorChampLectureSeule`) ; un champ **modifiable** a un fond blanc. Seuls les **boutons** indisponibles s'estompent.
- Ne jamais mettre une zone entière (carte, panneau, onglet) en `Enabled = False` pour signifier la consultation : Windows grise alors aussi les libellés et les valeurs.
- Basculer avec `UtilsControls.DefinirLectureSeule(zone, lectureSeule)` — ou `DefinirChampLectureSeule(champ, lectureSeule)` pour un champ seul (15/10/2026) : zones de texte en `ReadOnly`, nombres sans flèches, boutons indisponibles, libellés intacts.
  - `AltheaCase` et `AltheaListe` désactivées se dessinent déjà lisibles ; `AltheaDate` passe par sa propriété `LectureSeule` (texte normal, fond teinté, sans bouton).
- **Dates** (14/10/2026) : utiliser `AltheaDate` (champ du kit, saisie et calendrier) à la place du `DateTimePicker`, qui ne peut être ni habillé ni rendu lisible en consultation.
  - Date facultative : `Facultative = True` (vide = pas de date), jamais la case à cocher Windows.
  - Les remplacements se font écran par écran avec UI-7 ; les champs d'heure gardent provisoirement le contrôle Windows.
- **Heures** (16/10/2026) : utiliser `AltheaHeure` (saisie hh:mm, liste des heures proposées) à la place des listes saisissables et du `DateTimePicker` en mode heure.
- Les `ComboBox`, `CheckBox` et `DateTimePicker` Windows restent grisés une fois désactivés : les remplacer par les champs du kit (lot UI-7). Tour des écrans existants : ToDo, « Lecture seule lisible ».
- Référence : onglet Général de `UC_DomainesHome`.

### Fiche et listes rattachées (ADR-34)
- **Modifier / Enregistrer / Annuler** (barre d'actions de l'écran) concernent les **champs de la fiche**. Les **listes rattachées** (contacts, séances, tarifs ou natures d'un domaine…) se modifient en consultation, par leur **barre locale**, action par action, avec enregistrement immédiat.
- Pendant « Modifier », les listes rattachées sont indisponibles (rien ne se rattache à une fiche non enregistrée) ; une consigne le dit à l'utilisateur.
- **Modifier** n'est montré que sur l'onglet ou l'étape de la fiche ; ailleurs, la consigne dit que l'étape se modifie directement (Reference_UI § 5.7).

### ToolTips
- Initialisation dans une méthode dédiée (`InitializeToolTips` ou `InitialiserToolTips`).
- Pas de ToolTips dispersés dans les événements.
- Dans une modale `IContextAwareForm`, router les infobulles via `_context.SetToolTip` (repli local si pas de contexte) ; appeler l'initialisation dans `Load`, pas dans le constructeur.

## 16. Règles issues de `UC_Parametres` (référence) et évolutions DialogChoix

### UI - Feedback utilisateur

- **Éviter `MessageBox`** : utiliser systématiquement `DialogChoix` pour la cohérence visuelle et l'expérience utilisateur.
- **DialogChoix obligatoire** pour :
  - Messages d'information (`DialogChoix.Information`)
  - Avertissements (`DialogChoix.Avertissement`)
  - Erreurs (`DialogChoix.Erreur`)
  - Confirmations (`DialogChoix.Confirmer`, `DialogChoix.ConfirmerSuppression`)
  - Messages de succès (`DialogChoix.Succes`)
- **MessageBox toléré uniquement** en cas de blocage critique système (ex. : erreur démarrage avant chargement des ressources).
- Séparer Header (contexte) et Status (action).
- Nettoyer `errProvider` à chaque changement d'état.

### DialogChoix - Composant de dialogue personnalisé

**Principe** : `DialogChoix` remplace tous les `MessageBox.Show` de l'application pour garantir une cohérence visuelle avec la charte graphique.

**Types de dialogue** :
- `Information` : messages informatifs
- `Warning` : avertissements
- `Error` : erreurs
- `Question` : questions (utiliser `Confirmer` pour Yes/No)
- `Success` : confirmations de succès
- `Loading` / `Processing` : opérations en cours

**Utilisation** :

```vb
' Méthodes statiques simples (usage courant)
DialogChoix.Information("Opération réussie")
DialogChoix.Erreur("Une erreur s'est produite")
DialogChoix.Avertissement("Attention : données manquantes")
DialogChoix.Succes("Enregistrement effectué avec succès")

' Confirmation Yes/No
If DialogChoix.Confirmer("Voulez-vous continuer ?") = DialogResult.Yes Then
	' action
End If

' V2 (14/10/2026) : boutons qui disent l'action
If DialogChoix.Confirmer("Clôturer cette règle au 31/12/2026 ?", "Clôture de la règle", "Clôturer", "Annuler") = DialogResult.Yes Then …
If DialogChoix.ConfirmerSuppression(message, "Supprimer la règle ?") = DialogResult.Yes Then …

' V2 : message structuré (détails, complément, attention, remarque, question finale)
Using dlg As New DialogChoix()
	dlg.Titre = "Nouvelle règle de quota"
	dlg.TypeDialogue = TypeDialogue.Question
	dlg.Message = "Vérifiez la nouvelle règle avant de l'enregistrer :"
	dlg.AjouterDetail("Plafonds", "Soutien 10 + Traitement 20 = 30")
	dlg.Attention = "…"
	dlg.QuestionFinale = "Enregistrer cette règle ?"
	dlg.VarianteAction = VarianteBouton.Ocre
	dlg.SetBoutons("Enregistrer", "Revenir à la saisie")
	If dlg.ShowDialog() = DialogResult.Yes Then …
End Using
```

**Règles** (V2.0.0, 14/10/2026) :
- L'icône et la teinte de l'en-tête viennent de `TypeDialogue` (icônes dessinées par `AltheaIconeEtat`, couleurs de la charte ; plus de GIF).
- La taille s'adapte au contenu (420 à 640 px de large, 70 % de l'écran au plus, puis défilement).
- **Nouveau code** : des boutons qui disent l'action (« Clôturer », « Enregistrer ») plutôt que Oui / Non ; une action destructrice passe par `ConfirmerSuppression` (ou `VarianteAction = Terre`), Entrée y déclenche Annuler.
- Un récapitulatif à relire utilise les parties structurées (`AjouterDetail`, `Attention`…) plutôt qu'un long texte avec des puces et des « ATTENTION » écrits à la main.
- Jamais de détail technique ni de donnée sensible dans un message : le détail va au journal.
- Mapping DialogResult : Bouton1=Yes (OK s'il est seul), Bouton2=No, Bouton3=Cancel. Échap = dernier bouton.
- Toute évolution se vérifie dans la fenêtre de démonstration `TestDialogChoix`.

### Validation
- Validation visible (icône sur le champ en cause), expliquée (fenêtre récapitulative ou message) ; les erreurs d'un valideur métier s'affichent par `AffichageErreursSaisie.Signaler`.
- Ne jamais laisser un champ technique libre quand une liste contrôlée est possible.

### Architecture
- SQL → Query
- DB → Gestion
- UI → UserControl
- Règle de saisie d'un objet → valideur du module `Gestion*` (ADR-51) ; contrôle de format sans état (NISS, téléphone, IBAN…) → Utils ; affichage des erreurs d'un valideur → `AffichageErreursSaisie.Signaler` (icône sur chaque champ + fenêtre récapitulative + focus, même comportement dans toutes les fiches)

### UX
- Une erreur = feedback local + global.
- Un succès = message discret (DialogChoix.Succes).
- Une action critique = DialogChoix.Confirmer (boutons qui disent l'action) ; une suppression = DialogChoix.ConfirmerSuppression.

## 17. UITheme et couleurs
- Toutes les couleurs UI pilotées par code proviennent de `UITheme`.
- Interdiction de `Color.FromArgb(...)` directement dans Forms/UserControls, sauf exception documentée.
- Les contrôles dynamiques utilisent aussi `UITheme`.

## 18. États dans les grilles : pastilles (UtilsPastilles) ; UtilsIcons

**Principe** (UI-9, 18/10/2026) : un état se lit en **pastille** dessinée par le code, dans un ton sémantique (`TonEtat` : OK, à surveiller, problème, inactif, information, rôle), jamais en icône PNG.

La couleur vient d'une **famille fournie par le métier** (ex. `UtilisateurApplication.EtatCompte`, `ReferentielLigne.Etat`), convertie en couleurs par `UtilsPastilles.Teinte(...)`, et dessinée par `UtilsDataGrid.DessinerPastilleCellule` dans `CellPainting`.

**Règles** :
- Une ligne normale ne porte rien : seules les exceptions se voient ; ce qui est inactif ou terminé s'atténue.
- La règle de priorité entre plusieurs états (ex. Verrouillé > Actif > Désactivé) est portée par la classe métier, pas par l'écran.
- La colonne d'état reste liée à un libellé (`LibelleEtat`…) pour que le tri par en-tête fonctionne.
- Détail, tons et conventions : `Reference_UI.md` (grilles — états lisibles).

**UtilsIcons** ne garde que les icônes dessinées des filtres de la grille Patients (`IconStatutDossierDominant`, `IconPatientSansDossier`). Les accesseurs `IconOK`, `IconOFF`, `IconLock`, `IconNo` et les PNG `Assets/Tech_Ico` ont été retirés le 18/10/2026 (UI-9.4) ; ADR-12 est remplacée.

## 19. Terminologie métier / technique
| Contexte | Terme |
| --- | --- |
| Métier (psychologie) | Dossier |
| Système de fichiers (UI) | Répertoire |
| Technique / code | Path / Folder |

Règle : pas de mélange de terminologie dans un même contexte.

## 20. Écrans dynamiques
- Configurer complètement les contrôles à la création.
- Attacher les événements immédiatement après instanciation.
- Lire explicitement les valeurs UI via méthode dédiée (ex. `LireValeursDepuisUI`).
- Toujours valider l'UI avant opération DB.
- Éviter les rechargements complets inutiles si une mise à jour locale suffit.

## 21. Composant UC_RichTextEditor - Gestion des notes

**Principe** : `UC_RichTextEditor` est le composant standard pour toute saisie de notes formatées dans l'application (anamnèses, bilans, comptes-rendus, correspondances, etc.).

### Règles de sauvegarde obligatoires

Toute utilisation de `UC_RichTextEditor` impose une **sauvegarde double format** en base de données :

1. **Champ RTF (TEXT/MEDIUMTEXT)** : stocke le formatage complet
   - Préserve gras, italique, couleurs, alignement, puces, polices, retraits
   - Utilisé pour l'affichage fidèle du document
   - Exemple : `notes_rtf`, `anamnese_rtf`, `bilan_rtf`, `compte_rendu_rtf`

2. **Champ TXT (TEXT/MEDIUMTEXT)** : stocke le texte brut
   - Texte sans formatage, utilisé pour la recherche full-text SQL
   - Permet des recherches performantes au niveau Dossier/Patient
   - Exemple : `notes_txt`, `anamnese_txt`, `bilan_txt`, `compte_rendu_txt`

### Architecture base de données

Lors de la création ou modification d'une table contenant des notes :

```sql
CREATE TABLE xxx_bilans (
    id_bilan INT PRIMARY KEY,
    id_dossier INT NOT NULL,
    -- autres champs métier
    bilan_rtf MEDIUMTEXT,        -- ⚠️ Obligatoire : formatage préservé
    bilan_txt MEDIUMTEXT,        -- ⚠️ Obligatoire : recherche full-text
    date_creation DATETIME NOT NULL,
    date_modification DATETIME,
    -- ...
    FOREIGN KEY (id_dossier) REFERENCES xxx_dossiers(id_dossier)
);

-- Index full-text pour recherche performante
CREATE FULLTEXT INDEX idx_bilan_txt ON xxx_bilans(bilan_txt);
```

### Code de sauvegarde type

```vb
' Récupération du contenu éditeur
Dim contenuRtf As String = ucEditor.RtfContent
Dim contenuTxt As String = ucEditor.TextContent

' Sauvegarde en base (via classe métier)
bilan.BilanRtf = contenuRtf  ' Formatage complet
bilan.BilanTxt = contenuTxt  ' Texte brut pour recherche
GestionBilans.Enregistrer(bilan)
```

### Code de chargement type

```vb
' Chargement depuis base
Dim bilan As BilanPatient = GestionBilans.ChargerParId(idBilan)

' Affichage dans l'éditeur (RTF uniquement)
ucEditor.RtfContent = bilan.BilanRtf
```

### Règles strictes

1. **Interdiction de stocker uniquement RTF** : sans TXT, pas de recherche possible
2. **Interdiction de stocker uniquement TXT** : perte du formatage professionnel
3. **Vérification création tables** : lors de l'ajout de notes dans un écran, toujours vérifier que la table sous-jacente possède bien les deux champs (`xxx_rtf` + `xxx_txt`)
4. **Cohérence des noms** : même préfixe pour les deux champs (ex. : `anamnese_rtf` / `anamnese_txt`, pas `anamnese_rtf` / `notes_txt`)
5. **Types de champs** : `TEXT` (64KB max) ou `MEDIUMTEXT` (16MB max) selon besoin, **jamais VARCHAR**
6. **Index full-text** : toujours créer un index full-text sur le champ `xxx_txt` pour optimiser les recherches

### Modules métier concernés (à venir)

- **Patients** : notes générales patient (`notes_patient_rtf` / `notes_patient_txt`)
- **Dossiers** : synthèse dossier (`synthese_rtf` / `synthese_txt`)
- **Anamnèses** : historique patient (`anamnese_rtf` / `anamnese_txt`)
- **Bilans** : comptes-rendus bilans (`bilan_rtf` / `bilan_txt`)
- **Séances** : notes consultation (`notes_seance_rtf` / `notes_seance_txt`)
- **Correspondances** : courriers professionnels (`courrier_rtf` / `courrier_txt`)
- **Plans d'accompagnement** : documents évolutifs (`plan_rtf` / `plan_txt`)

### Recherche full-text exemple

```sql
-- Recherche dans tous les bilans d'un patient
SELECT b.id_bilan, b.date_bilan, 
       MATCH(b.bilan_txt) AGAINST ('graphomotricité' IN NATURAL LANGUAGE MODE) AS score
FROM xxx_bilans b
INNER JOIN xxx_dossiers d ON b.id_dossier = d.id_dossier
WHERE d.id_patient = @idPatient
  AND MATCH(b.bilan_txt) AGAINST ('graphomotricité' IN NATURAL LANGUAGE MODE)
ORDER BY score DESC, b.date_bilan DESC;
```

### Point de vigilance

⚠️ **Lors de l'ajout d'un `UC_RichTextEditor` dans un nouvel écran** :
1. Vérifier que la table cible possède les deux champs RTF + TXT
2. Si les champs manquent, créer une migration SQL avant d'intégrer le composant
3. Mettre à jour la classe métier pour exposer les deux propriétés
4. Documenter l'usage dans la documentation technique du module

**Règle** : Aucun `UC_RichTextEditor` (profil Complet ou Compact, ADR-42) ne doit être intégré dans un écran sans vérification préalable de la structure base de données (présence des colonnes `*_rtf` **et** `*_txt`).

Les champs `commentaire` enrichis (`famille_contacts`, `autres_suivis_patient`) suivent la même règle double format (cf. ADR-15, D-Q7bis).

## 22. Nommage & discipline projet
### Nommage
- Noms explicites, sans abréviation obscure.
- Classes : `Patient`, `Dossier`, etc.
- Modules : `GestionX`, `UtilsX`, etc.
- UI : préfixes obligatoires (`txt`, `btn`, `dgv`, `lbl`, `pnl`, ...).
- Champs privés : préfixe `_`.

### Standards de code/documentation
- Respecter `Option Strict On` et `Option Explicit On` : réglés au niveau du projet, et répétés en tête de chaque fichier, nouveau fichier compris (ADR-57).
- Appliquer les standards de commentaires : [Standards-Commentaires.md](./Standards-Commentaires.md)

### Structure des écrans (Forms & UserControls)
- À la création d'une **Form** ou d'un **UserControl**, prévoir systématiquement :
  - **`pnlForm`** : conteneur/fond principal de l'écran ;
  - **`enTete`** (`AltheaEnTete`) : en-tête d'écran — icône, titre, sous-titre (remplace `pnlTitre` + `lblTitreForm` + `lblTop` depuis le 10/10/2026) ;
  - **`pnlActions`** (quand nécessaire) : barre des boutons d'action, placée sous le contenu.
- Objectif : préserver l'homogénéité visuelle et la structure de navigation communes à l'application.

### Grilles (DataGridView)
- Initialisation visuelle via `UtilsDataGrid.InitDataGridBasique`.
- Tri par clic sur en-tête activé via `UtilsDataGrid.ActiverTriColonnes(dgv)` (tri sur `DataPropertyName` ; colonnes non liées, ex. icônes, non triables).

### Icônes de boutons construits par code
- Charger les icônes via `UtilsButtons.LoadImageFromAsset(dossierRelatif, nom)` (sans verrou de fichier, tolérant à l'absence).
- Les assets doivent être copiés vers la sortie (`CopyToOutputDirectory = PreserveNewest` dans `Althea.vbproj`).

### Discipline
- Changelog à chaque évolution.
- Documentation tenue à jour.
- Règles évolutives, mais validées et contrôlées.

### Vocabulaire métier officiel
- **Domaine** (et non « volet ») : domaine d'activité (Psychologie, Graphothérapie, Realism…). Tables : `ref_domaines`, `id_domaine`.
- **Thérapeute** (et non « médecin ») : tout intervenant (médecin traitant, logopède, adresseur…). Table : `therapeutes`.

### Référentiels (UC physique + classe de base)
- Un **UserControl physique par référentiel** (design libre, `.Designer.vb` propre à chaque écran).
les noms réels des UCs sont `UC_Pays`, `UC_Organismes`, `UC_LiensPatient`, `UC_RolesIntervenant`, `UC_RoleLegal`, `UC_SituationsFamiliales`, `UC_StatutsDossier`, `UC_StatutsSeance`, `UC_TypesDocuments`, `UC_TypesRendezVous`, `UC_NaturesSeance`, `UC_PrixSeance`, `UC_ModesPaiement` (pas de préfixe `Ref` dans le nom de fichier).
- Exception : la gestion complète d'un domaine (organisme, tarifs, catégories & quotas, natures, synthèse) se fait dans `UC_DomainesHome` (`UI/Controls/Domaines`), qui n'hérite pas de `UC_ReferentielBase`. `UC_Domaines` reste transitoire.
- Toute nouvelle form ou boîte de dialogue est construite avec le Designer (`.vb` + `.Designer.vb` + `.resx`).
- Les longues listes en mémoire utilisent `UC_Pagination` (tri sur la liste complète, retour page 1 sur nouvelle source).
- `ReferentielLigne` est le modèle de présentation générique — l'UC de base ne connaît pas les modèles métier.
- Les hooks champ supplémentaire (`ConfigurerChampSupplementaire`, etc.) sont no-op par défaut ; les référentiels à attributs propres (ex. `UC_NaturesSeance`, `UC_PrixSeance`, `UC_Domaines`) les surchargent.
- Soft-delete prioritaire : si `<X>EstUtilise()` retourne `True`, désactiver uniquement (jamais supprimer physiquement).
- Cf. ADR-18.

## 23. Chemins fichiers patients — code_patient et helper unique

**Principe** : Tout chemin fichier lié à un patient est calculé de façon **déterministe** par `CheminsPatientHelper`. La DB ne stocke jamais de chemin absolu, uniquement le **nom de fichier** (ADR-16 + ADR-20).

### Règles

- Le dossier patient est `{PATH_GENERAL}\Documents\{code_patient}\` (ex. `D:\Althea_Data\Documents\PA000003\`).
- **`code_patient`** (ex. `PA000003`) est toujours utilisé, jamais l'`id_patient` nu.
- `CheminsPatientHelper` est le **point d'entrée unique** : ne jamais construire un chemin patient manuellement.
- **Créer le dossier** avant toute écriture : `CheminsPatientHelper.AssurerDossierPatient(codePatient)`.
- Photo d'identité : nom déterministe `Identite.{ext}` (extension d'origine, minuscule) — une nouvelle photo remplace la précédente (`overwrite:=True`).
- Exports documentaires : nom **déterministe SANS horodatage** `{prefixe}_{code}[_{zone}].{ext}` (ex. `anamnese_PA000003.pdf`, `suivi_DO000012_motif.docx`) — chaque export **écrase** le fichier existant (document unique par contexte, cf. §25 / ADR-23).
- Après un upload réussi, **mettre à jour la colonne DB** (`patients.photo_fichier`) et l'objet DTO en mémoire.

### Formats d'image acceptés (PictureBox / GDI+)

`.jpg`, `.jpeg`, `.png`, `.bmp`, `.gif`, `.tif`, `.tiff` — validation défensive côté code (indépendante du filtre dialogue).

## 24. Export documentaire — délégation par événement (`UC_RichTextEditor`)

**Principe** : `UC_RichTextEditor` reste un contrôle **générique**. L'export contextuel (chemin déterministe, nom métier) est délégué au **conteneur** via des événements — le contrôle ne connaît aucune règle métier (ADR-21).

### Règles

- Le contrôle lève `ExportRequested` avant l'export : le conteneur renseigne `e.DestinationPath`.
- Si `DestinationPath` est renseigné → export direct ; sinon → fallback `SaveFileDialog` (comportement générique).
- Le contrôle lève `ExportCompleted` après un export contextuel réussi (le conteneur peut ouvrir le fichier ou tracer).
- L'abonnement se fait de façon **déclarative** via `Handles` (`Friend WithEvents`) — jamais via `AddHandler` dans un handler de chargement (risque d'abonnement multiple).
- À la construction de la brique **Documents**, les handlers `ExportCompleted` sont les **points d'ancrage** pour tracer dans la table `documents`. ✅ Fait pour l'anamnèse (`UC_PatientFiche.TracerAnamneseExportee`) et les zones cliniques (`DossierZoneEditor.TracerDocumentExporte`) ; ⚠️ reste à brancher `btnUploadPhoto_Click` (photo d'identité).

### Décision actée (11/07/2026 — ADR-23)

Le **niveau de document** n'est **pas** une colonne dédiée : il se **déduit de la hiérarchie** renseignée dans `documents` (`id_seance` > `id_dossier` > `id_patient`). La table a été corrigée (migration v2.5.0 : `id_patient` nullable + FK, `id_dossier` nullable).

## 25. Documents exportés — unicité par contexte (nom déterministe + upsert)

**Principe** : un document exporté depuis l'éditeur riche (anamnèse, zone clinique) est **unique par contexte + type** : une seule anamnèse par patient, une seule zone donnée par dossier. Il évolue mais ne se duplique jamais (ADR-23).

### Règles

- **Nom de fichier déterministe, sans horodatage** → chaque export écrase la version précédente sur disque.
- **Traçabilité en upsert** via `GestionDocuments.TracerDocument` : recherche par `chemin_relatif` (clé d'unicité naturelle) → `INSERT` la première fois, sinon `UPDATE` (source, extension, taille, `date_document`, `date_modification`).
- **`chemin_relatif` invariant** : stocké relatif à `PATH_GENERAL` (débute au sous-dossier fixe `Documents`, voir ADR-35), reconstructible si la racine change de machine.
- Traçabilité **best-effort** : une erreur d'insertion en base ne doit jamais empêcher l'écriture ni l'ouverture du fichier exporté.
- Un export PDF puis Word du même document cohabitent (clé incluant l'extension) tant que le doublement automatique DOCX+PDF n'est pas implémenté.

## 26. Coloration & Contexte (Grilles Paiements)

### Principe structurant

Le **statut paiement** (En attente / Encaissé / Annulé) doit être DISTINCT du **solde séance** (montant Reste dû).

**Erreur courante à éviter** : conclure qu'une séance est soldée parce que le paiement a un certain statut.

---

## Affichage colonne « Reste dû » (v20/09)

### Sélection du texte/style en fonction du contexte

| Contexte                                | Texte affiché                | Style  | Couleur      | Notes                                      |
| --------------------------------------- | ---------------------------- | ------ | ------------ | ------------------------------------------ |
| Status paiement `Annulé`                | « Annulé »                   | Barré  | Gris         | Jamais facturée = 0 reste dû.              |
| Status `En attente` → facture non reçue | « Prévision »                | Normal | Beige/centré | Utilisateur doit payer.                    |
| Status `Encaissé` + solde séance = 0    | « Soldé »                    | Normal | Vert pastel  | Séance payée en full.                      |
| Status `Encaissé` + solde séance > 0    | Montant (ex. « 45,00 €uro ») | Gras   | Orange       | Paiement partiel reçu ; reste à encaisser. |

---

## Importance de la clarté

### Avant v20/09 (problématique)

```
Paiement1 : Status En attente, colonne Reste → « 50,00 € »
Paiement2 : Status Encaissé, colonne Reste → « Soldé »
Utilisateur pense : « le paiement 2 est reçu = séance est payée
```

**Mauvaise conclusion** : paiement peut être Encaissé partiellement (40€ sur 50€ due).

### Après v20/09 (correct)

```
Paiement1 : Status En attente, colonne Reste → « Prévision »
Paiement2 : Status Encaissé, Reste dû > 0, colonne → « 10,00 €uro »
Utilisateur lit : « paiement 2 reçu MAIS 10€ restent dus »
```

**Bonne conclusion** : paiement partiel, facture incomplète.

---

## En-têtes multi-lignes (Helper UtilsDataGrid)

### Utilisation

```vb
UtilsDataGrid.ActiverEntetesMultiLignes(
	dgv:=dgvPaiements,
	hauteurEntete:=40,
	taillePolice:=9.0
)
```

### Impact

- Réduit largeur colonnes de ~20 % via saut de ligne en-têtes.
- Polices compactes optionnelles (-2pt).
- Élimine ascenseur horizontal sur 1680px.
- Applicable à toute grille de données.

**Résultat** : Vision globale sans scroll, meilleure densité d'info.

---

## Règle de contexte séance

### Cohérence d'affichage

Chaque paiement doit toujours afficher en face :

- Date séance (`jj/mm/aaaa`).
- Nature séance (ex. « Fin de traitement »).

**Implémentation**:

- Colonne « Pour » : affiche `S000043 · ven. 10/10/2026 · Nature` (+ « report de S… »), depuis le 17/10/2026.
- Propriété `PaiementListeItem.PourAffichage`.
- Double-clic paiement : rappel séance dans le label éditeur.

**Résultat** : Aucune confusion entre paiement et séance ; chaque ligne est auto-documentée.

---

## Séance d'un encaissement

Un paiement appartient toujours à une séance : on encaisse **depuis l'onglet Séances** (« Encaisser séance »), sur la séance que l'utilisatrice a **sélectionnée** ; la fenêtre Paiement reçoit la séance de l'appelant (`PaiementEditor.New(idSeance…)`, obligatoire) et rappelle `S000043 — 17/09/2026 (Fin de traitement)`.

Un double-clic sur une ligne de l'onglet Paiements ouvre l'encaissement de sa séance.

- Pas de bouton « Nouveau paiement » dans l'onglet Paiements, ni de choix de la séance dans la fenêtre (décision de Joëlle, 22/10/2026) : ce chemin encaissait la séance sélectionnée dans l'autre onglet et ne proposait pas l'avoir.
- **Une action sur une ligne exige une ligne choisie** : la ligne courante qu'une grille pose d'office au chargement ne compte pas (`UtilsDataGrid.ElementSelectionne`) ; sans sélection, les boutons sont grisés avec « Sélectionnez … dans la liste. »

---

## Cohérence séances ↔ paiements (v26/09, ADR-26)

> Numérotation : ADR-26 = cohérence séances ↔ paiements ; ADR-27 = domaine = convention ; ADR-28 = lignes système.

- **Règle unique** : tout code qui modifie une séance ou un paiement appelle `GestionPaiements.ResynchroniserDuSeance` **dans la transaction de l'écriture** (variante `(conn, tx)`, ADR-46). Ne jamais recoder localement l’ajustement du dû. Contrôle des écarts et réalignement : Administration › État du système (« Cohérence des données »).
- **Calculs** : réglé, reste dû et avoir ne comptent que les lignes `PAYE`. L’avoir est calculé, jamais stocké.
- **Aucune suppression de paiement** : annulation SuperUser/Admin avec motif obligatoire (`DialogMotifAnnulation`) ; trace date, utilisateur (identifiant) et motif dans les colonnes de l'annulation (v2.51.0, ADR-49 ; les annulations d'avant ont leur mention dans le commentaire).
- **Montant de séance** : modifiable uniquement via le cadenas (SuperUser/Admin) ; jamais de réalignement automatique en modification. Incohérence montant/tarif = blocage avec message adapté au rôle, sans fermer la fenêtre.
- **Décision utilisateur** : une situation légitime mais à risque (quota dépassé, tarif changé sur séance encaissée, report de crédit) donne un avertissement bien visible avec choix ; ne pas bloquer.
- **Grille Paiements** (relevé chronologique, 17/10/2026) : l'état d'un paiement se calcule dans `PaiementListeItem` (`FamilleEtat`, `EtatAffichage`) avec les familles et couleurs de la grille des séances (`FamillePaiementSeance`, `UtilsPastilles`)
  - « séance passée » vient de la séance chargée (`SeancePassee`, renseignée par l'écran).
  - Un nouveau filtre s'ajoute en fin de liste (les index sont des constantes).
- **Montant dû** : une seule règle — séance due et passée (`SeanceListeItem.MontantExigible`) — pour les grilles, le bandeau (`GestionSeances.CalculerSoldesPatient`) et la grille Patients (`montant_du`, même règle en SQL) ; l'avoir est une donnée **du patient**.
- **Dû en attente clos** (`PaiementListeItem.EstAttenteClose` : `ANNULE` sans date ni mention) : clos par l'application, ce n'est pas une annulation — jamais affiché dans le relevé, visible (« Clos ») dans le détail des paiements d'une séance.
- **Montants de paiement** : tout affichage ou total part de `lia_paiements_seances.montant_affecte`, jamais de `paiements.montant` (avoir utilisé = liaison diminuée ; paiement réparti par une réaffectation d'avant le 21/10/2026).
- **Avoir** : une seule définition (encaissé resté sur une séance non due, séances reportées comprises ; trop-perçu `réglé − montant` d'une séance due) partout — `SeanceListeItem.AvoirSeance`, `QueryPatients` (`montant_avoir`), `QueryPaiements.SelectAvoirsDisponiblesPatient` ; les faire évoluer ensemble.
- Référence : [Guide séances et paiements](../Guide/Guide_Seances_Paiements.md) (partie 2).

---

## Cabinet, praticien et habillage (06/10/2026, ADR-29)

- **Designer d’abord** : tout l’habillage visuel (couleurs, polices, fonds, styles des contrôles, icônes de titre) se règle dans le Designer. Le code ne réapplique pas de thème ; exceptions : style `DataGridView` (`UtilsDataGrid`) et états fonctionnels dynamiques.
- **Situation juridique** : le comportement (porteur du BCE, facturation future) dépend des attributs de `ref_situations_juridiques`, jamais du code de la situation.
- **BCE à un seul endroit** : cabinet ou praticien selon `porteur_bce`. Toute correction de placement est proposée à l’utilisateur ; aucune donnée n’est déplacée ni supprimée silencieusement.
- **Identifiants et coordonnées bancaires** : contrôles centralisés dans `UtilsIdentifiantBelge` (INAMI, BCE, TVA, IBAN, BIC) ; ne pas recoder localement.
- **Images** (logo, photo, cachet, signature, logos de domaine) : fichiers dans le **dossier de données** (ADR-35, compris dans les sauvegardes ; jamais dans `Assets`, remplacé à chaque déploiement), nom de fichier seul en base ; jamais de BLOB.
- **Écran avec saisie** : implémenter `IUnsavedChangesAware` pour que la navigation demande confirmation avant de perdre des modifications.
- **Modifications non enregistrées** (19/10/2026, ADR-43) : tout écran ou toute fenêtre de saisie prévient avant de perdre une modification, pour **tous** ses champs, avec `DialogChoix.DemanderAvantDepart` (Enregistrer / Quitter sans enregistrer / Rester ; Échap = Rester).
  - Fenêtre : une ligne dans le `Load`, `GardeModifications.SurveillerFenetre(Me, btnEnregistrer, "de la séance")`.
  - UserControl : `GardeModifications` (photo au passage en saisie, `EstModifie` dans `PeutQuitter`).
  - Le contrôle de saisie (éditeur de texte compris) ne pose jamais la question lui-même.
  - **Annuler** qui reste sur l'écran : `DialogChoix.ConfirmerAbandon` (Abandonner / Continuer la saisie) s'il y a des modifications
  - Annuler qui ferme une fenêtre : la question à trois choix.
  - Comportement identique dans toute l'application (Reference_UI § 5.8).

---

## Maintenance future

### Si nouveau statut paiement

1. Ajouter ligne `ref_statuts_paiement`.
2. Mettre à jour `PaiementListeItem.FamilleEtat` / `EtatAffichage` (pastille « État ») et `MouvementCourt`.
3. Éventuellement ajouter couleur dans `UtilsIcons`.

### Si nouvelle coloration nécessaire

1. Centraliser couleurs dans `UITheme` (futur).
2. Respecter l'ordre de priorité : statut paiement > solde séance.



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

