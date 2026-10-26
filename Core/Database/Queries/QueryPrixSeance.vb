' -------------------------------------------------------------------------------------------------
' Module      : QueryPrixSeance
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table référentielle ref_prix_seance.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les tarifs de séance actifs ou tous les tarifs de séance
' - Vérifier l'unicité du code et du libellé d'un tarif de séance
' - Vérifier si un tarif de séance est utilisé (dossiers.id_prix_seance)
' - V1.1.0 (22/10/2026, audit C.1, étape 3) : lignes système protégées dans la requête elle-même
'   (code inchangé et ligne active pour la modifier, désactivation exclue) ; le métier refuse si
'   aucune ligne n'est concernée ; SupprimerPrixSeance, inutilisée(s), supprimée(s).
' - Mettre à jour un tarif de séance existant (avec montant)
' - Désactiver (soft-delete), supprimer et insérer un tarif de séance (avec montant)
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères), aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_prix_seance est généré par la base (AUTO_INCREMENT) : jamais fourni par l'application
' - PARTICULARITÉ : ref_prix_seance possède un champ montant decimal(10,2) NOT NULL
' - Le montant est FIGÉ sur la séance à sa création (seances.tarif_seance) : le modifier ici
'   n'affecte jamais les séances déjà comptées
'
' Dépendances :
' - Utilisé par GestionPrixSeance pour les opérations CRUD sur ref_prix_seance
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryPrixSeance

#Region "SELECT"

    Public ReadOnly Property SelectPrixSeanceActifs As String
        Get
            Return "
SELECT
    id_prix_seance,
    code_prix_seance,
    libelle_prix_seance,
    montant,
    est_systeme,
    actif,
    ordre_affichage
FROM ref_prix_seance
WHERE actif = 1
ORDER BY ordre_affichage, libelle_prix_seance;
"
        End Get
    End Property

    Public ReadOnly Property SelectPrixSeanceTous As String
        Get
            Return "
SELECT
    id_prix_seance,
    code_prix_seance,
    libelle_prix_seance,
    montant,
    est_systeme,
    actif,
    ordre_affichage
FROM ref_prix_seance
ORDER BY ordre_affichage, libelle_prix_seance;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodePrixSeance As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_prix_seance
WHERE code_prix_seance = @code
AND id_prix_seance <> @id;
"
        End Get
    End Property

    ' Tarifs actifs rattachés à un domaine donné (via lia_prix_seance_domaines).
    ' Utilisé pour filtrer les tarifs proposés dans SeanceEditor selon le domaine du dossier.
    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectPrixSeanceParDomaine
    '
    ' Paramètres SQL :
    ' - @id_domaine     : Domaine du dossier
    ' - @date_reference : Date de la séance, la liaison étant historisée depuis le lot 14
    '
    ' Remarques :
    ' - Sans le filtre de validité, un tarif conventionné périmé resterait proposé à la saisie.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectPrixSeanceParDomaine As String
        Get
            Return "
SELECT
    p.id_prix_seance,
    p.code_prix_seance,
    p.libelle_prix_seance,
    p.montant,
    p.est_systeme,
    p.actif,
    p.ordre_affichage
FROM ref_prix_seance p
INNER JOIN lia_prix_seance_domaines l ON l.id_prix_seance = p.id_prix_seance
WHERE p.actif = 1
  AND l.id_domaine = @id_domaine
  AND l.actif = 1
  AND l.date_debut_validite <= @date_reference
  AND (l.date_fin_validite IS NULL OR l.date_fin_validite >= @date_reference)
ORDER BY p.ordre_affichage, p.libelle_prix_seance;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibellePrixSeance As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_prix_seance
WHERE libelle_prix_seance = @libelle
AND id_prix_seance <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountUsagePrixSeance As String
        Get
            Return "
SELECT
    (SELECT COUNT(*) FROM dossiers WHERE id_prix_seance = @id) AS nb_usages;
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    Public ReadOnly Property UpdatePrixSeance As String
        Get
            Return "
        UPDATE ref_prix_seance
        SET code_prix_seance = @code,
            libelle_prix_seance = @libelle,
            montant = @montant,
            actif = @actif,
            ordre_affichage = @ordre
        WHERE id_prix_seance = @id
          AND (est_systeme = 0 OR (BINARY code_prix_seance = BINARY @code AND @actif = 1))"
        End Get
    End Property

    Public ReadOnly Property DesactiverPrixSeance As String
        Get
            Return "
        UPDATE ref_prix_seance
        SET actif = 0
        WHERE id_prix_seance = @id
        AND est_systeme = 0"
        End Get
    End Property

#End Region

#Region "DELETE"

#End Region

#Region "INSERT"

    Public ReadOnly Property InsertPrixSeance As String
        Get
            Return "
INSERT INTO ref_prix_seance
(
    code_prix_seance,
    libelle_prix_seance,
    montant,
    actif,
    ordre_affichage
)
VALUES
(
    @code,
    @libelle,
    @montant,
    @actif,
    @ordre
);"
        End Get
    End Property

#End Region

End Module
