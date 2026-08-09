// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using Rulealize.Abstraction.Plugin;

namespace Rulealize.Plugin.Branch
{
    /// <summary>
    /// Conditional branching over the <c>branch</c> namespace.
    /// </summary>
    /// <remarks>
    /// Separate from the logic plugin, which builds conditions rather than choosing
    /// between them. A rule set that only states constraints — no branches anywhere — can
    /// leave this one out.
    /// </remarks>
    public sealed class BranchPlugin : IRulealizePlugin
    {
        /// <inheritdoc />
        public PluginManifest Manifest { get; } =
            new("Rulealize.Plugin.Branch", new Version(1, 0, 0), "branch");

        /// <inheritdoc />
        public void Register(IPluginRegistry registry)
        {
            ArgumentNullException.ThrowIfNull(registry);

            registry.AddExpression("if", IfNode.Build);
            registry.AddExpression("match", MatchNode.Build);
        }
    }
}
