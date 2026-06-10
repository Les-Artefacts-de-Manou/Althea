' -------------------------------------------------------------------------------------------------
' UserControl : UC_Domaines
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 09/06/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Écran de gestion du référentiel des domaines de prise en charge (table ref_domaines).
' Hérite de UC_ReferentielBase et se contente de fournir les métadonnées d'affichage
' et de brancher la couche métier GestionDomaines via les points d'extension.
'
' Responsabilités :
' - Définir le titre, le sous-titre et le fil d'Ariane propres aux domaines
' - Charger les domaines via GestionDomaines et les convertir en ReferentielLigne
' - Déléguer les vérifications d'unicité (code, libellé) et le CRUD à GestionDomaines
'
' Remarques   :
' - Aucun Designer propre : tout le visuel provient de UC_ReferentielBase.
' - Aucun accès direct à la base de données : tout passe par GestionDomaines.
'
' Dépendances :
' - UC_ReferentielBase (classe de base héritée)
' - GestionDomaines (couche métier)
' - Domaine (modèle métier)
' - ReferentielLigne (modèle de présentation générique)
' - AppRole (gestion des droits)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_Domaines
    Inherits UC_ReferentielBase

#Region "Métadonnées"

    Protected Overrides ReadOnly Property TitreReferentiel As String
        Get
            Return "Domaines"
        End Get
    End Property

    Protected Overrides ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les domaines de prise en charge (psychologie, graphothérapie, ...)."
        End Get
    End Property

    Protected Overrides ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels > Domaines"
        End Get
    End Property

    Protected Overrides ReadOnly Property RoleMinimum As AppRole
        Get
            Return AppRole.SuperUser
        End Get
    End Property

    Protected Overrides ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 3
        End Get
    End Property

    Protected Overrides ReadOnly Property RemplacerEspacesParUnderscore As Boolean
        Get
            Return False
        End Get
    End Property

#End Region

#Region "Données - branchement GestionDomaines"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ChargerElements
    ' Rôle     : Charge les domaines via GestionDomaines et les convertit en ReferentielLigne.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)

        Dim domaines As List(Of Domaine) = GestionDomaines.GetDomaines(afficherInactifs)

        Return domaines.Select(
            Function(d) New ReferentielLigne With {
                .IdReferentiel = d.IdDomaine,
                .Code = d.CodeDomaine,
                .Libelle = d.LibelleDomaine,
                .OrdreAffichage = d.OrdreAffichage,
                .Actif = d.Actif
            }).ToList()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CodeExisteDeja
    ' Rôle     : Vérifie l'unicité du code via GestionDomaines.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return GestionDomaines.CodeDomaineExiste(code, idExclu)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LibelleExisteDeja
    ' Rôle     : Vérifie l'unicité du libellé via GestionDomaines.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return GestionDomaines.LibelleDomaineExiste(libelle, idExclu)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InsererElement
    ' Rôle      : Insère un nouveau domaine via GestionDomaines.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim dom As New Domaine With {
            .CodeDomaine = code,
            .LibelleDomaine = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionDomaines.InsertDomaine(dom)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourElement
    ' Rôle      : Met à jour un domaine existant via GestionDomaines.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)

        Dim dom As New Domaine With {
            .IdDomaine = id,
            .CodeDomaine = code,
            .LibelleDomaine = libelle,
            .OrdreAffichage = ordre,
            .Actif = actif
        }

        GestionDomaines.UpdateDomaine(dom)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirActivation
    ' Rôle      : Active ou désactive un domaine (soft-delete) via GestionDomaines.
    ' -------------------------------------------------------------------------------------------------
    Protected Overrides Sub DefinirActivation(id As ULong, actif As Boolean)

        If actif Then
            ' Réactivation : on repasse Actif à 1 en conservant les autres valeurs.
            Dim ligne As Domaine = GestionDomaines.GetDomaines(True).
                FirstOrDefault(Function(d) d.IdDomaine = id)

            If ligne IsNot Nothing Then
                ligne.Actif = True
                GestionDomaines.UpdateDomaine(ligne)
            End If
        Else
            ' Désactivation : soft-delete.
            GestionDomaines.DesactiverDomaine(id)
        End If

    End Sub

#End Region

End Class
