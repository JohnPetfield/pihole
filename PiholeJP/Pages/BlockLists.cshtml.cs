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
        }

        ///https://www.learnrazorpages.com/razor-pages/handler-methods
        public IActionResult OnPostAdd(string ListType)
        {
            EditBlockList(ListType, Address);

            ///  https://exceptionnotfound.net/implementing-post-redirect-get-in-asp-net-core-razor-pages/
            return RedirectToPage("/BlockLists");
        }

        /// https://www.codeproject.com/Articles/1207962/Simple-CRUD-Operation-with-Razor-Pages#:~:text=For%20adding%20Razor%20page%2C%20just,and%20click%20on%20Add%20button.
        public IActionResult OnGetDelete(string id, string listtype)
        {
            /*in html asp-route-listtype is asp-route-{varname}
              where i choose the variable and set the value in html
              then on the get here, it is passed is as a parameter 
              with the same varname */

            EditBlockList(listtype, id, true);

            ///  https://exceptionnotfound.net/implementing-post-redirect-get-in-asp-net-core-razor-pages/
            return RedirectToPage("/BlockLists");
        }

        public IActionResult OnGetSwap(string id, string listtype)
        {
            // Delete from current blocklist 
            EditBlockList(listtype, id, true);

            string otherList = (listtype == "w" ? "b" : "w");

            // Add domain to the other list
            EditBlockList(otherList, id);

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
                PiResponse = BashCommand.run(action);
            }
            Message = PiResponse;
        }

        private void GetAllBlockLists()
        {
            action = "sudo pihole -w -l";

            /// https://www.michaco.net/blog/EnvironmentVariablesAndConfigurationInASPNETCoreApps
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                PiResponse = BashCommand.run(action);
            }
            WhiteList = ParseResponses(PiResponse);

            action = "sudo pihole -b -l";

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                BlackList = new List<string>();
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
