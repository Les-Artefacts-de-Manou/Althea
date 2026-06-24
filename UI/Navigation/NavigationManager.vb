' -------------------------------------------------------------------------------------------------
' Classe     : NavigationManager
' Projet     : Althéa
' Version    : V1.1
' Date       : 01/05/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Gestionnaire centralisé de la navigation entre les UserControls dans la Form principale Home.
' Gère le chargement dynamique des UserControls dans pnlContent avec injection automatique
' du contexte UI partagé (UserControlContext) pour les UserControls implémentant IContextAwareUserControl.
'
' Responsabilités :
' - Charger dynamiquement un UserControl dans le Panel central (pnlContent)
' - Décharger et libérer proprement les UserControls précédents (Dispose)
' - Injecter automatiquement le contexte UI (_context) via IContextAwareUserControl.SetContext()
' - Garantir que le UserControl chargé occupe tout l''espace disponible (Dock = Fill)
' - Assurer une navigation cohérente et sans fuite mémoire
'
' Remarques  :
' - Classe utilitaire instanciée dans Home.Load après authentification réussie
' - Utilisée exclusivement par Home via NavigateTo() et ses dérivées (NavigateToAccueil, NavigateToAdminView)
' - Ne contient aucune logique métier ni accès base de données
' - L''injection du contexte se fait AVANT l''ajout au Panel pour éviter les effets de bord
' - Tous les anciens contrôles sont explicitement Dispose() avant Clear() pour libérer les ressources
' - Le Panel cible (_pnlContent) et le contexte (_context) sont ReadOnly et fournis au constructeur
'
' Dépendances :
' - System.Windows.Forms (Panel, UserControl, Control, DockStyle)
' - UserControlContext (contexte UI partagé)
' - IContextAwareUserControl (interface pour injection du contexte)
'
' Imports    :
' - 
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class NavigationManager

#Region "Variables privées"

    ' Panel central de Home (pnlContent) dans lequel les UserControls sont chargés dynamiquement.
    Private ReadOnly _pnlContent As Panel

    ' Contexte UI partagé injecté automatiquement aux UserControls implémentant IContextAwareUserControl.
    Private ReadOnly _context As UserControlContext

    ' Historique de navigation (mini-pile Push/Pop, D-Q15) : chaque entrée sait recréer sa vue
    ' et porte le contexte + le dernier filtre à restaurer lors d'un retour en arrière.
    Private ReadOnly _historique As New Stack(Of NavigationEntry)

#End Region

#Region "Constructeur"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version      : V1.1.0
    ' Date         : 01/05/2026
    '
    ' Rôle         :
    ' Initialise le NavigationManager avec le Panel central et le contexte UI partagé.
    '
    ' Paramètres   :
    ' - pnlContent : Panel central de Home (pnlContent) où les UserControls seront chargés (Panel)
    ' - context    : Contexte UI partagé à injecter dans les UserControls (UserControlContext)
    '
    ' Remarques    :
    ' - Appelé dans Home.Load après création du contexte UI (_uiContext)
    ' - Les deux paramètres sont obligatoires et stockés dans des variables ReadOnly
    ' - Aucune validation : suppose que les paramètres fournis sont valides (responsabilité de Home)
    ' - L'instance de NavigationManager est créée une seule fois et réutilisée pendant toute la durée de vie de Home
    '
    ' Exceptions   :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(pnlContent As Panel, context As UserControlContext)
        _pnlContent = pnlContent
        _context = context
    End Sub

#End Region

