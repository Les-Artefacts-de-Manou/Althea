' -------------------------------------------------------------------------------------------------
' UserControl : UC_SituationsFamiliales
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des situations familiales (table ref_situations_familiales).
' Hérite de UC_ReferentielBase et fournit les métadonnées d'affichage et le branchement
' de la couche métier GestionSituationsFamiliales via les points d'extension.
'
' Remarques   :
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Aucun accès direct à la base de données : tout passe par GestionSituationsFamiliales.
' - Code long (varchar 50) avec espaces convertis en _ (comportement par défaut de la base).
'
' Dépendances :
' - UC_ReferentielBase (classe de base héritée)
' - GestionSituationsFamiliales (couche métier)
' - SituationFamiliale (modèle métier)
' - ReferentielLigne (modèle de présentation générique)
' - AppRole (gestion des droits)
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_SituationsFamiliales
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_situationsFamiliales
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Situations familiales"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les situations familiales"
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Situations familiales"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 50
        End Get
    End Property

#End Region

#Region "Données - branchement GestionSituationsFamiliales"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim situations As List(Of SituationFamiliale) = GestionSituationsFamiliales.GetSituationsFamiliales(afficherInactifs)

        Return situations.Select(
            Function(s) New ReferentielLigne With {
                .IdReferentiel = s.IdSituationFamiliale,
                .Code = s.CodeSituationFamiliale,
                .Libelle = s.LibelleSituationFamiliale,
                .OrdreAffichage = s.OrdreAffichage,
                .Actif = s.Actif
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionSituationsFamiliales.CodeSituationFamilialeExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionSituationsFamiliales.LibelleSituationFamilialeExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim situation As New SituationFamiliale With {
            .CodeSituationFamiliale = code,
            .LibelleSituationFamiliale = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionSituationsFamiliales.InsertSituationFamiliale(_context?.ContexteOperation, situation)

    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim situation As New SituationFamiliale With {
            .IdSituationFamiliale = id,
            .CodeSituationFamiliale = code,
            .LibelleSituationFamiliale = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionSituationsFamiliales.UpdateSituationFamiliale(_context?.ContexteOperation, situation)

    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            Dim ligne As SituationFamiliale = GestionSituationsFamiliales.GetSituationsFamiliales(True).
                FirstOrDefault(Function(s) s.IdSituationFamiliale = id)

            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionSituationsFamiliales.UpdateSituationFamiliale(_context?.ContexteOperation, ligne)
            End If
        Else
            GestionSituationsFamiliales.DesactiverSituationFamiliale(_context?.ContexteOperation, id)
        End If

    End Sub

#End Region

End Class
