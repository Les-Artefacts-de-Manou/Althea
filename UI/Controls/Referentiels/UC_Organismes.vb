' -------------------------------------------------------------------------------------------------
' UserControl : UC_Organismes
' Projet      : Althéa
' Version     : V1.4.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des organismes (table ref_organismes) : organismes de
' remboursement (rattachés aux domaines) et mutualités (rattachées aux patients).
' Hérite de UC_ReferentielBase et branche la couche métier GestionOrganismes.
'
' Remarques   :
' - V1.3.0 (16/10/2026, UI-7) : champs posés par AjouterLigneDetail (contrôles du kit, plus de
'   couleur ni de position en dur) ; consultation lisible gérée par UC_ReferentielBase.
' - V1.4.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Les lignes système (INAMI) ne peuvent ni changer de code ou de type, ni être désactivées.
' - Le type d'organisme est une énumération (TypeOrganisme) : il pilote les listes proposées.
' - Ouverture « en contexte » (fenêtre modale) : New(typeImpose, idASelectionner) limite la liste à
'   un type (ex. mutualités depuis la fiche Patient), verrouille ce type pour les créations et
'   présélectionne l'organisme courant.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_Organismes
    Inherits UC_ReferentielBase

#Region "Construction"

    ' Type imposé (ouverture en contexte) ; Nothing = tous les organismes (écran Référentiels)
    Private ReadOnly _typeImpose As TypeOrganisme?

    ' Organisme à présélectionner à l'ouverture (ex. mutualité du patient) ; Nothing = aucun
    Private ReadOnly _idASelectionner As ULong?

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(typeImpose As TypeOrganisme, Optional idASelectionner As ULong? = Nothing)
        MyBase.New()
        _typeImpose = typeImpose
        _idASelectionner = idASelectionner
    End Sub

    Private Sub UC_Organismes_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' Le chargement de la liste est fait par la classe de base (son gestionnaire passe avant).
        If DesignMode OrElse Not _idASelectionner.HasValue Then Return
        SelectionnerElement(_idASelectionner.Value)
    End Sub

    Private ReadOnly Property ListeMutualites As Boolean
        Get
            Return _typeImpose.HasValue AndAlso _typeImpose.Value = TypeOrganisme.Mutualite
        End Get
    End Property

#End Region

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_organismes
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return If(ListeMutualites, "Mutualités", "Organismes")
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            If ListeMutualites Then Return "Consultez la fiche d'une mutualité ou ajoutez-en une."
            If _typeImpose.HasValue Then Return "Consultez et gérez les organismes de remboursement."
            Return "Consultez et gérez les organismes de remboursement et les mutualités."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Organismes"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 30
        End Get
    End Property

#End Region

