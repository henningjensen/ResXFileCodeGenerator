using System.Globalization;
using FluentAssertions;
using Xunit;

namespace VocaDb.ResXFileCodeGenerator.Tests.IntegrationTests;

public class TestResxFiles
{
	[Fact]
	public void TestNormalResourceGen()
	{
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("da");
		Test1.CreateDate.Should().Be("OldestDa");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
		Test1.CreateDate.Should().Be("Oldest");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("ch");
		Test1.CreateDate.Should().Be("Oldest");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
		Test1.CreateDate.Should().Be("OldestEnUs");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("da-DK");
		Test1.CreateDate.Should().Be("OldestDaDK");
	}
	[Fact]
	public void TestCodeGenResourceGenWithGivenCultureInfo()
	{

		new Test2(new CultureInfo("da")).CreateDate.Should().Be("OldestDa");
		new Test2(new CultureInfo("en")).CreateDate.Should().Be("Oldest");
		new Test2(new CultureInfo("ch")).CreateDate.Should().Be("Oldest");
		new Test2(new CultureInfo("en-us")).CreateDate.Should().Be("OldestEnUs");
		new Test2(new CultureInfo("da-DK")).CreateDate.Should().Be("OldestDaDK");
	}

}
