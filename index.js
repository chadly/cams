var express = require("express");
var exphbs = require("express-handlebars");
var bodyParser = require("body-parser");
var cfg = require("./config");

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

var server = app.listen(process.env.PORT || 3000, function () {
	var host = server.address().address;
	var port = server.address().port;

	console.log("Camera app listening at http://%s:%s", host, port);
});