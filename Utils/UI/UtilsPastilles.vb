' -------------------------------------------------------------------------------------------------
' Module      : UtilsPastilles
' Projet      : Althéa
' Version     : V1.7.1
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Couleurs des pastilles d'état (grilles, résumé cliquable) : une teinte (fond pâle + texte foncé)
' par famille d'affichage. Point unique : les grilles des séances et des paiements et leurs résumés
' lisent ici leurs couleurs.
'
' Remarques   :
' - Les familles viennent de la couche métier (SeanceListeItem.FamilleStatut / FamillePaiement,
'   PaiementListeItem.FamilleEtat) ;
'   les couleurs restent dans UITheme (palette des statuts et du kit) : rien n'est écrit en dur ici.
' - Maquette « Grille des séances » retenue le 17/10/2026 : bleu = à venir / prévu, vert-de-gris =
'   réalisée, orange = absence non excusée, gris = non due, vert = réglée, rose = à régler,
'   mauve = avoir.
' - V1.2.0 (17/10/2026) : pastille du statut d'un dossier (bandeau du dossier) — vert-de-gris =
'   actif, ocre = en pause, gris foncé = clôturé ou archivé (suivi terminé, bien visible).
' - V1.3.0 (17/10/2026, maquette « Patients — proposition ») : « En pause » passe à l'ocre
'   (StatutPause) partout — l'orange reste aux absences ; grille Patients : pastille de suivi
'   (clôturé / archivé en gris pâle : dans une liste, ce qui est terminé s'efface) et pastille du
'   rendez-vous (aujourd'hui en bleu plein, à venir bleu pâle, « Aucun RDV » au trait ocre).
' - V1.3.1 (17/10/2026) : « Aucun RDV » porte un fond ocre pâle, utilisé par le résumé de la grille.
' - V1.4.0 (18/10/2026, UI-9, maquette « Grilles — états lisibles ») : tons sémantiques (TonEtat :
'   OK, à surveiller, problème, inactif, information, rôle), pris dans les teintes existantes, pour
'   les grilles d'administration et de référentiels ; les familles ci-dessous s'appuient sur eux
'   quand leurs couleurs sont les mêmes (une seule définition, aucune couleur changée).
' - V1.5.0 (18/10/2026, UI-9.3) : écrans d'administration — état d'un compte utilisateur, niveau
'   d'une ligne de l'état du système, résultat d'une sauvegarde et d'une restauration.
' - V1.6.0 (18/10/2026, UI-9.4) : référentiels — état d'une ligne (système, inactive), d'un
'   rattachement nature / domaine et d'un rattachement tarif / domaine ; règle de quota.
' - V1.7.0 (21/10/2026, ADR-47) : séance « À confirmer » (planifiée passée) en ocre, ton « à surveiller ».
' - V1.7.1 (24/10/2026, audit C.3) : codes de statut de dossier lus dans
'   CodesSysteme.StatutsDossier.
'
' Dépendances :
' - UITheme, FamilleStatutSeance, FamillePaiementSeance, Dossier, FamilleRendezVousPatient, TonEtat,
'   EtatCompteUtilisateur, NiveauEtat, StatutSauvegarde, StatutRestauration, EtatReferentielLigne,
'   EtatLiaisonNature, EtatLiaisonPrix, EtatRegleQuota
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

''' <summary>
''' Ton sémantique d'une pastille (UI-9, 18/10/2026) : ce que l'état veut dire pour la personne qui
''' lit, indépendamment de l'écran. Choisi par l'écran à partir d'une famille fournie par le métier.
''' </summary>
Public Enum TonEtat
    ''' <summary>Tout va bien : actif, réussi, complet, défini (sauge).</summary>
    Ok
    ''' <summary>Rien de bloquant mais à regarder : à compléter, à vérifier, en pause (ocre).</summary>
    ASurveiller
    ''' <summary>Échec, verrouillé, erreur, à régler (rose).</summary>
    Probleme
    ''' <summary>Désactivé, terminé, annulé ; la ligne s'atténue aussi (gris).</summary>
    Inactif
    ''' <summary>À savoir, sans enjeu : à venir, ligne système, session (bleu).</summary>
    Information
    ''' <summary>Droits d'un compte : Admin, SuperUser (mauve).</summary>
    Role
