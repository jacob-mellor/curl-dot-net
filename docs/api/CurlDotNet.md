#### [CurlDotNet](index.md 'index')

## CurlDotNet Namespace

| Classes | |
| :--- | :--- |
| [Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl') |   <b>🚀 THE MAIN CURLDOTNET CLASS - Start here!</b>  This class lets you run ANY curl command in C# by just copying and pasting it as a string.              No translation needed. No learning curve. If it works in curl, it works here.\<example\>   \<code language="csharp"\>              // Just paste any curl command as a string:              var response = await Curl\.ExecuteAsync\("curl https://api\.github\.com"\);              Console\.WriteLine\(response\.Body\);  // That's it\! You're done\!                           // Apply CurlSettings for retries/timeouts              var settings = new CurlSettings\(\)                  \.WithTimeout\(seconds: 10\)                  \.WithRetries\(count: 2, delayMs: 500\);              var json = await Curl\.ExecuteAsync\("curl https://httpbin\.org/json", settings\);              json\.EnsureSuccess\(\);              \</code\> \</example\>  <b>What is curl?</b> curl is the universal tool for making HTTP requests from the command line.              Every API documentation uses it. Now you can use those exact same commands in your C# code.  <b>Learn more:</b>\.\.\. |
| [CurlApiClient](CurlDotNet.CurlApiClient.md 'CurlDotNet\.CurlApiClient') | Simplified API client builder for common HTTP operations\. Provides a more ergonomic alternative to raw curl commands\. |
| [DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl') | DotNetCurl \- Alternative API entry point for curl commands\. The killer feature: Copy and paste any curl command and it works\! |
| [ErgonomicExtensions](CurlDotNet.ErgonomicExtensions.md 'CurlDotNet\.ErgonomicExtensions') | Ergonomic extension methods for CurlResult to improve developer experience\. These extensions are in the CurlDotNet namespace for easy discovery\. |
