using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        /// <summary>
        /// Gets the full file path of a file.
        /// </summary>
        /// <param name="filename">Name of the file.</param>
        /// <returns>The full path to the specified file.</returns>
        public static string FullFilePath(this string filename)
        {
            return ConfigurationManager.AppSettings["filePath"] + "\\" + filename;
        }

        /// <summary>
        /// Reads a given file and returns it as a list of strings.
        /// If the file does not exist, return an empty list of strings.
        /// </summary>
        /// <param name="file">The full path name to a file.</param>
        /// <returns>The lines in the given file.</returns>
        public static List<string> LoadFile(this string file)
        {
            // if file doesn't exist, return an empty list of strings
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        /// <summary>
        /// Convert the Prizes data lines from a text file to a list of PrizeModel.
        /// </summary>
        /// <param name="prizeFileLines">The lines of a text file with Prizes data.</param>
        /// <returns>The resulting list of PrizeModel.</returns>
        public static List<PrizeModel> ConvertToPrizeModels(this List<string> prizeFileLines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach (string line in prizeFileLines)
            {
                string[] cols = line.Split(',');

                // create a PrizeModel with the data from line
                PrizeModel p = new PrizeModel
                {
                    Id = int.Parse(cols[0]),
                    PlaceNumber = int.Parse(cols[1]),
                    PlaceName = cols[2],
                    PrizeAmount = decimal.Parse(cols[3]),
                    PrizePercentage = double.Parse(cols[4])
                };

                output.Add(p);
            }

            return output;
        }

        /// <summary>
        /// Converts prize models to a list of string and writes them to the specified file.
        /// </summary>
        /// <param name="prizes">List of prize models to be saved.</param>
        /// <param name="fileName">Name of file to be written to.</param>
        public static void SaveToPrizesFile(this List<PrizeModel> prizes, string fileName)
        {
            // convert prize models to lines in a list
            List<string> lines = new List<string>();

            foreach (PrizeModel p in prizes)
            {
                lines.Add($"{p.Id},{p.PlaceNumber},{p.PlaceName},{p.PrizeAmount},{p.PrizePercentage}");
            }

            // write all lines in the list to the text file
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
    }
}
