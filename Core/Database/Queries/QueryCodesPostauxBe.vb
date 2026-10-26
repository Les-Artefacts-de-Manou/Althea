' -------------------------------------------------------------------------------------------------
' Module      : QueryCodesPostauxBe
' Projet      : Althéa
' Version     : V1.0.0
' Date        : 24/10/2026
' Auteur      : Joëlle (Manou) / Projet Althéa
'
' Rôle        :
' Requêtes SQL du référentiel des codes postaux belges (ref_codes_postaux_be, migration v2.56.0).
'
' Remarques   :
' - Contient uniquement du SQL ; aucun accès à la base ici (GestionCodesPostauxBe).
' - Lecture seule : le contenu est remplacé par la migration ou le script de maintenance
'   (Docs/Database/Maintenance/codes_postaux_be), jamais par l'application.
' -------------------------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Public Module QueryCodesPostauxBe

    ' Toute la liste (≈ 4 000 lignes), lue une fois par session.
    Public ReadOnly Property SelectCodesPostauxBe As String
        Get
            Return "
SELECT code_postal, localite, est_proposee
FROM ref_codes_postaux_be
ORDER BY code_postal, localite;"
        End Get
    End Property

End Module
