using ChargifyNET.DotCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChargifyNET.DotCoreTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var chargify = new ChargifyConnect("https://subdomain.chargify.com/", "", "X");
            chargify.Test();
        }
    }
}
