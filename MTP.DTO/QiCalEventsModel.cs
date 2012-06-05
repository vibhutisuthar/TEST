using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTP.DTO
{
    public class QiCalEventsModel
    {
        public string GDS { get; set; }
        public string RLOC { get; set; }
        public string PaxName { get; set; }
        public int SegNumber { get; set; }
        public string SegType { get; set; }
        public string DTStart { get; set; }
        public string DTEnd { get; set; }
        public string DCreated { get; set; }
        public string UID { get; set; }
        public string Subject { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string AltDescription { get; set; }
        public string EventText { get; set; }
        public DateTime? Stamp { get; set; }

    }
}



