//pwsh bin/Debug/net8.0/playwright.ps1 codegen http://localhost:5273/

using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightTests;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class EndToEndTests : PageTest
{
    private Process? _serverProcess;

    private IBrowser? _browser;

    [SetUp]
    public async Task Init()
    {
        // initilazies our web application on localhost:5273
        _serverProcess = await MyEndToEndUtil.StartServer();

        //Opens a chromiun browser for testing - each test runs an isolated browser
        _browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
    }


    [TearDown] // This will run after all tests in the class
    public async Task Clean()
    {

        // Kill the processes(our localhost server and the browser for testing)
        _serverProcess?.Kill(true);
        _serverProcess?.Dispose();
        await _browser.DisposeAsync();



    }


    [Test]
    public async Task HomepageDoesNotHaveChirpboxWhenLoggedOut()
    {
        // go to homepage
        await Page.GotoAsync("http://localhost:5273/");
        // see if share button is visible
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Share" })).Not.ToBeVisibleAsync();
    }

    [Test]
    public async Task UserCanLogInThroughUI()
    {
        // go to homepage and login using a testuser
        await Page.GotoAsync("http://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Hello_1");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        // see if 'whats on your mind' heading now appears after login
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "What's on your mind test@mail" })).ToBeVisibleAsync();

    }
    [Test]
    public async Task HomepageDoesHaveChirpboxWhenLoggedIn()
    {
        // Log in using a test user
        await Page.GotoAsync("http://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Hello_1");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GotoAsync("http://localhost:5273/");
        // see if share button is now visible after login
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Share" })).ToBeVisibleAsync();
    }


}
