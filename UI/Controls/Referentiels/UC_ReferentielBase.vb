' -------------------------------------------------------------------------------------------------
' UserControl : UC_ReferentielBase
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Classe de base héritable pour tous les écrans de gestion de référentiels (tables ref_*).
' Centralise la mécanique commune : chargement, recherche, modes (consultation / création /
' modification), validation, activation/désactivation (soft-delete), droits et journalisation.
'
' Responsabilités :
' - Afficher la liste des éléments du référentiel dans une grille générique
' - Gérer la recherche et l'affichage des éléments inactifs
' - Gérer le panneau d'édition générique (Code, Libellé, Ordre, Actif)
' - Orchestrer les modes (consultation, création, modification) et l'état des boutons
' - Valider les saisies (champs requis, longueurs, unicité du code et du libellé)
' - Appliquer les droits d'accès selon le rôle de l'utilisateur courant
' - Journaliser les actions via GestionLog
'
' Remarques   :
' - Implémente IContextAwareUserControl (injection du contexte UI par Home/NavigationManager)
' - Ne contient AUCUN accès direct à la base de données : tout passe par les points
'   d'extension Overridable surchargés par les classes dérivées (ex. UC_Domaines).
' - N'est pas MustInherit afin de rester compatible avec le concepteur Windows Forms.
'
' Dépendances :
' - UserControlContext (contexte UI partagé)
' - IContextAwareUserControl (contrat d'injection)
' - ReferentielLigne (modèle de présentation générique)
' - AppRole (gestion des droits)
' - UtilsButtons / UtilsDataGrid (style UI commun)
' - GestionLog (journalisation)
'
' Interface   :
' - IContextAwareUserControl : injection du contexte UI partagé
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_ReferentielBase
    Implements IContextAwareUserControl

#Region "Variables privées"

    ' Contexte UI partagé injecté par Home via SetContext()
    Protected _context As UserControlContext

    ' Liste complète des éléments chargés depuis la base (avant filtrage mémoire)
    Private _lignes As List(Of ReferentielLigne)

    ' Mode courant de l'écran (consultation, création, modification)
    Private _mode As ModeReferentiel = ModeReferentiel.Consultation

    ' Identifiant de l'élément en cours d'édition (0 = aucun / création)
    Private _idEnCours As ULong = 0

    ' Garde anti-réentrance lors du nettoyage automatique du code (txtCode_TextChanged)
    Private _miseAJourCode As Boolean = False

#End Region

#Region "Points d'extension - Métadonnées (Overridable)"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : TitreReferentiel
    ' Rôle      : Titre affiché dans l'en-tête (ex. "Domaines"). À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable ReadOnly Property TitreReferentiel As String
        Get
            Return "Référentiel"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : SousTitreReferentiel
    ' Rôle      : Sous-titre descriptif affiché sous le titre. À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les valeurs de référence."
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : CheminContexte
    ' Rôle      : Fil d'Ariane affiché dans l'en-tête Home (ex. "Référentiels > Domaines"). À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : RoleMinimum
    ' Rôle      : Rôle minimal requis pour modifier le référentiel (SuperUser par défaut). À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable ReadOnly Property RoleMinimum As AppRole
        Get
            Return AppRole.SuperUser
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : LongueurMaxCode
    ' Rôle      : Longueur maximale autorisée pour le code (varie selon la table ref_*). À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 10
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : RemplacerEspacesParUnderscore
    ' Rôle      : Indique si les espaces saisis dans le code doivent être remplacés par des underscores.
    ' Remarque  : Activé par défaut pour les codes longs (multi-mots). À surcharger (ex. False pour
    '             les codes courts strictement alphanumériques comme les domaines).
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable ReadOnly Property RemplacerEspacesParUnderscore As Boolean
        Get
            Return True
        End Get
    End Property

#End Region

