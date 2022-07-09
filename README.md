# AutoBackup
Personal project designed to automatically backup files to a backup location upon startup.

# What does it do?
I needed a utility program that could run in the background and constantly sync the files in one location with another location.  The main use case for this is to use the program on my music studio computer in order to sync the studio recordings to a backup location.

# How is it supposed to work?
You should be able to configure one or more sets of directories.  A single directory set consists of a Source directory and a Target directory.  When the program is running it should fully clone the directories and files that exist in Source into corresponding directories and files that exist in Target.  It should also give progress indication for where it is in the current copy job.

# Why not just use something that is already available? Or use the Windows backup functionality?
This is an exercise.  It is meant to be a fun project to sharpen some of my development skills and perhaps learn a few new techniques.  In addition, many backup utilities create s single backup image which may also be encrypted and they require special steps in order to restore the backed up files.  I wanted a quick and clean utility that could just clone a set of directories and allow me to access both the original and the copy without any extra hassle.  It is also a pain to initiate backups or schedule regular backups and you can often miss files if you aren't backing up often enough.  Having a purpose-built program that I control and that does things under certain conditions that I have better control over is ideal for my use.

# This is WAY overengineered isn't it?
Yes, yes it is.  As mentioned above, this is an exercise.  I could've made a simple console app and even hard-coded the paths that I needed, but I wanted to approach this as if I was tasked with designing production code for some enterprise set of applications.  I also wanted to explore some designs that could be reused for other things in the future.

# Additional notes:
Included in this repo is a FrameworkLibrary.  The idea behind the FrameworkLibrary is that it would provide an easy way to set up dependency injection as well as a set of common classes and interfaces designed to facilitate unit testing.  Ultimately, I should be able to break out the FrameworkLibrary into its own repo, install it via NuGet, create a new application that uses it and simply call App.Initialize<ConfigurationClass>() to get everything started using the proper configuration options for my concrete application.

# Progress

1.  Create FrameworkLibrary and basic app structure.  Test.
2.  Add unit tests for many of the FrameworkLibrary elements.
3.  Flesh out application to allow configuration of paths.
4.  Start building JobFramework to allow for processing of the file copy jobs.
5.  Add the following Sequence Diagram.  
![alt text](DesignDocs/JobSchedulingSequence.duml.svg)
This represents the planned design for the job scheduler.  Essentially, any time the user makes changes to the path configuration we need to build up a representation of the files to copy into some object which implements IJob.  The IJob can be queued for a processor to pick up off of the queue and send to a Task scheduler so that each task can be executed on the ThreadPool.  The task scheduler is designed to act as a generic scheduler of .NET Task objects with a configurable concurrency limit.  The Job Processor is designed to specifically handle individual jobs, split them out into discrete child jobs and await processing of those discrete jobs on the task scheduler.  The Job Executor is designed to process one single discrete job on the thread pool -- in this project, that means copying a single file and reporting progress.
