' -------------------------------------------------------------------------------------------------
' Module      : OuvertureResultat
' Projet      : Althéa
' Version     : V1.2.1
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Aller à un élément trouvé par la recherche transversale (ADR-52) : une seule routine pour toutes
' les recherches, qui décide de l'écran selon la nature du résultat.
'
' Fonctionnement (décisions de Joëlle, 23/10/2026) :
' - Patient → sa fiche ; contact → fiche, onglet Famille / Contacts ; intervenant → fiche, onglet
'   Intervenants.
' - Dossier → son hub ; séance → hub, onglet Séances, séance sélectionnée ; paiement → hub, onglet
'   Paiements, paiement sélectionné.
' - Thérapeute (code TH…, décision de Joëlle du 24/10/2026) → le réseau par nom sur ce thérapeute et
'   ses patients (UC_RechercheReseau).
' - L'écran ouvert est empilé sur la recherche (IFenetrePrincipale.NavigateDepuisRecherche) : Retour y ramène.
'
' Remarques   :
' - Les droits sont ceux des écrans ouverts (fiche patient, dossier), comme depuis l'écran Patients.
' - V1.1.0 (23/10/2026, recherche R2) : contexte de la recherche d'origine (« Recherche > Séances »),
'   réaffiché au retour et en tête du contexte de l'écran ouvert.
' - V1.2.0 (24/10/2026) : nature Thérapeute (OuvrirTherapeute).
' - V1.2.1 (25/10/2026, audit C.4 étape 3, ADR-55) : fenêtre principale par IFenetrePrincipale
'   (lue dans le contexte) ; plus aucune référence à Home.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module OuvertureResultat

    ' Contexte du hub Recherche (contexte par défaut des écrans ouverts depuis une recherche).
    Public Const CONTEXTE_RECHERCHE As String = "Recherche"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : Ouvrir
    ' Version   : V1.2.1
    ' Date      : 25/10/2026
    '
    ' Rôle      :
    ' Ouvre l'écran de l'élément (voir l'en-tête du module).
    '
    ' Paramètres :
    ' - fenetre           : fenêtre principale (IFenetrePrincipale)
    ' - resultat          : élément trouvé
    ' - creerEcranCourant : fabrique recréant la recherche au retour
    ' - context           : contexte UI (barre d'état), facultatif
    ' - contexteRetour    : contexte de la recherche (« Recherche », « Recherche > Séances »…)
    ' - V1.2.1 (25/10/2026, audit C.4, ADR-55) : fenêtre principale par IFenetrePrincipale (contexte)
    '   au lieu de Home.
    ' -------------------------------------------------------------------------------------------------
    Public Sub Ouvrir(fenetre As IFenetrePrincipale, resultat As ResultatRecherche, creerEcranCourant As Func(Of UserControl),
                      Optional context As UserControlContext = Nothing,
                      Optional contexteRetour As String = CONTEXTE_RECHERCHE)

        If fenetre Is Nothing OrElse resultat Is Nothing Then Return

        Select Case resultat.Nature

            Case NatureResultat.Therapeute
                OuvrirTherapeute(fenetre, resultat, creerEcranCourant, contexteRetour)
                Return

            Case NatureResultat.Patient
                OuvrirFiche(fenetre, resultat, UC_PatientFiche.OngletPatient.Identite, creerEcranCourant, contexteRetour)

            Case NatureResultat.Contact
                OuvrirFiche(fenetre, resultat, UC_PatientFiche.OngletPatient.Famille, creerEcranCourant, contexteRetour)

            Case NatureResultat.Intervenant
                OuvrirFiche(fenetre, resultat, UC_PatientFiche.OngletPatient.Intervenants, creerEcranCourant, contexteRetour)

            Case NatureResultat.Dossier
                OuvrirDossier(fenetre, resultat, UC_DossierFiche.OngletDossier.Fiche, creerEcranCourant, contexteRetour)

            Case NatureResultat.Seance
                OuvrirDossier(fenetre, resultat, UC_DossierFiche.OngletDossier.Seances, creerEcranCourant, contexteRetour)

            Case NatureResultat.Paiement
                OuvrirDossier(fenetre, resultat, UC_DossierFiche.OngletDossier.Paiements, creerEcranCourant, contexteRetour)

        End Select

        If Not resultat.IdDossier.HasValue AndAlso resultat.IdPatient <= 0 Then
            context?.SetStatus($"{resultat.Code} n'est rattaché à aucun patient : rien à ouvrir.")
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : OuvrirPatient
    ' Rôle      : Ouvre la fiche du patient de l'élément (onglet Identité).
    ' -------------------------------------------------------------------------------------------------
    Public Sub OuvrirPatient(fenetre As IFenetrePrincipale, resultat As ResultatRecherche, creerEcranCourant As Func(Of UserControl),
                             Optional contexteRetour As String = CONTEXTE_RECHERCHE)
        OuvrirFiche(fenetre, resultat, UC_PatientFiche.OngletPatient.Identite, creerEcranCourant, contexteRetour)
    End Sub

    Private Sub OuvrirFiche(fenetre As IFenetrePrincipale, resultat As ResultatRecherche, onglet As UC_PatientFiche.OngletPatient,
                            creerEcranCourant As Func(Of UserControl), contexteRetour As String)

        If fenetre Is Nothing OrElse resultat Is Nothing OrElse resultat.IdPatient <= 0 Then Return

        Dim fiche As New UC_PatientFiche()
        fiche.InitialiserPourPatient(resultat.IdPatient, enModification:=False)
        fiche.SelectionnerOngletAuChargement(onglet)

        fenetre.NavigateDepuisRecherche(fiche, $"{contexteRetour} > {resultat.NomPatient}", creerEcranCourant, contexteRetour)

    End Sub

    ' Le réseau par nom sur ce thérapeute : ses patients, sa fiche (« Fiche de la personne »).
    Private Sub OuvrirTherapeute(fenetre As IFenetrePrincipale, resultat As ResultatRecherche,
                                 creerEcranCourant As Func(Of UserControl), contexteRetour As String)

        If Not resultat.IdTherapeute.HasValue Then Return

        Dim ecran As New UC_RechercheReseau()
        ecran.Prerenseigner(resultat.NomElement, False,
                            PersonneReseau.CleDe(NaturePersonneReseau.Therapeute, resultat.IdTherapeute.Value, Nothing, Nothing),
                            NatureResultat.Therapeute, 0L)

        fenetre.NavigateDepuisRecherche(ecran, UC_RechercheReseau.CONTEXTE, creerEcranCourant, contexteRetour)

    End Sub

    ' Le hub du dossier de l'élément, sur l'onglet demandé ; rien si l'élément n'a pas de dossier
    ' (paiement sans séance affectée).
    Private Sub OuvrirDossier(fenetre As IFenetrePrincipale, resultat As ResultatRecherche, onglet As UC_DossierFiche.OngletDossier,
                              creerEcranCourant As Func(Of UserControl), contexteRetour As String)

        If Not resultat.IdDossier.HasValue Then Return

        Dim hub As New UC_DossierFiche()
        hub.InitialiserPourDossier(resultat.IdDossier.Value)
        hub.CiblerAuChargement(onglet,
                               If(resultat.IdSeance.HasValue AndAlso onglet = UC_DossierFiche.OngletDossier.Seances, resultat.IdSeance.Value, 0L),
                               If(resultat.IdPaiement.HasValue, resultat.IdPaiement.Value, 0L))

        fenetre.NavigateDepuisRecherche(hub, $"{contexteRetour} > {resultat.NomPatient} > {resultat.CodeDossier}", creerEcranCourant, contexteRetour)

    End Sub

End Module
