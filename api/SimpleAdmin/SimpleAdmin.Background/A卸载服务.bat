@echo off
color 0e
@echo ==================================
@echo 提醒：请右键本文件，用管理员方式打开。
@echo ==================================
@echo Start Remove SimpleAdmin.Background

Net stop SimpleAdmin.Background
sc delete SimpleAdmin.Background
pause