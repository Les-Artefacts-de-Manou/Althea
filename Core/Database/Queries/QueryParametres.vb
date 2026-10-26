' -------------------------------------------------------------------------------------------------
' Module      : QueryParametres
' Projet      : Althéa
' Version     : V1.4.0
' Date        : 16/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table tec_parametres.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL sur tec_parametres
' - Sélectionner les paramètres actifs ou tous les paramètres ; lire une valeur (et ses bornes)
' - Mettre à jour la valeur d'un paramètre (seule écriture : les paramètres naissent par
'   migration, ADR-36)
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
' - V1.4.0 (16/10/2026) : les listes joignent tec_groupes_parametres (titre et ordre du groupe,
'   migration v2.40.0) et sont triées par ordre de groupe ; mise à jour unitaire de la valeur
'   remplacée par UpdateValeurParametreModifiable, utilisée dans une transaction.
'
' Dépendances :
' - Utilisé par GestionParametres pour les opérations CRUD sur tec_parametres
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryParametres

#Region "SELECT"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectParametresActifs
    ' Version    : V1.1.0
    ' Date       : 16/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour sélectionner les paramètres actifs de la table tec_parametres
    '
    ' Rôle       :
    ' Retourne tous les paramètres avec actif = 1, avec le titre et l'ordre de leur groupe, triés par
    ' ordre de groupe, ordre d'affichage et libellé.
    '
    ' Paramètres SQL :
    ' - Aucun
    '
    ' Utilisé par :
    ' - GestionParametres.GetParametresActifs
    '
    ' Remarques  :
    ' - Le filtrage Admin / SuperUser est appliqué dans la couche métier
    ' - Les paramètres inactifs ne sont pas retournés
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectParametresActifs As String
        Get
            Return "
SELECT
    p.id_parametre,
    p.code_parametre,
    p.cle_parametre,
    p.libelle_parametre,
    p.groupe_parametre,
    g.libelle_groupe,
    g.ordre_affichage AS ordre_groupe,
    p.type_valeur,
    p.valeur_parametre,
    p.valeur_min,
    p.valeur_max,
    p.description_parametre,
    p.modifiable_utilisateur,
    p.actif,
    p.ordre_affichage
FROM tec_parametres p
INNER JOIN tec_groupes_parametres g ON g.code_groupe = p.groupe_parametre
WHERE p.actif = 1
ORDER BY g.ordre_affichage, p.ordre_affichage, p.libelle_parametre;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectParametresTous
    ' Version    : V1.1.0
    ' Date       : 16/10/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour sélectionner tous les paramètres de la table tec_parametres
    '
    ' Rôle       :
    ' Retourne tous les paramètres, actifs et inactifs, avec le titre et l'ordre de leur groupe.
    '
    ' Paramètres SQL :
    ' - Aucun
    '
    ' Utilisé par :
    ' - GestionParametres.GetParametres
    '
    ' Remarques  :
    ' - Le filtrage Admin / SuperUser est appliqué dans la couche métier
    ' - Inclut les paramètres inactifs (actif = 0)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectParametresTous As String
        Get
            Return "
SELECT
    p.id_parametre,
    p.code_parametre,
    p.cle_parametre,
    p.libelle_parametre,
    p.groupe_parametre,
    g.libelle_groupe,
    g.ordre_affichage AS ordre_groupe,
    p.type_valeur,
    p.valeur_parametre,
    p.valeur_min,
    p.valeur_max,
    p.description_parametre,
    p.modifiable_utilisateur,
    p.actif,
    p.ordre_affichage
FROM tec_parametres p
INNER JOIN tec_groupes_parametres g ON g.code_groupe = p.groupe_parametre
ORDER BY g.ordre_affichage, p.ordre_affichage, p.libelle_parametre;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectValeurParametreByCle
    ' Version    : V1.0.0
    ' Date       : 11/06/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour lire la valeur d'un paramètre actif identifié par sa clé
    '
    ' Rôle       :
    ' Retourne la valeur (valeur_parametre) du paramètre actif correspondant à la clé fournie.
    '
    ' Paramètres SQL :
    ' - @cle : Clé unique du paramètre à lire (cle_parametre)
    '
    ' Utilisé par :
    ' - GestionParametres.GetValeurParametre
    '
    ' Remarques  :
    ' - Filtre sur actif = 1 (un paramètre désactivé est considéré comme absent)
    ' - Retourne une seule valeur scalaire (ExecuteScalar), Nothing si la clé n'existe pas
    ' - Utilisé pour l'accès programmatique aux paramètres (ex : racine de stockage documents)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectValeurParametreByCle As String
        Get
            Return "
SELECT valeur_parametre
FROM tec_parametres
WHERE cle_parametre = @cle
AND actif = 1;
"
        End Get
    End Property

    ' Lecture typée (GestionParametres.GetEntier) : valeur et bornes en une requête.
    Public ReadOnly Property SelectValeurEtBornesByCle As String
        Get
            Return "
SELECT valeur_parametre, valeur_min, valeur_max
FROM tec_parametres
WHERE cle_parametre = @cle
AND actif = 1;
"
        End Get
    End Property

#End Region

#Region "UPDATE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateValeurParametreModifiable
    ' Version    : V1.1.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Met à jour la seule valeur d'un paramètre, si le mode d'accès l'autorise
    ' (GestionParametres.UpdateValeursParametres, une commande par paramètre dans une transaction).
    '
    ' Paramètres SQL :
    ' - @id     : identifiant du paramètre
    ' - @valeur : nouvelle valeur
    ' - @admin  : 1 = Admin (tous les paramètres), 0 = SuperUser (paramètres ouverts seulement)
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateValeurParametreModifiable As String
        Get
            Return "
UPDATE tec_parametres
SET valeur_parametre = @valeur,
    date_modification = NOW()
WHERE id_parametre = @id
AND (@admin = 1 OR modifiable_utilisateur = 1);
"
        End Get
    End Property

    ' Même mise à jour, par clé (paramètre actif) : réglage depuis un écran métier (ex. dossier
    ' des sauvegardes choisi dans l'écran Sauvegardes).
    Public ReadOnly Property UpdateValeurParametreParCle As String
        Get
            Return "
UPDATE tec_parametres
SET valeur_parametre = @valeur,
    date_modification = NOW()
WHERE cle_parametre = @cle
AND actif = 1
AND (@admin = 1 OR modifiable_utilisateur = 1);
"
        End Get
    End Property

#End Region

End Module