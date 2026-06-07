Option Strict On
Option Explicit On
Imports System.Drawing.Printing
Imports System.Runtime.InteropServices

Public Module RichTextEditorHelper

#Region "Configuration"

    Public Sub ConfigurerRichTextBox(rtb As RichTextBox)

        If rtb Is Nothing Then Exit Sub

        rtb.AcceptsTab = True
        rtb.DetectUrls = True
        rtb.HideSelection = False
        rtb.WordWrap = True
        rtb.ScrollBars = RichTextBoxScrollBars.Vertical
        rtb.EnableAutoDragDrop = True

    End Sub

#End Region

#Region "API Windows pour impression RTF"

    ' Structures et constantes pour l'API EM_FORMATRANGE
    Private Const WM_USER As Integer = &H400
    Private Const EM_FORMATRANGE As Integer = WM_USER + 57

    <StructLayout(LayoutKind.Sequential)>
    Private Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure CHARRANGE
        Public cpMin As Integer
        Public cpMax As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure FORMATRANGE
        Public hdc As IntPtr
        Public hdcTarget As IntPtr
        Public rc As RECT
        Public rcPage As RECT
        Public chrg As CHARRANGE
    End Structure

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Méthode d'extension : Print
    ' Version    : V1.1
    ' Date       : 17/05/2026
    '
    ' Rôle       :
    ' Méthode d'extension pour RichTextBox permettant d'imprimer avec formatage RTF.
    '
    ' Paramètres :
    ' - rtb          : RichTextBox à imprimer
    ' - charFrom     : Index du premier caractère à imprimer
    ' - charTo       : Index du dernier caractère à imprimer
    ' - e            : Arguments de l'événement PrintPage
    '
    ' Retour     : Index du dernier caractère imprimé sur cette page
    '
    ' Remarques  :
    ' - Utilise l'API Windows EM_FORMATRANGE pour respecter le formatage RTF
    ' - Gère automatiquement la pagination
    ' - Envoie un message de cache clear entre les pages
    '
    ' Corrections V1.1 :
    ' - Ajout du cache clear pour impression physique correcte
    ' -------------------------------------------------------------------------------------------------
    <Runtime.CompilerServices.Extension>
    Public Function Print(rtb As RichTextBox, charFrom As Integer, charTo As Integer, e As PrintPageEventArgs) As Integer

        ' Calculer la zone d'impression (en twips : 1 inch = 1440 twips)
        Dim fr As FORMATRANGE
        Dim hdc As IntPtr = e.Graphics.GetHdc()

        fr.hdc = hdc
        fr.hdcTarget = hdc

        ' Zone imprimable (en twips)
        fr.rc.Left = HundredthInchToTwips(e.MarginBounds.Left)
        fr.rc.Top = HundredthInchToTwips(e.MarginBounds.Top)
        fr.rc.Right = HundredthInchToTwips(e.MarginBounds.Right)
        fr.rc.Bottom = HundredthInchToTwips(e.MarginBounds.Bottom)

        ' Zone de la page (en twips)
        fr.rcPage.Left = HundredthInchToTwips(CInt(e.PageBounds.Left))
        fr.rcPage.Top = HundredthInchToTwips(CInt(e.PageBounds.Top))
        fr.rcPage.Right = HundredthInchToTwips(CInt(e.PageBounds.Right))
        fr.rcPage.Bottom = HundredthInchToTwips(CInt(e.PageBounds.Bottom))

        ' Plage de caractères à imprimer
        fr.chrg.cpMin = charFrom
        fr.chrg.cpMax = charTo

        ' Allouer la structure en mémoire
        Dim lParam As IntPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(fr))
        Marshal.StructureToPtr(fr, lParam, False)

        ' Envoyer le message EM_FORMATRANGE pour imprimer/mesurer
        Dim result As IntPtr = SendMessage(rtb.Handle, EM_FORMATRANGE, New IntPtr(1), lParam)

        ' Libérer la mémoire
        Marshal.FreeCoTaskMem(lParam)

        ' Libérer le contexte graphique
        e.Graphics.ReleaseHdc(hdc)

        ' Envoyer un message pour libérer le cache de formatage
        ' Cela est crucial pour que l'impression physique fonctionne correctement
        SendMessage(rtb.Handle, EM_FORMATRANGE, IntPtr.Zero, IntPtr.Zero)

        ' Retourner l'index du dernier caractère imprimé
        Return result.ToInt32()

    End Function

    ' Conversion centièmes de pouce vers twips
    Private Function HundredthInchToTwips(n As Integer) As Integer
        Return CInt(n * 14.4)
    End Function

