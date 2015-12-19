threshold=`date -d "90 days ago" +%Y-%m-%d`

for cam in ~/processed
do
	for vidDate in [0-9][0-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9]
	do
		if test "$vidDate" -lt "$threshold"
		then
			echo $vidDate
		fi
	done
done
