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

        [BindProperty(SupportsGet = true)]
        public string action { get; set; }

        [BindProperty]
        public string piholeResponse{ get; set; }

        public void OnGet()
        {
            runBash();
        }

        public IActionResult OnPost()
        {
            runBash();
            return RedirectToPage("/Index" /*, new { action = action}*/);
        }
        private void runBash()
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                //action = "status";
            }
            /// https://www.michaco.net/blog/EnvironmentVariablesAndConfigurationInASPNETCoreApps
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Console.WriteLine($"action: {action}");
                string piholeResponse = BashCommand.run(action);
                Console.WriteLine($"piholeResponse: {piholeResponse}");
            }

        }
    }
}
