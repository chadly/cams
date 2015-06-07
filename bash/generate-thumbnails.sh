#!/bin/bash

folder=$1

#http://unix.stackexchange.com/a/63820/50868

for i in $folder/*.mp4; do
	filename=$(basename $i)
	filename="${filename%.*}"

	#-ss 00:00:05 seek to 5 seconds in (when motion was detected)
	#-vframes 1 capture 1 frame of the video (https://trac.ffmpeg.org/wiki/Create%20a%20thumbnail%20image%20every%20X%20seconds%20of%20the%20video)
	#-vf scale=-1:240 scale to 240px height (https://trac.ffmpeg.org/wiki/Scaling%20%28resizing%29%20with%20ffmpeg)
	avconv -i "$i" -ss 00:00:05 -vframes 1 -vf scale=-1:240 "$folder/$filename.png";
done
