' -------------------------------------------------------------------------------------------------
' UserControl : UC_Specialites
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Référentiel des spécialités (ref_specialites) proposées dans la fiche Thérapeute : code, libellé,
' ordre, activation.
'
' Remarques   :
' - Hérite de UC_ReferentielBase ; aucun accès direct à la base : GestionSpecialites.
' - Décisions de Joëlle (23/10/2026) : la spécialité n'est plus tapée librement (migration v2.55.0).
' - Pas de suppression : une spécialité désactivée n'est plus proposée, mais reste affichée chez
'   les thérapeutes qui l'ont.
' - Icône de titre provisoire (celle des rôles d'intervenant), en attendant une icône dédiée.
'
' Dépendances :
' - UC_ReferentielBase, GestionSpecialites, Specialite
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_Specialites
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_rolesIntervenant
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Spécialités"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Spécialités proposées dans la fiche Thérapeute."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Spécialités"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 30
        End Get
    End Property

#End Region

#Region "Données - branchement GestionSpecialites"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Return GestionSpecialites.GetSpecialites(afficherInactifs).Select(
            Function(s) New ReferentielLigne With {
                .IdReferentiel = s.IdSpecialite,
                .Code = s.CodeSpecialite,
                .Libelle = s.LibelleSpecialite,
                .OrdreAffichage = s.OrdreAffichage,
                .Actif = s.Actif
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionSpecialites.CodeSpecialiteExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionSpecialites.LibelleSpecialiteExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)
        GestionSpecialites.InsertSpecialite(_context?.ContexteOperation, New Specialite With {
            .CodeSpecialite = code,
            .LibelleSpecialite = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        })
    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)
        GestionSpecialites.UpdateSpecialite(_context?.ContexteOperation, New Specialite With {
            .IdSpecialite = id,
            .CodeSpecialite = code,
            .LibelleSpecialite = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        })
    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)
        GestionSpecialites.DefinirActivation(_context?.ContexteOperation, id, actif)
    End Sub

#End Region

End Class
