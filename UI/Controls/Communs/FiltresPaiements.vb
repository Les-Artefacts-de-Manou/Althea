' -------------------------------------------------------------------------------------------------
' Module      : FiltresPaiements
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Relevé des paiements, commun à l'onglet Paiements du dossier et à la recherche des paiements
' d'une période (recherche transversale R3, ADR-52) : mêmes lignes, mêmes axes de filtre, même
' résumé.
'
' Responsabilités :
' - Dire quelles lignes forment le relevé (LignesDuReleve)
' - Remplir la liste « Filtre » (un élément par axe, à la couleur de sa pastille) et filtrer selon un axe
' - Construire le résumé cliquable (encaissé, en attente ; avoir et reste à régler quand les séances
'   sont connues)
'
' Remarques   :
' - L'ordre de AxeFiltrePaiement est celui de la liste (l'index choisi est l'axe).
' - Sorti de UC_DossierFiche le 23/10/2026 (V1.23.0) sans changement de comportement.
'
' Dépendances :
' - PaiementListeItem, SeanceListeItem, ElementListePastille, AltheaListe, ResumeFiltre, UtilsPastilles
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

' Axes du filtre des paiements, dans l'ordre de la liste « Filtre ».
Public Enum AxeFiltrePaiement
    Tous = 0                    ' Aucun filtre d'axe
    Encaisses = 1               ' Paiements effectivement encaissés
    EnAttente = 2               ' Paiements prévus, non encore encaissés
    Annules = 3                 ' Paiements annulés
    SeancesSoldees = 4          ' Séances dues dont le solde est entièrement réglé
    SeancesDues = 5             ' Séances dues restant partiellement dues
    Avoirs = 6                  ' Avoirs (séance non due encaissée, trop-perçu après baisse de tarif)
    Reports = 7                 ' Mouvements liés à un report (séance reportée ou issue d'un report)
    ARegler = 8                 ' Séance passée et due, pas entièrement réglée (« Reste à régler »)
End Enum

