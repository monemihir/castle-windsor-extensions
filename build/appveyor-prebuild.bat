REM ~ Clone Castle Core and update to tag 3.0.0
cd..
git clone --no-checkout --progress --verbose https://github.com/castleproject/Core.git CastleCore
cd CastleCore
git checkout tags/3.0.0

REM ~ Clone Castle Windsor and update to tag 3.0.0
cd ..
git clone --no-checkout --progress --verbose https://github.com/castleproject/Windsor.git CastleWindsor
cd CastleWindsor
git checkout tags/3.0.0

REM ~ go back to build level
cd..
dir
cd castle-windsor-extensions

