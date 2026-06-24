' -------------------------------------------------------------------------------------------------
' Module      : UtilsButtons
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 14/05/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Centralise les comportements UI communs aux boutons (standards, larges, Home).
'
' Responsabilités :
' - Gérer les boutons standards 32px (InitStandardButton)
' - Gérer les boutons larges 48px (InitLargeIconButton)
' - Gérer les boutons du menu Home (InitHomeButton)
' - Charger les images depuis Assets sans passer par les Resources
' - Appliquer les couleurs de UITheme selon l'état (Normal, Hover, Down, Disabled, Selected)
' - Gérer les événements souris (MouseEnter, MouseLeave, MouseDown, MouseUp)
' - Gérer l'état Enabled/Disabled
'
' Remarques   :
' - Les couleurs et chemins viennent de UITheme
' - Les boutons standards utilisent Tag = xxx_normal (suffixe _normal obligatoire)
' - Les boutons Home utilisent Tag = xxx (sans suffixe)
' - Les alignements définis dans le Designer sont conservés
' - Module statique (méthodes partagées)
'
' Dépendances :
' - UITheme (couleurs et chemins)
' - GestionLog (logs d'erreurs)

' Imports :
' - System.Drawing (pour Color)
' - System.IO (pour Path)
' - System.Windows.Forms (pour Button, FlatStyle, etc.)
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On
Imports System.IO

Module UtilsButtons

#Region "Types internes"

    ' Enum pour différencier les dossiers d'images des boutons standards (32px vs 48px)
    Private Enum ButtonImageFolder
        Standard32
        Standard48
        StandardDivers
    End Enum

    ' Dictionnaire pour stocker le dossier d'images associé à chaque bouton standard
    Private ReadOnly _buttonImageFolders As New Dictionary(Of Button, ButtonImageFolder)

    ' Dictionnaire pour stocker l'état d'un bouton Home
    Private ReadOnly _homeButtonStates As New Dictionary(Of Button, String)

#End Region

