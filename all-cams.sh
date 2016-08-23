#!/bin/bash

action=$1
basePath=$2

#http://stackoverflow.com/a/6673798/316108
mydir=$(dirname "$0") && cd "${mydir}" || exit 1

./${action}.sh FI9805E_C4D65535806E front-yard $basePath
./${action}.sh FI9805EP_00626E537967 driveway $basePath
./${action}.sh FI9805EP_00626E537A4F backyard $basePath
./${action}.sh FI9805EP_C4D6553B70B1 backdoor $basePath
./${action}.sh FI9821P_00626E58BD75 kitchen $basePath
./${action}.sh FI9821W_00626E52CB67 living-room $basePath
./${action}.sh FI9821W_00626E52CB82 front-door $basePath
