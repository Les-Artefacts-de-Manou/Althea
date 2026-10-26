' -------------------------------------------------------------------------------------------------
' Form       : TestChampDate
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 14/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Fenêtre de démonstration du champ de date du kit (AltheaDate, UI-7) : date obligatoire,
' facultative, bornée, filtre du / au, passage en lecture seule par la règle commune
' (UtilsControls.DefinirLectureSeule), et calendrier seul (AltheaCalendrier).
'
' Remarques  :
' - Outil de test et de captures d'écran : ouverte depuis le bouton de test « Champ de date » de
'   UC_Accueil (Debug seulement). Aucune donnée réelle.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class TestChampDate

    Private Sub TestChampDate_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        dateFacultative.Valeur = Nothing
        dateBornee.DateMin = Date.Today
        dateBornee.DateMax = Date.Today.AddYears(1)
        dateDu.Valeur = New Date(Date.Today.Year, Date.Today.Month, 1)
        dateAu.Valeur = Date.Today

    End Sub

    ' Toute date qui change s'inscrit dans le journal (vérifie l'évènement ValeurChangee).
    Private Sub Date_ValeurChangee(sender As Object, e As EventArgs) Handles dateNormale.ValeurChangee, dateFacultative.ValeurChangee,
                                                                             dateBornee.ValeurChangee, dateDu.ValeurChangee, dateAu.ValeurChangee
        Dim champ As AltheaDate = DirectCast(sender, AltheaDate)
        Dim texte As String = If(champ.Valeur.HasValue, champ.Valeur.Value.ToString("dd/MM/yyyy"), "(aucune date)")
        lblJournal.Text = $"Dernier changement : {champ.Name} = {texte} à {Date.Now:HH:mm:ss}"
    End Sub

    Private Sub calendrierSeul_DateValidee(sender As Object, choix As Date) Handles calendrierSeul.DateValidee
        lblJournal.Text = $"Calendrier seul : {choix:dd/MM/yyyy} choisi à {Date.Now:HH:mm:ss}"
    End Sub

    Private Sub chkLectureSeule_CheckedChanged(sender As Object, e As EventArgs) Handles chkLectureSeule.CheckedChanged
        ' La case elle-même reste utilisable : on bascule les champs un par un.
        For Each champ As AltheaDate In {dateNormale, dateFacultative, dateBornee, dateDu, dateAu}
            champ.LectureSeule = chkLectureSeule.Checked
        Next
    End Sub

    Private Sub btnFermer_Click(sender As Object, e As EventArgs) Handles btnFermer.Click
        Close()
    End Sub

End Class
