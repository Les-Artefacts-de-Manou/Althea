' -------------------------------------------------------------------------------------------------
' Classe   : Document
' Projet   : Althéa
' Version  : V1.0.1
' Date     : 24/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Représente un document tracé dans la table `documents`.
' Un document est rattaché à un niveau de la hiérarchie patient / dossier / séance :
'   - id_seance renseigné  -> document de niveau séance
'   - sinon id_dossier      -> document de niveau dossier
'   - sinon id_patient      -> document de niveau patient (ex. anamnèse)
'
' Remarques :
' - Cette classe ne contient aucun accès base de données.
' - Elle sert de modèle métier entre la couche DB (QueryDocuments / GestionDocuments) et l'UI.
' - id_document et code_document sont générés par la base : jamais fournis par l'application.
' - CheminRelatif est relatif à PATH_GENERAL (inclut le sous-dossier Documents) afin de rester
'   valable même si la racine de stockage change de machine.
' - Les colonnes Google (synchro Drive) et miniature restent vides à ce stade (Flows ultérieurs).
' - V1.0.1 (24/10/2026, audit C.3) : source et statut de synchronisation par défaut lus dans
'   CodesSysteme.SourcesDocument.
' -------------------------------------------------------------------------------------------------
Option Strict On
Option Explicit On

Public Class Document

#Region "Propriétés"

    ' Identifiant unique du document (clé primaire, généré par la base)
    Public Property IdDocument As ULong

    ' Rattachement hiérarchique (une seule règle : séance > dossier > patient)
    Public Property IdPatient As ULong?
    Public Property IdDossier As ULong?
    Public Property IdSeance As ULong?
    Public Property IdRendezVous As ULong?

    ' Type de document (FK ref_types_documents)
    Public Property IdTypeDocument As ULong

    ' Origine du document : 'PDF', 'LOCAL_DOCX', 'LOCAL', ...
    Public Property TypeSourceDocument As String = CodesSysteme.SourcesDocument.Local

    ' True si le document est une image métier (Flow 4, ultérieur)
    Public Property EstDocumentImageMetier As Boolean

    ' Modèle utilisé (FK modeles_documents) : non utilisé pour les exports simples
    Public Property IdModeleDocument As ULong?

    ' Libellé lisible du document (ex. "Anamnèse", "Motif du suivi")
    Public Property NomDocument As String

    ' Nom physique du fichier (horodaté)
    Public Property NomFichier As String

    ' Extension du fichier (ex. "pdf", "docx")
    Public Property ExtensionFichier As String

    ' Taille du fichier en octets (optionnel)
    Public Property TailleFichierOctets As Long?

    ' Chemin relatif à PATH_GENERAL (inclut Documents/...)
    Public Property CheminRelatif As String

    ' Chemin relatif du PDF dérivé (Flow ultérieur)
    Public Property CheminPdfRelatif As String

    ' Statut de synchronisation Google Drive (par défaut LOCAL_ONLY)
    Public Property StatutSyncGoogle As String = CodesSysteme.SourcesDocument.SyncLocaleSeulement

    ' Date métier du document (optionnel)
    Public Property DateDocument As Date?

    ' Commentaire libre (optionnel)
    Public Property Commentaire As String

#End Region

End Class
