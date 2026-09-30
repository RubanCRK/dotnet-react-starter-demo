---
name: naming-checker
description: Validates file and variable naming conventions against project standards. Use this skill whenever the user asks to "check naming", "validate naming conventions", "find naming violations", "lint names", or review whether files, variables, constants, or types follow the project's naming rules — even if they just say "does this follow our conventions". Scans for camelCase files that should be kebab-case, flags `is`/`has`-prefixed booleans, non-PascalCase types, and non-SCREAMING_CASE constants, and reports findings as a severity table with suggested fixes.
allowed-tools: shell
---

# Naming Checker

Validate file and identifier naming against project conventions. This repo's authoritative rules live in `.github/copilot-instructions.md` — when a rule here and a rule there conflict, the instructions file wins.

## Conventions enforced

### Files

| Scope | Convention | Example |
|-------|------------|---------|
| Markdown/config/docs files | kebab-case | `task-breakdown.md`, `build-instructions.md` |
| Agent/skill metadata | kebab-case | `naming-checker.agent.md` |
| C# files | PascalCase, match contained type | `EmployeeService.cs` |
| React component files | PascalCase, match component | `ErrorMessage.tsx` |
| Frontend non-component modules | camelCase or kebab-case per folder precedent | `api.ts`, `types.ts` |

**Exemptions** — never flag these:
- Files containing `.agent.` (e.g., `api-builder.agent.md`)
- Files containing `.test.` (e.g., `Foo.test.ts`)
- Well-known files: `README.md`, `LICENSE`, `CHANGELOG.md`, `Dockerfile`, `.gitignore`, `package.json`, `package-lock.json`, `tsconfig*.json`, `vite.config.ts`, and similar tooling-mandated names
- `.github/skills/*/SKILL.md` (uppercase is the skills convention)

### Identifiers (C#)

| Kind | Convention | Flag pattern |
|------|------------|--------------|
| Types, methods, properties | PascalCase | `^[a-z]` on a class/interface/method name |
| Parameters, locals | camelCase | `^[A-Z]` on a parameter or local |
| Private fields | `_camelCase` | private field without leading `_` |
| Constants | PascalCase (C# convention, per Microsoft) | `const` in SCREAMING_CASE |
| Boolean properties | `Indicator` suffix, NO `is`/`has` prefix | `isActive`, `hasPaid` → `activeIndicator`, `paidIndicator` |
| Date properties | `Date` suffix | `created`, `updated` → `createdDate`, `updatedDate` |
| Async methods | `Async` suffix + `CancellationToken` last param | `GetById(...)` async without suffix |

### Identifiers (TypeScript)

| Kind | Convention | Flag pattern |
|------|------------|--------------|
| Interfaces, types, components | PascalCase | `interface employeeData` |
| Variables, functions | camelCase | `const EmployeeList = ...` (non-component) |
| Constants (module scope) | SCREAMING_SNAKE_CASE or camelCase per folder precedent | mixed styles in one file |
| Booleans | no `is`/`has` prefix when mirroring API DTOs | `activeIndicator`, not `isActive`, in DTO types |

## Workflow

### 1. Run the file-name scan

Run the bundled script from the repo root:

```bash
bash .github/skills/naming-checker/scripts/check-file-names.sh .
```

It lists files whose names use camelCase or mixed case where kebab-case is expected, excluding `.agent.md` and `.test.` files and the well-known names above. On Windows without bash, run the equivalent PowerShell one-liner the script prints with `--pshint`, or fall back to a manual `Get-ChildItem -Recurse` review.

### 2. Scan identifiers with search

Use text search (not guesswork) for the identifier rules that matter most in this repo:

- ``\bis[A-Z]\w*\s*[{;=]`` and `\bhas[A-Z]\w*\s*[{;=]` — `is`/`has`-prefixed booleans (forbidden by org standards)
- `public\s+(?!.*\bAsync\b)\w+\s+\w+\([^)]*\)\s*$` combined with `async` in the body — async methods missing the `Async` suffix
- `interface\s+[a-z]` / `class\s+[a-z]` — non-PascalCase types
- `const\s+[A-Z_]+` in C# files — SCREAMING_CASE constants (wrong for C#)

Read each hit in context before reporting it — regex hits include false positives (comments, strings, generated code).

### 3. Report findings

ALWAYS output a table in this exact shape, sorted by severity (High → Low):

```markdown
## Naming findings

| Severity | Location | Violation | Suggested fix |
|----------|----------|-----------|---------------|
| High | backend/src/Api/Services/FooService.cs#L12 | `isActive` boolean property | Rename to `activeIndicator` |
| Medium | docs/BuildGuide.md | PascalCase doc file | Rename to `build-guide.md` |
| Low | frontend/src/lib/api.ts#L8 | Mixed constant style | Pick one style per folder precedent |
```

Severity guide:

- **High** — violates an explicit org rule (the "Forbidden Actions" list in `.github/copilot-instructions.md`) or breaks a build/tooling assumption
- **Medium** — violates a stated convention but compiles/runs fine
- **Low** — inconsistency or style drift, no rule on record

Close with a one-line summary: counts by severity and whether any High findings block the change. If there are no findings, say so explicitly in the table's place — never return an empty table silently.

## Notes

- Only report; do not rename anything unless the user asks. Renames can break imports, links, and tooling config.
- Prefer folder precedent over global rules when conventions conflict within the repo (e.g., an existing folder of PascalCase docs).
