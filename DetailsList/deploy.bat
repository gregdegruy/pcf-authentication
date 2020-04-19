REM ************************************************************** 
REM * if you run into errors rebuild using -> msbuild /t:restore *
REM **************************************************************
msbuild && pac pcf push --publisher-prefix grdegr -v minimal
