' -------------------------------------------------------------------------------------------------
' UserControl : UC_PrixSeance
' Projet      : Althéa
' Version     : V1.3.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des tarifs de séance (table ref_prix_seance).
' Hérite de UC_ReferentielBase et fournit les métadonnées d'affichage ainsi que le branchement
' de la couche métier GestionPrixSeance via les points d'extension.
'
' Important   :
' - ref_prix_seance possède un champ supplémentaire montant (decimal(10,2) NOT NULL) qui n'existe
'   pas dans les référentiels simples. Il est ajouté au panneau d'édition générique via les points
'   d'extension « Champ supplémentaire » de UC_ReferentielBase :
'   ConfigurerChampSupplementaire, AfficherChampSupplementaire, ViderChampSupplementaire,
'   ValiderChampSupplementaire (la bascule consultation / saisie est faite par la base).
' - Le montant est la SOURCE lue à la création d'une séance puis FIGÉE dans seances.tarif_seance
'   (snapshot) : modifier ce montant n'affecte jamais les séances déjà comptées.
' - Un onglet intégré « Domaines rattachés » expose les liaisons historisées du tarif
'   (lia_prix_seance_domaines) : c'est ce rattachement qui rend le tarif applicable à un
'   domaine sur une période donnée (D-POR-4).
'
' Remarques   :
' - V1.2.0 (16/10/2026, UI-7) : champs posés par AjouterLigneDetail (contrôles du kit, plus de
'   couleur ni de position en dur) ; consultation lisible gérée par UC_ReferentielBase.
' - V1.3.0 (22/10/2026, audit C.1, étape 3) : écritures transmises avec le ContexteOperation, droit
'   vérifié par le métier ; surcharge RoleMinimum retirée (règle unique : UC_ReferentielBase /
'   ReglesReferentiels).
' - Aucun Designer propre : le visuel de base provient de UC_ReferentielBase ; le champ montant
'   est créé par code et ajouté au panneau d'édition (pnlEdition).
' - Aucun accès direct à la base de données : tout passe par GestionPrixSeance.
' - Conformément au choix d'ergonomie retenu, les données liées sont intégrées sous forme
'   d'onglet plutôt que de fenêtre modale séparée.
'
' Dépendances :
' - UC_ReferentielBase (classe de base héritée)
' - GestionPrixSeance (couche métier)
' - PrixSeance (modèle métier)
' - ReferentielLigne (modèle de présentation générique, propriété Tarif)
' - UC_LiaisonsPrixDomaines (panneau de données liées)
' - AppRole (gestion des droits)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_PrixSeance
    Inherits UC_ReferentielBase

#Region "Champ supplémentaire - montant"

    ' Sélecteur du montant (decimal(10,2))
    Private _numMontant As NumericUpDown

    ' Panneau des domaines rattachés, hébergé dans l'onglet de données liées
    Private _pnlDomaines As UC_LiaisonsPrixDomaines

#End Region

#Region "Métadonnées"

    Protected Overrides ReadOnly Property IconeTitre As Image
        Get
            Return My.Resources.Titre_tarifsSeance
        End Get
    End Property

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Tarifs de séance"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les tarifs de séance et leur montant"
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Prix de séance"
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 30
        End Get
    End Property

#End Region

