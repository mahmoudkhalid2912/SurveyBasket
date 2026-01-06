





namespace SurveyBasket.Api;

public static class DependencyInJection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddControllers();
        // Swagger and Mapster and FluentValidation
        services.AddSwagerConfig().
            AddMapsterConfig()
            .AddFluentValidationConfig().
            AddAuthConfig(configuration).
            AddDbContextConfig(configuration).
            AddHttpContextAccessor()
            .AddCors(Options=>Options.AddDefaultPolicy(
                builder=>builder.
                                 AllowAnyMethod().
                                 AllowAnyHeader().
                                 WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()!)
                ));

        //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddOptions<JwtOptions>().
            BindConfiguration(JwtOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();
        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPoolService, PollService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IResultService, ResultService>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
    private static IServiceCollection AddSwagerConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }

    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var MapConfig = TypeAdapterConfig.GlobalSettings;
        MapConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(MapConfig));
        return services;
    }

    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation()
                       .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
    private static IServiceCollection AddDbContextConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });
        return services;
    }
    private static IServiceCollection AddAuthConfig(this IServiceCollection services,IConfiguration configuration)
    {
        var jwtsetting = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(
             o =>
             {
                 o.SaveToken = true;
               o.TokenValidationParameters= new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = jwtsetting?.Issuer,
                   ValidAudience = jwtsetting?.Audience,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtsetting?.Key!))
               };


             });

        return services;
    }
}
