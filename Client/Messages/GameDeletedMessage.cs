using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Client.Messages;

public sealed class GameDeletedMessage : ValueChangedMessage<int>
{
    public GameDeletedMessage(int deletedId) : base(deletedId) { }
}
