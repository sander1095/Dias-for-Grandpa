using ChinhDo.Transactions;
using DiasForGrandpa.WPF.Exceptions;
using DiasForGrandpa.WPF.Helpers;
using Mecha.ViewModel.Attributes;
using Syroot.Windows.IO;
using System;
using System.IO;
using System.Linq;
using System.Transactions;

namespace DiasForGrandpa.WPF.ViewModels
{
    public class OrganizerViewModel
    {
        // TODO: Change DiaVolumePath and DiaFolderPath to the correct one. (G:\)
        private const string _diaVolumePath = "D:\\";
        private const string _diaVolumeName = "DIA";

        /// <summary>
        /// The Dia scanner stores the file on this path
        /// </summary>
        private string DiaFolderInputPath => Path.Combine(_diaVolumePath, "DCIM", "100COACH");

        /// <summary>
        /// We move the dia's from the dia volume to this folder.
        /// </summary>
        private string DiaFolderOutputPath => Path.Combine(KnownFolders.Pictures.Path, "Dia's", FolderName);

        private readonly TxFileManager _fileManager = new TxFileManager();

        [Readonly]
        public virtual string Intro =>
            $"Dit programma zal ervoor zorgen dat ingescande dia's {Environment.NewLine}" +
            $"automatisch gesorteerd worden in de Dia's map van de Afbeeldingen map!" +
            $"{Environment.NewLine}{Environment.NewLine}" +
            $"Stap 1: Stop het kaartje in de dia scanner{Environment.NewLine}" +
            $"Stap 2: Scan de dia's per jaar (of per aantal jaar) in{Environment.NewLine}" +
            $"Stap 3: Haal daarna het kaartje uit de scanner en stop het in de computer{Environment.NewLine}" +
            $"Stap 4: Start dit programma {Environment.NewLine}" +
            $"Stap 5: Vul het jaartal (of BEGINJAAR-EINDJAAR){Environment.NewLine}" +
            $"Stap 6: Klik op organiseer";

        [TextInput("Jaartal", Description = "Bijvoorbeeld 1971 of 1971-1973", Mandatory = true)]
        public virtual string FolderName { get; set; }

        [Action]
        [Message("Klaar", "Open de Dia's map op het bureaublad en kies dan het jaartal om de dia's te zien!")]
        public void Organiseer()
        {
            try
            {
                // Validate input and if the system is ready to import the pictures
                Validate();

                // We move the Dias to the Pictures -> Dias Folder
                // so the volume is empty again for more dia scanning goodness!
                ImportDias();
            }
            catch (Exception e) when (!(e is ErrorDialogException))
            {
                App.Logger.Error(e);

                throw new Exception(
                    $"Er is iets fout gegaan! {Environment.NewLine}" +
                    $"Stuur een berichtje naar uw kleinzoon of {Environment.NewLine}" +
                    $"nodig uw kleinzoon uit voor rijstepap en hij zal het oplossen!");
            }
        }

        private void Validate()
        {
            // Check if the drive is in the computer and it has the correct name
            // I gave my grandpa an SD card with the name "DIA's" and labeled it in real life
            // so he does not mix up SD card's. 
            // He wouldn't be happy if he accidentally used a different SD Card and 
            // our code would bug out and accidently delete important files....
            if (!Directory.Exists(_diaVolumePath) || !DriveInfo.GetDrives().Any(x => x.VolumeLabel == _diaVolumeName))
            {
                ErrorDialog.ShowError(
                    $"De SD kaart kan niet gevonden worden. Zit de SD kaart in de computer?{Environment.NewLine}" +
                    $"{Environment.NewLine}" +
                    $"Neem contact op met je kleinzoon als de DIA SD kaart er wél goed in zit!");

            }
            else if (!Directory.Exists(DiaFolderInputPath) || Directory.GetFiles(DiaFolderInputPath).Length == 0)
            {
                ErrorDialog.ShowError("De SD kaart bevat geen Dia's. Scan eerst wat Dia's in en probeer het opnieuw.");
            }

            try
            {
                // https://stackoverflow.com/a/3137165/3013479
                _ = Path.GetFullPath(DiaFolderInputPath);

                if (FolderName.Contains("/") || FolderName.Contains("\\"))
                {
                    throw new ErrorDialogException();
                }
            }
            catch
            {
                ErrorDialog.ShowError("Het jaartal mag de volgende karakters niet bevatten: \\ / : * ? \" < > |");
            }
        }

        private void ImportDias()
        {
            var inputDirectory = new DirectoryInfo(DiaFolderInputPath);

            // Already create the output directory.
            _ = Directory.CreateDirectory(DiaFolderOutputPath);

            var diasToImport = inputDirectory.GetFiles("*.jpg");

            // Create a transaction scope for moving all the files at once.
            // The process succeeds when they are all moved successfully
            // and fails if even one fails.
            // I don't want to bother my grandpa with saying that 20/30 succeeded and that he can try again.
            // If even one fails, he should just contact me so I can fix it for him.
            using (var scope = new TransactionScope())
            {
                foreach (var dia in diasToImport)
                {
                    // TODO: Figure out what to do if file exists
                    // If he scans a new batch with some forgotten pics of a year he has already processed
                    // the name of the file will be PIC001 again for example.
                    // Even tho PIC001 already exists, it can be a completely different picture.
                    // Perhaps we can check some metadata? We don't want to overwrite the existing file, cuz we wanna add to that year!
                    // But we also dont want the names of the file to be random in case he does get a duplicate; we dont wanna save those.
                    // Checking duplicates might be difficult though because the device scans the dia's; it will never be the same image.

                    if (_fileManager.FileExists(Path.Combine(DiaFolderOutputPath, dia.Name)))
                    {
                        // If the file exists, do not overwrite it. Just continue.
                        // He could have deleted scanned a new batch and entered the old name
                        // TODO: READ THE TODO ABOVE. THIS MEANS THAT THE FILES IN THE END WONT BE REMOVED, TAKING UP DATA.
                        // IT PROBABLY WONT EVER BE TRUE PROBLEM BUT WE NEED TO FIX IT NONE THE LESS!
                        continue;
                    }

                    _fileManager.Move(
                        srcFileName: dia.FullName,
                        destFileName: Path.Combine(DiaFolderOutputPath, dia.Name));
                }

                scope.Complete();
            }
        }
    }
}
