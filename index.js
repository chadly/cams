var express = require("express");
var cfg = require("./config");
var fs = require("fs");
var _ = require("lodash");
require("./polyfill");

var app = express();

app.use("/", express.static("public"));
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

	fs.readdir("/home/chad/cams/" + cam.id, function(err, files) {
		if (err) throw err;

		var dates = [];
		files.forEach(function(file) {
			dates.push(file);
		});

		res.json(_.extend({}, cam, {
			dates: dates
		}));
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

	fs.readdir("/home/chad/cams/" + cam.id + "/" + req.params.date, function(err, files) {
		if (err) {
			return res.status(404).json({
				message: "There are no recordings for camera id '" + req.params.id + "' on date '" + req.params.date + "'."
			});
		}

		var videos = {};
		videos[req.params.date] = [];

		files.forEach(function(file) {
			if (file.endsWith(".mp4")) {
				videos[req.params.date].push("/recordings/" + cam.id + "/" + req.params.date + "/" + file);
			}
		});

		res.json(_.extend({}, cam, videos));
	});
});

var server = app.listen(process.env.PORT || 3000, function() {
	var host = server.address().address;
	var port = server.address().port;

	console.log("Camera app listening at http://%s:%s", host, port);
});
