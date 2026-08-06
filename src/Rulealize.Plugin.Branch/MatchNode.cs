// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using System.Text.Json;
using Rulealize.Abstraction;
using Rulealize.Abstraction.Building;
using Rulealize.Abstraction.Evaluation;
using Rulealize.Abstraction.Nodes;
using Rulealize.Abstraction.Values;

namespace Rulealize.Plugin.Branch
{
    /// <summary>Multi-way branch on the canonical text of a value.</summary>
    /// <remarks>
    /// <para>
    /// Case keys are the keys of a JSON object, so they are always strings; the subject is
    /// converted to its canonical text and matched against them exactly. Numbers normalise
    /// first, so <c>1.0</c> matches the key <c>"1"</c>. Opaque values match through the text
    /// form their own plugin defines, which is what makes it possible to branch on a
    /// coordinate or a direction.
    /// </para>
    /// <para>
    /// Sequences and records have no canonical text and cannot be matched.
    /// </para>
    /// <para>
    /// There is no exhaustiveness check. Deciding statically that the cases cover
    /// everything would need the subject's type, and the DSL has no type inference yet; a
    /// missing case is found when it is missed. Othello's turn-flipping is safe in
    /// practice, but what makes it safe is the enumeration in <c>state.schema</c>, not
    /// anything this node knows.
    /// </para>
    /// </remarks>
    internal sealed class MatchNode(
        ExpressionNode subject,
        IReadOnlyDictionary<string, ExpressionNode> cases,
        ExpressionNode? fallback) : ExpressionNode
    {
        public static ExpressionNode Build(INodeBuildContext context)
        {
            JsonElement element = context.GetRequiredProperty("cases");
            if (element.ValueKind != JsonValueKind.Object)
            {
                throw context.Error("cases", "must be an object mapping case keys to expressions.");
            }

            Dictionary<string, ExpressionNode> cases = new(StringComparer.Ordinal);
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (cases.ContainsKey(property.Name))
                {
                    throw context.Error("cases", $"the case '{property.Name}' is given more than once.");
                }

                cases.Add(property.Name, context.BuildExpression(property.Value, $"cases/{property.Name}"));
            }

            if (cases.Count == 0)
            {
                throw context.Error("cases", "must not be empty.");
            }

            return new MatchNode(
                context.RequireExpression("value"),
                cases,
                context.OptionalExpression("default"));
        }

        public override RuleValue Evaluate(IEvaluationContext context)
        {
            RuleValue value = subject.Evaluate(context);
            string? key = value.GetCanonicalText();
            if (key is null)
            {
                throw new RuleEvaluationException(
                    "branch.match.value",
                    $"Cannot match on {RuleValue.Describe(value)}; it has no text form.");
            }

            if (cases.TryGetValue(key, out ExpressionNode? branch))
            {
                return branch.Evaluate(context);
            }

            if (fallback is not null)
            {
                return fallback.Evaluate(context);
            }

            throw new RuleEvaluationException(
                "branch.match",
                $"No case matches '{key}', and there is no default.");
        }
    }
}
