using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using PaymentAPI.Models;
using Xunit;

namespace PaymentAPI.Tests
{
    public class PaymentsControllerTests : IClassFixture<WebApplicationFactory<PaymentAPI.Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<PaymentAPI.Program> _factory;

        public PaymentsControllerTests(WebApplicationFactory<PaymentAPI.Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetPayments_ReturnsOkResult()
        {
            // Act
            var response = await _client.GetAsync("/api/payments");

            // Assert
            response.EnsureSuccessStatusCode(); // Verifica se o código de status é 2xx
            var payments = await response.Content.ReadFromJsonAsync<List<Payment>>();
            payments.Should().NotBeNull();
            payments.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetPayment_ReturnsNotFound_WhenPaymentDoesNotExist()
        {
            // Act
            var response = await _client.GetAsync("/api/payments/999"); // Id não existente

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreatePayment_ReturnsCreatedAtAction()
        {
            // Arrange
            var payment = new Payment
            {
                Amount = 100.50m,
                Currency = "USD",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/payments", payment);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            var createdPayment = await response.Content.ReadFromJsonAsync<Payment>();
            createdPayment.Should().NotBeNull();
            createdPayment.Amount.Should().Be(payment.Amount);
            createdPayment.Currency.Should().Be(payment.Currency);
            createdPayment.Status.Should().Be(payment.Status);
            createdPayment.CreatedAt.Should().BeCloseTo(payment.CreatedAt, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task UpdatePayment_ReturnsNoContent()
        {
            // Arrange
            var payment = new Payment
            {
                Amount = 100.50m,
                Currency = "USD",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            // Act - Create a new payment
            var createResponse = await _client.PostAsJsonAsync("/api/payments", payment);
            createResponse.EnsureSuccessStatusCode();
            var createdPayment = await createResponse.Content.ReadFromJsonAsync<Payment>();

            // Update the payment
            createdPayment.Amount = 150.00m;
            createdPayment.Status = "Completed";

            // Act - Update the existing payment
            var response = await _client.PutAsJsonAsync($"/api/payments/{createdPayment.Id}", createdPayment);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeletePayment_ReturnsNoContent()
        {
            // Arrange: Certifique-se de que o pagamento com ID 1 existe no banco

            // Act
            var response = await _client.DeleteAsync("/api/payments/1");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }
    }
}
