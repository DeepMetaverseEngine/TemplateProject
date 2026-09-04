@echo off
echo "------------------------ LFS PULL ------------------------"
git config lfs.allowincompletepush false
git config --global http.sslVerify false

FOR /D %%i in (%~dp0Deep*) DO (
echo LFS PULL : %%i
cd %%i
git lfs fetch --all origin
git lfs pull
cd ..
)
echo LFS PULL : %~dp0
git lfs fetch --all origin
git lfs pull
echo "------------------------ DONE ------------------------"
pause

