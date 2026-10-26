' -------------------------------------------------------------------------------------------------
' Form          : DialogChoix
' Projet        : Althéa
' Version       : V2.2.0
' Date          : 19/10/2026
' Auteur        : Joëlle (Manou) / Projet Althéa
'
' Rôle          :
' Boîte de dialogue d'Althéa, à la place des MessageBox (ADR-11) : information, avertissement,
' erreur, succès, question, avec 1 à 3 boutons.
'
' Utilisation   :
' ' Méthodes statiques (inchangées depuis la V1) :
' DialogChoix.Information("Opération réussie")
' DialogChoix.Erreur("Une erreur s'est produite")
' If DialogChoix.Confirmer("Continuer ?") = DialogResult.Yes Then...
'
' ' Boutons qui disent l'action (V2.0.0) :
' If DialogChoix.Confirmer(message, "Clôture", "Clôturer", "Annuler") = DialogResult.Yes Then...
' If DialogChoix.ConfirmerSuppression(message, "Suppression") = DialogResult.Yes Then...
'
' ' Message structuré (V2.0.0) :
' Using dlg As New DialogChoix()
'     dlg.Titre = "Nouvelle règle de quota"
'     dlg.TypeDialogue = TypeDialogue.Question
'     dlg.Message = "Vérifiez la nouvelle règle avant de l'enregistrer :"
'     dlg.AjouterDetail("Plafonds", "Soutien 10 + Traitement 20 = 30")
'     dlg.Complement = "La règle actuelle s'arrêtera le 31/12/2026."
'     dlg.Attention = "Aucune règle ne couvrira du … au …"
'     dlg.Remarque = "La règle restera corrigeable tant qu'aucune séance n'est concernée."
'     dlg.QuestionFinale = "Enregistrer cette règle ?"
'     dlg.SetBoutons("Enregistrer", "Revenir à la saisie")
'     dlg.VarianteAction = VarianteBouton.Ocre
'     If dlg.ShowDialog() = DialogResult.Yes Then...
' End Using
'
' Dépendances   :
' - Kit Althéa : AltheaFondFenetre, AltheaEnTete, AltheaCarte, AltheaBarreActions, AltheaBouton,
'   AltheaIconeEtat ; UITheme
'
' Remarques     :
' - V2.0.0 (14/10/2026) : icônes dessinées (AltheaIconeEtat) à la place des GIF, boutons du kit
'   qui peuvent dire l'action, parties structurées facultatives (détails, complément, encadré
'   d'attention, remarque, question finale), affichées dans cet ordre sous le message.
'   DialogChoix met en forme ; il n'interprète pas le texte du message.
' - V2.1.0 (16/10/2026, maquette « DialogChoix — C ») : la boîte devient une petite fenêtre du
'   kit, comme les autres : barre de titre Windows et fond beige à la feuille (AltheaFondFenetre),
'   en-tête AltheaEnTete (le type se lit sur l'icône, le titre reste mauve), message dans une
'   AltheaCarte, boutons dans une AltheaBarreActions, dans l'ordre habituel de l'application
'   (l'action à gauche, Annuler à droite). Méthodes, propriétés et résultats inchangés.
' - La fenêtre suit son contenu : 420 à 640 px de large, au plus 70 % de l'écran en hauteur ;
'   au-delà, le message défile dans sa carte.
' - Clavier : Entrée = bouton par défaut (l'action, sauf action destructrice : Annuler),
'   Échap ou la croix de la barre de titre = dernier bouton (Annuler / Non / OK).
' - Mapping DialogResult inchangé : Bouton1 = Yes (OK s'il est seul), Bouton2 = No, Bouton3 = Cancel.
' - V2.2.0 (19/10/2026) : DemanderAvantDepart (Enregistrer / Quitter sans enregistrer / Rester) et
'   l'énumération ChoixAvantDepart, pour les modifications non enregistrées (GardeModifications) ;
'   ConfirmerAbandon, même question pour tous les boutons Annuler d'une saisie.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

