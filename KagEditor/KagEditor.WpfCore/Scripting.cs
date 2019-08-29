using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace KagEditor.WpfCore
{
    public static class Scripting
    {
        /// <summary>
        /// Wrap les mots tout en gérant les autres balises !
        /// </summary>
        /// <param name="Lines">Liste de lignes à traiter.</param>
        public static void DoWordWrap(List<string> Lines)
        {
            string[] words;
            int word;
            Match MyMatch;

            //Pour chaque ligne
            for (int line = 0; line < Lines.Count; line++)
            {
                //Si la ligne commence par un espace
                if (Lines[line].StartWithWhiteSpace())
                {
                    //On fait un tableau avec chaque mot ou espace de la ligne (word)
                    words = Lines[line].Split(null);
                    //On vide la dite ligne
                    Lines[line] = null;

                    //Pour chaque word
                    for (word = 0; word < words.Length; word++)
                    {
                        //Si on a un mot
                        if (words[word].Length > 0)
                        {
                            //Si on rencontre un word composé,
                            if ((word < (words.Length - 1)) && (
                                //comme un mot suivit d'un ponctuation double
                                words[word] == "«" | words[word + 1] == "»" |
                                words[word + 1] == "?" | words[word + 1] == "?\"" | words[word + 1] == "?»" |
                                words[word + 1] == "!" | words[word + 1] == "!\"" | words[word + 1] == "!»" |
                                words[word + 1] == ":" | words[word + 1] == ":\"" | words[word + 1] == ":»" |
                                words[word + 1] == ";" | words[word + 1] == ";\"" | words[word + 1] == ";»" |
                                //ou comme une balise ouverte mais non fermé
                                (words[word].Contains("[") && !words[word].Contains("]"))
                                ))
                            {
                                //on ajoute ce morceau au suivant (et on ne traite pas ce morceau)
                                words[word + 1] = words[word + 1].Insert(0, words[word] + " ");
                            }

                            //Sinon, on essaye l'enrobage
                            else
                            {
                                //On écrit le mot enrobé, tout en remplaçant les doubles quotes par des tirets
                                //sinon ils font buguer... Comme tout bon double quote.
                                Lines[line] += "[wrap text=\"" + Regex.Replace(words[word], "\"", "-") + "\"]";

                                //On efface toutes balises [l] ou [r] rencontré
                                //il ne peut n'y en avoir qu'à la toute fin d'une ligne.
                                Lines[line] = Regex.Replace(Lines[line], "[[](l|r)[]]", string.Empty);

                                //Si on trouve la balise "[line X]" dans le mot qu'on vient d'insérer
                                MyMatch = Regex.Match(words[word], "[[]line.*?[]]");
                                if (MyMatch.Success)
                                {
                                    //On le remplace par des tirets dans le word-wrap.
                                    Lines[line] = FctRegex.ReplaceLineX(Lines[line], "[[]line.*?[]]",
                                        DoDash(words[word], MyMatch.Index + 5, MyMatch.Length - 6));
                                }
                                else
                                {
                                    //Sinon on essaye avec la balise "[wacky...]"
                                    MyMatch = Regex.Match(words[word], "[[]wacky len=.*?[]]");
                                    if (MyMatch.Success)
                                    {
                                        //On le remplace par des tirets dans le word-wrap.
                                        Lines[line] = FctRegex.ReplaceLineX(Lines[line], "[[]wacky len=.*?[]]",
                                            DoDash(words[word], MyMatch.Index + 11, MyMatch.Length - 12));
                                    }
                                    else
                                    {
                                        //Sinon on essaye avec la balise "[block...]"
                                        MyMatch = Regex.Match(words[word], "[[]block len=.*?[]]");
                                        if (MyMatch.Success)
                                        {
                                            //On le remplace par des tirets dans le word-wrap.
                                            Lines[line] = FctRegex.ReplaceLineX(Lines[line], "[[]block len=.*?[]]",
                                                DoDash(words[word], MyMatch.Index + 11, MyMatch.Length - 12));
                                        }
                                        else
                                        {
                                            //Sinon on essaye avec la balise "[ruby...]"
                                            MyMatch = Regex.Match(words[word], "[[]ruby text=.*?[]]");
                                            if (MyMatch.Success)
                                            {
                                                //On le vire du word-wrap.
                                                Lines[line] = FctRegex.ReplaceLineX(Lines[line], "[[]ruby text=.*?[]]", "");
                                            }
                                        }
                                    }
                                }

                                //On ajoute enfin le mot qui sera affiché
                                Lines[line] += words[word];

                                //Si nous ne sommes pas à la fin de la ligne
                                if (word < (words.Length - 1))
                                {
                                    //On ajoute un espace, pour préparer l'arrivé du mot suivant
                                    Lines[line] += " ";
                                }
                            }
                        }
                        //Si on a un espace
                        else
                        {
                            Lines[line] += " ";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fabrique le bon nombre de tiret pour une balise LineX
        /// </summary>
        /// <param name="word">Mot contenant la balise.</param>
        /// <param name="indexStart">Index où commence le nombre X dans le mot.</param>
        /// <param name="indexEnd">Index où fini le nombre X dans le mot.</param>
        /// <param name="charDash">Caratère à utilisé comme tiret.</param>
        /// <returns>Chainre de caractères contenant les tirets.</returns>
        public static string DoDash(string word, int indexStart, int indexEnd)
        {
            string dash = string.Empty;
            //On chope le nombre X et on le parse en int
            int nbDash = int.Parse(word.Substring(indexStart, indexEnd));

            //On crée une chaine avec le nombre de tiret défini par le nombre X
            for (int i = 0; i < nbDash; i++)
            {
                dash += '―';
            }

            return dash;
        }

        /// <summary>
        /// Ajoute les balises '[l][r]' ou '[lr]' à la fin de chaque ligne.
        /// </summary>
        /// <param name="Lines">Liste de lignes à traiter.</param>
        public static void DoLR(List<string> Lines, bool isRealtaNua)
        {
            //Pour chaque lignes
            for (int line = 0; line < Lines.Count; line++)
            {
                //On n'ajoute plus de LR une fois la section tladata atteinte
                if (Lines[line].IsTladata())
                {
                    break;
                }

                //On saute les lignes qui ne sont pas du texte
                if (Lines[line].IsTechnical())
                {
                    continue;
                }

                //Si c'est la dernière d'un fichier
                if (line == (Lines.Count - 1))
                {
                    //Si elle n'a pas déjà une forme particulières de ces balises
                    if (!Lines[line].EndsWith("[l]") && !Lines[line].EndsWith("[r]"))
                    {
                        //On lui ajoute la bonne balise
                        if (isRealtaNua)
                        {
                            Lines[line] += "[lr]";
                        }
                        else
                        {
                            Lines[line] += "[l][r]";
                        }
                    }

                    continue;
                }

                //Si ce n'est pas la dernière d'une page
                //if (!Lines[line + 1].StartsWith("*") & !Lines[line + 1].StartsWith("@"))
                if (!Lines[line + 1].StartsWith("@pg") && !Lines[line + 1].StartsWith("*"))
                {
                    //Si elle n'a pas déjà une forme particulières de ces balises
                    if (!Lines[line].EndsWith("[l]") && !Lines[line].EndsWith("[r]"))
                    {
                        //On lui ajoute la bonne balise
                        if (isRealtaNua)
                        {
                            Lines[line] += "[lr]";
                        }
                        else
                        {
                            Lines[line] += "[l][r]";
                        }
                    }

                    continue;
                }
            }
        }

        /// <summary>
        /// Ajoute des espace en début de phrases qui n'en ont pas.
        /// C'est une erreur récurrente de la VA.
        /// </summary>
        /// <param name="Lines">Liste de lignes à traiter.</param>
        /// <returns></returns>
        public static List<int> DoBlankSpace(List<string> Lines)
        {
            List<int> result = new List<int>();

            //Pour chaque lignes sauf la dernière
            for (int line = 0; line < (Lines.Count - 1); line++)
            {
                //Si il ne commence pas par un espace, par un caractère spécial ou une balise
                if (!(Lines[line].StartWithWhiteSpace()
                   || Lines[line].StartsWith("@")
                   || Lines[line].StartsWith("*")
                   || Lines[line].StartsWith(";")
                   || Lines[line].StartsWith("/")
                   || Lines[line].StartsWith("[resettime]")))
                {
                    //Si la ligne n'est pas vide, on lui ajoute deux espaces
                    if (!String.IsNullOrEmpty(Lines[line]))
                        Lines[line] = Lines[line].Insert(0, "  ");

                    result.Add(line);
                }
            }

            return result;
        }

        /// <summary>
        /// Remplace les lignes de texte d'un fichier scripté 
        /// par les lignes de texte d'un fichier nettoyé.
        /// </summary>
        /// <param name="MyFile">Fichier scripté.</param>
        /// <param name="TranslatedFile">Fichier nettoyé.</param>
        public static void ReplaceText(KsFile MyFile, KsFile TranslatedFile)
        {
            //On récupère toutes les lignes de texte du fichier nettoyé (logiquement celui traduit)
            List<string> TranslatedLines = TranslatedFile.Lines.FindAll(tl => !tl.IsTechnical());

            //Tant qu'il reste des lignes du fichier scripté à lire
            //ET des lignes de traduction à recopier
            for (int line = 0, translatedLine = 0;
                (line < MyFile.Lines.Count) && (translatedLine < TranslatedLines.Count); line++)
            {
                //Si la ligne du fichier scripté est une ligne de texte
                if (!MyFile.Lines[line].IsTechnical())
                {
                    //On la remplace par la ligne traduite
                    MyFile.Lines[line] = TranslatedLines[translatedLine];
                    //et on passe a la ligne de traduction suivante, prête à être recopier
                    translatedLine++;
                }
            }
        }
    }
}
