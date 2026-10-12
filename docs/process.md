# Delivery and approval process

How work on the WinUI front end (tracking issue #3) is delivered. It applies to
every PR into `feature/winui`.

## Branches

```
main                     protected by the owner only; changes arrive via one final PR
 └── feature/winui       integration branch
      └── winui/<n>-<slug>   one short-lived branch per issue (n = issue number)
```

- Work happens on `winui/<n>-<slug>`, cut from the latest `feature/winui`.
- Each issue gets one PR into `feature/winui`, squash-merged.
- `feature/winui` goes to `main` through **one** PR that the owner reviews and merges.
- Nobody (human or agent) pushes to `main` directly, and the orchestrator never merges into it.

## Roles

| Role | Who | Does |
|------|-----|------|
| Owner | repository owner | Approves and merges `feature/winui` -> `main` |
| Orchestrator | main Claude session | Plans waves, spawns agents, runs gates, merges PRs into `feature/winui`, keeps issues updated |
| Author agent | sub-agent per issue | Implements one issue on its own branch and opens the PR |
| Reviewer agent | separate sub-agent per PR | Reviews the diff against `.github/REVIEW_GUIDELINES.md`; never edits the branch |

The reviewer is always a different agent from the author.

## PR lifecycle and approval gates

1. **Ready:** issue is labelled `status: ready` and its dependencies are merged.
2. **In progress:** author agent starts; issue gets `status: in-progress` and a start comment.
3. **PR opened** into `feature/winui` using the PR template; issue gets `status: in-review`
   and a comment linking the PR.
4. **Gate A - CI:** workflow `CI` check `build-test-coverage` must pass:
   build, all tests, and **>= 95% line coverage** on `Blackjack.Game` and
   `Blackjack.Presentation`.
5. **Gate B - AI review:** a reviewer agent posts its verdict comment. Merge requires
   `APPROVE`. Findings are either fixed (author agent pushes to the same branch, reviewer
   re-reviews the new commit) or answered in the PR with a reason.
6. **Gate C - acceptance criteria:** every box in the issue is ticked or explained;
   the diff stays within the issue's scope.
7. **Merge:** the orchestrator squash-merges into `feature/winui` only when A, B and C
   hold. The PR description's "Review trail" records the reviewed commit, verdict and
   merge decision.
8. **Close-out:** the orchestrator comments on the issue with what was delivered and
   closes it (GitHub only auto-closes issues when a PR merges into the default branch, so
   this is manual), sets `status: done`, and removes the branch.

A failing gate sends the PR back to its author agent. If it fails twice for the same
reason, the issue gets `status: blocked` and a comment explaining what is needed from the owner.

## Issue tracking

- Status is carried by labels: `status: ready`, `status: in-progress`, `status: in-review`,
  `status: blocked`, `status: done`. Exactly one is set at a time.
- Each state change gets a short issue comment: what happened, link to the PR/commit,
  and any decision made.
- Agents document non-obvious decisions and deviations in the issue as they work, not only
  at the end.

## Final PR to `main`

Opened by the orchestrator, reviewed and merged by the owner. It contains:
- list of merged PRs with issue numbers (`Closes #n` for each, so issues close on merge to main),
- final coverage numbers,
- "Needs human eyes" items collected from the PRs (visual checks of the WinUI screens),
- the CI and review setup (`.github/`, `docs/process.md`) introduced by this branch.
