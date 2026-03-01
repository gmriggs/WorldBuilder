using CommunityToolkit.Mvvm.ComponentModel;
using System.Numerics;

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

        private Vector3 _position;

        /// <summary>
        /// The 3D position in global worldspace
        /// </summary>
        public Vector3 Position {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        private Vector2 _rotation;
        
        /// <summary>
        /// The yaw/pitch rotation
        /// </summary>
        public Vector2 Rotation {
            get => _rotation;
            set => SetProperty(ref _rotation, value);
        }
    }
}
