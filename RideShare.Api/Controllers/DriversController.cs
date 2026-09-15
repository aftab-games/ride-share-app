using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideShare.Api.Contracts;
using RideShare.Domain;
using RideShare.Infrastructure;

namespace RideShare.Api.Controllers;

[ApiController]
[Route("api/drivers")]
public class DriversController : ControllerBase
{
    private readonly RideShareDbContext _db;

    public DriversController(RideShareDbContext db) => _db = db;

    [HttpPost]
    public async Task<ActionResult<DriverResponse>> Create(CreateDriverRequest request)
    {
        var driver = new Driver
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            VehicleInfo = request.VehicleInfo,
            Availability = DriverAvailability.Offline
        };
        _db.Drivers.Add(driver);
        await _db.SaveChangesAsync();

        var response = new DriverResponse(driver.Id, driver.UserId, driver.Availability.ToString(), driver.VehicleInfo);
        return CreatedAtAction(nameof(GetById), new { id = driver.Id }, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DriverResponse>> GetById(Guid id)
    {
        var driver = await _db.Drivers.FindAsync(id);
        if (driver is null) return NotFound();

        return new DriverResponse(driver.Id, driver.UserId, driver.Availability.ToString(), driver.VehicleInfo);
    }
}