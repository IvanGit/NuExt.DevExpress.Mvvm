﻿using System.IO;

namespace DevExpress.Mvvm
{
    /// <summary>
    /// Provides a service for saving and loading settings.
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Occurs when there is an error during the load or save operation.
        /// </summary>
        event ErrorEventHandler? Error;

        /// <summary>
        /// Loads settings into the specified bindable object.
        /// </summary>
        /// <param name="settings">The bindable object into which the settings will be loaded.</param>
        /// <param name="name">The name of the settings file, defaulting to "Settings".</param>
        /// <returns>True if the settings were successfully loaded; otherwise, false.</returns>
        bool LoadSettings(IBindable settings, string name = "Settings");

        /// <summary>
        /// Saves the settings from the specified bindable object.
        /// </summary>
        /// <param name="settings">The bindable object from which the settings will be saved.</param>
        /// <param name="name">The name of the settings file, defaulting to "Settings".</param>
        /// <returns>True if the settings were successfully saved; otherwise, false.</returns>
        bool SaveSettings(IBindable settings, string name = "Settings");
    }
}
