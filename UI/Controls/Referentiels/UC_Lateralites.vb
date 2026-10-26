' -------------------------------------------------------------------------------------------------
' UserControl : UC_Lateralites
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Référentiel des latéralités (ref_lateralites) proposées dans la fiche patient : code, libellé,
' ordre, activation.
'
' Remarques   :
' - Hérite de UC_ReferentielBase ; aucun accès direct à la base : GestionLateralites.
' - Décision de Joëlle (20/10/2026) : la liste n'est plus écrite dans le code (migration v2.46.0).
' - Pas de suppression : une latéralité désactivée n'est plus proposée, mais reste affichée chez
'   les patients qui l'ont.
'
' Dépendances :
' - UC_ReferentielBase, GestionLateralites, Lateralite
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_Lateralites
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_lateralites
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Latéralités"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Latéralités proposées dans la fiche patient."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Latéralités"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 30
        End Get
    End Property

#End Region

#Region "Données - branchement GestionLateralites"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Return GestionLateralites.GetLateralites(afficherInactifs).Select(
            Function(l) New ReferentielLigne With {
                .IdReferentiel = l.IdLateralite,
                .Code = l.CodeLateralite,
                .Libelle = l.LibelleLateralite,
                .OrdreAffichage = l.OrdreAffichage,
                .Actif = l.Actif
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionLateralites.CodeLateraliteExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionLateralites.LibelleLateraliteExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)
        GestionLateralites.InsertLateralite(_context?.ContexteOperation, New Lateralite With {
            .CodeLateralite = code,
            .LibelleLateralite = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        })
    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)
        GestionLateralites.UpdateLateralite(_context?.ContexteOperation, New Lateralite With {
            .IdLateralite = id,
            .CodeLateralite = code,
            .LibelleLateralite = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        })
    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)
        GestionLateralites.DefinirActivation(_context?.ContexteOperation, id, actif)
    End Sub

#End Region

End Class