#Region "Points d'extension - Données (Overridable)"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ChargerElements
    ' Rôle     : Retourne les éléments du référentiel sous forme de lignes génériques.
    ' Paramètre: afficherInactifs - True pour inclure les éléments désactivés.
    ' Remarque : À surcharger par la classe dérivée (appel à sa couche métier).
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)
        Return New List(Of ReferentielLigne)()
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CodeExisteDeja
    ' Rôle     : Indique si un code existe déjà (hors élément exclu). À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return False
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LibelleExisteDeja
    ' Rôle     : Indique si un libellé existe déjà (hors élément exclu). À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return False
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InsererElement
    ' Rôle      : Insère un nouvel élément dans le référentiel. À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)
        ' Implémentation fournie par la classe dérivée
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourElement
    ' Rôle      : Met à jour un élément existant du référentiel. À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)
        ' Implémentation fournie par la classe dérivée
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirActivation
    ' Rôle      : Active ou désactive un élément (soft-delete). À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub DefinirActivation(id As ULong, actif As Boolean)
        ' Implémentation fournie par la classe dérivée
    End Sub

#End Region

#Region "Points d'extension - Champ supplémentaire (Overridable)"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ConfigurerChampSupplementaire
    ' Rôle      : Permet à la classe dérivée d'ajouter un champ d'édition supplémentaire au panneau
    '             d'édition (ex. tarif pour ref_types_seance). Appelée une fois au chargement.
    ' Remarque  : Implémentation vide par défaut (aucun champ supplémentaire).
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub ConfigurerChampSupplementaire()
        ' Aucun champ supplémentaire par défaut.
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherChampSupplementaire
    ' Rôle      : Affiche la valeur du champ supplémentaire à partir de la ligne sélectionnée.
    ' Remarque  : Implémentation vide par défaut.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub AfficherChampSupplementaire(ligne As ReferentielLigne)
        ' Aucun champ supplémentaire par défaut.
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ViderChampSupplementaire
    ' Rôle      : Réinitialise le champ supplémentaire (mode création / vidage du détail).
    ' Remarque  : Implémentation vide par défaut.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub ViderChampSupplementaire()
        ' Aucun champ supplémentaire par défaut.
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ActiverChampSupplementaire
    ' Rôle      : Active ou désactive le champ supplémentaire selon le mode d'édition.
    ' Remarque  : Implémentation vide par défaut.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub ActiverChampSupplementaire(enEdition As Boolean)
        ' Aucun champ supplémentaire par défaut.
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ValiderChampSupplementaire
    ' Rôle     : Valide le champ supplémentaire avant enregistrement.
    ' Retour   : True si valide (par défaut, aucun champ à valider).
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Function ValiderChampSupplementaire() As Boolean
        Return True
    End Function

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SetContext
    ' Rôle      : Reçoit le contexte UI partagé injecté par Home.
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) _
        Implements IContextAwareUserControl.SetContext

        _context = context

    End Sub

#End Region

#Region "Chargement UserControl"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : UC_ReferentielBase_Load
    ' Rôle      : Initialise l'écran, applique les droits et charge la liste.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_ReferentielBase_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Évite l'exécution au sein du concepteur Windows Forms
        If DesignMode Then Exit Sub

        InitialiserEntete()
        InitialiserBoutons()
        InitialiserDataGridView()
        InitialiserToolTips()
        ConfigurerChampSupplementaire()

        If _context IsNot Nothing Then
            _context.SetHeader(CheminContexte)
        End If

        PasserEnMode(ModeReferentiel.Consultation)
        AppliquerDroitsUtilisateur()
        ChargerListe()

    End Sub

#End Region

