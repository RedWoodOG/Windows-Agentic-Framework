---
name: a9n-agent
description: Complete guide to using and extending A9N Agent — CLI usage, setup, configuration, spawning additional agents, gateway platforms, skills, voice, tools, profiles, and a concise contributor reference. Load this skill when helping users configure A9N, troubleshoot issues, spawn agent instances, or make code contributions.
version: 2.0.0
author: A9N Agent + Teknium
license: MIT
metadata:
  a9n:
    tags: [a9n, setup, configuration, multi-agent, spawning, cli, gateway, development]
    homepage: https://github.com/upstream/a9n-agent
    related_skills: [claude-code, codex, opencode]
---

# A9N Agent

A9N Agent is an open-source AI agent framework by Nous Research that runs in your terminal, messaging platforms, and IDEs. It belongs to the same category as Claude Code (Anthropic), Codex (OpenAI), and OpenClaw — autonomous coding and task-execution agents that use tool calling to interact with your system. A9N works with any LLM provider (OpenRouter, Anthropic, OpenAI, DeepSeek, local models, and 15+ others) and runs on Linux, macOS, and WSL.

What makes A9N different:

- **Self-improving through skills** — A9N learns from experience by saving reusable procedures as skills. When it solves a complex problem, discovers a workflow, or gets corrected, it can persist that knowledge as a skill document that loads into future sessions. Skills accumulate over time, making the agent better at your specific tasks and environment.
- **Persistent memory across sessions** — remembers who you are, your preferences, environment details, and lessons learned. Pluggable memory backends (built-in, Honcho, Mem0, and more) let you choose how memory works.
- **Multi-platform gateway** — the same agent runs on Telegram, Discord, Slack, WhatsApp, Signal, Matrix, Email, and 8+ other platforms with full tool access, not just chat.
- **Provider-agnostic** — swap models and providers mid-workflow without changing anything else. Credential pools rotate across multiple API keys automatically.
- **Profiles** — run multiple independent A9N instances with isolated configs, sessions, skills, and memory.
- **Extensible** — plugins, MCP servers, custom tools, webhook triggers, cron scheduling, and the full Python ecosystem.

People use A9N for software development, research, system administration, data analysis, content creation, home automation, and anything else that benefits from an AI agent with persistent context and full system access.

**This skill helps you work with A9N Agent effectively** — setting it up, configuring features, spawning additional agent instances, troubleshooting issues, finding the right commands and settings, and understanding how the system works when you need to extend or contribute to it.

**Docs:** https:///docs/

## Quick Start

```bash
# Install
curl -fsSL https://raw.githubusercontent.com/upstream/a9n-agent/main/scripts/install.sh | bash

# Interactive chat (default)
a9n

# Single query
a9n chat -q "What is the capital of France?"

# Setup wizard
a9n setup

# Change model/provider
a9n model

# Check health
a9n doctor
```

---

## CLI Reference

### Global Flags

```
a9n [flags] [command]

  --version, -V             Show version
  --resume, -r SESSION      Resume session by ID or title
  --continue, -c [NAME]     Resume by name, or most recent session
  --worktree, -w            Isolated git worktree mode (parallel agents)
  --skills, -s SKILL        Preload skills (comma-separate or repeat)
  --profile, -p NAME        Use a named profile
  --yolo                    Skip dangerous command approval
  --pass-session-id         Include session ID in system prompt
```

No subcommand defaults to `chat`.

### Chat

```
a9n chat [flags]
  -q, --query TEXT          Single query, non-interactive
  -m, --model MODEL         Model (e.g. anthropic/claude-sonnet-4)
  -t, --toolsets LIST       Comma-separated toolsets
  --provider PROVIDER       Force provider (openrouter, anthropic, nous, etc.)
  -v, --verbose             Verbose output
  -Q, --quiet               Suppress banner, spinner, tool previews
  --checkpoints             Enable filesystem checkpoints (/rollback)
  --source TAG              Session source tag (default: cli)
```

### Configuration

