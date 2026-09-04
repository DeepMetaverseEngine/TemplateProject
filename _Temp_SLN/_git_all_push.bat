@echo off
echo "------------------------ PUSH ------------------------"
FOR /D %%i in (%~dp0Deep*) DO (
echo PUSH : %%i
cd %%i
git push "origin"  master:master
git lfs push --all "origin"
cd ..
)
echo PUSH : %~dp0
git push "origin"  master:master
git lfs push --all "origin"
echo "------------------------ DONE ------------------------"
pause

