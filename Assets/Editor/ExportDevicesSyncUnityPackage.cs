using UnityEditor;

public static class ExportDevicesSyncUnityPackage
{
    [MenuItem("DevicesSyncUnity/Export package")]
    public static void ExportPackage()
    {
        string[] projectContent = new string[]
        {
            "Assets/DevicesSyncUnity/Prefabs",
            "Assets/DevicesSyncUnity/Scenes",
            "Assets/DevicesSyncUnity/Scripts",
            "Assets/DevicesSyncUnity/Scenes",
        };
        AssetDatabase.ExportPackage(projectContent, "DevicesSyncUnity.unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);

        projectContent = new string[]
        {
            "Assets/DevicesSyncUnity/Examples"
        };
        AssetDatabase.ExportPackage(projectContent, "DevicesSyncUnity-Examples.unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);
    }
}
