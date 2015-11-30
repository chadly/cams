#!/bin/bash

#http://stackoverflow.com/a/6673798/316108
mydir=$(dirname "$0") && cd "${mydir}" || exit 1

./process.sh FI9805E_C4D65535806E front-yard
./process.sh FI9805EP_00626E537967 driveway
./process.sh FI9805EP_00626E537A4F backyard
./process.sh FI9805EP_C4D6553B70B1 backdoor
./process.sh FI9821P_00626E58BD75 kitchen
./process.sh FI9821W_00626E52CB67 living-room
./process.sh FI9821W_00626E52CB82 front-door
