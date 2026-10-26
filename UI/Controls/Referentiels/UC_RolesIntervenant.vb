' -------------------------------------------------------------------------------------------------
' UserControl : UC_RolesIntervenant
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des rôles d'intervenant (table ref_roles_intervenant).
' Hérite de UC_ReferentielBase et fournit les métadonnées d'affichage et le branchement
' de la couche métier GestionRolesIntervenant via les points d'extension.
'
' Remarques   :
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Aucun accès direct à la base de données : tout passe par GestionRolesIntervenant.
' - Code long (varchar 30) avec espaces convertis en _ (comportement par défaut de la base).
'
' Dépendances :
' - UC_ReferentielBase (classe de base héritée)
' - GestionRolesIntervenant (couche métier)
' - RoleIntervenant (modèle métier)
' - ReferentielLigne (modèle de présentation générique)
' - AppRole (gestion des droits)
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_RolesIntervenant
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_rolesIntervenant
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Rôles d'intervenant"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les rôles d'intervenant."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Rôles d'intervenant"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 30
        End Get
    End Property

#End Region

#Region "Données - branchement GestionRolesIntervenant"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim roles As List(Of RoleIntervenant) = GestionRolesIntervenant.GetRolesIntervenant(afficherInactifs)

        Return roles.Select(
            Function(r) New ReferentielLigne With {
                .IdReferentiel = r.IdRoleIntervenant,
                .Code = r.CodeRoleIntervenant,
                .Libelle = r.LibelleRoleIntervenant,
                .OrdreAffichage = r.OrdreAffichage,
                .Actif = r.Actif
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionRolesIntervenant.CodeRoleIntervenantExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionRolesIntervenant.LibelleRoleIntervenantExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim role As New RoleIntervenant With {
            .CodeRoleIntervenant = code,
            .LibelleRoleIntervenant = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionRolesIntervenant.InsertRoleIntervenant(_context?.ContexteOperation, role)

    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim role As New RoleIntervenant With {
            .IdRoleIntervenant = id,
            .CodeRoleIntervenant = code,
            .LibelleRoleIntervenant = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionRolesIntervenant.UpdateRoleIntervenant(_context?.ContexteOperation, role)

    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            Dim ligne As RoleIntervenant = GestionRolesIntervenant.GetRolesIntervenant(True).
                FirstOrDefault(Function(r) r.IdRoleIntervenant = id)

            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionRolesIntervenant.UpdateRoleIntervenant(_context?.ContexteOperation, ligne)
            End If
        Else
            GestionRolesIntervenant.DesactiverRoleIntervenant(_context?.ContexteOperation, id)
        End If

    End Sub

#End Region

End Class
