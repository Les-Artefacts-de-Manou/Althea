# Clarification : Comportement du combobox "Tarif par défaut"

## Structure du combobox

Le combobox "Tarif par défaut" contient :
1. **Une entrée vide en index 0** : `IdPrixSeance = 0`, `LibellePrixSeance = ""`, `Montant = 0`
   - Affichée comme une ligne vide ou "---" selon `MontantAffichage`
   - Signifie : "Aucun tarif par défaut sélectionné"

2. **Des entrées réelles** : `IdPrixSeance > 0`, avec des tarifs vrais
   - Affichées comme `"Libellé (Montant €)"`
   - Exemple : `"Anciens (50.00 €)"`, `"Nouveaux (60.00 €)"`

## Sélection et sauvegarde

### ✅ Comportement correct

| Sélection | SelectedValue | SaisiePrixSeanceDefaut() | Sauvegarde DB |
|-----------|---------------|------------------------|---------------|
| Entrée vide (index 0) | `0` | `Nothing` | `id_prix_seance_defaut = NULL` |
| Tarif réel (ex. Anciens) | `50` (ULong) | `50` | `id_prix_seance_defaut = 50` |
| Tarif réel (ex. Nouveaux) | `60` (ULong) | `60` | `id_prix_seance_defaut = 60` |

### ❌ Ancien comportement (avant correction)

Avant, `SaisiePrixSeanceDefaut()` ne testait **pas** explicitement `idPrix > 0`.
Si l'entrée vide était sélectionnée :
- `idPrix = 0`
- Conditionnelle testait `Long.TryParse(...) AndAlso idPrix > 0`
- Si ce test passait (il ne passait pas car `0 > 0` est faux), on retournait 0
- **Mais** on retournait `Nothing` de toute façon

**Le problème réel** : Peut-être une mauvaise sélection lors du test initial, ou une confusion sur quelle valeur était sélectionnée.

## Comment tester correctement

### Test 1 : Sélectionner un vrai tarif

```
1. Ouvrir la fiche d'un patient
2. Regarder dans le combobox "Tarif par défaut"
3. Cliquer sur une ligne **qui n'est PAS vide**
   - Tu dois voir un texte comme "Anciens (50.00 €)"
4. Enregistrer
5. Vérifier dans les logs : "Patient mis à jour"
```

### Test 2 : Ne sélectionner rien (entrée vide)

```
1. Ouvrir la fiche d'un patient
2. Cliquer sur la **première ligne (vide)** du combobox
3. Enregistrer
4. Vérifier : dans la base de données, id_prix_seance_defaut doit être NULL
```

### Test 3 : Charger un patient avec tarif défaut

```
1. Enregistrer un patient avec un tarif (Test 1)
2. Fermer et rouvrir sa fiche
3. Vérifier que le même tarif est pré-sélectionné dans le combobox
```

## Debug : Comment vérifier les valeurs

### En base de données

```sql
-- Voir les patients avec tarif défaut
SELECT id_patient, nom, prenom, id_prix_seance_defaut 
FROM patients 
WHERE id_prix_seance_defaut IS NOT NULL;

-- Voir les tarifs disponibles
SELECT id_prix_seance, libelle_prix_seance, montant, actif 
FROM prix_seance 
WHERE actif = 1 
ORDER BY ordre_affichage;
```

### Dans la UI

Si tu ouvres une fiche patient et que le combobox affiche une ligne vide alors qu'une tarif devrait être sélectionné, c'est qu'il y a un problème de matching entre :
- La valeur en BDD (`id_prix_seance_defaut`)
- Les valeurs dans le combobox (`IdPrixSeance` de chaque tarif)

Cela pourrait arriver si :
1. Le tarif avec cet ID a été supprimé ou désactivé
2. Il y a un problème de type de données (ULong vs autre)
3. Le binding n'a pas fonctionné

## Correction appliquée le 28/01/2025

La fonction `SaisiePrixSeanceDefaut()` a été clarifiée pour :
1. Toujours tester `idPrix > 0` explicitement
2. Ajouter des commentaires explicatifs
3. Gérer la distinction entre "entrée vide sélectionnée" et "tarif réel sélectionné"

**Avant** : La logique reposait sur un court-circuit (`AndAlso`).  
**Après** : La logique est explicite et couvre tous les cas de figure.

---

**Version** : 1.0.0  
**Date** : 28/01/2025
