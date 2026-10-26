' -------------------------------------------------------------------------------------------------
' Interface  : IUnsavedChangesAware
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 08/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Contrat optionnel permettant à un UserControl chargé dans Home de bloquer la navigation (ou la
' fermeture de l'application) tant que des modifications ne sont pas enregistrées.
'
' Remarques  :
' - Interrogé par NavigationManager.PeutQuitterVueCourante AVANT tout déchargement de la vue.
' - PeutQuitter retourne False pour annuler la navigation (l'implémentation affiche la confirmation).
' -------------------------------------------------------------------------------------------------
Option Strict On
Option Explicit On

Public Interface IUnsavedChangesAware

    Function PeutQuitter() As Boolean

End Interface
