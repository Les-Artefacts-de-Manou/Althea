' -------------------------------------------------------------------------------------------------
' Module     : UtilsSaisieReferentiels
' Projet     : Althéa
' Version    : V1.2.0
' Date       : 23/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Alimente et lit les listes déroulantes « Pays » et « Civilité » des écrans de saisie d'une
' personne (cabinet, praticien, patient, contact, thérapeute), ainsi que la liste « Mutualité » de
' la fiche Patient, à partir des référentiels ref_pays, ref_civilites et ref_organismes.
'
' Responsabilités :
' - Charger une liste : valeurs actives + valeur courante si elle a été désactivée depuis
' - Proposer le pays par défaut (pays du cabinet, sinon premier pays actif)
' - Restituer l'identifiant saisi et, pour le pays, son code ISO (clé des règles de validation
'   UtilsTelephone / UtilsCodePostal)
'
' Remarques  :
' - Un seul endroit pour ce comportement, partagé par cinq écrans.
' - En cas d'erreur de lecture du référentiel, la liste reste vide (ou réduite à l'entrée
'   « aucune ») et l'erreur est journalisée : l'écran reste utilisable.
' - Le rechargement remplace la source de données : SelectedIndexChanged est déclenché, les écrans
'   l'ignorent pendant leur chargement (garde _chargementEnCours).
'
' - V1.1.0 (20/10/2026) : latéralités (ref_lateralites, v2.46.0) — ChargerLateralites, IdLateraliteSaisie.
' - V1.2.0 (23/10/2026) : spécialités des thérapeutes (ref_specialites, v2.55.0) — ChargerSpecialites,
'   IdSpecialiteSaisie.
' Dépendances :
' - GestionLateralites / Lateralite, GestionSpecialites / Specialite, GestionPays / Pays, GestionCivilites / Civilite, GestionOrganismes / Organisme (mutualités),
'   GestionCabinet (pays par défaut), GestionLog
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module UtilsSaisieReferentiels

    ' Libellé de l'entrée « aucune » des listes facultatives (valeur NULL en base)
    Private Const LIBELLE_AUCUN As String = "—"

#Region "Pays"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerPays
    ' Rôle      : Alimente une liste de pays et sélectionne le pays courant.
    '
    ' Paramètres :
    ' - cbo           : liste à alimenter (DropDownList)
    ' - idCourant     : pays enregistré, ou Nothing
    ' - libelleAucun  : si renseigné, ajoute en tête une entrée « sans pays » (ex. praticien dont
    '                   l'adresse vide reprend celle du cabinet) ; sinon un pays est toujours choisi
    '
    ' Remarques :
    ' - Sans pays courant : entrée « sans pays » si elle existe, sinon pays par défaut.
    ' -------------------------------------------------------------------------------------------------
    Public Sub ChargerPays(cbo As ComboBox, idCourant As ULong?, Optional libelleAucun As String = Nothing)

        Dim liste As New List(Of Pays)
        If libelleAucun IsNot Nothing Then
            liste.Add(New Pays With {.IdPays = 0UL, .LibellePays = libelleAucun})
        End If

        Try
            liste.AddRange(GestionPays.GetPays(True).
                Where(Function(p) p.Actif OrElse (idCourant.HasValue AndAlso p.IdPays = idCourant.Value)))
        Catch ex As Exception
            GestionLog.EcrireException("UtilsSaisieReferentiels.ChargerPays : lecture du référentiel ref_pays impossible", ex,
                                       category:=GestionLog.LogCategory.Database)
        End Try

        Dim idASelectionner As ULong? = idCourant
        If Not idASelectionner.HasValue AndAlso libelleAucun Is Nothing Then
            idASelectionner = IdPaysParDefaut(liste)
        End If

        cbo.DataSource = Nothing
        cbo.DisplayMember = NameOf(Pays.LibellePays)
        cbo.ValueMember = NameOf(Pays.IdPays)
        cbo.DataSource = liste

        Dim index As Integer = If(idASelectionner.HasValue,
                                  liste.FindIndex(Function(p) p.IdPays = idASelectionner.Value), -1)
        cbo.SelectedIndex = If(index >= 0, index, If(liste.Count > 0, 0, -1))

    End Sub

    ' Pays choisi, ou Nothing (aucune sélection ou entrée « sans pays »)
    Public Function PaysSaisi(cbo As ComboBox) As Pays
        Dim p = TryCast(cbo.SelectedItem, Pays)
        Return If(p Is Nothing OrElse p.IdPays = 0UL, Nothing, p)
    End Function

    Public Function IdPaysSaisi(cbo As ComboBox) As ULong?
        Dim p = PaysSaisi(cbo)
        Return If(p Is Nothing, CType(Nothing, ULong?), p.IdPays)
    End Function

    ' Code ISO du pays choisi (ex. « BE »), ou Nothing : à transmettre aux règles de validation
    Public Function CodePaysSaisi(cbo As ComboBox) As String
        Return PaysSaisi(cbo)?.CodePays
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : IdPaysParDefaut
    ' Rôle     : Pays proposé à la création : celui du cabinet s'il figure dans la liste, sinon le
    '            premier pays (ordre d'affichage du référentiel).
    ' -------------------------------------------------------------------------------------------------
    Private Function IdPaysParDefaut(liste As List(Of Pays)) As ULong?

        Dim idCabinet As ULong = 0UL
        Try
            idCabinet = If(GestionCabinet.GetCabinetActif()?.IdPays, 0UL)
        Catch ex As Exception
            GestionLog.EcrireException("UtilsSaisieReferentiels.IdPaysParDefaut : lecture du pays du cabinet impossible", ex,
                                       category:=GestionLog.LogCategory.Database)
        End Try

        If idCabinet <> 0UL AndAlso liste.Exists(Function(p) p.IdPays = idCabinet) Then Return idCabinet

        Dim premier = liste.FirstOrDefault(Function(p) p.IdPays <> 0UL)
        Return If(premier Is Nothing, CType(Nothing, ULong?), premier.IdPays)

    End Function

