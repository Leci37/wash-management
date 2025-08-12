// Gestraf-Compliant Clean .NET 6 CQRS Template (Keycloak OIDC)
// ----------------------------------------------
// Structure: controlmat.Api / Application / Domain / Infrastructure
// Authentication: Keycloak OIDC (no local login endpoints)

// --- Domain Layer ---
namespace Controlmat.Domain.Entities
{
    public class ExampleEntity
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty; // From JWT claims
    }
}

namespace Controlmat.Domain.Interfaces
{
    public interface IExampleRepository
    {
        Task<IEnumerable<ExampleEntity>> GetAllAsync();
        Task AddAsync(ExampleEntity entity);
    }
}

// --- Application Layer ---
namespace Controlmat.Application.Common.Dto
{
    public class ExampleDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

namespace Controlmat.Application.Common.Validators
{
    using FluentValidation;

    public class ExampleDtoValidator : AbstractValidator<ExampleDto>
    {
        public ExampleDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        }
    }
}

namespace Controlmat.Application.Common.Mappings
{
    using AutoMapper;
    using Controlmat.Domain.Entities;
    using Controlmat.Application.Common.Dto;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ExampleEntity, ExampleDto>().ReverseMap();
        }
    }
}

namespace Controlmat.Application.Common.Queries.Example
{
    using MediatR;
    using Controlmat.Application.Common.Dto;
    using Controlmat.Domain.Interfaces;
    using Microsoft.AspNetCore.Http;

    public static class GetAllExamplesQuery
    {
        public class Request : IRequest<IEnumerable<ExampleDto>> {}

        public class Handler : IRequestHandler<Request, IEnumerable<ExampleDto>>
        {
            private readonly IExampleRepository _repo;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContext;

            public Handler(IExampleRepository repo, IMapper mapper, IHttpContextAccessor httpContext)
            {
                _repo = repo;
                _mapper = mapper;
                _httpContext = httpContext;
            }

            public async Task<IEnumerable<ExampleDto>> Handle(Request request, CancellationToken ct)
            {
                // Example: Get user info from JWT claims
                var userName = _httpContext.HttpContext?.User?.FindFirst("preferred_username")?.Value ?? "Unknown";
                
                var entities = await _repo.GetAllAsync();
                return _mapper.Map<IEnumerable<ExampleDto>>(entities);
            }
        }
    }
}

namespace Controlmat.Application.Common.Commands.Example
{
    using MediatR;
    using Controlmat.Application.Common.Dto;
    using Controlmat.Domain.Entities;
    using Controlmat.Domain.Interfaces;
    using Microsoft.AspNetCore.Http;

    public static class CreateExampleCommand
    {
        public class Request : IRequest<long>
        {
            public ExampleDto Dto { get; set; } = new();
        }

        public class Handler : IRequestHandler<Request, long>
        {
            private readonly IExampleRepository _repo;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContext;

            public Handler(IExampleRepository repo, IMapper mapper, IHttpContextAccessor httpContext)
            {
                _repo = repo;
                _mapper = mapper;
                _httpContext = httpContext;
            }

            public async Task<long> Handle(Request request, CancellationToken ct)
            {
                var entity = _mapper.Map<ExampleEntity>(request.Dto);
                
                // Get user info from JWT claims for auditing
                entity.CreatedBy = _httpContext.HttpContext?.User?.FindFirst("preferred_username")?.Value ?? "System";
                
                await _repo.AddAsync(entity);
                return entity.Id;
            }
        }
    }
}

// --- Infrastructure Layer ---
namespace Controlmat.Infrastructure.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Controlmat.Domain.Entities;

    public class ControlmatDbContext : DbContext
    {
        public DbSet<ExampleEntity> Examples => Set<ExampleEntity>();

        public ControlmatDbContext(DbContextOptions<ControlmatDbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ControlmatDbContext).Assembly);
        }
    }
}

