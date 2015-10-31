import fs from "fs";
import path from "path";
import Promise from "promise";
import moment from "moment";
import Cache from "node-cache";

const readdir = Promise.denodeify(fs.readdir);
const cache = new Cache({
	stdTTL: 43200, //12 hours
	checkPeriod: 3600, //1 hour
	useClones: false //saving promises to cache
});

export default class FileReader {
	constructor(baseDir) {
		this.baseDir = baseDir;
	}

	readCameras() {
		var value = cache.get("cameras");
		if (value === undefined) {
			value = readdir(this.baseDir);
			cache.set("cameras", value);
		}

		return value;
	}

	readDates(camera) {
		const self = this;
		const cacheKey = camera + "_dates";

		var value = cache.get(cacheKey);
		if (value === undefined) {
			value = readdir(path.join(this.baseDir, camera)).then(function(files) {
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

			cache.set(cacheKey, value);
		}

		return value;
	}

	readVideos(camera, date) {
		const cacheKey = camera + date + "_videos";

		var value = cache.get(cacheKey);
		if (value === undefined) {
			value = readdir(path.join(this.baseDir, camera, date)).then(function(files) {
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

			cache.set(cacheKey, value);
		}

		return value;
	}
}
