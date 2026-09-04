@echo off
echo "------------------------ PULL ------------------------"
FOR /D %%i in (%~dp0Deep*) DO (
echo PULL : %%i
cd %%i
git pull "origin"  master:master
git lfs pull
cd ..
)
echo PULL : %~dp0
git pull "origin"  master:master
git lfs pull
echo "------------------------ DONE ------------------------"
pause

