' -------------------------------------------------------------------------------------------------
' Formulaire  : DossierStatutEditor
' Projet      : Althéa
' Version     : V1.3.2
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Petite modale de changement de statut d'un dossier (cycle de vie, brique B6).
' Affiche le statut courant, propose uniquement les transitions légales (whitelist métier) et
' demande la date de clôture lorsque la cible est « Clôturé ». La transition effective est
' appliquée par l'appelant (UC_DossierFiche) via GestionDossiers.ChangerStatutDossier.
'
' Responsabilités :
' - Charger les statuts cibles autorisés depuis GestionDossiers.GetTransitionsPossibles
' - Activer la saisie de la date de clôture uniquement pour la cible « Clôturé »
' - Valider la saisie et exposer le code cible + la date de clôture retenus
'
' Important   :
' - Aucun accès base direct : la persistance reste dans la couche métier (appelée par l'appelant)
' - La modale renvoie DialogResult.OK uniquement si une transition valide a été confirmée
' - V1.2.0 (15/10/2026, UI-7) : carte « Statut du dossier », fond AltheaFondFenetre, liste AltheaListe,
'   date AltheaDate bornée à aujourd'hui ; le libellé de la date n'est plus grisé (consultation lisible).
' - V1.3.0 (17/10/2026) : le dossier est nommé par son code dans l'en-tête (paramètre codeDossier).
' - V1.3.1 (17/10/2026) : code « Clôturé » repris de GestionDossiers (une seule définition).
' - V1.3.2 (24/10/2026, audit C.3) : code « Clôturé » lu dans CodesSysteme.StatutsDossier (constante
'   locale retirée).
'
' Dépendances :
' - GestionDossiers (transitions possibles)
' - GestionStatutsDossier (libellés des statuts)
' - DialogChoix (messages), UserControlContext (contexte UI partagé)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class DossierStatutEditor
    Implements IContextAwareForm

#Region "Types"

    ' -------------------------------------------------------------------------------------------------
    ' Classe      : StatutCibleItem
    ' Version     : V1.1.0
    ' Date        : 14/07/2026
    '
    ' Rôle        :
    ' Représente un élément de la liste des statuts cibles : code technique et libellé affiché.
    '
    ' Important   :
    ' - DTO interne de présentation, sans accès à la base ni règle de transition.
    ' -------------------------------------------------------------------------------------------------
    Private NotInheritable Class StatutCibleItem

        Public Property Code As String
        Public Property Libelle As String

        Public Sub New(code As String, libelle As String)
            Me.Code = code
            Me.Libelle = libelle
        End Sub

        Public Overrides Function ToString() As String
            Return Libelle
        End Function

    End Class

#End Region

#Region "Variables privées"

    ' Contexte UI partagé fourni par l'appelant (peut être Nothing).
    Private _context As UserControlContext

    ' Code du statut courant du dossier (source des transitions possibles).
    Private ReadOnly _codeStatutCourant As String

    ' Libellé du statut courant (affichage).
    Private ReadOnly _libelleStatutCourant As String

#End Region

#Region "Propriétés publiques (résultat)"

    ' Code du statut cible retenu par l'utilisateur (valide uniquement si DialogResult.OK).
    Public ReadOnly Property CodeStatutCible As String
        Get
            Dim item As StatutCibleItem = TryCast(cboStatutCible.SelectedItem, StatutCibleItem)
            Return If(item Is Nothing, Nothing, item.Code)
        End Get
    End Property

    ' Date saisie dans le champ (AltheaDate obligatoire : jamais vide).
    Private ReadOnly Property DateClotureSaisie As Date
        Get
            Return dtpDateCloture.Valeur.Value.Date
        End Get
    End Property

    ' Date de clôture retenue (renseignée uniquement si la cible est « Clôturé »).
    Public ReadOnly Property DateClotureChoisie As Date?
        Get
            If String.Equals(CodeStatutCible, CodesSysteme.StatutsDossier.Cloture, StringComparison.OrdinalIgnoreCase) Then
                Return DateClotureSaisie
            End If
            Return Nothing
        End Get
    End Property

#End Region

#Region "Constructeur"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version       : V1.1.0
    ' Date          : 15/10/2026
    '
    ' Rôle          :
    ' Initialise la modale pour un statut courant donné.
    '
    ' Paramètres    :
    ' - codeStatutCourant    : Code technique du statut courant (ex. 'ACTIF')
    ' - libelleStatutCourant : Libellé du statut courant (affichage)
    ' -------------------------------------------------------------------------------------------------
    ' codeDossier : dossier concerné, nommé dans l'en-tête (facultatif).
    Public Sub New(codeStatutCourant As String, libelleStatutCourant As String, Optional codeDossier As String = "")

        InitializeComponent()

        ' Fond du kit : beige clair et petite feuille dessinée.
        AltheaFondFenetre.Appliquer(Me)

        If Not String.IsNullOrWhiteSpace(codeDossier) Then
            enTete.SousTitre = $"Changer le statut du dossier {codeDossier}"
        End If

        ' Une clôture ne peut pas être datée dans le futur : le calendrier le refuse d'emblée.
        dtpDateCloture.DateMax = Date.Today

        _codeStatutCourant = codeStatutCourant
        _libelleStatutCourant = libelleStatutCourant

    End Sub

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SetContext
    ' Rôle      : Reçoit le contexte UI partagé fourni par l'appelant (avant ShowDialog).
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) Implements IContextAwareForm.SetContext

        _context = context

    End Sub

