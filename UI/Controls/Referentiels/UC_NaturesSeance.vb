' -------------------------------------------------------------------------------------------------
' UserControl : UC_NaturesSeance
' Projet      : Althéa
' Version     : V1.3.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des natures de séance (table ref_natures_seance).
' Hérite de UC_ReferentielBase et se contente de fournir les métadonnées d'affichage
' et de brancher la couche métier GestionNaturesSeance via les points d'extension.
'
' Responsabilités :
' - Définir le titre, le sous-titre et le fil d'Ariane propres aux natures de séance
' - Charger les natures via GestionNaturesSeance et les convertir en ReferentielLigne
' - Déléguer les vérifications d'unicité (code, libellé) et le CRUD à GestionNaturesSeance
' - Héberger l'onglet des domaines où la nature est proposée à la saisie (D-POR-5)
'
' Important   :
' - La nature de séance (Bilan, Traitement, Fin de traitement, RV intervenant, Autre) n'a AUCUNE
'   incidence sur le prix : le tarif provient exclusivement de ref_prix_seance.
' - La catégorie de quota se paramètre par domaine (onglet Natures de l'espace Domaines).
'
' Remarques   :
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Aucun accès direct à la base de données : tout passe par GestionNaturesSeance.
' - Conformément au choix d'ergonomie retenu, les données liées sont intégrées sous forme
'   d'onglet plutôt que de fenêtre modale séparée.
' - V1.2.0 : retrait du cache des natures (NatureDeLaLigne / _naturesParId), devenu inutile
'   depuis que la nature ne porte plus ni code de prestation ni catégorie de quota.
' - V1.3.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
'
' Dépendances :
' - UC_ReferentielBase (classe de base héritée)
' - GestionNaturesSeance (couche métier)
' - NatureSeance (modèle métier)
' - ReferentielLigne (modèle de présentation générique)
' - UC_LiaisonsNaturesDomaines
' - AppRole (gestion des droits)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_NaturesSeance
    Inherits UC_ReferentielBase

#Region "Variables privées"

    ' Panneau
    Private _pnlDomaines As UC_LiaisonsNaturesDomaines

#End Region

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_naturesSeances
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Natures de séance"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les natures de séance"
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Natures de séance"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 30
        End Get
    End Property

#End Region

#Region "Données liées - onglet intégré"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : HauteurDonneesLiees
    ' Rôle      : Réserve la zone d'onglets sous la liste des natures.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides ReadOnly Property HauteurDonneesLiees As Integer
        Get
            Return 290
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ConfigurerDonneesLiees
    ' Rôle      : Crée l'onglet « Domaines concernés ».
    ' Remarques : Le panneau est orienté « par nature » : il liste les domaines dans lesquels
    '             cette nature est proposée à la saisie d'une séance.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub ConfigurerDonneesLiees()

        If TabDonneesLiees Is Nothing Then Exit Sub

        _pnlDomaines = New UC_LiaisonsNaturesDomaines With {
            .Mode = UC_LiaisonsNaturesDomaines.SensLiaison.ParNature,
            .FournisseurContexte = Function() _context?.ContexteOperation
        }

        Dim onglet As New TabPage("Domaines concernés")
        onglet.Controls.Add(_pnlDomaines)

        TabDonneesLiees.TabPages.Add(onglet)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerDonneesLiees
    ' Rôle      : Recharge l'onglet pour la nature sélectionnée.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub ChargerDonneesLiees(ligne As ReferentielLigne)

        If _pnlDomaines Is Nothing Then Exit Sub

        _pnlDomaines.ChargerPour(If(ligne Is Nothing, 0UL, ligne.IdReferentiel))

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ActiverDonneesLiees
    ' Rôle      : Passe le panneau en lecture seule pendant l'édition de la nature parente.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub ActiverDonneesLiees(enEdition As Boolean)

        MyBase.ActiverDonneesLiees(enEdition)

        If _pnlDomaines IsNot Nothing Then _pnlDomaines.DefinirLectureSeule(enEdition)

    End Sub

#End Region

#Region "Données - branchement GestionNaturesSeance"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim natures As List(Of NatureSeance) = GestionNaturesSeance.GetNaturesSeance(afficherInactifs)

        Return natures.Select(
            Function(n) New ReferentielLigne With {
                .IdReferentiel = n.IdNatureSeance,
                .Code = n.CodeNatureSeance,
                .Libelle = n.LibelleNatureSeance,
                .OrdreAffichage = n.OrdreAffichage,
                .Actif = n.Actif
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionNaturesSeance.CodeNatureSeanceExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionNaturesSeance.LibelleNatureSeanceExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim nature As New NatureSeance With {
            .CodeNatureSeance = code,
            .LibelleNatureSeance = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionNaturesSeance.InsertNatureSeance(_context?.ContexteOperation, nature)

    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim nature As New NatureSeance With {
            .IdNatureSeance = id,
            .CodeNatureSeance = code,
            .LibelleNatureSeance = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionNaturesSeance.UpdateNatureSeance(_context?.ContexteOperation, nature)

    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            Dim ligne As NatureSeance = GestionNaturesSeance.GetNaturesSeance(True).
                FirstOrDefault(Function(n) n.IdNatureSeance = id)

            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionNaturesSeance.UpdateNatureSeance(_context?.ContexteOperation, ligne)
            End If
        Else
            GestionNaturesSeance.DesactiverNatureSeance(_context?.ContexteOperation, id)
        End If

    End Sub

#End Region

End Class
