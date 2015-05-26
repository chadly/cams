#!/bin/bash

mkdir -p ~/cams

avconv \
	-i rtsp://admin:DeTcKNRr@lrcam.chadly.net:89/videoMain \
	-vcodec copy \
	-acodec aac \
	-strict experimental \
	-map 0 \
	-f segment \
	-segment_time 60 \
	-segment_format mp4 \
	-b:a 32k \
	~/cams/`date +%s`-%03d.mp4
