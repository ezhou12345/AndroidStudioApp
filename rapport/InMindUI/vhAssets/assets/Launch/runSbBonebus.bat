
setlocal

set SBM_DIR=..\..\..\bin\smartbody\sbgui\bin
set SBM_EXE=sbgui.exe

pushd %SBM_DIR%
start %SBM_EXE% -host=127.0.0.1 -fps=60
popd

endlocal
