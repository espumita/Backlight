using System.Linq;
using System.Text.Json;

namespace Backlight {
    public class BacklightProvidersService {
        private readonly BacklightServicesOptions options;

        public BacklightProvidersService(BacklightServicesOptions options) {
            this.options = options;
        }

        public string EntitiesToInject() {
            var entities = options.Providers.Keys.Select(key => {
                var entityProvidersOptions = options.Providers[key];
                var canCreate = entityProvidersOptions.CanCreate();
                var canRead = entityProvidersOptions.CanRead();
                var canUpdate = entityProvidersOptions.CanUpdate();
                var canDelete = entityProvidersOptions.CanDelete();
                return new HtmlEntity(key.Name.ToString(), canCreate, canRead, canUpdate, canDelete);
            }).ToList();
            return JsonSerializer.Serialize(entities);
        }
    }


}