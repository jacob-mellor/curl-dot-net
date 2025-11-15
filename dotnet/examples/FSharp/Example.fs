/***************************************************************************
 * CurlDotNet F# Examples
 *
 * Examples showing how to use CurlDotNet in F#
 ***************************************************************************/

open CurlDotNet
open System
open System.Threading.Tasks

module Program =
    let simpleGet () =
        async {
            let result = Curl.ExecuteAsync("curl https://jsonplaceholder.typicode.com/posts/1")
                          |> Async.AwaitTask
                          |> Async.RunSynchronously
            
            if result.IsSuccess then
                printfn "Status: %d" result.StatusCode
                printfn "Body: %s" (result.Body.Substring(0, min 100 result.Body.Length))
        }

    let postWithJson () =
        async {
            let command = """
                curl -X POST https://jsonplaceholder.typicode.com/posts \
                    -H 'Content-Type: application/json' \
                    -d '{
                      "title": "My Post",
                      "body": "This is the content",
                      "userId": 1
                    }'
            """
            
            let result = Curl.ExecuteAsync(command)
                          |> Async.AwaitTask
                          |> Async.RunSynchronously
            
            if result.IsSuccess then
                printfn "Created post with status: %d" result.StatusCode
                printfn "Response: %s" (result.Body.Substring(0, min 200 result.Body.Length))
        }

    let fluentBuilder () =
        async {
            let result = CurlRequestBuilder
                           .Get("https://jsonplaceholder.typicode.com/users/1")
                           .WithHeader("Accept", "application/json")
                           .WithTimeout(TimeSpan.FromSeconds(30.0))
                           .FollowRedirects()
                           .ExecuteAsync()
                           |> Async.AwaitTask
                           |> Async.RunSynchronously
            
            if result.IsSuccess then
                printfn "User data: %s" (result.Body.Substring(0, min 150 result.Body.Length))
        }

    let fileOperations () =
        async {
            let downloadResult = Curl.ExecuteAsync("curl -o example.txt https://jsonplaceholder.typicode.com/posts/1")
                                      |> Async.AwaitTask
                                      |> Async.RunSynchronously
            
            if downloadResult.IsSuccess then
                printfn "File saved to: %s" downloadResult.OutputFiles.[0]
                printfn "Size in memory: %d bytes" downloadResult.Body.Length
        }

    let authentication () =
        async {
            // Basic auth
            let basicAuthResult = Curl.ExecuteAsync("curl -u username:password https://httpbin.org/basic-auth/username/password")
                                        |> Async.AwaitTask
                                        |> Async.RunSynchronously
            
            printfn "Basic auth result: %d" basicAuthResult.StatusCode
            
            // Bearer token
            let bearerResult = Curl.ExecuteAsync("curl -H 'Authorization: Bearer token123' https://httpbin.org/headers")
                                    |> Async.AwaitTask
                                    |> Async.RunSynchronously
            
            if bearerResult.IsSuccess then
                printfn "Bearer token result: %d" bearerResult.StatusCode
        }

    [<EntryPoint>]
    let main argv =
        printfn "=== CurlDotNet F# Examples ===\n"
        
        simpleGet () |> Async.RunSynchronously
        printfn ""
        
        postWithJson () |> Async.RunSynchronously
        printfn ""
        
        fluentBuilder () |> Async.RunSynchronously
        printfn ""
        
        fileOperations () |> Async.RunSynchronously
        printfn ""
        
        authentication () |> Async.RunSynchronously
        printfn ""
        
        printfn "=== Examples Complete ==="
        0

