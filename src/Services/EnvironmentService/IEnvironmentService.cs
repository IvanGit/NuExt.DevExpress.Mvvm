namespace DevExpress.Mvvm
{
    public interface IEnvironmentService
    {
        /// <summary>
        /// Base application directory
        /// </summary>
        string BaseDirectory { get; }

        /// <summary>
        /// Application configuration directory
        /// </summary>
        string ConfigDirectory { get; }

        /// <summary>
        /// Application settings directory
        /// </summary>
        string SettingsDirectory { get; }
    }
}
