' -------------------------------------------------------------------------------------------------
' Module      : FiltresSeances
' Projet      : Althéa
' Version     : V1.0.1
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Filtres et résumé d'une liste de séances, communs à l'onglet Séances du dossier et à la recherche
' des séances d'une période (recherche transversale R2, ADR-52) : mêmes axes, mêmes couleurs, même
' définition de chaque axe.
'
' Responsabilités :
' - Remplir la liste « Filtre » (un élément par axe, à la couleur de sa pastille)
' - Filtrer une liste de séances selon un axe
' - Construire le résumé cliquable (nombres par famille, montants à régler, prévus, en avoir)
'
' Remarques   :
' - L'ordre de AxeFiltreSeance est celui de la liste (l'index choisi est l'axe) : temps, puis statut,
'   puis argent (demandes de Joëlle du 21/10/2026).
' - Les axes se lisent sur les propriétés de SeanceListeItem (famille du statut, dû, solde), jamais
'   sur une liste de codes : un statut ajouté au référentiel se range de lui-même.
' - Sorti de UC_DossierFiche le 23/10/2026 (V1.22.0) sans changement de comportement.
' - V1.0.1 (24/10/2026, audit C.3) : code « Reportée » lu dans CodesSysteme.StatutsSeance.
'
' Dépendances :
' - SeanceListeItem, GestionSeances (code du statut Reportée), ElementListePastille, AltheaListe,
'   ResumeFiltre, UtilsPastilles
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

' Axes du filtre des séances, dans l'ordre de la liste « Filtre ».
Public Enum AxeFiltreSeance
    Toutes = 0                  ' Aucun filtre d'axe
    Aujourdhui = 1              ' Séances du jour, quel que soit leur statut
    AVenir = 2                  ' Séances dues pas encore passées (famille « à venir »)
    AConfirmer = 3              ' Planifiées dont l'heure est passée (ADR-47)
    Passees = 4                 ' Séances passées, quel que soit leur statut
    Realisees = 5               ' Séances dues, passées et confirmées
    AbsencesSansPrevenir = 6    ' Absences non excusées (no-show)
    Reportees = 7               ' Séances reportées (statut Reportée)
    NonDues = 8                 ' Séances dont le statut ne génère pas de dû
    Exigibles = 9               ' Séances dues avec un reste à payer
    Reglees = 10                ' Séances dues entièrement soldées
    AvecAvoir = 11              ' Séances qui portent un avoir
End Enum

