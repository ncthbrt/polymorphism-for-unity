#nullable enable
using System;
using System.Threading.Tasks;

namespace Polymorphism4Unity.Editor.Utils
{
    internal static class TaskUtils
    {
        public static async Task SwallowCancellations(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception e) when (e.IsTaskCanceledException())
            {
                // ignored
            }
        }
        
        public static async Task<T?> SwallowCancellations<T>(this Task<T> task)
        {
            try
            {
                return await task;
            }
            catch (Exception e) when (e.IsTaskCanceledException())
            {
                return default;
            } 
        }
    }
}