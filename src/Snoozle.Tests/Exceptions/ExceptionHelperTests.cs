using NUnit.Framework;
using Snoozle.Exceptions;
using System;

namespace Snoozle.Tests.Exceptions
{
    [TestFixture]
    public class ExceptionHelperTests
    {
        #region ArgumentNull.ThrowIfNecessary

        [Test]
        public void ArgumentNullThrowIfNecessary_NullValue_ThrowsArgumentNullException()
        {
            // Arrange
            string obj = null;
            TestDelegate del = () => ExceptionHelper.ArgumentNull.ThrowIfNecessary(obj, string.Empty);

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(del);
        }

        [Test]
        public void ArgumentNullThrowIfNecessary_NonNullValue_DoesNotThrowArgumentNullException()
        {
            // Arrange
            string obj = string.Empty;
            TestDelegate del = () => ExceptionHelper.ArgumentNull.ThrowIfNecessary(obj, string.Empty);

            // Act
            // Assert
            Assert.DoesNotThrow(del);
        }

        #endregion
    }
}
