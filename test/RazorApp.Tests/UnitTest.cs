using Chirp.Razor;
using Xunit;

namespace Razor.App.Tests
{

    public class UnitTest1
    {
        [Fact]
        public void FromUnixTimeToDateTime_ConvertsCorrectly()
        {
            // Arrange
            double unixTime = 1728383396; //Unixtimestamp for: 12:29:50 08-10-2024

            //Act 
            string actualDateTime = DbFacade.UnixTimeStampToDateTimeString(unixTime);

            // Assert
            Assert.Equal("08/10/24 10:29:56", actualDateTime);
        }
    }

}