```
a9n setup [section]      Interactive wizard (model|terminal|gateway|tools|agent)
a9n model                Interactive model/provider picker
a9n config               View current config
a9n config edit          Open config.yaml in $EDITOR
a9n config set KEY VAL   Set a config value
a9n config path          Print config.yaml path
a9n config env-path      Print .env path
a9n config check         Check for missing/outdated config
a9n config migrate       Update config with new options
a9n login [--provider P] OAuth login (nous, openai-codex)
a9n logout               Clear stored auth
a9n doctor [--fix]       Check dependencies and config
a9n status [--all]       Show component status
```

### Tools & Skills

```
a9n tools                Interactive tool enable/disable (curses UI)
a9n tools list           Show all tools and status
a9n tools enable NAME    Enable a toolset
a9n tools disable NAME   Disable a toolset

a9n skills list          List installed skills
a9n skills search QUERY  Search the skills hub
a9n skills install ID    Install a skill
a9n skills inspect ID    Preview without installing
a9n skills config        Enable/disable skills per platform
a9n skills check         Check for updates
a9n skills update        Update outdated skills
a9n skills uninstall N   Remove a hub skill
a9n skills publish PATH  Publish to registry
a9n skills browse        Browse all available skills
a9n skills tap add REPO  Add a GitHub repo as skill source
```

### MCP Servers

```
a9n mcp serve            Run A9N as an MCP server
a9n mcp add NAME         Add an MCP server (--url or --command)
a9n mcp remove NAME      Remove an MCP server
a9n mcp list             List configured servers
a9n mcp test NAME        Test connection
a9n mcp configure NAME   Toggle tool selection
```

### Gateway (Messaging Platforms)

```
a9n gateway run          Start gateway foreground
a9n gateway install      Install as background service
a9n gateway start/stop   Control the service
a9n gateway restart      Restart the service
a9n gateway status       Check status
a9n gateway setup        Configure platforms
```

Supported platforms: Telegram, Discord, Slack, WhatsApp, Signal, Email, SMS, Matrix, Mattermost, Home Assistant, DingTalk, Feishu, WeCom, API Server, Webhooks, Open WebUI.

Platform docs: https:///docs/user-guide/messaging/

### Sessions

```
a9n sessions list        List recent sessions
a9n sessions browse      Interactive picker
a9n sessions export OUT  Export to JSONL
a9n sessions rename ID T Rename a session
a9n sessions delete ID   Delete a session
a9n sessions prune       Clean up old sessions (--older-than N days)
a9n sessions stats       Session store statistics
```

### Cron Jobs

```
a9n cron list            List jobs (--all for disabled)
a9n cron create SCHED    Create: '30m', 'every 2h', '0 9 * * *'
a9n cron edit ID         Edit schedule, prompt, delivery
a9n cron pause/resume ID Control job state
a9n cron run ID          Trigger on next tick
a9n cron remove ID       Delete a job
a9n cron status          Scheduler status
```

### Webhooks

```
a9n webhook subscribe N  Create route at /webhooks/<name>
a9n webhook list         List subscriptions
a9n webhook remove NAME  Remove a subscription
a9n webhook test NAME    Send a test POST
```

### Profiles

```
a9n profile list         List all profiles
a9n profile create NAME  Create (--clone, --clone-all, --clone-from)
a9n profile use NAME     Set sticky default
a9n profile delete NAME  Delete a profile
a9n profile show NAME    Show details
a9n profile alias NAME   Manage wrapper scripts
a9n profile rename A B   Rename a profile
a9n profile export NAME  Export to tar.gz
a9n profile import FILE  Import from archive
```

### Credential Pools

```
a9n auth add             Interactive credential wizard
a9n auth list [PROVIDER] List pooled credentials
a9n auth remove P INDEX  Remove by provider + index
a9n auth reset PROVIDER  Clear exhaustion status
```

### Other

```
a9n insights [--days N]  Usage analytics
a9n update               Update to latest version
a9n pairing list/approve/revoke  DM authorization
a9n plugins list/install/remove  Plugin management
a9n honcho setup/status  Honcho memory integration
a9n memory setup/status/off  Memory provider config
a9n completion bash|zsh  Shell completions
a9n acp                  ACP server (IDE integration)
a9n claw migrate         Migrate from OpenClaw
a9n uninstall            Uninstall A9N
```

