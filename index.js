var express = require("express");
var cfg = require("./config");
var _ = require("lodash");
var path = require("path");
var FileReader = require("./files");

var files = new FileReader(path.join(__dirname, cfg.baseDir));

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
