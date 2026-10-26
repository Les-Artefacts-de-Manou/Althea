' -------------------------------------------------------------------------------------------------
' Module      : AffichageErreursSaisie
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Montre à l'utilisatrice les erreurs trouvées par un valideur du métier (ErreurSaisie), de la même
' façon dans toutes les fiches (audit C.2, décision de Joëlle du 23/10/2026) :
' - chaque champ en cause reçoit son icône d'erreur et son message ;
' - une fenêtre récapitule toutes les erreurs ;
' - le premier champ en cause reçoit le focus.
'
' Fonctionnement :
' - L'écran fournit la correspondance « propriété de l'objet métier → contrôle » (NameOf) et sa façon
'   de marquer un champ (ErrorProvider de la fenêtre, ou contexte de Home pour une fiche hébergée).
' - Une erreur dont le champ n'a pas de contrôle n'est que dans la fenêtre récapitulative.
' - L'icône d'un champ disparaît dès que l'utilisatrice modifie ce champ (relevé de Joëlle,
'   23/10/2026 : elle restait jusqu'à l'enregistrement suivant, voire au-delà) ; pour une date du kit
'   (AltheaDate, qui n'a pas de TextChanged), dès que sa valeur change (V1.1.0, étape 2b) ; pour une
'   case à cocher, dès qu'on la coche ou la décoche (V1.2.0, étape 3 : « Soumis à quota »).
'
' Remarques   :
' - Remplace les écrans qui ne marquaient que l'icône (Contact, Intervenant, Thérapeute) ou
'   l'icône et la barre d'état (fiche patient) : un refus doit se voir (décision de Joëlle).
'
' Dépendances :
' - ErreurSaisie (Metier/Validation), DialogChoix, AltheaDate
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module AffichageErreursSaisie

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : Signaler
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Marque les champs en cause, montre la fenêtre récapitulative et place le focus ; ne fait rien
    ' s'il n'y a pas d'erreur.
    '
    ' Paramètres :
    ' - erreurs   : Erreurs rendues par le valideur métier
    ' - controles : Contrôle de chaque propriété (clé : NameOf de la propriété de l'objet métier)
    ' - marquer   : Comment marquer un contrôle (contrôle, message)
    ' - titre     : Titre de la fenêtre (« Fiche patient », « Contact »…)
    '
    ' Retour     :
    ' - Boolean : True s'il y avait au moins une erreur (l'enregistrement doit s'arrêter)
    ' -------------------------------------------------------------------------------------------------
    Public Function Signaler(erreurs As IList(Of ErreurSaisie),
                             controles As IDictionary(Of String, Control),
                             marquer As Action(Of Control, String),
                             titre As String) As Boolean

        If erreurs Is Nothing OrElse erreurs.Count = 0 Then Return False

        Dim premier As Control = Nothing

        For Each erreur As ErreurSaisie In erreurs
            Dim controle As Control = Nothing
            If controles IsNot Nothing AndAlso controles.TryGetValue(erreur.Champ, controle) AndAlso controle IsNot Nothing Then
                marquer?.Invoke(controle, erreur.Message)
                EffacerALaCorrection(controle, marquer)
                If premier Is Nothing Then premier = controle
            End If
        Next

        Dim message As String =
            If(erreurs.Count = 1, "Veuillez corriger ce point :", "Veuillez corriger ces points :") &
            Environment.NewLine & Environment.NewLine &
            String.Join(Environment.NewLine, erreurs.Select(Function(e) "• " & e.Message))

        DialogChoix.Avertissement(message, titre)

        premier?.Focus()

        Return True

    End Function

    ' Retire l'icône du champ à sa première modification (une seule fois : le gestionnaire se retire).
    ' Une date du kit (AltheaDate, UserControl) ne lève pas TextChanged : on suit son ValeurChangee ;
    ' une case à cocher, son CheckedChanged.
    Private Sub EffacerALaCorrection(controle As Control, marquer As Action(Of Control, String))

        If marquer Is Nothing Then Return

        Dim dateKit As AltheaDate = TryCast(controle, AltheaDate)
        Dim caseACocher As CheckBox = TryCast(controle, CheckBox)
        Dim gestionnaire As EventHandler = Nothing

        gestionnaire = Sub(s, e)
                           If dateKit IsNot Nothing Then
                               RemoveHandler dateKit.ValeurChangee, gestionnaire
                           ElseIf caseACocher IsNot Nothing Then
                               RemoveHandler caseACocher.CheckedChanged, gestionnaire
                           Else
                               RemoveHandler controle.TextChanged, gestionnaire
                           End If
                           marquer(controle, String.Empty)
                       End Sub

        If dateKit IsNot Nothing Then
            AddHandler dateKit.ValeurChangee, gestionnaire
        ElseIf caseACocher IsNot Nothing Then
            AddHandler caseACocher.CheckedChanged, gestionnaire
        Else
            AddHandler controle.TextChanged, gestionnaire
        End If

    End Sub

End Module
