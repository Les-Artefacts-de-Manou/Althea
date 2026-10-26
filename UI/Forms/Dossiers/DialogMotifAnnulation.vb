' -------------------------------------------------------------------------------------------------
' Formulaire  : DialogMotifAnnulation
' Projet      : Althéa
' Version     : V1.5.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Boîte de dialogue modale de saisie du motif d'annulation d'un encaissement.
'
' Important   :
' - Disposition portée par le Designer (DialogMotifAnnulation.Designer.vb).
' - Le motif est obligatoire : la fenêtre reste ouverte tant qu'il n'est pas saisi, le champ est
'   signalé visuellement et un message explicite est affiché (cas jugé critique).
' - V1.2.0 (15/10/2026, UI-7) : carte « Motif de l'annulation », fond AltheaFondFenetre ; couleurs
'   d'erreur tirées de la charte (AltheaIconeEtat.Teintes) au lieu de Firebrick / MistyRose.
' - V1.3.0 (17/10/2026) : l'encaissement annulé est nommé par son code dans l'en-tête (paramètre codePaiement).
' - V1.4.0 (20/10/2026) : motif dans une AltheaZoneTexte : correcteur d'orthographe de Windows, texte
'   simple ; Entrée valide toujours.
' - V1.5.0 (22/10/2026, audit B.5) : le motif est enregistré dans sa colonne (paiements.motif_annulation,
'   v2.51.0) ; sa longueur est limitée à GestionPaiements.LONGUEUR_MAX_MOTIF_ANNULATION.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class DialogMotifAnnulation

    Private _couleurFondNormal As Color

    ' Motif saisi (renseigné après DialogResult.OK)
    Public ReadOnly Property Motif As String
        Get
            Return txtMotif.Text.Trim()
        End Get
    End Property

    ' codePaiement : encaissement annulé, nommé dans l'en-tête (facultatif).
    Public Sub New(description As String, Optional codePaiement As String = "")

        InitializeComponent()

        ' Fond du kit : beige clair et petite feuille dessinée.
        AltheaFondFenetre.Appliquer(Me)

        If Not String.IsNullOrWhiteSpace(codePaiement) Then
            enTete.SousTitre = $"Annulez l'encaissement {codePaiement}"
        End If

        ' Erreur : teintes de la charte (même rose que les messages d'erreur DialogChoix).
        lblErreur.ForeColor = AltheaIconeEtat.Teintes(EtatIcone.Erreur).Foncee

        lblInfo.Text = description & Environment.NewLine &
            "La ligne de ce paiement sera conservée pour traçabilité (statut Annulé) mais ne comptera plus comme règlement : la séance redeviendra due (ou l'avoir sera réduit d'autant)."
        _couleurFondNormal = txtMotif.BackColor

        ' Même limite que la colonne : la saisie s'arrête là au lieu d'être refusée ensuite.
        txtMotif.MaxLength = GestionPaiements.LONGUEUR_MAX_MOTIF_ANNULATION

    End Sub

    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click

        If String.IsNullOrWhiteSpace(txtMotif.Text) Then
            txtMotif.BackColor = AltheaIconeEtat.Teintes(EtatIcone.Erreur).Pale
            lblErreur.Visible = True
            DialogChoix.Information(
                "L'annulation n'a pas été effectuée : le motif est obligatoire." & Environment.NewLine &
                "Saisissez la raison de l'annulation dans le champ signalé en rouge.",
                "Motif manquant")
            txtMotif.Focus()
            Return
        End If

        DialogResult = DialogResult.OK
        Close()

    End Sub

    Private Sub txtMotif_TextChanged(sender As Object, e As EventArgs) Handles txtMotif.TextChanged
        If Not String.IsNullOrWhiteSpace(txtMotif.Text) Then
            txtMotif.BackColor = _couleurFondNormal
            lblErreur.Visible = False
        End If
    End Sub

    Private Sub btnAnnuler_Click(sender As Object, e As EventArgs) Handles btnAnnuler.Click

    End Sub
End Class
