using System;
using System.Threading.Tasks;

namespace Polymorphism4Unity.Editor.Utils
{
    internal static class ExceptionUtils
    {
        public static bool IsTaskCanceledException(this Exception e)
        {
            switch (e)
            {
                case TaskCanceledException:
                case AggregateException aggregateException when aggregateException.Flatten().InnerExceptions.Any<Exception, TaskCanceledException>():
                    return true;
                default:
                    return false;
            }

        }
    }
}