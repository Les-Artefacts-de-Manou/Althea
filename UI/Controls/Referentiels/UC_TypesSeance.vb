' -------------------------------------------------------------------------------------------------
' UserControl : UC_TypesSeance
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des types de séance (table ref_types_seance).
' Hérite de UC_ReferentielBase et fournit les métadonnées d'affichage ainsi que le branchement
' de la couche métier GestionTypesSeance via les points d'extension.
'
' PARTICULARITÉ :
' - ref_types_seance possède un champ supplémentaire tarif_defaut (decimal(10,2) NOT NULL) qui
'   n'existe pas dans les autres référentiels. Ce champ est ajouté au panneau d'édition générique
'   via les points d'extension « Champ supplémentaire » de UC_ReferentielBase :
'   ConfigurerChampSupplementaire, AfficherChampSupplementaire, ViderChampSupplementaire,
'   ActiverChampSupplementaire et ValiderChampSupplementaire.
'
' Responsabilités :
' - Définir le titre, le sous-titre et le fil d'Ariane propres aux types de séance
' - Charger les types de séance via GestionTypesSeance et les convertir en ReferentielLigne
' - Déléguer les vérifications d'unicité (code, libellé) et le CRUD à GestionTypesSeance
' - Greffer, afficher, valider et transmettre le tarif par défaut au moment du CRUD
'
' Remarques   :
' - Aucun Designer propre : le visuel de base provient de UC_ReferentielBase ; le champ tarif
'   est créé par code et ajouté au panneau d'édition (pnlEdition).
' - Aucun accès direct à la base de données : tout passe par GestionTypesSeance.
'
' Dépendances :
' - UC_ReferentielBase (classe de base héritée)
' - GestionTypesSeance (couche métier)
' - TypeSeance (modèle métier)
' - ReferentielLigne (modèle de présentation générique, propriété Tarif)
' - AppRole (gestion des droits)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_TypesSeance
    Inherits UC_ReferentielBase

#Region "Champ supplémentaire - tarif par défaut"

    ' Libellé du champ tarif (créé par code, ajouté au panneau d'édition de la classe de base)
    Private _lblTarif As Label

    ' Sélecteur du tarif par défaut (decimal(10,2))
    Private _numTarif As NumericUpDown

#End Region

#Region "Métadonnées"

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Types de séance"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les types de séance et leur tarif par défaut (consultation, bilan, groupe, ...)."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Types de séance"
        End Get
    End Property

    Protected Overrides ReadOnly Property RoleMinimum As AppRole
        Get
            Return AppRole.SuperUser
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
    ' Rôle      : Crée le libellé et le sélecteur de tarif et les ajoute au panneau d'édition de base,
    '             juste sous le champ « État ». Appelée une fois au chargement.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub ConfigurerChampSupplementaire()

        _lblTarif = New Label With {
            .AutoSize = True,
            .ForeColor = Color.FromArgb(95, 125, 110),
            .Location = New Point(20, 264),
            .Name = "lblTarif",
            .Text = "Tarif (€)"
        }

        _numTarif = New NumericUpDown With {
            .BackColor = Color.FromArgb(244, 239, 234),
            .ForeColor = Color.FromArgb(74, 74, 74),
            .Location = New Point(110, 262),
            .Name = "numTarif",
            .Size = New Size(170, 25),
            .DecimalPlaces = 2,
            .Increment = 1D,
            .Minimum = 0D,
            .Maximum = 99999999.99D,
            .ThousandsSeparator = True
        }

        pnlEdition.Controls.Add(_lblTarif)
        pnlEdition.Controls.Add(_numTarif)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherChampSupplementaire
    ' Rôle      : Affiche le tarif par défaut de la ligne sélectionnée dans le sélecteur.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub AfficherChampSupplementaire(ligne As ReferentielLigne)

        If _numTarif Is Nothing Then Return

        Dim valeur As Decimal = If(ligne IsNot Nothing AndAlso ligne.Tarif.HasValue, ligne.Tarif.Value, 0D)
        _numTarif.Value = Math.Min(Math.Max(valeur, _numTarif.Minimum), _numTarif.Maximum)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ViderChampSupplementaire
    ' Rôle      : Réinitialise le tarif (mode création / vidage du détail).
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub ViderChampSupplementaire()
        If _numTarif IsNot Nothing Then _numTarif.Value = 0D
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ActiverChampSupplementaire
    ' Rôle      : Active ou désactive le sélecteur de tarif selon le mode d'édition.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub ActiverChampSupplementaire(enEdition As Boolean)
        If _numTarif IsNot Nothing Then _numTarif.Enabled = enEdition
    End Sub

#End Region

#Region "Données - branchement GestionTypesSeance"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ChargerElements
    ' Rôle     : Charge les types de séance via GestionTypesSeance et les convertit en ReferentielLigne
    '            (en renseignant le tarif par défaut dans la propriété Tarif).
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim types As List(Of TypeSeance) = GestionTypesSeance.GetTypesSeance(afficherInactifs)

        Return types.Select(
            Function(t) New ReferentielLigne With {
                .IdReferentiel = t.IdTypeSeance,
                .Code = t.CodeTypeSeance,
                .Libelle = t.LibelleTypeSeance,
                .OrdreAffichage = t.OrdreAffichage,
                .Actif = t.Actif,
                .Tarif = t.TarifDefaut
            }).ToList()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CodeExisteDeja
    ' Rôle     : Vérifie l'unicité du code via GestionTypesSeance.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionTypesSeance.CodeTypeSeanceExiste(code, idExclu)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LibelleExisteDeja
    ' Rôle     : Vérifie l'unicité du libellé via GestionTypesSeance.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionTypesSeance.LibelleTypeSeanceExiste(libelle, idExclu)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InsererElement
    ' Rôle      : Insère un nouveau type de séance via GestionTypesSeance (tarif lu dans le sélecteur).
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim typeSeance As New TypeSeance With {
            .CodeTypeSeance = code,
            .LibelleTypeSeance = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif,
            .TarifDefaut = If(_numTarif IsNot Nothing, _numTarif.Value, 0D)
        }

        GestionTypesSeance.InsertTypeSeance(typeSeance)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourElement
    ' Rôle      : Met à jour un type de séance existant via GestionTypesSeance (tarif lu dans le sélecteur).
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim typeSeance As New TypeSeance With {
            .IdTypeSeance = id,
            .CodeTypeSeance = code,
            .LibelleTypeSeance = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif,
            .TarifDefaut = If(_numTarif IsNot Nothing, _numTarif.Value, 0D)
        }

        GestionTypesSeance.UpdateTypeSeance(typeSeance)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirActivation
    ' Rôle      : Active ou désactive un type de séance (soft-delete) via GestionTypesSeance.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            ' Réactivation : on repasse Actif à 1 en conservant les autres valeurs (dont le tarif).
            Dim ligne As TypeSeance = GestionTypesSeance.GetTypesSeance(True).
                FirstOrDefault(Function(t) t.IdTypeSeance = id)

            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionTypesSeance.UpdateTypeSeance(ligne)
            End If
        Else
            ' Désactivation : soft-delete.
            GestionTypesSeance.DesactiverTypeSeance(id)
        End If

    End Sub

#End Region

End Class
