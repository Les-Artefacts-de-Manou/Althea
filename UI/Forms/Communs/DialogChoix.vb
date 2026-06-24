' -------------------------------------------------------------------------------------------------
' Form          : DialogChoix
' Projet        : Althéa
' Version       : V1.0.0
' Date          : 03/06/2026
' Auteur        : Joëlle (Manou) / Projet Althéa
'
' Rôle          :
' Form de dialogue personnalisée pour remplacer les MessageBox standards.
' Support des icônes (Information, Warning, Error, Question, Success), thématisation UITheme,
' et configuration flexible de 1 à 3 boutons.
'
' Utilisation   :
' ' Méthodes statiques simples :
' DialogChoix.Information("Opération réussie")
' DialogChoix.Erreur("Une erreur s'est produite")
' If DialogChoix.Confirmer("Continuer ?") = DialogResult.Yes Then...
'
' ' Configuration avancée :
' Dim dlg As New DialogChoix()
' dlg.Titre = "Suppression"
' dlg.Message = "Confirmer la suppression ?"
' dlg.TypeDialogue = TypeDialogue.Warning
' dlg.SetBoutons("Supprimer", "Annuler")
' If dlg.ShowDialog() = DialogResult.Yes Then...
'
' Dépendances   :
' - UITheme (thématisation)
' - UtilsButtons (style des boutons)
' - My.Resources (icônes - optionnel)

' Imports     :
' - System.Windows.Forms
' - System.Drawing 
'
' Remarques     :
' - Form modale centrée sur le parent
' - Taille adaptative selon le contenu
' - Support de 1 à 3 boutons configurables
' - Mapping DialogResult : Bouton1=Yes, Bouton2=No, Bouton3=Cancel
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On


' Type de dialogue pour définir l'icône et le style visuel

Public Enum TypeDialogue
    Information ' Pour les messages d'information généraux
    Warning ' Pour les avertissements 
    [Error] ' Pour les erreurs 
    Question ' Pour les questions 
    Success ' Pour les succès 
    Loading ' Pour les opérations en cours 
    Processing ' Pour les opérations en cours 
End Enum

Public Class DialogChoix

#Region "Propriétés publiques"

    'Titre de la boîte de dialogue (affiché dans la barre de titre)
    Public Property Titre As String = "Confirmation"

    ' Message principal affiché à l'utilisateur
    Public Property Message As String = ""

    ' Type de dialogue (détermine l'icône et le style visuel)
    Public Property TypeDialogue As TypeDialogue = TypeDialogue.Information

#End Region

#Region "Variables privées"

    ' Nombre de boutons actuellement configurés (1, 2 ou 3)
    Private _nombreBoutons As Integer = 3

#End Region

#Region "Constructeur"

    ' Constructeur par défaut
    Public Sub New()
        InitializeComponent()
    End Sub

#End Region

#Region "Configuration des boutons"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetBoutons
    ' Version    : V1.0.0
    ' Date       : 03/06/26
    '
    ' Rôle       :
    ' Configure les boutons de la boîte de dialogue en fonction du nombre de boutons souhaité.
    '
    ' Paramètres :
    ' - bouton1 : Texte du premier bouton (DialogResult.Yes)
    ' - bouton2 : Texte du second bouton (DialogResult.No)
    ' - bouton3 : Texte du troisième bouton (DialogResult.Cancel)
    '
    ' Remarques  :
    '  - Les boutons ne sont pas affichés si aucun bouton n'est configuré
    '  - Le mapping des DialogResult est fixe : Bouton1=Yes, Bouton2=No, Bouton3=Cancel
    '  - La procédure ajuste la visibilité des boutons et le nombre de boutons configurés
    '
    ' Exceptions :
    ' - ArgumentNullException si aucun bouton n'est configuré
    '
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetBoutons(bouton1 As String)
        btn1.Text = bouton1
        btn2.Visible = False
        btn3.Visible = False
        _nombreBoutons = 1
    End Sub

    ' Configure 2 boutons (les 2 premiers sont affichés)
    Public Sub SetBoutons(bouton1 As String, bouton2 As String)
        btn1.Text = bouton1
        btn2.Text = bouton2
        btn3.Visible = False
        _nombreBoutons = 2
    End Sub

    ' Configure 3 boutons (tous sont affichés)
    Public Sub SetBoutons(bouton1 As String, bouton2 As String, bouton3 As String)
        btn1.Text = bouton1
        btn2.Text = bouton2
        btn3.Text = bouton3
        btn3.Visible = True
        _nombreBoutons = 3
    End Sub

