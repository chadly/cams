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
	`date +%s`-%03d.mp4
