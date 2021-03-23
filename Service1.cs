using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;


namespace Gitalarmi
{
    public partial class Service1 : ServiceBase
    {
        private bool isRun = false;
        string _scheduleTime = "23";

        public Service1()
        {
            InitializeComponent();
         
            System.Timers.Timer aTimer = new System.Timers.Timer(60 * 60 * 1000);//60분마다 한번씩
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(aTimer_Elapsed);
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        void aTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //저녁 23시가량일 경우 프로그램 가동
            if (DateTime.Now.ToString("MM").Equals(_scheduleTime))
            {
                AutoAlarmingService();
            }
        }

        private void AutoAlarmingService()
        {
            Logger log = new Logger();
            log.WriteErrorLog("[" + DateTime.Now.ToString() + "] AUTO gitAlarmi 시작");

            isRun = true;

            AutoGitProcess info = new AutoGitProcess();
            info.AutoAlarmingStart();

            isRun = false;

            log.WriteErrorLog("[" + DateTime.Now.ToString() + "] AUTO gitAlarmi 종료");
        }
        protected override void OnStart(string[] args)
        {
            Logger log = new Logger();
            log.WriteErrorLog("[" + DateTime.Now.ToString() + "] 서비스 시작");
            AutoAlarmingService();
        }

        protected override void OnStop()
        {
            Logger log = new Logger();
            log.WriteErrorLog("[" + DateTime.Now.ToString() + "] 서비스 종료");
        }
    }
}
