---
name: documentation-writer
description: Generates concise project documentation for code changes, modules, and APIs. Use this skill whenever the user asks to "write documentation", "document this module", "update the docs", "generate docs", or needs a feature/change summary written up — including as the docs step of a larger development workflow. Produces markdown documentation grounded in the actual code, not generic boilerplate.
---

# Documentation Writer

Generate documentation for code by reading the code first. Documentation that drifts from implementation is worse than none — every claim in the output must be verifiable in the source.

## Workflow

### 1. Determine scope

Identify what to document:
- A **change set** (files modified in the current branch/diff) — document what changed, why, and how to use it
- A **module or feature** (a directory, service, or component) — document purpose, public API, and usage
- The **whole project** — README-style: what it is, how to build/run/test, structure

If scope is ambiguous, ask. Default to the change set when invoked from a development workflow.

### 2. Read before writing

- Read the actual source files in scope — public signatures, route definitions, exported types, config
- Check for existing docs that need updating rather than duplicating (README, `docs/` folder, XML doc comments)
- Note the project's documentation conventions and follow them

### 3. Write

Match the doc type to the scope:

**Feature/change doc** (e.g., `docs/<feature-name>.md`):
```markdown
# <Feature name>

## Overview
<1-2 sentences: what it does and why it exists>

## API / Usage
<endpoints, signatures, or component usage with one minimal working example>

## Configuration
<settings, env vars, defaults — or "None">

## Limitations / Notes
<known constraints, deferred work, security notes>
```

**Rules:**
- Every endpoint, type, and option mentioned must exist in the code — no aspirational docs
- One working example beats three paragraphs of description
- Keep it under ~100 lines; link to code for detail instead of reproducing it
- Use the naming and terminology already in the codebase (e.g., if the API uses `activeIndicator`, the doc uses `activeIndicator`)

### 4. Report

State what was written or updated (with paths) and anything you intentionally left out (e.g., "no config section — feature has no settings").
