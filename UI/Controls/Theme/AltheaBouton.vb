' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaBouton
' Projet     : Althéa
' Version    : V1.3.0
' Date       : 20/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Bouton du kit Althéa entièrement dessiné par le code : coins arrondis, couleurs de la charte,
' états normal / survol / enfoncé / désactivé, sans aucune image de fond.
'
' Responsabilités :
' - Dessiner le fond arrondi selon la Variante (Sauge, Ocre, Terre, Clair)
' - Dessiner l'icône (propriété Image) et le texte
' - Gérer les états visuels (survol, enfoncé, désactivé, focus clavier)
'
' Remarques  :
' - Hérite de Button : Click, DialogResult, AcceptButton/CancelButton, Tag, Image restent
'   utilisables comme avant. Les icônes _normal / _hover / _disabled posées par UtilsButtons
'   sont simplement dessinées.
' - BackColor est forcé à Transparent (coins arrondis) : un BackColor posé par le code ou par
'   UtilsButtons est ignoré, la couleur vient de la Variante.
' - Variantes :
'   * Sauge  : actions courantes (Nouveau, Modifier, Actualiser…)
'   * Ocre   : Enregistrer
'   * Terre  : Supprimer
'   * Clair  : actions secondaires (Annuler, Réinitialiser, Fermer…), contour sauge
'   * Outil, OutilPrincipal, OutilDanger : boutons des barres locales (AltheaBarreOutils) ;
'     sans cadre ni ombre, pour ne pas être confondus avec la barre d'actions de l'écran.
' - Bouton icône seule (Text vide) : l'icône est centrée et occupe presque toute la hauteur ;
'   c'est la forme des petits boutons [+] / loupe posés à côté d'un champ (Outil, V1.2.0).
' - V1.3.0 (20/10/2026) : inactif lisible (texte plus foncé, icône à 55 %) ; RaisonInactif et
'   DefinirDisponibilite : un bouton inactif dit pourquoi au survol (relevés de Joëlle, 16/10/2026).
'
' Dépendances :
' - UITheme, AltheaDessin
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

