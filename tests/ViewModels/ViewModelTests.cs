using DevExpress.Mvvm;
using System.ComponentModel;
using System.Diagnostics;

namespace NuExt.DevExpress.Mvvm.Tests.ViewModels
{
    public class ViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            ViewModelConfiguration.Default = new ViewModelConfiguration()
            {
                IsInDebugModeOverride = true,
                ThrowFinalizerException = true,
                ThrowParentViewModelIsNullException = false
            };
        }

        [Test]
        public async Task InitializeAsync()
        {
            var propertyList = new HashSet<string>();
            void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
            {
                Assert.That(sender is IViewModel, Is.True);
                Trace.WriteLine($"Property Changed: {e.PropertyName}");
                propertyList.Add(e.PropertyName!);
            };
            ValueTask OnDisposing(object? sender, EventArgs e, CancellationToken cancellationToken = default)
            {
                var vm = sender as IViewModel;
                Assert.Multiple(() =>
                {
                    Assert.That(vm, Is.Not.Null);
                    Assert.That(propertyList, Has.Count.EqualTo(3));
                    Assert.That(propertyList.SequenceEqual(new[] {
                            nameof(ViewModel.IsInitialized), nameof(ViewModel.IsUsable), nameof(ViewModel.IsDisposing)
                        }), Is.True);
                });
                vm!.Disposing -= OnDisposing;
                return default;
            };

            var viewModel = new TestViewModel();

            using var lifetime = new Lifetime();
            lifetime.AddBracket(
                () => viewModel.PropertyChanged += OnPropertyChanged,
                () => viewModel.PropertyChanged -= OnPropertyChanged);
            lifetime.AddBracket(
                () => viewModel.Disposing += OnDisposing,
                () => viewModel.Disposing -= OnDisposing);

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.IsInitialized, Is.EqualTo(false));
                Assert.That(propertyList, Has.Count.EqualTo(0));
            });
            await viewModel.InitializeAsync();
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.IsInitialized, Is.EqualTo(true));
                Assert.That(viewModel.IsDisposed, Is.EqualTo(false));
                Assert.That(propertyList, Has.Count.EqualTo(2));
                Assert.That(propertyList.SequenceEqual(new[] {
                        nameof(viewModel.IsInitialized), nameof(viewModel.IsUsable)
                    }),
                    Is.True);
            });
            await viewModel.DisposeAsync();
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.IsDisposed, Is.EqualTo(true));
                Assert.That(propertyList, Has.Count.EqualTo(4));
                Assert.That(propertyList.SequenceEqual(new[] {
                        nameof(viewModel.IsInitialized), nameof(viewModel.IsUsable), nameof(viewModel.IsDisposing),
                        nameof(viewModel.IsDisposed)
                    }), Is.True);
            });
        }
    }

    public class TestViewModel : ViewModel
    {
        protected override ValueTask OnDisposeAsync()
        {
            return default;
        }

        protected override ValueTask OnInitializeAsync(CancellationToken ct)
        {
            return default;
        }
    }
}
