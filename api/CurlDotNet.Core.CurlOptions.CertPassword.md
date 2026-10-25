#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.CertPassword Property

The password of [CertFile](CurlDotNet.Core.CurlOptions.CertFile.md 'CurlDotNet\.Core\.CurlOptions\.CertFile'), from the part after the colon in curl's `--cert FILE:PASSWORD`; used
for PKCS\#12 files and encrypted PEM keys\.

```csharp
public string? CertPassword { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The password, or null\. Default: null\.

### Remarks

Like curl, a colon inside the file name can be escaped as `\:`, and a Windows drive letter
            (`C:\certs\id.p12`) is not mistaken for the separator. The value is a secret: CurlDotNet never prints it in
            messages or logs. Example: `curl --cert id.p12:s3cret --cert-type P12 URL` parses to
            `CertFile = "id.p12", CertPassword = "s3cret"`.

### See Also
- [CertFile](CurlDotNet.Core.CurlOptions.CertFile.md 'CurlDotNet\.Core\.CurlOptions\.CertFile')
- [KeyPassword](CurlDotNet.Core.CurlOptions.KeyPassword.md 'CurlDotNet\.Core\.CurlOptions\.KeyPassword')