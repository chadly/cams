# Cams Viewer

> NodeJS app and bash scripts to allow me to post-process and view recorded surveillance camera footage remotely.

I run these bash scripts and the node app on a small ubuntu server running on some extra hardware I had lying around. I have [H.264 Foscams](http://foscam.com/) that record video to the server via FTP whenever they detect motion. The FTP server is setup with two folders, `raw` & `processed`. The cameras dump everything into the `raw` folder. The scripts process & move the raw footage to the `processed` folder.

> Note: the scripts make use of [libav](https://libav.org/), so make sure that is installed on your server.

## Post-Processing Video

The Foscams like to dump video onto the FTP server using an esoteric path and filename. e.g. my front door camera will dump video to the following path `FI9805E_C4D65535806E/record/alarm_20151018_093118.mkv` and the Foscam firmware has no way to change that. When you get more than a handful of motion events a day from more than a couple of cameras, this system quickly becomes unmanageable. The scripts in the `bash` folder process the videos and make them...better.

### `process-all-cams.sh`

This is the entry point and the script that I have cron setup to run once a night. This is where I map the camera's ID to a user-friendly name (e.g. `FI9805E_C4D65535806E` is `front-door`). This script simply calls `process.sh` once for each camera.

### `process.sh`

This script goes through all the `alarm_[date]_[time].mkv` video files in the camera's `record` folder and reorganizes the files. It creates a folder for the date of the event (e.g. `2015-10-18`) and renames/transcodes the video file to `[time].mp4`. So, `FI9805E_C4D65535806E/record/alarm_20151018_093118.mkv` becomes `front-door/2015-10-18/09-31-18.mp4`. It then calls the thumbnail generation script to generate a thumbnail for each video file it processes.

### `generate-thumbnail.sh`

This script will use [libav](https://libav.org/) to generate a thumbnail 5 seconds into each video. I have the Foscams setup to pre-record 5 seconds before motion is detected. So, effectively, this gives me a snapshot for each video of the thing that caused motion as a thumbnail for each video.

## Viewing the Recordings

I built a small node/react app that will look at the `processed` folder and show the video thumbnails in a grid and allow you to play each video. It uses HTML5 video tags to allow the browser to playback the videos without any plugins. On the server, I have the node app start automatically (and stay running) with [pm2](https://github.com/Unitech/pm2).

You will need to create a `config.json` file that points to the directory of processed videos.

```json
{
	"baseDir": "./path/to/video/files"
}
```

Assuming you cloned this repo to `/opt/cams`, from the `/opt` folder run:

```
cd cams
npm install
npm run build
cd ..
pm2 start cams
```

This will start the cams app and make it available on port `3000`. 

### Run Locally

If you want to debug/develop on it at all, run:

```
npm install
npm start
```

This will spin up a server at `localhost:3000`. Again, make sure to create the `config.json` in the root.


