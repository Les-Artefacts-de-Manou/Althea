# Plan de tests — Éditeur de texte riche (`UC_RichTextEditor`)

> Version : 2.6 — Dernière mise à jour : 20/10/2026 — correcteur dans les zones de texte simples, `AltheaZoneTexte`, section 15
> Périmètre : `UC_RichTextEditor` V2.0.0 (profils Complet et Compact), `RichTextEditorHelper` V2.0.0, `AltheaRenduBarre` V1.0.0, et les écrans qui l'emploient.
> Remplace le plan V1.0 du 17/05/2026 (éditeur à listes de police et de taille), dont les résultats restent dans l'historique Git.
> Références : maquette « Éditeurs de texte — refonte » (canevas, validée le 19/10/2026), guide `Docs/Guide/UC_RichTextEditor_Documentation.md`, ADR-42.

<details>
<summary>Historique des mises à jour</summary>

- 20/10/2026 — temps 2 : correcteur d'orthographe, section 14
- 19/10/2026 — textes types : plusieurs usages, écran des usages, RTE-119 à RTE-124
- 19/10/2026 — temps 2 : textes types, section 13
- 19/10/2026 — temps 2 : en-tête et pied de page, section 12
- 19/10/2026 — temps 2 : « Ouvrir en grand », section 11
- 19/10/2026 — refonte de l'éditeur, temps 1

</details>

## Prérequis

- Version compilée en **Debug** : accueil › outils de test › **éditeur de texte** ouvre `TestRichTextEditor` (cases « Mode lecture seule », « Afficher la toolbar », « Profil compact »).
- Dans l'application : anamnèse (fiche patient) et zones du dossier = profil **Complet** ; alerte (fiche patient), notes et évolution (séance), commentaires (contact, intervenant, thérapeute) = profil **Compact**.
- Données fictives uniquement pour les essais d'export et d'impression.

## 1. Allure et profils

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RTE-01 | Ouvrir la fenêtre de test (profil Complet). | Barre blanche, trait beige fin dessous ; groupes : Annuler / Rétablir · Style « Normal » · G I S barré, couleur, surligneur, effacer · puces, numéros, retraits, alignement · date · rechercher ; à droite Aperçu, Imprimer, « Exporter ». | Icônes sauge du même trait ; boutons sans cadre, survol vert-de-gris pâle. | ☐ |
| RTE-02 | Survoler chaque bouton. | Infobulle avec le raccourci (« Gras (Ctrl+G) »…). | — | ☐ |
| RTE-03 | Annuler et Rétablir sans historique. | Grisés à 40 % (pas de gris Windows), actifs dès la première frappe. | — | ☐ |
| RTE-04 | Cocher « Profil compact ». | Barre réduite : Annuler / Rétablir · G I S, couleur, surligneur, effacer · puces · date ; pas de recherche ni de ligne d'état. | — | ☐ |
| RTE-05 | Profil Complet : bas de l'éditeur. | Ligne d'état « 0 mot · Zoom 100 % ». | — | ☐ |
| RTE-06 | Texte collé aux bords ? | Marge intérieure de chaque côté ; fond beige clair ; plus de cadre 3D. | — | ☐ |

## 2. Mise en forme des caractères

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RTE-10 | Sélectionner un mot, cliquer **G**. | Gras ; le bouton reste coché (sauge clair) tant que le curseur est dans le mot. | Recliquer retire le gras. | ☐ |
| RTE-11 | Ctrl+G puis Ctrl+B. | Les deux basculent le gras. | — | ☐ |
| RTE-12 | Ctrl+I sur une sélection. | Italique, **sans tabulation insérée** (défaut corrigé). | — | ☐ |
| RTE-13 | Ctrl+U ; bouton barré. | Souligné ; barré. Boutons cochés selon la sélection. | — | ☐ |
| RTE-14 | Couleur du texte ▾. | Six pastilles : Texte (par défaut), Sauge, Mauve, Ocre, Terre, Bleu ; la couleur s'applique à la sélection. | Plus de sélecteur Windows ni de rouge vif. | ☐ |
| RTE-15 | Surligneur ▾ : Jaune, Sauge, Mauve, puis « Aucun surlignage ». | Fonds pâles ; « Aucun » rend le fond de l'éditeur. | — | ☐ |
| RTE-16 | Effacer la mise en forme sur un passage gras, coloré, en liste, centré. | Texte normal, sans liste, aligné à gauche. | Sans sélection : le texte tapé ensuite est normal. | ☐ |

