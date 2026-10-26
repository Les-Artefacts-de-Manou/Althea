' -------------------------------------------------------------------------------------------------
' UserControl : UC_SyntheseDomaine
' Projet      : Althéa
' Version     : V2.5.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Présente, en lecture seule et mise en forme, la synthèse d'un domaine-convention : organisme,
' comportements, tarifs en vigueur, natures de séance, règle de quota applicable et points
' d'attention (complétude).
'
' Utilisation :
' - Onglet « Synthèse » de UC_DomainesHome (avec contrôle de complétude).
' - Onglet « Fiche Dossier » de UC_DossierFiche, pour choisir un domaine en connaissance de cause.
'
' Présentation (V2.0.0, UI-5 — maquette « Fiche dossier » B) :
' - En-tête : libellé du domaine, pastille du code, pastille de l'organisme.
' - Corps : un bloc par rubrique. Disposition en trois colonnes séparées par un filet lorsque la
'   largeur le permet (fiche dossier), en blocs empilés défilants sinon (écran Domaines).
' - Remplace la V1 (RichTextBox mise en forme), dont le rendu texte ne s'accordait pas au kit.
'
' Remarques   :
' - Aucun accès direct à la base : tout passe par les modules Gestion*.
' - Les points d'attention sont les règles de complétude du métier (GestionDomaines.CalculerManques).
' - Logo du domaine (V2.4.0) : sous les règles, dans l'espace libre de la première colonne (ou
'   sous le bloc Règles quand les blocs sont empilés) ; miniature partagée de LogosDomaines.
' - Exception admise à « Designer obligatoire » (Rules § 9) : les blocs sont construits en code car
'   leur contenu dépend des données (nombre de tarifs, de natures…) ; couleurs et polices partagées.
' - V2.1.0 (14/10/2026, UI-6) : chaque manque est rattaché à son étape (ManqueDomaine) pour que
'   l'écran Domaines colore ses étapes ; les messages parlent d'« étape » et non plus d'« onglet ».
' - V2.2.0 (14/10/2026) : une période sans règle entre deux règles de quota est un manque.
' - V2.3.0 (14/10/2026) : en mode colonnes, le quota quitte la première colonne pour une bande pleine
'   largeur sous les trois colonnes, champs côte à côte (synthèse lisible d'un coup d'œil).
' - V2.4.0 (20/10/2026, v2.45.0) : logo du domaine sous les règles (demande de Joëlle), dans un
'   cadre de 180 × 80 (logos en longueur lisibles).
' - V2.5.0 (23/10/2026, audit C.2 étape 3) : les règles de complétude passent dans le métier
'   (GestionDomaines.CalculerManques, ManqueDomaine et EtapeDomaine dans Metier/Referentiels) ; la
'   synthèse ne fait plus que les afficher. Tarifs en vigueur : LiaisonPrixDomaine.EstEnVigueur.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_SyntheseDomaine

#Region "Constantes de présentation"

    ' En dessous de cette largeur utile, les blocs sont empilés au lieu d'être disposés en colonnes.
    Private Const LARGEUR_MIN_COLONNES As Integer = 780

    ' Espace de part et d'autre du filet séparant deux colonnes.
    Private Const DEMI_GOUTTIERE As Integer = 20

    ' Espace vertical entre deux blocs empilés.
    Private Const ECART_BLOCS As Integer = 16

    ' Cadre du logo du domaine, posé sous les règles (pixels) : large, les logos sont souvent en longueur.
    Private Const LARGEUR_LOGO As Integer = 180
    Private Const HAUTEUR_LOGO As Integer = 80

    ' Poids relatifs des colonnes Règles / Tarifs / Natures (maquette B).
    Private Shared ReadOnly PoidsColonnes As Single() = {1.0F, 1.15F, 1.0F}

    ' Rôles de couleur, tous tirés de la charte (UITheme).
    Private Shared ReadOnly CouleurSucces As Color = UITheme.StatutRegleFore
    Private Shared ReadOnly CouleurAlerte As Color = UITheme.ColorRougeMoyenClair
    Private Shared ReadOnly CouleurDiscret As Color = UITheme.ColorGrisDiscret

#End Region

#Region "Variables privées"

    ' Blocs affichés, dans l'ordre de lecture ; le placement en colonnes s'appuie sur leurs rôles.
    Private ReadOnly _blocs As New List(Of BlocSynthese)
    Private _blocRegles As BlocSynthese
    Private _blocTarifs As BlocSynthese
    Private _blocNatures As BlocSynthese
    Private _blocQuota As BlocSynthese
    Private _blocCompletude As BlocSynthese

    ' Abscisses des filets verticaux (mode colonnes) et leur hauteur, dessinés par pnlCorps_Paint.
    Private ReadOnly _filets As New List(Of Integer)
    Private _hauteurFilets As Integer

    Private _dispositionEnCours As Boolean

    ' Logo du domaine (miniature partagée de LogosDomaines : jamais libérée ici).
    Private ReadOnly _picLogo As New PictureBox With {
        .Name = "picLogo",
        .SizeMode = PictureBoxSizeMode.Zoom,
        .Size = New Size(LARGEUR_LOGO, HAUTEUR_LOGO),
        .BackColor = Color.Transparent,
        .TabStop = False,
        .Visible = False
    }

#End Region

#Region "API publique"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherDomaine
    ' Version   : V2.1.0
    ' Date      : 20/10/2026
    '
    ' Rôle      : Affiche la synthèse du domaine fourni (ou un message d'invite si Nothing).
    '
    ' Paramètres :
    ' - dom                : Domaine à présenter
    ' - inclureCompletude  : Ajoute le bloc « Points d'attention » (paramétrage incomplet)
    ' - V2.1.0 (20/10/2026) : logo du domaine (AfficherLogo).
    ' -------------------------------------------------------------------------------------------------
    Public Sub AfficherDomaine(dom As Domaine, Optional inclureCompletude As Boolean = True)

        pnlCorps.SuspendLayout()

        Try
            ViderBlocs()
            RemplirEntete(dom)
            AfficherLogo(dom)

            If dom Is Nothing Then
                AjouterBloc(Nothing).AjouterMessage("Sélectionnez un domaine pour afficher sa convention.",
                                                    CouleurDiscret, FontStyle.Italic)
                Return
            End If

            Try
                Dim tarifs As List(Of LiaisonPrixDomaine) = GestionLiaisonsPrixDomaines.GetLiaisonsParDomaine(dom.IdDomaine)
                Dim natures As List(Of LiaisonNatureDomaine) = GestionLiaisonsNaturesDomaines.GetLiaisonsParDomaine(dom.IdDomaine)
                Dim quotas As List(Of QuotaInamiRegle) = GestionQuotasInami.GetQuotasParDomaine(dom.IdDomaine)

                _blocRegles = ConstruireRegles(dom)
                _blocTarifs = ConstruireTarifs(dom, tarifs)
                _blocNatures = ConstruireNatures(natures)
                _blocQuota = ConstruireQuota(dom, quotas)
                If inclureCompletude Then _blocCompletude = ConstruireCompletude(dom, tarifs, natures, quotas)

            Catch ex As Exception
                GestionLog.EcrireLog($"UC_SyntheseDomaine : erreur synthèse domaine {dom.IdDomaine}.",
                                     GestionLog.LogLevel.Succinct, GestionLog.LogCategory.UI, ex)
                ViderBlocs()
                AjouterBloc(Nothing).AjouterMessage("Impossible de charger la synthèse (voir le journal).",
                                                    CouleurAlerte, FontStyle.Italic)
            End Try

        Finally
            pnlCorps.ResumeLayout(False)
            DisposerBlocs()
        End Try

    End Sub

