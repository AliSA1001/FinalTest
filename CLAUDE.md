# CLAUDE.md — Act to Wish

Project memory for Claude Code. Read this before working on the project.

## Project

**Act to Wish** is a theatrical third-person action game made in Unity. You play a regular person who strikes a deal with a clown-wizard (the **Impresario**) who runs a mysterious theatre: choose a wish, sign his Joker card, and perform three plays well enough to earn it. Each play is a third-person combat stage, scored by a **parry-driven rating system**, performed for an audience whose cheers and boos *are* the score.

Final-year student project. **6 people** (3 developers, 3 artists), **~1 month**, deliberately low scope, low-poly stylized.

## Tech stack

- Unity — **TODO: set exact LTS version here** (everyone uses the same one)
- C#
- Unity Input System (the new package)
- Target: PC, keyboard + mouse

## Working with Claude Code here

- Claude Code **writes and edits C# scripts only**. It cannot drive the Unity Editor GUI — a human wires scene references, prefabs, the Input System asset, Animator controllers, and serialized fields in the Inspector.
- **Plan before implementing.** Explore the relevant scripts, propose an approach, and wait for approval before editing (use plan mode).
- Keep changes small; commit often with clear messages.
- Don't add third-party packages or new dependencies without team sign-off.
- Match the conventions below and the patterns already in the codebase.

## Architecture — the one rule

**Systems communicate through events, not direct references.**

A system *raises* an event (`ParryLanded`, `EnemyKilled`, `PlayerDamaged`, `ActFinished`); other systems *subscribe*. Combat must never call Score directly — Score and Crowd listen for combat events. This keeps every system independent and testable in isolation, and is what lets the team build in parallel.

- Prefer C# events / a small event bus, or ScriptableObject event channels.
- Keep `MonoBehaviour`s thin; put logic in systems.
- Avoid singletons where an event will do. No `Find()` / `GetComponent` in `Update`.

## Systems & build order

Build bottom-up. Tier 0 has no dependencies — start there, in parallel.

| Tier | Build when | Systems |
|---|---|---|
| 0 — Foundations | start now, parallel | Input, HP, Score/Rating, Audio manager, UI manager, Interaction |
| 1 — Movement & presentation | on Tier 0 | Movement, Camera (dual), Animation (third-person) |
| 2 — Interactive layer | on the above | Pickup items, Collectables, Dialogue, Targeting/lock-on, Crowd reaction |
| 3 — Combat & enemies | integrative | Combat (modular parry), Enemy AI, Checkpoint/respawn |
| 4 — Orchestration | skeleton early, finish last | Act sequencer ("play script"), Game flow/state, Results + endings |

Two systems are easy to forget and costly if left late: **Crowd reaction** and the **Act sequencer**.

## Core gameplay the systems serve

- **Dual camera:** first-person in the dressing-room hub (choose wish, choose which act to perform); third-person during acts. The FP→3rd-person switch is the "stepping onto the stage" moment.
- **Scoring is a parry.** One parry system, re-shaped per act:
  - Act 1 (tutorial) — knight: directional sword parry; melee/ripostes also score.
  - Act 2 — sci-fi: ranged energy-bolt deflection (aim the reflection); radial pulse for swarms.
  - Act 3 — TBD (combine/escalate).
  - Perfect parry = big score + crowd cheer. Offense, kills, and collectables also score. A combo multiplier feeds crowd mood.
- **Soft lock-on** keeps third-person parries/reflections readable.
- **Crowd reaction:** the audience is visible in the theatre, then audio-only once a play begins. A crowd-mood value (0–1), driven by score events, drives the cheers/boos and the ambient bed. This is the scoring feedback and the game's identity.
- **Diegetic death:** no real death — die, reset to the last checkpoint, rating drops, the play continues until finished.
- **Three acts**, each yielding a star rating; the total gates **two endings** (win / lose) via a single threshold.

## Conventions

- C#: `PascalCase` for types/methods/properties, `_camelCase` for private fields, `camelCase` for locals/params.
- One system per folder: `Assets/Scripts/<System>/`.
- Scripts live in `Assets/`. Reference docs live in `/Docs` (outside `Assets/`, so Unity ignores them) — the full GDD is there.
- Serialize references via `[SerializeField]` and wire them in the Inspector (tell the human what to wire).
- Testing is in-editor Play mode; there's no CLI build step yet.

## Scope guardrails

- **Build the loop, not the content, first:** finish Act 1 (the knight) end-to-end — dressing room → step onto stage → parry/combat loop → review → return — before Acts 2 and 3.
- **Not in scope this month:** co-op, branching dialogue, full voice acting.
- **If behind:** cut Act 3 first (see the cut order in the GDD).

## Design docs

- Full design: `/Docs` (the GDD).
- The detailed narrative content of the endings is kept in the team's private notes, not in this repo.

## Current focus — edit this

- _Your name_ — working on _system(s)_.
