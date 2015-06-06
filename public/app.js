var Flux = require("flummox").Flux;

var Api = require("./api");

var RecordingsStore = require("./recordings/store");

function CameraApp() {
	Flux.call(this);

	this.api = new Api();

	this.createStore("recordings", RecordingsStore);
}

CameraApp.prototype = Object.create(Flux.prototype);

module.exports = CameraApp;
