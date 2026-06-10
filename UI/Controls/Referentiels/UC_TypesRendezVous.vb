' -------------------------------------------------------------------------------------------------
' UserControl : UC_TypesRendezVous
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des types de rendez-vous (table ref_types_rendez_vous).
' Hérite de UC_ReferentielBase et fournit les métadonnées d'affichage et le branchement
' de la couche métier GestionTypesRendezVous via les points d'extension.
'
' Remarques   :
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Aucun accès direct à la base de données : tout passe par GestionTypesRendezVous.
' - Code long (varchar 30) avec espaces convertis en _ (comportement par défaut de la base).
'
' Dépendances :
' - UC_ReferentielBase (classe de base héritée)
' - GestionTypesRendezVous (couche métier)
' - TypeRendezVous (modèle métier)
' - ReferentielLigne (modèle de présentation générique)
' - AppRole (gestion des droits)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_TypesRendezVous
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Types de rendez-vous"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les types de rendez-vous (première consultation, suivi, bilan, ...)."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Types de rendez-vous"
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

#Region "Données - branchement GestionTypesRendezVous"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim types As List(Of TypeRendezVous) = GestionTypesRendezVous.GetTypesRendezVous(afficherInactifs)

        Return types.Select(
            Function(t) New ReferentielLigne With {
                .IdReferentiel = t.IdTypeRendezVous,
                .Code = t.CodeTypeRendezVous,
                .Libelle = t.LibelleTypeRendezVous,
                .OrdreAffichage = t.OrdreAffichage,
                .Actif = t.Actif
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionTypesRendezVous.CodeTypeRendezVousExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionTypesRendezVous.LibelleTypeRendezVousExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim typeRdv As New TypeRendezVous With {
            .CodeTypeRendezVous = code,
            .LibelleTypeRendezVous = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionTypesRendezVous.InsertTypeRendezVous(typeRdv)

    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim typeRdv As New TypeRendezVous With {
            .IdTypeRendezVous = id,
            .CodeTypeRendezVous = code,
            .LibelleTypeRendezVous = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionTypesRendezVous.UpdateTypeRendezVous(typeRdv)

    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            Dim ligne As TypeRendezVous = GestionTypesRendezVous.GetTypesRendezVous(True).
                FirstOrDefault(Function(t) t.IdTypeRendezVous = id)

            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionTypesRendezVous.UpdateTypeRendezVous(ligne)
            End If
        Else
            GestionTypesRendezVous.DesactiverTypeRendezVous(id)
        End If

    End Sub

#End Region

End Class
