#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Clone\(\) Method

Returns a new options object with the same request settings, so you can derive several similar requests
from one template without the requests affecting each other\.

```csharp
public CurlDotNet.Core.CurlOptions Clone();
```

#### Returns
[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')  
A new [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') with every property copied\. Dictionaries, lists and sets
            \([Headers](CurlDotNet.Core.CurlOptions.Headers.md 'CurlDotNet\.Core\.CurlOptions\.Headers'), [FormData](CurlDotNet.Core.CurlOptions.FormData.md 'CurlDotNet\.Core\.CurlOptions\.FormData'), [Files](CurlDotNet.Core.CurlOptions.Files.md 'CurlDotNet\.Core\.CurlOptions\.Files'), [Resolve](CurlDotNet.Core.CurlOptions.Resolve.md 'CurlDotNet\.Core\.CurlOptions\.Resolve'), [Quote](CurlDotNet.Core.CurlOptions.Quote.md 'CurlDotNet\.Core\.CurlOptions\.Quote'),
            [EmptyHeaders](CurlDotNet.Core.CurlOptions.EmptyHeaders.md 'CurlDotNet\.Core\.CurlOptions\.EmptyHeaders'), [FormParts](CurlDotNet.Core.CurlOptions.FormParts.md 'CurlDotNet\.Core\.CurlOptions\.FormParts'), the content\-type lists, [UnknownOptions](CurlDotNet.Core.CurlOptions.UnknownOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnknownOptions'),
            [UnsupportedOptions](CurlDotNet.Core.CurlOptions.UnsupportedOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnsupportedOptions'), [GlobMatches](CurlDotNet.Core.CurlOptions.GlobMatches.md 'CurlDotNet\.Core\.CurlOptions\.GlobMatches')\) are new collections with the same entries and the same
            key comparer; [BinaryData](CurlDotNet.Core.CurlOptions.BinaryData.md 'CurlDotNet\.Core\.CurlOptions\.BinaryData') is a copy of the array; each of [AdditionalTransfers](CurlDotNet.Core.CurlOptions.AdditionalTransfers.md 'CurlDotNet\.Core\.CurlOptions\.AdditionalTransfers') is itself
            cloned\. [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials'), [ProxyCredentials](CurlDotNet.Core.CurlOptions.ProxyCredentials.md 'CurlDotNet\.Core\.CurlOptions\.ProxyCredentials'), [ProgressHandler](CurlDotNet.Core.CurlOptions.ProgressHandler.md 'CurlDotNet\.Core\.CurlOptions\.ProgressHandler'),
            [AwsSigV4](CurlDotNet.Core.CurlOptions.AwsSigV4.md 'CurlDotNet\.Core\.CurlOptions\.AwsSigV4') and the [CurlFormPart](CurlDotNet.Core.CurlFormPart.md 'CurlDotNet\.Core\.CurlFormPart') objects inside [FormParts](CurlDotNet.Core.CurlOptions.FormParts.md 'CurlDotNet\.Core\.CurlOptions\.FormParts') are shared
            references \(change them by assigning new ones\)\.

### Remarks

The clone does not belong to a running command line: the state CurlDotNet attaches while a command runs
            (the cookie store shared by its transfers) is not carried over.

Example: `var template = new CurlOptions { Url = "https://api.example.com", MaxTime = 10 };` then
            `var a = template.Clone(); a.Url += "/users";` and `var b = template.Clone(); b.Url += "/orders";`.

### See Also
- [GetOptions\(\)](CurlDotNet.Core.CurlRequestBuilder.GetOptions().md 'CurlDotNet\.Core\.CurlRequestBuilder\.GetOptions\(\)')