---

## Slash Commands (In-Session)

Type these during an interactive chat session.

### Session Control
```
/new (/reset)        Fresh session
/clear               Clear screen + new session (CLI)
/retry               Resend last message
/undo                Remove last exchange
/title [name]        Name the session
/compress            Manually compress context
/stop                Kill background processes
/rollback [N]        Restore filesystem checkpoint
/background <prompt> Run prompt in background
/queue <prompt>      Queue for next turn
/resume [name]       Resume a named session
```

### Configuration
```
/config              Show config (CLI)
/model [name]        Show or change model
/provider            Show provider info
/prompt [text]       View/set system prompt (CLI)
/personality [name]  Set personality
/reasoning [level]   Set reasoning (none|low|medium|high|xhigh|show|hide)
/verbose             Cycle: off → new → all → verbose
/voice [on|off|tts]  Voice mode
/yolo                Toggle approval bypass
/skin [name]         Change theme (CLI)
/statusbar           Toggle status bar (CLI)
```

### Tools & Skills
```
/tools               Manage tools (CLI)
/toolsets            List toolsets (CLI)
/skills              Search/install skills (CLI)
/skill <name>        Load a skill into session
/cron                Manage cron jobs (CLI)
/reload-mcp          Reload MCP servers
/plugins             List plugins (CLI)
```

### Info
```
/help                Show commands
/commands [page]     Browse all commands (gateway)
/usage               Token usage
/insights [days]     Usage analytics
/status              Session info (gateway)
/profile             Active profile info
```

### Exit
```
/quit (/exit, /q)    Exit CLI
```

---

## Key Paths & Config

```
~/.a9n/config.yaml       Main configuration
~/.a9n/.env              API keys and secrets
~/.a9n/skills/           Installed skills
~/.a9n/sessions/         Session transcripts
~/.a9n/logs/             Gateway and error logs
~/.a9n/auth.json         OAuth tokens and credential pools
~/.a9n/a9n-agent/     Source code (if git-installed)
```

Profiles use `~/.a9n/profiles/<name>/` with the same layout.

### Config Sections

Edit with `a9n config edit` or `a9n config set section.key value`.

| Section | Key options |
|---------|-------------|
| `model` | `default`, `provider`, `base_url`, `api_key`, `context_length` |
| `agent` | `max_turns` (90), `tool_use_enforcement` |
| `terminal` | `backend` (local/docker/ssh/modal), `cwd`, `timeout` (180) |
| `compression` | `enabled`, `threshold` (0.50), `target_ratio` (0.20) |
| `display` | `skin`, `tool_progress`, `show_reasoning`, `show_cost` |
| `stt` | `enabled`, `provider` (local/groq/openai) |
| `tts` | `provider` (edge/elevenlabs/openai/kokoro/fish) |
| `memory` | `memory_enabled`, `user_profile_enabled`, `provider` |
| `security` | `tirith_enabled`, `website_blocklist` |
| `delegation` | `model`, `provider`, `max_iterations` (50) |
| `smart_model_routing` | `enabled`, `cheap_model` |
| `checkpoints` | `enabled`, `max_snapshots` (50) |

Full config reference: https:///docs/user-guide/configuration

### Providers

18 providers supported. Set via `a9n model` or `a9n setup`.

| Provider | Auth | Key env var |
|----------|------|-------------|
| OpenRouter | API key | `OPENROUTER_API_KEY` |
| Anthropic | API key | `ANTHROPIC_API_KEY` |
| Nous Portal | OAuth | `a9n login --provider nous` |
| OpenAI Codex | OAuth | `a9n login --provider openai-codex` |
| GitHub Copilot | Token | `COPILOT_GITHUB_TOKEN` |
| DeepSeek | API key | `DEEPSEEK_API_KEY` |
| Hugging Face | Token | `HF_TOKEN` |
| Z.AI / GLM | API key | `GLM_API_KEY` |
| MiniMax | API key | `MINIMAX_API_KEY` |
| Kimi / Moonshot | API key | `KIMI_API_KEY` |
| Alibaba / DashScope | API key | `DASHSCOPE_API_KEY` |
| Kilo Code | API key | `KILOCODE_API_KEY` |
| Custom endpoint | Config | `model.base_url` + `model.api_key` in config.yaml |

