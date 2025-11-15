---
title: "How to Use curl in .NET (VB.NET)"
published: true
tags: ['dotnet', 'vbnet', 'http', 'api', 'curl', 'rest', 'tutorial', 'visual-basic']
date: 2025-01-20
canonical_url: https://dev.to/jacob/how-to-curl-in-dotnet-vbnet
---

# How to Use curl in .NET (VB.NET)

VB.NET developers can use CurlDotNet to execute curl commands directly in their code. The library works perfectly with VB.NET's async/await syntax and provides a straightforward way to make HTTP requests.

## Installing CurlDotNet

Add the NuGet package to your VB.NET project:

```xml
<ItemGroup>
  <PackageReference Include="CurlDotNet" Version="1.0.0" />
</ItemGroup>
```

Or via Package Manager Console:

```powershell
Install-Package CurlDotNet
```

## Basic Usage

Here's a simple example in VB.NET:

```vb
Imports CurlDotNet
Imports System.Threading.Tasks

Module Program
    Async Function Main(args As String()) As Task(Of Integer)
        Dim result = Await Curl.ExecuteAsync("curl https://api.github.com/users/octocat")
        
        Console.WriteLine($"Status: {result.StatusCode}")
        Console.WriteLine($"Body: {result.Body}")
        
        Return 0
    End Function
End Module
```

## Simple GET Request

```vb
Imports CurlDotNet
Imports System.Threading.Tasks

Module Program
    Async Function Main(args As String()) As Task(Of Integer)
        Dim result = Await Curl.ExecuteAsync("curl https://jsonplaceholder.typicode.com/posts/1")
        
        If result.IsSuccess Then
            Console.WriteLine($"Response: {result.Body}")
        Else
            Console.WriteLine($"Error: {result.StatusCode}")
        End If
        
        Return 0
    End Function
End Module
```

## POST Requests

Here's how to make POST requests:

```vb
Imports CurlDotNet
Imports System.Threading.Tasks

Module Program
    Async Function Main(args As String()) As Task(Of Integer)
        Dim command = "curl -X POST https://jsonplaceholder.typicode.com/posts " &
                      "-H 'Content-Type: application/json' " &
                      "-d '{""title"":""My Post"",""body"":""Content"",""userId"":1}'"
        
        Dim result = Await Curl.ExecuteAsync(command)
        
        If result.IsSuccess Then
            Console.WriteLine($"Created: {result.StatusCode}")
            Console.WriteLine($"Response: {result.Body}")
        End If
        
        Return 0
    End Function
End Module
```

## Working with JSON

VB.NET can work with JSON using standard .NET JSON libraries:

```vb
Imports CurlDotNet
Imports System.Text.Json
Imports System.Threading.Tasks

Public Class User
    Public Property Login As String
    Public Property Name As String
    Public Property PublicRepos As Integer
End Class

Module Program
    Async Function Main(args As String()) As Task(Of Integer)
        Dim result = Await Curl.ExecuteAsync("curl https://api.github.com/users/octocat")
        
        If result.IsSuccess Then
            Dim user = JsonSerializer.Deserialize(Of User)(result.Body)
            Console.WriteLine($"User: {user.Name}")
            Console.WriteLine($"Repos: {user.PublicRepos}")
        End If
        
        Return 0
    End Function
End Module
```

## Authentication

### Basic Authentication

```vb
Imports CurlDotNet
Imports System.Threading.Tasks

Module Program
    Async Function Main(args As String()) As Task(Of Integer)
        Dim result = Await Curl.ExecuteAsync("curl -u username:password https://api.example.com/protected")
        
        If result.IsSuccess Then
            Console.WriteLine(result.Body)
        End If
        
        Return 0
    End Function
End Module
```

### Bearer Token

```vb
Imports CurlDotNet
Imports System.Threading.Tasks

Module Program
    Async Function Main(args As String()) As Task(Of Integer)
        Dim command = "curl https://api.example.com/protected " &
                      "-H 'Authorization: Bearer YOUR_TOKEN'"
        
        Dim result = Await Curl.ExecuteAsync(command)
        
        If result.IsSuccess Then
            Console.WriteLine(result.Body)
        End If
        
        Return 0
    End Function
End Module
```

## Error Handling

VB.NET's exception handling works with CurlDotNet:

