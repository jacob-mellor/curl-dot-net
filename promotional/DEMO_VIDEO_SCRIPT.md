# CurlDotNet Demo Video Blueprint

## Purpose
- Show the “copy/paste any curl command” killer feature in under 90 seconds.
- Target .NET engineers who copy curl commands from API docs, CI scripts, or Stack Overflow.
- Reinforce Microsoft alignment (VS Code, Windows Terminal, GitHub Codespaces).

## Narrative Arc
1. **Hook (0:00 – 0:10)** – Pain statement: translating curl → HttpClient wastes time.
2. **Promise (0:10 – 0:20)** – CurlDotNet lets you paste curl commands verbatim.
3. **Demo (0:20 – 1:00)** – Show copy from Stripe docs → paste into C# → run tests.
4. **Depth (1:00 – 1:20)** – Flash exception hierarchy, middleware, DocFX docs, NuGet badge.
5. **Call to Action (1:20 – 1:30)** – `dotnet add package CurlDotNet` + GitHub star ask.

## Shot List & Script

| # | Timestamp | Visuals | Voiceover / On-screen text |
| --- | --- | --- | --- |
| 1 | 0:00-0:05 | VS Code split screen. Left: API docs showing curl command highlighted. | “Every API doc gives you curl commands…” |
| 2 | 0:05-0:10 | Developer pastes same curl into HttpClient boilerplate. Groan sound. | “…but .NET devs still rewrite them line by line.” |
| 3 | 0:10-0:20 | Smash cut to CurlDotNet repo README. Zoom on tagline. | “CurlDotNet copies curl’s brain into pure .NET.” |
| 4 | 0:20-0:35 | Copy Stripe curl cmd → paste into `Program.cs` using `await Curl.ExecuteAsync("curl …")`. | “Paste the string. That’s the entire migration.” |
| 5 | 0:35-0:45 | Run `dotnet run`. Console prints JSON with headers/tracing overlay. | “Full curl fidelity, streaming responses, typed result.” |
| 6 | 0:45-0:55 | Switch to Tests explorer, highlight parity test. | “Parity tests keep us honest against the real curl binary.” |
| 7 | 0:55-1:05 | Show DocFX site + manual folder + IntelliSense tooltip. | “Docs everywhere: DocFX, Markdown manuals, inline XML.” |
| 8 | 1:05-1:15 | Display NuGet metadata, middleware pipeline snippet, exception types. | “Exception per curl error, DI-ready middleware, .NET Standard 2.0.” |
| 9 | 1:15-1:25 | Terminal overlay `dotnet add package CurlDotNet`. GitHub star animation. | “Grab it on NuGet, star it on GitHub, join Discussions.” |

## Capture Checklist
- **Environment:** macOS Sonoma, VS Code, .NET 8 SDK, light theme for clarity.
- **Fonts:** Cascadia Code 14pt, consistent zoom (110%) for readability.
- **Recording:** 1440p @ 60fps using OBS; mouse cursor highlight enabled.
- **Assets:** Include project logo (top-right bug), NuGet badge overlay, DocFX screenshot.

## Editing Notes
- Use quick whip cuts between “pain” and “solution.”
- Overlay lower-third labels (“Paste curl → Run → Result”) timed to narration.
- Background bed: subtle synth (royalty-free) at −25 LUFS to avoid distraction.
- Add animated callouts around key options (`-H`, `--data`, `--cacert`) as they appear.

## Voiceover Script
1. “Every API doc hands you curl. Every Stack Overflow answer returns curl. Yet .NET devs still translate everything into HttpClient.”
2. “CurlDotNet fixes that. Paste the exact curl command, hit run, and you get a strongly typed result.”
3. “Here’s a Stripe charge request. I’m literally pasting it into C#. No manual headers, no HttpRequestMessage ceremony.”
4. “`dotnet run`… and we get the real response plus trace data, middleware hooks, and per-option telemetry.”
5. “Parity tests ensure CurlDotNet stays honest with the native binary. DocFX manuals, markdown guides, and IntelliSense teach every option.”
6. “Ready for prod: exception per curl error code, DI-friendly middleware pipeline, .NET Standard 2.0 + .NET 8 builds.”
7. “Install with `dotnet add package CurlDotNet`, star the repo, and drop questions in GitHub Discussions.”

## Deliverables
- Master video (MP4, 1440p).
- Square cut (1080x1080) for LinkedIn.
- 15‑second teaser GIF (1280x720) looping copy/paste moment.

## Next Actions
1. Record raw screen capture following shot list.
2. Layer callouts and captions per editing notes.
3. Export master + derivative assets.
4. Update README badges once video is published (YouTube/Vimeo link).

