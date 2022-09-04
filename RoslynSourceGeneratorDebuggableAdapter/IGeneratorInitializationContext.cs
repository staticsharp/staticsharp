﻿using System;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Exo.RoslynSourceGeneratorDebuggable {
    public interface IGeneratorInitializationContext {
        CancellationToken CancellationToken { get; }
        void RegisterForPostInitialization(Action<GeneratorPostInitializationContext> callback);
        void RegisterForSyntaxNotifications(SyntaxReceiverCreator receiverCreator);
        void RegisterForSyntaxNotifications(SyntaxContextReceiverCreator receiverCreator);
    }
}