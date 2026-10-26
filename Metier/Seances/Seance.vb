' -------------------------------------------------------------------------------------------------
' Classe      : Seance
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Représente une séance de suivi clinique rattachée à un dossier, typée et statuée.
'
' Responsabilités :
' - Contenir les données d'identité de la séance (dossier, type, statut, date, horaires, tarif)
' - Porter les notes et l'évolution enrichies (RTF + texte brut)
' - Exposer les libellés joints (type de séance, statut) pour l'affichage
' - Fournir des helpers de présentation simples (état honoré / annulé)
'
' Table source : seances
'
' Important   :
' - Une séance appartient TOUJOURS à un dossier (seances.id_dossier NOT NULL).
' - Le tarif est figé sur la séance (tarif_seance) : pré-rempli depuis le tarif applicable
'   (ref_prix_seance via lia_prix_seance_domaines, selon la date de séance) à la création,
'   il survit au transfert de dossier (D-Q10).
' - Le statut initial d'une séance créée est « Planifiée » (ref_statuts_seance).
' - En Lot 2, la date et les horaires sont saisis à la main ; au Lot 3, un rendez-vous
'   (rendez_vous.id_seance) synchronisé avec Google Agenda pourra les alimenter.
' - Notes et évolution sont stockées en double format xxx_rtf / xxx_txt (ADR-15).
'
' Architecture :
' - Classe de données (DTO-like) sans logique métier complexe
' - Ne contient aucun accès base de données ni logique UI
'
' Dépendances :
' - Utilisée par GestionSeances (mapping depuis DataReader)
' - Utilisée par QuerySeances (requêtes SQL)
' - Utilisée par les UserControls/Forms séances (UC_DossierFiche, SeanceEditor)
'
' Historique :
' - V1.0.1 (20/10/2026) : codes des statuts de séance lus dans GestionSeances.CODE_STATUT_* (une seule définition ; audit B9).
' - V1.1.0 (24/10/2026, audit C.3) : EstRealisee, EstPlanifiee et EstAnnulee retirées (lues nulle
'   part ; EstAnnulee était l'une des trois définitions divergentes de la séance « non tenue »).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class Seance

#Region "Propriétés — Identité"

    ' Identifiant unique de la séance (clé primaire, généré par la base via seq_seances)
    Public Property IdSeance As Long

    ' Code technique de la séance (généré automatiquement, ex. 'SE000012', non modifiable)
    Public Property CodeSeance As String

    ' Identifiant du dossier auquel la séance est rattachée (FK dossiers, obligatoire)
    Public Property IdDossier As Long

    ' Identifiant du tarif de la séance (FK ref_prix_seance, obligatoire) — source du montant
    Public Property IdPrixSeance As Long

    ' Libellé du tarif de séance (non stocké : alimenté par jointure pour l'affichage)
    Public Property LibellePrixSeance As String

    ' Identifiant de la nature de séance (FK ref_natures_seance, obligatoire) — sans incidence prix
    Public Property IdNatureSeance As Long

    ' Libellé de la nature de séance (non stocké : alimenté par jointure pour l'affichage)
    Public Property LibelleNatureSeance As String

    ' Identifiant du statut courant de la séance (FK ref_statuts_seance, obligatoire)
    Public Property IdStatutSeance As Long

    ' Code du statut courant (non stocké : alimenté par jointure ; ex. 'PLANIFIEE', 'REALISEE')
    Public Property CodeStatutSeance As String

    ' Libellé du statut courant (non stocké : alimenté par jointure pour l'affichage)
    Public Property LibelleStatutSeance As String

    ' Identifiant de la séance dont celle-ci est issue par report (FK auto-référencée, NULL si séance ordinaire)
    Public Property IdSeanceOrigine As Long?

    ' Code de la séance d'origine (non stocké : alimenté par jointure pour l'affichage, ex. 'SE000036')
    Public Property CodeSeanceOrigine As String

    ' Date de la séance (obligatoire)
    Public Property DateSeance As Date

    ' Heure de début de la séance (optionnelle en Lot 2, alimentée par l'agenda au Lot 3)
    Public Property HeureDebut As TimeSpan?

    ' Heure de fin de la séance (optionnelle)
    Public Property HeureFin As TimeSpan?

    ' Durée de la séance en minutes (optionnelle)
    Public Property DureeMinutes As Integer?

    ' Tarif de la séance, figé à la création (pré-rempli depuis ref_prix_seance.montant)
    Public Property TarifSeance As Decimal

    ' Date de création de la séance (gérée par la base)
    Public Property DateCreation As DateTime

    ' Date de dernière modification de la séance (gérée par la base)
    Public Property DateModification As DateTime

#End Region

#Region "Propriétés — Notes & évolution (RTF + texte)"

    ' Notes de séance (RTF)
    Public Property NotesSeanceRtf As String

    ' Notes de séance (texte brut, pour recherche/affichage simple)
    Public Property NotesSeanceTxt As String

    ' Évolution clinique (RTF)
    Public Property EvolutionRtf As String

    ' Évolution clinique (texte brut)
    Public Property EvolutionTxt As String

#End Region

End Class
