' -------------------------------------------------------------------------------------------------
' UserControl : UC_TypesDocuments
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des types de document (table ref_types_documents).
' Hérite de UC_ReferentielBase et fournit les métadonnées d'affichage et le branchement
' de la couche métier GestionTypesDocuments via les points d'extension.
'
' Remarques   :
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Aucun accès direct à la base de données : tout passe par GestionTypesDocuments.
' - Code long (varchar 30) avec espaces convertis en _ (comportement par défaut de la base).
'
' Dépendances :
' - UC_ReferentielBase (classe de base héritée)
' - GestionTypesDocuments (couche métier)
' - TypeDocument (modèle métier)
' - ReferentielLigne (modèle de présentation générique)
' - AppRole (gestion des droits)
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_TypesDocuments
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_typesDocuments
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Types de document"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les types de document."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Types de document"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 30
        End Get
    End Property

#End Region

#Region "Données - branchement GestionTypesDocuments"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim types As List(Of TypeDocument) = GestionTypesDocuments.GetTypesDocuments(afficherInactifs)

        Return types.Select(
            Function(t) New ReferentielLigne With {
                .IdReferentiel = t.IdTypeDocument,
                .Code = t.CodeTypeDocument,
                .Libelle = t.LibelleTypeDocument,
                .OrdreAffichage = t.OrdreAffichage,
                .Actif = t.Actif,
                .EstSysteme = t.EstSysteme
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionTypesDocuments.CodeTypeDocumentExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionTypesDocuments.LibelleTypeDocumentExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim typeDoc As New TypeDocument With {
            .CodeTypeDocument = code,
            .LibelleTypeDocument = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionTypesDocuments.InsertTypeDocument(_context?.ContexteOperation, typeDoc)

    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim typeDoc As New TypeDocument With {
            .IdTypeDocument = id,
            .CodeTypeDocument = code,
            .LibelleTypeDocument = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionTypesDocuments.UpdateTypeDocument(_context?.ContexteOperation, typeDoc)

    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            Dim ligne As TypeDocument = GestionTypesDocuments.GetTypesDocuments(True).
                FirstOrDefault(Function(t) t.IdTypeDocument = id)

            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionTypesDocuments.UpdateTypeDocument(_context?.ContexteOperation, ligne)
            End If
        Else
            GestionTypesDocuments.DesactiverTypeDocument(_context?.ContexteOperation, id)
        End If

    End Sub

#End Region

End Class
