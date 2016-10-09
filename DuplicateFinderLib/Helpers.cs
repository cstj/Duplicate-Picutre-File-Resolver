using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public static class Extentions
    {
        public static byte[] ComputeHash(this byte[] data)
        {
            byte[] hash = null;
            if (data != null)
            {
                using (System.Security.Cryptography.SHA512Managed hashAlgorithm = new System.Security.Cryptography.SHA512Managed())
                {
                    hash = hashAlgorithm.ComputeHash(data);
                }
            }
            return hash;
        }

        public static bool SequenceEqual(this byte[] data, System.IO.FileStream fs)
        {
            if (fs == null) return false;
            if (data == null) return false;
            if (data.LongLength != fs.Length) return false;
            //80 k buffer
            fs.Position = 0;
            int bufferSize = 80 * 1024;
            byte[] buffer = new byte[bufferSize];

            while (fs.Position < fs.Length)
            {
                long startPos = fs.Position;
                int bufferS;
                if (fs.Position + bufferSize < fs.Length)
                    bufferS = bufferSize;
                else
                    bufferS = Convert.ToInt32((fs.Length - fs.Position));

                fs.Read(buffer, 0, bufferS);

                for (int i = 0; i < bufferS; i++)
                {
                    if (data[startPos + i] != buffer[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
