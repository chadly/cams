import express from "express";
import cfg from "./config";
import path from "path";
import FileReader from "./files";
import { spawn } from "child_process";

var files = new FileReader(path.normalize(cfg.baseDir));

var app = express();

app.use(express.static(path.join(__dirname, "public")));
app.use("/recordings", express.static(cfg.baseDir));

app.get("/cameras", function(req, res) {
	files.readCameras().done(function(cameras) {
		res.json(cameras.map(function(cam) {
			return {
				id: cam,
				name: cam
			}
		}));
	});
});

app.post("/cameras", (req, res) => {
	var cmd = spawn(__dirname + "/bash/process-all-cams.sh");

	var output = [];
	cmd.stdout.on("data", chunk => output.push(chunk));

	cmd.on("close", code => {
		if (code !== 0) {
			res.status(500);
		}

		res.set("Content-Type", "text/plain").send(Buffer.concat(output));
	})
});

app.get("/cameras/:id", function(req, res) {
	files.readDates(req.params.id).done(function(dates) {
		res.json(dates);
	});
});

app.get("/cameras/:id/:date", function(req, res) {
	files.readVideos(req.params.id, req.params.date).then(function(videos) {
		res.json(videos);
	}, function(err) {
		res.status(404).json({
			message: "There are no recordings for camera id '" + req.params.id + "' on date '" + req.params.date + "'."
		});
	});
});

//SPA
app.get("*", function(req, res, next) {
	req.url = "/";
	next();
});
app.use(express.static(path.join(__dirname, "public")));

var server = app.listen(process.env.PORT || 3000, function() {
	var port = server.address().port;
	console.log("Camera app listening on port " + port);
});
