using System;

namespace GazRouter.Common
{
    public class ServerException : Exception
    {
        public ServerException(Exception exception) : base(string.Empty, exception)
        {}
    }
}
