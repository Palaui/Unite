using System.IO;
using System.Net;

namespace UniteCore
{
    public class InternetConnectivityProtocol : Protocol
    {
        // Properties
        #region Properties

        public bool InternetConnection { get { return CheckInternetConnection(); } }
        public bool Required { get { return required; } }

        #endregion

        // Override
        #region Override

        void Awake()
        {
            Flag = ProtocolFlag.Started;
        }

        #endregion

        // Private
        #region Private

        private bool CheckInternetConnection()
        {
            string html = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://google.com");
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    bool success = (int)response.StatusCode < 299 && (int)response.StatusCode >= 200;
                    if (success)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            char[] cs = new char[100];
                            reader.Read(cs, 0, cs.Length);
                            foreach (char ch in cs)
                                html += ch;
                        }
                    }
                }
            }
            catch { return false; }

            return html.Contains("schema.org/WebPage");
        }

        #endregion

    }
}