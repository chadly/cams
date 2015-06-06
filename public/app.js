var Flux = require("flummox").Flux;

var Api = require("./api");

var CameraStore = require("./store");

function CameraApp() {
	Flux.call(this);

	this.api = new Api();

	this.createActions("camera", CameraActions);
	this.createStore("camera", CameraStore, this);
}

CameraApp.prototype = Object.create(Flux.prototype);

module.exports = CameraApp;
