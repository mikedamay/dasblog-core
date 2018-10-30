#!/usr/bin/env bash
# Mike May - October 2018
# returns the version of GIT installed
#
DT=`date`
echo dasmeta ${DT} $0 $@
echo dasmeta_output_start
echo dasmeta_errors_start
git --version 2>1
echo dasmeta_output_complete errorlevel $?
