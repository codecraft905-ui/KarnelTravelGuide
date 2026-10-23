namespace KarnelTravelGuide.Models;

// One inquiry / feedback sent from the Contact page and read by the owner in Admin > Messages
public class ContactMessage
{
    public string Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Type { get; set; }          // Inquiry | Feedback
    public int? Rating { get; set; }          // Feedback only
    public string Text { get; set; }
    public string Destination { get; set; }   // optional
    public DateTime? TravelDate { get; set; } // optional
    public int? People { get; set; }          // optional
    public bool IsRead { get; set; }
    public ContactMessage Clone() => (ContactMessage)MemberwiseClone();
}
