using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
namespace ConsoleApplication1
{
    class Program
    {
        /// <summary>
        /// Together with the AcceptAllCertifications method right
        /// below this causes to bypass errors caused by SLL-Errors.
        /// </summary>
        public static void IgnoreBadCertificates()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);
        }

        /// <summary>
        /// In Short: the Method solves the Problem of broken Certificates.
        /// Sometime when requesting Data and the sending Webserverconnection
        /// is based on a SSL Connection, an Error is caused by Servers whoes
        /// Certificate(s) have Errors. Like when the Cert is out of date
        /// and much more... So at this point when calling the method,
        /// this behaviour is prevented
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certification"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns>true</returns>
        private static bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        /*
         * ********** Function to verfiy one email ****************************************
         */
        public static void verifyByOne()
        {
            string key = "PUT YOUR KEY HERE";
            string email = "emailtoverify@example.com";
            string sURL = "https://apps.emaillistverify.com/api/verifEmail?secret=" + key + "&email=" + email;
            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);

            WebProxy myProxy = new WebProxy("myproxy", 80);
            myProxy.BypassProxyOnLocal = true;

            wrGETURL.Proxy = WebProxy.GetDefaultProxy();

            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);

            string sLine = "";
            int i = 0;

            while (sLine != null)
            {
                i++;
                sLine = objReader.ReadLine();
                if (sLine != null)
                    Console.WriteLine("{0}:{1}", i, sLine);
            }
            Console.ReadLine();
        }
        /*
         * ********** Function to check state after upload file****************************************
         */
        private static void CheckFileStatus(string id)
        {
            string key = "PUT YOUR KEY HERE";
            string sURL = "https://apps.emaillistverify.com/api/getApiFileInfo?secret=" + key + "&id=" + id;
            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);

            WebProxy myProxy = new WebProxy("myproxy", 80);
            myProxy.BypassProxyOnLocal = true;

            wrGETURL.Proxy = WebProxy.GetDefaultProxy();

            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);

            string sLine = "";
            int i = 0;

            while (sLine != null)
            {
                i++;
                sLine = objReader.ReadLine();
                if (sLine != null)
                    Console.WriteLine("{0}:{1}", i, sLine);
            }
        }
        /*
         * ******************************************************************************
         */ 
        static void Main(string[] args)
        {
            //verifyByOne();
            //string file_contents ="@/home/Downloads/emails.txt"; //path to your file
            //string url = "http://ukr.net/api/verifApiFile?secret=" + key + "&filename=my_emails.txt";

            //I use a method to ignore bad certs caused by misc errors
            IgnoreBadCertificates();

            string key = "PUT YOUR KEY HERE";
            
            string file_contents = "c:\\php\\emails.txt";
            string url = "https://apps.emaillistverify.com/api/verifApiFile?secret=" + key + "&filename=my_emails.txt";            
            
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            //request.ServicePoint.Expect100Continue = false;
            request.KeepAlive = false;

            string header = "--" + boundary + "\r\n" + "Content-Disposition: form-data; name=\"file_contents\"; filename=\""+file_contents+"\"\r\nContent-Type: application/octet-stream\r\n\r\n";
                                
            string trailer = "\r\n--" + boundary + "--\r\n";

            //name="file_contents"; filename="c:\php\emails.txt"

            byte[] bytesHeader = Encoding.ASCII.GetBytes(header);
            byte[] bytesFileContent = File.ReadAllBytes(file_contents);
            byte[] bytesTrailer = Encoding.ASCII.GetBytes(trailer);

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytesHeader, 0, bytesHeader.Length);
            requestStream.Write(bytesFileContent, 0, bytesFileContent.Length);
            requestStream.Write(bytesTrailer, 0, bytesTrailer.Length);
            requestStream.Close();

            try
            {
                using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    string id = reader.ReadToEnd();
                    CheckFileStatus(id);

                };
            }
            catch (Exception ex)
            {
                // something strange...
            }

        }
    }
}
