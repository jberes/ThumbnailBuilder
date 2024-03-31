
# Creating the .NET Core Thumbnail Viewer

This step-by-step demonstrates how to build the .NET Core backend for a client-side thumbnail generator for Reveal dashboards.  This is generic backend code, it can be used with any HTML / JavaScript client, like pure HTML, Angular, Blazor, React, Vue, etc. 

## Create a new .NET Core Web API project in Visual Studio

- Start Visual Studio and create a new .NET Core Web API project.
- After creating the project, create a folder named `Dashboards` in the project.
- Download the file from [this link](https://github.com/jberes/ThumbnailBuilder/blob/main/Dashboards.zip), unzip it, and place the sample dashboards in the `Dashboards` folder. These sample dashboards will be used to render the thumbnails.

## Add the Reveal.Sdk and Reveal.Sdk.Dom Nuget packages

- Add the `Reveal.Sdk` and `Reveal.Sdk.Dom` Nuget packages to your project.
- Then, add the following using statements to your `program.cs` file:

```csharp
using Reveal.Sdk;
using Reveal.Sdk.Dom;
using Reveal.Sdk.Dom.Visualizations;
```

## Add Controllers to the builder.services

- Add the `AddControllers()` method to the `builder.Services` in your `program.cs` file.  This is required to ensure that the necessary dependencies are registered for routing and handling HTTP requests in this app.

```csharp
builder.Services.AddControllers();
```
- Also add `AddEndpointsApiExplorer()` and `AddSwaggerGen()` methods to the `builder.Services`.

## Add a Cors policy

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

## Add the UseAuthorization middleware

- Add the `UseAuthorization()` middleware prior to the `useHttpsRedirection` that is added by the project template:

```csharp
app.UseAuthorization();
app.UseHttpsRedirection();
```

## Add a DashboardNames class

- Add a `DashboardNames` class to your `program.cs` file:

```csharp
public class DashboardNames
{
    public string? DashboardFileName { get; set; }
    public string? DashboardTitle { get; set; }
}
```

## Add an API to Retrieve the Thumbnail Info

The next step is to add an API to retrieve the information needed to render the thumbnail of the dashboard. 

- `name`: This is a path parameter that should contain the name of the dashboard for which information is being requested. The dashboards are the files you added to the `Dashboards` folder in this sample, or the location of the dasdhboards in your application. 

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

## Add an API to Retrieve your Dashboards

Next you need to get a list of the dashboards.  This API uses the `Reveal.Sdk.Dom` to get the name of the dashboard in the `Rdash` file.  The File Name or the `.Rdash` and the Dashboard Name can be different - the actual `.Rdash` file name is needed get the file in the `/dashboards/{name}/thumbnail` API to retrieve the Thumbnail info, and the Reveal.Sdk.Dom will pull the `Title` property needed for the display name.

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

Finally, run your application with `app.Run();`. When you run the application, you'll see the default Swagger UI.  You can test both the `/dashboards/names` and the `"/dashboards/{name}/thumbnail` APIs.  Use one of the dashboard names, like `Healthcare` or `Marketing` to see the results in the Swagger UI.


The code for this application is on [GitHub](https://github.com/jberes/ThumbnailBuilder/tree/main/net-core).
