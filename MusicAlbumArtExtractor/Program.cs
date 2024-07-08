namespace MusicAlbumArtExtractor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool runProgram = true;
            try
            {
                while (runProgram)
                {
                    RunProgram();
                    runProgram = OnceAgain();
                }
                Environment.Exit(0);
            }
            catch
            {
                Console.WriteLine("\r\n An Error was Encountered. Restarting \r\n\r\n");
                while (runProgram)
                {
                    RunProgram();
                    runProgram = OnceAgain();
                }
                Environment.Exit(0);
            }
        }

        private static void RunProgram()
        {
            Welcome();
            bool isBatchMode = ModeAndInput();
            if (isBatchMode)
            {
                string directory = GetDirectory();
                List<bool> extractionLogs = AlbumArtExtractor.ExtractBatchAlbumArt(directory);
                int success = 0;
                int failure = 0;
                foreach (bool log in extractionLogs)
                {
                    if (log == true) { success++; }
                    else { failure++; }
                }
                Console.WriteLine("\r\n  Extraction is Over. \r\n");
                Console.WriteLine("\r\n  Sucessful Extractions and Saves: " + success);
                Console.WriteLine("\r\n  Failed Extractions or Saves: " + failure + "\r\n");
            }
            else
            {
                string path = GetPath();
                bool wasSuccessful = AlbumArtExtractor.ExtractSingleSongAlbumArt(path);
                if (wasSuccessful) { Console.WriteLine("  Extraction and Save was successful."); }
                else { Console.WriteLine("  Extraction and Save failed."); }
            }
            Console.WriteLine("\r\n  Thank you for using Music Album Extractor \r\n");
        }

        private static void Welcome()
        {
            Console.WriteLine("\r\n  Welcome to Music Album Art Extractor by Rushakh (Github @rushakh).");
            Console.WriteLine("\r\n  This simple program will extract the Album Art / Cover Art from your Audio Files (works \r\n  with most conventional formats) and save them as PNG images to your chosen destination.");
            Console.WriteLine("\r\n  There are two Modes. Batch Mode and Song Mode.");
            Console.WriteLine("\r\n  Press any key to continue.\r\n");
            Console.ReadKey();
            Console.WriteLine("\r\n  Batch Mode => You can input a directory path (folder path) that contains the songs, and \r\n  all the songs's images are extracted.");
            Console.WriteLine("\r\n       Example => C:\\Music");
            Console.WriteLine("\r\n  Single Song Mode => Or you can input the location of a song ");
            Console.WriteLine("\r\n       Example => C:\\Music\\05 - Aviators - Victorious.mp3");
            Console.WriteLine("\r\n  Happy Extraction \r\n\r\n");
            Console.WriteLine("\r\n  Press any key to continue.\r\n");
            Console.ReadKey();
        }

        /// <summary>
        /// Checks to see if the user wants to continue using the app.
        /// </summary>
        /// <returns></returns>
        private static bool OnceAgain()
        {
            Console.WriteLine("\r\n  Would you like to use the program again? -type y or press Enter to go at it again and n \r\n   or anything else to exist.\r\n");
            string? answer = Console.ReadLine();
            if (answer is null || answer == string.Empty || answer == "y")
            {
                return true;                
            }
            else if (answer is not null && answer != string.Empty && answer == "n")
            {
                return false;
            }
            else 
            {
                return false;
            }
        }
        
        /// <summary>
        /// Batch Mode - Get the user's chosen directory which contains audio files.
        /// </summary>
        /// <returns></returns>
        private static string GetDirectory()
        {
            Console.WriteLine("\r\n  Please provide the location of the directory \r\n");
            var tempDirectory = Console.ReadLine();
            while(tempDirectory is null || tempDirectory == string.Empty || !System.IO.Directory.Exists(tempDirectory))
            {
                Console.WriteLine("\r\n  Invalid directory. Either the inputed directory does not exist or this program was \r\n   not granted access.");
                Console.WriteLine("\r\n  Please provide the location of the directory \r\n");
                tempDirectory = Console.ReadLine();
            }
            return tempDirectory;
        }

        /// <summary>
        /// Single Song Mode - Get the user's chosen audio file's path.
        /// </summary>
        /// <returns></returns>
        private static string GetPath()
        {
            Console.WriteLine("\r\n  Please provide the location of the audio file \r\n");
            var tempPath = Console.ReadLine();
            while (tempPath == null || tempPath == string.Empty || !System.IO.File.Exists(tempPath))
            {
                Console.WriteLine("\r\n  Invalid path. Either the inputed audio file path does not exist or this program was \r\n   not granted access.");
                Console.WriteLine("\r\n  Please provide the location of the audio file \r\n");
                tempPath = Console.ReadLine();
            }
            return tempPath;
        }

        /// <summary>
        /// Asks the user to decide between Batch mode and single Song mode, returns true for Batch Mode and false for Single Song Mode.
        /// </summary>
        /// <returns></returns>
        private static bool ModeAndInput()
        {
            ChooseMode();
            var mode = Console.ReadLine();
            bool isAcceotableInput = CheckIfAcceptableModeInput(mode);
            while (isAcceotableInput == false)
            {
                ChooseMode();
                mode = Console.ReadLine();
                isAcceotableInput = CheckIfAcceptableModeInput(mode);
            }
            if (mode == "b")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool CheckIfAcceptableModeInput(string? input)
        {
            if (input is null || input is int || input is double)
            {
                Console.WriteLine("\r\n  Invalid Input.\r\n");
                return false;
            }
            else if (input is string && input != string.Empty)
            { 
                if (input == "s" || input == "b")
                { 
                    return true;
                }
                else
                {
                    Console.WriteLine("\r\n  Invalid Input.\r\n");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private static void ChooseMode()
        {
            Console.WriteLine("\r\n  Batch Mode or Song Mode? -type b for Batch Mode and s for Single Song Mode \r\n");
        }

    }
}
