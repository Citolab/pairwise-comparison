using System;

namespace Cito.Cat.Core.Helpers
{
    public class DomainException : Exception
    {
        public readonly bool IsBadRequest;

        public DomainException(string message, bool isBadRequest) : base(message)
        {
            IsBadRequest = isBadRequest;
        }
    }
}