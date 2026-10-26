# Althéa — fiche de reprise (état au 22/10/2026, soir)

> À lire en début de conversation. Les règles générales sont dans les instructions du projet ; ici, seulement l'état du chantier et ce qui est propre à ce dépôt.

## Où on en est

- Plan en cours : audit du 18/10/2026, `Docs/Conception/RAPPORT_AUDIT_2026-10-18.md` (suivi point par point au § 9, découpage du lot C au § 6).
- Lots A, B et C.1 terminés.
- Ordre validé par Joëlle : **C.2** règles métier hors des écrans → recherche transversale (ToDo) → **C.3** codes système centralisés (+ drapeaux de référentiel, dont « séance non tenue », ToDo) → **C.4** découplage des écrans de `Home` → **C.5** découpage des gros fichiers ; **C.6** au démarrage du module Documents.
- **C.2 : inventaire et découpage validés le 22/10/2026** (ADR-51, audit § 6). Décisions : valideur métier rendant la **liste des erreurs par champ** (`ErreurSaisie` avec `NameOf` de la propriété, `ErreurValidationException.LeverSiErreurs`, dossier `Metier/Validation`) ; l'écran appelle le valideur, l'écriture l'applique ; **chevauchement refusé par le métier**. Étapes : **1** séances et paiements → **2** patient, contacts, intervenants, thérapeutes (`ValiderPatient`…, `SuiviIntervenant.EstTerminee`) → **3** domaines (`ValiderDomaine`, `CalculerManques` sortie de `UC_SyntheseDomaine`) → **4** comptes (`PolitiqueMotDePasse.Valider`, login ≥ 3 partout).
- **C.2 étape 1 terminée** (`3f49830e`).
- **Finitions des onglets Séances / Paiements terminées** (`7afc40b9`, tests S-83 à S-91 et Administration 7.1 à 7.4 validés) : ligne choisie avant d'agir (`UtilsDataGrid.ElementSelectionne`), « Nouveau paiement » retiré, colonne « Solde » retirée, bandeau « ici / tous dossiers » + « Prévu », pastilles État alignées, État du système › Cohérence des données + bouton Admin « Réaligner les montants en attente » (4 dus réalignés le 22/10).
- **Prochaine étape : C.2 étape 2** — patient, contacts, intervenants, thérapeutes : valideurs métier (`ValiderPatient` : nom, prénom, téléphone/CP selon pays, e-mail, NISS format + unicité, CT1/CT2 ; contacts ; intervenants avec fin ≥ début ; thérapeutes avec unicité), appelés par les écrans (erreurs placées sur les champs via `ErreurSaisie.Champ` = `NameOf(Patient.X)`) et par `CreatePatient` / `UpdatePatient` / `CreateSuivi`… ; `SuiviIntervenant.EstTerminee` (grisé de la grille). Le doublon (nom + prénom + naissance) reste une confirmation de l'écran.
- **Observation à surveiller** (22/10, ~18:40) : l'onglet Séances a clignoté en continu une fois, juste après « Réaligner » puis ouverture d'un dossier du patient PA000005 ; plus reproduit. Piste si ça revient : va-et-vient actif/grisé des boutons à chaque affichage de page (la grille sélectionne d'office la 1re ligne puis `UC_Pagination.AfficherPage` efface la sélection) qui relancerait un recalcul. Questions : quoi clignote, quand, s'arrête au clic d'une ligne, le compteur « x–y sur n » change-t-il, onglet Paiements aussi ? Test rapide : `SeanceSelectionnee` revenue temporairement à `CurrentRow`.
- ToDo : généraliser « ligne choisie avant d'agir » aux autres grilles (à confirmer écran par écran) ; statut de séance ajouté → C.3.
- Point d'observation (couvert par la note ToDo C.3) : passer une séance planifiée à « Annulée » reste soumis au contrôle de chevauchement si un conflit préexiste.
- Dernier commit : `7afc40b9`. Joëlle pousse elle-même.

## Façon de travailler (spécifique)

- Dépôt : `D:\source\repos\Althea` (sur l'appareil, shell : `$HOME/mnt/Althea`). VB.NET WinForms .NET 8, MariaDB. Au début d'une conversation, il peut falloir redemander l'accès au dossier (device_request_folder_access) et la permission de suppression (verrous Git).
- Joëlle compile et teste (F5) ; de mon côté, contrôle de syntaxe seulement — ne jamais dire « testé ». Contrôle de syntaxe : dans le conteneur, `apt-get install dotnet8` puis petit outil Roslyn (`VisualBasicSyntaxTree.ParseText`, références aux DLL de `/usr/lib/dotnet/sdk/*/Roslyn/bincore`, `nuget.config` avec `<clear />`) sur les seuls fichiers modifiés, stagés.
- Donner à chaque étape la liste détaillée des tests, dans le plan de tests concerné (`Docs/Tests/PLAN_TESTS_*.md`), puis cocher après son retour.
- Ne pas piloter l'application ni lire de données patients (la base de développement peut en contenir) ; requêtes de contrôle données à Joëlle : identifiants, codes et montants seulement (pour retrouver un patient : requête séance → code dossier → code patient, puis recherche par code dans l'écran Patients).
- Décision métier, destructive ou durable → `AskUserQuestion`, option recommandée en premier.
- Chaque étape : code + en-têtes (Version, Date, ligne d'historique dans le fichier et la procédure) + documentation (CHANGELOG, ToDo, audit § 9, ADR, guide, processus, concepts, plan de tests, Database_technique, Securite_Utilisateurs, Structure_Solution, Rules selon le cas) ; commit après le feu vert de Joëlle ; puis résumé OneNote (une seule fois, après le commit).
- Encodage : `.vb` en UTF-8 (BOM conservé tel qu'il est), CRLF ; `.md` et `.sql` en CRLF, sauf `CHANGELOG.md` et `ARCHITECTURE_DECISIONS.md` en LF (état du dépôt) ; `.resx` en LF ; nouveaux `.vb` avec BOM.
- Scripts d'édition dans `$HOME/outils` sur la VM (`remplacer.py FICHIER SPEC` — blocs `<<<<`/`====`/`>>>>`, occurrence unique, BOM et fins de ligne conservés ; `nouveau.py`) — à recréer s'ils manquent.

## Git

- Commit : `git -c user.name="AngeljoNG" -c user.email="joelle@nguyen.eu" commit -q -F <message>`, fichiers ajoutés par chemins explicites ; terminer le message par les lignes d'attribution demandées par la session.
- Ne jamais commiter : `Docs/Database/backup_NoData_althea.sql`, `UI/Forms/Test/TestChampDate.resx`, `Docs/Illustrations/Doc_UI/`, `Docs/Architecture/Documentation_technique_UI_Althea.md`.
- Après chaque commit : `rm -f .git/*.lock .git/objects/maintenance.lock; find .git/objects -maxdepth 2 -name "tmp_obj_*" -delete`.
- Si « Operation not permitted » : la permission de suppression de la VM a été perdue — la redemander sur `D:\source\repos\Althea`, puis nettoyer les verrous.

## Pièges connus

- Ne jamais copier tout le dépôt dans le conteneur (refusé : exfiltration). Ne copier que les fichiers nécessaires au contrôle de syntaxe.
- Ne jamais afficher la clé Syncfusion.
- `/tmp` de la VM peut être effacé.
- Les écritures conditionnelles (« 0 ligne = refus ») reposent sur `UseAffectedRows = False` (`DatabaseManager`).
- Un bloc de remplacement « vide » laisse une ligne blanche en trop : vérifier les triples sauts de ligne après suppression d'une fonction.
- Grilles : `CurrentRow` vaut la première ligne dès le chargement, même sans sélection visible — lire la sélection par `UtilsDataGrid.ElementSelectionne`.
- Bandeau : « dû » = séances dues **passées** ; avant l'heure de fin, c'est du « prévu » (ne pas le prendre pour un bug).

## Côté Joëlle (en attente)

- Régénérer `backup_NoData_althea.sql` ; purger l'historique du miroir public.