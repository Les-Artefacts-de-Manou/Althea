' -------------------------------------------------------------------------------------------------
' Module      : QueryModesPaiement
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table référentielle ref_modes_paiement.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les modes de paiement actifs ou tous les modes de paiement
' - Vérifier l'unicité du code et du libellé d'un mode de paiement
' - Mettre à jour, désactiver (soft-delete), supprimer et insérer un mode de paiement
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères), aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_mode_paiement est généré par la base (AUTO_INCREMENT) : jamais fourni par l'application
' - paiements.id_mode_paiement est une clé étrangère (v2.50.0) : la base refuse la suppression d'un
'   mode utilisé ; il se désactive.
'
' Historique  :
' - V1.1.0 (21/10/2026, audit B.5) : SelectCountUsageModePaiement supprimée — elle cherchait le code
'   alors que paiements.mode_paiement contenait le libellé (jamais d'usage trouvé), et n'était appelée
'   par aucun écran.
' - V1.2.0 (22/10/2026, audit C.1, étape 3) : lignes système protégées dans la requête elle-même
'   (code inchangé et ligne active pour la modifier, désactivation exclue) ; le métier refuse si
'   aucune ligne n'est concernée ; SupprimerModePaiement, inutilisée(s), supprimée(s).
'
' Dépendances :
' - Utilisé par GestionModesPaiement pour les opérations CRUD sur ref_modes_paiement
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryModesPaiement

#Region "SELECT"

    Public ReadOnly Property SelectModesPaiementActifs As String
        Get
            Return "
SELECT
    id_mode_paiement,
    code_mode_paiement,
    libelle_mode_paiement,
    est_avoir,
    est_systeme,
    actif,
    ordre_affichage
FROM ref_modes_paiement
WHERE actif = 1
ORDER BY ordre_affichage, libelle_mode_paiement;
"
        End Get
    End Property

    Public ReadOnly Property SelectModesPaiementTous As String
        Get
            Return "
SELECT
    id_mode_paiement,
    code_mode_paiement,
    libelle_mode_paiement,
    est_avoir,
    est_systeme,
    actif,
    ordre_affichage
FROM ref_modes_paiement
ORDER BY ordre_affichage, libelle_mode_paiement;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountCodeModePaiement As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_modes_paiement
WHERE code_mode_paiement = @code
AND id_mode_paiement <> @id;
"
        End Get
    End Property

    Public ReadOnly Property SelectCountLibelleModePaiement As String
        Get
            Return "
SELECT COUNT(*)
FROM ref_modes_paiement
WHERE libelle_mode_paiement = @libelle
AND id_mode_paiement <> @id;
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    Public ReadOnly Property UpdateModePaiement As String
        Get
            Return "
        UPDATE ref_modes_paiement
        SET code_mode_paiement = @code,
            libelle_mode_paiement = @libelle,
            est_avoir = @est_avoir,
            actif = @actif,
            ordre_affichage = @ordre
        WHERE id_mode_paiement = @id
          AND (est_systeme = 0 OR (BINARY code_mode_paiement = BINARY @code AND @actif = 1))"
        End Get
    End Property

    Public ReadOnly Property DesactiverModePaiement As String
        Get
            Return "
        UPDATE ref_modes_paiement
        SET actif = 0
        WHERE id_mode_paiement = @id
        AND est_systeme = 0"
        End Get
    End Property

#End Region

#Region "DELETE"

#End Region

#Region "INSERT"

    Public ReadOnly Property InsertModePaiement As String
        Get
            Return "
INSERT INTO ref_modes_paiement
(
    code_mode_paiement,
    libelle_mode_paiement,
    est_avoir,
    actif,
    ordre_affichage
)
VALUES
(
    @code,
    @libelle,
    @est_avoir,
    @actif,
    @ordre
);"
        End Get
    End Property

#End Region

End Module
