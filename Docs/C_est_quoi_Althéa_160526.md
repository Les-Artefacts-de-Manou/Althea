## 🌿 Qu'est-ce qu'Althéa ?

> **Date de validité : 16/05/2026**   
> **Source de vérité technique :** [`Readme.md`](../Readme.md)

---

**Althéa** est une application de bureau Windows dédiée à la **gestion des dossiers patients** pour une pratique psychologique/thérapeutique. Le nom vient du grec *altho* (« guérir »). L'application vise à centraliser les informations de suivi des patients, les séances, les paiements et les documents, tout en garantissant confidentialité et sécurité.

Elle s'adresse à un praticien isolé souhaitant un outil **fiable, structuré et respectueux de la confidentialité**, avec ou sans dépendance à un service cloud.

---

## 🧩 Principes architecturaux clés

1. **Une seule Form principale (`Home`)** - Les écrans sont des `UserControl` injectés dynamiquement via le `NavigationManager`. Aucune fenêtre flottante pour les écrans métier.
2. **Point d'accès DB unique** - Tout accès à MariaDB passe **exclusivement** par `DatabaseManager`. Pas de connexion directe ailleurs dans le code.
3. **Séparation stricte des responsabilités** :
   - `Core` = infrastructure (pas de logique métier)
   - `Metier` = règles métier (pas de SQL direct)
   - `UI` = affichage uniquement (pas d'accès DB direct)
4. **Gestion des rôles** - 3 niveaux : `User`, `SuperUser`, `Admin`. La session supporte l'**élévation temporaire** de rôle (activation/désactivation contrôlée).
5. **Sécurité** - Les mots de passe DB sont chiffrés via **DPAPI** (Windows Data Protection API), liés au compte Windows de l'utilisateur.
6. **Contexte injecté** - Un objet `UserControlContext` est transmis aux `UserControl` via `IContextAwareUserControl`, évitant tout couplage fort entre écrans.

---

## 📦 Modèle métier prévu

```
Patient ──(1:n)──▶ Dossier ──(1:n)──▶ Séance
                              ├──(1:n)──▶ Paiement
                              └──(1:n)──▶ Document
```

Le modèle patients/dossiers est **en cours de développement**. Le code actuel couvre principalement l'infrastructure, la sécurité et la navigation.

---

> 📎 Pour les détails techniques (technologies, arborescence des dossiers, flux de démarrage), voir [`Readme.md`](../Readme.md).