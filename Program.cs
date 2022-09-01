using System;
using System.IO;
using System.Net;
using Renci.SshNet;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace SSH
{
    public enum httpVerb
    {
        GET,
        POST,
        PUT,
        DELETE,

    }
    class RestClient
    {
        public string endPoint { get; set; }
        public httpVerb httpMethod { get; set; }

        public RestClient()
        {
            endPoint = string.Empty;
            
        }

        public string makeRequest()
        {
            string strResponseValue = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
            request.Method = httpMethod.ToString();
            using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException("Error van: " + response.StatusCode);
                }

            using (Stream responseStream = response.GetResponseStream())
                {
                    if(responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            strResponseValue = reader.ReadToEnd();
                        }
                    }
                }
            }

            return strResponseValue;
        }
    }


    class Program
    {
        static Renci.SshNet.SshCommand Connection(string host, int port, string username, string password)
        {
            var sshClient = new SshClient(host, port, username, password);

            if (!sshClient.IsConnected)
            {
                Console.WriteLine("Connecting .....");
                sshClient.Connect();
            }
            Console.WriteLine("Reading......");
            var readCommand = sshClient.RunCommand("dcrctl --wallet getstakeinfo");
            // Console.WriteLine(readCommand.Result);
            Console.WriteLine("Done.....");

            sshClient.Disconnect();

            return readCommand;
        }

        static int ProcessResults(string resultData)
        {
            string[] lines = resultData.Split('\n');
            int blockNumber = 0;
            foreach (string x in lines)
            {
                if (x.Length > 21)
                {
                    string y = x.Trim().Substring(1);
                    {
                        if (y.StartsWith("blockheight"))
                        {
                            string z = y.Substring(14, 6);
                            blockNumber = int.Parse(z);
                        }

                    }
                }
            }
            return blockNumber;
                                  
        }

        static int ProcessReference()
        {
            Console.WriteLine("Connecting .....");
            RestClient rClient = new RestClient();
            rClient.endPoint = "https://dcrdata.decred.org/api/block/best";
            string blockStringReference = rClient.makeRequest(); ;
            Console.WriteLine("Reading......");
            string blockNumberReferenceString = blockStringReference.Substring(10, 6);

            int realBlockNumber = Int32.Parse(blockNumberReferenceString);
            Console.WriteLine("Done......");

            return realBlockNumber;
        }

        /*
      static int ProcessReference()
        {
            int realBlockNumber = 0;

            RestClient rClient = new RestClient();
            rClient.endPoint = "https://dcrdata.decred.org/api/block/best/height";
            int blockNumberReference = 0;
            blockNumberReference = Int32.Parse(rClient.makeRequest());

            return blockNumberReference;
        }
        */

        static void DatabaseCommand(int blockNumberGermany, int blockNumberCanada, int blockNumberAustralia, int referenceBlockNumber, int syncAustralia, int syncCanada, int syncGermany)
        {

            MySqlConnection DecredDB = new MySqlConnection(@"Server = " + ConfigurationManager.AppSettings["databaseURL"] + "; Database = " + ConfigurationManager.AppSettings["databaseName"] + "; Uid = " + ConfigurationManager.AppSettings["databaseUsr"] + "; Pwd = " + ConfigurationManager.AppSettings["databasePwd"] + ";");

            DecredDB.Open();

            string germanyQuery = $"UPDATE instances SET BlockHeight = {blockNumberGermany}, BlockDifference = {syncGermany} WHERE ID = 1";
            string CanadaQuery = $"UPDATE instances SET BlockHeight = {blockNumberCanada}, blockDifference = {syncCanada} WHERE ID = 2";
            string AustraliaQuery = $"UPDATE instances SET BlockHeight = {blockNumberAustralia}, BlockDifference = {syncAustralia} WHERE ID = 3";
            string ReferenceQuery = $"UPDATE instances SET BlockHeight = {referenceBlockNumber} WHERE ID = 4";

            MySqlCommand cmdGermany = new MySqlCommand(germanyQuery, DecredDB);
            MySqlCommand cmdUSA = new MySqlCommand(CanadaQuery, DecredDB);
            MySqlCommand cmdUK = new MySqlCommand(AustraliaQuery, DecredDB);
            MySqlCommand cmdReference = new MySqlCommand(ReferenceQuery, DecredDB);

            cmdGermany.ExecuteNonQuery();
            cmdUSA.ExecuteNonQuery();
            cmdUK.ExecuteNonQuery();
            cmdReference.ExecuteNonQuery();

            DecredDB.Close();

        }
        static void Main(string[] args)
        {
            var readCommandGermany = Connection(ConfigurationManager.AppSettings["germanyURL"], Int32.Parse(ConfigurationManager.AppSettings["germanyPort"]), ConfigurationManager.AppSettings["germanyUsr"], ConfigurationManager.AppSettings["germanyPwd"]);
            var blockNumberGermany = ProcessResults(readCommandGermany.Result);
            string locationGermany = "Germany";
            Console.WriteLine(locationGermany + " : " + blockNumberGermany);

            var readCommandCanada = Connection(ConfigurationManager.AppSettings["canadaURL"], Int32.Parse(ConfigurationManager.AppSettings["canadaPort"]), ConfigurationManager.AppSettings["canadaUsr"], ConfigurationManager.AppSettings["canadaPwd"]);
            var blockNumberCanada = ProcessResults(readCommandCanada.Result);
            string locationUSA = "Canada";
            Console.WriteLine(locationUSA + " : " + blockNumberCanada);


            var readCommandAustralia = Connection(ConfigurationManager.AppSettings["australiaURL"], Int32.Parse(ConfigurationManager.AppSettings["australiaPort"]), ConfigurationManager.AppSettings["australiaUsr"], ConfigurationManager.AppSettings["australiaPwd"]);
            var blockNumberAustralia = ProcessResults(readCommandAustralia.Result);
            string locationUK = "Australia";
            Console.WriteLine(locationUK + " : " + blockNumberAustralia);

            var blockNumberReference = ProcessReference(); 
            Console.WriteLine(blockNumberReference);

            int syncCanada = blockNumberReference - blockNumberCanada;
            int syncGermany = blockNumberReference - blockNumberGermany;
            int syncAustralia = blockNumberReference - blockNumberAustralia;


            DatabaseCommand(blockNumberGermany, blockNumberCanada, blockNumberAustralia, blockNumberReference, syncAustralia, syncCanada, syncGermany);

            Console.ReadLine();







        }
    }
}
