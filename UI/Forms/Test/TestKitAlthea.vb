' -------------------------------------------------------------------------------------------------
' Form       : TestKitAlthea
' Projet     : Althéa
' Version    : V2.2.0
' Date       : 20/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Vitrine du kit Althéa : chaque contrôle du kit, dans ses états (normal, désactivé,
' consultation, erreur…), rangé par famille dans un onglet. Sert aux captures d'écran de
' Docs/Rules/Reference_UI.md et de banc de vérification après chaque modification du kit.
'
' Onglets    :
' - Palette et polices : toutes les couleurs de UITheme (lues par réflexion, donc toujours à jour)
'   et les polices d'usage
' - Boutons : AltheaBouton (actions, Outil), AltheaTuile, AltheaLigne, AltheaBoutonMenu
' - Structure : AltheaEnTete, AltheaCarte, AltheaBarreOutils, AltheaBarreActions,
'   AltheaTitreSection, AltheaPastille, AltheaFilAriane
' - Champs : zone de texte, AltheaListe, AltheaDate, AltheaHeure, AltheaCase, AltheaRecherche dans leurs états ;
'   fiche type qui bascule entre saisie et consultation
' - Indicateurs : AltheaIconeEtat, AltheaJauge, AltheaEtapes, AltheaPuceUtilisateur, AltheaCalendrier
' - Grilles : grille principale et grille secondaire (UtilsDataGrid), couleurs de statut
'
' Remarques  :
' - V2.0.0 (16/10/2026) : la fenêtre de démonstration du 09/10 devient la vitrine complète
'   (décision du 15/10/2026, ToDo « Vitrine du kit »). Ouverte depuis la carte « Outils de test »
'   de l'accueil (Debug seulement).
' - V2.1.0 (16/10/2026) : ligne « Heure (AltheaHeure) » dans l'onglet Champs.
' - V2.2.0 (20/10/2026) : boutons désactivés avec une raison au survol (AltheaBouton V1.3.0).
' - Le cadre (en-tête, onglets, barre d'actions) est dans le Designer ; le contenu des onglets est
'   construit par le code : des dizaines de contrôles répétitifs, plus lisibles et plus faciles à
'   compléter sous forme de petites fonctions que dans un Designer de plusieurs milliers de lignes.
' - Le survol ne peut pas être forcé (état interne des contrôles) : il se montre à la souris.
' - Données d'exemple uniquement (aucune donnée réelle, aucun accès à la base).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System.Reflection

Public Class TestKitAlthea

#Region "Constantes de mise en page"

    Private Const MARGE As Integer = 12
    Private Const ECART As Integer = 8
    Private Const LARGEUR_CARTE As Integer = 1030

    ' Première ligne de contenu sous le titre d'une carte (règle des fenêtres : champ à y = 40).
    Private Const Y_CONTENU As Integer = 40

    ' Échantillons de couleur.
    Private Const LARGEUR_ECHANTILLON As Integer = 190
    Private Const HAUTEUR_ECHANTILLON As Integer = 64

    ' Colonnes des tableaux d'états (libellé à gauche, puis une colonne par état).
    Private Const X_PREMIERE_COLONNE As Integer = 190
    Private Const LARGEUR_COLONNE As Integer = 210

    ' Dossiers d'icônes.
    Private Const DOSSIER_ICONES_32 As String = "Assets\Boutons_ico_32"
    Private Const DOSSIER_ICONES_24 As String = "Assets\Ico_divers"
    Private Const DOSSIER_ICONES_48 As String = "Assets\Boutons_ico_48"
    Private Const DOSSIER_TITRES As String = "Assets\Titre_Ico"

#End Region

#Region "Variables privées"

    ' Polices partagées par les libellés construits ici (libérées à la fermeture).
    Private ReadOnly _policeLibelle As New Font("Calibri", 10.5F, FontStyle.Bold)
    Private ReadOnly _policeNote As New Font("Calibri", 9.75F, FontStyle.Italic)
    Private ReadOnly _policeChamp As New Font("Calibri", 11.0F)
    Private ReadOnly _policeEchantillon As New Font("Calibri", 9.0F, FontStyle.Bold)
    Private ReadOnly _policesExemples As New List(Of Font)

    ' Images chargées pour la démonstration (libérées à la fermeture).
    Private ReadOnly _images As New List(Of Image)

    ' Fiche type de l'onglet Champs et son bouton de bascule.
    Private _carteFiche As AltheaCarte
    Private _btnBasculeFiche As AltheaBouton
    Private _ficheEnConsultation As Boolean

#End Region

#Region "Surface d'un onglet"

    ' Surface défilante d'un onglet : empile les cartes de haut en bas.
    Private NotInheritable Class Planche

        Public ReadOnly Surface As Panel
        Private _y As Integer = MARGE

        Public Sub New(onglet As TabPage)
            Surface = New Panel With {
                .Dock = DockStyle.Fill,
                .AutoScroll = True,
                .AutoScrollMargin = New Size(0, MARGE),
                .BackColor = UITheme.ColorBeigeClair
            }
            onglet.Controls.Add(Surface)
        End Sub

        Public Function NouvelleCarte(titre As String, hauteur As Integer) As AltheaCarte
            Dim carte As New AltheaCarte With {
                .Titre = titre,
                .Padding = New Padding(12, 8, 12, 10),
                .TabStop = False
            }
            carte.SetBounds(MARGE, _y, LARGEUR_CARTE, hauteur)
            Surface.Controls.Add(carte)
            _y += hauteur + ECART
            Return carte
        End Function

    End Class

#End Region

