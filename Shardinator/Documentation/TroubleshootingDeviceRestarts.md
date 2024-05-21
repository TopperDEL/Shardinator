# Troubleshooting Device Restarts on Android Devices

## Introduction
This guide aims to help users of the Shardinator app on Android devices, including Huawei P30 Pro, troubleshoot and resolve issues leading to unexpected device restarts during the use of the app.

## Understanding Device Restarts
Device restarts can be caused by various factors including but not limited to excessive resource consumption, unhandled exceptions, and device-specific performance issues. It's crucial to gather as much information as possible to diagnose the cause.

## Memory Management Optimizations
The Shardinator app has been optimized to prevent excessive resource consumption which could lead to device restarts. These optimizations include efficient memory use during media retrieval and shardination processes.

## Enhanced Error Handling
Unhandled exceptions can cause the app to behave unexpectedly, leading to device restarts. The Shardinator app includes enhanced error handling mechanisms to ensure system stability during intensive operations like "Shardinate".

## Using ADB to Retrieve Crash Logs
Android Debug Bridge (ADB) is a versatile command-line tool that lets you communicate with a device. It provides access to a Unix shell that you can use to run a variety of commands on a device. To get deeper insights into why a device did a restart, follow these steps:

1. **Enable Developer Options** on your Android device.
2. **Enable USB Debugging** within the Developer Options.
3. **Connect your device** to a computer with ADB installed.
4. Open a **terminal or command prompt** and type `adb logcat > crashlog.txt` to save the crash logs to a file.
5. **Reproduce the issue** on your device.
6. Press `Ctrl+C` in the terminal to stop logging.
7. Review the `crashlog.txt` file for any errors or warnings that may indicate the cause of the restart.

## General Tips for Diagnosing and Resolving Issues
- Ensure your device's software is up to date.
- Check for app updates in the Google Play Store.
- Try clearing the app's cache and data.
- If the issue persists, consider reinstalling the app.

## Conclusion
By following this guide, users should be able to diagnose and potentially resolve issues leading to device restarts while using the Shardinator app on Android devices. If problems persist, please report them on the app's GitHub repository issue tracker.
