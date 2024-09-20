using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimalApi.Domain.DTOs;
using minimalApi.Domain.Entities;
using minimalApi.Domain.Interfaces;
using minimalApi.Domain.ModelView;
using minimalApi.Domain.Services;
using minimalApi.Infra.Db;

namespace API
{
    public class StartUp
    {
        public StartUp(IConfiguration configuration)
        {
            Configuration = configuration;
            key = Configuration?.GetSection("Jwt")?.ToString() ?? ""; ;
        }

        public IConfiguration Configuration { get; set; }
        private string key = "";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            services.AddAuthorization();


            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IVeiculoService, VeiculoService>();
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(opt =>
            {
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Bearer {token}"
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement{
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },new string[] {}
                }
                });
            });

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    Configuration.GetConnectionString("sqlserver")
                );
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");

                string GerarTokenJwt(Admin administrador)
                {
                    if (string.IsNullOrEmpty(key)) return string.Empty;

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var claims = new List<Claim>()
                    {
                        new Claim("Email", administrador.Email),
                        new Claim("Perfil", administrador.Perfil.ToString()),
                        new Claim(ClaimTypes.Role, administrador.Perfil.ToString()),
                    };

                    var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: credentials
                    );

                    return new JwtSecurityTokenHandler().WriteToken(token);
                }

                ValidationErrors ValidaDTO(VeiculoDTO veiculoDTO)
                {
                    var validacao = new ValidationErrors
                    {
                        Mensagens = new List<string>()
                    };

                    if (string.IsNullOrEmpty(veiculoDTO.Nome))
                        validacao.Mensagens.Add("O Nome não pode ser vazio");

                    if (string.IsNullOrEmpty(veiculoDTO.Marca))
                        validacao.Mensagens.Add("A Marca não pode ser vazio");

                    if (int.Parse(veiculoDTO.Ano) < 1980)
                        validacao.Mensagens.Add("Veiculo muito antigo");

                    return validacao;
                }


                endpoints.MapPost("admin/login", ([FromBody] LoginDTO loginRequest, IAdminService adminService) =>
                {
                    var adm = adminService.Login(loginRequest);
                    if (adm != null)
                    {
                        string token = GerarTokenJwt(adm);
                        return Results.Ok(new AdministradorLogado
                        {
                            Email = adm.Email,
                            Perfil = adm.Perfil.ToString(),
                            Token = token
                        });
                    }
                    else
                        return Results.Unauthorized();
                }).AllowAnonymous().WithTags("Admin");

                endpoints.MapGet("admin", ([FromQuery] int? page, IAdminService adminService) =>
                {
                    return Results.Ok(adminService.Todos(page));
                }).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Admin");


                endpoints.MapPost("/admin", ([FromBody] AdminDTO request, IAdminService adminService) =>
                {
                    var validacao = new ValidationErrors
                    {
                        Mensagens = new List<string>()
                    };

                    if (string.IsNullOrEmpty(request.Email))
                        validacao.Mensagens.Add("Email não pode ser vazio");

                    if (string.IsNullOrEmpty(request.Senha))
                        validacao.Mensagens.Add("Senha não pode ser vazio");


                    if (validacao.Mensagens.Count > 0)
                        return Results.BadRequest(validacao);

                    var admin = new Admin
                    {
                        Email = request.Email,
                        Senha = request.Senha,
                        Perfil = request.Perfil
                    };

                    adminService.Criar(admin);

                    return Results.Created(string.Empty, admin);


                }).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Admin");

                endpoints.MapPost("/veiculos", ([FromBody] VeiculoDTO request, IVeiculoService veiculoService) =>
                {
                    var validacao = ValidaDTO(request);
                    if (validacao.Mensagens.Count > 0)
                        return Results.BadRequest(validacao);

                    var veiculo = new Veiculo
                    {
                        Nome = request.Nome,
                        Ano = request.Marca,
                        Marca = request.Marca
                    };

                    veiculoService.Salvar(veiculo);

                    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
                }).WithTags("Veiculos");

                endpoints.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoService veiculoService) =>
                {
                    var result = veiculoService.Todos(pagina);

                    return Results.Ok(result);
                }).WithTags("Veiculos");

                endpoints.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoService veiculoService) =>
                {
                    var result = veiculoService.BuscarPorId(id);

                    if (result is null)
                        return Results.NotFound();

                    return Results.Ok(result);
                }).WithTags("Veiculos");

                endpoints.MapPut("/veiculos/{id}", ([FromRoute] int id, [FromBody] VeiculoDTO request, IVeiculoService veiculoService) =>
                {
                    var validacao = ValidaDTO(request);
                    if (validacao.Mensagens.Count > 0)
                        return Results.BadRequest(validacao);

                    var result = veiculoService.BuscarPorId(id);

                    if (result is null)
                        return Results.NotFound();

                    result.Nome = request.Nome;
                    result.Ano = request.Ano;
                    result.Marca = request.Marca;

                    veiculoService.Atualizar(result);

                    return Results.Ok(result);
                }).WithTags("Veiculos");

                endpoints.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoService veiculoService) =>
                {
                    var result = veiculoService.BuscarPorId(id);

                    if (result is null)
                        return Results.NotFound();

                    veiculoService.Apagar(result);

                    return Results.NoContent();
                }).WithTags("Veiculos");
            });
        }


    }
}