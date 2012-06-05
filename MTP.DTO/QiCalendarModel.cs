using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTP.DTO
{
    public class QiCalendarModel
    {
        public string GDS { get; set; }
        public string RLOC { get; set; }
        public string PaxName { get; set; }
        public string Language { get; set; }
        public string FileLocation { get; set; }
        public DateTime? Stamp { get; set; }

    }
}