## 3. Paragraphes

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RTE-20 | Style ▾ (curseur dans un paragraphe). | Menu : Titre, Sous-titre, Normal, Citation, Petit, chacun dans son rendu ; le style courant est coché. | — | ☐ |
| RTE-21 | Appliquer « Titre », puis « Sous-titre ». | Tout le paragraphe (pas seulement la sélection) : grand gras sauge, puis moyen gras sauge plus clair ; la barre affiche le nom du style. | La police reste Calibri. | ☐ |
| RTE-22 | Appliquer « Citation », puis « Normal ». | Italique gris avec retrait ; « Normal » retire le retrait. | — | ☐ |
| RTE-23 | Sélection de trois paragraphes, style « Petit ». | Les trois passent en petit. | — | ☐ |
| RTE-24 | Puces, puis Numéros, puis Numéros encore. | Liste à puces ; liste 1. 2. 3. ; troisième clic : plus de liste. Boutons cochés selon le paragraphe. | Lignes longues alignées sur le texte, pas sur la puce. | ☐ |
| RTE-25 | Retrait + / − ; − à zéro. | Retrait par pas ; ne passe pas sous zéro. | — | ☐ |
| RTE-26 | Alignement ▾ : gauche, centre, droite, **justifier**. | Alignements appliqués ; le justifié s'étale sur toute la largeur ; l'icône du bouton montre l'alignement courant. | Ctrl+L / E / R / J. | ☐ |

## 4. Insertion

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RTE-30 | Date ▾. | Trois choix à l'heure qu'il est : « 19/10/2026 », « 19/10/2026 14:32 », « — 19/10/2026 14:32 · P. N. (horodatage signé) ». | — | ☐ |
| RTE-31 | Horodatage signé au milieu d'une ligne. | Inséré sur sa propre ligne, en italique gris, initiales de l'utilisateur connecté ; la saisie reprend en texte normal à la ligne suivante. | Séance (notes), anamnèse, zone du dossier. | ☐ |
| RTE-32 | Fenêtre de test (pas de contexte). | « Horodatage signé » n'est pas proposé (auteur inconnu) ; les deux dates le sont. | — | ☐ |

## 5. Recherche et remplacement (profil Complet)

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RTE-40 | Ctrl+F (ou loupe). | Bande de recherche sous la barre ; le mot sélectionné est repris ; le champ a le focus. | — | ☐ |
| RTE-41 | Taper un mot présent cinq fois. | Sélection au fil de la frappe ; « 1 sur 5 ». | Casse ignorée par défaut. | ☐ |
| RTE-42 | Entrée, F3, chevron ▼ ; Maj+Entrée, Maj+F3, chevron ▲. | Occurrence suivante / précédente, en repartant du début ou de la fin ; compteur juste. | — | ☐ |
| RTE-43 | « Respecter la casse ». | Le compte change selon les majuscules. | — | ☐ |
| RTE-44 | Mot absent. | « Aucun résultat » ; chevrons grisés. | — | ☐ |
| RTE-45 | Ctrl+H ou « Remplacer… » ; Remplacer. | Ligne « Remplacer par » ; l'occurrence sélectionnée est remplacée (mise en forme gardée) puis la suivante est sélectionnée. | Entrée dans « Remplacer par » = Remplacer. | ☐ |
| RTE-46 | « Tout remplacer » « a » par « aa ». | Remplacement unique de chaque occurrence, pas de boucle ; « n remplacements ». | Annuler défait les remplacements un à un. | ☐ |
| RTE-47 | Échap dans la recherche (fenêtre Zone du dossier). | Ferme la bande, **pas la fenêtre** ; le focus revient au texte. | — | ☐ |

## 6. Presse-papiers et menu du clic droit

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RTE-50 | Clic droit sans sélection. | Couper et Copier grisés ; Coller selon le presse-papiers ; Tout sélectionner. | Même allure que les menus de la barre. | ☐ |
| RTE-51 | Copier un passage d'une page web, « Coller sans mise en forme » (ou Ctrl+Maj+V). | Texte seul, à la mise en forme du curseur. | Ctrl+V garde la mise en forme. | ☐ |

