using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WhetherData
{
    public class DataFileReader
    {
        /// <summary>
        /// Reads every line of a csv file and uploads every object to the database
        /// </summary>
        /// <param name="filePath"></param>
        public void UploadFromFile(string filePath)
        {
            Log log;
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                string[] tempArray;
                string[] tempArray2;
                using (EFContext context = new EFContext())
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        tempArray = line.Split(',');

                        //This takes up to much memory
                        tempArray2 = tempArray[2].Split('.');
                        string test = $"{tempArray2[0]},{tempArray2[1]}";

                        log = new Log(DateTime.Parse(tempArray[0]), tempArray[1], double.Parse(test), double.Parse(tempArray[3]));
                        context.Add(log);
                    }
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Opens the file explorer and lets the user select a file
        /// </summary>
        /// <returns>The file path of the selected file</returns>1
        public string GetFilePath()
        {
            string filePath;
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                filePath = ofd.FileName;
                return filePath;
            }
            throw new Exception("Unable to get file");
        }
    }
}
