' -------------------------------------------------------------------------------------------------
' Classe   : CategoriesQuotaEditor
' Projet   : Althéa
' Version  : V1.4.0
' Date     : 22/10/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Boîte de dialogue de gestion des catégories de quota d'un domaine (ref_categories_quota),
' ouverte depuis l'onglet Quotas de UC_DomainesHome.
'
' Remarques :
' - Schéma des fenêtres du kit (UI-7, 15/10/2026) : carte « Catégories » (barre locale, aide, grille)
'   et carte « Nouvelle catégorie » sur le fond AltheaFondFenetre.
' - Une catégorie utilisée (nature rattachée ou plafond historisé) ne peut qu'être désactivée.
' - Pas de modification de libellé/code : désactiver puis créer une nouvelle catégorie.
' - V1.3.0 (18/10/2026, UI-9.4) : colonne État — pastille « Inactive » (grise), rien pour une
'   catégorie active (seules les exceptions se voient) ; ligne inactive atténuée.
' - V1.4.0 (22/10/2026, audit C.1, étape 3) : reçoit le ContexteOperation de l'appelant ; les
'   écritures sont vérifiées par le métier.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class CategoriesQuotaEditor

    Private ReadOnly _idDomaine As ULong

    ' Qui modifie les catégories (droit vérifié par le métier, audit C.1) ; fourni par l'appelant.
    Private ReadOnly _contexte As ContexteOperation
    Private _categories As New List(Of CategorieQuota)

    ' Indique si au moins une modification a été enregistrée (l'appelant rafraîchit alors l'onglet).
    Public Property AModifie As Boolean

    Public Sub New(idDomaine As ULong, libelleDomaine As String, contexte As ContexteOperation)

        InitializeComponent()

        _idDomaine = idDomaine
        _contexte = contexte
        enTete.SousTitre = $"Catégories de quota — {libelleDomaine}"

    End Sub

    Private Sub CategoriesQuotaEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Fond beige à la feuille et barre de titre aux couleurs du kit.
        AltheaFondFenetre.Appliquer(Me)

        UtilsButtons.InitStandardButton(btnAjouter)
        UtilsButtons.InitStandardButton(btnActiver)
        UtilsButtons.InitStandardButton(btnSupprimer)
        UtilsButtons.InitStandardButton(btnFermer)
        UtilsDataGrid.InitDataGridSecondaire(dgvCategories)

        Charger()

    End Sub

    Private Sub Charger(Optional idASelectionner As ULong = 0)

        Try
            _categories = GestionCategoriesQuota.GetCategoriesParDomaine(_idDomaine, True)
        Catch ex As Exception
            DialogChoix.Erreur("Erreur lors du chargement des catégories.", "Catégories de quota")
            _categories = New List(Of CategorieQuota)
        End Try

        dgvCategories.Rows.Clear()

        For Each c In _categories
            Dim index As Integer = dgvCategories.Rows.Add(c.CodeCategorie, c.LibelleCategorie, If(c.Actif, String.Empty, "Inactive"))
            dgvCategories.Rows(index).Tag = c
            If Not c.Actif Then
                dgvCategories.Rows(index).DefaultCellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
                dgvCategories.Rows(index).DefaultCellStyle.SelectionForeColor = UITheme.ColorGrisDiscret
            End If
            If c.IdCategorieQuota = idASelectionner Then dgvCategories.Rows(index).Selected = True
        Next

        ActualiserBoutons()

    End Sub

    Private Function Selection() As CategorieQuota

        If dgvCategories.SelectedRows.Count = 0 Then Return Nothing
        Return TryCast(dgvCategories.SelectedRows(0).Tag, CategorieQuota)

    End Function

    Private Sub ActualiserBoutons()

        Dim c As CategorieQuota = Selection()
        btnActiver.Enabled = c IsNot Nothing
        btnSupprimer.Enabled = c IsNot Nothing
        btnActiver.Text = If(c IsNot Nothing AndAlso Not c.Actif, "Réactiver", "Désactiver")

    End Sub

    ' Pastille « Inactive » ; cellule vide pour une catégorie active (V1.3.0).
    Private Sub dgvCategories_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvCategories.CellPainting
        If e.RowIndex < 0 OrElse e.ColumnIndex <> colEtat.Index Then Return
        If Not TypeOf dgvCategories.Rows(e.RowIndex).Tag Is CategorieQuota Then Return
        UtilsDataGrid.DessinerPastilleCellule(dgvCategories, e, CStr(dgvCategories.Rows(e.RowIndex).Cells(e.ColumnIndex).Value),
                                              UtilsPastilles.Teinte(TonEtat.Inactif))
    End Sub

    Private Sub dgvCategories_SelectionChanged(sender As Object, e As EventArgs) Handles dgvCategories.SelectionChanged
        ActualiserBoutons()
    End Sub

    Private Sub btnAjouter_Click(sender As Object, e As EventArgs) Handles btnAjouter.Click

        Dim c As New CategorieQuota With {
            .IdDomaine = _idDomaine,
            .CodeCategorie = txtCode.Text.Trim(),
            .LibelleCategorie = txtLibelle.Text.Trim(),
            .Actif = True,
            .OrdreAffichage = If(_categories.Count = 0, 10, _categories.Max(Function(x) x.OrdreAffichage) + 10)
        }

        Dim erreur As String = GestionCategoriesQuota.ValiderCategorie(c)
        If erreur <> String.Empty Then
            DialogChoix.Avertissement(erreur, "Catégories de quota")
            Return
        End If

        Try
            GestionCategoriesQuota.InsertCategorie(_contexte, c)
            AModifie = True
            txtCode.Clear()
            txtLibelle.Clear()
            txtCode.Focus()
            Charger()
        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
        Catch ex As Exception
            DialogChoix.Erreur("La catégorie n'a pas pu être enregistrée.", "Catégories de quota")
        End Try

    End Sub

    Private Sub btnActiver_Click(sender As Object, e As EventArgs) Handles btnActiver.Click

        Dim c As CategorieQuota = Selection()
        If c Is Nothing Then Return

        c.Actif = Not c.Actif

        Try
            GestionCategoriesQuota.UpdateCategorie(_contexte, c)
            AModifie = True
        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
        Catch ex As Exception
            DialogChoix.Erreur("Le changement d'état n'a pas pu être enregistré.", "Catégories de quota")
        End Try

        Charger(c.IdCategorieQuota)

    End Sub

    Private Sub btnSupprimer_Click(sender As Object, e As EventArgs) Handles btnSupprimer.Click

        Dim c As CategorieQuota = Selection()
        If c Is Nothing Then Return

        Try
            If GestionCategoriesQuota.CategorieEstUtilisee(c.IdCategorieQuota) Then
                DialogChoix.Avertissement(
                    "Cette catégorie est déjà utilisée (nature rattachée ou règle de quota)." & Environment.NewLine &
                    "Pour garder l'historique, désactivez-la plutôt.", "Catégories de quota")
                Return
            End If

            If DialogChoix.ConfirmerSuppression($"Supprimer la catégorie « {c.LibelleCategorie} » ?", "Catégories de quota") <> DialogResult.Yes Then Return

            GestionCategoriesQuota.SupprimerCategorie(_contexte, c.IdCategorieQuota)
            AModifie = True
            Charger()
        Catch ex As UnauthorizedAccessException
            ' Droit vérifié par le métier (audit C.1) : message rédigé pour l'utilisatrice.
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")
        Catch ex As Exception
            DialogChoix.Erreur("La catégorie n'a pas pu être supprimée.", "Catégories de quota")
        End Try

    End Sub

End Class
