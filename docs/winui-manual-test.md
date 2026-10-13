# WinUI manual test checklist

A quick human check of the WinUI 3 front end (`src/Blackjack.WinUI`). The unit tests
cover the logic (`GameController`, text, layout); this list covers what only a person
can judge: how it looks, how it feels, real keyboard and mouse input, screen readers
and DPI scaling. It collects the "Needs human eyes" items of issues #4 to #12 and the
scenarios of issue #12 (tracking issue: #3).

Tick a box when the expected result is seen. Anything that does not match is a bug:
note the step, what you saw and a screenshot in the PR or issue.

## 0. Build and run

Windows 10 1809 (build 17763) or later and the .NET 10 SDK are needed. The Windows App
SDK runtime is bundled (the app is self-contained and unpackaged), so there is no
separate runtime or MSIX install; the Windows App SDK NuGet package is restored by the
build. Visual Studio is not required.

Pick one way to start the app. The output folder differs between them:

| Command | Where the exe ends up |
|---------|-----------------------|
| `dotnet build Blackjack.slnx -c Release` (whole solution) | `src\Blackjack.WinUI\bin\Release\net10.0-windows10.0.19041.0\win-x64\Blackjack.WinUI.exe` |
| `dotnet build src/Blackjack.WinUI -c Release -p:Platform=x64` (project only) | `src\Blackjack.WinUI\bin\x64\Release\net10.0-windows10.0.19041.0\win-x64\Blackjack.WinUI.exe` |
| `dotnet run --project src/Blackjack.WinUI -r win-x64` (Debug) | `src\Blackjack.WinUI\bin\x64\Debug\net10.0-windows10.0.19041.0\win-x64\Blackjack.WinUI.exe` |

Run the `.exe` from its own folder or by full path; the card art is read from the
`Assets\CardArt.txt` next to it. ARM64 builds are not supported on an x64 development
machine (XAML compiler errors); use x64.

Record the test run:

- Date / tester: 
- Commit (`git rev-parse --short HEAD`): 
- Windows version and display scaling used: 

## 1. First start

- [ ] A window titled **Blackjack** opens, about 1000 x 760 DIP, and the game is ready without any error.
- [ ] Header reads `Balance: 1000   Bet: 0   Last Round: -`, with a thin rule below it.
- [ ] Left column shows the **DECK** stack of three offset card backs with `52` on the top card.
- [ ] Right column from top to bottom: `-- PLAYER (-) --`, the action bar, `-- DEALER (-) --`. Both hands are empty but keep their height (the DEALER section does not jump when cards appear).
- [ ] Status line reads `Place your bet to start the first round.`
- [ ] Action bar shows `[B] Place Bet  [H] Hit  [S] Stand  [D] Double Down  [Q] Quit`; only **Place Bet** and **Quit** look enabled, Hit / Stand / Double Down are dimmed.
- [ ] Keyboard focus starts on a game action and not on Quit (a focus ring is visible on Place Bet).
- [ ] The window cannot be resized below about 1000 x 560 DIP.

## 2. Betting

