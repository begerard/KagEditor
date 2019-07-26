using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using KagEditor.Wpf.Utils;

namespace KagEditor.Wpf
{
    public class ViewModel : DependencyObject
    {
        #region DependencyProperty
        /// <summary>
        /// Fichier à afficher
        /// </summary>
        public KsFile CurrentFile
        {
            get { return (KsFile)GetValue(CurrentFileProperty); }
            set { SetValue(CurrentFileProperty, value); }
        }
        public static readonly DependencyProperty CurrentFileProperty =
            DependencyProperty.Register("CurrentFile", typeof(KsFile),
            typeof(ViewModel), new UIPropertyMetadata(null));

        /// <summary>
        /// Détermine si un thread parallèle est entrain de tourner
        /// </summary>
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool),
            typeof(ViewModel), new UIPropertyMetadata(false));

        /// <summary>
        /// Status du thread parallèle
        /// </summary>
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string),
            typeof(ViewModel), new UIPropertyMetadata(null));

        /// <summary>
        /// Encodage utilisé lors de la lecture de fichier .ks
        /// </summary>
        public SupportedEncoding OpenEncoding
        {
            get { return (SupportedEncoding)GetValue(OpenEncodingProperty); }
            set { SetValue(OpenEncodingProperty, value); }
        }
        public static readonly DependencyProperty OpenEncodingProperty =
            DependencyProperty.Register("OpenEncoding", typeof(SupportedEncoding),
            typeof(ViewModel), new UIPropertyMetadata(SupportedEncoding.Unicode));

        /// <summary>
        /// Encodage utilisé lors de l'écriture de fichier .ks
        /// </summary>
        public SupportedEncoding SaveEncoding
        {
            get { return (SupportedEncoding)GetValue(SaveEncodingProperty); }
            set { SetValue(SaveEncodingProperty, value); }
        }
        public static readonly DependencyProperty SaveEncodingProperty =
            DependencyProperty.Register("SaveEncoding", typeof(SupportedEncoding),
            typeof(ViewModel), new UIPropertyMetadata(SupportedEncoding.Unicode));

        /// <summary>
        /// Active le support pour al version PC de Realta Nua.
        /// </summary>
        public bool IsRealtaNua
        {
            get { return (bool)GetValue(IsRealtaNuaProperty); }
            set { SetValue(IsRealtaNuaProperty, value); }
        }
        public static readonly DependencyProperty IsRealtaNuaProperty =
            DependencyProperty.Register("IsRealtaNua", typeof(bool),
            typeof(ViewModel), new UIPropertyMetadata(false));
        #endregion

        #region Commands
        //Les commandes bindable à l'UI
        public ICommand DragOverCommand { get; set; }
        public ICommand DropCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand CleanFileAllCommand { get; set; }
        public ICommand CleanFileWrapCommand { get; set; }
        public ICommand CleanFileLRCommand { get; set; }
        public ICommand CleanFileVoicesCommand { get; set; }
        public ICommand CleanFileCommentCommand { get; set; }
        public ICommand AddWordWrapCommand { get; set; }
        public ICommand AddLRCommand { get; set; }
        public ICommand AddBlankSpaceCommand { get; set; }
        public ICommand ReplaceApostropheCommand { get; set; }
        public ICommand ReplaceDoubleQuoteCommand { get; set; }
        public ICommand ScriptFileCommand { get; set; }
        public ICommand RetranslateDirectoryCommand { get; set; }
        public ICommand WordCountDirectoryCommand { get; set; }
        public ICommand CleanDirectoryAllCommand { get; set; }
        public ICommand CleanDirectoryVoicesCommand { get; set; }
        public ICommand CleanDirectoryLRCommand { get; set; }
        public ICommand ScriptDirectoryCommand { get; set; }
        public ICommand RepackXP3Command { get; set; }
        public ICommand HelpCommand { get; set; }
        public ICommand AboutCommand { get; set; }

        public ViewModel()
        {
            this.DragOverCommand = new RelayCommand(exec => FileDragOver(exec as DragEventArgs));
            this.DropCommand = new RelayCommand(exec => FileDrop(exec as DragEventArgs));
            this.OpenCommand = new RelayCommand(exec => Open());
            this.SaveCommand = new RelayCommand(exec => Save(),
                canExec => CurrentFile != null);
            this.CloseCommand = new RelayCommand(exec => Close(),
                canExec => CurrentFile != null);
            this.CleanFileAllCommand = new RelayCommand(exec => CurrentFile.Clean(IsRealtaNua),
                canExec => CurrentFile != null);
            this.CleanFileWrapCommand = new RelayCommand(exec => CurrentFile.EraseWrap(),
                canExec => CurrentFile != null);
            this.CleanFileLRCommand = new RelayCommand(exec => CurrentFile.EraseLR(IsRealtaNua),
                canExec => CurrentFile != null);
            this.CleanFileVoicesCommand = new RelayCommand(exec => CurrentFile.EraseSay(),
                canExec => CurrentFile != null);
            this.CleanFileCommentCommand = new RelayCommand(exec => CurrentFile.EraseComment(),
                canExec => CurrentFile != null);
            this.AddWordWrapCommand = new RelayCommand(exec => this.AddWordWrap(),
                canExec => CurrentFile != null);
            this.AddLRCommand = new RelayCommand(exec => this.AddLR(),
                canExec => CurrentFile != null);
            this.AddBlankSpaceCommand = new RelayCommand(exec => this.AddBlankSpace(),
                canExec => CurrentFile != null);
            this.ReplaceApostropheCommand = new RelayCommand(exec => CurrentFile.ReplaceApostrophe(),
                canExec => CurrentFile != null);
            this.ReplaceDoubleQuoteCommand = new RelayCommand(exec => CurrentFile.ReplaceDoubleQuote(),
                canExec => CurrentFile != null);
            this.ScriptFileCommand = new RelayCommand(exec => this.ScriptFile(),
                canExec => CurrentFile != null);
            this.RetranslateDirectoryCommand = new RelayCommand(exec => RetranslateDirectory());
            this.WordCountDirectoryCommand = new RelayCommand(exec => WordCountDirectory());
            this.CleanDirectoryAllCommand = new RelayCommand(exec => CleanDirectoryAll());
            this.CleanDirectoryVoicesCommand = new RelayCommand(exec => CleanDirectoryVoices());
            this.CleanDirectoryLRCommand = new RelayCommand(exec => CleanDirectoryLR());
            this.ScriptDirectoryCommand = new RelayCommand(exec => ScriptDirectory());
            this.HelpCommand = new RelayCommand(exec => Help());
            this.AboutCommand = new RelayCommand(exec => About());
        }
        #endregion

        #region Menu File
        /// <summary>
        /// Ouvre un fichier .ks
        /// </summary>
        public void Open()
        {
            //On appelle la fonction qui gère le Form d'ouverture de fichier
            string nameFile = OpenFile("Open a KS file", PersistentConfig.OpenFilePhysicalLocation);

            if (nameFile != null)
            {
                try
                {
                    //On essaye de lire le fichier
                    CurrentFile = new KsFile(nameFile, OpenEncoding);

                    //On sauvegarde le chemin d'accès dans le fichier de conf
                    PersistentConfig.OpenFilePhysicalLocation = CurrentFile.NameFile;
                }
                catch
                {
                    //Si ça foire, on le dit
                    MessageBox.Show("Cannot read this file");
                }
            }
        }

        /// <summary>
        /// Sauvegarde le fichier .ks courant
        /// </summary>
        public void Save()
        {
            //On initialise un Form de sauvegarde avec le bon type de fichier
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "Kirikiri Script|*.ks";
            sfd.FileName = Path.GetFileName(CurrentFile.NameFile);
            sfd.InitialDirectory = Directory.GetParent(CurrentFile.NameFile).FullName;
            sfd.AddExtension = true;

            //On affiche le Form de sauvegarde et si l'utilisateur selectionne un emplacement de sauvegarde
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    //On essaie de sauvegarder
                    CurrentFile.Save(sfd.FileName, SaveEncoding);
                }
                catch (Exception)
                {
                    //Si ça foire, on le dit
                    MessageBox.Show("Cannot save this file.");
                }
            }
        }

        /// <summary>
        /// Ferme le fichier courant
        /// </summary>
        public void Close()
        {
            CurrentFile = null;
        }
        #endregion

        #region Menu Edit
        /// <summary>
        /// Ajoute des balises "[wrap text="..."]" à un fichier
        /// </summary>
        public void AddWordWrap()
        {
            //On transforme le texte du Fichier en un tableau de lignes pour son traitement
            CurrentFile.MakeList();
            //On ajoute les balises [wrap text="..."] à ces lignes
            Scripting.DoWordWrap(CurrentFile.Lines);
            //On retransforme le tableau de lignes en un texte sur une seule chaine de caractère
            CurrentFile.MakeStringFromLines();
        }

        /// <summary>
        /// Ajoute des balises "[l][r]" à un fichier
        /// </summary>
        public void AddLR()
        {
            //On transforme le texte du Fichier en un tableau de lignes pour son traitement
            CurrentFile.MakeList();
            //On ajoute les balises "[l][r]" à ces lignes
            Scripting.DoLR(CurrentFile.Lines, IsRealtaNua);
            //On retransforme le tableau de lignes en un texte sur une seule chaine de caractère
            CurrentFile.MakeStringFromLines();
        }

        /// <summary>
        /// Ajoute des espaces blancs là où manifestement il en manque
        /// </summary>
        public void AddBlankSpace()
        {
            //On transforme le texte du Fichier en un tableau de lignes pour son traitement
            CurrentFile.MakeList();
            //On ajoute deux espace aux lignes de text qui n'en ont pas
            List<int> linesModified = Scripting.DoBlankSpace(CurrentFile.Lines);
            //On retransforme le tableau de lignes en un texte sur une seule chaine de caractère
            CurrentFile.MakeStringFromLines();

            string message;
            if (linesModified.Count == 0)
            {
                message = "No missing blank space found.";
            }
            else
            {
                message = $"{linesModified.Count} missing blanks added at lines: ";
                foreach (int line in linesModified)
                {
                    message += $"\n- {line}: {CurrentFile.Lines[line]}";
                }
            }
            MessageBox.Show(message);
        }

        /// <summary>
        /// Scripte un fichier texte traduit à partir du fichier original correspondant
        /// </summary>
        public void ScriptFile()
        {
            //On déclare les Fichiers dont nous aurons besoin
            KsFile TranslatedFile;
            KsFile OriginalFile;
            KsFile TruncatedOriginalFile;

            //On considère que le fichier chargé dans l'appli est le fichier traduit
            TranslatedFile = CurrentFile;

            //Il nous reste donc le fichier original à charger
            string OriginalNameFile = OpenFile("Open your original .ks file", PersistentConfig.OriginalFilePhysicalLocation);
            if (OriginalNameFile == null)
            {
                return;
            }

            try
            {
                OriginalFile = new KsFile(OriginalNameFile, OpenEncoding);
            }
            catch
            {
                //Si ça foire, on le dit et on ignore le fichier ouvert
                MessageBox.Show("Cannot read the Original file");
                TranslatedFile = null;
                return;
            }

            //On sauvegarde le chemin d'accès dans le fichier de conf
            PersistentConfig.OriginalFilePhysicalLocation = OriginalFile.NameFile;

            //On prépare un fichier original sans section trompeuse
            TruncatedOriginalFile = new KsFile(OriginalFile);
            TruncatedOriginalFile.EraseTlaData();

            //On transforme le texte des Fichiers en un tableau de lignes pour les utiliser
            TranslatedFile.MakeList();
            OriginalFile.MakeList();
            TruncatedOriginalFile.MakeList();

            //On calcul le nombre de ligne de texte
            var nbTranslatedLines = TranslatedFile.Lines.Count(line => !line.IsTechnical());
            var nbToTranslateLines = TruncatedOriginalFile.Lines.Count(line => !line.IsTechnical());

            //On vérifie que les nombres de lignes de texte traduit/à traduire correspondent,
            //cela permet de prévenir la plupart des mauvaises associations de fichiers,
            //car chaque fichier a un nombre variable de lignes à traduire.
            if (nbTranslatedLines != nbToTranslateLines)
            {
                var msg = $"Translated file have {nbTranslatedLines} lines of text whereas"
                    + $" Original file have {nbToTranslateLines} lines of text.\nDo you want to continue?";
                if (MessageBox.Show(msg, "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning)
                    == MessageBoxResult.Cancel)
                {
                    //On se casse en cas de Cancel
                    return;
                }
            }

            //On rajoute les balises neccessaires au bonne affichage du texte dans le jeu
            Scripting.DoLR(TranslatedFile.Lines, IsRealtaNua);
            //On remplace les lignes originales par celles traduites et balisées
            Scripting.ReplaceText(OriginalFile, TranslatedFile);

            //On retransforme le tableau de lignes en un texte sur une seule chaine de caractère
            OriginalFile.MakeStringFromLines();
            //On affiche le tout
            CurrentFile = OriginalFile;
        }
        #endregion

        #region Menu Directory
        /// <summary>
        /// Nettoie les fichiers scripts, contenu dans un répertoire,
        /// de tous les types de scripts et balises.
        /// </summary>
        public void CleanDirectoryAll()
        {
            #region Init
            //On appelle la fonction d'ouverture de dossier pour obtenir un chemin
            string cheminToClean = OpenDirectory("Select the directory to clean.",
                PersistentConfig.OriginalFolderPhysicalLocation);
            if (cheminToClean == null)
            {
                return;
            }

            string cheminDestination = OpenDirectory("Select the directory where save Cleaned files",
                PersistentConfig.CleanedFolderPhysicalLocation);
            if (cheminDestination == null)
            {
                return;
            }

            //On sauvegarde les chemins d'accès dans le fichier de conf
            PersistentConfig.OriginalFolderPhysicalLocation = cheminToClean;
            PersistentConfig.CleanedFolderPhysicalLocation = cheminDestination;

            //Code qui dure, donc on le met dans un autre thread
            var worker = new System.ComponentModel.BackgroundWorker();
            worker.WorkerReportsProgress = true;
            var openEncoding = OpenEncoding;
            var saveEncoding = SaveEncoding;
            var isRealtaNua = IsRealtaNua;

            worker.ProgressChanged += (progressSender, progressArg) =>
            {
                Status = progressArg.UserState as string;
            };

            worker.RunWorkerCompleted += (completedSender, completedArg) =>
            {
                IsBusy = false;

                //Si on a eu une exception pendant le traitement
                if (completedArg.Error != null)
                {
                    //On le dit
                    MessageBox.Show(ErrorMessage(completedArg.Error.Message));
                }
                else if (completedArg.Result != null)
                {
                    //Sinon on affiche le résultat
                    MessageBox.Show($"In this directory, {(int)completedArg.Result} .ks files have been cleaned successfully.");
                }
            };
            #endregion

            #region Threading
            worker.DoWork += (workSender, workArg) =>
            {
                worker.ReportProgress(0, "Opening files...");

                //On ouvre les fichiers de ce dossier
                KsFiles ToCleanFiles = new KsFiles(cheminToClean, openEncoding);

                worker.ReportProgress(0, "Cleaning...");

                //Pour chaque Fichier
                for (int i = 0; i < ToCleanFiles.Count; i++)
                {
                    //On appelle la fonction de nettoyage complète
                    ToCleanFiles[i].Clean(isRealtaNua);

                    //On sauvegarde le fichier modifié
                    ToCleanFiles[i].Save(ToCleanFiles[i].NameFile.Replace
                        (cheminToClean, cheminDestination), saveEncoding);
                }

                workArg.Result = ToCleanFiles.Count;
            };
            #endregion

            IsBusy = true;
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Nettoie les fichiers scripts, contenu dans un répertoire,
        /// de tous les scripts de voix.
        /// </summary>
        public void CleanDirectoryVoices()
        {
            #region Init
            //On appelle la fonction d'ouverture de dossier pour obtenir un chemin
            string cheminToClean = OpenDirectory("Select the directory to clean.",
                PersistentConfig.OriginalFolderPhysicalLocation);
            if (cheminToClean == null)
            {
                return;
            }

            string cheminDestination = OpenDirectory("Select the directory where save Cleaned files");
            if (cheminDestination == null)
            {
                return;
            }

            //On sauvegarde les chemins d'accès dans le fichier de conf
            PersistentConfig.OriginalFolderPhysicalLocation = cheminToClean;

            //Code qui dure, donc on le met dans un autre thread
            var worker = new System.ComponentModel.BackgroundWorker();
            worker.WorkerReportsProgress = true;
            var openEncoding = OpenEncoding;
            var saveEncoding = SaveEncoding;

            worker.ProgressChanged += (progressSender, progressArg) =>
            {
                Status = progressArg.UserState as string;
            };

            worker.RunWorkerCompleted += (completedSender, completedArg) =>
            {
                IsBusy = false;

                //Si on a eu une exception pendant le traitement
                if (completedArg.Error != null)
                {
                    //On le dit
                    MessageBox.Show(ErrorMessage(completedArg.Error.Message));
                }
                else if (completedArg.Result != null)
                {
                    //Sinon on affiche le résultat
                    MessageBox.Show($"In this directory, {(int)completedArg.Result} .ks files have been cleaned successfully.");
                }
            };
            #endregion

            #region Threading
            worker.DoWork += (workSender, workArg) =>
            {
                worker.ReportProgress(0, "Opening files...");

                //On ouvre les fichiers de ce dossier
                KsFiles ToCleanFiles = new KsFiles(cheminToClean, openEncoding);

                worker.ReportProgress(0, "Cleaning...");

                //Pour chaque Fichier
                for (int i = 0; i < ToCleanFiles.Count; i++)
                {
                    //On appelle la fonction de nettoyage des voix
                    ToCleanFiles[i].EraseSay();

                    //On sauvegarde le fichier modifié
                    ToCleanFiles[i].Save(ToCleanFiles[i].NameFile.Replace
                        (cheminToClean, cheminDestination), saveEncoding);
                }

                workArg.Result = ToCleanFiles.Count;
            };
            #endregion

            IsBusy = true;
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Nettoie les fichiers scripts, contenu dans un répertoire, de tous les LR.
        /// </summary>
        public void CleanDirectoryLR()
        {
            #region Init
            //On appelle la fonction d'ouverture de dossier pour obtenir un chemin
            string cheminToClean = OpenDirectory("Select the directory to clean.",
                PersistentConfig.OriginalFolderPhysicalLocation);
            if (cheminToClean == null)
            {
                return;
            }

            string cheminDestination = OpenDirectory("Select the directory where save Cleaned files");
            if (cheminDestination == null)
            {
                return;
            }

            //On sauvegarde les chemins d'accès dans le fichier de conf
            PersistentConfig.OriginalFolderPhysicalLocation = cheminToClean;

            //Code qui dure, donc on le met dans un autre thread
            var worker = new System.ComponentModel.BackgroundWorker();
            worker.WorkerReportsProgress = true;
            var openEncoding = OpenEncoding;
            var saveEncoding = SaveEncoding;
            var isRealtaNua = IsRealtaNua;

            worker.ProgressChanged += (progressSender, progressArg) =>
            {
                Status = progressArg.UserState as string;
            };

            worker.RunWorkerCompleted += (completedSender, completedArg) =>
            {
                IsBusy = false;

                //Si on a eu une exception pendant le traitement
                if (completedArg.Error != null)
                {
                    //On le dit
                    MessageBox.Show(ErrorMessage(completedArg.Error.Message));
                }
                else if (completedArg.Result != null)
                {
                    //Sinon on affiche le résultat
                    MessageBox.Show($"In this directory, {(int)completedArg.Result} .ks files have been cleaned successfully.");
                }
            };
            #endregion

            #region Threading
            worker.DoWork += (workSender, workArg) =>
            {
                worker.ReportProgress(0, "Opening files...");

                //On ouvre les fichiers de ce dossier
                KsFiles ToCleanFiles = new KsFiles(cheminToClean, openEncoding);

                worker.ReportProgress(0, "Cleaning...");

                //Pour chaque Fichier
                for (int i = 0; i < ToCleanFiles.Count; i++)
                {
                    //On appelle la fonction de nettoyage des LR
                    ToCleanFiles[i].EraseLR(isRealtaNua);

                    //On sauvegarde le fichier modifié
                    ToCleanFiles[i].Save(ToCleanFiles[i].NameFile.Replace
                        (cheminToClean, cheminDestination), saveEncoding);
                }

                workArg.Result = ToCleanFiles.Count;
            };
            #endregion

            IsBusy = true;
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Compte les mots des fichiers de scénario, contenu dans un répertoire.
        /// </summary>
        public void WordCountDirectory()
        {
            #region Init
            //On appelle la fonction d'ouverture de dossier pour obtenir un chemin
            string chemin = OpenDirectory("Select the directory to word-count.",
                PersistentConfig.CountFolderPhysicalLocation);

            //On sauvegarde les chemins d'accès dans le fichier de conf
            PersistentConfig.CountFolderPhysicalLocation = chemin;

            //Code qui dure, donc on le met dans un autre thread
            var worker = new System.ComponentModel.BackgroundWorker();
            worker.WorkerReportsProgress = true;
            var openEncoding = OpenEncoding;
            var isRealtaNua = IsRealtaNua;

            worker.ProgressChanged += (progressSender, progressArg) =>
            {
                Status = progressArg.UserState as string;
            };

            worker.RunWorkerCompleted += (completedSender, completedArg) =>
            {
                IsBusy = false;

                //Si on a eu une exception pendant le traitement
                if (completedArg.Error != null)
                {
                    //On le dit
                    MessageBox.Show(ErrorMessage(completedArg.Error.Message));
                }
                else if (completedArg.Result != null)
                {
                    (int files, int pages, int lines, int words) = (Tuple<int, int, int, int>)completedArg.Result;
                    //Sinon on affiche le résultat
                    MessageBox.Show($"In this directory, {files} files have been count successfully."
                        + $"\nThey have {pages} pages, {lines} lines and {words} words.");
                }
            };
            #endregion

            #region Threading
            worker.DoWork += (workSender, workArg) =>
            {
                worker.ReportProgress(0, "Opening files...");

                KsFiles ToCountFiles = new KsFiles(chemin, openEncoding);
                int nbPages = 0;
                int nbLines = 0;
                int nbWords = 0;

                worker.ReportProgress(0, "Counting...");

                //Pour chaque Fichier
                for (int i = 0; i < ToCountFiles.Count; i++)
                {
                    //On nettoye le fichier
                    ToCountFiles[i].Clean(isRealtaNua);

                    //On compte le nombre de pages
                    nbPages += ToCountFiles[i].CountPage();

                    //On vire les "*pageX|" et les "[lineX]"
                    ToCountFiles[i].ErasePage();
                    ToCountFiles[i].EraseLineX();

                    //On compte le nombre de ligne de texte
                    ToCountFiles[i].MakeList();
                    nbLines += (ToCountFiles[i].Lines.Count - 1);

                    //On compte le nombre de mot
                    nbWords += ToCountFiles[i].CountWord();
                }

                workArg.Result = (ToCountFiles.Count, nbPages, nbLines, nbWords);
            };
            #endregion

            IsBusy = true;
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Script les fichiers traduits, contenu dans un répertoire,
        /// à partir des fichiers originaux correspondant, contenu dans un autre répertoire.
        /// </summary>
        public void ScriptDirectory()
        {
            #region Init
            //On appelle la fonction d'ouverture de dossier pour obtenir les chemins
            string cheminTraduits = OpenDirectory("Select the directory which contains Translated files",
                PersistentConfig.TranslatedFolderPhysicalLocation);
            if (cheminTraduits == null)
            {
                return;
            }

            string cheminOriginaux = OpenDirectory("Select the directory which contains Original files",
                PersistentConfig.OriginalFolderPhysicalLocation);
            if (cheminOriginaux == null)
            {
                return;
            }


            string cheminDestination = OpenDirectory("Select the directory where save Rescripted files",
                PersistentConfig.RescriptedFolderPhysicalLocation);
            if (cheminDestination == null)
            {
                return;
            }

            //On sauvegarde les chemins d'accès dans le fichier de conf
            PersistentConfig.TranslatedFolderPhysicalLocation = cheminTraduits;
            PersistentConfig.OriginalFolderPhysicalLocation = cheminOriginaux;
            PersistentConfig.RescriptedFolderPhysicalLocation = cheminDestination;

            //Code qui dure, donc on le met dans un autre thread
            var worker = new System.ComponentModel.BackgroundWorker();
            worker.WorkerReportsProgress = true;
            var openEncoding = OpenEncoding;
            var saveEncoding = SaveEncoding;
            var isRealtaNua = IsRealtaNua;

            worker.ProgressChanged += (progressSender, progressArg) =>
            {
                Status = progressArg.UserState as string;
            };

            worker.RunWorkerCompleted += (completedSender, completedArg) =>
            {
                IsBusy = false;

                //Si on a eu une exception pendant le traitement
                if (completedArg.Error != null)
                {
                    //On le dit
                    MessageBox.Show(ErrorMessage(completedArg.Error.Message));
                }
                else if (completedArg.Result != null)
                {
                    //Sinon on affiche le resultat
                    MessageBox.Show($"In this directory, {(int)completedArg.Result} .ks files have been rescripted successfully.");
                }
            };
            #endregion

            #region Threading
            worker.DoWork += (workSender, workArg) =>
            {
                worker.ReportProgress(0, "Opening files...");

                //On ouvre les fichiers de ces dossiers
                KsFiles TranslatedFiles = new KsFiles(cheminTraduits, openEncoding);
                KsFiles OriginalFiles = new KsFiles(cheminOriginaux, openEncoding);

                worker.ReportProgress(0, "Scripting...");

                //Pour chaque Fichier
                for (int i = 0; i < TranslatedFiles.Count; i++)
                {
                    KsFile originalFile = (from of in OriginalFiles
                                           where Path.GetFileName(of.NameFile) == Path.GetFileName(TranslatedFiles[i].NameFile)
                                           select of).FirstOrDefault();

                    if (originalFile == null)
                    {
                        MessageBox.Show($"Cannot find an original file for the translated file {TranslatedFiles[i].NameFile}.");

                        //On stop tout
                        return;
                    }

                    //On prépare un fichier original sans section trompeuse
                    KsFile truncatedOriginalFile = new KsFile(originalFile);
                    truncatedOriginalFile.EraseTlaData();

                    //On transforme le texte des Fichiers en un tableau de lignes pour les utiliser
                    TranslatedFiles[i].MakeList();
                    originalFile.MakeList();
                    truncatedOriginalFile.MakeList();

                    //On calcul le nombre de ligne de texte
                    var nbTranslatedLines = TranslatedFiles[i].Lines.Count(line => !line.IsTechnical());
                    var nbToTranslateLines = truncatedOriginalFile.Lines.Count(line => !line.IsTechnical());

                    //On vérifie que le nombre de ligne de texte traduit/à traduire correspondent,
                    //cela permet de prévenir la plupart des mauvaises associations de fichiers,
                    //car chaque fichier a un nombre variable de lignes à traduire.
                    if (nbTranslatedLines != nbToTranslateLines)
                    {
                        var msg = $"Translated file {TranslatedFiles[i].NameFile} have {nbTranslatedLines} lines"
                        + $" of text whereas Original file {originalFile.NameFile} have {nbToTranslateLines} lines"
                        + $" of text.\nDo you want to continue?";

                        if (MessageBox.Show(msg, "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning)
                            == MessageBoxResult.Cancel)
                        {
                            //On se casse en cas de Cancel
                            return;
                        }
                    }

                    //On rajoute les balises neccessaire au bonne affichage du texte dans le jeu
                    Scripting.DoLR(TranslatedFiles[i].Lines, isRealtaNua);
                    //On remplace les lignes originales par celles traduites et balisées
                    Scripting.ReplaceText(originalFile, TranslatedFiles[i]);

                    //On retransforme le tableau de lignes en un texte sur une seule chaine de caractère
                    originalFile.MakeStringFromLines();
                    //On sauvegarde le fichier modifié là où il faut
                    originalFile.Save(TranslatedFiles[i].NameFile.Replace(
                        cheminTraduits, cheminDestination), saveEncoding);
                }

                workArg.Result = TranslatedFiles.Count;
            };
            #endregion

            IsBusy = true;
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Traduit toutes les lignes de tous les fichiers d'un répertoire
        /// à partir de deux répertoires avant/après traduction de référence.
        /// </summary>
        public void RetranslateDirectory()
        {
            #region Init
            //On appelle la fonction d'ouverture de dossier pour obtenir les chemins
            string cheminTraduits = OpenDirectory("Select the directory which contains Translated files",
                PersistentConfig.TranslatedFolderPhysicalLocation);
            if (cheminTraduits == null)
            {
                return;
            }

            string cheminOriginaux = OpenDirectory("Select the directory which contains Original files",
                PersistentConfig.OriginalFolderPhysicalLocation);
            if (cheminOriginaux == null)
            {
                return;
            }

            string cheminATraduire = OpenDirectory("Select the directory which contains the files to Translate",
                PersistentConfig.ToTranslateFolderPhysicalLocation);
            if (cheminATraduire == null)
            {
                return;
            }

            string cheminDestination = OpenDirectory("Select the directory where save Retranslated files",
                PersistentConfig.RetranslatedFolderPhysicalLocation);
            if (cheminDestination == null)
            {
                return;
            }

            //On sauvegarde les chemins d'accès dans le fichier de conf
            PersistentConfig.TranslatedFolderPhysicalLocation = cheminTraduits;
            PersistentConfig.OriginalFolderPhysicalLocation = cheminOriginaux;
            PersistentConfig.ToTranslateFolderPhysicalLocation = cheminATraduire;
            PersistentConfig.RetranslatedFolderPhysicalLocation = cheminDestination;

            //Code qui dure, donc on le met dans un autre thread
            var worker = new System.ComponentModel.BackgroundWorker();
            worker.WorkerReportsProgress = true;
            var openEncoding = OpenEncoding;
            var saveEncoding = SaveEncoding;

            worker.ProgressChanged += (progressSender, progressArg) =>
            {
                Status = progressArg.UserState as string;
            };

            worker.RunWorkerCompleted += (completedSender, completedArg) =>
            {
                IsBusy = false;

                //Si on a eu une exception pendant le traitement
                if (completedArg.Error != null)
                {
                    //On le dit
                    MessageBox.Show(ErrorMessage(completedArg.Error.Message));
                }
                //Sinon tout a marché et on affiche le résultat
                else if (completedArg.Result != null)
                {
                    (int success, int reference, int pages) = (Tuple<int, int, int>)completedArg.Result;
                    MessageBox.Show($"{success} pages retranslated and {reference} referenced for {pages} pages with no match.");
                }
            };
            #endregion

            #region Process
            worker.DoWork += (workSender, workArg) =>
            {
                worker.ReportProgress(0, "Opening Files...");

                //On ouvre les fichiers de ces dossiers
                KsFiles TranslatedFiles = new KsFiles(cheminTraduits, openEncoding);
                KsFiles OriginalFiles = new KsFiles(cheminOriginaux, openEncoding);
                KsFiles ToTranslateFiles = new KsFiles(cheminATraduire, openEncoding);

                worker.ReportProgress(0, "Indexing translated pages...");

                var translatedPagesDictionary = new Dictionary<string, string>();
                //Pour chaque Fichier traduit
                foreach (KsFile translatedFile in TranslatedFiles)
                {
                    KsFile originalFile = OriginalFiles.FirstOrDefault(of => Path.GetFileName(of.NameFile)
                        == Path.GetFileName(translatedFile.NameFile));
                    if (originalFile == null)
                    {
                        MessageBox.Show($"Cannot find an original file for the translated file {translatedFile.NameFile}.");

                        //On stop tout si on a pas trouvé de fichier original correspondant à un fichier traduit
                        return;
                    }

                    translatedFile.MakePage();
                    originalFile.MakePage();

                    //On vérifie que le nombre de pages de texte traduit/à traduire correspondent,
                    //cela permet de prévenir la plus part des mauvaises associations de fichiers,
                    //car chaque fichier a un nombre variable de pages à traduire
                    if ((translatedFile.Pages.Count) != (originalFile.Pages.Count))
                    {
                        MessageBoxResult result2 = MessageBox.Show(
                            $"The Translated file {translatedFile.NameFile} have {translatedFile.Pages.Count} pages"
                            + $"of text whereas Original file {originalFile.NameFile} have {originalFile.Pages.Count}"
                            + $"pages of text.\nDo you want to force the retranslation?",
                            "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        switch (result2)
                        {
                            case MessageBoxResult.No:
                                //On se casse
                                return;
                            case MessageBoxResult.Yes:
                            //On continue comme si de rien n'était
                            default:
                                break;
                        }
                    }

                    //On utilise le plus petit nombre de pages pour itérer les paires
                    var nbPair = translatedFile.Pages.Count > originalFile.Pages.Count ?
                        originalFile.Pages.Count : translatedFile.Pages.Count;
                    var originalPages = FctRegex.CleanPages(originalFile.Pages);

                    //On stock chaque paire de page original/page traduite dans un dictionnaire
                    for (int i = 0; i < nbPair; i++)
                    {
                        if (!originalFile.Pages[i].StartsWith("*")
                            && !translatedPagesDictionary.ContainsKey(originalPages[i]))
                        {
                            translatedPagesDictionary.Add(originalPages[i], translatedFile.Pages[i]);
                        }
                    }
                }

                worker.ReportProgress(0, "Indexing original pages...");

                var referencedPagesDictionnary = new Dictionary<string, string>();
                //Pour chaque fichier original
                foreach (KsFile file in OriginalFiles)
                {
                    if (file.Pages == null)
                        file.MakePage();

                    var originalPages = FctRegex.CleanPages(file.Pages);

                    //On ajoute les pages qui ne sont pas déjà référencé
                    for (int i = 0; i < file.Pages.Count; i++)
                    {
                        //On ajoute les pages qui ne sont pas déjà référencé
                        if (!referencedPagesDictionnary.ContainsKey(originalPages[i]))
                            //Qui ont une taille sinificative
                            if (originalPages[i].Length > 3)
                                referencedPagesDictionnary.Add(originalPages[i],
                                    "page" + i + " of file " + Path.GetFileNameWithoutExtension(file.NameFile));
                    }
                }

                worker.ReportProgress(0, "Retranslating...");

                int nbAllFilePages = 0;
                int nbAllFileSuccess = 0;
                int nbAllFileReference = 0;
                foreach (KsFile file in ToTranslateFiles)
                {
                    int nbSuccess = 0;
                    int nbReference = 0;

                    file.MakePage();
                    var filePages = FctRegex.CleanPages(file.Pages);

                    //On remplace chaque page trouvée dans le dictionnaire par sa version traduite
                    for (int i = 0; i < file.Pages.Count; i++)
                    {
                        if (translatedPagesDictionary.ContainsKey(filePages[i]))
                        {
                            file.Pages[i] = translatedPagesDictionary[filePages[i]];
                            nbSuccess++;
                        }
                        //Pour les pages qui n'ont pas été directement trouvé dans la version original
                        else
                        {
                            //On les cherche dans le contenu des pages originals
                            var match = referencedPagesDictionnary.Where(
                                r => r.Key.Contains(filePages[i])).FirstOrDefault().Value;

                            //Et on les marques avec une référence vers le fichier original
                            if (match != null)
                            {
                                file.Pages[i] += "---> Found in " + match + "\n";
                                nbReference++;
                            }
                            //Pour les pages que l'on a pas non plus trouvé dans une page original
                            else
                            {
                                //On cherche des pages originals en elles
                                var matches = referencedPagesDictionnary.Where(
                                    r => filePages[i].Contains(r.Key)).Select(x => x.Value);

                                if (matches != null && matches.Any())
                                {
                                    foreach (string reverseMatch in matches)
                                    {
                                        file.Pages[i] += "---> Contains " + reverseMatch + "\n";
                                    }
                                    nbReference++;
                                }
                            }
                        }
                    }

                    nbAllFilePages += file.Pages.Count;
                    nbAllFileSuccess += nbSuccess;
                    nbAllFileReference += nbReference;

                    //On reconstruit le fichier
                    file.MakeStringFromPages();
                    //Puis on sauvegarde le fichier modifié là où il faut
                    file.Save(file.NameFile.Replace(cheminATraduire, cheminDestination), saveEncoding);
                }

                workArg.Result = (nbAllFileSuccess, nbAllFileReference, nbAllFilePages - (nbAllFileSuccess + nbAllFileReference));
            };
            #endregion

            IsBusy = true;
            worker.RunWorkerAsync();
        }
        #endregion

        #region Other Menus
        /// <summary>
        /// Ouvre la page web d'aide
        /// </summary>
        public void Help()
        {
            System.Diagnostics.Process.Start("https://github.com/begerard/KagEditor");
        }

        /// <summary>
        /// Affiche une petite fenêtre d'à propos
        /// </summary>
        public void About()
        {
            MessageBox.Show("Version " + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(),
                "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region Other Functions
        /// <summary>
        /// Affiche la fenetre d'ouverture de répertoire
        /// </summary>
        /// <param name="description">Description du répertoire à ouvrir
        /// qui s'affichera en haut de la fenetre</param>
        /// <param name="defaultFolder">Répertoire par defaut où s'ouvrira l'explorateur</param>
        /// <returns>Retourne le nom complet du répertoire si l'utilisateur en sélectionne un,
        /// Sinon null</returns>
        private string OpenDirectory(string description = null, string defaultFolder = null)
        {
            //On initialise le Form d'ouverture de dossier sur le Poste de Travail
            System.Windows.Forms.FolderBrowserDialog odd = new System.Windows.Forms.FolderBrowserDialog();

            odd.RootFolder = Environment.SpecialFolder.Desktop;
            if (!string.IsNullOrEmpty(defaultFolder))
                odd.SelectedPath = defaultFolder;
            odd.Description = description;
            odd.ShowNewFolderButton = true;

            //On affiche le Form
            if (odd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //On vérifie que le chemin récupéré soit correct
                if ((odd.SelectedPath != null) && (odd.SelectedPath.Length > 0))
                {
                    return odd.SelectedPath;
                }
                //Sinon on le dit,
                else
                {
                    MessageBox.Show("Cannot open this directory");
                }
            }
            //et on renvoit null
            return null;
        }

        /// <summary>
        /// Affiche la fenetre d'ouverture de fichier
        /// </summary>
        /// <param name="title">Titre de la fenetre</param>
        /// <param name="physicalLocation">Répertoire par defaut où s'ouvrira l'explorateur</param>
        /// <returns>Retourne le nom complet du fichier si l'utilisateur en sélectionne un,
        /// Sinon null</returns>
        private string OpenFile(string title, string physicalLocation)
        {
            //On initialise le Form d'ouverture de fichier avec des fichiers .ks
            //WinForm.OpenFileDialog ofd = new WinForm.OpenFileDialog();
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();

            ofd.Filter = "Kirikiri Script|*.ks";
            //Si on a un chemin de chargement de fichier dans le fichier de conf
            if (!string.IsNullOrEmpty(physicalLocation))
            {
                //On ouvre le Form là
                ofd.InitialDirectory = Path.GetDirectoryName(physicalLocation);
            }
            //Si on en a pas, on commence au Poste de Travail
            else
            {
                ofd.InitialDirectory = Environment.SpecialFolder.MyComputer.ToString();
            }
            //Ouverture d'un seul fichier, avec vérification d'existence
            ofd.Multiselect = false;
            ofd.CheckFileExists = true;
            //Le titre du Form est reçu en paramètre
            ofd.Title = title;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return ofd.FileName;
            }
            return null;
        }

        /// <summary>
        /// Decide quel effet appliquer à un survole de la zone client
        /// </summary>
        /// <param name="e">Evènement de drop contenant le nom de l'objet en survole</param>
        /// <returns></returns>
        public void FileDragOver(DragEventArgs e)
        {
            //Si il y a un fichier
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                //Si il y a un SEUL fichier
                string[] fileNames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (fileNames.Length == 1)
                {
                    //Si ce fichier existe
                    if (File.Exists(fileNames[0]))
                    {
                        //Si ce fichier est du bon type
                        if (Path.GetExtension(fileNames[0]) == ".ks")
                        {
                            //On a un bon fichier on autorise le drop comme copie
                            e.Effects = DragDropEffects.Copy;
                            e.Handled = true;
                            return;
                        }
                    }
                }
            }
            //Sinon ce n'est pas un bon fichier et on refuse tout drop
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        /// <summary>
        /// Essaye de charger le fichier dropé sur la zone client
        /// </summary>
        /// <param name="e">Evènement de drop contenant le nom de l'objet laché</param>
        public void FileDrop(DragEventArgs e)
        {
            try
            {
                //On essaye d'en créer un en récupérant son nom
                CurrentFile = new KsFile((e.Data.GetData(
                    DataFormats.FileDrop, true) as string[])[0], OpenEncoding);
            }
            catch
            {
                MessageBox.Show("Cannot read this file");
            }
            finally
            {
                e.Handled = true;
            }
        }

        public string ErrorMessage(string msg) => $"An error occured: {msg}";
        #endregion
    }
}
