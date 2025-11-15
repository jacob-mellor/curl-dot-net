---
title: "How to Use curl in .NET (F#)"
published: true
tags: ['dotnet', 'fsharp', 'http', 'api', 'curl', 'rest', 'tutorial', 'functional']
date: 2025-01-20
canonical_url: https://dev.to/jacob/how-to-curl-in-dotnet-fsharp
---

# How to Use curl in .NET (F#)

F# developers can also use CurlDotNet to execute curl commands directly in their functional code. The library works seamlessly with F#'s async workflows and type system.

## Installing CurlDotNet

Add the NuGet package to your F# project:

```bash
dotnet add package CurlDotNet
```

Or in your `.fsproj`:

```xml
<ItemGroup>
  <PackageReference Include="CurlDotNet" Version="1.0.0" />
</ItemGroup>
```

## Basic Usage in F#

Here's a simple example in F#:

```fsharp
open CurlDotNet
open System
open System.Threading.Tasks

let main argv =
    async {
        let! result = Curl.ExecuteAsync("curl https://api.github.com/users/octocat") |> Async.AwaitTask
        
        printfn "Status: %d" result.StatusCode
        printfn "Body: %s" result.Body
    } |> Async.RunSynchronously
    
    0
```

## Using F# Async Workflows

F#'s async workflows work naturally with CurlDotNet:

```fsharp
open CurlDotNet
open System
open System.Threading.Tasks

let fetchUser userId =
    async {
        let command = sprintf "curl https://api.github.com/users/%s" userId
        let! result = Curl.ExecuteAsync(command) |> Async.AwaitTask
        
        if result.IsSuccess then
            return Some result.Body
        else
            return None
    }

let main argv =
    async {
        match! fetchUser "octocat" with
        | Some body -> printfn "Success: %s" body
        | None -> printfn "Failed to fetch user"
    } |> Async.RunSynchronously
    
    0
```

## Working with JSON

F# can work with JSON responses using standard .NET JSON libraries:

```fsharp
open CurlDotNet
open System.Text.Json

type User = {
    Login: string
    Name: string
    PublicRepos: int
}

let fetchUser userId =
    async {
        let command = sprintf "curl https://api.github.com/users/%s" userId
        let! result = Curl.ExecuteAsync(command) |> Async.AwaitTask
        
        if result.IsSuccess then
            let user = JsonSerializer.Deserialize<User>(result.Body)
            return Some user
        else
            return None
    }

let main argv =
    async {
        match! fetchUser "octocat" with
        | Some user ->
            printfn "User: %s" user.Name
            printfn "Repos: %d" user.PublicRepos
        | None ->
            printfn "Failed"
    } |> Async.RunSynchronously
    
    0
```

## POST Requests

Here's how to make POST requests in F#:

```fsharp
open CurlDotNet
open System.Text.Json

let createPost title body userId =
    async {
        let command = sprintf @"
          curl -X POST https://jsonplaceholder.typicode.com/posts \
            -H 'Content-Type: application/json' \
            -d '{""title"":""%s"",""body"":""%s"",""userId"":%d}'
        " title body userId
        
        let! result = Curl.ExecuteAsync(command) |> Async.AwaitTask
        return result
    }

let main argv =
    async {
        let! result = createPost "My Post" "This is content" 1
        
        if result.IsSuccess then
            printfn "Created post with status: %d" result.StatusCode
    } |> Async.RunSynchronously
    
    0
```

## Error Handling

F#'s pattern matching makes error handling elegant:

```fsharp
open CurlDotNet
open CurlDotNet.Exceptions

let safeExecute command =
    async {
        try
            let! result = Curl.ExecuteAsync(command) |> Async.AwaitTask
            return Ok result
        with
        | :? CurlDnsException as ex -> return Error $"DNS error: {ex.Message}"
        | :? CurlTimeoutException as ex -> return Error $"Timeout: {ex.Message}"
        | :? CurlHttpException as ex -> return Error $"HTTP {ex.StatusCode}: {ex.Message}"
        | :? CurlException as ex -> return Error $"Error: {ex.Message}"
    }

let main argv =
    async {
        match! safeExecute "curl https://api.github.com/users/octocat" with
        | Ok result ->
            printfn "Success: %s" result.Body
        | Error msg ->
            printfn "Failed: %s" msg
    } |> Async.RunSynchronously
    
    0
```

