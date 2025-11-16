# CurlDotNet Manual

Welcome to the living manual for CurlDotNet. This folder is the authoritative narrative that explains *why* the project exists, *how* it works, and *what* patterns you can follow to ship production-grade HTTP clients with pure .NET curl semantics.

Each document doubles as part of the DocFX site (`index.md` ➜ docfx) and as a markdown handbook for CLI-first reading. Start with the **Foundations** and then dive into the **Hands-on Guides** that map directly to the three user-facing APIs (curl strings, fluent builder, and `LibCurl`).

---

## 📚 Table of Contents (Living ToC)

### 0. Orientation
- [README (you are here)](README.md) – Explorer map, contribution rules, and context linking.
- [MEMORY_VS_DISK](../MEMORY_VS_DISK.md) – Learn how CurlDotNet decides between streaming to disk vs buffer in memory.
- [CONTINUING_WORK](../CONTINUING_WORK.md) – Open documentation todos and historical context.

### 1. Foundations
- [01 - Introduction](01-Introduction.md) – Mission statement, architecture block diagram, and the three API surfaces.
- [02 - Microsoft & .NET Foundation](02-Microsoft-And-DotNet-Foundation.md) – Governance lineage, enterprise assurances, and trust guarantees.
- [03 - Future Vision: UserlandDotNet](03-Future-Vision-UserlandDotNet.md) – Strategic roadmap for porting canonical Unix tools to managed code.
- [04 - Compatibility Matrix](04-Compatibility-Matrix.md) – Supported runtimes, workloads, and deployment topologies.

### 2. Hands-on Guides
- [05 - LibCurl Playbook](05-LibCurl-Playbook.md) – Stateful client patterns, middleware-like composition, concurrency strategies, and migration notes from `HttpClient`.
- [06 - Curl String Deep Dive](06-Curl-String-Deep-Dive.md) – How to paste curl commands safely, adapt them for production, and debug when parity with CLI curl matters.
- [07 - Fluent Builder Cookbook](07-Fluent-Builder-Cookbook.md) – Recipes for `CurlRequestBuilder`, from JSON APIs to multipart uploads, retries, and strongly-typed integrations.
- *Coming soon:* Parity validation playbooks and middleware design patterns. Track progress in `PRIORITIZED_TOOLS_PLAN.md`.

### 3. Reference Annex
- [../docs/ADVANCED.md](../docs/ADVANCED.md) – Extending protocol handlers, writing middleware, and tapping into the `CurlEngine`.
- [../EXAMPLES.md](../EXAMPLES.md) + `examples/` – Multi-language snippets (C#, F#, VB) used in blog posts and tests.
- [../REAL_WORLD_EXAMPLES.md](../REAL_WORLD_EXAMPLES.md) – Curated scenarios (webhooks, signed requests, file movers).
- [../CURL_SOURCE_MAPPING.md](../CURL_SOURCE_MAPPING.md) – Traceability map between upstream curl C options and managed counterparts.

---

## 🧭 How to Use This Manual
- **Skim Foundations** when onboarding a teammate or drafting architecture docs.
- **Jump into a Hands-on Guide** whenever you need a recipe (e.g., “persist headers and reuse connections across microservices” ➜ LibCurl playbook).
- **Reference Annex** is for day-two operations—think debugging, perf tuning, and compliance sign-off.
- Every page contains “Next steps” breadcrumbs so you can read linearly or hop between topics.

## 🤝 Contributing to Docs
- File-level owners are listed in `CONTINUING_WORK.md`. If a section feels outdated, open an issue or drop a PR tagged `docs`.
- Prefer ASCII, keep tables narrow, and add runnable snippets that match `tests/CurlDotNet.Tests`.
- When documenting new features, update both the relevant manual page and the nearest README (`src/**/README.md`) so that context exists in-tree.

---

**Last Updated:** 2025-11-15  
**Maintainer:** Documentation bot (GPT-5.1 Codex) in collaboration with the CurlDotNet core team