- [ ] Press **B** (or click Place Bet): a dialog titled `Place Bet` opens with the text `Enter bet amount (balance 1000):`, an input box with focus, and buttons **Deal** (default) and **Cancel**.
- [ ] Type `100` and press Enter (or click Deal): the dialog closes, header shows `Balance: 900   Bet: 100`, player and dealer each have two cards, the dealer's second card shows the card back, status reads `Your turn: Hit, Stand, or Double Down.`
- [ ] Player header shows the real value, e.g. `-- PLAYER (15) --`; dealer header shows the visible card only, e.g. `-- DEALER (10 + ?) --` (an Ace counts 11, J/Q/K count 10).
- [ ] Hit / Stand / Double Down are enabled, Place Bet is dimmed, and focus is on Hit.
- [ ] Deck count dropped by 4 (52 -> 48).
- [ ] In the dialog type `abc`, `12.5` or leave it empty and press Deal: the dialog closes and the status reads `Invalid bet amount - enter a whole number.`; no round starts.
- [ ] Bet `0` and `-5`: status shows the engine message (`Bet amount must be positive.`); no round starts.
- [ ] Bet more than the balance (e.g. `5000`): status reads `Insufficient balance to place the bet.`; no round starts, balance is unchanged.
- [ ] Press **Cancel** or **Esc**: nothing changes (status, cards and balance as before).
- [ ] While the dialog is open and the input box is focused, type `h s d b q 1 2`: the letters appear in the box and no game action fires; the window does not quit.
- [ ] Hold **B** down (real key auto-repeat): exactly one dialog opens, no crash, no second dialog stacked on top.
- [ ] Known engine quirk (issue #20, not a UI bug): after a finished round, a rejected bet makes the previous round's cards disappear from the table.

## 3. Round outcomes

The deck is random, so play rounds until you have seen each result. After each one check
the status line, the header and the dealer hand.

- [ ] **Win:** `You win! Press [B] to play again.`; balance goes up by the bet; header `Last Round: PlayerWin`; `Bet: 0`.
- [ ] **Loss:** `Dealer wins. Press [B] to play again.`; balance went down by the bet; `Last Round: DealerWin`.
- [ ] **Bust** (Hit until over 21): the round ends at once with `Dealer wins. ...`; the dealer's hole card is turned face up and the dealer header shows a full value instead of `N + ?`.
- [ ] **Push:** `Push - bet returned. Press [B] to play again.`; balance back to what it was before the bet; `Last Round: Push`.
- [ ] **Blackjack** (Ace + 10-value on the deal; roughly 1 round in 20, keep playing): `Blackjack! You win 3:2. Press [B] to play again.`; balance up by 1.5 x bet; `Last Round: Blackjack`.
- [ ] **Double Down** (with exactly two cards): exactly one more card is dealt to the player, the round ends at once, and the balance changes by 2 x the original bet (win +2x, loss -2x).
- [ ] Double Down is dimmed after a Hit (three cards), and also when the balance is lower than the bet (e.g. bet 600 of 1000).
- [ ] The dealer's hole card stays face down during play and is revealed after Stand, Double Down and bust. The dealer header changes from `N + ?` to the full value at that moment.
- [ ] After every round Place Bet is enabled again, Hit / Stand / Double Down are dimmed, and keyboard focus is on **Place Bet** (not on Quit).
- [ ] Press **Enter** or **Space** right after Stand: the bet dialog opens, the app does not quit.

## 4. Deck and reshuffle

- [ ] Play 15-20 rounds. When the deck is below 15 cards at the end of a round, the deck stack shows `REFILLED` in green (success colour) for exactly one update and then the new count (about 52).
- [ ] The refill never happens in the middle of a round: the count only goes down while cards are on the table.
- [ ] After the refill the cards on the table still show the right art (no stale or blank cards).
- [ ] 20+ rounds in a row (bet, play, outcome, bet) without a crash, slowdown, or wrong card art.

## 5. Keyboard and mouse

- [ ] **H / S / D / B / Q** work in upper and lower case, wherever focus is.
- [ ] A key for a dimmed action does nothing (e.g. **H** before betting, **B** during a round).
- [ ] Ctrl+H, Alt+S, etc. are ignored by the game.
- [ ] **Tab** moves through the five buttons in display order (Place Bet, Hit, Stand, Double Down, Quit); Enter / Space activates the focused one; focus rings are clearly visible.
- [ ] A whole round can be played with the keyboard only (B, type amount, Enter, H / S / D) without touching the mouse.
- [ ] Mouse clicks on every enabled button work; clicking a dimmed button does nothing.
- [ ] Hold **H** during a round (real OS key auto-repeat): the round ends on bust and further presses are ignored; no crash, no unhandled-error message.
- [ ] Hammer **H, S, D, B** in every state (before betting, mid-round, after a round, with the dialog open): no crash and no "Something went wrong" message.

## 6. Quit

- [ ] **Q** closes the window and the process ends (no confirmation prompt).
- [ ] Clicking the **Quit** button does the same.
- [ ] Clicking the window's **X** does the same.
- [ ] With the bet dialog open, clicking the window's **X** closes the app cleanly (no crash, no hang). Q does not quit while the dialog is open (it is typed into the box).
- [ ] After closing, no `Blackjack.WinUI.exe` process stays behind in Task Manager.

## 7. Startup failure

- [ ] Close the app. In the exe's folder rename `Assets\CardArt.txt` to `CardArt.txt.bak` and start the exe: a small window titled **Blackjack cannot start** appears (no game window, no crash dialog).
- [ ] It states the reason and the path, starting `The card art could not be loaded. Card art file '...CardArt.txt' could not be read: ...`; the **Close** button has focus and closes the app.
- [ ] Put the file back and confirm the game starts normally again.
- [ ] Optional: replace `CardArt.txt` with a file of fewer than 371 lines (53 cards x 7 lines): the same window appears and says how many lines were found and how many are required.

## 8. Out of credits (WinUI-only addition)

- [ ] Bet the whole balance (`1000`) every round until you lose one: with `Balance: 0` and no round running, the status reads `You are out of credits.`
- [ ] Only **Quit** is enabled; Place Bet, Hit, Stand and Double Down are dimmed; pressing **B** does nothing.
- [ ] Focus is on Quit, so Enter or Space quits the app (intended in this state).
- [ ] The console app does not have this state: it still lets you try a bet and shows the engine's rejection message.

## 9. Many cards

- [ ] A hand with 8 cards (hit repeatedly on low cards; this is rare, so it may take many rounds) fits in the default window with every card fully visible.
- [ ] With 9 or more cards the row scrolls horizontally; it can be reached with **Tab** (a focus visual appears) and scrolled with the arrow keys; the scrollbar does not cover the card art.
- [ ] At the minimum window size with 8 cards, nothing overlaps the deck stack or the action bar.

## 10. Looks: theme and scaling

Check each of the following in **Light** and **Dark** (Settings > Personalization > Colors):

- [ ] Header, hand titles and status text are readable on the background.
- [ ] Red suits (hearts, diamonds) are red and readable in both themes; clubs and spades use the normal text colour.
- [ ] Card backs, the deck stack and the `REFILLED` green are distinguishable from the background and from each other.
- [ ] Disabled actions are clearly dimmer than enabled ones; focus rings are visible.
- [ ] Switching the theme while the app is open updates the colours without a restart.
- [ ] Optional: Windows High Contrast theme. (Known gap: the card view has no High Contrast palette; just note what you see.)

Check each of the following at **100%** and **150%** display scaling (Settings > System > Display; sign out and in if the app does not pick up the change):

- [ ] The window opens at a sensible size and the 1000 x 560 DIP minimum holds.
- [ ] Card art and the deck stack keep their grid: no vertical gaps between the lines of a card, no clipped edges.
- [ ] Cascadia Mono is used if installed; the Consolas fallback also lines up (rename or test on a machine without Cascadia).
- [ ] Moving the window to a monitor with a different scaling keeps it usable (known nit: the minimum size is only applied on first load).

## 11. Screen reader (Narrator)

Start Narrator with Win+Ctrl+Enter.

- [ ] The header is announced as one item, e.g. `Balance 1000, bet 0, last round none`; the individual labels are not repeated.
- [ ] Each hand is announced as `Player hand, 2 cards, value 15` and `Dealer hand, 2 cards, value 10 + ?`; an empty hand as `Player hand, empty`. The face-down card is never read out as a real card.
- [ ] The deck is announced as `Deck, 48 cards remaining`, or `Deck refilled` for the one update after a refill.
- [ ] Buttons are announced with their label and key (e.g. Place Bet, B); dimmed ones as unavailable.
- [ ] The status line is read out when it changes (after a bet, a Hit, an outcome, a rejected bet) without moving focus.
- [ ] The bet dialog title, prompt and input are read in a sensible order (the input is labelled by the prompt text).
- [ ] Tab order and reading order are the same: header, deck, PLAYER, actions, DEALER.
- [ ] The empty `Actions` group is not announced as an empty container.

## 12. Console app regression (issue #4)

The console app was refactored onto the shared `GameController`; it was not run
interactively by the agents. Run it from a real terminal (it needs a console window of
at least 96 columns; it fails when its input or output is redirected):

```
dotnet run --project src/Blackjack.Console -c Release
```

- [ ] The screen looks as before: header line, deck stack left, PLAYER / action bar / DEALER right, ASCII card art.
- [ ] One full round with B, H, S, D works and shows the same status wording as the WinUI app.
- [ ] After a refill `REFILLED` stays on the deck until the next mapped key (accepted deviation from the old one-frame display).
- [ ] **Q** exits and prints `Thanks for playing!`.
