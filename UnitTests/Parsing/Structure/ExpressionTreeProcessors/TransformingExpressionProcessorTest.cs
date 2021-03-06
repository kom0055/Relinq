// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.Structure.ExpressionTreeProcessors;
using Rhino.Mocks;

namespace Remotion.Linq.UnitTests.Parsing.Structure.ExpressionTreeProcessors
{
  [TestFixture]
  public class TransformingExpressionProcessorTest
  {
    [Test]
    public void Process ()
    {
      var inputExpression = Expression.Constant (1);
      var transformedExpression = Expression.Constant (2);
      var transformationProviderMock = MockRepository.GenerateStrictMock<IExpressionTranformationProvider>();
      transformationProviderMock
          .Expect (mock => mock.GetTransformations (inputExpression))
          .Return (new ExpressionTransformation[] { expr => transformedExpression });
      transformationProviderMock
          .Expect (mock => mock.GetTransformations (transformedExpression))
          .Return (new ExpressionTransformation[0]);
      transformationProviderMock.Replay();
      
      var processor = new TransformingExpressionTreeProcessor (transformationProviderMock);

      var result = processor.Process (inputExpression);

      transformationProviderMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (transformedExpression));
    }
  }
}