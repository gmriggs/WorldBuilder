using Avalonia.Controls;
using WorldBuilder.Modules.Landscape.ViewModels;

namespace WorldBuilder.Modules.Landscape.Views.Components {
    public partial class RenameBookmarkDialogView : Window {
        public RenameBookmarkDialogView() {
            InitializeComponent();
            DataContextChanged += RenameBookmarkDialogView_DataContextChanged;
        }

        private void RenameBookmarkDialogView_DataContextChanged(object? sender, EventArgs e) {
            if (DataContext is RenameBookmarkDialogViewModel vm) {
                vm.RequestClose += (s, ev) => Close();
                
                Opened += (s, ev) => {
                    var textBox = this.FindControl<TextBox>("BookmarkName");
                    textBox?.Focus();
                    textBox?.SelectAll();
                };
            }
        }
    }
}
