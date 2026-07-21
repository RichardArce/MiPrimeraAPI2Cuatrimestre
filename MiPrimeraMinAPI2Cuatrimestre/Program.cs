using Microsoft.AspNetCore.Http.HttpResults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    // Info general que Scalar muestra en el header de la doc
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info.Title = "API de Mascotas";
        document.Info.Version = "v1";
        document.Info.Description = "API para administrar el registro de mascotas.";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

app.MapScalarApiReference(options =>
{
    options.Title = "API de Mascotas - Docs";
    options.Theme = ScalarTheme.BluePlanet; // cambia el tema visual de Scalar
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var mascotas = new List<Mascota>
{
    new Mascota(1, "Firulais", 3, "Labrador"),
    new Mascota(2, "Michi", 2, "Siames"),
    new Mascota(3, "Rex", 5, "Pastor Aleman")
};

var group = app.MapGroup("/mascotas").WithTags("Mascotas");

group.MapGet("/", () =>
{
    return Results.Ok(mascotas);
})
  .WithName("GetMascotas")
  .WithSummary("Obtiene la lista de mascotas")
  .WithDescription("Devuelve todas las mascotas registradas en el sistema.")
  .Produces<List<Mascota>>(StatusCodes.Status200OK);

group.MapGet("/{id:int}", (int id) =>
{
    var mascota = mascotas.Find(x => x.Id == id);
    return mascota is not null
        ? Results.Ok(mascota)
        : Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Mascota no encontrada");
})
  .WithName("GetMascotaPorId")
  .WithSummary("Obtiene una mascota por su id")
  .WithDescription("Busca una mascota específica usando su identificador único.")
  .Produces<Mascota>(StatusCodes.Status200OK)
  .ProducesProblem(StatusCodes.Status404NotFound);

group.MapPost("/", (Mascota mascota) =>
{
    mascotas.Add(mascota);
    return Results.Created($"/mascotas/{mascota.Id}", mascota);
})
  .WithName("CreateMascota")
  .WithSummary("Crea una nueva mascota")
  .WithDescription("Registra una nueva mascota en el sistema. El id debe ser único.")
  .Produces<Mascota>(StatusCodes.Status201Created);

group.MapDelete("/{id:int}", (int id) =>
{
    var mascotaEliminar = mascotas.Find(x => x.Id == id);
    if (mascotaEliminar is null)
        return Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Mascota no encontrada");

    mascotas.Remove(mascotaEliminar);
    return Results.Ok(mascotaEliminar);
})
  .WithName("DeleteMascota")
  .WithSummary("Borra una mascota")
  .WithDescription("Elimina una mascota del sistema según su id.")
  .Produces<Mascota>(StatusCodes.Status200OK)
  .ProducesProblem(StatusCodes.Status404NotFound);

group.MapPut("/{id:int}", (int id, Mascota mascota) =>
{
    var mascotaActualizar = mascotas.Find(x => x.Id == id);
    if (mascotaActualizar is null)
        return Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Mascota no encontrada");

    mascotaActualizar.Nombre = mascota.Nombre;
    mascotaActualizar.Edad = mascota.Edad;
    mascotaActualizar.Raza = mascota.Raza;
    return Results.Ok(mascotaActualizar);
})
  .WithName("ActualizarMascota")
  .WithSummary("Actualiza una mascota")
  .WithDescription("Modifica los datos de una mascota existente según su id.")
  .Produces<Mascota>(StatusCodes.Status200OK)
  .ProducesProblem(StatusCodes.Status404NotFound);

app.Run();

internal record Mascota(int Id, string Nombre, int Edad, string Raza)
{
    public int Id { get; set; } = Id;
    public string Nombre { get; set; } = Nombre;
    public int Edad { get; set; } = Edad;
    public string Raza { get; set; } = Raza;
}