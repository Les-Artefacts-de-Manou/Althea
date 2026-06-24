' -------------------------------------------------------------------------------------------------
' Module      : QueryFamilleContacts
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 13/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table famille_contacts (contacts de l'entourage patient).
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les contacts d'un patient (avec le libellé du lien) et un contact par identifiant
' - Insérer, mettre à jour et supprimer un contact
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - id_famille_contact et code_famille_contact sont générés par la base : jamais fournis par l'application
' - Le commentaire est stocké en double format (commentaire_rtf + commentaire_txt) (D-Q7bis)
'
' Dépendances :
' - Utilisé par GestionFamilleContacts pour les opérations CRUD sur famille_contacts
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryFamilleContacts

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectContactsParPatient
    ' Version    : V1.0.0
    ' Date       : 13/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour charger les contacts d'un patient (avec le libellé du lien)
    '
    ' Rôle       :
    ' Retourne tous les contacts rattachés à un patient, triés par ordre d'affichage puis nom/prénom.
    '
    ' Paramètres SQL :
    ' - @id_patient : Identifiant du patient
    '
    ' Utilisé par :
    ' - GestionFamilleContacts.GetContactsParPatient
    '
    ' Remarques  :
    ' - Jointure sur ref_liens_patient pour exposer le libellé du lien (affichage en liste)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectContactsParPatient As String
        Get
            Return "
SELECT
    fc.id_famille_contact,
    fc.code_famille_contact,
    fc.id_patient,
    fc.id_lien_patient,
    lp.libelle_lien_patient,
    fc.nom,
    fc.prenom,
    fc.date_naissance,
    fc.telephone,
    fc.email,
    fc.adresse_ligne1,
    fc.adresse_ligne2,
    fc.code_postal,
    fc.localite,
    fc.pays,
    fc.id_role_legal,
    rl.libelle_role_legal,
    fc.ordre_affichage,
    fc.commentaire_rtf,
    fc.commentaire_txt,
    fc.date_creation,
    fc.date_modification
FROM famille_contacts fc
LEFT JOIN ref_liens_patient lp ON lp.id_lien_patient = fc.id_lien_patient
LEFT JOIN ref_role_legal rl ON rl.id_role_legal = fc.id_role_legal
WHERE fc.id_patient = @id_patient
ORDER BY fc.ordre_affichage, fc.nom, fc.prenom;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectContactById
    ' Version    : V1.0.0
    ' Date       : 13/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour charger un contact par son identifiant
    '
    ' Rôle       :
    ' Retourne un contact complet (avec le libellé du lien) identifié par @id_famille_contact.
    '
    ' Paramètres SQL :
    ' - @id_famille_contact : Identifiant du contact
    '
    ' Utilisé par :
    ' - GestionFamilleContacts.GetContactById
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectContactById As String
        Get
            Return "
SELECT
    fc.id_famille_contact,
    fc.code_famille_contact,
    fc.id_patient,
    fc.id_lien_patient,
    lp.libelle_lien_patient,
    fc.nom,
    fc.prenom,
    fc.date_naissance,
    fc.telephone,
    fc.email,
    fc.adresse_ligne1,
    fc.adresse_ligne2,
    fc.code_postal,
    fc.localite,
    fc.pays,
    fc.id_role_legal,
    rl.libelle_role_legal,
    fc.ordre_affichage,
    fc.commentaire_rtf,
    fc.commentaire_txt,
    fc.date_creation,
    fc.date_modification
FROM famille_contacts fc
LEFT JOIN ref_liens_patient lp ON lp.id_lien_patient = fc.id_lien_patient
LEFT JOIN ref_role_legal rl ON rl.id_role_legal = fc.id_role_legal
WHERE fc.id_famille_contact = @id_famille_contact
LIMIT 1;
"
        End Get
    End Property

#End Region

#Region "INSERT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : InsertContact
    ' Version    : V1.0.0
    ' Date       : 13/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour insérer un nouveau contact
    '
    ' Rôle       :
    ' Insère une nouvelle ligne dans famille_contacts ; id_famille_contact et code_famille_contact
    ' sont générés par la base.
    '
    ' Paramètres SQL :
    ' - @id_patient, @id_lien_patient, @nom, @prenom, @date_naissance, @telephone, @email,
    '   @adresse_ligne1, @adresse_ligne2, @code_postal, @localite, @pays,
    '   @id_role_legal, @ordre_affichage, @commentaire_rtf, @commentaire_txt
    '
    ' Utilisé par :
    ' - GestionFamilleContacts.CreateContact
    '
    ' Remarques  :
    ' - date_creation et date_modification sont gérées par la base (DEFAULT current_timestamp())
    ' - L'ID généré est récupéré séparément via SELECT LASTVAL(seq_famille_contacts)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertContact As String
        Get
            Return "
INSERT INTO famille_contacts (
    id_patient,
    id_lien_patient,
    nom,
    prenom,
    date_naissance,
    telephone,
    email,
    adresse_ligne1,
    adresse_ligne2,
    code_postal,
    localite,
    pays,
    id_role_legal,
    ordre_affichage,
    commentaire_rtf,
    commentaire_txt
) VALUES (
    @id_patient,
    @id_lien_patient,
    @nom,
    @prenom,
    @date_naissance,
    @telephone,
    @email,
    @adresse_ligne1,
    @adresse_ligne2,
    @code_postal,
    @localite,
    @pays,
    @id_role_legal,
    @ordre_affichage,
    @commentaire_rtf,
    @commentaire_txt
);
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateContact
    ' Version    : V1.0.0
    ' Date       : 13/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour mettre à jour un contact existant
    '
    ' Rôle       :
    ' Met à jour toutes les colonnes éditables d'un contact identifié par @id_famille_contact.
    '
    ' Paramètres SQL :
    ' - Les mêmes que InsertContact, plus @id_famille_contact
    '
    ' Utilisé par :
    ' - GestionFamilleContacts.UpdateContact
    '
    ' Remarques  :
    ' - Ne modifie ni id_famille_contact ni code_famille_contact ni id_patient
    ' - date_modification est gérée par la base (ON UPDATE current_timestamp())
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateContact As String
        Get
            Return "
UPDATE famille_contacts
SET
    id_lien_patient = @id_lien_patient,
    nom = @nom,
    prenom = @prenom,
    date_naissance = @date_naissance,
    telephone = @telephone,
    email = @email,
    adresse_ligne1 = @adresse_ligne1,
    adresse_ligne2 = @adresse_ligne2,
    code_postal = @code_postal,
    localite = @localite,
    pays = @pays,
    id_role_legal = @id_role_legal,
    ordre_affichage = @ordre_affichage,
    commentaire_rtf = @commentaire_rtf,
    commentaire_txt = @commentaire_txt
WHERE id_famille_contact = @id_famille_contact;
"
        End Get
    End Property

#End Region

#Region "DELETE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DeleteContact
    ' Version    : V1.0.0
    ' Date       : 13/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour supprimer physiquement un contact
    '
    ' Rôle       :
    ' Supprime un contact identifié par @id_famille_contact.
    '
    ' Paramètres SQL :
    ' - @id_famille_contact : Identifiant du contact à supprimer
    '
    ' Utilisé par :
    ' - GestionFamilleContacts.DeleteContact
    '
    ' Remarques  :
    ' - Suppression physique (la table famille_contacts ne porte pas d'indicateur d'activation)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DeleteContact As String
        Get
            Return "
DELETE FROM famille_contacts
WHERE id_famille_contact = @id_famille_contact;
"
        End Get
    End Property

#End Region

End Module