#Region "Initialisation UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserEntete
    ' Rôle      : Applique le titre et le sous-titre fournis par la classe dérivée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserEntete()
        lblTitreForm.Text = TitreReferentiel
        lblTop.Text = SousTitreReferentiel
        lblTitreEdition.Text = "Détail"
        txtCode.MaxLength = LongueurMaxCode
        txtCode.CharacterCasing = CharacterCasing.Upper
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserBoutons
    ' Rôle      : Initialise les boutons standards (icônes + style Althéa).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserBoutons()
        UtilsButtons.InitStandardButton(btnNouveau)
        UtilsButtons.InitStandardButton(btnModifier)
        UtilsButtons.InitStandardButton(btnActiverDesactiver)
        UtilsButtons.InitStandardButton(btnActualiser)
        UtilsButtons.InitStandardButton(btnEnregistrer)
        UtilsButtons.InitStandardButton(btnAnnuler)
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserDataGridView
    ' Rôle      : Configure la grille générique et lie les colonnes à ReferentielLigne.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserDataGridView()

        UtilsDataGrid.InitDataGridBasique(dgvReferentiel)

        dgvReferentiel.AutoGenerateColumns = False
        dgvReferentiel.ReadOnly = True
        dgvReferentiel.MultiSelect = False
        dgvReferentiel.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvReferentiel.AllowUserToAddRows = False
        dgvReferentiel.AllowUserToDeleteRows = False

        colId.DataPropertyName = NameOf(ReferentielLigne.IdReferentiel)
        colCode.DataPropertyName = NameOf(ReferentielLigne.Code)
        colLibelle.DataPropertyName = NameOf(ReferentielLigne.Libelle)
        colOrdre.DataPropertyName = NameOf(ReferentielLigne.OrdreAffichage)
        colActif.DataPropertyName = NameOf(ReferentielLigne.Actif)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserToolTips
    ' Rôle      : Initialise les aides contextuelles des boutons.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserToolTips()

        If _context Is Nothing Then Exit Sub

        _context.SetToolTip(btnNouveau, "Créer un nouvel élément.")
        _context.SetToolTip(btnModifier, "Modifier l'élément sélectionné.")
        _context.SetToolTip(btnActiverDesactiver, "Activer ou désactiver l'élément sélectionné.")
        _context.SetToolTip(btnActualiser, "Recharger la liste.")
        _context.SetToolTip(btnEnregistrer, "Enregistrer la saisie en cours.")
        _context.SetToolTip(btnAnnuler, "Annuler la saisie en cours.")
        _context.SetToolTip(chkAfficherInactifs, "Afficher également les éléments désactivés.")
        _context.SetToolTip(txtRecherche, "Filtrer sur le code ou le libellé.")
        _context.SetToolTip(txtCode, ConstruireToolTipCode())

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ConstruireToolTipCode
    ' Rôle     : Construit l'aide contextuelle du champ code selon la longueur et l'option underscore.
    ' -------------------------------------------------------------------------------------------------
    Private Function ConstruireToolTipCode() As String

        If RemplacerEspacesParUnderscore Then
            Return $"Code en majuscules ({LongueurMaxCode} caractères max ; lettres, chiffres ; espaces convertis en _)."
        End If

        Return $"Code en majuscules ({LongueurMaxCode} caractères ; lettres ou chiffres)."

    End Function

#End Region

