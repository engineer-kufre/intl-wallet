using System;
using System.Collections.Generic;
using System.Text;

namespace IntlWallet.Models
{
    public class Page
    {
        public bool success { get; set; }
        public int timestamp { get; set; }
        public string @base { get; set; }
        public string date { get; set; }
        public Rates rates { get; set; }
    }
}
