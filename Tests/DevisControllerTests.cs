
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using webAPIBrasserie.Controllers;
using webAPIBrasserie.Models;
using Xunit;
using static webAPIBrasserie.Controllers.DevisController;

public class DevisControllerTests
{
    [Fact]
    public void DemanderDevis_ReturnsOkResult()
    {
        // Arrange
        var mockContext = new Mock<BrasserieDBContext>(); 
        var controller = new DevisController(mockContext.Object);

        var demandeDevis = new DemandeDevisModel
        {
            GrossisteId = 1,
            Bieres = new List<Biere>
            {
                new Biere { Id = 1, Nom = "Biere1", Quantite = 5, Prix = 2.5 },
                new Biere { Id = 2, Nom = "Biere2", Quantite = 10, Prix = 3.00 }
            }
        };

        // Act
        var result = controller.DemanderDevis(demandeDevis);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

   
}