Plus: AI Gateway, OpenCode Zen, OpenCode Go, MiniMax CN, GitHub Copilot ACP.

Full provider docs: https:///docs/integrations/providers

### Toolsets

Enable/disable via `a9n tools` (interactive) or `a9n tools enable/disable NAME`.

| Toolset | What it provides |
|---------|-----------------|
| `web` | Web search and content extraction |
| `browser` | Browser automation (Browserbase, Camofox, or local Chromium) |
| `terminal` | Shell commands and process management |
| `file` | File read/write/search/patch |
| `code_execution` | Sandboxed Python execution |
| `vision` | Image analysis |
| `image_gen` | AI image generation |
| `tts` | Text-to-speech |
| `skills` | Skill browsing and management |
| `memory` | Persistent cross-session memory |
| `session_search` | Search past conversations |
| `delegation` | Subagent task delegation |
| `cronjob` | Scheduled task management |
| `clarify` | Ask user clarifying questions |
| `moa` | Mixture of Agents (off by default) |
| `homeassistant` | Smart home control (off by default) |

Tool changes take effect on `/reset` (new session). They do NOT apply mid-conversation to preserve prompt caching.

---

## Voice & Transcription

### STT (Voice → Text)

Voice messages from messaging platforms are auto-transcribed.

Provider priority (auto-detected):
1. **Local faster-whisper** — free, no API key: `pip install faster-whisper`
2. **Groq Whisper** — free tier: set `GROQ_API_KEY`
3. **OpenAI Whisper** — paid: set `VOICE_TOOLS_OPENAI_KEY`

Config:
```yaml
stt:
  enabled: true
  provider: local        # local, groq, openai
  local:
    model: base          # tiny, base, small, medium, large-v3
```

### TTS (Text → Voice)

| Provider | Env var | Free? |
|----------|---------|-------|
| Edge TTS | None | Yes (default) |
| ElevenLabs | `ELEVENLABS_API_KEY` | Free tier |
| OpenAI | `VOICE_TOOLS_OPENAI_KEY` | Paid |
| Kokoro (local) | None | Free |
| Fish Audio | `FISH_AUDIO_API_KEY` | Free tier |

Voice commands: `/voice on` (voice-to-voice), `/voice tts` (always voice), `/voice off`.

---

## Spawning Additional A9N Instances

Run additional A9N processes as fully independent subprocesses — separate sessions, tools, and environments.

### When to Use This vs delegate_task

| | `delegate_task` | Spawning `a9n` process |
|-|-----------------|--------------------------|
| Isolation | Separate conversation, shared process | Fully independent process |
| Duration | Minutes (bounded by parent loop) | Hours/days |
| Tool access | Subset of parent's tools | Full tool access |
| Interactive | No | Yes (PTY mode) |
| Use case | Quick parallel subtasks | Long autonomous missions |

### One-Shot Mode

```
terminal(command="a9n chat -q 'Research GRPO papers and write summary to ~/research/grpo.md'", timeout=300)

# Background for long tasks:
terminal(command="a9n chat -q 'Set up CI/CD for ~/myapp'", background=true)
```

### Interactive PTY Mode (via tmux)

A9N uses prompt_toolkit, which requires a real terminal. Use tmux for interactive spawning:

```
# Start
terminal(command="tmux new-session -d -s agent1 -x 120 -y 40 'a9n'", timeout=10)

# Wait for startup, then send a message
terminal(command="sleep 8 && tmux send-keys -t agent1 'Build a FastAPI auth service' Enter", timeout=15)

# Read output
terminal(command="sleep 20 && tmux capture-pane -t agent1 -p", timeout=5)

# Send follow-up
terminal(command="tmux send-keys -t agent1 'Add rate limiting middleware' Enter", timeout=5)

# Exit
terminal(command="tmux send-keys -t agent1 '/exit' Enter && sleep 2 && tmux kill-session -t agent1", timeout=10)
```

