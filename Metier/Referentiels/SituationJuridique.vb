' -------------------------------------------------------------------------------------------------
' Classe      : SituationJuridique
' Projet      : Althéa
' Version     : V1.0.0
'
' Rôle        :
' Modèle d'une situation juridique du cabinet (table ref_situations_juridiques).
' Le comportement (porteur du numéro BCE, plus tard la facturation) dépend uniquement des
' attributs de la ligne, jamais de son code.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class SituationJuridique

    Public Const PORTEUR_CABINET As String = "CABINET"
    Public Const PORTEUR_PRATICIEN As String = "PRATICIEN"

    Public Property IdSituationJuridique As ULong

    Public Property CodeSituationJuridique As String

    Public Property LibelleSituationJuridique As String

    ' Entité qui porte le numéro BCE : CABINET (cab_cabinet.numero_bce) ou PRATICIEN (identifiant BCE du praticien).
    Public Property PorteurBce As String = PORTEUR_CABINET

    Public Property Actif As Boolean

    Public Property EstSysteme As Boolean

    Public Property OrdreAffichage As Integer

    Public ReadOnly Property BceParPraticien As Boolean
        Get
            Return String.Equals(PorteurBce, PORTEUR_PRATICIEN, StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

End Class