#End Region

#Region "Civilités"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerCivilites
    ' Rôle      : Alimente une liste de civilités (entrée « — » = aucune en tête) et sélectionne la
    '             civilité courante.
    ' -------------------------------------------------------------------------------------------------
    Public Sub ChargerCivilites(cbo As ComboBox, idCourant As ULong?)

        Dim liste As New List(Of Civilite) From {
            New Civilite With {.IdCivilite = 0UL, .LibelleCivilite = LIBELLE_AUCUN}
        }

        Try
            liste.AddRange(GestionCivilites.GetCivilites(True).
                Where(Function(c) c.Actif OrElse (idCourant.HasValue AndAlso c.IdCivilite = idCourant.Value)))
        Catch ex As Exception
            GestionLog.EcrireException("UtilsSaisieReferentiels.ChargerCivilites : lecture du référentiel ref_civilites impossible", ex,
                                       category:=GestionLog.LogCategory.Database)
        End Try

        cbo.DataSource = Nothing
        cbo.DisplayMember = NameOf(Civilite.LibelleCivilite)
        cbo.ValueMember = NameOf(Civilite.IdCivilite)
        cbo.DataSource = liste

        Dim index As Integer = If(idCourant.HasValue, liste.FindIndex(Function(c) c.IdCivilite = idCourant.Value), -1)
        cbo.SelectedIndex = Math.Max(index, 0)

    End Sub

    Public Function IdCiviliteSaisie(cbo As ComboBox) As ULong?
        Dim c = TryCast(cbo.SelectedItem, Civilite)
        Return If(c Is Nothing OrElse c.IdCivilite = 0UL, CType(Nothing, ULong?), c.IdCivilite)
    End Function

#End Region

#Region "Latéralités"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerLateralites
    ' Version   : V1.0.0
    ' Date      : 20/10/2026
    ' Rôle      : Alimente une liste de latéralités (entrée « — » = aucune en tête, valeurs actives et
    '             valeur courante si elle a été désactivée) et sélectionne la latéralité courante.
    ' -------------------------------------------------------------------------------------------------
    Public Sub ChargerLateralites(cbo As ComboBox, idCourant As ULong?)

        Dim liste As New List(Of Lateralite) From {
            New Lateralite With {.IdLateralite = 0UL, .LibelleLateralite = LIBELLE_AUCUN}
        }

        Try
            liste.AddRange(GestionLateralites.GetLateralites(True).
                Where(Function(l) l.Actif OrElse (idCourant.HasValue AndAlso l.IdLateralite = idCourant.Value)))
        Catch ex As Exception
            GestionLog.EcrireException("UtilsSaisieReferentiels.ChargerLateralites : lecture du référentiel ref_lateralites impossible", ex,
                                       category:=GestionLog.LogCategory.Database)
        End Try

        cbo.DataSource = Nothing
        cbo.DisplayMember = NameOf(Lateralite.LibelleLateralite)
        cbo.ValueMember = NameOf(Lateralite.IdLateralite)
        cbo.DataSource = liste

        Dim index As Integer = If(idCourant.HasValue, liste.FindIndex(Function(l) l.IdLateralite = idCourant.Value), -1)
        cbo.SelectedIndex = Math.Max(index, 0)

    End Sub

    Public Function IdLateraliteSaisie(cbo As ComboBox) As ULong?
        Dim l = TryCast(cbo.SelectedItem, Lateralite)
        Return If(l Is Nothing OrElse l.IdLateralite = 0UL, CType(Nothing, ULong?), l.IdLateralite)
    End Function

