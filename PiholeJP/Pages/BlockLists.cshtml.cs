using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PiholeJP.Pages
{
    public class BlockListsModel : PageModel
    {
        private readonly ILogger<BlockListsModel> _logger;

        public BlockListsModel(ILogger<BlockListsModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            GetAllBlockLists();
            //Message += "Run OnGet()";
            Console.WriteLine("Run OnGet()");
        }

        public IActionResult OnPost()
        {
            //Message += "Run OnPost()";
            Console.WriteLine($"TestVar {Address}");
            Console.WriteLine($"Run OnPost()");

            // Assuming only 'adding' to a block list for now 
            // until i sort out
            //AddToABlockList("b", TestVar);

            return RedirectToPage("/BlockLists");
        }

        ///https://www.learnrazorpages.com/razor-pages/handler-methods
        public IActionResult OnPostAdd(string ListType)
        {
            //Message += "Run OnPost()";
            Console.WriteLine($"TestVar {Address}");
            Console.WriteLine($"Run OnPostAdd()");

            // Assuming only 'adding' to a block list for now 
            // until i sort out
            AddToABlockList(ListType, Address);

            ///  https://exceptionnotfound.net/implementing-post-redirect-get-in-asp-net-core-razor-pages/
            return RedirectToPage("/BlockLists");
        }
        
        public string NewBlackListItem;
        public string NewWhiteListItem;
        [BindProperty]
        public string action { get; set; }
        public string PiResponse{ get; set; }
        public string Message{ get; set; }
        [BindProperty]
        public string Address{ get; set; }

        public List<string> WhiteList;
        public List<string> BlackList;

        private void AddToABlockList(string _listType, string address)
        {
            string listType = "b";
            if (_listType.ToLower() == "w")
                listType = "w";
            action = "sudo pihole -" + listType + " " + address;

            /// https://www.michaco.net/blog/EnvironmentVariablesAndConfigurationInASPNETCoreApps
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Console.WriteLine($"action: {action}");
                PiResponse = BashCommand.run(action);
                Console.WriteLine("response: " + PiResponse);
            }
            Message = PiResponse;
        }

        private void GetAllBlockLists()
        {
            action = "sudo pihole -w -l";

            /// https://www.michaco.net/blog/EnvironmentVariablesAndConfigurationInASPNETCoreApps
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Console.WriteLine($"action: {action}");
                PiResponse = BashCommand.run(action);
            }
            WhiteList = ParseResponses(PiResponse);

            action = "sudo pihole -b -l";

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                BlackList = new List<string>();
                Console.WriteLine($"action: {action}");
                PiResponse = BashCommand.run(action);
            }
            BlackList = ParseResponses(PiResponse);
        }

        private List<string> ParseResponses(string PiResponse)
        {
            int i = 0;
            List<string> returnList = new List<string>();
            foreach (var line in PiResponse.Split("\n").ToList())
            {
                if (i == 0)
                {
                    i++;
                    continue;
                }
                if (line == "" || line == null)
                    continue;
                i++;
                string[] s = line.Split(' ');
                returnList.Add(s[3].Trim());
            }
            return returnList;
        }
    }
}
