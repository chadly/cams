var express = require("express");
var exphbs = require("express-handlebars");
var bodyParser = require("body-parser");
var cfg = require("./config");
var fs = require("fs");
var _ = require("lodash");

var app = express();

app.engine('.hbs', exphbs({
	extname: '.hbs'
}));
app.set('view engine', '.hbs');

app.use("/recordings", express.static("/home/chad/cams"));

app.use(bodyParser.urlencoded({
	extended: false
}));

app.get("/", function(req, res) {
	res.render("layout");
});

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
			videos[req.params.date].push(file);
		});

		res.json(_.extend({}, cam, videos));
	});
});

var server = app.listen(process.env.PORT || 3000, function() {
	var host = server.address().address;
	var port = server.address().port;

	console.log("Camera app listening at http://%s:%s", host, port);
});
