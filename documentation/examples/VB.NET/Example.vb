' ***************************************************************************
' CurlDotNet VB.NET Examples
'
' Examples showing how to use CurlDotNet in VB.NET
' ***************************************************************************

Imports CurlDotNet
Imports CurlDotNet.Core
Imports System
Imports System.Threading.Tasks

Module Program
    Async Function Main(args As String()) As Task(Of Integer)
        Console.WriteLine("=== CurlDotNet VB.NET Examples ===\n")

        ' Example 1: Simple GET request
        Await SimpleGet()

        ' Example 2: POST with JSON
        Await PostWithJson()

        ' Example 3: Using fluent builder
        Await FluentBuilder()

        ' Example 4: File operations
        Await FileOperations()

        ' Example 5: Authentication
        Await Authentication()

        Console.WriteLine("\n=== Examples Complete ===")
        Return 0
    End Function

    Async Function SimpleGet() As Task
        Console.WriteLine("Example 1: Simple GET Request")

        Dim result = Await Curl.ExecuteAsync("curl https://jsonplaceholder.typicode.com/posts/1")

        If result.IsSuccess Then
            Console.WriteLine($"Status: {result.StatusCode}")
            Console.WriteLine($"Body: {result.Body.Substring(0, Math.Min(100, result.Body.Length))}...")
        End If

        Console.WriteLine()
    End Function

    Async Function PostWithJson() As Task
        Console.WriteLine("Example 2: POST with JSON")

        Dim command = "curl -X POST https://jsonplaceholder.typicode.com/posts " &
                      "-H 'Content-Type: application/json' " &
                      "-d '{""title"":""My Post"",""body"":""This is the content"",""userId"":1}'"

        Dim result = Await Curl.ExecuteAsync(command)

        If result.IsSuccess Then
            Console.WriteLine($"Created post with status: {result.StatusCode}")
            Console.WriteLine($"Response: {result.Body.Substring(0, Math.Min(200, result.Body.Length))}...")
        End If

        Console.WriteLine()
    End Function

    Async Function FluentBuilder() As Task
        Console.WriteLine("Example 3: Fluent Builder API")

        Dim result = Await CurlRequestBuilder.
            Get("https://jsonplaceholder.typicode.com/users/1").
            WithHeader("Accept", "application/json").
            WithTimeout(TimeSpan.FromSeconds(30)).
            FollowRedirects().
            ExecuteAsync()

        If result.IsSuccess Then
            Console.WriteLine($"User data: {result.Body.Substring(0, Math.Min(150, result.Body.Length))}...")
        End If

        Console.WriteLine()
    End Function

    Async Function FileOperations() As Task
        Console.WriteLine("Example 4: File Operations")

        ' Download and save
        Dim downloadResult = Await Curl.ExecuteAsync("curl -o example.txt https://jsonplaceholder.typicode.com/posts/1")

        If downloadResult.IsSuccess Then
            Console.WriteLine($"File saved to: {downloadResult.OutputFiles(0)}")
            Console.WriteLine($"Size in memory: {downloadResult.Body.Length} bytes")
        End If

        Console.WriteLine()
    End Function

    Async Function Authentication() As Task
        Console.WriteLine("Example 5: Authentication")

        ' Basic auth
        Dim basicAuthResult = Await Curl.ExecuteAsync("curl -u username:password https://httpbin.org/basic-auth/username/password")

        Console.WriteLine($"Basic auth result: {basicAuthResult.StatusCode}")

        ' Bearer token
        Dim bearerResult = Await Curl.ExecuteAsync("curl -H 'Authorization: Bearer token123' https://httpbin.org/headers")

        If bearerResult.IsSuccess Then
            Console.WriteLine($"Bearer token result: {bearerResult.StatusCode}")
        End If

        Console.WriteLine()
    End Function
End Module

