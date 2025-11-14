using System;
using System.ComponentModel.DataAnnotations;

namespace main_datacontext.Emails;

public class DbEmail
{
    [Key]
    public long Id { get; set; }

    [Required]
    public string SenderEmail { get; set; } = null!;
    public string? SenderName { get; set; }
    [Required]
    public string To { get; set; } = null!;
    public string? Cc { get; set; } = null!;
    public string? Bcc { get; set; } = null!;
    public string? Subject { get; set; }
    public string? HtmlContent { get; set; }
    [Required]
    public DateTime SentDate { get; set; }
}
