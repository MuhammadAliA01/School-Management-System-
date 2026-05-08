using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class TicketBooking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Connects to the Event
        [Required]
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public virtual EventCalendar EventCalendar { get; set; }

        // Connects to the Parent
        [Required]
        public int ParentId { get; set; }

        // Ensure your Parent model is actually named "Parent" and not "Parents"
        [ForeignKey("ParentId")]
        public virtual Parent Parent { get; set; }

        // The transaction details
        public int Quantity { get; set; }

        public decimal AmountPaid { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.Now;
    }
}
