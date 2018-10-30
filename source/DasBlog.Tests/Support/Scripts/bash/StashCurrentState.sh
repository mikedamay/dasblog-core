#!/usr/bin/env bash
# Mike May - October 2018
#
# stashes the content of the path (typically the root of test data) and thereby resets the working directory
# to the last commit.  
# It then drops stash@{0} which will output the hash of the stash to stdoutput where it is grabbed and logged
#
# Would like to use 'git create' but it does not take a path spec.  So Heath Robinson measures are required...
#
# $1 = root of test resources
# $2 = string to identify the stash
# usage bash -c "StashCurrentState.sh <path-spec: root of test resources> <unique-id> <display-name>"
# e.g. bash -c "StashCurrentState.sh C:\alt/projs/dasblog-core\source/eDasBlog.Tests/Resources/Environments"

# *********** WARNING ***************
# before this script is executed DetectChanges.sh should be executed to ensure that there is something
# to stash.  Otherwise the DANGER is that some user invoked stash will be dropped by the second step,
# git stash drop command is executed.
# ***********************************
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
git stash push -m "functional-test environment state $2" --all -- $1 2>1
if [[ $? -ne 0 ]]
then
    exit $?
fi
# "drop" will cause the hash of the stash to be echoed to stdout where the caller can grab it and tell user
git stash drop stash@\{0\} 2>&1
echo dasmeta_output_complete errorlevel==$?
exit $?
