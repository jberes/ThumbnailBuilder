

// step 0, create a new .NET Core Web API project in Visual Studio.  After
// you have created this project, create a folder called 
// Dashboards in the project.  Then, download this file - https://github.com/jberes/ThumbnailBuilder/blob/main/Dashboards.zip - 
// unzip and place the sample dashboards in the Dashboards folder.
// You will use these sample dashboards to render the thumbnails.


// step 1, add the Reveal.Sdk and Reveal.Sdk.Dome Nuget packages,
// then add the using statements program.cs
using Reveal.Sdk;
using Reveal.Sdk.Dom;

var builder = WebApplication.CreateBuilder(args);

// step 2, AddControllers() to the builder.services
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// step 3, add a Cors policy that is necessary for accessing 
// this API from your local machine
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
      builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
    );
});

var app = builder.Build();

// step 4, after the builder.Build() statement, add the 
// UseCors for the Cors policy that you added previously
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// step 5, add the UseAuthorization middleware prior to the 
// useHttpsRedirection that is added by the project template 
app.UseAuthorization();
app.UseHttpsRedirection();

// step 7, add an API to retrieve the information needed to render the thumbnail
// of the dashboard.  The GetInfoAsync will return the JSON of the 
// requested dashboard with the full dashboard details
app.MapGet("/dashboards/{name}/thumbnail", async (string name) =>
{
    var path = "dashboards/" + name + ".rdash";
    if (File.Exists(path))
    {
        var dashboard = new Dashboard(path);
        var info = await dashboard.GetInfoAsync(Path.GetFileNameWithoutExtension(path));
        return TypedResults.Ok(info);
    }
    else
    {
        return Results.NotFound();
    }
});

// step 8, Get a list of the Dashboard File Names from the Dashboards
// folder, then use the Reveal.Sdk.Dom to get the name of the dashboard
// in the Rdash.  The File Name and the Dashboard Name can be different, the 
// actual file name is needed for the Thumbnail, but you want to display 
// the Dashboard Name to the user
app.MapGet("/dashboards/names", () =>
{
    try
    {
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dashboards");
        var files = Directory.GetFiles(folderPath);
        Random rand = new();

        var fileNames = files.Select(file =>
        {
            try
            {
                return new DashboardNames
                {
                    DashboardFileName = Path.GetFileNameWithoutExtension(file),
                    DashboardTitle = RdashDocument.Load(file).Title
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Reading FileData {file}: {ex.Message}");
                return null;
            }
        }).Where(fileData => fileData != null).ToList();

        return Results.Ok(fileNames);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error Reading Directory : {ex.Message}");
        return Results.Problem("An unexpected error occurred while processing the request.");
    }

}).Produces<IEnumerable<DashboardNames>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.ProducesProblem(StatusCodes.Status500InternalServerError);

app.Run();

// step 6, add a DashboardNames class
public class DashboardNames
{
    public string? DashboardFileName { get; set; }
    public string? DashboardTitle { get; set; }
}