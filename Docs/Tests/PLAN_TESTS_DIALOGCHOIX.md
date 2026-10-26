# Plan de tests — DialogChoix (boîte de dialogue Althéa)

> Version : 2.1 — Dernière mise à jour : 16/10/2026 (DialogChoix V2.1.0, maquette C : petite fenêtre du kit ; V2.0 du 14/10/2026 ; remplace le plan V1 du 07/06/2026).
> Périmètre : `UI/Forms/Communs/DialogChoix` (V2.1.0), `UI/Controls/Theme/AltheaIconeEtat` (V1.1.0), fenêtre de démonstration `UI/Forms/Test/TestDialogChoix`, premier écran adopté : règles de quota de `UC_DomainesHome`.
> Références : ADR-11 (DialogChoix à la place des MessageBox), Rules § 16, Charte (« Icônes d'état »), maquettes « DialogChoix — icônes » (style 2), « DialogChoix — B » (V2.0) et « DialogChoix — C » (V2.1).

## Objectif

Vérifier la présentation (petite fenêtre du kit : barre de titre, en-tête, carte, barre d'actions ; icônes dessinées, corps structuré, boutons du kit), la taille adaptée au texte, le clavier, la compatibilité des appels existants et l'adoption dans les règles de quota.

## Prérequis

- Version compilée en **Debug** (le bouton de test DialogChoix de l'accueil n'existe qu'en Debug).
- Accueil › carte « Outils de test » › bouton DialogChoix : ouvre `TestDialogChoix`. Le libellé « Dernier résultat » affiche la valeur renvoyée par chaque dialogue.

## 1. Fenêtre de démonstration (TestDialogChoix)

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| DC-01 | **Information**. | Fenêtre du kit (barre de titre beige, fond à la feuille) ; icône « i » bleue, titre mauve ; message dans une carte ; un bouton OK dans la barre d'actions. | Résultat `OK`. | ✅ |
| DC-02 | **Avertissement**. | Icône « ! » ocre, titre mauve. | Résultat `OK`. | ✅ |
| DC-03 | **Erreur**. | Icône « × » rose ; renvoi au journal, aucun détail technique. | Résultat `OK`. | ✅ |
| DC-04 | **Succès**. | Icône « ✓ » sauge — distincte de l'Information. | Résultat `OK`. | ✅ |
| DC-05 | **Confirmer (Oui / Non)**. | Icône « ? » mauve ; « Oui » à gauche, « Non » à droite (ordre habituel). | `Yes` / `No` ; Échap → `No`. | ✅ |
| DC-06 | **Question (Oui / Non / Annuler)**. | Trois boutons : Oui, Non, Annuler de gauche à droite. | `Yes` / `No` / `Cancel` ; Échap → `Cancel`. | ✅ |
| DC-07 | **Confirmer une action**. | Boutons « Clôturer » / « Annuler ». | Entrée → `Yes` ; Échap → `No`. | ✅ |
| DC-08 | **Suppression**. | « Supprimer » en terre, à gauche d'« Annuler ». | **Entrée → `No`** (Annuler par défaut) ; clic Supprimer → `Yes`. | ✅ |
| DC-09 | **Suppression avec attention**. | Encadré ocre pâle avec petite icône « ! » sous le message. | Texte complet visible. | ✅ |
| DC-10 | **Récapitulatif complet**. | Dans l'ordre : message, encadré « Plafonds / Alerte / Validité » (libellés alignés), complément, encadré d'attention, remarque grise, question finale en gras ; « Enregistrer » en ocre. | Rien de tronqué ; `Yes` / `No`. | ✅ |
| DC-11 | **Quota dépassé**. | Avertissement avec « Enregistrer quand même » / « Annuler ». | Largeur des boutons selon le texte. | ✅ |
| DC-12 | **Titre et texte longs**. | Fenêtre élargie (640 px au plus), titre complet ou terminé par « … ». | Pas de débordement des boutons. | ✅ |
| DC-13 | **Message très long**. | Fenêtre à 70 % de la hauteur de l'écran au plus ; le message défile dans sa carte. | Barre de défilement sans recouvrir le texte, boutons toujours visibles. | ✅ |
| DC-14 | **Attente**. | Icône sauge, l'arc tourne. | Pas de ralentissement ; s'arrête à la fermeture. | ✅ |
| DC-15 | **Croix de la barre de titre**. | La fenêtre se ferme comme avec Échap. | 1 bouton → `OK` ; 2 boutons → `No` ; 3 boutons → `Cancel`. | ✅ |
| DC-16 | Ouvrir un dialogue depuis la fenêtre de démonstration. | Centré sur la fenêtre appelante, sans déborder de l'écran. | — | ✅ |
| DC-17 | Carte « Icônes d'état ». | Six icônes nettes en 48 et 24 px, couleurs de la charte. | Prêtes pour une capture. | ✅ |
| DC-18 | **Titre long** (maquette C). | La fenêtre s'élargit pour que le titre ne passe pas sous la feuille (640 px au plus, puis « … »). | — | ✅ |

## 2. Compatibilité dans l'application

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| DC-20 | Un message existant (ex. Rattachement sans élément choisi, étape Tarifs des Domaines). | Nouvelle présentation, même texte, même comportement. | Aucun appel modifié. | ☐ |
| DC-21 | Une confirmation existante (ex. quitter une fiche modifiée). | Oui / Non, résultat inchangé pour l'écran appelant. | — | ☐ |
| DC-22 | `SeanceEditor` : séance au-delà du quota. | « Enregistrer quand même » / « Annuler » ; l'écran réagit comme avant. | Appel `ShowDialog(Me)`. | ☐ |
| DC-23 | Message d'erreur technique (ex. base indisponible). | Message clair, aucun détail technique ni donnée personnelle. | Détail dans le journal (ADR-39). | ☐ |

## 3. Premier écran adopté : règles de quota (Domaines, étape 3)

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| DC-30 | Nouvelle règle → **Ajouter la règle**. | Récapitulatif structuré (Plafonds, Alerte, Validité), « La règle actuelle s'arrêtera le … », remarque, « Enregistrer cette règle ? » ; boutons « Revenir à la saisie » / « Enregistrer » (ocre). | « Revenir à la saisie » laisse la saisie ouverte. | ☐ |
| DC-31 | Correction d'une règle inutilisée créant un écart. | Titre « Correction de la règle » ; encadré d'attention « Après cette correction, aucune règle ne couvrira … ». | — | ☐ |
| DC-32 | Clôture d'une règle. | « Clôturer » / « Annuler » ; encadré d'attention si un écart apparaît. | — | ☐ |
| DC-33 | Suppression d'une règle inutilisée. | « Supprimer la règle ? », « Supprimer » en terre, Entrée = Annuler ; réouverture de la règle précédente annoncée ; écart éventuel dans l'encadré. | — | ☐ |

## Validation

- [x] Fenêtre de démonstration (DC-01 à DC-17) : validée par Joëlle le 14/10/2026 ; V2.1 (DC-01 à DC-18) validée le 16/10/2026.
- [ ] Compatibilité dans l'application (DC-20 à DC-23).
- [ ] Règles de quota (DC-30 à DC-33).
- [ ] Captures d'écran de la documentation refaites (à la fin de la refonte UI).
