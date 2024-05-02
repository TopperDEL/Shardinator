[![Codacy Badge](https://app.codacy.com/project/badge/Grade/2ee73492397e4add859462c39225e8b7)](https://app.codacy.com/gh/TopperDEL/Shardinator/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)

# Shardinator
Shardinator - Pics in the cloud, privacy in your hands!

![ShardinateSmall](https://github.com/TopperDEL/Shardinator/assets/1833242/d4e5de15-e0f6-43d4-966f-26bacc834700)

# What is it?
Shardinator is an app on Android and iOs, that takes your images and videos, encrypts them locally on your device and sends them to a fast, private and secure cloud storage. It then deletes the files on your device so you have more room to store the memories that are to come!
The Shardinator uses [Storj](https://storj.io) to store your files. It is a distributes network with end-to-end encrpytion - meaning: you and only you can access your data! No one, not even from Storj can access your files!

# What does it cost?
Shardinator is free and Open Source! You only pay for the storage and bandwith you use. You get 25 GB for free if you [make an account on Storj](https://storj.io/signup)! After you've used the free quota you pay $0.004 per GiB for storing your files. If you download them again you pay $0.007 per GiB for egress bandwith - 25GB per month are free, too.

# Current state
The app is under heavy development! You may build it yourself using Visual Studio 2022 and .Net 8. Expect bugs and glitches! PR's and contributions are welcome!

# Troubleshooting Device Restarts
If you encounter device restarts while using Shardinator on Android, you can use Android Debug Bridge (ADB) to retrieve crash logs which may provide insights into the issue. Follow these steps to get crash logs:
1. Enable Developer Options on your Android device.
2. Enable USB debugging within the Developer Options.
3. Connect your device to a computer with ADB installed.
4. Open a terminal or command prompt and type `adb logcat > crashlog.txt` to save the crash logs to a file.
5. Reproduce the issue on your device.
6. Press `Ctrl+C` in the terminal to stop logging.
7. Review the `crashlog.txt` file for any errors or warnings that may indicate the cause of the restart.

For more detailed instructions and troubleshooting tips, please refer to the documentation in `Shardinator/Documentation/TroubleshootingDeviceRestarts.md`.
