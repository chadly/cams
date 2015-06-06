var request = require("superagent-promise")(require("superagent"), require("promise"));

exports.getCameras = function() {
	return request.get("/cameras/")
		.accept("json")
		.end();
};

exports.getVideos = function(camera, date) {
	return request.get("/cameras/" + camera + "/" + date + "/")
		.accept("json")
		.end();
};