#Region "Navigation"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Navigate
    ' Version    : V1.0.0
    ' Date       : 26/04/2026
    '
    ' Rôle       :
    ' Charge dynamiquement un UserControl dans le Panel central (_pnlContent) avec injection
    ' automatique du contexte UI pour les UserControls implémentant IContextAwareUserControl.
    '
    ' Paramètres :
    ' - view : UserControl à charger dans le Panel central (UserControl)
    '
    ' Remarques  :
    ' - Séquence de navigation :
    '   1. Vérification : si view est Nothing, retourne sans action
    '   2. Injection du contexte : TryCast vers IContextAwareUserControl, appel SetContext(_context) si implémenté
    '   3. Libération des anciens contrôles : Dispose() explicite de tous les contrôles enfants de _pnlContent
    '   4. Nettoyage : Controls.Clear() pour vider la collection
    '   5. Configuration du nouveau contrôle : Dock = Fill pour occuper tout l''espace
    '   6. Ajout au Panel : Controls.Add(view)
    ' - L''injection du contexte se fait AVANT l''ajout au Panel pour éviter les événements Load sans contexte
    ' - Dispose() explicite prévient les fuites mémoire en libérant les ressources des UserControls précédents
    ' - Un seul UserControl actif à la fois dans _pnlContent (navigation exclusive)
    ' - Appelée par Home.NavigateTo() qui gère également la synchronisation menu/contexte/statut
    '
    ' Exceptions :
    ' - Aucune gestion explicite (erreurs propagées à Home)
    ' -------------------------------------------------------------------------------------------------
    Public Sub Navigate(view As UserControl)

        If view Is Nothing Then
            Return
        End If

        ' Injection du contexte si le UserControl le supporte
        Dim contextAwareControl As IContextAwareUserControl =
        TryCast(view, IContextAwareUserControl)

        If contextAwareControl IsNot Nothing Then
            contextAwareControl.SetContext(_context)
        End If

        ' Libération des anciens contrôles
        For Each ctrl As Control In _pnlContent.Controls

            ctrl.Dispose()

        Next

        _pnlContent.Controls.Clear()

        ' Chargement du nouveau contrôle
        view.Dock = DockStyle.Fill
        _pnlContent.Controls.Add(view)

    End Sub

#End Region

#Region "Historique (mini-pile de navigation, D-Q15)"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : CanNavigateBack
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Type       : Boolean (ReadOnly)
    '
    ' Rôle       :
    ' Indique s'il existe une étape précédente vers laquelle revenir.
    '
    ' Retour     :
    ' - True : si l'historique contient au moins une étape antérieure (Pop possible)
    ' - False : si l'historique est vide (on est sur l'écran d'entrée)
    '
    ' Remarques  :
    ' - Utilisée par Home pour activer/désactiver un éventuel bouton "Retour"
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property CanNavigateBack As Boolean
        Get
            Return _historique.Count > 0
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : NavigateAndPush
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Empile l'étape COURANTE (pour pouvoir y revenir) puis navigue vers une nouvelle vue.
    '
    ' Paramètres :
    ' - nouvelleVue   : UserControl déjà instancié à charger immédiatement (UserControl)
    ' - etapeCourante : Étape décrivant l'écran actuel (fabrique + menu + contexte + filtre) à
    '                   empiler pour un retour ultérieur (NavigationEntry)
    '
    ' Remarques  :
    ' - etapeCourante.CreerVue doit recréer l'écran courant À L'IDENTIQUE (filtre restauré par closure)
    '   car la vue courante est libérée (Dispose) lors du chargement de nouvelleVue
    ' - Si etapeCourante est Nothing, la navigation a lieu sans empilement (équivaut à Navigate)
    ' - La nouvelle vue n'est PAS empilée ici : elle le sera lors de la prochaine NavigateAndPush
    ' -------------------------------------------------------------------------------------------------
    Public Sub NavigateAndPush(nouvelleVue As UserControl, etapeCourante As NavigationEntry)

        If etapeCourante IsNot Nothing Then
            _historique.Push(etapeCourante)
        End If

        Navigate(nouvelleVue)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : NavigateBack
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Dépile l'étape précédente, recrée sa vue et la charge, puis retourne l'étape restaurée.
    '
    ' Retour     :
    ' - NavigationEntry : l'étape restaurée (pour que Home resélectionne le menu et le contexte),
    '                     ou Nothing si l'historique est vide
    '
    ' Remarques  :
    ' - La vue est RECRÉÉE via la fabrique (CreerVue) car l'instance d'origine a été libérée
    ' - Home est responsable de restaurer le menu sélectionné et le contexte à partir de l'étape rendue
    ' - Ne lève pas d'exception si l'historique est vide (retourne Nothing)
    ' -------------------------------------------------------------------------------------------------
    Public Function NavigateBack() As NavigationEntry

        If _historique.Count = 0 Then
            Return Nothing
        End If

        Dim etapePrecedente As NavigationEntry = _historique.Pop()

        Dim vue As UserControl = etapePrecedente.CreerVue.Invoke()
        Navigate(vue)

        Return etapePrecedente

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ClearHistory
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Rôle       :
    ' Vide l'historique de navigation (réinitialise la mini-pile).
    '
    ' Remarques  :
    ' - À appeler lors d'une navigation "racine" (clic sur un bouton du menu principal) pour repartir
    '   d'un historique propre et éviter d'accumuler des chaînes de retour incohérentes
    ' -------------------------------------------------------------------------------------------------
    Public Sub ClearHistory()
        _historique.Clear()
    End Sub

#End Region

End Class
