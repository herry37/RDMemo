using MongoDB.Driver;

namespace CrossPlatformDataAccess.Infrastructure.Exceptions
{
    public class CustomMongoWriteException : Exception
    {
        public CustomMongoWriteException(string message, MongoWriteException innerException)
            : base(message, innerException)
        {
        }
    }
}
