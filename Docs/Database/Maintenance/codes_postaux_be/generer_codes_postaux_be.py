# -*- coding: utf-8 -*-
# -------------------------------------------------------------------------------------------------
# Script      : generer_codes_postaux_be.py
# Projet      : Althéa
# Version     : V1.0.0
# Date        : 24/10/2026
# Auteur      : Joëlle (Manou) / Projet Althéa
#
# Rôle        :
# Produit le script SQL qui remplit le référentiel des codes postaux belges (ref_codes_postaux_be)
# à partir des fichiers publics : liste de bpost et registre des adresses BeST (SPF BOSA).
#
# Pourquoi deux sources (décision de Joëlle, 24/10/2026) :
# - bpost est la liste officielle des codes postaux, mais n'y donne qu'un nom par localité, en
#   néerlandais pour Bruxelles et les communes à facilités (« Brussel », « Elsene », « Moeskroen ») ;
# - BeST, le registre officiel des adresses, donne les noms des communes en français, néerlandais
#   et allemand, et les noms postaux (sections, hameaux) employés dans les adresses réelles.
#
# Utilisation (Python 3, module xlrd pour le fichier .xls de bpost) :
#   python generer_codes_postaux_be.py <zipcodes_num_fr_AAAA.xls> <dossier des openaddress-be*.zip>
#                                      <sortie.sql>
#   - bpost   : https://www.bpost.be/fr/outil-de-validation-de-codes-postaux (liste par code postal)
#   - BeST    : https://opendata.bosa.be/download/best/openaddress-bebru.zip, -bewal.zip, -bevlg.zip
#
# Remarques   :
# - Le SQL produit remplace tout le contenu de la table (DELETE puis INSERT, dans une transaction
#   terminée par COMMIT) : il sert tel quel de script de maintenance, et la migration v2.56.0
#   l'inclut après la création de la table.
# - Codes retenus : de 1000 à 9999 (les codes bpost spéciaux de moins de 4 chiffres sont écartés).
# - Noms d'un code : localités bpost, noms des communes de BeST (fr, nl, de) et noms postaux de BeST
#   (« BRUGGE/Koolkerke » donne deux noms), plus AJOUTS_MANUELS, après correction des défauts des
#   sources (nettoyer : « œ » mal lu, « Liège_ », « Cuesmes (7000) ») ; un seul nom par écriture
#   (comparaison sans accents, majuscules, traits d'union, « œ » ni « Saint / Sint / St ») ; l'écriture
#   gardée est, dans l'ordre : en minuscules plutôt qu'en capitales, celle de bpost ou des ajouts,
#   puis le nom de la commune dans la langue de sa région, puis les autres noms (« Brugge » et non
#   « Brügge »).
# - Nom proposé (rempli d'office à la saisie du code) : le nom de la commune qui regroupe au moins
#   80 % des adresses du code, dans la langue de la région (Bruxelles et Wallonie : français ;
#   Communauté germanophone : allemand ; Flandre : néerlandais) ; aucun si le code est partagé.
#   Un code connu de bpost seul (codes d'organismes) propose son unique localité bpost.
# - Aucune donnée personnelle : les adresses de BeST ne servent qu'à compter les adresses par
#   commune ; rues et numéros ne sont pas conservés.
# -------------------------------------------------------------------------------------------------

import collections
import csv
import io
import os
import re
import sys
import unicodedata
import zipfile

import xlrd

# Noms français absents des deux sources (BeST ne donne pas les sections de Bruxelles-Ville).
AJOUTS_MANUELS = {
    "1020": ["Laeken"],
}

# Communes de la Communauté germanophone (code INS) : nom proposé en allemand.
COMMUNES_GERMANOPHONES = {"63001", "63012", "63013", "63023", "63040", "63048", "63061", "63067", "63087"}

SEUIL_PROPOSITION = 0.80

PARTICULES = {"op", "den", "de", "der", "aan", "ter", "ten", "het", "van", "sur", "sous", "lez", "les",
              "la", "le", "en", "du", "des", "aux", "au", "d", "l", "s", "t", "bij", "in", "am", "an"}