#End Region

#Region "En-tête"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : RemplirEntete
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      : Libellé du domaine suivi de la pastille du code ; organisme en pastille à droite.
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemplirEntete(dom As Domaine)

        If dom Is Nothing Then
            lblTitre.Text = "Aucun domaine sélectionné"
            pstCode.Visible = False
            pstOrganisme.Visible = False
            Return
        End If

        lblTitre.Text = dom.LibelleDomaine
        pstCode.Text = dom.CodeDomaine
        pstCode.Visible = Not String.IsNullOrWhiteSpace(dom.CodeDomaine)
        pstOrganisme.Text = If(String.IsNullOrWhiteSpace(dom.LibelleOrganisme),
                               "Sans organisme de remboursement",
                               "Organisme : " & dom.LibelleOrganisme)
        pstOrganisme.Visible = True

        PlacerEntete()

    End Sub

    ' Logo sous les règles (DisposerBlocs) ; masqué si le domaine n'en a pas.
    Private Sub AfficherLogo(dom As Domaine)

        If _picLogo.Parent Is Nothing Then pnlCorps.Controls.Add(_picLogo)

        _picLogo.Image = If(dom Is Nothing, Nothing, LogosDomaines.Miniature(dom.IdDomaine, dom.LogoFichier, LARGEUR_LOGO, HAUTEUR_LOGO))
        _picLogo.Visible = _picLogo.Image IsNot Nothing
        If _picLogo.Image IsNot Nothing Then _picLogo.Size = _picLogo.Image.Size

    End Sub

    Private Sub PlacerEntete()

        pstCode.Location = New Point(lblTitre.Right + 8, lblTitre.Top + (lblTitre.Height - pstCode.Height) \ 2)
        pstOrganisme.Location = New Point(Math.Max(pstCode.Right + 12, pnlEntete.ClientSize.Width - pstOrganisme.Width),
                                          pstCode.Top)

    End Sub

    Private Sub pnlEntete_Resize(sender As Object, e As EventArgs) Handles pnlEntete.Resize
        PlacerEntete()
    End Sub

    ' Filet sous l'en-tête, comme sous le titre des cartes du kit.
    Private Sub pnlEntete_Paint(sender As Object, e As PaintEventArgs) Handles pnlEntete.Paint

        Dim y As Integer = pnlEntete.ClientSize.Height - 6
        Using stylo As New Pen(UITheme.ColorBeige)
            e.Graphics.DrawLine(stylo, 0, y, pnlEntete.ClientSize.Width, y)
        End Using

    End Sub

