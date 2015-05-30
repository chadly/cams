#!/bin/bash

#http://stackoverflow.com/a/6673798/316108
mydir=$(dirname "$0") && cd "${mydir}" || exit 1

./rename.sh FI9821W_00626E52CB82 front-door
./rename.sh FI9821W_00626E52CB67 living-room
./rename.sh FI9805EP_00626E537A4F backyard
./rename.sh FI9805E_C4D65535806E front-yard
