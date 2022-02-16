using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiholeJP
{
    static public class BashCommand
    {
        static public string run(string _action)
        {
            //https://stackoverflow.com/questions/46419222/execute-linux-command-on-centos-using-dotnet-core

            _action = _action.ToLower();

            string action;
            if (_action == "enable")
                action = "enable";
            else if (_action == "disable")
                action = "disable";
            else action = "status";
            
            string command = "sudo pihole " + action;

            //Console.WriteLine($"BashCommand.run() - command: {command}");
            

            string result = "";
            using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
            {
                proc.StartInfo.FileName = "/bin/bash";
                proc.StartInfo.Arguments = "-c \" " + command + " \"";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.Start();

                result += proc.StandardOutput.ReadToEnd();
                result += proc.StandardError.ReadToEnd();

                //Console.WriteLine($"result: {result}"); 

                proc.WaitForExit();
            }

            return result;
        }
    }
}