#End Region

#Region "Affichage et initialisation"

    ' -----------------------------------------------------------------------------------------------
    ' Procédure  : DialogChoix_Load
    ' Version    : V1.0.0
    ' Date       : 03/06/26
    '
    ' Rôle       :
    ' Initialise la boîte de dialogue lors du chargement :
    ' - Applique le thème UITheme
    ' - Affiche l'icône appropriée selon le TypeDialogue
    ' - Ajuste la taille de la form selon la longueur du message
    ' - Repositionne les boutons selon le nombre de boutons visibles
    '
    ' Remarques  :
    ' - Cette procédure est déclenchée automatiquement lors du chargement de la form
    '
    ' Exceptions :
    ' - Aucune exception prévue, mais les erreurs d'affichage de l'icône sont gérées en interne
    ' -----------------------------------------------------------------------------------------------

    Private Sub DialogChoix_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Titre et message
        Me.Text = Titre
        lblMessage.Text = Message

        ' Appliquer la thématisation
        AppliquerTheme()

        ' Afficher l'icône selon le type
        AfficherIcone()

        ' Ajuster la taille de la form selon le contenu
        AjusterTaille()

        ' Repositionner les boutons
        RepositionnerBoutons()
    End Sub

    ' -----------------------------------------------------------------------------------------------
    ' Procédure  : AppliquerTheme
    ' Version    : V1.0.0
    ' Date       : 03/06/26
    '
    ' Rôle       :
    ' Applique le thème UITheme aux composants de la boîte de dialogue.
    '
    ' Remarques  :
    ' - Cette procédure est appelée lors du chargement de la form
    ' -----------------------------------------------------------------------------------------------
    Private Sub AppliquerTheme()

        'Panel
        pnlBoutons.BackColor = UITheme.ColorBeigeClair

        ' Message
        lblMessage.ForeColor = UITheme.DynamicTextFore
        lblMessage.Font = New Font("Calibri", 11.0F, FontStyle.Regular)

        ' Boutons avec UtilsButtons
        UtilsButtons.InitStandardButton(btn1)
        UtilsButtons.InitStandardButton(btn2)
        UtilsButtons.InitStandardButton(btn3)

        ' Colorisation selon le type
        Select Case TypeDialogue
            Case TypeDialogue.Error
                pnlIcone.BackColor = UITheme.ColorRougeMoyenFonce
            Case TypeDialogue.Warning
                pnlIcone.BackColor = UITheme.ColorOrangeMoyenFonce
            Case TypeDialogue.Success
                pnlIcone.BackColor = UITheme.ColorGrisVertClair
            Case TypeDialogue.Information
                pnlIcone.BackColor = UITheme.ColorBleuMoyen
            Case TypeDialogue.Question
                pnlIcone.BackColor = UITheme.ColorSauge
        End Select
    End Sub

    ' -----------------------------------------------------------------------------------------------
    ' Procédure  : AfficherIcone
    ' Version    : V1.0.0
    ' Date       : 03/06/26
    '
    ' Rôle       :
    ' Affiche l'icône appropriée selon le TypeDialogue.
    '
    ' Remarques  :
    ' - Cette procédure est appelée lors du chargement de la form
    ' - Les icônes sont chargées depuis My.Resources 
    ' - Gif animés 
    ' -----------------------------------------------------------------------------------------------
    Private Sub AfficherIcone()

        Try
            Select Case TypeDialogue
                Case TypeDialogue.Information
                    picIcone.Image = My.Resources.Information_64
                Case TypeDialogue.Warning
                    picIcone.Image = My.Resources.Warning_64
                Case TypeDialogue.Error
                    picIcone.Image = My.Resources.Error_64
                Case TypeDialogue.Question
                    picIcone.Image = My.Resources.Question_64
                Case TypeDialogue.Success
                    picIcone.Image = My.Resources.Success_64
                Case TypeDialogue.Loading
                    picIcone.Image = My.Resources.Loading_64
                Case TypeDialogue.Processing
                    picIcone.Image = My.Resources.Processing_64
            End Select

        Catch ex As Exception
            ' En cas d'erreur, masquer l'icône
            pnlIcone.Visible = False
        End Try
    End Sub

    ' -----------------------------------------------------------------------------------------------
    ' Procédure  : AjusterTaille
    ' Version    : V1.0.0
    ' Date       : 03/06/26
    '
    ' Rôle       :
    ' Ajuste la taille de la form en fonction de la longueur du message.
    '
    ' Remarques  :
    ' - Cette procédure est appelée lors du chargement de la form
    ' -----------------------------------------------------------------------------------------------
    Private Sub AjusterTaille()
        ' Mesurer la taille du texte
        Dim graphics As Graphics = lblMessage.CreateGraphics()
        Dim textSize As SizeF = graphics.MeasureString(
            lblMessage.Text,
            lblMessage.Font,
            New SizeF(450, 0) ' Largeur max
        )
        graphics.Dispose()

        ' Hauteur minimale et maximale
        Dim hauteurMessage As Integer = CInt(Math.Ceiling(textSize.Height)) + 20
        Dim hauteurMin As Integer = 150
        Dim hauteurMax As Integer = 500

        hauteurMessage = Math.Max(hauteurMin, Math.Min(hauteurMax, hauteurMessage))

        ' Ajuster la hauteur de la form
        Me.Height = hauteurMessage + pnlBoutons.Height + 50
    End Sub

    ' ------------------------------------------------------------------------------------------------
    ' Procédure  : RepositionnerBoutons
    ' Version    : V1.0.0
    ' Date       : 03/06/26
    '
    ' Rôle       :
    ' Repositionne les boutons de droite à gauche en fonction du nombre de boutons visibles.
    '
    ' Remarques  :
    ' - Cette procédure est appelée lors du chargement de la form
    ' ------------------------------------------------------------------------------------------------

    Private Sub RepositionnerBoutons()
        Dim espacementBoutons As Integer = 10
        Dim largeurBouton As Integer = 120
        Dim positionX As Integer = pnlBoutons.Width - 20

        ' Placer les boutons de droite à gauche
        If btn3.Visible Then
            btn3.Location = New Point(positionX - largeurBouton, btn3.Location.Y)
            positionX -= largeurBouton + espacementBoutons
        End If

        If btn2.Visible Then
            btn2.Location = New Point(positionX - largeurBouton, btn2.Location.Y)
            positionX -= largeurBouton + espacementBoutons
        End If

        btn1.Location = New Point(positionX - largeurBouton, btn1.Location.Y)
    End Sub

