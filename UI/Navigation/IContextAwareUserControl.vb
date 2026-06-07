' -------------------------------------------------------------------------------------------------
' Interface  : IContextAwareUserControl
' Projet     : Althéa
' Version    : V1.0
' Date       : 01/05/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Définit le contrat permettant à un UserControl de recevoir le contexte UI global (UserControlContext)
' fourni par Home via NavigationManager lors du chargement dynamique.
'
' Responsabilités :
' - Déclarer la méthode SetContext() pour injection du contexte UI
' - Permettre aux UserControls d'accéder aux composants partagés sans référencer Home directement
'
' Remarques  :
' - Implémentée par tous les UserControls chargés dynamiquement dans Home.pnlContent
' - Exemples d'implémentations : UC_Accueil, UC_AdminHome, UC_Parametres, ConfigurationConnexion
' - SetContext() est appelée automatiquement par NavigationManager.Navigate() avant l'ajout au Panel
' - Le contexte fourni contient : StatusStrip, ToolTip, ErrorProvider, lblContexte, UserSession
' - Permet aux UserControls d'afficher des messages de statut, erreurs, infobulles via _context
' - Alternative à la propagation de références : découplage entre Home et les UserControls
'
' Dépendances :
' - UserControlContext (classe injectée via SetContext)
' - NavigationManager (responsable de l'injection)
'
' Imports    :
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Public Interface IContextAwareUserControl

#Region "Méthodes"

    ' -------------------------------------------------------------------------------------------------
    ' Méthode    : SetContext
    ' Version    : V1.0.0
    ' Date       : 01/05/2026
    '
    ' Rôle       :
    ' Injecte le contexte UI partagé (UserControlContext) dans le UserControl implémentant l'interface.
    '
    ' Paramètres :
    ' - context : Instance de UserControlContext contenant les composants UI partagés (UserControlContext)
    '
    ' Remarques  :
    ' - Appelée automatiquement par NavigationManager.Navigate() lors du chargement du UserControl
    ' - L'injection se fait AVANT l'ajout du UserControl au Panel pour garantir la disponibilité du contexte
    ' - Les implémentations typiques stockent context dans une variable privée (_context)
    ' - Permet d'accéder à : stsLabelStatus, ttMain, errProvider, lblContexte, UserSession
    ' - Après injection, le UserControl peut appeler _context.SetStatus(), _context.SetError(), etc.
    '
    ' Exceptions :
    ' - Aucune spécification (responsabilité de l'implémentation)
    ' -------------------------------------------------------------------------------------------------

    Sub SetContext(context As UserControlContext)

#End Region

End Interface