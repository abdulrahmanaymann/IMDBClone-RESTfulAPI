using IMDbClone.Business.Mapper;
using IMDbClone.Business.Services;
using IMDbClone.Business.Services.IServices;
using IMDbClone.DataAccess.Data;
using IMDbClone.DataAccess.Repository;
using IMDbClone.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;

namespace IMDbClone.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling
                                = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            // Add services to the container.
            // Register the ApplicationDbContext with the DI container
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Register the AutoMapper with the DI container
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IMovieService, MovieService>();
            builder.Services.AddScoped<IRatingService, RatingService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