#Region "Boutons standards"

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InitStandardButton
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Initialise un bouton standard avec icône 32px.
    '
    ' Paramètres :
    ' - btn : Le bouton à initialiser
    '
    ' Remarques  :
    ' - Le Tag du bouton doit se terminer par "_normal"
    ' - Délègue à InitStandardButton privée avec ButtonImageFolder.Standard32
    ' -------------------------------------------------------------------------------------------------
    Public Sub InitStandardButton(
        btn As Button
    )

        InitStandardButton(
            btn,
            ButtonImageFolder.Standard32
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InitLargeIconButton
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Initialise un bouton large avec icône 48px.
    '
    ' Paramètres :
    ' - btn : Le bouton à initialiser
    '
    ' Remarques  :
    ' - Le Tag du bouton doit se terminer par "_normal"
    ' - Délègue à InitStandardButton privée avec ButtonImageFolder.Standard48
    ' -------------------------------------------------------------------------------------------------
    Public Sub InitLargeIconButton(
        btn As Button
    )

        InitStandardButton(
            btn,
            ButtonImageFolder.Standard48
        )

    End Sub

    Public Sub InitDiversIconButton(
        btn As Button
    )

        InitStandardButton(
            btn,
            ButtonImageFolder.StandardDivers
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InitStandardButton (Privée)
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Initialise un bouton standard avec le dossier d'images spécifié (32px ou 48px).
    '
    ' Paramètres :
    ' - btn         : Le bouton à initialiser
    ' - imageFolder : Dossier d'images (Standard32 ou Standard48)
    '
    ' Remarques  :
    ' - Valide que le Tag se termine par "_normal"
    ' - Configure FlatStyle, FlatAppearance et couleurs UITheme
    ' - Attache les handlers souris et EnabledChanged
    ' - Enregistre le bouton dans _buttonImageFolders
    ' -------------------------------------------------------------------------------------------------
    Private Sub InitStandardButton(
    btn As Button,
    imageFolder As ButtonImageFolder
)

        If btn Is Nothing Then Exit Sub

        Dim baseName As String =
        GetStandardButtonBaseName(btn)

        If String.IsNullOrWhiteSpace(baseName) Then

            If Not IsDesignMode(btn) Then

                GestionLog.EcrireLog(
                $"InitStandardButton : Tag invalide ou manquant pour {btn.Name}. Attendu : xxx_normal",
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI
            )

            End If

            Exit Sub

        End If

        btn.FlatStyle = FlatStyle.Flat
        btn.FlatAppearance.BorderSize = 0
        btn.FlatAppearance.MouseOverBackColor = UITheme.ButtonStandardBackHover
        btn.FlatAppearance.MouseDownBackColor = UITheme.ButtonStandardBackDown
        ' btn.FlatAppearance.BorderColor = btn.BackColor
        btn.UseMnemonic = False
        btn.UseVisualStyleBackColor = False

        RemoveStandardButtonHandlers(btn)

        _buttonImageFolders(btn) = imageFolder

        AddHandler btn.MouseEnter, AddressOf OnStandardButtonMouseEnter
        AddHandler btn.MouseLeave, AddressOf OnStandardButtonMouseLeave
        AddHandler btn.MouseDown, AddressOf OnStandardButtonMouseDown
        AddHandler btn.MouseUp, AddressOf OnStandardButtonMouseUp
        AddHandler btn.EnabledChanged, AddressOf OnStandardButtonEnabledChanged

        ApplyStandardButtonState(btn)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetStandardButtonBaseName
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Extrait le nom de base d'un bouton standard à partir de son Tag.
    '
    ' Paramètres :
    ' - btn : Le bouton à analyser
    '
    ' Retour     :
    ' - String : Le nom de base sans le suffixe "_normal", ou String.Empty si invalide
    '
    ' Remarques  :
    ' - Le Tag doit se terminer par UITheme.ImageSuffixNormal ("_normal")
    ' -------------------------------------------------------------------------------------------------
    Private Function GetStandardButtonBaseName(
        btn As Button
    ) As String

        If btn.Tag Is Nothing Then Return String.Empty

        Dim tagValue As String =
            btn.Tag.ToString().Trim()

        If Not tagValue.EndsWith(
            UITheme.ImageSuffixNormal,
            StringComparison.OrdinalIgnoreCase
        ) Then

            Return String.Empty

        End If

        Return tagValue.Substring(
            0,
            tagValue.Length - UITheme.ImageSuffixNormal.Length
        )

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OnStandardButtonMouseEnter
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Handler MouseEnter pour boutons standards : applique l'état "hover".
    '
    ' Paramètres :
    ' - sender : Le bouton source
    ' - e      : EventArgs
    '
    ' Remarques  :
    ' - Ne fait rien si le bouton est Disabled
    ' - Change BackColor, ForeColor et image vers "hover"
    ' -------------------------------------------------------------------------------------------------
    Private Sub OnStandardButtonMouseEnter(
        sender As Object,
        e As EventArgs
    )

        Dim btn As Button =
            TryCast(sender, Button)

        If btn Is Nothing OrElse Not btn.Enabled Then Exit Sub

        btn.BackColor = UITheme.ButtonStandardBackHover
        btn.ForeColor = UITheme.ButtonStandardForeHover

        ApplyStandardButtonImage(
            btn,
            "hover"
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OnStandardButtonMouseLeave
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Handler MouseLeave pour boutons standards : restaure l'état "normal".
    '
    ' Paramètres :
    ' - sender : Le bouton source
    ' - e      : EventArgs
    '
    ' Remarques  :
    ' - Ne fait rien si le bouton est Disabled
    ' - Restaure BackColor, ForeColor et image vers "normal"
    ' -------------------------------------------------------------------------------------------------
    Private Sub OnStandardButtonMouseLeave(
        sender As Object,
        e As EventArgs
    )

        Dim btn As Button =
            TryCast(sender, Button)

        If btn Is Nothing OrElse Not btn.Enabled Then Exit Sub

        btn.BackColor = UITheme.ButtonStandardBackNormal
        btn.ForeColor = UITheme.ButtonStandardForeNormal

        ApplyStandardButtonImage(
            btn,
            "normal"
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OnStandardButtonMouseDown
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Handler MouseDown pour boutons standards : applique l'état "down" (enfoncé).
    '
    ' Paramètres :
    ' - sender : Le bouton source
    ' - e      : MouseEventArgs
    '
    ' Remarques  :
    ' - Ne fait rien si le bouton est Disabled
    ' - Change BackColor et ForeColor vers "down" (image inchangée)
    ' -------------------------------------------------------------------------------------------------
    Private Sub OnStandardButtonMouseDown(
        sender As Object,
        e As MouseEventArgs
    )

        Dim btn As Button =
            TryCast(sender, Button)

        If btn Is Nothing OrElse Not btn.Enabled Then Exit Sub

        btn.BackColor = UITheme.ButtonStandardBackDown
        btn.ForeColor = UITheme.ButtonStandardForeDown

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OnStandardButtonMouseUp
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Handler MouseUp pour boutons standards : restaure l'état "hover".
    '
    ' Paramètres :
    ' - sender : Le bouton source
    ' - e      : MouseEventArgs
    '
    ' Remarques  :
    ' - Ne fait rien si le bouton est Disabled
    ' - Restaure BackColor, ForeColor et image vers "hover" (souris encore sur le bouton)
    ' -------------------------------------------------------------------------------------------------
    Private Sub OnStandardButtonMouseUp(
        sender As Object,
        e As MouseEventArgs
    )

        Dim btn As Button =
            TryCast(sender, Button)

        If btn Is Nothing OrElse Not btn.Enabled Then Exit Sub

        btn.BackColor = UITheme.ButtonStandardBackHover
        btn.ForeColor = UITheme.ButtonStandardForeHover

        ApplyStandardButtonImage(
            btn,
            "hover"
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OnStandardButtonEnabledChanged
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Handler EnabledChanged pour boutons standards : met à jour l'état visuel.
    '
    ' Paramètres :
    ' - sender : Le bouton source
    ' - e      : EventArgs
    '
    ' Remarques  :
    ' - Appelle ApplyStandardButtonState pour synchroniser l'apparence avec Enabled
    ' -------------------------------------------------------------------------------------------------
    Private Sub OnStandardButtonEnabledChanged(
        sender As Object,
        e As EventArgs
    )

        Dim btn As Button =
            TryCast(sender, Button)

        If btn Is Nothing Then Exit Sub

        ApplyStandardButtonState(btn)

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ApplyStandardButtonState
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Applique l'état visuel (couleurs et image) d'un bouton standard selon son Enabled.
    '
    ' Paramètres :
    ' - btn : Le bouton dont l'état doit être appliqué
    '
    ' Remarques  :
    ' - Si Enabled : état "normal" (BackColor, ForeColor, image _normal)
    ' - Si Disabled : état "disabled" (BackColor, ForeColor, image _disabled)
    ' -------------------------------------------------------------------------------------------------
    Private Sub ApplyStandardButtonState(
        btn As Button
    )

        If btn Is Nothing Then Exit Sub

        If btn.Enabled Then

            btn.BackColor = UITheme.ButtonStandardBackNormal
            btn.ForeColor = UITheme.ButtonStandardForeNormal

            ApplyStandardButtonImage(
                btn,
                "normal"
            )
        Else

            btn.BackColor = UITheme.ButtonStandardBackDisabled
            btn.ForeColor = UITheme.ButtonStandardForeDisabled

            ApplyStandardButtonImage(
                btn,
                "disabled"
            )

        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ApplyStandardButtonImage
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Charge et applique l'image d'un bouton standard selon l'état spécifié.
    '
    ' Paramètres :
    ' - btn   : Le bouton cible
    ' - state : État visuel ("normal", "hover", "disabled")
    '
    ' Remarques  :
    ' - Résout le suffixe d'image en fonction de state
    ' - Utilise ApplyImageWithFallback avec fallback sur _normal
    ' -------------------------------------------------------------------------------------------------
    Private Sub ApplyStandardButtonImage(
        btn As Button,
        state As String
    )

        Dim baseName As String =
            GetStandardButtonBaseName(btn)

        If String.IsNullOrWhiteSpace(baseName) Then Exit Sub

        Dim suffix As String =
            If(
                state = "hover",
                UITheme.ImageSuffixHover,
                If(
                    state = "disabled",
                    UITheme.ImageSuffixDisabled,
                    UITheme.ImageSuffixNormal
                )
            )

        Dim relativeFolder As String =
            GetStandardButtonAssetFolder(btn)

        Dim requestedPath As String =
            GetImagePath(
                relativeFolder,
                baseName & suffix
            )

        Dim fallbackPath As String =
            GetImagePath(
                relativeFolder,
                baseName & UITheme.ImageSuffixNormal
            )

        ApplyImageWithFallback(
            btn,
            requestedPath,
            fallbackPath
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetStandardButtonAssetFolder
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Retourne le chemin relatif du dossier d'assets pour un bouton standard (32px ou 48px).
    '
    ' Paramètres :
    ' - btn : Le bouton à analyser
    '
    ' Retour     :
    ' - String : Chemin relatif vers Assets\Boutons_ico_32 ou Assets\Boutons_ico_48
    '
    ' Remarques  :
    ' - Consulte _buttonImageFolders pour déterminer le dossier
    ' - Par défaut : Assets\Boutons_ico_32
    ' -------------------------------------------------------------------------------------------------
    Private Function GetStandardButtonAssetFolder(
    btn As Button
) As String

        If btn IsNot Nothing AndAlso
       _buttonImageFolders.ContainsKey(btn) Then
            Select Case _buttonImageFolders(btn)
                Case ButtonImageFolder.Standard48
                    Return UITheme.AssetsStandardButton48Path
                Case ButtonImageFolder.StandardDivers
                    Return UITheme.AssetsStandardButtonDiversPath
                Case ButtonImageFolder.Standard32
                    Return UITheme.AssetsStandardButton32Path
            End Select

        End If

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : RemoveStandardButtonHandlers
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Retire tous les handlers d'événements attachés à un bouton standard et nettoie le dictionnaire.
    '
    ' Paramètres :
    ' - btn : Le bouton à nettoyer
    '
    ' Remarques  :
    ' - RemoveHandler sur MouseEnter, MouseLeave, MouseDown, MouseUp, EnabledChanged
    ' - Supprime l'entrée dans _buttonImageFolders
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemoveStandardButtonHandlers(
    btn As Button
)

        If btn Is Nothing Then Exit Sub

        RemoveHandler btn.MouseEnter, AddressOf OnStandardButtonMouseEnter
        RemoveHandler btn.MouseLeave, AddressOf OnStandardButtonMouseLeave
        RemoveHandler btn.MouseDown, AddressOf OnStandardButtonMouseDown
        RemoveHandler btn.MouseUp, AddressOf OnStandardButtonMouseUp
        RemoveHandler btn.EnabledChanged, AddressOf OnStandardButtonEnabledChanged

        If _buttonImageFolders.ContainsKey(btn) Then
            _buttonImageFolders.Remove(btn)
        End If

    End Sub

#End Region

#Region "Boutons Home"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetHomeButtonState
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Récupère l'état enregistré d'un bouton Home.
    '
    ' Paramètres :
    ' - btn : Le bouton Home
    '
    ' Retour     :
    ' - String : État enregistré ("normal", "selected", etc.) ou "normal" par défaut
    '
    ' Remarques  :
    ' - Consulte le dictionnaire _homeButtonStates
    ' -------------------------------------------------------------------------------------------------
    Private Function GetHomeButtonState(
    btn As Button
) As String

        If btn IsNot Nothing AndAlso _homeButtonStates.ContainsKey(btn) Then
            Return _homeButtonStates(btn)
        End If

        Return "normal"

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetHomeButtonState
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Enregistre l'état d'un bouton Home dans le dictionnaire interne.
    '
    ' Paramètres :
    ' - btn   : Le bouton Home
    ' - state : État à enregistrer ("normal", "selected", etc.)
    '
    ' Remarques  :
    ' - Met à jour _homeButtonStates
    ' -------------------------------------------------------------------------------------------------
    Private Sub SetHomeButtonState(
    btn As Button,
    state As String
)

        If btn Is Nothing Then Exit Sub

        _homeButtonStates(btn) = state

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : InitHomeMenuButton
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Initialise un bouton du menu Home.
    '
    ' Paramètres :
    ' - btn : Le bouton Home à initialiser
    '
    ' Remarques  :
    ' - Le Tag du bouton doit contenir le nom logique (ex: "patients") sans suffixe
    ' - Attache les handlers MouseEnter, MouseLeave, EnabledChanged, Click
    ' - État initial : "normal"
    ' -------------------------------------------------------------------------------------------------
    Public Sub InitHomeMenuButton(
    btn As Button
)

        If btn Is Nothing Then Exit Sub

        Dim baseName As String =
        GetHomeButtonBaseName(btn)

        If String.IsNullOrWhiteSpace(baseName) Then

            If Not IsDesignMode(btn) Then
                GestionLog.EcrireLog(
            $"InitHomeMenuButton : Tag manquant pour {btn.Name}. Attendu : nom logique, ex. patients",
            GestionLog.LogLevel.Succinct,
            GestionLog.LogCategory.UI
        )
            End If

            Exit Sub

        End If

        btn.FlatStyle = FlatStyle.Flat
        btn.FlatAppearance.BorderSize = 0
        btn.FlatAppearance.MouseOverBackColor = UITheme.ButtonHomeBackHover
        btn.FlatAppearance.MouseDownBackColor = UITheme.ButtonHomeBackSelected
        btn.UseVisualStyleBackColor = False

        RemoveHomeMenuButtonHandlers(btn)

        SetHomeButtonState(btn, "normal")

        AddHandler btn.MouseEnter, AddressOf OnHomeMenuButtonMouseEnter
        AddHandler btn.MouseLeave, AddressOf OnHomeMenuButtonMouseLeave
        AddHandler btn.EnabledChanged, AddressOf OnHomeMenuButtonEnabledChanged
        AddHandler btn.Click, AddressOf OnHomeMenuButtonClick

        ApplyHomeMenuButtonState(
        btn,
        GetHomeButtonState(btn)
    )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : SetSelectedHomeMenuButton
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Définit le bouton sélectionné dans le menu Home et remet les autres à "normal".
    '
    ' Paramètres :
    ' - selectedButton : Le bouton à mettre en état "selected"
    ' - menuButtons    : Collection de tous les boutons du menu Home
    '
    ' Remarques  :
    ' - Parcourt tous les boutons et applique l'état approprié
    ' - Met à jour l'état interne (_homeButtonStates) et l'apparence
    ' -------------------------------------------------------------------------------------------------
    Public Sub SetSelectedHomeMenuButton(
    selectedButton As Button,
    menuButtons As IEnumerable(Of Button)
)

        If selectedButton Is Nothing OrElse menuButtons Is Nothing Then Exit Sub

        For Each btn As Button In menuButtons

            If btn Is Nothing Then Continue For

            If btn Is selectedButton Then

                SetHomeButtonState(btn, "selected")

                ApplyHomeMenuButtonState(
                btn,
                "selected"
            )
            Else

                SetHomeButtonState(btn, "normal")

                ApplyHomeMenuButtonState(
                btn,
                "normal"
            )

            End If

        Next

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetHomeButtonBaseName
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Extrait le nom de base d'un bouton Home à partir de son Tag.
    '
    ' Paramètres :
    ' - btn : Le bouton Home
    '
    ' Retour     :
    ' - String : Le nom logique (Tag du bouton), ou String.Empty si invalide
    '
    ' Remarques  :
    ' - Contrairement aux boutons standards, le Tag ne contient PAS de suffixe "_normal"
    ' -------------------------------------------------------------------------------------------------
    Private Function GetHomeButtonBaseName(
        btn As Button
    ) As String

        If btn.Tag Is Nothing Then Return String.Empty

        Return btn.Tag.ToString().Trim()

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OnHomeMenuButtonMouseEnter
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Handler MouseEnter pour boutons Home : applique l'état "hover" si non sélectionné.
    '
    ' Paramètres :
    ' - sender : Le bouton source
    ' - e      : EventArgs
    '
    ' Remarques  :
    ' - Ne fait rien si le bouton est Disabled
    ' - N'applique "hover" que si l'état n'est pas "selected"
    ' -------------------------------------------------------------------------------------------------
    Private Sub OnHomeMenuButtonMouseEnter(
    sender As Object,
    e As EventArgs
)

        Dim btn As Button =
        TryCast(sender, Button)

        If btn Is Nothing OrElse Not btn.Enabled Then Exit Sub

        If GetHomeButtonState(btn) <> "selected" Then

            ApplyHomeMenuButtonState(
            btn,
            "hover"
        )

        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OnHomeMenuButtonMouseLeave
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Handler MouseLeave pour boutons Home : restaure l'état enregistré ("selected" ou "normal").
    '
    ' Paramètres :
    ' - sender : Le bouton source
    ' - e      : EventArgs
    '
    ' Remarques  :
    ' - Ne fait rien si le bouton est Disabled
    ' - Restaure "selected" si c'est l'état enregistré, sinon "normal"
    ' -------------------------------------------------------------------------------------------------
    Private Sub OnHomeMenuButtonMouseLeave(
    sender As Object,
    e As EventArgs
)

        Dim btn As Button =
        TryCast(sender, Button)

        If btn Is Nothing OrElse Not btn.Enabled Then Exit Sub

        If GetHomeButtonState(btn) = "selected" Then

            ApplyHomeMenuButtonState(
            btn,
            "selected"
        )
        Else

            ApplyHomeMenuButtonState(
            btn,
            "normal"
        )

        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OnHomeMenuButtonEnabledChanged
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Handler EnabledChanged pour boutons Home : met à jour l'état visuel.
    '
    ' Paramètres :
    ' - sender : Le bouton source
    ' - e      : EventArgs
    '
    ' Remarques  :
    ' - Si Enabled : applique l'état enregistré
    ' - Si Disabled : applique l'état "disabled"
    ' -------------------------------------------------------------------------------------------------
    Private Sub OnHomeMenuButtonEnabledChanged(
    sender As Object,
    e As EventArgs
)

        Dim btn As Button =
        TryCast(sender, Button)

        If btn Is Nothing Then Exit Sub

        If btn.Enabled Then

            ApplyHomeMenuButtonState(
            btn,
            GetHomeButtonState(btn)
        )
        Else

            ApplyHomeMenuButtonState(
            btn,
            "disabled"
        )

        End If

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : OnHomeMenuButtonClick
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Handler Click pour boutons Home : enregistre le clic dans les logs.
    '
    ' Paramètres :
    ' - sender : Le bouton source
    ' - e      : EventArgs
    '
    ' Remarques  :
    ' - Écrit un log Rapide dans la catégorie UI
    ' -------------------------------------------------------------------------------------------------
    Private Sub OnHomeMenuButtonClick(
        sender As Object,
        e As EventArgs
    )

        Dim btn As Button =
            TryCast(sender, Button)

        If btn Is Nothing Then Exit Sub

        GestionLog.EcrireLog(
            $"Clic menu Home : {btn.Name}",
            GestionLog.LogLevel.Rapide,
            GestionLog.LogCategory.UI
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ApplyHomeMenuButtonState
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Applique l'état visuel (couleurs et image) d'un bouton Home selon l'état spécifié.
    '
    ' Paramètres :
    ' - btn   : Le bouton Home
    ' - state : État visuel ("selected", "hover", "disabled", "normal")
    '
    ' Remarques  :
    ' - Chaque état définit BackColor, ForeColor et suffixe d'image
    ' - Utilise ApplyHomeMenuImage pour charger l'image appropriée
    ' -------------------------------------------------------------------------------------------------
    Private Sub ApplyHomeMenuButtonState(
    btn As Button,
    state As String
)

        If btn Is Nothing Then Exit Sub

        Dim baseName As String =
        GetHomeButtonBaseName(btn)

        If String.IsNullOrWhiteSpace(baseName) Then Exit Sub

        Select Case state

            Case "selected"

                btn.BackColor = UITheme.ButtonHomeBackSelected
                btn.ForeColor = UITheme.ButtonHomeForeSelected

                ApplyHomeMenuImage(
                btn,
                baseName,
                UITheme.ImageSuffixSelected
            )

            Case "hover"

                btn.BackColor = UITheme.ButtonHomeBackHover
                btn.ForeColor = UITheme.ButtonHomeForeHover

                ApplyHomeMenuImage(
                btn,
                baseName,
                UITheme.ImageSuffixNormal
            )

            Case "disabled"

                btn.BackColor = UITheme.ButtonHomeBackDisabled
                btn.ForeColor = UITheme.ButtonHomeForeDisabled

                ApplyHomeMenuImage(
                btn,
                baseName,
                UITheme.ImageSuffixNormal
            )

            Case Else

                btn.BackColor = UITheme.ButtonHomeBackNormal
                btn.ForeColor = UITheme.ButtonHomeForeNormal

                ApplyHomeMenuImage(
                btn,
                baseName,
                UITheme.ImageSuffixNormal
            )

        End Select

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ApplyHomeMenuImage
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Charge et applique l'image d'un bouton Home avec fallback.
    '
    ' Paramètres :
    ' - btn      : Le bouton Home
    ' - baseName : Nom de base de l'image
    ' - suffix   : Suffixe d'image (ex: "_selected", "_normal")
    '
    ' Remarques  :
    ' - Utilise ApplyImageWithFallback avec fallback sur baseName sans suffixe
    ' - Chemin: UITheme.AssetsHomeButtonPath
    ' -------------------------------------------------------------------------------------------------
    Private Sub ApplyHomeMenuImage(
        btn As Button,
        baseName As String,
        suffix As String
    )

        Dim requestedPath As String =
            GetImagePath(
                UITheme.AssetsHomeButtonPath,
                baseName & suffix
            )

        Dim fallbackPath As String =
            GetImagePath(
                UITheme.AssetsHomeButtonPath,
                baseName
            )

        ApplyImageWithFallback(
            btn,
            requestedPath,
            fallbackPath
        )

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : RemoveHomeMenuButtonHandlers
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Retire tous les handlers d'événements attachés à un bouton Home et nettoie le dictionnaire.
    '
    ' Paramètres :
    ' - btn : Le bouton Home à nettoyer
    '
    ' Remarques  :
    ' - RemoveHandler sur MouseEnter, MouseLeave, EnabledChanged, Click
    ' - Supprime l'entrée dans _homeButtonStates
    ' -------------------------------------------------------------------------------------------------
    Private Sub RemoveHomeMenuButtonHandlers(
    btn As Button
)

        If btn Is Nothing Then Exit Sub

        RemoveHandler btn.MouseEnter, AddressOf OnHomeMenuButtonMouseEnter
        RemoveHandler btn.MouseLeave, AddressOf OnHomeMenuButtonMouseLeave
        RemoveHandler btn.EnabledChanged, AddressOf OnHomeMenuButtonEnabledChanged
        RemoveHandler btn.Click, AddressOf OnHomeMenuButtonClick

        If _homeButtonStates.ContainsKey(btn) Then
            _homeButtonStates.Remove(btn)
        End If

    End Sub

#End Region

#Region "Images"

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : GetImagePath
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Construit le chemin absolu complet vers une image.
    '
    ' Paramètres :
    ' - relativeFolder              : Chemin relatif du dossier (ex: "Assets\Boutons_ico_32")
    ' - imageNameWithoutExtension   : Nom de l'image sans extension (ex: "valider_normal")
    '
    ' Retour     :
    ' - String : Chemin absolu complet (StartupPath + relativeFolder + nom + .png)
    '
    ' Remarques  :
    ' - Ajoute automatiquement l'extension UITheme.ImageExtension (.png)
    ' -------------------------------------------------------------------------------------------------
    Private Function GetImagePath(
        relativeFolder As String,
        imageNameWithoutExtension As String
    ) As String

        Return Path.Combine(
            Application.StartupPath,
            relativeFolder,
            imageNameWithoutExtension & UITheme.ImageExtension
        )

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Procédure  : ApplyImageWithFallback
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Charge et applique une image à un bouton avec fallback en cas d'absence.
    '
    ' Paramètres :
    ' - btn           : Le bouton cible
    ' - requestedPath : Chemin de l'image demandée
    ' - fallbackPath  : Chemin de l'image de secours si requestedPath n'existe pas
    '
    ' Remarques  :
    ' - Vérifie d'abord File.Exists(requestedPath), sinon File.Exists(fallbackPath)
    ' - Utilise LoadImageWithoutLock pour éviter le verrouillage de fichier
    ' - Log l'exception en cas d'erreur
    ' -------------------------------------------------------------------------------------------------
    Private Sub ApplyImageWithFallback(
        btn As Button,
        requestedPath As String,
        fallbackPath As String
    )

        Try

            If File.Exists(requestedPath) Then

                btn.Image =
                    LoadImageWithoutLock(requestedPath)

            ElseIf File.Exists(fallbackPath) Then

                btn.Image =
                    LoadImageWithoutLock(fallbackPath)

            End If
        Catch ex As Exception

            GestionLog.EcrireException(
                $"UtilsForm - Erreur chargement image pour {btn.Name}",
                ex,
                GestionLog.LogLevel.Succinct,
                GestionLog.LogCategory.UI
            )

        End Try

    End Sub

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : LoadImageWithoutLock
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Charge une image depuis un fichier sans verrouiller le fichier.
    '
    ' Paramètres :
    ' - filePath : Chemin absolu de l'image
    '
    ' Retour     :
    ' - Image : Nouvelle instance Bitmap de l'image
    '
    ' Remarques  :
    ' - Utilise FileStream avec FileShare.ReadWrite pour éviter le verrouillage
    ' - Crée un nouveau Bitmap à partir du stream (copie)
    ' -------------------------------------------------------------------------------------------------
    Private Function LoadImageWithoutLock(
        filePath As String
    ) As Image

        Using fs As New FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite
        )

            Using tempImage As Image =
                Image.FromStream(fs)

                Return New Bitmap(tempImage)

            End Using

        End Using

    End Function

    ' -------------------------------------------------------------------------------------------------
    ' Fonction   : IsDesignMode
    ' Version    : V1.0.0
    ' Date       : 14/05/2026
    '
    ' Rôle       :
    ' Détermine si un contrôle est en mode Design (Visual Studio Designer).
    '
    ' Paramètres :
    ' - ctrl : Le contrôle à tester
    '
    ' Retour     :
    ' - Boolean : True si le contrôle est en mode Design, False sinon
    '
    ' Remarques  :
    ' - Utilisé pour éviter les logs d'erreur lors de l'exécution dans le Designer
    ' -------------------------------------------------------------------------------------------------
    Private Function IsDesignMode(
        ctrl As Control
    ) As Boolean

        Return ctrl IsNot Nothing AndAlso
               ctrl.Site IsNot Nothing AndAlso ctrl.Site.DesignMode

    End Function

#End Region

End Module