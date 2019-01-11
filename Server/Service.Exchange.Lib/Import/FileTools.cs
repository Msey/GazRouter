using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib.Cryptography;

namespace GazRouter.Service.Exchange.Lib.Import
{
    public static class FileTools
    {
        private const string ERR_FILE_NAME = "Error_";
        private const int CyrillicCodePage = 1251;
        private const int SleepTimeout = 500;

        public static string ReadFile(string fullPath)
        {
            var result = AttemptableFileAction.Execute(fullPath, path =>
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            });
            return result;
        }

        public static void Copy(string sourceFilePath, string directory, int attempts = 5)
        {
            var fileName = Path.GetFileName(sourceFilePath);
            var count = attempts;
            while (IsFileInUse(sourceFilePath) || count <= 0)
            {
                Thread.Sleep(SleepTimeout);
                count--;
                if (count < 0)
                    break;
            }
            if (count >= 0)
            {
                var destFileName = Path.Combine(directory, fileName);
                if (File.Exists(destFileName))
                    destFileName = Path.Combine(directory, "_" + fileName);
                try
                {
                    File.Copy(sourceFilePath, destFileName);
                }
                catch (Exception e)
                {
                    new MyLogger("exchangeLogger").WriteFullException(e, e.Message);
                }
            }
            else
            {
                new MyLogger("exchangeLogger").Error($@"Не удалось скопировать файл: {sourceFilePath}");
            }
        }

        public static void Move(string sourceFilePath, string directory, int attempts = 5)
        {
            var fileName = Path.GetFileName(sourceFilePath);
            var destFileName = Path.Combine(directory, fileName);
            EnsureDelete(destFileName);

            var count = attempts;
            while (IsFileInUse(sourceFilePath) || count <= 0)
            {
                Thread.Sleep(SleepTimeout);
                count--;
            }
            try
            {
                File.Move(sourceFilePath, destFileName);
            }
            catch (Exception e)
            {

                new MyLogger("exchangeLogger").WriteFullException(e, e.Message);
            }
        }

        public static void EnsureDelete(string filePath, int attempts = 5)
        {
            if (!File.Exists(filePath)) return;

            var count = attempts;
            while (IsFileInUse(filePath) || count <= 0)
            {
                Thread.Sleep(SleepTimeout);
                count--;
            }

            try
            {
                File.Delete(filePath);
            }
            catch (Exception e)
            {
                new MyLogger("exchangeLogger").WriteFullException(e, e.Message);
            }
        }

        public static void EnsureDelete(List<string> filePathes, int attempts = 5)
        {
            foreach (var filePath in filePathes)
            {
                EnsureDelete(filePath, attempts);
            }
        }


        public static bool IsFileInUse(string path)
        {
            try
            {
                using (File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
        }

        public static string EnsureDirectoryCreated(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
            return directoryPath;
        }

        /// <summary>
        ///     считывание из файла строк в кирилической кодировке
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> ReadFileCyrillicEncoding(string path)
        {
            var rawList = new List<string>();
            try
            {
                rawList = File.ReadAllLines(path, Encoding.GetEncoding(CyrillicCodePage)).ToList();
            }
            catch (SystemException)
            {
            }
            return rawList;
        }

        /// <summary>
        ///     перенос файла в архив
        /// </summary>
        public static void TransferFileToArchiveDirectory(string filename, string archFolder, bool nested = true, bool notProcessed = false, bool crypted = false)
        {
            try
            {
                var date = DateTime.Now.Date;
                var folder = Path.Combine(archFolder, date.ToLongDateString());
                if (nested)
                {
                    folder = Path.Combine(archFolder, date.Year.ToString(), date.Month.ToString(), date.Day.ToString());
                }
                if (notProcessed)
                {
                    folder = Path.Combine(folder, "NotProcessed");
                }
                ExchangeHelper.EnsureDirectoryExist(folder);
                var directory = new DirectoryInfo(folder);

                var fi = new FileInfo(filename);
                if (crypted)
                {
                    byte[] buffer;
                    using (var fs = FileTools.OpenOrCreate(fi.FullName))
                    {
                        buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, (int) fs.Length);
                        fs.Flush();
                        buffer = Cryptoghraphy.Decrypt(buffer);
                        fs.Position = 0;
                        fs.Write(buffer, 0, buffer.Length);
                        fs.Flush();
                    }

                }
                Move(fi.FullName, directory.FullName);
            }
            catch (Exception e)
            {
                new MyLogger("exchangeLogger").WriteFullException(e, e.Message);
            }
        }

        /// <summary>
        ///     перенос файла в архив
        /// </summary>
        public static void TransferFilesToArchiveDirectory(List<string> filenames, string archFolder, bool nested = true)
        {
            filenames = filenames ?? new List<string>();
            foreach (var filename in filenames)
            {
                TransferFileToArchiveDirectory(filename, archFolder, nested);
            }
        }

        public static string CopyFileToArchiveDirectory(string filename, string archFolder, bool nested = true, bool notProcessed = false, bool crypted = false)
        {
            try
            {
                var date = DateTime.Now.Date;
                var folder = Path.Combine(archFolder, date.ToLongDateString());
                if (nested)
                {
                    folder = Path.Combine(archFolder, date.Year.ToString(), date.Month.ToString(), date.Day.ToString());
                }
                if (notProcessed)
                {
                    folder = Path.Combine(folder, "NotProcessed");
                }
                ExchangeHelper.EnsureDirectoryExist(folder);
                var directory = new DirectoryInfo(folder);

                var fi = new FileInfo(filename);
                if (crypted)
                {
                    byte[] buffer;
                    using (var fs = FileTools.OpenOrCreate(fi.FullName))
                    {
                        buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, (int)fs.Length);
                        fs.Flush();
                        buffer = Cryptoghraphy.Decrypt(buffer);
                        fs.Position = 0;
                        fs.Write(buffer, 0, buffer.Length);
                        fs.Flush();
                    }

                }
                Copy(fi.FullName, directory.FullName);
                return Path.Combine(directory.FullName, Path.GetFileName(fi.FullName));
            }
            catch (Exception e)
            {
                new MyLogger("exchangeLogger").WriteFullException(e, e.Message);
                return null;
            }
        }


        public static FileStream OpenOrCreate(string fullPath, FileMode mode = FileMode.OpenOrCreate)
        {
            return AttemptableFileAction.Execute(fullPath, path => File.Open(path, FileMode.OpenOrCreate));
        }

        public static string EncodeTo64(string toEncode)
        {
            try
            {
                var bytes = ASCIIEncoding.ASCII.GetBytes(toEncode);
                return Convert.ToBase64String(bytes);
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }

    public class AttemptableFileAction
    {
        private const int Attempts = 5;
        private const int SleepTimeout = 500;

        private static bool IsFileInUse(string path)
        {
            try
            {
                using (File.Open(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException e)
            {
                return true;
            }
        }

        public static T Execute<T>(string filePath, Func<string, T> func)
        {
            var attempts = Attempts;
            while (IsFileInUse(filePath))
            {
                Thread.Sleep(SleepTimeout);
                if (--attempts < 0) throw new InvalidOperationException($@"File in use: {filePath}");
            }

            return func(filePath);
        }
    }
}