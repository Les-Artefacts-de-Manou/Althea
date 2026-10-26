' -------------------------------------------------------------------------------------------------
' Form       : TestDialogChoix
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 14/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Fenêtre de démonstration de DialogChoix V2 (UI-7) : chaque bouton ouvre un cas typique
' (information, erreur, confirmations, suppression, récapitulatif structuré, textes longs,
' attente) ; le résultat renvoyé s'affiche pour vérifier Entrée, Échap et les boutons.
' La carte de droite présente les icônes d'état (AltheaIconeEtat) en 48 et 24 px.
'
' Remarques  :
' - Outil de test et de captures d'écran : ouverte depuis le bouton de test DialogChoix de
'   UC_Accueil (Debug seulement). Aucune donnée réelle : textes d'exemple uniquement.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class TestDialogChoix

#Region "Cas simples (méthodes statiques)"

    Private Sub btnInformation_Click(sender As Object, e As EventArgs) Handles btnInformation.Click
        DialogChoix.Information("Choisissez d'abord l'élément à rattacher dans la liste.", "Rattachement")
        AfficherResultat(DialogResult.OK)
    End Sub

    Private Sub btnAvertissement_Click(sender As Object, e As EventArgs) Handles btnAvertissement.Click
        DialogChoix.Avertissement("Chaque plafond doit être un nombre entier positif ou nul.", "Règle de quota")
        AfficherResultat(DialogResult.OK)
    End Sub

    Private Sub btnErreur_Click(sender As Object, e As EventArgs) Handles btnErreur.Click
        DialogChoix.Erreur("La règle n'a pas pu être supprimée. Rien n'a été modifié." & Environment.NewLine &
                           "Le détail est dans le journal (Administration › Journaux).", "Suppression impossible")
        AfficherResultat(DialogResult.OK)
    End Sub

    Private Sub btnSucces_Click(sender As Object, e As EventArgs) Handles btnSucces.Click
        DialogChoix.Succes("La sauvegarde a été créée.", "Sauvegarde")
        AfficherResultat(DialogResult.OK)
    End Sub

    Private Sub btnConfirmerOuiNon_Click(sender As Object, e As EventArgs) Handles btnConfirmerOuiNon.Click
        AfficherResultat(DialogChoix.Confirmer("Quitter sans enregistrer les modifications ?", "Modifications en cours"))
    End Sub

    Private Sub btnQuestion_Click(sender As Object, e As EventArgs) Handles btnQuestion.Click
        AfficherResultat(DialogChoix.Question("Enregistrer les modifications avant de fermer la fiche ?", "Fiche patient"))
    End Sub

#End Region

