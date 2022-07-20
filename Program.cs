using JwtAPi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



var key = Encoding.ASCII.GetBytes(Settings.SecretKey);

//Esquema de autentifica��o que ser� utlizado - o JwtBearerDeafult � o esquema padr�o.
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    //Nesse caso a aplica��o n�o solicita o Https mas caso vc queira que o token aprenas funcione com Https � possivel 
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        //Validar a Chave 
        ValidateIssuerSigningKey = true,
        //Passando a chave que ser� utilizada para a valida��o
        IssuerSigningKey = new SymmetricSecurityKey(key),
        // AS dudas de baixo ser�o negativas pois s�o usadas com valida��o AOuth
        ValidateIssuer = false,
        ValidateAudience = false,

    };
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JwtAPI", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Example: 'Bearer abc...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }, Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authentifica��o sempre vem primeiro que autoriza��o , SEMPRE !!
// Se a ordem for invertida n�o ir� funcionar
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
