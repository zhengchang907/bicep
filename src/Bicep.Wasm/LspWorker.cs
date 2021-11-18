// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO.Pipelines;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Az;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Registry;
using Bicep.LanguageServer.Snippets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Bicep.Wasm
{
    public class LspWorker
    {
        private readonly IJSRuntime jsRuntime;
        private Server? server;
        private PipeWriter? inputWriter;
        private PipeReader? outputReader;

        public LspWorker(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async Task RunAsync()
        {
            var inputPipe = new Pipe();
            var outputPipe = new Pipe();

            server = new Server(inputPipe.Reader, outputPipe.Writer, new Server.CreationOptions {
                onRegisterServices = services => services
                    .AddSingleton<IModuleRegistryProvider>(new EmptyModuleRegistryProvider())
                    .AddSingleton<ISnippetsProvider>(new EmptySnippetsProvider())
                    .AddSingleton<IAzResourceTypeLoader>(new EmptyAzResourceTypeLoader())
                    .AddSingleton<IModuleRestoreScheduler>(new EmptyModuleRestoreScheduler()),
            }, options => options.Services.AddSingleton<IScheduler>(ImmediateScheduler.Instance));

            inputWriter = inputPipe.Writer;
            outputReader = outputPipe.Reader;

            await Task.WhenAll(
                server.RunAsync(CancellationToken.None),
                ProcessInputStreamAsync(CancellationToken.None),
                jsRuntime.InvokeAsync<object>("LspInitialized", DotNetObjectReference.Create(this)).AsTask());
        }

        [JSInvokable("SendLspDataAsync")]
        public async Task SendMessage(string message)
        {
            await inputWriter!.WriteAsync(Encoding.UTF8.GetBytes(message));
        }

        public async Task ReceiveMessage(string message)
        {
            await jsRuntime.InvokeVoidAsync("ReceiveLspData", message);
        }

        private async Task ProcessInputStreamAsync(CancellationToken cancellationToken)
        {
            do
            {
                var result = await outputReader!.ReadAsync(cancellationToken).ConfigureAwait(false);
                var buffer = result.Buffer;

                var message = Encoding.UTF8.GetString(buffer.Slice(buffer.Start, buffer.End));
                await ReceiveMessage(message);
                outputReader.AdvanceTo(buffer.End, buffer.End);

                // Stop reading if there's no more data coming.
                if (result.IsCompleted && buffer.IsEmpty)
                {
                    break;
                }
            } while (!cancellationToken.IsCancellationRequested);
        }
    }
}