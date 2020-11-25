# OssHelper
Oss Helper for .Net Core, which provide STS,Policy Authentication and common callback functions.

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

we just verify the mime type and size of the uploaded object in Callback function. about how to use this,please check the Sample project.