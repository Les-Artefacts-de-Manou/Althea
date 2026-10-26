' -------------------------------------------------------------------------------------------------
' UserControl : UC_RoleLegal
' Projet      : Althéa
' Version     : V1.2.1
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des rôles légaux des contacts de l'entourage du patient
' (table ref_role_legal). Hérite de UC_ReferentielBase et se contente de fournir les
' métadonnées d'affichage et de brancher la couche métier GestionRoleLegal via les
' points d'extension.
'
' Responsabilités :
' - Définir le titre, le sous-titre et le fil d'Ariane propres aux rôles légaux
' - Charger les rôles légaux via GestionRoleLegal et les convertir en ReferentielLigne
' - Déléguer les vérifications d'unicité (code, libellé) et le CRUD à GestionRoleLegal
'
' Remarques   :
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Aucun accès direct à la base de données : tout passe par GestionRoleLegal.
' - Le code peut être long (varchar 30) avec des espaces remplacés automatiquement par des _
'   (comportement par défaut de UC_ReferentielBase, donc aucune surcharge nécessaire ici).
'
' Dépendances :
' - UC_ReferentielBase (classe de base héritée)
' - GestionRoleLegal (couche métier)
' - RoleLegal (modèle métier)
' - ReferentielLigne (modèle de présentation générique)
' - AppRole (gestion des droits)
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
' - V1.2.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : case « Proposé par défaut pour un nouveau contact » (par_defaut).
' - V1.2.1 (25/10/2026, relevé aux captures) : libellé de la case raccourci en « Proposé pour un
'   nouveau contact » (coupé dans le panneau Détail ; la ligne s'intitule déjà « Par défaut »).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_RoleLegal
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_rolesLegaux
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Rôles légaux"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les rôles légaux des contacts du patient"
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Rôles légaux"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 30
        End Get
    End Property

#End Region

#Region "Champ supplémentaire - par défaut"

    ' Case « Par défaut » (colonne par_defaut, v2.57.0) et cache des lignes chargées,
    ' ReferentielLigne ne transportant pas ce drapeau.
    Private _chkParDefaut As AltheaCase
    Private _parId As New Dictionary(Of ULong, RoleLegal)

    Protected Overrides Sub ConfigurerChampSupplementaire()

        _chkParDefaut = CreerCaseDetail("chkParDefaut", "Proposé pour un nouveau contact")
        AjouterLigneDetail("Par défaut", _chkParDefaut)

        _context?.SetToolTip(_chkParDefaut, "Rôle légal présélectionné à la création d'un contact (un seul à la fois : cocher ici décoche l'ancien). Ex. : Autorité parentale.")

    End Sub

    Protected Overrides Sub AfficherChampSupplementaire(ligne As ReferentielLigne)
        If _chkParDefaut Is Nothing Then Return
        Dim element As RoleLegal = Nothing
        _chkParDefaut.Checked = ligne IsNot Nothing AndAlso _parId.TryGetValue(ligne.IdReferentiel, element) AndAlso element.ParDefaut
    End Sub

    Protected Overrides Sub ViderChampSupplementaire()
        If _chkParDefaut IsNot Nothing Then _chkParDefaut.Checked = False
    End Sub

    Private Function ParDefautSaisi() As Boolean
        Return _chkParDefaut IsNot Nothing AndAlso _chkParDefaut.Checked
    End Function

#End Region

#Region "Données - branchement GestionRoleLegal"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ChargerElements
    ' Rôle     : Charge les rôles légaux via GestionRoleLegal et les convertit en ReferentielLigne.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim roles As List(Of RoleLegal) = GestionRoleLegal.GetRolesLegaux(afficherInactifs)

        _parId = roles.ToDictionary(Function(x) x.IdRoleLegal)

        Return roles.Select(
            Function(r) New ReferentielLigne With {
                .IdReferentiel = r.IdRoleLegal,
                .Code = r.CodeRoleLegal,
                .Libelle = r.LibelleRoleLegal,
                .OrdreAffichage = r.OrdreAffichage,
                .Actif = r.Actif,
                .EstSysteme = r.EstSysteme
            }).ToList()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CodeExisteDeja
    ' Rôle     : Vérifie l'unicité du code via GestionRoleLegal.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionRoleLegal.CodeRoleLegalExiste(code, idExclu)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LibelleExisteDeja
    ' Rôle     : Vérifie l'unicité du libellé via GestionRoleLegal.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionRoleLegal.LibelleRoleLegalExiste(libelle, idExclu)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InsererElement
    ' Rôle      : Insère un nouveau rôle légal via GestionRoleLegal.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim role As New RoleLegal With {
            .CodeRoleLegal = code,
            .LibelleRoleLegal = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif,
            .ParDefaut = ParDefautSaisi()
        }

        GestionRoleLegal.InsertRoleLegal(_context?.ContexteOperation, role)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourElement
    ' Rôle      : Met à jour un rôle légal existant via GestionRoleLegal.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim role As New RoleLegal With {
            .IdRoleLegal = id,
            .CodeRoleLegal = code,
            .LibelleRoleLegal = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif,
            .ParDefaut = ParDefautSaisi()
        }

        GestionRoleLegal.UpdateRoleLegal(_context?.ContexteOperation, role)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirActivation
    ' Rôle      : Active ou désactive un rôle légal (soft-delete) via GestionRoleLegal.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            ' Réactivation : on repasse Actif à 1 en conservant les autres valeurs.
            Dim ligne As RoleLegal = GestionRoleLegal.GetRolesLegaux(True).
                FirstOrDefault(Function(r) r.IdRoleLegal = id)

            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionRoleLegal.UpdateRoleLegal(_context?.ContexteOperation, ligne)
            End If
        Else
            ' Désactivation : soft-delete.
            GestionRoleLegal.DesactiverRoleLegal(_context?.ContexteOperation, id)
        End If

    End Sub

#End Region

End Class
