' -------------------------------------------------------------------------------------------------
' Module     : UsagesTexteType
' Projet     : Althéa
' Version    : V1.1.0
' Date       : 19/10/2026
' Auteur     : Joëlle (Manou) / Projet Althéa
'
' Rôle       :
' Point unique des codes d'usage des textes types (ref_usages_textes_types.code_usage) : chaque
' éditeur de texte déclare le sien (UC_RichTextEditor.UsageTextesTypes) et ne propose que les
' textes de cet usage, plus ceux proposés partout.
'
' Remarques  :
' - Les lignes existent en base (migrations v2.42.0 et v2.43.0, lignes système) ; leur libellé et
'   leur ordre se changent dans Référentiels › Usages des textes types.
' - Règle (Reference_UI § 7.23) : tout éditeur de texte reçoit son usage. Nouvel éditeur : ligne
'   ajoutée par migration ET constante ici, puis l'écran la déclare.
' - V1.1.0 (19/10/2026, v2.43.0) : alerte du patient ; commentaires séparés (contact, intervenant,
'   thérapeute) au lieu d'un usage commun.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module UsagesTexteType

    Public Const Anamnese As String = "ANAMNESE"
    Public Const AlertePatient As String = "ALERTE_PATIENT"
    Public Const ZoneDossier As String = "ZONE_DOSSIER"
    Public Const NotesSeance As String = "NOTES_SEANCE"
    Public Const EvolutionSeance As String = "EVOLUTION_SEANCE"
    Public Const CommentaireContact As String = "COMMENTAIRE_CONTACT"
    Public Const CommentaireIntervenant As String = "COMMENTAIRE_INTERVENANT"
    Public Const CommentaireTherapeute As String = "COMMENTAIRE_THERAPEUTE"

End Module
