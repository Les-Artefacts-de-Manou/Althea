' -------------------------------------------------------------------------------------------------
' UserControl : UC_Periodicites
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 22/10/2026
'
' Rôle        :
' Écran de gestion du référentiel des périodicités d'encodage / de facturation (ref_periodicites).
' Hérite de UC_ReferentielBase ; champ supplémentaire « Durée (mois) » avec case « non périodique ».
'
' Remarques   :
' - V1.1.0 (16/10/2026, UI-7) : champs posés par AjouterLigneDetail (contrôles du kit, plus de
'   couleur ni de position en dur) ; consultation lisible gérée par UC_ReferentielBase.
' - V1.2.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Aucun comportement n'est déduit du code : la durée est portée par nb_mois (NULL = à la demande).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_Periodicites
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_periodicite
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Périodicités"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les périodicités d'encodage / de facturation des organismes."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Périodicités"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 30
        End Get
    End Property

#End Region

#Region "Champ supplémentaire - durée en mois"

    Private _numNbMois As NumericUpDown
    Private _chkNonPeriodique As AltheaCase
    Private _periodicitesParId As New Dictionary(Of ULong, Periodicite)

    Protected Overrides Sub ConfigurerChampSupplementaire()

        _numNbMois = CreerNombreDetail("numNbMois", 1D, 120D, 0, 80)
        _numNbMois.Value = 1D
        AjouterLigneDetail("Durée (mois)", _numNbMois)

        _chkNonPeriodique = CreerCaseDetail("chkNonPeriodique", "Non périodique (à la demande)")
        AjouterLigneDetail("Rythme", _chkNonPeriodique)

        AddHandler _chkNonPeriodique.CheckedChanged, Sub() AppliquerNonPeriodique()

        If _context IsNot Nothing Then
            _context.SetToolTip(_numNbMois, "Durée de la période en mois (1 = mensuelle, 3 = trimestrielle, 12 = annuelle).")
            _context.SetToolTip(_chkNonPeriodique, "Aucune période fixe : l'encodage se fait à la demande.")
        End If

    End Sub

    Protected Overrides Sub AfficherChampSupplementaire(ligne As ReferentielLigne)
        If _numNbMois Is Nothing Then Return
        Dim p As Periodicite = Nothing
        If ligne IsNot Nothing AndAlso _periodicitesParId.TryGetValue(ligne.IdReferentiel, p) AndAlso p.NbMois.HasValue Then
            _chkNonPeriodique.Checked = False
            _numNbMois.Value = Math.Min(Math.Max(p.NbMois.Value, _numNbMois.Minimum), _numNbMois.Maximum)
        Else
            _chkNonPeriodique.Checked = True
            _numNbMois.Value = 1D
        End If
    End Sub

    Protected Overrides Sub ViderChampSupplementaire()
        If _numNbMois Is Nothing Then Return
        _chkNonPeriodique.Checked = False
        _numNbMois.Value = 1D
    End Sub

    Protected Overrides Sub ActiverChampSupplementaire(enEdition As Boolean)
        AppliquerNonPeriodique()
    End Sub

    ' Une périodicité « à la demande » n'a pas de durée : en saisie, la durée passe alors en lecture
    ' seule (la case est active seulement en saisie : c'est elle qui dit le mode courant).
    Private Sub AppliquerNonPeriodique()
        If _numNbMois Is Nothing Then Return
        UtilsControls.DefinirChampLectureSeule(_numNbMois, Not _chkNonPeriodique.Enabled OrElse _chkNonPeriodique.Checked)
    End Sub

    Private Function NbMoisSaisi() As Integer?
        If _chkNonPeriodique Is Nothing OrElse _chkNonPeriodique.Checked Then Return Nothing
        Return CInt(_numNbMois.Value)
    End Function

#End Region

#Region "Colonne supplémentaire - grille"

    Protected Overrides Sub ConfigurerColonnesSupplementaires()

        Dim colNbMois As New DataGridViewTextBoxColumn With {
            .Name = "colNbMois",
            .HeaderText = "Mois",
            .DataPropertyName = NameOf(ReferentielLigne.Complement),
            .ReadOnly = True,
            .Width = 60
        }
        colNbMois.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        Dim indexActif As Integer = dgvReferentiel.Columns.IndexOf(colActif)
        If indexActif >= 0 Then
            dgvReferentiel.Columns.Insert(indexActif, colNbMois)
        Else
            dgvReferentiel.Columns.Add(colNbMois)
        End If

    End Sub

#End Region

#Region "Données - branchement GestionPeriodicites"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim periodicites = GestionPeriodicites.GetPeriodicites(afficherInactifs)
        _periodicitesParId = periodicites.ToDictionary(Function(p) p.IdPeriodicite)

        Return periodicites.Select(
            Function(p) New ReferentielLigne With {
                .IdReferentiel = p.IdPeriodicite,
                .Code = p.CodePeriodicite,
                .Libelle = p.LibellePeriodicite,
                .Complement = If(p.NbMois.HasValue, p.NbMois.Value.ToString(), "—"),
                .OrdreAffichage = p.OrdreAffichage,
                .Actif = p.Actif,
                .EstSysteme = p.EstSysteme
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionPeriodicites.CodePeriodiciteExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionPeriodicites.LibellePeriodiciteExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)
        GestionPeriodicites.InsertPeriodicite(_context?.ContexteOperation, New Periodicite With {
            .CodePeriodicite = code,
            .LibellePeriodicite = libelle,
            .NbMois = NbMoisSaisi(),
            .OrdreAffichage = ordre,
            .Actif = actif
        })
    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)
        GestionPeriodicites.UpdatePeriodicite(_context?.ContexteOperation, New Periodicite With {
            .IdPeriodicite = id,
            .CodePeriodicite = code,
            .LibellePeriodicite = libelle,
            .NbMois = NbMoisSaisi(),
            .OrdreAffichage = ordre,
            .Actif = actif
        })
    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)
        If actif Then
            Dim ligne = GestionPeriodicites.GetPeriodicites(True).
                FirstOrDefault(Function(p) p.IdPeriodicite = id)
            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionPeriodicites.UpdatePeriodicite(_context?.ContexteOperation, ligne)
            End If
        Else
            GestionPeriodicites.DesactiverPeriodicite(_context?.ContexteOperation, id)
        End If
    End Sub

#End Region

End Class