#Region "Chargement et fermeture"

    Public Sub New()
        InitializeComponent()

        ' Fond beige à la feuille et barre de titre aux couleurs du kit.
        AltheaFondFenetre.Appliquer(Me)
    End Sub

    Private Sub TestKitAlthea_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        UtilsButtons.InitStandardButton(btnFermer)
        enTete.Icone = ChargerImage(DOSSIER_TITRES, "parametres")

        ConstruirePalette()
        ConstruireBoutons()
        ConstruireStructure()
        ConstruireChamps()
        ConstruireIndicateurs()
        ConstruireGrilles()

    End Sub

    Private Sub btnFermer_Click(sender As Object, e As EventArgs) Handles btnFermer.Click
        Close()
    End Sub

    Private Sub TestKitAlthea_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        For Each police In {_policeLibelle, _policeNote, _policeChamp, _policeEchantillon}.Concat(_policesExemples)
            police.Dispose()
        Next
        For Each image In _images
            image.Dispose()
        Next

    End Sub

#End Region

#Region "Onglet Palette et polices"

    ' Couleurs de UITheme lues par réflexion (ordre de déclaration), regroupées selon leur préfixe :
    ' une couleur ajoutée à UITheme apparaît ici sans rien modifier.
    Private Sub ConstruirePalette()

        Dim planche As New Planche(tpPalette)

        Dim couleurs = GetType(UITheme).GetFields(BindingFlags.Public Or BindingFlags.Static).
                       Where(Function(f) f.FieldType Is GetType(Color)).
                       OrderBy(Function(f) f.MetadataToken).
                       ToList()

        Dim parLigne As Integer = Math.Max(1, (LARGEUR_CARTE - 24) \ (LARGEUR_ECHANTILLON + ECART))

        For Each groupe In couleurs.GroupBy(Function(f) GroupeDeCouleur(f.Name))

            Dim liste = groupe.ToList()
            Dim lignes As Integer = CInt(Math.Ceiling(liste.Count / parLigne))
            Dim carte = planche.NouvelleCarte($"{groupe.Key} ({liste.Count})",
                                              Y_CONTENU + lignes * (HAUTEUR_ECHANTILLON + ECART) + 6)

            For i As Integer = 0 To liste.Count - 1
                Dim couleur As Color = DirectCast(liste(i).GetValue(Nothing), Color)
                Poser(carte, CreerEchantillon(liste(i).Name, couleur),
                      12 + (i Mod parLigne) * (LARGEUR_ECHANTILLON + ECART),
                      Y_CONTENU + (i \ parLigne) * (HAUTEUR_ECHANTILLON + ECART))
            Next

        Next

        ConstruirePolices(planche)

    End Sub

    Private Shared Function GroupeDeCouleur(nom As String) As String

        If nom.StartsWith("Color", StringComparison.Ordinal) Then Return "Charte (UITheme.Color…)"
        If nom.StartsWith("ButtonStandard", StringComparison.Ordinal) Then Return "Boutons standard (UtilsButtons)"
        If nom.StartsWith("ButtonHome", StringComparison.Ordinal) Then Return "Menu de la Home"
        If nom.StartsWith("DataGrid", StringComparison.Ordinal) Then Return "Grilles (UtilsDataGrid)"
        If nom.StartsWith("Statut", StringComparison.Ordinal) Then Return "Statuts dans les grilles"
        If nom.StartsWith("Dynamic", StringComparison.Ordinal) Then Return "Écrans construits par le code"
        Return "Autres"

    End Function

    ' Échantillon : la couleur, son nom UITheme et ses valeurs RVB.
    Private Function CreerEchantillon(nom As String, couleur As Color) As Control

        Dim panneau As New Panel With {.Size = New Size(LARGEUR_ECHANTILLON, HAUTEUR_ECHANTILLON), .BackColor = Color.Transparent}

        panneau.Controls.Add(New Label With {
            .Bounds = New Rectangle(0, 0, LARGEUR_ECHANTILLON, 28),
            .BackColor = couleur,
            .BorderStyle = BorderStyle.FixedSingle
        })
        panneau.Controls.Add(New Label With {
            .Bounds = New Rectangle(0, 30, LARGEUR_ECHANTILLON, 17),
            .AutoEllipsis = True,
            .BackColor = Color.Transparent,
            .Font = _policeEchantillon,
            .ForeColor = UITheme.ColorGrisFonce,
            .Text = nom,
            .UseMnemonic = False
        })
        panneau.Controls.Add(New Label With {
            .Bounds = New Rectangle(0, 47, LARGEUR_ECHANTILLON, 16),
            .BackColor = Color.Transparent,
            .Font = _policeNote,
            .ForeColor = UITheme.ColorGrisDiscret,
            .Text = $"{couleur.R}; {couleur.G}; {couleur.B}"
        })

        Return panneau

    End Function

    ' Les polices d'usage (Reference_UI § 1.2), chacune écrite dans son propre style.
    Private Sub ConstruirePolices(planche As Planche)

        Dim exemples As (Texte As String, Taille As Single, Style As FontStyle, Couleur As Color, Usage As String)() = {
            ("Titre d'écran", 18.0F, FontStyle.Bold, UITheme.ColorMauveFonce, "AltheaEnTete — Calibri 18 gras, Mauve foncé"),
            ("Sous-titre d'écran", 11.0F, FontStyle.Regular, UITheme.ColorSaugeFonce, "AltheaEnTete.SousTitre — Calibri 11, Sauge foncé"),
            ("Titre de carte", 12.0F, FontStyle.Bold, UITheme.ColorMauveFonce, "AltheaCarte.Titre — Calibri 12 gras, Mauve foncé"),
            ("Libellé de champ", 10.5F, FontStyle.Bold, UITheme.ColorSaugeFonce, "Calibri 10,5 gras, Sauge foncé"),
            ("Valeur d'un champ", 11.0F, FontStyle.Regular, UITheme.ColorGrisFonce, "Champs — Calibri 11, Gris foncé"),
            ("Consigne dans une carte", 10.0F, FontStyle.Italic, UITheme.ColorSaugeFonce, "Calibri 10 italique, Sauge foncé"),
            ("Message d'erreur", 11.0F, FontStyle.Bold, UITheme.ColorRoseFonce, "Calibri 11 gras, Rose foncé"),
            ("Texte secondaire", 9.75F, FontStyle.Regular, UITheme.ColorGrisDiscret, "Aides, remarques — Calibri 9,75, Gris discret")
        }

        Const PAS As Integer = 32
        Dim carte = planche.NouvelleCarte("Polices (une seule famille : Calibri)", Y_CONTENU + exemples.Length * PAS + 4)

        For i As Integer = 0 To exemples.Length - 1
            Dim exemple = exemples(i)
            Dim police As New Font("Calibri", exemple.Taille, exemple.Style)
            _policesExemples.Add(police)

            Dim y As Integer = Y_CONTENU + i * PAS
            Poser(carte, New Label With {.AutoSize = True, .BackColor = Color.Transparent, .Font = police,
                                         .ForeColor = exemple.Couleur, .Text = exemple.Texte}, 12, y)
            Poser(carte, Note(exemple.Usage), 320, y + 6)
        Next

    End Sub

