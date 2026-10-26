' -------------------------------------------------------------------------------------------------
' Classe      : PersonneReseau (et LienReseau, NaturePersonneReseau)
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Une personne du réseau des patients — thérapeute du référentiel, intervenant saisi dans une fiche,
' contact (famille, entourage) — et les patients qui en relèvent, pour la recherche par nom (ADR-52,
' R5).
'
' Remarques   :
' - Produit par GestionRecherche (ChargerReseau, FiltrerReseau) ; aucun accès base de données,
'   aucune logique UI.
' - Une ligne par personne (décision de Joëlle, 24/10/2026) : un thérapeute est une personne ; un
'   intervenant lié à un thérapeute compte sous ce thérapeute ; les intervenants saisis et les
'   contacts de même nom (UtilsTexte.CleComparaison) forment une seule personne, chacun de ses liens
'   gardant sa fiche (FC…, AS…).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

' Nature d'une personne du réseau.
Public Enum NaturePersonneReseau
    Therapeute
    Intervenant
    Contact
End Enum

' -------------------------------------------------------------------------------------------------
' Classe : LienReseau
' Rôle   : Le lien d'une personne avec un patient : une fiche contact (FC…) ou un suivi (AS…).
' -------------------------------------------------------------------------------------------------
Public Class LienReseau

    ' Contact (famille / contacts) ou Intervenant (suivi) : la fiche à ouvrir.
    Public Property Nature As NatureResultat
    Public Property IdElement As Long
    Public Property CodeElement As String = String.Empty

    Public Property IdPatient As Long
    Public Property CodePatient As String = String.Empty
    Public Property NomPatient As String = String.Empty
    Public Property PrenomPatient As String = String.Empty
    Public Property PatientActif As Boolean = True

    ' Lien avec le patient (« Mère ») ou rôle de l'intervenant (« Logopède »).
    Public Property Role As String = String.Empty

    ' Période du suivi (intervenant) ; Nothing pour un contact.
    Public Property DateDebut As Date?
    Public Property DateFin As Date?

    Public ReadOnly Property PatientAffichage As String
        Get
            Return $"{NomPatient} {PrenomPatient}".Trim()
        End Get
    End Property

    ' Suivi terminé : date de fin passée.
    Public ReadOnly Property EstTermine As Boolean
        Get
            Return DateFin.HasValue AndAlso DateFin.Value.Date < Date.Today
        End Get
    End Property

    ' « en cours », « terminé le 12/03/2026 », vide pour un contact.
    Public ReadOnly Property EtatAffichage As String
        Get
            If Nature <> NatureResultat.Intervenant Then Return String.Empty
            If EstTermine Then Return $"terminé le {DateFin.Value:dd/MM/yyyy}"
            If DateFin.HasValue Then Return $"jusqu'au {DateFin.Value:dd/MM/yyyy}"
            Return "en cours"
        End Get
    End Property

End Class

' -------------------------------------------------------------------------------------------------
' Classe : PersonneReseau
' Rôle   : Une personne du réseau et ses liens avec les patients.
' -------------------------------------------------------------------------------------------------
Public Class PersonneReseau

    Public Property Nature As NaturePersonneReseau
    Public Property Nom As String = String.Empty
    Public Property Prenom As String = String.Empty

    ' Spécialité (thérapeute, intervenant) ou liens avec les patients (contact : « Mère, Tutrice »).
    Public Property Precision As String = String.Empty

    ' Thérapeute du référentiel : identifiant, code (TH…) et état ; 0 sinon.
    Public Property IdTherapeute As Long
    Public Property CodeTherapeute As String = String.Empty
    Public Property TherapeuteActif As Boolean = True

    ' Liens avec les patients, par patient.
    Public Property Liens As New List(Of LienReseau)

    ' Clé de la personne (regroupement, resélection au retour).
    Public ReadOnly Property Cle As String
        Get
            Return CleDe(Nature, IdTherapeute, Nom, Prenom)
        End Get
    End Property

    Public ReadOnly Property NomComplet As String
        Get
            Return $"{Nom} {Prenom}".Trim()
        End Get
    End Property

    ' « Thérapeute », « Intervenant », « Contact » ; « (inactif) » pour un thérapeute désactivé.
    Public ReadOnly Property NatureAffichage As String
        Get
            Select Case Nature
                Case NaturePersonneReseau.Therapeute
                    Return If(TherapeuteActif, "Thérapeute", "Thérapeute (inactif)")
                Case NaturePersonneReseau.Intervenant
                    Return "Intervenant"
                Case Else
                    Return "Contact"
            End Select
        End Get
    End Property

    ' Nombre de patients distincts (une personne peut avoir deux liens avec un même patient).
    Public ReadOnly Property NbPatients As Integer
        Get
            Return Liens.Select(Function(l) l.IdPatient).Distinct().Count()
        End Get
    End Property

    ' Clé d'une personne : le thérapeute par son identifiant ; sinon nature, nom et prénom comparés
    ' sans accents ni majuscules.
    Public Shared Function CleDe(nature As NaturePersonneReseau, idTherapeute As Long, nom As String, prenom As String) As String
        If nature = NaturePersonneReseau.Therapeute Then Return $"T|{idTherapeute}"
        Return $"{If(nature = NaturePersonneReseau.Intervenant, "I", "C")}|{UtilsTexte.CleComparaison(nom)}|{UtilsTexte.CleComparaison(prenom)}"
    End Function

    ' La même personne avec seulement certains de ses liens (filtre des patients inactifs).
    Public Function AvecLiens(liensGardes As IEnumerable(Of LienReseau)) As PersonneReseau
        Return New PersonneReseau With {
            .Nature = Nature, .Nom = Nom, .Prenom = Prenom, .Precision = Precision,
            .IdTherapeute = IdTherapeute, .CodeTherapeute = CodeTherapeute, .TherapeuteActif = TherapeuteActif,
            .Liens = liensGardes.ToList()
        }
    End Function

End Class