#End Region

#Region "Construction des blocs"

    Private Function ConstruireRegles(dom As Domaine) As BlocSynthese

        Dim bloc As BlocSynthese = AjouterBloc("Règles")
        bloc.AjouterChamp("Tarif", If(dom.TarifConvention, "fixé par la convention du domaine", "tarif patient du dossier"))
        bloc.AjouterChamp("Absence non excusée", If(dom.NoShowFacturable, "facturée au patient", "non facturée"))
        bloc.AjouterChamp("Quota", If(dom.SoumisQuota, "soumis à un quota de séances", "aucun quota"),
                          If(dom.SoumisQuota, UITheme.ColorGrisFonce, CouleurDiscret))

        If Not dom.Actif Then
            bloc.AjouterMessage("Domaine inactif : il n'est plus proposé à la création de dossiers.",
                                CouleurAlerte, FontStyle.Italic)
        End If

        Return bloc

    End Function

    Private Function ConstruireTarifs(dom As Domaine, tarifs As List(Of LiaisonPrixDomaine)) As BlocSynthese

        Dim bloc As BlocSynthese = AjouterBloc("Tarifs en vigueur")
        Dim enVigueur As List(Of LiaisonPrixDomaine) = tarifs.Where(Function(l) l.EstEnVigueur).ToList()

        If enVigueur.Count = 0 Then
            bloc.AjouterMessage(If(dom.TarifConvention, "Aucun tarif en vigueur aujourd'hui.",
                                   "Aucun : le tarif patient du dossier s'applique."),
                                CouleurDiscret, FontStyle.Italic)
            Return bloc
        End If

        For Each t In enVigueur
            bloc.AjouterTarif(t.MontantAffichage, t.LibellePrixSeance, t.LibellePeriodeValidite)
        Next

        Return bloc

    End Function

    Private Function ConstruireNatures(natures As List(Of LiaisonNatureDomaine)) As BlocSynthese

        Dim bloc As BlocSynthese = AjouterBloc("Natures de séance")
        Dim actives As List(Of LiaisonNatureDomaine) = natures.Where(Function(l) l.Actif).ToList()

        If actives.Count = 0 Then
            bloc.AjouterMessage("Aucune nature rattachée.", CouleurDiscret, FontStyle.Italic)
            Return bloc
        End If

        For Each n In actives
            bloc.AjouterPastille(n.LibelleNatureSeance & " · " &
                                 If(n.ConsommeQuota, "quota " & n.LibelleCategorieQuota, "hors quota"))
        Next

        Return bloc

    End Function

    Private Function ConstruireQuota(dom As Domaine, quotas As List(Of QuotaInamiRegle)) As BlocSynthese

        If Not dom.SoumisQuota Then Return Nothing

        Dim bloc As BlocSynthese = AjouterBloc("Quota applicable")
        Dim regle As QuotaInamiRegle = quotas.FirstOrDefault(Function(r) r.EstEnVigueur)

        If regle Is Nothing Then
            bloc.AjouterMessage("Aucune règle en vigueur aujourd'hui.", CouleurAlerte, FontStyle.Italic)
            Return bloc
        End If

        bloc.AjouterChamp("Plafonds", regle.LibellePlafonds)
        bloc.AjouterChamp("Période", $"{regle.DureePeriodeMois} mois glissants")
        bloc.AjouterChamp("Alerte à", $"{regle.SeuilAlerteTotal} séances")
        bloc.AjouterChamp("Validité", regle.LibellePeriodeValidite)

        Return bloc

    End Function

    Private Function ConstruireCompletude(dom As Domaine,
                                          tarifs As List(Of LiaisonPrixDomaine),
                                          natures As List(Of LiaisonNatureDomaine),
                                          quotas As List(Of QuotaInamiRegle)) As BlocSynthese

        Dim bloc As BlocSynthese = AjouterBloc("Points d'attention")
        Dim manques As List(Of String) = GestionDomaines.CalculerManques(dom, tarifs, natures, quotas).Select(Function(m) m.Message).ToList()

        If manques.Count = 0 Then
            bloc.AjouterMessage("Domaine complet : il peut être utilisé pour ouvrir des dossiers.",
                                CouleurSucces, FontStyle.Bold)
            Return bloc
        End If

        For Each m In manques
            bloc.AjouterMessage(m, CouleurAlerte, FontStyle.Regular)
        Next

        Return bloc

    End Function

    Private Function AjouterBloc(titre As String) As BlocSynthese

        Dim bloc As New BlocSynthese(titre)
        _blocs.Add(bloc)
        pnlCorps.Controls.Add(bloc)
        Return bloc

    End Function

    Private Sub ViderBlocs()

        For Each bloc In _blocs
            pnlCorps.Controls.Remove(bloc)
            bloc.Dispose()
        Next

        _blocs.Clear()
        _blocRegles = Nothing
        _blocTarifs = Nothing
        _blocNatures = Nothing
        _blocQuota = Nothing
        _blocCompletude = Nothing

    End Sub