' Variante de couleur d'un AltheaBouton / AltheaTuile.
Public Enum VarianteBouton
    Sauge = 0
    Ocre = 1
    Terre = 2
    Clair = 3
    ''' <summary>Barre locale : sans cadre, texte sauge, fond vert pâle au survol.</summary>
    Outil = 4
    ''' <summary>Barre locale : action principale (Ajouter, Nouvelle…), fond vert pâle.</summary>
    OutilPrincipal = 5
    ''' <summary>Barre locale : action destructive (Supprimer), texte terre.</summary>
    OutilDanger = 6
End Enum

<DesignerCategory("Code")>
<ToolboxItem(True)>
<Description("Bouton Althéa dessiné par le code (coins arrondis, variantes Sauge / Ocre / Terre / Clair).")>
Public Class AltheaBouton
    Inherits Button

#Region "Variables privées"

    Private _variante As VarianteBouton = VarianteBouton.Sauge
    Private _rayon As Integer = 8
    Private _survol As Boolean
    Private _enfonce As Boolean

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.SupportsTransparentBackColor, True)

        FlatStyle = FlatStyle.Flat

        ' ButtonBase se déclare « Opaque » (il peint lui-même son fond) : on le désactive pour que
        ' WinForms peigne le fond du parent derrière les coins arrondis.
        SetStyle(ControlStyles.Opaque, False)
        FlatAppearance.BorderSize = 0
        UseVisualStyleBackColor = False
        MyBase.BackColor = Color.Transparent
        ForeColor = Color.White
        Font = New Font("Calibri", 11.0F, FontStyle.Bold)
        Cursor = Cursors.Hand
        Size = New Size(130, 40)

    End Sub

#End Region

#Region "Propriétés publiques"

    <Category("Althéa")>
    <Description("Couleur du bouton : Sauge (courant), Ocre (Enregistrer), Terre (Supprimer), Clair (secondaire).")>
    <DefaultValue(GetType(VarianteBouton), "Sauge")>
    Public Property Variante As VarianteBouton
        Get
            Return _variante
        End Get
        Set(value As VarianteBouton)
            If _variante <> value Then
                _variante = value
                Invalidate()
            End If
        End Set
    End Property

    <Category("Althéa")>
    <Description("Rayon des coins arrondis, en pixels.")>
    <DefaultValue(8)>
    Public Property Rayon As Integer
        Get
            Return _rayon
        End Get
        Set(value As Integer)
            Dim v As Integer = Math.Max(0, value)
            If _rayon <> v Then
                _rayon = v
                Invalidate()
            End If
        End Set
    End Property

    ' Toujours transparent : la couleur vient de la Variante (coins arrondis sans rectangle opaque).
    <Browsable(False)>
    <EditorBrowsable(EditorBrowsableState.Never)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overrides Property BackColor As Color
        Get
            Return Color.Transparent
        End Get
        Set(value As Color)
            ' Ignoré volontairement.
        End Set
    End Property

#End Region

#Region "État visuel (pour les classes dérivées)"

    Protected ReadOnly Property EstSurvole As Boolean
        Get
            Return _survol AndAlso Enabled
        End Get
    End Property

    Protected ReadOnly Property EstEnfonce As Boolean
        Get
            Return _enfonce AndAlso _survol AndAlso Enabled
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : CalculerCouleurs
    ' Rôle       : Couleurs de fond, de bordure et de texte selon la Variante et l'état.
    ' -------------------------------------------------------------------------------------------------
    ' Variantes des barres locales (Outil, OutilPrincipal, OutilDanger).
    Private ReadOnly Property EstOutil As Boolean
        Get
            Return _variante = VarianteBouton.Outil OrElse
                   _variante = VarianteBouton.OutilPrincipal OrElse
                   _variante = VarianteBouton.OutilDanger
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : CalculerCouleursOutil
    ' Version    : V1.0.0
    ' Date       : 11/10/2026
    '
    ' Rôle       :
    ' Couleurs des boutons de barre locale : pas de cadre ni d'ombre ; seul le fond apparaît au
    ' survol (ou en permanence pour l'action principale).
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Private Sub CalculerCouleursOutil(ByRef fond As Color, ByRef bordure As Color, ByRef texte As Color)

        bordure = Color.Transparent

        If Not Enabled Then
            fond = Color.Transparent
            texte = AltheaDessin.CouleurDesactiveTexteBouton
            Return
        End If

        If _variante = VarianteBouton.OutilDanger Then
            texte = AltheaDessin.Assombrir(UITheme.ColorRougeMoyenClair, 0.2F)
            If EstEnfonce Then
                fond = AltheaDessin.Melanger(Color.White, UITheme.ColorRougeMoyenClair, 0.35F)
            ElseIf EstSurvole Then
                fond = AltheaDessin.Melanger(Color.White, UITheme.ColorRougeMoyenClair, 0.18F)
            Else
                fond = Color.Transparent
            End If
            Return
        End If

        texte = UITheme.ColorSaugeTresFonce

        If EstEnfonce Then
            fond = UITheme.ColorSaugeClair
        ElseIf EstSurvole OrElse _variante = VarianteBouton.OutilPrincipal Then
            fond = If(_variante = VarianteBouton.OutilPrincipal AndAlso EstSurvole,
                      AltheaDessin.Melanger(UITheme.ColorGrisVertClair, UITheme.ColorSaugeClair, 0.5F),
                      UITheme.ColorGrisVertClair)
        Else
            fond = Color.Transparent
        End If

    End Sub

    Protected Sub CalculerCouleurs(ByRef fond As Color, ByRef bordure As Color, ByRef texte As Color)

        If EstOutil Then
            CalculerCouleursOutil(fond, bordure, texte)
            Return
        End If

        If Not Enabled Then
            If _variante = VarianteBouton.Clair Then
                fond = Color.Transparent
                bordure = AltheaDessin.CouleurDesactiveBordure
            Else
                fond = AltheaDessin.CouleurDesactiveFond
                bordure = Color.Transparent
            End If
            texte = AltheaDessin.CouleurDesactiveTexteBouton
            Return
        End If

        Dim baseCouleur As Color

        Select Case _variante
            Case VarianteBouton.Ocre
                baseCouleur = UITheme.ColorJauneModere
            Case VarianteBouton.Terre
                baseCouleur = UITheme.ColorRougeMoyenClair
            Case VarianteBouton.Clair
                baseCouleur = UITheme.ColorSauge
            Case Else
                baseCouleur = UITheme.ColorSauge
        End Select

        If _variante = VarianteBouton.Clair Then

            bordure = UITheme.ColorSauge
            texte = UITheme.ColorSaugeTresFonce

            If EstEnfonce Then
                fond = UITheme.ColorSaugeClair
            ElseIf EstSurvole Then
                fond = UITheme.ColorGrisVertClair
            Else
                fond = AltheaDessin.AvecAlpha(UITheme.ColorBeigeClair, 215)
            End If

            Return

        End If

        bordure = Color.Transparent
        texte = Color.White

        If _variante = VarianteBouton.Sauge Then
            ' La charte a déjà les teintes foncées du sauge.
            If EstEnfonce Then
                fond = UITheme.ColorSaugeTresFonce
            ElseIf EstSurvole Then
                fond = UITheme.ColorSaugeFonce
            Else
                fond = UITheme.ColorSauge
            End If
        Else
            If EstEnfonce Then
                fond = AltheaDessin.Assombrir(baseCouleur, 0.22F)
            ElseIf EstSurvole Then
                fond = AltheaDessin.Assombrir(baseCouleur, 0.1F)
            Else
                fond = baseCouleur
            End If
        End If

    End Sub

#End Region

#Region "Dessin"

    Protected Overrides Sub OnPaint(pevent As PaintEventArgs)

        Dim g As Graphics = pevent.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.PixelOffsetMode = PixelOffsetMode.HighQuality
        g.InterpolationMode = InterpolationMode.HighQualityBicubic

        Dim fond As Color, bordure As Color, texte As Color
        CalculerCouleurs(fond, bordure, texte)

        ' 1 px réservé en bas pour l'ombre légère.
        Dim rect As New RectangleF(0.5F, 0.5F, Width - 1.0F, Height - 2.0F)

        If Enabled AndAlso _variante <> VarianteBouton.Clair AndAlso Not EstOutil AndAlso Not EstEnfonce Then
            Using ombre As GraphicsPath = AltheaDessin.CheminArrondi(New RectangleF(rect.X, rect.Y + 1.0F, rect.Width, rect.Height), _rayon)
                Using pinceau As New SolidBrush(Color.FromArgb(64, UITheme.ColorSaugeTresFonce))
                    g.FillPath(pinceau, ombre)
                End Using
            End Using
        End If

        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(rect, _rayon)

            If fond.A > 0 Then
                Using pinceau As New SolidBrush(fond)
                    g.FillPath(pinceau, chemin)
                End Using
            End If

            If bordure.A > 0 Then
                Using stylo As New Pen(bordure, 1.0F)
                    g.DrawPath(stylo, chemin)
                End Using
            End If

        End Using

        DessinerContenu(g, Rectangle.Round(rect), texte)

        ' Repère de focus clavier (navigation au clavier uniquement).
        If Focused AndAlso ShowFocusCues Then
            Dim couleurFocus As Color = If(_variante = VarianteBouton.Clair OrElse EstOutil OrElse Not Enabled,
                                           UITheme.ColorSaugeFonce,
                                           Color.FromArgb(170, Color.White))
            Dim rFocus As RectangleF = RectangleF.Inflate(rect, -3.0F, -3.0F)
            Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(rFocus, Math.Max(0, _rayon - 3))
                Using stylo As New Pen(couleurFocus, 1.0F)
                    stylo.DashStyle = DashStyle.Dot
                    g.DrawPath(stylo, chemin)
                End Using
            End Using
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : DessinerContenu
    ' Rôle       : Icône + texte. TextAlign à gauche : contenu calé à gauche ; sinon centré.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DessinerContenu(g As Graphics, zone As Rectangle, couleurTexte As Color)

        Dim img As Image = Image
        Dim tailleIcone As Integer = 0

        ' Icône seule : pas de marges de texte à réserver, l'icône peut être plus grande.
        Dim iconeSeule As Boolean = img IsNot Nothing AndAlso String.IsNullOrEmpty(Text)

        If img IsNot Nothing Then
            Dim reserve As Integer = If(iconeSeule, 6, 12)
            tailleIcone = Math.Min(Math.Max(img.Width, img.Height), Math.Max(0, zone.Height - reserve))
        End If

        Dim flags As TextFormatFlags = TextFormatFlags.VerticalCenter Or
                                       TextFormatFlags.WordBreak Or
                                       TextFormatFlags.EndEllipsis Or
                                       TextFormatFlags.NoPadding
        If Not ShowKeyboardCues Then flags = flags Or TextFormatFlags.HidePrefix

        Dim margeG As Integer = Math.Max(10, Padding.Left)
        Dim margeD As Integer = Math.Max(12, Padding.Right)
        Dim ecart As Integer = If(tailleIcone > 0 AndAlso Not String.IsNullOrEmpty(Text), 8, 0)

        Dim largeurDispo As Integer = zone.Width - margeG - margeD - tailleIcone - ecart
        Dim tailleTexte As Size = Size.Empty
        If Not String.IsNullOrEmpty(Text) AndAlso largeurDispo > 0 Then
            tailleTexte = TextRenderer.MeasureText(g, Text, Font, New Size(largeurDispo, zone.Height), flags)
            tailleTexte.Width = Math.Min(tailleTexte.Width, largeurDispo)
        End If

        Dim largeurContenu As Integer = tailleIcone + ecart + tailleTexte.Width
        Dim alignGauche As Boolean = (TextAlign = ContentAlignment.TopLeft OrElse
                                      TextAlign = ContentAlignment.MiddleLeft OrElse
                                      TextAlign = ContentAlignment.BottomLeft)

        Dim x As Integer
        If iconeSeule Then
            x = zone.X + (zone.Width - tailleIcone) \ 2
        ElseIf alignGauche Then
            x = zone.X + margeG
        Else
            x = zone.X + Math.Max(margeG, (zone.Width - largeurContenu) \ 2)
        End If

        If tailleIcone > 0 Then
            Dim rIcone As New Rectangle(x, zone.Y + (zone.Height - tailleIcone) \ 2, tailleIcone, tailleIcone)
            AltheaDessin.DessinerImage(g, img, rIcone, If(Enabled, 1.0F, AltheaDessin.OpaciteIconeDesactivee))
            x += tailleIcone + ecart
        End If

        If tailleTexte.Width > 0 Then
            Dim rTexte As New Rectangle(x, zone.Y, Math.Max(0, zone.Right - margeD - x), zone.Height)
            TextRenderer.DrawText(g, Text, Font, rTexte, couleurTexte, flags Or TextFormatFlags.Left)
        End If

    End Sub

#End Region

#Region "Événements souris et état"

    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        _survol = True
        Invalidate()
        MyBase.OnMouseEnter(e)
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        _survol = False
        _enfonce = False
        Invalidate()
        MyBase.OnMouseLeave(e)
    End Sub

    Protected Overrides Sub OnMouseDown(mevent As MouseEventArgs)
        If mevent.Button = MouseButtons.Left Then
            _enfonce = True
            Invalidate()
        End If
        MyBase.OnMouseDown(mevent)
    End Sub

    Protected Overrides Sub OnMouseUp(mevent As MouseEventArgs)
        _enfonce = False
        Invalidate()
        MyBase.OnMouseUp(mevent)
    End Sub

    Protected Overrides Sub OnEnabledChanged(e As EventArgs)
        If Not Enabled Then
            _survol = False
            _enfonce = False
        ElseIf _boutonExplique Is Me Then
            CacherInfobulleInactif()
        End If
        Invalidate()
        MyBase.OnEnabledChanged(e)
    End Sub

    Protected Overrides Sub OnGotFocus(e As EventArgs)
        Invalidate()
        MyBase.OnGotFocus(e)
    End Sub

    Protected Overrides Sub OnLostFocus(e As EventArgs)
        Invalidate()
        MyBase.OnLostFocus(e)
    End Sub

#End Region

#Region "Bouton inactif : dire pourquoi"

    ' Une infobulle Windows ne s'affiche pas sur un contrôle désactivé : il ne reçoit plus la souris,
    ' c'est son parent qui la reçoit. Le parent est donc écouté (une fois, pour tous ses boutons) et
    ' montre la raison du bouton inactif survolé. Une seule infobulle à la fois dans l'application.
    Private Shared ReadOnly _infobulleInactif As New ToolTip With {.ShowAlways = True}
    Private Shared ReadOnly _parentsEcoutes As New HashSet(Of Control)
    Private Shared _boutonExplique As AltheaBouton

    Private _raisonInactif As String = String.Empty

    ' -------------------------------------------------------------------------------------------------
    ' Propriété  : RaisonInactif
    ' Version    : V1.0.0
    ' Date       : 20/10/2026
    '
    ' Rôle       : Pourquoi le bouton est inactif (« Sélectionnez une séance. »), montré au survol
    '              tant qu'il l'est. Vide : aucune infobulle.
    ' Remarques  : Relevé de Joëlle (16/10/2026) : un bouton gris sans explication laisse
    '              l'utilisateur sans piste. Le plus simple : DefinirDisponibilite.
    ' -------------------------------------------------------------------------------------------------
    <Category("Althéa")>
    <Description("Raison montrée au survol quand le bouton est inactif (vide = aucune).")>
    <DefaultValue("")>
    Public Property RaisonInactif As String
        Get
            Return _raisonInactif
        End Get
        Set(value As String)
            _raisonInactif = If(value, String.Empty).Trim()
        End Set
    End Property

    ' Active le bouton, ou le désactive en disant pourquoi (une seule ligne dans l'écran).
    Public Sub DefinirDisponibilite(disponible As Boolean, Optional raisonInactif As String = Nothing)
        Me.RaisonInactif = If(disponible, String.Empty, raisonInactif)
        Enabled = disponible
    End Sub

    Protected Overrides Sub OnParentChanged(e As EventArgs)
        If _boutonExplique Is Me Then CacherInfobulleInactif()
        MyBase.OnParentChanged(e)
        If LicenseManager.UsageMode <> LicenseUsageMode.Designtime Then EcouterParent(Parent)
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso _boutonExplique Is Me Then CacherInfobulleInactif()
        MyBase.Dispose(disposing)
    End Sub

    Private Shared Sub EcouterParent(parent As Control)

        If parent Is Nothing OrElse Not _parentsEcoutes.Add(parent) Then Return

        AddHandler parent.MouseMove, AddressOf Parent_MouseMove
        AddHandler parent.MouseLeave, AddressOf Parent_MouseLeave
        AddHandler parent.Disposed, Sub(s, ev) _parentsEcoutes.Remove(DirectCast(s, Control))

    End Sub

    ' Souris au-dessus d'un bouton inactif qui a une raison : infobulle sous le bouton.
    Private Shared Sub Parent_MouseMove(sender As Object, e As MouseEventArgs)

        Dim parent As Control = DirectCast(sender, Control)
        Dim bouton As AltheaBouton = TryCast(parent.GetChildAtPoint(e.Location, GetChildAtPointSkip.Invisible), AltheaBouton)
        If bouton IsNot Nothing AndAlso (bouton.Enabled OrElse bouton._raisonInactif.Length = 0) Then bouton = Nothing

        If bouton Is _boutonExplique Then Return

        CacherInfobulleInactif()
        If bouton Is Nothing Then Return

        _boutonExplique = bouton
        _infobulleInactif.Show(bouton._raisonInactif, parent, bouton.Left, bouton.Bottom + 4)

    End Sub

    Private Shared Sub Parent_MouseLeave(sender As Object, e As EventArgs)
        CacherInfobulleInactif()
    End Sub

    Private Shared Sub CacherInfobulleInactif()

        If _boutonExplique Is Nothing Then Return

        Dim parent As Control = _boutonExplique.Parent
        If parent IsNot Nothing AndAlso Not parent.IsDisposed Then _infobulleInactif.Hide(parent)
        _boutonExplique = Nothing

    End Sub

#End Region

End Class
