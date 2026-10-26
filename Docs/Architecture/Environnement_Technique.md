# Althéa - Environnement technique

> *Dernière mise à jour : 26/10/2026 — intégration continue sur GitHub (ADR-59)*

---

## Architecture technique

#### Application

- VB.NET
- .NET 8 LTS
- Windows Forms
- `Option Strict On` et `Option Explicit On` pour tout le projet (`Althea.vbproj`, ADR-57)
- Analyseurs .NET intégrés au SDK, jeu « Recommended » ; niveaux par règle et conventions d'écriture dans `.editorconfig` (racine du dépôt)

#### Base de données

- MariaDB
- UTF8MB4
- Couches : UI / Metier / Core, et Utils (services transverses)
  voir détail dans [Architecture_Technique](Architecture_Technique.md)

### Outils de développement

| Outil | Usage | Version | Date |
| ----- | ----- | ----- | ----- |
| **Visual Studio Community** | IDE principal - VB.NET / .NET 8 LTS / WinForms | 18.11.1 | 21/10/26 |
| **MariaDB** | Base de données (charset `utf8mb4`, collation `utf8mb4_uca1400_ai_ci`, base `Althea`) | 12.3.3 | 05/09/26 |
| **HeidiSQL** | Administration et visualisation de la base MariaDB | 12.21.7360 | 21/10/26 |
| **DBeaver** | Visualisation de la base MariaDB, génération de diagrammes | 26.2.2 | 07/10/26 |
| **Typora** | Rédaction de la documentation Markdown | 1.14.15 | 25/10/26 |
| **LordIcon** | Icônes pour l'interface utilisateur – https://lordicon.com/ |  | Abonnement annuel - repris en 10/26 |
| **Icone8** | Icônes pour l'interface utilisateur – https://icons8.fr/ |  | Libre de droits. Abonnement Non|

Compilation hors de Visual Studio : `Tools\build.cmd` (journal complet dans `obj\_build.log`, en UTF-8) ; les options données au script sont transmises à `dotnet build`.

Tests automatisés : `Tools\test.cmd` (compile, lance tous les tests, journal `obj\_tests.log`) ou l'Explorateur de tests de Visual Studio (ADR-58).

### Packages NuGet

| Package | Version | Date |  |
| ------- | ------- | ----- | ------- |
| **MySqlConnector** | 2.6.2 | 08/26 | Bradley Grainger - A truly async MySQL ADO.NET provider, supporting MySQL Server, MariaDB, Amazon Aurora, Azure Database for MySQL, Google Cloud SQL, and more. https://mysqlconnector.net/ |
| **SharpZipLib** | 1.4.2 | 12/10/26 | Licence MIT (icsharpcode). Écriture et lecture des archives ZIP chiffrées AES-256 des sauvegardes (format WinZip AES, ouvrable avec 7-Zip) — `GestionSauvegardes`. |
| **Syncfusion.DocIO.WinForms** | 35.1.41 | 20/10/26 | Avec licence gratuite ; clé saisie dans l'application, chiffrée sur le poste (ADR-41). The Syncfusion® [WinForms Word library](https://www.syncfusion.com/word-framework/net/word-library?utm_source=nuget&utm_medium=listing&utm_campaign=winforms-docio-nuget) (Essential® DocIO) is a feature-rich and high-performance .NET Word library that is used to create, read, and edit Word documents programmatically without Microsoft Office dependencies. |
| **Syncfusion.DocToPDFConverter.WinForms** | 35.1.41 | 20/10/26 | The Syncfusion® [WinForms Word library](https://www.syncfusion.com/word-framework/net/word-library?utm_source=nuget&utm_medium=listing&utm_campaign=winforms-doctopdfconverter-nuget) (Essential® DocIO) converts a [Word document to PDF](https://www.syncfusion.com/word-framework/net/word-to-pdf-conversion?utm_source=nuget&utm_medium=listing&utm_campaign=winforms-doctopdfconverter-nuget) with just five lines of code and also it does not require Adobe and Microsoft Word application to be installed in the machine. It preserves the original appearance of the Word document in the converted PDF document |
| **xunit.v3** | 3.2.2 | 26/10/26 | Projet de tests `Tests/Althea.Tests` uniquement (ADR-58) : cadre de tests xUnit.net v3. |
| **xunit.runner.visualstudio** | 3.1.5 | 26/10/26 | Projet de tests uniquement : exécution par `dotnet test` et l'Explorateur de tests de Visual Studio. |
| **Microsoft.NET.Test.Sdk** | 18.10.1 | 26/10/26 | Projet de tests uniquement : plateforme d'exécution des tests (VSTest). |

### Versioning

- **GitHub (privé)** : https://github.com/AngeljoNG/Althea - mis à jour en continu
- **Intégration continue** : GitHub Actions, `.github/workflows/ci.yml` (ADR-59) — compilation Release et tests sur Windows, chaque soir s'il y a eu un commit et à la demande (onglet Actions).
- **GitHub (public)** : https://github.com/Les-Artefacts-de-Manou/Althea.git  dernière mise à jour 24/06/26

------

> [!NOTE]
>
> ### **Contact**
>
> **Joëlle De Poorter - Les Artefacts de Manou**
>
> Projet réalisé pour soutenir une pratique de psychologie et de graphologie dans la gestion fiable et évolutive des patients et documents.
>
> - Site web     Auteur/Les Artefacts de Manou/Joëlle De Poorter : [https://lesartefactsdemanou.be](https://lesartefactsdemanou.be/)
> - Site web     Psychologue & Graphologue: https://pearlnguyenduy.be/
> - mailto: [Privé](mailto:joelle@nguyen.eu) et [Les Artefacts de Manou](mailto:lesartefactsdemanou@gmail.com)
> - GitHub privé:     Althea https://github.com/AngeljoNG/Althea
> - GitHub public :     https://github.com/Les-Artefacts-de-Manou/Althea

------





[TOC]