#End Region

#Region "Cycle de vie"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DossierStatutEditor_Load
    ' Version   : V1.0.0
    ' Date      : 14/07/2026
    '
    ' Rôle      :
    ' Renseigne le statut courant et charge les transitions légales dans la combo.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DossierStatutEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        lblStatutCourantValeur.Text = If(String.IsNullOrWhiteSpace(_libelleStatutCourant), "—", _libelleStatutCourant)

        If _context IsNot Nothing Then
            _context.SetToolTip(btnValider, "Appliquer le changement de statut sélectionné.")
            _context.SetToolTip(btnAnnuler, "Annuler sans changer le statut.")
        End If

        ChargerTransitions()
        MettreAJourDateCloture()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerTransitions
    ' Version   : V1.0.0
    ' Date      : 14/07/2026
    '
    ' Rôle      :
    ' Charge les statuts cibles autorisés depuis la couche métier et les affiche dans la combo.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerTransitions()

        Dim codesCibles As List(Of String) = GestionDossiers.GetTransitionsPossibles(_codeStatutCourant)

        ' Résolution des libellés depuis le référentiel des statuts actifs.
        Dim statuts As List(Of StatutDossier) = GestionStatutsDossier.GetStatutsDossierActifs()

        Dim items As New List(Of StatutCibleItem)

        For Each code As String In codesCibles

            Dim statut As StatutDossier = statuts.
                FirstOrDefault(Function(s) String.Equals(s.CodeStatutDossier, code, StringComparison.OrdinalIgnoreCase))

            Dim libelle As String = If(statut Is Nothing, code, statut.LibelleStatutDossier)
            items.Add(New StatutCibleItem(code, libelle))

        Next

        cboStatutCible.DataSource = items
        cboStatutCible.SelectedIndex = If(items.Count > 0, 0, -1)

        ' Aucune transition possible (ex. dossier archivé, terminal) : on désactive la validation.
        Dim aucuneTransition As Boolean = items.Count = 0

        cboStatutCible.Enabled = Not aucuneTransition
        btnValider.Enabled = Not aucuneTransition

        If aucuneTransition AndAlso _context IsNot Nothing Then
            _context.SetStatus("Ce dossier est archivé : aucune transition de statut n'est possible.")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : cboStatutCible_SelectedIndexChanged
    ' Rôle      : Active/désactive la date de clôture selon la cible sélectionnée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub cboStatutCible_SelectedIndexChanged(sender As Object, e As EventArgs) _
        Handles cboStatutCible.SelectedIndexChanged

        MettreAJourDateCloture()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourDateCloture
    ' Version   : V1.1.0
    ' Date      : 15/10/2026
    '
    ' Rôle      :
    ' N'active la saisie de la date de clôture que si la cible sélectionnée est « Clôturé ».
    ' -------------------------------------------------------------------------------------------------
    Private Sub MettreAJourDateCloture()

        Dim cibleCloture As Boolean =
            String.Equals(CodeStatutCible, CodesSysteme.StatutsDossier.Cloture, StringComparison.OrdinalIgnoreCase)

        ' Le libellé reste lisible (règle « Consultation lisible ») ; seule la date est inactive.
        dtpDateCloture.Enabled = cibleCloture

        If cibleCloture AndAlso DateClotureSaisie > Date.Today Then
            dtpDateCloture.Valeur = Date.Today
        End If

    End Sub

#End Region

#Region "Actions"

    ' -------------------------------------------------------------------------------------------------
    ' Événement : btnValider_Click
    ' Version    : V1.0.0
    ' Date       : 14/07/2026
    '
    ' Rôle       :
    ' Valide la saisie puis ferme la modale en DialogResult.OK si une cible est sélectionnée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnValider_Click(sender As Object, e As EventArgs) Handles btnValider.Click

        If String.IsNullOrWhiteSpace(CodeStatutCible) Then
            DialogChoix.Avertissement("Veuillez sélectionner un nouveau statut.")
            Return
        End If

        ' Garde-fou : la date de clôture ne peut pas être postérieure à aujourd'hui.
        If String.Equals(CodeStatutCible, CodesSysteme.StatutsDossier.Cloture, StringComparison.OrdinalIgnoreCase) _
           AndAlso DateClotureSaisie > Date.Today Then

            DialogChoix.Avertissement("La date de clôture ne peut pas être postérieure à aujourd'hui.")
            Return

        End If

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Événement : btnAnnuler_Click
    ' Rôle      : Ferme la modale sans appliquer de changement.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAnnuler_Click(sender As Object, e As EventArgs) Handles btnAnnuler.Click

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

#End Region

End Class
