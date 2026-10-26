' -------------------------------------------------------------------------------------------------
' Module      : QueryNaturesSeance
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table référentielle ref_natures_seance.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les natures de séance actives ou toutes les natures de séance
' - Vérifier l'unicité du code et du libellé d'une nature de séance
' - Vérifier si une nature de séance est utilisée (seances.id_nature_seance)
' - Mettre à jour, désactiver (soft-delete), supprimer et insérer une nature de séance
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères), aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_nature_seance est généré par la base (AUTO_INCREMENT) : jamais fourni par l'application
' - PARTICULARITÉ : la nature de séance n'a AUCUNE incidence sur le prix
' - V1.0.1 (20/10/2026) : SelectNaturesSeanceToutes se terminait par un ORDER BY vide (erreur SQL
'   avec « Afficher les inactifs » ; audit B4).
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : SupprimerNatureSeance, plus appelée par le métier,
'   supprimée.
'
' Dépendances :
' - Utilisé par GestionNaturesSeance pour les opérations CRUD sur ref_natures_seance
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryNaturesSeance

#Region "SELECT"

    Public ReadOnly Property SelectNaturesSeanceActives As String
        Get
            Return "
SELECT
    id_nature_seance,
    code_nature_seance,
    libelle_nature_seance,
    actif,
    ordre_affichage
FROM ref_natures_seance
WHERE actif = 1
ORDER BY ordre_affichage, libelle_nature_seance;
"
        End Get
    End Property

    Public ReadOnly Property SelectNaturesSeanceToutes As String
        Get
            Return "
SELECT
    id_nature_seance,
    code_nature_seance,
    libelle_nature_seance,
    actif,
    ordre_affichage
FROM ref_natures_seance
ORDER BY ordre_affichage, libelle_nature_seance;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : SelectNaturesSeanceParDomaine
    ' Version   : V1.0.0
    ' Date      : 19/09/2026
    '
    ' Type      : String (ReadOnly)
    '
    ' Description : Natures de séance actives rattachées à un domaine (lia_natures_seance_domaines)
    '
    ' Paramètres SQL :
    ' - @id_domaine : Domaine du dossier
    '
    ' Remarques :
    ' - Remplace le filtrage par « code_prestation_inami non vide », qui ne permettait pas de
    '   distinguer deux domaines conventionnés aux nomenclatures différentes (lot 14).
    '
    ' Utilisé par :
    ' - GestionNaturesSeance.GetNaturesSeanceParDomaine
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectNaturesSeanceParDomaine As String
        Get
            Return "
SELECT
    ns.id_nature_seance,
    ns.code_nature_seance,
    ns.libelle_nature_seance,
    ns.actif,
    ns.ordre_affichage
FROM ref_natures_seance ns
INNER JOIN lia_natures_seance_domaines lia ON lia.id_nature_seance = ns.id_nature_seance
WHERE lia.id_domaine = @id_domaine
  AND lia.actif = 1
  AND ns.actif = 1
ORDER BY ns.ordre_affichage, ns.libelle_nature_seance;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeNatureSeance As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_natures_seance
WHERE code_nature_seance = @code
AND id_nature_seance <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleNatureSeance As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_natures_seance
WHERE libelle_nature_seance = @libelle
AND id_nature_seance <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountUsageNatureSeance As String
        Get
            Return "
SELECT COUNT(*) FROM seances WHERE id_nature_seance = @id;
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    Public ReadOnly Property UpdateNatureSeance As String
        Get
            Return "
        UPDATE ref_natures_seance
        SET code_nature_seance = @code,
            libelle_nature_seance = @libelle,
            actif = @actif,
            ordre_affichage = @ordre
        WHERE id_nature_seance = @id"
        End Get
    End Property

    Public ReadOnly Property DesactiverNatureSeance As String
        Get
            Return "
        UPDATE ref_natures_seance
        SET actif = 0
        WHERE id_nature_seance = @id"
        End Get
    End Property

#End Region

#Region "DELETE"

#End Region

#Region "INSERT"

    Public ReadOnly Property InsertNatureSeance As String
        Get
            Return "
INSERT INTO ref_natures_seance
(
    code_nature_seance,
    libelle_nature_seance,
    actif,
    ordre_affichage
)
VALUES
(
    @code,
    @libelle,
    @actif,
    @ordre
);"
        End Get
    End Property

#End Region

End Module
