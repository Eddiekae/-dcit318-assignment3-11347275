using System;

namespace GradingSystem
{
    public class MissingFieldException : Exception
    {
        public MissingFieldException() : base() { }
        
        public MissingFieldException(string message) : base(message) { }
        
        public MissingFieldException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
