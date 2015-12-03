using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API
{
    public interface IVKLogger
    {
        void Info(string info, params object[] formatParameters);
        void Warning(string warning);
        void Error(string error, Exception exc = null);
    }

}
