' -------------------------------------------------------------------------------------------------
' Classe     : EnTeteDocument
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 19/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Contenu de l'en-tête et du pied de page d'un document imprimé ou exporté (anamnèse, zone du
' dossier) : cabinet, praticien, titre, patient ; mention de confidentialité, date d'édition.
'
' Responsabilités :
' - Rassembler ces textes au moment de l'impression ou de l'export (Creer) : le cabinet et le
'   praticien sont relus à chaque fois, une modification dans l'écran Cabinet est donc prise en
'   compte tout de suite.
' - Désigner un patient de la même façon partout (DesignerPatient : « Prénom NOM (P000123) »).
'
' Remarques  :
' - Choix de Joëlle (19/10/2026, temps 2 de l'éditeur) : en-tête cabinet et praticien, titre,
'   patient ; pied mention, date d'édition, page n / N. La mise en page est faite par
'   RichTextEditorHelper (impression et aperçu) et par Syncfusion DocIO (PDF, Word).
' - Mention : paramètre DOCUMENT_MENTION_CONFIDENTIALITE (migration v2.41.0) ; vide = pas de
'   mention.
' - Base injoignable ou cabinet non renseigné : le document sort quand même, sans les lignes
'   manquantes (journalisé, sans donnée de patient).
'
' Dépendances :
' - GestionCabinet, GestionParametres, ClesParametres, CheminsPatientHelper, GestionLog
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public NotInheritable Class EnTeteDocument

#Region "Contenu"

    ' Nom du cabinet (vide si non renseigné).
    Public Property Cabinet As String = String.Empty

    ' Praticien principal : « Prénom Nom — titre professionnel ».
    Public Property Praticien As String = String.Empty

    ' Titre du document (« Anamnèse », nom de la zone…).
    Public Property Titre As String = String.Empty

    ' Patient concerné (DesignerPatient) ; vide si le document ne concerne pas un patient.
    Public Property Patient As String = String.Empty

    ' Mention imprimée en pied de page ; vide = pas de mention.
    Public Property Mention As String = String.Empty

    ' Date d'édition imprimée en pied de page.
    Public Property DateEdition As Date = Date.Today

#End Region

#Region "Construction"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : Creer
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : En-tête complet pour un document, lu au moment où il sort.
    '
    ' Paramètres :
    ' - titre   : Titre du document.
    ' - patient : Patient concerné (DesignerPatient), ou vide.
    '
    ' Remarques  : Une erreur de lecture du cabinet n'empêche pas d'imprimer : l'en-tête garde le
    '              titre, le patient et la mention.
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function Creer(titre As String, patient As String) As EnTeteDocument

        Dim enTete As New EnTeteDocument With {
            .Titre = If(titre, String.Empty).Trim(),
            .Patient = If(patient, String.Empty).Trim(),
            .Mention = GestionParametres.GetTexte(ClesParametres.DocumentMentionConfidentialite,
                                                  ClesParametres.DefautDocumentMentionConfidentialite).Trim()
        }

        Try
            Dim cabinetActif As Cabinet = GestionCabinet.GetCabinetActif()
            If cabinetActif IsNot Nothing Then enTete.Cabinet = If(cabinetActif.NomCabinet, String.Empty).Trim()

            enTete.Praticien = DesignerPraticien(GestionCabinet.GetPraticienPrincipal())

        Catch ex As Exception
            GestionLog.EcrireLog("En-tête de document : cabinet ou praticien illisible, document édité sans ces lignes.",
                                 GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Database, ex)
        End Try

        Return enTete

    End Function

    ' « Prénom NOM (P000123) » ; le code est omis s'il n'est pas encore attribué (création).
    Public Shared Function DesignerPatient(patient As Patient) As String

        If patient Is Nothing Then Return String.Empty

        Dim code As String = patient.CodePatient
        If String.IsNullOrWhiteSpace(code) AndAlso patient.IdPatient > 0 Then
            code = CheminsPatientHelper.FormaterCodePatient(patient.IdPatient)
        End If

        Return If(String.IsNullOrWhiteSpace(code), patient.NomComplet, $"{patient.NomComplet} ({code})")

    End Function

    ' « Prénom Nom — titre professionnel » ; vide sans praticien.
    Private Shared Function DesignerPraticien(praticien As Praticien) As String

        If praticien Is Nothing Then Return String.Empty

        Dim nom As String = $"{praticien.Prenom} {praticien.Nom}".Trim()
        Dim titre As String = If(praticien.TitreProfessionnel, String.Empty).Trim()

        If nom.Length = 0 Then Return titre
        Return If(titre.Length = 0, nom, $"{nom} — {titre}")

    End Function

#End Region

End Class
