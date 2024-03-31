
# Project Construction Guide Using Reveal BI SDK and Reveal SDK Dom Library

This is a detailed step-by-step guide on how to construct the project.

## Step 0: Create a new .NET Core Web API project in Visual Studio

- Start Visual Studio and create a new .NET Core Web API project.
- After creating the project, create a folder named `Dashboards` in the project.
- Download the file from [this link](https://github.com/jberes/ThumbnailBuilder/blob/main/Dashboards.zip), unzip it, and place the sample dashboards in the `Dashboards` folder. These sample dashboards will be used to render the thumbnails.

## Step 1: Add the Reveal.Sdk and Reveal.Sdk.Dom Nuget packages

- Add the `Reveal.Sdk` and `Reveal.Sdk.Dom` Nuget packages to your project.
- Then, add the following using statements to your `program.cs` file:

```csharp
using Reveal.Sdk;
using Reveal.Sdk.Dom;
using Reveal.Sdk.Dom.Visualizations;
```

## Step 2: Add Controllers to the builder.services

- Add the `AddControllers()` method to the `builder.Services` in your `program.cs` file:

```csharp
builder.Services.AddControllers();
```

- Also add `AddEndpointsApiExplorer()` and `AddSwaggerGen()` methods to the `builder.Services`.

## Step 3: Add a Cors policy

- Add a Cors policy that allows any origin, header, and method. This is necessary for accessing this API from your local machine:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
      builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
    );
});
```

- After the `builder.Build()` statement, add the `UseCors` method for the Cors policy that you added previously:

```csharp
app.UseCors("AllowAll");
```

## Step 4: Add the UseAuthorization middleware

- Add the `UseAuthorization()` middleware prior to the `useHttpsRedirection` that is added by the project template:

```csharp
app.UseAuthorization();
app.UseHttpsRedirection();
```

## Step 6: Add a DashboardNames class

- Add a `DashboardNames` class to your `program.cs` file:

```csharp
public class DashboardNames
{
    public string? DashboardFileName { get; set; }
    public string? DashboardTitle { get; set; }
}
```

## Step 7: Add an API to retrieve the information needed to render the thumbnail of the dashboard

- Add an API to retrieve the information needed to render the thumbnail of the dashboard. The `GetInfoAsync` method will return the JSON of the requested dashboard with the full dashboard details.  This code defines an asynchronous HTTP GET endpoint in your application at the path `/dashboards/{name}/thumbnail`. This endpoint is designed to fetch and return information about a specific dashboard based on its name.  The request to this endpoint should be a GET request with the name of the dashboard embedded in the URL.

- `name`: This is a path parameter that should contain the name of the dashboard for which information is being requested.

### Process

1. The function first constructs the path to the dashboard file by concatenating the string "dashboards/" with the name of the dashboard and the extension ".rdash".
2. It then checks if a file exists at the constructed path.
3. If the file exists:
    - A new `Dashboard` object is created with the path to the dashboard file.
    - The `GetInfoAsync` method is called on the `Dashboard` object to fetch the dashboard's information asynchronously. The name of the dashboard, obtained by removing the extension from the filename, is passed as a parameter to this method.
    - The function returns a successful HTTP response (`200 OK`) with the fetched dashboard information.
4. If the file does not exist, the function returns a `404 Not Found` HTTP response.

The response from this endpoint can be one of the following:

- `200 OK`: This indicates that the dashboard information was successfully fetched. The response body contains the dashboard information.
- `404 Not Found`: This indicates that no dashboard file was found with the provided name.

```csharp
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
```

## Step 8: Get a list of the Dashboard File Names from the Dashboards folder

- Get a list of the Dashboard File Names from the Dashboards folder, then use the `Reveal.Sdk.Dom` to get the name of the dashboard in the `Rdash`. The File Name and the Dashboard Name can be different, the actual file name is needed for the Thumbnail, but you want to display the Dashboard Name to the user.  This code defines an asynchronous HTTP GET endpoint in your application at the path `/dashboards/names`. This endpoint is designed to fetch and return a list of dashboard file names and their corresponding titles from the `Dashboards` folder. The request to this endpoint should be a GET request.

1. The function first constructs the path to the `Dashboards` folder in the current directory.
2. It then retrieves a list of all files in the `Dashboards` folder.
3. For each file in the list:
    - A new `DashboardNames` object is created.
    - The `DashboardFileName` property is set to the filename without the extension.
    - The `DashboardTitle` property is set to the title of the dashboard, which is read from the `.rdash` file using the `RdashDocument.Load` method.
    - If an error occurs while reading a file, an error message is written to the console, and the file is skipped.
4. The function returns a successful HTTP response (`200 OK`) with the list of `DashboardNames` objects.
5. If an error occurs while reading the directory, an error message is written to the console, and the function returns a `500 Internal Server Error` HTTP response.

The response from this endpoint can be one of the following:

- `200 OK`: This indicates that the list of dashboard names was successfully fetched. The response body contains the list of `DashboardNames` objects.
- `500 Internal Server Error`: This indicates that an unexpected error occurred while processing the request.

```csharp
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
```

Finally, run your application with `app.Run();`. This completes the construction of your project using the Reveal BI SDK and the Reveal SDK Dom library. Remember to handle exceptions and errors appropriately to ensure your application runs smoothly.
