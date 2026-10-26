' -------------------------------------------------------------------------------------------------
' Classe      : CodesSysteme
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Point unique des codes techniques dont dépend le comportement d'Althéa (audit C.3, décision de
' Joëlle du 24/10/2026) : statuts de séance, de dossier et de paiement, types et sources de
' document. Le code, le SQL (Query*) et les écrans s'y réfèrent au lieu de répéter les chaînes.
'
' Remarques   :
' - Ces valeurs existent en base : lignes système des référentiels (est_systeme = 1 : code et
'   activation verrouillés, audit C.1) ou valeurs d'une colonne (paiements.statut_paiement,
'   documents.type_source_document, documents.statut_sync_google). Les changer ici sans migration
'   casserait l'application.
' - Une règle qui peut se dire par une case du référentiel (séance non tenue, statut proposé à la
'   création…) doit y être lue plutôt que tirée d'un code (C.3 étape 2).
' - Les familles déjà rangées chez leur propriétaire n'y figurent pas : usages des textes types
'   (UsagesTexteType), types d'organisme (TypeOrganisme), statuts des sauvegardes et restaurations
'   (StatutSauvegarde, StatutRestauration), types d'identifiant (UtilsIdentifiantBelge), clés de
'   paramètres (ClesParametres).
' - Classe et non Module : les familles portent des noms au pluriel, distincts des classes du
'   métier (StatutSeance, StatutDossier), et un Module rendrait ses membres visibles partout.
' - La vue v_seances_solde (v2.49.0) contient le code 'PAYE' en clair : elle est définie en base.
' - V1.1.0 (24/10/2026, audit C.3 étape 2, v2.57.0) : Absente, Malade, AnnuleePatient,
'   AnnuleePraticien et PrefixeAnnulee retirés : absence et « non tenue » se lisent dans le référentiel.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public NotInheritable Class CodesSysteme

    Private Sub New()
    End Sub

    ' Statuts de séance (ref_statuts_seance.code_statut_seance, lignes système) : ceux qu'un circuit
    ' désigne (nouvelle séance planifiée, report). Les autres se reconnaissent à leurs cases du
    ' référentiel : absence non excusée (est_absence, v2.53.0), non tenue (est_non_tenue) et proposé à
    ' la création (propose_creation, v2.57.0).
    Public NotInheritable Class StatutsSeance
        Private Sub New()
        End Sub
        Public Const Planifiee As String = "PLANIFIEE"
        Public Const Realisee As String = "REALISEE"
        Public Const Reportee As String = "REPORTEE"
    End Class

    ' Statuts de dossier (ref_statuts_dossier.code_statut_dossier, lignes système) : cycle de vie
    ' Actif ↔ En pause → Clôturé → Archivé (GestionDossiers).
    Public NotInheritable Class StatutsDossier
        Private Sub New()
        End Sub
        Public Const Actif As String = "ACTIF"
        Public Const Pause As String = "PAUSE"
        Public Const Cloture As String = "CLOTURE"
        Public Const Archive As String = "ARCHIVE"
    End Class

    ' Statuts d'un paiement (colonne paiements.statut_paiement, D-PAY-4) : dû non encaissé,
    ' encaissé, annulé (séance non due ou encaissement annulé).
    Public NotInheritable Class StatutsPaiement
        Private Sub New()
        End Sub
        Public Const EnAttente As String = "EN_ATTENTE"
        Public Const Paye As String = "PAYE"
        Public Const Annule As String = "ANNULE"
    End Class

    ' Types de document produits par l'application (ref_types_documents.code_type_document, lignes
    ' système).
    Public NotInheritable Class TypesDocument
        Private Sub New()
        End Sub
        Public Const Anamnese As String = "ANAMNESE"
        Public Const ZoneClinique As String = "ZONE_CLINIQUE"
        Public Const PhotoIdentite As String = "PHOTO_IDENTITE"
    End Class

    ' Origine d'un document (documents.type_source_document) et statut de synchronisation Google
    ' (documents.statut_sync_google). Le module Documents (audit C.6) pourra en faire des Enum.
    Public NotInheritable Class SourcesDocument
        Private Sub New()
        End Sub
        Public Const Local As String = "LOCAL"
        Public Const LocalDocx As String = "LOCAL_DOCX"
        Public Const Pdf As String = "PDF"
        Public Const SyncLocaleSeulement As String = "LOCAL_ONLY"
    End Class

End Class