def nettoyer(nom):
    """Corrige les défauts des fichiers sources : « œ » lu comme un caractère de contrôle (fichier
    bpost en cp1252), soulignés parasites (« Liège_ »), code répété entre parenthèses (« Cuesmes
    (7000) »)."""
    texte = "".join(c.encode("latin-1").decode("cp1252") if "\x80" <= c <= "\x9f" else c for c in nom)
    texte = re.sub(r"\s*\(\d{4}\)\s*$", "", texte)
    return re.sub(r"\s+", " ", texte.replace("_", " ")).strip()


def cle(nom):
    """Forme de comparaison, la même que UtilsCodePostal.CleLocalite dans Althéa."""
    texte = unicodedata.normalize("NFD", nom.strip())
    texte = "".join(c for c in texte if unicodedata.category(c) != "Mn").lower()
    texte = texte.replace("œ", "oe").replace("æ", "ae")
    for sep in ("-", "'", "’", "."):
        texte = texte.replace(sep, " ")
    mots = []
    for mot in texte.split():
        if mot in ("saint", "sint", "st"):
            mot = "st"
        elif mot in ("sainte", "ste"):
            mot = "ste"
        mots.append(mot)
    return " ".join(mots)


def en_capitales(nom):
    lettres = [c for c in nom if c.isalpha()]
    return bool(lettres) and all(c.isupper() for c in lettres)


def titre(nom):
    """« SINT-PIETERS-LEEUW » → « Sint-Pieters-Leeuw » ; particules en minuscules."""
    resultat, mot, premier = [], "", True
    def vider():
        nonlocal mot, premier
        if mot:
            m = mot.lower()
            resultat.append(m if (m in PARTICULES and not premier) else m[:1].upper() + m[1:])
            premier = False
            mot = ""
    for c in nom:
        if c in " -'":
            vider()
            resultat.append(c)
        else:
            mot += c
    vider()
    return "".join(resultat)


# Priorité des sources pour l'écriture gardée (plus petit = préféré).
PRIORITE_BPOST, PRIORITE_COMMUNE, PRIORITE_POSTAL, PRIORITE_AUTRE_LANGUE = 0, 1, 2, 3


def meilleure_ecriture(ecritures):
    """ecritures : {écriture: priorité}. Minuscules d'abord, puis la priorité, puis les accents."""
    def rang(n):
        accents = sum(1 for c in unicodedata.normalize("NFD", n) if unicodedata.category(c) == "Mn")
        return (en_capitales(n), ecritures[n], -accents, n)
    choisi = sorted(ecritures, key=rang)[0]
    return titre(choisi) if en_capitales(choisi) else choisi


def lire_bpost(chemin):
    feuille = xlrd.open_workbook(chemin).sheet_by_index(0)
    lignes = []
    for r in range(1, feuille.nrows):
        v = feuille.row_values(r)
        try:
            code = int(v[0])
        except (TypeError, ValueError):
            continue
        if 1000 <= code <= 9999 and str(v[1]).strip():
            lignes.append((str(code), str(v[1]).strip()))
    return lignes


def lire_best(dossier):
    communes = {}                                  # INS → (fr, nl, de, région)
    adresses = collections.Counter()               # (code, INS) → nombre d'adresses
    noms_postaux = collections.defaultdict(set)    # code → noms postaux
    for region in ("bebru", "bewal", "bevlg"):
        archive = zipfile.ZipFile(os.path.join(dossier, f"openaddress-{region}.zip"))
        with archive.open(archive.namelist()[0]) as f:
            for ligne in csv.DictReader(io.TextIOWrapper(f, encoding="utf-8")):
                if ligne.get("status") != "current":
                    continue
                code, ins = ligne["postcode"].strip(), ligne["municipality_id"].strip()
                communes[ins] = (ligne.get("municipality_name_fr", "").strip(),
                                 ligne.get("municipality_name_nl", "").strip(),
                                 ligne.get("municipality_name_de", "").strip(),
                                 ligne["region_code"].strip())
                adresses[(code, ins)] += 1
                for champ in ("postname_fr", "postname_nl", "postname_de"):
                    for nom in (ligne.get(champ) or "").split("/"):
                        if nom.strip():
                            noms_postaux[code].add(nom.strip())
    return communes, adresses, noms_postaux


def nom_dans_la_langue(ins, commune):
    fr, nl, de, region = commune
    if ins in COMMUNES_GERMANOPHONES and de:
        return de
    if region == "BE-VLG":
        return nl or fr
    return fr or nl


