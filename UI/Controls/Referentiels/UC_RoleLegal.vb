' -------------------------------------------------------------------------------------------------
' UserControl : UC_RoleLegal
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 14/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des rôles légaux des contacts de l'entourage du patient
' (table ref_role_legal). Hérite de UC_ReferentielBase et se contente de fournir les
' métadonnées d'affichage et de brancher la couche métier GestionRoleLegal via les
' points d'extension.
'
' Responsabilités :
' - Définir le titre, le sous-titre et le fil d'Ariane propres aux rôles légaux
' - Charger les rôles légaux via GestionRoleLegal et les convertir en ReferentielLigne
' - Déléguer les vérifications d'unicité (code, libellé) et le CRUD à GestionRoleLegal
'
' Remarques   :
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Aucun accès direct à la base de données : tout passe par GestionRoleLegal.
' - Le code peut être long (varchar 30) avec des espaces remplacés automatiquement par des _
'   (comportement par défaut de UC_ReferentielBase, donc aucune surcharge nécessaire ici).
'
' Dépendances :
' - UC_ReferentielBase (classe de base héritée)
' - GestionRoleLegal (couche métier)
' - RoleLegal (modèle métier)
' - ReferentielLigne (modèle de présentation générique)
' - AppRole (gestion des droits)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_RoleLegal
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Rôles légaux"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les rôles légaux des contacts du patient (autorité parentale, représentant légal, ...)."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Rôles légaux"
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

#Region "Données - branchement GestionRoleLegal"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ChargerElements
    ' Rôle     : Charge les rôles légaux via GestionRoleLegal et les convertit en ReferentielLigne.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim roles As List(Of RoleLegal) = GestionRoleLegal.GetRolesLegaux(afficherInactifs)

        Return roles.Select(
            Function(r) New ReferentielLigne With {
                .IdReferentiel = r.IdRoleLegal,
                .Code = r.CodeRoleLegal,
                .Libelle = r.LibelleRoleLegal,
                .OrdreAffichage = r.OrdreAffichage,
                .Actif = r.Actif
            }).ToList()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CodeExisteDeja
    ' Rôle     : Vérifie l'unicité du code via GestionRoleLegal.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionRoleLegal.CodeRoleLegalExiste(code, idExclu)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LibelleExisteDeja
    ' Rôle     : Vérifie l'unicité du libellé via GestionRoleLegal.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionRoleLegal.LibelleRoleLegalExiste(libelle, idExclu)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InsererElement
    ' Rôle      : Insère un nouveau rôle légal via GestionRoleLegal.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim role As New RoleLegal With {
            .CodeRoleLegal = code,
            .LibelleRoleLegal = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionRoleLegal.InsertRoleLegal(role)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourElement
    ' Rôle      : Met à jour un rôle légal existant via GestionRoleLegal.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim role As New RoleLegal With {
            .IdRoleLegal = id,
            .CodeRoleLegal = code,
            .LibelleRoleLegal = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionRoleLegal.UpdateRoleLegal(role)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirActivation
    ' Rôle      : Active ou désactive un rôle légal (soft-delete) via GestionRoleLegal.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            ' Réactivation : on repasse Actif à 1 en conservant les autres valeurs.
            Dim ligne As RoleLegal = GestionRoleLegal.GetRolesLegaux(True).
                FirstOrDefault(Function(r) r.IdRoleLegal = id)

            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionRoleLegal.UpdateRoleLegal(ligne)
            End If
        Else
            ' Désactivation : soft-delete.
            GestionRoleLegal.DesactiverRoleLegal(id)
        End If

    End Sub

#End Region

End Class
