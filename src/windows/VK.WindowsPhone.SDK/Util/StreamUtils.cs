
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.Util
{
    public static class StreamUtils
    {
        public static void CopyStream(Stream input, Stream output, Action<double> progressCallback = null)
        {
            Debug.WriteLine("STREAMUTILS.COPYSTREAM: " + input.Length);
            byte[] buffer = new byte[32768];
            int read;
            int totalcopied = 0;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
                totalcopied += read;

                Debug.WriteLine("STREAMUTILS.COPYSTREAM COPIED " + totalcopied + " out of " + input.Length);

                if (progressCallback != null)
                {
                    if (input.Length > 0)
                    {
                        progressCallback((((double)totalcopied) * 100) / input.Length);
                    }
                }
            }
        }

        public static MemoryStream ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            MemoryStream ms = new MemoryStream();

            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms;

        }

        public static byte[] ReadFullyToByteArray(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
