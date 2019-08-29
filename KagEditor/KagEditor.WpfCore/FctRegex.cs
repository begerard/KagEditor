using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace KagEditor.WpfCore
{
    public static class FctRegex
    {
        /// <summary>
        /// Efface toutes les lignes de script.
        /// </summary>
        /// <param name="MyFile">Fichier à nettoyer.</param>
        public static void EraseScript(this KsFile MyFile)
        {
            MyFile.EraseTlaData();
            MyFile.TextString = Regex.Replace(MyFile.TextString, @"@.*\n", string.Empty);
        }

        /// <summary>
        /// Efface la page '*tladata' jusqu'a la fin du fichier
        /// </summary>
        /// <param name="MyFile"></param>
        public static void EraseTlaData(this KsFile MyFile)
        {
            var matches = Regex.Matches(MyFile.TextString, @"\*tladata");
            if (matches.Count > 0)
            {
                MyFile.TextString = MyFile.TextString.Remove(matches[matches.Count - 1].Index);
            }
        }

        /// <summary>
        /// Efface toutes les lignes de doublage.
        /// </summary>
        /// <param name="MyFile">Fichier à nettoyer.</param>
        public static void EraseSay(this KsFile MyFile)
        {
            MyFile.TextString = Regex.Replace(MyFile.TextString, @"^@say.*\n", string.Empty);
        }

        /// <summary>
        /// Efface toutes les lignes de commentaire.
        /// </summary>
        /// <param name="MyFile">Fichier à nettoyer.</param>
        public static void EraseComment(this KsFile MyFile)
        {
            MyFile.TextString = Regex.Replace(MyFile.TextString, "^;.*\n", string.Empty, RegexOptions.Multiline);
        }

        /// <summary>
        /// Efface toutes les lignes de commentaire.
        /// </summary>
        /// <param name="MyFile">Fichier à nettoyer.</param>
        public static void EraseCommentedScript(this KsFile MyFile)
        {
            MyFile.TextString = Regex.Replace(MyFile.TextString, "^;@.*\n", string.Empty, RegexOptions.Multiline);
        }

        /// <summary>
        /// Efface toutes les balises '[l][r]' ou '[lr]'.
        /// </summary>
        /// <param name="MyFile">Fichier à nettoyer.</param>
        public static void EraseLR(this KsFile MyFile, bool isRealtaNua)
        {
            if (isRealtaNua)
            {
                MyFile.TextString = Regex.Replace(MyFile.TextString, "[[]lr[]]", string.Empty);
            }
            else
            {
                MyFile.TextString = Regex.Replace(MyFile.TextString, "[[]l[]][[]r[]]", string.Empty);
            }
        }

        /// <summary>
        /// Efface une couche de balises de Wrap.
        /// </summary>
        /// <param name="MyFile">Fichier à nettoyer.</param>
        public static void EraseWrap(this KsFile MyFile)
        {
            MyFile.TextString = Regex.Replace(MyFile.TextString, "[[]wrap text=\".*?\"[]]", string.Empty);
        }

        /// <summary>
        /// Efface toutes les balises '[r]'.
        /// </summary>
        /// <param name="MyFile">Fichier à nettoyer.</param>
        public static void EraseSlashR(this KsFile MyFile)
        {
            MyFile.TextString = Regex.Replace(MyFile.TextString, "[\r]", string.Empty);
        }

        /// <summary>
        /// Efface toutes les lignes 'pageX'.
        /// </summary>
        /// <param name="MyFile">Fichier à nettoyer.</param>
        public static void ErasePage(this KsFile MyFile)
        {
            MyFile.TextString = Regex.Replace(MyFile.TextString, @"[*].*\n", string.Empty);
        }

        /// <summary>
        /// Efface les balises '[lineX]'.
        /// </summary>
        /// <param name="MyFile">Fichier à nettoyer.</param>
        public static void EraseLineX(this KsFile MyFile)
        {
            MyFile.TextString = Regex.Replace(MyFile.TextString, @"[[]line.*[]]", string.Empty);
        }

        /// <summary>
        /// Compte le nombre de page d'un Fichier.
        /// </summary>
        /// <param name="MyFile">Fichier à traiter.</param>
        /// <returns>Nombre de pages du Fichier.</returns>
        public static int CountPage(this KsFile MyFile)
        {
            MatchCollection MyMatchCollection = Regex.Matches(MyFile.TextString, "[*]page");

            return MyMatchCollection.Count;
        }

        /// <summary>
        /// Compte le nombre de mot de la liste de lignes d'un Fichier.
        /// </summary>
        /// <param name="MyFile">Fichier à traiter.</param>
        /// <returns>Nombre de pages du Fichiers.</returns>
        public static Int32 CountWord(this KsFile MyFile)
        {
            Regex myRegex = new Regex(@"\w");

            int nbWords = (from line in MyFile.Lines
                           from word in line.Split(null)
                           where myRegex.IsMatch(word)
                           select word).Count();

            return nbWords;
        }

        /// <summary>
        /// Remplace les apostrophes par des simples quotes.
        /// </summary>
        /// <param name="MyFile">Fichier à nettoyer.</param>
        public static void ReplaceApostrophe(this KsFile MyFile)
        {
            MyFile.TextString = Regex.Replace(MyFile.TextString, "‘", "'");
        }

        /// <summary>
        /// Remplace les doubles quotes par des guillemets.
        /// </summary>
        /// <param name="MyFile">Fichier à nettoyer.</param>
        public static void ReplaceDoubleQuote(this KsFile MyFile)
        {
            MyFile.TextString = Regex.Replace(MyFile.TextString, " \"", " « ");
            MyFile.TextString = Regex.Replace(MyFile.TextString, "\"", " »");
        }

        /// <summary>
        /// Teste si la ligne commence par un espace blanc.
        /// </summary>
        /// <param name="MyFile">Ligne à tester.</param>
        public static bool StartWithWhiteSpace(this string line)
        {
            return Regex.IsMatch(line, @"^\s");
        }

        /// <summary>
        /// Teste si la ligne n'est pas une ligne technique.
        /// </summary>
        /// <param name="MyFile">Ligne à tester.</param>
        public static bool IsTechnical(this string line)
        {
            return Regex.IsMatch(line, @"^[@*;]") || Regex.IsMatch(line, @"^\[resettime\]");
        }

        /// <summary>
        /// Teste si la ligne n'est pas un début de section tladata.
        /// </summary>
        /// <param name="MyFile">Ligne à tester.</param>
        public static bool IsTladata(this string line)
        {
            return line.StartsWith("*tladata");
        }

        /// <summary>
        /// Remplace une balise type LineX par des tirets.
        /// </summary>
        /// <param name="line">Ligne à traiter.</param>
        /// <param name="regex">Expression régulière définissant la balise.</param>
        /// <param name="dash">Chaine de caractères contenant les tirets.</param>
        /// <returns>Ligne avec la balise word-wrapper.</returns>
        public static string ReplaceLineX(string line, string regex, string dash)
        {
            //On cherche toute les balises du type donnée
            MatchCollection ReplaceMatches = Regex.Matches(line, regex);

            //On efface la dernière balise trouvé
            line = line.Remove(ReplaceMatches[ReplaceMatches.Count - 1].Index,
                ReplaceMatches[ReplaceMatches.Count - 1].Length);
            //On insert les X tirets crée auparavant à la place
            line = line.Insert(ReplaceMatches[ReplaceMatches.Count - 1].Index, dash);

            return line;
        }

        /// <summary>
        /// Découpe les pages d'un texte en supprimant les balises *page.
        /// </summary>
        /// <param name="text">Le texte à découper.</param>
        /// <returns>La liste des pages.</returns>
        public static List<string> SplitPage(string text)
        {
            return Regex.Split(text, @"[*].*\n").ToList();
        }

        /// <summary>
        /// Nettoye une liste de pages de tout caractère blanc ou de retour à la ligne.
        /// </summary>
        /// <param name="pages">Les pages à nettoyer.</param>
        /// <returns>Les pages nettoyées.</returns>
        public static List<string> CleanPages(List<string> pages)
        {
            return pages.Select(page => Regex.Replace(page, @"[[][^]]*[]]", String.Empty))
                .Select(page => Regex.Replace(page, @"\W", String.Empty)).ToList();
        }
    }
}
