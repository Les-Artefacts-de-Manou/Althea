' -------------------------------------------------------------------------------------------------
' Module      : QueryDocuments
' Projet      : Althéa
' Version     : V1.0.1
' Date        : 20/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table `documents` (traçabilité documentaire).
'
' Responsabilités :
' - Fournir la requête d'insertion d'une trace de document (export ou import)
' - Résoudre l'identifiant d'un type de document à partir de son code référentiel
'
' Important   :
' - Contient uniquement du SQL (chaînes de caractères), aucun accès base ici
' - Module statique (propriétés ReadOnly partagées)
' - id_document et code_document sont générés par la base : jamais fournis par l'application
' - Le niveau (patient / dossier / séance) est porté par les colonnes id_patient / id_dossier /
'   id_seance ; une seule est renseignée selon la règle applicative (séance > dossier > patient)
'
' - V1.0.1 (20/10/2026) : identifiant du document lu par LASTVAL(seq_documents) (SelectDernierIdDocument) :
'   id_document vient d'une séquence, LAST_INSERT_ID() rendait toujours 0 (audit B5).
'
' Dépendances :
' - Utilisé par GestionDocuments pour tracer les documents
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryDocuments

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectIdTypeDocumentParCode
    ' Version    : V1.1.0
    ' Date       : 09/06/2026
    '
    ' Rôle       :
    ' Retourne la requête qui résout l’identifiant d’un type de document à partir de son code.
    '
    ' Paramètres SQL :
    ' - @code : Code du type de document recherché.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectIdTypeDocumentParCode As String
        Get
            Return "
SELECT id_type_document
FROM ref_types_documents
WHERE code_type_document = @code
LIMIT 1;
"
        End Get
    End Property

    ' Recherche un document existant par son chemin relatif (clé d'unicité naturelle d'un
    ' document exporté déterministe : un seul fichier par contexte + type + extension).
    Public ReadOnly Property SelectIdDocumentParCheminRelatif As String
        Get
            Return "
SELECT id_document
FROM documents
WHERE chemin_relatif = @chemin_relatif
LIMIT 1;
"
        End Get
    End Property

    ' Recherche un document existant par (patient + type). Utilisé pour les documents uniques par
    ' contexte dont le nom de fichier peut varier d'extension (ex. photo d'identité : identite.jpg
    ' remplacée par identite.png). Garantit une seule ligne quel que soit le format.
    Public ReadOnly Property SelectIdDocumentParPatientEtType As String
        Get
            Return "
SELECT id_document
FROM documents
WHERE id_patient = @id_patient
  AND id_type_document = @id_type_document
LIMIT 1;
"
        End Get
    End Property

#End Region

#Region "INSERT"

    Public ReadOnly Property InsertDocument As String
        Get
            Return "
INSERT INTO documents
(
    id_dossier,
    id_patient,
    id_seance,
    id_rendez_vous,
    id_type_document,
    type_source_document,
    est_document_image_metier,
    id_modele_document,
    nom_document,
    nom_fichier,
    extension_fichier,
    taille_fichier_octets,
    chemin_relatif,
    statut_sync_google,
    date_document,
    commentaire
)
VALUES
(
    @id_dossier,
    @id_patient,
    @id_seance,
    @id_rendez_vous,
    @id_type_document,
    @type_source_document,
    @est_document_image_metier,
    @id_modele_document,
    @nom_document,
    @nom_fichier,
    @extension_fichier,
    @taille_fichier_octets,
    @chemin_relatif,
    @statut_sync_google,
    @date_document,
    @commentaire
);
"
        End Get
    End Property

    ' Identifiant du document qui vient d'être inséré : à exécuter sur la MÊME connexion que
    ' l'insertion (LASTVAL est propre à la session).
    Public ReadOnly Property SelectDernierIdDocument As String
        Get
            Return "SELECT LASTVAL(seq_documents);"
        End Get
    End Property

#End Region

#Region "UPDATE"

    ' Met à jour un document exporté existant (identifié par son id_document).
    ' Les documents exportés sont uniques par contexte : on écrase la version précédente et on
    ' rafraîchit la source, l'extension, la taille, la date métier et la date de modification.
    Public ReadOnly Property UpdateDocument As String
        Get
            Return "
UPDATE documents
SET type_source_document = @type_source_document,
    nom_document = @nom_document,
    nom_fichier = @nom_fichier,
    extension_fichier = @extension_fichier,
    taille_fichier_octets = @taille_fichier_octets,
    chemin_relatif = @chemin_relatif,
    date_document = @date_document,
    date_modification = NOW()
WHERE id_document = @id_document;
"
        End Get
    End Property

#End Region

End Module