End Enum

Public Module UtilsPastilles

    ''' <summary>Teinte d'une pastille : fond pâle et couleur du texte.</summary>
    Public Structure TeintePastille
        Public ReadOnly Fond As Color
        Public ReadOnly Texte As Color

        Public Sub New(fond As Color, texte As Color)
            Me.Fond = fond
            Me.Texte = texte
        End Sub
    End Structure

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Teinte (ton sémantique)
    ' Version  : V1.0.0
    ' Date     : 18/10/2026
    ' Rôle     : Couleurs d'un ton sémantique ; toutes viennent de la palette des statuts (UITheme).
    ' -------------------------------------------------------------------------------------------------
    Public Function Teinte(ton As TonEtat) As TeintePastille
        Select Case ton
            Case TonEtat.Ok
                Return New TeintePastille(UITheme.ColorGrisVertClair, UITheme.ColorSaugeTresFonce)
            Case TonEtat.ASurveiller
                Return New TeintePastille(UITheme.StatutPauseBack, UITheme.StatutPauseFore)
            Case TonEtat.Probleme
                Return New TeintePastille(UITheme.StatutDuBack, UITheme.StatutDuFore)
            Case TonEtat.Information
                Return New TeintePastille(UITheme.StatutPrevisionnelBack, UITheme.StatutPrevisionnelFore)
            Case TonEtat.Role
                Return New TeintePastille(UITheme.ColorMauveTresClair, UITheme.ColorMauveFonce)
            Case Else
                Return New TeintePastille(UITheme.StatutAnnuleBack, UITheme.StatutAnnuleFore)
        End Select
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Teinte (statut de séance)
    ' Rôle     : Couleurs de la pastille « Statut » de la grille des séances.
    ' -------------------------------------------------------------------------------------------------
    Public Function Teinte(famille As FamilleStatutSeance) As TeintePastille
        Select Case famille
            Case FamilleStatutSeance.AVenir
                Return Teinte(TonEtat.Information)
            Case FamilleStatutSeance.Realisee
                Return Teinte(TonEtat.Ok)
            Case FamilleStatutSeance.AConfirmer
                ' Issue à pointer : à regarder, sans rien de bloquant.
                Return Teinte(TonEtat.ASurveiller)
            Case FamilleStatutSeance.AbsenceNonExcusee
                ' Orange propre aux absences, distinct de l'ocre « à surveiller ».
                Return New TeintePastille(UITheme.StatutNoShowBack, UITheme.StatutNoShowFore)
            Case Else
                Return Teinte(TonEtat.Inactif)
        End Select
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Teinte (paiement d'une séance)
    ' Rôle     : Couleurs de la pastille « Paiement » de la grille des séances et de la pastille
    '            « État » de la grille des paiements (gris NonDue : paiement annulé ou reporté).
    ' -------------------------------------------------------------------------------------------------
    Public Function Teinte(famille As FamillePaiementSeance) As TeintePastille
        Select Case famille
            Case FamillePaiementSeance.Reglee
                Return New TeintePastille(UITheme.StatutRegleBack, UITheme.StatutRegleFore)
            Case FamillePaiementSeance.ARegler
                Return Teinte(TonEtat.Probleme)
            Case FamillePaiementSeance.Prevue
                Return Teinte(TonEtat.Information)
            Case FamillePaiementSeance.Avoir
                ' Même mauve que le ton « rôle », sans en partager le sens.
                Return New TeintePastille(UITheme.ColorMauveTresClair, UITheme.ColorMauveFonce)
            Case Else
                Return Teinte(TonEtat.Inactif)
        End Select
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : TeinteStatutDossier
    ' Rôle     : Couleurs de la pastille « Statut » du bandeau d'un dossier. Sans dossier (création),
    '            teinte d'un dossier actif.
    ' Remarque : seul cas inversé (fond foncé, texte blanc) : un suivi terminé doit se voir d'emblée.
    ' -------------------------------------------------------------------------------------------------
    Public Function TeinteStatutDossier(dossier As Dossier) As TeintePastille
        If dossier IsNot Nothing Then
            If dossier.EstCloture OrElse dossier.EstArchive Then
                Return New TeintePastille(UITheme.ColorGrisFonce, UITheme.ColorBlanc)
            End If
            If dossier.EstEnPause Then
                Return Teinte(TonEtat.ASurveiller)
            End If
        End If
        Return Teinte(TonEtat.Ok)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : TeinteSuiviListe
    ' Rôle     : Couleurs de la pastille « Suivi » de la grille Patients, d'après le code de statut du
    '            dossier dominant.
    ' Remarque : différente de TeinteStatutDossier pour un suivi terminé (gris pâle ici, gris foncé
    '            dans l'en-tête du dossier ouvert) : décision du 17/10/2026.
    ' -------------------------------------------------------------------------------------------------
    Public Function TeinteSuiviListe(codeStatut As String) As TeintePastille
        Select Case If(codeStatut, String.Empty).Trim().ToUpperInvariant()
            Case CodesSysteme.StatutsDossier.Actif
                Return Teinte(TonEtat.Ok)
            Case CodesSysteme.StatutsDossier.Pause
                Return Teinte(TonEtat.ASurveiller)
            Case Else
                Return Teinte(TonEtat.Inactif)
        End Select
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Teinte (rendez-vous d'un patient)
    ' Rôle     : Couleurs de la pastille « Prochain RDV » de la grille Patients. « Aucun RDV » se
    '            dessine au trait (UtilsDataGrid.DessinerPastilleCellule, contour).
    ' -------------------------------------------------------------------------------------------------
    Public Function Teinte(famille As FamilleRendezVousPatient) As TeintePastille
        Select Case famille
            Case FamilleRendezVousPatient.AujourdhuiAVenir
                Return New TeintePastille(UITheme.StatutAujourdhuiBack, UITheme.StatutAujourdhuiFore)
            Case FamilleRendezVousPatient.SansRendezVous
                ' Fond utilisé par la pastille du résumé ; la grille la dessine au trait, sans fond.
                Return Teinte(TonEtat.ASurveiller)
            Case Else
                Return Teinte(TonEtat.Information)
        End Select
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Teinte (état d'un compte utilisateur)
    ' Version  : V1.0.0
    ' Date     : 18/10/2026
    ' Rôle     : Pastille « État » de la liste des utilisateurs (Administration).
    ' -------------------------------------------------------------------------------------------------
    Public Function Teinte(etat As EtatCompteUtilisateur) As TeintePastille
        Select Case etat
            Case EtatCompteUtilisateur.Actif : Return Teinte(TonEtat.Ok)
            Case EtatCompteUtilisateur.Verrouille : Return Teinte(TonEtat.Probleme)
            Case Else : Return Teinte(TonEtat.Inactif)
        End Select
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Teinte (niveau d'une ligne de l'état du système)
    ' Version  : V1.0.0
    ' Date     : 18/10/2026
    ' Rôle     : Pastille « État » de l'état du système : trois tons distincts (« À vérifier » était
    '            rouge comme « Problème »). Une simple information ne porte pas de pastille (libellé
    '            vide, GestionEtatSysteme.LibelleNiveau).
    ' -------------------------------------------------------------------------------------------------
    Public Function Teinte(niveau As NiveauEtat) As TeintePastille
        Select Case niveau
            Case NiveauEtat.Ok : Return Teinte(TonEtat.Ok)
            Case NiveauEtat.Avertissement : Return Teinte(TonEtat.ASurveiller)
            Case NiveauEtat.Probleme : Return Teinte(TonEtat.Probleme)
            Case Else : Return Teinte(TonEtat.Information)
        End Select
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Teinte (résultat d'une sauvegarde)
    ' Version  : V1.0.0
    ' Date     : 18/10/2026
    ' Rôle     : Pastille « Résultat » de l'historique des sauvegardes. « Interrompue » (application
    '            fermée pendant la sauvegarde) n'est pas un échec de la sauvegarde elle-même : à surveiller.
    ' -------------------------------------------------------------------------------------------------
    Public Function Teinte(statut As StatutSauvegarde) As TeintePastille
        Select Case statut
            Case StatutSauvegarde.Reussie : Return Teinte(TonEtat.Ok)
            Case StatutSauvegarde.Echec : Return Teinte(TonEtat.Probleme)
            Case Else : Return Teinte(TonEtat.ASurveiller)
        End Select
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Teinte (résultat d'une restauration)
    ' Version  : V1.0.0
    ' Date     : 18/10/2026
    ' Rôle     : Pastille « Résultat » de l'historique des restaurations.
    ' -------------------------------------------------------------------------------------------------
    Public Function Teinte(statut As StatutRestauration) As TeintePastille
        Return If(statut = StatutRestauration.Reussie, Teinte(TonEtat.Ok), Teinte(TonEtat.Probleme))
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Teinte (état d'une ligne de référentiel)
    ' Version  : V1.0.0
    ' Date     : 18/10/2026
    ' Rôle     : Pastille « État » des référentiels : « Système » en bleu (à savoir, sans enjeu —
    '            décision de la maquette), « Inactif » en gris. Une ligne normale n'en porte pas.
    ' -------------------------------------------------------------------------------------------------
    Public Function Teinte(etat As EtatReferentielLigne) As TeintePastille
        Return If(etat = EtatReferentielLigne.Inactif, Teinte(TonEtat.Inactif), Teinte(TonEtat.Information))
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Teinte (rattachement nature / domaine)
    ' Version  : V1.0.0
    ' Date     : 18/10/2026
    ' Rôle     : « Nature désactivée » est à surveiller (ocre) : le rattachement semble actif, mais la
    '            nature n'est pas proposée à la saisie — c'est ce que l'utilisateur doit remarquer.
    ' -------------------------------------------------------------------------------------------------
    Public Function Teinte(etat As EtatLiaisonNature) As TeintePastille
        Select Case etat
            Case EtatLiaisonNature.Proposee : Return Teinte(TonEtat.Ok)
            Case EtatLiaisonNature.NatureDesactivee : Return Teinte(TonEtat.ASurveiller)
            Case Else : Return Teinte(TonEtat.Inactif)
        End Select
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Teinte (rattachement tarif / domaine)
    ' Version  : V1.0.0
    ' Date     : 18/10/2026
    ' Rôle     : En vigueur (sauge), à venir (bleu), clôturé ou désactivé (gris).
    ' -------------------------------------------------------------------------------------------------
    Public Function Teinte(etat As EtatLiaisonPrix) As TeintePastille
        Select Case etat
            Case EtatLiaisonPrix.EnVigueur : Return Teinte(TonEtat.Ok)
            Case EtatLiaisonPrix.AVenir : Return Teinte(TonEtat.Information)
            Case Else : Return Teinte(TonEtat.Inactif)
        End Select
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : Teinte (règle de quota)
    ' Version  : V1.0.0
    ' Date     : 18/10/2026
    ' Rôle     : En vigueur (sauge), à venir (bleu), terminée ou désactivée (gris). Une période sans
    '            règle n'est pas une règle : l'écran la montre « à surveiller » (TonEtat).
    ' -------------------------------------------------------------------------------------------------
    Public Function Teinte(etat As EtatRegleQuota) As TeintePastille
        Select Case etat
            Case EtatRegleQuota.EnVigueur : Return Teinte(TonEtat.Ok)
            Case EtatRegleQuota.AVenir : Return Teinte(TonEtat.Information)
            Case Else : Return Teinte(TonEtat.Inactif)
        End Select
    End Function

End Module
