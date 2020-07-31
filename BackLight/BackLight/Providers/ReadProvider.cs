using System;
using System.Threading.Tasks;

namespace Backlight.Providers {
    public interface ReadProvider {
        Task<BacklightEntity> Read<T>(string entityId, T returnType) where T : Type;
    }
}