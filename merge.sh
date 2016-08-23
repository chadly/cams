#!/bin/bash -e

camName=$1
basePath=$2

#http://unix.stackexchange.com/a/84859/50868
shopt -s nullglob

for path in $basePath/processed/**/.cams/$camName/*.mkv ; do
	date=$(basename $(dirname $(dirname $(dirname $path))))
	dates=("${dates[@]}" $date)
done

echo ${dates[@]}

for date in ${dates[@]}; do
	stuffToProcess=false

	# include stuff that was previously merged
	for path in $basePath/processed/$date/.cams/$camName/merged/*.mkv ; do
		mergeStr="$mergeStr +$path"
	done

	# look for new stuff to merge
	for path in $basePath/processed/$date/.cams/$camName/*.mkv ; do
		stuffToProcess=true
		mergeStr="$mergeStr +$path"
	done

	if [ "$stuffToProcess" = true ]
	then
		mkvmerge --generate-chapters when-appending --generate-chapters-name-template '<FILE_NAME>' -o $basePath/processed/$date/.$camName.mkv ${mergeStr:2}

		#switch out the tmp file for the real one
		rm -f $basePath/processed/$date/$camName.mkv
		mv $basePath/processed/$date/.$camName.mkv $basePath/processed/$date/$camName.mkv

		#mark the files as merged
		mkdir -p $basePath/processed/$date/.cams/$camName/merged
		mv $basePath/processed/$date/.cams/$camName/* $basePath/processed/$date/.cams/$camName/merged
	fi
done