﻿using System;
using System.Threading.Tasks;

namespace Backlight.Providers {
    public interface ReadProvider {
        Task<object> Read<T>(string entityId, T returnType);
    }
}