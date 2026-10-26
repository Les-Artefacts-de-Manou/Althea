' -------------------------------------------------------------------------------------------------
' Module      : QueryEtatSysteme
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Requêtes de l'écran État du système et du contrôle de version au démarrage (lecture seule).
'
' Historique  :
' - V1.1.0 (20/10/2026) : migrations lues à part (SelectMigrationsAppliquees) : la version de la base
'   est la plus haute appliquée, choisie dans le code — trier sur la date se trompait si une ancienne
'   migration était rejouée.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryEtatSysteme

    ' Version du serveur MariaDB.
    Public ReadOnly Property SelectInfosServeur As String
        Get
            Return "SELECT VERSION() AS version_serveur;"
        End Get
    End Property

    ' Migrations appliquées (tec_meta_schema) : la plus haute version est choisie dans le code.
    Public ReadOnly Property SelectMigrationsAppliquees As String
        Get
            Return "SELECT version_schema, date_application FROM tec_meta_schema;"
        End Get
    End Property

End Module
