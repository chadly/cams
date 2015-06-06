var Store = require("flummox").Store;

function CameraStore(flux) {
	Store.call(this, flux);

	this.actions = flux.getActions("camera");
	this.api = flux.api;

	this.register(this.actions.loadCamerasCompleted, this.onCamerasLoaded);
	this.register(this.actions.loadCamerasFailed, this.onCamerasLoadFailed);

	this.state = {};
}

CameraStore.prototype = Object.create(Store.prototype);

CameraStore.prototype.getCameras = function() {
	loadCamerasIfNeeded.call(this);
	return this.state;
};

CameraStore.prototype.onCamerasLoaded = function(response) {
	this.setState({
		isLoading: false,
		isLoaded: true,
		cameras: response.body
	});
};

CameraStore.prototype.onCamerasLoadFailed = function(response) {
	this.setState({
		isLoading: false,
		isErrored: true
	});
};

function loadCamerasIfNeeded() {
	if (!this.state.isLoaded && !this.state.isLoading) {
		this.setState({
			isLoading: true
		});

		this.api.getCameras().done(
			this.actions.loadCamerasCompleted,
			this.actions.loadCamerasFailed
		);
	}
}

module.exports = CameraStore;
