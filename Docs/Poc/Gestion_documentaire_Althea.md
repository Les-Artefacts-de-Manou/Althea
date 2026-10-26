#   **📘** **Manifeste - Gestion documentaire Althéa (Version 15/04/2026 consolidée)**

- ## 📦 Packages et composants techniques

  ### 🧩 Syncfusion (DOCX / PDF / Viewer)

  #### Packages utilisés

  - Syncfusion.DocIO.WinForms
  - Syncfusion.DocToPdfConverter.WinForms
  - Syncfusion.Pdf.WinForms
  - Syncfusion.PdfToImageConverter.WinForms
  - Syncfusion.Licensing

  #### Rôle

  - création et manipulation DOCX
  - conversion DOCX → PDF (local uniquement)
  - affichage PDF intégré (viewer)

  #### ⚠️ Règles

  - Syncfusion **n’est pas un éditeur utilisateur**
  - Word reste l’outil d’édition principal
  - conversion PDF uniquement pour formats Word compatibles

  ------

  ## ☁️ Google APIs

  #### Packages

  - Google.Apis.Drive.v3
  - Google.Apis.Docs.v1
  - Google.Apis.Auth

  #### Rôle

  - stockage cloud (backup + accès)
  - édition via Google Docs
  - synchronisation bidirectionnelle

  ------

  ## 🔐 Authentification

  - `google_client_secret.json`
  - `token.json`

  #### Règles

  - jamais versionner
  - OAuth Desktop
  - séparation application / utilisateur

  ------

  ## 🧠 Règles techniques critiques

  ### Upload

  - CREATE si `google_file_id` absent
  - UPDATE sinon

  👉 interdit de recréer systématiquement

  ------

  ### Download

  - DOCX uploadé → `Files.Get`
  - Google Doc natif → `Export`

  👉 sinon document vide

  ------

  ### MIME TYPE

  👉 toujours dynamique

  - jamais codé en dur
  - dépend du fichier réel
  - critique pour Flow 3 et Flow 4

  ------

  ## 🔁 Flows documentaires

  ------

  ## 🟢 Flow 1 - Word (principal)

  ### Process

  1. Création DOCX
  2. Ouverture Word
  3. Modification
  4. Sauvegarde
  5. Génération PDF local
  6. Upload DOCX

  ### Sens

  👉 Local → Drive

  ------

  ## 🔵 Flow 2 - Google Docs

  ### Process

  1. Création DOCX
  2. Upload
  3. Ouverture Google Docs
  4. Modification
  5. Download DOCX
  6. Génération PDF local

  ### Sens

  👉 Drive → Local

  ------

  ## 🟡 Flow 3 - Document externe

  ### 🎯 Définition

  👉 Admission d’un document externe dans un contexte métier

  ------

  ### 🧩 Contexte

  #### A. Document patient (principal)

  - courrier
  - notes externes
  - documents reçus

  #### B. Document général

  - administratif
  - ressource

  ------

  ### 🔄 Process

  1. sélection fichier
  2. copie locale
  3. renommage unique
  4. reset contexte
  5. détection type
  6. conversion PDF si Word
  7. upload Drive (MIME dynamique)

  ------

  ### 🧠 Règles

  - accepter tout fichier
  - ne jamais refuser un format
  - ne convertir que Word
  - respecter le format source

  ------

  ### 🧩 Types

  | Type             | Traitement     |
  | ---------------- | -------------- |
  | DOCX / DOC / RTF | conversion PDF |
  | PDF              | stockage       |
  | image            | stockage       |
  | autres           | stockage       |

  ------

  ### ⚠️ Critiques

  - reset contexte obligatoire
  - sinon UPDATE incorrect
  - éviter doublons
  - éviter double conversion

  ------

  ## 🟣 Flow 4 - Images / Photos (métier)

  ### 🎯 Définition

  👉 Image = **objet métier**

  ------

  ### Différence fondamentale

  | Flow 3   | Flow 4        |
  | -------- | ------------- |
  | fichier  | donnée métier |
  | lecture  | observation   |
  | stockage | exploitation  |

  ------

  ### 🔄 Process validé

  1. sélection (multi possible)
  2. rattachement patient/dossier
  3. copie locale
  4. renommage métier
  5. génération miniature
  6. upload Drive
  7. affichage local

  ------

  ### 🧠 Règles métier

  - ne jamais modifier l’original
  - ne jamais remplacer par PDF
  - PDF = dérivé optionnel
  - image = donnée d’analyse

  ------

  ### 📛 Nommage

  ```
  Photo_{idPatient}_{idDossier}_{yyyyMMdd_HHmmss}_{index}.ext
  ```

  ------

  ### 🖼️ Miniatures

  - suffixe `_thumb`
  - stockage local uniquement
  - usage UI (grille, sélection, performance)
  - jamais source métier

  ------

  ### 📦 Multi-images

  - sélection multiple
  - traitement en boucle
  - upload unitaire
  - nommage sécurisé par index

  ------

  ## 📄 PDF - règle définitive

  👉 PDF = **dérivé local uniquement**

  - jamais source
  - jamais obligatoire
  - jamais critique
  - toujours reconstruisable

  ------

  ## 📂 Stockage

  ### Local (référence métier)

  ```
  Patients/
      {id_patient}/
          {id_dossier}/
              Documents/
              Photos/
  ```

  ------

  ### ☁️ Drive (miroir logique)

  ```
  Althea/
      Patients/
          {id_patient}/
              {id_dossier}/
                  Documents/
                  Photos/
  ```

  ------

  ### Règles Drive

  - basé sur ID (pas chemin)
  - création dynamique dossiers
  - `GetOrCreateDriveFolder` obligatoire
  - aucun stockage en vrac

  ------

  ## 🧠 Gestion du contexte

  ### POC

  - variables globales fragiles

  ### Althéa (cible)

  👉 toujours basé sur :

  - document sélectionné
  - ID document
  - données DB

  ------

  ## ⚠️ Pièges critiques (définitifs)

  - MIME type en dur
  - conversion PDF non conditionnelle
  - variables non reset
  - confusion CREATE / UPDATE
  - nom fichier comme identifiant
  - double conversion PDF
  - dépendance au “dernier fichier”
  - stockage Drive non structuré

  ------

  ## 🚀 Validation du POC

  ✔ Flow 1 complet
   ✔ Flow 2 complet
   ✔ Flow 3 multi-format
   ✔ Flow 4 images + miniatures + multi
   ✔ arborescence Drive dynamique
   ✔ synchronisation stable
   ✔ PDF maîtrisé

  ------

  ## 🏁 Conclusion

  👉 Althéa ne gère pas des fichiers
   👉 Althéa gère des **objets documentaires métier synchronisés**

  - DOCX = source éditable
  - PDF = dérivé
  - Image = donnée métier
  - Google = extension de travail
  - Local = référence
  - DB = intelligence

------




> ---
>
> **Contact** : ***Joëlle (Manou)  - Les Artefacts de Manou***
>
> Projet réalisé pour ma fille, Psychologue et Graphologue, pour l'aider à gérer ses patients et documents de manière structurée, fiable et évolutive.
>
> - Site web P.Nguyen Duy:  https://pearlnguyenduy.be/
> - mailto: `joelle@nguyen.eu`
>
> - GitHub privé: Althea    https://github.com/AngeljoNG/Althea
> - GitHub public : Althea None
>
> ---

