' -------------------------------------------------------------------------------------------------
' Classe      : ResumeFiltre
' Projet      : Althéa
' Version     : V1.1.0
' Date        : 17/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Résumé cliquable au-dessus d'une grille : une rangée de pastilles qui choisissent chacune un
' filtre (un index) ; un nouveau clic revient à « Tous ». La pastille du filtre en cours est cerclée
' et en gras.
'
' Remarques   :
' - Le filtre reste porté par l'écran : une seule logique de filtrage, le résumé ne fait que choisir
'   un index (le changement d'index réapplique les filtres, qui reconstruisent le résumé).
' - Ce clic détruit donc la pastille cliquée : le changement d'index est différé (BeginInvoke)
'   pour ne pas libérer le contrôle au milieu de son propre événement.
' - L'index est lu et choisi par deux délégués : une liste de filtre (séances, paiements du
'   dossier) ou un état propre à l'écran (situation de la grille Patients).
'
' Historique  :
' - V1.0.0 (17/10/2026) : classe privée de UC_DossierFiche (grilles des séances et des paiements).
' - V1.1.0 (17/10/2026) : sortie de UC_DossierFiche pour servir aussi à la grille Patients ; index
'   lu et choisi par délégués ; AjouterLibre ; la pastille du filtre en cours reste affichée même
'   si son nombre tombe à zéro (sinon plus moyen de revenir à « Tous » d'un clic).
'
' Dépendances :
' - AltheaPastille, UtilsPastilles (teintes), UITheme
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Friend NotInheritable Class ResumeFiltre

    Private Shared ReadOnly PoliceNormale As New Font("Calibri", 10.0F, FontStyle.Regular)
    Private Shared ReadOnly PoliceChoisie As New Font("Calibri", 10.0F, FontStyle.Bold)

    Private ReadOnly _panneau As FlowLayoutPanel
    Private ReadOnly _lireIndex As Func(Of Integer)
    Private ReadOnly _choisirIndex As Action(Of Integer)
    Private ReadOnly _indexTous As Integer
    Private ReadOnly _libelleTous As String
    Private ReadOnly _libelleCeux As String
    Private ReadOnly _infobulle As Action(Of Control, String)

    ' Résumé qui pilote une liste de filtre : la pastille choisit un index de la liste.
    Public Sub New(panneau As FlowLayoutPanel, filtre As AltheaListe, indexTous As Integer,
                   libelleTous As String, libelleCeux As String, infobulle As Action(Of Control, String))
        Me.New(panneau, Function() filtre.SelectedIndex, Sub(index) filtre.SelectedIndex = index,
               indexTous, libelleTous, libelleCeux, infobulle)
    End Sub

    ' panneau : rangée qui reçoit les pastilles ; lireIndex / choisirIndex : filtre en cours et son
    ' changement ; indexTous : index « Tous » ; libelleTous / libelleCeux : fins des infobulles
    ' (« toutes les séances », « ces séances ») ; infobulle : pose une infobulle (Nothing = retirer).
    Public Sub New(panneau As FlowLayoutPanel, lireIndex As Func(Of Integer), choisirIndex As Action(Of Integer),
                   indexTous As Integer, libelleTous As String, libelleCeux As String,
                   infobulle As Action(Of Control, String))
        _panneau = panneau
        _lireIndex = lireIndex
        _choisirIndex = choisirIndex
        _indexTous = indexTous
        _libelleTous = libelleTous
        _libelleCeux = libelleCeux
        _infobulle = infobulle
    End Sub

    ' Vide la rangée puis la remplit (sans redessin intermédiaire).
    Public Sub Reconstruire(remplir As Action)
        _panneau.SuspendLayout()
        Try
            For Each ancien As Control In _panneau.Controls.Cast(Of Control)().ToList()
                _infobulle(ancien, Nothing)
                ancien.Dispose()
            Next
            _panneau.Controls.Clear()
            remplir()
        Finally
            _panneau.ResumeLayout(True)
        End Try
    End Sub

    ' « 3 à venir », « 1 absence »… ; rien si le nombre est nul (sauf filtre en cours).
    Public Sub AjouterNombre(nombre As Integer, singulier As String, pluriel As String,
                             teinte As UtilsPastilles.TeintePastille, index As Integer)
        If nombre <= 0 AndAlso Not EstChoisi(index) Then Return
        AjouterPastille($"{nombre} {If(nombre = 1, singulier, pluriel)}", teinte, index)
    End Sub

    ' « À régler 80,00 € »… ; rien si le montant est nul (sauf filtre en cours).
    Public Sub AjouterMontant(montant As Decimal, libelle As String,
                              teinte As UtilsPastilles.TeintePastille, index As Integer)
        If montant <= 0D AndAlso Not EstChoisi(index) Then Return
        AjouterPastille($"{libelle} {montant:N2} €", teinte, index)
    End Sub

    ' Pastille au texte libre (toujours affichée).
    Public Sub AjouterLibre(texte As String, teinte As UtilsPastilles.TeintePastille, index As Integer)
        AjouterPastille(texte, teinte, index)
    End Sub

    ' Filet vertical entre deux groupes de pastilles (nombres | montants).
    Public Sub AjouterSeparateur()
        _panneau.Controls.Add(New Panel With {
            .BackColor = UITheme.ColorBeigeFonce, .Size = New Size(1, 20), .Margin = New Padding(4, 7, 10, 0)})
    End Sub

    Private Function EstChoisi(index As Integer) As Boolean
        Return _lireIndex() = index
    End Function

    Private Sub AjouterPastille(texte As String, teinte As UtilsPastilles.TeintePastille, index As Integer)

        Dim choisie As Boolean = EstChoisi(index)
        Dim pastille As New AltheaPastille With {
            .CouleurFond = teinte.Fond,
            .CouleurBordure = If(choisie, teinte.Texte, Color.Empty),
            .Cursor = Cursors.Hand,
            .Font = If(choisie, PoliceChoisie, PoliceNormale),
            .ForeColor = teinte.Texte,
            .Margin = New Padding(0, 4, 6, 0),
            .Padding = New Padding(10, 3, 10, 3),
            .Tag = index,
            .Text = texte
        }
        AddHandler pastille.Click, AddressOf Pastille_Click
        _infobulle(pastille, If(choisie, $"Cliquer pour revenir à {_libelleTous}.", $"Cliquer pour n'afficher que {_libelleCeux}."))
        _panneau.Controls.Add(pastille)

    End Sub

    Private Sub Pastille_Click(sender As Object, e As EventArgs)
        Dim index As Integer = CInt(DirectCast(sender, Control).Tag)
        Dim cible As Integer = If(EstChoisi(index), _indexTous, index)
        _panneau.BeginInvoke(New Action(Sub() _choisirIndex(cible)))
    End Sub

End Class
