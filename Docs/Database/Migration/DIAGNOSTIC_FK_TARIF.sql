-- Diagnostic problème FK tarif patient par défaut

-- 1. Vérifier les tarifs disponibles dans prix_seance
SELECT 
    id_prix_seance, 
    code_prix_seance,
    libelle_prix_seance, 
    montant, 
    actif, 
    ordre_affichage
FROM prix_seance
ORDER BY ordre_affichage;

-- 2. Vérifier l'état du patient Julie Fontaine (ID=7)
SELECT 
    id_patient,
    nom,
    prenom,
    id_prix_seance_defaut
FROM patients
WHERE id_patient = 7;

-- 3. Vérifier s'il y a des patients avec id_prix_seance_defaut != NULL
SELECT 
    id_patient,
    nom,
    prenom,
    id_prix_seance_defaut
FROM patients
WHERE id_prix_seance_defaut IS NOT NULL;

-- 4. Vérifier quelle valeur d'id_prix_seance_defaut référence un prix_seance non existant
SELECT 
    p.id_patient,
    p.nom,
    p.id_prix_seance_defaut,
    ps.id_prix_seance
FROM patients p
LEFT JOIN prix_seance ps ON ps.id_prix_seance = p.id_prix_seance_defaut
WHERE p.id_prix_seance_defaut IS NOT NULL
AND ps.id_prix_seance IS NULL;

-- 5. Récupérer la dernière valeur tentée pour id_prix_seance_defaut du patient 7
-- (Cela montre quelle valeur a été rejetée)
SELECT 
    id_patient,
    nom,
    prenom,
    id_prix_seance_defaut
FROM patients
WHERE id_patient = 7;
