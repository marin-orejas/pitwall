using Microsoft.EntityFrameworkCore;

namespace PitWall.Data;

public class PitWallDbContext(DbContextOptions<PitWallDbContext> options) : DbContext(options);
