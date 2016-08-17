#!/bin/bash -e

#http://unix.stackexchange.com/a/84859/50868
shopt -s nullglob

# http://stackoverflow.com/q/11448885
threshold=`date -d "90 days ago" +%Y%m%d`

for vidDate in ~/*
do
	vidDatePath=$(basename $vidDate)
	vidNum=`echo "$vidDatePath" | tr -d -`   # remove dashes

	if test "$vidNum" -lt "$threshold"
	then
		rm -rf $vidDate
	fi
done