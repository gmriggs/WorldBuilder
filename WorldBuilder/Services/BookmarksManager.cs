using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Text.Json;
using WorldBuilder.Lib;
using WorldBuilder.ViewModels;

namespace WorldBuilder.Services {
    public class BookmarksManager {
        private readonly ILogger<RecentProjectsManager> _log;
        private readonly WorldBuilderSettings _settings;
        private readonly TaskCompletionSource<bool> _loadTask = new();

        /// <summary>
        /// Gets a task that completes when the recent projects have been loaded.
        /// </summary>
        public Task InitializationTask => _loadTask.Task;

        /// <summary>
        /// Gets the collection of recently opened projects.
        /// </summary>
        public ObservableCollection<Bookmark> Bookmarks { get; }

        /// <summary>
        /// Gets the file path for storing bookmarks data.
        /// </summary>
        private string BookmarksFilePath => Path.Combine(_settings.AppDataDirectory, "bookmarks.json");

        /// <summary>
        /// Initializes a new instance of the BookmarksManager class for design-time use.
        /// </summary>
        public BookmarksManager() {
            _settings = new WorldBuilderSettings();
            _log = Microsoft.Extensions.Logging.Abstractions.NullLogger<RecentProjectsManager>.Instance;
            Bookmarks = new ObservableCollection<Bookmark>();
            // Add sample data for design-time
            Bookmarks.Add(new Bookmark { Name = "Yaraq" });
            Bookmarks.Add(new Bookmark { Name = "Holtburg" });
            Bookmarks.Add(new Bookmark { Name = "Shoushi" });
            Bookmarks.Add(new Bookmark { Name = "Dungeon" });
        }

        /// <summary>
        /// Initializes a new instance of the BookmarksManager class with the specified dependencies.
        /// </summary>
        /// <param name="settings">The application settings</param>
        /// <param name="log">The logger instance</param>
        public BookmarksManager(WorldBuilderSettings settings, ILogger<RecentProjectsManager> log) {
            _settings = settings;
            _log = log;
            Bookmarks = new ObservableCollection<Bookmark>();

            // Load recent projects asynchronously
            _ = Task.Run(LoadBookmarks);
        }

        /// <summary>
        /// Adds a new bookmark to the collection and saves it to persistent storage.
        /// </summary>
        /// <param name="position">The camera's 3D position in global worldpsace</param>
        /// <param name="yaw">The camera's yaw (left/right rotation) in degrees</param>
        /// <param name="pitch">The camera's pitch (up/down rotation) in degrees</param>
        /// <param name="name">An optional name for the bookmark</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task AddBookmark(Vector3 position, float yaw, float pitch, string name = "") {
            var bookmark = new Bookmark {
                Name = name,
                Position = position,
                Rotation = new Vector2(yaw, pitch)
            };
            Bookmarks.Add(bookmark);
            await SaveBookmarks();
        }

        /// <summary>
        /// Removes a bookmark from the collection and updates persistent storage.
        /// </summary>
        /// <param name="bookmark">The bookmark to remove</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task RemoveBookmark(Bookmark bookmark) {
            if (Bookmarks.Remove(bookmark))
            {
                await SaveBookmarks();
            }
        }

        /// <summary>
        /// Loads bookmarks from persistent storage.
        /// </summary>
        private async Task LoadBookmarks() {
            try {
                if (!File.Exists(BookmarksFilePath)) {
                    _loadTask.TrySetResult(true);
                    return;
                }

                var json = await File.ReadAllTextAsync(BookmarksFilePath);
                var bookmarks = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.ListBookmark);

                if (bookmarks != null) {
                    Bookmarks.Clear();
                    foreach (var bookmark in bookmarks) {
                        Bookmarks.Add(bookmark);
                    }
                }
            }
            catch (Exception ex) {
                _log.LogError(ex, "Failed to load bookmarks");
                Bookmarks.Clear();
            }
            finally {
                _loadTask.TrySetResult(true);
            }
        }

        /// <summary>
        /// Saves recent projects to persistent storage.
        /// </summary>
        private async Task SaveBookmarks() {
            try {
                var json = JsonSerializer.Serialize(Bookmarks.ToList(), SourceGenerationContext.Default.ListBookmark);
                await File.WriteAllTextAsync(BookmarksFilePath, json);
            }
            catch (Exception ex) {
                _log.LogError(ex, "Failed to save bookmarks");
            }
        }
    }
}
