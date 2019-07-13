# OculusInputForMRTK
This repository provides oculus input extension for mixed reality toolkit. 
The extension code is ported from [XRTK/Oculus](https://github.com/XRTK/Oculus).

# Prerequisites
- Unity 2019.1.x

# Current supported Oculus Inputs

|Device|Controller|Hand|
|:---:|:---:|:---:|
|Oculus S|||
|Oculus Quest|x||
|Oculus Go|||

# Getting Started
```
> git clone git@github.com:HoloLabInc/OculusInputForMRTK.git --recursive
> cd OculusInputForMRTK
> External\createSymlink.bat
```
Open OculusInputForMRTK project with Unity 2019.1.x.

> NOTE: If you don't want to include MixedRealityToolkit.Example, remove set "FOLDER_5=MixedRealityToolkit.Examples" from createSymlink.bat before execute bat file.

Apply <b>OculusConfiguration</b> Profile in OculusExtension/Profiles to MRTK Profile.

# Build Settings
- Change target platform to Android.
- Remove Vulkan from Graphics APIs in Player settings.
- Change Texture compression to ASTC.
- Change PackageName "com.Company.ProductName" to any company and product name.
- Enable Virtual Reality Supported, then add Oculus.