def construire(bpost, communes, adresses, noms_postaux):
    # code → clé → {écriture: priorité}
    ecritures = collections.defaultdict(lambda: collections.defaultdict(dict))
    def ajouter(code, nom, priorite):
        nom = nettoyer(nom)
        if nom:
            actuelles = ecritures[code][cle(nom)]
            actuelles[nom] = min(priorite, actuelles.get(nom, priorite))

    bpost_par_code = collections.defaultdict(list)
    for code, nom in bpost:
        ajouter(code, nom, PRIORITE_BPOST)
        bpost_par_code[code].append(nom)
    codes_best = set()
    for (code, ins), _ in adresses.items():
        codes_best.add(code)
        dans_la_langue = nom_dans_la_langue(ins, communes[ins])
        for nom in communes[ins][:3]:
            ajouter(code, nom, PRIORITE_COMMUNE if nom == dans_la_langue else PRIORITE_AUTRE_LANGUE)
    for code, noms in noms_postaux.items():
        for nom in noms:
            ajouter(code, nom, PRIORITE_POSTAL)
    for code, noms in AJOUTS_MANUELS.items():
        for nom in noms:
            ajouter(code, nom, PRIORITE_BPOST)

    # Nom proposé par code.
    proposes = {}
    total_par_code = collections.Counter()
    for (code, ins), n in adresses.items():
        total_par_code[code] += n
    for code in ecritures:
        if code in codes_best:
            ins, n = max(((i, k) for (c, i), k in adresses.items() if c == code), key=lambda x: x[1])
            if n / total_par_code[code] >= SEUIL_PROPOSITION:
                proposes[code] = cle(nettoyer(nom_dans_la_langue(ins, communes[ins])))
        elif len(set(cle(nettoyer(n)) for n in bpost_par_code[code])) == 1:
            proposes[code] = cle(nettoyer(bpost_par_code[code][0]))

    lignes = []
    for code in sorted(ecritures):
        for k, noms in sorted(ecritures[code].items()):
            lignes.append((code, meilleure_ecriture(noms), 1 if proposes.get(code) == k else 0))
    return lignes


def sql(texte):
    return "'" + texte.replace("\\", "\\\\").replace("'", "''") + "'"


def ecrire(lignes, sortie, sources):
    with open(sortie, "w", encoding="utf-8", newline="\r\n") as f:
        f.write("-- ---------------------------------------------------------------------------\n")
        f.write("-- Référentiel des codes postaux belges : contenu (généré)\n")
        f.write("--\n")
        f.write("-- Produit par Docs/Database/Maintenance/codes_postaux_be/generer_codes_postaux_be.py\n")
        for s in sources:
            f.write(f"--   {s}\n")
        nb_codes = len({l[0] for l in lignes})
        f.write(f"-- {len(lignes)} noms pour {nb_codes} codes postaux ; remplace tout le contenu de la table.\n")
        f.write("-- ---------------------------------------------------------------------------\n\n")
        f.write("/*!40101 SET NAMES utf8mb4 */;\n\n")
        f.write("START TRANSACTION;\n\n")
        f.write("DELETE FROM ref_codes_postaux_be;\n\n")
        for i in range(0, len(lignes), 500):
            bloc = lignes[i:i + 500]
            f.write("INSERT INTO ref_codes_postaux_be (code_postal, localite, est_proposee) VALUES\n")
            f.write(",\n".join(f"({sql(c)}, {sql(n)}, {p})" for c, n, p in bloc))
            f.write(";\n\n")
        f.write("COMMIT;\n")


def main():
    if len(sys.argv) != 4:
        print(__doc__ or "Usage : generer_codes_postaux_be.py <bpost.xls> <dossier BeST> <sortie.sql>")
        sys.exit(2)
    chemin_bpost, dossier_best, sortie = sys.argv[1:]
    bpost = lire_bpost(chemin_bpost)
    communes, adresses, noms_postaux = lire_best(dossier_best)
    lignes = construire(bpost, communes, adresses, noms_postaux)
    sources = [f"bpost : {os.path.basename(chemin_bpost)}",
               "BeST (SPF BOSA) : openaddress-bebru.zip, openaddress-bewal.zip, openaddress-bevlg.zip"]
    ecrire(lignes, sortie, sources)
    print(f"{len(lignes)} noms, {len({l[0] for l in lignes})} codes, "
          f"{sum(l[2] for l in lignes)} noms proposés -> {sortie}")


if __name__ == "__main__":
    main()