#End Region

#Region "Chargement / Sauvegarde"

    Public Sub ChargerContenu(rtb As RichTextBox, rtfContent As String, txtFallback As String)

        If rtb Is Nothing Then Exit Sub

        rtb.Clear()

        If String.IsNullOrWhiteSpace(rtfContent) Then
            rtb.Text = If(txtFallback, "")
            Exit Sub
        End If

        Try
            rtb.Rtf = rtfContent
        Catch
            rtb.Text = If(txtFallback, "")
        End Try

    End Sub

    Public Function ExtraireRtf(rtb As RichTextBox) As String

        If rtb Is Nothing Then Return ""

        If String.IsNullOrWhiteSpace(rtb.Text) Then Return ""

        Return rtb.Rtf

    End Function

    Public Function ExtraireTxt(rtb As RichTextBox) As String

        If rtb Is Nothing Then Return ""

        Return rtb.Text.Trim()

    End Function

#End Region

#Region "Formatage de caractères"

    Public Sub BasculerGras(rtb As RichTextBox)

        BasculerStyle(rtb, FontStyle.Bold)

    End Sub

    Public Sub BasculerItalique(rtb As RichTextBox)

        BasculerStyle(rtb, FontStyle.Italic)

    End Sub

    Public Sub BasculerSouligne(rtb As RichTextBox)

        BasculerStyle(rtb, FontStyle.Underline)

    End Sub

    Public Sub BasculerBarre(rtb As RichTextBox)

        BasculerStyle(rtb, FontStyle.Strikeout)

    End Sub

    Private Sub BasculerStyle(rtb As RichTextBox, style As FontStyle)

        If rtb Is Nothing Then Exit Sub
        If rtb.SelectionFont Is Nothing Then Exit Sub

        Dim currentFont As Font = rtb.SelectionFont
        Dim newStyle As FontStyle

        If currentFont.Style.HasFlag(style) Then
            newStyle = currentFont.Style And Not style
        Else
            newStyle = currentFont.Style Or style
        End If

        rtb.SelectionFont = New Font(currentFont, newStyle)

    End Sub

    Public Sub ChangerCouleurTexte(rtb As RichTextBox, couleur As Color)

        If rtb Is Nothing Then Exit Sub

        rtb.SelectionColor = couleur

    End Sub

    Public Sub ChangerCouleurFond(rtb As RichTextBox, couleur As Color)

        If rtb Is Nothing Then Exit Sub

        rtb.SelectionBackColor = couleur

    End Sub

#End Region

#Region "Formatage de police"

    Public Sub ChangerPolice(rtb As RichTextBox, nomPolice As String)

        If rtb Is Nothing Then Exit Sub
        If String.IsNullOrWhiteSpace(nomPolice) Then Exit Sub
        If rtb.SelectionFont Is Nothing Then Exit Sub

        Try
            Dim currentFont As Font = rtb.SelectionFont
            rtb.SelectionFont = New Font(nomPolice, currentFont.Size, currentFont.Style)
        Catch
            ' Police inexistante, on ne fait rien
        End Try

    End Sub

    Public Sub ChangerTaillePolice(rtb As RichTextBox, taille As Single)

        If rtb Is Nothing Then Exit Sub
        If rtb.SelectionFont Is Nothing Then Exit Sub
        If taille < 6 OrElse taille > 72 Then Exit Sub

        Dim currentFont As Font = rtb.SelectionFont
        rtb.SelectionFont = New Font(currentFont.FontFamily, taille, currentFont.Style)

    End Sub

#End Region

