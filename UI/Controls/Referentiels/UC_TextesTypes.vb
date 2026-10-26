' -------------------------------------------------------------------------------------------------
' UserControl : UC_TextesTypes
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Référentiel des textes types (ref_textes_types) : phrases ou trames réutilisables, insérées
' dans les éditeurs de texte selon leurs usages (temps 2 de l'éditeur, ADR-44).
'
' Responsabilités :
' - Grille : code, libellé, usages (« Partout » sans usage), ordre, état.
' - Détail : usages (cases à cocher ; rien de coché = proposé partout) et texte, dans un éditeur
'   au profil Compact (« Ouvrir en grand » pour écrire à l'aise).
'
' Remarques   :
' - Hérite de UC_ReferentielBase (modes, validation du code et du libellé, activation, garde
'   des modifications) ; aucun accès direct à la base : GestionTextesTypes.
' - Pas de suppression : un texte qui ne sert plus est désactivé (il n'est plus proposé).
' - Journal : identifiants seulement, jamais le contenu d'un texte.
' - V1.1.0 (19/10/2026, v2.43.0) : plusieurs usages par texte (liste à cocher) au lieu d'une liste
'   à un seul choix ; usage désactivé encore lié à un texte : affiché « (inactif) », gardé.
' - V1.2.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
'
' Dépendances :
' - UC_ReferentielBase, GestionTextesTypes, TexteType, UsageTexteType, UC_RichTextEditor
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_TextesTypes
    Inherits UC_ReferentielBase

#Region "Constantes"

    ' Libellé d'un texte sans usage, dans la grille.
    Private Const LIBELLE_PARTOUT As String = "Partout"

    ' Lignes de la grille du panneau « Détail » occupées par la liste des usages et par l'éditeur.
    Private Const LIGNES_USAGES As Integer = 3
    Private Const LIGNES_EDITEUR As Integer = 6
    Private Const PAS_LIGNE As Integer = 34

#End Region

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_textesTypes
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Textes types"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Phrases et trames à insérer dans les notes, l'anamnèse et les zones du dossier."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Textes types"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 30
        End Get
    End Property

#End Region

#Region "Champs supplémentaires - usages et texte"

    Private _lstUsages As CheckedListBox
    Private _rteContenu As UC_RichTextEditor
    Private _textesParId As New Dictionary(Of ULong, TexteType)

    ' Les cases ne changent qu'en saisie (consultation : liste lisible mais figée) ou par le code.
    Private _usagesModifiables As Boolean
    Private _cochageParLeCode As Boolean

    ' Police de l'indication sous la liste, libérée avec l'écran.
    Private ReadOnly _policeIndication As New Font("Calibri", 9.0F, FontStyle.Italic)

    ' Élément de la liste : l'usage et son libellé affiché (« (inactif) » si désactivé).
    Private NotInheritable Class ElementUsage
        Public Property Usage As UsageTexteType
        Public Overrides Function ToString() As String
            Return If(Usage.Actif, Usage.LibelleUsage, $"{Usage.LibelleUsage} (inactif)")
        End Function
    End Class

    Protected Overrides Sub ConfigurerChampSupplementaire()

        _lstUsages = New CheckedListBox With {
            .Name = "lstUsages",
            .CheckOnClick = True,
            .IntegralHeight = False,
            .BorderStyle = BorderStyle.FixedSingle,
            .Font = txtCode.Font,
            .ForeColor = txtCode.ForeColor,
            .Size = New Size(LARGEUR_CHAMP_DETAIL, LIGNES_USAGES * PAS_LIGNE - 9)
        }
        AddHandler _lstUsages.ItemCheck, AddressOf LstUsages_ItemCheck
        AjouterLigneDetail("Usages", _lstUsages, lignes:=LIGNES_USAGES)

        Dim indication As New Label With {
            .AutoSize = True,
            .BackColor = Color.Transparent,
            .Font = _policeIndication,
            .ForeColor = UITheme.ColorGrisDiscret,
            .Text = "Rien de coché : texte proposé partout."
        }
        AjouterLigneDetail(String.Empty, indication)

        _rteContenu = New UC_RichTextEditor With {
            .Name = "rteContenu",
            .Profil = ProfilEditeur.Compact,
            .Size = New Size(LARGEUR_CHAMP_DETAIL, LIGNES_EDITEUR * PAS_LIGNE - 9),
            .TitreDocument = "Texte type"
        }
        If _context IsNot Nothing Then _rteContenu.SetContext(_context)
        AjouterLigneDetail("Texte", _rteContenu, lignes:=LIGNES_EDITEUR)

        If _context IsNot Nothing Then
            _context.SetToolTip(_lstUsages, "Éditeurs où le texte est proposé ; rien de coché : dans tous les éditeurs.")
        End If

        AddHandler Disposed, Sub(sender, e) _policeIndication.Dispose()

        ChargerUsages()

    End Sub

    ' Tous les usages (un usage désactivé peut encore être lié à un texte : il reste visible).
    Private Sub ChargerUsages()

        _lstUsages.Items.Clear()

        Try
            For Each usage As UsageTexteType In GestionTextesTypes.GetUsages(True)
                _lstUsages.Items.Add(New ElementUsage With {.Usage = usage})
            Next
        Catch ex As Exception
            GestionLog.EcrireException("UC_TextesTypes.ChargerUsages : usages des textes types illisibles.", ex,
                                       category:=GestionLog.LogCategory.Database)
        End Try

    End Sub

    Private Sub LstUsages_ItemCheck(sender As Object, e As ItemCheckEventArgs)
        If Not _usagesModifiables AndAlso Not _cochageParLeCode Then e.NewValue = e.CurrentValue
    End Sub

    Private Sub CocherUsages(ids As IEnumerable(Of ULong))

        Dim choisis As New HashSet(Of ULong)(ids)

        _cochageParLeCode = True
        Try
            For i As Integer = 0 To _lstUsages.Items.Count - 1
                Dim element As ElementUsage = DirectCast(_lstUsages.Items(i), ElementUsage)
                _lstUsages.SetItemChecked(i, choisis.Contains(element.Usage.IdUsageTexteType))
            Next
        Finally
            _cochageParLeCode = False
        End Try

    End Sub

    Private Function UsagesCoches() As List(Of ULong)
        Return _lstUsages.CheckedItems.Cast(Of ElementUsage)().Select(Function(u) u.Usage.IdUsageTexteType).ToList()
    End Function

    Protected Overrides Sub AfficherChampSupplementaire(ligne As ReferentielLigne)

        If _rteContenu Is Nothing Then Return

        Dim texte As TexteType = Nothing
        If ligne Is Nothing OrElse Not _textesParId.TryGetValue(ligne.IdReferentiel, texte) Then
            ViderChampSupplementaire()
            Return
        End If

        CocherUsages(texte.IdsUsages)
        _rteContenu.TitreDocument = $"Texte type « {texte.LibelleTexteType} »"
        _rteContenu.ChargerContenu(texte.ContenuRtf, texte.ContenuTxt)

    End Sub

    Protected Overrides Sub ViderChampSupplementaire()

        If _rteContenu Is Nothing Then Return

        CocherUsages(Enumerable.Empty(Of ULong)())
        _rteContenu.TitreDocument = "Nouveau texte type"
        _rteContenu.ChargerContenu(String.Empty, String.Empty)

    End Sub

    ' La liste et l'éditeur ne sont pas des champs simples : la base ne les bascule pas.
    Protected Overrides Sub ActiverChampSupplementaire(enEdition As Boolean)

        If _rteContenu Is Nothing Then Return

        _usagesModifiables = enEdition
        _lstUsages.BackColor = If(enEdition, Color.White, UITheme.ColorChampLectureSeule)
        _rteContenu.ReadOnlyMode = Not enEdition

    End Sub

    ' Un texte type vide n'aurait rien à insérer.
    Protected Overrides Function ValiderChampSupplementaire() As Boolean

        If Not String.IsNullOrWhiteSpace(_rteContenu.TextContent) Then Return True

        _context?.SetError(_rteContenu, "Le texte est vide : écrivez le texte à insérer.")
        Return False

    End Function

    Private Sub RenseignerChampsSupplementaires(texte As TexteType)
        texte.IdsUsages = UsagesCoches()
        texte.ContenuRtf = _rteContenu.RtfContent
        texte.ContenuTxt = _rteContenu.TextContent
    End Sub

