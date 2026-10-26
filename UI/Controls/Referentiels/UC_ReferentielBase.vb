' -------------------------------------------------------------------------------------------------
' UserControl : UC_ReferentielBase
' Projet      : Althéa
' Version     : V1.8.0
' Date        : 22/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Classe de base héritable pour tous les écrans de gestion de référentiels (tables ref_*).
' Centralise la mécanique commune : chargement, recherche, modes (consultation / création /
' modification), validation, activation/désactivation (soft-delete), droits et journalisation.
'
' Responsabilités :
' - Afficher la liste des éléments du référentiel dans une grille générique
' - Gérer la recherche et l'affichage des éléments inactifs
' - Gérer le panneau d'édition générique (Code, Libellé, Ordre, Actif)
' - Héberger, pour les référentiels qui en ont besoin, une zone d'onglets de données liées
'   affichée sous la liste et synchronisée avec l'élément sélectionné
' - Orchestrer les modes (consultation, création, modification) et l'état des boutons
' - Valider les saisies (champs requis, longueurs, unicité du code et du libellé)
' - Appliquer les droits d'accès selon le rôle de l'utilisateur courant
' - Journaliser les actions via GestionLog
'
' Remarques   :
' - Implémente IContextAwareUserControl (injection du contexte UI par Home/NavigationManager)
' - Ne contient AUCUN accès direct à la base de données : tout passe par les points
'   d'extension Overridable surchargés par les classes dérivées (ex. UC_Pays).
' - N'est pas MustInherit afin de rester compatible avec le concepteur Windows Forms.
' - V1.4.0 (16/10/2026, UI-7) : panneau « Détail » à la règle des fenêtres — libellé à gauche,
'   champ à droite, une ligne tous les 34 px ; les champs propres à un référentiel se posent par
'   AjouterLigneDetail (plus de position ni de couleur calculée dans les écrans dérivés) ; l'état
'   (case Actif) est toujours la dernière ligne ; consultation lisible (DefinirLectureSeule) au
'   lieu des champs grisés. Correctif : le remplissage de la grille ne recharge plus 3 ou 4 fois
'   le détail et les données liées, et un changement de sélection sur la même ligne ne recharge
'   plus rien (clignotement de Tarifs et Natures au chargement : boucle entre les deux grilles).
' - V1.5.0 (18/10/2026, UI-9.4, maquette « Grilles — états lisibles ») : colonne « État » en pastille
'   (« Système » en bleu, « Inactif » en gris ; rien pour une ligne normale — ReferentielLigne.Etat)
'   à la place de la case « Actif » et du cadenas ; ligne inactive atténuée ; Ordre à droite. Le tri
'   par en-tête vient de la pagination (UC_Pagination).
' - V1.6.0 (19/10/2026) : modifications non enregistrées — quitter l'écran (Retour, menu de Home,
'   fermeture d'Althéa : IUnsavedChangesAware) propose Enregistrer / Quitter sans enregistrer / Rester,
'   Annuler demande « Abandonner / Continuer la saisie » (GardeModifications ; ADR-43).
' - V1.7.0 (19/10/2026) : CreationPermise (référentiel sans création, ex. usages des textes types :
'   Nouveau masqué, les autres boutons se resserrent).
' - V1.8.0 (22/10/2026, audit C.1, étape 3) : RoleMinimum = ReglesReferentiels.ROLE_ECRITURE (plus
'   surchargeable, les 20 surcharges identiques retirées) ; refus du métier (ligne système, droit)
'   montrés tels quels ; messages en DialogChoix (MessageBox retirés).
'
' Dépendances :
' - UserControlContext (contexte UI partagé)
' - IContextAwareUserControl (contrat d'injection)
' - ReferentielLigne (modèle de présentation générique)
' - AppRole (gestion des droits)
' - UtilsButtons / UtilsDataGrid (style UI commun), UtilsPastilles (couleurs de l'état)
' - AltheaOnglets (zone d'onglets des données liées)
' - GestionLog (journalisation)
'
' Interface   :
' - IContextAwareUserControl : injection du contexte UI partagé
' - IUnsavedChangesAware : avertit avant de quitter un détail modifié (Home, fenêtre hôte)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Class UC_ReferentielBase
    Implements IContextAwareUserControl, IUnsavedChangesAware

#Region "Variables privées"

    ' Espacement horizontal (px) entre la liste et le panneau « Détail » (~2 cm à 96 dpi)
    Private Const ESPACEMENT_LISTE_DETAIL As Integer = 60

    ' Grille du panneau « Détail » (règle des fenêtres, Reference_UI § 1.3) : libellé à gauche,
    ' champ à droite, première ligne à y = 40 sous le titre de la carte, une ligne tous les 34 px.
    Private Const X_LIBELLE_DETAIL As Integer = 12
    Private Const Y_PREMIERE_LIGNE_DETAIL As Integer = 40
    Private Const PAS_LIGNE_DETAIL As Integer = 34
    Private Const DECALAGE_LIBELLE_DETAIL As Integer = 4

    ' Lignes déjà occupées par les champs génériques du Designer : Code, Libellé, Ordre.
    Private Const LIGNES_GENERIQUES_DETAIL As Integer = 3

    ' Prochaine ligne libre du panneau « Détail » (champs ajoutés par la classe dérivée, puis État).
    Private _prochaineLigneDetail As Integer = LIGNES_GENERIQUES_DETAIL

    ' Vrai pendant que la grille reçoit sa nouvelle liste : elle lève alors plusieurs SelectionChanged
    ' (vidage, première ligne…) ; les traiter rechargeait 3 ou 4 fois le détail et les données liées
    ' (clignotement de Tarifs et Natures). Le détail est affiché une seule fois, après.
    Private _remplissageGrille As Boolean

    ' Élément dont le détail est affiché (Nothing = aucun affichage encore). La grille relève parfois
    ' SelectionChanged sans que la ligne change (retour du focus quand le panneau des données liées
    ' active ou désactive ses boutons) : sans ce repère, chaque signal rechargeait les données liées,
    ' qui relançaient un signal — boucle visible sur Tarifs et Natures (traces du 16/10/2026).
    Private _idDetailAffiche As ULong?

    ' Largeur minimale de la carte de recherche : le champ et la case « Afficher les inactifs »
    ' doivent y tenir même quand la liste est étroite.
    Private Const LARGEUR_MIN_RECHERCHE As Integer = 520

    ' Contexte UI partagé injecté par Home via SetContext()
    Protected _context As UserControlContext

    ' Liste complète des éléments chargés depuis la base (avant filtrage mémoire)
    Private _lignes As List(Of ReferentielLigne)

    ' Mode courant de l'écran (consultation, création, modification)
    Private _mode As ModeReferentiel = ModeReferentiel.Consultation

    ' Identifiant de l'élément en cours d'édition (0 = aucun / création)
    Private _idEnCours As ULong = 0

    ' Photo du panneau Détail en création ou en modification (créée au chargement, ADR-43).
    Private _garde As GardeModifications

    ' Garde anti-réentrance lors du nettoyage automatique du code (txtCode_TextChanged)
    Private _miseAJourCode As Boolean = False

    ' Indique si l'élément en cours de modification est une ligne système (code et activation verrouillés)
    Private _ligneSystemeEnCours As Boolean = False

    ' Conteneur d'onglets des données liées, créé à la demande si HauteurDonneesLiees > 0
    Private _tabDonneesLiees As AltheaOnglets

