' -------------------------------------------------------------------------------------------------
' Module      : GenerateurCachet
' Projet      : Althéa
' Version     : V1.2.0
' Date        : 12/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Génère l'image du cachet professionnel à partir des données du praticien (identité, titre,
' adresse, contacts, identifiants et qualifications cochés « afficher sur cachet »).
'
' Remarques   :
' - Aucune donnée codée en dur : tout provient de cab_cabinet / cab_praticiens.
' - Fond transparent, encre bleu nuit ou noire (paramètre CACHET_ENCRE_NOIRE) ; modèle visuel :
'   Assets/Praticien/Cachet_modele.svg.
' - Adresse du praticien vide : adresse du cabinet utilisée.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text

Public Module GenerateurCachet

    ' Les deux encres proposées (décision du 11/10/2026) ; le choix est un paramètre.
    Private ReadOnly EncreBleuNuit As Color = Color.FromArgb(30, 45, 90)
    Private ReadOnly EncreNoire As Color = Color.FromArgb(25, 25, 25)

    Private Class LigneCachet
        Public Texte As String
        Public Taille As Single
        Public Style As FontStyle
        Public SeparateurApres As Boolean
    End Class

    ''' <summary>Construit l'image du cachet (largeur en pixels, hauteur calculée).</summary>
    Public Function Generer(cab As Cabinet,
                            prat As Praticien,
                            identifiants As IEnumerable(Of PraticienIdentifiant),
                            qualifications As IEnumerable(Of PraticienQualification),
                            Optional largeur As Integer = 1800) As Bitmap

        Dim lignes = ConstruireLignes(cab, prat, identifiants, qualifications)
        Dim couleurEncre As Color =
            If(GestionParametres.GetBooleen(ClesParametres.CachetEncreNoire, ClesParametres.DefautCachetEncreNoire),
               EncreNoire, EncreBleuNuit)
        Dim echelle = largeur / 1800.0F
        Dim marge = 90 * echelle
        Dim interligne = 22 * echelle

        Dim hauteurTexte As Single = 0
        For Each l In lignes
            hauteurTexte += l.Taille * echelle * 1.35F + If(l.SeparateurApres, interligne * 1.5F, 0)
        Next
        Dim hauteur = CInt(Math.Ceiling(hauteurTexte + 2 * marge))

        Dim bmp As New Bitmap(largeur, hauteur, Imaging.PixelFormat.Format32bppArgb)
        Using g = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit
            g.Clear(Color.Transparent)

            Using penExt As New Pen(couleurEncre, 12 * echelle), penInt As New Pen(couleurEncre, 4 * echelle)
                DessinerRectangleArrondi(g, penExt, New RectangleF(20 * echelle, 20 * echelle, largeur - 40 * echelle, hauteur - 40 * echelle), 44 * echelle)
                DessinerRectangleArrondi(g, penInt, New RectangleF(48 * echelle, 48 * echelle, largeur - 96 * echelle, hauteur - 96 * echelle), 28 * echelle)
            End Using

            Using br As New SolidBrush(couleurEncre), penSep As New Pen(couleurEncre, 3 * echelle),
                  sf As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                Dim y = marge
                For Each l In lignes
                    Dim h = l.Taille * echelle * 1.35F
                    Using f As New Font("Segoe UI", l.Taille * echelle, l.Style, GraphicsUnit.Pixel)
                        Dim taille = l.Taille * echelle
                        Dim fonte = f
                        ' Réduction automatique si la ligne déborde
                        While g.MeasureString(l.Texte, fonte).Width > largeur - 2 * marge AndAlso taille > 10
                            If fonte IsNot f Then fonte.Dispose()
                            taille -= 2
                            fonte = New Font("Segoe UI", taille, l.Style, GraphicsUnit.Pixel)
                        End While
                        g.DrawString(l.Texte, fonte, br, New RectangleF(0, y, largeur, h), sf)
                        If fonte IsNot f Then fonte.Dispose()
                    End Using
                    y += h
                    If l.SeparateurApres Then
                        g.DrawLine(penSep, largeur * 0.16F, y + interligne * 0.5F, largeur * 0.84F, y + interligne * 0.5F)
                        y += interligne * 1.5F
                    End If
                Next
            End Using
        End Using

        Return bmp
    End Function

    Private Function ConstruireLignes(cab As Cabinet,
                                      prat As Praticien,
                                      identifiants As IEnumerable(Of PraticienIdentifiant),
                                      qualifications As IEnumerable(Of PraticienQualification)) As List(Of LigneCachet)

        Dim lignes As New List(Of LigneCachet)

        lignes.Add(New LigneCachet With {.Texte = $"{prat.Prenom} {prat.Nom?.ToUpperInvariant()}".Trim(), .Taille = 96, .Style = FontStyle.Bold})

        Dim titre = prat.TitreProfessionnel
        If Not String.IsNullOrWhiteSpace(titre) Then
            lignes.Add(New LigneCachet With {.Texte = titre.Trim(), .Taille = 56, .Style = FontStyle.Italic})
        End If
        lignes.Last().SeparateurApres = True

        Dim adresse = FormaterAdresse(prat, cab)
        If adresse.Item1.Length > 0 Then lignes.Add(New LigneCachet With {.Texte = adresse.Item1, .Taille = 54})
        If adresse.Item2.Length > 0 Then lignes.Add(New LigneCachet With {.Texte = adresse.Item2, .Taille = 54})

        Dim contacts = String.Join(" · ", {Premier(prat.Telephone, cab?.Telephone), Premier(prat.Email, cab?.Email)}.Where(Function(s) s.Length > 0))
        If contacts.Length > 0 Then lignes.Add(New LigneCachet With {.Texte = contacts, .Taille = 54})

        Dim aujourdhui = Date.Today
        For Each q In qualifications.Where(Function(x) x.AfficherSurCachet).OrderBy(Function(x) x.OrdreAffichage)
            Dim texte = q.Libelle & If(String.IsNullOrWhiteSpace(q.Etablissement), "", " — " & q.Etablissement)
            lignes.Add(New LigneCachet With {.Texte = texte, .Taille = 46})
        Next

        For Each i In identifiants.Where(Function(x) x.AfficherSurCachet AndAlso
                                                  (Not x.DateDebut.HasValue OrElse x.DateDebut.Value <= aujourdhui) AndAlso
                                                  (Not x.DateFin.HasValue OrElse x.DateFin.Value >= aujourdhui))
            ' INAMI : les 8 premiers chiffres identifient la personne (n° d'agrément), les 3 derniers
            ' la qualification. Le n° d'agrément est dérivé, jamais stocké en double.
            If String.Equals(i.CodeType, UtilsIdentifiantBelge.CodeInami, StringComparison.OrdinalIgnoreCase) Then
                Dim agrement = UtilsIdentifiantBelge.FormaterNumeroPersonneInami(i.Valeur)
                If agrement IsNot Nothing Then
                    lignes.Add(New LigneCachet With {.Texte = $"N° agrément : {agrement}", .Taille = 54, .Style = FontStyle.Bold})
                End If
            End If
            lignes.Add(New LigneCachet With {.Texte = $"{i.LibelleType} : {i.Valeur}", .Taille = 54, .Style = FontStyle.Bold})
        Next

        Return lignes
    End Function

    Private Function Premier(a As String, b As String) As String
        If Not String.IsNullOrWhiteSpace(a) Then Return a.Trim()
        If Not String.IsNullOrWhiteSpace(b) Then Return b.Trim()
        Return String.Empty
    End Function

    Private Function FormaterAdresse(prat As Praticien, cab As Cabinet) As Tuple(Of String, String)
        Dim utiliserPraticien = Not String.IsNullOrWhiteSpace(prat.AdresseRue)
        Dim rue = If(utiliserPraticien, prat.AdresseRue, cab?.AdresseRue)
        Dim num = If(utiliserPraticien, prat.AdresseNumero, cab?.AdresseNumero)
        Dim cp = If(utiliserPraticien, prat.CodePostal, cab?.CodePostal)
        Dim loc = If(utiliserPraticien, prat.Localite, cab?.Localite)
        Dim pays = If(utiliserPraticien, prat.LibellePays, cab?.LibellePays)

        Dim ligne1 = $"{rue} {num}".Trim()
        Dim ligne2 = String.Join(" — ", {$"{cp} {loc}".Trim(), If(pays, "").Trim()}.Where(Function(s) s.Length > 0))
        Return Tuple.Create(ligne1, ligne2)
    End Function

    Private Sub DessinerRectangleArrondi(g As Graphics, pen As Pen, r As RectangleF, rayon As Single)
        Using p As New GraphicsPath()
            Dim d = rayon * 2
            p.AddArc(r.X, r.Y, d, d, 180, 90)
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90)
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90)
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90)
            p.CloseFigure()
            g.DrawPath(pen, p)
        End Using
    End Sub

End Module