## 7. Consultation (lecture seule)

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RTE-60 | Profil Complet, « Mode lecture seule ». | Barre de consultation : pastille grise « Lecture seule » · Rechercher, Copier ; à droite Aperçu, Imprimer, Exporter. | Plus aucun bouton de mise en forme. | ☐ |
| RTE-61 | En consultation : Ctrl+G, Ctrl+I, Ctrl+E, Ctrl+Maj+V, taper du texte. | Rien ne change. | Ligne d'état sans « Modifié ». | ☐ |
| RTE-62 | En consultation : sélectionner et copier ; Ctrl+F ; imprimer. | Possibles ; pas de « Remplacer… ». | — | ☐ |
| RTE-63 | Profil Compact en lecture seule. | Barre masquée (comme avant). | Alerte de la fiche patient en consultation. | ☐ |
| RTE-64 | « Afficher la toolbar » décochée, puis lecture seule cochée / décochée. | La barre reste masquée (elle réapparaissait). | — | ☐ |

## 8. Sortie : aperçu, impression, export

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RTE-70 | Aperçu sur un texte de deux pages avec titres, listes, couleurs. | Fenêtre d'aperçu agrandie, mise en forme gardée, deux pages. | Imprimer depuis l'aperçu fonctionne. | ☐ |
| RTE-71 | Exporter ▾ › Mise en page… : paysage, marges 30 mm ; puis Aperçu. | L'aperçu suit la mise en page choisie (elle était ignorée). | Rouvrir la mise en page : marges inchangées (pas de rétrécissement). | ☐ |
| RTE-72 | Imprimer (Ctrl+P) ; annuler le dialogue. | Rien n'est imprimé, aucun message. | — | ☐ |
| RTE-73 | Aperçu, impression ou export d'un document vide. | Message « Le document est vide : il n'y a rien à … ». | — | ☐ |
| RTE-74 | Anamnèse : Exporter › PDF. | Fichier écrit dans le dossier du patient et ouvert ; **pas** de message « exporté avec succès » en plus ; trace documentaire comme avant. | Journal : aucun chemin de fichier. | ☐ |
| RTE-75 | Fenêtre de test : Exporter › Document Word. | Choix de l'emplacement (nom proposé « Notes_Althéa.docx ») ; message de réussite avec le nom du fichier. | — | ☐ |
| RTE-76 | Exporter vers un PDF déjà ouvert dans un lecteur. | Message « L'export n'a pas abouti… fermez-le » ; journal : « Échec de l'export PDF … IOException » sans chemin. | — | ☐ |

