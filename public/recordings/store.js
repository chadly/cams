var Store = require("flummox").Store;

function RecordingsStore(flux) {
	Store.call(this, flux);

	this.api = flux.api;

	this.state = {};
}

RecordingsStore.prototype = Object.create(Store.prototype);

module.exports = RecordingsStore;
