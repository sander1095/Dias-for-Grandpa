using DiasForGrandpa.WPF.ViewModels;
using LLibrary;
using Mecha.Wpf.Settings;
using Syroot.Windows.IO;
using System;
using System.IO;

public class App : IApp
{
    private static readonly string _storageDirectory =
        Path.Combine(KnownFolders.LocalAppData.Path, "DiasForGrandpa");

    public static L Logger { get; } = new L(
        deleteOldFiles: TimeSpan.FromDays(90),
        directory: _storageDirectory);

    public void Init(AppSettings s)
    {
        Logger.Debug("Starting application");

        AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

        s.Title = "Dia's organiseren";
        s.Window.Width = 450;
        s.Window.Height = 375;
        s.Content = typeof(OrganizerViewModel);
    }


    /// <remarks>
    /// https://stackoverflow.com/a/1119869/3013479
    /// </remarks>
    private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
        Logger.Debug("Quitting application");
    }
}