## Using Computation Expressions

You can create a custom computation expression for cleaner syntax:

```fsharp
open CurlDotNet
open System

type CurlBuilder() =
    member _.Yield(result: CurlResult) = result
    member _.Bind(result: Task<CurlResult>, f) = 
        async {
            let! res = Async.AwaitTask result
            return! f res |> Async.AwaitTask
        } |> Async.StartAsTask
    
    member _.Zero() = Task.FromResult(Unchecked.defaultof<CurlResult>)

let curl = CurlBuilder()

let fetchData =
    async {
        let! result = Curl.ExecuteAsync("curl https://api.github.com/users/octocat") |> Async.AwaitTask
        
        if result.IsSuccess then
            printfn "Data: %s" result.Body
    }
```

## Pipelining and Composition

F#'s pipelining works great with CurlDotNet:

```fsharp
open CurlDotNet

let fetchUser userId =
    sprintf "curl https://api.github.com/users/%s" userId
    |> Curl.ExecuteAsync
    |> Async.AwaitTask

let parseBody (result: CurlResult) =
    if result.IsSuccess then Some result.Body else None

let main argv =
    async {
        "octocat"
        |> fetchUser
        |> Async.RunSynchronously
        |> parseBody
        |> Option.iter (printfn "Body: %s")
    } |> Async.RunSynchronously
    
    0
```

## Real-World Example: GitHub API Client

Here's a complete F# example:

```fsharp
open CurlDotNet
open System
open System.Text.Json

type GitHubUser = {
    Login: string
    Name: string
    PublicRepos: int
    Followers: int
}

type Repository = {
    Name: string
    FullName: string
    Private: bool
    Description: string option
}

let githubApi token endpoint =
    async {
        let command = sprintf @"
          curl %s \
            -H 'Accept: application/vnd.github.v3+json' \
            -H 'Authorization: token %s'
        " endpoint token
        
        let! result = Curl.ExecuteAsync(command) |> Async.AwaitTask
        
        if result.IsSuccess then
            return Ok result.Body
        else
            return Error $"HTTP {result.StatusCode}"
    }

let getUser token =
    async {
        match! githubApi token "https://api.github.com/user" with
        | Ok body -> 
            let user = JsonSerializer.Deserialize<GitHubUser>(body)
            return Some user
        | Error _ -> return None
    }

let getRepos token =
    async {
        match! githubApi token "https://api.github.com/user/repos" with
        | Ok body ->
            let repos = JsonSerializer.Deserialize<Repository[]>(body)
            return Some repos
        | Error _ -> return None
    }

let main argv =
    let token = Environment.GetEnvironmentVariable("GITHUB_TOKEN")
    
    async {
        match! getUser token with
        | Some user ->
            printfn "Logged in as: %s" user.Login
            printfn "Name: %s" user.Name
            printfn "Public repos: %d" user.PublicRepos
            
            match! getRepos token with
            | Some repos ->
                printfn "\nRepositories:"
                repos
                |> Array.iter (fun repo -> printfn "  - %s" repo.FullName)
            | None -> printfn "Failed to fetch repos"
        | None -> printfn "Failed to authenticate"
    } |> Async.RunSynchronously
    
    0
```

## Conclusion

CurlDotNet works beautifully with F#'s functional programming style. The async workflows, pattern matching, and pipelining all make for clean, elegant code when working with HTTP requests.

Try it in your F# project - you'll find it fits naturally with F#'s idioms!

---

**Learn More:**
- [CurlDotNet on GitHub](https://github.com/jacob/curl-dot-net)
- [NuGet Package](https://www.nuget.org/packages/CurlDotNet)
- [F# Documentation](https://docs.microsoft.com/en-us/dotnet/fsharp/)

