// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using MediatR;
using OmniSharp.Extensions.JsonRpc;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Handlers
{
    [Method("bicep/status", Direction.ClientToServer)]
    public record BicepStatusParams : IRequest<BicepStatus?>;

    public record BicepStatus(int Pid);

    public class BicepStatusHandler : IJsonRpcRequestHandler<BicepStatusParams, BicepStatus?>
    {
        public Task<BicepStatus?> Handle(BicepStatusParams request, CancellationToken cancellationToken)
        {
            var status = new BicepStatus(System.Environment.ProcessId);
            return Task.FromResult<BicepStatus?>(status);
        }
    }
}
