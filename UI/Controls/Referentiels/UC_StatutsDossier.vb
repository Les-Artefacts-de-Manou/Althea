' -------------------------------------------------------------------------------------------------
' UserControl : UC_StatutsDossier
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des statuts de dossier (table ref_statuts_dossier).
' Hérite de UC_ReferentielBase et fournit les métadonnées d'affichage et le branchement
' de la couche métier GestionStatutsDossier via les points d'extension.
'
' Remarques   :
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Aucun accès direct à la base de données : tout passe par GestionStatutsDossier.
' - Code long (varchar 30) avec espaces convertis en _ (comportement par défaut de la base).
'
' Dépendances :
' - UC_ReferentielBase (classe de base héritée)
' - GestionStatutsDossier (couche métier)
' - StatutDossier (modèle métier)
' - ReferentielLigne (modèle de présentation générique)
' - AppRole (gestion des droits)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_StatutsDossier
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Statuts de dossier"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les statuts de dossier (en cours, clôturé, en attente, ...)."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Statuts de dossier"
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

#Region "Données - branchement GestionStatutsDossier"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim statuts As List(Of StatutDossier) = GestionStatutsDossier.GetStatutsDossier(afficherInactifs)

        Return statuts.Select(
            Function(s) New ReferentielLigne With {
                .IdReferentiel = s.IdStatutDossier,
                .Code = s.CodeStatutDossier,
                .Libelle = s.LibelleStatutDossier,
                .OrdreAffichage = s.OrdreAffichage,
                .Actif = s.Actif
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionStatutsDossier.CodeStatutDossierExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionStatutsDossier.LibelleStatutDossierExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim statut As New StatutDossier With {
            .CodeStatutDossier = code,
            .LibelleStatutDossier = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionStatutsDossier.InsertStatutDossier(statut)

    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim statut As New StatutDossier With {
            .IdStatutDossier = id,
            .CodeStatutDossier = code,
            .LibelleStatutDossier = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionStatutsDossier.UpdateStatutDossier(statut)

    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            Dim ligne As StatutDossier = GestionStatutsDossier.GetStatutsDossier(True).
                FirstOrDefault(Function(s) s.IdStatutDossier = id)

            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionStatutsDossier.UpdateStatutDossier(ligne)
            End If
        Else
            GestionStatutsDossier.DesactiverStatutDossier(id)
        End If

    End Sub

#End Region

End Class
