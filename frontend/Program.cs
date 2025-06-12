var builder = WebApplication.CreateBuilder(args);

// Juste Razor Pages, session (si besoin), et HttpContextAccessor (optionnel)
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(); // seulement si tu utilises la session

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();        // si tu utilises session
// Pas besoin d'UseAuthentication() ni UseAuthorization() ici

app.MapRazorPages();

app.Run();
