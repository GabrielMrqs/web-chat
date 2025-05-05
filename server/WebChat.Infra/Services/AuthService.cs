using Cassandra;
using Microsoft.Extensions.Options;
using WebChat.Application.DTOs;
using WebChat.Application.Models;
using WebChat.Application.Services;
using WebChat.Infra.Configuration;
using WebChat.Infra.Helper;

namespace WebChat.Infra.Services
{
    public class AuthService : IAuthService
    {
        private readonly ISession _session;
        private readonly ScyllaDbSettings _scyllaDbSettings;

        public AuthService(IOptions<ScyllaDbSettings> options)
        {

            _scyllaDbSettings = options.Value;

            var cluster = Cluster.Builder()
                .AddContactPoints(_scyllaDbSettings.ContactPoints)
                .WithPort(_scyllaDbSettings.Port)
                .WithLoadBalancingPolicy(
                    new DCAwareRoundRobinPolicy(_scyllaDbSettings.DataCenter))
                .Build();

            _session = cluster.Connect();

            _session.Execute($@"
                CREATE KEYSPACE IF NOT EXISTS {_scyllaDbSettings.Keyspace}
                WITH replication = {{ 'class': 'NetworkTopologyStrategy', '{_scyllaDbSettings.DataCenter}': 3 }};
            ");
            _session.ChangeKeyspace(_scyllaDbSettings.Keyspace);
            _session.Execute($@"
                CREATE TABLE IF NOT EXISTS {_scyllaDbSettings.UsersTable} (
                  email          text PRIMARY KEY,
                  name           text,
                  user_id        uuid,
                  hash           text,
                  salt           text,
                  created_at     timestamp
                );
            ");
        }

        public async Task RegisterUserAsync(RegisterUserDTO dto)
        {
            var insertPs = await _session.PrepareAsync($@"
                INSERT INTO {_scyllaDbSettings.UsersTable} 
                    (email, name, user_id, hash, salt, created_at)
                VALUES (?, ?, ?, ?, ?, toTimestamp(now()))
                ");

            var salt = PasswordHasher.GenerateSalt();
            var hash = PasswordHasher.HashPassword(dto.Password, salt);

            var bound = insertPs.Bind(
                dto.Email,
                dto.Name,
                Guid.NewGuid(),
                PasswordHasher.ToBase64(hash),
                PasswordHasher.ToBase64(salt)
            );

            await _session.ExecuteAsync(bound).ConfigureAwait(false);
        }

        public async Task LoginAsync(LoginDTO dto)
        {
            var psUser = await _session.PrepareAsync($@"
                SELECT user_id, hash, salt
                FROM {_scyllaDbSettings.UsersTable}
                WHERE email = ?
                ");

            var bound = psUser.Bind(dto.Email);

            var rsUser = await _session.ExecuteAsync(bound).ConfigureAwait(false);

            var row = rsUser.FirstOrDefault();
            if (row is null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            var user = new User
            {
                UserId = row.GetValue<Guid>("user_id"),
                Hash = row.GetValue<string>("hash"),
                Salt = row.GetValue<string>("salt"),
            };

            var saltBytes = PasswordHasher.FromBase64(user.Salt);
            var attemptHash = PasswordHasher.ToBase64(
               PasswordHasher.HashPassword(dto.Password, saltBytes)
            );

            if (attemptHash != user.Hash)
            {
                throw new UnauthorizedAccessException("Email or password are invalid");
            }
        }
    }
}
