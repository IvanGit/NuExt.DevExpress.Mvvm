﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using System.Windows;
using System.Windows.Input;
using WpfAppSample.Models;
using WpfAppSample.Views;

namespace WpfAppSample.ViewModels
{
    internal partial class MoviesViewModel
    {
        #region Commands

        public ICommand? DeleteCommand
        {
            get => GetProperty(() => DeleteCommand);
            private set { SetProperty(() => DeleteCommand, value); }
        }

        public ICommand? EditCommand
        {
            get => GetProperty(() => EditCommand);
            private set { SetProperty(() => EditCommand, value); }
        }

        public ICommand? ExpandCollapseCommand
        {
            get => GetProperty(() => ExpandCollapseCommand);
            private set { SetProperty(() => ExpandCollapseCommand, value); }
        }

        public ICommand? MoveCommand
        {
            get => GetProperty(() => MoveCommand);
            private set { SetProperty(() => MoveCommand, value); }
        }

        public ICommand? NewGroupCommand
        {
            get => GetProperty(() => NewGroupCommand);
            private set { SetProperty(() => NewGroupCommand, value); }
        }

        public ICommand? NewMovieCommand
        {
            get => GetProperty(() => NewMovieCommand);
            private set { SetProperty(() => NewMovieCommand, value); }
        }

        public ICommand? OpenMovieCommand
        {
            get => GetProperty(() => OpenMovieCommand);
            private set { SetProperty(() => OpenMovieCommand, value); }
        }

        #endregion

        #region Command Methods

        private bool CanDelete() => CanEdit();

