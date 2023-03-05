using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using QuickApi.DataValidation;
using QuickApi.HttpResponse;
using QuickApi.JsonSerialization;
using QuickApi.JwtAuthorization;
using Savorboard.CAP.InMemoryMessageQueue;
using SqlSugar;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMvc(action =>
{
    action.Filters.Add<ResponseResultWrapperFilter>();
    action.Filters.Add<ModelValidatorFilter>();
    
}).AddCustomJsonSerialization();

builder.Services.AddJwtAuth(builder.Configuration);

builder.Services.AddCap(x =>
{
    x.UseMongoDB(config =>
    {
        config.DatabaseConnection = "mongodb://root:chaojiyonghu@localhost:30000";
        config.DatabaseName = "QuickApi";
    });
    x.UseInMemoryMessageQueue();
});
builder.Services.AddAuthentication();
builder.Services.AddMongoDB("QuickApi", MongoClientSettings.FromConnectionString(
    "mongodb://root:chaojiyonghu@localhost:30000"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();