' -------------------------------------------------------------------------------------------------
' Classe      : ElementListePastille
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 21/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Élément d'une AltheaListe affiché comme une pastille colorée : les filtres des grilles reprennent
' ainsi les couleurs des pastilles et du résumé (demande de Joëlle, 21/10/2026). Sans teinte,
' l'élément s'affiche en texte simple.
'
' Remarques   :
' - ToString rend le libellé : la liste et le code existant (texte de l'élément) restent inchangés.
' - Les couleurs viennent de UtilsPastilles (aucune couleur écrite ici).
' - V1.1.0 (21/10/2026) : Contour — pastille au trait, sans fond (axe large comme « Passées »).
'
' Dépendances :
' - UtilsPastilles.TeintePastille ; dessiné par AltheaListe (UtilsDataGrid.DessinerPastilleZone)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class ElementListePastille

    ' Texte de l'élément (et de la pastille).
    Public ReadOnly Property Libelle As String

    ' Couleurs de la pastille ; Nothing : texte simple.
    Public ReadOnly Property Teinte As UtilsPastilles.TeintePastille?

    ' Pastille au trait (bord en pointillé dans la couleur du texte, sans fond).
    Public ReadOnly Property Contour As Boolean

    Public Sub New(libelle As String, Optional teinte As UtilsPastilles.TeintePastille? = Nothing,
                   Optional contour As Boolean = False)
        Me.Libelle = If(libelle, String.Empty)
        Me.Teinte = teinte
        Me.Contour = contour
    End Sub

    Public Overrides Function ToString() As String
        Return Libelle
    End Function

End Class
