using System;

namespace GradingSystem
{
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException() : base() { }
        
        public InvalidScoreFormatException(string message) : base(message) { }
        
        public InvalidScoreFormatException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
