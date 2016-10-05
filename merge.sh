#!/bin/bash

camName=$2
basePath=$3

#http://unix.stackexchange.com/a/84859/50868
shopt -s nullglob

yesterday=`date -d "1 day ago" +%Y-%m-%d`

stuffToProcess=false

# look for new stuff to merge
for path in $basePath/processed/$yesterday/$camName/*.mkv ; do
	stuffToProcess=true
	mergeStr="$mergeStr +$path"
done

if [ "$stuffToProcess" = true ]
then
	#merge all clips into one mkv
	mkvmerge --generate-chapters when-appending --generate-chapters-name-template '<FILE_NAME>' -o $basePath/processed/$yesterday/$camName.mkv ${mergeStr:2}

	#remove the clip folder
	rm -rf $basePath/processed/$yesterday/$camName
fi