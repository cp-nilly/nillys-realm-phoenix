#region

using wServer.realm.entities;
using wServer.realm.entities.player;

#endregion

namespace wServer.realm.commands
{
    internal interface ICommand
    {
        string Command { get; }
        int RequiredRank { get; }
        void Execute(Player player, string[] args);
    }
}