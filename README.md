# Cams Post Processing

> Post-process recorded surveillance camera footage from Foscam & Amcrest cameras

I run this app every 30 minutes on a small ubuntu server running on some extra hardware I had lying around. I have [Foscam](https://foscam.com/) & [Amcrest](https://amcrest.com/) cameras that record video to the server via FTP whenever they detect motion. The cameras dump everything into the `raw` folder. The app processes & moves the raw footage to date-specific folders.

## Installation

Make sure you have [ffmpeg](https://www.ffmpeg.org/) installed. Download the binaries for your OS (either Ubuntu or Windows) for [the latest release](https://github.com/chadly/cams/releases/latest) and copy it somewhere appropriate (`/opt/cams` on Ubuntu). Update the `settings.json` with the path to your camera directory (see below). From there, you can run `cams` and set it up to run automatically as a cron job:

```
# m	h	dom	dow	command
30	*	*	*	/opt/cams/cams
```

This will run the app once an hour at the 30 minute mark.

## How it Works

The cameras like to dump video onto the FTP server using an esoteric path and filename. e.g. my front door camera will dump video to the following path `FI9805E_C4D65535806E/record/alarm_20151018_093118.mkv` and the firmware has no way to change that. When you get more than a handful of motion events a day from more than a couple of cameras, this system quickly becomes unmanageable. The app processes the videos and make them...better.

### Reorganize

The app first goes through the `raw` folder and scans for the Foscam/Amcrest dumping folders. It goes through all the video files in each camera's folder and reorganizes the files. It creates a folder for the date of the event (e.g. `2015-10-18`) in `processed` and renames the video file to `[time].mp4`. So, as an example, `FI9805E_C4D65535806E/record/alarm_20151018_093118.mkv` becomes `2015-10-18/front-door/09-31-18.mp4`.

It will only process video files that are at least 5 minutes old (see `settings.json`) based on the filename to avoid working on a file the camera is still writing to disk.

### Create Summary Video

The app then will merge all the separate video clips for the day into one video and then uses [ffmpeg](https://ffmpeg.org/) to create a fast motion summary file to quickly be able to view the entire day to look for interesting events.

### Clear Old Videos

The app then will clear out any video folders older than 90 days (see `settings.json`).

## Viewing the Recordings

I setup a samba share for the recordings folder so that I can use [VLC player](http://www.videolan.org/) to scrub through the videos and skip over to specific motion events. This works even on mobile thanks to the excelent VLC Android app which allows you to browse network shares directly from the app.
