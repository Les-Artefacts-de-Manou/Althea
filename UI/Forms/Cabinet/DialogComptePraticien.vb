' -------------------------------------------------------------------------------------------------
' Formulaire  : DialogComptePraticien
' Projet      : Althéa
' Version     : V1.4.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Boîte de dialogue modale pour attacher un compte de travail (SuperUser) au praticien du cabinet.
' Deux modes :
'   - Creation : saisie login + nom d'affichage ; le compte est créé en SuperUser avec élévation
'                Admin possible et mot de passe temporaire à changer à la première connexion.
'   - Liaison  : choix parmi les SuperUser actifs non encore liés à un praticien.
'
' Important   :
' - Le compte Admin technique n'est jamais proposé ni modifié.
' - Disposition portée par le Designer (DialogComptePraticien.Designer.vb).
' - Schéma des fenêtres du kit (UI-7, 15/10/2026) : carte « Compte de travail » sur le fond
'   AltheaFondFenetre ; erreurs aux couleurs de la charte, sans détail technique (détail au journal).
' - V1.2.0 (20/10/2026) : rôle courant reçu et transmis au métier, qui réserve ces opérations à
'   l'administrateur (audit A.3).
' - V1.2.1 (21/10/2026, audit B.2) : création du compte d'un seul tenant (compte et lien) ; le
'   message d'échec le dit (« rien n'a été enregistré »).
' - V1.3.0 (22/10/2026, audit C.1) : les opérations sensibles reçoivent le ContexteOperation de l'écran (utilisateur + rôle
'   courant), vérifié par le métier (Autorisation).
' - V1.4.0 (23/10/2026, audit C.2 étape 4, ADR-51) : login et nom jugés par GestionUtilisateurs.ValiderCompte
'   (login d'au moins 3 caractères, désormais aussi unique, nom d'affichage) — la même règle que la
'   fiche Utilisateur ; tous les points dans la zone d'erreur.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class DialogComptePraticien

    Public Enum ModeDialog
        Creation
        Liaison
    End Enum

    Private ReadOnly _mode As ModeDialog
    Private ReadOnly _praticien As Praticien
    Private ReadOnly _contexte As ContexteOperation

    ''' <summary>Mot de passe temporaire généré (mode Création, après OK).</summary>
    Public ReadOnly Property MotDePasseTemporaire As String

    Public Sub New(mode As ModeDialog, praticien As Praticien, contexte As ContexteOperation)
        InitializeComponent()

        ' Fond beige à la feuille et barre de titre aux couleurs du kit.
        AltheaFondFenetre.Appliquer(Me)

        ' Erreur : teintes de la charte (même rose que les messages d'erreur DialogChoix).
        lblErreur.ForeColor = AltheaIconeEtat.Teintes(EtatIcone.Erreur).Foncee

        _mode = mode
        _praticien = praticien
        _contexte = contexte

        If mode = ModeDialog.Creation Then
            enTete.SousTitre = "Créer le compte de travail"
            Text = enTete.SousTitre
            lblInfo.Text = "Un compte SuperUser sera créé pour ce praticien, avec possibilité d'élévation temporaire en Admin via le cadenas. " &
                           "Un mot de passe temporaire sera généré : il devra être changé à la première connexion. " &
                           "Le compte Admin existant n'est pas modifié."
            txtLogin.Text = ProposerLogin()
            txtNomAffichage.Text = praticien.NomComplet
            btnOk.Text = "Créer le compte"
        Else
            enTete.SousTitre = "Lier un compte existant"
            Text = enTete.SousTitre
            lblInfo.Text = "Choisissez le compte SuperUser actif à associer à ce praticien. " &
                           "Seuls les comptes non encore liés sont proposés ; le compte Admin n'apparaît jamais ici."
            lblLogin.Visible = False : txtLogin.Visible = False
            lblNomAffichage.Visible = False : txtNomAffichage.Visible = False
            lblCompte.Visible = True : cboCompte.Visible = True
            btnOk.Text = "Lier"
            ChargerCandidats()
        End If
    End Sub

    Private Function ProposerLogin() As String
        Dim prenom = If(_praticien.Prenom, "").Trim().ToLowerInvariant()
        Dim brut = If(prenom.Length > 0, prenom, If(_praticien.Nom, "").Trim().ToLowerInvariant())
        Dim normalise = brut.Normalize(System.Text.NormalizationForm.FormD)
        Dim sb As New System.Text.StringBuilder()
        For Each c In normalise
            If Globalization.CharUnicodeInfo.GetUnicodeCategory(c) <> Globalization.UnicodeCategory.NonSpacingMark AndAlso
               (Char.IsLetterOrDigit(c) OrElse c = "."c OrElse c = "_"c) Then
                sb.Append(c)
            End If
        Next
        Return sb.ToString()
    End Function

    Private Sub ChargerCandidats()
        cboCompte.Items.Clear()
        For Each u In GestionCabinet.GetUtilisateursLiables()
            cboCompte.Items.Add(u)
        Next
        If cboCompte.Items.Count > 0 Then
            cboCompte.SelectedIndex = 0
        Else
            AfficherErreur("Aucun compte SuperUser actif disponible. Utilisez « Créer le compte… ».")
            btnOk.Enabled = False
        End If
    End Sub

    Private Sub AfficherErreur(message As String)
        lblErreur.Text = "⚠ " & message
        lblErreur.Visible = True
    End Sub

    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        lblErreur.Visible = False
        Try
            If _mode = ModeDialog.Creation Then
                Dim login = txtLogin.Text.Trim()
                Dim nom = txtNomAffichage.Text.Trim()
                ' Règles du compte (métier) : le compte du praticien est un SuperUser élevable en Admin.
                Dim erreurs = GestionUtilisateurs.ValiderCompte(login, nom, AppRole.SuperUser, AppRole.Admin)
                If erreurs.Count > 0 Then
                    AfficherErreur(String.Join(" ", erreurs.Select(Function(er) er.Message)))
                    If erreurs.Any(Function(er) er.Champ = NameOf(UtilisateurApplication.LoginUtilisateur)) Then
                        txtLogin.Focus()
                    Else
                        txtNomAffichage.Focus()
                    End If
                    Return
                End If
                _MotDePasseTemporaire = GestionCabinet.CreerCompteSuperUser(_contexte, _praticien, login, nom)
            Else
                Dim choix = TryCast(cboCompte.SelectedItem, UtilisateurLiable)
                If choix Is Nothing Then
                    AfficherErreur("Sélectionnez un compte.")
                    Return
                End If
                GestionCabinet.LierUtilisateur(_contexte, _praticien, choix.IdUtilisateur)
            End If
            DialogResult = DialogResult.OK
            Close()
        Catch ex As InvalidOperationException
            AfficherErreur(ex.Message)
        Catch ex As UnauthorizedAccessException
            AfficherErreur(ex.Message)
        Catch ex As Exception
            ' Détail technique au journal seulement : le message affiché reste général.
            GestionLog.EcrireLog($"DialogComptePraticien : échec de l'opération (mode {_mode}).",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Security, ex)
            AfficherErreur("L'opération n'a pas pu être effectuée ; rien n'a été enregistré. Le détail a été noté dans le journal.")
        End Try
    End Sub

End Class