#End Region

#Region "Propriétés protégées"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : IdEnCours
    ' Rôle      : Identifiant de l'élément en cours d'édition (0 en création).
    ' Remarques : Exposé aux classes dérivées pour leurs contrôles de cohérence avant
    '             enregistrement.
    ' -------------------------------------------------------------------------------------------------
    Protected ReadOnly Property IdEnCours As ULong
        Get
            Return _idEnCours
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SelectionnerElement
    ' Rôle      : Sélectionne dans la grille l'élément d'identifiant donné (et affiche son détail).
    ' Remarques : Destinée aux ouvertures « en contexte » (ex. fiche de la mutualité d'un patient).
    '             Seule la page affichée est parcourue : sans effet si l'élément n'y figure pas.
    ' -------------------------------------------------------------------------------------------------
    Protected Sub SelectionnerElement(id As ULong)
        For Each row As DataGridViewRow In dgvReferentiel.Rows
            Dim ligne = TryCast(row.DataBoundItem, ReferentielLigne)
            If ligne IsNot Nothing AndAlso ligne.IdReferentiel = id Then
                Dim cellule = row.Cells.Cast(Of DataGridViewCell)().FirstOrDefault(Function(c) c.Visible)
                If cellule IsNot Nothing Then dgvReferentiel.CurrentCell = cellule
                Return
            End If
        Next
    End Sub

#End Region

#Region "Événements"

    ' -------------------------------------------------------------------------------------------------
    ' Événement : EnregistrementEffectue
    ' Rôle      : Notifie qu'un enregistrement (création ou modification) vient de réussir.
    '
    ' Remarques :
    ' - Sert principalement à l'usage « en contexte » (hôte modal ReferentielModalHost) pour
    '   proposer un retour automatique vers l'écran appelant (ex. fiche patient).
    ' - L'usage normal (navigation depuis AdminHome) ne s'y abonne pas : aucun impact sur ce flux.
    ' -------------------------------------------------------------------------------------------------
    Public Event EnregistrementEffectue As EventHandler

#End Region

#Region "Points d'extension - Métadonnées (Overridable)"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : TitreReferentiel
    ' Rôle      : Titre affiché dans l'en-tête (ex. "Domaines"). À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable ReadOnly Property TitreReferentiel As String
        Get
            Return "Référentiel"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : SousTitreReferentiel
    ' Rôle      : Sous-titre descriptif affiché sous le titre. À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable ReadOnly Property SousTitreReferentiel As String
        Get
            Return "Consultez et gérez les valeurs de référence."
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : IconeTitre
    ' Rôle      : Icône affichée dans picTitre (Assets/Titre_Ico). À surcharger.
    ' Remarque  : Nothing conserve l'icône « Référentiels » définie dans le Designer (repli).
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable ReadOnly Property IconeTitre As Image
        Get
            Return Nothing
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : CheminContexte
    ' Rôle      : Fil d'Ariane affiché dans l'en-tête Home (ex. "Référentiels > Domaines"). À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable ReadOnly Property CheminContexte As String
        Get
            Return "Référentiels"
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : RoleMinimum
    ' Rôle      : Rôle requis pour modifier le référentiel : celui que le métier exige
    '             (ReglesReferentiels.ROLE_ECRITURE), une seule définition pour tous (audit C.1).
    ' Remarques : Plus surchargeable : les 20 écrans le redéfinissaient à l'identique ; un rôle
    '             différent de celui du métier n'aurait de toute façon pas d'effet.
    ' -------------------------------------------------------------------------------------------------
    Protected ReadOnly Property RoleMinimum As AppRole
        Get
            Return ReglesReferentiels.ROLE_ECRITURE
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : LongueurMaxCode
    ' Rôle      : Longueur maximale autorisée pour le code (varie selon la table ref_*). À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable ReadOnly Property LongueurMaxCode As Integer
        Get
            Return 10
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : RemplacerEspacesParUnderscore
    ' Rôle      : Indique si les espaces saisis dans le code doivent être remplacés par des underscores.
    ' Remarque  : Activé par défaut pour les codes longs (multi-mots). À surcharger (ex. False pour
    '             les codes courts strictement alphanumériques comme les domaines).
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable ReadOnly Property RemplacerEspacesParUnderscore As Boolean
        Get
            Return True
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : CreationPermise
    ' Version   : V1.0.0
    ' Date      : 19/10/2026
    ' Rôle      : False pour un référentiel dont les lignes naissent par migration (le code de
    '             l'application en dépend) : le bouton Nouveau est retiré de la barre.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable ReadOnly Property CreationPermise As Boolean
        Get
            Return True
        End Get
    End Property

#End Region

#Region "Points d'extension - Données (Overridable)"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ChargerElements
    ' Rôle     : Retourne les éléments du référentiel sous forme de lignes génériques.
    ' Paramètre: afficherInactifs - True pour inclure les éléments désactivés.
    ' Remarque : À surcharger par la classe dérivée (appel à sa couche métier).
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Function ChargerElements(afficherInactifs As Boolean) As List(Of ReferentielLigne)
        Return New List(Of ReferentielLigne)()
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : CodeExisteDeja
    ' Rôle     : Indique si un code existe déjà (hors élément exclu). À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Function CodeExisteDeja(code As String, idExclu As ULong) As Boolean
        Return False
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LibelleExisteDeja
    ' Rôle     : Indique si un libellé existe déjà (hors élément exclu). À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Function LibelleExisteDeja(libelle As String, idExclu As ULong) As Boolean
        Return False
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InsererElement
    ' Rôle      : Insère un nouvel élément dans le référentiel. À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub InsererElement(code As String, libelle As String, ordre As Integer, actif As Boolean)
        ' Implémentation fournie par la classe dérivée
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : MettreAJourElement
    ' Rôle      : Met à jour un élément existant du référentiel. À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub MettreAJourElement(id As ULong, code As String, libelle As String, ordre As Integer, actif As Boolean)
        ' Implémentation fournie par la classe dérivée
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : DefinirActivation
    ' Rôle      : Active ou désactive un élément (soft-delete). À surcharger.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub DefinirActivation(id As ULong, actif As Boolean)
        ' Implémentation fournie par la classe dérivée
    End Sub

#End Region

