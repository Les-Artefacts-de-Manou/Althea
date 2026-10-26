' -------------------------------------------------------------------------------------------------
' Classes     : InfoArchive, ResultatVerification, ResultatRestauration, RestaurationJournal
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 13/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Modèles échangés entre GestionRestaurations et l'écran de restauration (lot 30, étape P3).
' L'avancement réutilise ProgressionSauvegarde.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

''' <summary>Contenu du manifeste d'une archive, lu et vérifié avec le mot de passe.</summary>
Public Class InfoArchive
    Public Property CheminArchive As String
    Public Property NomArchive As String
    Public Property DateSauvegarde As DateTime?
    Public Property VersionSchema As String
    Public Property VersionApplication As String
    Public Property NomBase As String
    Public Property Machine As String
    Public Property Utilisateur As String
    Public Property NbFichiers As Integer
    Public Property TailleDonneesOctets As Long
End Class

''' <summary>Résultat de « Vérifier l'archive » : manifeste lu (ou erreur de lecture) et empêchements.</summary>
Public Class ResultatVerification
    ''' <summary>Contenu du manifeste ; Nothing si l'archive n'a pas pu être lue.</summary>
    Public Property Info As InfoArchive
    ''' <summary>Message si l'archive n'a pas pu être lue (mot de passe, fichier), sinon Nothing.</summary>
    Public Property ErreurLecture As String
    ''' <summary>Empêchements relevés par les contrôles (vide = restauration possible).</summary>
    Public Property Refus As New List(Of String)

    Public ReadOnly Property RestaurationPossible As Boolean
        Get
            Return Info IsNot Nothing AndAlso ErreurLecture Is Nothing AndAlso Refus.Count = 0
        End Get
    End Property
End Class

Public Class ResultatRestauration
    Public Property Reussie As Boolean
    ''' <summary>Message destiné à l'utilisateur (sans détail technique ni donnée sensible).</summary>
    Public Property Message As String
    ''' <summary>Sauvegarde de sécurité faite juste avant (chemin complet), si elle a eu lieu.</summary>
    Public Property ArchiveSecurite As String
    ''' <summary>Ancien dossier de données, renommé et conservé, après une restauration réussie.</summary>
    Public Property DossierDonneesPrecedent As String
    ''' <summary>True si la base a été modifiée : l'application doit redémarrer.</summary>
    Public Property RedemarrageNecessaire As Boolean
End Class

Public Class RestaurationJournal
    Public Property DateDebut As DateTime
    Public Property Statut As StatutRestauration
    Public Property NomArchive As String
    Public Property DateArchive As DateTime?
    Public Property NbFichiers As Integer?
    Public Property ArchiveSecurite As String
    Public Property DossierDonneesPrecedent As String
    Public Property LoginUtilisateur As String
    Public Property MessageErreur As String

    Public ReadOnly Property LibelleStatut As String
        Get
            Return StatutsRestauration.Libelle(Statut)
        End Get
    End Property
End Class