#Region "Champs supplémentaires - type, coordonnées, plateforme, périodicité"

    Private _cboType As AltheaListe
    Private _cboPeriodicite As AltheaListe
    Private _txtTelephone As TextBox
    Private _txtTelephone2 As TextBox
    Private _txtEmail As TextBox
    Private _txtEmail2 As TextBox
    Private _txtSiteWeb As TextBox
    Private _txtUrlPlateforme As TextBox
    Private _txtAdresse As TextBox
    Private _txtProcedure As TextBox
    Private _organismesParId As New Dictionary(Of ULong, Organisme)

    ' Organisme affiché dans le détail (Nothing en création) : sert au verrouillage du type
    Private _organismeAffiche As Organisme

    Protected Overrides Sub ConfigurerChampSupplementaire()

        _cboType = CreerListeDetail("cboType")
        _cboType.DisplayMember = "Value"
        _cboType.ValueMember = "Key"
        _cboType.DataSource = New List(Of KeyValuePair(Of TypeOrganisme, String)) From {
            New KeyValuePair(Of TypeOrganisme, String)(TypeOrganisme.Remboursement, TypesOrganisme.LibelleType(TypeOrganisme.Remboursement)),
            New KeyValuePair(Of TypeOrganisme, String)(TypeOrganisme.Mutualite, TypesOrganisme.LibelleType(TypeOrganisme.Mutualite))
        }
        AjouterLigneDetail("Type", _cboType)

        _cboPeriodicite = CreerListeDetail("cboPeriodicite")
        _cboPeriodicite.DisplayMember = NameOf(Periodicite.LibellePeriodicite)
        _cboPeriodicite.ValueMember = NameOf(Periodicite.IdPeriodicite)
        AjouterLigneDetail("Périodicité", _cboPeriodicite)

        _txtTelephone = CreerZoneTexteDetail("txtTelephone", 30) : AjouterLigneDetail("Téléphone", _txtTelephone)
        _txtTelephone2 = CreerZoneTexteDetail("txtTelephone2", 30) : AjouterLigneDetail("Téléphone 2", _txtTelephone2)
        _txtEmail = CreerZoneTexteDetail("txtEmail", 150) : AjouterLigneDetail("E-mail", _txtEmail)
        _txtEmail2 = CreerZoneTexteDetail("txtEmail2", 150) : AjouterLigneDetail("E-mail 2", _txtEmail2)
        _txtSiteWeb = CreerZoneTexteDetail("txtSiteWeb", 250) : AjouterLigneDetail("Site web", _txtSiteWeb)
        _txtUrlPlateforme = CreerZoneTexteDetail("txtUrlPlateforme", 250) : AjouterLigneDetail("Plateforme", _txtUrlPlateforme)
        _txtAdresse = CreerZoneTexteDetail("txtAdresse", 250) : AjouterLigneDetail("Adresse", _txtAdresse)

        ' Mémo sur deux lignes de la grille du panneau.
        _txtProcedure = CreerZoneTexteDetail("txtProcedure", 0)
        _txtProcedure.Multiline = True
        _txtProcedure.ScrollBars = ScrollBars.Vertical
        _txtProcedure.Height = 58
        AjouterLigneDetail("Procédure", _txtProcedure, lignes:=2)

        If _context IsNot Nothing Then
            _context.SetToolTip(_cboType, "Remboursement : organisme rattaché aux domaines (ex. INAMI). Mutualité : organisme assureur du patient.")
            _context.SetToolTip(_cboPeriodicite, "Périodicité d'encodage / de facturation auprès de l'organisme (référentiel Périodicités).")
            _context.SetToolTip(_txtUrlPlateforme, "Adresse de la plateforme d'encodage en ligne de l'organisme.")
            _context.SetToolTip(_txtProcedure, "Mémo libre : marche à suivre pour l'encodage auprès de l'organisme.")
        End If

        ChargerPeriodicites()

    End Sub

    ' Charge les périodicités actives + une entrée vide (« aucune »). Repli journalisé si vide.
    Private Sub ChargerPeriodicites()

        Dim liste As New List(Of Periodicite) From {
            New Periodicite With {.IdPeriodicite = 0UL, .LibellePeriodicite = "(aucune)"}
        }

        Try
            liste.AddRange(GestionPeriodicites.GetPeriodicitesActives())
        Catch ex As Exception
            GestionLog.EcrireException("UC_Organismes.ChargerPeriodicites : chargement des périodicités impossible", ex, category:=LogCategory.Database)
        End Try

        If liste.Count = 1 Then
            GestionLog.EcrireLog("UC_Organismes : référentiel ref_periodicites vide, aucune périodicité proposée.", category:=LogCategory.UI)
        End If

        _cboPeriodicite.DataSource = liste
        _cboPeriodicite.SelectedIndex = 0

    End Sub

    Private Sub SelectionnerPeriodicite(id As ULong)
        For i As Integer = 0 To _cboPeriodicite.Items.Count - 1
            If DirectCast(_cboPeriodicite.Items(i), Periodicite).IdPeriodicite = id Then
                _cboPeriodicite.SelectedIndex = i
                Return
            End If
        Next
        _cboPeriodicite.SelectedIndex = 0
    End Sub

    Private Function TypeSaisi() As TypeOrganisme
        If _cboType.SelectedValue Is Nothing Then Return TypeOrganisme.Remboursement
        Return CType(_cboType.SelectedValue, TypeOrganisme)
    End Function

    Private Function PeriodiciteSaisie() As ULong
        Dim p = TryCast(_cboPeriodicite.SelectedItem, Periodicite)
        Return If(p Is Nothing, 0UL, p.IdPeriodicite)
    End Function

    Protected Overrides Sub AfficherChampSupplementaire(ligne As ReferentielLigne)
        If _cboPeriodicite Is Nothing Then Return
        Dim o As Organisme = Nothing
        If ligne Is Nothing OrElse Not _organismesParId.TryGetValue(ligne.IdReferentiel, o) Then
            ViderChampSupplementaire()
            Return
        End If
        _organismeAffiche = o
        _cboType.SelectedValue = o.TypeOrganisme
        SelectionnerPeriodicite(o.IdPeriodicite)
        _txtTelephone.Text = If(o.Telephone, "")
        _txtTelephone2.Text = If(o.Telephone2, "")
        _txtEmail.Text = If(o.Email, "")
        _txtEmail2.Text = If(o.Email2, "")
        _txtSiteWeb.Text = If(o.SiteWeb, "")
        _txtUrlPlateforme.Text = If(o.UrlPlateforme, "")
        _txtAdresse.Text = If(o.Adresse, "")
        _txtProcedure.Text = If(o.ProcedureEncodage, "")
    End Sub

    Protected Overrides Sub ViderChampSupplementaire()
        If _cboPeriodicite Is Nothing Then Return
        _organismeAffiche = Nothing
        _cboType.SelectedValue = If(_typeImpose, TypeOrganisme.Remboursement)
        _cboPeriodicite.SelectedIndex = 0
        For Each t In ChampsTexte()
            t.Clear()
        Next
    End Sub

    Protected Overrides Sub ActiverChampSupplementaire(enEdition As Boolean)
        If _cboType Is Nothing Then Return
        ' Le type d'une ligne système est verrouillé (comme son code) : le code s'y réfère.
        ' Ouverture en contexte : le type est imposé (un organisme d'un autre type sortirait de la liste).
        ' Le reste du panneau a déjà été basculé par la base (consultation lisible).
        Dim typeVerrouille As Boolean = _typeImpose.HasValue OrElse
            (IdEnCours <> 0UL AndAlso _organismeAffiche IsNot Nothing AndAlso _organismeAffiche.EstSysteme)
        If typeVerrouille Then UtilsControls.DefinirChampLectureSeule(_cboType, True)
    End Sub

    Protected Overrides Function ValiderChampSupplementaire() As Boolean

        Dim erreur As String = Nothing

        If Not UtilsValidation.IsValidTelephone(_txtTelephone.Text, erreur) Then
            Return SignalerErreur(_txtTelephone, "Téléphone : " & erreur)
        End If
        If Not UtilsValidation.IsValidTelephone(_txtTelephone2.Text, erreur) Then
            Return SignalerErreur(_txtTelephone2, "Téléphone 2 : " & erreur)
        End If
        If Not UtilsValidation.IsValidEmail(_txtEmail.Text, erreur) Then
            Return SignalerErreur(_txtEmail, "E-mail : " & erreur)
        End If
        If Not UtilsValidation.IsValidEmail(_txtEmail2.Text, erreur) Then
            Return SignalerErreur(_txtEmail2, "E-mail 2 : " & erreur)
        End If

        Return True

    End Function

    Private Function SignalerErreur(champ As Control, message As String) As Boolean
        MessageBox.Show(message, "Organismes", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        champ.Focus()
        Return False
    End Function

    Private Iterator Function ChampsTexte() As IEnumerable(Of TextBox)
        Yield _txtTelephone
        Yield _txtTelephone2
        Yield _txtEmail
        Yield _txtEmail2
        Yield _txtSiteWeb
        Yield _txtUrlPlateforme
        Yield _txtAdresse
        Yield _txtProcedure
    End Function

    Private Function Texte(t As TextBox) As String
        Dim v = t.Text.Trim()
        Return If(v.Length = 0, Nothing, v)
    End Function

    Private Sub RenseignerChampsSupplementaires(o As Organisme)
        o.TypeOrganisme = TypeSaisi()
        o.IdPeriodicite = PeriodiciteSaisie()
        o.Telephone = Texte(_txtTelephone)
        o.Telephone2 = Texte(_txtTelephone2)
        o.Email = Texte(_txtEmail)
        o.Email2 = Texte(_txtEmail2)
        o.SiteWeb = Texte(_txtSiteWeb)
        o.UrlPlateforme = Texte(_txtUrlPlateforme)
        o.Adresse = Texte(_txtAdresse)
        o.ProcedureEncodage = Texte(_txtProcedure)
    End Sub

#End Region

#Region "Colonnes supplémentaires - grille"

    Private _colType As DataGridViewTextBoxColumn

    Protected Overrides Sub ConfigurerColonnesSupplementaires()

        ' Type : valorisé dans CellFormatting à partir de l'organisme (ReferentielLigne n'a qu'un complément)
        _colType = New DataGridViewTextBoxColumn With {
            .Name = "colType",
            .HeaderText = "Type",
            .ReadOnly = True,
            .Width = 115
        }

        Dim colPeriodicite As New DataGridViewTextBoxColumn With {
            .Name = "colPeriodicite",
            .HeaderText = "Périodicité",
            .DataPropertyName = NameOf(ReferentielLigne.Complement),
            .ReadOnly = True,
            .Width = 110
        }

        Dim indexActif As Integer = dgvReferentiel.Columns.IndexOf(colActif)
        If indexActif >= 0 Then
            dgvReferentiel.Columns.Insert(indexActif, colPeriodicite)
            dgvReferentiel.Columns.Insert(indexActif, _colType)
        Else
            dgvReferentiel.Columns.Add(_colType)
            dgvReferentiel.Columns.Add(colPeriodicite)
        End If

        AddHandler dgvReferentiel.CellFormatting, AddressOf dgvReferentiel_FormaterType

    End Sub

    Private Sub dgvReferentiel_FormaterType(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If _colType Is Nothing OrElse e.RowIndex < 0 OrElse e.ColumnIndex <> _colType.Index Then Return
        Dim ligne = TryCast(dgvReferentiel.Rows(e.RowIndex).DataBoundItem, ReferentielLigne)
        Dim o As Organisme = Nothing
        If ligne IsNot Nothing AndAlso _organismesParId.TryGetValue(ligne.IdReferentiel, o) Then
            e.Value = TypesOrganisme.LibelleType(o.TypeOrganisme)
            e.FormattingApplied = True
        End If
    End Sub

#End Region

#Region "Données - branchement GestionOrganismes"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim organismes = GestionOrganismes.GetOrganismes(afficherInactifs).
            Where(Function(o) Not _typeImpose.HasValue OrElse o.TypeOrganisme = _typeImpose.Value).ToList()
        _organismesParId = organismes.ToDictionary(Function(o) o.IdOrganisme)

        Return organismes.Select(
            Function(o) New ReferentielLigne With {
                .IdReferentiel = o.IdOrganisme,
                .Code = o.CodeOrganisme,
                .Libelle = o.LibelleOrganisme,
                .Complement = o.LibellePeriodicite,
                .OrdreAffichage = o.OrdreAffichage,
                .Actif = o.Actif,
                .EstSysteme = o.EstSysteme
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionOrganismes.CodeOrganismeExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionOrganismes.LibelleOrganismeExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim o As New Organisme With {
            .CodeOrganisme = code,
            .LibelleOrganisme = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }
        RenseignerChampsSupplementaires(o)
        GestionOrganismes.InsertOrganisme(_context?.ContexteOperation, o)

    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim o As New Organisme With {
            .IdOrganisme = id,
            .CodeOrganisme = code,
            .LibelleOrganisme = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }
        RenseignerChampsSupplementaires(o)
        GestionOrganismes.UpdateOrganisme(_context?.ContexteOperation, o)

    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            Dim org As Organisme = GestionOrganismes.GetOrganismes(True).
                FirstOrDefault(Function(o) o.IdOrganisme = id)

            If org IsNot Nothing Then
                org.Actif = True
                GestionOrganismes.UpdateOrganisme(_context?.ContexteOperation, org)
            End If
        Else
            GestionOrganismes.DesactiverOrganisme(_context?.ContexteOperation, id)
        End If

    End Sub

#End Region

End Class
