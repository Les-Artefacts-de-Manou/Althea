@echo off
rem -------------------------------------------------------------------------------------------------
rem Script      : build.cmd
rem Projet      : Althea
rem Version     : V1.1.0
rem Date        : 25/10/2026
rem Auteur      : Joelle (Manou) / Projet Althea
rem
rem Role        :
rem Genere la solution et ecrit le resultat complet dans obj\_build.log, en UTF-8.
rem Le journal est lisible depuis une session Claude liee au dossier du projet.
rem
rem Remarques   :
rem - obj\ est ignore par Git : le journal ne part jamais sur GitHub.
rem - Double-clic, ou "Tools\build.cmd" depuis une invite de commandes.
rem - Les options eventuelles sont transmises a dotnet build, par exemple
rem   "Tools\build.cmd -p:OptionStrict=On --no-incremental" (inventaire Option Strict).
rem - Ferme les fichiers du projet dans Visual Studio avant de lancer une session de
rem   modification externe : VS reecrit les fichiers ouverts lors de la generation.
rem
rem Historique :
rem - V1.0.0 (10/10/2026) : creation.
rem - V1.1.0 (25/10/2026) : options de la ligne de commande transmises a dotnet build (audit D-1).
rem -------------------------------------------------------------------------------------------------

chcp 65001 >nul
cd /d "%~dp0.."

where dotnet >nul 2>&1
if errorlevel 1 (
    echo.
    echo ERREUR : la commande "dotnet" est introuvable.
    echo Ouvre plutot "Developer Command Prompt for VS" et relance ce script,
    echo ou ajoute le SDK .NET au PATH.
    echo.
    pause
    exit /b 9009
)

if not exist "obj" mkdir "obj"

echo Generation d'Althea en cours...
echo.

dotnet build Althea.vbproj -v minimal --nologo %* > "obj\_build.log" 2>&1
set CODE=%ERRORLEVEL%

type "obj\_build.log"

echo.
if "%CODE%"=="0" (
    echo RESULTAT : generation reussie.
) else (
    echo RESULTAT : echec ^(code %CODE%^).
)
echo Journal complet : obj\_build.log
echo.
pause
exit /b %CODE%
