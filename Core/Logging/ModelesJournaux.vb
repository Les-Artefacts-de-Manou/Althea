' -------------------------------------------------------------------------------------------------
' Classes     : FichierJournal, EntreeJournal
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 13/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Modèles de lecture des journaux (LecteurJournaux → écran Journaux).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

''' <summary>Un fichier journal (un par jour).</summary>
Public Class FichierJournal
    Public Property Chemin As String
    Public Property Jour As DateTime
    Public Property TailleOctets As Long
End Class

''' <summary>Une entrée d'un journal : une ligne écrite par GestionLog, ou un en-tête de session.</summary>
Public Class EntreeJournal
    Public Property NumeroLigne As Integer
    Public Property Horodatage As DateTime?
    ''' <summary>Rapide, Succinct ou Complet ; vide pour un en-tête de session.</summary>
    Public Property Niveau As String
    ''' <summary>General, Startup, Database, UI, Process, Security ; vide pour un en-tête de session.</summary>
    Public Property Categorie As String
    Public Property Message As String
    ''' <summary>Message de l'exception jointe, vide s'il n'y en a pas.</summary>
    Public Property Exception As String = String.Empty
    ''' <summary>Exception, piles d'appels et causes, mises en forme pour la lecture.</summary>
    Public Property DetailTechnique As String = String.Empty
    ''' <summary>True pour la ligne « [SESSION] Démarrage application ».</summary>
    Public Property EstSession As Boolean

    Public ReadOnly Property AUneException As Boolean
        Get
            Return Not String.IsNullOrEmpty(Exception)
        End Get
    End Property
End Class
