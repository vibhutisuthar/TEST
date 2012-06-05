using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTP.DTO
{
    public class UsersModel
    {
        public int ID { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastLogin { get; set; }
        public string Status { get; set; }

        public string encodestring { get; set; }
    }
}
