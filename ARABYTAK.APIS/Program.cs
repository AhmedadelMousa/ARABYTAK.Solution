
using Arabytak.Core.Entities.Identity;
using Arabytak.Core.Repositories.Contract;
using Arabytak.Core.Service.Contract;
using Arabytak.Repository.Data;
using Arabytak.Repository.Data.Identity;
using Arabytak.Repository.Repository.Contract;
using Arabytak.Service;
using ARABYTAK.APIS.Errors;
using ARABYTAK.APIS.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace ARABYTAK.APIS
    {
        public class Program
        {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            builder.Services.AddScoped(typeof(IAuthService), typeof(AuthService));
            builder.Services.AddSwaggerGen(Options =>
            {

                Options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authentication",
                    Description = "Authentication Based On JWT Token Bearer <Your_Token>",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                Options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id   = "Bearer",
                            Type = ReferenceType.SecurityScheme,
                        }
                    },
                    new string[] { }

                }
            });
            });
        

            builder.Services.AddDbContext<ArabytakContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));// second Step to connection db , to use AddDbContext We need use take reference from Repository To read library
            builder.Services.AddDbContext<AppIdentityDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            }); 
            builder.Services.AddAutoMapper(typeof(MappingProfiles));
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.Configure<ApiBehaviorOptions>(option =>
            {
                option.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(p => p.Value.Errors.Count() > 0)
                                                         .SelectMany(p => p.Value.Errors)
                                                         .Select(E => E.ErrorMessage)
                                                         .ToList();
                    var Response = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(Response);
                };
            });
            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>();//register for service All for user manager and role (All Identity Service)
            builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,option=>
            {
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer= builder.Configuration["JWT:ValidIssure"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidateLifetime = true,
                    ClockSkew=TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:AuthKey"]??string.Empty))

                };
            } );

            var app = builder.Build();
                var scope=app.Services.CreateScope();//AddScope(PerRequest)
                var Services=scope.ServiceProvider;// That mean This service work in scoped(peer Request)
                var _dbContext= Services.GetRequiredService<ArabytakContext>();
                var _identityDbContext=Services.GetRequiredService<AppIdentityDbContext>();//create Object from AppIdentityDbContext
                var _userManager=Services.GetRequiredService<UserManager<AppUser>>();
                var LoggerFactory=Services.GetRequiredService<ILoggerFactory>();
                try
                {
                  await _dbContext.Database.MigrateAsync();//update DataBase
                  await ArabytakContextSeed.SeedAsync(_dbContext);// SeedData
                  await _identityDbContext.Database.MigrateAsync();
                  await AppIdentityDbContextSeed.SeedUserAsync(_userManager);//seed user 
                }
                catch (Exception ex) 
                {
                    var logger= LoggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An Error Occurred during migration");
                }


                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthentication();
                app.UseAuthorization();
                app.UseStaticFiles();


              app.MapControllers();

                app.Run();
            }
        }
    }