#Region "Formatage de paragraphe"

    Public Sub ChangerAlignement(rtb As RichTextBox, alignement As HorizontalAlignment)

        If rtb Is Nothing Then Exit Sub

        rtb.SelectionAlignment = alignement

    End Sub

    Public Sub BasculerPuces(rtb As RichTextBox)

        If rtb Is Nothing Then Exit Sub

        rtb.SelectionBullet = Not rtb.SelectionBullet

    End Sub

    Public Sub InsererTabulation(rtb As RichTextBox)

        If rtb Is Nothing Then Exit Sub

        rtb.SelectedText = vbTab

    End Sub

    Public Sub AugmenterRetrait(rtb As RichTextBox)

        If rtb Is Nothing Then Exit Sub

        Dim nouveauRetrait As Integer = rtb.SelectionIndent + 20

        If nouveauRetrait <= 200 Then
            rtb.SelectionIndent = nouveauRetrait
        End If

    End Sub

    Public Sub DiminuerRetrait(rtb As RichTextBox)

        If rtb Is Nothing Then Exit Sub

        Dim nouveauRetrait As Integer = rtb.SelectionIndent - 20

        If nouveauRetrait >= 0 Then
            rtb.SelectionIndent = nouveauRetrait
        End If

    End Sub

#End Region

#Region "Presse-papiers"

    Public Sub Couper(rtb As RichTextBox)

        If rtb Is Nothing Then Exit Sub
        If rtb.SelectionLength = 0 Then Exit Sub

        rtb.Cut()

    End Sub

    Public Sub Copier(rtb As RichTextBox)

        If rtb Is Nothing Then Exit Sub
        If rtb.SelectionLength = 0 Then Exit Sub

        rtb.Copy()

    End Sub

    Public Sub Coller(rtb As RichTextBox)

        If rtb Is Nothing Then Exit Sub

        rtb.Paste()

    End Sub

#End Region

#Region "Actions"

    Public Sub EffacerFormatage(rtb As RichTextBox)

        If rtb Is Nothing Then Exit Sub
        If rtb.SelectionLength = 0 Then Exit Sub

        rtb.SelectionFont = rtb.Font
        rtb.SelectionColor = rtb.ForeColor
        rtb.SelectionBackColor = rtb.BackColor
        rtb.SelectionBullet = False
        rtb.SelectionIndent = 0
        rtb.SelectionAlignment = HorizontalAlignment.Left

    End Sub

#End Region

#Region "Insertion"

    Public Sub InsererDateHeure(rtb As RichTextBox, Optional format As String = "dd/MM/yyyy HH:mm")

        If rtb Is Nothing Then Exit Sub

        Dim dateTimeString As String = DateTime.Now.ToString(format)
        rtb.SelectedText = dateTimeString

    End Sub

#End Region

#Region "Recherche"

    Public Function Rechercher(rtb As RichTextBox, texteRecherche As String, sensibleCasse As Boolean, Optional debutRecherche As Integer = 0) As Integer

        If rtb Is Nothing Then Return -1
        If String.IsNullOrEmpty(texteRecherche) Then Return -1

        Dim options As RichTextBoxFinds = RichTextBoxFinds.None

        If Not sensibleCasse Then
            options = options Or RichTextBoxFinds.None
        Else
            options = options Or RichTextBoxFinds.MatchCase
        End If

        Dim index As Integer = rtb.Find(texteRecherche, debutRecherche, options)

        Return index

    End Function

    Public Sub Remplacer(rtb As RichTextBox, nouveauTexte As String)

        If rtb Is Nothing Then Exit Sub
        If rtb.SelectionLength = 0 Then Exit Sub

        rtb.SelectedText = nouveauTexte

    End Sub

    Public Function RemplacerTout(rtb As RichTextBox, texteRecherche As String, nouveauTexte As String, sensibleCasse As Boolean) As Integer

        If rtb Is Nothing Then Return 0
        If String.IsNullOrEmpty(texteRecherche) Then Return 0

        Dim count As Integer = 0
        Dim index As Integer = 0

        Do
            index = Rechercher(rtb, texteRecherche, sensibleCasse, index)

            If index >= 0 Then
                rtb.Select(index, texteRecherche.Length)
                rtb.SelectedText = nouveauTexte
                index += nouveauTexte.Length
                count += 1
            End If

        Loop While index >= 0

        Return count

    End Function

