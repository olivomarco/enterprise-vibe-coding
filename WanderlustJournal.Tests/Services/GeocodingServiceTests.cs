using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using WanderlustJournal.Services;
using Xunit;

namespace WanderlustJournal.Tests.Services
{
    public class GeocodingServiceTests
    {
        [Fact]
        public async Task GeocodeLocationAsync_ValidLocation_ReturnsCoordinates()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new[]
                    {
                        new 
                        { 
                            lat = "48.8584", 
                            lon = "2.2945",
                            display_name = "Paris, Île-de-France, France"
                        }
                    }))
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var loggerMock = new Mock<ILogger<GeocodingService>>();
            var geocodingService = new GeocodingService(httpClientFactoryMock.Object, loggerMock.Object);

            // Act
            var (latitude, longitude) = await geocodingService.GeocodeLocationAsync("Paris, France");

            // Assert
            Assert.Equal(48.8584m, latitude);
            Assert.Equal(2.2945m, longitude);
            mockHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GeocodeLocationAsync_EmptyLocation_ReturnsNull()
        {
            // Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var loggerMock = new Mock<ILogger<GeocodingService>>();
            var geocodingService = new GeocodingService(httpClientFactoryMock.Object, loggerMock.Object);

            // Act
            var (latitude, longitude) = await geocodingService.GeocodeLocationAsync("");

            // Assert
            Assert.Null(latitude);
            Assert.Null(longitude);
            httpClientFactoryMock.Verify(f => f.CreateClient(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GeocodeLocationAsync_ApiError_ReturnsNull()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var loggerMock = new Mock<ILogger<GeocodingService>>();
            var geocodingService = new GeocodingService(httpClientFactoryMock.Object, loggerMock.Object);

            // Act
            var (latitude, longitude) = await geocodingService.GeocodeLocationAsync("Invalid Location");

            // Assert
            Assert.Null(latitude);
            Assert.Null(longitude);
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task GeocodeLocationAsync_EmptyResponse_ReturnsNull()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[]")
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var loggerMock = new Mock<ILogger<GeocodingService>>();
            var geocodingService = new GeocodingService(httpClientFactoryMock.Object, loggerMock.Object);

            // Act
            var (latitude, longitude) = await geocodingService.GeocodeLocationAsync("Unknown Location");

            // Assert
            Assert.Null(latitude);
            Assert.Null(longitude);
        }
    }
}