'------------------------------------------------------------
' Fichier :  AppRole.vb
' Projet : Althéa
' Version : V1.0.0
' Date    : 01/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle    :
' Définit les rôles globaux des utilisateurs de l’application.
'
'Responsabilités :
' - Fournir une énumération claire et centralisée des rôles d'application.
'------------------------------------------------------------

Option Strict On
Option Explicit On

' Enumération des rôles utilisateurs dans l'application
Public Enum AppRole
    User = 0
    SuperUser = 1
    Admin = 2
End Enum