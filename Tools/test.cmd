@echo off
rem -------------------------------------------------------------------------------------------------
rem Script      : test.cmd
rem Projet      : Althea
rem Version     : V1.0.0
rem Date        : 26/10/2026
rem Auteur      : Joelle (Manou) / Projet Althea
rem
rem Role        :
rem Compile l'application et le projet de tests, lance tous les tests automatises et ecrit le
rem resultat complet dans obj\_tests.log, en UTF-8 (lisible depuis une session Claude liee au dossier).
rem
rem Remarques   :
rem - obj\ est ignore par Git : le journal ne part jamais sur GitHub.
rem - Double-clic, ou "Tools\test.cmd" depuis une invite de commandes.
rem - Les options eventuelles sont transmises a dotnet test, par exemple
rem   --filter "FullyQualifiedName~UtilsNiss" pour ne lancer qu'une classe de tests.
rem - Le test de fumee des requetes (base althea_tests) est ignore si ALTHEA_TESTS_CONNEXION
rem   n'est pas definie sur le poste.
rem
rem Historique :
rem - V1.0.0 (26/10/2026) : creation (audit D-2).
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

echo Tests automatises d'Althea en cours...
echo.

dotnet test Tests\Althea.Tests\Althea.Tests.vbproj --nologo -v minimal --logger "console;verbosity=normal" %* > "obj\_tests.log" 2>&1
set CODE=%ERRORLEVEL%

type "obj\_tests.log"

echo.
if "%CODE%"=="0" (
    echo RESULTAT : tous les tests sont passes.
) else (
    echo RESULTAT : echec ^(code %CODE%^) - voir les tests en erreur ci-dessus.
)
echo Journal complet : obj\_tests.log
echo.
pause
exit /b %CODE%
