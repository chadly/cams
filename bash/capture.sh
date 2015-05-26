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
	~/cams/living-room-`date +%s`-%03d.mp4 & \

avconv \
	-i rtsp://admin:Wodj0X9DBlYY@fycam.chadly.net:90/videoMain \
	-vcodec copy \
	-acodec aac \
	-strict experimental \
	-map 0 \
	-f segment \
	-segment_time 60 \
	-segment_format mp4 \
	-b:a 32k \
	~/cams/front-yard-`date +%s`-%03d.mp4 & \

avconv \
	-i rtsp://admin:bt3tD8y4z8S3@fdcam.chadly.net:88/videoMain \
	-vcodec copy \
	-acodec aac \
	-strict experimental \
	-map 0 \
	-f segment \
	-segment_time 60 \
	-segment_format mp4 \
	-b:a 32k \
	~/cams/front-door-`date +%s`-%03d.mp4 & \

avconv \
	-i rtsp://admin:tJ2ju5alBTUR@bycam.chadly.net:91/videoMain \
	-vcodec copy \
	-acodec aac \
	-strict experimental \
	-map 0 \
	-f segment \
	-segment_time 60 \
	-segment_format mp4 \
	-b:a 32k \
	~/cams/backyard-`date +%s`-%03d.mp4