' Type de dialogue : détermine l'icône de l'en-tête.
Public Enum TypeDialogue
    Information ' Pour les messages d'information généraux
    Warning ' Pour les avertissements
    [Error] ' Pour les erreurs
    Question ' Pour les questions
    Success ' Pour les succès
    Loading ' Pour les opérations en cours
    Processing ' Pour les opérations en cours
End Enum

' -------------------------------------------------------------------------------------------------
' Enum        : ChoixAvantDepart
' Version     : V1.0.0
' Date        : 19/10/2026
'
' Rôle        : Réponse à « modifications non enregistrées » (DialogChoix.DemanderAvantDepart).
' -------------------------------------------------------------------------------------------------
Public Enum ChoixAvantDepart
    Enregistrer
    QuitterSansEnregistrer
    Rester
End Enum

Public Class DialogChoix

#Region "Constantes de présentation"

    Private Const LARGEUR_MIN As Integer = 420
    Private Const LARGEUR_MAX As Integer = 640
    Private Const PART_ECRAN_MAX As Single = 0.7F
    Private Const ECART_PARTIES As Integer = 10
    Private Const ECART_BOUTONS As Integer = 10
    Private Const MARGE_BOUTONS As Integer = 16
    Private Const LARGEUR_BOUTON_MIN As Integer = 96
    Private Const ECART_DETAIL As Integer = 12

    ' Carte du message : marge à gauche et à droite, écart sous l'en-tête et au-dessus des boutons.
    Private Const MARGE_CARTE As Integer = 12
    Private Const ECART_CARTE_HAUT As Integer = 8
    Private Const ECART_CARTE_BAS As Integer = 4

    ' Écart entre l'icône et le titre (celui d'AltheaEnTete).
    Private Const ECART_ICONE_TITRE As Integer = 12

    ' La barre d'actions (AltheaCarte) réserve 8 px en bas pour son ombre : les boutons se centrent
    ' sur la partie visible.
    Private Const RESERVE_OMBRE_BARRE As Integer = 8

    Private Const FLAGS_MESURE As TextFormatFlags =
        TextFormatFlags.WordBreak Or TextFormatFlags.TextBoxControl Or TextFormatFlags.NoPrefix

#End Region

#Region "Propriétés publiques"

    ' Titre affiché dans l'en-tête et dans la barre de titre.
    Public Property Titre As String = "Confirmation"

    ' Message principal.
    Public Property Message As String = ""

    ' Type de dialogue (icône de l'en-tête).
    Public Property TypeDialogue As TypeDialogue = TypeDialogue.Information

    ' Texte complémentaire (facultatif), sous les détails, au même style que le message.
    Public Property Complement As String = ""

    ' Encadré d'attention (facultatif), sous le complément.
    Public Property Attention As String = ""

    ' Remarque discrète (facultative), en petit et en gris.
    Public Property Remarque As String = ""

    ' Question finale (facultative), en gras, juste au-dessus des boutons.
    Public Property QuestionFinale As String = ""

    ' Couleur du bouton d'action (bouton 1) : Sauge par défaut, Ocre pour Enregistrer, Terre pour
    ' une action destructrice (le bouton par défaut devient alors Annuler).
    Public Property VarianteAction As VarianteBouton = VarianteBouton.Sauge

#End Region

#Region "Variables privées"

    ' Nombre de boutons configurés (1, 2 ou 3).
    Private _nombreBoutons As Integer = 3

    ' Liste « libellé : valeur » affichée dans un encadré (facultative).
    Private ReadOnly _details As New List(Of (Libelle As String, Valeur As String))

    ' Polices des lignes de détail (libérées avec la fenêtre : un Label ne libère pas sa police).
    Private ReadOnly _policeLibelleDetail As New Font("Calibri", 11.25F, FontStyle.Bold)
    Private ReadOnly _policeValeurDetail As New Font("Calibri", 11.25F)

    ' Icône d'état de l'en-tête (image créée selon le type, libérée avec la fenêtre).
    Private _imageEtat As Bitmap

    ' Vrai dès qu'un bouton ou Échap a fixé le résultat : la croix de la barre de titre, elle,
    ' doit donner le même résultat qu'Échap.
    Private _resultatChoisi As Boolean

