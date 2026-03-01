using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;

using WorldBuilder.Services;
using WorldBuilder.Shared.Models;
using WorldBuilder.ViewModels;

namespace WorldBuilder.Modules.Landscape.ViewModels {
    public partial class BookmarksPanelViewModel : ViewModelBase {
        private readonly BookmarksManager _bookmarksManager;
        private readonly LandscapeViewModel _landScapeViewModel;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private ObservableCollection<Bookmark> _bookmarks = new();

        [ObservableProperty]
        private Bookmark? _selectedBookmark;

        public BookmarksPanelViewModel(BookmarksManager bookmarksManager, LandscapeViewModel landScapeViewModel, IDialogService dialogService) {
            _bookmarksManager = bookmarksManager ?? throw new ArgumentNullException(nameof(bookmarksManager));
            _landScapeViewModel = landScapeViewModel ?? throw new ArgumentNullException(nameof(landScapeViewModel));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            LoadBookmarks();
        }

        private async void LoadBookmarks() {
            await _bookmarksManager.InitializationTask;
            Bookmarks.Clear();
            foreach (var bookmark in _bookmarksManager.Bookmarks) {
                Bookmarks.Add(bookmark);
            }
        }
        
        [RelayCommand]
        public async Task AddBookmark() {
            var gameScene = _landScapeViewModel.GameScene;
            var loc = Position.FromGlobal(gameScene.Camera.Position, _landScapeViewModel.ActiveDocument?.Region, gameScene.CurrentEnvCellId != 0 ? gameScene.CurrentEnvCellId : null);
            loc.Rotation = gameScene.Camera.Rotation;

            var bookmarkName = $"{loc.LandblockX:X2}{loc.LandblockY:X2} [{loc.LocalX:0} {loc.LocalY:0} {loc.LocalZ:0}]";
            await _bookmarksManager.AddBookmark(loc.ToLandblockString(), bookmarkName);
            
            // Refresh the bookmarks collection
            LoadBookmarks();    // shouldn't this bind automatically?
            
            // Select the newly added bookmark (it should be the last one)
            if (_bookmarksManager.Bookmarks.Count > 0) {
                SelectedBookmark = _bookmarksManager.Bookmarks.Last();
            }
        }

        [RelayCommand]
        public void GoToBookmark(Bookmark? bookmark) {
            if (!string.IsNullOrEmpty(bookmark?.Location) && Position.TryParse(bookmark.Location, out var pos, _landScapeViewModel.ActiveDocument?.Region)) {
                _landScapeViewModel.GameScene.Teleport(pos!.GlobalPosition, (uint)((pos.LandblockId << 16) | pos.CellId));
                if (pos.Rotation.HasValue) {
                    _landScapeViewModel.GameScene.CurrentCamera.Rotation = pos.Rotation.Value;
                }
            }
        }

        [RelayCommand]
        public async Task UpdateBookmark(Bookmark? bookmark) {
            if (bookmark == null) return;

            var gameScene = _landScapeViewModel.GameScene;
            var loc = Position.FromGlobal(gameScene.Camera.Position, _landScapeViewModel.ActiveDocument?.Region, gameScene.CurrentEnvCellId != 0 ? gameScene.CurrentEnvCellId : null);
            loc.Rotation = gameScene.Camera.Rotation;

            // Remove the old bookmark and add a new one with updated position
            await _bookmarksManager.RemoveBookmark(bookmark);
            await _bookmarksManager.AddBookmark(loc.ToLandblockString(), bookmark.Name);

            // Refresh the bookmarks collection
            LoadBookmarks();
        }

        [RelayCommand]
        public async Task RenameBookmark(Bookmark? bookmark) {
            if (bookmark == null) return;

            var newName = await ShowRenameDialog(bookmark.Name);
            if (string.IsNullOrWhiteSpace(newName) || newName == bookmark.Name) return;

            // Remove old bookmark and add new one with updated name
            await _bookmarksManager.RemoveBookmark(bookmark);
            await _bookmarksManager.AddBookmark(bookmark.Location, newName);
            
            // Refresh the bookmarks collection
            LoadBookmarks();
        }

        private async Task<string?> ShowRenameDialog(string currentName) {
            var vm = new RenameBookmarkDialogViewModel(currentName);

            var owner = (Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow?.DataContext as System.ComponentModel.INotifyPropertyChanged;
            if (owner != null) {
                await _dialogService.ShowDialogAsync(owner, vm);
            }
            return vm.DialogResult == true ? vm.BookmarkName : null;
        }

        [RelayCommand]
        public async Task DeleteBookmark(Bookmark? item) {
            if (item == null) return;
            await _bookmarksManager.RemoveBookmark(item);
            if (SelectedBookmark == item) SelectedBookmark = null;
            LoadBookmarks();
        }

        [RelayCommand]
        public void MoveUp(Bookmark? bookmark) {
            if (bookmark == null) return;
            var idx = Bookmarks.IndexOf(bookmark);
            if (idx > 0) {
                Bookmarks.Move(idx, idx - 1);
                // Note: BookmarksManager doesn't support reordering yet, so this only affects the UI
            }
        }

        [RelayCommand]
        public void MoveDown(Bookmark? bookmark) {
            if (bookmark == null) return;
            var idx = Bookmarks.IndexOf(bookmark);
            if (idx >= 0 && idx < Bookmarks.Count - 1) {
                Bookmarks.Move(idx, idx + 1);
                // Note: BookmarksManager doesn't support reordering yet, so this only affects the UI
            }
        }
    }
}
