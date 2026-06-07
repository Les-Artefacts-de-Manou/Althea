# Manifeste - Gestion Calendrier Althéa (POC initial) 17/04/26

## 🎯 Objectif

Valider l’intégration de Google Calendar dans Althéa afin de pouvoir gérer les séances comme des événements Agenda, de manière simple, fiable et maintenable.

Le POC doit prouver qu’Althéa peut :

- lire les calendriers accessibles
- lire les événements d’un calendrier
- créer un événement
- modifier un événement
- supprimer un événement

Ce POC ne vise pas encore la synchronisation métier complète, mais la validation technique et fonctionnelle du socle Agenda.

------

## 🧠 Positionnement métier

Dans Althéa :

- le **patient** est la personne
- le **dossier** est le contexte de suivi
- la **séance** est l’unité concrète de suivi
- Google Calendar est une **extension de travail et de visualisation**

La séance Althéa est le futur équivalent métier d’un événement Google Calendar.
 Le calendrier n’est donc pas la source métier principale.
 La source métier restera Althéa, sa base de données, et ses objets métier. 

------

## ☁️ Google APIs concernées

### API utilisée

- Google Calendar API

### Packages utilisés pour le POC

- Google.Apis.Calendar.v3
- Google.Apis.Auth
- Google.Apis
- Google.Apis.Services
- Google.Apis.Util.Store

Le POC réutilise l’infrastructure OAuth Desktop déjà validée dans le POC documentaire. 

------

## 🔐 Authentification

### Principe

L’authentification Google repose sur :

- `google_client_secret.json`
- stockage local du token utilisateur via `token.json`

### Règles

- `google_client_secret.json` = configuration OAuth fournie par Google
- `token.json` = cache local des autorisations utilisateur
- le token doit être supprimé si les scopes changent
- les scopes doivent être centralisés
- l’authentification ne doit pas être dupliquée dans chaque bouton

### Scopes validés dans le POC

- `DriveService.Scope.DriveFile`
- `CalendarService.Scope.Calendar`

### Règle critique

👉 un seul token = un seul ensemble global de permissions

Donc si l’on ajoute Calendar, il faut régénérer le token pour que les nouvelles autorisations soient prises en compte.

------

## 🏗️ Architecture POC retenue

Le POC Calendar est réalisé :

- dans la **même solution**
- mais dans une **form dédiée**
- avec une authentification centralisée

### Form dédiée

- `FormCalendarPOC`

### Principe d’architecture

- pas de nouveau projet séparé
- pas de mélange avec la form du POC documentaire
- pas de duplication sauvage du code OAuth
- une méthode centrale fournit le `CalendarService`

### Méthode centrale validée

- `InitCalendarService()`

Cette méthode :

- initialise le service Google Calendar
- réutilise le token local existant si valide
- évite de recréer le service à chaque clic bouton

------

## ✅ Fonctionnalités validées dans le POC

### 1. Initialisation du service Calendar

Validation obtenue :

- API activée
- OAuth fonctionnel
- lecture des calendriers possible

### 2. Lecture des calendriers

Validation obtenue :

- récupération de la liste des calendriers accessibles
- affichage du calendrier principal et des calendriers liés

### 3. Lecture des événements du jour

Validation obtenue :

- lecture filtrée par plage de dates
- récupération du titre
- récupération des dates début / fin
- récupération de l’identifiant Google de l’événement

### 4. Création d’un événement

Validation obtenue :

- création d’un événement test depuis Althéa
- apparition correcte dans Google Agenda
- récupération de l’ID Google créé

### 5. Modification d’un événement

Validation obtenue :

- récupération d’un événement par son ID
- modification de son titre
- mise à jour correcte dans Google Agenda

### 6. Suppression d’un événement

Validation obtenue :

- suppression d’un événement via son ID
- disparition correcte dans Google Calendar
- gestion correcte des erreurs (ID invalide ou déjà supprimé)

### 7. Lecture des événements sur une semaine

Validation obtenue :

- lecture sur une plage de 7 jours
- tri correct par date de début
- gestion des événements multi-jours
- base validée pour un affichage planning

------

### 8. Gestion des cas limites

Validation obtenue :

- gestion d’un jour sans événement
- gestion des événements “journée entière”
- gestion des événements sans description
- gestion des erreurs API (ID invalide)
- robustesse globale du code validée

------

### 9. Création d’événements structurés Althéa

Validation obtenue :

- création d’événements contenant des informations métier structurées
- utilisation de la description pour stocker des marqueurs :

```
ALTH_ID_SEANCE
ALTH_ID_PATIENT
ALTH_TYPE
ALTH_STATUS
```

- séparation claire entre :
  - affichage utilisateur (titre)
  - données métier (description)

------

### 10. Détection des événements Althéa

Validation obtenue :

- capacité à distinguer :
  - événements Althéa
  - événements externes (personnels, autres)
- détection basée sur la présence des marqueurs `ALTH_` dans la description
- indépendance totale vis-à-vis :
  - des couleurs
  - du titre

------

### 11. Validation sur calendrier réel (mixte)

Validation obtenue :

- lecture correcte d’un calendrier contenant des événements variés
- coexistence sans conflit entre :
  - événements Althéa
  - événements personnels
- robustesse confirmée en conditions proches du réel

------

## 📅 Règles de gestion des événements

### Règle 1 - Le calendrier peut être mixte

Le calendrier réel de Pearl peut contenir :

- rendez-vous professionnels
- rendez-vous personnels
- rendez-vous enfants / famille
- rappels divers

👉 Althéa doit fonctionner dans un calendrier mixte si nécessaire.

### Règle 2 - Les couleurs sont visuelles uniquement

