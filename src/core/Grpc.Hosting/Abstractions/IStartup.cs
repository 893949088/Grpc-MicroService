// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Grpc.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Hosting
{
    public interface IStartup
    {
        IServiceProvider ConfigureServices(IServiceCollection services);

        void Configure(IGrpcServer app);
    }
}