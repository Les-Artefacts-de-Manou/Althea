' -------------------------------------------------------------------------------------------------
' Classe      : ApercuPatient
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 17/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Ce que montre le panneau d'aperçu de l'écran Patients pour le patient sélectionné : identité,
' alerte, dossiers, séances du jour, prochaines et dernières séances, finances, contacts.
'
' Remarques   :
' - Produit par GestionApercuPatient.GetApercuPatient (maquette « Aperçu patient », 17/10/2026).
' - Aucune donnée clinique au-delà de l'existant des listes : le texte des notes n'est pas montré
'   (décision de Joëlle : « pour lire, on ouvre la séance »), seulement leur présence.
' - Objet de lecture, sans accès base.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class ApercuPatient

    ' Fiche du patient (identité, naissance, photo, alerte, coordonnées)
    Public Property Patient As Patient

    ' Tous les dossiers du patient
    Public Property Dossiers As New List(Of DossierListeItem)

    ' Dossier montré comme suivi du patient (GestionDossiers.ChoisirDossierDominant) ; Nothing sans dossier
    Public Property DossierCourant As DossierListeItem

    ' Compteurs du quota INAMI du dossier courant (« 6/20 … ») ; vide hors domaine soumis à quota
    Public Property QuotaDossierCourant As String = String.Empty

    ' Séances dues d'aujourd'hui, passées ou non, par heure
    Public Property SeancesDuJour As New List(Of SeanceListeItem)

    ' Prochaines séances dues après aujourd'hui (les plus proches d'abord)
    Public Property ProchainesSeances As New List(Of SeanceListeItem)

    ' Dernières séances dues passées avant aujourd'hui (la plus récente d'abord)
    Public Property DernieresSeances As New List(Of SeanceListeItem)

    ' Entourage du patient (parents, responsables…)
    Public Property Contacts As New List(Of FamilleContact)

    ' Somme à régler maintenant, tous dossiers (mêmes règles que le bandeau du dossier)
    Public Property MontantARegler As Decimal

    ' Avoir du patient, tous dossiers
    Public Property Avoir As Decimal

    ' Nombre d'absences non excusées, tous dossiers
    Public Property NbAbsences As Integer

    ' Code d'un dossier du patient à partir de son identifiant (affichage des séances)
    Public Function CodeDossier(idDossier As Long) As String
        Dim dossier As DossierListeItem = Dossiers.FirstOrDefault(Function(d) d.IdDossier = idDossier)
        Return If(dossier Is Nothing, String.Empty, dossier.CodeDossier)
    End Function

End Class
