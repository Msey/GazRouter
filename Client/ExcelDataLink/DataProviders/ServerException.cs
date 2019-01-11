using System;

namespace DataProviders
{
    public class ServerException : Exception
    {
        public ServerException(Exception exception) : base(string.Empty, exception)
        {}
    }
}
