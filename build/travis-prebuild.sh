#!/bin/bash

# Get Castle Core repo
echo ""
echo "Checking out Castle Core at v3.0.0"
echo "----------------------------------"
cd ..
ls -l
git clone --no-checkout --progress --verbose https://github.com/castleproject/Core.git CastleCore
cd CastleCore
git checkout tags/3.0.0
echo "----------------------------------"

# Get Castle Windsor repo
echo ""
echo "Checking out Castle Windsor at v3.0.0"
echo "-------------------------------------"
cd ..
ls -l
git clone --no-checkout --progress --verbose https://github.com/castleproject/Windsor.git CastleWindsor
cd CastleWindsor
git checkout tags/3.0.0
echo "----------------------------------"

# Go back to root level for build
echo ""
echo "Going back to root level for build to proceed"
echo "---------------------------------------------"
cd ..
ls -l
cd castle-windsor-extensions
echo "---------------------------------------------"