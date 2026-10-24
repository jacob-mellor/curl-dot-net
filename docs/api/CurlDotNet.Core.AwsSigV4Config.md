#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## AwsSigV4Config Class

The four parts of a `--aws-sigv4 provider1[:provider2[:region[:service]]]` value, which tell
CurlDotNet how to sign a request with AWS Signature Version 4 \(or a compatible scheme such as GCP's\)\.

```csharp
public class AwsSigV4Config
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; AwsSigV4Config

### Example

```csharp
var result = await Curl.ExecuteAsync(
    "curl --aws-sigv4 'aws:amz:us-east-1:s3' -u AKIA...:wJalr... https://my-bucket.s3.amazonaws.com/object.txt");
```

### Remarks

You normally never build this yourself: `curl --aws-sigv4 "aws:amz:us-east-1:s3" -u ACCESS_KEY:SECRET_KEY URL`
            fills [AwsSigV4](CurlDotNet.Core.CurlOptions.AwsSigV4.md 'CurlDotNet\.Core\.CurlOptions\.AwsSigV4') with it, and the request is signed just before it is sent.
            Set it directly only when you build [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') in code:
            `options.AwsSigV4 = AwsSigV4Config.Parse("aws:amz:eu-west-1:execute-api");`.

The parts: [Provider1](CurlDotNet.Core.AwsSigV4Config.Provider1.md 'CurlDotNet\.Core\.AwsSigV4Config\.Provider1') (required, `aws` or `gcp`) names the algorithm
            (`AWS4-HMAC-SHA256`) and the key-derivation prefix; [Provider2](CurlDotNet.Core.AwsSigV4Config.Provider2.md 'CurlDotNet\.Core\.AwsSigV4Config\.Provider2') (default: provider1,
            `amz` for AWS) names the headers (`x-amz-date`, `x-amz-content-sha256`);
            [Region](CurlDotNet.Core.AwsSigV4Config.Region.md 'CurlDotNet\.Core\.AwsSigV4Config\.Region') (default: taken from the host, else `us-east-1`) and [Service](CurlDotNet.Core.AwsSigV4Config.Service.md 'CurlDotNet\.Core\.AwsSigV4Config\.Service')
            (default: the first label of the host, such as `s3`) go into the credential scope.

The access key and secret key are not part of this value: pass them with
            `-u ACCESS_KEY_ID:SECRET_ACCESS_KEY`. For temporary credentials add
            `-H "x-amz-security-token: TOKEN"`. When `--oauth2-bearer` is also given, no SigV4 signature is added.

Guide with a full S3 example: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/auth-schemes.md
            curl's own reference: https://curl.se/libcurl/c/CURLOPT_AWS_SIGV4.html.

### See Also
- [Parse\(string\)](CurlDotNet.Core.AwsSigV4Config.Parse(string).md 'CurlDotNet\.Core\.AwsSigV4Config\.Parse\(string\)')
- [AwsSigV4](CurlDotNet.Core.CurlOptions.AwsSigV4.md 'CurlDotNet\.Core\.CurlOptions\.AwsSigV4')

| Properties | |
| :--- | :--- |
| [Provider1](CurlDotNet.Core.AwsSigV4Config.Provider1.md 'CurlDotNet\.Core\.AwsSigV4Config\.Provider1') | The provider that names the signing algorithm and key prefix: `aws` gives `AWS4-HMAC-SHA256`, `gcp` gives `GCP4-HMAC-SHA256`\. The first part of the `--aws-sigv4` value; required\. |
| [Provider2](CurlDotNet.Core.AwsSigV4Config.Provider2.md 'CurlDotNet\.Core\.AwsSigV4Config\.Provider2') | The provider that names the signing headers: `amz` gives `x-amz-date` and `x-amz-content-sha256`\. The second part of the `--aws-sigv4` value; defaults to [Provider1](CurlDotNet.Core.AwsSigV4Config.Provider1.md 'CurlDotNet\.Core\.AwsSigV4Config\.Provider1')\. |
| [Region](CurlDotNet.Core.AwsSigV4Config.Region.md 'CurlDotNet\.Core\.AwsSigV4Config\.Region') | The region in the credential scope, such as `us-east-1` or `eu-west-1`\. The third part of the `--aws-sigv4` value; when empty it is taken from the host name\. |
| [Service](CurlDotNet.Core.AwsSigV4Config.Service.md 'CurlDotNet\.Core\.AwsSigV4Config\.Service') | The service in the credential scope, such as `s3`, `execute-api` or `dynamodb`\. The fourth part of the `--aws-sigv4` value; when empty it is taken from the host name\. |

| Methods | |
| :--- | :--- |
| [Parse\(string\)](CurlDotNet.Core.AwsSigV4Config.Parse(string).md 'CurlDotNet\.Core\.AwsSigV4Config\.Parse\(string\)') | Reads a `--aws-sigv4` value such as `aws:amz:us-east-1:s3` into its provider, header provider, region and service parts; missing parts fall back to their defaults\. |
