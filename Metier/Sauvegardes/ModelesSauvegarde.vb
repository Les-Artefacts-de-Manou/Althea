' -------------------------------------------------------------------------------------------------
' Classes     : SauvegardeJournal, ProgressionSauvegarde, ResultatSauvegarde
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 12/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Modèles échangés entre GestionSauvegardes et l'écran Sauvegardes : une ligne du journal
' (tec_sauvegardes), l'avancement d'une sauvegarde en cours, le résultat d'une sauvegarde.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

''' <summary>Une ligne du journal des sauvegardes (tec_sauvegardes).</summary>
Public Class SauvegardeJournal
    Public Property IdSauvegarde As ULong
    Public Property DateDebut As DateTime
    Public Property DateFin As DateTime?
    Public Property Statut As StatutSauvegarde
    Public Property NomFichier As String
    Public Property DossierSauvegarde As String
    Public Property TailleOctets As Long?
    Public Property NbFichiers As Integer?
    Public Property VersionSchema As String
    Public Property LoginUtilisateur As String
    Public Property Machine As String
    Public Property MessageErreur As String

    Public ReadOnly Property LibelleStatut As String
        Get
            Return StatutsSauvegarde.Libelle(Statut)
        End Get
    End Property
End Class

''' <summary>Avancement d'une sauvegarde en cours (étape affichée et pourcentage 0-100).</summary>
Public Class ProgressionSauvegarde
    Public Property Etape As String
    Public Property Pourcentage As Integer
End Class

''' <summary>Résultat d'une sauvegarde : réussite, archive produite ou message d'échec.</summary>
Public Class ResultatSauvegarde
    Public Property Reussie As Boolean
    Public Property CheminArchive As String
    Public Property TailleOctets As Long
    Public Property NbFichiers As Integer
    Public Property NbArchivesSupprimees As Integer
    ''' <summary>Message destiné à l'utilisateur (sans détail technique ni donnée sensible).</summary>
    Public Property Message As String
End Class
