using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using System;
using WorldBuilder.ViewModels;

namespace WorldBuilder.Modules.Landscape.ViewModels {
    public partial class RenameBookmarkDialogViewModel : ViewModelBase, IModalDialogViewModel {
        [ObservableProperty]
        private string _bookmarkName;

        public bool? DialogResult { get; set; }

        public event EventHandler? RequestClose;

        public RenameBookmarkDialogViewModel(string currentName) {
            _bookmarkName = currentName;
        }

        [RelayCommand]
        private void Confirm() {
            DialogResult = true;
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void Cancel() {
            DialogResult = false;
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
