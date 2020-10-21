using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backlight.Providers {
    public interface ReadAllIdsProvider {
        Task<List<string>> ReadAllIds();
    }
}