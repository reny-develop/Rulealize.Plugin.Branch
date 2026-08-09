// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using Rulealize.Abstraction.Building;
using Rulealize.Abstraction.Evaluation;
using Rulealize.Abstraction.Node;
using Rulealize.Abstraction.Value;

namespace Rulealize.Plugin.Branch
{
    /// <summary>Two-way branch on a boolean condition.</summary>
    /// <remarks>
    /// <para>
    /// The unchosen arm is not evaluated. That is more than an optimisation: it is what
    /// lets a rule author guard an expression that would otherwise fault, such as indexing
    /// into a sequence only after establishing that it is long enough.
    /// </para>
    /// <para>
    /// The condition must be a boolean. Null is not falsy here, because <c>grid.at</c>
    /// answers null for an empty square and a guard that meant to ask about a stone would
    /// otherwise take the else arm for a reason its author never wrote down.
    /// </para>
    /// <para>
    /// Omitting <c>else</c> yields null rather than being an error, so that a branch used
    /// for its value in one arm and nothing in the other stays short.
    /// </para>
    /// </remarks>
    internal sealed class IfNode(ExpressionNode condition, ExpressionNode consequent, ExpressionNode? alternative)
        : ExpressionNode
    {
        public static ExpressionNode Build(INodeBuildContext context) =>
            new IfNode(
                context.RequireExpression("cond"),
                context.RequireExpression("then"),
                context.OptionalExpression("else"));

        public override RuleValue Evaluate(IEvaluationContext context)
        {
            if (condition.Evaluate(context).AsBoolean("branch.if.cond"))
            {
                return consequent.Evaluate(context);
            }

            return alternative is null ? RuleValue.Null : alternative.Evaluate(context);
        }
    }
}
