var express = require("express");
var exphbs = require("express-handlebars");
var bodyParser = require("body-parser");
var cfg = require("./config");
var fs = require("fs");

if (!String.prototype.endsWith) {
	String.prototype.endsWith = function(searchString, position) {
		var subjectString = this.toString();
		if (position === undefined || position > subjectString.length) {
			position = subjectString.length;
		}
		position -= searchString.length;
		var lastIndex = subjectString.indexOf(searchString, position);
		return lastIndex !== -1 && lastIndex === position;
	};
}

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
	res.render("login");
});

app.post("/", function(req, res) {
	if (req.body.username == cfg.username && req.body.password == cfg.password) {
		fs.readdir("/home/chad/cams/front-yard/2015-06-06/", function(err, files) {
			if (err) throw err;

			var theVideos = [];
			files.forEach(function(file) {
				if (file.endsWith(".mp4")) {
					theVideos.push("/recordings/front-yard/2015-06-06/" + file);
				}
			});

			res.render("recordings", {
				recordings: theVideos
			});
		});
	} else {
		res.render("login", {
			authError: true
		});
	}
});

var server = app.listen(process.env.PORT || 3000, function() {
	var host = server.address().address;
	var port = server.address().port;

	console.log("Camera app listening at http://%s:%s", host, port);
});
