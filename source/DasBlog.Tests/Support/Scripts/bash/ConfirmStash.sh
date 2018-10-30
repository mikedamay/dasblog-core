#!/usr/bin/env bash
# Mike May - October 2018
#
# prints out the message associated with the stash so that the caller can verify it.
# $1 = the hash of a stash
DT=`date`
echo dasmeta ${DT} $0 $@
echo dasmeta_output_start
echo dasmeta_errors_start
if [[ $# -eq 0 ]] 
then 
    echo one or more command line arguments are missing
    echo dasmeta_errors_complete
    exit 1
fi
git log --format=%B -n 1 $1 2>1
echo dasmeta_output_complete errorlevel==$?
exit $?
