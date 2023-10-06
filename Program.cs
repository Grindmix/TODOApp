using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using TODOApp.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddNpgsql<TodoDb>("Host=database;Database=todoapp_database;Username=admin;Password=admin");   
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TODO Api", 
        Description = "TODO App", 
        Version = "v1"
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TODO Api V1");
    });

app.MapGet("/", () => "Hello World!");

app.MapGet("/todos", async (TodoDb db) => await db.Todos.ToListAsync());

app.MapPost("/todo", async (TodoDb db, TodoItem todoItem) =>
{
    await db.Todos.AddAsync(todoItem);
    await db.SaveChangesAsync();
    return Results.Created($"/todo/{todoItem.Id}", todoItem);
});

app.MapGet("/todo/{id}", async (TodoDb db, int id) => await db.Todos.FindAsync(id));

app.MapPut("/todo/{id}", async (TodoDb db,TodoItem updateItem, int id) =>
{
    var todoItem = await db.Todos.FindAsync(id);
    if (todoItem is null) return Results.NotFound();
    todoItem.Title = updateItem.Title;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todo/{id}", async (TodoDb db, int id) =>
{
    var todoItem = await db.Todos.FindAsync(id);
    if (todoItem is null) return Results.NotFound();
    db.Todos.Remove(todoItem);
    await db.SaveChangesAsync();
    return Results.Ok();
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<TodoDb>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

app.Run();