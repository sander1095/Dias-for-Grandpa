using ChinhDo.Transactions;
using DiasForGrandpa.WPF.Exceptions;
using DiasForGrandpa.WPF.Helpers;
using Mecha.ViewModel.Attributes;
using System;
using System.IO;
using System.Linq;
using System.Transactions;

namespace DiasForGrandpa.WPF.ViewModels
{
    public class OrganizerViewModel
    {
        private readonly TxFileManager _fileManager = new TxFileManager();

        /// <summary>
        /// We move the dia's from the dia volume to this folder.
        /// </summary>
        private string DiaFolderOutputPath => Path.Combine(App.Settings.DiaFolderBaseOutputPath, FolderName);

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

                throw new ApplicationException(
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

            if (!Directory.Exists(App.Settings.DiaFolderInputPath) ||
                !DriveInfo.GetDrives().Any(x => x.VolumeLabel == App.Settings.DiaVolumeName) ||
                Directory.GetFiles(App.Settings.DiaFolderInputPath).Length == 0)
            {
                ErrorDialog.ShowError(
                    $"De {App.Settings.DiaVolumeName} SD kaart kan niet gevonden worden of er staan geen ingescande Dia's op. " +
                    $"Controleer dat je de juiste SD kaart in de computer hebt zitten en dat je dia's hebt gescand." +
                    $"{Environment.NewLine}" +
                    $"Neem contact op met je kleinzoon als de DIA SD kaart er wél goed in zit!");
            }

            try
            {
                // https://stackoverflow.com/a/3137165/3013479
                _ = Path.GetFullPath(App.Settings.DiaFolderInputPath);

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
            var inputDirectory = new DirectoryInfo(App.Settings.DiaFolderInputPath);
            var diasToImport = inputDirectory.GetFiles("*.jpg");

            var outputDirectory = new DirectoryInfo(DiaFolderOutputPath);
            outputDirectory.Create();

            // Make the filenames an incrementing integer.
            // This makes it easy to just add some pictures he found later on to a year that he already scanned in.
            var amountOfExistingDias = outputDirectory.GetFiles().Length;
            var fileNameNumber = amountOfExistingDias + 1;

            // Create a transaction scope for moving all the files at once.
            // The process succeeds when they are all moved successfully
            // and fails if even one fails.
            // I don't want to bother my grandpa with saying that 20/30 succeeded and that he can try again.
            // If even one fails, he should just contact me so I can fix it for him.
            using (var scope = new TransactionScope())
            {
                for (var i = 0; i < diasToImport.Length; i++, fileNameNumber++)
                {
                    var dia = diasToImport[i];
                    var expectedFileName = $"{fileNameNumber}{dia.Extension}";
                    var outputPath = Path.Combine(DiaFolderOutputPath, expectedFileName);

                    _fileManager.Move(
                        srcFileName: dia.FullName,
                        destFileName: outputPath);
                }

                scope.Complete();
            }
        }
    }
}
