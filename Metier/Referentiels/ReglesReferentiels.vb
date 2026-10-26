' -------------------------------------------------------------------------------------------------
' Module      : ReglesReferentiels
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Règles d'écriture communes à tous les référentiels (audit C.1, étape 3) : qui peut écrire, et le
' refus explicite quand une ligne système est protégée par le SQL.
'
' Responsabilités :
' - Exiger le rôle d'écriture des référentiels (SuperUser) : une seule définition, lue aussi par
'   les écrans (UC_ReferentielBase.RoleMinimum)
' - Exécuter une écriture protégée par est_systeme et refuser si aucune ligne n'est concernée
'
' Important   :
' - Lignes système (est_systeme = 1, ADR-28) : code non modifiable, ni désactivation ni suppression ;
'   la condition est écrite dans la requête SQL elle-même (WHERE … est_systeme = 0 …), pas
'   seulement dans l'écran. Libellé, ordre et champs descriptifs restent modifiables.
' - Le compte des lignes repose sur UseAffectedRows = False (DatabaseManager) : une ligne trouvée
'   compte même si ses valeurs ne changent pas.
' - L'ajout « à la volée » depuis une fiche (bouton [+]) suit la même règle : un User s'élève
'   d'abord (décision de Joëlle, 22/10/2026 ; ouverture aux Users à cadrer, ToDo).
'
' Dépendances :
' - Autorisation, ContexteOperation, AppRole
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports MySqlConnector

Public Module ReglesReferentiels

    ' Rôle exigé pour créer, modifier, désactiver ou supprimer une valeur de référentiel.
    Public Const ROLE_ECRITURE As AppRole = AppRole.SuperUser

    Private Const MESSAGE_DROIT As String =
        "La modification des référentiels est réservée au SuperUser ou à l'administrateur."

    Private Const MESSAGE_LIGNE_PROTEGEE As String =
        "Modification refusée : c'est une ligne système (cadenas), utilisée par l'application. " &
        "Son code ne change pas et elle reste active ; son libellé et son ordre restent modifiables." &
        vbCrLf & "Si la liste n'est plus à jour, actualisez-la puis recommencez."

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ExigerDroitEcriture
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       : Première ligne de chaque écriture de référentiel : refuse si le rôle courant
    '              n'atteint pas ROLE_ECRITURE (UnauthorizedAccessException, message pour l'utilisatrice).
    ' -------------------------------------------------------------------------------------------------
    Public Sub ExigerDroitEcriture(contexte As ContexteOperation)
        Autorisation.Exiger(contexte, ROLE_ECRITURE, MESSAGE_DROIT)
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ExecuterSurLigneModifiable
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       : Exécute une écriture dont la requête exclut elle-même ce qu'une ligne système
    '              interdit ; aucune ligne concernée → refus (InvalidOperationException).
    '
    ' Remarques  :
    ' - Aucune ligne peut aussi vouloir dire « supprimée entre-temps » : le message invite à actualiser.
    ' -------------------------------------------------------------------------------------------------
    Public Sub ExecuterSurLigneModifiable(cmd As MySqlCommand)

        If cmd.ExecuteNonQuery() = 0 Then
            Throw New InvalidOperationException(MESSAGE_LIGNE_PROTEGEE)
        End If

    End Sub

End Module
