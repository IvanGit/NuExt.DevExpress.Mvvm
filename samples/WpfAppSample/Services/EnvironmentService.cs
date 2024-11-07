using DevExpress.Mvvm;
using System.IO;
using WpfAppSample.Interfaces.Services;

namespace WpfAppSample.Services
{
    internal sealed class EnvironmentService : EnvironmentServiceBase, IEnvironmentService
    {
        public EnvironmentService(string baseDirectory, params string[] args) : base(baseDirectory, Path.Combine(baseDirectory, "AppData"), args)
        {
        }
    }
}
