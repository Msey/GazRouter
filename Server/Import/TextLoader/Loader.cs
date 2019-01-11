using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GazRouter.TextLoader
{
    public class ExtSystemTxtFile
    {
        const string ERR_FILE_NAME = "Error_";
        const int CyrillicCodePage = 1251;
        /// <summary>
        /// считывание из файла строк
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<string> ReadFile(string path)
        {
            var rawList = new List<string>();
            try{
                rawList = File.ReadAllLines(path).ToList();
            }
            catch (SystemException){ }
            return rawList;
        }
        /// <summary>
        /// считывание из файла строк в кирилической кодировке
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<string> ReadFileCyrillicEncoding(string path)
        {            
            var rawList = new List<string>();
            try{
                rawList = File.ReadAllLines(path, Encoding.GetEncoding(CyrillicCodePage)).ToList();
            }
            catch (SystemException){ }
            return rawList;
        }

        /// <summary>
        /// перенос файла в архив
        /// </summary>
        public void TransferFileToArchiveDirectory(string filename, string archFolder, bool nested = true)
        {
            try
            {
                if (!Directory.Exists(archFolder))
                {
                    var directoryInfo = Directory.CreateDirectory(archFolder);
                }
                var date = DateTime.Now.Date;
                var currDayPath = Path.Combine(archFolder, date.ToLongDateString());
                if (nested)
                {
                    currDayPath = Path.Combine(archFolder, date.Year.ToString());
                    if (!Directory.Exists(currDayPath))
                    {
                        Directory.CreateDirectory(currDayPath);
                    }
                    currDayPath = Path.Combine(currDayPath, date.Month.ToString());
                    if (!Directory.Exists(currDayPath))
                    {
                        Directory.CreateDirectory(currDayPath);
                    }
                    currDayPath = Path.Combine(currDayPath, date.Day.ToString());
                }
                DirectoryInfo directory;
                if (!Directory.Exists(currDayPath))
                {
                    directory = Directory.CreateDirectory(currDayPath);
                }
                else
                {
                    directory = new DirectoryInfo(currDayPath);
                }

                var fi = new FileInfo(filename);
                if (File.Exists(Path.Combine(directory.FullName, fi.Name)))
                {
                    File.Delete(Path.Combine(directory.FullName, fi.Name));
                }
                fi.MoveTo(Path.Combine(directory.FullName, fi.Name));
            }
            catch (Exception)
            {

            }

        }

        /// <summary>
        /// перенос файла в архив
        /// </summary>
        public void TransferFilesToArchiveDirectory(string[] filenames, string archFolder, bool nested = true)
        {
            filenames = filenames ?? new string[]{};
            foreach (var filename in filenames)
            {
                TransferFileToArchiveDirectory(filename, archFolder, nested);
            }
        }

        
    }
}
