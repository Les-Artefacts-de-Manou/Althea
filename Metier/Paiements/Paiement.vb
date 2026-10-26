' -------------------------------------------------------------------------------------------------
' Classe      : Paiement
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 21/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Représente un paiement encaissé, rattaché en V1 à une séance unique via lia_paiements_seances.
'
' Responsabilités :
' - Contenir les données d'identité du paiement (date, montant, mode, référence, commentaire)
' - Porter l'identifiant de la séance affectée et le montant affecté (liaison N-N en V1 : 1↔1)
' - Porter le mode de paiement choisi (identifiant du référentiel ref_modes_paiement)
'
' Table source : paiements (+ lia_paiements_seances pour la liaison à la séance)
'
' Important   :
' - En V1, un paiement est affecté à UNE seule séance (D-PAY-1) : IdSeance + MontantAffecte.
' - Le mode de paiement est relié au référentiel par son identifiant (paiements.id_mode_paiement,
'   v2.50.0) : renommer un mode ne coupe plus le lien avec l'historique.
' - id_paiement et code_paiement sont générés par la base : jamais fournis par l'application.
'
' Architecture :
' - Classe de données (DTO-like) sans logique métier complexe
' - Ne contient aucun accès base de données ni logique UI
'
' Dépendances :
' - Utilisée par GestionPaiements (mapping depuis DataReader)
' - Utilisée par QueryPaiements (requêtes SQL)
' - Utilisée par les Forms/UserControls paiements (PaiementEditor, UC_DossierFiche)
'
' Historique  :
' - V1.1.0 (21/10/2026, audit B.5) : ModePaiement (libellé) remplacé par IdModePaiement.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class Paiement

#Region "Propriétés — Identité"

    ' Identifiant unique du paiement (clé primaire, généré par la base via seq_paiements)
    Public Property IdPaiement As Long

    ' Code technique du paiement (généré automatiquement, ex. 'PAI000012', non modifiable)
    Public Property CodePaiement As String

    ' Date du paiement (obligatoire)
    Public Property DatePaiement As Date

    ' Montant total du paiement (obligatoire)
    Public Property Montant As Decimal

    ' Mode de paiement (identifiant dans ref_modes_paiement) ; Nothing si non renseigné
    Public Property IdModePaiement As ULong?

    ' Référence externe du paiement (n° de virement, ticket…), optionnelle
    Public Property ReferencePaiement As String

    ' Commentaire libre, optionnel
    Public Property Commentaire As String

    ' Date de création du paiement (gérée par la base)
    Public Property DateCreation As DateTime

    ' Date de dernière modification du paiement (gérée par la base)
    Public Property DateModification As DateTime

#End Region

#Region "Propriétés — Liaison séance (V1 : 1 paiement ↔ 1 séance)"

    ' Identifiant de la séance à laquelle ce paiement est affecté (via lia_paiements_seances)
    Public Property IdSeance As Long

    ' Montant affecté à la séance (en V1, égal au montant total du paiement)
    Public Property MontantAffecte As Decimal

#End Region

End Class
