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
            Console.WriteLine("Run OnGet()");
        }
        /*
        public IActionResult OnPost()
        {
            Console.WriteLine($"TestVar {Address}");
            Console.WriteLine($"Run OnPost()");
            return RedirectToPage("/BlockLists");
        }*/

        ///https://www.learnrazorpages.com/razor-pages/handler-methods
        public IActionResult OnPostAdd(string ListType)
        {
            Console.WriteLine($"TestVar {Address}");
            Console.WriteLine($"Run OnPostAdd()");

            EditBlockList(ListType, Address);

            Console.WriteLine("about to redirect to GET");
            ///  https://exceptionnotfound.net/implementing-post-redirect-get-in-asp-net-core-razor-pages/
            return RedirectToPage("/BlockLists");
        }

        /// https://www.codeproject.com/Articles/1207962/Simple-CRUD-Operation-with-Razor-Pages#:~:text=For%20adding%20Razor%20page%2C%20just,and%20click%20on%20Add%20button.
        public IActionResult OnGetDelete(string id, string listtype)
        {
            /*so in html asp-route-listtype is asp-route-{varname}
              where i choose the variable and set the value in html
              then on the get here, it is passed is as a parameter 
              with the same varname */
            Console.WriteLine($"Run OnGetDelete()");
            Console.WriteLine($"URL {id} listtype {listtype} ");

            EditBlockList(listtype, id, true);

            Console.WriteLine("about to redirect to GET");
            ///  https://exceptionnotfound.net/implementing-post-redirect-get-in-asp-net-core-razor-pages/
            return RedirectToPage("/BlockLists");
        }

        public IActionResult OnGetSwap(string id, string listtype)
        {
            Console.WriteLine($"Run OnGetSwap()");
            Console.WriteLine($"URL {id} listtype {listtype} ");

            // Delete from current blocklist 
            EditBlockList(listtype, id, true);

            string otherList = (listtype == "w" ? "b" : "w");

            // Add domain to the other list
            EditBlockList(otherList, id);

            Console.WriteLine("about to redirect to GET");
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

        private void EditBlockList(string _listType, string address, bool delete = false)
        {
            string listType = "b";
            if (_listType.ToLower() == "w")
                listType = "w";
            action = "sudo pihole -" + listType + 
                (delete?" -d ": " ")
                + address;

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
