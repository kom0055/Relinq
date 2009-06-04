// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using System.Linq.Expressions;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using System.Reflection;
using Rhino.Mocks.Interfaces;

namespace Remotion.Data.UnitTests.Linq.Parsing.ExpressionTreeVisitorTests
{
  [TestFixture]
  public class ExpressionTreeVisitor_SpecificExpressionsTest : ExpressionTreeVisitor_SpecificExpressionsTestBase
  {
    [Test]
    public void VisitUnaryExpression_Unchanges ()
    {
      var expression = (UnaryExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.UnaryPlus);
      Expression expectedNextVisit = expression.Operand;
      Expect.Call (InvokeVisitMethod ("VisitExpression", expectedNextVisit)).Return (expectedNextVisit);

      Assert.That (InvokeAndCheckVisitExpression ("VisitUnaryExpression", expression), Is.SameAs (expression));
    }

    [Test]
    public void VisitUnaryExpression_UnaryPlus_Changes ()
    {
      var expression = (UnaryExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.UnaryPlus);
      Expression expectedNextVisit = expression.Operand;
      Expression newOperand = Expression.Constant (1);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expectedNextVisit)).Return (newOperand);

      var result = (UnaryExpression) InvokeAndCheckVisitExpression ("VisitUnaryExpression", expression);
      Assert.That (result.Operand, Is.SameAs (newOperand));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.UnaryPlus));
    }

    [Test]
    public void VisitUnaryExpression_Negate_Changes ()
    {
      var expression = (UnaryExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Negate);
      Expression expectedNextVisit = expression.Operand;
      Expression newOperand = Expression.Constant (1);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expectedNextVisit)).Return (newOperand);

      var result = (UnaryExpression) InvokeAndCheckVisitExpression ("VisitUnaryExpression", expression);
      Assert.That (result.Operand, Is.SameAs (newOperand));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.Negate));
    }

    [Test]
    public void VisitTypeBinary_Unchanged ()
    {
      var expression = (TypeBinaryExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.TypeIs);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Expression)).Return (expression.Expression);
      var result = (TypeBinaryExpression) InvokeAndCheckVisitExpression ("VisitTypeBinaryExpression", expression);
      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    public void VisitTypeBinary_Changed ()
    {
      var expression = (TypeBinaryExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.TypeIs);
      Expression newExpression = Expression.Constant (1);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Expression)).Return (newExpression);
      var result = (TypeBinaryExpression) InvokeAndCheckVisitExpression ("VisitTypeBinaryExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.TypeIs));
      Assert.That (result.Expression, Is.SameAs (newExpression));
    }

    [Test]
    public void VisitConstantExpression ()
    {
      var expression = (ConstantExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Constant);
      var result = (ConstantExpression) InvokeAndCheckVisitExpression ("VisitConstantExpression", expression);
      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    public void VisitConditionalExpression_Unchanged ()
    {
      var expression = (ConditionalExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Conditional);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Test)).Return (expression.Test);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.IfFalse)).Return (expression.IfFalse);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.IfTrue)).Return (expression.IfTrue);
      var result = (ConditionalExpression) InvokeAndCheckVisitExpression ("VisitConditionalExpression", expression);
      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    public void VisitConditionalExpression_ChangedTest ()
    {
      var expression = (ConditionalExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Conditional);
      Expression newTest = Expression.Constant (true);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Test)).Return (newTest);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.IfFalse)).Return (expression.IfFalse);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.IfTrue)).Return (expression.IfTrue);
      var result = (ConditionalExpression) InvokeAndCheckVisitExpression ("VisitConditionalExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.Conditional));
      Assert.That (result.Test, Is.SameAs (newTest));
      Assert.That (result.IfFalse, Is.SameAs (expression.IfFalse));
      Assert.That (result.IfTrue, Is.SameAs (expression.IfTrue));
    }

    [Test]
    public void VisitConditionalExpression_ChangedFalse ()
    {
      var expression = (ConditionalExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Conditional);
      Expression newFalse = Expression.Constant (1);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Test)).Return (expression.Test);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.IfFalse)).Return (newFalse);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.IfTrue)).Return (expression.IfTrue);
      var result = (ConditionalExpression) InvokeAndCheckVisitExpression ("VisitConditionalExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.Conditional));
      Assert.That (result.IfFalse, Is.SameAs (newFalse));
      Assert.That (result.Test, Is.SameAs (expression.Test));
      Assert.That (result.IfTrue, Is.SameAs (expression.IfTrue));
    }

    [Test]
    public void VisitConditionalExpression_ChangedTrue ()
    {
      var expression = (ConditionalExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Conditional);
      Expression newTrue = Expression.Constant (1);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Test)).Return (expression.Test);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.IfFalse)).Return (expression.IfFalse);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.IfTrue)).Return (newTrue);
      var result = (ConditionalExpression) InvokeAndCheckVisitExpression ("VisitConditionalExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.Conditional));
      Assert.That (result.IfTrue, Is.SameAs (newTrue));
      Assert.That (result.Test, Is.SameAs (expression.Test));
      Assert.That (result.IfFalse, Is.SameAs (expression.IfFalse));
    }

    [Test]
    public void VisitParameterExpression ()
    {
      var expression = (ParameterExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Parameter);
      var result = (ParameterExpression) InvokeAndCheckVisitExpression ("VisitParameterExpression", expression);
      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    public void VisitLambdaExpression_Unchanged ()
    {
      var expression = (LambdaExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Lambda);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Body)).Return (expression.Body);
      Expect.Call (InvokeVisitExpressionListMethod (expression.Parameters)).Return (expression.Parameters);
      var result = (LambdaExpression) InvokeAndCheckVisitExpression ("VisitLambdaExpression", expression);
      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    public void VisitLambdaExpression_ChangedBody ()
    {
      var expression = (LambdaExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Lambda);
      Expression newBody = Expression.Constant (1);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Body)).Return (newBody);
      Expect.Call (InvokeVisitExpressionListMethod (expression.Parameters)).Return (expression.Parameters);
      var result = (LambdaExpression) InvokeAndCheckVisitExpression ("VisitLambdaExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.Lambda));
      Assert.That (result.Body, Is.SameAs (newBody));
      Assert.That (result.Parameters, Is.SameAs (expression.Parameters));
    }

    [Test]
    public void VisitLambdaExpression_ChangedParameters ()
    {
      var expression = (LambdaExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Lambda);
      ReadOnlyCollection<ParameterExpression> newParameters = new List<ParameterExpression>().AsReadOnly();
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Body)).Return (expression.Body);
      Expect.Call (InvokeVisitExpressionListMethod (expression.Parameters)).Return (newParameters);
      var result = (LambdaExpression) InvokeAndCheckVisitExpression ("VisitLambdaExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.Lambda));
      Assert.That (result.Parameters, Is.SameAs (newParameters));
      Assert.That (result.Body, Is.SameAs (expression.Body));
    }

    [Test]
    public void VisitMethodCallExpression_Unchanged ()
    {
      var expression = (MethodCallExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Call);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Object)).Return (expression.Object);
      Expect.Call (InvokeVisitExpressionListMethod (expression.Arguments)).Return (expression.Arguments);
      var result = (MethodCallExpression) InvokeAndCheckVisitExpression ("VisitMethodCallExpression", expression);
      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    public void VisitMethodCallExpression_ChangedObject ()
    {
      var expression = (MethodCallExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Call);
      Expression newObject = Expression.Constant (1);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Object)).Return (newObject);
      Expect.Call (InvokeVisitExpressionListMethod (expression.Arguments)).Return (expression.Arguments);
      var result = (MethodCallExpression) InvokeAndCheckVisitExpression ("VisitMethodCallExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.Call));
      Assert.That (result.Object, Is.SameAs (newObject));
      Assert.That (result.Arguments, Is.SameAs (expression.Arguments));
    }

    [Test]
    public void VisitMethodCallExpression_ChangedArguments ()
    {
      var expression = (MethodCallExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Call);
      ReadOnlyCollection<Expression> newParameters = new List<Expression>().AsReadOnly();
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Object)).Return (expression.Object);
      Expect.Call (InvokeVisitExpressionListMethod (expression.Arguments)).Return (newParameters);
      var result = (MethodCallExpression) InvokeAndCheckVisitExpression ("VisitMethodCallExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.Call));
      Assert.That (result.Arguments, Is.SameAs (newParameters));
      Assert.That (result.Object, Is.SameAs (expression.Object));
    }

    [Test]
    public void VisitInvocationExpression_Unchanged ()
    {
      var expression = (InvocationExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Invoke);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Expression)).Return (expression.Expression);
      Expect.Call (InvokeVisitExpressionListMethod (expression.Arguments)).Return (expression.Arguments);
      var result = (InvocationExpression) InvokeAndCheckVisitExpression ("VisitInvocationExpression", expression);
      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    public void VisitInvocationExpression_ChangedObject ()
    {
      var expression = (InvocationExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Invoke);
      Expression newExpression = Expression.Lambda (Expression.Constant (1));
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Expression)).Return (newExpression);
      Expect.Call (InvokeVisitExpressionListMethod (expression.Arguments)).Return (expression.Arguments);
      var result = (InvocationExpression) InvokeAndCheckVisitExpression ("VisitInvocationExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.Invoke));
      Assert.That (result.Expression, Is.SameAs (newExpression));
      Assert.That (result.Arguments, Is.SameAs (expression.Arguments));
    }

    [Test]
    public void VisitInvocationExpression_ChangedArguments ()
    {
      var expression = (InvocationExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Invoke);
      ReadOnlyCollection<Expression> newParameters = new List<Expression>().AsReadOnly();
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Expression)).Return (expression.Expression);
      Expect.Call (InvokeVisitExpressionListMethod (expression.Arguments)).Return (newParameters);
      var result = (InvocationExpression) InvokeAndCheckVisitExpression ("VisitInvocationExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.Invoke));
      Assert.That (result.Arguments, Is.SameAs (newParameters));
      Assert.That (result.Expression, Is.SameAs (expression.Expression));
    }

    [Test]
    public void VisitMemberExpression_Unchanged ()
    {
      var expression = (MemberExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.MemberAccess);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Expression)).Return (expression.Expression);
      var result = (MemberExpression) InvokeAndCheckVisitExpression ("VisitMemberExpression", expression);
      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    public void VisitMemberExpression_ChangedExpression ()
    {
      var expression = (MemberExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.MemberAccess);
      Expression newExpression = Expression.Constant (DateTime.Now);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Expression)).Return (newExpression);
      var result = (MemberExpression) InvokeAndCheckVisitExpression ("VisitMemberExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.MemberAccess));
      Assert.That (result.Expression, Is.SameAs (newExpression));
    }

    [Test]
    public void VisitNewExpression_Unchanged ()
    {
      var expression = (NewExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.New);
      Expect.Call (InvokeVisitExpressionListMethod (expression.Arguments)).Return (expression.Arguments);
      var result = (NewExpression) InvokeAndCheckVisitExpression ("VisitNewExpression", expression);
      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    public void VisitNewExpression_ChangedArguments ()
    {
      var expression = (NewExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.New);
      ReadOnlyCollection<Expression> newArguments = new List<Expression>().AsReadOnly();
      Expect.Call (InvokeVisitExpressionListMethod (expression.Arguments)).Return (newArguments);
      var result = (NewExpression) InvokeAndCheckVisitExpression ("VisitNewExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.New));
      Assert.That (result.Arguments, Is.SameAs (newArguments));
    }

    [Test]
    public void VisitNewExpression_ChangedArguments_NoMembers ()
    {
      NewExpression expression = Expression.New (typeof (TypeForNewExpression).GetConstructors()[0], Expression.Constant (0));

      var newArguments = new List<Expression> { Expression.Constant (1) }.AsReadOnly();
      Expect.Call (InvokeVisitExpressionListMethod (expression.Arguments)).Return (newArguments);
      var result = (NewExpression) InvokeAndCheckVisitExpression ("VisitNewExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.New));
      Assert.That (result.Arguments, Is.SameAs (newArguments));
    }

    [Test]
    public void VisitNewExpression_ChangedArguments_WithMembers ()
    {
      NewExpression expression = Expression.New (
          typeof (TypeForNewExpression).GetConstructors()[0],
          new Expression[] { Expression.Constant (0) },
          typeof (TypeForNewExpression).GetProperty ("A"));

      var newArguments = new List<Expression> { Expression.Constant (1) }.AsReadOnly();
      Expect.Call (InvokeVisitExpressionListMethod (expression.Arguments)).Return (newArguments);
      var result = (NewExpression) InvokeAndCheckVisitExpression ("VisitNewExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.New));
      Assert.That (result.Arguments, Is.SameAs (newArguments));
      Assert.That (result.Members, Is.SameAs (expression.Members));
    }

    [Test]
    public void VisitNewArrayExpression_Unchanged ()
    {
      var expression = (NewArrayExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.NewArrayInit);
      Expect.Call (InvokeVisitExpressionListMethod (expression.Expressions)).Return (expression.Expressions);
      var result = (NewArrayExpression) InvokeAndCheckVisitExpression ("VisitNewArrayExpression", expression);
      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    public void VisitNewArrayInitExpression_Changed ()
    {
      var expression = (NewArrayExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.NewArrayInit);
      ReadOnlyCollection<Expression> newExpressions = new List<Expression>().AsReadOnly();
      Expect.Call (InvokeVisitExpressionListMethod (expression.Expressions)).Return (newExpressions);
      var result = (NewArrayExpression) InvokeAndCheckVisitExpression ("VisitNewArrayExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.NewArrayInit));
      Assert.That (result.Expressions, Is.SameAs (newExpressions));
    }

    [Test]
    public void VisitNewArrayBoundsExpression_Changed ()
    {
      var expression = (NewArrayExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.NewArrayBounds);
      ReadOnlyCollection<Expression> newExpressions = new List<Expression> (new Expression[] { Expression.Constant (0) }).AsReadOnly();
      Expect.Call (InvokeVisitExpressionListMethod (expression.Expressions)).Return (newExpressions);
      var result = (NewArrayExpression) InvokeAndCheckVisitExpression ("VisitNewArrayExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.NewArrayBounds));
      Assert.That (result.Expressions, Is.SameAs (newExpressions));
    }

    [Test]
    public void VisitMemberInitExpression_Unchanged ()
    {
      var expression = (MemberInitExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.MemberInit);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.NewExpression)).Return (expression.NewExpression);
      Expect.Call (InvokeVisitMethod ("VisitMemberBindingList", expression.Bindings)).Return (expression.Bindings);
      var result = (MemberInitExpression) InvokeAndCheckVisitExpression ("VisitMemberInitExpression", expression);
      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException),
        ExpectedMessage = "MemberInitExpressions only support non-null instances of type 'NewExpression' as their NewExpression member.")]
    public void VisitMemberInitExpression_InvalidNewExpression ()
    {
      var expression = (MemberInitExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.MemberInit);
      Expect.Call (InvokeVisitMethod<NewExpression, ConstantExpression> ("VisitExpression", expression.NewExpression)).Return (
          Expression.Constant (0));
      Expect.Call (InvokeVisitMethod ("VisitMemberBindingList", expression.Bindings)).Return (expression.Bindings);
      try
      {
        InvokeAndCheckVisitExpression ("VisitMemberInitExpression", expression);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    public void VisitMemberInitExpression_ChangedNewExpression ()
    {
      var expression = (MemberInitExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.MemberInit);
      NewExpression newNewExpression = Expression.New (typeof (List<int>));
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.NewExpression)).Return (newNewExpression);
      Expect.Call (InvokeVisitMethod ("VisitMemberBindingList", expression.Bindings)).Return (expression.Bindings);
      var result = (MemberInitExpression) InvokeAndCheckVisitExpression ("VisitMemberInitExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.MemberInit));
      Assert.That (result.NewExpression, Is.SameAs (newNewExpression));
      Assert.That (result.Bindings, Is.SameAs (expression.Bindings));
    }

    [Test]
    public void VisitMemberInitExpression_ChangedBindings ()
    {
      var expression = (MemberInitExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.MemberInit);
      ReadOnlyCollection<MemberBinding> newBindings = new List<MemberBinding>().AsReadOnly();
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.NewExpression)).Return (expression.NewExpression);
      Expect.Call (InvokeVisitMethod ("VisitMemberBindingList", expression.Bindings)).Return (newBindings);
      var result = (MemberInitExpression) InvokeAndCheckVisitExpression ("VisitMemberInitExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.MemberInit));
      Assert.That (result.Bindings, Is.SameAs (newBindings));
      Assert.That (result.NewExpression, Is.SameAs (expression.NewExpression));
    }

    [Test]
    public void VisitListInitExpression_Unchanged ()
    {
      var expression = (ListInitExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.ListInit);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.NewExpression)).Return (expression.NewExpression);
      Expect.Call (InvokeVisitMethod ("VisitElementInitList", expression.Initializers)).Return (expression.Initializers);
      var result = (ListInitExpression) InvokeAndCheckVisitExpression ("VisitListInitExpression", expression);
      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException),
        ExpectedMessage = "ListInitExpressions only support non-null instances of type 'NewExpression' as their NewExpression member.")]
    public void VisitListInitExpression_InvalidNewExpression ()
    {
      var expression = (ListInitExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.ListInit);
      Expect.Call (InvokeVisitMethod<NewExpression, ConstantExpression> ("VisitExpression", expression.NewExpression)).Return (
          Expression.Constant (0));
      Expect.Call (InvokeVisitMethod ("VisitElementInitList", expression.Initializers)).Return (expression.Initializers);
      try
      {
        InvokeAndCheckVisitExpression ("VisitListInitExpression", expression);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    public void VisitListInitExpression_ChangedNewExpression ()
    {
      var expression = (ListInitExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.ListInit);
      NewExpression newNewExpression = Expression.New (typeof (List<int>));
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.NewExpression)).Return (newNewExpression);
      Expect.Call (InvokeVisitMethod ("VisitElementInitList", expression.Initializers)).Return (expression.Initializers);
      var result = (ListInitExpression) InvokeAndCheckVisitExpression ("VisitListInitExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.ListInit));
      Assert.That (result.NewExpression, Is.SameAs (newNewExpression));
      Assert.That (result.Initializers, Is.SameAs (expression.Initializers));
    }

    [Test]
    public void VisitListInitExpression_ChangedInitializers ()
    {
      var expression = (ListInitExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.ListInit);
      ReadOnlyCollection<ElementInit> newInitializers =
          new List<ElementInit> (new[] { Expression.ElementInit (typeof (List<int>).GetMethod ("Add"), Expression.Constant (1)) }).AsReadOnly();
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.NewExpression)).Return (expression.NewExpression);
      Expect.Call (InvokeVisitMethod ("VisitElementInitList", expression.Initializers)).Return (newInitializers);
      var result = (ListInitExpression) InvokeAndCheckVisitExpression ("VisitListInitExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.NodeType, Is.EqualTo (ExpressionType.ListInit));
      Assert.That (result.Initializers, Is.SameAs (newInitializers));
      Assert.That (result.NewExpression, Is.SameAs (expression.NewExpression));
    }

    [Test]
    public void VisitElementInit_Unchanged ()
    {
      ElementInit elementInit = Expression.ElementInit (typeof (List<int>).GetMethod ("Add"), Expression.Constant (1));
      Expect.Call (InvokeVisitExpressionListMethod (elementInit.Arguments)).Return (elementInit.Arguments);

      var result = (ElementInit) InvokeAndCheckVisitObject ("VisitElementInit", elementInit);
      Assert.That (result, Is.SameAs (elementInit));
    }

    [Test]
    public void VisitElementInit_Changed ()
    {
      ElementInit elementInit = Expression.ElementInit (typeof (List<int>).GetMethod ("Add"), Expression.Constant (1));
      ReadOnlyCollection<Expression> newArguments = new List<Expression> (new Expression[] { Expression.Constant (1) }).AsReadOnly();
      Expect.Call (InvokeVisitExpressionListMethod (elementInit.Arguments)).Return (newArguments);

      var result = (ElementInit) InvokeAndCheckVisitObject ("VisitElementInit", elementInit);
      Assert.That (result, Is.Not.SameAs (elementInit));
      Assert.That (result.AddMethod, Is.SameAs (elementInit.AddMethod));
      Assert.That (result.Arguments, Is.SameAs (newArguments));
    }

    [Test]
    public void VisitMemberBinding_Delegation_MemberAssignment ()
    {
      MemberAssignment memberAssignment = Expression.Bind (typeof (SimpleClass).GetField ("Value"), Expression.Constant ("test"));

      Expect.Call (InvokeVisitMethod ("VisitMemberBinding", memberAssignment)).CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      Expect.Call (InvokeVisitMethod ("VisitMemberAssignment", memberAssignment)).Return (memberAssignment);

      MockRepository.ReplayAll();
      object result = InvokeVisitMethod ("VisitMemberBinding", memberAssignment);
      MockRepository.VerifyAll();

      Assert.That (result, Is.SameAs (memberAssignment));
    }

    [Test]
    public void VisitMemberBinding_Delegation_MemberBinding ()
    {
      MemberMemberBinding memberMemberBinding = Expression.MemberBind (typeof (SimpleClass).GetField ("Value"), new List<MemberBinding>());

      Expect.Call (InvokeVisitMethod ("VisitMemberBinding", memberMemberBinding)).CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (InvokeVisitMethod ("VisitMemberMemberBinding", memberMemberBinding)).Return (memberMemberBinding);

      MockRepository.ReplayAll();
      object result = InvokeVisitMethod ("VisitMemberBinding", memberMemberBinding);
      MockRepository.VerifyAll();

      Assert.That (result, Is.SameAs (memberMemberBinding));
    }

    [Test]
    public void VisitMemberBinding_Delegation_ListBinding ()
    {
      MemberListBinding memberListBinding =
          Expression.ListBind (typeof (SimpleClass).GetField ("Value"), new ElementInit[] { });

      Expect.Call (InvokeVisitMethod ("VisitMemberBinding", memberListBinding)).CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (InvokeVisitMethod ("VisitMemberListBinding", memberListBinding)).Return (memberListBinding);

      MockRepository.ReplayAll();
      object result = InvokeVisitMethod ("VisitMemberBinding", memberListBinding);
      MockRepository.VerifyAll();

      Assert.That (result, Is.SameAs (memberListBinding));
    }

    [Test]
    public void VisitMemberAssignment_Unchanged ()
    {
      MemberAssignment memberAssignment = Expression.Bind (typeof (SimpleClass).GetField ("Value"), Expression.Constant ("1"));
      Expect.Call (InvokeVisitMethod ("VisitExpression", memberAssignment.Expression)).Return (memberAssignment.Expression);
      var result = (MemberAssignment) InvokeAndCheckVisitObject ("VisitMemberAssignment", memberAssignment);
      Assert.That (result, Is.SameAs (memberAssignment));
    }

    [Test]
    public void VisitMemberAssignment_Changed ()
    {
      MemberAssignment memberAssignment = Expression.Bind (typeof (SimpleClass).GetField ("Value"), Expression.Constant ("1"));
      MemberAssignment newMemberAssignment = Expression.Bind (typeof (SimpleClass).GetField ("Value"), Expression.Constant ("2"));

      Expect.Call (InvokeVisitMethod ("VisitExpression", memberAssignment.Expression)).Return (newMemberAssignment.Expression);

      var result = (MemberAssignment) InvokeAndCheckVisitObject ("VisitMemberAssignment", memberAssignment);
      Assert.That (result, Is.Not.SameAs (memberAssignment));
    }

    [Test]
    public void VisitMemberMemberBinding_Unchanged ()
    {
      MemberMemberBinding memberMemberBinding = Expression.MemberBind (typeof (SimpleClass).GetField ("Value"), new List<MemberBinding>());
      Expect.Call (InvokeVisitMethod ("VisitMemberBindingList", memberMemberBinding.Bindings)).Return (memberMemberBinding.Bindings);
      var result = (MemberMemberBinding) InvokeAndCheckVisitObject ("VisitMemberMemberBinding", memberMemberBinding);
      Assert.That (result, Is.SameAs (memberMemberBinding));
    }

    [Test]
    public void VisitMemberMemberBinding_Changed ()
    {
      MemberMemberBinding memberMemberBinding = Expression.MemberBind (typeof (SimpleClass).GetField ("Value"), new List<MemberBinding>());
      ReadOnlyCollection<MemberBinding> newBindings = new List<MemberBinding>().AsReadOnly();
      Expect.Call (InvokeVisitMethod ("VisitMemberBindingList", memberMemberBinding.Bindings)).Return (newBindings);
      var result = (MemberMemberBinding) InvokeAndCheckVisitObject ("VisitMemberMemberBinding", memberMemberBinding);
      Assert.That (result, Is.Not.SameAs (memberMemberBinding));
      Assert.That (result.Bindings, Is.SameAs (newBindings));
      Assert.That (result.BindingType, Is.EqualTo (memberMemberBinding.BindingType));
      Assert.That (result.Member, Is.EqualTo (memberMemberBinding.Member));
    }

    [Test]
    public void VisitMemberListBinding_Unchanged ()
    {
      MemberListBinding memberListBinding =
          Expression.ListBind (typeof (SimpleClass).GetField ("Value"), new ElementInit[] { });
      Expect.Call (InvokeVisitMethod ("VisitElementInitList", memberListBinding.Initializers)).Return (memberListBinding.Initializers);
      var result = (MemberListBinding) InvokeAndCheckVisitObject ("VisitMemberListBinding", memberListBinding);
      Assert.That (result, Is.SameAs (memberListBinding));
    }

    [Test]
    public void VisitMemberListBinding_Changed ()
    {
      MemberListBinding memberListBinding =
          Expression.ListBind (typeof (SimpleClass).GetField ("Value"), new ElementInit[] { });
      ReadOnlyCollection<ElementInit> newInitializers = new List<ElementInit>().AsReadOnly();
      Expect.Call (InvokeVisitMethod ("VisitElementInitList", memberListBinding.Initializers)).Return (newInitializers);
      var result = (MemberListBinding) InvokeAndCheckVisitObject ("VisitMemberListBinding", memberListBinding);
      Assert.That (result, Is.Not.SameAs (memberListBinding));
      Assert.That (result.Initializers, Is.SameAs (newInitializers));
      Assert.That (result.BindingType, Is.EqualTo (memberListBinding.BindingType));
      Assert.That (result.Member, Is.EqualTo (memberListBinding.Member));
    }

    [Test]
    public void VisitExpressionList_Unchanged ()
    {
      Expression expr1 = Expression.Constant (1);
      Expression expr2 = Expression.Constant (2);
      ReadOnlyCollection<Expression> expressions = new List<Expression> (new[] { expr1, expr2 }).AsReadOnly();
      Expect.Call (InvokeVisitMethod ("VisitExpression", expr1)).Return (expr1);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expr2)).Return (expr2);
      ReadOnlyCollection<Expression> result = InvokeAndCheckVisitExpressionList (expressions);
      Assert.That (result, Is.SameAs (expressions));
    }

    [Test]
    public void VisitExpressionList_Changed ()
    {
      Expression expr1 = Expression.Constant (1);
      Expression expr2 = Expression.Constant (2);
      Expression expr3 = Expression.Constant (3);
      Expression newExpression = Expression.Constant (4);
      ReadOnlyCollection<Expression> expressions = new List<Expression> (new[] { expr1, expr2, expr3 }).AsReadOnly();
      Expect.Call (InvokeVisitMethod ("VisitExpression", expr1)).Return (expr1);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expr2)).Return (newExpression);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expr3)).Return (expr3);
      ReadOnlyCollection<Expression> result = InvokeAndCheckVisitExpressionList (expressions);
      Assert.That (result, Is.Not.SameAs (expressions));
      Assert.That (result, Is.EqualTo (new object[] { expr1, newExpression, expr3 }));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException),
        ExpectedMessage = "The current list only supports objects of type 'ConstantExpression' as its elements.")]
    public void VisitExpressionList_Changed_InvalidType ()
    {
      ConstantExpression expr1 = Expression.Constant (1);
      ConstantExpression expr2 = Expression.Constant (2);
      ConstantExpression expr3 = Expression.Constant (3);
      ParameterExpression newExpression = Expression.Parameter (typeof (int), "a");

      ReadOnlyCollection<ConstantExpression> expressions = new List<ConstantExpression> (new[] { expr1, expr2, expr3 }).AsReadOnly();
      Expect.Call (InvokeVisitMethod ("VisitExpression", expr1)).Return (expr1);
      Expect.Call (InvokeVisitMethod<ConstantExpression, ParameterExpression> ("VisitExpression", expr2)).Return (newExpression);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expr3)).Return (expr3);

      try
      {
        InvokeAndCheckVisitExpressionList (expressions);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    public void VisitMemberBindingList_Unchanged ()
    {
      MemberBinding memberBinding1 = Expression.Bind (typeof (SimpleClass).GetField ("Value"), Expression.Constant ("0"));
      MemberBinding memberBinding2 = Expression.Bind (typeof (SimpleClass).GetField ("Value"), Expression.Constant ("1"));
      ReadOnlyCollection<MemberBinding> memberBindings = new List<MemberBinding> (new[] { memberBinding1, memberBinding2 }).AsReadOnly();
      Expect.Call (InvokeVisitMethod ("VisitMemberBinding", memberBinding1)).Return (memberBinding1);
      Expect.Call (InvokeVisitMethod ("VisitMemberBinding", memberBinding2)).Return (memberBinding2);
      ReadOnlyCollection<MemberBinding> result = InvokeAndCheckVisitMemberBindingList (memberBindings);
      Assert.That (result, Is.SameAs (memberBindings));
    }

    [Test]
    public void VisitMemberBindingList_Changed ()
    {
      MemberBinding memberBinding1 = Expression.Bind (typeof (SimpleClass).GetField ("Value"), Expression.Constant ("0"));
      MemberBinding memberBinding2 = Expression.Bind (typeof (SimpleClass).GetField ("Value"), Expression.Constant ("1"));
      MemberBinding memberBinding3 = Expression.Bind (typeof (SimpleClass).GetField ("Value"), Expression.Constant ("2"));
      MemberBinding newMemberBinding = Expression.Bind (typeof (SimpleClass).GetField ("Value"), Expression.Constant ("3"));
      ReadOnlyCollection<MemberBinding> memberBindings =
          new List<MemberBinding> (new[] { memberBinding1, memberBinding2, memberBinding3 }).AsReadOnly();
      Expect.Call (InvokeVisitMethod ("VisitMemberBinding", memberBinding1)).Return (memberBinding1);
      Expect.Call (InvokeVisitMethod ("VisitMemberBinding", memberBinding2)).Return (newMemberBinding);
      Expect.Call (InvokeVisitMethod ("VisitMemberBinding", memberBinding3)).Return (memberBinding3);
      ReadOnlyCollection<MemberBinding> result = InvokeAndCheckVisitMemberBindingList (memberBindings);
      Assert.That (result, Is.Not.SameAs (memberBindings));
      Assert.That (result, Is.EqualTo (new object[] { memberBinding1, newMemberBinding, memberBinding3 }));
    }

    [Test]
    public void VisitElementInitList_Unchanged ()
    {
      ElementInit elementInit1 = Expression.ElementInit (typeof (List<int>).GetMethod ("Add"), Expression.Constant (0));
      ElementInit elementInit2 = Expression.ElementInit (typeof (List<int>).GetMethod ("Add"), Expression.Constant (1));
      ReadOnlyCollection<ElementInit> elementInits = new List<ElementInit> (new[] { elementInit1, elementInit2 }).AsReadOnly();
      Expect.Call (InvokeVisitMethod ("VisitElementInit", elementInit1)).Return (elementInit1);
      Expect.Call (InvokeVisitMethod ("VisitElementInit", elementInit2)).Return (elementInit2);
      ReadOnlyCollection<ElementInit> result = InvokeAndCheckVisitElementInitList (elementInits);
      Assert.That (result, Is.SameAs (elementInits));
    }

    [Test]
    public void VisitElementInitList_Changed ()
    {
      ElementInit elementInit1 = Expression.ElementInit (typeof (List<int>).GetMethod ("Add"), Expression.Constant (0));
      ElementInit elementInit2 = Expression.ElementInit (typeof (List<int>).GetMethod ("Add"), Expression.Constant (1));
      ElementInit elementInit3 = Expression.ElementInit (typeof (List<int>).GetMethod ("Add"), Expression.Constant (2));
      ElementInit newElementInit = Expression.ElementInit (typeof (List<int>).GetMethod ("Add"), Expression.Constant (3));
      ReadOnlyCollection<ElementInit> elementInits = new List<ElementInit> (new[] { elementInit1, elementInit2, elementInit3 }).AsReadOnly();
      Expect.Call (InvokeVisitMethod ("VisitElementInit", elementInit1)).Return (elementInit1);
      Expect.Call (InvokeVisitMethod ("VisitElementInit", elementInit2)).Return (newElementInit);
      Expect.Call (InvokeVisitMethod ("VisitElementInit", elementInit3)).Return (elementInit3);
      ReadOnlyCollection<ElementInit> result = InvokeAndCheckVisitElementInitList (elementInits);
      Assert.That (result, Is.Not.SameAs (elementInits));
      Assert.That (result, Is.EqualTo (new object[] { elementInit1, newElementInit, elementInit3 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Expression type -1 is not supported.", MatchType = MessageMatch.Contains)]
    public void VisitUnknownExpression ()
    {
      var expressionNode = new SpecialExpressionNode ((ExpressionType) (-1), typeof (int));
      Expect.Call (InvokeVisitMethod ("VisitUnknownExpression", expressionNode)).CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      MockRepository.ReplayAll();

      try
      {
        InvokeVisitMethod ("VisitUnknownExpression", expressionNode);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }
  }
}