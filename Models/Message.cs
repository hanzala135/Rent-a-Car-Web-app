using System.ComponentModel.DataAnnotations;

namespace SmartCarRentACar.Models
{
public class Message
{
    public int Id { get; set; }

    [Required]
    public string SenderName { get; set; }

    [Required]
    public string Email { get; set; }

    public string Content { get; set; }
}
}
