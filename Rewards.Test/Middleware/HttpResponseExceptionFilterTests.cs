using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace Rewards.Middleware.Test
{
    public class HttpResponseExceptionFilterTests
    {
        [Fact]
        public void ShouldFilterHttpResponseException()
        {
            // Arrange
            var filter = new HttpResponseExceptionFilter();
            var context = FakeContext(new HttpResponseException(400, "Test error"));
            var expectedResult = new ObjectResult(new { Error = new { Message = "Test error" } }) { StatusCode = 400 };
            // Act
            filter.OnActionExecuted(context);
            // Assert
            Assert.True(context.ExceptionHandled);
            AreEqualByJson(expectedResult, context.Result);
        }

        [Fact]
        public void ShouldNotFilterNormalException()
        {
            // Arrange
            var filter = new HttpResponseExceptionFilter();
            var context = FakeContext(new Exception("Test error"));
            // Act
            filter.OnActionExecuted(context);
            // Assert
            Assert.False(context.ExceptionHandled);
            Assert.Null(context.Result);
        }

        [Fact]
        public void ShouldIgnoreNonException()
        {
            // Arrange
            var filter = new HttpResponseExceptionFilter();
            var context = FakeContext(null);
            // Act
            filter.OnActionExecuted(context);
            // Assert
            Assert.False(context.ExceptionHandled);
            Assert.Null(context.Result);
        }

        private static void AreEqualByJson(object expected, object actual)
        {
            var expectedJson = JsonSerializer.Serialize(expected);
            var actualJson = JsonSerializer.Serialize(actual);
            Assert.Equal(expectedJson, actualJson);
        }

        private static ActionExecutedContext FakeContext(Exception exception) => new ActionExecutedContext(
            new ActionContext() 
            {
                HttpContext = new Mock<HttpContext>().Object,
                RouteData = new Mock<RouteData>().Object,
                ActionDescriptor = new Mock<ActionDescriptor>().Object
            },
            new List<IFilterMetadata>(),
            new object())
        {
            Exception = exception
        };
    }
}