#End Region

#Region "Onglet Boutons"

    Private Sub ConstruireBoutons()

        Dim planche As New Planche(tpBoutons)

        ' Boutons d'action : une ligne par variante, une colonne par état.
        Dim actions As (Libelle As String, Variante As VarianteBouton, Texte As String, Icone As String)() = {
            ("Sauge — action courante", VarianteBouton.Sauge, "Valider", "valider_normal"),
            ("Ocre — Enregistrer", VarianteBouton.Ocre, "Enregistrer", "enregistrer_normal"),
            ("Terre — Supprimer", VarianteBouton.Terre, "Supprimer", "supprimer_normal"),
            ("Clair — Annuler, Fermer", VarianteBouton.Clair, "Annuler", "annuler_normal")
        }

        Const PAS_ACTION As Integer = 50
        Dim carteActions = planche.NouvelleCarte("Boutons d'action — AltheaBouton (barre d'actions : hauteur 40, icône 32 px par UtilsButtons)",
                                                 Y_CONTENU + 22 + actions.Length * PAS_ACTION)
        PoserEntetes(carteActions, "Normal", "Désactivé", "Sans icône")

        For i As Integer = 0 To actions.Length - 1
            Dim a = actions(i)
            Dim y As Integer = Y_CONTENU + 22 + i * PAS_ACTION
            Poser(carteActions, Libelle(a.Libelle), 12, y + 11)
            Poser(carteActions, BoutonAction(a.Texte, a.Variante, a.Icone, True), Colonne(0), y)
            Poser(carteActions, BoutonAction(a.Texte, a.Variante, a.Icone, False), Colonne(1), y)
            Poser(carteActions, BoutonAction(a.Texte, a.Variante, Nothing, True), Colonne(2), y)
        Next

        ' Boutons Outil : barres locales et petits boutons à côté d'un champ.
        Dim outils As (Texte As String, Variante As VarianteBouton, Icone As String)() = {
            ("Ajouter", VarianteBouton.OutilPrincipal, "plus_24_normal"),
            ("Désactiver", VarianteBouton.Outil, Nothing),
            ("Supprimer", VarianteBouton.OutilDanger, Nothing)
        }
        Dim iconesSeules As String() = {"oeil_24_normal", "cadenas_24_normal", "plus_24_normal", "importer_24_normal",
                                        "compte_24_normal", "lier_24_normal", "delier_24_normal", "cachet_24_normal"}

        Dim carteOutils = planche.NouvelleCarte("Boutons Outil — barres locales et petits boutons (hauteur 30, icône 24 px du Designer, sans UtilsButtons)",
                                                Y_CONTENU + 22 + 5 * 40)
        PoserEntetes(carteOutils, "Normal", "Désactivé")

        For i As Integer = 0 To outils.Length - 1
            Dim o = outils(i)
            Dim y As Integer = Y_CONTENU + 22 + i * 40
            Poser(carteOutils, Libelle(o.Variante.ToString()), 12, y + 6)
            Poser(carteOutils, BoutonOutil(o.Texte, o.Variante, o.Icone, True), Colonne(0), y)
            Poser(carteOutils, BoutonOutil(o.Texte, o.Variante, o.Icone, False), Colonne(1), y)
        Next

        Dim yIcones As Integer = Y_CONTENU + 22 + outils.Length * 40
        ' Icône seule : une ligne par état (huit boutons ne tiennent pas dans une colonne).
        Poser(carteOutils, Libelle("Icône seule, 30 × 30"), 12, yIcones + 6)
        Poser(carteOutils, Libelle("Icône seule, désactivé"), 12, yIcones + 46)
        For i As Integer = 0 To iconesSeules.Length - 1
            Poser(carteOutils, BoutonOutil(Nothing, VarianteBouton.Outil, iconesSeules(i), True), Colonne(0) + i * 36, yIcones)
            Poser(carteOutils, BoutonOutil(Nothing, VarianteBouton.Outil, iconesSeules(i), False), Colonne(0) + i * 36, yIcones + 40)
        Next

        ' Tuiles des hubs.
        Dim carteTuiles = planche.NouvelleCarte("Tuiles — AltheaTuile (hubs : 214 × 90, icône 48 px)", Y_CONTENU + 90 + 14)
        Poser(carteTuiles, Tuile("Domaines" & vbCrLf & "Prises en charge", "domaines_normal", True), 12, Y_CONTENU)
        Poser(carteTuiles, Tuile("Thérapeutes" & vbCrLf & "Intervenants", "therapeutes_normal", True), 12 + 214 + 16, Y_CONTENU)
        Poser(carteTuiles, Tuile("Cabinet" & vbCrLf & "Tuile désactivée", "cabinet_normal", False), 12 + 2 * (214 + 16), Y_CONTENU)

        ' Lignes cliquables (Suivi clinique).
        Dim carteLignes = planche.NouvelleCarte("Ligne cliquable — AltheaLigne (Suivi clinique : toute la ligne réagit au survol)", Y_CONTENU + 3 * 54 + 10)
        Poser(carteLignes, Ligne("Motif de suivi", "Difficultés de concentration en classe (exemple)", "MotifSuivi", True), 12, Y_CONTENU)
        Poser(carteLignes, Ligne("Objectifs du suivi", "", "ObjectifsSuivi", True), 12, Y_CONTENU + 54)
        Poser(carteLignes, Ligne("Antécédents", "Ligne désactivée", "Antecedents", False), 12, Y_CONTENU + 108)

        ' Menu de la Home : posé sur un fond aux couleurs du menu, comme dans la Home.
        Dim carteMenu = planche.NouvelleCarte("Menu de la Home — AltheaBoutonMenu (au repos, écran actif, désactivé)", Y_CONTENU + 3 * 46 + 26)
        Dim fondMenu As New Panel With {.BackColor = UITheme.ColorBeige, .Size = New Size(250, 3 * 46 + 12)}
        Dim boutonsMenu As New List(Of Button)
        For i As Integer = 0 To 2
            Dim nom As String = {"accueil_normal", "patients_normal", "agenda_normal"}(i)
            Dim btn As New AltheaBoutonMenu With {
                .Bounds = New Rectangle(8, 6 + i * 46, 234, 42),
                .Tag = nom,
                .Text = {"Accueil", "Patients (écran actif)", "Agenda (désactivé)"}(i)
            }
            UtilsButtons.InitHomeMenuButton(btn)
            fondMenu.Controls.Add(btn)
            boutonsMenu.Add(btn)
        Next
        UtilsButtons.SetSelectedHomeMenuButton(boutonsMenu(1), boutonsMenu)
        boutonsMenu(2).Enabled = False
        Poser(carteMenu, fondMenu, 12, Y_CONTENU)

    End Sub

    ' Raison montrée au survol des boutons désactivés de la vitrine (AltheaBouton.RaisonInactif).
    Private Const RAISON_EXEMPLE As String = "Exemple : sélectionnez d'abord un élément dans la liste."

    Private Function BoutonAction(texte As String, variante As VarianteBouton, icone As String, actif As Boolean) As AltheaBouton

        Dim btn As New AltheaBouton With {.Text = texte, .Variante = variante}
        If icone IsNot Nothing Then
            btn.Tag = icone
            UtilsButtons.InitStandardButton(btn)
        End If
        btn.Size = New Size(TextRenderer.MeasureText(texte, btn.Font).Width + If(icone Is Nothing, 40, 72), 40)
        btn.DefinirDisponibilite(actif, RAISON_EXEMPLE)
        Return btn

    End Function

    Private Function BoutonOutil(texte As String, variante As VarianteBouton, icone As String, actif As Boolean) As AltheaBouton

        Dim btn As New AltheaBouton With {.Variante = variante}

        If icone IsNot Nothing Then btn.Image = ChargerImage(DOSSIER_ICONES_24, icone)

        If String.IsNullOrEmpty(texte) Then
            btn.Size = New Size(30, 30)
        Else
            btn.Text = texte
            btn.Size = New Size(TextRenderer.MeasureText(texte, btn.Font).Width + If(icone Is Nothing, 24, 52), 30)
        End If

        btn.DefinirDisponibilite(actif, RAISON_EXEMPLE)
        Return btn

    End Function

    Private Function Tuile(texte As String, icone As String, actif As Boolean) As AltheaTuile
        Return New AltheaTuile With {
            .Text = texte,
            .Image = ChargerImage(DOSSIER_ICONES_48, icone),
            .Size = New Size(214, 90),
            .Enabled = actif
        }
    End Function

    Private Function Ligne(titre As String, description As String, icone As String, actif As Boolean) As AltheaLigne
        Return New AltheaLigne With {
            .Text = titre,
            .Description = description,
            .TexteVide = "Rien de saisi pour l'instant",
            .TexteAction = "Ouvrir",
            .Image = ChargerImage("Assets\Boutons_Suivi", icone),
            .Size = New Size(LARGEUR_CARTE - 24, 54),
            .Enabled = actif
        }
    End Function

