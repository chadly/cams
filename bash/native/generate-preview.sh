#!/bin/bash

folder=$1

cd $folder
mkdir -p thumbs

#http://unix.stackexchange.com/a/63820/50868

for i in *.mp4; do
	avconv -i "$i" -r 1/10 -vf scale=-1:240 -vcodec png "thumbs/$i-%002d.png";
done

montage -geometry +4+4 -tile 4x thumbs/*.png preview.png

rm -rf thumbs
