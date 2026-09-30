---
name: task-breakdown
description: Breaks down feature requests and user stories into implementable sub-tasks with T-shirt size estimates, dependencies, and acceptance criteria. Use this skill whenever the user asks to "break down", "decompose", "split up", or "plan implementation" for a feature, user story, epic, or piece of work — even if they just paste a feature description and ask "what needs to be done" or "how would we build this". Produces a structured task table plus a risk assessment for work touching shared code paths or requiring database migrations.
---

# Task Breakdown

Decompose a feature request or user story into concrete, implementable sub-tasks that a developer can pick up one at a time.

## Workflow

Follow these steps in order. Do not skip the risk assessment — it is cheap and catches the tasks that blow up estimates.

### 1. Understand the request

- Read the feature request carefully. If it references existing code, inspect the relevant files before decomposing — a breakdown that ignores the actual codebase produces imaginary tasks.
- Identify the **components** the feature touches. In this workspace the standard components are: `backend` (.NET 8 Web API), `frontend` (React 18 + TypeScript), `tests`, `database`, `docs`, and `infra`. Use whatever components fit the codebase at hand.
- If the request is ambiguous enough to change the breakdown materially (e.g., unclear whether auth is required), ask up to 3 clarifying questions before proceeding. Do not ask about things you can reasonably assume and note as assumptions.

### 2. Decompose into sub-tasks

Split the feature into tasks that are:

- **Independently implementable** — each task should be mergeable on its own (possibly behind a flag).
- **Vertically sliced where practical** — prefer "add Employee endpoint (DTO → service → controller → tests)" over horizontal layers like "write all DTOs".
- **Scoped to one component** — a task that spans backend and frontend should usually be two tasks.

Follow the conventions of the repository. In this workspace, backend tasks must respect the organization standards in `.github/copilot-instructions.md` (envelope pattern, repository/service pattern, xUnit + Moq tests with AAA comments, etc.).

### 3. Estimate T-shirt sizes

Assign each task exactly one size using this rubric:

| Size | Effort guide | Typical example |
|------|--------------|-----------------|
| XS   | < 2 hours, trivial, one file or config change | Add a query-string filter to an existing endpoint |
| S    | Half a day, 1–3 files, no new patterns | Add a field to a DTO and plumb it through |
| M    | 1–2 days, a new vertical slice with tests | New CRUD endpoint following existing patterns |
| L    | 3–5 days, multiple slices or a new pattern | New aggregate with repository, service, controller, and frontend page |
| XL   | > 1 week, architectural or cross-cutting | Auth system, migration framework, realtime sync |

Rules:

- If a task comes out XL, **split it** — XL tasks in the final table are a smell. Only leave one if it genuinely cannot be decomposed, and explain why.
- Estimate assuming a developer familiar with the codebase, following existing patterns.
- Do not pad. An honest S beats a padded M.

### 4. Map dependencies

For each task, list the tasks that must complete first (by task number). Keep dependencies minimal — if B only needs the DTO from A, say so rather than marking a hard dependency. Note opportunities for parallel work.

### 5. Write acceptance criteria

Each task gets 1–3 acceptance criteria that are **objectively verifiable** — a reviewer should be able to check them without judgment calls. Prefer observable behavior ("GET /v1/employees returns envelope-wrapped list with pagination") over implementation statements ("code is clean").

### 6. Risk assessment

After the table, assess every task that either:

- **Touches a shared code path** — code used by features other than this one (e.g., the response envelope DTOs, middleware, shared UI components, the API client), or
- **Requires a database migration** — schema changes, data backfills, index changes.

For each flagged task, output:

```markdown
**Task <N> — <title>**
- **Risk:** <what could go wrong — regression in other consumers, irreversible migration, locking on large tables, etc.>
- **Mitigation:** <concrete step — feature flag, expand-and-contract migration, backward-compatible API change, extra regression test coverage, etc.>
```

If no tasks qualify, state that explicitly — never silently omit this section.

## Output format

ALWAYS produce output in this structure:

```markdown
# Task Breakdown: <feature name>

**Assumptions:** <bullet list of anything assumed, or "None">

| # | Task | Component | Size | Depends on | Acceptance criteria |
|---|------|-----------|------|------------|---------------------|
| 1 | ... | backend | M | — | ... |
| 2 | ... | frontend | S | 1 | ... |

**Suggested order:** <1-2 sentences on sequencing and what can run in parallel>

## Risk assessment

<flagged tasks as above, or "No tasks touch shared code paths or require database migrations.">
```

Keep the table cells terse — a task title is a phrase, not a paragraph. Acceptance criteria in the table can be abbreviated; add detail below the table only when a criterion needs it.

## Example

**Input:** "Break down: employees should be able to upload a profile photo."

**Output (abridged):**

| # | Task | Component | Size | Depends on | Acceptance criteria |
|---|------|-----------|------|------------|---------------------|
| 1 | Add `photoUrl` column migration | database | XS | — | Migration applies and rolls back cleanly |
| 2 | POST /v1/employees/{id}/photo endpoint (upload + validation) | backend | M | 1 | Accepts jpeg/png ≤ 5 MB; rejects other types with ORG-VAL-001; returns envelope |
| 3 | Photo upload UI on employee page | frontend | S | 2 | User can select and upload a photo; error shown on rejection |

**Risk assessment**

**Task 1 — Add `photoUrl` column migration**
- **Risk:** ALTER on a large Employees table can lock writes.
- **Mitigation:** Nullable column add (metadata-only in most engines); verify against production-sized data before release.
