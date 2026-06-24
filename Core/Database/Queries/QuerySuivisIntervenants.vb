' -------------------------------------------------------------------------------------------------
' Module      : QuerySuivisIntervenants
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 15/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table autres_suivis_patient (réseau d'intervenants
' externes d'un patient, liaison N-N — D-Q1bis, Option A actée).
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les suivis d'un patient (avec le libellé du rôle) et un suivi par identifiant
' - Insérer, mettre à jour et supprimer un suivi
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_autre_suivi_patient et code_autre_suivi_patient sont générés par la base : jamais fournis
'   par l'application
' - Le commentaire est stocké en double format (commentaire_rtf + commentaire_txt) (D-Q7bis)
' - En V1, id_therapeute est laissé NULL (saisie texte libre via nom_professionnel)
'
' Dépendances :
' - Utilisé par GestionSuivisIntervenants pour les opérations CRUD sur autres_suivis_patient
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QuerySuivisIntervenants

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectSuivisParPatient
    ' Version    : V1.0.0
    ' Date       : 15/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour charger les intervenants d'un patient (avec le libellé du rôle)
    '
    ' Rôle       :
    ' Retourne tous les suivis rattachés à un patient, triés par date de début décroissante
    ' (les plus récents en premier) puis par nom professionnel.
    '
    ' Paramètres SQL :
    ' - @id_patient : Identifiant du patient
    '
    ' Utilisé par :
    ' - GestionSuivisIntervenants.GetSuivisParPatient
    '
    ' Remarques  :
    ' - Jointure sur ref_roles_intervenant pour exposer le libellé du rôle (affichage en liste)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectSuivisParPatient As String
        Get
            Return "
SELECT
    asp.id_autre_suivi_patient,
    asp.code_autre_suivi_patient,
    asp.id_patient,
    asp.id_role_intervenant,
    ri.libelle_role_intervenant,
    asp.id_therapeute,
    asp.nom_professionnel,
    asp.specialite,
    asp.lieu,
    asp.date_debut,
    asp.date_fin,
    asp.commentaire_rtf,
    asp.commentaire_txt,
    asp.date_creation,
    asp.date_modification
FROM autres_suivis_patient asp
LEFT JOIN ref_roles_intervenant ri ON ri.id_role_intervenant = asp.id_role_intervenant
WHERE asp.id_patient = @id_patient
ORDER BY asp.date_debut DESC, asp.nom_professionnel;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectSuiviById
    ' Version    : V1.0.0
    ' Date       : 15/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour charger un suivi par son identifiant
    '
    ' Rôle       :
    ' Retourne un suivi complet (avec le libellé du rôle) identifié par @id_autre_suivi_patient.
    '
    ' Paramètres SQL :
    ' - @id_autre_suivi_patient : Identifiant du suivi
    '
    ' Utilisé par :
    ' - GestionSuivisIntervenants.GetSuiviById
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectSuiviById As String
        Get
            Return "
SELECT
    asp.id_autre_suivi_patient,
    asp.code_autre_suivi_patient,
    asp.id_patient,
    asp.id_role_intervenant,
    ri.libelle_role_intervenant,
    asp.id_therapeute,
    asp.nom_professionnel,
    asp.specialite,
    asp.lieu,
    asp.date_debut,
    asp.date_fin,
    asp.commentaire_rtf,
    asp.commentaire_txt,
    asp.date_creation,
    asp.date_modification
FROM autres_suivis_patient asp
LEFT JOIN ref_roles_intervenant ri ON ri.id_role_intervenant = asp.id_role_intervenant
WHERE asp.id_autre_suivi_patient = @id_autre_suivi_patient
LIMIT 1;
"
        End Get
    End Property

#End Region

#Region "INSERT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : InsertSuivi
    ' Version    : V1.0.0
    ' Date       : 15/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour insérer un nouveau suivi
    '
    ' Rôle       :
    ' Insère une nouvelle ligne dans autres_suivis_patient ; id_autre_suivi_patient et
    ' code_autre_suivi_patient sont générés par la base.
    '
    ' Paramètres SQL :
    ' - @id_patient, @id_role_intervenant, @id_therapeute, @nom_professionnel, @specialite,
    '   @lieu, @date_debut, @date_fin, @commentaire_rtf, @commentaire_txt
    '
    ' Utilisé par :
    ' - GestionSuivisIntervenants.CreateSuivi
    '
    ' Remarques  :
    ' - date_creation et date_modification sont gérées par la base (DEFAULT current_timestamp())
    ' - L'ID généré est récupéré séparément via SELECT LASTVAL(seq_autres_suivis_patient)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertSuivi As String
        Get
            Return "
INSERT INTO autres_suivis_patient (
    id_patient,
    id_role_intervenant,
    id_therapeute,
    nom_professionnel,
    specialite,
    lieu,
    date_debut,
    date_fin,
    commentaire_rtf,
    commentaire_txt
) VALUES (
    @id_patient,
    @id_role_intervenant,
    @id_therapeute,
    @nom_professionnel,
    @specialite,
    @lieu,
    @date_debut,
    @date_fin,
    @commentaire_rtf,
    @commentaire_txt
);
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateSuivi
    ' Version    : V1.0.0
    ' Date       : 15/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour mettre à jour un suivi existant
    '
    ' Rôle       :
    ' Met à jour toutes les colonnes éditables d'un suivi identifié par @id_autre_suivi_patient.
    '
    ' Paramètres SQL :
    ' - Les mêmes que InsertSuivi, plus @id_autre_suivi_patient
    '
    ' Utilisé par :
    ' - GestionSuivisIntervenants.UpdateSuivi
    '
    ' Remarques  :
    ' - Ne modifie ni id_autre_suivi_patient ni code_autre_suivi_patient ni id_patient
    ' - date_modification est gérée par la base (ON UPDATE current_timestamp())
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateSuivi As String
        Get
            Return "
UPDATE autres_suivis_patient
SET
    id_role_intervenant = @id_role_intervenant,
    id_therapeute = @id_therapeute,
    nom_professionnel = @nom_professionnel,
    specialite = @specialite,
    lieu = @lieu,
    date_debut = @date_debut,
    date_fin = @date_fin,
    commentaire_rtf = @commentaire_rtf,
    commentaire_txt = @commentaire_txt
WHERE id_autre_suivi_patient = @id_autre_suivi_patient;
"
        End Get
    End Property

#End Region

#Region "DELETE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DeleteSuivi
    ' Version    : V1.0.0
    ' Date       : 15/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour supprimer physiquement un suivi
    '
    ' Rôle       :
    ' Supprime un suivi identifié par @id_autre_suivi_patient.
    '
    ' Paramètres SQL :
    ' - @id_autre_suivi_patient : Identifiant du suivi à supprimer
    '
    ' Utilisé par :
    ' - GestionSuivisIntervenants.DeleteSuivi
    '
    ' Remarques  :
    ' - Suppression physique (la table autres_suivis_patient ne porte pas d'indicateur d'activation)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DeleteSuivi As String
        Get
            Return "
DELETE FROM autres_suivis_patient
WHERE id_autre_suivi_patient = @id_autre_suivi_patient;
"
        End Get
    End Property

#End Region

End Module
