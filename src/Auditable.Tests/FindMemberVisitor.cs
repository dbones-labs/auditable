namespace Auditable.Tests
{
    using System.Linq.Expressions;
    using System.Text;

    internal class FindMemberVisitor : ExpressionVisitor
    {
        public static string GetMember(Expression linqExpression)
        {
            var visitor = new FindMemberVisitor();
            visitor.Visit(linqExpression);
            return visitor.ToString();
        }

        private readonly StringBuilder _memberName = new StringBuilder();
        private bool _isBody = false;
        private readonly string _alias;

        private FindMemberVisitor()
        {
        }

        public override string ToString()
        {
            return _memberName.ToString();
        }


        protected override Expression VisitMember(MemberExpression expression)
        {
            Visit(expression.Expression);
            if (_memberName.Length > 0)
            {
                _memberName.Append(".");
            }
            _memberName.AppendFormat(expression.Member.Name);

            return expression;
        }
    }
}