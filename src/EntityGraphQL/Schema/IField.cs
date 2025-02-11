using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EntityGraphQL.Compiler;
using EntityGraphQL.Compiler.Util;
using EntityGraphQL.Schema.Directives;
using EntityGraphQL.Schema.FieldExtensions;

namespace EntityGraphQL.Schema
{
    public enum GraphQLQueryFieldType
    {
        Query,
        Mutation,
        Subscription,
    }
    /// <summary>
    /// Represents a field in a GraphQL type. This can be a mutation field in the Mutation type or a field on a query type
    /// </summary>
    public interface IField
    {
        GraphQLQueryFieldType FieldType { get; }
        ISchemaProvider Schema { get; }
        ParameterExpression? FieldParam { get; set; }
        List<GraphQLExtractedField>? ExtractedFieldsFromServices { get; }
        string? Description { get; }
        IDictionary<string, ArgType> Arguments { get; }
        /// <summary>
        /// The parameter expression for the field arguments (if any). Note that this instance is replaced within the
        /// expression at execution time. You should not store this at configuration time in field extensions
        /// </summary>
        ParameterExpression? ArgumentParam { get; }
        Type? ArgumentsType { get; set; }
        string Name { get; }
        /// <summary>
        /// GraphQL type this fiel belongs to
        /// </summary>
        ISchemaType FromType { get; }
        GqlTypeInfo ReturnType { get; }
        List<IFieldExtension> Extensions { get; set; }
        RequiredAuthorization? RequiredAuthorization { get; }

        IList<ISchemaDirective> DirectivesReadOnly { get; }
        IField AddDirective(ISchemaDirective directive);

        ArgType GetArgumentType(string argName);
        bool HasArgumentByName(string argName);
        bool ArgumentsAreInternal { get; }
        IEnumerable<Type> Services { get; }
        IReadOnlyCollection<Action<ArgumentValidatorContext>> Validators { get; }
        IField? UseArgumentsFromField { get; }

        /// <summary>
        /// Given the current context, a type and a field name, it returns the expression for that field. Allows the provider to have a complex expression for a simple field
        /// </summary>
        /// <param name="context"></param>
        /// <param name="previousContext"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        (Expression? expression, ParameterExpression? argumentParam) GetExpression(Expression fieldExpression, Expression? fieldContext, IGraphQLNode? parentNode, ParameterExpression? schemaContext, CompileContext? compileContext, IReadOnlyDictionary<string, object> args, ParameterExpression? docParam, object? docVariables, IEnumerable<GraphQLDirective> directives, bool contextChanged, ParameterReplacer replacer);
        Expression? ResolveExpression { get; }

        IField UpdateExpression(Expression expression);

        void AddExtension(IFieldExtension extension);
        void AddArguments(object args);
        IField Returns(GqlTypeInfo gqlTypeInfo);
        void UseArgumentsFrom(IField field);
        IField AddValidator<TValidator>() where TValidator : IArgumentValidator;
        IField AddValidator(Action<ArgumentValidatorContext> callback);

        /// <summary>
        /// To access this field all roles listed here are required
        /// </summary>
        /// <param name="roles"></param>
        IField RequiresAllRoles(params string[] roles);
        /// <summary>
        /// To access this field any role listed is required
        /// </summary>
        /// <param name="roles"></param>
        IField RequiresAnyRole(params string[] roles);
        /// <summary>
        /// To access this field all policies listed here are required
        /// </summary>
        /// <param name="policies"></param>
        IField RequiresAllPolicies(params string[] policies);
        /// <summary>
        /// To access this field any policy listed is required
        /// </summary>
        /// <param name="policies"></param>
        IField RequiresAnyPolicy(params string[] policies);
    }
}