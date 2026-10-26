' -------------------------------------------------------------------------------------------------
' UserControl : UC_Accueil
' Projet      : Althéa
' Version     : V1.6.0
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Contrôle d’accueil affiché dans la zone centrale de l’application.
'
' Responsabilités :
' - Servir de point d’entrée visuel neutre avant la navigation vers un module fonctionnel.
' - Héberger provisoirement les outils de test de développement (carte « Outils de test »),
'   retirés du menu de la Home le 11/10/2026 ; l'accueil sera le dernier écran refait (lot 31).
'
' Important   :
' - Composant volontairement minimal à ce stade ; il ne porte ni logique métier ni accès aux données.
' - Les outils de test ne sont visibles que dans une version compilée en Debug : ils ne doivent pas
'   apparaître dans la version installée au cabinet.
'
' Dépendances :
' - Home et NavigationManager (chargement dans le panneau principal).
' - PasswordSecurityHelper, TestDialogChoix, TestChampDate, TestRichTextEditor, TestKitAlthea
'   (vitrine du kit, V1.5.0) (outils de test).
'
' Historique  :
' - V1.6.0 (20/10/2026) : l'outil « hash » demande le mot de passe (zone masquée) au lieu d'un mot
'   de passe écrit dans le code (audit § 2 ; ce mot de passe est à considérer comme compromis).
'   Bouton « Provoquer une erreur » : vérifie le filet des erreurs non interceptées (GestionErreursGlobales).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_Accueil

#Region "Cycle de vie"

    Private Sub UC_Accueil_Load(sender As Object, e As EventArgs) Handles MyBase.Load

#If DEBUG Then
        carteTests.Visible = True
#Else
        carteTests.Visible = False
#End If

    End Sub

#End Region

#Region "Outils de test (provisoire — à supprimer après usage)"

    ' PROVISOIRE : génère un sel et le hash d'un mot de passe saisi, pour amorcer un compte en base
    ' (avant l'installation). Aucun mot de passe dans le code (règle de sécurité du projet).
    Private Sub btnTestHash_Click(sender As Object, e As EventArgs) Handles btnTestHash.Click

        Dim motDePasse As String = DemanderMotDePasse()
        If String.IsNullOrEmpty(motDePasse) Then Return

        Dim salt As String = PasswordSecurityHelper.GenerateSalt()
        Dim newHash As String = PasswordSecurityHelper.HashPassword(motDePasse, salt)

        Debug.WriteLine($"Salt : {salt}{Environment.NewLine}{Environment.NewLine}Hash : {newHash}")
        DialogChoix.Information($"Salt : {salt}{Environment.NewLine}{Environment.NewLine}Hash : {newHash}", "Debug Hash")

    End Sub

    ' Petite fenêtre de saisie masquée (outil de test seulement) ; Nothing si abandon.
    Private Function DemanderMotDePasse() As String

        Using fenetre As New Form With {
            .Text = "Hash d'un mot de passe",
            .FormBorderStyle = FormBorderStyle.FixedDialog,
            .StartPosition = FormStartPosition.CenterParent,
            .MinimizeBox = False,
            .MaximizeBox = False,
            .ShowInTaskbar = False,
            .ClientSize = New Size(380, 128),
            .Font = Font
        }
            Dim libelle As New Label With {.Text = "Mot de passe à hacher :", .AutoSize = True, .Location = New Point(16, 16)}
            Dim saisie As New TextBox With {.UseSystemPasswordChar = True, .Location = New Point(16, 40), .Width = 348}
            Dim btnOk As New AltheaBouton With {.Text = "Calculer", .Variante = VarianteBouton.Sauge,
                                                .DialogResult = DialogResult.OK, .Bounds = New Rectangle(160, 80, 100, 32)}
            Dim btnAnnuler As New AltheaBouton With {.Text = "Annuler", .Variante = VarianteBouton.Clair,
                                                     .DialogResult = DialogResult.Cancel, .Bounds = New Rectangle(266, 80, 100, 32)}

            fenetre.Controls.AddRange(New Control() {libelle, saisie, btnOk, btnAnnuler})
            fenetre.AcceptButton = btnOk
            fenetre.CancelButton = btnAnnuler
            AltheaFondFenetre.Appliquer(fenetre)

            Return If(fenetre.ShowDialog(FindForm()) = DialogResult.OK, saisie.Text, Nothing)
        End Using

    End Function

    ' TEST : erreur volontaire, NON interceptée, pour vérifier le filet global (message clair, détail au
    ' journal, l'application continue).
    Private Sub btnTestErreur_Click(sender As Object, e As EventArgs) Handles btnTestErreur.Click
        Throw New InvalidOperationException("Erreur de test volontaire (outil de test de l'accueil).")
    End Sub

    ' PROVISOIRE : ouvre la démonstration de DialogChoix V2 (tous les cas, icônes d'état).
    Private Sub btnTestDialogChoix_Click(sender As Object, e As EventArgs) Handles btnTestDialogChoix.Click

        Using fenetre As New TestDialogChoix()
            fenetre.ShowDialog(FindForm())
        End Using

    End Sub

    ' PROVISOIRE : ouvre la démonstration du champ de date du kit (AltheaDate, AltheaCalendrier).
    Private Sub btnTestChampDate_Click(sender As Object, e As EventArgs) Handles btnTestChampDate.Click

        Using fenetre As New TestChampDate()
            fenetre.ShowDialog(FindForm())
        End Using

    End Sub

    ' PROVISOIRE : ouvre la vitrine du kit (tous les contrôles Althéa dans leurs états, captures).
    Private Sub btnTestVitrineKit_Click(sender As Object, e As EventArgs) Handles btnTestVitrineKit.Click

        Using fenetre As New TestKitAlthea()
            fenetre.ShowDialog(FindForm())
        End Using

    End Sub

    ' PROVISOIRE : ouvre la fenêtre de démonstration de l'éditeur de texte riche.
    Private Sub btnTestRichTextEditor_Click(sender As Object, e As EventArgs) Handles btnTestRichTextEditor.Click

        Using fenetre As New TestRichTextEditor()
            fenetre.ShowDialog(FindForm())
        End Using

    End Sub

#End Region

End Class
