' -------------------------------------------------------------------------------------------------
' UserControl : UC_SituationsJuridiques
' Projet      : Althéa
' Version     : V1.3.0
' Date        : 22/10/2026
'
' Rôle        :
' Écran de gestion du référentiel des situations juridiques du cabinet (ref_situations_juridiques).
' Hérite de UC_ReferentielBase ; champ supplémentaire « Porteur du BCE » (Cabinet / Praticien).
'
' Remarques   :
' - V1.1.0 (16/10/2026) : liste du kit posée par AjouterLigneDetail ; libellé « Porteur BCE »
'   (l'explication complète est dans l'infobulle).
' - V1.2.0 (20/10/2026) : chemin affiché « Administration > Situations juridiques » (l'écran
'   s'ouvre depuis le hub Administration, pas depuis les Référentiels).
' - V1.3.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_SituationsJuridiques
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_situationJuridique
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Situations juridiques"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les situations juridiques du cabinet"
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Administration > Situations juridiques"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 50
        End Get
    End Property

#End Region

#Region "Champ supplémentaire - porteur du BCE"

    Private _cboPorteur As AltheaListe
    Private _situationsParId As New Dictionary(Of ULong, SituationJuridique)

    Protected Overrides Sub ConfigurerChampSupplementaire()

        _cboPorteur = CreerListeDetail("cboPorteurBce", 160)
        _cboPorteur.Items.AddRange(New Object() {"Cabinet", "Praticien"})
        _cboPorteur.SelectedIndex = 0
        AjouterLigneDetail("Porteur BCE", _cboPorteur)

        If _context IsNot Nothing Then
            _context.SetToolTip(_cboPorteur,
                "Numéro BCE porté par :" & vbCrLf &
                "Cabinet : le numéro BCE de la fiche cabinet est utilisé (indépendant, société)." & vbCrLf &
                "Praticien : chaque praticien porte son propre numéro BCE (cabinet de groupe).")
        End If

    End Sub

    Protected Overrides Sub AfficherChampSupplementaire(ligne As ReferentielLigne)
        If _cboPorteur Is Nothing Then Return
        Dim s As SituationJuridique = Nothing
        Dim parPraticien = ligne IsNot Nothing AndAlso
                           _situationsParId.TryGetValue(ligne.IdReferentiel, s) AndAlso
                           s.BceParPraticien
        _cboPorteur.SelectedIndex = If(parPraticien, 1, 0)
    End Sub

    Protected Overrides Sub ViderChampSupplementaire()
        If _cboPorteur IsNot Nothing Then _cboPorteur.SelectedIndex = 0
    End Sub

    Private Function PorteurSaisi() As String
        Return If(_cboPorteur IsNot Nothing AndAlso _cboPorteur.SelectedIndex = 1,
                  SituationJuridique.PORTEUR_PRATICIEN,
                  SituationJuridique.PORTEUR_CABINET)
    End Function

#End Region

#Region "Données - branchement GestionSituationsJuridiques"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim situations = GestionSituationsJuridiques.GetSituationsJuridiques(afficherInactifs)
        _situationsParId = situations.ToDictionary(Function(s) s.IdSituationJuridique)

        Return situations.Select(
            Function(s) New ReferentielLigne With {
                .IdReferentiel = s.IdSituationJuridique,
                .Code = s.CodeSituationJuridique,
                .Libelle = s.LibelleSituationJuridique,
                .OrdreAffichage = s.OrdreAffichage,
                .Actif = s.Actif,
                .EstSysteme = s.EstSysteme
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionSituationsJuridiques.CodeSituationJuridiqueExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionSituationsJuridiques.LibelleSituationJuridiqueExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)
        GestionSituationsJuridiques.InsertSituationJuridique(_context?.ContexteOperation, New SituationJuridique With {
            .CodeSituationJuridique = code,
            .LibelleSituationJuridique = libelle,
            .PorteurBce = PorteurSaisi(),
            .OrdreAffichage = ordre,
            .Actif = actif
        })
    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)
        GestionSituationsJuridiques.UpdateSituationJuridique(_context?.ContexteOperation, New SituationJuridique With {
            .IdSituationJuridique = id,
            .CodeSituationJuridique = code,
            .LibelleSituationJuridique = libelle,
            .PorteurBce = PorteurSaisi(),
            .OrdreAffichage = ordre,
            .Actif = actif
        })
    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)
        If actif Then
            Dim ligne = GestionSituationsJuridiques.GetSituationsJuridiques(True).
                FirstOrDefault(Function(s) s.IdSituationJuridique = id)
            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionSituationsJuridiques.UpdateSituationJuridique(_context?.ContexteOperation, ligne)
            End If
        Else
            GestionSituationsJuridiques.DesactiverSituationJuridique(_context?.ContexteOperation, id)
        End If
    End Sub

#End Region

End Class
