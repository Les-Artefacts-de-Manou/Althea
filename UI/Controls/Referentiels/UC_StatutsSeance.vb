' -------------------------------------------------------------------------------------------------
' UserControl : UC_StatutsSeance
' Projet      : Althéa
' Version     : V1.4.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des statuts de séance (table ref_statuts_seance).
' Hérite de UC_ReferentielBase et fournit les métadonnées d'affichage et le branchement
' de la couche métier GestionStatutsSeance via les points d'extension.
'
' Remarques   :
' - V1.1.0 (16/10/2026, UI-7) : champs posés par AjouterLigneDetail (contrôles du kit, plus de
'   couleur ni de position en dur) ; consultation lisible gérée par UC_ReferentielBase.
' - V1.2.0 (22/10/2026, audit B.6, migration v2.53.0) : case « Absence non excusée (no-show) »
'   (est_absence) : le comportement d'absence se règle ici, plus par le code ABSENTE.
' - V1.3.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
' - V1.4.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : cases « Non tenue, libère le créneau »
'   (est_non_tenue) et « Proposé à la création » (propose_creation) : ces comportements se règlent ici,
'   plus par les codes des statuts.
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Aucun accès direct à la base de données : tout passe par GestionStatutsSeance.
' - Code long (varchar 30) avec espaces convertis en _ (comportement par défaut de la base).
'
' Dépendances :
' - UC_ReferentielBase (classe de base héritée)
' - GestionStatutsSeance (couche métier)
' - StatutSeance (modèle métier)
' - ReferentielLigne (modèle de présentation générique)
' - AppRole (gestion des droits)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_StatutsSeance
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_statutsSeance
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Statuts de séance"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les statuts de séance."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Statuts de séance"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 30
        End Get
    End Property

#End Region

#Region "Champ supplémentaire - séance due"

    ' Case « Séance due » (colonne est_du) et cache des statuts chargés, ReferentielLigne ne
    ' transportant pas ce drapeau.
    Private _chkEstDu As AltheaCase
    Private _chkConsommeQuota As AltheaCase
    Private _chkEstAbsence As AltheaCase
    Private _chkNonTenue As AltheaCase
    Private _chkProposeCreation As AltheaCase
    Private _statutsParId As New Dictionary(Of ULong, StatutSeance)

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ConfigurerChampSupplementaire
    ' Rôle      : Crée les cases « Séance due », « Consomme le quota », « Absence non excusée »,
    '             « Non tenue, libère le créneau » et « Proposé à la création » dans le panneau « Détail ».
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub ConfigurerChampSupplementaire()

        _chkEstDu = CreerCaseDetail("chkEstDu", "Séance due (à payer)")
        AjouterLigneDetail("Paiement", _chkEstDu)

        _chkConsommeQuota = CreerCaseDetail("chkConsommeQuota", "Consomme le quota du domaine")
        AjouterLigneDetail("Quota", _chkConsommeQuota)

        _chkEstAbsence = CreerCaseDetail("chkEstAbsence", "Absence non excusée (no-show)")
        AjouterLigneDetail("Absence", _chkEstAbsence)

        _chkNonTenue = CreerCaseDetail("chkNonTenue", "Non tenue, libère le créneau")
        AjouterLigneDetail("Créneau", _chkNonTenue)

        _chkProposeCreation = CreerCaseDetail("chkProposeCreation", "Proposé à la création d'une séance")
        AjouterLigneDetail("Création", _chkProposeCreation)

        If _context IsNot Nothing Then
            _context.SetToolTip(_chkEstDu,
                "Une séance dans ce statut est exigible : elle entre dans le calcul du dû " &
                "et des paiements en attente.")
            _context.SetToolTip(_chkConsommeQuota,
                "Une séance dans ce statut est décomptée du quota (domaines soumis à quota)." &
                Environment.NewLine & "Ex. : planifiée, réalisée. Une séance annulée ou reportée libère le quota.")
            _context.SetToolTip(_chkEstAbsence,
                "Le patient ne s'est pas présenté sans prévenir : la séance n'est pas facturée dans un domaine " &
                "qui ne facture pas les absences, et elle compte dans les absences (grille Patients, aperçu, " &
                "filtre « Absences »).")
            _context.SetToolTip(_chkNonTenue,
                "La séance n'a pas eu lieu : son horaire redevient libre (planning du jour, contrôle de " &
                "chevauchement), et passer d'un statut non tenu à un autre ne recontrôle pas l'horaire." &
                Environment.NewLine & "Ex. : annulée, maladie, reportée.")
            _context.SetToolTip(_chkProposeCreation,
                "Ce statut peut être choisi en créant une séance (ex. : planifiée, réalisée). Une séance créée " &
                "ne peut être ni annulée ni reportée d'emblée.")
        End If

    End Sub

    Protected Overrides Sub AfficherChampSupplementaire(ligne As ReferentielLigne)

        If _chkEstDu Is Nothing Then Return

        Dim statut As StatutSeance = Nothing
        _chkEstDu.Checked = ligne IsNot Nothing AndAlso
                            _statutsParId.TryGetValue(ligne.IdReferentiel, statut) AndAlso
                            statut.EstDu

        If _chkConsommeQuota IsNot Nothing Then
            _chkConsommeQuota.Checked = statut IsNot Nothing AndAlso ligne IsNot Nothing AndAlso statut.ConsommeQuota
        End If

        If _chkEstAbsence IsNot Nothing Then
            _chkEstAbsence.Checked = statut IsNot Nothing AndAlso ligne IsNot Nothing AndAlso statut.EstAbsence
        End If

        If _chkNonTenue IsNot Nothing Then
            _chkNonTenue.Checked = statut IsNot Nothing AndAlso ligne IsNot Nothing AndAlso statut.EstNonTenue
        End If

        If _chkProposeCreation IsNot Nothing Then
            _chkProposeCreation.Checked = statut IsNot Nothing AndAlso ligne IsNot Nothing AndAlso statut.ProposeCreation
        End If

    End Sub

    ' Un nouveau statut est dû par défaut (valeur par défaut de la colonne en base).
    Protected Overrides Sub ViderChampSupplementaire()
        If _chkEstDu IsNot Nothing Then _chkEstDu.Checked = True
        If _chkConsommeQuota IsNot Nothing Then _chkConsommeQuota.Checked = False
        If _chkEstAbsence IsNot Nothing Then _chkEstAbsence.Checked = False
        If _chkNonTenue IsNot Nothing Then _chkNonTenue.Checked = False
        If _chkProposeCreation IsNot Nothing Then _chkProposeCreation.Checked = False
    End Sub

    Private Function EstDuSaisi() As Boolean
        Return _chkEstDu Is Nothing OrElse _chkEstDu.Checked
    End Function

    Private Function ConsommeQuotaSaisi() As Boolean
        Return _chkConsommeQuota IsNot Nothing AndAlso _chkConsommeQuota.Checked
    End Function

    Private Function EstAbsenceSaisi() As Boolean
        Return _chkEstAbsence IsNot Nothing AndAlso _chkEstAbsence.Checked
    End Function

    Private Function NonTenueSaisi() As Boolean
        Return _chkNonTenue IsNot Nothing AndAlso _chkNonTenue.Checked
    End Function

    Private Function ProposeCreationSaisi() As Boolean
        Return _chkProposeCreation IsNot Nothing AndAlso _chkProposeCreation.Checked
    End Function

