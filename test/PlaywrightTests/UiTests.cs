//IN WSL: pwsh bin/Debug/net8.0/playwright.ps1 codegen https://localhost:5001

//pwsh bin/Debug/net8.0/playwright.ps1 codegen --ignore-https-errors  https://localhost:5001

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

    private IBrowserContext? _context;

    private IPage? _page;

    [SetUp]
    public async Task Init()
    {
        // initilazies our web application on https://localhost:5001
        _serverProcess = await MyEndToEndUtil.StartServer();


        //Opens a chromiun browser for testing - each test runs an isolated browser
        _browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });

        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true

        });

        _page = await _context.NewPageAsync();

    }


    [TearDown] // This will run after all tests in the class
    public async Task Clean()
    {

        // Kill the processes(our localhost server and the browser for testing)

        await _context!.DisposeAsync();
        _serverProcess?.Kill(true);
        _serverProcess?.Dispose();
        await _browser!.DisposeAsync();

        // _context.Cheeps.RemoveRange(_context.Cheeps.Where(c => c.Text.Contains("Hello i am test")));
        // await _context.SaveChangesAsync();

    }


    [Test, Order(1)]
    public async Task HomepageDoesNotHaveChirpboxWhenLoggedOut()
    {
        // go to homepage
        await _page!.GotoAsync("https://localhost:5001");
        // see if share button is visible
        await Expect(_page!.GetByRole(AriaRole.Button, new() { Name = "Share" })).Not.ToBeVisibleAsync();
    }

    [Test, Order(2)]
    public async Task RegisterTestUserThroughRegisterPage()
    {

        await _page!.GotoAsync("https://localhost:5001");
        await _page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await _page.GetByPlaceholder("Username").ClickAsync();
        await _page.GetByPlaceholder("Username").FillAsync("testuser");
        await _page.GetByPlaceholder("name@example.com").ClickAsync();
        await _page.GetByPlaceholder("name@example.com").FillAsync("test@mail.dk");
        await _page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await _page.GetByLabel("Password", new() { Exact = true }).FillAsync("Hello_1");
        await _page.GetByLabel("Confirm Password").ClickAsync();
        await _page.GetByLabel("Confirm Password").FillAsync("Hello_1");
        await _page.GetByRole(AriaRole.Button, new() { Name = "Register", Exact = true }).ClickAsync();
        await Expect(_page.GetByRole(AriaRole.Heading, new() { Name = "What's on your mind testuser" })).ToBeVisibleAsync();
        await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "logout [testuser]" })).ToBeVisibleAsync();
        await _page.GetByRole(AriaRole.Link, new() { Name = "logout [testuser]" }).ClickAsync();

    }

    [Test, Order(3)]
    public async Task UserCanLogInThroughUI()
    {
        // go to homepage and login using a testuser
        await _page!.GotoAsync("https://localhost:5001");
        await _page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await _page.GetByPlaceholder("username").ClickAsync();
        await _page.GetByPlaceholder("username").FillAsync("testuser");
        await _page.GetByPlaceholder("password").ClickAsync();
        await _page.GetByPlaceholder("password").FillAsync("Hello_1");
        await _page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        // see if 'whats on your mind' heading now appears after login
        await Expect(_page.GetByRole(AriaRole.Heading, new() { Name = "What's on your mind testuser" })).ToBeVisibleAsync();

        await _page.GetByRole(AriaRole.Link, new() { Name = "logout [testuser]" }).ClickAsync();


    }
    [Test, Order(4)]
    public async Task HomepageDoesHaveChirpboxWhenLoggedIn()
    {
        // Log in using a test user
        await _page!.GotoAsync("https://localhost:5001");
        await _page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await _page.GetByPlaceholder("username").ClickAsync();
        await _page.GetByPlaceholder("username").FillAsync("testuser");
        await _page.GetByPlaceholder("password").ClickAsync();
        await _page.GetByPlaceholder("password").FillAsync("Hello_1");
        await _page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        // see if share button is now visible after login
        await Expect(_page.GetByRole(AriaRole.Button, new() { Name = "Share" })).ToBeVisibleAsync();

        await _page.GetByRole(AriaRole.Link, new() { Name = "logout [testuser]" }).ClickAsync();

    }

    [Test, Order(5)]
    public async Task UsersCannotEnterCheepsLongerThan160Characters()
    {
        var CheepLongerthan160 = "Planning an amazing event takes effort, but teamwork and creativity make it unforgettable. Lets aim for success and celebrate together. Join us soon—excited! Also you should cut me off soon";
        await _page!.GotoAsync("https://localhost:5001");
        await _page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await _page.GetByPlaceholder("username").ClickAsync();
        await _page.GetByPlaceholder("username").FillAsync("testuser");
        await _page.GetByPlaceholder("password").ClickAsync();
        await _page.GetByPlaceholder("password").FillAsync("Hello_1");
        await _page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await _page.Locator("#cheepTextInput").ClickAsync();
        await _page.Locator("#cheepTextInput").FillAsync(CheepLongerthan160);
        string actual = await _page!.Locator("#cheepTextInput").InputValueAsync();

        Assert.That(actual.Length, Is.EqualTo(160));

        await _page.GetByRole(AriaRole.Link, new() { Name = "logout [testuser]" }).ClickAsync();


    }

    [Test, Order(6)]
    public async Task WhenUsersSendCheepsItsAddedToDb()
    {

        await _page!.GotoAsync("https://localhost:5001");
        await _page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await _page.GetByPlaceholder("username").ClickAsync();
        await _page.GetByPlaceholder("username").FillAsync("testuser");
        await _page.GetByPlaceholder("password").ClickAsync();
        await _page.GetByPlaceholder("password").FillAsync("Hello_1");
        await _page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await _page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await _page.Locator("#cheepTextInput").ClickAsync();
        await _page.Locator("#cheepTextInput").FillAsync("Hello i am test");
        await _page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(_page.Locator("#messagelist")).ToContainTextAsync("Hello i am test");
        await _page.GetByRole(AriaRole.Link, new() { Name = "logout [testuser]" }).ClickAsync();
        await _page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "logout [testuser]" })).Not.ToBeVisibleAsync();
        await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "login" })).ToBeVisibleAsync();
        await Expect(_page.Locator("#messagelist")).ToContainTextAsync("Hello i am test");

    }

    [Test, Order(7)]

    public async Task LogInAndTestVulnerabilityToXss()
    {
        await _page!.GotoAsync("https://localhost:5001/");
        await _page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await _page.GetByPlaceholder("username").ClickAsync();
        await _page.GetByPlaceholder("username").FillAsync("testuser");
        await _page.GetByPlaceholder("password").ClickAsync();
        await _page.GetByPlaceholder("password").FillAsync("Hello_1");
        await _page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(_page.GetByRole(AriaRole.Heading, new() { Name = "What's on your mind testuser" })).ToBeVisibleAsync();
        await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "logout [testuser]" })).ToBeVisibleAsync();
        await _page.Locator("#cheepTextInput").ClickAsync();
        await _page.Locator("#cheepTextInput").FillAsync("Hello, I am feeling good!<script>alert('If you see this in a popup, you are in trouble!');</script>");
        await _page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(_page.Locator("#messagelist")).ToContainTextAsync("testuser Hello, I am feeling good!<script>alert('If you see this in a popup, you are in trouble!');</script>");
        await _page.GetByRole(AriaRole.Link, new() { Name = "logout [testuser]" }).ClickAsync();
    }


    [Test, Order(8)]
    public async Task DeleteCheepFromPrivateAndPublic()
    {
        await _page!.GotoAsync("https://localhost:5001/");
        await _page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await _page.GetByPlaceholder("username").ClickAsync();
        await _page.GetByPlaceholder("username").FillAsync("testuser");
        await _page.GetByPlaceholder("password").ClickAsync();
        await _page.GetByPlaceholder("password").FillAsync("Hello_1");
        await _page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await _page.Locator("#cheepTextInput").ClickAsync();
        await _page.Locator("#cheepTextInput").FillAsync("Delete me from public");
        await _page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(_page.Locator("#messagelist")).ToContainTextAsync("Delete me from public");
        //await Expect(_page.GetByRole(AriaRole.Button, new() { Name = "Delete" })).ToBeVisibleAsync();

        void Page_Dialog_EventHandler(object sender, IDialog dialog)
        {
            //Console.WriteLine($"Dialog message: {dialog.Message}");
            dialog.AcceptAsync();
            _page.Dialog -= Page_Dialog_EventHandler;
        }
        _page.Dialog += Page_Dialog_EventHandler;
        await _page.Locator("li").Filter(new() { HasText = "testuser Delete me from public" }).GetByRole(AriaRole.Button).ClickAsync();

        await Expect(_page.Locator("#messagelist")).Not.ToContainTextAsync("Delete me from public");

        await _page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();

        await _page.Locator("#cheepTextInput").ClickAsync();
        await _page.Locator("#cheepTextInput").FillAsync("Delete me from private");
        await _page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        //await Expect(_page.GetByRole(AriaRole.Button, new() { Name = "Delete" })).ToBeVisibleAsync();

        await Expect(_page.Locator("#messagelist")).ToContainTextAsync("Delete me from private");

        void Page2_Dialog_EventHandler(object sender, IDialog dialog)
        {
            //Console.WriteLine($"Dialog message: {dialog.Message}");
            dialog.AcceptAsync();
            _page.Dialog -= Page2_Dialog_EventHandler;
        }
        _page.Dialog += Page2_Dialog_EventHandler;
        await _page.Locator("li").Filter(new() { HasText = "testuser Delete me from private" }).GetByRole(AriaRole.Button).ClickAsync();

        //await Expect(_page.GetByRole(AriaRole.Button, new() { Name = "Delete" })).Not.ToBeVisibleAsync();

        await _page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();

        await Expect(_page.Locator("#messagelist")).Not.ToContainTextAsync("Delete me from private");

        await Expect(_page.Locator("#messagelist")).Not.ToContainTextAsync("Delete me from public");

        await _page.GetByRole(AriaRole.Link, new() { Name = "logout [testuser]" }).ClickAsync();


    }

    [Test, Order(9)]
    public async Task ForgetTestUser()
    {
        await _page!.GotoAsync("https://localhost:5001/");
        await _page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await _page.GetByPlaceholder("username").ClickAsync();
        await _page.GetByPlaceholder("username").FillAsync("testuser");
        await _page.GetByPlaceholder("password").ClickAsync();
        await _page.GetByPlaceholder("password").FillAsync("Hello_1");
        await _page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await _page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
        await Expect(_page.GetByRole(AriaRole.Button, new() { Name = "Forget Me!" })).ToBeVisibleAsync();
        await _page.GetByRole(AriaRole.Button, new() { Name = "Forget Me!" }).ClickAsync();
        await Expect(_page.GetByRole(AriaRole.Button, new() { Name = "Forget Me!" })).Not.ToBeVisibleAsync();
        await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "login" })).ToBeVisibleAsync();
        await Expect(_page!.GetByRole(AriaRole.Button, new() { Name = "Share" })).Not.ToBeVisibleAsync();



    }

    [Test, Order(10)]
    public async Task PaginationWorksCorrectlyOnPublicTimeline()
    {
        await _page!.GotoAsync("https://localhost:5001");

        await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "1" })).ToHaveClassAsync("active");
        await Expect(_page.Locator("#messagelist li")).ToHaveCountAsync(32); // Assuming page size is 32

        await _page.GetByRole(AriaRole.Link, new() { Name = "Next" }).ClickAsync();

        await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "2" })).ToHaveClassAsync("active");
        await Expect(_page.Locator("#messagelist li")).ToHaveCountAsync(32); // Assuming there are cheeps on page 2

        await _page.GetByRole(AriaRole.Link, new() { Name = "Previous" }).ClickAsync();

        await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "1" })).ToHaveClassAsync("active");

        await _page.GetByRole(AriaRole.Link, new() { Name = "3" }).ClickAsync();

        await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "3" })).ToHaveClassAsync("active");
        await Expect(_page.Locator("#messagelist li")).ToHaveCountAsync(32); // Assuming there are cheeps on page 3

        if (await _page.GetByRole(AriaRole.Link, new() { Name = "Next" }).IsVisibleAsync())
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = "Next" }).ClickAsync();
            await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "Next" })).Not.ToBeVisibleAsync();
        }

        await _page.GetByRole(AriaRole.Link, new() { Name = "1" }).ClickAsync();
        await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "Previous" })).Not.ToBeVisibleAsync();
    }
}