#End Region

#Region "Spécialités"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerSpecialites
    ' Version   : V1.0.0
    ' Date      : 23/10/2026
    ' Rôle      : Alimente une liste de spécialités (entrée « — » = aucune en tête, valeurs actives et
    '             valeur courante si elle a été désactivée) et sélectionne la spécialité courante.
    ' -------------------------------------------------------------------------------------------------
    Public Sub ChargerSpecialites(cbo As ComboBox, idCourant As ULong?)

        Dim liste As New List(Of Specialite) From {
            New Specialite With {.IdSpecialite = 0UL, .LibelleSpecialite = LIBELLE_AUCUN}
        }

        Try
            liste.AddRange(GestionSpecialites.GetSpecialites(True).
                Where(Function(s) s.Actif OrElse (idCourant.HasValue AndAlso s.IdSpecialite = idCourant.Value)))
        Catch ex As Exception
            GestionLog.EcrireException("UtilsSaisieReferentiels.ChargerSpecialites : lecture du référentiel ref_specialites impossible", ex,
                                       category:=GestionLog.LogCategory.Database)
        End Try

        cbo.DataSource = Nothing
        cbo.DisplayMember = NameOf(Specialite.LibelleSpecialite)
        cbo.ValueMember = NameOf(Specialite.IdSpecialite)
        cbo.DataSource = liste

        Dim index As Integer = If(idCourant.HasValue, liste.FindIndex(Function(s) s.IdSpecialite = idCourant.Value), -1)
        cbo.SelectedIndex = Math.Max(index, 0)

    End Sub

    Public Function IdSpecialiteSaisie(cbo As ComboBox) As ULong?
        Dim s = TryCast(cbo.SelectedItem, Specialite)
        Return If(s Is Nothing OrElse s.IdSpecialite = 0UL, CType(Nothing, ULong?), s.IdSpecialite)
    End Function

#End Region

#Region "Mutualités"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerMutualites
    ' Rôle      : Alimente une liste de mutualités (organismes de type MUTUALITE, entrée « — » = aucune
    '             en tête) et sélectionne la mutualité courante.
    ' -------------------------------------------------------------------------------------------------
    Public Sub ChargerMutualites(cbo As ComboBox, idCourant As ULong?)

        Dim liste As New List(Of Organisme) From {
            New Organisme With {.IdOrganisme = 0UL, .LibelleOrganisme = LIBELLE_AUCUN}
        }

        Try
            liste.AddRange(GestionOrganismes.GetOrganismes(True).
                Where(Function(o) o.TypeOrganisme = TypeOrganisme.Mutualite AndAlso
                                  (o.Actif OrElse (idCourant.HasValue AndAlso o.IdOrganisme = idCourant.Value))))
        Catch ex As Exception
            GestionLog.EcrireException("UtilsSaisieReferentiels.ChargerMutualites : lecture du référentiel ref_organismes impossible", ex,
                                       category:=GestionLog.LogCategory.Database)
        End Try

        cbo.DataSource = Nothing
        cbo.DisplayMember = NameOf(Organisme.LibelleOrganisme)
        cbo.ValueMember = NameOf(Organisme.IdOrganisme)
        cbo.DataSource = liste

        Dim index As Integer = If(idCourant.HasValue, liste.FindIndex(Function(o) o.IdOrganisme = idCourant.Value), -1)
        cbo.SelectedIndex = Math.Max(index, 0)

    End Sub

    Public Function IdMutualiteSaisie(cbo As ComboBox) As ULong?
        Dim o = TryCast(cbo.SelectedItem, Organisme)
        Return If(o Is Nothing OrElse o.IdOrganisme = 0UL, CType(Nothing, ULong?), o.IdOrganisme)
    End Function

    ' Identifiants des mutualités présentes dans la liste (détection d'un ajout après la fenêtre modale)
    Public Function IdsMutualites(cbo As ComboBox) As HashSet(Of ULong)
        Dim liste = TryCast(cbo.DataSource, List(Of Organisme))
        Return If(liste Is Nothing, New HashSet(Of ULong),
                  New HashSet(Of ULong)(liste.Where(Function(o) o.IdOrganisme <> 0UL).Select(Function(o) o.IdOrganisme)))
    End Function

#End Region

End Module
