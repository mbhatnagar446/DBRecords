using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseHelper;
using System.IO;
namespace DBRecords
{
    class Program
    {
        static void Main(string[] args)
        {

            string fileName = args[0];
            
            Dictionary<string, string> addresses = new Dictionary<string, string>();

            using (var reader = new StreamReader(File.OpenRead(fileName)))
            {
                string directEmailAddress;
                string forwardingAddress;
                 
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    directEmailAddress = values[0];
                    forwardingAddress = values[1];
                    addresses.Add(directEmailAddress, forwardingAddress);
                }
            }
            //foreach (var key in addresses.Keys)
            //{
            //     Console.WriteLine(key + ":" + addresses[key]);
                 
            //}

            //addresses.Add("Test1233341@test4.directaddress.net", "orgAdvanceFRWDto@test4.directaddress.net");
  
             DataEntityHelper.insertRecords(addresses);

        }
    }
}
