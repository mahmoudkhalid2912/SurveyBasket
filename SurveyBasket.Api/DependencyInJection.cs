namespace SurveyBasket.Api;

public static class DependencyInJection
{
  
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

         services.AddHybridCache();

        services
            .AddSwaggerConfig()
            .AddMapsterConfig()
            .AddFluentValidationConfig()
            .AddAuthConfig(configuration)
            .AddDbContextConfig(configuration)
            .AddCorsConfig(configuration);

       
        services.AddJwtOptions(configuration);

       
        services
            .AddAuthService()
            .AddPollService()
            .AddQuestionService()
            .AddVoteService()
            .AddResultService();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }



    private static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }

    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var mapConfig = TypeAdapterConfig.GlobalSettings;
        mapConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(mapConfig));
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
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );

        return services;
    }

    private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSetting = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>();

        services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSetting?.Issuer,
                ValidAudience = jwtSetting?.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSetting?.Key ?? string.Empty))
            };
        });

        return services;
    }

    private static IServiceCollection AddCorsConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddCors(options =>
            options.AddDefaultPolicy(builder =>
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins(
                        configuration.GetSection("AllowedOrigins")
                                     .Get<string[]>() ?? Array.Empty<string>())
            ));

        return services;
    }

    private static IServiceCollection AddJwtOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        return services;
    }

    

    private static IServiceCollection AddAuthService(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }

    private static IServiceCollection AddPollService(this IServiceCollection services)
    {
        services.AddScoped<IPoolService, PollService>();
        return services;
    }

    private static IServiceCollection AddQuestionService(this IServiceCollection services)
    {
        services.AddScoped<IQuestionService, QuestionService>();
        return services;
    }

    private static IServiceCollection AddVoteService(this IServiceCollection services)
    {
        services.AddScoped<IVoteService, VoteService>();
        return services;
    }

    private static IServiceCollection AddResultService(this IServiceCollection services)
    {
        services.AddScoped<IResultService, ResultService>();
        return services;
    }
}
