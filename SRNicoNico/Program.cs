using System.IO;
using SRNicoNico;
using SRNicoNico.Models;
using SRNicoNico.ViewModels;
using SRNicoNico.Views;
using Microsoft.EntityFrameworkCore;

var builder = WpfApplication<App, MainWindow>.CreateBuilder(args);

builder.Services.AddDbContext<ViewerDbContext>(options => {
    var env = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    var appData = Path.Combine(env, "SRNicoNico");
    // %APPDATA%\SRNicoNicoを作成
    Directory.CreateDirectory(appData);

    options.UseSqlite($"Filename={Path.Combine(appData, "viewer.db")}");
});

builder.Services.AddSingleton<MainWindowViewModel>();

builder.Services.AddHttpClient();

var app = builder.Build();

// 起動時に必要であればDBをマイグレーション
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<ViewerDbContext>();
    db.Database.Migrate();
}

app.Run();