#End Region

#Region "Onglet Structure"

    Private Sub ConstruireStructure()

        Dim planche As New Planche(tpStructure)

        ' En-têtes (AltheaEnTete se met en Dock Top par défaut : on le libère pour le poser).
        Dim carteEnTetes = planche.NouvelleCarte("En-tête — AltheaEnTete (fenêtres : hauteur 62 ; Dock Top dans un écran)", Y_CONTENU + 62 + ECART + 62 + 6)
        Dim enTeteComplet As New AltheaEnTete With {
            .Dock = DockStyle.None,
            .Text = "Cabinet et praticien",
            .SousTitre = "Avec icône (Titre_Ico) et sous-titre",
            .Icone = ChargerImage(DOSSIER_TITRES, "cabinet")
        }
        enTeteComplet.SetBounds(12, Y_CONTENU, 700, 62)
        carteEnTetes.Controls.Add(enTeteComplet)
        Dim enTeteSimple As New AltheaEnTete With {.Dock = DockStyle.None, .Text = "Titre seul"}
        enTeteSimple.SetBounds(12, Y_CONTENU + 62 + ECART, 700, 62)
        carteEnTetes.Controls.Add(enTeteSimple)

        ' Cartes titrée et sans titre.
        Dim carteCartes = planche.NouvelleCarte("Cartes — AltheaCarte (fenêtres : Padding 12; 8; 12; 10, premier champ à y = 40)", Y_CONTENU + 130 + 14)
        Dim carteTitree As New AltheaCarte With {.Titre = "Coordonnées", .Padding = New Padding(12, 8, 12, 10), .TabStop = False}
        carteTitree.SetBounds(12, Y_CONTENU, 490, 130)
        PoserChamp(carteTitree, "Téléphone", New TextBox With {.Text = "+32 2 000 00 00"}, 0)
        PoserChamp(carteTitree, "E-mail", New TextBox With {.Text = "exemple@exemple.be"}, 1)
        carteCartes.Controls.Add(carteTitree)

        Dim carteSansTitre As New AltheaCarte With {.Padding = New Padding(16, 12, 16, 14), .TabStop = False}
        carteSansTitre.SetBounds(12 + 490 + 16, Y_CONTENU, 490, 130)
        Dim consigne As Label = Note("Carte sans titre : un message, une consigne ou une grille qui n'a pas besoin de nom.")
        consigne.AutoSize = False
        consigne.Bounds = New Rectangle(16, 14, 450, 60)
        carteSansTitre.Controls.Add(consigne)
        carteCartes.Controls.Add(carteSansTitre)

        ' Carte avec barre locale et grille (ADR-34).
        Dim carteListe = planche.NouvelleCarte("Carte avec barre locale — AltheaBarreOutils (Dock Top) + grille (Dock Fill), ADR-34", Y_CONTENU + 200 + 10)
        Dim carteGrille As New AltheaCarte With {.Padding = New Padding(12, 8, 12, 12), .TabStop = False}
        carteGrille.SetBounds(12, Y_CONTENU, LARGEUR_CARTE - 24, 200)
        Dim grille As DataGridView = GrilleExemple()
        grille.Dock = DockStyle.Fill
        Dim barre As New AltheaBarreOutils With {.Titre = "Catégories", .Dock = DockStyle.Top}
        Dim xBouton As Integer = 120
        For Each o In {("Ajouter", VarianteBouton.OutilPrincipal, "plus_24_normal"),
                       ("Désactiver", VarianteBouton.Outil, CStr(Nothing)),
                       ("Supprimer", VarianteBouton.OutilDanger, CStr(Nothing))}
            Dim btn As AltheaBouton = BoutonOutil(o.Item1, o.Item2, o.Item3, True)
            btn.Location = New Point(xBouton, 6)
            barre.Controls.Add(btn)
            xBouton += btn.Width + 6
        Next
        ' La grille d'abord, la barre ensuite : le docking se calcule à l'envers de l'ordre d'ajout.
        carteGrille.Controls.Add(grille)
        carteGrille.Controls.Add(barre)
        carteListe.Controls.Add(carteGrille)

        ' Barre d'actions.
        Dim carteBarreActions = planche.NouvelleCarte("Barre d'actions — AltheaBarreActions (vague ; Enregistrer en Ocre, Annuler en Clair)", Y_CONTENU + 57 + 10)
        Dim barreActions As New AltheaBarreActions With {.Dock = DockStyle.None}
        barreActions.SetBounds(12, Y_CONTENU, LARGEUR_CARTE - 24, 57)
        Dim btnAnnuler As AltheaBouton = BoutonAction("Annuler", VarianteBouton.Clair, "annuler_normal", True)
        Dim btnEnregistrer As AltheaBouton = BoutonAction("Enregistrer", VarianteBouton.Ocre, "enregistrer_normal", True)
        btnAnnuler.Location = New Point(barreActions.Width - 16 - btnAnnuler.Width, 8)
        btnEnregistrer.Location = New Point(btnAnnuler.Left - 8 - btnEnregistrer.Width, 8)
        barreActions.Controls.Add(btnEnregistrer)
        barreActions.Controls.Add(btnAnnuler)
        carteBarreActions.Controls.Add(barreActions)

        ' Titre de section, pastilles, fil d'Ariane.
        Dim carteDivers = planche.NouvelleCarte("Titre de section, pastilles, fil d'Ariane", Y_CONTENU + 3 * 40 + 6)
        Dim titreSection As New AltheaTitreSection With {.Text = "Titre de section (AltheaTitreSection)"}
        titreSection.SetBounds(12, Y_CONTENU, 500, 28)
        carteDivers.Controls.Add(titreSection)

        Dim xPastille As Integer = 12
        For Each p In {("Psychologie", UITheme.ColorGrisVertClair, UITheme.ColorGrisFonce),
                       ("Réglé", UITheme.StatutRegleBack, UITheme.StatutRegleFore),
                       ("Dû", UITheme.StatutDuBack, UITheme.StatutDuFore),
                       ("Prévisionnel", UITheme.StatutPrevisionnelBack, UITheme.StatutPrevisionnelFore),
                       ("Annulé", UITheme.StatutAnnuleBack, UITheme.StatutAnnuleFore),
                       ("Absent", UITheme.StatutNoShowBack, UITheme.StatutNoShowFore)}
            Dim pastille As New AltheaPastille With {.AutoSize = True, .Text = p.Item1, .CouleurFond = p.Item2, .ForeColor = p.Item3}
            Poser(carteDivers, pastille, xPastille, Y_CONTENU + 42)
            xPastille += pastille.Width + 10
        Next

        Dim fil As New AltheaFilAriane With {.Text = "Patients > Alex Exemple (P-0012) > Identité", .Pastille = "Psychologie"}
        fil.SetBounds(12, Y_CONTENU + 80, 700, 28)
        carteDivers.Controls.Add(fil)

    End Sub

