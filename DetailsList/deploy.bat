REM use this build
REM msbuild /t:restore
msbuild && pac pcf push --publisher-prefix grdegr -v minimal
