//pwsh bin/Debug/net8.0/playwright.ps1 codegen https://localhost:5001

using System.Diagnostics;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;


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
        // initilazies our web application on https://localhost:5001
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
        await _browser!.DisposeAsync();

        // _context.Cheeps.RemoveRange(_context.Cheeps.Where(c => c.Text.Contains("Hello i am test")));
        // await _context.SaveChangesAsync();

    }


    [Test]
    public async Task HomepageDoesNotHaveChirpboxWhenLoggedOut()
    {
        // go to homepage
        await Page.GotoAsync("http://localhost:5000/");
        // see if share button is visible
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Share" })).Not.ToBeVisibleAsync();
    }

    [Test]
    public async Task UserCanLogInThroughUI()
    {
        // go to homepage and login using a testuser
        await Page.GotoAsync("http://localhost:5000/");
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
        await Page.GotoAsync("http://localhost:5000/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Hello_1");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GotoAsync("https://localhost:5001");
        // see if share button is now visible after login
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Share" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task UsersCannotEnterCheepsLongerThan160Characters()
    {
        var CheepLongerthan160 = "Planning an amazing event takes effort, but teamwork and creativity make it unforgettable. Lets aim for success and celebrate together. Join us soon—excited! Also you should cut me off soon";
        await Page.GotoAsync("http://localhost:5000/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Hello_1");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.Locator("#cheepTextInput").ClickAsync();
        await Page.Locator("#cheepTextInput").FillAsync(CheepLongerthan160);
        string actual = await Page.Locator("#cheepTextInput").InputValueAsync();

        Assert.That(actual.Length, Is.EqualTo(160));

    }

    [Test]
    public async Task WhenUsersSendCheepsItsAddedToDb()
    {

        await Page.GotoAsync("http://localhost:5000/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Hello_1");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Page.Locator("#cheepTextInput").ClickAsync();
        await Page.Locator("#cheepTextInput").FillAsync("Hello i am test");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(Page.GetByText("Hello i am test", new() { Exact = true }).Nth(0)).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "logout [test@mail.dk]" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "logout [test@mail.dk]" })).Not.ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "login" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Hello i am test", new() { Exact = true }).Nth(0)).ToBeVisibleAsync();

    }


}
