using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTP.DTO
{
    public class emailModel
    {
        public int ID { get; set; }
        public string email_code { get; set; }
        public string email_type { get; set; }
        public Int32 seq { get; set; }
        public string from_address { get; set; }
        public string from_name { get; set; }
        public string cc { get; set; }
        public string bcc { get; set; }
        public string subject { get; set; }
        public DateTime stamp { get; set; }
        public string access_criteria { get; set; }
        public string owner_market { get; set; }
        public string owner_region { get; set; }
        public string deployment { get; set; }
        public string txt_xslt_file { get; set; }
        public string html_xslt_file { get; set; }
    }
}
