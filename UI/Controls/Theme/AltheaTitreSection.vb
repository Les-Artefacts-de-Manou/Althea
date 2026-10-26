' -------------------------------------------------------------------------------------------------
' Contrôle   : AltheaTitreSection
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 09/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Titre de section / de groupe du kit Althéa : texte (Mauve foncé, gras) suivi d'un trait fin
' jusqu'au bord droit. Remplace les GroupBox et les labels de groupe habillés.
'
' Remarques  :
' - Hérite de Label : Text, Font, ForeColor, Dock, Anchor se règlent dans le Designer.
' - CouleurTrait règle le trait (Beige foncé par défaut).
' - Exemples : « Cabinet », « Patient et entourage » (hubs), « Identité du domaine ».
'
' Dépendances :
' - UITheme, AltheaDessin
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel

<DesignerCategory("Code")>
<ToolboxItem(True)>
<Description("Titre de section Althéa : texte suivi d'un trait fin.")>
Public Class AltheaTitreSection
    Inherits Label

    Private _couleurTrait As Color = UITheme.ColorBeigeFonce

    Public Sub New()

        SetStyle(ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.SupportsTransparentBackColor, True)

        AutoSize = False
        BackColor = Color.Transparent
        ForeColor = UITheme.ColorMauveFonce
        Font = New Font("Calibri", 12.0F, FontStyle.Bold)
        Size = New Size(400, 26)

    End Sub

    <Category("Althéa")>
    <Description("Couleur du trait qui suit le titre (Transparent = pas de trait).")>
    Public Property CouleurTrait As Color
        Get
            Return _couleurTrait
        End Get
        Set(value As Color)
            _couleurTrait = value
            Invalidate()
        End Set
    End Property

    Private Function ShouldSerializeCouleurTrait() As Boolean
        Return _couleurTrait <> UITheme.ColorBeigeFonce
    End Function

    Private Sub ResetCouleurTrait()
        CouleurTrait = UITheme.ColorBeigeFonce
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        ' Le fond (transparent) est déjà peint par OnPaintBackground.
        Dim zone As New Rectangle(Padding.Left, Padding.Top,
                                  Math.Max(0, Width - Padding.Horizontal),
                                  Math.Max(0, Height - Padding.Vertical))

        AltheaDessin.DessinerTitreSection(e.Graphics, Text, Font,
                                          If(Enabled, ForeColor, AltheaDessin.CouleurDesactiveTexte),
                                          _couleurTrait, zone)

    End Sub

End Class