#End Region

#Region "Actions boutons"

    '-------------------------------------------------------------------------------------------------
    ' Procédure  : btn1_Click
    ' Version    : V1.0.0
    ' Date       : 03/06/26
    '
    ' Rôle       :
    ' Retourne DialogResult.OK si le nombre de boutons est 1, sinon DialogResult.Yes
    '
    ' Remarques  :
    ' - Cette procédure est appelée lors du clic sur le bouton 1
    '-------------------------------------------------------------------------------------------------

    Private Sub btn1_Click(sender As Object, e As EventArgs) Handles btn1.Click
        Me.DialogResult = If(_nombreBoutons = 1, DialogResult.OK, DialogResult.Yes)
        Me.Close()
    End Sub

    '-------------------------------------------------------------------------------------------------
    ' Procédure  : btn2_Click
    ' Version    : V1.0.0
    ' Date       : 03/06/26
    '
    ' Rôle       :
    ' Retourne DialogResult.No
    '
    ' Remarques  :
    ' - Cette procédure est appelée lors du clic sur le bouton 2
    '-------------------------------------------------------------------------------------------------

    Private Sub btn2_Click(sender As Object, e As EventArgs) Handles btn2.Click
        Me.DialogResult = DialogResult.No
        Me.Close()
    End Sub

    '-------------------------------------------------------------------------------------------------
    ' Procédure  : btn3_Click
    ' Version    : V1.0.0
    ' Date       : 03/06/26
    '
    ' Rôle       :
    ' Retourne DialogResult.Cancel
    '
    ' Remarques  :
    ' - Cette procédure est appelée lors du clic sur le bouton 3
    '-------------------------------------------------------------------------------------------------

    Private Sub btn3_Click(sender As Object, e As EventArgs) Handles btn3.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

#End Region

