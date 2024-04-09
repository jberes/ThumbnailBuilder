// .Info API not in NODE

var express = require('express');
var reveal = require('reveal-sdk-node');
var cors = require('cors');
const fs = require('fs');
const path = require('path');
const { RdashDocument } = require('@revealbi/dom');

const app = express();
app.use(cors());

app.get("/dashboards/names", async (req, resp) => {
	try {
		const dashboards = [];
		const dashboardFiles = fs.readdirSync('dashboards').filter(file => file.endsWith('.rdash'));

		for (const fileName of dashboardFiles) {
			const filePath = path.join('dashboards', fileName);
			const fileData = fs.readFileSync(filePath);
			const blob = new Blob([fileData], { type: 'application/zip' });
			const document = await RdashDocument.load(blob);	
            const dashboardFileName = path.basename(fileName, '.rdash');
            dashboards.push(new DashboardNames(dashboardFileName, document.title));
		}
		resp.status(200).json(dashboards);
	} catch (ex) {
		resp.status(500).send(`An error occurred: ${ex.message}`);
	}
});

app.get("/dashboards/:name/thumbnail", async (req, resp) => {
    const name = req.params.name;
    const path = `dashboards/${name}.rdash`;
    if (fs.existsSync(path)) {
        const db = new Dashboard(path);
        console.log(db);
        //const info = await db.getInfoAsync(path.basename(path, '.rdash'));
        //resp.status(200).json(info);
    } else {
        resp.status(404).send('Not Found');
    }
});


const revealOptions = {
}

app.use('/', reveal(revealOptions));

app.listen(7122, () => {
    console.log(`Reveal server accepting http requests`);
});

class DashboardNames {
  constructor(dashboardFileName, dashboardTitle) {
    this.dashboardFileName = dashboardFileName || null;
    this.dashboardTitle = dashboardTitle || null;
  }
}
