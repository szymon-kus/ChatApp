using OnlineChat;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

var login = new Users("/Users/szymonkus/Desktop/Projekty IT/C#/WebChat/WebChat/users.json");

while (true)
{
    Console.WriteLine("Wybierz opcję: " +
                      "\n 1. Dodaj użytkownika" +
                      "\n 2. Zaloguj się " +
                      "\n 0. Zakończ");
    int userInput = Convert.ToInt32(Console.ReadLine());
    if (userInput == 1)
    {
        login.CreateUser();
    }
    else if (userInput == 2)
    {
        login.LoginUser();
    }
    else
    {
        Console.WriteLine("Zakończono");
        break;
    }
}