using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTP.DTO
{
    public class MailRecordsModel
    {
        public int ID { get; set; }
        public string RLOC { get; set; }
        public string Subject { get; set; }
        public string FromName { get; set; }
        public string SenderName { get; set; }
        public string ToName { get; set; }
        public string Received { get; set; }
        public DateTime DeliveryTime { get; set; }
        public string Contents { get; set; }
        public string Agent { get; set; }
        public string Method { get; set; }
        public int? Processor { get; set; }
        public string ProcessorName { get; set; }
        public string FormName { get; set; }
        public int? PreprocStatus { get; set; }
        public string TicketNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerReference { get; set; }
        public string Company { get; set; }
        public string Category { get; set; }
        public string TripRequest { get; set; }
        public string FileName { get; set; }
        public string Pseudo { get; set; }
        public string Team { get; set; }
        public int Processed { get; set; }
        public int ItinType { get; set; }
        public int BounceEmailID { get; set; }
        public DateTime? Created { get; set; }
        public DateTime time_stamp { get; set; }

    }
}
