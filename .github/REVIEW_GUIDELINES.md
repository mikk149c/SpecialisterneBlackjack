# Review guidelines (used by the AI reviewer sub-agent)

The reviewer is an independent agent: it did not write the code under review.
It reads the PR diff, the linked issue and these guidelines, and reports findings
as a PR comment. It never edits the branch.

## Severity

- **blocker** - must be fixed before merge (broken behaviour, rule violation, scope breach, failing gate).
- **should-fix** - clear violation of a guideline below; fix unless you can justify it in the PR.
- **nit** - optional polish.

## 1. Scope and correctness

- Every acceptance-criteria box in the linked issue is met, or explained.
- No changes outside the issue's scope (no drive-by refactors or unrelated fixes).
- Behaviour of the console app is unchanged unless the issue says otherwise.
- Edge cases: empty hand, 8+ cards, balance 0, bet > balance, deck low.

## 2. SOLID

- **Single responsibility:** each class/method has one reason to change. Flag classes
  that mix rendering, rules and I/O (e.g. game rules inside code-behind).
- **Open/closed:** new behaviour is added by extension, not by growing `switch`/`if`
  chains on type or enum in several places.
- **Liskov substitution:** implementations honour the interface contract; no
  `NotImplementedException`, no strengthened preconditions.
- **Interface segregation:** consumers depend on the small interface they need,
  not on a wide one.
- **Dependency inversion:** high-level code depends on abstractions (`IBlackjackGame`,
  `IHand`, `IDeck`, `IGameFactory`); concrete engine classes are only created in
  `GameFactory`. The engine must not reference UI types.

## 3. Clean Code

- **Names** reveal intent; no abbreviations, no misleading names, consistent vocabulary.
- **Functions** are small, do one thing, one level of abstraction, few parameters
  (flag arguments are a smell).
- **No duplication** of logic or magic numbers/strings; named constants instead.
- **Comments** explain *why*, never restate *what*; no commented-out code, no stale TODOs.
- **Error handling** uses exceptions, not return codes; no swallowed exceptions
  without a stated reason; no `catch (Exception)` that hides bugs.
- **No dead code**, unused usings, unused parameters.
- **Formatting** matches the surrounding code.
- **Law of Demeter / tell-don't-ask**: avoid long call chains into other objects' internals.

## 4. Repo-specific rules

- `Blackjack.Game` has no UI, console or WinUI references.
- Game/presentation decisions (status text, enable rules, labels, refill rule) live in
  `Blackjack.Presentation`, not duplicated in a front end.
- Front ends contain drawing and input wiring only.
- New logic in `Blackjack.Game` / `Blackjack.Presentation` comes with tests.
  The CI coverage gate is 95% line coverage on those two projects.
- No new NuGet packages unless the issue names them.

## 5. Output format

Post one PR comment, structured as:

```
## AI review - <PR title>
Reviewed commit: <sha>
Verdict: APPROVE | CHANGES REQUESTED

### Blockers
- <file>:<line> - <finding> (<principle>)
### Should-fix
- ...
### Nits
- ...
### Checked and fine
- <one line per area verified: scope, SOLID, clean code, tests, repo rules>
```

`APPROVE` means zero blockers and zero unaddressed should-fix items.