Pearl utilise un code couleur utile humainement :

- jaune = Dysmoi
- bleu foncé = Grapho
- mauve foncé = Psycho
- mauve clair = PPL Réalism
- vert = attente de validation

👉 Ces couleurs devront être respectées plus tard pour l’affichage et le confort d’usage.
 👉 Mais elles ne seront **jamais** utilisées comme source de vérité métier.

### Règle 3 - La logique métier ne dépend pas du titre seul

Le titre d’un événement doit rester lisible pour l’humain.
 Il ne doit pas être la seule base d’identification par l’application.

### Règle 4 - Les événements Althéa devront être marqués structurellement

À terme, un événement lié à une séance Althéa devra contenir des identifiants métier, par exemple :

- `id_seance`
- `id_patient`
- type de séance
- statut

Ces informations permettront de distinguer :

- les événements gérés par Althéa
- les événements externes ou personnels

------

## 🧪 Stratégie POC

Le POC travaille sur un calendrier de test simple, peu utilisé, afin de :

- éviter toute confusion
- éviter toute modification accidentelle d’un agenda réel chargé
- visualiser facilement les créations / modifications / suppressions

Les événements de test doivent être clairement identifiables, par exemple avec un préfixe :

- `POC_ALTHEA - ...`

------

## ⚠️ Limites volontaires du POC

Le POC **ne traite pas encore** :

- synchronisation automatique
- gestion des conflits
- multi-calendriers métier
- invitations participants
- rappels avancés
- récurrence
- import métier massif
- reprise automatique intelligente des rendez-vous existants

Ces points appartiennent à une phase ultérieure.

------

## 🔄 Reprise future de l’existant

Lors de l’installation réelle chez Pearl, des rendez-vous futurs existeront déjà dans Google Agenda.

### Principe retenu

La reprise ne devra **pas** être automatique à 100 %.

### Règle future

La reprise devra être :

- **assistée**
- **semi-automatique**
- **validée par l’utilisatrice**

Pourquoi :

- calendrier mixte
- titres parfois ambigus
- événements perso mélangés
- absence d’identifiants métier dans les anciens événements

### Conséquence

Althéa devra plus tard proposer :

- lecture des événements futurs
- suggestion de correspondances
- validation manuelle par Pearl
- exclusion explicite des événements non métier

------

## 🧠 Règles techniques importantes

### Dates / horaires

Google Calendar utilise désormais des propriétés basées sur `DateTimeOffset`.

Règle :

- utiliser les propriétés modernes
- éviter les anciennes propriétés obsolètes

### Lecture temporelle

Pour lire les événements d’un jour :

- définir une plage claire
- `today` → `tomorrow`
- trier par heure de début

### Gestion des événements

Un événement Google peut être :

- horaire
- journée entière

Le code doit donc gérer :

- `DateTimeDateTimeOffset`
- ou `Date`

## 🖥️ Affichage Agenda (POC UI)

### Objectif

Valider la capacité d’Althéa à afficher les événements Google Calendar dans une interface agenda intégrée.

------

### Contrôle retenu (POC)

- `ScheduleControl` (Syncfusion WinForms) 
- Ajouter le package via NuGet (la toolbox de Syncfusion ne s'affiche que lorsqu'un contrôle est ajouté dans le projet)

------

### Validation obtenue

- affichage correct des événements sur une vue semaine
- positionnement correct des événements dans le temps
- affichage des titres
- intégration fonctionnelle avec .NET 8
- alimentation via `ArrayListDataProvider`

------

### Mapping technique validé

- Google Event → `ArrayListAppointment`
- stockage de l’objet Google via `Tag`
- affichage des notes via `Content`

------

### Règle validée

👉 séparation stricte :

- UI → `Content`
- Données métier → `Tag`

------

### Distinction visuelle (POC)

- préfixe `[ALTH]` pour événements Althéa
- préfixe `[EXT]` pour événements externes

------

### Interaction validée

- clic sur un événement
- récupération des données Google associées via `Tag`

------

### Évaluation du contrôle

- rendu visuel jugé :
  - classique mais acceptable
  - cohérent avec un usage métier
- lisibilité correcte en vue semaine
- contrôle validé pour :
  - POC
  - version V1 possible

------

### Limites identifiées

- API ancienne (ArrayListDataProvider)
- personnalisation visuelle limitée
- gestion des couleurs moins directe

------

### Décision

👉 Le contrôle est **retenu pour la suite du projet (provisoirement)**
 👉 Il reste **remplaçable** sans impact sur la logique métier

------

## 🚀 Validation actuelle du pilier Calendar

À ce stade, le POC a démontré que :

- l’authentification Google existante est réutilisable
- Google Calendar est accessible depuis Althéa
- le CRUD de base sur les événements est faisable
- la base technique du futur module Agenda est saine

------

## 🏁 Conclusion

Le POC Calendar valide désormais :

- intégration complète de Google Calendar
- CRUD complet des événements
- robustesse face aux cas réels
- capacité à distinguer événements métier et externes
- structuration des événements pour Althéa
- affichage agenda fonctionnel dans l’application

👉 Le pilier Calendar est désormais **techniquement et fonctionnellement validé**

------

### Vision retenue

- Google Calendar = outil de planification et de visualisation
- Althéa = source de vérité métier
- les événements Google = projection des séances Althéa

------

### Principe clé validé

👉 Althéa ne dépend pas :

- des couleurs
- du titre
- du contexte visuel

👉 Althéa repose sur :

- des identifiants métier
- une structure de données fiable

------

### État du projet

Le module Calendar est prêt pour :

- intégration avec la base de données
- liaison avec les objets métier (séances)
- évolution vers une synchronisation contrôlée





[TOC]

