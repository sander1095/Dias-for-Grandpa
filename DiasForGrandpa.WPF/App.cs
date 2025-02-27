﻿using DiasForGrandpa.WPF.Helpers;
using DiasForGrandpa.WPF.ViewModels;
using LLibrary;
using Mecha.Wpf.Settings;
using SettingManagement;
using Syroot.Windows.IO;
using System;
using System.IO;
using System.Windows;

public class App : IApp
{
    private static readonly string _storageDirectory =
        Path.Combine(KnownFolders.LocalAppData.Path, "DiasForGrandpa");

    private static readonly string _settingsPath = Path.Combine(_storageDirectory, "settings.xml");

    public static L Logger { get; } = new L(
        deleteOldFiles: TimeSpan.FromDays(90),
        directory: _storageDirectory);

    public static DiaSettings Settings { get; private set; }

    public void Init(AppSettings s)
    {
        try
        {

            Logger.Debug("Starting application and initializing settings");
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            Settings = InitializeSettings();

            s.Title = "Dia's organiseren";
            s.Window.Width = 450;
            s.Window.Height = 375;
            s.Content = typeof(OrganizerViewModel);
        }
        catch (Exception e) when (!(e is ApplicationException))
        {
            // If the application can't start, show a pop-up so the user knows what is going on.
            // We don't catch ApplicationException because they are caused in the viewmodel which shows its own pop-ups.
            MessageBox.Show("De applicatie kon niet gestart worden. Neem contact op met je kleinzoon!", "Error");

            Logger.Error(e);
            throw;
        }
    }


    /// <remarks>
    /// https://stackoverflow.com/a/1119869/3013479
    /// </remarks>
    private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
        Logger.Debug("Quitting application");
    }

    private static DiaSettings InitializeSettings()
    {
        Logger.Debug($"Loading settings from {_settingsPath}");
        var settings = SettingManager.Load<DiaSettings>(_settingsPath);

        if (!settings.IsInitialized())
        {
            Logger.Debug($"Settings are not initialized yet. Initializing..");
            settings = DiaSettings.GetDefaultSettings();
            SettingManager.Save(_settingsPath, settings);
            Logger.Debug("Settings have been initialized!");
        }

        return settings;
    }

}