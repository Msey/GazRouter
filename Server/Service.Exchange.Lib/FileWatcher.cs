using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GazRouter.Log;

namespace GazRouter.Service.Exchange.Lib
{
    internal class FileWatcher
    {
        private readonly FileSystemWatcher _fileSystemWatcher;
        private string _watchDirectory;
        private Action<string> _action;

        public FileWatcher(string watchDirectory, Action<string> action)
        {
            _action = action;
            _watchDirectory = watchDirectory;

            _fileSystemWatcher = new FileSystemWatcher(watchDirectory)
            {
                EnableRaisingEvents = true,
                //NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size,
                IncludeSubdirectories = true,
            };
            _fileSystemWatcher.Created += OnFileCreated;
            //_fileSystemWatcher.Changed += (sender, args) =>
            //    {
            //        action(args.FullPath);
            //    };
            //    _fileSystemWatcher.Error += (sender, args) =>
            //    {
            //        var e = args.GetException();
            //        Error = e.Message;
            //    };
        }

        public string Error { get; set; }

        
    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        // Running the loop on another thread. That means the event
        // callback will be on the new thread. This can be omitted
        // if it does not matter if you are blocking the current thread.
        Task.Run(() =>
        {
            // Obviously some sort of timeout could be useful here.
            // Test until you can open the file, then trigger the CreeatedAndReleased event.
            while (!CanOpen(e.FullPath))
            {
                Thread.Sleep(200);
            }
            _action(e.FullPath);
        });
    }
    private static bool CanOpen(string file)
    {
        FileStream stream = null;
        try
        {
            stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
        }
        catch (IOException)
        {
            return false;
        }
        finally
        {
            stream?.Close();
        }
        return true;
    }

        public void Init()
        {
            try
            {
                _fileSystemWatcher.BeginInit();
                Directory.GetFiles(_watchDirectory, "*", SearchOption.AllDirectories).ToList().ForEach(_action);
                _fileSystemWatcher.EndInit();
            }
            catch (Exception e)
            {
                new MyLogger("exchangeLogger").WriteFullException(e, "test");
            }
        }

    }
}