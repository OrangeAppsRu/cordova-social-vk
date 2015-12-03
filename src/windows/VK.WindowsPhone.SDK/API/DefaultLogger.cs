using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API
{
    public class DefaultLogger : IVKLogger
    {
        public void Info(string info, params object[] formatParameters)
        {
            if (string.IsNullOrWhiteSpace(info))
            {
                return;
            }

            string strToLog = info;

            if (formatParameters != null && formatParameters.Length > 0)
            {
                strToLog = string.Format(info, formatParameters);
            }

            string debugLogMsg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + ": " + strToLog;

            WriteDebugString("INFO", strToLog);            
        }
       
        public void Warning(string warning)
        {
            if (string.IsNullOrWhiteSpace(warning))
            {
                return;
            }

            WriteDebugString("WARNING", warning);
        }

        public void Error(string error, Exception exc = null)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                return;
            }

            if (exc != null)
            {
                error += GetExceptionData(exc);
            }            

            WriteDebugString("ERROR", error);            
        }

        private void WriteDebugString(string logType, string strToLog)
        {           
            string debugLogMsg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + ": " +  logType + " " + strToLog;

            Debug.WriteLine(debugLogMsg.Substring(0, Math.Min(500, debugLogMsg.Length)));
        }
        
        private string GetExceptionData(Exception e)
        {
            string excData = "e.Message = " + e.Message + Environment.NewLine + "e.Stack = " + e.StackTrace;
            if (e.InnerException != null)
                return excData + Environment.NewLine + GetExceptionData(e.InnerException);
            return excData;
        }
    }
}
