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
    }
}
