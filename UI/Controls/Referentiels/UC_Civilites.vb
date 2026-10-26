' -------------------------------------------------------------------------------------------------
' UserControl : UC_Civilites
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des civilités (table ref_civilites).
' Hérite de UC_ReferentielBase ; champ supplémentaire « Abréviation » (courriers : Mme, M.).
'
' Remarques   :
' - V1.1.0 (16/10/2026, UI-7) : champs posés par AjouterLigneDetail (contrôles du kit, plus de
'   couleur ni de position en dur) ; consultation lisible gérée par UC_ReferentielBase.
' - V1.2.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Les civilités sont proposées partout où une personne est saisie (praticien, patient,
'   contact, thérapeute, intervenant).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_Civilites
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_civilites
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Civilités"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les civilités proposées lors de la saisie d'une personne."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Civilités"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 20
        End Get
    End Property

#End Region

#Region "Champ supplémentaire - abréviation"

    ' Longueur de ref_civilites.abreviation_civilite
    Private Const LONGUEUR_MAX_ABREVIATION As Integer = 10

    Private _txtAbreviation As TextBox
    Private _civilitesParId As New Dictionary(Of ULong, Civilite)

    Protected Overrides Sub ConfigurerChampSupplementaire()

        _txtAbreviation = CreerZoneTexteDetail("txtAbreviation", LONGUEUR_MAX_ABREVIATION, 100)
        AjouterLigneDetail("Abréviation", _txtAbreviation)

        If _context IsNot Nothing Then
            _context.SetToolTip(_txtAbreviation, "Forme abrégée utilisée dans les courriers et documents (ex. Mme, M.). Facultative.")
        End If

    End Sub

    Protected Overrides Sub AfficherChampSupplementaire(ligne As ReferentielLigne)
        If _txtAbreviation Is Nothing Then Return
        Dim c As Civilite = Nothing
        If ligne IsNot Nothing AndAlso _civilitesParId.TryGetValue(ligne.IdReferentiel, c) Then
            _txtAbreviation.Text = If(c.AbreviationCivilite, String.Empty)
        Else
            _txtAbreviation.Clear()
        End If
    End Sub

    Protected Overrides Sub ViderChampSupplementaire()
        _txtAbreviation?.Clear()
    End Sub

    Private Function AbreviationSaisie() As String
        Dim v As String = If(_txtAbreviation?.Text, String.Empty).Trim()
        Return If(v.Length = 0, Nothing, v)
    End Function

#End Region

#Region "Colonne supplémentaire - grille"

    Protected Overrides Sub ConfigurerColonnesSupplementaires()

        Dim colAbreviation As New DataGridViewTextBoxColumn With {
            .Name = "colAbreviation",
            .HeaderText = "Abrév.",
            .DataPropertyName = NameOf(ReferentielLigne.Complement),
            .ReadOnly = True,
            .Width = 70
        }

        Dim indexActif As Integer = dgvReferentiel.Columns.IndexOf(colActif)
        If indexActif >= 0 Then
            dgvReferentiel.Columns.Insert(indexActif, colAbreviation)
        Else
            dgvReferentiel.Columns.Add(colAbreviation)
        End If

    End Sub

#End Region

#Region "Données - branchement GestionCivilites"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim civilites = GestionCivilites.GetCivilites(afficherInactifs)
        _civilitesParId = civilites.ToDictionary(Function(c) c.IdCivilite)

        Return civilites.Select(
            Function(c) New ReferentielLigne With {
                .IdReferentiel = c.IdCivilite,
                .Code = c.CodeCivilite,
                .Libelle = c.LibelleCivilite,
                .Complement = c.AbreviationCivilite,
                .OrdreAffichage = c.OrdreAffichage,
                .Actif = c.Actif,
                .EstSysteme = c.EstSysteme
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionCivilites.CodeCiviliteExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionCivilites.LibelleCiviliteExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)
        GestionCivilites.InsertCivilite(_context?.ContexteOperation, New Civilite With {
            .CodeCivilite = code,
            .LibelleCivilite = libelle,
            .AbreviationCivilite = AbreviationSaisie(),
            .OrdreAffichage = ordre,
            .Actif = actif
        })
    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)
        GestionCivilites.UpdateCivilite(_context?.ContexteOperation, New Civilite With {
            .IdCivilite = id,
            .CodeCivilite = code,
            .LibelleCivilite = libelle,
            .AbreviationCivilite = AbreviationSaisie(),
            .OrdreAffichage = ordre,
            .Actif = actif
        })
    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)
        If actif Then
            Dim ligne = GestionCivilites.GetCivilites(True).FirstOrDefault(Function(c) c.IdCivilite = id)
            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionCivilites.UpdateCivilite(_context?.ContexteOperation, ligne)
            End If
        Else
            GestionCivilites.DesactiverCivilite(_context?.ContexteOperation, id)
        End If
    End Sub

#End Region

End Class
