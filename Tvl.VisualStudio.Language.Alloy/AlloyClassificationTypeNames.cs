#pragma warning disable 169 // The field 'fieldname' is never used

namespace Tvl.VisualStudio.Language.Alloy
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    public static class AlloyClassificationTypeNames
    {
        public const string ExpressionToken = "AlloyExpressionToken";
        public const string ExpressionStartToken = "AlloyExpressionStartToken";
        public const string ExpressionEndToken = "AlloyExpressionEndToken";
        public const string LeadingExpressionKeyword = "AlloyLeadingExpressionKeyword";
        public const string TrailingExpressionKeyword = "AlloyTrailingExpressionKeyword";
        public const string DoubleEndedExpressionKeyword = "AlloyDoubleEndedExpressionKeyword";
        public const string InnerExpressionKeyword = "AlloyInnerExpressionKeyword";
        public const string LeadingOperator = "AlloyLeadingOperator";
        public const string TrailingOperator = "AlloyTrailingOperator";
        public const string InnerOperator = "AlloyInnerOperator";
        public const string Identifier = "AlloyIdentifier";
        public const string Number = "AlloyNumber";

        [Export]
        [Name(AlloyClassificationTypeNames.ExpressionToken)]
        private static readonly ClassificationTypeDefinition ExpressionTokenClassification;

        [Export]
        [Name(AlloyClassificationTypeNames.ExpressionStartToken)]
        [BaseDefinition(AlloyClassificationTypeNames.ExpressionToken)]
        private static readonly ClassificationTypeDefinition ExpressionStartTokenClassification;

        [Export]
        [Name(AlloyClassificationTypeNames.ExpressionEndToken)]
        [BaseDefinition(AlloyClassificationTypeNames.ExpressionToken)]
        private static readonly ClassificationTypeDefinition ExpressionEndTokenClassification;

        [Export]
        [Name(AlloyClassificationTypeNames.LeadingExpressionKeyword)]
        [BaseDefinition(AlloyClassificationTypeNames.ExpressionStartToken)]
        [BaseDefinition(PredefinedClassificationTypeNames.Keyword)]
        private static readonly ClassificationTypeDefinition LeadingExpressionKeywordClassification;

        [Export]
        [Name(AlloyClassificationTypeNames.TrailingExpressionKeyword)]
        [BaseDefinition(AlloyClassificationTypeNames.ExpressionEndToken)]
        [BaseDefinition(PredefinedClassificationTypeNames.Keyword)]
        private static readonly ClassificationTypeDefinition TrailingExpressionKeywordClassification;

        [Export]
        [Name(AlloyClassificationTypeNames.DoubleEndedExpressionKeyword)]
        [BaseDefinition(AlloyClassificationTypeNames.ExpressionStartToken)]
        [BaseDefinition(AlloyClassificationTypeNames.ExpressionEndToken)]
        [BaseDefinition(PredefinedClassificationTypeNames.Keyword)]
        private static readonly ClassificationTypeDefinition DoubleEndedExpressionKeywordClassification;

        [Export]
        [Name(AlloyClassificationTypeNames.InnerExpressionKeyword)]
        [BaseDefinition(AlloyClassificationTypeNames.ExpressionToken)]
        [BaseDefinition(PredefinedClassificationTypeNames.Keyword)]
        private static readonly ClassificationTypeDefinition InnerExpressionKeywordClassification;

        [Export]
        [Name(AlloyClassificationTypeNames.LeadingOperator)]
        [BaseDefinition(AlloyClassificationTypeNames.ExpressionStartToken)]
        [BaseDefinition(PredefinedClassificationTypeNames.Operator)]
        private static readonly ClassificationTypeDefinition LeadingOperatorClassification;

        [Export]
        [Name(AlloyClassificationTypeNames.TrailingOperator)]
        [BaseDefinition(AlloyClassificationTypeNames.ExpressionEndToken)]
        [BaseDefinition(PredefinedClassificationTypeNames.Operator)]
        private static readonly ClassificationTypeDefinition TrailingOperatorClassification;

        [Export]
        [Name(AlloyClassificationTypeNames.InnerOperator)]
        [BaseDefinition(AlloyClassificationTypeNames.ExpressionToken)]
        [BaseDefinition(PredefinedClassificationTypeNames.Operator)]
        private static readonly ClassificationTypeDefinition InnerOperatorClassification;

        [Export]
        [Name(AlloyClassificationTypeNames.Identifier)]
        [BaseDefinition(AlloyClassificationTypeNames.ExpressionStartToken)]
        [BaseDefinition(AlloyClassificationTypeNames.ExpressionEndToken)]
        [BaseDefinition(PredefinedClassificationTypeNames.Identifier)]
        private static readonly ClassificationTypeDefinition IdentifierClassification;

        [Export]
        [Name(AlloyClassificationTypeNames.Number)]
        [BaseDefinition(AlloyClassificationTypeNames.ExpressionStartToken)]
        [BaseDefinition(AlloyClassificationTypeNames.ExpressionEndToken)]
        [BaseDefinition(PredefinedClassificationTypeNames.Number)]
        private static readonly ClassificationTypeDefinition NumberClassification;
    }
}
