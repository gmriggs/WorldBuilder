using Avalonia.Controls;
using Avalonia.Input;
using WorldBuilder.Lib.Platform;
using WorldBuilder.Modules.Landscape.ViewModels;

namespace WorldBuilder.Modules.Landscape.Views.Components;

public partial class BookmarksPanel : UserControl {
    public BookmarksPanel() {
        InitializeComponent();
        
        // Apply platform-specific padding for arrow alignment
        if (Platform.IsLinux)
            GoToButton.Padding = new Avalonia.Thickness(0, 4, 0, 0);
        else if (Platform.IsMacOS)
            GoToButton.Padding = new Avalonia.Thickness(0, 1, 0, 0);
    }

    private void BookmarkListBox_KeyDown(object? sender, KeyEventArgs e) {
        if (DataContext is BookmarksPanelViewModel viewModel) {
            var selectedItem = BookmarkListBox.SelectedItem;
            if (selectedItem != null) {
                if (e.Key == Key.Delete) {
                    viewModel.DeleteBookmarkCommand.Execute(selectedItem);
                }
                else if (e.Key == Key.Enter) {
                    viewModel.GoToBookmarkCommand.Execute(selectedItem);
                }
            }
        }
    }

    private void BookmarkListBox_DoubleTapped(object? sender, TappedEventArgs e) {
        if (DataContext is BookmarksPanelViewModel viewModel) {
            var selectedItem = BookmarkListBox.SelectedItem;
            if (selectedItem != null) {
                viewModel.GoToBookmarkCommand.Execute(selectedItem);
            }
        }
    }
}
