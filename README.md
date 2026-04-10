# A9N Desktop

**A native Windows agentic framework** built with WinUI 3 and .NET 10.

Runtime model swapping, 27+ tools with parallel execution, native Telegram/Discord gateway, production hardening, persistent identity system, 94 skills, and a wiki-based knowledge base.

**Current version: v2.2.0**

## What This Is

A9N Desktop is a **Windows-native agent runtime, control plane, and UX layer** for agentic workflows. It runs an in-process agent with tool calling, context management, and provider abstraction — no Python dependencies required for core functionality.

- **In-process agent runtime** with tool calling, permissions, and context management
- **Runtime model swapping** — switch Anthropic, OpenAI, Ollama, Qwen mid-conversation
- **Native Telegram and Discord** — messaging works without external CLI tools
- **27+ tools** with parallel execution (8 workers for read-only operations)
- **Production hardened** — compression cooldown, provider fallback, atomic persistence, secret scanning
- **94 skills** across 28 categories with visual browser
- **Wiki knowledge base** with SQLite FTS5 search

## Desktop Application

9 pages, each pulling real data from the agent runtime:

| Page | Description |
|------|-------------|
| **Dashboard** | KPI cards, usage insights, platform badges, recent sessions |
| **Chat** | Agent chat with tool calling, reasoning display, model switcher, side panels |
| **Agent** | Identity editor (SOUL.md, USER.md), souls browser, agent profiles |
| **Skills** | Searchable library with category chips, color-coded badges, sort, preview |
| **Memory** | Memory browser with type badges, project rules editor |
| **Buddy** | Companion with deterministic ASCII art, stats, personality |
| **Integrations** | Native C# gateway (Telegram, Discord) + Python sidecar for advanced platforms |
| **Settings** | 9 sections: User Profile, Model, Agent, Gateway, Memory, Display, Execution, Plugins, Paths |

## Providers

Switch between providers mid-conversation — no restart needed:

- **Anthropic** (Claude Sonnet 4.6, Opus) — with full tool calling
- **OpenAI** (GPT-5.4, GPT-5.4 Mini)
- **Ollama** (any local model — GLM, Gemma, Llama, etc.)
- **Qwen, DeepSeek, MiniMax, OpenRouter, Nous**

## Quick Start

### Prerequisites

- Windows 10 (1809+) or Windows 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Windows App SDK 1.7](https://learn.microsoft.com/windows/apps/windows-app-sdk/)

### Build and Run

```bash
git clone https://github.com/RedWoodOG/Windows-Agentic-Framework.git
cd Windows-Agentic-Framework
dotnet build Desktop/A9NDesktop/A9NDesktop.csproj
```

Register and launch:

```powershell
cd Desktop\A9NDesktop\bin\x64\Debug\net10.0-windows10.0.26100.0\win-x64
Add-AppxPackage -Register AppxManifest.xml
```

### Configuration

Create `%LOCALAPPDATA%\a9n\config.yaml`:

```yaml
model:
  provider: anthropic
  default: claude-sonnet-4-6
  base_url: https://api.anthropic.com
  api_key: sk-ant-your-key-here

# Keys for runtime model swapping
provider_keys:
  anthropic: sk-ant-your-key
  openai: sk-proj-your-key
  ollama_url: http://127.0.0.1:11434/v1

# Native Telegram gateway
platforms:
  telegram:
    token: "your-bot-token"
    enabled: true
```

## Project Structure

```
Windows-Agentic-Framework/
├── src/                              # Core agent library (A9N.Core)
│   ├── Core/                         # Agent loop, models, interfaces
│   ├── Tools/                        # 27+ tool implementations
│   ├── LLM/                          # Provider abstraction, ChatClientFactory
│   ├── wiki/                         # Wiki system with FTS5 search
│   ├── Context/                      # PromptBuilder, TokenBudget, ContextManager
│   ├── gateway/                      # Native Telegram/Discord adapters
│   ├── soul/                         # Identity system (SOUL.md, USER.md)
│   ├── memory/                       # Memory manager
│   ├── skills/                       # Skill system
│   ├── transcript/                   # Session persistence (JSONL)
│   ├── security/                     # Secret scanning, SSRF protection
│   └── compaction/                   # Context compression with cooldown
├── Desktop/A9NDesktop/               # WinUI 3 desktop application
│   ├── Views/                        # 9 pages + side panels
│   ├── Services/                     # A9NChatService, A9NEnvironment
│   └── Controls/                     # CodeBlock, PermissionDialog
├── skills/                           # 94 skill definitions (28 categories)
└── docs/                             # Architecture documentation
```

## Production Hardening

| Pattern | What It Prevents |
|---------|-----------------|
| **Compression cooldown** (600s) | Infinite token-burning retry loops |
| **Provider fallback** with restoration | Stuck on expensive fallback provider |
| **Credential pool rotation** on 401/429 | Silent key exhaustion |
| **Atomic writes** (WriteThrough) | Data loss on crash |
| **Deterministic tool-call IDs** | Prompt cache misses |
| **Secret scanning** on all outputs | API key exposure |
| **Parallel execution** (8 workers) | Slow sequential tool calls |

## License

MIT
