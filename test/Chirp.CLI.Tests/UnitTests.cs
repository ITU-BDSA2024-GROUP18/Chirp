using System;
using Xunit;
using Chirp.CLI;

namespace Chirp.CLI.Tests
{
    public class ConversionTests
    {
        [Fact]
        public void FromDateTimeToUnix_ConvertsCorrectly()
        {
            // Arrange
            string testDateTime = "09/17/24 12:00:00";
            DateTime expectedDateTime = DateTime.ParseExact(testDateTime, "MM/dd/yy HH:mm:ss", null);
            long expectedUnixTime = new DateTimeOffset(expectedDateTime).ToUnixTimeSeconds();

            // Act
            long unixTime = FromDateTimeToUnix(testDateTime);

            // Assert
            Assert.Equal(expectedUnixTime, unixTime);
        }

        private long FromDateTimeToUnix(string dateTimeStamp)
        {
            DateTime parsedTime = DateTime.Parse(dateTimeStamp, System.Globalization.CultureInfo.InvariantCulture);
            return new DateTimeOffset(parsedTime).ToUnixTimeSeconds();
        }

        [Fact]
        public void FromUnixTimeToDateTime_ConvertsCorrectly()
        {
            // Arrange
            long unixTime = 1726555200; // Unix timestamp for 09/17/24 12:00:00 UTC
            DateTimeOffset expectedDateTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToLocalTime(); // Adjust expected time to local time zone
            string expectedDateTimeString = expectedDateTime.ToString("MM/dd/yy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            // Act
            string actualDateTime = UserInterface.FromUnixTimeToDateTime(unixTime);

            // Assert
            Assert.Equal(expectedDateTimeString, actualDateTime);
        }
    }
}