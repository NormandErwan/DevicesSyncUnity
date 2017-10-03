# Devices synchronization for Unity

Unity library that allow to synchronize devices information in real time using [Unity Networking](https://docs.unity3d.com/Manual/UNet.html).

This project has been developed as part of the master thesis of [Erwan Normand](https://twitter.com/NormandErwan) and was supported by the [ÉTS - École de Technologie Supérieure](https://www.etsmtl.ca/).

## Features

- Devices synchronize their touches information.
- Devices synchronize their touch gestures and events with the input library [LeanTouch](https://www.assetstore.unity3d.com/en/#!/content/30111).
- Devices synchronize their acceleration events information.
- Server keeps the latest information from all devices client and send them to every new connected device client.

## Installation

1. Open the TCP/UDP port you're going to use.
2. Clone this repository directly on your Unity project.
3. Try the test scene:
    1. Connect a mobile device on the same local network than your PC.
    2. Build and run the `Assets/DevicesSyncUnity/Scene/MobileSync.unity` scene on the mobile device and on the PC.
    3. Use the HUD to host on the PC and to connect as client with the mobile device.
    4. Touch the mobile device and see the touches displayed on the PC screen.

## Usage

1. Set up the [NetworkManager](https://docs.unity3d.com/Manual/UNetManager.html) on your scene.
2. Optionally use `Assets/DevicesSyncUnity/Prefabs/NetworkManagerHUD.prefab` as replacement of the default Unity's NetworkManagerHUD.
3. Drag and drop to scene the `Assets/DevicesSyncUnity/Prefabs/{DeviceInfoSync, AccelerationSync, TouchesSync}.prefab` prefabs and configure them.
4. Optionally use `Assets/DevicesSyncUnity/Prefabs/DevicesSyncDisplay.prefab` to debug the prefab.

## Documentation

The documentation of available online: [https://enormand.github.io/DevicesSyncUnity/](https://enormand.github.io/DevicesSyncUnity/)

## Contributions

If you'd like to contribute, please fork the repository and use a feature branch. Pull requests are warmly welcome.

## License

See the [LICENSE](https://github.com/enormand/DevicesSyncUnity/blob/master/LICENSE) file for license rights and limitations (3-clause BSD license).