#End Region

#Region "Onglet Champs"

    Private Sub ConstruireChamps()

        Dim planche As New Planche(tpChamps)

        ' Tableau des états : une ligne par contrôle, une colonne par état.
        Const PAS As Integer = 40
        Dim carteEtats = planche.NouvelleCarte("États d'un champ (règle « Consultation lisible » : texte normal sur fond teinté)",
                                               Y_CONTENU + 22 + 6 * PAS + 28)
        PoserEntetes(carteEtats, "Saisie", "Consultation", "Désactivé", "Variante")

        Dim y As Integer = Y_CONTENU + 22

        ' Zone de texte.
        Poser(carteEtats, Libelle("Zone de texte"), 12, y + 4)
        Poser(carteEtats, ZoneTexte("Exemple"), Colonne(0), y)
        Dim texteConsulte As TextBox = ZoneTexte("Exemple")
        UtilsControls.DefinirChampLectureSeule(texteConsulte, True)
        Poser(carteEtats, texteConsulte, Colonne(1), y)
        Dim texteDesactive As TextBox = ZoneTexte("Exemple")
        texteDesactive.Enabled = False
        Poser(carteEtats, texteDesactive, Colonne(2), y)
        Dim texteErreur As TextBox = ZoneTexte("")
        texteErreur.BackColor = AltheaIconeEtat.Teintes(EtatIcone.Erreur).Pale
        Poser(carteEtats, texteErreur, Colonne(3), y)
        Poser(carteEtats, Note("champ refusé"), Colonne(3) + 186, y + 5)
        y += PAS

        ' Liste.
        Poser(carteEtats, Libelle("Liste (AltheaListe)"), 12, y + 4)
        Poser(carteEtats, Liste(Nothing), Colonne(0), y)
        Dim listeConsultee As AltheaListe = Liste(Nothing)
        UtilsControls.DefinirChampLectureSeule(listeConsultee, True)
        Poser(carteEtats, listeConsultee, Colonne(1), y)
        Dim listeDesactivee As AltheaListe = Liste(Nothing)
        listeDesactivee.Enabled = False
        Poser(carteEtats, listeDesactivee, Colonne(2), y)
        Poser(carteEtats, Liste("Pays"), Colonne(3), y)
        y += PAS

        ' Date.
        Poser(carteEtats, Libelle("Date (AltheaDate)"), 12, y + 4)
        Poser(carteEtats, ChampDate(Date.Today, False), Colonne(0), y)
        Dim dateConsultee As AltheaDate = ChampDate(Date.Today, False)
        UtilsControls.DefinirChampLectureSeule(dateConsultee, True)
        Poser(carteEtats, dateConsultee, Colonne(1), y)
        Dim dateDesactivee As AltheaDate = ChampDate(Date.Today, False)
        dateDesactivee.Enabled = False
        Poser(carteEtats, dateDesactivee, Colonne(2), y)
        Poser(carteEtats, ChampDate(Nothing, True), Colonne(3), y)
        Poser(carteEtats, Note("facultative, vide"), Colonne(3) + 156, y + 5)
        y += PAS

        ' Heure.
        Poser(carteEtats, Libelle("Heure (AltheaHeure)"), 12, y + 4)
        Poser(carteEtats, ChampHeure(New TimeSpan(9, 30, 0), False), Colonne(0), y)
        Dim heureConsultee As AltheaHeure = ChampHeure(New TimeSpan(9, 30, 0), False)
        UtilsControls.DefinirChampLectureSeule(heureConsultee, True)
        Poser(carteEtats, heureConsultee, Colonne(1), y)
        Dim heureDesactivee As AltheaHeure = ChampHeure(New TimeSpan(9, 30, 0), False)
        heureDesactivee.Enabled = False
        Poser(carteEtats, heureDesactivee, Colonne(2), y)
        Poser(carteEtats, ChampHeure(New TimeSpan(10, 15, 0), True), Colonne(3), y)
        Poser(carteEtats, Note("facultative, croix"), Colonne(3) + 126, y + 5)
        y += PAS

        ' Case.
        Poser(carteEtats, Libelle("Case (AltheaCase)"), 12, y + 4)
        Poser(carteEtats, CaseACocher(True), Colonne(0), y)
        Dim caseConsultee As AltheaCase = CaseACocher(True)
        UtilsControls.DefinirChampLectureSeule(caseConsultee, True)
        Poser(carteEtats, caseConsultee, Colonne(1), y)
        Dim caseDesactivee As AltheaCase = CaseACocher(True)
        caseDesactivee.Enabled = False
        Poser(carteEtats, caseDesactivee, Colonne(2), y)
        Poser(carteEtats, CaseACocher(False), Colonne(3), y)
        y += PAS

        ' Recherche.
        Poser(carteEtats, Libelle("Recherche"), 12, y + 4)
        Poser(carteEtats, Recherche(""), Colonne(0), y)
        Poser(carteEtats, Note("— (pas de consultation)"), Colonne(1), y + 6)
        Dim rechercheDesactivee As AltheaRecherche = Recherche("")
        rechercheDesactivee.Enabled = False
        Poser(carteEtats, rechercheDesactivee, Colonne(2), y)
        Poser(carteEtats, Recherche("Exemple"), Colonne(3), y)
        y += PAS

        Poser(carteEtats, Note("En consultation, listes et cases du kit sont désactivées mais se dessinent lisibles (UtilsControls.DefinirChampLectureSeule)."), 12, y + 4)

        ' Fiche type : libellés et champs posés selon la règle des fenêtres, avec bascule.
        Dim carteFicheConteneur = planche.NouvelleCarte("Fiche type — libellés à gauche, champs à x = 170, pas de 34 px", Y_CONTENU + 5 * 34 + 40)
        _carteFiche = New AltheaCarte With {.Titre = "Identité (exemple)", .Padding = New Padding(12, 8, 12, 10), .TabStop = False}
        _carteFiche.SetBounds(12, Y_CONTENU, 520, Y_CONTENU + 5 * 34 + 4)
        PoserChamp(_carteFiche, "Nom", ZoneTexte("Exemple"), 0)
        PoserChamp(_carteFiche, "Prénom", ZoneTexte("Alex"), 1)
        PoserChamp(_carteFiche, "Date de naissance", ChampDate(New Date(1990, 5, 12), True), 2)
        PoserChamp(_carteFiche, "Pays", Liste(Nothing), 3)
        PoserChamp(_carteFiche, "Suivi", CaseACocher(True), 4)
        carteFicheConteneur.Controls.Add(_carteFiche)

        _btnBasculeFiche = BoutonOutil("Passer en consultation", VarianteBouton.Outil, Nothing, True)
        AddHandler _btnBasculeFiche.Click, AddressOf BasculeFiche_Click
        Poser(carteFicheConteneur, _btnBasculeFiche, 12 + 520 + 16, Y_CONTENU)
        Dim aide As Label = Note("Bascule la carte entière avec UtilsControls.DefinirLectureSeule(carte, …), comme les fiches réelles.")
        aide.AutoSize = False
        aide.Bounds = New Rectangle(12 + 520 + 16, Y_CONTENU + 40, 440, 40)
        carteFicheConteneur.Controls.Add(aide)

    End Sub

    Private Sub BasculeFiche_Click(sender As Object, e As EventArgs)
        _ficheEnConsultation = Not _ficheEnConsultation
        UtilsControls.DefinirLectureSeule(_carteFiche, _ficheEnConsultation)
        _btnBasculeFiche.Text = If(_ficheEnConsultation, "Repasser en saisie", "Passer en consultation")
    End Sub

    Private Function ZoneTexte(texte As String) As TextBox
        Return New TextBox With {.Font = _policeChamp, .ForeColor = UITheme.ColorGrisFonce, .Size = New Size(180, 25), .Text = texte}
    End Function

    Private Function Liste(etiquette As String) As AltheaListe
        Dim champ As New AltheaListe With {.DropDownStyle = ComboBoxStyle.DropDownList, .Font = _policeChamp, .Size = New Size(180, 26)}
        champ.Items.AddRange(New Object() {"Belgique", "France", "Luxembourg"})
        champ.SelectedIndex = 0
        If etiquette IsNot Nothing Then champ.Etiquette = etiquette
        Return champ
    End Function

    Private Shared Function ChampDate(valeur As Date?, facultative As Boolean) As AltheaDate
        Return New AltheaDate With {.Facultative = facultative, .Valeur = valeur, .Size = New Size(150, 26)}
    End Function

    ' Heures proposées de démonstration : 8:00 à 20:00 par quart d'heure.
    Private Shared Function ChampHeure(valeur As TimeSpan?, facultative As Boolean) As AltheaHeure
        Dim heures As New List(Of TimeSpan)
        For minutes As Integer = 8 * 60 To 20 * 60 Step 15
            heures.Add(TimeSpan.FromMinutes(minutes))
        Next
        Return New AltheaHeure With {.Facultative = facultative, .Valeur = valeur, .HeuresProposees = heures,
                                     .Size = New Size(120, 26)}
    End Function

    Private Shared Function CaseACocher(cochee As Boolean) As AltheaCase
        Return New AltheaCase With {.Text = "Actif", .Checked = cochee, .AutoSize = True, .BackColor = Color.Transparent}
    End Function

    Private Shared Function Recherche(texte As String) As AltheaRecherche
        Return New AltheaRecherche With {.TexteAide = "Rechercher un patient…", .Text = texte, .Size = New Size(200, 32)}
    End Function

