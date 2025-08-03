using System;
using UnityEngine;

namespace Polymorphism4Unity.Editor.Utils
{
    public static class ActionUtils
    {
        public static void SafelyInvoke<T1>(this Action<T1> action, T1 t1)
        {
            try
            {
                action.Invoke(t1);
            }
            catch(Exception e)
            {
                Debug.LogError("Invoke Failed");
                Debug.LogException(e);
            }
        }
        
        public static void SafelyInvoke<T1, T2>(this Action<T1,T2> action, T1 t1, T2 t2)
        {
            try
            {
                action.Invoke(t1, t2);
            }
            catch(Exception e)
            {
                Debug.LogError("Invoke Failed");
                Debug.LogException(e);
            }
        }
        
        public static void SafelyInvoke<T1, T2, T3>(this Action<T1,T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            try
            {
                action.Invoke(t1, t2, t3);
            }   
            catch(Exception e)
            {
                Debug.LogError("Invoke Failed");
                Debug.LogException(e);
            }
        }
    }
}