#Region "Champ supplémentaire - points d'extension"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ConfigurerChampSupplementaire
    ' Rôle      : Crée le montant, sous l'ordre, dans le panneau « Détail » (l'état suit, placé par la
    '             base). Appelée une fois au chargement.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub ConfigurerChampSupplementaire()

        _numMontant = CreerNombreDetail("numMontant", 0D, 99999999.99D, 2, 130)
        _numMontant.Increment = 1D
        AjouterLigneDetail("Montant (€)", _numMontant)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherChampSupplementaire
    ' Rôle      : Affiche le montant de la ligne sélectionnée dans le sélecteur.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub AfficherChampSupplementaire(ligne As ReferentielLigne)

        If _numMontant Is Nothing Then Return

        Dim valeur As Decimal = If(ligne IsNot Nothing AndAlso ligne.Tarif.HasValue, ligne.Tarif.Value, 0D)
        _numMontant.Value = Math.Min(Math.Max(valeur, _numMontant.Minimum), _numMontant.Maximum)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ViderChampSupplementaire
    ' Rôle      : Réinitialise le montant (mode création / vidage du détail).
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub ViderChampSupplementaire()
        If _numMontant IsNot Nothing Then _numMontant.Value = 0D
    End Sub

#End Region

#Region "Colonnes supplémentaires - grille principale"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ConfigurerColonnesSupplementaires
    ' Rôle      : Ajoute la colonne « Montant » à la liste des tarifs.
    ' Remarques : Le montant est l'information la plus utile de ce référentiel : l'afficher dans la
    '             grille évite de devoir sélectionner chaque ligne pour le consulter.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub ConfigurerColonnesSupplementaires()

        Dim colMontant As New DataGridViewTextBoxColumn With {
            .Name = "colMontant",
            .HeaderText = "Montant",
            .DataPropertyName = NameOf(ReferentielLigne.Tarif),
            .ReadOnly = True,
            .Width = 110
        }

        colMontant.DefaultCellStyle.Format = "C2"
        colMontant.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        ' Insérée juste avant la colonne « Actif » afin de conserver l'état en fin de ligne.
        Dim indexActif As Integer = dgvReferentiel.Columns.IndexOf(colActif)

        If indexActif >= 0 Then
            dgvReferentiel.Columns.Insert(indexActif, colMontant)
        Else
            dgvReferentiel.Columns.Add(colMontant)
        End If

    End Sub

#End Region

#Region "Données liées - onglet intégré"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : HauteurDonneesLiees
    ' Rôle      : Réserve la zone d'onglets sous la liste des tarifs.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides ReadOnly Property HauteurDonneesLiees As Integer
        Get
            Return 310
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ConfigurerDonneesLiees
    ' Rôle      : Crée l'onglet « Domaines rattachés ».
    ' Remarques : Le panneau est orienté « par tarif » : il liste les domaines auxquels ce tarif
    '             s'applique, avec la période de validité de chaque rattachement.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub ConfigurerDonneesLiees()

        If TabDonneesLiees Is Nothing Then Exit Sub

        _pnlDomaines = New UC_LiaisonsPrixDomaines With {
            .Mode = UC_LiaisonsPrixDomaines.SensLiaison.ParPrix,
            .FournisseurContexte = Function() _context?.ContexteOperation
        }

        Dim onglet As New TabPage("Domaines rattachés")
        onglet.Controls.Add(_pnlDomaines)

        TabDonneesLiees.TabPages.Add(onglet)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerDonneesLiees
    ' Rôle      : Recharge l'onglet pour le tarif sélectionné.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub ChargerDonneesLiees(ligne As ReferentielLigne)

        If _pnlDomaines Is Nothing Then Exit Sub

        _pnlDomaines.ChargerPour(If(ligne Is Nothing, 0UL, ligne.IdReferentiel))

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ActiverDonneesLiees
    ' Rôle      : Passe le panneau en lecture seule pendant l'édition du tarif parent.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub ActiverDonneesLiees(enEdition As Boolean)

        MyBase.ActiverDonneesLiees(enEdition)

        If _pnlDomaines IsNot Nothing Then _pnlDomaines.DefinirLectureSeule(enEdition)

    End Sub

#End Region

#Region "Données - branchement GestionPrixSeance"

    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim tarifs As List(Of PrixSeance) = GestionPrixSeance.GetPrixSeance(afficherInactifs)

        Return tarifs.Select(
            Function(t) New ReferentielLigne With {
                .IdReferentiel = t.IdPrixSeance,
                .Code = t.CodePrixSeance,
                .Libelle = t.LibellePrixSeance,
                .OrdreAffichage = t.OrdreAffichage,
                .Actif = t.Actif,
                .EstSysteme = t.EstSysteme,
                .Tarif = t.Montant
            }).ToList()

    End Function

    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionPrixSeance.CodePrixSeanceExiste(code, idExclu)
    End Function

    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionPrixSeance.LibellePrixSeanceExiste(libelle, idExclu)
    End Function

    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim prix As New PrixSeance With {
            .CodePrixSeance = code,
            .LibellePrixSeance = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif,
            .Montant = If(_numMontant IsNot Nothing, _numMontant.Value, 0D)
        }

        GestionPrixSeance.InsertPrixSeance(_context?.ContexteOperation, prix)

    End Sub

    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim prix As New PrixSeance With {
            .IdPrixSeance = id,
            .CodePrixSeance = code,
            .LibellePrixSeance = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif,
            .Montant = If(_numMontant IsNot Nothing, _numMontant.Value, 0D)
        }

        GestionPrixSeance.UpdatePrixSeance(_context?.ContexteOperation, prix)

    End Sub

    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            ' Réactivation : on repasse Actif à 1 en conservant les autres valeurs (dont le montant).
            Dim ligne As PrixSeance = GestionPrixSeance.GetPrixSeance(True).
                FirstOrDefault(Function(t) t.IdPrixSeance = id)

            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionPrixSeance.UpdatePrixSeance(_context?.ContexteOperation, ligne)
            End If
        Else
            ' Désactivation : soft-delete.
            GestionPrixSeance.DesactiverPrixSeance(_context?.ContexteOperation, id)
        End If

    End Sub

#End Region

End Class
