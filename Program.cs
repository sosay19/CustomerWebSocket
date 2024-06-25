using CustomerWebSocket.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173") // Replace with your client URL
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                .AllowCredentials();

});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();   // Redirect HTTP to HTTPS
app.UseCors("CorsPolicy");  // Apply CORS policy early in the pipeline

app.UseRouting();            // Enable routing

//app.UseCors(options =>
//{
//    options.AllowAnyOrigin()
//           .AllowAnyMethod()
//           .AllowAnyHeader();
//});

app.UseAuthorization();      // Enable authorization

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapHub<ChatHub>("/chathub");
    endpoints.MapControllers();
});

app.Run();  // Start the application
