using DiasForGrandpa.WPF.Exceptions;
using DiasForGrandpa.WPF.Helpers;
using Mecha.ViewModel.Attributes;
using System;
using System.IO;
using System.Linq;

namespace DiasForGrandpa.WPF.ViewModels
{
    public class OrganizerViewModel
    {
        // TODO: Change DiaDrivePath and DiaFolderPath to the correct one. (G)
        private string DiaDrivePath => "D:\\";
        private string DiaFolderPath => Path.Combine(DiaDrivePath, "DCIM", "100COACH");
        private string DiaDriveName => "DIA";


        [Readonly]
        public virtual string Intro =>
            $"Dit programma zal ervoor zorgen dat ingescande dia's {Environment.NewLine}" +
            $"automatisch gesorteerd worden in de Dia's map van de Afbeeldingen {Environment.NewLine}" +
            $"Video map!{Environment.NewLine}" +
            $"{Environment.NewLine}" +
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
                Validate();
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
            if (!Directory.Exists(DiaDrivePath) || !DriveInfo.GetDrives().Any(x => x.VolumeLabel == DiaDriveName))
            {
                ErrorDialog.ShowError(
                    $"De SD kaart kan niet gevonden worden. Zit de SD kaart in de computer?{Environment.NewLine}" +
                    $"{Environment.NewLine}" +
                    $"Neem contact op met je kleinzoon als de DIA SD kaart er wél goed in zit!");

            }
            else if (!Directory.Exists(DiaFolderPath) || Directory.GetFiles(DiaFolderPath).Length == 0)
            {
                ErrorDialog.ShowError("De SD kaart bevat geen Dia's. Scan eerst wat Dia's in en probeer het opnieuw.");
            }

            try
            {
                // https://stackoverflow.com/a/3137165/3013479
                _ = Path.GetFullPath(DiaFolderPath);

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
    }
}
