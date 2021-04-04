using Syroot.Windows.IO;
using System;
using System.IO;

namespace DiasForGrandpa.WPF.Helpers
{
    public class DiaSettings
    {
        /// <summary>
        /// The name of the volume where dia's are stored.
        /// If the name of the drive is different than this value, 
        /// an error will be thrown to prevent moving files from a wrong drive
        /// and accidentally deleting files if they are stored 
        /// on the same path as <see cref="DiaFolderInputPath"/>
        /// </summary>
        public string DiaVolumeName { get; set; }

        /// <summary>
        /// The path where the scanned Dia's can be found.
        /// The program will move files from this path to <see cref="DiaFolderBaseOutputPath"/>
        /// so ensure your folder permissions are set up correctly if this fails.
        /// </summary>
        public string DiaFolderInputPath { get; set; }

        /// <summary>
        /// The path where the Dia's should be stored.
        /// The program will move files from <see cref="DiaFolderInputPath"/> to this path
        /// so ensure your folder permissions are set up correctly if this fails.
        /// </summary>
        public string DiaFolderBaseOutputPath { get; set; }

        /// <exception cref="System.Exception">When not all values have been configured (correctly)</exception> <returns></returns>
        public bool IsInitialized()
        {

            var diaVolumeNameHasValue = !string.IsNullOrWhiteSpace(DiaVolumeName);
            var diaFolderInputPathHasValue = !string.IsNullOrWhiteSpace(DiaFolderInputPath);
            var diaFolderBaseOutputPathHasValue = !string.IsNullOrWhiteSpace(DiaFolderBaseOutputPath);

            if (!diaVolumeNameHasValue && !diaFolderInputPathHasValue && !diaFolderBaseOutputPathHasValue)
            {
                return false;
            }
            else if (diaVolumeNameHasValue && diaFolderInputPathHasValue && diaFolderBaseOutputPathHasValue)
            {
                return true;
            }
            else
            {
                throw new Exception($"De applicatie instellingen zijn niet goed ingesteld. " +
                    $"Stel deze goed in om dit probleem op te lossen.");
            }
        }

        public static DiaSettings GetDefaultSettings()
        {
            return new DiaSettings
            {
                DiaVolumeName = "DIA",
                DiaFolderInputPath = Path.Combine(@"G:\", "DCIM", "100COACH"),
                DiaFolderBaseOutputPath = Path.Combine(KnownFolders.Pictures.Path, "Dia's")
            };
        }
    }
}
