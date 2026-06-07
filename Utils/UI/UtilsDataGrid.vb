' -------------------------------------------------------------------------------------------------
' Module      : UtilsDataGrid
' Projet      : Althéa
' Version     : V1.4.1
' Date        : 14/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les comportements UI communs aux DataGridView.
'
' Responsabilités :
' - Appliquer le style basique des DataGridView (InitDataGridBasique)
' - Configurer les couleurs depuis UITheme
' - Configurer les polices et alignements
' - Configurer le mode sélection (FullRowSelect, ReadOnly, etc.)
'
' Remarques   :
' - Les couleurs viennent de UITheme
' - Module statique (méthodes partagées)
'
' Dépendances :
' - UITheme (couleurs)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On
Option Infer On

Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms

Public Module UtilsDataGrid

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InitDataGridBasique
    ' Version    : V1.4.1
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Applique le style basique Althéa à un DataGridView.
    '
    ' Paramètres :
    ' - dgv : Le DataGridView à styler
    '
    ' Remarques  :
    ' - Configure les couleurs (fond, grille, en-têtes, cellules, sélection, alternance) depuis UITheme
    ' - Configure les polices (Calibri 10pt, Bold pour en-têtes, Regular pour cellules)
    ' - Configure les hauteurs (en-têtes : 32px)
    ' - Configure le mode sélection (FullRowSelect, ReadOnly, MultiSelect=False, etc.)
    ' - Masque les RowHeaders
    ' -------------------------------------------------------------------------------------------------
    Public Sub InitDataGridBasique(
        dgv As DataGridView
    )

        If dgv Is Nothing Then Return

        dgv.BorderStyle = BorderStyle.None
        dgv.BackgroundColor = UITheme.DataGridBack
        dgv.GridColor = UITheme.DataGridGridColor

        dgv.EnableHeadersVisualStyles = False

        dgv.ColumnHeadersDefaultCellStyle.BackColor = UITheme.DataGridHeaderBack
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = UITheme.DataGridHeaderFore
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Calibri", 10, FontStyle.Bold)
        dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgv.ColumnHeadersHeight = 32

        dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = UITheme.DataGridHeaderBack
        dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = UITheme.DataGridHeaderFore

        dgv.DefaultCellStyle.BackColor = UITheme.DataGridCellBack
        dgv.DefaultCellStyle.ForeColor = UITheme.DataGridCellFore
        dgv.DefaultCellStyle.SelectionBackColor = UITheme.DataGridCellSelectionBack
        dgv.DefaultCellStyle.SelectionForeColor = UITheme.DataGridCellSelectionFore
        dgv.DefaultCellStyle.Font = New Font("Calibri", 10, FontStyle.Regular)

        dgv.RowsDefaultCellStyle.BackColor = UITheme.DataGridCellBack
        dgv.RowsDefaultCellStyle.ForeColor = UITheme.DataGridCellFore
        dgv.AlternatingRowsDefaultCellStyle.BackColor = UITheme.DataGridAlternatingBack

        dgv.RowHeadersVisible = False
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.MultiSelect = False
        dgv.ReadOnly = True
        dgv.AutoGenerateColumns = False
        dgv.AllowUserToAddRows = False
        dgv.AllowUserToDeleteRows = False
        dgv.AllowUserToResizeRows = False

    End Sub


End Module


