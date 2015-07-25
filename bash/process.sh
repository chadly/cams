#!/bin/bash

folder=$1
camName=$2

#http://unix.stackexchange.com/a/84859/50868
shopt -s nullglob

for path in ~/raw/$folder/record/*.mkv ; do
	#wait for file to be done being written to
	#http://askubuntu.com/a/14254/266063
	while :
	do
		if ! [[ `lsof | grep $path` ]]
		then
			break
		fi
		sleep 0.5
	done

	file=$(basename $path)

	#http://stackoverflow.com/a/5257398/316108
	parts=(${file//_/ })

	#http://www.catonmat.net/blog/bash-one-liners-explained-part-two/
	date=${parts[1]:0:4}-${parts[1]:4:2}-${parts[1]:6:2}
	time=${parts[2]:0:2}-${parts[2]:2:2}-${parts[2]:4:2}

	dates=("${dates[@]}" $date)

	mkdir -p ~/processed/$camName/$date

	mv $path ~/processed/$camName/$date/$time.mkv
	avconv -i ~/processed/$camName/$date/$time.mkv -codec copy ~/processed/$camName/$date/$time.mp4
	rm ~/processed/$camName/$date/$time.mkv

	./generate-thumbnail.sh ~/processed/$camName/$date/$time.mp4
done
