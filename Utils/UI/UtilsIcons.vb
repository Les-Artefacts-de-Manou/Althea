' -------------------------------------------------------------------------------------------------
' Module      : UtilsIcons
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 03/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Classe utilitaire centralisant l'accès aux icônes d'état de l'application.
'
' Responsabilités :
' - Fournir un accès centralisé aux icônes d'état (OK, OFF, Lock)
' - Charger les icônes depuis My.Resources
' - Supposer que les icônes sont disponibles en différentes tailles (16x16, 26x26, 32x32)
' - Éviter les chargements multiples depuis les fichiers
'
' Remarques   :
' - Les icônes sont chargées via My.Resources 
' - Les Fonctions sont Shared pour un accès global sans instanciation
' - Utilisé par UC_Utilisateurs, UC_Parametres et autres contrôles affichant des états
'
' Dépendances :
' - My.Resources (ressources embarquées de l'application)
' - System.Drawing (Image)
'
' Usage :
' - UtilsIcons.IconOK : Icône pour état actif/valide
' - UtilsIcons.IconOFF : Icône pour état inactif/désactivé
' - UtilsIcons.IconLock : Icône pour compte verrouillé
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UtilsIcons

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : IconOK (Shared)
    ' Version   : V1.0.0
    ' Date      : 03/06/2026
    '
    ' Rôle      :
    ' Retourne l'icône représentant un état actif/valide (OK_32x32).

    'Paramètres :
    '- size (Integer, optionnel) : Taille de l'icône souhaitée (16, 20, 26, 32). Par défaut : 32.
    '
    ' Retour    :
    ' - Image : Icône OK (vert)
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function IconOK(Optional size As Integer = 32) As Image
        Select Case size
            Case 16
                Return My.Resources.OK_16x16
            Case 20
                Return My.Resources.OK_20x20
            Case 26
                Return My.Resources.OK_26x26
            Case 32
                Return My.Resources.OK_32x32
            Case Else
                ' Taille par défaut si valeur non supportée
                Return My.Resources.OK_32x32
        End Select
    End Function


    ' -------------------------------------------------------------------------------------------------
    ' Fonction : IconOFF (Shared)
    ' Version   : V1.0.0
    ' Date      : 03/06/2026
    '
    ' Rôle      :
    ' Retourne l'icône représentant un état inactif/désactivé (OFF_32x32).

    ' Paramètres :
    ' - size (Integer, optionnel) : Taille de l'icône souhaitée (16, 20, 26, 32). Par défaut : 32.  
    '
    ' Retour    :
    ' - Image : Icône OFF (rouge/gris)
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function IconOFF(Optional size As Integer = 32) As Image
        Select Case size
            Case 16
                Return My.Resources.OFF_16x16
            Case 20
                Return My.Resources.OFF_20x20
            Case 26
                Return My.Resources.OFF_26x26
            Case 32
                Return My.Resources.OFF_32x32
            Case Else
                Return My.Resources.OFF_32x32
        End Select
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : IconLock (Shared)
    ' Version   : V1.0.0
    ' Date      : 03/06/2026
    '
    ' Rôle      :
    ' Retourne l'icône représentant un compte verrouillé (Lock_32x32).

    ' Paramètres :
    ' - size (Integer, optionnel) : Taille de l'icône souhaitée (16, 20, 26, 32). Par défaut : 32.
    '
    ' Retour    :
    ' - Image : Icône Lock (cadenas)
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function IconLock(Optional size As Integer = 32) As Image
        Select Case size
            Case 16
                Return My.Resources.Lock_16x16
            Case 20
                Return My.Resources.Lock_20x20
            Case 26
                Return My.Resources.Lock_26x26
            Case 32
                Return My.Resources.Lock_32x32
            Case Else
                Return My.Resources.Lock_32x32
        End Select
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : IconLock (Shared)
    ' Version   : V1.0.0
    ' Date      : 03/06/2026
    '
    ' Rôle      :
    ' Retourne l'icône représentant un compte verrouillé (Lock_32x32).

    ' Paramètres :
    ' - size (Integer, optionnel) : Taille de l'icône souhaitée (16, 20, 26, 32). Par défaut : 32.
    '
    ' Retour    :
    ' - Image : Icône Lock (cadenas)
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function IconNo(Optional size As Integer = 32) As Image
        Select Case size
            Case 16
                Return My.Resources.NO_16x16
            Case 20
                Return My.Resources.NO_20x20
            Case 26
                Return My.Resources.NO_26x26
            Case 32
                Return My.Resources.NO_32x32
            Case Else
                Return My.Resources.NO_32x32
        End Select
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : IconPatientEnCours (Shared)
    ' Version   : V1.0.0
    ' Date      : 19/07/2026
    '
    ' Rôle      :
    ' Retourne l'icône représentant un patient dont le suivi est en cours (patientEncours_20).
    '
    ' Retour    :
    ' - Image : Icône « suivi en cours » (20x20)
    '
    ' Remarques :
    ' - Disponible uniquement en 20x20 (icône d'état pour la liste des patients).
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function IconPatientEnCours() As Image
        Return My.Resources.patientEncours_20
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : IconPatientNonEnCours (Shared)
    ' Version   : V1.0.0
    ' Date      : 19/07/2026
    '
    ' Rôle      :
    ' Retourne l'icône représentant un patient dont le suivi est clôturé/archivé (patientNonEncours_20).
    '
    ' Retour    :
    ' - Image : Icône « suivi clôturé / archivé » (20x20)
    '
    ' Remarques :
    ' - Disponible uniquement en 20x20 (icône d'état pour la liste des patients).
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function IconPatientNonEnCours() As Image
        Return My.Resources.patientNonEncours_20
    End Function

End Class