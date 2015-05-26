#!/bin/bash

#http://unix.stackexchange.com/a/84859/50868
shopt -s nullglob

for file in *.mkv ; do
	#http://stackoverflow.com/a/5257398/316108
	parts=(${file//_/ })

	#http://www.catonmat.net/blog/bash-one-liners-explained-part-two/
	date=${parts[1]:0:4}-${parts[1]:4:2}-${parts[1]:6:2}
	time=${parts[2]:0:2}-${parts[2]:2:2}-${parts[2]:4:2}

	dates=("${dates[@]}" $date)

	mkdir -p $date
	mv $file $date/$time.mkv
	avconv -i $date/$time.mkv -codec copy $date/$time.mp4
	rm $date/$time.mkv
done

#http://superuser.com/a/513153/302579
dates=($(echo ${dates[@]} | tr ' ' '\n' | sort -u))

for date in "${dates[@]}" ; do
	cd $date
	mkdir -p thumbs

	#http://unix.stackexchange.com/a/63820/50868

	for i in *.mkv; do
        	avconv -i "$i" -r 1/10 -vf scale=-1:240 -vcodec png "thumbs/$i-%002d.png";
	done

	montage -geometry +4+4 -tile 6x thumbs/*.png preview.png

	rm -rf thumbs
	cd ..
done

