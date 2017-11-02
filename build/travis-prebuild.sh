#!/usr/bin/env sh
cd ..
ls -l
git clone --no-checkout --progress --verbose https://github.com/castleproject/Core.git CastleCore
cd CastleCore
git checkout tags/3.0.0

cd ..
ls -l
cd castle-windsor-extensions