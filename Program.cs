var builder = WebApplication.CreateBuilder(args);

// ✅ إعداد الرخصة الخاصة بـ QuestPDF
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ السماح بالوصول إلى swagger
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/swagger"))
    {
        await next.Invoke();
        return;
    }
    await next.Invoke();
});

app.UseAuthorization();
app.MapControllers();
app.Run();
