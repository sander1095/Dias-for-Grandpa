using Mecha.ViewModel.Attributes;
using System;

namespace DiasForGrandpa.WPF.ViewModels
{
    public class OrganizerViewModel
    {
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
        { }
    }
}
