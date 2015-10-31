import fs from "fs";
import path from "path";
import Promise from "promise";
import moment from "moment";

const readdir = Promise.denodeify(fs.readdir);

export default class FileReader {
	constructor(baseDir) {
		this.baseDir = baseDir;
	}

	readCameras() {
		return readdir(this.baseDir);
	}

	readDates(camera) {
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
	}

	readVideos(camera, date) {
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
	}
}
