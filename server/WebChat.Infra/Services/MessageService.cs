using Cassandra;
using Microsoft.Extensions.Options;
using WebChat.Application.Models;
using WebChat.Application.Services;
using WebChat.Infra.Configuration;

namespace WebChat.Infra.Services
{
    public class MessageService : IMessageService
    {
        private readonly ISession _session;
        private readonly ScyllaDbSettings _scyllaDbSettings;

        public MessageService(IOptions<ScyllaDbSettings> options)
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
                CREATE TABLE IF NOT EXISTS {_scyllaDbSettings.MessagesTable} (
                  room_id     uuid,
                  sent_at     timestamp,
                  message_id  timeuuid,
                  sender_id   uuid,
                  content     text,
                  PRIMARY KEY ((room_id), sent_at, message_id)
                ) WITH CLUSTERING ORDER BY (sent_at ASC);
            ");
        }

        public async Task SaveMessageAsync(Message message)
        {
            var insertPs = await _session.PrepareAsync($@"
                INSERT INTO {_scyllaDbSettings.MessagesTable} 
                    (room_id, sent_at, message_id, sender_id, content)
                VALUES (?, toTimestamp(now()), now(), ?, ?)
                ");

            var bound = insertPs.Bind(
                message.RoomId,
                message.SenderId,
                message.Content
            );

            await _session.ExecuteAsync(bound).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Message>> GetChatHistoryAsync(Guid roomId, int limit = 50)
        {
            var selectPs = _session.Prepare($@"
                    SELECT room_id, message_id, sender_id, content, sent_at
                    FROM {_scyllaDbSettings.MessagesTable}
                    WHERE room_id = ?
                    LIMIT ?;
                ");

            var bound = selectPs.Bind(roomId, limit);

            var rs = await _session.ExecuteAsync(bound).ConfigureAwait(false);

            var messages = rs.Select(row => new Message
            {
                RoomId = row.GetValue<Guid>("room_id"),
                MessageId = row.GetValue<Guid>("message_id"),
                SenderId = row.GetValue<Guid>("sender_id"),
                Content = row.GetValue<string>("content"),
                SentAt = row.GetValue<DateTimeOffset>("sent_at").UtcDateTime
            });

            return messages;
        }
    }
}