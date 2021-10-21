using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Diagnostics;
using System.IO;
//using System.Linq;
using System.ServiceProcess;
//using System.Text;
//using System.Threading.Tasks;
using System.Timers;

namespace TOTVS_Analytics_Service
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer();

        String NameofService = "TOTVS-Analytics-Service";
        public Service1()
        {
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            WriteToFile(String.Format("{0} iniciado em {1}", NameofService, DateTime.Now));
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 10000; //number in miliseconds  
            timer.Enabled = true;

            String executiontimespath = AppDomain.CurrentDomain.BaseDirectory + "\\ExecutionTimes\\ExecutionTimes.txt";
            string[] lines = System.IO.File.ReadAllLines(executiontimespath);
            WriteToFile("Horarios configurados para execucao do Agent:");
            foreach (string line in lines)
            {
                WriteToFile(line);
            }       

            ExecuteAgent();
        }
        protected override void OnStop()
        {
            WriteToFile(String.Format("{0} finalizado em {1}", NameofService, DateTime.Now));
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            timer.Interval = 10000;
            WriteToFile(String.Format("{0} ativo em {1}", NameofService, DateTime.Now));
            String currentHour = DateTime.Now.ToShortTimeString();
            String executiontimespath = AppDomain.CurrentDomain.BaseDirectory + "\\ExecutionTimes\\ExecutionTimes.txt";
            string[] lines = System.IO.File.ReadAllLines(executiontimespath);
            foreach (string line in lines)
            {
                if (currentHour == line)
                {
                    ExecuteAgent();
                    WriteToFile("Aguarde 1 minuto para a retomada do log do servico");
                    timer.Interval = 60000;
                }
            }
        }
        public void ExecuteAgent()
        {
            WriteToFile(String.Format("Rotina de execucao do Agent acionada em {0}", DateTime.Now));
            WriteToFile("Verifique o log de execucao do Agent no arquivo agent.log.");
            var processInfo = new ProcessStartInfo();
            processInfo.FileName = "cmd.exe";
            processInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            processInfo.Arguments = "/c cd .. && cd .. && Run.bat";
            var process = Process.Start(processInfo);

        }
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
