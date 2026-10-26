' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaJauge
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 11/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Jauge horizontale du kit Althéa (quota consommé, progression) : barre arrondie dont la
' couleur indique l'état — Sauge en temps normal, Ocre au seuil d'alerte, Terre au plafond.
'
' Remarques  :
' - Valeur, Maximum et SeuilAlerte sont des entiers (nombre de séances) ; SeuilAlerte = 0
'   désactive l'état d'alerte.
' - Maximum = 0 : la jauge reste vide (pas de règle connue).
'
' Dépendances :
' - UITheme, AltheaDessin, AltheaFondParent
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

<DesignerCategory("Code")>
<ToolboxItem(True)>
<Description("Jauge Althéa : barre arrondie Sauge / Ocre / Terre selon le seuil.")>
Public Class AltheaJauge
    Inherits Control

#Region "Variables privées"

    Private _valeur As Integer
    Private _maximum As Integer
    Private _seuilAlerte As Integer

#End Region

#Region "Initialisation"

    Public Sub New()

        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.SupportsTransparentBackColor, True)

        BackColor = Color.Transparent
        Size = New Size(200, 8)
        TabStop = False

    End Sub

#End Region

#Region "Propriétés publiques"

    <Category("Althéa")>
    <Description("Valeur atteinte (ex. séances consommées).")>
    <DefaultValue(0)>
    Public Property Valeur As Integer
        Get
            Return _valeur
        End Get
        Set(value As Integer)
            _valeur = Math.Max(0, value)
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Valeur maximale (ex. plafond du quota) ; 0 = jauge vide.")>
    <DefaultValue(0)>
    Public Property Maximum As Integer
        Get
            Return _maximum
        End Get
        Set(value As Integer)
            _maximum = Math.Max(0, value)
            Invalidate()
        End Set
    End Property

    <Category("Althéa")>
    <Description("Valeur à partir de laquelle la jauge passe en Ocre (0 = pas d'alerte).")>
    <DefaultValue(0)>
    Public Property SeuilAlerte As Integer
        Get
            Return _seuilAlerte
        End Get
        Set(value As Integer)
            _seuilAlerte = Math.Max(0, value)
            Invalidate()
        End Set
    End Property

#End Region

#Region "Dessin"

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        AltheaFondParent.Peindre(Me, e)
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias

        Dim rayon As Single = Math.Max(1.0F, Height / 2.0F)
        Dim fond As New RectangleF(0, 0, Width - 1, Height - 1)

        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(fond, rayon)
            Using pinceau As New SolidBrush(UITheme.ColorBeige)
                g.FillPath(pinceau, chemin)
            End Using
        End Using

        If _maximum <= 0 OrElse _valeur <= 0 Then Return

        Dim ratio As Single = Math.Min(1.0F, CSng(_valeur) / _maximum)
        Dim largeur As Single = Math.Max(Height - 1, fond.Width * ratio)

        Dim couleur As Color
        If Not Enabled Then
            couleur = AltheaDessin.CouleurDesactiveBordure
        ElseIf _valeur >= _maximum Then
            couleur = UITheme.ColorRougeMoyenClair
        ElseIf _seuilAlerte > 0 AndAlso _valeur >= _seuilAlerte Then
            couleur = UITheme.ColorJauneModere
        Else
            couleur = UITheme.ColorSauge
        End If

        Using chemin As GraphicsPath = AltheaDessin.CheminArrondi(New RectangleF(0, 0, largeur, fond.Height), rayon)
            Using pinceau As New SolidBrush(couleur)
                g.FillPath(pinceau, chemin)
            End Using
        End Using

    End Sub

#End Region

End Class
