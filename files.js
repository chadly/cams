var fs = require("fs");
var path = require("path");
var Promise = require("promise");
var moment = require("moment");
require("./polyfill");

var readdir = Promise.denodeify(fs.readdir);

function FileReader(baseDir) {
	this.baseDir = baseDir;
}

FileReader.prototype.readCameras = function() {
	var self = this;
	return readdir(this.baseDir);
};

FileReader.prototype.readDates = function(camera) {
	var self = this;

	return readdir(path.join(this.baseDir, camera)).then(function(files) {
		return Promise.all(files.map(function(file) {
			return new Promise(function(resolve, reject) {
				self.readVideos(camera, file).then(function(files) {
					resolve({
						date: file,
						files: files
					});
				}, reject);
			});
		}));
	}).then(function(dates) {
		return dates.map(function(date) {
			return {
				date: date.date,
				videoCount: date.files.length
			};
		});
	});
};

FileReader.prototype.readVideos = function(camera, date) {
	return readdir(path.join(this.baseDir, camera, date)).then(function(files) {
		var videos = [];
		files.forEach(function(file) {
			if (file.endsWith(".mp4")) {
				videos.push(file);
			}
		});
		return videos;
	}).then(function(videos) {
		return videos.map(function(file) {
			var timeString = date + " " + file.replace(/.mp4/, "").replace(/-/g, ":");
			var time = moment(timeString, "YYYY-MM-DD HH:mm:ss");

			return {
				time: time,
				url: "/recordings/" + camera + "/" + date + "/" + file,
				thumbnail: "/recordings/" + camera + "/" + date + "/" + file.replace(/mp4/, "png"),
			};
		});
	});
};

module.exports = FileReader;