#End Region

#Region "Colonnes supplémentaires - grille"

    Protected Overrides Sub ConfigurerColonnesSupplementaires()

        Dim colUsages As New DataGridViewTextBoxColumn With {
            .Name = "colUsages",
            .HeaderText = "Usages",
            .DataPropertyName = NameOf(ReferentielLigne.Complement),
            .ReadOnly = True,
            .Width = 220
        }

        Dim indexActif As Integer = dgvReferentiel.Columns.IndexOf(colActif)
        If indexActif >= 0 Then
            dgvReferentiel.Columns.Insert(indexActif, colUsages)
        Else
            dgvReferentiel.Columns.Add(colUsages)
        End If

    End Sub

#End Region

#Region "Données - branchement GestionTextesTypes"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim textes As List(Of TexteType) = GestionTextesTypes.GetTextesTypes(afficherInactifs)
        _textesParId = textes.ToDictionary(Function(t) t.IdTexteType)

        Return textes.Select(
            Function(t) New ReferentielLigne With {
                .IdReferentiel = t.IdTexteType,
                .Code = t.CodeTexteType,
                .Libelle = t.LibelleTexteType,
                .Complement = If(String.IsNullOrEmpty(t.LibellesUsages), LIBELLE_PARTOUT, t.LibellesUsages),
                .OrdreAffichage = t.OrdreAffichage,
                .Actif = t.Actif
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionTextesTypes.CodeTexteTypeExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionTextesTypes.LibelleTexteTypeExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim texte As New TexteType With {
            .CodeTexteType = code,
            .LibelleTexteType = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }
        RenseignerChampsSupplementaires(texte)
        GestionTextesTypes.InsertTexteType(_context?.ContexteOperation, texte)

    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim texte As New TexteType With {
            .IdTexteType = id,
            .CodeTexteType = code,
            .LibelleTexteType = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }
        RenseignerChampsSupplementaires(texte)
        GestionTextesTypes.UpdateTexteType(_context?.ContexteOperation, texte)

    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)
        GestionTextesTypes.DefinirActivation(_context?.ContexteOperation, id, actif)
    End Sub

#End Region

End Class
