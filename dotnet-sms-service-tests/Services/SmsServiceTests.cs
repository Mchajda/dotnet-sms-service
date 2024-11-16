using dotnet_sms_service.Services;
using Moq;
using Moq.AutoMock;

namespace dotnet_sms_service_tests.Services
{
    [TestClass]
    public class SmsServiceTests
    {
        private readonly AutoMocker _autoMocker = new(MockBehavior.Strict);
        private Mock<ISmsService> _smsServiceMock;

        [TestInitialize]
        public void Setup()
        {
            _smsServiceMock = new Mock<ISmsService>();
        }

        [TestMethod]
        [DataRow("660093822", "48660093822")]
        [DataRow("698 995 625", "48698995625")]
        [DataRow("881616550", "48881616550")]
        public void ParsePhoneNumber_WithDifferentPhoneNumbers_ReturnsParsedPhoneNumbers(string inputPhoneNumber, string expectedParsedPhoneNumber)
        {
            // Arrange
            _smsServiceMock
                .Setup(service => service.ParsePhoneNumber(inputPhoneNumber))
                .Returns(expectedParsedPhoneNumber);

            // Act
            var result = _smsServiceMock.Object.ParsePhoneNumber(inputPhoneNumber);

            // Assert
            Assert.AreEqual(expectedParsedPhoneNumber, result);
        }
    }
}