Public Module FiltresSeances

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirListe
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Remplit la liste « Filtre » avec les axes, dans l'ordre de AxeFiltreSeance, chacun à la couleur
    ' de sa pastille ; « Toutes » est sélectionné.
    '
    ' Remarques :
    ' - « Aujourd'hui » en bleu plein (comme le rendez-vous du jour de la grille Patients) ;
    '   « Passées », axe large, au trait (demande de Joëlle : ils se perdaient en texte).
    ' -------------------------------------------------------------------------------------------------
    Public Sub RemplirListe(liste As AltheaListe)

        If liste Is Nothing Then Return

        liste.Items.Clear()
        liste.Items.Add(New ElementListePastille("Toutes"))
        liste.Items.Add(New ElementListePastille("Aujourd'hui", UtilsPastilles.Teinte(FamilleRendezVousPatient.AujourdhuiAVenir)))
        liste.Items.Add(New ElementListePastille("À venir", UtilsPastilles.Teinte(FamilleStatutSeance.AVenir)))
        liste.Items.Add(New ElementListePastille("À confirmer", UtilsPastilles.Teinte(FamilleStatutSeance.AConfirmer)))
        liste.Items.Add(New ElementListePastille("Passées", UtilsPastilles.Teinte(TonEtat.Inactif), contour:=True))
        liste.Items.Add(New ElementListePastille("Réalisées", UtilsPastilles.Teinte(FamilleStatutSeance.Realisee)))
        liste.Items.Add(New ElementListePastille("Absences sans prévenir", UtilsPastilles.Teinte(FamilleStatutSeance.AbsenceNonExcusee)))
        liste.Items.Add(New ElementListePastille("Reportées", UtilsPastilles.Teinte(FamilleStatutSeance.NonDue)))
        liste.Items.Add(New ElementListePastille("Non dues (annulées / maladie)", UtilsPastilles.Teinte(FamilleStatutSeance.NonDue)))
        liste.Items.Add(New ElementListePastille("Exigibles (impayées)", UtilsPastilles.Teinte(FamillePaiementSeance.ARegler)))
        liste.Items.Add(New ElementListePastille("Réglées", UtilsPastilles.Teinte(FamillePaiementSeance.Reglee)))
        liste.Items.Add(New ElementListePastille("Avec avoir", UtilsPastilles.Teinte(FamillePaiementSeance.Avoir)))

        liste.SelectedIndex = AxeFiltreSeance.Toutes

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : Axe
    ' Rôle      : Axe choisi dans la liste « Filtre » (Toutes si rien n'est choisi).
    ' -------------------------------------------------------------------------------------------------
    Public Function Axe(liste As AltheaListe) As AxeFiltreSeance
        If liste Is Nothing OrElse liste.SelectedIndex < 0 Then Return AxeFiltreSeance.Toutes
        Return CType(liste.SelectedIndex, AxeFiltreSeance)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction  : Filtrer
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Les séances qui relèvent d'un axe (toutes pour « Toutes »).
    ' -------------------------------------------------------------------------------------------------
    Public Function Filtrer(Of T As SeanceListeItem)(seances As IEnumerable(Of T), axe As AxeFiltreSeance) As IEnumerable(Of T)

        If seances Is Nothing Then Return Enumerable.Empty(Of T)()

        Select Case axe
            Case AxeFiltreSeance.Aujourdhui
                Return seances.Where(Function(s) s.DateSeance.Date = Date.Today)
            Case AxeFiltreSeance.AVenir
                ' Même définition que la pastille « à venir » du résumé : séances dues pas encore
                ' passées (une séance reportée ou annulée n'est pas un rendez-vous à venir).
                Return seances.Where(Function(s) s.FamilleStatut = FamilleStatutSeance.AVenir)
            Case AxeFiltreSeance.AConfirmer
                Return seances.Where(Function(s) s.EstAConfirmer)
            Case AxeFiltreSeance.Passees
                Return seances.Where(Function(s) s.EstPassee)
            Case AxeFiltreSeance.Realisees
                Return seances.Where(Function(s) s.FamilleStatut = FamilleStatutSeance.Realisee)
            Case AxeFiltreSeance.AbsencesSansPrevenir
                Return seances.Where(Function(s) s.EstNoShow)
            Case AxeFiltreSeance.Reportees
                Return seances.Where(Function(s) String.Equals(s.CodeStatutSeance, CodesSysteme.StatutsSeance.Reportee, StringComparison.OrdinalIgnoreCase))
            Case AxeFiltreSeance.NonDues
                Return seances.Where(Function(s) Not s.EstDuStatut)
            Case AxeFiltreSeance.Exigibles
                Return seances.Where(Function(s) s.MontantExigible > 0D)
            Case AxeFiltreSeance.Reglees
                Return seances.Where(Function(s) s.EstDuStatut AndAlso s.EstSoldee)
            Case AxeFiltreSeance.AvecAvoir
                Return seances.Where(Function(s) s.AvoirSeance > 0D)
            Case Else
                Return seances
        End Select

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourResume
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    '
    ' Rôle      :
    ' Reconstruit le résumé cliquable d'une liste de séances : nombre de séances à venir, à confirmer,
    ' réalisées, absences, non dues ; montants à régler, prévus et en avoir. Chaque pastille choisit
    ' l'axe correspondant.
    '
    ' Paramètres :
    ' - resumeSeances : résumé de l'écran (ResumeFiltre branché sur sa liste « Filtre »)
    ' - seances       : toutes les séances chargées (pas seulement celles affichées)
    '
    ' Remarques :
    ' - Une famille ou un montant nul n'a pas de pastille : le résumé ne montre que ce qui existe.
    ' - Friend : ResumeFiltre est interne au projet.
    ' -------------------------------------------------------------------------------------------------
    Friend Sub MettreAJourResume(resumeSeances As ResumeFiltre, seances As IEnumerable(Of SeanceListeItem))

        If resumeSeances Is Nothing Then Return

        resumeSeances.Reconstruire(
            Sub()
                If seances Is Nothing OrElse Not seances.Any() Then Return

                Dim nombre = Function(famille As FamilleStatutSeance) seances.Count(Function(s) s.FamilleStatut = famille)

                resumeSeances.AjouterNombre(nombre(FamilleStatutSeance.AVenir), "à venir", "à venir",
                                     UtilsPastilles.Teinte(FamilleStatutSeance.AVenir), AxeFiltreSeance.AVenir)
                resumeSeances.AjouterNombre(nombre(FamilleStatutSeance.AConfirmer), "à confirmer", "à confirmer",
                                     UtilsPastilles.Teinte(FamilleStatutSeance.AConfirmer), AxeFiltreSeance.AConfirmer)
                resumeSeances.AjouterNombre(nombre(FamilleStatutSeance.Realisee), "réalisée", "réalisées",
                                     UtilsPastilles.Teinte(FamilleStatutSeance.Realisee), AxeFiltreSeance.Realisees)
                resumeSeances.AjouterNombre(nombre(FamilleStatutSeance.AbsenceNonExcusee), "absence", "absences",
                                     UtilsPastilles.Teinte(FamilleStatutSeance.AbsenceNonExcusee), AxeFiltreSeance.AbsencesSansPrevenir)
                resumeSeances.AjouterNombre(nombre(FamilleStatutSeance.NonDue), "non due", "non dues",
                                     UtilsPastilles.Teinte(FamilleStatutSeance.NonDue), AxeFiltreSeance.NonDues)

                resumeSeances.AjouterSeparateur()

                resumeSeances.AjouterMontant(seances.Sum(Function(s) s.MontantExigible), "À régler",
                                      UtilsPastilles.Teinte(FamillePaiementSeance.ARegler), AxeFiltreSeance.Exigibles)
                resumeSeances.AjouterMontant(seances.Sum(Function(s) s.MontantPrevisionnel), "Prévu",
                                      UtilsPastilles.Teinte(FamillePaiementSeance.Prevue), AxeFiltreSeance.AVenir)
                resumeSeances.AjouterMontant(seances.Sum(Function(s) s.AvoirSeance), "Avoir",
                                      UtilsPastilles.Teinte(FamillePaiementSeance.Avoir), AxeFiltreSeance.AvecAvoir)
            End Sub)

    End Sub

End Module