#End Region

#Region "Constructeur"

    Public Sub New()
        InitializeComponent()

        ' Fond beige à la feuille et barre de titre aux couleurs du kit.
        AltheaFondFenetre.Appliquer(Me)

        AddHandler Disposed, AddressOf LibererRessources
    End Sub

    Private Sub LibererRessources(sender As Object, e As EventArgs)
        _policeLibelleDetail.Dispose()
        _policeValeurDetail.Dispose()
        _imageEtat?.Dispose()
    End Sub

#End Region

#Region "Configuration"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetBoutons
    ' Version    : V1.1.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Configure 1, 2 ou 3 boutons. Mapping fixe : Bouton1 = Yes (OK s'il est seul), Bouton2 = No,
    ' Bouton3 = Cancel. Affichés de gauche à droite dans cet ordre, groupe aligné à droite :
    ' l'action d'abord, Annuler en dernier (ordre habituel de l'application).
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetBoutons(bouton1 As String)
        btn1.Text = bouton1
        btn2.Visible = False
        btn3.Visible = False
        _nombreBoutons = 1
    End Sub

    Public Sub SetBoutons(bouton1 As String, bouton2 As String)
        btn1.Text = bouton1
        btn2.Text = bouton2
        btn2.Visible = True
        btn3.Visible = False
        _nombreBoutons = 2
    End Sub

    Public Sub SetBoutons(bouton1 As String, bouton2 As String, bouton3 As String)
        btn1.Text = bouton1
        btn2.Text = bouton2
        btn3.Text = bouton3
        btn2.Visible = True
        btn3.Visible = True
        _nombreBoutons = 3
    End Sub

    ' Ajoute une ligne « libellé : valeur » à l'encadré de détails.
    Public Sub AjouterDetail(libelle As String, valeur As String)
        _details.Add((If(libelle, String.Empty), If(valeur, String.Empty)))
    End Sub

#End Region

