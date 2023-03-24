using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAuthAPI.Models;
using MinimalAuthAPI.Services;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Auth with JWT token",
        Type = SecuritySchemeType.Http
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },

            new List<string>()
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IMovieService, MovieService>();

var app = builder.Build();

app.UseSwagger();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", 
    (UserLogin user, IUserService service) => Login(user, service));

app.MapPost("/create",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
    (Movie movie, IMovieService service) => Create(movie, service));

app.MapGet("/get",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standard, Administrator")]
    (int id, IMovieService service) => Get(id, service));

app.MapGet("/list", 
    (IMovieService service) => List(service));

app.MapPut("/update",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
    (Movie newMovie, IMovieService service) => Update(newMovie, service));

app.MapDelete("/delete",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
    (int id, IMovieService service) => Delete(id, service));

IResult Login(UserLogin user, IUserService service)
{
    if (!string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.Password))
    {
        var loggedUser = service.Get(user);
        if (loggedUser == null)
        {
            return Results.NotFound("User not found.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, loggedUser.Username),
            new Claim(ClaimTypes.Email, loggedUser.EmailAdress),
            new Claim(ClaimTypes.GivenName, loggedUser.GivenName),
            new Claim(ClaimTypes.Surname, loggedUser.Surname),
            new Claim(ClaimTypes.Role, loggedUser.Role)
        };

        var token = new JwtSecurityToken
        (
            issuer: builder.Configuration["Jwt:Issuer"],
            audience: builder.Configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(10),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                SecurityAlgorithms.HmacSha256)
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Results.Ok(tokenString);
    }

    return Results.BadRequest("Invalid user credentials.");
}

IResult Create(Movie movie, IMovieService service)
{
    var result = service.Create(movie);
    return Results.Ok(result);
}

IResult Get(int id, IMovieService service)
{
    var movie = service.Get(id);
    if (movie is null) 
        return Results.NotFound("Movie not found");

    return Results.Ok(movie);
}

IResult List(IMovieService service)
{
    var movies = service.List();
    return Results.Ok(movies);
}

IResult Update(Movie newMovie, IMovieService service)
{
    var updatedMovie = service.Update(newMovie);
    if (updatedMovie == null) return Results.NotFound("Movie not found");

    return Results.Ok(updatedMovie);
}

IResult Delete(int id, IMovieService service)
{
    var result = service.Delete(id);
    if (!result)
        return Results.BadRequest("Something went wrong");

    return Results.Ok(result);
}

app.UseSwaggerUI();
app.Run();
