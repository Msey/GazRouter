using System.IO;
using GazRouter.Service.Exchange.Lib.Import;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace ExchangeTest
{
    [TestClass]
    public class FileToolsTests : TransactionTestsBase
    {
        [TestMethod ,TestCategory(UnStable)]
        public void MoveTest()
        {

            for (int i = 0; i < 50; i++)
            {
                var fileName = "1.txt";
                using (var writer = File.CreateText(fileName))
                {
                    writer.Flush();
                }
                var directory1 = FileTools.EnsureDirectoryCreated(@"\\10.240.5.120\c$\TEMP\Exchange\dir1");
                var directory2 = FileTools.EnsureDirectoryCreated(@"\\10.240.5.120\c$\TEMP\Exchange\dir2");
                FileTools.Move(fileName, directory1);
                var sourceFileName1 = Path.Combine(directory1, fileName);
                FileTools.Move(sourceFileName1, directory2);
            }
                
        }

        
    }
}