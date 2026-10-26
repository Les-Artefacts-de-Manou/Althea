' -------------------------------------------------------------------------------------------------
' Classe   : Cabinet / Praticien / PraticienIdentifiant / PraticienQualification / TypeIdentifiantPro
' Projet   : Althéa
' Version  : V1.3.1
' Date     : 25/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Modèles de la fondation Cabinet (lot 27, schéma 2.29.0).
'
' Remarques :
' - Aucune donnée propre à un cabinet ou un praticien n'est codée en dur.
' - Les chemins d'images sont relatifs au stockage applicatif (comme la photo patient).
' - V1 : un cabinet, un praticien ; le modèle reste multi-praticiens.
' - V1.2.0 (20/10/2026) : TypeIdentifiantPro.CIBLE_PRATICIEN / CIBLE_CABINET (valeurs de la colonne
'   cible, contrainte chk_ref_types_identifiant_pro_cible).
' - V1.3.0 (24/10/2026) : code du compte (USR…) : Praticien.UtilisateurCode (rappel du compte lié) et
'   UtilisateurLiable.CodeUtilisateur, cité après le nom dans la liste des comptes à lier (code
'   toujours visible).
' - V1.3.1 (25/10/2026, audit D-1) : Option Strict On.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class Cabinet
    Public Property IdCabinet As ULong
    Public Property NomCabinet As String

    ' Situation juridique (ref_situations_juridiques) ; 0 = non renseignée. Détermine le porteur du BCE.
    Public Property IdSituationJuridique As ULong
    Public Property AdresseRue As String
    Public Property AdresseNumero As String
    Public Property CodePostal As String
    Public Property Localite As String

    ' Pays (ref_pays) ; 0 = non renseigné. CodePays / LibellePays : lecture seule (sous-requête).
    Public Property IdPays As ULong
    Public Property CodePays As String
    Public Property LibellePays As String
    Public Property Telephone As String
    Public Property Email As String
    Public Property SiteWeb As String
    Public Property NumeroBce As String
    Public Property NumeroTva As String
    Public Property Iban As String
    Public Property Bic As String
    Public Property CheminLogo As String
    Public Property Actif As Boolean = True
End Class

''' <summary>Compte utilisateur candidat à la liaison avec un praticien (projection légère de sec_utilisateurs).</summary>
Public Class UtilisateurLiable
    Public Property IdUtilisateur As ULong
    Public Property CodeUtilisateur As String = String.Empty
    Public Property Login As String
    Public Property NomAffichage As String
    Public Property Role As String

    ' Libellé de la liste des comptes : on choisit par le nom, le code suit (Rules § 15).
    Public Overrides Function ToString() As String
        Dim libelle As String = $"{NomAffichage} ({Login})"
        Return If(String.IsNullOrWhiteSpace(CodeUtilisateur), libelle, $"{libelle} · {CodeUtilisateur}")
    End Function
End Class

Public Class Praticien
    Public Property IdPraticien As ULong
    Public Property IdCabinet As ULong

    ' Civilité (ref_civilites) ; 0 = non renseignée.
    Public Property IdCivilite As ULong
    Public Property Nom As String
    Public Property Prenom As String
    Public Property TitreProfessionnel As String
    Public Property Telephone As String
    Public Property Email As String
    Public Property SiteWeb As String
    Public Property AdresseRue As String
    Public Property AdresseNumero As String
    Public Property CodePostal As String
    Public Property Localite As String

    ' Pays (ref_pays) ; 0 = non renseigné (adresse vide = adresse du cabinet). Code / libellé en lecture seule.
    Public Property IdPays As ULong
    Public Property CodePays As String
    Public Property LibellePays As String
    Public Property CheminPhoto As String
    Public Property CheminCachet As String
    Public Property CheminSignature As String
    Public Property Actif As Boolean = True

    ' Compte de travail lié (sec_utilisateurs) ; 0 = non lié. Les champs Utilisateur* sont en lecture seule (jointure).
    Public Property IdUtilisateur As ULong
    Public Property UtilisateurCode As String
    Public Property UtilisateurLogin As String
    Public Property UtilisateurNomAffichage As String
    Public Property UtilisateurRole As String
    Public Property UtilisateurActif As Boolean

    Public ReadOnly Property ACompteLie As Boolean
        Get
            Return IdUtilisateur > 0UL
        End Get
    End Property

    Public ReadOnly Property NomComplet As String
        Get
            Return $"{Prenom} {Nom}".Trim()
        End Get
    End Property
End Class

Public Class TypeIdentifiantPro

    ' Qui porte le numéro (colonne cible) : chaque praticien, ou l'entité qui facture.
    Public Const CIBLE_PRATICIEN As String = "PRATICIEN"
    Public Const CIBLE_CABINET As String = "CABINET"

    Public Property IdTypeIdentifiant As ULong
    Public Property CodeTypeIdentifiant As String
    Public Property LibelleTypeIdentifiant As String
    Public Property MasqueSaisie As String
    Public Property RegexValidation As String
    Public Property Cible As String = CIBLE_PRATICIEN
    Public Property EstSysteme As Boolean
    Public Property Actif As Boolean
    Public Property OrdreAffichage As Integer
End Class

Public Class PraticienIdentifiant
    Public Property IdIdentifiant As ULong
    Public Property IdPraticien As ULong
    Public Property IdTypeIdentifiant As ULong
    Public Property LibelleType As String
    Public Property CodeType As String
    Public Property Valeur As String
    Public Property DateDebut As Date?
    Public Property DateFin As Date?
    Public Property AfficherSurCachet As Boolean = True
End Class

Public Class PraticienQualification
    Public Property IdQualification As ULong
    Public Property IdPraticien As ULong
    Public Property Libelle As String
    Public Property Etablissement As String
    Public Property AnneeObtention As Integer?
    Public Property AfficherSurCachet As Boolean
    Public Property OrdreAffichage As Integer
End Class
