namespace Application.Abstractions
{
    public interface IUnitOfWork
    {
        IMessageRepository Messages {  get; }
        IConversationRepository Conversations { get; }
    }
}
