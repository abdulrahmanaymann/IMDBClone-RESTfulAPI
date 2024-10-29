using IMDbClone.Common;
using IMDbClone.Core.Models;
using IMDbClone.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IMDbClone.DataAccess.DbInitializer
{
    public class DbInitializer : IDBInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DbInitializer> _logger;
        private readonly IConfiguration _configuration;

        private readonly string _adminUserName;
        private readonly string _adminEmail;
        private readonly string _adminName;
        private readonly string _adminPassword;

        public DbInitializer(
              UserManager<ApplicationUser> userManager,
              RoleManager<IdentityRole> roleManager,
              ApplicationDbContext context,
              ILogger<DbInitializer> logger,
              IConfiguration configuration)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _adminUserName = _configuration["AdminUser:UserName"] ?? throw new ArgumentNullException(nameof(_adminUserName));
            _adminEmail = _configuration["AdminUser:Email"] ?? throw new ArgumentNullException(nameof(_adminEmail));
            _adminName = _configuration["AdminUser:Name"] ?? throw new ArgumentNullException(nameof(_adminName));
            _adminPassword = _configuration["AdminUser:Password"] ?? throw new ArgumentNullException(nameof(_adminPassword));
        }


        public async Task Initialize()
        {
            await ApplyMigrationsAsync();
            await CreateRolesAsync();
            await SeedMoviesAsync();
        }

        private async Task ApplyMigrationsAsync()
        {
            try
            {
                if ((await _context.Database.GetPendingMigrationsAsync()).Any())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying migrations.");
            }
        }

        private async Task CreateRolesAsync()
        {
            try
            {
                if (!await _roleManager.RoleExistsAsync(Roles.User))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Roles.User));
                    await _roleManager.CreateAsync(new IdentityRole(Roles.Admin));

                    var adminUser = new ApplicationUser
                    {
                        UserName = _adminUserName,
                        Email = _adminEmail,
                        Name = _adminName
                    };

                    var result = await _userManager.CreateAsync(adminUser, _adminPassword);
                    if (!result.Succeeded)
                    {
                        _logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                        return;
                    }

                    var user = await _context.ApplicationUsers
                                .FirstOrDefaultAsync(u => u.Email == _adminEmail);

                    if (user != null)
                    {
                        await _userManager.AddToRoleAsync(user, Roles.Admin);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating roles.");
            }
        }

        private async Task SeedMoviesAsync()
        {
            try
            {
                // Check if the Movies table is empty
                if (!await _context.Movies.AnyAsync())
                {
                    var movies = await LoadDataFromJsonAsync();

                    if (movies.Count != 0)
                    {
                        // Disable change tracking
                        _context.ChangeTracker.AutoDetectChangesEnabled = false;

                        using var transaction = await _context.Database.BeginTransactionAsync();
                        try
                        {
                            int batchSize = 1000;
                            for (int i = 0; i < movies.Count; i += batchSize)
                            {
                                var batch = movies.Skip(i).Take(batchSize).ToList();
                                await _context.Movies.AddRangeAsync(batch);
                                await _context.SaveChangesAsync();
                                _logger.LogInformation("{Count} movies seeded.", batch.Count);
                            }
                            await transaction.CommitAsync(); // Commit the transaction
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync(); // Rollback if there's an error
                            _logger.LogError(ex, "Error seeding movies.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No movies found to seed.");
                    }
                }
                else
                {
                    _logger.LogInformation("Movies table already contains data. Skipping seeding.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding movies.");
            }
            finally
            {
                // Re-enable change tracking
                _context.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        private async Task<List<Movie>> LoadDataFromJsonAsync()
        {
            try
            {
                var dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SeedData");
                var moviesJsonPath = Path.Combine(dataDirectory, "movies.json");

                if (!File.Exists(moviesJsonPath))
                {
                    _logger.LogError("File not found: {MoviesJsonPath}", moviesJsonPath);
                    return new List<Movie>();
                }

                var moviesJson = await File.ReadAllTextAsync(moviesJsonPath);
                var movies = JsonConvert.DeserializeObject<List<Movie>>(moviesJson) ?? new List<Movie>();

                return movies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading data from JSON.");
                return new List<Movie>();
            }
        }
    }
}