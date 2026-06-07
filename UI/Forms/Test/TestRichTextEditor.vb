' -------------------------------------------------------------------------------------------------
' PROVISOIRE : Form de test pour le UserControl UC_RichTextEditor
' Form       : TestRichTextEditor  
' Projet     : Althéa
' Version    : V1.0
' Date       : 16/05/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Form de test pour valider le UserControl UC_RichTextEditor.
'
' Responsabilités :
' - Tester le chargement de contenu RTF
' - Tester la sauvegarde RTF + TXT
' - Tester toutes les fonctionnalités de formatage
' - Tester les modes ReadOnly et ShowToolbar
'
' Remarques  :
' - Form temporaire pour développement/validation
' - Peut être supprimée ou désactivée en production
'
' Dépendances :
' - UC_RichTextEditor
'
' Imports    :
' - Option Strict On / Option Explicit On
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class TestRichTextEditor

#Region "Initialisation"

    Private Sub TestRichTextEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Contenu de test initial
        Dim contenuTest As String = "Bienvenue dans l'éditeur Althéa !" & Environment.NewLine & Environment.NewLine &
                                     "Testez toutes les fonctionnalités :" & Environment.NewLine &
                                     "• Gras, Italique, Souligné, Barré" & Environment.NewLine &
                                     "• Couleurs de texte et surbrillance" & Environment.NewLine &
                                     "• Polices et tailles" & Environment.NewLine &
                                     "• Alignement et retraits" & Environment.NewLine &
                                     "• Listes à puces" & Environment.NewLine &
                                     "• Insertion de date/heure" & Environment.NewLine &
                                     "• Impression" & Environment.NewLine & Environment.NewLine &
                                     "Raccourcis clavier : Ctrl+B (Gras), Ctrl+I (Italique), Ctrl+U (Souligné), Ctrl+F (Rechercher), Ctrl+P (Imprimer)"

        ucEditor.RtfContent = contenuTest

        ' Connecter les événements
        AddHandler ucEditor.ContentChanged, AddressOf OnContentChanged

    End Sub

#End Region

#Region "Gestionnaires d'événements"

    Private Sub OnContentChanged(sender As Object, e As EventArgs)
        lblStatus.Text = "Contenu modifié - Non sauvegardé"
        lblStatus.ForeColor = Color.Red
        btnCharger.Enabled = False
        btnSauvegarder.Enabled = True
    End Sub

    Private Sub btnSauvegarder_Click(sender As Object, e As EventArgs) Handles btnSauvegarder.Click

        ' Simuler une sauvegarde
        Dim rtfContent As String = ucEditor.RtfContent
        Dim txtContent As String = ucEditor.TextContent

        MessageBox.Show($"Sauvegarde simulée :{Environment.NewLine}{Environment.NewLine}" &
                       $"RTF : {rtfContent.Length} caractères{Environment.NewLine}" &
                       $"TXT : {txtContent.Length} caractères{Environment.NewLine}{Environment.NewLine}" &
                       $"Texte brut (100 premiers car.) :{Environment.NewLine}{If(txtContent.Length > 100, txtContent.Substring(0, 100), txtContent)}",
                       "Sauvegarde", MessageBoxButtons.OK, MessageBoxIcon.Information)

        lblStatus.Text = "Contenu sauvegardé"
        lblStatus.ForeColor = Color.Green
        btnSauvegarder.Enabled = False
        btnCharger.Enabled = True

    End Sub

    Private Sub btnCharger_Click(sender As Object, e As EventArgs) Handles btnCharger.Click

        ' Simuler un chargement depuis la base
        Dim contenuSimule As String = "{\rtf1\ansi\deff0 {\fonttbl {\f0 Calibri;}}" &
                                      "{\colortbl;\red255\green0\blue0;\red0\green0\blue255;}" &
                                      "\f0\fs22 Ceci est un \b contenu charg\'e9\b0  depuis la base de donn\'e9es.\par" &
                                      "Avec du \cf1 texte color\'e9\cf0  et du \i texte italique\i0.\par}"

        ucEditor.RtfContent = contenuSimule

        lblStatus.Text = "Contenu chargé depuis la base (simulé)"
        lblStatus.ForeColor = Color.Blue
        btnSauvegarder.Enabled = False

    End Sub

    Private Sub chkReadOnly_CheckedChanged(sender As Object, e As EventArgs) Handles chkReadOnly.CheckedChanged
        ucEditor.ReadOnlyMode = chkReadOnly.Checked
    End Sub

    Private Sub chkShowToolbar_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowToolbar.CheckedChanged
        ucEditor.ShowToolbar = chkShowToolbar.Checked
    End Sub

    Private Sub btnEffacer_Click(sender As Object, e As EventArgs) Handles btnEffacer.Click
        If MessageBox.Show("Effacer tout le contenu ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            ucEditor.RtfContent = String.Empty
            lblStatus.Text = "Contenu effacé"
            lblStatus.ForeColor = Color.Gray
        End If
    End Sub

#End Region

End Class
