using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace KagEditor.WpfCore
{
    /// <summary>
    /// Enum listant les encodages supporté à l'écriture et à la lecture.
    /// </summary>
    public enum SupportedEncoding
    {
        None = 0,
        Japanese = 932,
        Unicode = 1200
    }

    /// <summary>
    /// Gère un fichier Kirikiri Script et son contenu.
    /// </summary>
    public class KsFile : INotifyPropertyChanged
    {
        #region Properties
        //Alors ci dessous vous pouvez admirer une superbe implémentation de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));

        private string textString;
        /// <summary>
        /// Le contenu du Fichier.
        /// </summary>
        public string TextString
        {
            get => textString;
            set
            {
                if (value != this.textString)
                {
                    this.textString = value;
                    NotifyPropertyChanged("TextString");
                }
            }
        }

        private string nameFile;
        /// <summary>
        /// Le nom avec chemin complet du Fichier.
        /// </summary>
        public string NameFile
        {
            get => nameFile;
            set
            {
                if (value != this.nameFile)
                {
                    this.nameFile = value;
                    NotifyPropertyChanged("NameFile");
                }
            }
        }

        /// <summary>
        /// Ensemble des lignes composants le Fichier.
        /// </summary>
        public List<string> Lines { get; set; }

        /// <summary>
        /// Ensemble des pages composants le Fichier.
        /// </summary>
        public List<string> Pages { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructeur pour ouvrir un fichier Kirikiri Script.
        /// </summary>
        /// <param name="file">Nom complet du fichier à ouvrir.</param>
        /// <param name="encoding">Encodage à utiliser pour la lecture.</param>
        public KsFile(string file, SupportedEncoding encoding)
        {
            //On affecte le nom du fichier
            NameFile = file;

            //On utilise un StreamReader pour lire le fichier avec l'encodage spécifié
            using (StreamReader sr = new StreamReader(file, Encoding.GetEncoding((int)encoding)))
            {
                //On lit tout le fichier
                TextString = sr.ReadToEnd();
            }
        }
        /// <summary>
        /// Constructeur par copie.
        /// </summary>
        public KsFile(KsFile file)
        {
            NameFile = file.NameFile;
            TextString = file.TextString;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sauvegarde le contenu d'un Fichier.
        /// </summary>
        /// <param name="fileName">Nom complet du fichier où sauvegarder.</param>
        /// <param name="encoding">Encodage à utiliser pour l'écriture.</param>
        /// <returns></returns>
        public void Save(string fileName, SupportedEncoding encoding)
        {
            //On verifie que le chemin existe
            if (!Directory.Exists(fileName))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }

            //On initialise un FileStream avec les options qu'on désire
            using (FileStream fs = new FileStream(fileName,
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                //On utilise un StreamWriter pour écrire le fichier avec l'encodage spécifié
                using (StreamWriter sw = new StreamWriter(fs,
                    Encoding.GetEncoding((int)encoding)))
                {
                    sw.Write(TextString);
                }
            }
        }

        /// <summary>
        /// Efface tous les scripts, balises et commentaires du Fichier.
        /// </summary>
        public void Clean(bool isRealtaNua)
        {
            this.EraseCommentedScript();
            this.EraseScript();
            this.EraseLR(isRealtaNua);
            this.EraseWrap();
        }

        /// <summary>
        /// Transforme la chaine de texte d'un Fichier en la liste de lignes.
        /// </summary>
        public void MakeList()
        {
            Lines = new List<string>();
            this.EraseSlashR();
            Lines.AddRange(TextString.Split('\n'));
            Lines.RemoveAll(line => String.IsNullOrWhiteSpace(line));
        }

        /// <summary>
        /// Transforme la liste de lignes en la chaine de texte.
        /// </summary>
        public void MakeStringFromLines()
        {
            StringBuilder constructString = new StringBuilder();
            Lines.ForEach(line => constructString.AppendLine(line));
            TextString = constructString.ToString();
        }

        /// <summary>
        /// Transforme la chaine de texte d'un Fichier en la liste de pages.
        /// </summary>
        public void MakePage()
        {
            Pages = FctRegex.SplitPage(TextString);

            if (!Pages.Any()) return;

            if (String.IsNullOrWhiteSpace(Pages[0])) Pages.RemoveAt(0);
            if (String.IsNullOrWhiteSpace(Pages[Pages.Count - 1])) Pages.RemoveAt(Pages.Count - 1);
        }

        /// <summary>
        /// Transforme la liste de pages en la chaine de texte.
        /// </summary>
        public void MakeStringFromPages()
        {
            StringBuilder constructString = new StringBuilder();
            constructString.AppendLine("*page0|&f.scripttitle");

            for (int i = 0; i < Pages.Count; i++)
            {
                constructString.Append(Pages[i]);
                constructString.AppendLine("*page" + (i + 1) + "|");
            }

            TextString = constructString.ToString();
        }
        #endregion
    }
}
