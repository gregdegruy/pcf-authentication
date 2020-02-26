REM wt cmd /k cd \ ; split-pane -H cmd /k dir ; split-pane powershell -noexit -c echo hi

IF /i "%COMPUTERNAME%"=="plusultra" wt -p "Visual Studio 2019" -d "c:\Users\grego\Documents\GitHub\greg\pcf-auth-temp"
IF /i "%COMPUTERNAME%"=="gurrenlagann" wt -p "Visual Studio 2019" -d "d:\grego\GitHub\_greg\pcf-auth-temp"

REM You need a setting for Visual Studio 20xx
REM {
REM     "guid": "{7a2309ac-bece-4bf1-9a50-72ceaaedbe92}",            
REM     "name": "Visual Studio 2019",
REM     "commandline": "%comspec% /k \"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\Common7\\Tools\\VsDevCmd.bat\"",
REM     "fontFace" : "Source Code Pro",
REM     "fontSize" : 9,
REM     "colorScheme" : "One Half Dark",
REM     "icon": "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\Common7\\IDE\\Assets\\VisualStudio.70x70.contrast-standard_scale-180.png"
REM }
