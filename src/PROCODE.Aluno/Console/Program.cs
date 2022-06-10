using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Console
{
    public class Program
    {
        public static Dataverse dataverse;

        static void Main(string[] args)
        {
            dataverse = new Dataverse();
        }
    }
}
