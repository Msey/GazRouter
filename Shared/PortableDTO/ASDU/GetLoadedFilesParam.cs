namespace GazRouter.DTO.ASDU
{
    public enum LoadedFilesType
    {
        Input = 1,
        Output = 2
    }

    public class GetLoadedFilesParam
    {
        public LoadedFilesType LoadedFilesType { get; set; }

    }
}