### Multi-Agent Coordination

```
# Agent A: backend
terminal(command="tmux new-session -d -s backend -x 120 -y 40 'a9n -w'", timeout=10)
terminal(command="sleep 8 && tmux send-keys -t backend 'Build REST API for user management' Enter", timeout=15)

# Agent B: frontend
terminal(command="tmux new-session -d -s frontend -x 120 -y 40 'a9n -w'", timeout=10)
terminal(command="sleep 8 && tmux send-keys -t frontend 'Build React dashboard for user management' Enter", timeout=15)

# Check progress, relay context between them
terminal(command="tmux capture-pane -t backend -p | tail -30", timeout=5)
terminal(command="tmux send-keys -t frontend 'Here is the API schema from the backend agent: ...' Enter", timeout=5)
```

### Session Resume

```
# Resume most recent session
terminal(command="tmux new-session -d -s resumed 'a9n --continue'", timeout=10)

# Resume specific session
terminal(command="tmux new-session -d -s resumed 'a9n --resume 20260225_143052_a1b2c3'", timeout=10)
```

### Tips

- **Prefer `delegate_task` for quick subtasks** — less overhead than spawning a full process
- **Use `-w` (worktree mode)** when spawning agents that edit code — prevents git conflicts
- **Set timeouts** for one-shot mode — complex tasks can take 5-10 minutes
- **Use `a9n chat -q` for fire-and-forget** — no PTY needed
- **Use tmux for interactive sessions** — raw PTY mode has `\r` vs `\n` issues with prompt_toolkit
- **For scheduled tasks**, use the `cronjob` tool instead of spawning — handles delivery and retry

---

## Troubleshooting

### Voice not working
1. Check `stt.enabled: true` in config.yaml
2. Verify provider: `pip install faster-whisper` or set API key
3. Restart gateway: `/restart`

### Tool not available
1. `a9n tools` — check if toolset is enabled for your platform
2. Some tools need env vars (check `.env`)
3. `/reset` after enabling tools

### Model/provider issues
1. `a9n doctor` — check config and dependencies
2. `a9n login` — re-authenticate OAuth providers
3. Check `.env` has the right API key

### Changes not taking effect
- **Tools/skills:** `/reset` starts a new session with updated toolset
- **Config changes:** `/restart` reloads gateway config
- **Code changes:** Restart the CLI or gateway process

### Skills not showing
1. `a9n skills list` — verify installed
2. `a9n skills config` — check platform enablement
3. Load explicitly: `/skill name` or `a9n -s name`

### Gateway issues
Check logs first:
```bash
grep -i "failed to send\|error" ~/.a9n/logs/gateway.log | tail -20
```

---

## Where to Find Things

