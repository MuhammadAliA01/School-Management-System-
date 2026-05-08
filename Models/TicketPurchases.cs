using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class TicketPurchases
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int EventID { get; set; }
        public int ParentID { get; set; }
        public int ticketsOwned { get; set; }


    }
}
