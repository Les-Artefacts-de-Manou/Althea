' -------------------------------------------------------------------------------------------------
' UserControl : UC_StatutsSeance
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des statuts de séance (table ref_statuts_seance).
' Hérite de UC_ReferentielBase et fournit les métadonnées d'affichage et le branchement
' de la couche métier GestionStatutsSeance via les points d'extension.
'
' Remarques   :
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Aucun accès direct à la base de données : tout passe par GestionStatutsSeance.
' - Code long (varchar 30) avec espaces convertis en _ (comportement par défaut de la base).
'
' Dépendances :
' - UC_ReferentielBase (classe de base héritée)
' - GestionStatutsSeance (couche métier)
' - StatutSeance (modèle métier)
' - ReferentielLigne (modèle de présentation générique)
' - AppRole (gestion des droits)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_StatutsSeance
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Statuts de séance"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les statuts de séance (planifiée, réalisée, annulée, ...)."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Statuts de séance"
        End Get
    End Property

    Protected Overrides ReadOnly Property RoleMinimum As AppRole
        Get
            Return AppRole.SuperUser
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 30
        End Get
    End Property

#End Region

#Region "Données - branchement GestionStatutsSeance"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim statuts As List(Of StatutSeance) = GestionStatutsSeance.GetStatutsSeance(afficherInactifs)

        Return statuts.Select(
            Function(s) New ReferentielLigne With {
                .IdReferentiel = s.IdStatutSeance,
                .Code = s.CodeStatutSeance,
                .Libelle = s.LibelleStatutSeance,
                .OrdreAffichage = s.OrdreAffichage,
                .Actif = s.Actif
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionStatutsSeance.CodeStatutSeanceExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionStatutsSeance.LibelleStatutSeanceExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim statut As New StatutSeance With {
            .CodeStatutSeance = code,
            .LibelleStatutSeance = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionStatutsSeance.InsertStatutSeance(statut)

    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim statut As New StatutSeance With {
            .IdStatutSeance = id,
            .CodeStatutSeance = code,
            .LibelleStatutSeance = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionStatutsSeance.UpdateStatutSeance(statut)

    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            Dim ligne As StatutSeance = GestionStatutsSeance.GetStatutsSeance(True).
                FirstOrDefault(Function(s) s.IdStatutSeance = id)

            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionStatutsSeance.UpdateStatutSeance(ligne)
            End If
        Else
            GestionStatutsSeance.DesactiverStatutSeance(id)
        End If

    End Sub

#End Region

End Class
