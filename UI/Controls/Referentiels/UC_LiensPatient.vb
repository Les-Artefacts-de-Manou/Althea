' -------------------------------------------------------------------------------------------------
' UserControl : UC_LiensPatient
' Projet      : Althéa
' Version     : V1.2.1
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des liens de parenté/relation avec le patient
' (table ref_liens_patient). Hérite de UC_ReferentielBase et se contente de fournir les
' métadonnées d'affichage et de brancher la couche métier GestionLiensPatient via les
' points d'extension.
'
' Responsabilités :
' - Définir le titre, le sous-titre et le fil d'Ariane propres aux liens patient
' - Charger les liens patient via GestionLiensPatient et les convertir en ReferentielLigne
' - Déléguer les vérifications d'unicité (code, libellé) et le CRUD à GestionLiensPatient
'
' Remarques   :
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Aucun accès direct à la base de données : tout passe par GestionLiensPatient.
' - Le code peut être long (varchar 50) avec des espaces remplacés automatiquement par des _
'   (comportement par défaut de UC_ReferentielBase, donc aucune surcharge nécessaire ici).
'
' Dépendances :
' - UC_ReferentielBase (classe de base héritée)
' - GestionLiensPatient (couche métier)
' - LienPatient (modèle métier)
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

Public Class UC_LiensPatient
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_liensPatient
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Liens patient"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les liens de parenté/relation avec le patient."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Liens patient"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 50
        End Get
    End Property

#End Region

#Region "Champ supplémentaire - par défaut"

    ' Case « Par défaut » (colonne par_defaut, v2.57.0) et cache des lignes chargées,
    ' ReferentielLigne ne transportant pas ce drapeau.
    Private _chkParDefaut As AltheaCase
    Private _parId As New Dictionary(Of ULong, LienPatient)

    Protected Overrides Sub ConfigurerChampSupplementaire()

        _chkParDefaut = CreerCaseDetail("chkParDefaut", "Proposé pour un nouveau contact")
        AjouterLigneDetail("Par défaut", _chkParDefaut)

        _context?.SetToolTip(_chkParDefaut, "Lien présélectionné à la création d'un contact (un seul à la fois : cocher ici décoche l'ancien). Ex. : Mère.")

    End Sub

    Protected Overrides Sub AfficherChampSupplementaire(ligne As ReferentielLigne)
        If _chkParDefaut Is Nothing Then Return
        Dim element As LienPatient = Nothing
        _chkParDefaut.Checked = ligne IsNot Nothing AndAlso _parId.TryGetValue(ligne.IdReferentiel, element) AndAlso element.ParDefaut
    End Sub

    Protected Overrides Sub ViderChampSupplementaire()
        If _chkParDefaut IsNot Nothing Then _chkParDefaut.Checked = False
    End Sub

    Private Function ParDefautSaisi() As Boolean
        Return _chkParDefaut IsNot Nothing AndAlso _chkParDefaut.Checked
    End Function

#End Region

#Region "Données - branchement GestionLiensPatient"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ChargerElements
    ' Rôle     : Charge les liens patient via GestionLiensPatient et les convertit en ReferentielLigne.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim liens As List(Of LienPatient) = GestionLiensPatient.GetLiensPatient(afficherInactifs)

        _parId = liens.ToDictionary(Function(x) x.IdLienPatient)

        Return liens.Select(
            Function(l) New ReferentielLigne With {
                .IdReferentiel = l.IdLienPatient,
                .Code = l.CodeLienPatient,
                .Libelle = l.LibelleLienPatient,
                .OrdreAffichage = l.OrdreAffichage,
                .Actif = l.Actif,
                .EstSysteme = l.EstSysteme
            }).ToList()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CodeExisteDeja
    ' Rôle     : Vérifie l'unicité du code via GestionLiensPatient.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionLiensPatient.CodeLienPatientExiste(code, idExclu)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LibelleExisteDeja
    ' Rôle     : Vérifie l'unicité du libellé via GestionLiensPatient.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionLiensPatient.LibelleLienPatientExiste(libelle, idExclu)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InsererElement
    ' Rôle      : Insère un nouveau lien patient via GestionLiensPatient.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim lien As New LienPatient With {
            .CodeLienPatient = code,
            .LibelleLienPatient = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif,
            .ParDefaut = ParDefautSaisi()
        }

        GestionLiensPatient.InsertLienPatient(_context?.ContexteOperation, lien)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourElement
    ' Rôle      : Met à jour un lien patient existant via GestionLiensPatient.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim lien As New LienPatient With {
            .IdLienPatient = id,
            .CodeLienPatient = code,
            .LibelleLienPatient = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif,
            .ParDefaut = ParDefautSaisi()
        }

        GestionLiensPatient.UpdateLienPatient(_context?.ContexteOperation, lien)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirActivation
    ' Rôle      : Active ou désactive un lien patient (soft-delete) via GestionLiensPatient.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            ' Réactivation : on repasse Actif à 1 en conservant les autres valeurs.
            Dim ligne As LienPatient = GestionLiensPatient.GetLiensPatient(True).
                FirstOrDefault(Function(l) l.IdLienPatient = id)

            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionLiensPatient.UpdateLienPatient(_context?.ContexteOperation, ligne)
            End If
        Else
            ' Désactivation : soft-delete.
            GestionLiensPatient.DesactiverLienPatient(_context?.ContexteOperation, id)
        End If

    End Sub

#End Region

End Class
