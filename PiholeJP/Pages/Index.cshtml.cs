using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;

namespace PiholeJP.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        //[BindProperty(SupportsGet = true)]
        public string action { get; set; }

        [BindProperty]
        public string piholeResponse{ get; set; }

        public void OnGet()
        {
            if (action != "enable" && action != "disable")
            {
                action = "status";
            }

            //action = "status";
            runBash();
        }

        public void OnPost()
        {
            runBash();
            //return RedirectToPage("/Index" , new { piholeResponse = piholeResponse });
            //return RedirectToPage("/Index",
        }
        private void runBash()
        {
            //Console.WriteLine("runBash()");

            if (string.IsNullOrWhiteSpace(action))
            {
                //action = "status";
                piholeResponse = "action blank";
            }
            /// https://www.michaco.net/blog/EnvironmentVariablesAndConfigurationInASPNETCoreApps
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Console.WriteLine($"action: {action}");

                string _action = action.ToLower();

                //string action;
                if (_action == "enable")
                    action = "enable";
                else if (_action == "disable")
                    action = "disable";
                else action = "status";

                string command = "sudo pihole " + action;

                piholeResponse = BashCommand.run(command);
            }
            else piholeResponse = "On Windows - no action";
            
            if(piholeResponse.ToLower().Contains("enabling blocking") || 
               piholeResponse.ToLower().Contains("blocking already enabled") ||
               piholeResponse.ToLower().Contains("blocking is enabled"))
            {
                piholeResponse = "enabled";
            }
            else if (piholeResponse.ToLower().Contains("disabling blocking") ||
                     piholeResponse.ToLower().Contains("blocking already disabled") ||
                     piholeResponse.ToLower().Contains("blocking is disabled"))
            {
                piholeResponse = "disabled";
            }
            Console.WriteLine($"cleansed response: {piholeResponse}");
        }
    }
}