## 9. Modifications, chargement, ligne d'état

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RTE-80 | Anamnèse en modification : passer un mot en gras **sans taper**, puis Annuler la modification. | La fiche voit la modification (confirmation d'abandon demandée). | Le gras seul n'était pas vu. | ☐ |
| RTE-81 | Ligne d'état en tapant. | Nombre de mots à jour (« 1 mot », « 12 mots ») ; « · Modifié » dès la première frappe. | — | ☐ |
| RTE-82 | Anamnèse : Enregistrer. | « Modifié » disparaît. | — | ☐ |
| RTE-83 | Ctrl + molette ; clic sur « Zoom … % ». | Zoom affiché ; le clic revient à 100 %. | — | ☐ |
| RTE-84 | Ouvrir un contenu existant (anamnèse, note de séance). | Mise en forme intacte ; Annuler grisé (le chargement n'est pas dans l'historique) ; pas de « Modifié ». | — | ☐ |
| RTE-85 | (Si un RTF abîmé existe en base de test.) | Texte brut affiché ; journal « Contenu RTF illisible (rteXxx) », sans contenu. | — | ☐ |

## 10. Écrans

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RTE-90 | Séance : notes et évolution (Compact), en saisie puis en consultation. | Barre compacte en saisie ; masquée en consultation ; enregistrement inchangé. | — | ☐ |
| RTE-91 | Contact, intervenant, thérapeute : commentaire. | Barre compacte, horodatage signé disponible ; enregistrement inchangé. | — | ☐ |
| RTE-92 | Fiche patient : alerte (Compact) et anamnèse (Complet). | Alerte compacte ; anamnèse avec barre de consultation hors modification. | — | ☐ |
| RTE-93 | Zone du dossier, zone d'archive (lecture seule). | Barre de consultation ; impression titrée du nom de la zone. | — | ☐ |

## 11. Ouvrir en grand (profil Compact, temps 2)

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RTE-94 | Profil Compact en édition : barre. | Bouton « Ouvrir en grand » à droite ; absent en consultation et au profil Complet. | Infobulle « Ouvrir en grand : écrire dans une grande fenêtre ». | ☐ |
| RTE-95 | Ouvrir en grand depuis les notes d'une séance. | Grande fenêtre titrée « Notes de séance », icône de la fenêtre Séance, texte et mise en forme repris ; barre Complet sans aperçu, impression ni export. | Ctrl+P sans effet ; taille ≤ 90 % de l'écran, centrée ; fenêtre agrandissable. | ☐ |
| RTE-96 | Modifier (style Titre, liste numérotée, horodatage), Valider. | Texte et mise en forme reportés dans le champ ; la séance doit encore être enregistrée. | Fermer la séance sans enregistrer : question à trois choix (KM). | ☐ |
| RTE-97 | Ouvrir en grand, ne rien changer, Valider. | Rien n'est reporté ; le champ n'est pas « modifié ». | Fermer la séance : aucune question. | ☐ |
| RTE-98 | Modifier puis Annuler, croix ou Échap. | Enregistrer (= Valider) / Quitter sans enregistrer / Rester ; Quitter laisse le champ intact. | Échap avec la recherche ouverte ferme d'abord la recherche. | ☐ |
| RTE-99 | Alerte (fiche patient), commentaires (contact, intervenant, thérapeute), évolution de séance. | Titres « Alerte / Notes », « Commentaire du contact », « … de l'intervenant », « … du thérapeute », « Évolution de la séance ». | Horodatage signé disponible (utilisateur connecté). | ☐ |

## 12. En-tête et pied de page (temps 2)

Prérequis : sauvegarde de la base, migration `migration_v2.41.0_mention_documents.sql` appliquée (État du système : base 2.41.0 = version attendue) ; cabinet et praticien renseignés.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RTE-100 | Migration v2.41.0 appliquée puis rejouée. | Aucune erreur, aucun doublon. | Paramètres : carte « Documents imprimés » (après Cabinet), « Mention en pied de page » = Confidentiel, modifiable par le SuperUser. | ☐ |
| RTE-101 | Anamnèse d'un patient : Aperçu. | Chaque page : en-tête cabinet (sauge) / « Anamnèse », praticien / « Prénom NOM (P…) », trait ; pied trait, « Confidentiel », « Édité le … », « Page n / N ». | Texte entre l'en-tête et le pied, rien de coupé ni de superposé ; N juste sur 1, 2 et 3 pages. | ☐ |
| RTE-102 | Imprimer (imprimante réelle ou PDF de Windows). | Même rendu que l'aperçu. | Mise en page changée (marges, paysage) : en-tête et pied suivent les marges. | ☐ |
| RTE-103 | Exporter en PDF puis en Word. | Même en-tête et même pied ; numéros de page justes. | Word : en-tête et pied modifiables dans Word ; champs de page à jour ; langue du texte = français (barre d'état de Word), orthographe vérifiée en français. | ☐ |
| RTE-104 | Zone d'un dossier (et zone d'archive) : aperçu, export. | Titre « nom de la zone — dossier D… », patient du dossier. | Export contextuel inchangé (dossier du patient, nom de fichier). | ☐ |
| RTE-105 | Paramètres : vider la mention, puis la changer (« Confidentiel — secret professionnel »). | Vide : pas de mention ; nouvelle valeur au document suivant. | — | ☐ |
| RTE-106 | Écran Cabinet : changer le nom du cabinet ou le titre du praticien, puis aperçu. | En-tête à jour sans redémarrer. | Texte long : points de suspension, rien ne déborde. | ☐ |
| RTE-107 | Fenêtre de test (Debug), sans patient. | En-tête sans ligne patient. | Journal : aucune donnée de patient. | ☐ |

## 13. Textes types (temps 2)

Prérequis : sauvegarde de la base, migration `migration_v2.42.0_textes_types.sql` appliquée (État du système : base 2.42.0 = version attendue).

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RTE-110 | Migration v2.42.0 appliquée puis rejouée. | Aucune erreur, aucun doublon. | `ref_usages_textes_types` : 5 usages système ; `ref_textes_types` vide. | ☐ |
| RTE-111 | Référentiels : tuile « Textes types » (groupe Dossier), SuperUser puis utilisateur simple. | Écran ouvert (icône, titre) ; tuile inactive sans droits. | Grille : Code, Libellé, Usage (« Partout » sans usage), Ordre, État. | ☐ |
| RTE-112 | Nouveau texte : code, libellé, usage « Notes de séance », texte mis en forme (gras, liste) via « Ouvrir en grand ». | Enregistré ; repris tel quel en consultation (éditeur sans barre). | Texte vide refusé (« Le texte est vide ») ; code et libellé uniques. | ☐ |
| RTE-113 | Séance : notes, puis évolution. | Bouton Textes types dans la barre compacte ; notes : les textes « Notes de séance » et « Partout » ; évolution : « Évolution de séance » et « Partout ». | Infobulle : début du texte ; insertion au curseur, mise en forme gardée ; Annuler (Ctrl+Z) la retire. | ☐ |
| RTE-114 | Anamnèse, zone du dossier, commentaire d'un contact / intervenant / thérapeute. | Chacun ne propose que son usage et « Partout ». | Alerte de la fiche patient : pas de bouton. | ☐ |
| RTE-115 | Aucun texte pour un usage. | Ligne grisée « Aucun texte type ici (Référentiels › Textes types) ». | — | ☐ |
| RTE-116 | Désactiver un texte, puis le réactiver. | Il disparaît du menu, puis revient (sans redémarrer). | Modification d'un texte : la nouvelle version est insérée à l'ouverture suivante du menu. | ☐ |
| RTE-117 | Modifier un texte type puis quitter l'écran ou Annuler. | Questions habituelles (trois choix ; Abandonner / Continuer la saisie). | Consultation : pas de barre ; Modifier : barre compacte. | ☐ |
| RTE-118 | Après une insertion, quitter la séance sans enregistrer. | Question à trois choix (le champ est modifié). | Journal : aucun contenu de texte. | ☐ |

Plusieurs usages et écran des usages (migration `migration_v2.43.0_textes_types_usages.sql` appliquée : base 2.43.0) :

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RTE-119 | Migration v2.43.0 sur une base où des textes existent (dont un « Commentaires »). | Chaque texte garde son usage ; le texte « Commentaires » est coché pour contact, intervenant et thérapeute ; plus d'usage `COMMENTAIRE`. | `ref_textes_types` n'a plus `id_usage_texte_type` ; rejouer : aucune erreur, aucun doublon. | ☐ |
| RTE-120 | Textes types : cocher deux usages (Anamnèse + Zones du dossier), enregistrer. | Colonne Usages « Anamnèse, Zones du dossier » ; texte proposé dans les deux éditeurs, pas ailleurs. | Rien de coché : « Partout » ; consultation : cases figées, fond teinté. | ☐ |
| RTE-121 | Cocher / décocher un usage puis quitter ou Annuler. | Questions habituelles (le changement de cases compte comme une modification). | — | ☐ |
| RTE-122 | Alerte de la fiche patient (en modification). | Bouton Textes types : textes « Alerte du patient » et « Partout ». | — | ☐ |
| RTE-123 | Référentiels › Usages des textes types. | 8 usages (cadenas Système) ; pas de bouton Nouveau, les autres boutons resserrés à gauche. | Modifier : libellé et ordre seulement (code et état verrouillés) ; le nouveau libellé apparaît dans la liste à cocher et la colonne Usages. | ☐ |
| RTE-124 | Contact, intervenant, thérapeute : commentaire. | Chacun ne propose que son usage et « Partout ». | — | ☐ |

## 14. Correcteur d'orthographe (temps 2, ADR-45)

Prérequis : migration `migration_v2.44.0_correcteur_orthographe.sql` appliquée (base 2.44.0) ; clavier français.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RTE-130 | Notes d'une séance (saisie) : taper « Le pacient est venu hier ». | « pacient » souligné en rouge ; rien d'autre. | Grammaire non vérifiée (« venus ») : normal. | ☐ |
| RTE-131 | Clic droit sur « pacient ». | En tête : suggestions en gras (dont « patient »), « Ajouter au dictionnaire », séparateur, puis le menu habituel. | Clic sur « patient » : mot remplacé, mise en forme gardée, Ctrl+Z l'annule ; la séance compte une modification. | ☐ |
| RTE-132 | Clic droit sur un mot correct, ou hors d'un mot. | Menu habituel seul. | Menu ouvert au clavier (Maj+F10) : mot au curseur. | ☐ |
| RTE-133 | « Ajouter au dictionnaire » sur un mot de métier (« graphothérapie »). | Plus souligné, ici et dans les autres éditeurs, y compris après redémarrage. | — | ☐ |
| RTE-134 | ~~« Ignorer »~~ | Retiré le 20/10/2026 : mot retapé de nouveau souligné, sans correction proposée (relevé par Joëlle). | — | ✅ |
| RTE-135 | Anamnèse existante (texte d'avant le correcteur), passer en modification. | Texte vérifié en français (pas tout souligné) ; quitter sans rien changer : aucune question. | Consultation : aucun soulignement. | ☐ |
| RTE-136 | Paramètres › Éditeur de texte : décocher « Correcteur d'orthographe », rouvrir une séance. | Plus de soulignement ni de suggestions. | Recocher : de retour à l'écran suivant. | ☐ |
| RTE-137 | Fenêtre de test : case « Correcteur », ligne de diagnostic. | Clavier et langues (fr-BE, fr-FR) affichés ; décocher retire les soulignements. | — | ☐ |

## 15. Correcteur dans les zones de texte simples (`AltheaZoneTexte`, ADR-45 point 5)

Commentaire d'un paiement (`PaiementEditor`) et motif d'une annulation (`DialogMotifAnnulation`) : texte simple, avec le correcteur. Données fictives.

| # | Test | Résultat attendu | Vérifications | État |
|---|---|---|---|---|
| RTE-140 | Encaisser séance (fenêtre Paiement), Commentaire : taper « Le pacient a payé ». | « pacient » souligné en rouge. | — | ☐ |
| RTE-141 | Clic droit sur « pacient ». | En tête : suggestions en gras, « Ajouter au dictionnaire », séparateur ; puis Couper, Copier, Coller (icônes de l'éditeur), Tout sélectionner, au rendu du kit. | Clic sur « patient » : mot remplacé, Ctrl+Z l'annule. | ☐ |
| RTE-142 | Copier un texte en gras (éditeur, Word), le coller dans le commentaire (Ctrl+V, Maj+Inser, menu Coller). | Collé sans mise en forme. | — | ☐ |
| RTE-143 | Dans le commentaire : Ctrl+E, Ctrl+R, Ctrl+I, Ctrl+1, Ctrl+5 ; puis Alt Gr+E, Ctrl+A, Ctrl+C, Ctrl+X, Ctrl+Z. | Les premiers ne font rien (ni centrage, ni tabulation, ni interligne) ; « € », tout sélectionner, copier, couper, annuler fonctionnent. | — | ☐ |
| RTE-144 | Taper un commentaire puis fermer la fenêtre (croix, Échap). | Question des modifications non enregistrées ; Enregistrer : commentaire enregistré tel que tapé. | — | ☐ |
| RTE-145 | Annuler un encaissement : motif « absnet » souligné ; Entrée. | Entrée valide (comme avant) ; Maj+Entrée passe à la ligne ; motif vide : message habituel, champ rose. | — | ☐ |
| RTE-146 | Après l'annulation : commentaire du paiement, puis Administration › Journaux. | Commentaire : « [Annulé le … : motif] » ; journal : « Paiement encaissé annulé (ID=…) par … » **sans** le motif. | — | ☐ |
| RTE-147 | Paiement d'un dossier archivé (consultation). | Commentaire lisible, sans soulignement ; clic droit : Copier et Tout sélectionner seulement. | — | ☐ |
| RTE-148 | Paramètres : décocher « Correcteur d'orthographe », rouvrir un paiement ; puis repasser RTE-130 à RTE-133 dans une séance. | Plus de soulignement dans le commentaire ; recoché, l'éditeur se comporte comme avant (clic droit partagé). | Non-régression de l'éditeur. | ☐ |

## Validation

- [ ] Fenêtre de test (sections 1 à 9).
- [ ] Écrans (section 10).
- [ ] Ouvrir en grand (section 11).
- [ ] En-tête et pied de page (section 12).
- [ ] Textes types (section 13).
- [ ] Correcteur d'orthographe (section 14).
- [ ] Zones de texte simples (section 15).
