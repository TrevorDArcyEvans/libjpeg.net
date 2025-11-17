#!/bin/bash
GITREVNUM=`git rev-list HEAD --count`
sed 's/\$REVNUM\$/'"$GITREVNUM"'/g' AssemblyVersion.txt > AssemblyVersion.cs