#Region "Affichage"

    Private Sub DialogChoix_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Text = Titre
        enTete.Text = Titre
        lblMessage.Text = Message

        AppliquerType()
        PreparerBoutons()
        DisposerContenu()

    End Sub

    ' Icône d'état de l'en-tête selon le type ; encadré d'attention aux couleurs de l'avertissement.
    Private Sub AppliquerType()

        ' Image dessinée au double de sa taille d'affichage : reste nette une fois réduite.
        _imageEtat?.Dispose()
        _imageEtat = AltheaIconeEtat.CreerImage(EtatDuType(TypeDialogue), enTete.TailleIcone * 2)
        enTete.Icone = _imageEtat

        Dim teinteAttention = AltheaIconeEtat.Teintes(EtatIcone.Avertissement)
        carteAttention.CouleurFond = teinteAttention.Pale
        lblAttention.ForeColor = teinteAttention.Foncee

    End Sub

    Private Shared Function EtatDuType(valeur As TypeDialogue) As EtatIcone

        Select Case valeur
            Case TypeDialogue.Warning : Return EtatIcone.Avertissement
            Case TypeDialogue.Error : Return EtatIcone.Erreur
            Case TypeDialogue.Question : Return EtatIcone.Question
            Case TypeDialogue.Success : Return EtatIcone.Succes
            Case TypeDialogue.Loading, TypeDialogue.Processing : Return EtatIcone.Attente
            Case Else : Return EtatIcone.Information
        End Select

    End Function

    ' Couleurs et rôle clavier des boutons.
    Private Sub PreparerBoutons()

        btn1.Variante = VarianteAction
        btn2.Variante = VarianteBouton.Clair
        btn3.Variante = VarianteBouton.Clair

        ' Action destructrice : Entrée ne doit pas la déclencher par mégarde.
        Dim parDefaut As AltheaBouton = If(VarianteAction = VarianteBouton.Terre AndAlso _nombreBoutons > 1, btn2, btn1)
        AcceptButton = parDefaut
        ActiveControl = parDefaut

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DisposerContenu
    ' Version    : V2.1.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Calcule la largeur de la fenêtre (420 à 640 px, selon le texte, le titre et les boutons),
    ' place les parties du message les unes sous les autres dans la carte, puis fixe la hauteur
    ' (au plus 70 % de l'écran : au-delà, le message défile dans sa carte).
    '
    ' Remarques  :
    ' - Si le message défile, il est replacé un peu moins large pour laisser la place à la barre de
    '   défilement.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DisposerContenu()

        Dim margesMessage As Integer = 2 * MARGE_CARTE + carteMessage.Padding.Horizontal
        Dim totalBoutons As Integer = LargeurBoutons()

        ' Largeur naturelle : la ligne la plus longue ou le titre, bornée ; jamais moins que les boutons.
        Dim naturelle As Integer = Math.Max(LargeurNaturelleContenu() + margesMessage, LargeurTitreNecessaire())
        Dim largeurFenetre As Integer = Math.Max(LARGEUR_MIN, Math.Min(LARGEUR_MAX, naturelle))
        largeurFenetre = Math.Max(largeurFenetre, totalBoutons + 2 * MARGE_BOUTONS)

        Dim largeurContenu As Integer = largeurFenetre - margesMessage
        Dim hauteurContenu As Integer = PlacerParties(largeurContenu)

        ' Hauteur bornée par l'écran : au-delà, le message défile.
        Dim ecran As Rectangle = Screen.FromPoint(Cursor.Position).WorkingArea
        Dim bordures As Integer = Height - ClientSize.Height
        Dim hauteurFixe As Integer = enTete.Height + ECART_CARTE_HAUT + carteMessage.Padding.Vertical +
                                     ECART_CARTE_BAS + pnlActions.Height
        Dim hauteurContenuMax As Integer = Math.Max(80, CInt(ecran.Height * PART_ECRAN_MAX) - bordures - hauteurFixe)
        Dim defile As Boolean = hauteurContenu > hauteurContenuMax

        If defile Then
            hauteurContenu = PlacerParties(largeurContenu - SystemInformation.VerticalScrollBarWidth)
            pnlCorps.AutoScrollMinSize = New Size(0, hauteurContenu)
        End If
        pnlCorps.AutoScroll = defile

        Dim hauteurCarte As Integer = Math.Min(hauteurContenu, hauteurContenuMax) + carteMessage.Padding.Vertical

        ClientSize = New Size(largeurFenetre,
                              enTete.Height + ECART_CARTE_HAUT + hauteurCarte + ECART_CARTE_BAS + pnlActions.Height)
        carteMessage.SetBounds(MARGE_CARTE, enTete.Bottom + ECART_CARTE_HAUT,
                               largeurFenetre - 2 * MARGE_CARTE, hauteurCarte)
        PerformLayout()

        PlacerBoutons()
        Recentrer()

    End Sub

    ' Parties visibles du message, les unes sous les autres ; renvoie la hauteur occupée.
    Private Function PlacerParties(largeur As Integer) As Integer

        Dim y As Integer = 0
        Dim premier As Boolean = True

        y = PlacerTexte(lblMessage, Message, y, largeur, premier)
        y = PlacerDetails(y, largeur, premier)
        y = PlacerTexte(lblComplement, Complement, y, largeur, premier)
        y = PlacerAttention(y, largeur, premier)
        y = PlacerTexte(lblRemarque, Remarque, y, largeur, premier)
        y = PlacerTexte(lblQuestionFinale, QuestionFinale, y, largeur, premier)

        Return y

    End Function

    ' La taille change au chargement, après le placement initial : on recentre sur la fenêtre
    ' appelante (ou l'écran), sans déborder de la zone de travail.
    Private Sub Recentrer()

        Dim appelante As Form = If(Owner, Form.ActiveForm)
        Dim zone As Rectangle = If(appelante IsNot Nothing AndAlso appelante IsNot Me,
                                   appelante.Bounds, Screen.FromPoint(Cursor.Position).WorkingArea)
        Dim ecran As Rectangle = Screen.FromRectangle(zone).WorkingArea

        Dim x As Integer = zone.Left + (zone.Width - Width) \ 2
        Dim y As Integer = zone.Top + (zone.Height - Height) \ 2
        x = Math.Max(ecran.Left, Math.Min(x, ecran.Right - Width))
        y = Math.Max(ecran.Top, Math.Min(y, ecran.Bottom - Height))

        StartPosition = FormStartPosition.Manual
        Location = New Point(x, y)

    End Sub

    ' Place un texte simple (masqué s'il est vide) ; renvoie l'ordonnée suivante.
    Private Function PlacerTexte(lbl As Label, texte As String, y As Integer, largeur As Integer, ByRef premier As Boolean) As Integer

        ' La fenêtre n'est pas encore affichée : on décide de la visibilité, sans la relire.
        Dim affiche As Boolean = Not String.IsNullOrWhiteSpace(texte)
        lbl.Visible = affiche
        If Not affiche Then Return y

        lbl.Text = texte
        If Not premier Then y += ECART_PARTIES
        Dim h As Integer = MesurerHauteur(texte, lbl.Font, largeur)
        lbl.SetBounds(0, y, largeur, h)
        premier = False
        Return y + h

    End Function

    ' Encadré « libellé : valeur » : libellés alignés sur une colonne, valeurs à la ligne si besoin.
    Private Function PlacerDetails(y As Integer, largeur As Integer, ByRef premier As Boolean) As Integer

        carteDetails.Visible = _details.Count > 0
        If _details.Count = 0 Then Return y

        For Each ctrl As Control In carteDetails.Controls.Cast(Of Control)().ToList()
            ctrl.Dispose()
        Next

        Dim interieur As Padding = carteDetails.Padding
        Dim colonneLibelle As Integer = _details.Max(Function(d) TextRenderer.MeasureText(d.Libelle, _policeLibelleDetail).Width)
        Dim largeurValeur As Integer = Math.Max(60, largeur - interieur.Horizontal - colonneLibelle - ECART_DETAIL)
        Dim yLigne As Integer = interieur.Top

        For Each detail In _details

            Dim hValeur As Integer = MesurerHauteur(detail.Valeur, _policeValeurDetail, largeurValeur)
            Dim hLibelle As Integer = MesurerHauteur(detail.Libelle, _policeLibelleDetail, colonneLibelle)

            carteDetails.Controls.Add(CreerLabel(detail.Libelle, _policeLibelleDetail, UITheme.ColorSaugeFonce,
                                                 New Rectangle(interieur.Left, yLigne, colonneLibelle, hLibelle)))
            carteDetails.Controls.Add(CreerLabel(detail.Valeur, _policeValeurDetail, UITheme.ColorGrisFonce,
                                                 New Rectangle(interieur.Left + colonneLibelle + ECART_DETAIL, yLigne, largeurValeur, hValeur)))

            yLigne += Math.Max(hLibelle, hValeur) + 4

        Next

        If Not premier Then y += ECART_PARTIES
        carteDetails.SetBounds(0, y, largeur, yLigne - 4 + interieur.Bottom)
        premier = False
        Return carteDetails.Bottom

    End Function

    ' Encadré d'attention : petite icône d'avertissement, texte ocre.
    Private Function PlacerAttention(y As Integer, largeur As Integer, ByRef premier As Boolean) As Integer

        Dim affiche As Boolean = Not String.IsNullOrWhiteSpace(Attention)
        carteAttention.Visible = affiche
        If Not affiche Then Return y

        Dim interieur As Padding = carteAttention.Padding
        Dim xTexte As Integer = interieur.Left + iconeAttention.Width + 10
        Dim largeurTexte As Integer = largeur - xTexte - interieur.Right
        Dim hTexte As Integer = MesurerHauteur(Attention, lblAttention.Font, largeurTexte)

        lblAttention.Text = Attention
        lblAttention.SetBounds(xTexte, interieur.Top, largeurTexte, hTexte)
        iconeAttention.Location = New Point(interieur.Left, interieur.Top + 1)

        If Not premier Then y += ECART_PARTIES
        carteAttention.SetBounds(0, y, largeur, interieur.Vertical + Math.Max(hTexte, iconeAttention.Height))
        premier = False
        Return carteAttention.Bottom

    End Function

    ' Boutons de gauche à droite : 1 (l'action), 2, 3 ; groupe aligné à droite, largeur selon le texte.
    Private Sub PlacerBoutons()

        Dim x As Integer = pnlActions.ClientSize.Width - MARGE_BOUTONS - LargeurBoutons()
        Dim yBouton As Integer = 1 + (pnlActions.ClientSize.Height - RESERVE_OMBRE_BARRE - btn1.Height) \ 2

        For Each btn As AltheaBouton In BoutonsAffiches()
            btn.Width = LargeurBouton(btn)
            btn.Location = New Point(x, yBouton)
            x += btn.Width + ECART_BOUTONS
        Next

    End Sub

