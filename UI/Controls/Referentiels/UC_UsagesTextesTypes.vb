' -------------------------------------------------------------------------------------------------
' UserControl : UC_UsagesTextesTypes
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Référentiel des usages des textes types (ref_usages_textes_types) : les éditeurs de texte où un
' texte type peut être proposé. On y change le libellé et l'ordre (liste à cocher de l'écran
' Textes types, colonne Usages).
'
' Remarques   :
' - Hérite de UC_ReferentielBase ; aucun accès direct à la base : GestionTextesTypes.
' - Pas de création ici (CreationPermise = False) : un usage n'a de sens que si un éditeur le
'   demande ; il naît par migration avec la constante UsagesTexteType (Reference_UI § 7.23).
' - Lignes système : le code (repris par l'application) et l'activation sont verrouillés par la
'   base ; libellé et ordre restent libres.
' - Décision de Joëlle (19/10/2026, proposition 3 acceptée).
'
' Dépendances :
' - UC_ReferentielBase, GestionTextesTypes, UsageTexteType
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_UsagesTextesTypes
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_usagesTextes
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Usages des textes types"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Éditeurs où un texte type peut être proposé : nom affiché et ordre."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Usages des textes types"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 30
        End Get
    End Property

    Protected Overrides ReadOnly Property CreationPermise As Boolean
        Get
            Return False
        End Get
    End Property

#End Region

#Region "Données - branchement GestionTextesTypes"

    Private _usagesParId As New Dictionary(Of ULong, UsageTexteType)

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim usages As List(Of UsageTexteType) = GestionTextesTypes.GetUsages(afficherInactifs)
        _usagesParId = usages.ToDictionary(Function(u) u.IdUsageTexteType)

        Return usages.Select(
            Function(u) New ReferentielLigne With {
                .IdReferentiel = u.IdUsageTexteType,
                .Code = u.CodeUsage,
                .Libelle = u.LibelleUsage,
                .OrdreAffichage = u.OrdreAffichage,
                .Actif = u.Actif,
                .EstSysteme = u.EstSysteme
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionTextesTypes.CodeUsageExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionTextesTypes.LibelleUsageExiste(libelle, idExclu)
    End Function

    ' Jamais appelée (pas de bouton Nouveau) : un usage naît par migration.
    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)
        Throw New InvalidOperationException("Un usage de texte type se crée par migration, avec son code dans UsagesTexteType.")
    End Sub

    ' Le code n'est pas repris : il est verrouillé (ligne système) et l'application en dépend.
    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)
        GestionTextesTypes.UpdateUsage(_context?.ContexteOperation, New UsageTexteType With {
            .IdUsageTexteType = id,
            .LibelleUsage = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        })
    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        Dim usage As UsageTexteType = Nothing
        If Not _usagesParId.TryGetValue(id, usage) Then Return

        usage.Actif = actif
        GestionTextesTypes.UpdateUsage(_context?.ContexteOperation, usage)

    End Sub

#End Region

End Class
