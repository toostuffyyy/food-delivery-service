using System;

namespace desktop.Models;

public class Review
{
    public int OrderId { get; set; }
    public int Rating { get; set; }
    public DateTime CreateDateTime { get; set; }
    public string? Comment { get; set; }
}