' -------------------------------------------------------------------------------------------------
' Classe     : NavigationEntry
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 11/06/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Décrit une étape de navigation rejouable, conservée dans l'historique Push/Pop du
' NavigationManager (mini-pile de navigation, D-Q15).
'
' Responsabilités :
' - Mémoriser une fabrique de vue (Func(Of UserControl)) permettant de RECRÉER l'écran au retour
' - Mémoriser le bouton de menu à resélectionner pour rester cohérent visuellement
' - Mémoriser le libellé de contexte hiérarchique associé à l'écran
' - Transporter optionnellement un état de filtre (dernier filtre de recherche) à restaurer
'
' Remarques  :
' - Les vues précédentes sont libérées (Dispose) lors de la navigation : on ne stocke donc PAS
'   l'instance mais une FABRIQUE qui la recrée à l'identique au moment du retour
' - La fabrique capture généralement l'état de filtre par closure ; EtatFiltre reste disponible
'   pour les écrans qui préfèrent y accéder explicitement
' - Classe de données immuable (propriétés ReadOnly affectées au constructeur)
'
' Dépendances :
' - System.Windows.Forms (UserControl, Button)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class NavigationEntry

#Region "Propriétés"

    ' Fabrique recréant la vue de cette étape (les anciennes vues étant libérées à la navigation).
    Public ReadOnly Property CreerVue As Func(Of UserControl)

    ' Bouton du menu principal à resélectionner lors du retour sur cette étape.
    Public ReadOnly Property MenuButton As Button

    ' Libellé de contexte hiérarchique à réafficher (ex : "Patients").
    Public ReadOnly Property Contexte As String

    ' État de filtre/recherche optionnel à restaurer (ex : critères de recherche patient).
    Public ReadOnly Property EtatFiltre As Object

#End Region

#Region "Constructeur"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version      : V1.0.0
    ' Date         : 11/06/2026
    '
    ' Rôle         :
    ' Initialise une étape de navigation rejouable.
    '
    ' Paramètres   :
    ' - creerVue   : Fabrique recréant la vue de l'étape (Func(Of UserControl), obligatoire)
    ' - menuButton : Bouton de menu à resélectionner au retour (Button)
    ' - contexte   : Libellé de contexte hiérarchique à réafficher (String)
    ' - etatFiltre : État de filtre/recherche optionnel à restaurer (Object, par défaut Nothing)
    '
    ' Exceptions   :
    ' - ArgumentNullException : si creerVue est Nothing (une étape non rejouable n'a pas de sens)
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(creerVue As Func(Of UserControl),
                   menuButton As Button,
                   contexte As String,
                   Optional etatFiltre As Object = Nothing)

        If creerVue Is Nothing Then
            Throw New ArgumentNullException(NameOf(creerVue))
        End If

        _CreerVue = creerVue
        _MenuButton = menuButton
        _Contexte = contexte
        _EtatFiltre = etatFiltre

    End Sub

#End Region

End Class
