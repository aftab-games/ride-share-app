namespace RideShare.Domain;

public class Rider
{
    public Guid Id { get; set; }
    public required string UserId { get; set; }
}