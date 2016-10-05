#!/bin/bash

camName=$2
basePath=$3

yesterday=`date -d "1 day ago" +%Y-%m-%d`

#create fast-forwarded summary file
ffmpeg -i $basePath/processed/$yesterday/$camName.mkv -map_chapters -1 -filter:v "setpts=0.01*PTS" $basePath/processed/$yesterday/$camName-fast.mkv