| Looking for... | Location |
|----------------|----------|
| Config options | `a9n config edit` or [Configuration docs](https:///docs/user-guide/configuration) |
| Available tools | `a9n tools list` or [Tools reference](https:///docs/reference/tools-reference) |
| Slash commands | `/help` in session or [Slash commands reference](https:///docs/reference/slash-commands) |
| Skills catalog | `a9n skills browse` or [Skills catalog](https:///docs/reference/skills-catalog) |
| Provider setup | `a9n model` or [Providers guide](https:///docs/integrations/providers) |
| Platform setup | `a9n gateway setup` or [Messaging docs](https:///docs/user-guide/messaging/) |
| MCP servers | `a9n mcp list` or [MCP guide](https:///docs/user-guide/features/mcp) |
| Profiles | `a9n profile list` or [Profiles docs](https:///docs/user-guide/profiles) |
| Cron jobs | `a9n cron list` or [Cron docs](https:///docs/user-guide/features/cron) |
| Memory | `a9n memory status` or [Memory docs](https:///docs/user-guide/features/memory) |
| Env variables | `a9n config env-path` or [Env vars reference](https:///docs/reference/environment-variables) |
| CLI commands | `a9n --help` or [CLI reference](https:///docs/reference/cli-commands) |
| Gateway logs | `~/.a9n/logs/gateway.log` |
| Session files | `~/.a9n/sessions/` or `a9n sessions browse` |
| Source code | `~/.a9n/a9n-agent/` |

---

## Contributor Quick Reference

For occasional contributors and PR authors. Full developer docs: https:///docs/developer-guide/

### Project Layout

```
a9n-agent/
├── run_agent.py          # AIAgent — core conversation loop
├── model_tools.py        # Tool discovery and dispatch
├── toolsets.py           # Toolset definitions
├── cli.py                # Interactive CLI (A9NCLI)
├── a9n_state.py       # SQLite session store
├── agent/                # Prompt builder, compression, display, adapters
├── a9n_cli/           # CLI subcommands, config, setup, commands
│   ├── commands.py       # Slash command registry (CommandDef)
│   ├── config.py         # DEFAULT_CONFIG, env var definitions
│   └── main.py           # CLI entry point and argparse
├── tools/                # One file per tool
│   └── registry.py       # Central tool registry
├── gateway/              # Messaging gateway
│   └── platforms/        # Platform adapters (telegram, discord, etc.)
├── cron/                 # Job scheduler
├── tests/                # ~3000 pytest tests
└── website/              # Docusaurus docs site
```

Config: `~/.a9n/config.yaml` (settings), `~/.a9n/.env` (API keys).

### Adding a Tool (3 files)

**1. Create `tools/your_tool.py`:**
```python
import json, os
from tools.registry import registry

def check_requirements() -> bool:
    return bool(os.getenv("EXAMPLE_API_KEY"))

def example_tool(param: str, task_id: str = None) -> str:
    return json.dumps({"success": True, "data": "..."})

registry.register(
    name="example_tool",
    toolset="example",
    schema={"name": "example_tool", "description": "...", "parameters": {...}},
    handler=lambda args, **kw: example_tool(
        param=args.get("param", ""), task_id=kw.get("task_id")),
    check_fn=check_requirements,
    requires_env=["EXAMPLE_API_KEY"],
)
```

**2. Add import** in `model_tools.py` → `_discover_tools()` list.

**3. Add to `toolsets.py`** → `_A9N_CORE_TOOLS` list.

All handlers must return JSON strings. Use `get_a9n_home()` for paths, never hardcode `~/.a9n`.

### Adding a Slash Command

1. Add `CommandDef` to `COMMAND_REGISTRY` in `a9n_cli/commands.py`
2. Add handler in `cli.py` → `process_command()`
3. (Optional) Add gateway handler in `gateway/run.py`

All consumers (help text, autocomplete, Telegram menu, Slack mapping) derive from the central registry automatically.

### Agent Loop (High Level)

```
run_conversation():
  1. Build system prompt
  2. Loop while iterations < max:
     a. Call LLM (OpenAI-format messages + tool schemas)
     b. If tool_calls → dispatch each via handle_function_call() → append results → continue
     c. If text response → return
  3. Context compression triggers automatically near token limit
```

### Testing

```bash
source venv/bin/activate  # or .venv/bin/activate
python -m pytest tests/ -o 'addopts=' -q   # Full suite
python -m pytest tests/tools/ -q            # Specific area
```

- Tests auto-redirect `A9N_HOME` to temp dirs — never touch real `~/.a9n/`
- Run full suite before pushing any change
- Use `-o 'addopts='` to clear any baked-in pytest flags

### Commit Conventions

```
type: concise subject line

Optional body.
```

Types: `fix:`, `feat:`, `refactor:`, `docs:`, `chore:`

### Key Rules

- **Never break prompt caching** — don't change context, tools, or system prompt mid-conversation
- **Message role alternation** — never two assistant or two user messages in a row
- Use `get_a9n_home()` from `a9n_constants` for all paths (profile-safe)
- Config values go in `config.yaml`, secrets go in `.env`
- New tools need a `check_fn` so they only appear when requirements are met
