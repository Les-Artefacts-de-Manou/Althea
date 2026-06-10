' -------------------------------------------------------------------------------------------------
' UserControl : UC_LiensPatient
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des liens de parenté/relation avec le patient
' (table ref_liens_patient). Hérite de UC_ReferentielBase et se contente de fournir les
' métadonnées d'affichage et de brancher la couche métier GestionLiensPatient via les
' points d'extension.
'
' Responsabilités :
' - Définir le titre, le sous-titre et le fil d'Ariane propres aux liens patient
' - Charger les liens patient via GestionLiensPatient et les convertir en ReferentielLigne
' - Déléguer les vérifications d'unicité (code, libellé) et le CRUD à GestionLiensPatient
'
' Remarques   :
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Aucun accès direct à la base de données : tout passe par GestionLiensPatient.
' - Le code peut être long (varchar 50) avec des espaces remplacés automatiquement par des _
'   (comportement par défaut de UC_ReferentielBase, donc aucune surcharge nécessaire ici).
'
' Dépendances :
' - UC_ReferentielBase (classe de base héritée)
' - GestionLiensPatient (couche métier)
' - LienPatient (modèle métier)
' - ReferentielLigne (modèle de présentation générique)
' - AppRole (gestion des droits)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_LiensPatient
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Liens patient"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les liens de parenté/relation avec le patient (mère, père, tuteur légal, ...)."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Liens patient"
        End Get
    End Property

    Protected Overrides ReadOnly Property RoleMinimum As AppRole
        Get
            Return AppRole.SuperUser
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 50
        End Get
    End Property

#End Region

#Region "Données - branchement GestionLiensPatient"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ChargerElements
    ' Rôle     : Charge les liens patient via GestionLiensPatient et les convertit en ReferentielLigne.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim liens As List(Of LienPatient) = GestionLiensPatient.GetLiensPatient(afficherInactifs)

        Return liens.Select(
            Function(l) New ReferentielLigne With {
                .IdReferentiel = l.IdLienPatient,
                .Code = l.CodeLienPatient,
                .Libelle = l.LibelleLienPatient,
                .OrdreAffichage = l.OrdreAffichage,
                .Actif = l.Actif
            }).ToList()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CodeExisteDeja
    ' Rôle     : Vérifie l'unicité du code via GestionLiensPatient.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionLiensPatient.CodeLienPatientExiste(code, idExclu)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LibelleExisteDeja
    ' Rôle     : Vérifie l'unicité du libellé via GestionLiensPatient.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionLiensPatient.LibelleLienPatientExiste(libelle, idExclu)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InsererElement
    ' Rôle      : Insère un nouveau lien patient via GestionLiensPatient.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim lien As New LienPatient With {
            .CodeLienPatient = code,
            .LibelleLienPatient = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionLiensPatient.InsertLienPatient(lien)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourElement
    ' Rôle      : Met à jour un lien patient existant via GestionLiensPatient.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim lien As New LienPatient With {
            .IdLienPatient = id,
            .CodeLienPatient = code,
            .LibelleLienPatient = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionLiensPatient.UpdateLienPatient(lien)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirActivation
    ' Rôle      : Active ou désactive un lien patient (soft-delete) via GestionLiensPatient.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            ' Réactivation : on repasse Actif à 1 en conservant les autres valeurs.
            Dim ligne As LienPatient = GestionLiensPatient.GetLiensPatient(True).
                FirstOrDefault(Function(l) l.IdLienPatient = id)

            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionLiensPatient.UpdateLienPatient(ligne)
            End If
        Else
            ' Désactivation : soft-delete.
            GestionLiensPatient.DesactiverLienPatient(id)
        End If

    End Sub

#End Region

End Class
