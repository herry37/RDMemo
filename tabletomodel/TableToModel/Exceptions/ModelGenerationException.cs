namespace TableToModel
{
    /// <summary>
    /// Model 生成例外
    /// </summary>
    public class ModelGenerationException : Exception
    {
        public ModelGenerationException(string message) : base(message) { }
        
        public ModelGenerationException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
