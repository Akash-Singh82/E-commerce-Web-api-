using E_commerce.Application.Interfaces;
using E_commerce.Data;
using E_commerce.Application.Services;
//using E_commerce.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Climate;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
builder.Services.AddControllers().AddNewtonsoftJson(); // for Stripe events

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// db
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// stripe config
StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];



// services
builder.Services.AddScoped<IMyProductService, MyProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IMyCheckoutService, MyCheckoutService>();


//  =============>>>>>  Important <<<<<<<<===============
// fixing circular reference error 

//builder.Services.AddControllers().AddNewtonsoftJson(options =>
//{
//    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
//});


// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DemoData.Seed(db);
}



    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
    app.UseSwagger();
    app.UseSwaggerUI();
    }

app.UseHttpsRedirection();
app.UseCors("AllowAngular");

//app.UseAuthorization();

app.MapControllers();

app.Run();
