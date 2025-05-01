using WebChat.API.Hubs;
using WebChat.Application.Services;
using WebChat.Infra.Configuration;
using WebChat.Infra.Services;

namespace WebChat.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<ScyllaDbSettings>(
                builder.Configuration.GetSection("ScyllaDbSettings"));

            builder.Services.AddSingleton<IRoomService, RoomService>();
            builder.Services.AddSingleton<IMessageService, MessageService>();

            builder.Services.AddSignalR();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            //builder.Services.AddAuthentication().AddGoogle(options =>)

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<ChatHub>("/chatHub");

            app.UseCors(x => x.WithOrigins("http://localhost:3000")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials());

            app.Run();
        }
    }
}
