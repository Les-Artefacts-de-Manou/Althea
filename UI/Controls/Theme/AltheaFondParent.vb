' -------------------------------------------------------------------------------------------------
' Classe     : AltheaFondParent
' Projet     : Althéa
' Version    : V1.0.0
' Date       : 10/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Outil partagé du kit : recopie, sous un contrôle, le fond et le dessin de son parent
' (feuillage compris), pour les contrôles qui ne peuvent pas compter sur la transparence native.
'
' Remarques  :
' - TabControl, ComboBox et les champs composés ignorent BackColor = Transparent : ils passent
'   par cet outil pour laisser voir la carte ou le fond de l'écran derrière leurs coins arrondis.
' - InvokePaintBackground / InvokePaint sont des membres protégés de Control. La classe en hérite
'   uniquement pour pouvoir les appeler au nom d'un autre contrôle ; son instance relais n'est
'   jamais affichée et ne crée aucune fenêtre.
' - Le dessin se fait dans le Graphics fourni, donc dans le tampon du contrôle appelant : rien
'   n'est peint directement à l'écran, rien ne clignote.
'
' Dépendances :
' - Aucune
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

<ToolboxItem(False)>
<DesignerCategory("Code")>
Friend NotInheritable Class AltheaFondParent
    Inherits Control

    ' Instance relais : seul moyen d'appeler les membres protégés de Control pour un tiers.
    Private Shared ReadOnly _relais As New AltheaFondParent()

    Private Sub New()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : Peindre
    ' Version    : V1.0.0
    ' Date       : 10/10/2026
    '
    ' Rôle       :
    ' Peint, dans la zone à redessiner de « enfant », ce que son parent afficherait à cet endroit.
    '
    ' Paramètres :
    ' - enfant : contrôle qui demande le fond
    ' - e      : arguments de dessin de ce contrôle (Graphics et zone à redessiner)
    '
    ' Exceptions :
    ' - Aucune
    ' -------------------------------------------------------------------------------------------------
    Friend Shared Sub Peindre(enfant As Control, e As PaintEventArgs)

        If enfant Is Nothing OrElse enfant.Parent Is Nothing Then Return

        Dim g As Graphics = e.Graphics
        Dim etat As GraphicsState = g.Save()

        Try
            ' Le parent dessine dans ses propres coordonnées.
            g.TranslateTransform(-enfant.Left, -enfant.Top)

            Dim zoneParent As Rectangle = e.ClipRectangle
            zoneParent.Offset(enfant.Left, enfant.Top)

            ' Les arguments ne possèdent pas l'objet Graphics : ils ne le libèrent pas.
            Dim argsParent As New PaintEventArgs(g, zoneParent)
            _relais.InvokePaintBackground(enfant.Parent, argsParent)
            _relais.InvokePaint(enfant.Parent, argsParent)

        Finally
            g.Restore(etat)
        End Try

    End Sub

End Class
