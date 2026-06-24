' -------------------------------------------------------------------------------------------------
' Module      : QueryParametres
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 02/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les requêtes SQL liées à la table tec_parametres.
'
' Responsabilités :
' - Fournir des propriétés ReadOnly contenant les requêtes SQL pour les opérations CRUD
' - Sélectionner les paramètres actifs ou tous les paramètres
' - Vérifier l'existence d'une clé de paramètre
' - Mettre à jour les paramètres applicatifs
' - Désactiver les paramètres (soft-delete)
' - Insérer de nouveaux paramètres
'
' Remarques   :
' - Contient uniquement du SQL (chaînes de caractères)
' - Aucun accès base de données ici
' - Module statique (propriétés ReadOnly partagées)
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
    ' Version    : V1.0.0
    ' Date       : 28/04/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour sélectionner les paramètres actifs de la table tec_parametres
    '
    ' Rôle       :
    ' Retourne tous les paramètres avec actif = 1, triés par groupe, ordre d'affichage et libellé.
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
    id_parametre,
    code_parametre,
    cle_parametre,
    libelle_parametre,
    groupe_parametre,
    type_valeur,
    valeur_parametre,
    description_parametre,
    modifiable_utilisateur,
    actif,
    ordre_affichage
FROM tec_parametres
WHERE actif = 1
ORDER BY groupe_parametre, ordre_affichage, libelle_parametre;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectParametresTous
    ' Version    : V1.0.0
    ' Date       : 28/04/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour sélectionner tous les paramètres de la table tec_parametres
    '
    ' Rôle       :
    ' Retourne tous les paramètres, actifs et inactifs, avec dates de création/modification.
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
    id_parametre,
    code_parametre,
    cle_parametre,
    libelle_parametre,
    groupe_parametre,
    type_valeur,
    valeur_parametre,
    description_parametre,
    modifiable_utilisateur,
    actif,
    ordre_affichage,
    date_creation,
    date_modification
FROM tec_parametres
ORDER BY groupe_parametre, ordre_affichage, libelle_parametre;
"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : SelectCountCleParametre
    ' Version    : V1.0.0
    ' Date       : 02/05/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour vérifier l'existence d'une clé de paramètre
    '
    ' Rôle       :
    ' Vérifie l'unicité d'une clé de paramètre, en excluant un identifiant donné.
    '
    ' Paramètres SQL :
    ' - @cle : Clé de paramètre à vérifier
    ' - @id  : Identifiant du paramètre à exclure (utile lors de la mise à jour)
    '
    ' Utilisé par :
    ' - GestionParametres.CleParametreExiste
    '
    ' Remarques  :
    ' - Permet de vérifier l'unicité de la clé lors de la création ou modification
    ' - Retourne un COUNT(*), 0 = clé disponible, >0 = clé déjà utilisée
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property SelectCountCleParametre As String
        Get
            Return "
SELECT COUNT(*)
FROM tec_parametres
WHERE cle_parametre = @cle
AND id_parametre <> @id;
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

#End Region

#Region "UPDATE"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : UpdateParametre
    ' Version    : V1.0.0
    ' Date       : 30/04/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour mettre à jour un paramètre applicatif existant
    '
    ' Rôle       :
    ' Met à jour les champs modifiables d'un paramètre identifié par son id.
    '
    ' Paramètres SQL :
    ' - @id          : Identifiant du paramètre à mettre à jour
    ' - @valeur      : Nouvelle valeur du paramètre
    ' - @libelle     : Nouveau libellé du paramètre
    ' - @groupe      : Nouveau groupe du paramètre
    ' - @type        : Nouveau type de valeur (Texte, Entier, Booléen, etc.)
    ' - @description : Nouvelle description du paramètre
    ' - @modifiable  : Indique si modifiable par l'utilisateur (1 ou 0)
    ' - @actif       : Indique si le paramètre est actif (1 ou 0)
    ' - @ordre       : Nouvel ordre d'affichage
    '

    ' Utilisé par :
    ' - GestionParametres.UpdateParametre
    '
    ' Remarques :
    ' - id_parametre sert d’identifiant technique.
    ' - code_parametre n’est jamais modifié.
    ' - cle_parametre n’est pas modifiée ici.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property UpdateParametre As String
        Get
            Return "
        UPDATE tec_parametres 
        SET valeur_parametre = @valeur,
            libelle_parametre = @libelle,
            groupe_parametre = @groupe,
            type_valeur = @type,
            description_parametre = @description,
            modifiable_utilisateur = @modifiable,
            actif = @actif,
            ordre_affichage = @ordre,
            date_modification = NOW()
        WHERE id_parametre = @id"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : DesactiverParametre
    ' Version    : V1.0.0
    ' Date       : 30/04/2026
    '
    ' Type       : String (ReadOnly)
    '
    ' Description : Requête SQL pour désactiver un paramètre applicatif (soft-delete)
    '
    ' Rôle       :
    ' Désactive un paramètre sans suppression physique en base de données.
    '
    ' Paramètres SQL :
    ' - @id : Identifiant du paramètre à désactiver
    '
    ' Utilisé par :
    ' - GestionParametres.DesactiverParametre
    '
    ' Remarques  :
    ' - Remplace toute suppression DELETE (soft-delete)
    ' - Le paramètre reste présent en base avec actif = 0
    ' - date_modification est automatiquement mise à jour avec NOW()
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property DesactiverParametre As String
        Get
            Return "
        UPDATE tec_parametres
        SET actif = 0,
            date_modification = NOW()
        WHERE id_parametre = @id"
        End Get
    End Property

#End Region

#Region "INSERT"

    ' -------------------------------------------------------------------------------------------------
    ' Requête   : InsertParametre
    ' Projet    : Althéa
    ' Version   : V1.0.0
    ' Date      : 30/04/2026
    ' Auteur    : Manou / Projet Althéa
    '
    ' Rôle      :
    ' Insère un nouveau paramètre applicatif.
    '
    ' Responsabilités :
    '- Retourne une requête SQL pour insérer un nouveau paramètre.

    ' Paramètres attendus :
    ' - @cle : la clé du paramètre
    ' - @libelle : le libellé du paramètre
    ' - @groupe : le groupe du paramètre
    ' - @type : le type de valeur du paramètre
    ' - @valeur : la valeur du paramètre
    ' - @description : la description du paramètre
    ' - @modifiable : indique si le paramètre est modifiable par l'utilisateur (1 ou 0)
    ' - @actif : indique si le paramètre est actif (1 ou 0)
    ' - @ordre : l'ordre d'affichage du paramètre
    '
    ' Utilisé par :
    ' - GestionParametres.InsertParametre
    '
    ' Remarques :
    ' - id_parametre est généré par la séquence DB.
    ' - code_parametre est généré automatiquement par la DB.
    ' - Aucune valeur id/code ne doit être fournie par l’application.
    ' -------------------------------------------------------------------------------------------------
    Public ReadOnly Property InsertParametre As String
        Get
            Return "
INSERT INTO tec_parametres
(
    cle_parametre,
    libelle_parametre,
    groupe_parametre,
    type_valeur,
    valeur_parametre,
    description_parametre,
    modifiable_utilisateur,
    actif,
    ordre_affichage
)
VALUES
(
    @cle,
    @libelle,
    @groupe,
    @type,
    @valeur,
    @description,
    @modifiable,
    @actif,
    @ordre
);"
        End Get
    End Property

#End Region

End Module