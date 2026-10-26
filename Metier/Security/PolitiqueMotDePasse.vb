' -------------------------------------------------------------------------------------------------
' Module      : PolitiqueMotDePasse
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Règle de solidité d'un mot de passe, définie une seule fois (audit C.2 étape 4, ADR-51) : longueur
' minimale, au moins une majuscule, une minuscule et un chiffre (lettres accentuées admises).
'
' Utilisation :
' - Mots de passe des comptes (LongueurMinCompte) : GestionUtilisateurs (changement, réinitialisation,
'   création) et la fenêtre ChangePassword.
' - Mot de passe des archives de sauvegarde (LongueurMinSauvegarde, plus exigeant : il protège toutes
'   les données, y compris hors de l'application) : GestionSauvegardes.
'
' Remarques   :
' - La règle était écrite dans ChangePassword (comptes) et recopiée dans GestionSauvegardes (archives) ;
'   le métier des comptes l'ignorait (il acceptait « a »).
' - Les longueurs restent celles d'avant (10 et 12).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module PolitiqueMotDePasse

    ' Longueur minimale du mot de passe d'un compte.
    Public Const LongueurMinCompte As Integer = 10

    ' Longueur minimale du mot de passe des archives de sauvegarde.
    Public Const LongueurMinSauvegarde As Integer = 12

    ' Nom du champ des erreurs de saisie (ErreurSaisie.Champ) : le mot de passe n'est pas une propriété
    ' lisible d'un objet métier (seule son empreinte l'est).
    Public Const ChampMotDePasse As String = "MotDePasse"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : EstConforme
    ' Rôle     : Vrai si le mot de passe respecte la règle pour la longueur minimale donnée.
    ' -------------------------------------------------------------------------------------------------
    Public Function EstConforme(motDePasse As String, Optional longueurMin As Integer = LongueurMinCompte) As Boolean

        If String.IsNullOrEmpty(motDePasse) OrElse motDePasse.Length < longueurMin Then Return False

        Return motDePasse.Any(AddressOf Char.IsUpper) AndAlso
               motDePasse.Any(AddressOf Char.IsLower) AndAlso
               motDePasse.Any(AddressOf Char.IsDigit)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Description
    ' Rôle     : La règle en clair (« Au moins 10 caractères, dont une majuscule… »), pour l'écran.
    ' -------------------------------------------------------------------------------------------------
    Public Function Description(Optional longueurMin As Integer = LongueurMinCompte) As String
        Return $"Au moins {longueurMin} caractères, dont une majuscule, une minuscule et un chiffre."
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Valider
    ' Version  : V1.0.0
    ' Date     : 23/10/2026
    '
    ' Rôle     :
    ' Erreur de saisie du mot de passe (liste vide s'il est conforme).
    '
    ' Paramètres :
    ' - motDePasse : mot de passe saisi
    ' - longueurMin : LongueurMinCompte (défaut) ou LongueurMinSauvegarde
    ' -------------------------------------------------------------------------------------------------
    Public Function Valider(motDePasse As String, Optional longueurMin As Integer = LongueurMinCompte) As List(Of ErreurSaisie)

        Dim erreurs As New List(Of ErreurSaisie)

        If Not EstConforme(motDePasse, longueurMin) Then
            erreurs.Add(New ErreurSaisie(ChampMotDePasse, "Le mot de passe ne respecte pas les règles : " & Description(longueurMin)))
        End If

        Return erreurs

    End Function

End Module
