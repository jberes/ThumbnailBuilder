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

1 - Modify the `index.html` file to include the `infragistics.reveal.js`  and the Roboto fonts from Goggle script at the bottom of the page just before the closing `</body>` tag.

```html
<script src="https://dl.revealbi.io/reveal/libs/1.6.4/infragistics.reveal.js"></script>
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

The final `index.html` file should look like this:

```html title="index.html"
<!DOCTYPE html>
<html>
<head>
    <title>Dashboard Thumbnails</title>
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


2 - Add a JavaScript `Script` tag at the bottom of the `index.html` file.

```html
<script type="text/javascript">


</script>
```



## Step 4 - Run the Application

Double-click on the `index.html` file to launch the webpage in your default browser.

![](images/angular-app-running.jpg)

**Congratulations!** You have written your first Reveal SDK application.

:::info Get the Code

The source code to this sample can be found on [GitHub](https://github.com/RevealBi/sdk-samples-javascript/tree/main/01-GettingStarted/client/html).

:::
