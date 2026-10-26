' -------------------------------------------------------------------------------------------------
' UserControl : UC_Cabinet
' Projet      : Althéa
' Version     : V1.11.1
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran d'administration de la fondation Cabinet (lot 27) : identité du cabinet, fiche du
' praticien, images (logo, photo, signature), cachet généré ou importé, identifiants
' professionnels et qualifications.
'
' Remarques   :
' - V1 : un cabinet, un praticien (règle portée par GestionCabinet).
' - Aucune donnée codée en dur : tout est saisi ici.
' - Images stockées via CheminsCabinetHelper (la base ne garde que le nom du fichier).
' - La fiche s'ouvre en consultation (lecture seule) ; le bouton Modifier ouvre l'édition, comme
'   UC_PatientFiche. Sans cabinet enregistré, la fiche s'ouvre directement en saisie.
' - Le cabinet et le praticien doivent être enregistrés avant l'import d'images et la saisie
'   des identifiants (dossier de stockage et clé étrangère).
' - UI-7 (15/10/2026) : onglet Praticien en cartes (Identité, Coordonnées, Adresse professionnelle),
'   listes du kit, consultation lisible ; icônes des boutons Outil de l'onglet redessinées.
'   Onglet Cabinet de même : cartes Cabinet, Adresse, Coordonnées, Facturation et Logo.
' - V1.4.0 (16/10/2026) : onglet Identifiants en deux cartes titrées (consigne + grille) à la place
'   du SplitContainer ; onglet Cachet : consigne dans la carte, icônes du Designer (tampon, import) ;
'   messages par DialogChoix, sans détail technique à l'écran (le détail reste au journal).
' - V1.5.0 (19/10/2026) : modifications non enregistrées — quitter l'écran propose Enregistrer /
'   Quitter sans enregistrer / Rester, Annuler demande « Abandonner / Continuer la saisie », comme
'   partout dans l'application (DialogChoix.DemanderAvantDepart, ConfirmerAbandon ; ADR-43).
' - V1.6.0 (20/10/2026) : types d'identifiant lus par GestionTypesIdentifiantPro (référentiel
'   modifiable dans Administration › Types d'identifiant). Un type désactivé n'est plus proposé
'   pour une nouvelle ligne, mais reste affiché sur les identifiants qui l'utilisent ; le masque du
'   type est rappelé quand une valeur est refusée. IBAN et BIC : exemple de format dans le champ
'   vide, présentation en groupes de 4 / en majuscules à l'affichage et à la sortie du champ ;
'   l'IBAN est enregistré sans espace (colonne de 34 caractères), le champ en accepte 42.
' - V1.7.0 (20/10/2026) : cohérence des enregistrements (décision de Joëlle : « tout différer »).
'   Images et cachet attendent Enregistrer (Annuler les abandonne) ; cachet généré depuis la
'   saisie en cours ; fiche écrite en une transaction (GestionCabinet.EnregistrerFiche), puis les
'   images (fichiers, puis noms en base). Grilles limitées à la longueur des colonnes.
' - V1.8.0 (20/10/2026) : compte de travail du praticien réservé à l'administrateur (élévation
'   comprise ; audit A.3) — boutons inactifs avec leur raison pour un SuperUser.
'
' Interface   :
' - IContextAwareUserControl : injection du contexte UI partagé
' - V1.9.0 (22/10/2026, audit C.1) : les opérations sensibles reçoivent le ContexteOperation de l'écran (utilisateur + rôle
'   courant), vérifié par le métier (Autorisation).
' - V1.10.0 (24/10/2026) : le compte lié au praticien est rappelé par son code (« Compte lié :
'   USR000002 — … », Rules § 15).
' - V1.11.0 (24/10/2026, v2.56.0) : adresses du cabinet et du praticien : code postal belge contrôlé
'   par le référentiel, localité proposée, suggérée et confirmée si elle ne correspond pas au code
'   (SaisieLocaliteBelge).
' - V1.11.1 (25/10/2026, audit C.4, ADR-55) : la fenêtre principale est lue dans le contexte
'   (_context.FenetrePrincipale, IFenetrePrincipale) ; plus aucune référence à Home.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_Cabinet
    Implements IContextAwareUserControl
    Implements IUnsavedChangesAware

#Region "Variables privées"

    Private _context As UserControlContext
    Private _cabinet As Cabinet
    Private _praticien As Praticien
    Private _typesIdentifiant As List(Of TypeIdentifiantPro)
    Private _situationsJuridiques As New List(Of SituationJuridique)
    Private _identifiantsSupprimes As New List(Of ULong)
    Private _qualificationsSupprimees As New List(Of ULong)
    Private _isDirty As Boolean
    Private _chargementEnCours As Boolean
    Private _enEdition As Boolean

    ' Aide à la saisie du code postal et de la localité (adresses belges, v2.56.0).
    Private _saisieLocaliteCabinet As SaisieLocaliteBelge
    Private _saisieLocalitePraticien As SaisieLocaliteBelge

    ' Entrée « sans pays » du praticien : son adresse vide reprend celle du cabinet
    Private Const LIBELLE_PAYS_CABINET As String = "(pays du cabinet)"

    ' Longueurs des colonnes (cab_praticien_identifiants, cab_praticien_qualifications) : une saisie
    ' plus longue était refusée par la base à l'enregistrement.
    Private Const LONGUEUR_MAX_VALEUR_IDENTIFIANT As Integer = 50
    Private Const LONGUEUR_MAX_LIBELLE_QUALIFICATION As Integer = 200
    Private Const LONGUEUR_MAX_ETABLISSEMENT As Integer = 150
    Private Const LONGUEUR_MAX_ANNEE As Integer = 4


#End Region

#Region "Contexte UI"

    Public Sub SetContext(context As UserControlContext) _
        Implements IContextAwareUserControl.SetContext

        _context = context

    End Sub

#End Region

#Region "Chargement"

    Private Sub UC_Cabinet_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If _context IsNot Nothing Then
            _context.SetHeader("Administration > Cabinet")
        End If

        ' Seuls les boutons de la barre d'actions passent par UtilsButtons. Les boutons Outil (logo,
        ' photo, signature, compte, cachet) portent l'icône 24 px du Designer : celles d'UtilsButtons,
        ' fines et claires, disparaissaient sur le fond des cartes.
        For Each btn In {btnModifier, btnEnregistrer, btnAnnuler}
            UtilsButtons.InitStandardButton(btn)
        Next

        txtNomCabinet.PlaceholderText = "Ex. le nom du praticien, ou le nom commun d'un cabinet à plusieurs praticiens"
        txtCabTva.PlaceholderText = "Uniquement si assujetti à la TVA"
        txtCabIban.PlaceholderText = "BE00 0000 0000 0000"
        txtCabBic.PlaceholderText = "GEBABEBB"

        ' Un cachet généré mais pas enregistré est une image en mémoire : libérée avec l'écran.
        AddHandler Disposed, Sub(s, a) OublierImagesEnAttente()

        _saisieLocaliteCabinet = New SaisieLocaliteBelge(txtCabCp, txtCabLocalite,
                                                         Function() UtilsSaisieReferentiels.CodePaysSaisi(cboCabPays))
        _saisieLocalitePraticien = New SaisieLocaliteBelge(txtPraCp, txtPraLocalite, AddressOf PaysPraticienSaisi)
        AddHandler Disposed, Sub(s, a)
                                 _saisieLocaliteCabinet.Dispose()
                                 _saisieLocalitePraticien.Dispose()
                             End Sub

        InitialiserGrilles()
        AppliquerTheme()
        AbonnerSuiviModifications()
        Charger()

    End Sub

    Private Sub AppliquerTheme()

        ' Couleurs des onglets, libellés et champs : définies dans le Designer.
        For Each dgv As DataGridView In {dgvIdentifiants, dgvQualifications}
            UtilsDataGrid.InitDataGridBasique(dgv)
            dgv.ReadOnly = False
            dgv.AllowUserToAddRows = True
            dgv.AllowUserToDeleteRows = True
            dgv.SelectionMode = DataGridViewSelectionMode.CellSelect
            dgv.EditMode = DataGridViewEditMode.EditOnEnter
        Next

    End Sub

    Private Function TousLesControles(parent As Control) As IEnumerable(Of Control)
        Dim liste As New List(Of Control)
        For Each c As Control In parent.Controls
            liste.Add(c)
            liste.AddRange(TousLesControles(c))
        Next
        Return liste
    End Function

#End Region

#Region "Suivi des modifications"

    Private Sub AbonnerSuiviModifications()
        For Each ctrl In TousLesControles(tabCabinet)
            If TypeOf ctrl Is TextBox OrElse TypeOf ctrl Is ComboBox Then
                AddHandler ctrl.TextChanged, AddressOf Champ_Modifie
            End If
        Next
        For Each dgv In {dgvIdentifiants, dgvQualifications}
            AddHandler dgv.CellValueChanged, AddressOf Grille_Modifiee
            AddHandler dgv.UserDeletedRow, AddressOf Grille_LigneSupprimee
            AddHandler dgv.CurrentCellDirtyStateChanged, AddressOf Grille_CelluleSale
        Next
    End Sub

    Private Sub Champ_Modifie(sender As Object, e As EventArgs)
        DefinirModifie(True)
    End Sub

    Private Sub Grille_Modifiee(sender As Object, e As DataGridViewCellEventArgs)
        DefinirModifie(True)
    End Sub

    Private Sub Grille_LigneSupprimee(sender As Object, e As DataGridViewRowEventArgs)
        DefinirModifie(True)
    End Sub

    Private Sub Grille_CelluleSale(sender As Object, e As EventArgs)
        Dim dgv = DirectCast(sender, DataGridView)
        ' Valide immédiatement cases à cocher et listes pour déclencher CellValueChanged.
        If dgv.IsCurrentCellDirty AndAlso (TypeOf dgv.CurrentCell Is DataGridViewCheckBoxCell OrElse TypeOf dgv.CurrentCell Is DataGridViewComboBoxCell) Then
            dgv.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub DefinirModifie(valeur As Boolean)
        If _chargementEnCours AndAlso valeur Then Return
        _isDirty = valeur
        btnEnregistrer.Enabled = valeur
    End Sub

    Public Function PeutQuitter() As Boolean Implements IUnsavedChangesAware.PeutQuitter
        dgvIdentifiants.EndEdit()
        dgvQualifications.EndEdit()
        If Not _isDirty Then Return True
        ' Même question que les autres écrans (19/10/2026) : « Enregistrer » passe par le bouton, qui
        ' valide la saisie ; l'enregistrement réussi recharge la fiche et efface l'état modifié.
        Select Case DialogChoix.DemanderAvantDepart("du cabinet")
            Case ChoixAvantDepart.Enregistrer
                btnEnregistrer.PerformClick()
                Return Not _isDirty
            Case ChoixAvantDepart.QuitterSansEnregistrer
                Return True
            Case Else
                Return False
        End Select
    End Function

#End Region

#Region "Chargement des données"

    Private Sub InitialiserGrilles()

        ' Tous les types (désactivés compris) pour relire les identifiants existants ; la liste
        ' proposée pour une nouvelle ligne ne contient que les types actifs.
        _typesIdentifiant = GestionTypesIdentifiantPro.GetTypesIdentifiant(afficherInactifs:=True)

        dgvIdentifiants.AutoGenerateColumns = False
        dgvIdentifiants.Columns.Clear()
        dgvIdentifiants.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "colIdIdent", .Visible = False})
        dgvIdentifiants.Columns.Add(New DataGridViewComboBoxColumn With {
            .Name = "colType", .HeaderText = "Type", .FillWeight = 35,
            .DataSource = TypesProposes(Nothing), .DisplayMember = NameOf(TypeIdentifiantPro.LibelleTypeIdentifiant),
            .ValueMember = NameOf(TypeIdentifiantPro.IdTypeIdentifiant), .ValueType = GetType(ULong)})
        dgvIdentifiants.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "colValeur", .HeaderText = "Valeur", .FillWeight = 25,
                                    .MaxInputLength = LONGUEUR_MAX_VALEUR_IDENTIFIANT})
        dgvIdentifiants.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "colDebut", .HeaderText = "Début", .FillWeight = 12})
        dgvIdentifiants.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "colFin", .HeaderText = "Fin", .FillWeight = 12})
        dgvIdentifiants.Columns.Add(New DataGridViewCheckBoxColumn With {.Name = "colCachetIdent", .HeaderText = "Cachet", .FillWeight = 8})

        dgvQualifications.AutoGenerateColumns = False
        dgvQualifications.Columns.Clear()
        dgvQualifications.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "colIdQualif", .Visible = False})
        dgvQualifications.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "colLibelle", .HeaderText = "Libellé", .FillWeight = 45,
                                      .MaxInputLength = LONGUEUR_MAX_LIBELLE_QUALIFICATION})
        dgvQualifications.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "colEtablissement", .HeaderText = "Établissement", .FillWeight = 30,
                                      .MaxInputLength = LONGUEUR_MAX_ETABLISSEMENT})
        dgvQualifications.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "colAnnee", .HeaderText = "Année", .FillWeight = 8,
                                      .MaxInputLength = LONGUEUR_MAX_ANNEE})
        dgvQualifications.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "colOrdre", .HeaderText = "Ordre", .FillWeight = 7})
        dgvQualifications.Columns.Add(New DataGridViewCheckBoxColumn With {.Name = "colCachetQualif", .HeaderText = "Cachet", .FillWeight = 8})

    End Sub

    Private Sub Charger()

        Try
            _chargementEnCours = True
            OublierImagesEnAttente()
            _cabinet = If(GestionCabinet.GetCabinetActif(), New Cabinet())
            _praticien = If(If(_cabinet.IdCabinet > 0UL, GestionCabinet.GetPraticiens(_cabinet.IdCabinet, False).FirstOrDefault(), Nothing), New Praticien())
            _identifiantsSupprimes.Clear()
            _qualificationsSupprimees.Clear()

            AfficherCabinet()
            AfficherPraticien()
            AfficherIdentifiants()
            AfficherQualifications()
            AfficherImages()
            AppliquerMode(_cabinet.IdCabinet = 0UL)

            If _context IsNot Nothing Then
                _context.SetStatus(If(_cabinet.IdCabinet = 0UL,
                    "Aucun cabinet paramétré : saisissez les informations puis enregistrez.",
                    "Informations du cabinet chargées (consultation)."))
            End If

        Catch ex As Exception
            GestionLog.EcrireLog("Erreur lors du chargement du cabinet.", GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            DialogChoix.Erreur("Impossible de charger les informations du cabinet." & vbCrLf &
                               "Le détail est dans le journal.", "Cabinet")
        Finally
            _chargementEnCours = False
            DefinirModifie(False)
        End Try

        _saisieLocaliteCabinet?.Actualiser()
        _saisieLocalitePraticien?.Actualiser()

    End Sub

    Private Sub AfficherCabinet()
        txtNomCabinet.Text = _cabinet.NomCabinet
        ChargerSituationsJuridiques()
        txtCabRue.Text = _cabinet.AdresseRue
        txtCabNumero.Text = _cabinet.AdresseNumero
        txtCabCp.Text = _cabinet.CodePostal
        txtCabLocalite.Text = _cabinet.Localite
        UtilsSaisieReferentiels.ChargerPays(cboCabPays, IdOuNothing(_cabinet.IdPays))
        txtCabTelephone.Text = _cabinet.Telephone
        txtCabEmail.Text = _cabinet.Email
        txtCabSite.Text = _cabinet.SiteWeb
        txtCabBce.Text = _cabinet.NumeroBce
        txtCabTva.Text = _cabinet.NumeroTva
        txtCabIban.Text = UtilsIdentifiantBelge.FormaterIban(_cabinet.Iban)
        txtCabBic.Text = UtilsIdentifiantBelge.FormaterBic(_cabinet.Bic)
    End Sub

    ' IBAN et BIC remis en forme à la sortie du champ (comme le téléphone de la fiche patient) ;
    ' rien en consultation, où le champ est en lecture seule.
    Private Sub txtCabIban_Leave(sender As Object, e As EventArgs) Handles txtCabIban.Leave
        If _chargementEnCours OrElse txtCabIban.ReadOnly Then Return
        txtCabIban.Text = UtilsIdentifiantBelge.FormaterIban(txtCabIban.Text)
    End Sub

    Private Sub txtCabBic_Leave(sender As Object, e As EventArgs) Handles txtCabBic.Leave
        If _chargementEnCours OrElse txtCabBic.ReadOnly Then Return
        txtCabBic.Text = UtilsIdentifiantBelge.FormaterBic(txtCabBic.Text)
    End Sub

#Region "Situation juridique"

    ' Charge les situations actives (plus la situation courante si elle a été désactivée) et sélectionne celle du cabinet.
    Private Sub ChargerSituationsJuridiques()
        Dim idCourant = _cabinet.IdSituationJuridique
        _situationsJuridiques = GestionSituationsJuridiques.GetSituationsJuridiques(True).
            Where(Function(s) s.Actif OrElse s.IdSituationJuridique = idCourant).ToList()

        RemoveHandler cboCabSituation.SelectedIndexChanged, AddressOf CboCabSituation_SelectedIndexChanged
        cboCabSituation.DataSource = Nothing
        cboCabSituation.DisplayMember = "LibelleSituationJuridique"
        cboCabSituation.ValueMember = "IdSituationJuridique"
        cboCabSituation.DataSource = _situationsJuridiques
        cboCabSituation.SelectedIndex = _situationsJuridiques.FindIndex(Function(s) s.IdSituationJuridique = idCourant)
        AddHandler cboCabSituation.SelectedIndexChanged, AddressOf CboCabSituation_SelectedIndexChanged

        AppliquerPorteurBce()
    End Sub

    Private Function SituationSelectionnee() As SituationJuridique
        Return TryCast(cboCabSituation.SelectedItem, SituationJuridique)
    End Function

    Private Sub CboCabSituation_SelectedIndexChanged(sender As Object, e As EventArgs)
        AppliquerPorteurBce()
        DefinirModifie(True)
    End Sub

    ' Le BCE du cabinet n'est saisissable que si la situation désigne le cabinet comme porteur.
    ' Aucune valeur n'est effacée : un changement de situation reste réversible.
    Private Sub AppliquerPorteurBce()
        Dim situation = SituationSelectionnee()
        Dim parPraticien = situation IsNot Nothing AndAlso situation.BceParPraticien
        txtCabBce.Enabled = Not parPraticien
        If _context IsNot Nothing Then
            _context.SetToolTip(txtCabBce, If(parPraticien,
                "Situation juridique « " & situation.LibelleSituationJuridique & " » : le numéro BCE est porté par chaque praticien (onglet Praticien > Identifiants).",
                "Numéro BCE de l'entité qui facture (cabinet)."))
        End If
    End Sub

    ' Une seule saisie du BCE, à l'endroit désigné par la situation juridique (porteur_bce) :
    ' - porteur Cabinet   : BCE dans la fiche cabinet ; une ligne BCE praticien est proposée au déplacement.
    ' - porteur Praticien : BCE dans les identifiants du praticien ; le BCE cabinet est proposé au déplacement.
    ' Aucun déplacement n'est fait sans confirmation de l'utilisateur.
    Private Function ValiderPorteurBce() As Boolean
        Dim situation = SituationSelectionnee()
        If situation Is Nothing Then Return True

        Dim typeBce = _typesIdentifiant.FirstOrDefault(Function(t) String.Equals(t.CodeTypeIdentifiant, UtilsIdentifiantBelge.CodeBce, StringComparison.OrdinalIgnoreCase))
        If typeBce Is Nothing Then Return True

        Dim lignesBce = dgvIdentifiants.Rows.Cast(Of DataGridViewRow)().
            Where(Function(r) Not r.IsNewRow AndAlso r.Cells("colType").Value IsNot Nothing AndAlso
                              Convert.ToUInt64(r.Cells("colType").Value) = typeBce.IdTypeIdentifiant AndAlso
                              Not String.IsNullOrWhiteSpace(Convert.ToString(r.Cells("colValeur").Value))).ToList()

        If situation.BceParPraticien Then
            If lignesBce.Count > 0 Then Return True
            If Not String.IsNullOrWhiteSpace(txtCabBce.Text) AndAlso
               DialogChoix.Confirmer("La situation « " & situation.LibelleSituationJuridique & " » prévoit que le praticien porte le numéro BCE." & vbCrLf &
                                     "Déplacer le BCE du cabinet (" & txtCabBce.Text.Trim() & ") vers les identifiants du praticien ?",
                                     "Numéro BCE") = DialogResult.Yes Then
                dgvIdentifiants.Rows.Add(Nothing, typeBce.IdTypeIdentifiant, txtCabBce.Text.Trim(), Nothing, Nothing, True)
                txtCabBce.Text = String.Empty
                Return True
            End If
            Return Signaler(tpIdentifiants, dgvIdentifiants,
                "La situation juridique « " & situation.LibelleSituationJuridique & " » prévoit que le praticien porte le numéro BCE." & vbCrLf &
                "Ajoutez un identifiant de type « " & typeBce.LibelleTypeIdentifiant & " » au praticien.")
        End If

        If lignesBce.Count = 0 Then Return True
        Dim valeurPraticien = Convert.ToString(lignesBce(0).Cells("colValeur").Value).Trim()
        Dim message = "La situation « " & situation.LibelleSituationJuridique & " » prévoit que le numéro BCE est porté par le cabinet." & vbCrLf &
                      If(String.IsNullOrWhiteSpace(txtCabBce.Text),
                         "Déplacer le BCE du praticien (" & valeurPraticien & ") vers la fiche cabinet ?",
                         "Le cabinet a déjà le BCE " & txtCabBce.Text.Trim() & ". Retirer la ligne BCE des identifiants du praticien ?")
        If DialogChoix.Confirmer(message, "Numéro BCE") <> DialogResult.Yes Then Return True

        If String.IsNullOrWhiteSpace(txtCabBce.Text) Then txtCabBce.Text = valeurPraticien
        For Each r In lignesBce
            Dim idTexte = Convert.ToString(r.Cells("colIdIdent").Value)
            If Not String.IsNullOrEmpty(idTexte) Then _identifiantsSupprimes.Add(ULong.Parse(idTexte))
            dgvIdentifiants.Rows.Remove(r)
        Next
        Return True
    End Function

#End Region

    Private Sub AfficherPraticien()
        UtilsSaisieReferentiels.ChargerCivilites(cboPraCivilite, IdOuNothing(_praticien.IdCivilite))
        txtPraNom.Text = _praticien.Nom
        txtPraPrenom.Text = _praticien.Prenom
        txtPraTitre.Text = _praticien.TitreProfessionnel
        txtPraTelephone.Text = _praticien.Telephone
        txtPraEmail.Text = _praticien.Email
        txtPraSite.Text = _praticien.SiteWeb
        txtPraRue.Text = _praticien.AdresseRue
        txtPraNumero.Text = _praticien.AdresseNumero
        txtPraCp.Text = _praticien.CodePostal
        txtPraLocalite.Text = _praticien.Localite
        UtilsSaisieReferentiels.ChargerPays(cboPraPays, IdOuNothing(_praticien.IdPays), LIBELLE_PAYS_CABINET)
        AfficherCompte()
    End Sub

    Private Sub AfficherCompte()
        If _praticien.ACompteLie Then
            lblCompteEtat.Text = $"Compte lié : {_praticien.UtilisateurCode} — {_praticien.UtilisateurNomAffichage}" & Environment.NewLine &
                                 $"Identifiant : {_praticien.UtilisateurLogin}" & Environment.NewLine &
                                 $"Rôle : {_praticien.UtilisateurRole}" & Environment.NewLine &
                                 $"État : {If(_praticien.UtilisateurActif, "actif", "inactif")}"
        ElseIf _praticien.IdPraticien = 0UL Then
            lblCompteEtat.Text = "Enregistrez d'abord le praticien pour pouvoir lui attacher un compte de travail."
        Else
            lblCompteEtat.Text = "Aucun compte lié." & Environment.NewLine &
                                 "Créez un compte SuperUser dédié ou liez un compte SuperUser existant. Le compte Admin reste inchangé."
        End If
    End Sub

    Private Sub AfficherIdentifiants()
        dgvIdentifiants.Rows.Clear()
        If _praticien.IdPraticien = 0UL Then Return
        For Each i In GestionCabinet.GetIdentifiants(_praticien.IdPraticien)
            Dim index As Integer = dgvIdentifiants.Rows.Add(i.IdIdentifiant, i.IdTypeIdentifiant, i.Valeur,
                                     If(i.DateDebut.HasValue, i.DateDebut.Value.ToString("dd/MM/yyyy"), ""),
                                     If(i.DateFin.HasValue, i.DateFin.Value.ToString("dd/MM/yyyy"), ""),
                                     i.AfficherSurCachet)

            ' Type désactivé depuis : la liste de CETTE ligne le garde, sinon la cellule resterait vide.
            Dim typeLigne = _typesIdentifiant.FirstOrDefault(Function(t) t.IdTypeIdentifiant = i.IdTypeIdentifiant)
            If typeLigne IsNot Nothing AndAlso Not typeLigne.Actif Then
                Dim cellule = DirectCast(dgvIdentifiants.Rows(index).Cells("colType"), DataGridViewComboBoxCell)
                cellule.DisplayMember = NameOf(TypeIdentifiantPro.LibelleTypeIdentifiant)
                cellule.ValueMember = NameOf(TypeIdentifiantPro.IdTypeIdentifiant)
                cellule.DataSource = TypesProposes(typeLigne)
                cellule.Value = i.IdTypeIdentifiant
            End If
        Next
    End Sub

    ' Types proposés dans la liste : les actifs, plus le type désactivé d'une ligne existante.
    Private Function TypesProposes(typeCourant As TypeIdentifiantPro) As List(Of TypeIdentifiantPro)
        Return _typesIdentifiant.Where(Function(t) t.Actif OrElse t Is typeCourant).ToList()
    End Function

    Private Sub AfficherQualifications()
        dgvQualifications.Rows.Clear()
        If _praticien.IdPraticien = 0UL Then Return
        For Each q In GestionCabinet.GetQualifications(_praticien.IdPraticien)
            dgvQualifications.Rows.Add(q.IdQualification, q.Libelle, q.Etablissement,
                                       If(q.AnneeObtention.HasValue, q.AnneeObtention.Value.ToString(), ""),
                                       q.OrdreAffichage, q.AfficherSurCachet)
        Next
    End Sub

    Private Sub AfficherImages()
        UtilsImages.RemplacerImage(picLogo, If(_cabinet.IdCabinet > 0UL, CheminsCabinetHelper.ChargerImage(0UL, _cabinet.CheminLogo), Nothing))
        If _praticien.IdPraticien > 0UL Then
            UtilsImages.RemplacerImage(picPhoto, CheminsCabinetHelper.ChargerImage(_praticien.IdPraticien, _praticien.CheminPhoto))
            UtilsImages.RemplacerImage(picSignature, CheminsCabinetHelper.ChargerImage(_praticien.IdPraticien, _praticien.CheminSignature))
            UtilsImages.RemplacerImage(picCachet, CheminsCabinetHelper.ChargerImage(_praticien.IdPraticien, _praticien.CheminCachet))
        Else
            UtilsImages.RemplacerImage(picPhoto, Nothing)
            UtilsImages.RemplacerImage(picSignature, Nothing)
            UtilsImages.RemplacerImage(picCachet, Nothing)
        End If
    End Sub


    ' Consultation : champs en lecture seule, grilles verrouillées, imports désactivés, seul Modifier est proposé.
    ' Édition : saisie libre ; Enregistrer / Annuler remplacent Modifier. Les imports d'images restent
    ' conditionnés à l'existence en base du cabinet / du praticien (dossier de stockage).
    Private Sub AppliquerMode(edition As Boolean)
        _enEdition = edition

        For Each ctrl In TousLesControles(tabCabinet)
            If TypeOf ctrl Is TextBox Then
                DirectCast(ctrl, TextBox).ReadOnly = Not edition
            ElseIf TypeOf ctrl Is ComboBox Then
                ctrl.Enabled = edition
            End If
        Next

        ' Onglets Cabinet et Praticien : consultation lisible (texte normal sur fond teinté), zones
        ' blanches en saisie.
        For Each carte As AltheaCarte In {carteCabIdentite, carteCabAdresse, carteCabCoordonnees, carteCabFacturation,
                                          carteIdentite, carteCoordonnees, carteAdresse}
            UtilsControls.DefinirLectureSeule(carte, Not edition)
        Next

        ' Le BCE du cabinet n'est désactivé qu'en saisie (AppliquerPorteurBce) : en consultation il reste lisible.
        If Not edition Then txtCabBce.Enabled = True

        For Each dgv As DataGridView In {dgvIdentifiants, dgvQualifications}
            dgv.ReadOnly = Not edition
            dgv.AllowUserToAddRows = edition
            dgv.AllowUserToDeleteRows = edition
        Next

        btnModifier.Visible = Not edition
        btnEnregistrer.Visible = edition
        btnAnnuler.Visible = edition

        MettreAJourEtat()
        If edition Then AppliquerPorteurBce()
    End Sub

    Private Sub MettreAJourEtat()
        Dim cabinetOk = _cabinet.IdCabinet > 0UL
        Dim praticienOk = _praticien.IdPraticien > 0UL
        btnLogoImporter.Enabled = _enEdition AndAlso cabinetOk
        tpPraticien.Enabled = cabinetOk
        btnPhotoImporter.Enabled = _enEdition AndAlso praticienOk
        btnSignatureImporter.Enabled = _enEdition AndAlso praticienOk
        btnCachetGenerer.Enabled = _enEdition AndAlso praticienOk
        btnCachetImporter.Enabled = _enEdition AndAlso praticienOk
        dgvIdentifiants.Enabled = praticienOk
        dgvQualifications.Enabled = praticienOk
        ' Actions de compte : opérations immédiates et journalisées, indépendantes du mode Modifier/Enregistrer ;
        ' conditions : praticien enregistré, et administrateur (élévation comprise ; audit A.3 — le compte
        ' créé peut s'élever jusqu'à Admin).
        Dim estAdmin As Boolean = RoleCourant() = AppRole.Admin
        Dim raisonCompte As String =
            If(Not praticienOk, "Enregistrez d'abord le praticien.",
               If(Not estAdmin, "Réservé à l'administrateur (élevez vos droits avec le cadenas).", Nothing))
        btnCompteCreer.DefinirDisponibilite(praticienOk AndAlso estAdmin AndAlso Not _praticien.ACompteLie,
                                            If(raisonCompte, "Un compte est déjà lié à ce praticien."))
        btnCompteLier.DefinirDisponibilite(praticienOk AndAlso estAdmin AndAlso Not _praticien.ACompteLie,
                                           If(raisonCompte, "Un compte est déjà lié à ce praticien."))
        btnCompteDelier.DefinirDisponibilite(praticienOk AndAlso estAdmin AndAlso _praticien.ACompteLie,
                                             If(raisonCompte, "Aucun compte n'est lié à ce praticien."))
    End Sub

#End Region

#Region "Enregistrement"

    Private Sub btnEnregistrer_Click(sender As Object, e As EventArgs) Handles btnEnregistrer.Click

        dgvIdentifiants.EndEdit()
        dgvQualifications.EndEdit()

        If String.IsNullOrWhiteSpace(txtNomCabinet.Text) Then
            tabCabinet.SelectedTab = tpCabinet
            DialogChoix.Avertissement("Le nom du cabinet est obligatoire.", "Cabinet")
            txtNomCabinet.Focus()
            Return
        End If

        Dim praticienSaisi = Not String.IsNullOrWhiteSpace(txtPraNom.Text) OrElse Not String.IsNullOrWhiteSpace(txtPraPrenom.Text)
        If praticienSaisi AndAlso (String.IsNullOrWhiteSpace(txtPraNom.Text) OrElse String.IsNullOrWhiteSpace(txtPraPrenom.Text)) Then
            tabCabinet.SelectedTab = tpPraticien
            DialogChoix.Avertissement("Le nom et le prénom du praticien sont obligatoires.", "Cabinet")
            Return
        End If

        If Not ValiderChamps() Then Return
        If Not ConfirmerLocalite(tpCabinet, _saisieLocaliteCabinet) OrElse
           Not ConfirmerLocalite(tpPraticien, _saisieLocalitePraticien) Then Return

        Dim identifiants As List(Of PraticienIdentifiant) = Nothing
        Dim qualifications As List(Of PraticienQualification) = Nothing
        Dim erreur As String = Nothing
        If Not LireIdentifiants(identifiants, erreur) OrElse Not LireQualifications(qualifications, erreur) Then
            tabCabinet.SelectedTab = tpIdentifiants
            DialogChoix.Avertissement(erreur, "Cabinet")
            Return
        End If

        Dim imagesEnregistrees As Boolean

        Try
            Cursor = Cursors.WaitCursor

            ' Fiche entière en une transaction : en cas d'échec, rien n'est écrit.
            LireCabinet()
            If praticienSaisi Then LirePraticien()
            GestionCabinet.EnregistrerFiche(_cabinet, If(praticienSaisi, _praticien, Nothing),
                                            identifiants, _identifiantsSupprimes,
                                            qualifications, _qualificationsSupprimees)

            imagesEnregistrees = EnregistrerImagesEnAttente()

            GestionLog.EcrireLog("Informations du cabinet enregistrées.", GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)
            Charger()
            RafraichirBandeauHome()
            _context?.SetStatus("Informations du cabinet enregistrées.")

        Catch ex As Exception
            GestionLog.EcrireLog("Erreur lors de l'enregistrement du cabinet.", GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
            DialogChoix.Erreur("Rien n'a été enregistré : votre saisie est toujours à l'écran." & vbCrLf &
                               "Vérifiez la connexion à la base de données, puis cliquez de nouveau sur Enregistrer." & vbCrLf &
                               "Le détail est dans le journal.", "Cabinet")
            Return
        Finally
            Cursor = Cursors.Default
        End Try

        If Not imagesEnregistrees Then
            DialogChoix.Avertissement("La fiche est enregistrée, mais une image n'a pas pu l'être (dossier de données inaccessible ?)." & vbCrLf &
                                      "Le détail est dans le journal ; choisissez-la de nouveau avec Modifier.", "Cabinet")
        End If

    End Sub

    Private Sub btnModifier_Click(sender As Object, e As EventArgs) Handles btnModifier.Click
        AppliquerMode(True)
        _context?.SetStatus("Modification des informations du cabinet.")
    End Sub

    Private Sub btnAnnuler_Click(sender As Object, e As EventArgs) Handles btnAnnuler.Click
        If _isDirty AndAlso Not DialogChoix.ConfirmerAbandon("du cabinet") Then Return
        Charger()
    End Sub

    Private Function ValiderChamps() As Boolean
        Dim msg As String = Nothing

        Dim paysCabinet As String = UtilsSaisieReferentiels.CodePaysSaisi(cboCabPays)
        msg = _saisieLocaliteCabinet.MessageCodePostal()
        If msg.Length > 0 Then Return Signaler(tpCabinet, txtCabCp, msg)
        If Not UtilsTelephone.Valider(txtCabTelephone.Text, paysCabinet, msg) Then Return Signaler(tpCabinet, txtCabTelephone, msg)
        If Not UtilsValidation.IsValidEmail(txtCabEmail.Text, msg) Then Return Signaler(tpCabinet, txtCabEmail, msg)
        If txtCabBce.Enabled AndAlso Not UtilsIdentifiantBelge.Valider(UtilsIdentifiantBelge.CodeBce, Nothing, txtCabBce.Text, msg) Then Return Signaler(tpCabinet, txtCabBce, msg)
        If Not ValiderPorteurBce() Then Return False
        If Not UtilsIdentifiantBelge.ValiderIban(txtCabIban.Text, msg) Then Return Signaler(tpCabinet, txtCabIban, msg)
        If Not UtilsIdentifiantBelge.ValiderBic(txtCabBic.Text, msg) Then Return Signaler(tpCabinet, txtCabBic, msg)
        If Not UtilsIdentifiantBelge.Valider(UtilsIdentifiantBelge.CodeTva, Nothing, txtCabTva.Text, msg) Then Return Signaler(tpCabinet, txtCabTva, msg)
        Dim paysPraticien As String = PaysPraticienSaisi()
        msg = _saisieLocalitePraticien.MessageCodePostal()
        If msg.Length > 0 Then Return Signaler(tpPraticien, txtPraCp, msg)
        If Not UtilsTelephone.Valider(txtPraTelephone.Text, paysPraticien, msg) Then Return Signaler(tpPraticien, txtPraTelephone, msg)
        If Not UtilsValidation.IsValidEmail(txtPraEmail.Text, msg) Then Return Signaler(tpPraticien, txtPraEmail, msg)
        Return True
    End Function

    ' Les modèles Cabinet / Praticien codent « non renseigné » par 0 ; les listes attendent Nothing.
    Private Shared Function IdOuNothing(id As ULong) As ULong?
        Return If(id = 0UL, CType(Nothing, ULong?), id)
    End Function

    ' Praticien sans pays propre : son adresse est celle du cabinet, contrôlée selon le pays du cabinet.
    Private Function PaysPraticienSaisi() As String
        Return If(UtilsSaisieReferentiels.CodePaysSaisi(cboPraPays), UtilsSaisieReferentiels.CodePaysSaisi(cboCabPays))
    End Function

    ' Localité qui ne correspond pas au code postal belge : l'onglet de l'adresse est affiché, puis la
    ' question « Enregistrer quand même / Corriger » (SaisieLocaliteBelge).
    Private Function ConfirmerLocalite(onglet As TabPage, saisie As SaisieLocaliteBelge) As Boolean
        If Not saisie.LocaliteAConfirmer() Then Return True
        tabCabinet.SelectedTab = onglet
        Return saisie.ConfirmerLocalite()
    End Function

    Private Function Signaler(onglet As TabPage, ctrl As Control, message As String) As Boolean
        tabCabinet.SelectedTab = onglet
        DialogChoix.Avertissement(message, "Cabinet")
        ctrl.Focus()
        Return False
    End Function

    Private Sub LireCabinet()
        _cabinet.NomCabinet = txtNomCabinet.Text.Trim()
        _cabinet.IdSituationJuridique = If(SituationSelectionnee()?.IdSituationJuridique, 0UL)
        _cabinet.AdresseRue = txtCabRue.Text
        _cabinet.AdresseNumero = txtCabNumero.Text
        _cabinet.CodePostal = txtCabCp.Text
        _cabinet.Localite = txtCabLocalite.Text
        _cabinet.IdPays = If(UtilsSaisieReferentiels.IdPaysSaisi(cboCabPays), 0UL)
        ' Libellé (lu par la base d'habitude) : le cachet généré depuis la saisie l'affiche.
        _cabinet.LibellePays = If(_cabinet.IdPays = 0UL, String.Empty, UtilsSaisieReferentiels.PaysSaisi(cboCabPays)?.LibellePays)
        _cabinet.Telephone = txtCabTelephone.Text
        _cabinet.Email = txtCabEmail.Text
        _cabinet.SiteWeb = txtCabSite.Text
        _cabinet.NumeroBce = txtCabBce.Text
        _cabinet.NumeroTva = txtCabTva.Text
        _cabinet.Iban = UtilsIdentifiantBelge.IbanCompact(txtCabIban.Text)
        _cabinet.Bic = UtilsIdentifiantBelge.FormaterBic(txtCabBic.Text)
    End Sub

    Private Sub LirePraticien()
        _praticien.IdCivilite = If(UtilsSaisieReferentiels.IdCiviliteSaisie(cboPraCivilite), 0UL)
        _praticien.Nom = txtPraNom.Text.Trim()
        _praticien.Prenom = txtPraPrenom.Text.Trim()
        _praticien.TitreProfessionnel = txtPraTitre.Text
        _praticien.Telephone = txtPraTelephone.Text
        _praticien.Email = txtPraEmail.Text
        _praticien.SiteWeb = txtPraSite.Text
        _praticien.AdresseRue = txtPraRue.Text
        _praticien.AdresseNumero = txtPraNumero.Text
        _praticien.CodePostal = txtPraCp.Text
        _praticien.Localite = txtPraLocalite.Text
        _praticien.IdPays = If(UtilsSaisieReferentiels.IdPaysSaisi(cboPraPays), 0UL)
        _praticien.LibellePays = If(_praticien.IdPays = 0UL, String.Empty, UtilsSaisieReferentiels.PaysSaisi(cboPraPays)?.LibellePays)
        _praticien.Actif = True
    End Sub

    Private Function LireDateCellule(valeur As Object, ByRef resultat As Date?) As Boolean
        Dim texte = Convert.ToString(valeur)?.Trim()
        resultat = Nothing
        If String.IsNullOrEmpty(texte) Then Return True
        Dim d As Date
        If Date.TryParseExact(texte, {"dd/MM/yyyy", "d/M/yyyy"}, Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
            resultat = d
            Return True
        End If
        Return False
    End Function

    Private Function LireIdentifiants(ByRef liste As List(Of PraticienIdentifiant), ByRef erreur As String) As Boolean
        liste = New List(Of PraticienIdentifiant)
        For Each row As DataGridViewRow In dgvIdentifiants.Rows
            If row.IsNewRow Then Continue For
            Dim valeur = Convert.ToString(row.Cells("colValeur").Value)?.Trim()
            Dim type = row.Cells("colType").Value
            If String.IsNullOrEmpty(valeur) AndAlso type Is Nothing Then Continue For
            If String.IsNullOrEmpty(valeur) OrElse type Is Nothing Then
                erreur = $"Identifiant ligne {row.Index + 1} : le type et la valeur sont obligatoires."
                Return False
            End If
            Dim typeRef = _typesIdentifiant.FirstOrDefault(Function(t) t.IdTypeIdentifiant = Convert.ToUInt64(type))
            Dim msgIdent As String = Nothing
            If typeRef IsNot Nothing AndAlso Not UtilsIdentifiantBelge.Valider(typeRef.CodeTypeIdentifiant, typeRef.RegexValidation, valeur, msgIdent) Then
                erreur = $"Identifiant ligne {row.Index + 1} ({typeRef.LibelleTypeIdentifiant}) : {msgIdent}"
                ' Les messages d'INAMI, BCE et TVA donnent déjà le format ; sinon, rappel du masque du type.
                If Not UtilsIdentifiantBelge.EstControleParAlthea(typeRef.CodeTypeIdentifiant) AndAlso
                   Not String.IsNullOrWhiteSpace(typeRef.MasqueSaisie) Then
                    erreur &= $" Format attendu : {typeRef.MasqueSaisie}."
                End If
                Return False
            End If
            Dim debut As Date?, fin As Date?
            If Not LireDateCellule(row.Cells("colDebut").Value, debut) OrElse Not LireDateCellule(row.Cells("colFin").Value, fin) Then
                erreur = $"Identifiant ligne {row.Index + 1} : date invalide (format jj/mm/aaaa)."
                Return False
            End If
            Dim idTexte = Convert.ToString(row.Cells("colIdIdent").Value)
            liste.Add(New PraticienIdentifiant With {
                .IdIdentifiant = If(String.IsNullOrEmpty(idTexte), 0UL, ULong.Parse(idTexte)),
                .IdTypeIdentifiant = Convert.ToUInt64(type),
                .LibelleType = typeRef?.LibelleTypeIdentifiant,
                .CodeType = typeRef?.CodeTypeIdentifiant,
                .Valeur = valeur,
                .DateDebut = debut,
                .DateFin = fin,
                .AfficherSurCachet = Convert.ToBoolean(If(row.Cells("colCachetIdent").Value, False))
            })
        Next
        Return True
    End Function

    Private Function LireQualifications(ByRef liste As List(Of PraticienQualification), ByRef erreur As String) As Boolean
        liste = New List(Of PraticienQualification)
        For Each row As DataGridViewRow In dgvQualifications.Rows
            If row.IsNewRow Then Continue For
            Dim libelle = Convert.ToString(row.Cells("colLibelle").Value)?.Trim()
            If String.IsNullOrEmpty(libelle) Then
                erreur = $"Qualification ligne {row.Index + 1} : le libellé est obligatoire."
                Return False
            End If
            Dim anneeTexte = Convert.ToString(row.Cells("colAnnee").Value)?.Trim()
            Dim annee As Integer
            If Not String.IsNullOrEmpty(anneeTexte) AndAlso Not Integer.TryParse(anneeTexte, annee) Then
                erreur = $"Qualification ligne {row.Index + 1} : année invalide."
                Return False
            End If
            Dim ordre As Integer
            Integer.TryParse(Convert.ToString(row.Cells("colOrdre").Value), ordre)
            Dim idTexte = Convert.ToString(row.Cells("colIdQualif").Value)
            liste.Add(New PraticienQualification With {
                .IdQualification = If(String.IsNullOrEmpty(idTexte), 0UL, ULong.Parse(idTexte)),
                .Libelle = libelle,
                .Etablissement = Convert.ToString(row.Cells("colEtablissement").Value),
                .AnneeObtention = If(String.IsNullOrEmpty(anneeTexte), CType(Nothing, Integer?), annee),
                .OrdreAffichage = ordre,
                .AfficherSurCachet = Convert.ToBoolean(If(row.Cells("colCachetQualif").Value, False))
            })
        Next
        Return True
    End Function

    Private Sub dgvIdentifiants_UserDeletingRow(sender As Object, e As DataGridViewRowCancelEventArgs) Handles dgvIdentifiants.UserDeletingRow
        Dim idTexte = Convert.ToString(e.Row.Cells("colIdIdent").Value)
        If Not String.IsNullOrEmpty(idTexte) Then _identifiantsSupprimes.Add(ULong.Parse(idTexte))
    End Sub

    Private Sub dgvQualifications_UserDeletingRow(sender As Object, e As DataGridViewRowCancelEventArgs) Handles dgvQualifications.UserDeletingRow
        Dim idTexte = Convert.ToString(e.Row.Cells("colIdQualif").Value)
        If Not String.IsNullOrEmpty(idTexte) Then _qualificationsSupprimees.Add(ULong.Parse(idTexte))
    End Sub

    Private Sub dgv_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles dgvIdentifiants.DataError, dgvQualifications.DataError
        e.ThrowException = False
    End Sub

#End Region

#Region "Images"

    ' -------------------------------------------------------------------------------------------------
    ' Images en attente (décision de Joëlle, 20/10/2026 : « tout différer »)
    '
    ' Une image choisie ou un cachet généré n'est écrit qu'avec la fiche (Enregistrer) ; Annuler
    ' l'abandonne, comme le logo des domaines. Rien n'étant définitif avant Enregistrer, plus de
    ' confirmation de remplacement. Clé : nom fixe du fichier (CheminsCabinetHelper).
    ' -------------------------------------------------------------------------------------------------
    Private Class ImageEnAttente
        ' Fichier choisi, copié tel quel à l'enregistrement (format d'origine gardé)
        Public Property Source As String
        ' Cachet généré, écrit en PNG à l'enregistrement
        Public Property Generee As Image
    End Class

    Private ReadOnly _imagesEnAttente As New Dictionary(Of String, ImageEnAttente)

    Private Sub MettreEnAttente(nomFixe As String, attente As ImageEnAttente, pic As PictureBox, apercu As Image)
        OublierImageEnAttente(nomFixe)
        _imagesEnAttente(nomFixe) = attente
        UtilsImages.RemplacerImage(pic, apercu)
        DefinirModifie(True)
    End Sub

    Private Sub OublierImageEnAttente(nomFixe As String)
        Dim attente As ImageEnAttente = Nothing
        If _imagesEnAttente.TryGetValue(nomFixe, attente) Then
            attente.Generee?.Dispose()
            _imagesEnAttente.Remove(nomFixe)
        End If
    End Sub

    ' Fiche rechargée (enregistrée ou abandonnée) ou écran fermé : plus rien en attente.
    Private Sub OublierImagesEnAttente()
        For Each attente In _imagesEnAttente.Values
            attente.Generee?.Dispose()
        Next
        _imagesEnAttente.Clear()
    End Sub

    ' Dialogue commun (fichier contrôlé), aperçu dans la carte, écriture à l'enregistrement.
    Private Sub ChoisirImageEnAttente(nomFixe As String, titre As String, pic As PictureBox)
        Dim source As String = UtilsImages.ChoisirFichierImage(Me, titre)
        If source Is Nothing Then Return

        Dim apercu As Image = ImagesHelper.ChargerSansVerrou(source)
        If apercu Is Nothing Then
            DialogChoix.Erreur("Cette image n'a pas pu être lue. Choisissez un autre fichier.", titre)
            Return
        End If

        MettreEnAttente(nomFixe, New ImageEnAttente With {.Source = source}, pic, apercu)
        _context?.SetStatus("Image choisie : elle sera enregistrée avec la fiche (Enregistrer).")
    End Sub

    ' Rôle courant de la session (élévation comprise), transmis au métier pour le compte du praticien.
    Private Function RoleCourant() As AppRole
        Return If(_context Is Nothing, AppRole.User, _context.RoleCourant)
    End Function

    ' Le bandeau de la Home affiche le logo, le nom du cabinet et le praticien : il suit les données.
    Private Sub RafraichirBandeauHome()
        _context?.FenetrePrincipale?.RafraichirBandeauCabinet()
    End Sub

    Private Sub btnLogoImporter_Click(sender As Object, e As EventArgs) Handles btnLogoImporter.Click
        ChoisirImageEnAttente(CheminsCabinetHelper.NomLogo, "Logo du cabinet", picLogo)
    End Sub

    Private Sub btnPhotoImporter_Click(sender As Object, e As EventArgs) Handles btnPhotoImporter.Click
        ChoisirImageEnAttente(CheminsCabinetHelper.NomPhoto, "Photo du praticien", picPhoto)
    End Sub

    Private Sub btnSignatureImporter_Click(sender As Object, e As EventArgs) Handles btnSignatureImporter.Click
        ChoisirImageEnAttente(CheminsCabinetHelper.NomSignature, "Signature", picSignature)
    End Sub

    Private Sub btnCachetImporter_Click(sender As Object, e As EventArgs) Handles btnCachetImporter.Click
        ChoisirImageEnAttente(CheminsCabinetHelper.NomCachet, "Scan du cachet", picCachet)
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnCachetGenerer_Click
    ' Version   : V2.0.0
    ' Date      : 20/10/2026
    '
    ' Rôle      : Génère le cachet depuis la SAISIE EN COURS (fiches Cabinet et Praticien, lignes
    '             d'identifiants et de qualifications cochées « Cachet », même non enregistrées) ; il
    '             sera écrit avec la fiche.
    ' Remarques :
    ' - V2.0.0 (20/10/2026) : auparavant généré depuis la base et enregistré aussitôt (ToDo).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnCachetGenerer_Click(sender As Object, e As EventArgs) Handles btnCachetGenerer.Click

        dgvIdentifiants.EndEdit()
        dgvQualifications.EndEdit()

        If String.IsNullOrWhiteSpace(txtPraNom.Text) OrElse String.IsNullOrWhiteSpace(txtPraPrenom.Text) Then
            tabCabinet.SelectedTab = tpPraticien
            DialogChoix.Avertissement("Saisissez le nom et le prénom du praticien, puis générez le cachet.", "Cachet")
            Return
        End If

        Dim identifiants As List(Of PraticienIdentifiant) = Nothing
        Dim qualifications As List(Of PraticienQualification) = Nothing
        Dim erreur As String = Nothing
        If Not LireIdentifiants(identifiants, erreur) OrElse Not LireQualifications(qualifications, erreur) Then
            tabCabinet.SelectedTab = tpIdentifiants
            DialogChoix.Avertissement(erreur & vbCrLf & "Corrigez cette ligne, puis générez le cachet.", "Cachet")
            Return
        End If

        Try
            LireCabinet()
            LirePraticien()
            Dim cachet As Bitmap = GenerateurCachet.Generer(_cabinet, _praticien, identifiants, qualifications)
            MettreEnAttente(CheminsCabinetHelper.NomCachet, New ImageEnAttente With {.Generee = cachet},
                            picCachet, New Bitmap(cachet))
            _context?.SetStatus("Cachet généré depuis la saisie en cours : il sera enregistré avec la fiche (Enregistrer).")
        Catch ex As Exception
            SignalerErreurImage(ex)
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : EnregistrerImagesEnAttente
    ' Version   : V1.0.0
    ' Date      : 20/10/2026
    '
    ' Rôle      : Après la fiche, écrit les images en attente (fichiers), puis leurs noms en base
    '             (GestionCabinet.EnregistrerImages). False si l'une n'a pas pu l'être (journalisé) :
    '             la fiche reste enregistrée.
    ' Remarques : Fichier d'abord, nom ensuite : la base ne cite pas un fichier absent. Une image
    '             du praticien attend qu'il soit enregistré (dossier Praticien_{id}).
    ' -------------------------------------------------------------------------------------------------
    Private Function EnregistrerImagesEnAttente() As Boolean

        If _imagesEnAttente.Count = 0 Then Return True

        Dim toutesEcrites As Boolean = True
        Dim uneEcrite As Boolean = False

        For Each paire In _imagesEnAttente
            Dim nomFixe As String = paire.Key
            Dim pourCabinet As Boolean = nomFixe = CheminsCabinetHelper.NomLogo
            Dim idPraticien As ULong = If(pourCabinet, 0UL, _praticien.IdPraticien)
            If Not pourCabinet AndAlso idPraticien = 0UL Then Continue For

            Try
                Dim nomFichier As String =
                    If(paire.Value.Generee IsNot Nothing,
                       CheminsCabinetHelper.EnregistrerImagePng(idPraticien, nomFixe, paire.Value.Generee),
                       CheminsCabinetHelper.ImporterImage(idPraticien, nomFixe, paire.Value.Source))

                Select Case nomFixe
                    Case CheminsCabinetHelper.NomLogo : _cabinet.CheminLogo = nomFichier
                    Case CheminsCabinetHelper.NomPhoto : _praticien.CheminPhoto = nomFichier
                    Case CheminsCabinetHelper.NomSignature : _praticien.CheminSignature = nomFichier
                    Case CheminsCabinetHelper.NomCachet : _praticien.CheminCachet = nomFichier
                End Select
                uneEcrite = True

            Catch ex As Exception
                toutesEcrites = False
                GestionLog.EcrireException($"Image « {nomFixe} » du cabinet non enregistrée.", ex,
                                           GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI)
            End Try
        Next

        If Not uneEcrite Then Return toutesEcrites

        Try
            GestionCabinet.EnregistrerImages(_cabinet, _praticien)
        Catch ex As Exception
            GestionLog.EcrireException("Noms des images du cabinet non enregistrés.", ex,
                                       GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database)
            Return False
        End Try

        Return toutesEcrites

    End Function

    Private Sub SignalerErreurImage(ex As Exception)
        GestionLog.EcrireLog("Erreur image cabinet.", GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
        DialogChoix.Erreur("Opération sur l'image impossible." & vbCrLf &
                           "Le détail est dans le journal.", "Cabinet")
    End Sub

#End Region

#Region "Compte de travail (lot 28)"

    ' Opérations immédiates et journalisées, indépendantes du cycle Modifier/Enregistrer.
    ' Le compte Admin technique n'est jamais touché : seuls des SuperUser sont créés ou liés.
    Private Sub btnCompteCreer_Click(sender As Object, e As EventArgs) Handles btnCompteCreer.Click
        Using dlg As New DialogComptePraticien(DialogComptePraticien.ModeDialog.Creation, _praticien, _context?.ContexteOperation)
            If dlg.ShowDialog(Me) <> DialogResult.OK Then Return
            RafraichirCompte()
            Try
                Clipboard.SetText(dlg.MotDePasseTemporaire)
            Catch
            End Try
            DialogChoix.Succes(
                $"Le compte « {_praticien.UtilisateurLogin} » a été créé et lié au praticien." & vbCrLf & vbCrLf &
                $"Mot de passe temporaire : {dlg.MotDePasseTemporaire}" & vbCrLf & vbCrLf &
                "✓ Copié dans le presse-papiers." & vbCrLf &
                "✓ À changer à la première connexion.",
                "Compte créé")
        End Using
    End Sub

    Private Sub btnCompteLier_Click(sender As Object, e As EventArgs) Handles btnCompteLier.Click
        Using dlg As New DialogComptePraticien(DialogComptePraticien.ModeDialog.Liaison, _praticien, _context?.ContexteOperation)
            If dlg.ShowDialog(Me) <> DialogResult.OK Then Return
        End Using
        _context?.SetStatus("Compte lié au praticien.")
        RafraichirCompte()
    End Sub

    Private Sub btnCompteDelier_Click(sender As Object, e As EventArgs) Handles btnCompteDelier.Click
        If DialogChoix.Confirmer(
            $"Délier le compte « {_praticien.UtilisateurLogin} » de ce praticien ?" & vbCrLf & vbCrLf &
            "Le compte utilisateur lui-même n'est ni supprimé ni désactivé.",
            "Délier le compte") <> DialogResult.Yes Then Return
        Try
            GestionCabinet.LierUtilisateur(_context?.ContexteOperation, _praticien, 0UL)
            _context?.SetStatus("Compte délié du praticien.")
        Catch ex As Exception
            GestionLog.EcrireLog("Erreur déliaison compte praticien.", GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security, ex)
            DialogChoix.Erreur("Déliaison impossible : " & ex.Message)
        End Try
        RafraichirCompte()
    End Sub

    ' Recharge le praticien pour récupérer les colonnes jointes du compte (login, rôle, état).
    Private Sub RafraichirCompte()
        Dim recharge = GestionCabinet.GetPraticiens(_cabinet.IdCabinet, False).FirstOrDefault(Function(p) p.IdPraticien = _praticien.IdPraticien)
        If recharge IsNot Nothing Then
            _praticien.IdUtilisateur = recharge.IdUtilisateur
            _praticien.UtilisateurCode = recharge.UtilisateurCode
            _praticien.UtilisateurLogin = recharge.UtilisateurLogin
            _praticien.UtilisateurNomAffichage = recharge.UtilisateurNomAffichage
            _praticien.UtilisateurRole = recharge.UtilisateurRole
            _praticien.UtilisateurActif = recharge.UtilisateurActif
        End If
        AfficherCompte()
        MettreAJourEtat()
    End Sub

#End Region

End Class
