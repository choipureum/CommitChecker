using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitalarmi
{
    class AutoGitProcess
    {
        private static string path = @"../../key.txt";
        private static string[] apiKey = File.ReadAllLines(path);
        private static string [] ids = { apiKey[4] };
        private static string toPhone = apiKey[5] ;
        private Logger log = new Logger();
        private gitAPI api = new gitAPI();
        public void AutoAlarmingStart()
        {
            try
            {
                foreach (string userName in ids)
                {
                    //커밋 오늘 안했을시 문자 발송
                    if (!api.CommitYN(userName))
                    {
                        string status = api.SendSMS(userName, toPhone);
                        Console.WriteLine(status);
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
