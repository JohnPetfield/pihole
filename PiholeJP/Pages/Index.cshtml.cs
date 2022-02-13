using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            if(string.IsNullOrWhiteSpace(action))
            {
                //action = "status";
            }
            else
            {
                Console.WriteLine($"action: {action}");
                string piholeResponse = BashCommand.run(action);
                Console.WriteLine($"piholeResponse: {piholeResponse}");
            }
        }

        public void OnPost()
        {

        }
    }
}
