# dev-flow

> SDLC orchestration framework for GitHub Copilot — coordinating planning, coding, testing, and review through specialized agents and skills.

## What's Included

### Agents

| Agent | File | Model | Purpose |
|-------|------|-------|---------|
| **DevFlow** | `agents/dev-flow.agent.md` | Auto | Orchestrator — coordinates the full SDLC pipeline |
| **Planner** | `agents/planner.agent.md` | Auto | Breaks features into implementable tasks |
| **Code Reviewer** | `agents/reviewer.agent.md` | Claude Sonnet 4.6 | Reviews code for bugs, security, and quality |
| **Tester** | `agents/tester.agent.md` | Claude Haiku 4.5 | Generates and validates tests |

### Skills

| Skill | Directory | Purpose |
|-------|-----------|---------|
| **task-breakdown** | `skills/task-breakdown/` | Decomposes feature requests into T-shirt sized tasks with acceptance criteria |
| **naming-checker** | `skills/naming-checker/` | Validates file and variable naming conventions with automated scanning |
| **documentation-writer** | `skills/documentation-writer/` | Generates project documentation (from [github/awesome-copilot](https://github.com/github/awesome-copilot)) |

## Installation

```bash
# Install via plugin
/plugin install dev-flow@plugin-marketplace
```

## Usage

### Run the full SDLC pipeline

Select the **DevFlow** agent from the agent picker, then describe your feature:

```
Build a user authentication feature with login, logout, and JWT token refresh
```

DevFlow will guide you through each stage — planning, implementation, testing, and review — pausing for your approval before advancing.

### Use individual agents

| Goal | How |
|------|-----|
| Plan a feature | Select **Planner** agent → describe the feature |
| Review code changes | Select **Code Reviewer** agent → ask it to review recent changes |
| Generate tests | Select **Tester** agent → ask it to write tests for a file or module |

### Use individual skills

Skills activate automatically when your prompt matches, or invoke explicitly:

```
Break down this feature: "Add notification preferences to user settings"
Check naming conventions in the src directory
Write documentation for the authentication module
```

## Model Strategy

Each agent is right-sized for its task:

| Agent | Model | Rationale |
|-------|-------|-----------|
| DevFlow | Auto | Intent-based routing adapts to each orchestration request; paid plans receive a 10% model-cost discount |
| Planner | Auto | Intent-based routing adapts to each planning request; paid plans receive a 10% model-cost discount |
| Code Reviewer | Claude Sonnet 4.6 | Balanced task — quality matters |
| Tester | Claude Haiku 4.5 | Focused task — speed and generally lower token rates |

## License

MIT
