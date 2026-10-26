' -------------------------------------------------------------------------------------------------
' Classe      : ErreurSaisie
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Une erreur de saisie trouvée par un valideur du métier (audit C.2) : le champ en cause et le
' message à montrer à l'utilisatrice.
'
' Fonctionnement :
' - Un valideur métier (GestionPaiements.ValiderEncaissement…) rend la liste des erreurs d'un objet,
'   vide s'il est valide. L'écran l'appelle avant d'enregistrer et place chaque erreur sur son
'   contrôle ; la méthode d'écriture appelle le même valideur et refuse s'il reste une erreur
'   (ErreurValidationException). La règle n'existe donc qu'une fois (décision de Joëlle, 22/10/2026).
'
' Important   :
' - Champ est le nom de la propriété de l'objet métier, écrit avec NameOf (NameOf(Paiement.Montant)) :
'   aucun nom de contrôle d'écran ne descend dans le métier, et un renommage est vu à la compilation.
' - Champ vide : erreur qui ne vise aucun champ en particulier.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public NotInheritable Class ErreurSaisie

    ' Propriété de l'objet métier en cause (NameOf), ou vide.
    Public ReadOnly Property Champ As String

    ' Message rédigé pour l'utilisatrice, sans détail technique.
    Public ReadOnly Property Message As String

    Public Sub New(champ As String, message As String)
        Me.Champ = If(champ, String.Empty)
        Me.Message = If(message, String.Empty)
    End Sub

End Class
