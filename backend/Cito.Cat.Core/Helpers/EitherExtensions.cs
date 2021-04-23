using LanguageExt;

// ReSharper disable InconsistentNaming

namespace Cito.Cat.Core.Helpers
{
    public static class EitherExtensions
    {
        /// <summary>
        /// By convention, the Left value is the 'error'-value in this project,
        /// so this helper function makes the check more explicit.
        /// </summary>
        /// <param name="either"></param>
        /// <typeparam name="L"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <returns></returns>
        public static bool IsError<L,R>(this Either<L,R> either) 
        {
            return either.IsLeft;
        }
    }
}