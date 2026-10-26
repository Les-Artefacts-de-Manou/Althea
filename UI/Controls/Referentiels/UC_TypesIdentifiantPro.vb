' -------------------------------------------------------------------------------------------------
' UserControl : UC_TypesIdentifiantPro
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Référentiel des types d'identifiant professionnel (ref_types_identifiant_pro) proposés dans
' l'onglet Identifiants de la fiche Cabinet : code, libellé, porté par (praticien ou cabinet),
' masque indicatif, expression de contrôle, ordre, activation.
'
' Remarques   :
' - Hérite de UC_ReferentielBase ; aucun accès direct à la base : GestionTypesIdentifiantPro.
' - Décision de Joëlle (20/10/2026) : écran rattaché au hub Administration (paramètre du cabinet,
'   comme les situations juridiques), réservé au SuperUser.
' - INAMI, BCE et TVA sont des lignes système (migration v2.47.0) : leur contrôle est écrit dans
'   l'application (UtilsIdentifiantBelge, choisi par le code), donc code et activation verrouillés
'   et expression de contrôle en lecture seule (elle ne serait pas utilisée).
' - L'expression de contrôle est vérifiée avant d'enregistrer : une expression invalide serait
'   ignorée à la saisie des identifiants, sans que personne ne s'en aperçoive.
' - « Porté par » prépare la V2 (cabinet de plusieurs praticiens) : numéro propre à chaque
'   praticien ou numéro de l'entité. La V1 (un praticien) propose tous les types actifs.
'
' Dépendances :
' - UC_ReferentielBase, GestionTypesIdentifiantPro, TypeIdentifiantPro, UtilsIdentifiantBelge
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Text.RegularExpressions

Public Class UC_TypesIdentifiantPro
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_typesIdentifiant
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Types d'identifiant"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Numéros professionnels proposés dans la fiche Cabinet (INAMI, agrément, BCE…)."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Administration > Types d'identifiant"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 30
        End Get
    End Property

#End Region

#Region "Champs supplémentaires - porté par, masque, expression"

    ' Longueurs des colonnes masque_saisie et regex_validation
    Private Const LONGUEUR_MAX_MASQUE As Integer = 50
    Private Const LONGUEUR_MAX_EXPRESSION As Integer = 200

    ' Délai d'évaluation de l'expression pendant sa vérification (garde-fou)
    Private Shared ReadOnly DELAI_VERIFICATION_EXPRESSION As TimeSpan = TimeSpan.FromSeconds(1)

    Private _cboCible As AltheaListe
    Private _txtMasque As TextBox
    Private _txtExpression As TextBox
    Private _typesParId As New Dictionary(Of ULong, TypeIdentifiantPro)

    Protected Overrides Sub ConfigurerChampSupplementaire()

        _cboCible = CreerListeDetail("cboCible", 160)
        _cboCible.Items.AddRange(New Object() {"Praticien", "Cabinet"})
        _cboCible.SelectedIndex = 0
        AjouterLigneDetail("Porté par", _cboCible)

        _txtMasque = CreerZoneTexteDetail("txtMasque", LONGUEUR_MAX_MASQUE)
        AjouterLigneDetail("Masque", _txtMasque)

        _txtExpression = CreerZoneTexteDetail("txtExpression", LONGUEUR_MAX_EXPRESSION)
        AjouterLigneDetail("Contrôle", _txtExpression)

        If _context IsNot Nothing Then
            _context.SetToolTip(_cboCible,
                "Qui porte ce numéro :" & vbCrLf &
                "Praticien : chaque praticien a le sien (INAMI, agrément)." & vbCrLf &
                "Cabinet : numéro de l'entité qui facture (BCE, TVA).")
            _context.SetToolTip(_txtMasque,
                "Format indicatif, rappelé quand une valeur saisie est refusée (ex. 0-00000-00-000)." & vbCrLf &
                "Il ne contrôle rien à lui seul.")
            _context.SetToolTip(_txtExpression,
                "Expression régulière que la valeur saisie doit respecter (facultative)." & vbCrLf &
                "Commencez par ^ et terminez par $ pour contrôler toute la valeur, ex. ^[0-9]{6}$." & vbCrLf &
                "INAMI, BCE et TVA : contrôle intégré à Althéa (clé de contrôle), ce champ n'est pas utilisé.")
        End If

    End Sub

    Protected Overrides Sub AfficherChampSupplementaire(ligne As ReferentielLigne)
        If _cboCible Is Nothing Then Return
        Dim t As TypeIdentifiantPro = Nothing
        If ligne Is Nothing OrElse Not _typesParId.TryGetValue(ligne.IdReferentiel, t) Then
            ViderChampSupplementaire()
            Return
        End If
        _cboCible.SelectedIndex = If(String.Equals(t.Cible, TypeIdentifiantPro.CIBLE_CABINET, StringComparison.OrdinalIgnoreCase), 1, 0)
        _txtMasque.Text = t.MasqueSaisie
        _txtExpression.Text = t.RegexValidation
    End Sub

    Protected Overrides Sub ViderChampSupplementaire()
        If _cboCible Is Nothing Then Return
        _cboCible.SelectedIndex = 0
        _txtMasque.Clear()
        _txtExpression.Clear()
    End Sub

    ' La base a remis le panneau en saisie : seule l'expression d'un type contrôlé par Althéa reste figée.
    Protected Overrides Sub ActiverChampSupplementaire(enEdition As Boolean)
        If enEdition AndAlso ExpressionNonUtilisee() Then
            UtilsControls.DefinirChampLectureSeule(_txtExpression, True)
        End If
    End Sub

    Protected Overrides Function ValiderChampSupplementaire() As Boolean

        Dim expression As String = _txtExpression.Text.Trim()
        If expression.Length = 0 OrElse ExpressionNonUtilisee() Then Return True

        Try
            ' Une expression mal formée lève ArgumentException dès son analyse.
            Regex.Match(String.Empty, expression, RegexOptions.None, DELAI_VERIFICATION_EXPRESSION)
            Return True
        Catch ex As ArgumentException
            If _context IsNot Nothing Then
                _context.SetError(_txtExpression,
                    "Cette expression de contrôle n'est pas valide (parenthèse ou crochet non fermé, " &
                    "caractère spécial non précédé de \…). Corrigez-la ou videz le champ.")
            End If
            Return False
        End Try

    End Function

    ' Type en cours de modification dont le contrôle est écrit dans l'application (INAMI, BCE, TVA).
    Private Function ExpressionNonUtilisee() As Boolean
        Dim t As TypeIdentifiantPro = Nothing
        Return IdEnCours <> 0UL AndAlso _typesParId.TryGetValue(IdEnCours, t) AndAlso
               UtilsIdentifiantBelge.EstControleParAlthea(t.CodeTypeIdentifiant)
    End Function

    Private Function CibleSaisie() As String
        Return If(_cboCible IsNot Nothing AndAlso _cboCible.SelectedIndex = 1,
                  TypeIdentifiantPro.CIBLE_CABINET,
                  TypeIdentifiantPro.CIBLE_PRATICIEN)
    End Function

    Private Function TypeSaisi(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean) As TypeIdentifiantPro
        Return New TypeIdentifiantPro With {
            .IdTypeIdentifiant = id,
            .CodeTypeIdentifiant = code,
            .LibelleTypeIdentifiant = libelle,
            .Cible = CibleSaisie(),
            .MasqueSaisie = _txtMasque.Text.Trim(),
            .RegexValidation = _txtExpression.Text.Trim(),
            .OrdreAffichage = ordre,
            .Actif = actif
        }
    End Function

