git pull
set /P m="Commit message: "
git add *
git commit -m "%m%"
git push -u origin master