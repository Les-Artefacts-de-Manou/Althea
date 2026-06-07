' -------------------------------------------------------------------------------------------------
' Interface  : IContextAwareForm
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 16/05/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Définit le contrat permettant à une Form modale de recevoir le contexte UI global (UserControlContext)
' fourni par Home avant l'ouverture via ShowDialog().
'
' Responsabilités :
' - Déclarer la méthode SetContext() pour injection du contexte UI
' - Permettre aux Forms modales d'accéder aux composants partagés sans référencer Home directement
'
' Remarques  :
' - Implémentée par les Forms modales ouvertes depuis Home ou depuis les UserControls
' - Exemples d'implémentations : ConfigurationConnexion, ElevationAcces (potentiellement)
' - SetContext() est appelée manuellement avant ShowDialog() par le code appelant
' - Le contexte fourni contient : StatusStrip, ToolTip, ErrorProvider, lblContexte, UserSession
' - Permet aux Forms modales d'afficher des messages de statut, erreurs, infobulles via _context
' - Différence avec IContextAwareUserControl : injection manuelle vs automatique par NavigationManager
' - Alternative au passage de Home comme paramètre constructeur : découplage et injection de dépendance
'
' Dépendances :
' - UserControlContext (classe injectée via SetContext)
'
' Imports    :
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Public Interface IContextAwareForm

#Region "Méthodes"

    ' -------------------------------------------------------------------------------------------------
    ' Méthode    : SetContext
    ' Version    : V1.0.0
    ' Date       : 16/05/2026
    '
    ' Rôle       :
    ' Injecte le contexte UI partagé (UserControlContext) dans la Form implémentant l'interface.
    '
    ' Paramètres :
    ' - context : Instance de UserControlContext contenant les composants UI partagés (UserControlContext)
    '
    ' Remarques  :
    ' - Appelée manuellement par le code appelant AVANT l'appel à ShowDialog()
    ' - Pattern typique : form.SetContext(_context) puis form.ShowDialog(Me)
    ' - Les implémentations typiques stockent context dans une variable privée (_context)
    ' - Permet d'accéder à : stsLabelStatus, ttMain, errProvider, lblContexte, UserSession
    ' - Après injection, la Form peut appeler _context.SetStatus(), _context.SetError(), etc.
    ' - Différence avec IContextAwareUserControl : injection manuelle (appelant) vs automatique (NavigationManager)
    '
    ' Exceptions :
    ' - Aucune spécification (responsabilité de l'implémentation)
    ' -------------------------------------------------------------------------------------------------
    Sub SetContext(
        context As UserControlContext
    )

#End Region

End Interface