#Region "Gestion des modes"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : PasserEnMode
    ' Rôle      : Bascule l'écran dans un mode donné et ajuste l'état des contrôles.
    ' -------------------------------------------------------------------------------------------------
    Private Sub PasserEnMode(mode As ModeReferentiel)

        _mode = mode

        Dim enEdition As Boolean = mode <> ModeReferentiel.Consultation

        ' Zone liste / recherche désactivée pendant l'édition
        dgvReferentiel.Enabled = Not enEdition
        txtRecherche.Enabled = Not enEdition
        chkAfficherInactifs.Enabled = Not enEdition

        ' Champs d'édition actifs uniquement en création / modification
        txtCode.Enabled = enEdition
        txtLibelle.Enabled = enEdition
        numOrdre.Enabled = enEdition
        chkActif.Enabled = enEdition
        ActiverChampSupplementaire(enEdition)

        ' Boutons de gestion (désactivés pendant l'édition)
        btnNouveau.Enabled = Not enEdition
        btnModifier.Enabled = Not enEdition
        btnActiverDesactiver.Enabled = Not enEdition
        btnActualiser.Enabled = Not enEdition

        ' Boutons d'édition (actifs uniquement pendant l'édition)
        btnEnregistrer.Enabled = enEdition
        btnAnnuler.Enabled = enEdition

        ' Titre du panneau d'édition selon le mode
        Select Case mode
            Case ModeReferentiel.Creation
                lblTitreEdition.Text = "Nouvel élément"
            Case ModeReferentiel.Modification
                lblTitreEdition.Text = "Modification"
            Case Else
                lblTitreEdition.Text = "Détail"
        End Select

        ' En consultation, l'état des boutons dépend de la sélection
        If Not enEdition Then
            ActualiserBoutonsSelection()
        End If

        EffacerErreurs()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ActualiserBoutonsSelection
    ' Rôle      : Active/désactive Modifier et Activer/Désactiver selon la ligne sélectionnée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ActualiserBoutonsSelection()

        Dim ligne As ReferentielLigne = LigneSelectionnee()
        Dim aSelection As Boolean = ligne IsNot Nothing

        btnModifier.Enabled = aSelection
        btnActiverDesactiver.Enabled = aSelection

        If aSelection Then
            btnActiverDesactiver.Text = If(ligne.Actif, "Désactiver", "Réactiver")
        Else
            btnActiverDesactiver.Text = "Désactiver"
        End If

    End Sub

#End Region

#Region "Chargement et filtrage des données"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerListe
    ' Rôle      : Charge les éléments via le point d'extension et applique le filtre courant.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerListe()

        Try
            Cursor = Cursors.WaitCursor

            _lignes = ChargerElements(chkAfficherInactifs.Checked)

            AppliquerFiltre()

            GestionLog.EcrireLog(
                $"Référentiel '{TitreReferentiel}' chargé : {_lignes.Count} élément(s).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI)

        Catch ex As Exception
            GestionLog.EcrireException($"Erreur de chargement du référentiel '{TitreReferentiel}'.", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.UI)
            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement du référentiel.")
            End If
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerFiltre
    ' Rôle      : Filtre la liste en mémoire sur le texte de recherche et lie la grille.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerFiltre()

        If _lignes Is Nothing Then Return

        Dim resultats = _lignes.AsEnumerable()

        Dim texte As String = txtRecherche.Text.Trim()
        If Not String.IsNullOrEmpty(texte) Then
            resultats = resultats.Where(
                Function(l) (l.Code IsNot Nothing AndAlso l.Code.Contains(texte, StringComparison.OrdinalIgnoreCase)) OrElse
                            (l.Libelle IsNot Nothing AndAlso l.Libelle.Contains(texte, StringComparison.OrdinalIgnoreCase)))
        End If

        Dim liste = resultats.ToList()

        dgvReferentiel.DataSource = Nothing
        dgvReferentiel.DataSource = liste
        dgvReferentiel.ClearSelection()

        If _context IsNot Nothing Then
            _context.SetStatus($"{liste.Count} élément(s) affiché(s).")
        End If

        ActualiserBoutonsSelection()
        AfficherDetailSelection()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LigneSelectionnee
    ' Rôle     : Retourne la ReferentielLigne sélectionnée dans la grille, ou Nothing.
    ' -------------------------------------------------------------------------------------------------
    Private Function LigneSelectionnee() As ReferentielLigne

        If dgvReferentiel.CurrentRow Is Nothing Then Return Nothing
        Return TryCast(dgvReferentiel.CurrentRow.DataBoundItem, ReferentielLigne)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherDetailSelection
    ' Rôle      : Affiche les valeurs de l'élément sélectionné dans le panneau d'édition (lecture).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherDetailSelection()

        Dim ligne As ReferentielLigne = LigneSelectionnee()

        If ligne Is Nothing Then
            ViderDetail()
            Return
        End If

        txtCode.Text = ligne.Code
        txtLibelle.Text = ligne.Libelle
        numOrdre.Value = Math.Min(Math.Max(ligne.OrdreAffichage, CInt(numOrdre.Minimum)), CInt(numOrdre.Maximum))
        chkActif.Checked = ligne.Actif
        AfficherChampSupplementaire(ligne)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ViderDetail
    ' Rôle      : Réinitialise le panneau d'édition.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ViderDetail()
        txtCode.Clear()
        txtLibelle.Clear()
        numOrdre.Value = 0
        chkActif.Checked = True
        ViderChampSupplementaire()
    End Sub

#End Region

#Region "Droits d'accès"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerDroitsUtilisateur
    ' Rôle      : Restreint les actions de modification selon le rôle de l'utilisateur courant.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerDroitsUtilisateur()

        Dim peutModifier As Boolean =
            _context IsNot Nothing AndAlso _context.HasRole(RoleMinimum)

        If Not peutModifier Then
            btnNouveau.Enabled = False
            btnModifier.Enabled = False
            btnActiverDesactiver.Enabled = False

            If _context IsNot Nothing Then
                _context.SetStatus("Consultation seule : droits insuffisants pour modifier ce référentiel.")
            End If
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : PeutModifier
    ' Rôle     : Vérifie le droit de modification avant toute action d'écriture.
    ' -------------------------------------------------------------------------------------------------
    Private Function PeutModifier() As Boolean
        Return _context IsNot Nothing AndAlso _context.HasRole(RoleMinimum)
    End Function

#End Region

#Region "Validation"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : SaisieValide
    ' Rôle     : Valide les champs du panneau d'édition (requis, longueurs, unicité).
    ' Retour   : True si la saisie est valide, False sinon (erreurs affichées via ErrorProvider).
    ' -------------------------------------------------------------------------------------------------
    Private Function SaisieValide() As Boolean

        EffacerErreurs()

        Dim valide As Boolean = True

        Dim code As String = txtCode.Text.Trim()
        Dim libelle As String = txtLibelle.Text.Trim()

        ' Code requis
        If String.IsNullOrEmpty(code) Then
            DefinirErreur(txtCode, "Le code est obligatoire.")
            valide = False
        ElseIf CodeExisteDeja(code, _idEnCours) Then
            DefinirErreur(txtCode, "Ce code est déjà utilisé.")
            valide = False
        End If

        ' Libellé requis
        If String.IsNullOrEmpty(libelle) Then
            DefinirErreur(txtLibelle, "Le libellé est obligatoire.")
            valide = False
        ElseIf LibelleExisteDeja(libelle, _idEnCours) Then
            DefinirErreur(txtLibelle, "Ce libellé est déjà utilisé.")
            valide = False
        End If

        ' Validation du champ supplémentaire éventuel (ex. tarif pour ref_types_seance)
        If Not ValiderChampSupplementaire() Then
            valide = False
        End If

        Return valide

    End Function

    Private Sub DefinirErreur(control As Control, message As String)
        If _context IsNot Nothing Then
            _context.SetError(control, message)
        End If
    End Sub

    Private Sub EffacerErreurs()
        If _context IsNot Nothing Then
            _context.ClearAllErrors()
        End If
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : txtCode_KeyPress
    ' Rôle      : Filtre la saisie du code au clavier (lettres, chiffres et caractères de contrôle).
    ' Remarque  : L'espace est accepté ici uniquement si RemplacerEspacesParUnderscore est actif ;
    '             sa conversion en underscore est ensuite réalisée par txtCode_TextChanged.
    '             La mise en majuscules est assurée par CharacterCasing ; la longueur par MaxLength.
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtCode_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtCode.KeyPress

        If Char.IsControl(e.KeyChar) Then Return

        If e.KeyChar = " "c AndAlso RemplacerEspacesParUnderscore Then Return

        If Char.IsLetterOrDigit(e.KeyChar) Then Return

        e.Handled = True

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : txtCode_TextChanged
    ' Rôle      : Normalise le code après toute modification (frappe ou collage).
    ' Remarque  : Remplace les espaces par des underscores (si activé) et retire tout caractère
    '             non autorisé (hors lettres, chiffres et underscore). Conserve la position du curseur.
    '             Un garde (_miseAJourCode) évite la réentrance lors de la réécriture du texte.
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtCode_TextChanged(sender As Object, e As EventArgs) Handles txtCode.TextChanged

        If _miseAJourCode Then Return

        Dim original As String = txtCode.Text
        Dim resultat As New System.Text.StringBuilder(original.Length)

        For Each c As Char In original
            If RemplacerEspacesParUnderscore AndAlso c = " "c Then
                resultat.Append("_"c)
            ElseIf Char.IsLetterOrDigit(c) Then
                resultat.Append(c)
            ElseIf c = "_"c AndAlso RemplacerEspacesParUnderscore Then
                resultat.Append(c)
            End If
        Next

        Dim nettoye As String = resultat.ToString()

        If nettoye = original Then Return

        Dim position As Integer = txtCode.SelectionStart - (original.Length - nettoye.Length)

        _miseAJourCode = True
        txtCode.Text = nettoye
        txtCode.SelectionStart = Math.Max(0, Math.Min(position, nettoye.Length))
        _miseAJourCode = False

    End Sub

#End Region

#Region "Actions - Boutons"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnNouveau_Click
    ' Rôle      : Passe en mode création et prépare un panneau d'édition vierge.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnNouveau_Click(sender As Object, e As EventArgs) Handles btnNouveau.Click

        If Not PeutModifier() Then Return

        _idEnCours = 0
        ViderDetail()
        PasserEnMode(ModeReferentiel.Creation)
        txtCode.Focus()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnModifier_Click
    ' Rôle      : Passe en mode modification sur l'élément sélectionné.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifier_Click(sender As Object, e As EventArgs) Handles btnModifier.Click

        If Not PeutModifier() Then Return

        Dim ligne As ReferentielLigne = LigneSelectionnee()
        If ligne Is Nothing Then Return

        _idEnCours = ligne.IdReferentiel
        AfficherDetailSelection()
        PasserEnMode(ModeReferentiel.Modification)
        txtLibelle.Focus()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnActiverDesactiver_Click
    ' Rôle      : Bascule l'état actif de l'élément sélectionné (soft-delete / réactivation).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnActiverDesactiver_Click(sender As Object, e As EventArgs) Handles btnActiverDesactiver.Click

        If Not PeutModifier() Then Return

        Dim ligne As ReferentielLigne = LigneSelectionnee()
        If ligne Is Nothing Then Return

        Dim nouvelEtat As Boolean = Not ligne.Actif
        Dim action As String = If(nouvelEtat, "réactiver", "désactiver")

        Dim reponse As DialogResult = MessageBox.Show(
            $"Voulez-vous {action} l'élément « {ligne.Libelle} » ?",
            "Confirmation",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

        If reponse <> DialogResult.Yes Then Return

        Try
            DefinirActivation(ligne.IdReferentiel, nouvelEtat)

            GestionLog.EcrireLog(
                $"Référentiel '{TitreReferentiel}' : élément Id={ligne.IdReferentiel} {action}.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Process)

            If _context IsNot Nothing Then
                _context.SetStatus($"Élément {action}.")
            End If

            ChargerListe()

        Catch ex As Exception
            GestionLog.EcrireException($"Erreur lors de l'opération '{action}' sur le référentiel '{TitreReferentiel}'.", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.Process)
            MessageBox.Show($"Impossible de {action} l'élément.", "Erreur",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnActualiser_Click
    ' Rôle      : Recharge la liste depuis la base.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnActualiser_Click(sender As Object, e As EventArgs) Handles btnActualiser.Click
        ChargerListe()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnEnregistrer_Click
    ' Rôle      : Valide et enregistre la saisie (création ou modification).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEnregistrer_Click(sender As Object, e As EventArgs) Handles btnEnregistrer.Click

        If Not PeutModifier() Then Return
        If Not SaisieValide() Then Return

        Dim code As String = txtCode.Text.Trim()
        Dim libelle As String = txtLibelle.Text.Trim()
        Dim ordre As Integer = CInt(numOrdre.Value)
        Dim actif As Boolean = chkActif.Checked

        Try
            If _mode = ModeReferentiel.Creation Then
                InsererElement(code, libelle, ordre, actif)
                GestionLog.EcrireLog(
                    $"Référentiel '{TitreReferentiel}' : création de l'élément '{code}'.",
                    GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Process)
            Else
                MettreAJourElement(_idEnCours, code, libelle, ordre, actif)
                GestionLog.EcrireLog(
                    $"Référentiel '{TitreReferentiel}' : modification de l'élément Id={_idEnCours} ('{code}').",
                    GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Process)
            End If

            If _context IsNot Nothing Then
                _context.SetStatus("Enregistrement effectué.")
            End If

            _idEnCours = 0
            PasserEnMode(ModeReferentiel.Consultation)
            ChargerListe()

        Catch ex As Exception
            GestionLog.EcrireException($"Erreur lors de l'enregistrement dans le référentiel '{TitreReferentiel}'.", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.Process)
            MessageBox.Show("Impossible d'enregistrer l'élément.", "Erreur",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAnnuler_Click
    ' Rôle      : Annule la saisie en cours et revient en consultation.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAnnuler_Click(sender As Object, e As EventArgs) Handles btnAnnuler.Click
        _idEnCours = 0
        PasserEnMode(ModeReferentiel.Consultation)
        AfficherDetailSelection()
    End Sub

#End Region

#Region "Actions - Grille et filtres"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvReferentiel_SelectionChanged
    ' Rôle      : Met à jour le détail et l'état des boutons à chaque changement de sélection.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvReferentiel_SelectionChanged(sender As Object, e As EventArgs) Handles dgvReferentiel.SelectionChanged

        If _mode <> ModeReferentiel.Consultation Then Exit Sub

        ActualiserBoutonsSelection()
        AfficherDetailSelection()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : txtRecherche_TextChanged
    ' Rôle      : Applique le filtre de recherche à chaque frappe.
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtRecherche_TextChanged(sender As Object, e As EventArgs) Handles txtRecherche.TextChanged
        If _mode = ModeReferentiel.Consultation Then
            AppliquerFiltre()
        End If
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : chkAfficherInactifs_CheckedChanged
    ' Rôle      : Recharge la liste en incluant ou non les éléments désactivés.
    ' -------------------------------------------------------------------------------------------------
    Private Sub chkAfficherInactifs_CheckedChanged(sender As Object, e As EventArgs) Handles chkAfficherInactifs.CheckedChanged
        If _mode = ModeReferentiel.Consultation Then
            ChargerListe()
        End If
    End Sub

#End Region

End Class


' -------------------------------------------------------------------------------------------------
' Enum     : ModeReferentiel
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Définit le mode courant d'un écran de gestion de référentiel.
'
' Valeurs :
' - Consultation : lecture seule, sélection dans la grille
' - Creation     : saisie d'un nouvel élément
' - Modification : édition d'un élément existant
' -------------------------------------------------------------------------------------------------
Public Enum ModeReferentiel
    Consultation = 0
    Creation = 1
    Modification = 2
End Enum