```vb
Imports CurlDotNet
Imports CurlDotNet.Exceptions
Imports System.Threading.Tasks

Module Program
    Async Function Main(args As String()) As Task(Of Integer)
        Try
            Dim result = Await Curl.ExecuteAsync("curl https://api.github.com/users/octocat")
            Console.WriteLine($"Success: {result.Body}")
        Catch ex As CurlDnsException
            Console.WriteLine($"DNS error: {ex.Message}")
        Catch ex As CurlTimeoutException
            Console.WriteLine($"Timeout: {ex.Message}")
        Catch ex As CurlHttpException
            Console.WriteLine($"HTTP {ex.StatusCode}: {ex.Message}")
        Catch ex As CurlException
            Console.WriteLine($"Error: {ex.Message}")
        End Try
        
        Return 0
    End Function
End Module
```

## File Operations

### Downloading Files

```vb
Imports CurlDotNet
Imports System.Threading.Tasks

Module Program
    Async Function Main(args As String()) As Task(Of Integer)
        Dim result = Await Curl.ExecuteAsync("curl -o image.png https://example.com/image.png")
        
        Console.WriteLine($"Downloaded {result.BinaryData.Length} bytes")
        
        Return 0
    End Function
End Module
```

### Uploading Files

```vb
Imports CurlDotNet
Imports System.Threading.Tasks

Module Program
    Async Function Main(args As String()) As Task(Of Integer)
        Dim command = "curl -X POST https://api.example.com/upload " &
                      "-F 'file=@C:\path\to\document.pdf' " &
                      "-F 'description=My Document' " &
                      "-H 'Authorization: Bearer YOUR_TOKEN'"
        
        Dim result = Await Curl.ExecuteAsync(command)
        
        If result.IsSuccess Then
            Console.WriteLine("File uploaded successfully")
        End If
        
        Return 0
    End Function
End Module
```

## Real-World Example: GitHub API

Here's a complete VB.NET example:

```vb
Imports CurlDotNet
Imports System.Text.Json
Imports System.Threading.Tasks
Imports System.Environment

Public Class GitHubUser
    Public Property Login As String
    Public Property Name As String
    Public Property PublicRepos As Integer
End Class

Public Class Repository
    Public Property Name As String
    Public Property FullName As String
    Public Property Private As Boolean
End Class

Module Program
    Async Function Main(args As String()) As Task(Of Integer)
        Dim token = GetEnvironmentVariable("GITHUB_TOKEN")
        
        ' Get user information
        Dim userCommand = "curl https://api.github.com/user " &
                          "-H 'Accept: application/vnd.github.v3+json' " &
                          "-H 'Authorization: token " & token & "'"
        
        Dim userResult = Await Curl.ExecuteAsync(userCommand)
        
        If userResult.IsSuccess Then
            Dim user = JsonSerializer.Deserialize(Of GitHubUser)(userResult.Body)
            Console.WriteLine($"Logged in as: {user.Login}")
            Console.WriteLine($"Name: {user.Name}")
            Console.WriteLine($"Public repos: {user.PublicRepos}")
        End If
        
        ' List repositories
        Dim reposCommand = "curl https://api.github.com/user/repos " &
                           "-H 'Accept: application/vnd.github.v3+json' " &
                           "-H 'Authorization: token " & token & "'"
        
        Dim reposResult = Await Curl.ExecuteAsync(reposCommand)
        
        If reposResult.IsSuccess Then
            Dim repos = JsonSerializer.Deserialize(Of Repository())(reposResult.Body)
            Console.WriteLine($"{vbCrLf}You have {repos.Length} repositories:")
            For Each repo In repos
                Console.WriteLine($"  - {repo.FullName}")
            Next
        End If
        
        Return 0
    End Function
End Module
```

## Using the Fluent Builder API

The fluent builder API also works in VB.NET:

```vb
Imports CurlDotNet
Imports CurlDotNet.Core
Imports System.Threading.Tasks

Module Program
    Async Function Main(args As String()) As Task(Of Integer)
        Dim result = Await CurlRequestBuilder.
            Post("https://api.example.com/users").
            WithHeader("Content-Type", "application/json").
            WithJson(New With {.name = "John", .email = "john@example.com"}).
            WithTimeout(TimeSpan.FromSeconds(30)).
            FollowRedirects().
            ExecuteAsync()
        
        If result.IsSuccess Then
            Console.WriteLine($"Status: {result.StatusCode}")
        End If
        
        Return 0
    End Function
End Module
```

## Conclusion

CurlDotNet works great with VB.NET! You can paste curl commands directly into your VB.NET code, making API integration straightforward and easy. The async/await syntax in VB.NET pairs perfectly with CurlDotNet's async methods.

Try it in your VB.NET project - you'll find it makes HTTP requests much simpler!

---

**Learn More:**
- [CurlDotNet on GitHub](https://github.com/jacob/curl-dot-net)
- [NuGet Package](https://www.nuget.org/packages/CurlDotNet)
- [VB.NET Documentation](https://docs.microsoft.com/en-us/dotnet/visual-basic/)

