#!/bin/bash

#http://stackoverflow.com/a/6673798/316108
mydir=$(dirname "$0") && cd "${mydir}" || exit 1

#http://stackoverflow.com/q/11448885
dateToProcess=`date -d "1 day ago" +%Y%m%d`

./process.sh FI9805E_C4D65535806E front-yard ~ $dateToProcess
./process.sh FI9805EP_00626E537967 driveway ~ $dateToProcess
./process.sh FI9805EP_00626E537A4F backyard ~ $dateToProcess
./process.sh FI9805EP_C4D6553B70B1 backdoor ~ $dateToProcess
./process.sh FI9821P_00626E58BD75 kitchen ~ $dateToProcess
./process.sh FI9821W_00626E52CB67 living-room ~ $dateToProcess
./process.sh FI9821W_00626E52CB82 front-door ~ $dateToProcess
