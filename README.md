# AutoBackup
Personal project designed to automatically backup files to a backup location upon startup.

# What does it do?
I needed a utility program that could run in the background and constantly sync the files in one location with another location.  The main use case for this is to use the program on my music studio computer in order to sync the studio recordings to a backup location.

# How is it supposed to work?
You should be able to configure one or more sets of directories.  A single directory set consists of a Source directory and a Target directory.  When the program is running it should fully clone the directories and files that exist in Source into corresponding directories and files that exist in Target.  It should also give progress indication for where it is in the current copy job.

# Why not just use something that is already available? Or use the Windows backup functionality?
This is an exercise.  It is meant to be a fun project to sharpen some of my development skills and perhaps learn a few new techniques.  In addition, many backup utilities create s single backup image which may also be encrypted and they require special steps in order to restore the backed up files.  I wanted a quick and clean utility that could just clone a set of directories and allow me to access both the original and the copy without any extra hassle.  It is also a pain to initiate backups or schedule regular backups and you can often miss files if you aren't backing up often enough.  Having a purpose-built program that I control and that does things under certain conditions that I have better control over is ideal for my use.

# Additional notes:
Included in this repo is a FrameworkLibrary.  The idea behind the FrameworkLibrary is that it would provide an easy way to set up dependency injection as well as a set of common classes and interfaces designed to facilitate unit testing.  Ultimately, I should be able to break out the FrameworkLibrary into its own repo, install it via NuGet, create a new application that uses it and simply call App.Initialize<ConfigurationClass>() to get everything started using the proper configuration options for my concrete application.
