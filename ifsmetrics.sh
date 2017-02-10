#!/bin/bash

function usage
{
    echo "usage: ifsmetrics [ [ [-c] | [-r] | [-b] ] configFile | [-h] | [-v]]"
}


##### Main
test -e IFSBuilder.exe
if [ "$?" = "1" ] ; then
    echo "Could not find IFSBuilder!"
    exit 1
fi

test -e IFSComparator.exe
if [ "$?" = "1" ] ; then
    echo "Could not find IFSComparator!"
    exit 1
fi

test -e IFSSimReporter.exe
if [ "$?" = "1" ] ; then
    echo "Could not find IFSSimReporter!"
    exit 1
fi

while [ "$1" != "" ]; do
    case $1 in
	-b | --build )    	shift	
				./IFSBuilder.exe $1
                                ;;
	-c | --compare )    	shift	
				./IFSComparator.exe $1 
                                ;;
	-r | --report )    	shift	
				./IFSSimReporter.exe $1
                                ;;
        -v | --version )    	./IFSBuilder.exe $1 && ./IFSComparator.exe $1 && ./IFSSimReporter.exe $1
                                ;;
        -h | --help )           usage
                                exit 0
                                ;;
        * )                     ./IFSBuilder.exe $1 && ./IFSComparator.exe $1 && ./IFSSimReporter.exe $1
                                exit 1
    esac
    shift
done



