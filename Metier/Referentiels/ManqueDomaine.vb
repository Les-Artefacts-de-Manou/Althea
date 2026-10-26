' -------------------------------------------------------------------------------------------------
' Fichier     : ManqueDomaine
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 23/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Point à compléter pour qu'un domaine soit utilisable sans surprise (tarif, quota, natures…), avec
' l'étape de l'écran Domaines où le corriger.
'
' Remarques   :
' - Rendu par GestionDomaines.CalculerManques (règles de complétude, audit C.2 étape 3) ; lu par
'   l'écran Domaines (état des étapes, pastille Complet / À compléter) et par sa synthèse.
' - Déplacé depuis UC_SyntheseDomaine (V2.1.0, 14/10/2026), sans changement : les règles sont du
'   métier, l'écran ne fait que les afficher.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

' Étape de l'écran Domaines concernée par un manque de complétude.
Public Enum EtapeDomaine
    General = 0
    Tarifs = 1
    Quotas = 2
    Natures = 3
End Enum

' Point à compléter pour qu'un domaine soit utilisable, avec l'étape où le corriger.
Public Class ManqueDomaine
    Public Property Etape As EtapeDomaine
    Public Property Message As String
End Class