#End Region

#Region "Disposition"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DisposerBlocs
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Place les blocs selon la largeur disponible : trois colonnes (Règles / Tarifs / Natures)
    ' séparées par un filet, puis en pleine largeur le quota (champs côte à côte) et la complétude ;
    ' sinon empilés.
    '
    ' Remarques :
    ' - La hauteur de chaque bloc dépend de sa largeur (retour à la ligne) : tout est recalculé à
    '   chaque redimensionnement.
    ' - Le garde _dispositionEnCours évite la boucle : modifier AutoScrollMinSize peut faire
    '   apparaître la barre de défilement, donc changer la largeur, donc relancer la disposition.
    ' -------------------------------------------------------------------------------------------------
    Private Sub DisposerBlocs()

        If _dispositionEnCours Then Return
        _dispositionEnCours = True

        Try
            _filets.Clear()
            _hauteurFilets = 0

            Dim largeur As Integer = pnlCorps.ClientSize.Width
            Dim origine As Point = pnlCorps.AutoScrollPosition
            Dim enColonnes As Boolean = largeur >= LARGEUR_MIN_COLONNES AndAlso
                                        _blocRegles IsNot Nothing AndAlso _blocTarifs IsNot Nothing AndAlso
                                        _blocNatures IsNot Nothing

            Dim hauteurTotale As Integer = If(enColonnes,
                                              DisposerEnColonnes(largeur, origine),
                                              DisposerEmpiles(largeur, origine))

            pnlCorps.AutoScrollMinSize = New Size(0, hauteurTotale)
            pnlCorps.Invalidate()

        Finally
            _dispositionEnCours = False
        End Try

    End Sub

    Private Function DisposerEnColonnes(largeur As Integer, origine As Point) As Integer

        Dim colonnes As BlocSynthese()() = {
            New BlocSynthese() {_blocRegles},
            New BlocSynthese() {_blocTarifs},
            New BlocSynthese() {_blocNatures}
        }

        Dim utile As Integer = largeur - 4 * DEMI_GOUTTIERE
        Dim totalPoids As Single = PoidsColonnes.Sum()
        Dim x As Integer = 0
        Dim hauteurColonnes As Integer = 0

        For i As Integer = 0 To colonnes.Length - 1

            Dim largeurColonne As Integer = CInt(utile * PoidsColonnes(i) / totalPoids)
            Dim y As Integer = 0

            For Each bloc In colonnes(i)
                If bloc Is Nothing Then Continue For
                Dim h As Integer = bloc.Disposer(largeurColonne)
                bloc.Location = New Point(x + origine.X, y + origine.Y)
                y += h + ECART_BLOCS
            Next

            ' Le logo occupe l'espace libre sous les règles (première colonne).
            If i = 0 AndAlso _picLogo.Visible Then
                _picLogo.Location = New Point(x + origine.X, y + origine.Y)
                y += _picLogo.Height + ECART_BLOCS
            End If

            hauteurColonnes = Math.Max(hauteurColonnes, y - ECART_BLOCS)
            x += largeurColonne

            If i < colonnes.Length - 1 Then
                _filets.Add(x + DEMI_GOUTTIERE)
                x += 2 * DEMI_GOUTTIERE
            End If

        Next

        _hauteurFilets = hauteurColonnes

        ' Sous les colonnes, en pleine largeur : le quota (ses champs côte à côte, pour ne pas allonger
        ' la première colonne), puis les points d'attention.
        Dim yBas As Integer = hauteurColonnes

        If _blocQuota IsNot Nothing Then
            yBas += ECART_BLOCS
            Dim hQuota As Integer = _blocQuota.Disposer(largeur, champsEnLigne:=True)
            _blocQuota.Location = New Point(origine.X, yBas + origine.Y)
            yBas += hQuota
        End If

        If _blocCompletude IsNot Nothing Then
            yBas += ECART_BLOCS
            Dim hCompletude As Integer = _blocCompletude.Disposer(largeur)
            _blocCompletude.Location = New Point(origine.X, yBas + origine.Y)
            yBas += hCompletude
        End If

        Return yBas

    End Function

    Private Function DisposerEmpiles(largeur As Integer, origine As Point) As Integer

        Dim y As Integer = 0

        For Each bloc In _blocs
            Dim h As Integer = bloc.Disposer(largeur)
            bloc.Location = New Point(origine.X, y + origine.Y)
            y += h + ECART_BLOCS

            If bloc Is _blocRegles AndAlso _picLogo.Visible Then
                _picLogo.Location = New Point(origine.X, y + origine.Y)
                y += _picLogo.Height + ECART_BLOCS
            End If
        Next

        Return Math.Max(0, y - ECART_BLOCS)

    End Function

    Private Sub pnlCorps_Resize(sender As Object, e As EventArgs) Handles pnlCorps.Resize
        DisposerBlocs()
    End Sub

    Private Sub pnlCorps_Paint(sender As Object, e As PaintEventArgs) Handles pnlCorps.Paint

        If _filets.Count = 0 OrElse _hauteurFilets <= 0 Then Return

        Dim dy As Integer = pnlCorps.AutoScrollPosition.Y
        Using stylo As New Pen(UITheme.ColorBeige)
            For Each x In _filets
                e.Graphics.DrawLine(stylo, x, dy, x, dy + _hauteurFilets)
            Next
        End Using

    End Sub

