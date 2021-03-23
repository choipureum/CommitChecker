using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitalarmi
{
    class Logger
    {
        private string path = "";
        private static string rootPath = "C:\\gitAlarmiLog";

        private static string LogPath
        {
            get
            {
                return rootPath;
            }
        }

        public Logger()
        {
            this.path = LogPath + DateTime.Today.ToString("yyyyMM") + "\\";

            if (Directory.Exists(this.path) == false)
            {
                Directory.CreateDirectory(this.path);
            }

            this.path += DateTime.Today.ToString("yyyyMMdd") + ".cart_auto_order.log";
        }

        public void WriteErrorLog(string strLog)
        {
            try
            {
                StreamWriter sw = new StreamWriter(this.path, true, Encoding.Default);
                sw.WriteLine(strLog + "\n");
                sw.Close();
            }
            catch (Exception)
            {
            }
        }
    }
}