#End Region

#Region "Onglet Indicateurs"

    Private Sub ConstruireIndicateurs()

        Dim planche As New Planche(tpIndicateurs)

        ' Icônes d'état : six types, trois tailles.
        Dim etats As EtatIcone() = DirectCast([Enum].GetValues(GetType(EtatIcone)), EtatIcone())
        Dim tailles As Integer() = {48, 32, 20}
        Dim carteIcones = planche.NouvelleCarte("Icônes d'état — AltheaIconeEtat (48, 32, 20 px ; l'Attente tourne)", Y_CONTENU + 48 + 22 + 6)
        For i As Integer = 0 To etats.Length - 1
            Dim x As Integer = 12 + i * 165
            Dim xIcone As Integer = x
            For Each taille In tailles
                Dim icone As New AltheaIconeEtat With {.Etat = etats(i), .Size = New Size(taille, taille)}
                Poser(carteIcones, icone, xIcone, Y_CONTENU + (48 - taille) \ 2)
                xIcone += taille + 8
            Next
            Poser(carteIcones, Libelle(etats(i).ToString()), x, Y_CONTENU + 52)
        Next

        ' Jauges.
        Dim jauges As (Texte As String, Valeur As Integer, Seuil As Integer)() = {
            ("3 / 10 — normal (Sauge)", 3, 8),
            ("8 / 10 — seuil d'alerte atteint (Ocre)", 8, 8),
            ("10 / 10 — plafond (Terre)", 10, 8)
        }
        Dim carteJauges = planche.NouvelleCarte("Jauges — AltheaJauge (quota consommé)", Y_CONTENU + jauges.Length * 28 + 4)
        For i As Integer = 0 To jauges.Length - 1
            Dim j = jauges(i)
            Dim y As Integer = Y_CONTENU + i * 28
            Poser(carteJauges, New AltheaJauge With {.Maximum = 10, .SeuilAlerte = j.Seuil, .Valeur = j.Valeur, .Size = New Size(300, 8)}, 12, y + 6)
            Poser(carteJauges, Note(j.Texte), 330, y)
        Next

        ' Étapes d'un assistant.
        Dim carteEtapes = planche.NouvelleCarte("Étapes — AltheaEtapes (fait, fait, à compléter et affichée, à faire, non nécessaire)", Y_CONTENU + 84 + 10)
        Dim etapes As New AltheaEtapes With {.Etapes = {"1. Général", "2. Tarifs", "3. Natures", "4. Quotas", "5. Synthèse"}}
        etapes.SetBounds(12, Y_CONTENU, LARGEUR_CARTE - 24, 84)
        etapes.DefinirEtat(0, EtatEtape.Fait)
        etapes.DefinirEtat(1, EtatEtape.Fait)
        etapes.DefinirEtat(2, EtatEtape.ACompleter, "1 nature sans tarif")
        etapes.DefinirEtat(3, EtatEtape.AFaire)
        etapes.DefinirEtat(4, EtatEtape.NonNecessaire)
        etapes.IndexCourant = 2
        carteEtapes.Controls.Add(etapes)

        ' Utilisateur connecté (bandeau de la Home).
        Dim cartePuces = planche.NouvelleCarte("Utilisateur connecté — AltheaPuceUtilisateur (initiales à défaut de photo ; élévation active)", Y_CONTENU + 46 + 12)
        Poser(cartePuces, New AltheaPuceUtilisateur With {.Nom = "Alex Exemple", .Precision = "Psychologue", .Role = "SuperUser"}, 12, Y_CONTENU)
        Poser(cartePuces, New AltheaPuceUtilisateur With {.Nom = "Alex Exemple", .Precision = "Psychologue", .Role = "Admin", .Eleve = True}, 320, Y_CONTENU)

        ' Calendrier.
        Dim tailleCalendrier As Size = AltheaCalendrier.TailleConseillee()
        Dim carteCalendrier = planche.NouvelleCarte("Calendrier — AltheaCalendrier (ouvert par AltheaDate)", Y_CONTENU + tailleCalendrier.Height + 22)
        Poser(carteCalendrier, New AltheaCalendrier With {.DateChoisie = Date.Today}, 12, Y_CONTENU)

    End Sub