#End Region

#Region "Bloc de synthèse"

    ' -------------------------------------------------------------------------------------------------
    ' Classe    : BlocSynthese
    ' Version   : V1.0.0
    ' Date      : 11/10/2026
    '
    ' Rôle      :
    ' Une rubrique de la synthèse : petit titre en capitales puis des éléments (champ libellé/valeur,
    ' ligne de tarif, pastille, message), placés par Disposer selon la largeur reçue.
    '
    ' Remarques :
    ' - Disposition manuelle plutôt que FlowLayoutPanel/TableLayoutPanel imbriqués : le retour à la
    '   ligne des textes et des pastilles y est imprévisible, alors que la mesure directe est fiable.
    ' -------------------------------------------------------------------------------------------------
    Private NotInheritable Class BlocSynthese
        Inherits Panel

        Private Enum GenreElement
            Legende
            Texte
            Tarif
            Pastille
        End Enum

        Private Const LARGEUR_MONTANT As Integer = 72
        Private Const ECART_MONTANT As Integer = 10

        Private Shared ReadOnly PoliceTitre As New Font("Calibri", 9.75F, FontStyle.Bold)
        Private Shared ReadOnly PoliceLegende As New Font("Calibri", 9.75F, FontStyle.Bold)
        Private Shared ReadOnly PoliceTexte As New Font("Calibri", 11.0F)
        Private Shared ReadOnly PoliceDetail As New Font("Calibri", 9.75F)
        Private Shared ReadOnly PolicePastille As New Font("Calibri", 10.5F)
        Private Shared ReadOnly PoliceTexteGras As New Font(PoliceTexte, FontStyle.Bold)
        Private Shared ReadOnly PoliceTexteItalique As New Font(PoliceTexte, FontStyle.Italic)

        Private Const FLAGS_MESURE As TextFormatFlags =
            TextFormatFlags.WordBreak Or TextFormatFlags.TextBoxControl Or TextFormatFlags.NoPrefix

        Private ReadOnly _titre As Label
        Private ReadOnly _elements As New List(Of (Genre As GenreElement, Principal As Control, Secondaire As Control))

        Public Sub New(titre As String)

            DoubleBuffered = True
            BackColor = Color.Transparent
            Margin = Padding.Empty

            If Not String.IsNullOrEmpty(titre) Then
                _titre = New Label() With {
                    .AutoSize = True,
                    .BackColor = Color.Transparent,
                    .Font = PoliceTitre,
                    .ForeColor = UITheme.ColorMauveFonce,
                    .Text = titre.ToUpperInvariant(),
                    .UseMnemonic = False
                }
                Controls.Add(_titre)
            End If

        End Sub

        Public Sub AjouterChamp(libelle As String, valeur As String, Optional couleurValeur As Color = Nothing)
            Ajouter(GenreElement.Legende, CreerLabel(libelle, PoliceLegende, UITheme.ColorSaugeFonce), Nothing)
            Ajouter(GenreElement.Texte,
                    CreerLabel(valeur, PoliceTexte, If(couleurValeur.IsEmpty, UITheme.ColorGrisFonce, couleurValeur)),
                    Nothing)
        End Sub

        Public Sub AjouterTarif(montant As String, libelle As String, detail As String)
            Dim lblMontant As Label = CreerLabel(montant, PoliceTexteGras, UITheme.ColorSaugeTresFonce)
            lblMontant.TextAlign = ContentAlignment.TopRight
            Dim lblLibelle As Label = CreerLabel(libelle, PoliceTexte, UITheme.ColorGrisFonce)
            Ajouter(GenreElement.Tarif, lblMontant, lblLibelle)
            If Not String.IsNullOrWhiteSpace(detail) Then
                Dim lblDetail As Label = CreerLabel(detail, PoliceDetail, CouleurDiscret)
                lblDetail.Tag = LARGEUR_MONTANT + ECART_MONTANT
                Ajouter(GenreElement.Texte, lblDetail, Nothing)
            End If
        End Sub

        Public Sub AjouterPastille(texte As String)
            Dim pastille As New AltheaPastille() With {
                .Font = PolicePastille,
                .ForeColor = UITheme.ColorGrisFonce,
                .CouleurFond = UITheme.ColorBlanc,
                .CouleurBordure = UITheme.ColorBeige,
                .Padding = New Padding(10, 3, 10, 3),
                .Text = texte
            }
            Ajouter(GenreElement.Pastille, pastille, Nothing)
        End Sub

        Public Sub AjouterMessage(texte As String, couleur As Color, style As FontStyle)
            Ajouter(GenreElement.Texte, CreerLabel(texte, PoliceMessage(style), couleur), Nothing)
        End Sub

        ' Polices partagées (jamais recréées à chaque affichage : un Label ne libère pas sa police).
        Private Shared Function PoliceMessage(style As FontStyle) As Font
            If style.HasFlag(FontStyle.Bold) Then Return PoliceTexteGras
            If style.HasFlag(FontStyle.Italic) Then Return PoliceTexteItalique
            Return PoliceTexte
        End Function

        ' Place le titre et les éléments sur la largeur donnée ; renvoie (et applique) la hauteur obtenue.
        ' champsEnLigne : si le bloc ne contient que des champs (libellé + valeur), ils sont placés
        ' côte à côte au lieu d'être empilés.
        Public Function Disposer(largeur As Integer, Optional champsEnLigne As Boolean = False) As Integer

            Dim y As Integer = 0
            If _titre IsNot Nothing Then
                _titre.Location = Point.Empty
                y = _titre.Height + 6
            End If

            If champsEnLigne AndAlso NeContientQueDesChamps() Then Return DisposerChampsEnLigne(largeur, y)

            Dim xPastille As Integer = 0
            Dim hLignePastilles As Integer = 0

            For Each element In _elements

                If element.Genre <> GenreElement.Pastille AndAlso hLignePastilles > 0 Then
                    y += hLignePastilles + 6
                    xPastille = 0
                    hLignePastilles = 0
                End If

                Select Case element.Genre

                    Case GenreElement.Legende
                        y += PlacerTexte(element.Principal, 0, y, largeur) + 1

                    Case GenreElement.Texte
                        Dim retrait As Integer = If(TypeOf element.Principal.Tag Is Integer, CInt(element.Principal.Tag), 0)
                        y += PlacerTexte(element.Principal, retrait, y, largeur - retrait) + 6

                    Case GenreElement.Tarif
                        Dim hMontant As Integer = PlacerTexte(element.Principal, 0, y, LARGEUR_MONTANT)
                        Dim xLibelle As Integer = LARGEUR_MONTANT + ECART_MONTANT
                        Dim hLibelle As Integer = PlacerTexte(element.Secondaire, xLibelle, y, largeur - xLibelle)
                        y += Math.Max(hMontant, hLibelle)

                    Case GenreElement.Pastille
                        Dim taille As Size = element.Principal.GetPreferredSize(Size.Empty)
                        Dim w As Integer = Math.Min(taille.Width, largeur)
                        If xPastille > 0 AndAlso xPastille + w > largeur Then
                            y += hLignePastilles + 6
                            xPastille = 0
                            hLignePastilles = 0
                        End If
                        element.Principal.AutoSize = False
                        element.Principal.SetBounds(xPastille, y, w, taille.Height)
                        xPastille += w + 6
                        hLignePastilles = Math.Max(hLignePastilles, taille.Height)

                End Select

            Next

            If hLignePastilles > 0 Then y += hLignePastilles + 6

            ' Le dernier élément ajoute toujours un petit écart : on le retire pour un bas net.
            Dim hauteur As Integer = Math.Max(0, y - 4)
            Size = New Size(largeur, hauteur)
            Return hauteur

        End Function

        ' Vrai si les éléments sont uniquement des paires libellé / valeur (AjouterChamp).
        Private Function NeContientQueDesChamps() As Boolean

            If _elements.Count < 4 OrElse _elements.Count Mod 2 <> 0 Then Return False

            For i As Integer = 0 To _elements.Count - 1 Step 2
                If _elements(i).Genre <> GenreElement.Legende OrElse _elements(i + 1).Genre <> GenreElement.Texte Then Return False
            Next

            Return True

        End Function

        ' -------------------------------------------------------------------------------------------------
        ' Fonction  : DisposerChampsEnLigne
        ' Rôle      : Champs côte à côte, chacun dans sa colonne ; le premier (le plus long : les
        '             plafonds du quota) reçoit une double largeur. Renvoie la hauteur du bloc.
        ' -------------------------------------------------------------------------------------------------
        Private Function DisposerChampsEnLigne(largeur As Integer, yDebut As Integer) As Integer

            Const ECART_CHAMPS As Integer = 24

            Dim nombre As Integer = _elements.Count \ 2
            Dim parts As Integer = nombre + 1
            Dim unite As Single = CSng(largeur - (nombre - 1) * ECART_CHAMPS) / parts
            Dim x As Integer = 0
            Dim hauteurMax As Integer = 0

            For i As Integer = 0 To nombre - 1

                Dim largeurChamp As Integer = CInt(If(i = 0, 2 * unite, unite))
                Dim hLegende As Integer = PlacerTexte(_elements(2 * i).Principal, x, yDebut, largeurChamp)
                Dim hValeur As Integer = PlacerTexte(_elements(2 * i + 1).Principal, x, yDebut + hLegende + 1, largeurChamp)
                hauteurMax = Math.Max(hauteurMax, hLegende + 1 + hValeur)
                x += largeurChamp + ECART_CHAMPS

            Next

            Dim hauteur As Integer = yDebut + hauteurMax
            Size = New Size(largeur, hauteur)
            Return hauteur

        End Function

        Private Shared Function PlacerTexte(lbl As Control, x As Integer, y As Integer, largeur As Integer) As Integer

            Dim l As Integer = Math.Max(1, largeur)
            Dim h As Integer = TextRenderer.MeasureText(lbl.Text, lbl.Font, New Size(l, Integer.MaxValue), FLAGS_MESURE).Height
            lbl.SetBounds(x, y, l, h)
            Return h

        End Function

        Private Shared Function CreerLabel(texte As String, police As Font, couleur As Color) As Label

            Return New Label() With {
                .AutoSize = False,
                .BackColor = Color.Transparent,
                .Font = police,
                .ForeColor = couleur,
                .Text = If(texte, String.Empty),
                .UseMnemonic = False
            }

        End Function

        Private Sub Ajouter(genre As GenreElement, principal As Control, secondaire As Control)

            Controls.Add(principal)
            If secondaire IsNot Nothing Then Controls.Add(secondaire)
            _elements.Add((genre, principal, secondaire))

        End Sub

    End Class

#End Region

End Class
