' -------------------------------------------------------------------------------------------------
' Module      : VersionSchema
' Projet      : Althéa
' Version     : V1.0.19
' Date        : 25/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Version du schéma de base attendue par CETTE version de l'application, et comparaison avec la
' version inscrite dans la base (tec_meta_schema).
'
' Règle de maintenance (Rules.md, § Base de données) :
' - À CHAQUE migration, mettre Attendue à la version de la migration, dans le même commit.
'
' Remarques   :
' - Utilisée par l'écran État du système (13/10/2026) et, depuis le 20/10/2026, par le contrôle au
'   démarrage (AppStartupManager) : base en retard → message et fermeture ; base plus récente ou
'   version illisible → avertissement, l'application continue (décisions de Joëlle).
' - V1.0.1 (19/10/2026) : Attendue = 2.41.0 (mention de confidentialité des documents).
' - V1.0.2 (19/10/2026) : Attendue = 2.42.0 (textes types).
' - V1.0.3 (19/10/2026) : Attendue = 2.43.0 (plusieurs usages par texte type).
' - V1.0.4 (20/10/2026) : Attendue = 2.44.0 (réglage du correcteur d'orthographe).
' - V1.0.5 (20/10/2026) : Attendue = 2.45.0 (logo des domaines).
' - V1.0.6 (20/10/2026) : Attendue = 2.46.0 (latéralités, modalités de paiement proposées).
' - V1.0.7 (20/10/2026) : Attendue = 2.47.0 (types d'identifiant INAMI, BCE, TVA en lignes système).
' - V1.0.8 (20/10/2026) : contrôle au démarrage (remarque ci-dessus) ; pas de changement de version.
' - V1.0.9 (20/10/2026) : Attendue = 2.48.0 (fin de l'élévation après inactivité).
' - V1.0.10 (21/10/2026) : Attendue = 2.49.0 (vues v_seances_solde et v_dossier_dominant).
' - V1.0.11 (21/10/2026) : Attendue = 2.50.0 (paiements.id_mode_paiement).
' - V1.0.12 (22/10/2026) : Attendue = 2.51.0 (colonnes de l'annulation d'un encaissement).
' - V1.0.13 (22/10/2026) : Attendue = 2.52.0 (table lia_paiements_avoirs).
' - V1.0.14 (22/10/2026) : Attendue = 2.53.0 (ref_statuts_seance.est_absence).
' - V1.0.15 (23/10/2026) : Attendue = 2.54.0 (NISS des patients normalisé).
' - V1.0.16 (23/10/2026) : Attendue = 2.55.0 (référentiel des spécialités des thérapeutes).
' - V1.0.17 (24/10/2026) : Attendue = 2.56.0 (référentiel des codes postaux belges).
' - V1.0.18 (24/10/2026) : Attendue = 2.57.0 (drapeaux de référentiel, audit C.3 étape 2).
' - V1.0.19 (25/10/2026) : Attendue = 2.58.0 (lignes système devenues inutiles déverrouillées).
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module VersionSchema

    ''' <summary>Dernière migration connue de cette version de l'application.</summary>
    Public Const Attendue As String = "2.58.0"

    ''' <summary>
    ''' Compare la version de la base à la version attendue : &lt; 0 si la base est en retard
    ''' (migration à appliquer), 0 si elles sont égales, &gt; 0 si la base est plus récente que
    ''' l'application ; Nothing si la version de la base est absente ou illisible.
    ''' </summary>
    Public Function ComparerAAttendue(versionBase As String) As Integer?
        Dim base As Version = Nothing
        Dim attendueLue As Version = Nothing
        If String.IsNullOrWhiteSpace(versionBase) OrElse
           Not Version.TryParse(versionBase.Trim(), base) OrElse
           Not Version.TryParse(Attendue, attendueLue) Then
            Return Nothing
        End If
        Return base.CompareTo(attendueLue)
    End Function

End Module
