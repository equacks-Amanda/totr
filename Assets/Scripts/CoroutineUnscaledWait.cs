using System.Collections;
using UnityEngine;
public static class CoroutineUnscaledWait
 {
     public static IEnumerator WaitForSecondsUnscaled(float time)
     {
         float start = Time.realtimeSinceStartup;
         while (Time.realtimeSinceStartup < start + time)
         {
             yield return null;
         }
     }
 }