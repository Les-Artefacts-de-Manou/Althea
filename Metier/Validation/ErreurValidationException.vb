' -------------------------------------------------------------------------------------------------
' Classe      : ErreurValidationException
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Refus d'une écriture dont la saisie ne respecte pas les règles du métier (audit C.2) ; porte la
' liste des erreurs trouvées par le valideur (ErreurSaisie).
'
' Important   :
' - Hérite d'InvalidOperationException : les écrans qui affichent déjà un refus métier (« Catch
'   exRefus As InvalidOperationException » → DialogChoix) l'affichent sans changement. Le message
'   reprend les messages des erreurs, une par ligne.
' - En temps normal, l'écran a appelé le valideur avant d'enregistrer et ce refus n'arrive pas ; il
'   protège les autres chemins d'écriture (autre écran, Agenda, version web).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public NotInheritable Class ErreurValidationException
    Inherits InvalidOperationException

    ' Erreurs trouvées par le valideur (au moins une).
    Public ReadOnly Property Erreurs As IReadOnlyList(Of ErreurSaisie)

    Public Sub New(erreurs As IReadOnlyList(Of ErreurSaisie))
        MyBase.New(String.Join(Environment.NewLine, erreurs.Select(Function(e) e.Message)))
        Me.Erreurs = erreurs
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : LeverSiErreurs
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Refuse l'écriture si le valideur a trouvé au moins une erreur ; ne fait rien sinon.
    ' -------------------------------------------------------------------------------------------------
    Public Shared Sub LeverSiErreurs(erreurs As List(Of ErreurSaisie))

        If erreurs IsNot Nothing AndAlso erreurs.Count > 0 Then
            Throw New ErreurValidationException(erreurs.AsReadOnly())
        End If

    End Sub

End Class
