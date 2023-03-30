using Corona.Pageant.Database;
using Corona.Pageant.Models;
using Microsoft.EntityFrameworkCore;
using MiniValidation;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("PageantDb") ?? "Data Source=pageant.db";
builder.Services.AddSqlite<PageantDb>(connectionString)
    .AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddRazorPages();

WebApplication app = builder.Build();

await EnsureDb(app.Services, app.Logger);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/api/script", async (PageantDb db) => await db.Scripts.ToListAsync())
    .WithName("GetScript")
    .Produces<List<Scripts>>();

app.MapGet("/api/script/{act}/{scene}", async (string act, string scene, PageantDb db) =>
        await db.Scripts.FirstOrDefaultAsync(s => s.Act == act && s.Scene == scene)
            is { } script
            ? Results.Ok(script)
            : Results.NotFound())
    .WithName("GetScriptByActAndScene")
    .Produces<Scripts>()
    .Produces(StatusCodes.Status404NotFound);

app.MapGet("/api/settings", async (PageantDb db) => await db.Settings.ToListAsync())
    .WithName("GetSettings")
    .Produces<List<Settings>>();

app.MapPost("/api/script", async (Scripts script, PageantDb db) =>
    {
        if (string.IsNullOrEmpty(script.Camera1Position))
        {
            script.Camera1Position = string.Empty;
        }

        if (string.IsNullOrEmpty(script.Camera2Position))
        {
            script.Camera2Position = string.Empty;
        }

        if (string.IsNullOrEmpty(script.Camera3Position))
        {
            script.Camera3Position = string.Empty;
        }

        if (!MiniValidator.TryValidate(script, out IDictionary<string, string[]>? errors))
        {
            return Results.ValidationProblem(errors);
        }

        Scripts? scriptEntity = await db.Scripts.FirstOrDefaultAsync(s => s.Act == script.Act && s.Scene == script.Scene);
        if (scriptEntity is null)
        {
            db.Scripts.Add(script);
            await db.SaveChangesAsync();
            return Results.Created($"/api/script/{script.Act}/{script.Scene}", script);
        }

        scriptEntity.Text = script.Text;
        scriptEntity.Camera1Action = script.Camera1Action;
        scriptEntity.Camera1Position = script.Camera1Position;
        scriptEntity.Camera2Action = script.Camera2Action;
        scriptEntity.Camera2Position = script.Camera2Position;
        scriptEntity.Camera3Action = script.Camera3Action;
        scriptEntity.Camera3Position = script.Camera3Position;
        scriptEntity.SwitchToScene = script.SwitchToScene;
        await db.SaveChangesAsync();
        return Results.NoContent();
    })
    .WithName("CreateOrUpdateScriptPart")
    .ProducesValidationProblem()
    .Produces<Scripts>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status204NoContent);

app.MapGet("/api/camera/{camera}", async (string camera, PageantDb db) =>
        await db.Settings.FirstOrDefaultAsync(s => s.SettingType == "Camera" && s.SettingId == camera)
            is { } setting
            ? Results.Ok(setting)
            : Results.NotFound())
    .WithName("GetCameraSettingByCameraId")
    .Produces<Settings>()
    .Produces(StatusCodes.Status404NotFound);

app.MapPost("/api/camera/{camera}", async (string camera, Settings setting, PageantDb db) =>
    {
        setting.SettingType = "Camera";
        if (!MiniValidator.TryValidate(setting, out IDictionary<string, string[]>? errors))
        {
            return Results.ValidationProblem(errors);
        }

        Settings? settingEntity = await db.Settings.FirstOrDefaultAsync(s => s.SettingType == "Camera" && s.SettingId == camera);
        if (settingEntity is null)
        {
            db.Settings.Add(setting);
            await db.SaveChangesAsync();
            return Results.Created($"/api/camera/{camera}", setting);
        }

        settingEntity.Setting = setting.Setting;
        await db.SaveChangesAsync();
        return Results.NoContent();
    })
    .WithName("CreateOrUpdateCameraSetting")
    .ProducesValidationProblem()
    .Produces<Settings>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status204NoContent);

app.MapGet("/api/obs/{scene}", async (string scene, PageantDb db) =>
        await db.Settings.FirstOrDefaultAsync(s => s.SettingType == "OBS" && s.SettingId == scene)
            is { } setting
            ? Results.Ok(setting)
            : Results.NotFound())
    .WithName("GetOBSSettingByScene")
    .Produces<Settings>()
    .Produces(StatusCodes.Status404NotFound);

app.MapPost("/api/obs/{scene}", async (string scene, Settings setting, PageantDb db) =>
    {
        setting.SettingType = "OBS";
        if (!MiniValidator.TryValidate(setting, out IDictionary<string, string[]>? errors))
        {
            return Results.ValidationProblem(errors);
        }

        Settings? settingEntity = await db.Settings.FirstOrDefaultAsync(s => s.SettingType == "OBS" && s.SettingId == scene);
        if (settingEntity is null)
        {
            db.Settings.Add(setting);
            await db.SaveChangesAsync();
            return Results.Created($"/api/obs/{scene}", setting);
        }

        settingEntity.Setting = setting.Setting;
        await db.SaveChangesAsync();
        return Results.NoContent();
    })
    .WithName("CreateOrUpdateOBSSetting")
    .ProducesValidationProblem()
    .Produces<Settings>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status204NoContent);

app.Run();

async Task EnsureDb(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("Ensuring database exists and is up to date at connection string '{connectionString}'", connectionString);

    await using PageantDb db = services.CreateScope().ServiceProvider.GetRequiredService<PageantDb>();
    await db.Database.MigrateAsync();
}