namespace Controlmat.Infrastructure.Repositories
{
    using Controlmat.Domain.Entities;
    using Controlmat.Domain.Interfaces;
    using Controlmat.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class ExampleRepository : IExampleRepository
    {
        private readonly ControlmatDbContext _context;

        public ExampleRepository(ControlmatDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExampleEntity>> GetAllAsync()
        {
            return await _context.Examples.ToListAsync();
        }

        public async Task AddAsync(ExampleEntity entity)
        {
            _context.Examples.Add(entity);
            await _context.SaveChangesAsync();
        }
    }
}

// --- API Layer ---
namespace Controlmat.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using MediatR;
    using Controlmat.Application.Common.Queries.Example;
    using Controlmat.Application.Common.Commands.Example;
    using Controlmat.Application.Common.Dto;

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "WarehouseUser")] // Keycloak role requirement
    public class ExampleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExampleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _mediator.Send(new GetAllExamplesQuery.Request());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExampleDto dto)
        {
            var id = await _mediator.Send(new CreateExampleCommand.Request { Dto = dto });
            return CreatedAtAction(nameof(Get), new { id }, null);
        }
    }
}

// --- Program.cs (Keycloak OIDC Configuration) ---
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Controlmat.Infrastructure.Persistence;
using Controlmat.Domain.Interfaces;
using Controlmat.Infrastructure.Repositories;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// ✅ Database Context
builder.Services.AddDbContext<ControlmatDbContext>(options =>
    options.UseInMemoryDatabase("Controlmat"));

// ✅ Repository Registration
builder.Services.AddScoped<IExampleRepository, ExampleRepository>();

// ✅ MediatR Registration
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateExampleCommand.Handler).Assembly));

// ✅ AutoMapper Registration  
builder.Services.AddAutoMapper(typeof(Controlmat.Application.Common.Mappings.MappingProfile).Assembly);

// ✅ FluentValidation Registration
builder.Services.AddValidatorsFromAssemblyContaining<Controlmat.Application.Common.Validators.ExampleDtoValidator>();

// ✅ HttpContextAccessor for JWT Claims Access
builder.Services.AddHttpContextAccessor();

// ✅ Keycloak JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Keycloak realm configuration
        options.Authority = builder.Configuration["Keycloak:Authority"]; // e.g., https://keycloak.example.com/realms/sumisan
        options.Audience = builder.Configuration["Keycloak:Audience"];   // e.g., controlmat-api
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true, 
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            RoleClaimType = "roles", // Map to realm_access.roles from Keycloak
            NameClaimType = "preferred_username" // Use preferred_username as name claim
        };
    });

// ✅ Authorization with Role Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("WarehouseUser", policy =>
        policy.RequireRole("WarehouseUser"));
});

// ✅ CORS for Frontend Integration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:4200" })
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ✅ Controllers and API Documentation
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Add JWT Bearer support to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ✅ Development Environment Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Controlmat API V1");
    });
}

// ✅ Middleware Pipeline
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication(); // Must come before UseAuthorization
app.UseAuthorization();
app.MapControllers();

// ✅ Health Check (Optional)
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow })
   .AllowAnonymous();

app.Run();

/*
📋 Required appsettings.json configuration:

{
  "Keycloak": {
    "Authority": "https://your-keycloak-server/realms/controlmat",
    "Audience": "controlmat-api"
  },
  "AllowedOrigins": [
    "http://localhost:4200",
    "https://your-frontend-domain.com"
  ],
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1434;Database=CONTROLMAT;User Id=sa;Password=YourPassword!;TrustServerCertificate=true;"
  }
}

🔧 Keycloak Setup Required:
1. Create realm: 'controlmat'
2. Create client: 'controlmat-api' (Bearer-only)
3. Create role: 'WarehouseUser'
4. Create frontend client: 'controlmat-frontend' (Public client for OIDC)
5. Assign 'WarehouseUser' role to appropriate users
6. Configure CORS settings in Keycloak for frontend domain
*/
    