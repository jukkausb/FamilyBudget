using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyBudget.Www.Models.Spa.Shared
{
    public class PageHeaderViewModel
    {
        public bool IsLoggedIn { get; set; }
        public string UserName { get; set; }
        public string LogoUrl { get; set; }
        public string LogoutUrl { get; set; }
    }
}