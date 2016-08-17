#!/bin/bash -e

folder=$1
camName=$2
basePath=$3
dateToProcess=$4

#http://unix.stackexchange.com/a/84859/50868
shopt -s nullglob

for path in $basePath/_raw/$folder/record/alarm_${dateToProcess}_*.mkv ; do
	file=$(basename $path)

	#http://stackoverflow.com/a/5257398/316108
	parts=(${file//_/ })

	#http://www.catonmat.net/blog/bash-one-liners-explained-part-two/
	date=${parts[1]:0:4}-${parts[1]:4:2}-${parts[1]:6:2}
	time=${parts[2]:0:2}:${parts[2]:2:2}:${parts[2]:4:2}

	dates=("${dates[@]}" $date)

	mkdir -p $basePath/$date/$camName
	mv $path $basePath/$date/$camName/$time.mkv

	ffmpeg -i $basePath/$date/$camName/$time.mkv -codec copy $basePath/$date/$camName/$time.mp4
done

for path in $basePath/$date/$camName/*.mp4 ; do
	mergeStr="$mergeStr +$path"
done

mkvmerge --generate-chapters when-appending --generate-chapters-name-template '<FILE_NAME>' -o $basePath/$date/$camName.mp4 ${mergeStr:2}

rm -rf $basePath/$date/$camName/