#Region "Cas V2 : boutons qui disent l'action, parties structurées"

    Private Sub btnConfirmerAction_Click(sender As Object, e As EventArgs) Handles btnConfirmerAction.Click
        AfficherResultat(DialogChoix.Confirmer("Clôturer cette règle au 31/12/2026 ?", "Clôture de la règle", "Clôturer", "Annuler"))
    End Sub

    Private Sub btnSuppression_Click(sender As Object, e As EventArgs) Handles btnSuppression.Click
        AfficherResultat(DialogChoix.ConfirmerSuppression(
            "Ce rattachement sera supprimé définitivement. Cette action est irréversible.", "Supprimer le rattachement ?"))
    End Sub

    Private Sub btnSuppressionAttention_Click(sender As Object, e As EventArgs) Handles btnSuppressionAttention.Click
        AfficherResultat(DialogChoix.ConfirmerSuppression(
            "Cette règle (saisie erronée) sera supprimée définitivement." & Environment.NewLine & Environment.NewLine &
            "La règle précédente (Soutien 10 + Traitement 20 = 30) avait été arrêtée le 30/09/2026 à cause de celle-ci : " &
            "elle sera rouverte, sans date de fin.",
            "Supprimer la règle ?",
            "Après la suppression, aucune règle ne couvrira du 01/11/2026 au 30/11/2026. Pensez à la couvrir par une « Nouvelle règle »."))
    End Sub

    Private Sub btnRecapitulatif_Click(sender As Object, e As EventArgs) Handles btnRecapitulatif.Click

        Using dlg As New DialogChoix()
            dlg.Titre = "Nouvelle règle de quota"
            dlg.TypeDialogue = TypeDialogue.Question
            dlg.Message = "Vérifiez la nouvelle règle avant de l'enregistrer :"
            dlg.AjouterDetail("Plafonds", "Soutien 10 + Traitement 20 = 30")
            dlg.AjouterDetail("Alerte", "à 28 séances")
            dlg.AjouterDetail("Validité", "à partir du 01/01/2027")
            dlg.Complement = "La règle actuelle (Soutien 8 = 8) s'arrêtera le 31/12/2026."
            dlg.Attention = "Avec cette règle, aucune règle ne couvrira du 15/10/2026 au 31/10/2026. Les séances de cette période " &
                            "ne seront soumises à aucun quota ; pour les y soumettre, créez la règle qui manque (« Nouvelle règle »)."
            dlg.Remarque = "La règle restera corrigeable tant qu'aucune séance n'est concernée ; ensuite, elle ne se modifie plus."
            dlg.QuestionFinale = "Enregistrer cette règle ?"
            dlg.VarianteAction = VarianteBouton.Ocre
            dlg.SetBoutons("Enregistrer", "Revenir à la saisie")
            AfficherResultat(dlg.ShowDialog(Me))
        End Using

    End Sub

    ' Même présentation que le contrôle de quota de SeanceEditor (message libre, deux boutons nommés).
    Private Sub btnQuotaDepasse_Click(sender As Object, e As EventArgs) Handles btnQuotaDepasse.Click

        Using dlg As New DialogChoix()
            dlg.Titre = "Quota dépassé"
            dlg.TypeDialogue = TypeDialogue.Warning
            dlg.Message = "Cette séance dépasse le quota remboursé du dossier."
            dlg.AjouterDetail("Soutien", "11 séances pour un plafond de 10")
            dlg.Complement = "Au-delà du plafond, la séance n'est plus couverte par la convention."
            dlg.QuestionFinale = "Enregistrer quand même cette séance ?"
            dlg.SetBoutons("Enregistrer quand même", "Annuler")
            AfficherResultat(dlg.ShowDialog(Me))
        End Using

    End Sub

    Private Sub btnTitreLong_Click(sender As Object, e As EventArgs) Handles btnTitreLong.Click
        DialogChoix.Avertissement(
            "Des séances ont déjà été enregistrées avec la nature « Séance individuelle de suivi » dans le domaine « Realism ». " &
            "La suppression est impossible afin de préserver l'historique ; utilisez « Retirer » pour ne plus la proposer à la saisie.",
            "Suppression impossible : la nature de séance est déjà utilisée dans ce domaine")
        AfficherResultat(DialogResult.OK)
    End Sub

    Private Sub btnMessageTresLong_Click(sender As Object, e As EventArgs) Handles btnMessageTresLong.Click

        Dim lignes As New List(Of String) From {"Liste des contrôles effectués avant la restauration :", String.Empty}
        For i As Integer = 1 To 45
            lignes.Add($"• Contrôle n° {i} : vérification réussie, aucune anomalie constatée sur cet élément de l'archive.")
        Next
        lignes.Add(String.Empty)
        lignes.Add("Fin de la liste : la fenêtre ne dépasse pas 70 % de l'écran, le texte défile.")

        DialogChoix.Information(String.Join(Environment.NewLine, lignes), "Message très long")
        AfficherResultat(DialogResult.OK)

    End Sub

    Private Sub btnAttente_Click(sender As Object, e As EventArgs) Handles btnAttente.Click

        Using dlg As New DialogChoix()
            dlg.Titre = "Opération en cours"
            dlg.TypeDialogue = TypeDialogue.Loading
            dlg.Message = "Démonstration de l'icône d'attente : l'arc tourne tant que la fenêtre est ouverte."
            dlg.SetBoutons("OK")
            AfficherResultat(dlg.ShowDialog(Me))
        End Using

    End Sub

#End Region

#Region "Divers"

    Private Sub AfficherResultat(resultat As DialogResult)
        lblResultat.Text = $"Dernier résultat : {resultat}"
    End Sub

    Private Sub btnFermer_Click(sender As Object, e As EventArgs) Handles btnFermer.Click
        Close()
    End Sub

#End Region

End Class
