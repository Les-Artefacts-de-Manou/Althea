' -------------------------------------------------------------------------------------------------
' Module      : QuerySauvegardes
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 12/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Requêtes du journal des sauvegardes (tec_sauvegardes, migration v2.38.0) et lecture de la
' version du schéma inscrite dans l'archive.
'
' Remarques   :
' - Uniquement du SQL ; utilisé par GestionSauvegardes.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QuerySauvegardes

    ' Ligne créée au lancement d'une sauvegarde (statut EN_COURS).
    Public ReadOnly Property InsertDebut As String
        Get
            Return "
INSERT INTO tec_sauvegardes (statut_sauvegarde, nom_fichier, dossier_sauvegarde, version_schema,
                             version_application, id_utilisateur, login_utilisateur, machine)
VALUES (@statut, @nom_fichier, @dossier, @version_schema, @version_application,
        @id_utilisateur, @login, @machine);
SELECT LAST_INSERT_ID();
"
        End Get
    End Property

    ' Clôture d'une sauvegarde : réussie ou en échec.
    Public ReadOnly Property UpdateFin As String
        Get
            Return "
UPDATE tec_sauvegardes
SET date_fin = NOW(),
    statut_sauvegarde = @statut,
    taille_octets = @taille,
    nb_fichiers = @nb_fichiers,
    message_erreur = @message
WHERE id_sauvegarde = @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectHistorique As String
        Get
            Return "
SELECT id_sauvegarde, date_debut, date_fin, statut_sauvegarde, nom_fichier, dossier_sauvegarde,
       taille_octets, nb_fichiers, version_schema, login_utilisateur, machine, message_erreur
FROM tec_sauvegardes
ORDER BY date_debut DESC, id_sauvegarde DESC
LIMIT @limite;
"
        End Get
    End Property

    Public ReadOnly Property SelectDateDerniereReussie As String
        Get
            Return "
SELECT MAX(date_fin)
FROM tec_sauvegardes
WHERE statut_sauvegarde = @statut;
"
        End Get
    End Property

    Public ReadOnly Property SelectVersionSchema As String
        Get
            Return "
SELECT version_schema
FROM tec_meta_schema
ORDER BY date_application DESC
LIMIT 1;
"
        End Get
    End Property

End Module
