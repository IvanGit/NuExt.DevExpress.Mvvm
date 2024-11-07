using WpfAppSample.Models;

namespace WpfAppSample.Interfaces.ViewModels
{
    public interface IMainWindowViewModel
    {
        ValueTask CloseMovieAsync(MovieModel movie, CancellationToken cancellationToken);
        ValueTask OpenMovieAsync(MovieModel movie, CancellationToken cancellationToken);
    }
}