#End Region

#Region "Onglet Grilles"

    Private Sub ConstruireGrilles()

        Dim planche As New Planche(tpGrilles)

        Dim carteBasique = planche.NouvelleCarte("Grille principale — UtilsDataGrid.InitDataGridBasique", Y_CONTENU + 190 + 6)
        Dim grilleBasique As DataGridView = GrilleExemple()
        grilleBasique.SetBounds(12, Y_CONTENU, LARGEUR_CARTE - 24, 190)
        carteBasique.Controls.Add(grilleBasique)

        Dim carteSecondaire = planche.NouvelleCarte("Grille secondaire et couleurs de statut — UtilsDataGrid.InitDataGridSecondaire, UITheme.Statut…", Y_CONTENU + 190 + 6)
        Dim grilleSecondaire As New DataGridView With {
            .AllowUserToAddRows = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .RowHeadersVisible = False
        }
        UtilsDataGrid.InitDataGridSecondaire(grilleSecondaire)
        grilleSecondaire.Columns.Add("colDate", "Date")
        grilleSecondaire.Columns.Add("colSeance", "Séance")
        grilleSecondaire.Columns.Add("colMontant", "Montant")
        grilleSecondaire.Columns.Add("colStatut", "Statut")

        For Each s In {("02/10/2026", "Séance individuelle", "60,00 €", "Réglé", UITheme.StatutRegleBack, UITheme.StatutRegleFore),
                       ("09/10/2026", "Séance individuelle", "60,00 €", "Dû", UITheme.StatutDuBack, UITheme.StatutDuFore),
                       ("16/10/2026", "Bilan", "90,00 €", "Prévisionnel", UITheme.StatutPrevisionnelBack, UITheme.StatutPrevisionnelFore),
                       ("23/10/2026", "Séance individuelle", "60,00 €", "Annulé", UITheme.StatutAnnuleBack, UITheme.StatutAnnuleFore),
                       ("30/10/2026", "Séance individuelle", "60,00 €", "Absent non excusé", UITheme.StatutNoShowBack, UITheme.StatutNoShowFore)}
            Dim index As Integer = grilleSecondaire.Rows.Add(s.Item1, s.Item2, s.Item3, s.Item4)
            grilleSecondaire.Rows(index).DefaultCellStyle.BackColor = s.Item5
            grilleSecondaire.Rows(index).DefaultCellStyle.ForeColor = s.Item6
        Next

        grilleSecondaire.SetBounds(12, Y_CONTENU, LARGEUR_CARTE - 24, 190)
        carteSecondaire.Controls.Add(grilleSecondaire)

    End Sub

    ' Grille principale d'exemple (référentiel) : utilisée ici et dans la carte à barre locale.
    Private Shared Function GrilleExemple() As DataGridView

        Dim grille As New DataGridView With {
            .AllowUserToAddRows = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .RowHeadersVisible = False
        }
        UtilsDataGrid.InitDataGridBasique(grille)
        grille.Columns.Add("colCode", "Code")
        grille.Columns.Add("colLibelle", "Libellé")
        grille.Columns.Add("colOrdre", "Ordre")
        grille.Columns.Add(New DataGridViewCheckBoxColumn With {.Name = "colActif", .HeaderText = "Actif"})

        grille.Rows.Add("SOUT", "Soutien", "1", True)
        grille.Rows.Add("TRAIT", "Traitement", "2", True)
        grille.Rows.Add("BILAN", "Bilan", "3", True)
        grille.Rows.Add("ANC", "Ancienne catégorie", "4", False)

        Return grille

    End Function

