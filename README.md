# Cams Post Processing

> Bash scripts to allow me to post-process recorded surveillance camera footage from FOSCAM cameras.

I run these bash scripts on a small ubuntu server running on some extra hardware I had lying around. I have [H.264 Foscams](http://foscam.com/) that record video to the server via FTP whenever they detect motion. The cameras dump everything into the `_raw` folder. The scripts process & move the raw footage to date-specific folders.

## Post-Processing Video

The Foscams like to dump video onto the FTP server using an esoteric path and filename. e.g. my front door camera will dump video to the following path `FI9805E_C4D65535806E/record/alarm_20151018_093118.mkv` and the Foscam firmware has no way to change that. When you get more than a handful of motion events a day from more than a couple of cameras, this system quickly becomes unmanageable. The scripts process the videos and make them...better.

### `all-cams.sh`

This is the entry point and will call either `process.sh`, `merge.sh`, or `summary.sh` depending upon parameters. This is where I map the camera's ID to a user-friendly name (e.g. `FI9805E_C4D65535806E` is `front-door`). This script simply calls the specified script once for each camera. The first parameter is what script to run while the second parameter is the base directory where the `raw` & `processed` folders live. 

#### Process All Cameras

```
./all-cams.sh process ~
```

This will run `process.sh` on the current user's home folder and is intended to run every couple of minutes. This script goes through all the `alarm_[date]_[time].mkv` video files in the camera's `record` folder and reorganizes the files. It creates a folder for the date of the event (e.g. `2015-10-18`) in `processed` and renames the video file to `[time].mkv`. So, `FI9805E_C4D65535806E/record/alarm_20151018_093118.mkv` becomes `2015-10-18/front-door/09-31-18.mkv`. It also runs the video file through [ffmpeg](https://ffmpeg.org/) to fix inconsistencies that foscam puts in the mkv file that prevent [mkvtoolnix](https://mkvtoolnix.download/) from working on it later.

It will only process video files that are at least 10 minutes old (based on the filename) to avoid working on a file the camera is still writing to disk.

#### Merge Motion Clips

```
./all-cams.sh merge ~
```

This will run `merge.sh` which uses [mkvtoolnix](https://mkvtoolnix.download/) to merge all the separate video clips for the day into one mkv video with different chapter markers for each motion event. So, you end up with a single `2015-10-18` folder with a single mkv file for each camera. The script is intended to be run once a day.

#### Create Summary Video

```
./all-cams.sh summary ~
```

This will run `summary.sh` which uses [ffmpeg](https://ffmpeg.org/) to create a fast motion summary file to quickly be able to view the entire day to look for interesting events. The script is intended to be run once a day.

### `clear-old.sh`

This will clear out any video folders older than 90 days. The script is intended to be run once a day. 

### Example Crontab

```
# m	h	dom	dow	command
*/3	*	*	*	/opt/cams/all-cams.sh process ~
30	0	*	*	/opt/cams/all-cams.sh merge ~
00	1	*	*	/opt/cams/all-cams.sh summary ~
00	5	*	*	/opt/cams/clear-old.sh ~
```

This will run `process.sh` every 3 minutes, `merge.sh` at 12:30am each day, `summary.sh` at 1am each day, and `clear-old.sh` at 5am each day.

## Viewing the Recordings

I setup a samba share for the recordings folder so that I can use [VLC player](http://www.videolan.org/) to scrub through the videos and skip over to specific motion events. This works even on mobile thanks to the excelent VLC Android app which allows you to browse network shares directly from the app.
