// VenteControllerTests.cs
using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using webAPIBrasserie.Controllers;
using webAPIBrasserie.Models;
using Xunit;

public class VenteControllerTests
{
    [Fact]
    public void AjouterVente_ValidData_ReturnsOkResult()
    {
        // Arrange
        var mockContext = new Mock<BrasserieDBContext>();
        var controller = new VentesController(mockContext.Object);

        var venteModel = new Vente
        {
            BiereId = 1,
            GrossisteId = 2,
            Qtevendue = 10
        };

        // Act
        var result = controller.AjouterVente(venteModel) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Vente ajoutée avec succès", result.Value);
    }

    
}