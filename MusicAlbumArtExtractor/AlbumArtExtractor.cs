using System.Drawing;
using System.Drawing.Imaging;

namespace MusicAlbumArtExtractor
{
    /// <summary>
    /// A Class for Extracting and Saving album arts embedded in audio files. Has Batch Mode and Single Song Mode.
    /// </summary>
    internal class AlbumArtExtractor
    {
        /// <summary>
        /// Extracts and saves the album arts of audio files in a given directory. returns <list type="<bool>"></list> 
        /// of the operation. returns an empty list if an exception was encountered.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        internal static List<bool> ExtractBatchAlbumArt (string directory)
        {
            List<bool> ExtractionLog;
            try
            {
                ExtractionLog = BatchModeOperation(directory);
            }
            catch
            { ExtractionLog = new List<bool>(); }
            return ExtractionLog;
        }

        /// <summary>
        /// Extracts and saves the album art of an audio file at the given directory. returns true if the image was saved, 
        /// and false if the audio file did not contain an image or an exception was encountered.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static bool ExtractSingleSongAlbumArt (string path)
        {
            bool picExists  = SingleSongModeOperation(path);
            return picExists;
        }

        private static bool SingleSongModeOperation(string path)
        {
            string audioPath = path;
            bool isAcceptablePath = System.IO.File.Exists(audioPath);
            while (isAcceptablePath == false)
            {
                Console.WriteLine("\r\n  Invalid File Path. The File either does not exist or this program was not granted access.");
                Console.WriteLine("\r\n  Please provided the Audio File's path. \r\n");
                audioPath = Console.ReadLine();
                isAcceptablePath = System.IO.File.Exists(audioPath);
            }
            string saveLocation = GetSaveLocation(path);
            bool picExists = GetAndSaveAlbumArt(audioPath, saveLocation);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            int gen = GC.MaxGeneration;
            GC.Collect(gen, GCCollectionMode.Aggressive);
            return picExists;
        }

        private static List<bool> BatchModeOperation(string directory)
        {
            List<bool> ExtractionLog = new List<bool>();
            bool exists = Directory.Exists(directory);
            while (exists == false)
            {
                Console.WriteLine("\r\n  The Provided Directory does not exists. Please provide an existing Directory \r\n");
                directory = Console.ReadLine();
                exists = Directory.Exists(directory);
            }
            string saveLocation = GetSaveLocation(directory);
            string[] audioPaths = ScanDirectory(directory);

            for (int i = 0; i < audioPaths.Length; i++)
            {
                var audioPath = audioPaths[i];
                bool picExists = GetAndSaveAlbumArt(audioPath, saveLocation);
                ExtractionLog.Add(picExists);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            int gen = GC.MaxGeneration;
            GC.Collect(gen, GCCollectionMode.Aggressive);
            return ExtractionLog;
        }

        private static bool GetAndSaveAlbumArt(string audioPath, string saveLocation)
        {
            ATL.Track track = new ATL.Track(audioPath);
            var name = Path.GetFileNameWithoutExtension(audioPath);
            string artPath = saveLocation + name + ".png";
            if (track.EmbeddedPictures.Any() && track.EmbeddedPictures[0] is not null)
            {
                var pic = track.EmbeddedPictures[0];
                if (pic.PictureData != null && pic.NativeFormat != Commons.ImageFormat.Unsupported)
                {
                    MemoryStream memory = new MemoryStream(pic.PictureData, true);
                    Image art = Image.FromStream(memory);
                    art.Save(artPath, ImageFormat.Png);
                    art.Dispose();
                    art = null;
                    memory.Dispose();
                }
            }
            track = null;
            if (System.IO.File.Exists(artPath))
            { return true; }
            else { return false; }
        }

        private static string GetSaveLocation (string path)
        {
            Console.WriteLine("\r\n  (Optional) Please provide a path for the extracted album arts to be saved to; if no path is \r\n" +
                                    "  provided then the arts are by default saved to:\r\n...\\the previously inputed directory or song directory\\Album Arts\\ \r\n");
            Console.WriteLine("\r\n  In case of encountaring any errors, they will be saved to C:\\Album Arts\\");
            string? saveLocation = Console.ReadLine();
            if (saveLocation is null || saveLocation == string.Empty)
            {
                saveLocation = path + "\\Album Arts\\";
            }           
            try
            {
                System.IO.Directory.CreateDirectory(saveLocation);
            }
            catch
            {
                saveLocation = "C:\\Album Arts\\";
                System.IO.Directory.CreateDirectory(saveLocation);
            }
            saveLocation = saveLocation + "\\";
            return saveLocation;
        }

        private static string[] ScanDirectory(string directory)
        {
            List<string> tempAudioPaths = new List<string>();
            string[]? tempDirectoryFiles = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
            if (tempDirectoryFiles != null)
            {
                foreach (string tempFile in tempDirectoryFiles)
                {
                    bool isAcceptable = false;
                    var temp = Path.GetExtension(tempFile).Trim().ToLower();
                    if (temp == ".mp3" || temp == ".wav" || temp == ".flac" || temp == ".aiff" || temp == ".wma" ||
                        temp == ".pcm" || temp == ".aac" || temp == ".oog" || temp == ".alac" || temp == ".m4a")
                    { isAcceptable = true; }

                    if (isAcceptable)
                    {
                        tempAudioPaths.Add(tempFile);
                    }
                }
                tempAudioPaths.TrimExcess();
            }
            tempDirectoryFiles = null;
            string[] audioPaths = tempAudioPaths.ToArray();
            tempAudioPaths.Clear();
            tempAudioPaths = null;
            return audioPaths;
        }
    }
}
