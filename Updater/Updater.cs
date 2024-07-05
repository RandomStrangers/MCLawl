using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
namespace MCLawlUpdater
{
    public static class Updater
    {
        class CustomWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                HttpWebRequest req = (HttpWebRequest)base.GetWebRequest(address);
                req.ServicePoint.BindIPEndPointDelegate = BindIPEndPointCallback;
                req.UserAgent = "MCLawlUpdater";
                return req;
            }
        }
        public static INetListen Listener = new TcpListen();
        static IPEndPoint BindIPEndPointCallback(ServicePoint servicePoint, IPEndPoint remoteEP, int retryCount)
        {
            IPAddress localIP;
            if (Listener.IP != null)
            {
                localIP = Listener.IP;
            }
            else if (!IPAddress.TryParse("0.0.0.0", out localIP))
            {
                return null;
            }
            if (remoteEP.AddressFamily != localIP.AddressFamily) return null;
            return new IPEndPoint(localIP, 0);
        }

        public static WebClient CreateWebClient() { return new CustomWebClient(); }
        public const string BaseURL = "https://github.com/RandomStrangers/MCLawl/raw/master/Uploads/";
        public static string dll = BaseURL + "MCLawl_.dll";
        public static string cli = BaseURL + "MCLawlCLI.exe";
        public static string exe = BaseURL + "MCLawl.exe";

        public static void PerformUpdate()
        {
            try
            {
                try
                {
                    DeleteFiles("MCLawl.update", "MCLawl_.update", "MCLawlCLI.update",
                        "prev_MCLawl.exe","prev_MCLawl_.dll", "prev_MCLawlCLI.exe");
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error deleting files:");
                    Console.WriteLine(e.ToString());
                    Console.ReadKey(false);
                    return;
                }
                    try
                    {
                        WebClient client = HttpUtil.CreateWebClient();
                        File.Move("MCLawl.exe", "prev_MCLawl.exe");
                        File.Move("MCLawlCLI.exe", "prev_MCLawlCLI.exe");
                        File.Move("MCLawl_.dll", "prev_MCLawl_.dll");
                        client.DownloadFile(dll, "MCLawl_.update");
                        client.DownloadFile(cli, "MCLawlCLI.update");
                        client.DownloadFile(exe, "MCLawl.update");

                }
                catch (Exception x) 
                    {
                        Console.WriteLine("Error downloading update:");
                        Console.WriteLine(x.ToString());
                        Console.ReadKey(false);
                        return;
                    }
                File.Move("MCLawl.update", "MCLawl.exe");
                File.Move("MCLawlCLI.update", "MCLawlCLI.exe");
                File.Move("MCLawl_.update", "MCLawl_.dll");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error performing update:");
                Console.WriteLine(ex.ToString());
                Console.ReadKey(false);
                return;
            }
        }
        static void DeleteFiles(params string[] paths)
        {
            foreach (string path in paths) { AtomicIO.TryDelete(path); }
        }
    }
}
