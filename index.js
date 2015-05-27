var express = require("express");
var exphbs = require("express-handlebars");
var bodyParser = require("body-parser");
var cfg = require("./config");
var avconv = require('avconv');

var app = express();

app.engine('.hbs', exphbs({ extname: '.hbs' }));
app.set('view engine', '.hbs');

app.use(bodyParser.urlencoded({
	extended: false
}));

app.get("/", function(req, res) {
	res.render("login");
});

app.post("/", function (req, res) {
	if (req.body.username == cfg.username && req.body.password == cfg.password) {
		res.render("cameras", {
			cameras: cfg.cameras
		});
	} else {
		res.render("login", {
			authError: true
		});
	}
});

app.get("/stream", function(req, res) {
	var stream = avconv([
		"-i", cfg.cameras[0],
		"-vcodec", "copy",
		"-acodec", "aac",
		"-strict", "experimental",
		"-b:a", "32k",
		"-f", "hls",
		"pipe:1"
	]);

	res.setHeader("content-type", "video/mp4");
	stream.pipe(res);
});

var server = app.listen(process.env.PORT || 3000, function () {
	var host = server.address().address;
	var port = server.address().port;

	console.log("Camera app listening at http://%s:%s", host, port);
});
