using DiasForGrandpa.WPF.ViewModels;
using LLibrary;
using Mecha.Wpf.Settings;
using Syroot.Windows.IO;
using System;
using System.IO;

public class App : IApp
{
    public static L Logger { get; } = new L(
        deleteOldFiles: TimeSpan.FromDays(90),
        directory: Path.Combine(KnownFolders.LocalAppData.Path, "DiasForGrandpa"));

    public void Init(AppSettings s)
    {
        s.Title = "Dia's organiseren";
        s.Window.Width = 450;
        s.Window.Height = 375;
        s.Content = typeof(OrganizerViewModel);
    }
}