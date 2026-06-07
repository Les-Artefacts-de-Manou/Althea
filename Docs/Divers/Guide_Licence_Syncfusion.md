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

Ouvrez le fichier `Program.vb` et localisez cette ligne :

```vb
SyncfusionLicenseProvider.RegisterLicense("VOTRE-CLE-SYNCFUSION")
```

Remplacez `"VOTRE-CLE-SYNCFUSION"` par votre vraie clé :

```vb
SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt...votre-clé...xyz123==")
```

### 5. Vérifier l'enregistrement

Compilez et lancez Althéa :
- ✅ Si aucune popup Syncfusion n'apparaît → Licence valide
- ❌ Si popup "Invalid License" → Vérifiez que la clé est complète et entre guillemets

---

## 🔄 Renouvellement

- La licence Community est **valable 1 an**
- Vous recevrez un email de rappel avant expiration
- Renouvellement gratuit si vous êtes toujours éligible
- Il suffit de re-demander une clé et remplacer dans `Program.vb`

---

## 📝 Bonnes pratiques

### ❌ À NE PAS FAIRE
- **Ne jamais** committer la clé dans Git public (risque de révocation)
- Ne pas partager votre clé avec d'autres organisations

### ✅ Recommandations pour Althéa

**Option 1 : Variable d'environnement (recommandé pour production)**
```vb
' Dans Program.vb
Dim syncfusionKey As String = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY")
If String.IsNullOrEmpty(syncfusionKey) Then
	' Clé de dev en dur pour développement local
	syncfusionKey = "VOTRE-CLE-DE-DEV"
End If
SyncfusionLicenseProvider.RegisterLicense(syncfusionKey)
```

**Option 2 : Fichier de configuration non versionné**
```vb
' Dans Program.vb
Dim syncfusionKey As String = File.ReadAllText("syncfusion.key").Trim()
SyncfusionLicenseProvider.RegisterLicense(syncfusionKey)
```

Puis ajoutez `syncfusion.key` au `.gitignore` :
```
# .gitignore
syncfusion.key
```

**Option 3 : App.config / appsettings.json**
```xml
<!-- App.config -->
<appSettings>
  <add key="SyncfusionLicenseKey" value="VOTRE-CLE"/>
</appSettings>
```

```vb
' Dans Program.vb
Dim syncfusionKey As String = ConfigurationManager.AppSettings("SyncfusionLicenseKey")
SyncfusionLicenseProvider.RegisterLicense(syncfusionKey)
```

---

## 🆘 Problèmes courants

### "Invalid License" au démarrage

**Causes possibles :**
- Clé incomplète (copié-collé partiel)
- Clé expirée (> 1 an)
- Clé révoquée (partagée publiquement)

**Solution :**
- Vérifiez que la clé est complète dans `Program.vb`
- Demandez une nouvelle clé sur le site Syncfusion
- Vérifiez que vous êtes toujours éligible Community

### "License file not found"

**Cause :** Appel à `RegisterLicense()` trop tard dans le code

**Solution :**
- Assurez-vous que `RegisterLicense()` est appelé **avant** toute utilisation de Syncfusion
- Dans Althéa, c'est dans `Main()` juste après `SetCompatibleTextRenderingDefault()`

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
- [ ] Clé enregistrée dans `Program.vb`
- [ ] Application compilée sans erreur
- [ ] Aucune popup "Invalid License" au démarrage
- [ ] Export PDF fonctionne (bouton 📑 dans RichTextEditor)

🎉 **C'est prêt !** Althéa peut maintenant exporter des documents RTF en PDF de qualité professionnelle.

---

**Dernière mise à jour :** 17/05/2026  
**Version Syncfusion utilisée :** 33.2.10  
**Projet :** Althéa