#End Region

#Region "Données - branchement GestionStatutsSeance"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim statuts As List(Of StatutSeance) = GestionStatutsSeance.GetStatutsSeance(afficherInactifs)

        _statutsParId = statuts.ToDictionary(Function(s) s.IdStatutSeance)

        Return statuts.Select(
            Function(s) New ReferentielLigne With {
                .IdReferentiel = s.IdStatutSeance,
                .Code = s.CodeStatutSeance,
                .Libelle = s.LibelleStatutSeance,
                .OrdreAffichage = s.OrdreAffichage,
                .Actif = s.Actif,
                .EstSysteme = s.EstSysteme
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionStatutsSeance.CodeStatutSeanceExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionStatutsSeance.LibelleStatutSeanceExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim statut As New StatutSeance With {
            .CodeStatutSeance = code,
            .LibelleStatutSeance = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif,
            .EstDu = EstDuSaisi(),
            .ConsommeQuota = ConsommeQuotaSaisi(),
            .EstAbsence = EstAbsenceSaisi(),
            .EstNonTenue = NonTenueSaisi(),
            .ProposeCreation = ProposeCreationSaisi()
        }

        GestionStatutsSeance.InsertStatutSeance(_context?.ContexteOperation, statut)

    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim statut As New StatutSeance With {
            .IdStatutSeance = id,
            .CodeStatutSeance = code,
            .LibelleStatutSeance = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif,
            .EstDu = EstDuSaisi(),
            .ConsommeQuota = ConsommeQuotaSaisi(),
            .EstAbsence = EstAbsenceSaisi(),
            .EstNonTenue = NonTenueSaisi(),
            .ProposeCreation = ProposeCreationSaisi()
        }

        GestionStatutsSeance.UpdateStatutSeance(_context?.ContexteOperation, statut)

    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            Dim ligne As StatutSeance = GestionStatutsSeance.GetStatutsSeance(True).
                FirstOrDefault(Function(s) s.IdStatutSeance = id)

            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionStatutsSeance.UpdateStatutSeance(_context?.ContexteOperation, ligne)
            End If
        Else
            GestionStatutsSeance.DesactiverStatutSeance(_context?.ContexteOperation, id)
        End If

    End Sub

#End Region

End Class
