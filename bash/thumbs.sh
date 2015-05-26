#!/bin/bash

#http://unix.stackexchange.com/a/84859/50868
shopt -s nullglob

mkdir -p thumbs

for file in *.mp4 ; do
	#http://unix.stackexchange.com/a/63820/50868
        avconv -i "$file" -r 1/10 -vf scale=-1:240 -vcodec png "thumbs/$file-%002d.png";
done

montage -geometry +4+4 -tile 6x thumbs/*.png preview.png
rm -rf thumbs

