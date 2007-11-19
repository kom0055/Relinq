using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.Linq.Clauses;

namespace Rubicon.Data.DomainObjects.Linq.UnitTests.ClausesTest
{
  [TestFixture]
  public class FromClauseTest
  {
    [Test]
    public void Initialize_WithIDAndExpression()
    {
      ParameterExpression id = ExpressionHelper.CreateParameterExpression ();
      IQueryable querySource = ExpressionHelper.CreateQuerySource();
      
      FromClause fromClause = new FromClause (id, querySource);
      
      Assert.AreSame (id, fromClause.Identifier);
      Assert.AreSame (querySource, fromClause.QuerySource);
      
      Assert.That (fromClause.JoinClauses, Is.Empty);
      Assert.AreEqual (0, fromClause.JoinClauseCount);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Initialize_ThrowsOnNullID ()
    {
      IQueryable querySource = ExpressionHelper.CreateQuerySource ();
      new FromClause (null, querySource);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Initialize_ThrowsOnNullQuerySource ()
    {
      ParameterExpression id = ExpressionHelper.CreateParameterExpression();
      new FromClause (id, null);
    }

    [Test]
    public void AddJoinClause()
    {
      FromClause fromClause = ExpressionHelper.CreateFromClause();

      JoinClause joinClause1 = ExpressionHelper.CreateJoinClause();
      JoinClause joinClause2 = ExpressionHelper.CreateJoinClause();

      fromClause.Add (joinClause1);
      fromClause.Add (joinClause2);

      Assert.That (fromClause.JoinClauses, Is.EqualTo (new object[] { joinClause1, joinClause2 }));
      Assert.AreEqual (2, fromClause.JoinClauseCount);
    }

    
    [Test]
    public void ImplementInterface_IFromLetWhereClause()
    {
      ParameterExpression id = ExpressionHelper.CreateParameterExpression ();
      IQueryable querySource = ExpressionHelper.CreateQuerySource ();
      

      FromClause fromClause = new FromClause (id, querySource);

      Assert.IsInstanceOfType (typeof (IFromLetWhereClause), fromClause);
    }

    [Test]
    public void FromClause_ImplementsIQueryElement()
    {
      FromClause fromClause = ExpressionHelper.CreateFromClause();
      Assert.IsInstanceOfType (typeof (IQueryElement), fromClause);
    }

    [Test]
    public void Accept()
    {
      FromClause fromClause = ExpressionHelper.CreateFromClause();

      MockRepository repository = new MockRepository();
      IQueryVisitor visitorMock = repository.CreateMock<IQueryVisitor>();

      visitorMock.VisitFromClause(fromClause);

      repository.ReplayAll();

      fromClause.Accept (visitorMock);

      repository.VerifyAll();

    }
  }
}