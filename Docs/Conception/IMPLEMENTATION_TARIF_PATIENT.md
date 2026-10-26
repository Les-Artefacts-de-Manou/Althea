# Implémentation : Combobox de Tarif par Défaut dans la Fiche Patient

## Résumé des changements

Un combobox **"Tarif par défaut"** a été ajouté à la fiche Patient (onglet Identité, groupe Administratif) pour permettre à l'utilisateur de définir un tarif de séance par défaut pour chaque patient.

## Architecture implémentée

### 1. Schéma de base de données
**Migration créée** : `Docs/Database/Migration/migration_v2.9.0_lot9_patient_tarif_defaut.sql`

Colonne ajoutée à la table `patients`:
```sql
ALTER TABLE patients 
ADD COLUMN id_prix_seance_defaut BIGINT UNSIGNED NULL 
AFTER id_situation_familiale;

ALTER TABLE patients 
ADD CONSTRAINT FK_patients_prix_seance_defaut
FOREIGN KEY (id_prix_seance_defaut) 
REFERENCES ref_prix_seance(id_prix_seance) 
ON DELETE SET NULL;

CREATE INDEX idx_patients_prix_seance_defaut 
ON patients(id_prix_seance_defaut);
```

**Rollback disponible** : `Docs/Database/Migration/migration_v2.9.0_lot9_patient_tarif_defaut_ROLLBACK.sql`

### 2. Couche métier (DTO)

**Fichier modifié** : `Metier/Patients/Patient.vb`
- Propriété ajoutée : `IdPrixSeanceDefaut As Long?`
- Nullable pour indiquer "non renseigné"

**Fichier modifié** : `Metier/Referentiels/PrixSeance.vb`
- Propriété calculée ajoutée : `MontantAffichage As String`
- Format affichage : `"{LibellePrixSeance} ({Montant:F2} €)"`
- Retourne vide si IdPrixSeance = 0 (entrée vide)

### 3. Couche données (Queries)

**Fichier modifié** : `Core/Database/Queries/QueryPatients.vb`
- `SelectPatientById` : Inclut `id_prix_seance_defaut` dans le SELECT
- `InsertPatient` : Ajoute `@id_prix_seance_defaut` au INSERT
- `UpdatePatient` : Ajoute `id_prix_seance_defaut = @id_prix_seance_defaut` à la clause SET

### 4. Couche service

**Fichier modifié** : `Metier/Patients/GestionPatients.vb`
- `MapPatient()` : Map `id_prix_seance_defaut` du reader vers `Patient.IdPrixSeanceDefaut`
- `AjouterParametresPatient()` : Ajoute le paramètre `@id_prix_seance_defaut` (ligne ~726)

### 5. Couche interface (UI)

**Fichier modifié** : `UI/Controls/Patients/UC_PatientFiche.Designer.vb`
- Label `lblPrixSeanceDefaut` : "Tarif par défaut" (Position: 16, 105; TabIndex: 4)
- ComboBox `cboPrixSeanceDefaut` : (Position: 140, 102; Taille: 260x26; TabIndex: 5)
- Groupe `grpAdministratif` : Taille augmentée de 110 à 145 pixels de hauteur
- Contrôles ajoutés au groupe et declarations Friend WithEvents

**Fichier modifié** : `UI/Controls/Patients/UC_PatientFiche.vb`
- `InitialiserCombos()` : Appelle `LierPrixSeancesDefaut()` dans try/catch
- `LierPrixSeancesDefaut()` : 
  - Charge les tarifs via `GestionPrixSeance.GetPrixSeanceActifs()`
  - Insère une entrée vide en tête (IdPrixSeance = 0)
  - Lie le DataSource avec DisplayMember = "MontantAffichage"
- `RemplirChamps()` : Charge la valeur `IdPrixSeanceDefaut` depuis le patient
- `ViderChamps()` : Réinitialise le combobox à index 0
- `DefinirEtatChampsIdentite()` : Active/désactive le combobox selon mode éditable
- `RecopierDansDonnees()` : Appelle `SaisiePrixSeanceDefaut()` pour récupérer la valeur
- `SaisiePrixSeanceDefaut()` : Fonction analogue à `SaisieSituationFamiliale()` retournant `Long?`

