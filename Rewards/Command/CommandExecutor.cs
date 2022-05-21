namespace Rewards.Command
{
    public interface ICommand<TCommandResult>
    {
        public Task<TCommandResult> ExecuteAsync();
    }

    public interface ICommandExecutor
    {
        public Task<TResult> ExecuteCommandAsync<TCommand, TResult>(Action<TCommand> action) where TCommand : ICommand<TResult>;
    }


    public class CommandExecutor : ICommandExecutor
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandExecutor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResult> ExecuteCommandAsync<TCommand, TResult>(Action<TCommand> action) where TCommand : ICommand<TResult>
        {
            var command = _serviceProvider.GetService<TCommand>();
            if (command == null)
                throw new Exception($"Could not resolve instance for {typeof(TCommand).FullName}");

            action(command);
            return await command.ExecuteAsync();
        }
    }
}
