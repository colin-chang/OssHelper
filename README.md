# OssHelper
Oss Helper for .Net Core, which provide STS,Policy Authentication and common callback functions.

## [Nuget](https://www.nuget.org/packages/ColinChang.OssHelper)
```bash
# Package Manager
Install-Package ColinChang.OssHelper

# .NET CLI
dotnet add package ColinChang.OssHelper
```

## Functions
```csharp
/// <summary>
/// 阿里云OSS STS授权
/// </summary>
/// <returns></returns>
Task<AssumeRoleResponse.AssumeRole_Credentials> GetStsAsync();

/// <summary>
/// 阿里云OSS Policy授权
/// </summary>
/// <returns></returns>
Task<dynamic> GetPolicyAsync(ObjectType category);

/// <summary>
/// OSS 通用回调
/// </summary>
/// <param name="request"></param>
/// <returns></returns>
Task<OssObject> CallbackAsync(HttpRequest request);
```

## Sample
we just verify the mime type and size of the uploaded object in Callback function. about how to use this,please check the [Sample project](https://github.com/colin-chang/OssHelper/tree/main/ColinChang.OssHelper.WebSample).

be sure you've set your Access information in [appsettings.json](https://github.com/colin-chang/OssHelper/blob/main/ColinChang.OssHelper.WebSample/appsettings.json) before running the sample.

## Reference
* [服务端签名直传并设置上传回调](https://help.aliyun.com/document_detail/31927.html?spm=a2c4g.11174283.6.1714.6b0c7da2d7aCJy)
* [快速搭建移动应用上传回调服务](https://help.aliyun.com/document_detail/31922.html?spm=a2c4g.11186623.6.1717.4b016aefO4mo7S)