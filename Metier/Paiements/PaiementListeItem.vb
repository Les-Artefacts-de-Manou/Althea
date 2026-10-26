' -------------------------------------------------------------------------------------------------
' Classe      : PaiementListeItem
' Projet      : Althéa
' Version     : V1.9.1
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Modèle de présentation léger pour la liste des paiements d'un dossier (onglet Paiements
' de UC_DossierFiche).
'
' Responsabilités :
' - Porter les données minimales d'un paiement pour l'affichage en grille
' - Exposer les libellés joints (séance affectée, mode de paiement) pour un affichage direct
'
' Remarques   :
' - Cette classe ne contient aucun accès base de données.
' - Elle est produite par GestionPaiements à partir de la requête SelectPaiementsParDossier.
' - Volontairement distincte de Paiement (vue allégée pour la grille).
' - En V1, un paiement est affecté à une séance unique (D-PAY-1) : le montant affecté est
'   affiché en regard de la séance concernée.
' - V1.1.0 (17/10/2026) : grille « relevé chronologique » — MouvementCourt, PourAffichage,
'   FamilleEtat / EtatAffichage / MentionEtat (pastille « État »), EstARegler, DateMouvement ;
'   EstAvoir couvre aussi la séance reportée ; retrait des lignes d'en-tête de groupe,
'   d'IndicateurEncaisse, de SeanceAffichage et de ResteDuAffichage (colonnes supprimées).
' - V1.2.0 (17/10/2026) : une ligne vaut ce qu'elle porte sur sa séance (MontantAffecte) — un
'   paiement réparti par une réaffectation apparaît sur plusieurs lignes, un avoir en partie utilisé
'   ne vaut plus que son reste ; MontantLigne, EstAvoirUtilise, MentionsAvoirUtilise.
' - V1.3.0 (17/10/2026) : AvoirSeance (trop-perçu d'une séance encore due, renseigné par l'écran),
'   mention « avoir 10 € » à côté de « Soldée ».
' - V1.4.0 (17/10/2026) : EstAttenteClose (dû en attente clos automatiquement : séance réglée, plus
'   due ou reportée) — masqué du relevé ; libellés des lignes annulées simplifiés en conséquence.
' - V1.4.1 (20/10/2026) : codes des statuts de séance lus dans GestionSeances.CODE_STATUT_* (une seule définition ; audit B9).
' - V1.5.0 (21/10/2026, audit B.5) : IdModePaiement et EstReglementParAvoir (mode marqué est_avoir,
'   lu par jointure) ; ModePaiement reste le libellé actuel du mode, pour l'affichage.
' - V1.6.0 (22/10/2026, audit B.5, migration v2.51.0) : DateAnnulation, MotifAnnulation et
'   AuteurAnnulation (colonnes de l'annulation) ; MentionAnnulation et MentionEtat les lisent, et ne
'   relisent le commentaire que pour les annulations d'avant le 22/10/2026 (MotifAnnulationAffiche).
' - V1.7.0 (22/10/2026, audit B.5, migration v2.52.0) : UtilisationsAvoirSource et OriginesAvoir
'   (lia_paiements_avoirs) ; MentionsAvoirUtilise et MentionEtat les lisent, le commentaire n'est relu
'   que pour les utilisations d'avant ; MentionOrigineAvoir (d'où vient un règlement par avoir).
' - V1.8.0 (22/10/2026, demande de Joëlle) : pastille « État » d'un encaissement partiel avec les mots
'   de la grille des séances (« Reste · 20 € », « Prévue · reste 20 € »).
' - V1.9.0 (23/10/2026, recherche transversale R3) : dossier et patient de la ligne (IdDossier,
'   CodeDossier, IdPatient, CodePatient, NomPatient, PrenomPatient, PatientAffichage) ; InfobulleLigne
'   (sortie de UC_DossierFiche) ; SeancePassee et AvoirSeance aussi calculés par la recherche.
' - V1.9.1 (24/10/2026, audit C.3) : statuts de paiement et de séance lus dans CodesSysteme.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class PaiementListeItem

#Region "Propriétés"

    ' Identifiant unique du paiement (clé primaire)
    Public Property IdPaiement As Long

    ' Code technique du paiement (ex. 'PAI000012')
    Public Property CodePaiement As String

    ' Date du paiement (NULL tant que le paiement est en attente / non encaissé)
    Public Property DatePaiement As Date?

    ' Montant total du paiement
    Public Property Montant As Decimal

    ' Mode de paiement : identifiant dans ref_modes_paiement (Nothing pour un dû en attente)
    Public Property IdModePaiement As ULong?

    ' Libellé actuel du mode de paiement (ex. « Espèces », « Virement », « INAMI »), pour l'affichage
    Public Property ModePaiement As String

    ' Règlement par avoir : le mode est marqué est_avoir dans le référentiel (actif ou non). Un tel
    ' règlement ne s'annule pas : l'avoir d'origine, déjà diminué, serait perdu.
    Public Property EstReglementParAvoir As Boolean

    ' Référence externe du paiement (optionnelle)
    Public Property ReferencePaiement As String

    ' Identifiant de la séance affectée par ce paiement (via lia_paiements_seances)
    Public Property IdSeance As Long

    ' Code technique de la séance affectée (ex. 'SE000012')
    Public Property CodeSeance As String

    ' Date de la séance affectée (pour repérage rapide en grille)
    Public Property DateSeance As Date

    ' Tarif de la séance affectée (base de calcul du reste dû)
    Public Property TarifSeance As Decimal

    ' Libellé de la nature clinique de la séance (ref_natures_seance)
    Public Property NatureSeance As String

    ' Solde restant à encaisser sur la séance, tous paiements confondus
    Public Property ResteDuSeance As Decimal

    ' Indique si le statut de la séance la rend due (ref_statuts_seance.est_du).
    Public Property SeanceEstDue As Boolean

    ' Montant affecté à la séance de cette ligne (lia_paiements_seances.montant_affecte). Égal au
    ' montant du paiement dans le cas courant ; plus petit quand une réaffectation a réparti le
    ' paiement sur plusieurs séances (une ligne par séance) ou qu'un avoir a été en partie utilisé ;
    ' 0 pour un paiement annulé ou un avoir entièrement utilisé.
    Public Property MontantAffecte As Decimal

    ' Statut du paiement (PAYE, EN_ATTENTE, ...)
    Public Property StatutPaiement As String

    ' Code du statut de la séance rattachée (ex. 'PLANIFIEE', 'REPORTEE', 'REALISEE')
    Public Property CodeStatutSeance As String

    ' Code de la séance d'origine, si la séance rattachée est elle-même issue d'un report (sinon vide)
    Public Property CodeSeanceOrigine As String

    ' Code de la séance de remplacement, si la séance rattachée a été reportée sur une autre (sinon vide)
    Public Property CodeSeanceReport As String

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SeanceReportee
    ' Rôle       : Indique que la séance rattachée à ce paiement a été reportée (statut REPORTEE).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SeanceReportee As Boolean
        Get
            Return String.Equals(CodeStatutSeance, CodesSysteme.StatutsSeance.Reportee, StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SeanceIssueDeReport
    ' Rôle       : Indique que la séance rattachée est elle-même le résultat d'un report.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SeanceIssueDeReport As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(CodeSeanceOrigine)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstAnnule
    ' Version    : V1.0.0
    ' Date       : 17/09/2026
    '
    ' Rôle       :
    ' Indique que le paiement a été neutralisé (statut ANNULE). La ligne est conservée pour la
    ' traçabilité, mais son montant ne doit plus compter dans les totaux.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstAnnule As Boolean
        Get
            Return String.Equals(StatutPaiement, CodesSysteme.StatutsPaiement.Annule, StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstEncaisse
    ' Version    : V1.0.0
    ' Date       : 17/09/2026
    '
    ' Rôle       :
    ' Indique que le paiement a réellement été encaissé (statut PAYE).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstEncaisse As Boolean
        Get
            Return String.Equals(StatutPaiement, CodesSysteme.StatutsPaiement.Paye, StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : LibelleStatutPaiement
    ' Version    : V1.1.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Libellé lisible du statut du paiement pour l'affichage en grille (détail des paiements).
    '
    ' Remarques  :
    ' - V1.1.0 : « Clos » pour un dû en attente clos par l'application (EstAttenteClose), qui n'est
    '   pas une annulation.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property LibelleStatutPaiement As String
        Get
            If EstAttenteClose Then
                Return "Clos"
            End If
            If EstAnnule Then
                Return "Annulé"
            End If
            If EstEncaisse Then
                Return "Encaissé"
            End If
            Return "En attente"
        End Get
    End Property

    ' Date de dernière modification du paiement
    Public Property DateModification As DateTime

    ' Commentaire du paiement (les annulations d'avant le 22/10/2026 y ont leur mention
    ' « [Annulé le … par … : motif] »)
    Public Property Commentaire As String

    ' Annulation d'un encaissement (colonnes, migration v2.51.0) : date et heure, motif saisi et nom
    ' affiché de l'utilisateur (lu par jointure, jamais recopié). Vides pour une annulation d'avant.
    Public Property DateAnnulation As Date?
    Public Property MotifAnnulation As String
    Public Property AuteurAnnulation As String

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : MentionAnnulation
    ' Version    : V2.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Phrase de l'annulation (« Annulé le jj/mm/aaaa par X : motif »), ou vide.
    '
    ' Remarques  :
    ' - V2.0.0 (22/10/2026, audit B.5) : composée depuis les colonnes de l'annulation ; la mention
    '   écrite dans le commentaire n'est relue que pour une annulation d'avant (colonnes vides).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property MentionAnnulation As String
        Get
            If DateAnnulation.HasValue Then
                Dim parAuteur As String = If(String.IsNullOrWhiteSpace(AuteurAnnulation), String.Empty, $" par {AuteurAnnulation.Trim()}")
                Return $"Annulé le {DateAnnulation.Value:dd/MM/yyyy}{parAuteur} : {MotifAnnulationAffiche}"
            End If
            Return MentionAnnulationDuCommentaire
        End Get
    End Property

    ' Motif de l'annulation : colonne, sinon la fin de la mention d'une annulation d'avant le 22/10/2026.
    Public ReadOnly Property MotifAnnulationAffiche As String
        Get
            If DateAnnulation.HasValue Then
                Return If(MotifAnnulation, String.Empty).Trim()
            End If
            Dim mention As String = MentionAnnulationDuCommentaire
            Dim separateur As Integer = mention.IndexOf(" : ", StringComparison.Ordinal)
            Return If(separateur < 0, String.Empty, mention.Substring(separateur + 3).Trim())
        End Get
    End Property

    ' Annulations d'avant le 22/10/2026 : dernière mention tracée dans le commentaire (sans crochets).
    Private ReadOnly Property MentionAnnulationDuCommentaire As String
        Get
            If String.IsNullOrWhiteSpace(Commentaire) Then Return String.Empty
            Dim debut As Integer = Commentaire.LastIndexOf("[Annulé le ", StringComparison.Ordinal)
            If debut < 0 Then Return String.Empty
            Dim fin As Integer = Commentaire.IndexOf("]"c, debut)
            If fin < 0 Then fin = Commentaire.Length
            Return Commentaire.Substring(debut + 1, fin - debut - 1).Trim()
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstAvoir
    ' Version    : V1.2.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Indique que ce paiement encaissé constitue un avoir : il reste rattaché à une séance qui n'est
    ' plus due (annulée, maladie, reportée) et n'a pas été réaffecté à une autre séance. Le montant
    ' est donc un crédit disponible pour le dossier, pas une créance ni un règlement de séance.
    '
    ' Remarques  :
    ' - V1.1.0 : la séance reportée n'est plus exclue. Un encaissement que l'on choisit de ne pas
    '   réaffecter au report est « conservé en avoir » (message de ProposerReaffectationPaiement) et
    '   compte déjà dans l'avoir du dossier (SeanceListeItem.AvoirSeance) : la grille des paiements
    '   le montrait pourtant comme « séance soldée », hors du filtre Avoirs.
    ' - V1.2.0 : seul ce qui reste affecté compte (un avoir entièrement utilisé n'en est plus un, voir
    '   EstAvoirUtilise).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstAvoir As Boolean
        Get
            Return EstEncaisse AndAlso Not SeanceEstDue AndAlso MontantAffecte > 0D
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstAvoirUtilise
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Indique un encaissement sur séance non due dont l'avoir a été entièrement utilisé pour une autre
    ' séance (GestionPaiements.UtiliserAvoirPourSeance ramène son affectation à 0) : la ligne reste
    ' pour la trace, elle ne vaut plus rien ici.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstAvoirUtilise As Boolean
        Get
            Return EstEncaisse AndAlso Not SeanceEstDue AndAlso MontantAffecte <= 0D
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : MontantLigne
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Montant affiché sur la ligne : ce que le paiement porte sur cette séance (MontantAffecte), pour
    ' qu'un paiement réparti ne soit pas compté deux fois. Quand l'affectation a été ramenée à 0
    ' (paiement annulé, avoir entièrement utilisé), le montant d'origine du paiement, affiché grisé
    ' (et barré s'il est annulé), garde la trace de ce qui s'est passé.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property MontantLigne As Decimal
        Get
            Return If(EstAnnule OrElse EstAvoirUtilise, Montant, MontantAffecte)
        End Get
    End Property

    ' Utilisations de l'avoir de cette ligne (encaissement source sur cette séance), et origines d'un
    ' règlement par avoir — lues dans lia_paiements_avoirs (v2.52.0) et rattachées par GestionPaiements.
    ' Vides pour les utilisations d'avant le 22/10/2026, tracées dans le commentaire.
    Public Property UtilisationsAvoirSource As New List(Of UtilisationAvoir)
    Public Property OriginesAvoir As New List(Of UtilisationAvoir)

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : MentionsAvoirUtilise
    ' Version    : V2.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Utilisations de l'avoir de cette ligne (« Avoir 45,00 € utilisé le … par … pour S000050 »), de la
    ' plus ancienne à la plus récente ; liste vide sinon.
    '
    ' Remarques  :
    ' - V2.0.0 (22/10/2026, audit B.5) : les mentions écrites dans le commentaire (utilisations d'avant
    '   le 22/10/2026) d'abord, puis les utilisations enregistrées dans lia_paiements_avoirs.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property MentionsAvoirUtilise As List(Of String)
        Get
            Dim mentions As List(Of String) = MentionsAvoirDuCommentaire()
            mentions.AddRange(UtilisationsAvoirSource.Select(Function(u) u.MentionSource))
            Return mentions
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : MentionOrigineAvoir
    ' Version    : V1.0.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Pour un règlement par avoir, d'où vient l'argent : « Avoir utilisé : PAI000012 (S000045) 20,00 € ».
    ' Vide pour un autre paiement.
    '
    ' Remarques  :
    ' - Avant le 22/10/2026, cette phrase était le commentaire du règlement (écrit par Althéa, jamais
    '   saisi) : elle y est relue quand la liaison n'existe pas.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property MentionOrigineAvoir As String
        Get
            If OriginesAvoir.Count > 0 Then
                Return "Avoir utilisé : " & String.Join(", ", OriginesAvoir.Select(Function(u) u.LibelleOrigine))
            End If
            If EstReglementParAvoir AndAlso Not String.IsNullOrWhiteSpace(Commentaire) AndAlso
               Commentaire.TrimStart().StartsWith("Avoir utilisé :", StringComparison.Ordinal) Then
                Return Commentaire.Trim()
            End If
            Return String.Empty
        End Get
    End Property

    ' Utilisations d'avant le 22/10/2026 : mentions « [Avoir … utilisé le … pour S…] » du commentaire.
    Private Function MentionsAvoirDuCommentaire() As List(Of String)
        Dim mentions As New List(Of String)
        If String.IsNullOrWhiteSpace(Commentaire) Then Return mentions
        Dim debut As Integer = Commentaire.IndexOf("[Avoir ", StringComparison.Ordinal)
        While debut >= 0
            Dim fin As Integer = Commentaire.IndexOf("]"c, debut)
            If fin < 0 Then fin = Commentaire.Length
            mentions.Add(Commentaire.Substring(debut + 1, fin - debut - 1).Trim())
            debut = Commentaire.IndexOf("[Avoir ", fin, StringComparison.Ordinal)
        End While
        Return mentions
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : MouvementAffichage
    ' Version    : V1.2.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Phrase explicative, en langage courant, du mouvement représenté par la ligne de paiement. Une
    ' ligne isolée (montant + statut + reste dû) reste difficile à interpréter : ce libellé résume
    ' d'un coup d'œil ce qui s'est passé pour la séance concernée.
    '
    ' Retour     :
    ' - String : ex. « Encaissement de 55,00 € — séance soldée », « Acompte de 30,00 € — reste 25,00 € »,
    '            « Encaissement prévu de 55,00 € », « Paiement annulé ».
    '
    ' Remarques  :
    ' - V1.1.0 : un encaissement est décrit par ce qu'il porte sur cette séance (MontantAffecte) ;
    '   avoir en partie ou entièrement utilisé.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property MouvementAffichage As String
        Get
            Dim reference As String = If(String.IsNullOrWhiteSpace(CodeSeance), String.Empty, $" ({CodeSeance})")

            If EstAttenteClose Then
                ' Dû en attente clos par l'application : dire pourquoi, ce n'est pas une annulation.
                If SeanceReportee Then
                    If Not String.IsNullOrWhiteSpace(CodeSeanceReport) Then
                        Return $"Séance reportée — paiement attendu reporté sur {CodeSeanceReport}{reference}"
                    End If
                    Return $"Séance reportée — paiement attendu reporté{reference}"
                End If
                Return If(SeanceEstDue,
                          $"Paiement attendu clos — séance réglée{reference}",
                          $"Paiement attendu clos — séance non due{reference}")
            End If

            If EstAnnule Then
                Dim mention As String = MentionAnnulation
                If Not String.IsNullOrEmpty(mention) Then
                    Return $"Encaissement de {Montant:N2} € annulé — {mention}{reference}"
                End If

                Return $"Encaissement annulé{reference}"
            End If

            ' Cas d'un montant reporté depuis une séance d'origine vers cette séance de remplacement.
            Dim origine As String = If(SeanceIssueDeReport, $" — montant reporté de la séance {CodeSeanceOrigine}", String.Empty)

            If Not EstEncaisse Then
                Return $"Encaissement prévu de {Montant:N2} €{origine}{reference}"
            End If

            ' Cas d'un encaissement subsistant sur une séance qui n'est plus due (annulée, maladie…) :
            ' le montant n'a pas été réaffecté et reste disponible en avoir sur le dossier.
            If EstAvoirUtilise Then
                Return $"Encaissement de {Montant:N2} € — avoir entièrement utilisé{reference}"
            End If

            If EstAvoir Then
                If MontantAffecte < Montant Then
                    Return $"Encaissement de {Montant:N2} € — {MontantAffecte:N2} € encore en avoir{reference}"
                End If
                Return $"Encaissement de {Montant:N2} € — conservé en avoir{reference}"
            End If

            If SeanceSoldee Then
                Return $"Encaissement de {MontantAffecte:N2} € — séance soldée{origine}{reference}"
            End If

            Return $"Acompte de {MontantAffecte:N2} € — reste {ResteDuSeance:N2} € dû{origine}{reference}"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SeanceSoldee
    ' Version    : V1.0.0
    ' Date       : 20/09/2026
    '
    ' Rôle       :
    ' Indique que la séance rattachée est intégralement réglée (plus rien à encaisser).
    '
    ' Remarques  :
    ' - Distinct de EstEncaisse : un paiement peut être encaissé sans solder la séance lorsque le
    '   patient règle en plusieurs fois.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SeanceSoldee As Boolean
        Get
            Return ResteDuSeance <= 0D
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SeancePassee
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Indique que la séance rattachée est passée. Renseignée par l'écran à partir de la séance du
    ' dossier (SeanceListeItem.EstPassee) : même règle, heures comprises, que la grille des séances.
    '
    ' Remarques  :
    ' - Recherche des paiements (23/10/2026) : calculée par GestionPaiements.GetPaiementsParPeriode,
    '   même règle (SeanceListeItem.EstMomentPasse).
    ' - Ailleurs (détail des paiements d'une séance), elle reste à False.
    ' -------------------------------------------------------------------------------------------------
    Public Property SeancePassee As Boolean

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : AvoirSeance
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Avoir porté par la séance rattachée (SeanceListeItem.AvoirSeance), renseigné par l'écran comme
    ' SeancePassee (la recherche des paiements la lit dans la vue v_seances_solde). Sert au trop-perçu d'une séance encore due (montant baissé sous l'encaissé), que
    ' la ligne seule ne permet pas de calculer : le reste dû de la requête est borné à 0.
    ' -------------------------------------------------------------------------------------------------
    Public Property AvoirSeance As Decimal

    ' Encaissement d'une séance due qui porte un trop-perçu (filtre Avoirs, mention « avoir 10 € »).
    Public ReadOnly Property EstSurSeanceAvecTropPercu As Boolean
        Get
            Return EstEncaisse AndAlso SeanceEstDue AndAlso AvoirSeance > 0D
        End Get
    End Property

    ' Date du mouvement : date d'encaissement, ou date de la séance pour un paiement encore en attente
    ' (sans date). Clé de l'ordre chronologique du relevé.
    Public ReadOnly Property DateMouvement As Date
        Get
            Return If(DatePaiement, DateSeance).Date
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstARegler
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Indique que la séance de ce paiement (non annulé) est due, passée et pas entièrement réglée :
    ' paiement attendu en retard, ou encaissement partiel laissant un reste dû.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstARegler As Boolean
        Get
            Return Not EstAnnule AndAlso SeanceEstDue AndAlso SeancePassee AndAlso Not SeanceSoldee
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : MouvementCourt
    ' Version    : V1.1.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Nature du mouvement en deux ou trois mots (colonne « Mouvement » du relevé) : ce qui s'est passé,
    ' pas ce que c'est devenu (la pastille « État » s'en charge). La phrase complète reste disponible
    ' dans MouvementAffichage (infobulle, détail des paiements d'une séance).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property MouvementCourt As String
        Get
            If EstAttenteClose Then Return "Paiement attendu clos"
            If EstAnnule Then Return "Encaissement annulé"
            If Not EstEncaisse Then Return "Encaissement prévu"
            If DatePaiement.HasValue AndAlso DatePaiement.Value.Date < DateSeance.Date Then Return "Paiement d'avance"
            If SeanceEstDue AndAlso MontantAffecte < TarifSeance Then Return "Paiement partiel"
            Return "Encaissement"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : PourAffichage
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Séance concernée, sur une ligne : « S000043 · ven. 10/10/2026 · Traitement », suivie de
    ' l'origine pour une séance issue d'un report (« report de S000040 »).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property PourAffichage As String
        Get
            Dim morceaux As New List(Of String)
            If Not String.IsNullOrWhiteSpace(CodeSeance) Then morceaux.Add(CodeSeance)
            morceaux.Add($"{DateSeance:ddd} {DateSeance.ToShortDateString()}")
            If Not String.IsNullOrWhiteSpace(NatureSeance) Then morceaux.Add(NatureSeance)
            If SeanceIssueDeReport Then morceaux.Add($"report de {CodeSeanceOrigine}")
            Return String.Join(" · ", morceaux)
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : FamilleEtat
    ' Version    : V1.1.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Famille de la pastille « État » : ce que le paiement est devenu. Mêmes familles, donc mêmes
    ' couleurs (UtilsPastilles), que la pastille « Paiement » de la grille des séances :
    ' - Reglee  : encaissé, séance soldée (vert) ;
    ' - ARegler : séance passée pas entièrement réglée — attendu en retard ou reste dû (rose) ;
    ' - Prevue  : en attente, ou acompte pour une séance à venir (bleu) ;
    ' - Avoir   : encaissé sur une séance qui n'est plus due (mauve) ;
    ' - NonDue  : paiement annulé ou reporté, avoir entièrement utilisé — gardé pour la trace (gris).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property FamilleEtat As FamillePaiementSeance
        Get
            If EstNeutralise Then Return FamillePaiementSeance.NonDue
            If EstAvoir Then Return FamillePaiementSeance.Avoir
            If EstARegler Then Return FamillePaiementSeance.ARegler
            If EstEncaisse AndAlso SeanceSoldee Then Return FamillePaiementSeance.Reglee
            Return FamillePaiementSeance.Prevue
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EtatAffichage
    ' Version    : V1.3.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Texte de la pastille « État » : Soldée / Payée d'avance / Reste · 20 € / À régler /
    ' Prévue · reste 20 € / En attente / Avoir · 60 € / Avoir utilisé / Annulé / Reporté.
    '
    ' Remarques  :
    ' - V1.1.0 : l'avoir affiche ce qui reste (« Avoir · 15 € » après une utilisation partielle), comme
    '   la pastille Paiement de la grille des séances ; « Avoir utilisé » quand il ne reste rien.
    ' - V1.3.0 (22/10/2026, demande de Joëlle) : mêmes mots que la pastille Paiement des séances pour
    '   un encaissement partiel — « Reste · 20 € » (séance passée, à régler) et « Prévue · reste 20 € »
    '   (séance à venir) au lieu de « Reste 20 € dû » et « Reste 20 € », qui se confondaient.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EtatAffichage As String
        Get
            Dim reste As String = SeanceListeItem.MontantCourt(ResteDuSeance)
            Select Case FamilleEtat
                Case FamillePaiementSeance.NonDue
                    If EstAvoirUtilise Then Return "Avoir utilisé"
                    Return If(EstAttenteClose, "Clos", "Annulé")
                Case FamillePaiementSeance.Avoir
                    Return $"Avoir · {SeanceListeItem.MontantCourt(MontantAffecte)}"
                Case FamillePaiementSeance.ARegler
                    Return If(EstEncaisse, $"Reste · {reste}", "À régler")
                Case FamillePaiementSeance.Reglee
                    Return If(SeancePassee, "Soldée", "Payée d'avance")
                Case Else
                    Return If(EstEncaisse, $"Prévue · reste {reste}", "En attente")
            End Select
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : MentionEtat
    ' Version    : V1.5.0
    ' Date       : 22/10/2026
    '
    ' Rôle       :
    ' Précision grise à droite de la pastille : pour une ligne neutralisée, séance de destination
    ' d'un report ou d'un avoir utilisé (« → S000045 ») ou motif de l'annulation (« doublon ») ;
    ' pour un encaissement d'une séance due avec trop-perçu, « avoir 10 € ». Vide sinon.
    ' - V1.4.0 (22/10/2026, audit B.5) : motif lu dans sa colonne (MotifAnnulationAffiche).
    ' - V1.5.0 (22/10/2026, audit B.5) : séance réglée par l'avoir lue dans la liaison (v2.52.0).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property MentionEtat As String
        Get
            If EstAvoirUtilise Then
                ' Dernière utilisation : séance réglée (liaison), sinon fin de la dernière mention du
                ' commentaire (« Avoir 45,00 € utilisé le jj/mm/aaaa par X pour S000050 »).
                If UtilisationsAvoirSource.Count > 0 Then
                    Return $"→ {UtilisationsAvoirSource(UtilisationsAvoirSource.Count - 1).CodeSeanceReglee}"
                End If
                Dim mentions As List(Of String) = MentionsAvoirDuCommentaire()
                If mentions.Count = 0 Then Return String.Empty
                Dim derniere As String = mentions(mentions.Count - 1)
                Dim pour As Integer = derniere.LastIndexOf(" pour ", StringComparison.Ordinal)
                Return If(pour < 0, String.Empty, $"→ {derniere.Substring(pour + 6).Trim()}")
            End If
            If EstSurSeanceAvecTropPercu Then Return $"avoir {SeanceListeItem.MontantCourt(AvoirSeance)}"
            If Not EstAnnule Then Return String.Empty
            If EstAttenteClose Then
                Return If(SeanceReportee AndAlso Not String.IsNullOrWhiteSpace(CodeSeanceReport), $"→ {CodeSeanceReport}", String.Empty)
            End If
            ' Seul le motif de l'annulation est repris.
            Return MotifAnnulationAffiche
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : EstAttenteClose
    ' Version    : V1.0.0
    ' Date       : 17/10/2026
    '
    ' Rôle       :
    ' Indique un dû en attente que l'application a clos d'elle-même (ResynchroniserDuSeance) : séance
    ' réglée, devenue non due ou reportée. Ce n'est pas une annulation décidée par l'utilisatrice — on
    ' n'annule jamais un paiement attendu à la main, seul un encaissement s'annule (avec date et motif).
    '
    ' Remarques  :
    ' - Masqué du relevé de l'onglet Paiements (demande de Joëlle du 17/10/2026 : une ligne « annulée »
    '   après chaque encaissement prêtait à confusion) ; visible dans le détail des paiements d'une
    '   séance, pour la trace.
    ' - Pourquoi ne pas « solder » la ligne d'attente au lieu de la clore : elle représente ce qui
    '   reste dû (60 → 40 → 0 €) et suit la séance, alors qu'un encaissement est un fait daté (date,
    '   mode, montant) qui ne se modifie jamais (ADR-26).
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property EstAttenteClose As Boolean
        Get
            Return EstAnnule AndAlso Not DatePaiement.HasValue AndAlso String.IsNullOrEmpty(MentionAnnulation)
        End Get
    End Property

    ' Ligne gardée pour la trace mais qui ne vaut plus rien sur sa séance (paiement annulé, avoir
    ' entièrement utilisé) : affichée en gris, pastille atténuée.
    Public ReadOnly Property EstNeutralise As Boolean
        Get
            Return EstAnnule OrElse EstAvoirUtilise
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : InfobulleLigne
    ' Version    : V1.0.0
    ' Date       : 23/10/2026
    '
    ' Rôle       :
    ' Infobulle d'une ligne du relevé : code du paiement, phrase complète du mouvement, montant du
    ' paiement s'il diffère de la ligne (paiement réparti, avoir utilisé), référence, mentions
    ' d'annulation et d'utilisation d'avoir, origine d'un règlement par avoir.
    '
    ' Remarques  :
    ' - Sortie de UC_DossierFiche (23/10/2026) : même infobulle dans le dossier et dans la recherche.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InfobulleLigne As String
        Get
            Dim lignes As New List(Of String) From {$"Paiement {CodePaiement}", MouvementAffichage}
            If Not EstNeutralise AndAlso MontantAffecte <> Montant Then
                lignes.Add($"Paiement de {Montant:N2} €, dont {MontantAffecte:N2} € sur cette séance")
            End If
            If Not String.IsNullOrWhiteSpace(ReferencePaiement) Then lignes.Add($"Référence : {ReferencePaiement}")
            If Not String.IsNullOrEmpty(MentionAnnulation) Then lignes.Add(MentionAnnulation)
            If Not String.IsNullOrEmpty(MentionOrigineAvoir) Then lignes.Add(MentionOrigineAvoir)
            lignes.AddRange(MentionsAvoirUtilise)
            Return String.Join(Environment.NewLine, lignes)
        End Get
    End Property

#End Region

#Region "Patient et dossier (recherche transversale)"

    ' Dossier et patient de la ligne (lus avec elle ; la recherche des paiements les affiche et y va).
    Public Property IdDossier As Long
    Public Property CodeDossier As String
    Public Property IdPatient As Long
    Public Property CodePatient As String
    Public Property NomPatient As String
    Public Property PrenomPatient As String

    ' Patient pour une liste triable par nom : « Nom Prénom » (comme la recherche des séances).
    Public ReadOnly Property PatientAffichage As String
        Get
            Return $"{NomPatient} {PrenomPatient}".Trim()
        End Get
    End Property

#End Region

End Class
