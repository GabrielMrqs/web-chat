using Cassandra;
using Microsoft.Extensions.Options;
using WebChat.Application.DTOs;
using WebChat.Application.Models;
using WebChat.Application.Services;
using WebChat.Infra.Configuration;

namespace WebChat.Infra.Services
{
    public class RoomService : IRoomService
    {
        private readonly ISession _session;
        private readonly ScyllaDbSettings _scyllaDbSettings;

        public RoomService(IOptions<ScyllaDbSettings> options)
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
                CREATE TABLE IF NOT EXISTS {_scyllaDbSettings.RoomsTable} (
                  room_id    uuid PRIMARY KEY,
                  name       text,
                  created_at timestamp
                );
            ");
        }

        public async Task CreateRoomAsync(CreateRoomDto dto)
        {
            var insertPs = await _session.PrepareAsync($@"
                INSERT INTO {_scyllaDbSettings.RoomsTable} 
                    (room_id, name, created_at)
                VALUES (uuid(), ?, toTimestamp(now()))
                ");

            var bound = insertPs.Bind(
                dto.Name
            );

            await _session.ExecuteAsync(bound).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Room>> GetRoomsAsync()
        {
            var selectPs = _session.Prepare($@"
                    SELECT room_id, name, created_at
                    FROM {_scyllaDbSettings.RoomsTable};
                ");

            var rs = await _session.ExecuteAsync(selectPs.Bind()).ConfigureAwait(false);

            var rooms = rs.Select(row => new Room
            {
                RoomId = row.GetValue<Guid>("room_id"),
                Name = row.GetValue<string>("name"),
                CreatedAt = row.GetValue<DateTimeOffset>("created_at").UtcDateTime
            });

            return rooms;
        }
    }
} 