#End Region

#Region "Colonne supplémentaire - grille"

    Protected Overrides Sub ConfigurerColonnesSupplementaires()

        Dim colControle As New DataGridViewTextBoxColumn With {
            .Name = "colControle",
            .HeaderText = "Contrôle",
            .DataPropertyName = NameOf(ReferentielLigne.Complement),
            .ReadOnly = True,
            .Width = 90,
            .ToolTipText = "Althéa : contrôle intégré (clé de contrôle INAMI, BCE, TVA)." & vbCrLf &
                           "Expression : expression de contrôle du type." & vbCrLf &
                           "— : saisie libre."
        }
        colControle.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        Dim indexActif As Integer = dgvReferentiel.Columns.IndexOf(colActif)
        If indexActif >= 0 Then
            dgvReferentiel.Columns.Insert(indexActif, colControle)
        Else
            dgvReferentiel.Columns.Add(colControle)
        End If

    End Sub

    Private Shared Function LibelleControle(t As TypeIdentifiantPro) As String
        If UtilsIdentifiantBelge.EstControleParAlthea(t.CodeTypeIdentifiant) Then Return "Althéa"
        Return If(String.IsNullOrWhiteSpace(t.RegexValidation), "—", "Expression")
    End Function

#End Region

#Region "Données - branchement GestionTypesIdentifiantPro"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim types = GestionTypesIdentifiantPro.GetTypesIdentifiant(afficherInactifs)
        _typesParId = types.ToDictionary(Function(t) t.IdTypeIdentifiant)

        Return types.Select(
            Function(t) New ReferentielLigne With {
                .IdReferentiel = t.IdTypeIdentifiant,
                .Code = t.CodeTypeIdentifiant,
                .Libelle = t.LibelleTypeIdentifiant,
                .Complement = LibelleControle(t),
                .OrdreAffichage = t.OrdreAffichage,
                .Actif = t.Actif,
                .EstSysteme = t.EstSysteme
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionTypesIdentifiantPro.CodeTypeIdentifiantExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionTypesIdentifiantPro.LibelleTypeIdentifiantExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)
        GestionTypesIdentifiantPro.InsertTypeIdentifiant(_context?.ContexteOperation, TypeSaisi(0UL, code, libelle, ordre, actif))
    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)
        GestionTypesIdentifiantPro.UpdateTypeIdentifiant(_context?.ContexteOperation, TypeSaisi(id, code, libelle, ordre, actif))
    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)
        GestionTypesIdentifiantPro.DefinirActivation(_context?.ContexteOperation, id, actif)
    End Sub

#End Region

End Class
