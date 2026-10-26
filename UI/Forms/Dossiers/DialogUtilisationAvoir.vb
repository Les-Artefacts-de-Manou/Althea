' -------------------------------------------------------------------------------------------------
' Formulaire  : DialogUtilisationAvoir
' Projet      : Althéa
' Version     : V2.0.2
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Quand une séance déjà encaissée cesse d'être due (report, annulation, maladie), son encaissement
' devient un avoir. Cette fenêtre propose de l'utiliser tout de suite pour régler une séance due du
' dossier, ou de le garder pour plus tard (pas de remboursement implicite).
'
' Responsabilités :
' - Présenter l'avoir né de la séance et les séances dues non soldées du dossier (la séance de
'   remplacement présélectionnée après un report)
' - Point d'entrée unique ProposerApresChangementStatut, partagé par l'onglet Séances (report) et la
'   fenêtre Séance (changement de statut)
'
' Important   :
' - La fenêtre elle-même ne modifie rien ; ProposerApresChangementStatut applique le choix par
'   GestionPaiements.UtiliserAvoirDeSeance (une transaction ; le paiement d'origine reste sur sa
'   séance, avec la mention de l'avoir utilisé).
' - Seul l'avoir de la séance concernée est proposé ; seules les séances dues du même dossier peuvent
'   être réglées ; sans séance due, l'avoir est conservé (décisions de Joëlle, 21/10/2026).
'
' Historique  :
' - V1.2.0 (15/10/2026, UI-7) : carte « Report du paiement », fond AltheaFondFenetre, listes AltheaListe.
' - V1.3.0 (17/10/2026) : la séance d'origine est nommée par son code dans l'en-tête.
' - V2.0.0 (21/10/2026, audit B.2, décision de Joëlle) : ex-DialogPaiementReaffectation. L'avoir est
'   utilisé au lieu de réaffecter l'encaissement ; plus de choix du paiement ; logique d'appel, qui
'   était écrite deux fois (UC_DossierFiche, SeanceEditor), réunie dans ProposerApresChangementStatut.
' - V2.0.1 (22/10/2026, audit B.5) : ProposerApresChangementStatut reçoit l'identifiant de l'utilisateur
'   (il recevait son nom affiché, recopié dans le commentaire du paiement d'origine).
' - V2.0.2 (22/10/2026, audit C.1) : ProposerApresChangementStatut reçoit le ContexteOperation.
'
' Dépendances :
' - GestionSeances (séances du dossier, avoir et reste dû : SeanceListeItem)
' - GestionPaiements (UtiliserAvoirDeSeance)
' - DialogChoix (messages), GestionLog (incidents)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Linq
Imports System.Windows.Forms

Partial Public Class DialogUtilisationAvoir

