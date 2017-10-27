﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("clean"), Name("Clean")]
    [Summary("Clean messages from a channel.")]
    [RequireContext(ContextType.Guild)]
    [RequireUserPermission(ChannelPermission.ManageMessages)]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    public class CleanModule : DogeyModuleBase
    {
        [Command("all")]
        [Summary("Clean all recent messages")]
        public async Task AllAsync(int history = 25)
        {
            var messages = await GetMessageAsync(history);
            await DeleteMessagesAsync(messages);

            var reply = await ReplyAsync($"Deleted **{messages.Count()}** message(s)");
            await DelayDeleteMessageAsync(reply);
        }

        [Command("user")]
        [Summary("Clean all recent messages from the specified user")]
        public async Task UserAsync(SocketUser user, int history = 25)
        {
            var messages = (await GetMessageAsync(history)).Where(x => x.Author.Id == user.Id);
            await DeleteMessagesAsync(messages);

            var reply = await ReplyAsync($"Deleted **{messages.Count()}** message(s) by **{user}**");
            await DelayDeleteMessageAsync(reply);
        }

        [Command("bots")]
        [Summary("Clean all recent messages made by bots")]
        public async Task BotsAsync(int history = 25)
        {
            var messages = (await GetMessageAsync(history)).Where(x => x.Author.IsBot);
            await DeleteMessagesAsync(messages);

            var reply = await ReplyAsync($"Deleted **{messages.Count()}** message(s) by bots");
            await DelayDeleteMessageAsync(reply);
        }
        
        [Command("contains")]
        [Summary("Clean all recent messages that contain a certain phrase")]
        public async Task ContainsAsync(string text, int history = 25)
        {
            var messages = (await GetMessageAsync(history)).Where(x => x.Content.ToLower().Contains(text.ToLower()));
            await DeleteMessagesAsync(messages);

            var reply = await ReplyAsync($"Deleted **{messages.Count()}** message(s) containing `{text}`.");
            await DelayDeleteMessageAsync(reply);
        }

        [Command("attachments")]
        [Summary("Clean all recent messages with attachments")]
        public async Task AttachmentsAsync(int history = 25)
        {
            var messages = (await GetMessageAsync(history)).Where(x => x.Attachments.Count() != 0);
            await DeleteMessagesAsync(messages);

            var reply = await ReplyAsync($"Deleted **{messages.Count()}** message(s) with attachments.");
            await DelayDeleteMessageAsync(reply);
        }

        [Command("duplicates")]
        [Summary("Clean all recent messages with attachments")]
        public async Task DuplicatesAsync(int history = 25)
        {
            var groups = (await GetMessageAsync(history)).GroupBy(x => x.Content).Where(x => x.Count() > 1);
            var messages = groups.SelectMany(x => x.Skip(1));
            await DeleteMessagesAsync(messages);

            var reply = await ReplyAsync($"Deleted **{messages.Count()}** message(s) with **{groups.Count()-1}** duplicate content(s).");
            await DelayDeleteMessageAsync(reply);
        }

        private Task<IEnumerable<IMessage>> GetMessageAsync(int count)
            => Context.Channel.GetMessagesAsync(count).Flatten();

        private Task DeleteMessagesAsync(IEnumerable<IMessage> messages)
            => Context.Channel.DeleteMessagesAsync(messages);

        private async Task DelayDeleteMessageAsync(IMessage message, int ms = 10000)
        {
            await Task.Delay(ms);
            await message.DeleteAsync();
        }
    }
}
