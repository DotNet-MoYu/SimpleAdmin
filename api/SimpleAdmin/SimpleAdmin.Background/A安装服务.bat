@echo off
color 0e
@echo ==================================
@echo 提醒：请右键本文件，用管理员方式打开。
@echo ==================================
@echo Start Install SimpleAdmin.Background

cd ..
sc create SimpleAdmin.Background binPath=%~dp0SimpleAdmin.Background.exe start= auto 
sc description SimpleAdmin.Background "SimpleAdmin后台服务"
Net Start SimpleAdmin.Background
pause