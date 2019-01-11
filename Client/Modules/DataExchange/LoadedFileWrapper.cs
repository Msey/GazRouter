using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GazRouter.DTO.ASDU;

namespace DataExchange.ASDU
{
    public class LoadedFileWrapper
    {

        public static IList<LoadedFileWrapper> FromLoadedFiles(IEnumerable<LoadedFile> source)
        {
            return new List<LoadedFileWrapper>(source.Select(lf => new LoadedFileWrapper(lf)));
        }

        private readonly LoadedFile _loadedFile;

        public LoadedFile LoadedFile => _loadedFile;

        public LoadedFileWrapper(LoadedFile loadedFile)
        {
            _loadedFile = loadedFile;
        }

        private string GetStatusHumanReadable()
        {
            switch (_loadedFile.Status)
            {
                case LoadedFileStatus.Loaded:
                    return "Загружен в систему";
                case LoadedFileStatus.Read:
                    return "Выполнен разбор";
                case LoadedFileStatus.Applied:
                    return "Применен";
                case LoadedFileStatus.Error:
                    return "Ошибка";
                case LoadedFileStatus.XmlValidated:
                    return "XML проверен";
                case LoadedFileStatus.XmlValidationError:
                    return "Ошибка валидации XML";
                case LoadedFileStatus.RequestCreated:
                    return "Заявка создана";
                case LoadedFileStatus.RequestNameEdited:
                    return "Заявка отредактирована";
                case LoadedFileStatus.RequestXmlCreated:
                    return "Заявка сформирована";
                case LoadedFileStatus.RequestXmlError:
                    return "Ошибка при формировании";
                case LoadedFileStatus.RequestSent:
                    return "Отправлен транспортом";
                case LoadedFileStatus.InDir:
                    return "Получен при транспорте";
                default:
                    return "?";
            }
        }


        public string Key => _loadedFile.Key;
        public DateTime LoadDate => _loadedFile.LoadDate;
        public LoadedFileStatus Status => _loadedFile.Status;
        public string StatusDescription => GetStatusHumanReadable();
        public string FileName => _loadedFile.FileName;
        public string Name => _loadedFile.Name;
    }
}