var request = require("superagent-promise")(require("superagent"), require("promise"));

exports.getCameras = function() {
	return request.get("/cameras/")
		.accept("json")
		.end();
};