#End Region

#Region "Outils de construction"

    Private Shared Sub Poser(parent As Control, ctrl As Control, x As Integer, y As Integer)
        ctrl.Location = New Point(x, y)
        parent.Controls.Add(ctrl)
    End Sub

    Private Shared Function Colonne(index As Integer) As Integer
        Return X_PREMIERE_COLONNE + index * LARGEUR_COLONNE
    End Function

    ' En-têtes des colonnes d'états, juste sous le titre de la carte.
    Private Sub PoserEntetes(carte As AltheaCarte, ParamArray titres As String())
        For i As Integer = 0 To titres.Length - 1
            Poser(carte, Note(titres(i)), Colonne(i), Y_CONTENU)
        Next
    End Sub

    ' Libellé de champ (Calibri 10,5 gras, Sauge foncé).
    Private Function Libelle(texte As String) As Label
        Return New Label With {
            .AutoSize = True,
            .BackColor = Color.Transparent,
            .Font = _policeLibelle,
            .ForeColor = UITheme.ColorSaugeFonce,
            .Text = texte,
            .UseMnemonic = False
        }
    End Function

    ' Texte d'explication (Calibri 9,75 italique, Gris discret).
    Private Function Note(texte As String) As Label
        Return New Label With {
            .AutoSize = True,
            .BackColor = Color.Transparent,
            .Font = _policeNote,
            .ForeColor = UITheme.ColorGrisDiscret,
            .Text = texte,
            .UseMnemonic = False
        }
    End Function

    ' Ligne « libellé + champ » d'une carte, selon la règle des fenêtres (libellé à y + 4).
    Private Sub PoserChamp(carte As AltheaCarte, texteLibelle As String, champ As Control, ligne As Integer)
        Dim y As Integer = Y_CONTENU + ligne * 34
        Poser(carte, Libelle(texteLibelle), 12, y + 4)
        Poser(carte, champ, 170, y)
    End Sub

    ' Image d'un dossier d'assets, gardée pour être libérée à la fermeture (Nothing si absente).
    Private Function ChargerImage(dossier As String, nom As String) As Image
        Dim image As Image = UtilsButtons.LoadImageFromAsset(dossier, nom)
        If image IsNot Nothing Then _images.Add(image)
        Return image
    End Function

#End Region

End Class
