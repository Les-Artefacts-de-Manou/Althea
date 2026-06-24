' -------------------------------------------------------------------------------------------------
' Module      : Program
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 21/04/2026
' Auteur      : Manou / Projet Althéa
'
' Rôle        :
' Point d'entrée principal de l'application.
' Initialise l'environnement WinForms et lance la Form principale (Home).
'
' Remarques   :
' - Le flux de démarrage complet sera géré ultérieurement (Config, Logs, DB, Splash).
' - Aucun code métier ne doit être ajouté ici.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On
Option Infer On
Imports Syncfusion.Licensing

Friend Module Program

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Run
    ' Projet     : Althéa
    ' Version    : V 1.2.0
    ' Date       : 24/04/2026
    ' Auteur     : Manou / Projet Althéa
    '
    ' Rôle       : 
    ' Lance le flux de démarrage complet de l'application.
    ' -------------------------------------------------------------------------------------------------
    <STAThread()>
    Friend Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        ' Enregistrer la licence Syncfusion (Community gratuite)
        ' IMPORTANT : Remplacer "VOTRE-CLE-SYNCFUSION" par votre vraie clé de licence
        SyncfusionLicenseProvider.RegisterLicense("")

        ' Lancement de la Form principale
        AppStartupManager.Run()


    End Sub

End Module
