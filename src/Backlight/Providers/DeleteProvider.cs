﻿using System.Threading.Tasks;

namespace Backlight.Providers {
    public interface DeleteProvider {
        Task Delete(string entityId);
    }
}