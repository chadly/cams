# Cams Post Processing

> Bash scripts to allow me to post-process recorded surveillance camera footage from FOSCAM cameras.

I run these bash scripts on a small ubuntu server running on some extra hardware I had lying around. I have [H.264 Foscams](http://foscam.com/) that record video to the server via FTP whenever they detect motion. The cameras dump everything into the `_raw` folder. The scripts process & move the raw footage to date-specific folders.

## Post-Processing Video

The Foscams like to dump video onto the FTP server using an esoteric path and filename. e.g. my front door camera will dump video to the following path `FI9805E_C4D65535806E/record/alarm_20151018_093118.mkv` and the Foscam firmware has no way to change that. When you get more than a handful of motion events a day from more than a couple of cameras, this system quickly becomes unmanageable. The scripts process the videos and make them...better.

### `process-all-cams.sh`

This is the entry point and the script that I have cron setup to run once a night a little after midnight. This is where I map the camera's ID to a user-friendly name (e.g. `FI9805E_C4D65535806E` is `front-door`). This script simply calls `process.sh` once for each camera. It will only process files from the previous day (so as not to accidentally move a file that is still being written).

### `process.sh`

This script goes through all the `alarm_[date]_[time].mkv` video files in the camera's `record` folder and reorganizes the files. It creates a folder for the date of the event (e.g. `2015-10-18`) and renames/transcodes the video file to `[time].mp4`. So, `FI9805E_C4D65535806E/record/alarm_20151018_093118.mkv` becomes `2015-10-18/front-door/09-31-18.mp4`. It then uses [mkvtoolnix](https://mkvtoolnix.download/) to merge all the separate videos for the day into one mp4 video with different chapter markers for each motion event. So, you end up with a single `2015-10-18` folder with a single mkv file for each camera.

### `clear-old.sh`

Cron runs this script once a day to clear out any video folders older than 90 days.

## Viewing the Recordings

I setup a samba share for the recordings folder so that I can use [VLC player](http://www.videolan.org/) to scrub through the videos and skip over to specific motion events. This works even on mobile thanks to the excelent VLC Android app which allows you to browse network shares directly from the app.
