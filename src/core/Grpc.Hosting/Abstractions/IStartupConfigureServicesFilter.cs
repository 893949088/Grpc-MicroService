// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Hosting
{
    public interface IStartupConfigureServicesFilter
    {
        Action<IServiceCollection> ConfigureServices(Action<IServiceCollection> next);
    }
}
