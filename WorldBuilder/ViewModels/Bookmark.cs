using CommunityToolkit.Mvvm.ComponentModel;

namespace WorldBuilder.ViewModels {
    public partial class Bookmark : ObservableObject {
        private string _name = string.Empty;

        /// <summary>
        /// Gets or sets the name of the bookmark.
        /// </summary>
        public string Name {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _location = string.Empty;

        /// <summary>
        /// The AC /loc string format 0xXXYYCCCC [X Y Z] w x y z
        /// </summary>
        public string Location {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        /// <summary>
        /// Creates a shallow copy of this bookmark.
        /// </summary>
        public Bookmark Clone() {
            return new Bookmark {
                Name = this.Name,
                Location = this.Location
            };
        }
    }
}
