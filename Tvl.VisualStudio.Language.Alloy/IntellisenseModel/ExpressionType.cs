namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public enum ExpressionType
    {
        Constant,
        Name,
        Declaration,
        Relation,
        Block,

        // the special global sets
        EmptySet,
        UniversalSet,
        IdentitySet,

        // set operators
        Union,
        Intersection,
        Difference,
        Subset,
        Equal,

        // relational operators
        ArrowProduct,
        DotJoin,
        BoxJoin,
        Transpose,
        TransitiveClosure,
        ReflexiveTransitiveClosure,
        DomainRestriction,
        RangeRestriction,
        Override,

        Call,

        // logical operators
        Not,
        AndAlso,
        OrElse,
        Implies,
        Iff,

        // let
        Let,

        // quantification and multiplicity
        None,
        Lone,
        One,
        Some,
        All,
        Sum,
        Set,
        Sequence,

        // integer operators
        Add,
        Subtract,
        Equals,
        LessThan,
        GreaterThan,
        LessThanOrEqualTo,
        GreaterThanOrEqualTo,
        Count,
        LeftShift,
        RightShift,
        RightRotate,
        And,

        // ambiguous operators
        UnionOrAdd,
        DifferenceOrSubtract,
    }
}