Public Module FiltresPaiements

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : LignesDuReleve
    ' Rôle      : Les lignes qui forment le relevé : toutes, sauf les dus en attente clos par
    '             l'application (séance réglée, non due, reportée), qui ne sont pas des mouvements
    '             pour l'utilisatrice — ils restent visibles dans le détail d'une séance.
    ' -------------------------------------------------------------------------------------------------
    Public Function LignesDuReleve(Of T As PaiementListeItem)(paiements As IEnumerable(Of T)) As IEnumerable(Of T)
        If paiements Is Nothing Then Return Enumerable.Empty(Of T)()
        Return paiements.Where(Function(p) Not p.EstAttenteClose)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirListe
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Remplit la liste « Filtre » avec les axes, dans l'ordre de AxeFiltrePaiement, chacun à la
    ' couleur de sa pastille (21/10/2026, comme le filtre des séances) ; « Tous » est sélectionné.
    ' -------------------------------------------------------------------------------------------------
    Public Sub RemplirListe(liste As AltheaListe)

        If liste Is Nothing Then Return

        liste.Items.Clear()
        liste.Items.Add(New ElementListePastille("Tous"))
        liste.Items.Add(New ElementListePastille("Encaissés", UtilsPastilles.Teinte(FamillePaiementSeance.Reglee)))
        liste.Items.Add(New ElementListePastille("En attente", UtilsPastilles.Teinte(FamillePaiementSeance.Prevue)))
        liste.Items.Add(New ElementListePastille("Annulés", UtilsPastilles.Teinte(FamillePaiementSeance.NonDue)))
        liste.Items.Add(New ElementListePastille("Séances soldées", UtilsPastilles.Teinte(FamillePaiementSeance.Reglee)))
        liste.Items.Add(New ElementListePastille("Séances restant dues", UtilsPastilles.Teinte(FamillePaiementSeance.ARegler)))
        liste.Items.Add(New ElementListePastille("Avoirs", UtilsPastilles.Teinte(FamillePaiementSeance.Avoir)))
        liste.Items.Add(New ElementListePastille("Séances reportées", UtilsPastilles.Teinte(FamillePaiementSeance.NonDue)))
        liste.Items.Add(New ElementListePastille("À régler", UtilsPastilles.Teinte(FamillePaiementSeance.ARegler)))

        liste.SelectedIndex = AxeFiltrePaiement.Tous

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : Axe
    ' Rôle      : Axe choisi dans la liste « Filtre » (Tous si rien n'est choisi).
    ' -------------------------------------------------------------------------------------------------
    Public Function Axe(liste As AltheaListe) As AxeFiltrePaiement
        If liste Is Nothing OrElse liste.SelectedIndex < 0 Then Return AxeFiltrePaiement.Tous
        Return CType(liste.SelectedIndex, AxeFiltrePaiement)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : Filtrer
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Les lignes qui relèvent d'un axe (toutes pour « Tous »).
    '
    ' Remarques :
    ' - Avoirs : aussi les encaissements d'une séance due avec trop-perçu (le reste dû de la requête
    '   est borné à 0 : seul AvoirSeance le révèle).
    ' -------------------------------------------------------------------------------------------------
    Public Function Filtrer(Of T As PaiementListeItem)(paiements As IEnumerable(Of T), axe As AxeFiltrePaiement) As IEnumerable(Of T)

        If paiements Is Nothing Then Return Enumerable.Empty(Of T)()

        Select Case axe
            Case AxeFiltrePaiement.Encaisses
                Return paiements.Where(Function(p) p.EstEncaisse)
            Case AxeFiltrePaiement.EnAttente
                Return paiements.Where(Function(p) Not p.EstEncaisse AndAlso Not p.EstAnnule)
            Case AxeFiltrePaiement.Annules
                Return paiements.Where(Function(p) p.EstAnnule)
            Case AxeFiltrePaiement.SeancesSoldees
                Return paiements.Where(Function(p) p.SeanceEstDue AndAlso p.SeanceSoldee)
            Case AxeFiltrePaiement.SeancesDues
                Return paiements.Where(Function(p) p.SeanceEstDue AndAlso Not p.SeanceSoldee)
            Case AxeFiltrePaiement.Avoirs
                Return paiements.Where(Function(p) p.EstAvoir OrElse p.EstSurSeanceAvecTropPercu)
            Case AxeFiltrePaiement.Reports
                Return paiements.Where(Function(p) p.SeanceReportee OrElse p.SeanceIssueDeReport)
            Case AxeFiltrePaiement.ARegler
                Return paiements.Where(Function(p) p.EstARegler)
            Case Else
                Return paiements
        End Select

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourResume
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Reconstruit le résumé cliquable d'un relevé : total encaissé, total en attente ; avec les
    ' séances (dossier), avoir disponible et reste à régler.
    '
    ' Paramètres :
    ' - resumePaiements : résumé de l'écran (ResumeFiltre branché sur sa liste « Filtre »)
    ' - paiements       : toutes les lignes chargées (pas seulement celles affichées)
    ' - seances         : séances du dossier, ou Nothing (recherche : pas d'avoir ni de reste global)
    '
    ' Remarques :
    ' - Avoir et reste à régler viennent des séances (SeanceListeItem.AvoirSeance / MontantExigible) :
    '   mêmes montants que le résumé de l'onglet Séances et que l'avoir du bandeau du dossier.
    ' - « En attente » et « Reste à régler » se recoupent : un paiement attendu pour une séance passée
    '   compte dans les deux (il est attendu, et en retard).
    ' - Les montants sont ceux affectés aux séances : un paiement réparti ou un avoir réutilisé n'est
    '   compté qu'une fois (l'avoir utilisé quitte la ligne d'origine).
    ' - Friend : ResumeFiltre est interne au projet.
    ' -------------------------------------------------------------------------------------------------
    Friend Sub MettreAJourResume(resumePaiements As ResumeFiltre, paiements As IEnumerable(Of PaiementListeItem),
                                 Optional seances As IEnumerable(Of SeanceListeItem) = Nothing)

        If resumePaiements Is Nothing Then Return

        resumePaiements.Reconstruire(
            Sub()
                If paiements Is Nothing OrElse Not paiements.Any() Then Return

                resumePaiements.AjouterMontant(paiements.Where(Function(p) p.EstEncaisse).Sum(Function(p) p.MontantAffecte),
                                               "Encaissé", UtilsPastilles.Teinte(FamillePaiementSeance.Reglee), AxeFiltrePaiement.Encaisses)
                resumePaiements.AjouterMontant(paiements.Where(Function(p) Not p.EstEncaisse AndAlso Not p.EstAnnule).Sum(Function(p) p.MontantAffecte),
                                               "En attente", UtilsPastilles.Teinte(FamillePaiementSeance.Prevue), AxeFiltrePaiement.EnAttente)

                If seances Is Nothing Then Return

                resumePaiements.AjouterMontant(seances.Sum(Function(s) s.AvoirSeance),
                                               "Avoir disponible", UtilsPastilles.Teinte(FamillePaiementSeance.Avoir), AxeFiltrePaiement.Avoirs)
                resumePaiements.AjouterMontant(seances.Sum(Function(s) s.MontantExigible),
                                               "Reste à régler", UtilsPastilles.Teinte(FamillePaiementSeance.ARegler), AxeFiltrePaiement.ARegler)
            End Sub)

    End Sub

End Module
