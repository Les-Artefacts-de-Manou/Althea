' -------------------------------------------------------------------------------------------------
' Module      : ClesParametres
' Projet      : Althéa
' Version     : V1.5.0
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Point unique des clés de tec_parametres lues par le code, et des valeurs de secours utilisées
' quand la base ne répond pas ou que le paramètre est absent, inactif ou invalide.
'
' Remarques   :
' - La valeur de référence est en base (migration v2.37.0) ; les valeurs de secours ne servent
'   qu'à garder l'application utilisable et reprennent la valeur livrée.
' - Les bornes (valeur_min / valeur_max) sont en base, pas ici.
' - Seule la racine du dossier de données est un paramètre (PATH_GENERAL) ; les sous-dossiers
'   sont fixes (ArborescenceDonnees). Pas de valeur de secours pour la racine : écrire des
'   fichiers patients dans un dossier par défaut serait pire qu'une erreur claire.
' - V1.2.0 (19/10/2026) : mention de confidentialité des documents (v2.41.0).
' - V1.3.0 (20/10/2026) : correcteur d'orthographe de l'éditeur (v2.44.0).
' - V1.4.0 (20/10/2026) : modalités de paiement proposées à la création d'un dossier (v2.46.0).
' - V1.5.0 (20/10/2026) : fin de l'élévation après inactivité (v2.48.0).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module ClesParametres

    ' Dossier de données
    Public Const CheminRacineDonnees As String = "PATH_GENERAL"

    ' Séances
    Public Const SeanceDureeDefautMinutes As String = "SEANCE_DUREE_DEFAUT_MIN"
    Public Const DefautSeanceDureeMinutes As Integer = 45
    Public Const SeancePasHoraireMinutes As String = "SEANCE_PAS_HORAIRE_MIN"
    Public Const DefautSeancePasHoraireMinutes As Integer = 15
    Public Const SeanceHeureDebutGrille As String = "SEANCE_HEURE_DEBUT_GRILLE"
    Public Const DefautSeanceHeureDebutGrille As Integer = 8
    Public Const SeanceHeureFinGrille As String = "SEANCE_HEURE_FIN_GRILLE"
    Public Const DefautSeanceHeureFinGrille As Integer = 20

    ' Sécurité
    Public Const SecuriteMaxEchecsLogin As String = "SECURITE_MAX_ECHECS_LOGIN"
    Public Const DefautMaxEchecsLogin As Integer = 5
    Public Const SecuriteElevationMinutes As String = "SECURITE_ELEVATION_MINUTES"
    Public Const DefautElevationMinutes As Integer = 15

    ' Journaux
    Public Const LogRetentionJours As String = "LOG_RETENTION_JOURS"
    Public Const DefautLogRetentionJours As Integer = 7

    ' Cabinet
    Public Const CachetEncreNoire As String = "CACHET_ENCRE_NOIRE"
    Public Const DefautCachetEncreNoire As Boolean = False

    ' Sauvegardes (v2.38.0) ; dossiers sans valeur de secours (vide = pas encore choisi)
    Public Const SauvegardeDossier As String = "SAUVEGARDE_DOSSIER"
    Public Const SauvegardeDossierMariaDB As String = "SAUVEGARDE_DOSSIER_MARIADB"
    Public Const SauvegardeNbConservees As String = "SAUVEGARDE_NB_CONSERVEES"
    Public Const DefautSauvegardeNbConservees As Integer = 10
    Public Const SauvegardeRappelJours As String = "SAUVEGARDE_RAPPEL_JOURS"
    Public Const DefautSauvegardeRappelJours As Integer = 7

    ' Documents imprimés et exportés (v2.41.0) ; vide en base = pas de mention
    Public Const DocumentMentionConfidentialite As String = "DOCUMENT_MENTION_CONFIDENTIALITE"
    Public Const DefautDocumentMentionConfidentialite As String = "Confidentiel"

    ' Éditeur de texte (v2.44.0)
    Public Const EditeurCorrecteurOrthographe As String = "EDITEUR_CORRECTEUR_ORTHOGRAPHE"
    Public Const DefautEditeurCorrecteurOrthographe As Boolean = True

    ' Dossiers (v2.46.0) ; vide en base = rien n'est proposé
    Public Const DossierModalitePaiementDefaut As String = "DOSSIER_MODALITE_PAIEMENT_DEFAUT"
    Public Const DefautDossierModalitePaiement As String = "À la séance"

End Module
