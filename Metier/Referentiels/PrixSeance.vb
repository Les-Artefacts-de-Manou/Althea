' -------------------------------------------------------------------------------------------------
' Classe   : PrixSeance
' Projet   : Althéa
' Version  : V1.0.1
' Date     : 25/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un tarif de séance issu de la table référentielle ref_prix_seance
' (ex. Tarif patient 50 €, Tarif patient 60 €, Séance INAMI).
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB et l'interface de gestion du référentiel.
' - id_prix_seance est généré par la base (AUTO_INCREMENT).
' - code_prix_seance est saisi par l'utilisateur (majuscules, espaces remplacés par _) et doit être unique.
' - PARTICULARITÉ : possède un montant (montant decimal(10,2) NOT NULL). Ce montant est la source
'   lue à la création d'une séance puis FIGÉE dans seances.tarif_seance (snapshot) : modifier ce
'   montant n'affecte jamais les séances déjà comptées.
' - V1.0.1 (25/10/2026, audit D-1) : Option Strict On.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class PrixSeance

#Region "Propriétés"

    ' Identifiant unique du tarif de séance (clé primaire, généré par la base)
    Public Property IdPrixSeance As ULong

    ' Code du tarif de séance (majuscules, espaces remplacés par _, unique)
    Public Property CodePrixSeance As String

    ' Libellé affiché dans l'interface utilisateur (unique)
    Public Property LibellePrixSeance As String

    ' Montant courant du tarif (decimal(10,2), NOT NULL) — figé sur la séance à sa création
    Public Property Montant As Decimal

    ' Indique si le tarif de séance est actif (0 = désactivé, 1 = actif)
    Public Property Actif As Boolean

    ' Ligne système (est_systeme) : code utilisé par l'application, non modifiable ni désactivable
    Public Property EstSysteme As Boolean

    ' Ordre d'affichage dans l'interface utilisateur
    Public Property OrdreAffichage As Integer

    ' Propriété calculée pour afficher le montant formaté dans les combos
    Public ReadOnly Property MontantAffichage As String
        Get
            If IdPrixSeance = 0 Then
                Return String.Empty
            End If
            Return $"{LibellePrixSeance} ({Montant:F2} €)"
        End Get
    End Property

#End Region

End Class
