' -------------------------------------------------------------------------------------------------
' UserControl : UC_ModesPaiement
' Projet      : Althéa
' Version     : V1.3.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des modes de paiement (table ref_modes_paiement).
' Hérite de UC_ReferentielBase et fournit les métadonnées d'affichage et le branchement
' de la couche métier GestionModesPaiement via les points d'extension.
'
' Important   :
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Aucun accès direct à la base de données : tout passe par GestionModesPaiement.
' - Code long (varchar 30) avec espaces convertis en _ (comportement par défaut de la base).
' - Un mode de paiement reste une donnée de référentiel : aucun comportement métier ne dépend de
'   son libellé ou de son code.
'
' Dépendances :
' - UC_ReferentielBase (classe de base héritée)
' - GestionModesPaiement (couche métier)
' - ModePaiement (modèle métier)
' - ReferentielLigne (modèle de présentation générique)
' - AppRole (gestion des droits)
'
' Remarques   :
' - V1.2.0 (16/10/2026, UI-7) : champs posés par AjouterLigneDetail (contrôles du kit, plus de
'   couleur ni de position en dur) ; consultation lisible gérée par UC_ReferentielBase.
' - V1.3.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_ModesPaiement
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_modesPaiement
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Modes de paiement"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les modes de paiement."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Modes de paiement"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 30
        End Get
    End Property

#End Region

#Region "Champ supplémentaire - mode avoir"

    ' Case « Mode avoir » (colonne est_avoir) et cache des modes chargés, ReferentielLigne ne
    ' transportant pas ce drapeau.
    Private _chkEstAvoir As AltheaCase
    Private _modesParId As New Dictionary(Of ULong, ModePaiement)

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ConfigurerChampSupplementaire
    ' Rôle      : Crée la case « Mode avoir » dans le panneau « Détail » (l'état suit, placé par la base).
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub ConfigurerChampSupplementaire()

        _chkEstAvoir = CreerCaseDetail("chkEstAvoir", "Mode avoir (remboursement / crédit)")
        AjouterLigneDetail("Avoir", _chkEstAvoir)

        If _context IsNot Nothing Then
            _context.SetToolTip(_chkEstAvoir,
                "Mode utilisé automatiquement pour les avoirs. Il n'est pas proposé à la saisie " &
                "manuelle d'un encaissement.")
        End If

    End Sub

    Protected Overrides Sub AfficherChampSupplementaire(ligne As ReferentielLigne)

        If _chkEstAvoir Is Nothing Then Return

        Dim mode As ModePaiement = Nothing
        _chkEstAvoir.Checked = ligne IsNot Nothing AndAlso
                               _modesParId.TryGetValue(ligne.IdReferentiel, mode) AndAlso
                               mode.EstAvoir

    End Sub

    Protected Overrides Sub ViderChampSupplementaire()
        If _chkEstAvoir IsNot Nothing Then _chkEstAvoir.Checked = False
    End Sub

    Private Function EstAvoirSaisi() As Boolean
        Return _chkEstAvoir IsNot Nothing AndAlso _chkEstAvoir.Checked
    End Function

#End Region

#Region "Données - branchement GestionModesPaiement"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim modes As List(Of ModePaiement) = GestionModesPaiement.GetModesPaiement(afficherInactifs)

        _modesParId = modes.ToDictionary(Function(m) m.IdModePaiement)

        Return modes.Select(
            Function(m) New ReferentielLigne With {
                .IdReferentiel = m.IdModePaiement,
                .Code = m.CodeModePaiement,
                .Libelle = m.LibelleModePaiement,
                .OrdreAffichage = m.OrdreAffichage,
                .Actif = m.Actif,
                .EstSysteme = m.EstSysteme
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionModesPaiement.CodeModePaiementExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionModesPaiement.LibelleModePaiementExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim mode As New ModePaiement With {
            .CodeModePaiement = code,
            .LibelleModePaiement = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif,
            .EstAvoir = EstAvoirSaisi()
        }

        GestionModesPaiement.InsertModePaiement(_context?.ContexteOperation, mode)

    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim mode As New ModePaiement With {
            .IdModePaiement = id,
            .CodeModePaiement = code,
            .LibelleModePaiement = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif,
            .EstAvoir = EstAvoirSaisi()
        }

        GestionModesPaiement.UpdateModePaiement(_context?.ContexteOperation, mode)

    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            Dim ligne As ModePaiement = GestionModesPaiement.GetModesPaiement(True).
                FirstOrDefault(Function(m) m.IdModePaiement = id)

            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionModesPaiement.UpdateModePaiement(_context?.ContexteOperation, ligne)
            End If
        Else
            GestionModesPaiement.DesactiverModePaiement(_context?.ContexteOperation, id)
        End If

    End Sub

#End Region

End Class
