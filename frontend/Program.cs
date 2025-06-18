var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();//Utilis� dans les appels a l'API backend
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.UseRouting(); //Active le syst�me de routage 
//Configuration des routes pour le controlleur MVC
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Utilisateurs}/{action=Index}/{id?}");
app.Run();