#End Region

#Region "Mesures"

    Private Function LargeurNaturelleContenu() As Integer

        Dim largeur As Integer = 0

        For Each texte In {Message, Complement, Remarque, QuestionFinale}
            largeur = Math.Max(largeur, LargeurLigneLaPlusLongue(texte, lblMessage.Font))
        Next

        If Not String.IsNullOrWhiteSpace(Attention) Then
            largeur = Math.Max(largeur, LargeurLigneLaPlusLongue(Attention, lblAttention.Font) +
                                        carteAttention.Padding.Horizontal + iconeAttention.Width + 10)
        End If

        If _details.Count > 0 Then
            Dim colonne As Integer = _details.Max(Function(d) TextRenderer.MeasureText(d.Libelle, _policeLibelleDetail).Width)
            Dim valeur As Integer = _details.Max(Function(d) TextRenderer.MeasureText(d.Valeur, _policeValeurDetail).Width)
            largeur = Math.Max(largeur, colonne + ECART_DETAIL + valeur + carteDetails.Padding.Horizontal)
        End If

        Return largeur

    End Function

    ' Largeur de fenêtre nécessaire au titre : icône, titre, puis la place de la feuille dessinée
    ' dans le coin haut droit (le titre ne doit pas passer dessous).
    Private Function LargeurTitreNecessaire() As Integer

        Dim placeFeuille As Integer = CInt(AltheaFondFenetre.LargeurZone * DeviceDpi / 96.0F)

        Return enTete.Padding.Left + enTete.TailleIcone + ECART_ICONE_TITRE +
               TextRenderer.MeasureText(Titre, enTete.PoliceTitre).Width + placeFeuille

    End Function

    Private Function LargeurBoutons() As Integer

        Dim total As Integer = 0
        For Each btn As AltheaBouton In BoutonsAffiches()
            total += LargeurBouton(btn) + ECART_BOUTONS
        Next
        Return Math.Max(0, total - ECART_BOUTONS)

    End Function

    ' Boutons configurés, dans l'ordre 1, 2, 3 (Visible ne se relit pas avant l'affichage).
    Private Function BoutonsAffiches() As AltheaBouton()
        Return {btn1, btn2, btn3}.Take(_nombreBoutons).ToArray()
    End Function

    Private Shared Function LargeurBouton(btn As AltheaBouton) As Integer
        Return Math.Max(LARGEUR_BOUTON_MIN, TextRenderer.MeasureText(btn.Text, btn.Font).Width + 36)
    End Function

    Private Shared Function LargeurLigneLaPlusLongue(texte As String, police As Font) As Integer

        If String.IsNullOrEmpty(texte) Then Return 0
        Dim lignes As String() = texte.Replace(vbCrLf, vbLf).Split(ChrW(10))
        Return lignes.Max(Function(l) TextRenderer.MeasureText(l, police, Size.Empty, TextFormatFlags.NoPrefix).Width)

    End Function

    Private Shared Function MesurerHauteur(texte As String, police As Font, largeur As Integer) As Integer
        Return TextRenderer.MeasureText(If(texte, String.Empty), police, New Size(Math.Max(1, largeur), Integer.MaxValue), FLAGS_MESURE).Height
    End Function

    Private Shared Function CreerLabel(texte As String, police As Font, couleur As Color, limites As Rectangle) As Label

        Dim lbl As New Label() With {
            .AutoSize = False,
            .BackColor = Color.Transparent,
            .Font = police,
            .ForeColor = couleur,
            .Text = texte,
            .UseMnemonic = False
        }
        lbl.Bounds = limites
        Return lbl

    End Function

#End Region

#Region "Clavier et fermeture"

    ' Résultat d'Échap et de la croix : le dernier bouton affiché (Annuler, Non, ou OK s'il est seul).
    Private Function ResultatAbandon() As DialogResult

        Select Case _nombreBoutons
            Case 1 : Return DialogResult.OK
            Case 2 : Return DialogResult.No
            Case Else : Return DialogResult.Cancel
        End Select

    End Function

    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, keyData As Keys) As Boolean

        If keyData = Keys.Escape Then
            ChoisirResultat(ResultatAbandon())
            Return True
        End If

        Return MyBase.ProcessCmdKey(msg, keyData)

    End Function

    ' Croix de la barre de titre (ou Alt+F4) : Windows renverrait Cancel, quel que soit le nombre de
    ' boutons ; on renvoie le même résultat qu'Échap pour que l'appelant n'ait qu'un cas à traiter.
    Private Sub DialogChoix_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not _resultatChoisi Then DialogResult = ResultatAbandon()
    End Sub

    Private Sub ChoisirResultat(resultat As DialogResult)
        _resultatChoisi = True
        DialogResult = resultat
    End Sub

#End Region

#Region "Actions boutons"

    Private Sub btn1_Click(sender As Object, e As EventArgs) Handles btn1.Click
        ChoisirResultat(If(_nombreBoutons = 1, DialogResult.OK, DialogResult.Yes))
    End Sub

    Private Sub btn2_Click(sender As Object, e As EventArgs) Handles btn2.Click
        ChoisirResultat(DialogResult.No)
    End Sub

    Private Sub btn3_Click(sender As Object, e As EventArgs) Handles btn3.Click
        ChoisirResultat(DialogResult.Cancel)
    End Sub

#End Region

#Region "Méthodes statiques facilitantes"

    ' Message d'information, bouton OK.
    Public Shared Sub Information(message As String, Optional titre As String = "Information")
        Afficher(message, titre, TypeDialogue.Information)
    End Sub

    ' Avertissement, bouton OK.
    Public Shared Sub Avertissement(message As String, Optional titre As String = "Avertissement")
        Afficher(message, titre, TypeDialogue.Warning)
    End Sub

    ' Erreur, bouton OK. Le détail technique va au journal, jamais dans le message.
    Public Shared Sub Erreur(message As String, Optional titre As String = "Erreur")
        Afficher(message, titre, TypeDialogue.Error)
    End Sub

    ' Succès, bouton OK.
    Public Shared Sub Succes(message As String, Optional titre As String = "Succès")
        Afficher(message, titre, TypeDialogue.Success)
    End Sub

    ' Question Oui / Non (V1, conservée pour les appels existants).
    Public Shared Function Confirmer(message As String, Optional titre As String = "Confirmation") As DialogResult
        Return Confirmer(message, titre, "Oui", "Non")
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : Confirmer (Shared, V2.0.0)
    ' Rôle       : Question dont les boutons disent l'action (« Clôturer » / « Annuler »…).
    ' Retour     : Yes = action, No = annulation (Échap et croix compris).
    ' -------------------------------------------------------------------------------------------------
    ' - attention : encadré d'avertissement facultatif sous le message.
    Public Shared Function Confirmer(message As String, titre As String, texteAction As String, texteAnnuler As String,
                                     Optional variante As VarianteBouton = VarianteBouton.Sauge,
                                     Optional attention As String = "") As DialogResult

        Using dlg As New DialogChoix()
            dlg.Titre = titre
            dlg.Message = message
            dlg.Attention = attention
            dlg.TypeDialogue = TypeDialogue.Question
            dlg.VarianteAction = variante
            dlg.SetBoutons(texteAction, texteAnnuler)
            Return dlg.ShowDialog()
        End Using

    End Function

    ' Confirmation d'une suppression : bouton « Supprimer » en Terre, Annuler par défaut.
    Public Shared Function ConfirmerSuppression(message As String, Optional titre As String = "Suppression",
                                                Optional attention As String = "") As DialogResult
        Return Confirmer(message, titre, "Supprimer", "Annuler", VarianteBouton.Terre, attention)
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : DemanderAvantDepart (Shared)
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Avant de quitter un écran ou une fenêtre modifiés : Enregistrer (bouton par défaut,
    '              Entrée), Quitter sans enregistrer, Rester (Échap et croix : on ne perd rien).
    ' Paramètres :
    ' - deQuoi : Ce qui serait perdu, précédé de « de » (« de la séance », « du contact »,
    '            « de l'anamnèse »…).
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function DemanderAvantDepart(deQuoi As String) As ChoixAvantDepart

        Using dlg As New DialogChoix()
            dlg.Titre = "Modifications non enregistrées"
            dlg.Message = $"Les modifications {deQuoi} n'ont pas été enregistrées." & Environment.NewLine &
                          "Voulez-vous les enregistrer avant de quitter ?"
            dlg.TypeDialogue = TypeDialogue.Question
            dlg.SetBoutons("Enregistrer", "Quitter sans enregistrer", "Rester")

            Select Case dlg.ShowDialog()
                Case DialogResult.Yes
                    Return ChoixAvantDepart.Enregistrer
                Case DialogResult.No
                    Return ChoixAvantDepart.QuitterSansEnregistrer
                Case Else
                    Return ChoixAvantDepart.Rester
            End Select
        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : ConfirmerAbandon (Shared)
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Bouton Annuler d'une saisie modifiée : Abandonner (Terre) / Continuer la saisie (par
    '              défaut : Entrée, Échap et croix ne perdent rien). Même question dans toute l'application.
    ' Paramètres :
    ' - deQuoi : Ce qui serait perdu, précédé de « de » (« de la fiche du patient », « du domaine »…).
    ' Retour     : True si l'utilisateur abandonne ses modifications.
    ' -------------------------------------------------------------------------------------------------
    Public Shared Function ConfirmerAbandon(deQuoi As String) As Boolean

        Dim message As String = $"Abandonner les modifications {deQuoi} ?" & Environment.NewLine &
                                "Les changements non enregistrés seront perdus."

        Return Confirmer(message, "Annuler les modifications", "Abandonner", "Continuer la saisie",
                         VarianteBouton.Terre) = DialogResult.Yes

    End Function

    ' Question Oui / Non / Annuler.
    Public Shared Function Question(message As String, Optional titre As String = "Question") As DialogResult

        Using dlg As New DialogChoix()
            dlg.Titre = titre
            dlg.Message = message
            dlg.TypeDialogue = TypeDialogue.Question
            dlg.SetBoutons("Oui", "Non", "Annuler")
            Return dlg.ShowDialog()
        End Using

    End Function

    Private Shared Sub Afficher(message As String, titre As String, typeMessage As TypeDialogue)

        Using dlg As New DialogChoix()
            dlg.Titre = titre
            dlg.Message = message
            dlg.TypeDialogue = typeMessage
            dlg.SetBoutons("OK")
            dlg.ShowDialog()
        End Using

    End Sub

#End Region

End Class