#End Region

#Region "Impression"

    ' Variables pour gérer l'impression du contenu RTF
    Private printDocument As PrintDocument
    Private printRichTextBox As RichTextBox
    Private checkPrint As Integer
    Private pageSettings As PageSettings

    ' -------------------------------------------------------------------------------------------------
    ' Méthode    : Imprimer
    ' Version    : V1.4
    ' Date       : 17/05/2026
    '
    ' Rôle       :
    ' Affiche le dialogue d'impression système et lance l'impression directement.
    '
    ' Paramètres :
    ' - rtb      : RichTextBox à imprimer
    ' - titre    : Titre optionnel du document
    '
    ' Retour     : Aucun (Sub)
    '
    ' Remarques  :
    ' - Affiche PrintDialog pour choisir l'imprimante et configurer l'impression
    ' - Lance l'impression directement après validation (pas de preview)
    ' - Utilise les paramètres système (marges, format, orientation) définis par l'utilisateur
    ' - Gère la pagination automatique via PrintPageEventArgs
    '
    ' Corrections V1.4 :
    ' - Suppression du PrintPreviewDialog automatique (impression directe)
    ' - L'utilisateur configure tout via PrintDialog puis imprime immédiatement
    ' -------------------------------------------------------------------------------------------------
    Public Sub Imprimer(rtb As RichTextBox, Optional titre As String = "Document Althéa")

        If rtb Is Nothing Then Exit Sub
        If String.IsNullOrWhiteSpace(rtb.Text) Then
            MessageBox.Show("Le document est vide.", "Impression", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Try
            ' Initialisation
            printDocument = New PrintDocument()
            printDocument.DocumentName = titre
            printRichTextBox = rtb
            checkPrint = 0

            ' Afficher le dialogue d'impression système pour configuration
            Dim printDialog As New PrintDialog()
            printDialog.Document = printDocument
            printDialog.AllowSomePages = False
            printDialog.AllowSelection = False
            printDialog.AllowCurrentPage = False
            printDialog.UseEXDialog = True  ' Utiliser le dialogue étendu Windows

            ' L'utilisateur peut configurer l'imprimante, les marges, le format via "Propriétés"
            If printDialog.ShowDialog() = DialogResult.OK Then
                ' Associer l'événement PrintPage SEULEMENT maintenant
                AddHandler printDocument.PrintPage, AddressOf ImprimerPageRTF

                ' Réinitialiser le compteur juste avant l'impression
                checkPrint = 0

                ' Imprimer directement
                printDocument.Print()

                ' Nettoyer l'événement
                RemoveHandler printDocument.PrintPage, AddressOf ImprimerPageRTF
            End If

        Catch ex As Exception
            MessageBox.Show($"Erreur lors de l'impression : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Méthode    : ImprimerPageRTF
    ' Version    : V1.2
    ' Date       : 17/05/2026
    '
    ' Rôle       :
    ' Gestionnaire d'événement PrintPage pour imprimer le contenu RTF page par page.
    '
    ' Paramètres :
    ' - sender   : Source de l'événement
    ' - e        : Arguments contenant le contexte d'impression
    '
    ' Retour     : Aucun (Sub)
    '
    ' Remarques  :
    ' - Utilise RichTextBox.Print() avec EM_FORMATRANGE pour respecter le formatage RTF
    ' - Gère automatiquement la pagination
    ' - Définit HasMorePages pour imprimer plusieurs pages si nécessaire
    '
    ' Corrections V1.2 :
    ' - Gestion améliorée de la libération du contexte graphique
    ' - Support correct de l'impression sur imprimante physique
    ' -------------------------------------------------------------------------------------------------
    Private Sub ImprimerPageRTF(sender As Object, e As PrintPageEventArgs)

        Try
            ' Calculer le nombre de caractères qui tiennent sur la page
            checkPrint = printRichTextBox.Print(checkPrint, printRichTextBox.TextLength, e)

            ' Vérifier s'il reste du contenu à imprimer
            If checkPrint < printRichTextBox.TextLength Then
                e.HasMorePages = True
            Else
                e.HasMorePages = False
                ' Réinitialiser pour une prochaine impression
                checkPrint = 0
            End If

        Catch ex As Exception
            ' En cas d'erreur, arrêter l'impression
            e.HasMorePages = False
            checkPrint = 0
            MessageBox.Show($"Erreur lors de l'impression de la page : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Méthode    : AfficherApercu
    ' Version    : V1.0
    ' Date       : 17/05/2026
    '
    ' Rôle       :
    ' Affiche l'aperçu avant impression sans imprimer directement.
    '
    ' Paramètres :
    ' - rtb      : RichTextBox à prévisualiser
    ' - titre    : Titre optionnel du document
    '
    ' Retour     : Aucun (Sub)
    '
    ' Remarques  :
    ' - Permet de voir l'aperçu du document avant de décider d'imprimer
    ' - L'utilisateur peut imprimer depuis l'aperçu s'il le souhaite
    ' - Utilise les paramètres de page configurés (marges, format, orientation)
    ' -------------------------------------------------------------------------------------------------
    Public Sub AfficherApercu(rtb As RichTextBox, Optional titre As String = "Document Althéa")

        If rtb Is Nothing Then Exit Sub
        If String.IsNullOrWhiteSpace(rtb.Text) Then
            MessageBox.Show("Le document est vide.", "Aperçu", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Try
            ' Initialisation
            If printDocument Is Nothing Then
                printDocument = New PrintDocument()
            End If

            printDocument.DocumentName = titre
            printRichTextBox = rtb
            checkPrint = 0

            ' Associer l'événement PrintPage
            AddHandler printDocument.PrintPage, AddressOf ImprimerPageRTF

            ' Afficher l'aperçu
            Dim previewDialog As New PrintPreviewDialog()
            previewDialog.Document = printDocument
            previewDialog.Text = $"Aperçu avant impression - {titre}"
            previewDialog.WindowState = FormWindowState.Maximized

            previewDialog.ShowDialog()

            ' Nettoyer l'événement
            RemoveHandler printDocument.PrintPage, AddressOf ImprimerPageRTF

        Catch ex As Exception
            MessageBox.Show($"Erreur lors de l'aperçu : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Méthode    : ConfigurerMiseEnPage
    ' Version    : V1.0
    ' Date       : 17/05/2026
    '
    ' Rôle       :
    ' Affiche le dialogue de mise en page pour configurer format, orientation et marges.
    '
    ' Paramètres : Aucun
    '
    ' Retour     : Aucun (Sub)
    '
    ' Remarques  :
    ' - Permet de configurer les paramètres de page avant impression
    ' - Les paramètres sont mémorisés pour la prochaine impression
    ' - Format A4, A3, Letter, etc. disponibles
    ' - Orientation portrait ou paysage
    ' - Marges personnalisables
    ' -------------------------------------------------------------------------------------------------
    Public Sub ConfigurerMiseEnPage()

        Try
            ' Initialiser le PrintDocument si nécessaire
            If printDocument Is Nothing Then
                printDocument = New PrintDocument()
            End If

            ' Dialogue de mise en page
            Dim pageSetupDialog As New PageSetupDialog()
            pageSetupDialog.Document = printDocument
            pageSetupDialog.AllowMargins = True
            pageSetupDialog.AllowOrientation = True
            pageSetupDialog.AllowPaper = True
            pageSetupDialog.AllowPrinter = True

            ' Afficher le dialogue
            pageSetupDialog.ShowDialog()

        Catch ex As Exception
            MessageBox.Show($"Erreur lors de la configuration de la mise en page : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

#End Region

#Region "Export PDF"

    ' -------------------------------------------------------------------------------------------------
    ' Méthode    : ExporterPDF
    ' Version    : V1.0
    ' Date       : 17/05/2026
    '
    ' Rôle       :
    ' Exporte le contenu RTF d'un RichTextBox vers un fichier PDF avec Syncfusion.
    '
    ' Paramètres :
    ' - rtb           : RichTextBox source
    ' - cheminFichier : Chemin complet du fichier PDF de sortie
    '
    ' Retour     : Boolean (True si succès, False si erreur)
    '
    ' Remarques  :
    ' - Utilise Syncfusion.DocIO pour convertir RTF → PDF
    ' - Préserve le formatage complet (gras, couleurs, alignement, puces, etc.)
    ' - Requiert une licence Syncfusion Community (gratuite) enregistrée dans Program.vb
    ' - Si le fichier existe déjà, il sera écrasé
    '
    ' Exceptions :
    ' - Affiche un MessageBox en cas d'erreur
    ' -------------------------------------------------------------------------------------------------
    Public Function ExporterPDF(rtb As RichTextBox, cheminFichier As String) As Boolean

        If rtb Is Nothing Then
            MessageBox.Show("L'éditeur est invalide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        If String.IsNullOrWhiteSpace(rtb.Text) Then
            MessageBox.Show("Le document est vide.", "Export PDF", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return False
        End If

        If String.IsNullOrWhiteSpace(cheminFichier) Then
            MessageBox.Show("Le chemin du fichier est invalide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Try
            ' Extraire le RTF du RichTextBox
            Dim rtfContent As String = rtb.Rtf

            ' Convertir la chaîne RTF en flux mémoire
            Dim rtfBytes() As Byte = System.Text.Encoding.UTF8.GetBytes(rtfContent)
            Using rtfStream As New System.IO.MemoryStream(rtfBytes)

                ' Charger le RTF dans un WordDocument Syncfusion
                Dim wordDoc As New Syncfusion.DocIO.DLS.WordDocument(rtfStream, Syncfusion.DocIO.FormatType.Rtf)

                ' Créer le convertisseur PDF
                Dim pdfRenderer As New Syncfusion.DocToPDFConverter.DocToPDFConverter()

                ' Convertir le document Word en PDF
                Dim pdfDoc As Syncfusion.Pdf.PdfDocument = pdfRenderer.ConvertToPDF(wordDoc)

                ' Sauvegarder le PDF
                pdfDoc.Save(cheminFichier)

                ' Libérer les ressources
                pdfDoc.Close(True)
                wordDoc.Close()

            End Using

            MessageBox.Show($"Le document a été exporté avec succès :{vbCrLf}{cheminFichier}", "Export PDF réussi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return True

        Catch ex As Exception
            MessageBox.Show($"Erreur lors de l'export PDF :{vbCrLf}{ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Méthode    : ExporterPDFAvecDialogue
    ' Version    : V1.0
    ' Date       : 17/05/2026
    '
    ' Rôle       :
    ' Exporte le contenu RTF vers PDF en affichant un SaveFileDialog pour choisir l'emplacement.
    '
    ' Paramètres :
    ' - rtb            : RichTextBox source
    ' - nomFichierInit : Nom de fichier proposé par défaut (optionnel)
    '
    ' Retour     : Boolean (True si succès, False si annulé ou erreur)
    '
    ' Remarques  :
    ' - Affiche un SaveFileDialog standard Windows
    ' - Filtre : fichiers PDF uniquement
    ' - Appelle ExporterPDF() après validation
    ' -------------------------------------------------------------------------------------------------
    Public Function ExporterPDFAvecDialogue(rtb As RichTextBox, Optional nomFichierInit As String = "Document.pdf") As Boolean

        If rtb Is Nothing Then Return False

        ' Créer le dialogue de sauvegarde
        Using saveDialog As New SaveFileDialog()
            saveDialog.Filter = "Fichiers PDF (*.pdf)|*.pdf"
            saveDialog.Title = "Exporter en PDF"
            saveDialog.FileName = nomFichierInit
            saveDialog.DefaultExt = "pdf"
            saveDialog.AddExtension = True

            ' Afficher le dialogue
            If saveDialog.ShowDialog() = DialogResult.OK Then
                ' Exporter vers le chemin choisi
                Return ExporterPDF(rtb, saveDialog.FileName)
            End If
        End Using

        Return False

    End Function

#End Region

#Region "Export Word"

    ' -------------------------------------------------------------------------------------------------
    ' Méthode    : ExporterWord
    ' Version    : V1.0
    ' Date       : 17/05/2026
    '
    ' Rôle       :
    ' Exporte le contenu RTF d'un RichTextBox vers un fichier Word (.docx) avec Syncfusion.
    '
    ' Paramètres :
    ' - rtb           : RichTextBox source
    ' - cheminFichier : Chemin complet du fichier DOCX de sortie
    '
    ' Retour     : Boolean (True si succès, False si erreur)
    '
    ' Remarques  :
    ' - Utilise Syncfusion.DocIO pour convertir RTF → DOCX
    ' - Préserve le formatage complet (gras, couleurs, alignement, puces, etc.)
    ' - Compatible avec le système de gestion documentaire Althéa (local + Google Drive)
    ' - Requiert une licence Syncfusion Community (gratuite) enregistrée dans Program.vb
    ' - Si le fichier existe déjà, il sera écrasé
    ' - Le fichier .docx généré peut être ouvert/modifié avec Word ou Google Docs
    '
    ' Exceptions :
    ' - Affiche un MessageBox en cas d'erreur
    ' -------------------------------------------------------------------------------------------------
    Public Function ExporterWord(rtb As RichTextBox, cheminFichier As String) As Boolean

        If rtb Is Nothing Then
            MessageBox.Show("L'éditeur est invalide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        If String.IsNullOrWhiteSpace(rtb.Text) Then
            MessageBox.Show("Le document est vide.", "Export Word", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return False
        End If

        If String.IsNullOrWhiteSpace(cheminFichier) Then
            MessageBox.Show("Le chemin du fichier est invalide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Try
            ' Extraire le RTF du RichTextBox
            Dim rtfContent As String = rtb.Rtf

            ' Convertir la chaîne RTF en flux mémoire
            Dim rtfBytes() As Byte = System.Text.Encoding.UTF8.GetBytes(rtfContent)
            Using rtfStream As New System.IO.MemoryStream(rtfBytes)

                ' Charger le RTF dans un WordDocument Syncfusion
                Dim wordDoc As New Syncfusion.DocIO.DLS.WordDocument(rtfStream, Syncfusion.DocIO.FormatType.Rtf)

                ' Sauvegarder au format DOCX
                wordDoc.Save(cheminFichier, Syncfusion.DocIO.FormatType.Docx)

                ' Libérer les ressources
                wordDoc.Close()

            End Using

            MessageBox.Show($"Le document a été exporté avec succès :{vbCrLf}{cheminFichier}", "Export Word réussi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return True

        Catch ex As Exception
            MessageBox.Show($"Erreur lors de l'export Word :{vbCrLf}{ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Méthode    : ExporterWordAvecDialogue
    ' Version    : V1.0
    ' Date       : 17/05/2026
    '
    ' Rôle       :
    ' Exporte le contenu RTF vers Word en affichant un SaveFileDialog pour choisir l'emplacement.
    '
    ' Paramètres :
    ' - rtb            : RichTextBox source
    ' - nomFichierInit : Nom de fichier proposé par défaut (optionnel)
    '
    ' Retour     : Boolean (True si succès, False si annulé ou erreur)
    '
    ' Remarques  :
    ' - Affiche un SaveFileDialog standard Windows
    ' - Filtre : fichiers Word (.docx) uniquement
    ' - Appelle ExporterWord() après validation
    ' - S'intègre avec le système documentaire Althéa (Patients/{id}/Documents/)
    ' -------------------------------------------------------------------------------------------------
    Public Function ExporterWordAvecDialogue(rtb As RichTextBox, Optional nomFichierInit As String = "Document.docx") As Boolean

        If rtb Is Nothing Then Return False

        ' Créer le dialogue de sauvegarde
        Using saveDialog As New SaveFileDialog()
            saveDialog.Filter = "Documents Word (*.docx)|*.docx"
            saveDialog.Title = "Exporter en Word"
            saveDialog.FileName = nomFichierInit
            saveDialog.DefaultExt = "docx"
            saveDialog.AddExtension = True

            ' Afficher le dialogue
            If saveDialog.ShowDialog() = DialogResult.OK Then
                ' Exporter vers le chemin choisi
                Return ExporterWord(rtb, saveDialog.FileName)
            End If
        End Using

        Return False

    End Function

#End Region

End Module
