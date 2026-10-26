' -------------------------------------------------------------------------------------------------
' UserControl : UC_Pays
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des pays (table ref_pays).
' Hérite de UC_ReferentielBase et branche la couche métier GestionPays.
'
' Remarques   :
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Le code est le code ISO 3166-1 alpha-2 (2 lettres) : il sert de clé aux règles de
'   validation du téléphone et du code postal. La colonne « Contrôles » indique si l'application
'   dispose de règles pour ce pays ; un pays ajouté sans règles est accepté, sans contrôle de format.
' - Les pays dotés de règles sont des lignes système (cadenas) : code et activation verrouillés.
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Text.RegularExpressions

Public Class UC_Pays
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_pays
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Pays"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les pays proposés dans les adresses."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Pays"
        End Get
    End Property

    ' Code ISO 3166-1 alpha-2
    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 2
        End Get
    End Property

    Protected Overrides ReadOnly Property RemplacerEspacesParUnderscore As Boolean
        Get
            Return False
        End Get
    End Property

#End Region

#Region "Validation du code ISO"

    Protected Overrides Function ValiderChampSupplementaire() As Boolean

        If Regex.IsMatch(txtCode.Text.Trim(), "^[A-Z]{2}$") Then Return True

        If _context IsNot Nothing Then
            _context.SetError(txtCode, "Le code doit être le code ISO du pays : 2 lettres (ex. BE, FR, CH).")
        End If
        Return False

    End Function

#End Region

#Region "Colonne supplémentaire - grille"

    Protected Overrides Sub ConfigurerColonnesSupplementaires()

        Dim colControles As New DataGridViewTextBoxColumn With {
            .Name = "colControles",
            .HeaderText = "Contrôles",
            .DataPropertyName = NameOf(ReferentielLigne.Complement),
            .ReadOnly = True,
            .Width = 90,
            .ToolTipText = "Oui : l'application contrôle le format du téléphone (et du code postal, si une règle existe) pour ce pays."
        }
        colControles.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        Dim indexActif As Integer = dgvReferentiel.Columns.IndexOf(colActif)
        If indexActif >= 0 Then
            dgvReferentiel.Columns.Insert(indexActif, colControles)
        Else
            dgvReferentiel.Columns.Add(colControles)
        End If

    End Sub

#End Region

#Region "Données - branchement GestionPays"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Return GestionPays.GetPays(afficherInactifs).Select(
            Function(p) New ReferentielLigne With {
                .IdReferentiel = p.IdPays,
                .Code = p.CodePays,
                .Libelle = p.LibellePays,
                .Complement = If(UtilsTelephone.DeduirePays(p.CodePays) IsNot Nothing, "Oui", "—"),
                .OrdreAffichage = p.OrdreAffichage,
                .Actif = p.Actif,
                .EstSysteme = p.EstSysteme
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionPays.CodePaysExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionPays.LibellePaysExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)
        GestionPays.InsertPays(_context?.ContexteOperation, New Pays With {
            .CodePays = code,
            .LibellePays = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        })
    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)
        GestionPays.UpdatePays(_context?.ContexteOperation, New Pays With {
            .IdPays = id,
            .CodePays = code,
            .LibellePays = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        })
    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)
        If actif Then
            Dim ligne = GestionPays.GetPays(True).FirstOrDefault(Function(p) p.IdPays = id)
            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionPays.UpdatePays(_context?.ContexteOperation, ligne)
            End If
        Else
            GestionPays.DesactiverPays(_context?.ContexteOperation, id)
        End If
    End Sub

#End Region

End Class
