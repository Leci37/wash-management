// Gestraf-Compliant Clean .NET 6 CQRS Template
// ----------------------------------------------
// Structure: controlmat.Api / Application / Domain / Infrastructure

// --- Domain Layer ---
namespace Controlmat.Domain.Entities
{
    public class ExampleEntity
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
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

    public static class GetAllExamplesQuery
    {
        public class Request : IRequest<IEnumerable<ExampleDto>> {}

        public class Handler : IRequestHandler<Request, IEnumerable<ExampleDto>>
        {
            private readonly IExampleRepository _repo;
            private readonly IMapper _mapper;

            public Handler(IExampleRepository repo, IMapper mapper)
            {
                _repo = repo;
                _mapper = mapper;
            }

            public async Task<IEnumerable<ExampleDto>> Handle(Request request, CancellationToken ct)
            {
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

            public Handler(IExampleRepository repo, IMapper mapper)
            {
                _repo = repo;
                _mapper = mapper;
            }

            public async Task<long> Handle(Request request, CancellationToken ct)
            {
                var entity = _mapper.Map<ExampleEntity>(request.Dto);
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
    using MediatR;
    using Controlmat.Application.Common.Queries.Example;
    using Controlmat.Application.Common.Commands.Example;
    using Controlmat.Application.Common.Dto;

    [ApiController]
    [Route("api/[controller]")]
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


// --- Program.cs ---
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ControlmatDbContext>(options =>
    options.UseInMemoryDatabase("Controlmat"));

builder.Services.AddScoped<IExampleRepository, ExampleRepository>();
builder.Services.AddMediatR(typeof(CreateExampleCommand.Handler).Assembly);
builder.Services.AddAutoMapper(typeof(Controlmat.Application.Common.Mappings.MappingProfile).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<Controlmat.Application.Common.Validators.ExampleDtoValidator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
