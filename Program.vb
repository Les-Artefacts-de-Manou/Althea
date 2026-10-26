' -------------------------------------------------------------------------------------------------
' Module      : Program
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 20/10/2026
' Auteur      : Manou / Projet Althéa
'
' Rôle        :
' Point d'entrée principal de l'application.
' Initialise l'environnement WinForms et lance le flux de démarrage (AppStartupManager).
'
' Remarques   :
' - Aucun code métier ne doit être ajouté ici.
' - Aucune clé ni secret ici : la licence Syncfusion est lue sur le poste au démarrage
'   (GestionLicences.AppliquerLicenceSyncfusion, appelée par AppStartupManager.Run).
'
' Historique  :
' - V1.1.0 (18/10/2026) : clé de licence Syncfusion retirée du code (elle était écrite en clair et
'   versionnée) ; elle est désormais saisie dans Paramètres et chiffrée sur le poste.
' - V1.2.0 (20/10/2026) : filets des erreurs non interceptées installés en premier
'   (GestionErreursGlobales ; audit § 3.3).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On
Option Infer On

Friend Module Program

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Main
    ' Version    : V1.4.0
    ' Date       : 20/10/2026
    '
    ' Rôle       :
    ' Prépare WinForms puis lance le flux de démarrage complet de l'application.
    '
    ' Remarques  :
    ' - V1.4.0 (20/10/2026) : GestionErreursGlobales.Installer avant toute fenêtre (le mode de
    '   traitement des erreurs doit être fixé avant la première).
    ' -------------------------------------------------------------------------------------------------
    <STAThread()>
    Friend Sub Main()

        GestionErreursGlobales.Installer()

        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        AppStartupManager.Run()

    End Sub

End Module
