#!/bin/bash

folder=$1

#http://unix.stackexchange.com/a/63820/50868

for i in $folder/*.mp4; do
	./generate-thumbnail.sh $i
done
