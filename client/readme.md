# Create the HTML Client to Render Dashboard Thumbnails

## Step 1 - Create an HTML File 

1 - Open your favorite code editor and create a new HTML file and save the file with the name `index.html`

```html title="index.html"
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Reveal Sdk - HTML/JavaScript</title>  
</head>
<body>

</body>
</html>
```

## Step 2 - Add Reveal JavaScript API

1 - Modify the `index.html` file to include the `infragistics.reveal.js`. jQuery and the Roboto fonts from Goggle script at the bottom of the page just before the closing `</body>` tag.

```html
<script src="https://dl.revealbi.io/reveal/libs/1.6.4/infragistics.reveal.js"></script>
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
@import url('https://fonts.googleapis.com/css?family=Roboto:400,500,700&display=swap');
```

## Step 3 - Add the CSS for the Dashboard Container and Fonts

```html
<style>
    @import url('https://fonts.googleapis.com/css?family=Roboto:400,500,700&display=swap');
    .thumbnail-container {
        display: flex;
        flex-wrap: wrap;
        overflow-y: scroll;
        height: 600px; 
        width: 190px;
    }
    .dashboard-item {
        margin: 10px;
        text-align: center;
        font-family: 'Roboto', sans-serif;
    }
    .dashboard-thumbnail {
        height: 125px;
        width: 150px;
        position: relative;
    }
</style>
```

## Step 4 - Add Containter for Thumbnail Images

This `div` tag is the target element for the `.thumbnail-container` that you just added in the CSS block.  It is where your thumbnails will render.

``` html
<body>
    <div class="thumbnail-container"></div>
</body>
```

The final `index.html` file should look like this:

```html title="index.html"
<!DOCTYPE html>
<html>
<head>
    <title>Dashboard Thumbnails</title>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://dl.revealbi.io/reveal/libs/1.6.4/infragistics.reveal.js"></script>
    <style>
        @import url('https://fonts.googleapis.com/css?family=Roboto:400,500,700&display=swap');
        .thumbnail-container {
            display: flex;
            flex-wrap: wrap;
            overflow-y: scroll;
            height: 600px; 
            width: 190px;
        }
        .dashboard-item {
            margin: 10px;
            text-align: center;
            font-family: 'Roboto', sans-serif;
        }
        .dashboard-thumbnail {
            height: 125px;
            width: 150px;
            position: relative;
        }
    </style>
</head>
<body>
    <div class="thumbnail-container"></div>
</body>
</html>
```

## Step 5 - Add a JavaScript Block 

To ensure our JavaScript runs after the HTML document has been fully loaded, we wrap our code in `$(document).ready()` in the `<head>` section of your HTML file.

```html
<script>
    $(document).ready(function() {
        // Our code will go here
    });
</script>

```

## Step 6 - Fetch Dashboard Names
Use $.get in an asynchronous request to fetch dashboard names from the URL running either the .NET Core or Node server. 

```javascript
$.get("https://localhost:7273/dashboards/names", function(dashboards) {
    // Processing each dashboard goes here
});
```


## Step 7: Iterating Over Fetched Dashboards
For each dashboard received, we create new HTML elements to display its name and a placeholder for its thumbnail:

```javascript
dashboards.forEach(function(dashboard) {
    var dashboardContainer = $('<div/>', { class: 'dashboard-item' }).appendTo('.thumbnail-container');
    var thumbnailDiv = $('<div/>', {
        class: 'dashboard-thumbnail'
    }).appendTo(dashboardContainer);

    var titleDiv = $('<div/>').text(dashboard.dashboardTitle).appendTo(dashboardContainer);
    // Fetching thumbnail will go here
});
```

## Step 8: Fetching and Displaying Thumbnails
For each dashboard, we fetch its thumbnail using another $.get request. Upon success, we initialize a thumbnail view and set its dashboard info:

```javascript
$.get("https://localhost:7273/dashboards/" + dashboard.dashboardFileName + "/thumbnail", function(data) {
    var thumbnailView = new $.ig.RevealDashboardThumbnailView(thumbnailDiv[0]);
    console.log("Thumbnail view initialized: ", data.info);
    thumbnailView.dashboardInfo = data.info;
});
```

Putting it all together, your full script within the HTML file will look like this:

```javascript
<!DOCTYPE html>
<html>
<head>
    <title>Dashboard Thumbnails</title>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://unpkg.com/dayjs@1.8.21/dayjs.min.js"></script>
    <script src="https://dl.revealbi.io/reveal/libs/1.6.4/infragistics.reveal.js"></script>
<style>
    @import url('https://fonts.googleapis.com/css?family=Roboto:400,500,700&display=swap');
    .thumbnail-container {
        display: flex;
        flex-wrap: wrap;
        overflow-y: scroll;
        height: 600px; 
        width: 190px;
    }
    .dashboard-item {
        margin: 10px;
        text-align: center;
        font-family: 'Roboto', sans-serif;
    }
    .dashboard-thumbnail {
        height: 125px;
        width: 150px;
        position: relative;
    }
</style>
    <script>
        $(document).ready(function() {
                var baseUrl = "https://localhost:7273/dashboards";
                $.get(baseUrl + "/names", function(dashboards) {
                    dashboards.forEach(function(dashboard) {
                    var dashboardContainer = $('<div/>', { class: 'dashboard-item' }).appendTo('.thumbnail-container');
                    var thumbnailDiv = $('<div/>', {
                        class: 'dashboard-thumbnail'
                    }).appendTo(dashboardContainer);

                    var titleDiv = $('<div/>').text(dashboard.dashboardTitle).appendTo(dashboardContainer);

                    $.get(baseUrl + "/" + dashboard.dashboardFileName + "/thumbnail", function(data) {
                        var thumbnailView = new $.ig.RevealDashboardThumbnailView(thumbnailDiv[0]);
                        console.log("Thumbnail view initialized: ", data.info);
                        thumbnailView.dashboardInfo = data.info;
                    });
                });
            });
        });
    </script>
</head>
<body>
    <div class="thumbnail-container"></div>
</body>
</html>
```

## Step 9 - Run the Application

Double-click on the `index.html` file to launch the webpage in your default browser. In order for your thumbnails to render, ensure that you have a server running and you are correctly pointing your APIs to that server by setting the `baseUrl` property in the JavaScript you just added.

**Congratulations!** You have written an application that pulls thumbnails from your Reveal dashboards.  This is a very powerful tool that enables a how of exciting experiences in your BI applications using Reveal.

:::info Get the Code

The source code to this sample can be found on [GitHub](https://github.com/jberes/ThumbnailBuilder/tree/main/client).

:::
