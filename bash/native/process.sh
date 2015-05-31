#!/bin/bash

folder=$1
camName=$2

#http://unix.stackexchange.com/a/84859/50868
shopt -s nullglob

for path in ~/raw/$folder/record/*.mkv ; do
	file=$(basename $path)

	#http://stackoverflow.com/a/5257398/316108
	parts=(${file//_/ })

	#http://www.catonmat.net/blog/bash-one-liners-explained-part-two/
	date=${parts[1]:0:4}-${parts[1]:4:2}-${parts[1]:6:2}
	time=${parts[2]:0:2}-${parts[2]:2:2}-${parts[2]:4:2}

	dates=("${dates[@]}" $date)

	mkdir -p ~/processed/$camName/$date
	cd ~/processed/$camName/$date

	mv $path $time.mkv
	avconv -i $time.mkv -codec copy $time.mp4
	rm $time.mkv
done

#http://superuser.com/a/513153/302579
dates=($(echo ${dates[@]} | tr ' ' '\n' | sort -u))

for date in "${dates[@]}" ; do
	./generate-preview.sh ~/processed/$camName/$date
done