#Region "Variables privées"

    ' Séances que l'avoir peut régler (dues, non soldées, même dossier, hors séance d'origine).
    Private ReadOnly _seancesCandidates As List(Of SeanceListeItem)

    ' Identifiant de la séance à présélectionner dans la liste (0 = aucune).
    Private ReadOnly _idSeancePreselection As Long

#End Region

#Region "Propriétés publiques"

    ' Séance à régler choisie (Nothing si l'utilisateur garde l'avoir).
    Public ReadOnly Property SeanceCibleSelectionnee As SeanceListeItem
        Get
            Return TryCast(cboSeances.SelectedItem, SeanceListeItem)
        End Get
    End Property

#End Region

#Region "Constructeur"

    ' -------------------------------------------------------------------------------------------------
    ' Constructeur : New
    ' Version       : V2.0.0
    ' Date          : 21/10/2026
    '
    ' Rôle          :
    ' Initialise la fenêtre avec l'avoir de la séance et les séances qu'il peut régler.
    '
    ' Paramètres    :
    ' - montantAvoir         : Avoir né de la séance (> 0)
    ' - seancesCandidates    : Séances dues non soldées du dossier (non vide)
    ' - libelleStatut        : Libellé du statut appliqué, affiché dans le message
    ' - idSeancePreselection : Séance à présélectionner (0 = la première)
    ' - codeSeanceOrigine    : Code de la séance d'où vient l'avoir
    ' -------------------------------------------------------------------------------------------------
    Public Sub New(montantAvoir As Decimal,
                   seancesCandidates As List(Of SeanceListeItem),
                   libelleStatut As String,
                   Optional idSeancePreselection As Long = 0,
                   Optional codeSeanceOrigine As String = "")

        _seancesCandidates = If(seancesCandidates, New List(Of SeanceListeItem))
        _idSeancePreselection = idSeancePreselection

        InitializeComponent()

        ' Fond du kit : beige clair et petite feuille dessinée.
        AltheaFondFenetre.Appliquer(Me)

        If Not String.IsNullOrWhiteSpace(codeSeanceOrigine) Then
            enTete.SousTitre = $"Avoir de la séance {codeSeanceOrigine}"
        End If

        AppliquerContenu(montantAvoir, libelleStatut, codeSeanceOrigine)

    End Sub

#End Region

#Region "Construction de l'interface"

    ' Message, montant et liste des séances (valeurs dynamiques ; la disposition est dans le Designer).
    Private Sub AppliquerContenu(montantAvoir As Decimal, libelleStatut As String, codeSeanceOrigine As String)

        lblMessage.Text =
            $"Le statut « {libelleStatut} » ne justifie plus de paiement : l'encaissement de cette séance devient un avoir." &
            Environment.NewLine &
            "Vous pouvez l'utiliser dès maintenant pour régler une séance du dossier, ou le garder pour plus tard."

        lblMontantAvoir.Text = If(String.IsNullOrWhiteSpace(codeSeanceOrigine),
                                  $"{montantAvoir:N2} €",
                                  $"{montantAvoir:N2} € (séance {codeSeanceOrigine})")

        cboSeances.DataSource = _seancesCandidates

        If _idSeancePreselection > 0 Then
            Dim cible = _seancesCandidates.FirstOrDefault(Function(s) s.IdSeance = _idSeancePreselection)
            If cible IsNot Nothing Then
                cboSeances.SelectedItem = cible
            End If
        End If

    End Sub

#End Region

#Region "Point d'entrée"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ProposerApresChangementStatut
    ' Version    : V1.2.0
    ' Date       : 21/10/2026
    '
    ' Rôle       :
    ' À appeler une fois enregistré le statut non dû d'une séance (report, annulation…) : si elle porte
    ' un avoir, propose de l'utiliser pour une séance due du dossier, l'applique et dit ce qui a été
    ' réglé et ce qui reste en avoir.
    '
    ' Paramètres :
    ' - owner                : Fenêtre propriétaire des boîtes de dialogue
    ' - idDossier            : Dossier de la séance
    ' - idSeanceOrigine      : Séance devenue non due
    ' - libelleStatut        : Libellé du statut appliqué (message)
    ' - idSeancePreselection : Séance à présélectionner (séance de remplacement d'un report ; 0 = aucune)
    ' - contexte             : Qui fait l'opération (identifiant enregistré avec l'utilisation de l'avoir)
    '
    ' Remarques  :
    ' - Le changement de statut est déjà enregistré : un incident ici ne l'annule pas ; l'avoir reste
    '   alors disponible (proposé au prochain encaissement).
    ' - V1.1.0 (22/10/2026, audit B.5) : idUtilisateur au lieu du nom affiché (auteur).
    ' - V1.2.0 (22/10/2026, audit C.1) : ContexteOperation au lieu de idUtilisateur.
    ' -------------------------------------------------------------------------------------------------
    Public Shared Sub ProposerApresChangementStatut(owner As IWin32Window,
                                                    idDossier As Long,
                                                    idSeanceOrigine As Long,
                                                    libelleStatut As String,
                                                    idSeancePreselection As Long,
                                                    contexte As ContexteOperation)

        Try

            Dim seances As List(Of SeanceListeItem) = GestionSeances.GetSeancesParDossier(idDossier)
            Dim origine As SeanceListeItem = seances.FirstOrDefault(Function(s) s.IdSeance = idSeanceOrigine)

            If origine Is Nothing OrElse origine.AvoirSeance <= 0D Then
                Return
            End If

            Dim avoir As Decimal = origine.AvoirSeance

            Dim candidates As List(Of SeanceListeItem) = seances.
                Where(Function(s) s.IdSeance <> idSeanceOrigine AndAlso s.EstDue).
                OrderBy(Function(s) s.DateSeance).
                ToList()

            If candidates.Count = 0 Then
                DialogChoix.Information(
                    $"L'encaissement de la séance {origine.CodeSeance} ({avoir:N2} €) est conservé en avoir." & Environment.NewLine &
                    "Aucune séance du dossier n'est à régler ; l'avoir sera proposé au prochain encaissement d'une séance du patient.",
                    "Montant conservé en avoir")
                Return
            End If

            Dim cible As SeanceListeItem

            Using dlg As New DialogUtilisationAvoir(avoir, candidates, libelleStatut, idSeancePreselection, origine.CodeSeance)

                If dlg.ShowDialog(owner) <> DialogResult.OK Then
                    Return
                End If

                cible = dlg.SeanceCibleSelectionnee

            End Using

            If cible Is Nothing Then
                Return
            End If

            Dim impute As Decimal = GestionPaiements.UtiliserAvoirDeSeance(idSeanceOrigine, cible.IdSeance, cible.ResteDu, contexte)

            If impute <= 0D Then
                DialogChoix.Information(
                    "Aucun avoir n'a pu être utilisé : il a peut-être déjà servi entre-temps. Rechargez le dossier pour voir l'état actuel.",
                    "Avoir non utilisé")
                Return
            End If

            Dim resteCible As Decimal = Math.Max(0D, cible.ResteDu - impute)
            Dim resteAvoir As Decimal = Math.Max(0D, avoir - impute)

            Dim message As String =
                $"Avoir de {impute:N2} € utilisé pour la séance {cible.CodeSeance}" &
                If(resteCible <= 0D, " : elle est réglée.", $" : il lui reste {resteCible:N2} € à régler.")

            If resteAvoir > 0D Then
                message &= Environment.NewLine & $"{resteAvoir:N2} € restent en avoir sur la séance {origine.CodeSeance}."
            End If

            DialogChoix.Succes(message, "Avoir utilisé")

        Catch exRefus As InvalidOperationException

            ' Refus métier (ex. mode de paiement « Avoir » absent) : message rédigé pour l'utilisatrice.
            DialogChoix.Information(exRefus.Message, "Utilisation de l'avoir impossible")

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur ProposerApresChangementStatut (DialogUtilisationAvoir, séance ID={idSeanceOrigine}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

            DialogChoix.Erreur(
                "L'avoir n'a pas pu être utilisé ; rien n'a été modifié de ce côté. Le changement de statut, lui, est bien enregistré." &
                Environment.NewLine & "L'avoir reste disponible : il sera proposé au prochain encaissement.",
                "Utilisation de l'avoir impossible")

        End Try

    End Sub

#End Region

End Class
