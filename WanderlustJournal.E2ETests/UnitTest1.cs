namespace WanderlustJournal.E2ETests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text.RegularExpressions;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1_VerifyTestingInfrastructure()
    {
        // Basic test to verify the MSTest framework is working
        Assert.IsTrue(true, "This test should always pass");
    }
    
    [TestMethod]
    public void TestFileStructure_ShouldHaveRequiredComponents()
    {
        // This test validates that our testing infrastructure has the expected structure
        // without requiring Playwright browsers to be installed

        // Verify Pages directory exists for the Page Object Model
        string pagesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Pages");
        Assert.IsTrue(Directory.Exists(pagesDirectory), "Pages directory should exist for Page Object Model");

        // Verify Tests directory exists
        string testsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Tests");
        Assert.IsTrue(Directory.Exists(testsDirectory), "Tests directory should exist");

        // Verify expected test files exist
        string[] expectedTestFiles = {
            "NavigationTests.cs",
            "JournalEntryTests.cs",
            "FormValidationTests.cs"
        };

        foreach (var file in expectedTestFiles)
        {
            string filePath = Path.Combine(testsDirectory, file);
            Assert.IsTrue(File.Exists(filePath), $"Expected test file {file} should exist");
        }
    }
}