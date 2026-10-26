' -------------------------------------------------------------------------------------------------
' Module      : QueryRestaurations
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 13/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Requêtes de la restauration (lot 30, étape P3) : journal tec_restaurations (migration v2.39.0),
' inventaire des objets de la base à vider, droits du compte d'administration.
'
' Remarques   :
' - Uniquement du SQL ; utilisé par GestionRestaurations.
' - Les requêtes d'inventaire portent sur DATABASE() : la base de la connexion, jamais une autre.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryRestaurations

    Public ReadOnly Property InsertJournal As String
        Get
            Return "
INSERT INTO tec_restaurations (date_debut, date_fin, statut_restauration, nom_archive, dossier_archive,
                               date_archive, version_schema_archive, machine_archive, nb_fichiers,
                               archive_securite, dossier_donnees_precedent, login_utilisateur, machine,
                               version_application, message_erreur)
VALUES (@date_debut, NOW(), @statut, @nom_archive, @dossier_archive,
        @date_archive, @version_schema_archive, @machine_archive, @nb_fichiers,
        @archive_securite, @dossier_precedent, @login, @machine,
        @version_application, @message);
"
        End Get
    End Property

    ' La ligne de l'archive restaurée est dans son propre dump à l'état EN_COURS (écrite avant
    ' l'export, clôturée après) : on la clôture dans la base restaurée.
    Public ReadOnly Property UpdateLigneArchiveRestauree As String
        Get
            Return "
UPDATE tec_sauvegardes
SET statut_sauvegarde = @reussie,
    date_fin = COALESCE(date_fin, date_debut),
    taille_octets = @taille,
    nb_fichiers = @nb_fichiers
WHERE nom_fichier = @nom
  AND statut_sauvegarde = @en_cours;
"
        End Get
    End Property

    ' Sauvegarde de sécurité faite juste avant la restauration : sa ligne était dans la base remplacée.
    Public ReadOnly Property InsertLigneSauvegardeSecurite As String
        Get
            Return "
INSERT INTO tec_sauvegardes (date_debut, date_fin, statut_sauvegarde, nom_fichier, dossier_sauvegarde,
                             taille_octets, nb_fichiers, version_schema, version_application,
                             login_utilisateur, machine)
SELECT @date_debut, NOW(), @statut, @nom_fichier, @dossier, @taille, @nb_fichiers,
       (SELECT version_schema FROM tec_meta_schema ORDER BY date_application DESC LIMIT 1),
       @version_application, @login, @machine;
"
        End Get
    End Property

    Public ReadOnly Property SelectHistorique As String
        Get
            Return "
SELECT date_debut, statut_restauration, nom_archive, date_archive, nb_fichiers, archive_securite,
       dossier_donnees_precedent, login_utilisateur, message_erreur
FROM tec_restaurations
ORDER BY date_debut DESC, id_restauration DESC
LIMIT @limite;
"
        End Get
    End Property

    ' Tables, vues et séquences de la base courante ; les séquences en dernier (les tables qui les
    ' utilisent en valeur par défaut sont supprimées avant elles).
    Public ReadOnly Property SelectObjetsBase As String
        Get
            Return "
SELECT table_name, table_type
FROM information_schema.tables
WHERE table_schema = DATABASE()
ORDER BY (table_type = 'SEQUENCE'), table_name;
"
        End Get
    End Property

    Public ReadOnly Property SelectRoutinesBase As String
        Get
            Return "
SELECT routine_name, routine_type
FROM information_schema.routines
WHERE routine_schema = DATABASE();
"
        End Get
    End Property

    ' Nombre de vues, déclencheurs et routines : droits supplémentaires à exiger s'il y en a.
    Public ReadOnly Property SelectNbObjetsProgrammes As String
        Get
            Return "
SELECT (SELECT COUNT(*) FROM information_schema.views WHERE table_schema = DATABASE())    AS nb_vues,
       (SELECT COUNT(*) FROM information_schema.triggers WHERE trigger_schema = DATABASE()) AS nb_declencheurs,
       (SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema = DATABASE()) AS nb_routines;
"
        End Get
    End Property

    Public ReadOnly Property ShowGrants As String
        Get
            Return "SHOW GRANTS FOR CURRENT_USER();"
        End Get
    End Property

    Public ReadOnly Property DesactiverClesEtrangeres As String
        Get
            Return "SET FOREIGN_KEY_CHECKS = 0;"
        End Get
    End Property

End Module
