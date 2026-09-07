using Microsoft.EntityFrameworkCore;

namespace RideShare.Domain;

[Owned]
public class Coordinate
{
    public decimal Lat { get; set; }
    public decimal Lng { get; set; }
}