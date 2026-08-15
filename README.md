# Rulealize.Plugin.Branch

Conditional branching for [Rulealize](https://github.com/reny-develop/Rulealize) rule sets.

| | |
| --- | --- |
| Plugin id | `Rulealize.Plugin.Branch` |
| Namespace | `branch` |
| Reserved prefix | none |
| Depends on | `Rulealize.Abstraction` |
| Specification | [doc/specification.md](doc/specification.md) |

`branch.if` chooses on a condition, `branch.match` on a value. Separate from
`Rulealize.Plugin.Logic`, which builds conditions rather than choosing between them, so a
rule set that only states constraints can leave this plugin out.

Only the chosen arm is evaluated. That is not just about cost — it is what lets a rule
author guard an expression that would otherwise fault, such as indexing into a sequence
only once its length has been established.

Two things the specification settles that are worth knowing before you read it. A condition
must be a boolean: null is not falsy, zero is not falsy, an empty sequence is not falsy.
And `branch.match` compares the canonical text of its subject against literal case keys,
which is what lets a rule set branch on a coordinate or a direction without this plugin
knowing what either of those is.

## Building

`dotnet build`. `Rulealize.Abstraction` restores from nuget.org like any other package, so
this repository builds on its own.

[`NuGet.config`](NuGet.config) also adds a folder feed named `LocalNuGet` beside the
repositories — added to nuget.org rather than replacing it — which is how a change to the
abstraction is tried out before it is published. Pack it when you have changed it:

```sh
dotnet pack path\to\Rulealize.Abstraction\src\Rulealize.Abstraction -c Release -o path\to\LocalNuGet
```

## License

Apache-2.0.