        private async Task DeleteAsync()
        {
            var cancellationToken = GetCurrentCancellationToken();

            var dialogResult = MessageBoxService.Show($"Are you sure you want to delete '{SelectedItem?.Name}'?", "Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialogResult != MessageBoxResult.Yes)
            {
                return;
            }

            var itemToDelete = SelectedItem!;
            var parentPath = itemToDelete.Parent?.GetPath();
            bool result = await MoviesService.DeleteAsync(itemToDelete, cancellationToken);
            if (result)
            {
                if (itemToDelete is MovieModel movie)
                {
                    await ParentViewModel!.CloseMovieAsync(movie, cancellationToken);
                }
                await ReloadMoviesAsync(cancellationToken);
                var item = Movies!.FindByPath(parentPath);
                //item?.Expand();
                SelectedItem = item;
            }
        }

        private bool CanEdit()
        {
            return IsUsable && SelectedItem?.IsEditable == true && DialogService != null;
        }

        private async Task EditAsync()
        {
            var cancellationToken = GetCurrentCancellationToken();

            var clone = SelectedItem!.Clone();

            switch (clone)
            {
                case MovieGroupModel group:
                    await using (var viewModel = new InputDialogViewModel() { InputMessage = "Enter new group name" })
                    {
                        await viewModel.SetParameter(group.Name).SetParentViewModel(this).InitializeAsync(cancellationToken);
                        var dlgResult = await DialogService!.ShowDialogAsync(MessageButton.OKCancel,
                            "Edit Group Name", nameof(InputDialogView), viewModel, cancellationToken);
                        if (dlgResult != MessageResult.OK || string.IsNullOrWhiteSpace(viewModel.InputText))
                        {
                            return;
                        }
                        group.Name = viewModel.InputText!;
                    }
                    break;
                case MovieModel movie:

                    await using (var viewModel = new EditMovieViewModel())
                    {
                        await viewModel.SetParameter(movie).SetParentViewModel(this).InitializeAsync(cancellationToken);

                        var dlgResult = await DialogService!.ShowDialogAsync(MessageButton.OKCancel, "Edit Movie",
                            nameof(EditMovieView), viewModel, cancellationToken);
                        if (dlgResult != MessageResult.OK) 
                        {
                            return;
                        }
                    }
                    break;
            }

            var path = clone.GetPath();
            bool result = await MoviesService.SaveAsync(SelectedItem, clone, cancellationToken);
            if (result)
            {
                await ReloadMoviesAsync(cancellationToken);
                var item = Movies!.FindByPath(path);
                //item?.Expand();
                SelectedItem = item;
            }
        }

        private bool CanNewGroup()
        {
            return IsUsable && SelectedItem is MovieGroupModel { IsLost: false } && DialogService != null;
        }

        private async Task NewGroupAsync()
        {
            var cancellationToken = GetCurrentCancellationToken();

            string? groupName = null;
            await using (var viewModel = new InputDialogViewModel() { InputMessage = "Enter new group name" })
            {
                await viewModel.SetParameter(groupName).SetParentViewModel(this).InitializeAsync(cancellationToken);
                var dlgResult = await DialogService!.ShowDialogAsync(MessageButton.OKCancel, "New Group Name",
                    nameof(InputDialogView), viewModel, cancellationToken);
                if (dlgResult != MessageResult.OK)
                {
                    return;
                }
                groupName = viewModel.InputText;
            }

            if (string.IsNullOrWhiteSpace(groupName))
            {
                return;
            }

            var model = new MovieGroupModel()
            {
                Name = groupName!,
                Parent = SelectedItem is MovieGroupModel { IsRoot: false } group ? group : null
            };
            var path = model.GetPath();

            bool result = await MoviesService.AddAsync(model, cancellationToken);
            if (result)
            {
                await ReloadMoviesAsync(cancellationToken);
                var item = Movies!.FindByPath(path);
                //item?.Expand();
                SelectedItem = item;
            }
        }

        private bool CanNewMovie() => CanNewGroup();

        private async Task NewMovieAsync()
        {
            var cancellationToken = GetCurrentCancellationToken();

            await using var viewModel = new EditMovieViewModel();

            var movie = new MovieModel()
            {
                Name = "New Movie",
                ReleaseDate = DateTime.Today,
                Parent = SelectedItem is MovieGroupModel { IsRoot: false } group ? group : null
            };

            await viewModel.SetParameter(movie).SetParentViewModel(this).InitializeAsync(cancellationToken);

            var dlgResult = await DialogService!.ShowDialogAsync(MessageButton.OKCancel, "New Movie", nameof(EditMovieView), viewModel, cancellationToken);
            if (dlgResult != MessageResult.OK)
            {
                return;
            }

            var path = viewModel.Movie.GetPath();
            bool result = await MoviesService.AddAsync(viewModel.Movie, cancellationToken);
            if (result)
            {
                await ReloadMoviesAsync(cancellationToken);
                var item = Movies!.FindByPath(path);
                //item?.Expand();
                SelectedItem = item;
            }
        }

        private bool CanOpenMovie(MovieModelBase? item)
        {
            return IsUsable && item is MovieModel && ParentViewModel is not null;
        }

        private async Task OpenMovieAsync(MovieModelBase? item)
        {
            var cancellationToken = GetCurrentCancellationToken();
            await ParentViewModel!.OpenMovieAsync((item as MovieModel)!, cancellationToken);
        }

        private bool CanMove(MovieModelBase? draggedObject)
        {
            return IsUsable && draggedObject?.CanDrag == true;
        }

        private async Task MoveAsync(MovieModelBase? draggedObject)
        {
            var cancellationToken = GetCurrentCancellationToken();

            var path = draggedObject!.GetPath();
            await ReloadMoviesAsync(cancellationToken);
            var item = Movies!.FindByPath(path);
            //item?.Expand();
            SelectedItem = item;
        }

        private void ExpandOrCollapse(bool expand)
        {
            if (expand)
            {
                Movies!.OfType<MovieGroupModel>().ForEach(m => m.ExpandAll());
            }
            else
            {
                Movies!.OfType<MovieGroupModel>().ForEach(m => m.CollapseAll());
            }
        }

        #endregion

        #region Methods

        protected override void CreateCommands()
        {
            base.CreateCommands();
            DeleteCommand = RegisterAsyncCommand(DeleteAsync, CanDelete);
            EditCommand = RegisterAsyncCommand(EditAsync, CanEdit);
            NewGroupCommand = RegisterAsyncCommand(NewGroupAsync, CanNewGroup);
            NewMovieCommand = RegisterAsyncCommand(NewMovieAsync, CanNewMovie);
            MoveCommand = RegisterAsyncCommand<MovieModelBase?>(MoveAsync, CanMove);
            OpenMovieCommand = RegisterAsyncCommand<MovieModelBase?>(OpenMovieAsync, CanOpenMovie);
            ExpandCollapseCommand = RegisterCommand<bool>(ExpandOrCollapse, _ => IsUsable);
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            CreateSettings();
            if (!string.IsNullOrEmpty(Settings!.SelectedPath))
            {
                SelectedItem = Movies?.FindByPath(Settings.SelectedPath);
            }
        }

        #endregion
    }
}