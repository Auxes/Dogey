﻿using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules.Moderation
{
    [RequireOwner]
    [Group("sudo"), Name("Sudo")]
    [Summary("Execute a command as another user")]
    public class SudoModule : DogeyModuleBase
    {
        private readonly CommandHandler _handler;
        private readonly IServiceProvider _provider;

        public SudoModule(CommandHandler handler, IServiceProvider provider)
        {
            _handler = handler;
            _provider = provider;
        }

        [Command]
        [Summary("Execute a command as the specified user")]
        public async Task SudoAsync(SocketUser user, [Remainder]string command)
        {
            var context = new DogeyCommandContext(Context.Client, Context.Message, user);
            await _handler.ExecuteAsync(context, _provider, command);
        }
    }
}
