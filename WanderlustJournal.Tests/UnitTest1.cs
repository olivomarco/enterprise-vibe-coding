namespace WanderlustJournal.Tests;

using WanderlustJournal.Models;
using System;
using Xunit;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

public class UnitTest1
{
    [Fact]
    public void JournalEntry_ValidationRequirements()
    {
        // Arrange
        var entry = new JournalEntry
        {
            // Not setting required fields to test validation
            Notes = "Some notes about the journey",
            ImageFileName = "photo.jpg"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(entry);
        var isValid = Validator.TryValidateObject(
            entry, 
            validationContext, 
            validationResults, 
            true);

        // Assert
        Assert.False(isValid);
        Assert.Equal(2, validationResults.Count); // Should have 2 validation errors
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Location"));
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Title"));
        // DateVisited has a default value, so it won't show validation error
    }

    [Fact]
    public void JournalEntry_WhenValid_PassesValidation()
    {
        // Arrange
        var entry = new JournalEntry
        {
            Location = "Paris, France",
            Title = "Eiffel Tower Visit",
            DateVisited = DateTime.Now.AddDays(-10),
            Notes = "Amazing views from the top!"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(entry);
        var isValid = Validator.TryValidateObject(
            entry, 
            validationContext, 
            validationResults, 
            true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }
}