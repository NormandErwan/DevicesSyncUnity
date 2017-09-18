# Device synchronization for Unity

Synchronize devices information in real time through Unity Networking (UNet).

## Features

- Devices sync their touches information
- Devices sync their acceleration information
- Server keeps the latest information from all devices client and send them to every new connected device client
- A debug display that show connected devices and their touches information in real time

## Installlation

1. Make sure the tcp/udp port you're going to use is open on your firewall
2. Clone and build the repo directly on your project
3. Test
    1. Connect a mobile device on the same local network than your PC
    2. Build and run the `Assets/DeviceSyncUnity/Scene/MobileSync.unity` scene on the mobile device and on the PC
    3. Use the HUD to host on the PC and to connect as client with the mobile device
    4. Touch the mobile device and see the touches on the PC screen

## Usage

1. Set up the [NetworkManager](https://docs.unity3d.com/Manual/UNetManager.html) on your scene
2. Optionally use `Assets/DeviceSyncUnity/Prefabs/NetworkManagerHUD.prefab` as replacement of the default Unity's NetworkManagerHUD
3. Drag and drop to scene the `Assets/DeviceSyncUnity/Prefabs/{DeviceInfoSync, AccelerationSync, TouchesSync}.prefab` prefabs and configure them
4. Optionally use `Assets/DeviceSyncUnity/Prefabs/DeviceSyncDisplay.prefab` to debug

This project has been developped using Unity 5.6.