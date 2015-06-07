var express = require("express");
var cfg = require("./config");
var _ = require("lodash");
var FileReader = require("./files");

var files = new FileReader("/home/chad/cams");

var app = express();

app.use(express.static("public"));
app.use("/recordings", express.static("/home/chad/cams"));

app.get("/cameras", function(req, res) {
	return res.json(cfg.cameras);
});

app.get("/cameras/:id", function(req, res) {
	var cam = _.find(cfg.cameras, {
		id: req.params.id
	});

	if (!cam) {
		return res.status(404).json({
			message: "The camera with id '" + req.params.id + "' could not be found."
		});
	}

	files.readDates(cam.id).done(function(dates) {
		res.json(dates);
	});
});

app.get("/cameras/:id/:date", function(req, res) {
	var cam = _.find(cfg.cameras, {
		id: req.params.id
	});

	if (!cam) {
		return res.status(404).json({
			message: "The camera with id '" + req.params.id + "' could not be found."
		});
	}

	files.readVideos(cam.id, req.params.date).then(function(videos) {
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
app.use(express.static("public"));

var server = app.listen(process.env.PORT || 3000, function() {
	var host = server.address().address;
	var port = server.address().port;

	console.log("Camera app listening at http://%s:%s", host, port);
});
