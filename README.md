# Rulealize.Plugin.Branch

Conditional branching for [Rulealize](https://github.com/reny-develop/Rulealize) rule sets.

| | |
| --- | --- |
| Plugin id | `Rulealize.Plugin.Branch` |
| Namespace | `branch` |
| Reserved prefix | none |
| Depends on | `Rulealize.Abstraction` |

Separate from `Rulealize.Plugin.Logic`, which builds conditions rather than choosing
between them. A rule set that only states constraints can leave this plugin out.

## Operations

```jsonc
{ "op": "branch.if", "cond": <bool>, "then": <expr>, "else": <expr> }   // else optional

{ "op": "branch.match",
  "value": <expr>,
  "cases": { "<key>": <expr>, … },    // keys are literal
  "default": <expr> }                 // optional
```

Only the chosen arm is evaluated. That is not just about cost — it is what lets a rule
author guard an expression that would otherwise fault, such as indexing into a sequence
only once its length has been established.

`branch.if` without an `else` yields null.

## The condition must be a boolean

Null is not falsy. Zero is not falsy. An empty sequence is not falsy. A condition of any
other kind is an evaluation error.

`grid.at` is the reason. It answers null for an empty square, for a square off the board
and for a null coordinate alike, and under truthiness a guard that meant to ask "is there
a black stone here" would take the else arm for a reason nobody wrote down. Emptiness gets
written out, as `cmp.isNull`.

## How `branch.match` matches

The subject is converted to its canonical text and compared to the case keys exactly.

| Kind | Canonical text |
| --- | --- |
| Text | itself |
| Bool | `"true"` / `"false"` |
| Number | normalised decimal — `1.0` matches the key `"1"` |
| Null | `"null"` |
| Opaque | whatever the defining plugin says, e.g. `"d3"` for a coordinate |
| Sequence / Record | none; matching one is an evaluation error |

Opaque values matching by text is what makes it possible to branch on a coordinate or a
direction without this plugin knowing what either of those is.

A subject that matches no case and has no `default` is an evaluation error. There is no
exhaustiveness check: deciding statically that the cases cover every possibility would
require the subject's type, and the DSL has no type inference. Othello's

```jsonc
{ "op": "branch.match", "value": "#me", "cases": { "black": "white", "white": "black" } }
```

is safe in practice, but what makes it safe is the `type.enum` in `state.schema`, not
anything this node checks.

## Building

`Rulealize.Abstraction` is not on nuget.org yet, so `NuGet.config` points at a folder
feed. Produce it from the abstraction repository first:

```
dotnet pack path\to\Rulealize.Abstraction\src\Rulealize.Abstraction -c Release -o path\to\LocalNuGet
```

with `LocalNuGet` a sibling of this repository. Then `dotnet build`.

## License

Apache-2.0.
