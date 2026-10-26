' -------------------------------------------------------------------------------------------------
' Classe      : ControleClotureDossier
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 17/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Résultat du contrôle fait avant de clôturer un dossier (GestionDossiers.ControlerCloture) :
' séances encore à venir (bloquantes) et séances passées restant à régler (avertissement).
'
' Remarques   :
' - Décision de Joëlle du 17/10/2026 : on ne clôture pas un suivi qui a encore des rendez-vous à
'   venir — il faut d'abord les annuler (un report recréerait une séance à venir dans le même
'   dossier). Un montant à régler est seulement signalé : la décision reste à l'utilisatrice (une
'   dette peut ne jamais être payée — règle « décision utilisateur », ADR-26).
' - L'archivage ne se fait que depuis un dossier clôturé, dont les séances sont figées : le contrôle
'   de la clôture couvre donc aussi l'archivage.
' - Objet de lecture, sans accès base : il ne fait que porter le résultat et formuler les messages,
'   pour que l'écran et le contrôle de ChangerStatutDossier disent la même chose.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class ControleClotureDossier

    ' Séances dues encore à venir (planifiées, payées d'avance ou non) : elles empêchent la clôture.
    Public Property SeancesAVenir As New List(Of SeanceListeItem)

    ' Séances dues et passées qui ne sont pas entièrement réglées : signalées avant la clôture.
    Public Property SeancesARegler As New List(Of SeanceListeItem)

    ' Total restant à régler sur ces séances.
    Public ReadOnly Property MontantARegler As Decimal
        Get
            Return SeancesARegler.Sum(Function(s) s.MontantExigible)
        End Get
    End Property

    ' Vrai si la clôture est impossible (séances à venir).
    Public ReadOnly Property EstBloquant As Boolean
        Get
            Return SeancesAVenir.Count > 0
        End Get
    End Property

    ' Message affiché quand la clôture est refusée (séances à venir).
    Public ReadOnly Property MessageBlocage As String
        Get
            Dim libelle As String = If(SeancesAVenir.Count > 1,
                                       $"{SeancesAVenir.Count} séances sont encore prévues",
                                       "Une séance est encore prévue")
            Return $"Ce dossier ne peut pas être clôturé : {libelle} ({ListerSeances(SeancesAVenir)})." &
                   Environment.NewLine & Environment.NewLine &
                   "Annulez d'abord ces séances, puis clôturez le dossier. Un paiement déjà reçu pour " &
                   "l'une d'elles devient un avoir du patient, utilisable dans un autre dossier." &
                   Environment.NewLine & "La mise en pause reste possible à tout moment."
        End Get
    End Property

    ' Encadré d'avertissement de la confirmation quand des séances passées restent à régler : la
    ' clôture reste possible, mais l'utilisatrice la décide en connaissance de cause.
    Public ReadOnly Property AvertissementMontants As String
        Get
            Return $"Il reste {SeanceListeItem.MontantCourt(MontantARegler)} à régler sur ce dossier " &
                   $"({ListerSeances(SeancesARegler)}). Une fois le dossier clôturé, ces montants ne " &
                   "pourront plus y être encaissés sans le rouvrir."
        End Get
    End Property

    ' Nombre de séances citées dans un message : au-delà, on résume (« … et 4 autres »).
    Private Const NB_SEANCES_CITEES As Integer = 6

    ' Liste lisible « SE000074 du 07/11/2026, SE000075 du 14/11/2026 » pour les messages.
    Public Shared Function ListerSeances(seances As IEnumerable(Of SeanceListeItem)) As String
        Dim liste As List(Of SeanceListeItem) = seances.OrderBy(Function(s) s.DateSeance).ToList()
        Dim texte As String = String.Join(", ", liste.Take(NB_SEANCES_CITEES).
                                                Select(Function(s) $"{s.CodeSeance} du {s.DateSeance:dd/MM/yyyy}"))
        Dim reste As Integer = liste.Count - NB_SEANCES_CITEES
        If reste > 0 Then
            texte &= $" et {reste} autre{If(reste > 1, "s", "")}"
        End If
        Return texte
    End Function

End Class
