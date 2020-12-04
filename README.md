# OssHelper
Oss Helper for .Net Core, which provides STS, Policy Authentication, and common callback functions.

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

/// <summary>
/// 流式下载(如果要下载的文件太大，或者一次性下载耗时太长，您可以通过流式下载，一次处理部分内容，直到完成文件的下载)
/// </summary>
/// <param name="objectName"></param>
/// <param name="filename">本地文件存储</param>
/// <returns></returns>
Task DownloadAsync(string objectName, string filename);

/// <summary>
/// 流式下载(如果要下载的文件太大，或者一次性下载耗时太长，您可以通过流式下载，一次处理部分内容，直到完成文件的下载)
/// </summary>
/// <param name="objectName"></param>
/// <returns></returns>
Task<Stream> DownloadAsync(string objectName);
```

## Sample
we just verify the mime type and size of the uploaded object in the Callback function. about how to use this, please check the [Sample project](https://github.com/colin-chang/OssHelper/tree/main/ColinChang.OssHelper.WebSample).

be sure you've set your Access information in [appsettings.json](https://github.com/colin-chang/OssHelper/blob/main/ColinChang.OssHelper.WebSample/appsettings.json) before running the sample.

## Reference
* [服务端签名直传并设置上传回调](https://help.aliyun.com/document_detail/31927.html?spm=a2c4g.11174283.6.1714.6b0c7da2d7aCJy)
* [快速搭建移动应用上传回调服务](https://help.aliyun.com/document_detail/31922.html?spm=a2c4g.11186623.6.1717.4b016aefO4mo7S)
* [流式下载](https://help.aliyun.com/document_detail/91748.html?spm=a2c4g.11186623.2.7.5aa71c78ao0Bos#concept-91748-zh)