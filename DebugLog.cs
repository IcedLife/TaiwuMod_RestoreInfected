using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddInfected
{
    internal class DebugLog
    {
        static public void debug(String message)
        {
            FileLog.Log(DateTime.Now.ToString("o")+ " | " + message);
        }

        public static void checkNull(Object obj, String varName)
        {
           String ret = obj == null ? "Null" : "Not Null";
            debug(varName + " is " + ret); 
        }
    }


}