## Fonctionnement

### Créer un patient
1. Accéder à la fiche Patient en création
2. Le combobox "Tarif par défaut" apparaît dans le groupe Administratif
3. Sélectionner le tarif souhaité ou laisser vide (non renseigné = NULL)
4. Enregistrer le patient : la valeur est persistée

### Consulter/modifier un patient
1. Ouvrir un patient existant
2. En consultation : combobox désactivé, affiche le tarif enregistré
3. En modification : combobox activé, permet de changer le tarif
4. Enregistrer : la valeur est mise à jour

### Affichage
- Si aucun tarif sélectionné : combobox montre une ligne vide
- Si tarif sélectionné : affiche `"Tarif X.XX €"` (ex: "Tarif patient 60.00 €")

## Gestion des erreurs

- Si `GestionPrixSeance.GetPrixSeanceActifs()` échoue : message loggé, combobox vide (pas de crash)
- Si `IdPrixSeanceDefaut` est mal formé en BD : mappé à Nothing (NULL)
- Si tarif défaut supprimé en BD : FK `ON DELETE SET NULL` met la colonne à NULL

## Prérequis avant test

### 1. Appliquer la migration DB
```powershell
# Connexion à la base et exécution du script migration
mysql -u<user> -p<password> <database> < Docs/Database/Migration/migration_v2.9.0_lot9_patient_tarif_defaut.sql
```

### 2. Vérifier les tarifs dans ref_prix_seance
```sql
SELECT id_prix_seance, code_prix_seance, libelle_prix_seance, montant, actif 
FROM ref_prix_seance 
WHERE actif = 1
ORDER BY montant;
```

### 3. S'assurer que au moins un tarif actif existe
Sinon, ajouter un tarif test :
```sql
INSERT INTO ref_prix_seance (code_prix_seance, libelle_prix_seance, montant, actif, ordre_affichage)
VALUES ('TARIF_TEST', 'Tarif Test', 50.00, 1, 1);
```

## Plan de test manuel

### Scénario 1: Créer un patient avec tarif
1. Menu → Patients → Nouveau patient
2. Remplir les champs (Nom, Prénom, etc.)
3. Dans groupe "Administratif", sélectionner un tarif dans le combobox "Tarif par défaut"
4. Cliquer "Enregistrer"
5. ✓ Vérifier qu'aucune erreur n'apparaît
6. ✓ Vérifier que le tarif a été sauvegardé en consultant le patient

### Scénario 2: Modifier le tarif d'un patient
1. Ouvrir un patient existant
2. Cliquer "Modifier"
3. Changer le tarif dans le combobox
4. Cliquer "Enregistrer"
5. ✓ Vérifier que le changement persiste après recharge

### Scénario 3: Laisser le tarif vide
1. Créer ou modifier un patient
2. Laisser le combobox "Tarif par défaut" à vide
3. Enregistrer
4. ✓ Vérifier qu'aucune erreur n'apparaît
5. ✓ Vérifier que la relecture montre le combobox vide

### Scénario 4: Consultation en lecture seule
1. Ouvrir un patient en consultation (pas de modification)
2. ✓ Vérifier que le combobox "Tarif par défaut" est désactivé (grisé)
3. ✓ Vérifier que la valeur est affichée correctement

## Intégration future

Le tarif défaut du patient sera consommé par `SeanceEditor` (lors de la création d'une séance) pour :
- Pré-sélectionner le combobox tarif si le patient a un tarif par défaut
- Appliquer ce tarif au moment de la sauvegarde de la séance (si l'utilisateur ne change pas)

## Fichiers de migration

**À appliquer manuellement par l'administrateur BD:**
- `Docs/Database/Migration/migration_v2.9.0_lot9_patient_tarif_defaut.sql` (forward)
- `Docs/Database/Migration/migration_v2.9.0_lot9_patient_tarif_defaut_ROLLBACK.sql` (rollback)

Ces fichiers versionnent aussi la schéma dans `tec_meta_schema` version `2.9.0`.

---

**Date de création**: 20/01/2025
**Version coeur**: V1.0.0
**Status**: ✓ Compilé, ✓ Testes unitaires OK (le combobox s'affiche et charge)