#Region "Méthodes statiques facilitantes"

    '-------------------------------------------------------------------------------------------------
    ' Procédure  : Information (Shared)
    ' Version    : V1.0.0
    ' Date       : 03/06/26
    '
    ' Rôle       :
    ' Affiche un message d'information avec un bouton OK

    ' Paramètres :
    ' - message : Message à afficher
    ' - titre : Titre de la fenêtre (optionnel, par défaut "Information")

    ' Remarques  :
    ' - Cette procédure affiche un message d'information avec un bouton OK
    '-------------------------------------------------------------------------------------------------
    Public Shared Sub Information(message As String, Optional titre As String = "Information")
        Dim dlg As New DialogChoix()
        dlg.Titre = titre
        dlg.Message = message
        dlg.TypeDialogue = TypeDialogue.Information
        dlg.SetBoutons("OK")
        dlg.ShowDialog()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Avertissement (Shared)
    ' Version    : V1.0.0
    ' Date       : 03/06/26
    '
    ' Rôle       :
    ' Affiche un message d'avertissement avec un bouton OK

    ' Paramètres : 
    ' - message : Message à afficher
    ' - titre : Titre de la fenêtre (optionnel, par défaut "Avertissement")

    ' Remarques  :
    ' - Cette procédure affiche un message d'avertissement avec un bouton OK
    ' -------------------------------------------------------------------------------------------------
    Public Shared Sub Avertissement(message As String, Optional titre As String = "Avertissement")
        Dim dlg As New DialogChoix()
        dlg.Titre = titre
        dlg.Message = message
        dlg.TypeDialogue = TypeDialogue.Warning
        dlg.SetBoutons("OK")
        dlg.ShowDialog()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Erreur (Shared)
    ' Version    : V1.0.0
    ' Date       : 03/06/26

    ' Rôle       :
    ' Affiche un message d'erreur avec un bouton OK

    ' Paramètres :
    ' - message : Message à afficher
    ' - titre : Titre de la fenêtre (optionnel, par défaut "Erreur")

    ' Remarques  :
    ' - Cette procédure affiche un message d'erreur avec un bouton OK
    ' -------------------------------------------------------------------------------------------------
    Public Shared Sub Erreur(message As String, Optional titre As String = "Erreur")
        Dim dlg As New DialogChoix()
        dlg.Titre = titre
        dlg.Message = message
        dlg.TypeDialogue = TypeDialogue.Error
        dlg.SetBoutons("OK")
        dlg.ShowDialog()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Succes (Shared)
    ' Version    : V1.0.0
    ' Date       : 03/06/26
    '
    ' Rôle       :
    ' Affiche un message de succès avec un bouton OK
    '
    ' Paramètres :
    ' - message : Message à afficher
    ' - titre : Titre de la fenêtre (optionnel, par défaut "Succès")
    '
    ' Remarques  :
    ' - Cette procédure affiche un message de succès avec un bouton OK
    ' -------------------------------------------------------------------------------------------------

    Public Shared Sub Succes(message As String, Optional titre As String = "Succès")
        Dim dlg As New DialogChoix()
        dlg.Titre = titre
        dlg.Message = message
        dlg.TypeDialogue = TypeDialogue.Success
        dlg.SetBoutons("OK")
        dlg.ShowDialog()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Confirmer (Shared)
    ' Version    : V1.0.0
    ' Date       : 03/06/26
    '
    ' Rôle       :
    ' Affiche une question avec boutons Oui/Non
    '
    ' Paramètres :
    ' - message : Message à afficher
    ' - titre : Titre de la fenêtre (optionnel, par défaut "Confirmation")
    '
    ' Remarques  :
    ' - Cette procédure affiche une question avec boutons Oui/Non
    ' -------------------------------------------------------------------------------------------------

    Public Shared Function Confirmer(message As String, Optional titre As String = "Confirmation") As DialogResult
        Dim dlg As New DialogChoix()
        dlg.Titre = titre
        dlg.Message = message
        dlg.TypeDialogue = TypeDialogue.Question
        dlg.SetBoutons("Oui", "Non")
        Return dlg.ShowDialog()
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Question (Shared)
    ' Version    : V1.0.0
    ' Date       : 03/06/26
    '
    ' Rôle       :
    ' Affiche une question avec boutons Oui/Non/Annuler
    '
    ' Paramètres :
    ' - message : Message à afficher
    ' - titre : Titre de la fenêtre (optionnel, par défaut "Question")
    '
    ' Remarques  :
    ' - Cette procédure affiche une question avec boutons Oui/Non/Annuler
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function Question(message As String, Optional titre As String = "Question") As DialogResult
        Dim dlg As New DialogChoix()
        dlg.Titre = titre
        dlg.Message = message
        dlg.TypeDialogue = TypeDialogue.Question
        dlg.SetBoutons("Oui", "Non", "Annuler")
        Return dlg.ShowDialog()
    End Function

#End Region

End Class
