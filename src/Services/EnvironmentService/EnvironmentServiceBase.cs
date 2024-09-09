using System.IO;
using System.Reflection;

namespace DevExpress.Mvvm
{
    public abstract class EnvironmentServiceBase
    {
        protected EnvironmentServiceBase(string baseDirectory, string[] args)
        {
            BaseDirectory = baseDirectory;
            Args = args;

            ConfigDirectory = Path.Combine(BaseDirectory, "Config");
            IOUtils.CheckDirectory(ConfigDirectory, true);
            AppDataDirectory = Path.Combine(BaseDirectory, "AppData");
            IOUtils.CheckDirectory(AppDataDirectory, true);
            WorkingDirectory = Path.Combine(AppDataDirectory, AssemblyInfo.Version!.ToString(2));
            IOUtils.CheckDirectory(WorkingDirectory, true);
            SettingsDirectory = Path.Combine(WorkingDirectory, "Settings");
            IOUtils.CheckDirectory(SettingsDirectory, true);
        }

        #region Properties

        public string AppDataDirectory { get; }

        public string[] Args { get; }

        public string ConfigDirectory { get; }

        public string BaseDirectory { get; }

        public string SettingsDirectory { get; }

        public string WorkingDirectory { get; }

        #endregion
    }
}
