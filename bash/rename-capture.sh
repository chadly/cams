#!/bin/bash

#http://unix.stackexchange.com/a/84859/50868
shopt -s nullglob

for file in *.mp4 ; do
	cdate=$(stat -c %y "$file")
	parts=($cdate)

	date=${parts[0]}
	time=${parts[1]}
	timeParts=(${time//./ })

	mkdir -p $date
	mv $file $date/${timeParts[0]}.mp4
done

