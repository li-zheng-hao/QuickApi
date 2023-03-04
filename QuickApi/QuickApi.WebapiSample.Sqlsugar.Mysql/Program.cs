using Savorboard.CAP.InMemoryMessageQueue;
using SqlSugar;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCap(x =>
{
    x.UseMySql("server=localhost;port=55000;Database=SqlSugarTest;Uid=root;Pwd=mysqlpw;");
    x.UseInMemoryMessageQueue();
});
builder.Services.AddSqlSugar(new ConnectionConfig()
    {
        ConnectionString = "server=localhost;port=55000;Database=SqlSugarTest;Uid=root;Pwd=mysqlpw;",
        DbType = DbType.MySql,
        IsAutoCloseConnection = true
    },
    db =>
    {
        db.Aop.OnLogExecuting = (sql, pars) =>
        {
            Console.WriteLine(sql); //输出sql,查看执行sql 性能无影响
        };
    });
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