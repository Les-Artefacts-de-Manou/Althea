# 🔑 Guide d'obtention de la licence Syncfusion Community (gratuite)

## 📋 Pré-requis

La licence **Syncfusion Community** est **gratuite** si vous remplissez l'une de ces conditions :
- ✅ Revenus annuels de votre organisation < 1 million USD
- ✅ Utilisation personnelle / projet open source
- ✅ Projet étudiant / académique

---

## 🚀 Étapes pour obtenir votre clé de licence

### 1. Créer un compte Syncfusion

Rendez-vous sur : [https://www.syncfusion.com/account/register](https://www.syncfusion.com/account/register)

- Remplissez le formulaire d'inscription
- Validez votre email

### 2. Demander la licence Community

Une fois connecté, allez sur : [https://www.syncfusion.com/account/claim-license-key](https://www.syncfusion.com/account/claim-license-key)

- Sélectionnez **"Community License"**
- Acceptez les conditions (revenus < 1M USD)
- Cliquez sur **"Claim Free License"**

### 3. Récupérer votre clé

- La clé de licence s'affiche immédiatement sur la page
- Elle est également envoyée par email
- **Format** : Une longue chaîne alphananumérique (environ 88 caractères)

**Exemple de clé** (fictif) :
```
Mgo+DSMBaFt...très longue chaîne...xyz123==
```

### 4. Enregistrer la clé dans Althéa

La clé se saisit **dans l'application**, jamais dans le code (depuis le 18/10/2026) :

1. Se connecter en **Admin**, ouvrir **Administration > Paramètres**.
2. Carte **« Licences — propre à ce poste »** : cliquer **Modifier**.
3. Coller la clé dans **« Clé Syncfusion »** (le champ reste masqué ; espaces et retours à la ligne d'une copie sont ignorés).
4. **Enregistrer**. La clé est remise à Syncfusion aussitôt, sans redémarrer.

Où elle est rangée : `%APPDATA%\Althea\althea.syncfusion.json`, **chiffrée par Windows (DPAPI) pour le compte Windows courant** (`LicenceSyncfusion`, même construction que le mot de passe des sauvegardes). Elle n'est jamais en base, jamais dans le dépôt, jamais dans les journaux. Conséquence : **à saisir sur chaque poste et pour chaque compte Windows** (poste de développement, poste du cabinet).

Au démarrage, `AppStartupManager.Run` appelle `GestionLicences.AppliquerLicenceSyncfusion()`, avant toute utilisation de Syncfusion.

### 5. Vérifier l'enregistrement

- La pastille de la carte « Licences » indique **« Définie le jj/mm/aaaa »**.
- **Administration > État du système**, section Application : « Licence Syncfusion — Définie sur ce poste ».
- Un export PDF depuis l'éditeur de texte ne porte plus l'avertissement de licence.

---

## 🔄 Renouvellement

- La licence Community est **valable 1 an**
- Vous recevrez un email de rappel avant expiration
- Renouvellement gratuit si vous êtes toujours éligible
- Il suffit de redemander une clé et de la saisir dans Paramètres (elle remplace l'ancienne)

---

## 📝 Bonnes pratiques

- **Ne jamais** écrire la clé dans le code ni la committer (risque de révocation) ; ne pas la partager avec d'autres organisations.
- **Historique** : jusqu'au 17/10/2026, deux clés étaient écrites en clair dans `Program.vb` ; elles restent lisibles dans l'historique Git. Les considérer comme publiques : en demander une nouvelle et la saisir dans Paramètres.
- Sans clé, Althéa fonctionne normalement : seuls les exports PDF et Word portent l'avertissement de licence ; le démarrage le note dans le journal.

---

## 🆘 Problèmes courants

### Avertissement de licence dans les exports

**Causes possibles :**
- Aucune clé saisie sur ce poste ou pour ce compte Windows (pastille « Absente »)
- Clé saisie par un autre compte Windows (pastille « Illisible ») : DPAPI ne la déchiffre que pour le compte qui l'a enregistrée
- Clé expirée (> 1 an) ou révoquée (partagée publiquement)

**Solution :**
- Saisir à nouveau la clé dans Paramètres, avec le compte Windows qui utilise Althéa
- Si l'avertissement persiste, demander une nouvelle clé sur le site Syncfusion (éligibilité Community)

### « Ce texte ne ressemble pas à une clé de licence »

La saisie est incomplète ou contient autre chose que la clé : la recopier en entier depuis le compte Syncfusion.

---

## 📚 Ressources

- **Documentation officielle** : [https://help.syncfusion.com/common/essential-studio/licensing/overview](https://help.syncfusion.com/common/essential-studio/licensing/overview)
- **FAQ Community License** : [https://www.syncfusion.com/sales/communitylicense](https://www.syncfusion.com/sales/communitylicense)
- **Support Syncfusion** : [https://www.syncfusion.com/support](https://www.syncfusion.com/support)

---

## ✅ Checklist finale

- [ ] Compte Syncfusion créé
- [ ] Licence Community réclamée
- [ ] Clé copiée (longue chaîne de ~88 caractères)
- [ ] Clé saisie dans Administration > Paramètres, carte « Licences » (pastille « Définie »)
- [ ] État du système : « Licence Syncfusion — Définie sur ce poste »
- [ ] Export PDF fonctionne (éditeur de texte : Exporter › PDF)

🎉 **C'est prêt !** Althéa peut maintenant exporter des documents RTF en PDF de qualité professionnelle.

---

**Dernière mise à jour :** 18/10/2026 (clé saisie dans Paramètres, chiffrée sur le poste)  
**Version Syncfusion utilisée :** 35.1.40  
**Projet :** Althéa
