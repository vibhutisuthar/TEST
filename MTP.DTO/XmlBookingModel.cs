using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTP.DTO
{
    //[Serializable]
    public class XmlBookingModel
    {
        //id,utid_id,phase,entry_date,stamp,xml
        public int id { get; set; }
        public int utid_id { get; set; }
        public string phase { get; set; }
        public DateTime entry_date { get; set; }
        public DateTime stamp { get; set; }
        public string xml { get; set; }
    }


    public class BookingModel
    {
        public string RLOC { get; set; }
        public string last_name { get; set; }
        public DateTime booking_date { get; set; }

    }
}


