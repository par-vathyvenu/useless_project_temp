using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace AnnoyingCat
{
    public class AssetManager
    {
        private readonly string _baseDir;
        private readonly Dictionary<string, List<BitmapImage>> _animationCache = new Dictionary<string, List<BitmapImage>>();
        private readonly Dictionary<string, BitmapImage> _singleFrameCache = new Dictionary<string, BitmapImage>();
        private BitmapImage _fallbackImage;

        public string BaseDirectory { get { return _baseDir; } }

        public AssetManager(string baseDir = null)
        {
            _baseDir = ResolveBaseDirectory(baseDir);
            Console.WriteLine("[AssetManager] Resolved Base Directory: " + _baseDir);
            LoadAllAssets();
        }

        public static string ResolveBaseDirectory(string hintedDir = null)
        {
            // 1. Check custom AppDomain data
            object custom = AppDomain.CurrentDomain.GetData("APP_BASE_DIR");
            if (custom != null && !string.IsNullOrEmpty(custom.ToString()))
            {
                string p = custom.ToString();
                if (Directory.Exists(Path.Combine(p, "assets", "sprites"))) return Path.GetFullPath(p);
            }

            // 2. Check hinted directory
            if (!string.IsNullOrEmpty(hintedDir))
            {
                if (Directory.Exists(Path.Combine(hintedDir, "assets", "sprites"))) return Path.GetFullPath(hintedDir);
                try
                {
                    string up1 = Path.GetFullPath(Path.Combine(hintedDir, ".."));
                    if (Directory.Exists(Path.Combine(up1, "assets", "sprites"))) return up1;
                    string up2 = Path.GetFullPath(Path.Combine(hintedDir, "..", ".."));
                    if (Directory.Exists(Path.Combine(up2, "assets", "sprites"))) return up2;
                }
                catch { }
            }

            // 3. Check current working directory
            string cwd = Directory.GetCurrentDirectory();
            if (Directory.Exists(Path.Combine(cwd, "assets", "sprites"))) return Path.GetFullPath(cwd);
            try
            {
                string upCwd1 = Path.GetFullPath(Path.Combine(cwd, ".."));
                if (Directory.Exists(Path.Combine(upCwd1, "assets", "sprites"))) return upCwd1;
                string upCwd2 = Path.GetFullPath(Path.Combine(cwd, "..", ".."));
                if (Directory.Exists(Path.Combine(upCwd2, "assets", "sprites"))) return upCwd2;
            }
            catch { }

            // 4. Check Desktop\useless directly
            string desktopUseless = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "useless");
            if (Directory.Exists(Path.Combine(desktopUseless, "assets", "sprites"))) return Path.GetFullPath(desktopUseless);

            // 5. Check assembly location
            try
            {
                string asmLoc = Assembly.GetExecutingAssembly().Location;
                if (!string.IsNullOrEmpty(asmLoc))
                {
                    string asmDir = Path.GetDirectoryName(asmLoc);
                    if (Directory.Exists(Path.Combine(asmDir, "assets", "sprites"))) return asmDir;
                    string up1 = Path.GetFullPath(Path.Combine(asmDir, ".."));
                    if (Directory.Exists(Path.Combine(up1, "assets", "sprites"))) return up1;
                    string up2 = Path.GetFullPath(Path.Combine(asmDir, "..", ".."));
                    if (Directory.Exists(Path.Combine(up2, "assets", "sprites"))) return up2;
                }
            }
            catch { }

            // 6. AppDomain base directory
            string appBase = AppDomain.CurrentDomain.BaseDirectory;
            if (Directory.Exists(Path.Combine(appBase, "assets", "sprites"))) return Path.GetFullPath(appBase);
            try
            {
                string up1 = Path.GetFullPath(Path.Combine(appBase, ".."));
                if (Directory.Exists(Path.Combine(up1, "assets", "sprites"))) return up1;
                string up2 = Path.GetFullPath(Path.Combine(appBase, "..", ".."));
                if (Directory.Exists(Path.Combine(up2, "assets", "sprites"))) return up2;
            }
            catch { }

            return cwd;
        }

        private void LoadAllAssets()
        {
            string spritesDir = Path.Combine(_baseDir, "assets", "sprites");

            // Load fallback first
            string mainIdlePath = Path.Combine(spritesDir, "idle", "idle_stare.png");
            if (File.Exists(mainIdlePath))
            {
                _fallbackImage = LoadBitmap(mainIdlePath);
                _singleFrameCache["idle_stare"] = _fallbackImage;
            }

            // Load categorized folders
            LoadAnimationFolder("walk", Path.Combine(spritesDir, "walk"));
            LoadAnimationFolder("dance", Path.Combine(spritesDir, "dance"));
            LoadAnimationFolder("mouth", Path.Combine(spritesDir, "mouth"));
            LoadAnimationFolder("idle", Path.Combine(spritesDir, "idle"));
            LoadAnimationFolder("actions", Path.Combine(spritesDir, "actions"));

            // If fallback wasn't loaded from idle_stare, pick first available image
            if (_fallbackImage == null)
            {
                foreach (var list in _animationCache.Values)
                {
                    if (list.Count > 0)
                    {
                        _fallbackImage = list[0];
                        break;
                    }
                }
            }

            Console.WriteLine(string.Format("[AssetManager] Loaded: walk={0}, idle={1}, dance={2}, mouth={3}, actions={4}",
                GetAnimation("walk").Count,
                GetAnimation("idle").Count,
                GetAnimation("dance").Count,
                GetAnimation("mouth").Count,
                GetAnimation("actions").Count));
        }

        private void LoadAnimationFolder(string category, string folderPath)
        {
            var list = new List<BitmapImage>();
            if (Directory.Exists(folderPath))
            {
                var files = Directory.GetFiles(folderPath, "*.png")
                    .Concat(Directory.GetFiles(folderPath, "*.jpg"))
                    .Concat(Directory.GetFiles(folderPath, "*.jpeg"))
                    .OrderBy(f => f)
                    .ToList();

                foreach (var file in files)
                {
                    try
                    {
                        var bmp = LoadBitmap(file);
                        list.Add(bmp);
                        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
                        _singleFrameCache[fileNameWithoutExt.ToLowerInvariant()] = bmp;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[AssetManager] Warning loading " + file + ": " + ex.Message);
                    }
                }
            }

            _animationCache[category.ToLowerInvariant()] = list;
        }

        private BitmapImage LoadBitmap(string path)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.UriSource = new Uri(Path.GetFullPath(path), UriKind.Absolute);
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }

        public BitmapImage GetFrame(string frameName)
        {
            string key = frameName.ToLowerInvariant();
            if (_singleFrameCache.ContainsKey(key))
            {
                return _singleFrameCache[key];
            }
            return _fallbackImage;
        }

        public List<BitmapImage> GetAnimation(string category)
        {
            string key = category.ToLowerInvariant();
            if (_animationCache.ContainsKey(key) && _animationCache[key].Count > 0)
            {
                return _animationCache[key];
            }

            if (_fallbackImage != null)
            {
                return new List<BitmapImage> { _fallbackImage };
            }

            return new List<BitmapImage>();
        }

        public BitmapImage GetFallback()
        {
            return _fallbackImage;
        }
    }
}
