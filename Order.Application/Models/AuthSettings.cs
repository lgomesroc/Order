using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Models
{
    public class AuthSettings
    {
        internal readonly char[] Secret;

        public string Secrets { get; set; }
        public int ExpireIn { get; set; }
    }
}