#Region "Points d'extension - Champ supplémentaire (Overridable)"

    ' Position et largeur des champs du panneau « Détail », pour les classes dérivées qui doivent
    ' poser un contrôle à côté d'un champ (ex. une case à droite d'un nombre).
    Protected Const X_CHAMP_DETAIL As Integer = 112
    Protected Const LARGEUR_CHAMP_DETAIL As Integer = 270

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ConfigurerChampSupplementaire
    ' Rôle      : Permet à la classe dérivée d'ajouter ses champs au panneau « Détail » (ex. montant
    '             pour ref_prix_seance). Appelée une fois au chargement.
    ' Remarques : - Chaque champ se pose avec AjouterLigneDetail (libellé + champ sur la ligne
    '               suivante) ; les champs se créent avec CreerZoneTexteDetail, CreerListeDetail,
    '               CreerCaseDetail ou CreerNombreDetail (polices et couleurs du kit).
    '             - La ligne « État » est placée par la base après ces champs.
    '             - Implémentation vide par défaut (aucun champ supplémentaire).
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub ConfigurerChampSupplementaire()
        ' Aucun champ supplémentaire par défaut.
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherChampSupplementaire
    ' Rôle      : Affiche la valeur du champ supplémentaire à partir de la ligne sélectionnée.
    ' Remarque  : Implémentation vide par défaut.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub AfficherChampSupplementaire(ligne As ReferentielLigne)
        ' Aucun champ supplémentaire par défaut.
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ViderChampSupplementaire
    ' Rôle      : Réinitialise le champ supplémentaire (mode création / vidage du détail).
    ' Remarque  : Implémentation vide par défaut.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub ViderChampSupplementaire()
        ' Aucun champ supplémentaire par défaut.
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ActiverChampSupplementaire
    ' Rôle      : Règles propres à un référentiel quand l'écran change de mode (ex. un champ
    '             verrouillé sur une ligne système).
    ' Remarques : La base a déjà basculé tout le panneau « Détail » (champs ajoutés compris) entre
    '             consultation et saisie : seules les exceptions sont à traiter ici, avec
    '             UtilsControls.DefinirChampLectureSeule. Implémentation vide par défaut.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub ActiverChampSupplementaire(enEdition As Boolean)
        ' Aucun champ supplémentaire par défaut.
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ValiderChampSupplementaire
    ' Rôle     : Valide le champ supplémentaire avant enregistrement.
    ' Retour   : True si valide (par défaut, aucun champ à valider).
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Function ValiderChampSupplementaire() As Boolean
        Return True
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : AjouterLigneDetail
    ' Version    : V1.0.0
    ' Date       : 16/10/2026
    '
    ' Rôle       :
    ' Pose un libellé et son champ sur la prochaine ligne libre du panneau « Détail ».
    '
    ' Paramètres :
    ' - libelle : texte du libellé (court : la colonne des libellés fait 100 px)
    ' - champ   : champ à poser (créé de préférence par CreerZoneTexteDetail, CreerListeDetail…)
    ' - lignes  : nombre de lignes occupées (2 pour une zone de texte multiligne, par exemple)
    '
    ' Retour     : le libellé créé (pour une infobulle ou un masquage éventuels).
    ' -------------------------------------------------------------------------------------------------
    Protected Function AjouterLigneDetail(libelle As String, champ As Control, Optional lignes As Integer = 1) As Label

        Dim y As Integer = Y_PREMIERE_LIGNE_DETAIL + _prochaineLigneDetail * PAS_LIGNE_DETAIL

        Dim etiquette As New Label With {
            .AutoSize = True,
            .BackColor = Color.Transparent,
            .Font = lblCode.Font,
            .ForeColor = lblCode.ForeColor,
            .Location = New Point(X_LIBELLE_DETAIL, y + DECALAGE_LIBELLE_DETAIL),
            .Text = libelle,
            .UseMnemonic = False
        }

        ' Une case (22 px) est un peu moins haute qu'une zone de texte (25 px) : centrée sur la ligne.
        champ.Location = New Point(X_CHAMP_DETAIL, y + If(TypeOf champ Is CheckBox, 1, 0))

        pnlEdition.Controls.Add(etiquette)
        pnlEdition.Controls.Add(champ)

        _prochaineLigneDetail += Math.Max(1, lignes)
        Return etiquette

    End Function

    ' Zone de texte du panneau « Détail » (police et couleur des champs du kit).
    Protected Function CreerZoneTexteDetail(nom As String, longueurMax As Integer,
                                            Optional largeur As Integer = LARGEUR_CHAMP_DETAIL) As TextBox
        Return New TextBox With {
            .Font = txtCode.Font,
            .ForeColor = txtCode.ForeColor,
            .MaxLength = longueurMax,
            .Name = nom,
            .Size = New Size(largeur, txtCode.Height)
        }
    End Function

    ' Liste du kit (AltheaListe) du panneau « Détail ».
    Protected Function CreerListeDetail(nom As String, Optional largeur As Integer = LARGEUR_CHAMP_DETAIL) As AltheaListe
        Return New AltheaListe With {
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = txtCode.Font,
            .Name = nom,
            .Size = New Size(largeur, 26)
        }
    End Function

    ' Case du kit (AltheaCase) du panneau « Détail ».
    Protected Function CreerCaseDetail(nom As String, texte As String) As AltheaCase
        Return New AltheaCase With {
            .AutoSize = True,
            .BackColor = Color.Transparent,
            .Font = chkActif.Font,
            .ForeColor = chkActif.ForeColor,
            .Name = nom,
            .Text = texte
        }
    End Function

    ' Nombre du panneau « Détail » (pas encore de champ nombre dans le kit : NumericUpDown).
    Protected Function CreerNombreDetail(nom As String, minimum As Decimal, maximum As Decimal,
                                         Optional decimales As Integer = 0,
                                         Optional largeur As Integer = 90) As NumericUpDown
        Return New NumericUpDown With {
            .DecimalPlaces = decimales,
            .Font = numOrdre.Font,
            .ForeColor = numOrdre.ForeColor,
            .Maximum = maximum,
            .Minimum = minimum,
            .Name = nom,
            .Size = New Size(largeur, numOrdre.Height),
            .TextAlign = HorizontalAlignment.Right,
            .ThousandsSeparator = decimales > 0
        }
    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : PlacerLigneEtat
    ' Rôle       : Place la ligne « État » (case Actif) après les champs ajoutés par la classe dérivée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub PlacerLigneEtat()

        Dim y As Integer = Y_PREMIERE_LIGNE_DETAIL + _prochaineLigneDetail * PAS_LIGNE_DETAIL

        lblActif.Location = New Point(X_LIBELLE_DETAIL, y + DECALAGE_LIBELLE_DETAIL)
        chkActif.Location = New Point(X_CHAMP_DETAIL, y + 1)

    End Sub

#End Region

#Region "Points d'extension - Données liées (Overridable)"

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : HauteurDonneesLiees
    ' Rôle      : Hauteur en pixels de la zone d'onglets des données liées, sous la liste.
    ' Retour    : 0 par défaut, ce qui signifie « aucune donnée liée » (zone non créée).
    ' Remarques : Les référentiels porteurs de liaisons (domaines, tarifs, natures) la surchargent
    '             afin d'administrer leurs données rattachées sans quitter l'écran parent.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable ReadOnly Property HauteurDonneesLiees As Integer
        Get
            Return 0
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Propriété : TabDonneesLiees
    ' Rôle      : Conteneur d'onglets mis à disposition des classes dérivées.
    ' Retour    : Le TabControl, ou Nothing si HauteurDonneesLiees vaut 0.
    ' -------------------------------------------------------------------------------------------------
    Protected ReadOnly Property TabDonneesLiees As TabControl
        Get
            Return _tabDonneesLiees
        End Get
    End Property

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ConfigurerDonneesLiees
    ' Rôle      : Permet à la classe dérivée d'ajouter ses onglets à TabDonneesLiees.
    '             Appelée une fois au chargement, après création du conteneur.
    ' Remarque  : Implémentation vide par défaut.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub ConfigurerDonneesLiees()
        ' Aucune donnée liée par défaut.
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerDonneesLiees
    ' Rôle      : Recharge les onglets pour l'élément sélectionné dans la liste.
    ' Paramètre : ligne - élément sélectionné, ou Nothing si la sélection est vide
    ' Remarque  : Implémentation vide par défaut.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub ChargerDonneesLiees(ligne As ReferentielLigne)
        ' Aucune donnée liée par défaut.
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ActiverDonneesLiees
    ' Rôle      : Neutralise la zone des données liées pendant l'édition de l'élément parent.
    ' Paramètre : enEdition - True si l'écran est en création ou en modification
    ' Remarques : Évite que l'utilisateur modifie des liaisons alors que l'élément parent n'est
    '             pas encore enregistré.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub ActiverDonneesLiees(enEdition As Boolean)

        If _tabDonneesLiees Is Nothing Then Exit Sub

        _tabDonneesLiees.Enabled = Not enEdition

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserDonneesLiees
    ' Rôle      : Crée la zone d'onglets sous la liste si la classe dérivée en déclare une.
    ' Remarques : Le conteneur est inséré entre la grille et le panneau d'édition afin d'occuper
    '             la largeur de la liste sans empiéter sur le panneau de droite.
    '             Onglets du kit (AltheaOnglets), fond transparent comme sur les autres écrans.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserDonneesLiees()

        If HauteurDonneesLiees <= 0 Then Exit Sub

        _tabDonneesLiees = New AltheaOnglets With {
            .Name = "tabDonneesLiees",
            .Dock = DockStyle.Bottom,
            .Height = HauteurDonneesLiees,
            .CouleurFond = Color.Transparent
        }

        pnlCenter.Controls.Add(_tabDonneesLiees)

        ' Le conteneur est placé juste sous la barre de filtres (pnlTop) dans l'ordre d'ancrage :
        ' il est donc docké AVANT le panneau de détail et occupe toute la largeur disponible.
        ' Conséquence voulue : le panneau « Détail », dont la partie basse était vide, se trouve
        ' raccourci d'autant, et la zone de rattachement gagne la largeur nécessaire pour afficher
        ' tous ses boutons sans ascenseur.
        Dim indexTop As Integer = pnlCenter.Controls.GetChildIndex(pnlTop)
        pnlCenter.Controls.SetChildIndex(_tabDonneesLiees, indexTop)

        ConfigurerDonneesLiees()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : PositionnerPanneauDetail
    ' Rôle      : Cale la largeur de la liste sur ses colonnes et accole le panneau « Détail »
    '             juste à droite de la grille, avec un espacement fixe et la même hauteur.
    ' Remarques : Évite le rejet du panneau à l'extrême droite (grand vide central) tout en
    '             laissant libre la zone de droite pour un habillage ultérieur.
    ' -------------------------------------------------------------------------------------------------
    Private Sub PositionnerPanneauDetail()

        If DesignMode Then Exit Sub

        ' Largeur utile de la grille = colonnes visibles + en-tête de ligne + marge ascenseur
        Dim largeurGrille As Integer = dgvReferentiel.RowHeadersWidth + 4
        For Each col As DataGridViewColumn In dgvReferentiel.Columns
            If col.Visible Then largeurGrille += col.Width
        Next
        largeurGrille += SystemInformation.VerticalScrollBarWidth

        ' La grille ne doit jamais empiéter sur le panneau de détail
        Dim largeurMax As Integer = pnlCenter.ClientSize.Width - pnlCenter.Padding.Horizontal _
                                    - pnlEdition.Width - ESPACEMENT_LISTE_DETAIL
        If largeurMax > 100 AndAlso largeurGrille > largeurMax Then largeurGrille = largeurMax
        If largeurGrille < 100 Then Exit Sub

        dgvReferentiel.Width = largeurGrille

        ' Panneau de détail accolé à la grille, aligné en haut et en bas
        pnlEdition.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Bottom
        pnlEdition.Left = dgvReferentiel.Right + ESPACEMENT_LISTE_DETAIL
        pnlEdition.Top = dgvReferentiel.Top
        pnlEdition.Height = dgvReferentiel.Height
        pnlEdition.BringToFront()

        AjusterHauteurPanneauDetail()

        ' La pagination et la carte de recherche restent calées sur la largeur de la liste
        pagReferentiel.Width = largeurGrille + pnlCenter.Padding.Left
        carteRecherche.Width = Math.Max(largeurGrille, LARGEUR_MIN_RECHERCHE)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AjusterHauteurPanneauDetail
    ' Rôle      : Garantit que le contenu du panneau « Détail » reste entièrement visible, même
    '             lorsque la grille est raccourcie par une zone de données liées (ex. Tarifs de séance).
    ' Remarques : Deux leviers successifs, du moins perturbant au plus visible :
    '             1) le panneau déborde sous la grille jusqu'à la limite basse disponible ;
    '             2) si cela ne suffit pas, les champs sont remontés vers le haut du panneau.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AjusterHauteurPanneauDetail()

        If pnlEdition.Controls.Count = 0 Then Exit Sub

        ' Enveloppe verticale réelle des champs du panneau
        Dim hautContenu As Integer = Integer.MaxValue
        Dim basContenu As Integer = 0
        For Each ctl As Control In pnlEdition.Controls
            If Not ctl.Visible Then Continue For
            hautContenu = Math.Min(hautContenu, ctl.Top)
            basContenu = Math.Max(basContenu, ctl.Bottom)
        Next
        If basContenu = 0 Then Exit Sub

        Dim hauteurRequise As Integer = basContenu + pnlEdition.Padding.Bottom
        If hauteurRequise <= pnlEdition.Height Then Exit Sub

        ' Levier 1 : laisser le panneau descendre sous la grille (la zone de droite est libre)
        Dim basDisponible As Integer = pnlCenter.ClientSize.Height - pnlCenter.Padding.Bottom
        If _tabDonneesLiees IsNot Nothing Then
            basDisponible = Math.Min(basDisponible, _tabDonneesLiees.Top - ESPACEMENT_LISTE_DETAIL \ 4)
        End If

        Dim hauteurMax As Integer = basDisponible - pnlEdition.Top
        If hauteurMax > pnlEdition.Height Then
            pnlEdition.Height = Math.Min(hauteurRequise, hauteurMax)
        End If

        If hauteurRequise <= pnlEdition.Height Then Exit Sub

        ' Levier 2 : remonter les champs pour récupérer la marge haute inutilisée
        ' DisplayRectangle tient compte du titre dessiné par la carte : les champs ne remontent pas dessus.
        Dim decalage As Integer = Math.Min(hautContenu - pnlEdition.DisplayRectangle.Top,
                                           hauteurRequise - pnlEdition.Height)
        If decalage <= 0 Then Exit Sub

        For Each ctl As Control In pnlEdition.Controls
            ctl.Top -= decalage
        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : pnlCenter_Resize
    ' Rôle      : Recalcule la position du panneau de détail lors d'un redimensionnement.
    ' -------------------------------------------------------------------------------------------------
    Private Sub pnlCenter_Resize(sender As Object, e As EventArgs) Handles pnlCenter.Resize
        PositionnerPanneauDetail()
    End Sub

#End Region

#Region "Contexte UI"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : SetContext
    ' Rôle      : Reçoit le contexte UI partagé injecté par Home.
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetContext(context As UserControlContext) _
        Implements IContextAwareUserControl.SetContext

        _context = context

    End Sub

#End Region

#Region "Chargement UserControl"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : UC_ReferentielBase_Load
    ' Rôle      : Initialise l'écran, applique les droits et charge la liste.
    ' -------------------------------------------------------------------------------------------------
    Private Sub UC_ReferentielBase_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Évite l'exécution au sein du concepteur Windows Forms
        If DesignMode Then Exit Sub

        _garde = New GardeModifications(pnlEdition)

        InitialiserEntete()
        InitialiserBoutons()
        RetirerCreationSiInterdite()
        InitialiserDataGridView()
        InitialiserToolTips()
        ConfigurerChampSupplementaire()
        PlacerLigneEtat()
        InitialiserDonneesLiees()

        If _context IsNot Nothing Then
            _context.SetHeader(CheminContexte)
        End If

        PasserEnMode(ModeReferentiel.Consultation)
        AppliquerDroitsUtilisateur()
        ChargerListe()
        PositionnerPanneauDetail()

    End Sub

#End Region

#Region "Initialisation UI"

    ' Référentiel sans création : Nouveau retiré, Modifier, Actualiser et Activer se resserrent à
    ' gauche (pas de trou dans la barre).
    Private Sub RetirerCreationSiInterdite()

        If CreationPermise Then Return

        Dim decalage As Integer = btnModifier.Left - btnNouveau.Left
        btnNouveau.Visible = False
        For Each bouton As Control In {CType(btnModifier, Control), btnActualiser, btnActiverDesactiver}
            bouton.Left -= decalage
        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserEntete
    ' Rôle      : Applique le titre et le sous-titre fournis par la classe dérivée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserEntete()
        enTete.Text = TitreReferentiel
        enTete.SousTitre = SousTitreReferentiel

        Dim icone As Image = IconeTitre
        If icone IsNot Nothing Then
            enTete.Icone = icone
        End If
        pnlEdition.Titre = "Détail"
        txtCode.MaxLength = LongueurMaxCode
        txtCode.CharacterCasing = CharacterCasing.Upper
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserBoutons
    ' Rôle      : Initialise les boutons standards (icônes + style Althéa).
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserBoutons()
        UtilsButtons.InitStandardButton(btnNouveau)
        UtilsButtons.InitStandardButton(btnModifier)
        UtilsButtons.InitStandardButton(btnActiverDesactiver)
        UtilsButtons.InitStandardButton(btnActualiser)
        UtilsButtons.InitStandardButton(btnEnregistrer)
        UtilsButtons.InitStandardButton(btnAnnuler)
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserDataGridView
    ' Rôle      : Configure la grille générique et lie les colonnes à ReferentielLigne.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserDataGridView()

        UtilsDataGrid.InitDataGridBasique(dgvReferentiel)

        dgvReferentiel.AutoGenerateColumns = False
        dgvReferentiel.ReadOnly = True
        dgvReferentiel.MultiSelect = False
        dgvReferentiel.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvReferentiel.AllowUserToAddRows = False
        dgvReferentiel.AllowUserToDeleteRows = False

        colId.DataPropertyName = NameOf(ReferentielLigne.IdReferentiel)
        colCode.DataPropertyName = NameOf(ReferentielLigne.Code)
        colLibelle.DataPropertyName = NameOf(ReferentielLigne.Libelle)
        colOrdre.DataPropertyName = NameOf(ReferentielLigne.OrdreAffichage)
        ' Colonne « État » : libellé lié (tri), dessiné en pastille (dgvReferentiel_CellPainting).
        colActif.DataPropertyName = NameOf(ReferentielLigne.LibelleEtat)
        UtilsDataGrid.AlignerDroite(colOrdre)

        ConfigurerColonnesSupplementaires()

        ' Pagination : liste filtrée complète en mémoire, seule la page courante est affichée.
        pagReferentiel.Attacher(dgvReferentiel, "élément(s)")

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ConfigurerColonnesSupplementaires
    ' Rôle      : Point d'extension permettant à un référentiel d'ajouter ses propres colonnes
    '             à la grille principale (ex. le montant des tarifs de séance).
    ' Remarques : Appelée une seule fois, juste après la configuration des colonnes génériques.
    ' -------------------------------------------------------------------------------------------------
    Protected Overridable Sub ConfigurerColonnesSupplementaires()
        ' Aucune colonne supplémentaire par défaut.
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : InitialiserToolTips
    ' Rôle      : Initialise les aides contextuelles des boutons.
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitialiserToolTips()

        If _context Is Nothing Then Exit Sub

        _context.SetToolTip(btnNouveau, "Créer un nouvel élément.")
        _context.SetToolTip(btnModifier, "Modifier l'élément sélectionné.")
        _context.SetToolTip(btnActiverDesactiver, "Activer ou désactiver l'élément sélectionné.")
        _context.SetToolTip(btnActualiser, "Recharger la liste.")
        _context.SetToolTip(btnEnregistrer, "Enregistrer la saisie en cours.")
        _context.SetToolTip(btnAnnuler, "Annuler la saisie en cours.")
        _context.SetToolTip(chkAfficherInactifs, "Afficher également les éléments désactivés.")
        _context.SetToolTip(txtRecherche, "Filtrer sur le code ou le libellé.")
        _context.SetToolTip(txtCode, ConstruireToolTipCode())

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : ConstruireToolTipCode
    ' Rôle     : Construit l'aide contextuelle du champ code selon la longueur et l'option underscore.
    ' -------------------------------------------------------------------------------------------------
    Private Function ConstruireToolTipCode() As String

        If RemplacerEspacesParUnderscore Then
            Return $"Code en majuscules ({LongueurMaxCode} caractères max ; lettres, chiffres ; espaces convertis en _)."
        End If

        Return $"Code en majuscules ({LongueurMaxCode} caractères ; lettres ou chiffres)."

    End Function

#End Region

#Region "Gestion des modes"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : PasserEnMode
    ' Rôle      : Bascule l'écran dans un mode donné et ajuste l'état des contrôles.
    ' -------------------------------------------------------------------------------------------------
    Private Sub PasserEnMode(mode As ModeReferentiel)

        _mode = mode

        Dim enEdition As Boolean = mode <> ModeReferentiel.Consultation

        ' Zone liste / recherche désactivée pendant l'édition
        dgvReferentiel.Enabled = Not enEdition
        txtRecherche.Enabled = Not enEdition
        chkAfficherInactifs.Enabled = Not enEdition

        ' Consultation lisible : tout le panneau (champs ajoutés compris) en lecture seule hors
        ' création / modification, texte normal sur fond teinté.
        UtilsControls.DefinirLectureSeule(pnlEdition, Not enEdition)

        ' Ligne système : le code et l'activation restent verrouillés, même en modification.
        Dim verrouSysteme As Boolean = mode = ModeReferentiel.Modification AndAlso _ligneSystemeEnCours
        If verrouSysteme Then
            UtilsControls.DefinirChampLectureSeule(txtCode, True)
            UtilsControls.DefinirChampLectureSeule(chkActif, True)
        End If

        ActiverChampSupplementaire(enEdition)
        ActiverDonneesLiees(enEdition)

        ' Boutons de gestion (désactivés pendant l'édition)
        btnNouveau.Enabled = Not enEdition
        btnModifier.Enabled = Not enEdition
        btnActiverDesactiver.Enabled = Not enEdition
        btnActualiser.Enabled = Not enEdition

        ' Boutons d'édition (actifs uniquement pendant l'édition)
        btnEnregistrer.Enabled = enEdition
        btnAnnuler.Enabled = enEdition

        ' Titre du panneau d'édition selon le mode
        Select Case mode
            Case ModeReferentiel.Creation
                pnlEdition.Titre = "Nouvel élément"
            Case ModeReferentiel.Modification
                pnlEdition.Titre = "Modification"
            Case Else
                pnlEdition.Titre = "Détail"
        End Select

        ' En consultation, l'état des boutons dépend de la sélection
        If Not enEdition Then
            ActualiserBoutonsSelection()
        End If

        ' Création / modification : photo du détail une fois rempli ; consultation : rien à perdre.
        If enEdition Then
            _garde?.PhotographierApresChargement()
        Else
            _garde?.Oublier()
        End If

        EffacerErreurs()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ActualiserBoutonsSelection
    ' Rôle      : Active/désactive Modifier et Activer/Désactiver selon la ligne sélectionnée.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ActualiserBoutonsSelection()

        Dim ligne As ReferentielLigne = LigneSelectionnee()
        Dim aSelection As Boolean = ligne IsNot Nothing

        btnModifier.Enabled = aSelection
        btnActiverDesactiver.Enabled = aSelection

        If aSelection Then
            btnActiverDesactiver.Text = If(ligne.Actif, "Désactiver", "Réactiver")
            ' Une ligne système active ne peut pas être désactivée
            If ligne.EstSysteme AndAlso ligne.Actif Then btnActiverDesactiver.Enabled = False
        Else
            btnActiverDesactiver.Text = "Désactiver"
        End If

    End Sub

#End Region

#Region "Chargement et filtrage des données"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ChargerListe
    ' Rôle      : Charge les éléments via le point d'extension et applique le filtre courant.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ChargerListe()

        Try
            Cursor = Cursors.WaitCursor

            _lignes = ChargerElements(chkAfficherInactifs.Checked)

            AppliquerFiltre()

            GestionLog.EcrireLog(
                $"Référentiel '{TitreReferentiel}' chargé : {_lignes.Count} élément(s).",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI)

        Catch ex As Exception
            GestionLog.EcrireException($"Erreur de chargement du référentiel '{TitreReferentiel}'.", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.UI)
            If _context IsNot Nothing Then
                _context.SetStatus("Erreur lors du chargement du référentiel.")
            End If
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerFiltre
    ' Rôle      : Filtre la liste en mémoire sur le texte de recherche et lie la grille.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerFiltre()

        If _lignes Is Nothing Then Return

        Dim resultats = _lignes.AsEnumerable()

        Dim texte As String = txtRecherche.Text.Trim()
        If Not String.IsNullOrEmpty(texte) Then
            resultats = resultats.Where(
                Function(l) (l.Code IsNot Nothing AndAlso l.Code.Contains(texte, StringComparison.OrdinalIgnoreCase)) OrElse
                            (l.Libelle IsNot Nothing AndAlso l.Libelle.Contains(texte, StringComparison.OrdinalIgnoreCase)))
        End If

        Dim liste = resultats.ToList()

        _remplissageGrille = True
        Try
            pagReferentiel.DefinirSource(liste)
        Finally
            _remplissageGrille = False
        End Try

        If _context IsNot Nothing Then
            _context.SetStatus($"{liste.Count} élément(s) affiché(s).")
        End If

        ActualiserBoutonsSelection()
        AfficherDetailSelection()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : LigneSelectionnee
    ' Rôle     : Retourne la ReferentielLigne sélectionnée dans la grille, ou Nothing.
    ' -------------------------------------------------------------------------------------------------
    Private Function LigneSelectionnee() As ReferentielLigne

        If dgvReferentiel.CurrentRow Is Nothing Then Return Nothing
        Return TryCast(dgvReferentiel.CurrentRow.DataBoundItem, ReferentielLigne)

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AfficherDetailSelection
    ' Rôle      : Affiche les valeurs de l'élément sélectionné dans le panneau d'édition (lecture).
    ' -------------------------------------------------------------------------------------------------
    Private Sub AfficherDetailSelection(Optional seulementSiChange As Boolean = False)

        Dim ligne As ReferentielLigne = LigneSelectionnee()

        ' Même ligne qu'à l'affichage précédent : rien à recharger (voir _idDetailAffiche).
        Dim idLigne As ULong = If(ligne Is Nothing, 0UL, ligne.IdReferentiel)
        If seulementSiChange AndAlso _idDetailAffiche.HasValue AndAlso _idDetailAffiche.Value = idLigne Then Return
        _idDetailAffiche = idLigne

        If ligne Is Nothing Then
            ViderDetail()
            Return
        End If

        txtCode.Text = ligne.Code
        txtLibelle.Text = ligne.Libelle
        numOrdre.Value = Math.Min(Math.Max(ligne.OrdreAffichage, CInt(numOrdre.Minimum)), CInt(numOrdre.Maximum))
        chkActif.Checked = ligne.Actif
        AfficherChampSupplementaire(ligne)
        ChargerDonneesLiees(ligne)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : ViderDetail
    ' Rôle      : Réinitialise le panneau d'édition.
    ' -------------------------------------------------------------------------------------------------
    Private Sub ViderDetail()
        txtCode.Clear()
        txtLibelle.Clear()
        numOrdre.Value = 0
        chkActif.Checked = True
        ViderChampSupplementaire()
        ChargerDonneesLiees(Nothing)
    End Sub

#End Region

#Region "Droits d'accès"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : AppliquerDroitsUtilisateur
    ' Rôle      : Restreint les actions de modification selon le rôle de l'utilisateur courant.
    ' -------------------------------------------------------------------------------------------------
    Private Sub AppliquerDroitsUtilisateur()

        Dim peutModifier As Boolean =
            _context IsNot Nothing AndAlso _context.HasRole(RoleMinimum)

        If Not peutModifier Then
            btnNouveau.Enabled = False
            btnModifier.Enabled = False
            btnActiverDesactiver.Enabled = False

            If _context IsNot Nothing Then
                _context.SetStatus("Consultation seule : droits insuffisants pour modifier ce référentiel.")
            End If
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : PeutModifier
    ' Rôle     : Vérifie le droit de modification avant toute action d'écriture.
    ' -------------------------------------------------------------------------------------------------
    Private Function PeutModifier() As Boolean
        Return _context IsNot Nothing AndAlso _context.HasRole(RoleMinimum)
    End Function

#End Region

#Region "Validation"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction : SaisieValide
    ' Rôle     : Valide les champs du panneau d'édition (requis, longueurs, unicité).
    ' Retour   : True si la saisie est valide, False sinon (erreurs affichées via ErrorProvider).
    ' -------------------------------------------------------------------------------------------------
    Private Function SaisieValide() As Boolean

        EffacerErreurs()

        Dim valide As Boolean = True

        Dim code As String = txtCode.Text.Trim()
        Dim libelle As String = txtLibelle.Text.Trim()

        ' Code requis
        If String.IsNullOrEmpty(code) Then
            DefinirErreur(txtCode, "Le code est obligatoire.")
            valide = False
        ElseIf CodeExisteDeja(code, _idEnCours) Then
            DefinirErreur(txtCode, "Ce code est déjà utilisé.")
            valide = False
        End If

        ' Libellé requis
        If String.IsNullOrEmpty(libelle) Then
            DefinirErreur(txtLibelle, "Le libellé est obligatoire.")
            valide = False
        ElseIf LibelleExisteDeja(libelle, _idEnCours) Then
            DefinirErreur(txtLibelle, "Ce libellé est déjà utilisé.")
            valide = False
        End If

        ' Validation du champ supplémentaire éventuel (ex. montant pour ref_prix_seance)
        If Not ValiderChampSupplementaire() Then
            valide = False
        End If

        Return valide

    End Function

    Private Sub DefinirErreur(control As Control, message As String)
        If _context IsNot Nothing Then
            _context.SetError(control, message)
        End If
    End Sub

    Private Sub EffacerErreurs()
        If _context IsNot Nothing Then
            _context.ClearAllErrors()
        End If
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : txtCode_KeyPress
    ' Rôle      : Filtre la saisie du code au clavier (lettres, chiffres et caractères de contrôle).
    ' Remarque  : L'espace est accepté ici uniquement si RemplacerEspacesParUnderscore est actif ;
    '             sa conversion en underscore est ensuite réalisée par txtCode_TextChanged.
    '             La mise en majuscules est assurée par CharacterCasing ; la longueur par MaxLength.
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtCode_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtCode.KeyPress

        If Char.IsControl(e.KeyChar) Then Return

        If e.KeyChar = " "c AndAlso RemplacerEspacesParUnderscore Then Return

        If Char.IsLetterOrDigit(e.KeyChar) Then Return

        e.Handled = True

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : txtCode_TextChanged
    ' Rôle      : Normalise le code après toute modification (frappe ou collage).
    ' Remarque  : Remplace les espaces par des underscores (si activé) et retire tout caractère
    '             non autorisé (hors lettres, chiffres et underscore). Conserve la position du curseur.
    '             Un garde (_miseAJourCode) évite la réentrance lors de la réécriture du texte.
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtCode_TextChanged(sender As Object, e As EventArgs) Handles txtCode.TextChanged

        If _miseAJourCode Then Return

        Dim original As String = txtCode.Text
        Dim resultat As New System.Text.StringBuilder(original.Length)

        For Each c As Char In original
            If RemplacerEspacesParUnderscore AndAlso c = " "c Then
                resultat.Append("_"c)
            ElseIf Char.IsLetterOrDigit(c) Then
                resultat.Append(c)
            ElseIf c = "_"c AndAlso RemplacerEspacesParUnderscore Then
                resultat.Append(c)
            End If
        Next

        Dim nettoye As String = resultat.ToString()

        If nettoye = original Then Return

        Dim position As Integer = txtCode.SelectionStart - (original.Length - nettoye.Length)

        _miseAJourCode = True
        txtCode.Text = nettoye
        txtCode.SelectionStart = Math.Max(0, Math.Min(position, nettoye.Length))
        _miseAJourCode = False

    End Sub

#End Region

#Region "Actions - Boutons"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnNouveau_Click
    ' Rôle      : Passe en mode création et prépare un panneau d'édition vierge.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnNouveau_Click(sender As Object, e As EventArgs) Handles btnNouveau.Click

        If Not PeutModifier() Then Return

        _idEnCours = 0
        ViderDetail()
        PasserEnMode(ModeReferentiel.Creation)
        txtCode.Focus()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnModifier_Click
    ' Rôle      : Passe en mode modification sur l'élément sélectionné.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnModifier_Click(sender As Object, e As EventArgs) Handles btnModifier.Click

        If Not PeutModifier() Then Return

        Dim ligne As ReferentielLigne = LigneSelectionnee()
        If ligne Is Nothing Then Return

        _idEnCours = ligne.IdReferentiel
        _ligneSystemeEnCours = ligne.EstSysteme
        AfficherDetailSelection()
        PasserEnMode(ModeReferentiel.Modification)
        txtLibelle.Focus()

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnActiverDesactiver_Click
    ' Rôle      : Bascule l'état actif de l'élément sélectionné (soft-delete / réactivation).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnActiverDesactiver_Click(sender As Object, e As EventArgs) Handles btnActiverDesactiver.Click

        If Not PeutModifier() Then Return

        Dim ligne As ReferentielLigne = LigneSelectionnee()
        If ligne Is Nothing Then Return

        Dim nouvelEtat As Boolean = Not ligne.Actif
        Dim action As String = If(nouvelEtat, "réactiver", "désactiver")

        If ligne.EstSysteme AndAlso Not nouvelEtat Then
            DialogChoix.Information($"L'élément « {ligne.Libelle} » est une ligne système utilisée par l'application : il ne peut pas être désactivé.",
                                    "Ligne système")
            Return
        End If

        Dim reponse As DialogResult = DialogChoix.Confirmer(
            $"Voulez-vous {action} l'élément « {ligne.Libelle} » ?",
            "Confirmation")

        If reponse <> DialogResult.Yes Then Return

        Try
            DefinirActivation(ligne.IdReferentiel, nouvelEtat)

            GestionLog.EcrireLog(
                $"Référentiel '{TitreReferentiel}' : élément Id={ligne.IdReferentiel} {action}.",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.Process)

            If _context IsNot Nothing Then
                _context.SetStatus($"Élément {action}.")
            End If

            ChargerListe()

        Catch ex As InvalidOperationException
            ' Refus du métier (ligne système, règle) : message rédigé pour l'utilisatrice.
            DialogChoix.Information(ex.Message, "Opération impossible")

        Catch ex As UnauthorizedAccessException
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")

        Catch ex As Exception
            GestionLog.EcrireException($"Erreur lors de l'opération '{action}' sur le référentiel '{TitreReferentiel}'.", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.Process)
            DialogChoix.Erreur($"Impossible de {action} l'élément ; rien n'a été modifié." & Environment.NewLine &
                               "Vérifiez la connexion à la base de données, puis recommencez.", "Erreur")
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnActualiser_Click
    ' Rôle      : Recharge la liste depuis la base.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnActualiser_Click(sender As Object, e As EventArgs) Handles btnActualiser.Click
        ChargerListe()
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnEnregistrer_Click
    ' Rôle      : Valide et enregistre la saisie (création ou modification).
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnEnregistrer_Click(sender As Object, e As EventArgs) Handles btnEnregistrer.Click

        If Not PeutModifier() Then Return
        If Not SaisieValide() Then Return

        Dim code As String = txtCode.Text.Trim()
        Dim libelle As String = txtLibelle.Text.Trim()
        Dim ordre As Integer = CInt(numOrdre.Value)
        Dim actif As Boolean = chkActif.Checked

        ' Garde-fou : une ligne système conserve son code d'origine et reste active
        If _mode = ModeReferentiel.Modification AndAlso _ligneSystemeEnCours Then
            Dim origine As ReferentielLigne = _lignes?.FirstOrDefault(Function(l) l.IdReferentiel = _idEnCours)
            If origine IsNot Nothing Then code = origine.Code
            actif = True
        End If

        Try
            If _mode = ModeReferentiel.Creation Then
                InsererElement(code, libelle, ordre, actif)
                GestionLog.EcrireLog(
                    $"Référentiel '{TitreReferentiel}' : création de l'élément '{code}'.",
                    GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Process)
            Else
                MettreAJourElement(_idEnCours, code, libelle, ordre, actif)
                GestionLog.EcrireLog(
                    $"Référentiel '{TitreReferentiel}' : modification de l'élément Id={_idEnCours} ('{code}').",
                    GestionLog.LogLevel.Succinct, GestionLog.LogCategory.Process)
            End If

            If _context IsNot Nothing Then
                _context.SetStatus("Enregistrement effectué.")
            End If

            _idEnCours = 0
            _ligneSystemeEnCours = False
            PasserEnMode(ModeReferentiel.Consultation)
            ChargerListe()

            ' Notifie un éventuel hôte (modal en contexte) qu'un enregistrement a réussi.
            RaiseEvent EnregistrementEffectue(Me, EventArgs.Empty)

        Catch ex As InvalidOperationException
            ' Refus du métier (ligne système, règle) : la saisie reste ouverte.
            DialogChoix.Information(ex.Message, "Enregistrement impossible")

        Catch ex As UnauthorizedAccessException
            DialogChoix.Avertissement(ex.Message, "Autorisation insuffisante")

        Catch ex As Exception
            GestionLog.EcrireException($"Erreur lors de l'enregistrement dans le référentiel '{TitreReferentiel}'.", ex,
                                       GestionLog.LogLevel.Complet, GestionLog.LogCategory.Process)
            DialogChoix.Erreur("Impossible d'enregistrer l'élément ; rien n'a été modifié." & Environment.NewLine &
                               "Vérifiez la connexion à la base de données, puis cliquez à nouveau sur Enregistrer.", "Erreur")
        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : btnAnnuler_Click
    ' Rôle      : Annule la saisie en cours et revient en consultation.
    ' -------------------------------------------------------------------------------------------------
    Private Sub btnAnnuler_Click(sender As Object, e As EventArgs) Handles btnAnnuler.Click
        If _garde IsNot Nothing AndAlso Not _garde.ConfirmerAnnulation(If(_mode = ModeReferentiel.Creation, "du nouvel élément", "de l'élément")) Then Return
        _idEnCours = 0
        _ligneSystemeEnCours = False
        PasserEnMode(ModeReferentiel.Consultation)
        AfficherDetailSelection()
    End Sub

#End Region

#Region "Actions - Grille et filtres"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvReferentiel_CellFormatting
    ' Version   : V2.0.0
    ' Date      : 18/10/2026
    ' Rôle      : Atténue une ligne inactive, comme dans les autres listes (UI-9.4 ; le cadenas des
    '             lignes système est devenu la pastille « Système », dgvReferentiel_CellPainting).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvReferentiel_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvReferentiel.CellFormatting

        If e.RowIndex < 0 Then Return

        Dim ligne As ReferentielLigne = TryCast(dgvReferentiel.Rows(e.RowIndex).DataBoundItem, ReferentielLigne)
        If ligne IsNot Nothing AndAlso Not ligne.Actif Then
            e.CellStyle.ForeColor = UITheme.ColorGrisMoyenFonce
            e.CellStyle.SelectionForeColor = UITheme.ColorGrisDiscret
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvReferentiel_CellPainting
    ' Version   : V1.0.0
    ' Date      : 18/10/2026
    ' Rôle      : Pastille de la colonne « État » ; une ligne normale n'en porte pas (cellule vide).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvReferentiel_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvReferentiel.CellPainting

        If e.RowIndex < 0 OrElse e.ColumnIndex <> colActif.Index Then Return

        Dim ligne As ReferentielLigne = TryCast(dgvReferentiel.Rows(e.RowIndex).DataBoundItem, ReferentielLigne)
        If ligne Is Nothing Then Return

        UtilsDataGrid.DessinerPastilleCellule(dgvReferentiel, e, ligne.LibelleEtat, UtilsPastilles.Teinte(ligne.Etat))

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvReferentiel_CellToolTipTextNeeded
    ' Version   : V1.0.0
    ' Date      : 18/10/2026
    ' Rôle      : Infobulle de la pastille « Système » (ce qu'elle interdit).
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvReferentiel_CellToolTipTextNeeded(sender As Object, e As DataGridViewCellToolTipTextNeededEventArgs) Handles dgvReferentiel.CellToolTipTextNeeded

        If e.RowIndex < 0 OrElse e.ColumnIndex <> colActif.Index Then Return

        Dim ligne As ReferentielLigne = TryCast(dgvReferentiel.Rows(e.RowIndex).DataBoundItem, ReferentielLigne)
        If ligne IsNot Nothing AndAlso ligne.EstSysteme Then
            e.ToolTipText = "Ligne système : code et activation verrouillés."
        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : dgvReferentiel_SelectionChanged
    ' Rôle      : Met à jour le détail et l'état des boutons à chaque changement de sélection.
    ' -------------------------------------------------------------------------------------------------
    Private Sub dgvReferentiel_SelectionChanged(sender As Object, e As EventArgs) Handles dgvReferentiel.SelectionChanged

        If _mode <> ModeReferentiel.Consultation Then Exit Sub
        If _remplissageGrille Then Exit Sub

        ActualiserBoutonsSelection()
        AfficherDetailSelection(seulementSiChange:=True)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : txtRecherche_TextChanged
    ' Rôle      : Applique le filtre de recherche à chaque frappe.
    ' -------------------------------------------------------------------------------------------------
    Private Sub txtRecherche_TextChanged(sender As Object, e As EventArgs) Handles txtRecherche.TextChanged
        If _mode = ModeReferentiel.Consultation Then
            AppliquerFiltre()
        End If
    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure : chkAfficherInactifs_CheckedChanged
    ' Rôle      : Recharge la liste en incluant ou non les éléments désactivés.
    ' -------------------------------------------------------------------------------------------------
    Private Sub chkAfficherInactifs_CheckedChanged(sender As Object, e As EventArgs) Handles chkAfficherInactifs.CheckedChanged
        If _mode = ModeReferentiel.Consultation Then
            ChargerListe()
        End If
    End Sub

#End Region

#Region "Modifications non enregistrées"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : PeutQuitter
    ' Version    : V1.0.0
    ' Date       : 19/10/2026
    '
    ' Rôle       : Avant de quitter l'écran (Home ou fenêtre hôte) : rien à perdre, ou Enregistrer /
    '              Quitter sans enregistrer / Rester, comme partout dans l'application (ADR-43).
    ' -------------------------------------------------------------------------------------------------
    Public Function PeutQuitter() As Boolean Implements IUnsavedChangesAware.PeutQuitter
        Return _garde Is Nothing OrElse _garde.PeutQuitter(If(_mode = ModeReferentiel.Creation, "du nouvel élément", "de l'élément"), AddressOf EnregistrerParLeBouton)
    End Function

    ' « Enregistrer » avant de quitter : même chemin que le bouton (validation, messages) ; True si
    ' l'enregistrement a abouti (l'écran est revenu en consultation).
    Private Function EnregistrerParLeBouton() As Boolean
        btnEnregistrer.PerformClick()
        Return _mode = ModeReferentiel.Consultation
    End Function

#End Region

End Class


' -------------------------------------------------------------------------------------------------
' Enum     : ModeReferentiel
' Projet   : Althéa
' Version  : V1.0.0
' Date     : 09/06/2026
' Auteur   : Joëlle (Manou) / Projet Althéa
'
' Rôle :
' Définit le mode courant d'un écran de gestion de référentiel.
'
' Valeurs :
' - Consultation : lecture seule, sélection dans la grille
' - Creation     : saisie d'un nouvel élément
' - Modification : édition d'un élément existant
' -------------------------------------------------------------------------------------------------
Public Enum ModeReferentiel
    Consultation = 0
    Creation = 1
    Modification = 2
End Enum
