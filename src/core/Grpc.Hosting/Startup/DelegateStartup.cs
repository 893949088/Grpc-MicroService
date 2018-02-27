// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Grpc.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Hosting
{
    public class DelegateStartup : StartupBase<IServiceCollection>
    {
        private Action<IGrpcServer> _configureApp;

        public DelegateStartup(IServiceProviderFactory<IServiceCollection> factory, Action<IGrpcServer> configureApp) : base(factory)
        {
            _configureApp = configureApp;
        }

        public override void Configure(IGrpcServer app) => _configureApp(app);
    }
}