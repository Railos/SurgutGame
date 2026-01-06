<INSTRUCTIONS>
# Unity C# Shooter Project - Agent Rules

## Scope
- Applies to all code, assets, and documentation changes in this repository.
- Prioritize gameplay stability, frame-time safety, and predictable behavior.

## Feature Docs Requirement (Mandatory)
- For each new feature and each heavy change, you must write a Markdown doc in `Docs/`.
- The doc must explain what changed and why, include usage or setup steps if applicable, and list any risks or TODOs.
- Name format: `Docs/FEATURE-<short-name>.md` or `Docs/CHANGE-<short-name>.md`.
- Include a brief changelog line at the top: `Date: YYYY-MM-DD`.
- If no doc exists for a change, stop and ask before continuing.
- Provide simple examples for user to understand usage of certain features and changes.
- Try to fully explain the feature, explain every new method and function and class and so on, so no questions will be left.

## Unity / C# Guidelines
- Prefer simple, deterministic logic over complex patterns.
- Avoid allocations in hot paths (`Update`, `FixedUpdate`, `LateUpdate`).
- Cache component references; do not use `Find`/`GetComponent` repeatedly in hot paths.
- Use `SerializeField` over `public` fields unless a type truly needs to be public.
- Keep script responsibilities tight; avoid "god" components.
- When adding new assets, ensure import settings are appropriate for runtime.

## Project Hygiene
- Keep `Docs/` updated with feature/change docs.
- Do not edit files under `Library/`, `Temp/`, or `Logs/`.
- Respect existing project structure and naming conventions.
</INSTRUCTIONS>
