using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();

var app = builder.Build();


app.MapGet("/", static () => "Prova A1");

//ENDPOINTS DE CATEGORIA
//GET: http://localhost:5273/categoria/listar
app.MapGet("/categoria/listar", static ([FromServices] AppDataContext ctx) =>
{
    if (ctx.Categorias.Any())
    {
        return Results.Ok(ctx.Categorias.ToList());
    }
    return Results.NotFound("Nenhuma categoria encontrada");
});

//POST: http://localhost:5273/categoria/cadastrar
app.MapPost("/categoria/cadastrar", static ([FromServices] AppDataContext ctx, [FromBody] Categoria categoria) =>
{
    ctx.Categorias.Add(categoria);
    ctx.SaveChanges();
    return Results.Created("", categoria);
});

//ENDPOINTS DE TAREFA
//GET: http://localhost:5273/tarefas/listar
app.MapGet("/tarefas/listar", static ([FromServices] AppDataContext ctx) =>
{
    if (ctx.Tarefas.Any())
    {
        return Results.Ok(ctx.Tarefas.ToList());
    }
    return Results.NotFound("Nenhuma tarefa encontrada");
});

//POST: http://localhost:5273/tarefas/cadastrar
app.MapPost("/tarefas/cadastrar", static ([FromServices] AppDataContext ctx, [FromBody] Tarefa tarefa) =>
{
    Categoria? categoria = ctx.Categorias.Find(tarefa.CategoriaId);
    if (categoria == null)
    {
        return Results.NotFound("Categoria não encontrada");
    }
    tarefa.Categoria = categoria;
    ctx.Tarefas.Add(tarefa);
    ctx.SaveChanges();
    return Results.Created("", tarefa);
});

// PATCH: http://localhost:5273/tarefas/{id}/alterar-status
app.MapPatch("/tarefas/{id}/alterar-status", static ([FromServices] AppDataContext ctx, int id, [FromBody] Tarefa status) =>
{
    Tarefa? tarefa = ctx.Tarefas.Find(id);
    if (tarefa == null)
    {
        return Results.NotFound("Tarefa não encontrada");
    }
    tarefa.Status = status;
    ctx.SaveChanges();
    return Results.Ok(tarefa);
});



//GET: http://localhost:5273/tarefas/naoconcluidas
app.MapGet("/tarefas/naoconcluidas", static ([FromServices] AppDataContext ctx) =>
{
    var tarefasNaoConcluidas = ctx.Tarefas.Where(static t => t.Status!= "Concluída");
    return Results.Ok(tarefasNaoConcluidas.ToList());
});

//GET: http://localhost:5273/tarefas/concluidas
app.MapGet("/tarefas/concluidas", static ([FromServices] AppDataContext ctx) =>
{
    var tarefasConcluidas = ctx.Tarefas.Where(static t => t.Status == "Concluída");
    return Results.Ok(tarefasConcluidas.ToList());
});

//GET: http://localhost:5273/pages/tarefas/listar
app.MapGet("/tarefas", static ([FromServices] AppDataContext ctx) =>
{
    var todasTarefas = ctx.Tarefas.ToList();
    return Results.Ok(todasTarefas);
});


// POST: http://localhost:5273/pages/tarefas/cadastrar
app.MapPost("pages/tarefas/cadastrar", static ([FromServices] AppDataContext ctx, [FromBody] Tarefa novaTarefa) =>
{
    ctx.Tarefas.Add(novaTarefa);
    ctx.SaveChanges();
    return Results.Created($"/tarefas/{novaTarefa.TarefaId}", novaTarefa);
});


//POST: http://localhost:5273/pages/tarefa/alterar
app.MapPost("pages/tarefa/alterar", static ([FromServices] AppDataContext ctx, [FromBody] Tarefa tarefaAlterada) =>
{
    var tarefaExistente = ctx.Tarefas.Find(tarefaAlterada.TarefaId);
    if (tarefaExistente != null)
    {
        tarefaExistente.TarefaId = tarefaAlterada.TarefaId;
        tarefaExistente.Descricao = tarefaAlterada.Descricao;
        ctx.SaveChanges();
        return Results.Ok(tarefaAlterada);
    }
    else
    {
        return Results.NotFound();
    }
});

//GET: http://localhost:5273/pages/tarefas/concluidas
app.MapGet("/tarefas/concluidas", static ([FromServices] AppDataContext ctx) =>
{
    var tarefasConcluidas = ctx.Tarefas.Where(static t => t.Status == "Concluída");
    return Results.Ok(tarefasConcluidas.ToList());
});

//GET: http://localhost:5273/pages/tarefas/listarnaoconcluidas
app.MapGet("/tarefas/listarnaoconcluidas", static ([FromServices] AppDataContext ctx) =>
{
    var tarefasNaoConcluidas = ctx.Tarefas.Where(static t => t.Status != "Concluída");
    return Results.Ok(tarefasNaoConcluidas.ToList());
});

app.Run();
