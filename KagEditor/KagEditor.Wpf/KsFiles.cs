using System;
using System.Collections.Generic;
using System.IO;

namespace KagEditor.Wpf
{
    /// <summary>
    /// Gère l'ouverture d'une liste de Fichiers.
    /// </summary>
    public class KsFiles : List<KsFile>
    {
        /// <summary>
        /// Constructeur pour ouvrir tous les fichiers Kirikiri Script d'un répertoire.
        /// </summary>
        /// <param name="nameDirectory">Nom complet du répertoire à ouvrir.</param>
        public KsFiles(string nameDirectory, SupportedEncoding encoding)
        {
            //Si on a chemin vers un dossier
            if (!String.IsNullOrWhiteSpace(nameDirectory))
            {
                //On ouvre tous les fichiers avec l'extension .ks, y compris dans les sous-répertoires
                foreach (string fileName in Directory.EnumerateFiles(nameDirectory, "*.ks", SearchOption.AllDirectories))
                {
                    this.Add(new KsFile(fileName, encoding));
                }
            }
        }
        /// <summary>
        /// Constructeur par copie.
        /// </summary>
        /// <param name="nameDirectory">Nom complet du répertoire à ouvrir.</param>
        public KsFiles(IEnumerable<KsFile> files)
        {
            //On recopie chaque fichier
            foreach (KsFile file in files)
            {
                this.Add(new KsFile(file));
            }
        }
    }
}
