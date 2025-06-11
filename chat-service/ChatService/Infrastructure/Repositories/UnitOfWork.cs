using Application.Abstractions;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IMessageRepository Messages { get; }

        public IConversationRepository Conversations { get; }

        public UnitOfWork(IMessageRepository messages, IConversationRepository conversations)
        {
            Messages = messages;
            Conversations = conversations;
        }
    }
}
