using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Backlight.Api.Methods;
using Backlight.Api.Serialization;
using Backlight.Exceptions;
using Backlight.Services;
using Microsoft.AspNetCore.Http;

namespace Backlight.Api {
    public class ApiRunner {
        private readonly BacklightService service;
        private readonly StreamSerializer streamSerializer;

        public ApiRunner(BacklightService service, StreamSerializer streamSerializer) {
            this.service = service;
            this.streamSerializer = streamSerializer;
        }

        public async Task<ApiResult> Run(HttpContext httpContext) {
            if (IsNotAllowed(httpContext.Request.Method)) return await MethodNotAllowedResponse(httpContext);
            try {
                var entityTypeName = TryToGetEntityTypeNameFrom(httpContext.Request);
                if (IsReadAllIds(httpContext.Request)) return await new ReadAllIds(service, httpContext).Execute(entityTypeName);
                var entityId = TryToGetEntityIdFrom(httpContext.Request);
                var entityPayload = await TryToGetEntityPayloadFrom(httpContext.Request.Body);
                return await (httpContext.Request.Method switch {
                    var method when method.Equals(HttpMethods.Put) => new Create(service, httpContext).Execute(entityTypeName, entityPayload),
                    var method when method.Equals(HttpMethods.Get) => new Read(service, httpContext).Execute(entityTypeName, entityId),
                    var method when method.Equals(HttpMethods.Post) => new Update(service, httpContext).Execute(entityTypeName, entityId, entityPayload),
                    var method when method.Equals(HttpMethods.Delete) => new Delete(service, httpContext).Execute(entityTypeName, entityId)
                });
            } catch (TypeCanNotBeSerializedFromPathException) {
                return await TypeCannotBeDeserializedFromPathResponse(httpContext);
            } catch (EntityIdCanNotBeSerializedFromPathException) {
                return await EntityIdCannotBeSerializedFromPathResponse(httpContext);
            } catch (EntityProviderIsNotAvailableException) {
                return await EntityProviderIsNotAvailableResponse(httpContext);
            } catch (EntityIsNotConfiguredException) {
                return await EntityIsNotConfiguredResponse(httpContext);
            } catch (EntityDeserializationException) {
                return await EntityPayloadDeserializationErrorResponse(httpContext);
            }
        }

        private bool IsReadAllIds(HttpRequest request) {
            return Regex.IsMatch(request.Path.Value, "^/types/([\\w\\.\\-]+)/all$");
        }

        private async Task<string> TryToGetEntityPayloadFrom(Stream requestBody) {
            return await streamSerializer.EntityPayloadFrom(requestBody);
        }

        private static bool IsNotAllowed(string requestMethod) {
            var allowedMethods = new List<string> {
                HttpMethods.Put,
                HttpMethods.Get,
                HttpMethods.Post,
                HttpMethods.Delete
            };
            return !allowedMethods.Contains(requestMethod);
        }

        private async Task<ApiResult> MethodNotAllowedResponse(HttpContext httpContext) {
            await ResponseWith(HttpStatusCode.MethodNotAllowed, ErrorMessages.MethodNotAllowed, httpContext);
            return ApiResult.ERROR;
        }

        private static string TryToGetEntityTypeNameFrom(HttpRequest request) {
            if (request.Method == HttpMethods.Put) {
                if (!Regex.IsMatch(request.Path.Value, "^/types/([\\w\\.\\-]+)$")) throw new TypeCanNotBeSerializedFromPathException();
                return request.Path.Value.Split("/types/")[1];
            };
            if (Regex.IsMatch(request.Path.Value, "^/types/([\\w\\.\\-]+)/all$")) return request.Path.Value.Split("/types/")[1].Split('/')[0];
            if (!Regex.IsMatch(request.Path.Value, "^/types/([\\w\\.\\-]+)/entities([\\w\\.\\-\\/]+)$")) throw new TypeCanNotBeSerializedFromPathException();
            return request.Path.Value.Split("/types/")[1].Split('/')[0];
        }
        private static string TryToGetEntityIdFrom(HttpRequest request) {
            if (request.Method == HttpMethods.Put) return string.Empty;
            if (!Regex.IsMatch(request.Path.Value, "^/types/([\\w\\.\\-]+)/entities/([\\w\\.\\-]+)(?<!\\/)$")) throw new EntityIdCanNotBeSerializedFromPathException();
            return request.Path.Value.Split("/entities/")[1];
        }

        private async Task<ApiResult> TypeCannotBeDeserializedFromPathResponse(HttpContext httpContext) {
            await ResponseWith(HttpStatusCode.BadRequest, ErrorMessages.TypeCannotBeDeserializedFromPathError, httpContext);
            return ApiResult.ERROR;
        }
        private async Task<ApiResult> EntityIdCannotBeSerializedFromPathResponse(HttpContext httpContext) {
            await ResponseWith(HttpStatusCode.BadRequest, ErrorMessages.EntityIdCannotBeDeserializedFromPathError, httpContext);
            return ApiResult.ERROR;
        }

        private async Task<ApiResult> EntityIsNotConfiguredResponse(HttpContext httpContext) {
            await ResponseWith(HttpStatusCode.BadRequest, ErrorMessages.EntityIsNotConfigured, httpContext);
            return ApiResult.ERROR;
        }
        private async Task<ApiResult> EntityPayloadDeserializationErrorResponse(HttpContext httpContext) {
            await ResponseWith(HttpStatusCode.BadRequest, ErrorMessages.EntityPayloadDeserializationError, httpContext);
            return ApiResult.ERROR;
        }

        private async Task<ApiResult> EntityProviderIsNotAvailableResponse(HttpContext httpContext) {
            await ResponseWith(HttpStatusCode.BadRequest, ErrorMessages.EntityProviderIsNotAvailable, httpContext);
            return ApiResult.ERROR;
        }

        private async Task ResponseWith(HttpStatusCode httpStatusCode, string responseBody, HttpContext httpContext) {
            httpContext.Response.StatusCode = (int) httpStatusCode;
            await httpContext.Response.WriteAsync(responseBody, Encoding.UTF8);
        }

    }

}