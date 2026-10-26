' -------------------------------------------------------------------------------------------------
' Classe      : UC_DossierFiche — Zones cliniques (fichier partiel)
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Onglet Suivi clinique du dossier : une ligne cliquable par zone clinique (ZonesDossier),
' aperçu au survol, ouverture de la zone dans DossierZoneEditor et rafraîchissement des aperçus.
'
' Remarques   :
' - Audit C.5 (décision de Joëlle du 25/10/2026) : la fiche est répartie en fichiers partiels, un par
'   onglet ; cette région est déplacée telle quelle depuis UC_DossierFiche.vb (V1.23.2), sans
'   changement de comportement. Les procédures gardent leur en-tête et leur historique.
' - Champs, contexte et méthodes communes : UC_DossierFiche.vb ; contrôles : UC_DossierFiche.Designer.vb.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Partial Class UC_DossierFiche

#Region "Zones cliniques"

    ' Mapping zone → ligne cliquable (titre + aperçu), renseigné à la construction dynamique.
    Private ReadOnly _lignesZones As New Dictionary(Of ZoneDossier, AltheaLigne)

    ' Nombre maximal de caractères affichés dans un aperçu de zone.
    Private Const APERCU_LONGUEUR_MAX As Integer = 160

    ' Texte d'aperçu d'une zone vide.
    Private Const APERCU_VIDE As String = "Vide"

    ' Hauteur d'une ligne de zone (maquette « Suivi clinique A »).
    Private Const HAUTEUR_LIGNE_ZONE As Single = 56.0F

    ' Fenêtre flottante d'aperçu (infobulle fabriquée) affichée au survol d'un bouton de zone.
    Private _apercuPopup As Form

    ' Éditeur riche en lecture seule contenu dans la fenêtre flottante d'aperçu.
    Private _apercuRtb As RichTextBox

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ConstruireZonesCliniques
    ' Version   : V1.1.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Construit dynamiquement, dans le TableLayoutPanel de la carte « Zones cliniques », une ligne
    ' cliquable par zone (AltheaLigne : icône, libellé, aperçu, « Ouvrir »).
    '
    ' Remarques :
    ' - Les zones proviennent EXCLUSIVEMENT de la whitelist ZonesDossier (aucune valeur codée en dur).
    ' - La zone d'archive (lecture seule) n'est ajoutée que lorsqu'elle est renseignée (voir aperçu).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ConstruireZonesCliniques()

        tlpZones.SuspendLayout()
        tlpZones.Controls.Clear()
        tlpZones.RowStyles.Clear()
        _lignesZones.Clear()

        Dim ligne As Integer = 0

        For Each info As ZoneDossierInfo In ZonesDossier.GetZonesEditables()
            AjouterLigneZone(info, ligne)
            ligne += 1
        Next

        ' Pas de filet sous la dernière ligne : le bas de la carte suffit.
        If _lignesZones.Count > 0 Then _lignesZones.Values.Last().Separateur = False

        ' Ligne de remplissage : sans elle, le TableLayoutPanel attribue l'espace restant à la
        ' dernière ligne (toutes les autres étant en hauteur absolue), qui s'étirerait.
        tlpZones.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        tlpZones.RowCount = ligne + 1
        tlpZones.ResumeLayout(True)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AjouterLigneZone
    ' Version   : V1.1.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Ajoute la ligne cliquable d'une zone clinique dans le TableLayoutPanel (UI-5 : remplace le
    ' couple bouton + label d'aperçu).
    '
    ' Paramètres :
    ' - info  : Métadonnées de la zone (libellé, lecture seule)
    ' - ligne : Index de ligne dans le TableLayoutPanel
    ' -------------------------------------------------------------------------------------------------
    Private Sub AjouterLigneZone(info As ZoneDossierInfo, ligne As Integer)

        tlpZones.RowStyles.Add(New RowStyle(SizeType.Absolute, HAUTEUR_LIGNE_ZONE))

        Dim ligneZone As New AltheaLigne With {
            .Text = info.Libelle,
            .Tag = info.Zone,
            .Dock = DockStyle.Fill,
            .Margin = Padding.Empty,
            .TexteVide = APERCU_VIDE
        }

        ' Icône de la zone : le nom de fichier PNG correspond au nom de l'enum ZoneDossier
        ' (Assets\Boutons_suivi\<Zone>.png). Chargement sans verrou ; Nothing si absente.
        ligneZone.Image = UtilsButtons.LoadImageFromAsset(UITheme.AssetsBoutonsSuiviPath, info.Zone.ToString())

        AddHandler ligneZone.Click, AddressOf ZoneBouton_Click
        AddHandler ligneZone.MouseEnter, AddressOf ZoneBouton_MouseEnter
        AddHandler ligneZone.MouseLeave, AddressOf ZoneBouton_MouseLeave

        If _context IsNot Nothing Then
            _context.SetToolTip(ligneZone, $"Ouvrir et éditer la zone clinique « {info.Libelle} » du dossier.")
        End If

        tlpZones.Controls.Add(ligneZone, 0, ligne)
        _lignesZones(info.Zone) = ligneZone

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirApercusZones
    ' Version   : V1.0.0
    ' Date      : 11/07/2026
    '
    ' Rôle      :
    ' Renseigne le texte d'aperçu de chaque zone à partir du dossier chargé (texte brut tronqué).
    '
    ' Remarques :
    ' - En l'absence de dossier chargé, les aperçus affichent APERCU_VIDE (TexteVide de la ligne).
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemplirApercusZones()

        For Each kv As KeyValuePair(Of ZoneDossier, AltheaLigne) In _lignesZones
            kv.Value.Description = ConstruireApercu(TexteZone(kv.Key))
        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : TexteZone
    ' Version  : V1.0.0
    ' Date     : 11/07/2026
    '
    ' Rôle     :
    ' Retourne le texte brut d'une zone à partir du dossier chargé (pour l'aperçu rapide).
    '
    ' Paramètres :
    ' - zone : Zone clinique dont on veut le texte brut
    '
    ' Retour   :
    ' - String : Texte brut de la zone (éventuellement Nothing/vide)
    ' -------------------------------------------------------------------------------------------------
    Private Function TexteZone(zone As ZoneDossier) As String

        If _dossier Is Nothing Then
            Return String.Empty
        End If

        Select Case zone
            Case ZoneDossier.MotifSuivi
                Return _dossier.MotifSuiviTxt
            Case ZoneDossier.Historique
                Return _dossier.HistoriqueTxt
            Case ZoneDossier.Antecedents
                Return _dossier.AntecedentsTxt
            Case ZoneDossier.Diagnostic
                Return _dossier.DiagnosticTxt
            Case ZoneDossier.TraitementsEnCours
                Return _dossier.TraitementsEnCoursTxt
            Case ZoneDossier.ObjectifsSuivi
                Return _dossier.ObjectifsSuiviTxt
            Case ZoneDossier.HistoriqueArchive
                Return _dossier.HistoriqueArchiveTxt
            Case Else
                Return String.Empty
        End Select

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : RtfZone
    ' Version  : V1.0.0
    ' Date     : 11/07/2026
    '
    ' Rôle     :
    ' Retourne le contenu RTF d'une zone à partir du dossier chargé (pour l'aperçu riche au survol).
    '
    ' Paramètres :
    ' - zone : Zone clinique dont on veut le RTF
    '
    ' Retour   :
    ' - String : Contenu RTF de la zone (éventuellement Nothing/vide)
    ' -------------------------------------------------------------------------------------------------
    Private Function RtfZone(zone As ZoneDossier) As String

        If _dossier Is Nothing Then
            Return String.Empty
        End If

        Select Case zone
            Case ZoneDossier.MotifSuivi
                Return _dossier.MotifSuiviRtf
            Case ZoneDossier.Historique
                Return _dossier.HistoriqueRtf
            Case ZoneDossier.Antecedents
                Return _dossier.AntecedentsRtf
            Case ZoneDossier.Diagnostic
                Return _dossier.DiagnosticRtf
            Case ZoneDossier.TraitementsEnCours
                Return _dossier.TraitementsEnCoursRtf
            Case ZoneDossier.ObjectifsSuivi
                Return _dossier.ObjectifsSuiviRtf
            Case ZoneDossier.HistoriqueArchive
                Return _dossier.HistoriqueArchiveRtf
            Case Else
                Return String.Empty
        End Select

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ConstruireApercu
    ' Version  : V1.0.0
    ' Date     : 11/07/2026
    '
    ' Rôle     :
    ' Construit un aperçu court et sur une seule ligne à partir d'un texte brut de zone.
    '
    ' Paramètres :
    ' - texte : Texte brut de la zone
    '
    ' Retour   :
    ' - String : Aperçu tronqué, ou chaîne vide (la ligne affiche alors son TexteVide).
    ' -------------------------------------------------------------------------------------------------
    Private Function ConstruireApercu(texte As String) As String

        If String.IsNullOrWhiteSpace(texte) Then
            Return String.Empty
        End If

        Dim aplati As String = texte.Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ").Trim()

        If aplati.Length > APERCU_LONGUEUR_MAX Then
            Return aplati.Substring(0, APERCU_LONGUEUR_MAX).TrimEnd() & "…"
        End If

        Return aplati

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ActiverBoutonsZones
    ' Version   : V1.0.0
    ' Date      : 11/07/2026
    '
    ' Rôle      :
    ' Active ou désactive les boutons d'ouverture des zones cliniques.
    '
    ' Paramètres :
    ' - actif : True pour activer (dossier persisté), False sinon (création).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ActiverBoutonsZones(actif As Boolean)

        For Each kv As KeyValuePair(Of ZoneDossier, AltheaLigne) In _lignesZones
            kv.Value.Enabled = actif
        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ZoneBouton_MouseEnter
    ' Version   : V1.0.0
    ' Date      : 11/07/2026
    '
    ' Rôle      :
    ' Affiche, au survol d'un bouton de zone, une fenêtre flottante (infobulle fabriquée) présentant
    ' le contenu riche (RTF) de la zone, ou son texte brut à défaut.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ZoneBouton_MouseEnter(sender As Object, e As EventArgs)

        Dim btn As Button = TryCast(sender, Button)

        If btn Is Nothing OrElse btn.Tag Is Nothing Then
            Return
        End If

        Dim zone As ZoneDossier = CType(btn.Tag, ZoneDossier)
        Dim rtf As String = RtfZone(zone)
        Dim txt As String = TexteZone(zone)

        ' Rien à afficher si la zone est vide
        If String.IsNullOrWhiteSpace(rtf) AndAlso String.IsNullOrWhiteSpace(txt) Then
            Return
        End If

        AfficherApercuPopup(btn, rtf, txt)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ZoneBouton_MouseLeave
    ' Version   : V1.0.0
    ' Date      : 11/07/2026
    '
    ' Rôle      :
    ' Masque la fenêtre flottante d'aperçu lorsque la souris quitte le bouton de zone.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ZoneBouton_MouseLeave(sender As Object, e As EventArgs)

        MasquerApercuPopup()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherApercuPopup
    ' Version   : V1.1.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Construit (si besoin) et positionne la fenêtre flottante d'aperçu sous la ligne survolée,
    ' puis y charge le contenu riche de la zone en lecture seule.
    '
    ' Paramètres :
    ' - btn : Bouton de zone survolé (sert de point d'ancrage)
    ' - rtf : Contenu RTF de la zone
    ' - txt : Texte brut de repli
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherApercuPopup(btn As Button, rtf As String, txt As String)

        If _apercuPopup Is Nothing Then
            CreerApercuPopup()
        End If

        RichTextEditorHelper.ChargerContenu(_apercuRtb, rtf, txt)

        ' Positionnement sous la ligne survolée, aligné sur le début de l'aperçu (la ligne occupe
        ' toute la largeur : un placement à sa droite sortirait de l'écran).
        Dim ligneZone As AltheaLigne = TryCast(btn, AltheaLigne)
        Dim x As Integer = If(ligneZone IsNot Nothing, ligneZone.DebutDescription, btn.Width + 8)
        Dim ancre As Point = btn.PointToScreen(New Point(x, btn.Height + 2))
        _apercuPopup.Location = ancre
        _apercuPopup.Show()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MasquerApercuPopup
    ' Version   : V1.0.0
    ' Date      : 11/07/2026
    '
    ' Rôle      :
    ' Masque la fenêtre flottante d'aperçu si elle est visible.
    ' -------------------------------------------------------------------------------------------------
    Private Sub MasquerApercuPopup()

        If _apercuPopup IsNot Nothing AndAlso _apercuPopup.Visible Then
            _apercuPopup.Hide()
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : CreerApercuPopup
    ' Version   : V1.0.0
    ' Date      : 11/07/2026
    '
    ' Rôle      :
    ' Crée la fenêtre flottante d'aperçu (sans bordure, non focusable) et son RichTextBox lecture seule.
    ' -------------------------------------------------------------------------------------------------
    Private Sub CreerApercuPopup()

        _apercuRtb = New RichTextBox With {
            .Dock = DockStyle.Fill,
            .[ReadOnly] = True,
            .BorderStyle = BorderStyle.None,
            .BackColor = Color.FromArgb(252, 250, 247),
            .Font = New Font("Calibri", 10.0F),
            .ScrollBars = RichTextBoxScrollBars.Vertical,
            .TabStop = False
        }

        _apercuPopup = New Form With {
            .FormBorderStyle = FormBorderStyle.None,
            .ShowInTaskbar = False,
            .StartPosition = FormStartPosition.Manual,
            .Size = New Size(420, 220),
            .BackColor = Color.FromArgb(200, 185, 170),
            .Padding = New Padding(1),
            .TopMost = True
        }

        _apercuPopup.Controls.Add(_apercuRtb)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ZoneBouton_Click
    ' Version   : V1.1.0
    ' Date      : 19/10/2026
    '
    ' Rôle      :
    ' Ouvre la modale d'édition (DossierZoneEditor) pour la zone associée au bouton cliqué, puis
    ' rafraîchit le dossier et les aperçus si un enregistrement a eu lieu.
    ' - V1.1.0 (19/10/2026) : transmet le patient à la zone (en-tête de ses documents).
    ' -------------------------------------------------------------------------------------------------
    Private Sub ZoneBouton_Click(sender As Object, e As EventArgs)

        Dim btn As Button = TryCast(sender, Button)

        If btn Is Nothing OrElse btn.Tag Is Nothing OrElse _idDossier <= 0 Then
            Return
        End If

        Dim zone As ZoneDossier = CType(btn.Tag, ZoneDossier)

        MasquerApercuPopup()

        Dim codePatient As String = Nothing
        Dim codeDossier As String = Nothing

        If _dossier IsNot Nothing Then
            codePatient = CheminsPatientHelper.FormaterCodePatient(_dossier.IdPatient)
            codeDossier = _dossier.CodeDossier
        End If

        Dim archive As Boolean = _dossier IsNot Nothing AndAlso _dossier.EstArchive

        Using editeur As New DossierZoneEditor(_idDossier, zone, codePatient, codeDossier, archive,
                                               EnTeteDocument.DesignerPatient(_patient))

            editeur.SetContext(_context)
            SignalerAction($"Zone {btn.Text}", $"Édition de la zone clinique « {btn.Text} » du dossier {codeDossier}.")

            If editeur.ShowDialog(FindForm()) = DialogResult.OK Then
                RafraichirZones()
                _context?.SetStatus($"Zone clinique « {btn.Text} » enregistrée.")
            End If

        End Using

        RafraichirContexteNavigation()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RafraichirZones
    ' Version   : V1.0.0
    ' Date      : 11/07/2026
    '
    ' Rôle      :
    ' Recharge le dossier depuis la couche métier et met à jour les aperçus des zones cliniques.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RafraichirZones()

        Try

            _dossier = GestionDossiers.GetDossier(_idDossier)
            RemplirApercusZones()

        Catch ex As Exception

            GestionLog.EcrireLog(
                $"Erreur rafraîchissement des zones (UC_DossierFiche, id={_idDossier}).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI,
                ex
            )

        End Try

    End Sub

#End Region

End Class
