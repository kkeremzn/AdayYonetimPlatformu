System.FormatException: An error occurred while deserializing the AdayDetails property of class YetenekYonetimAPI.Models.ApplicationDetailsDto: Element 'id' does not match any field or property of class YetenekYonetimAPI.Models.AdayDetailsDto.
 ---> System.FormatException: Element 'id' does not match any field or property of class YetenekYonetimAPI.Models.AdayDetailsDto.
   at MongoDB.Bson.Serialization.BsonClassMapSerializer`1.DeserializeClass(BsonDeserializationContext context)
   at MongoDB.Bson.Serialization.BsonClassMapSerializer`1.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
   at MongoDB.Bson.Serialization.Serializers.SerializerBase`1.MongoDB.Bson.Serialization.IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
   at MongoDB.Bson.Serialization.IBsonSerializerExtensions.Deserialize(IBsonSerializer serializer, BsonDeserializationContext context)
   at MongoDB.Bson.Serialization.BsonClassMapSerializer`1.DeserializeMemberValue(BsonDeserializationContext context, BsonMemberMap memberMap)
   --- End of inner exception stack trace ---
   at MongoDB.Bson.Serialization.BsonClassMapSerializer`1.DeserializeMemberValue(BsonDeserializationContext context, BsonMemberMap memberMap)
   at MongoDB.Bson.Serialization.BsonClassMapSerializer`1.DeserializeClass(BsonDeserializationContext context)
   at MongoDB.Bson.Serialization.BsonClassMapSerializer`1.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
   at MongoDB.Bson.Serialization.IBsonSerializerExtensions.Deserialize[TValue](IBsonSerializer`1 serializer, BsonDeserializationContext context)
   at MongoDB.Bson.Serialization.Serializers.EnumerableSerializerBase`2.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
   at MongoDB.Bson.Serialization.IBsonSerializerExtensions.Deserialize[TValue](IBsonSerializer`1 serializer, BsonDeserializationContext context)
   at MongoDB.Driver.Core.Operations.AggregateOperation`1.CursorDeserializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
   at MongoDB.Bson.Serialization.IBsonSerializerExtensions.Deserialize[TValue](IBsonSerializer`1 serializer, BsonDeserializationContext context)
   at MongoDB.Driver.Core.Operations.AggregateOperation`1.AggregateResultDeserializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
   at MongoDB.Bson.Serialization.IBsonSerializerExtensions.Deserialize[TValue](IBsonSerializer`1 serializer, BsonDeserializationContext context)
   at MongoDB.Driver.Core.WireProtocol.CommandUsingCommandMessageWireProtocol`1.ProcessResponse(ConnectionId connectionId, CommandMessage responseMessage)
   at MongoDB.Driver.Core.WireProtocol.CommandUsingCommandMessageWireProtocol`1.SendMessageAndProcessResponse(CommandRequestMessage message, Int32 responseTo, IConnection connection, CancellationToken cancellationToken)
   at MongoDB.Driver.Core.WireProtocol.CommandUsingCommandMessageWireProtocol`1.Execute(IConnection connection, CancellationToken cancellationToken)
   at MongoDB.Driver.Core.WireProtocol.CommandWireProtocol`1.Execute(IConnection connection, CancellationToken cancellationToken)
   at MongoDB.Driver.Core.Servers.Server.ServerChannel.ExecuteProtocol[TResult](IWireProtocol`1 protocol, ICoreSession session, CancellationToken cancellationToken)
   at MongoDB.Driver.Core.Servers.Server.ServerChannel.Command[TResult](ICoreSession session, ReadPreference readPreference, DatabaseNamespace databaseNamespace, BsonDocument command, IEnumerable`1 commandPayloads, IElementNameValidator commandValidator, BsonDocument additionalOptions, Action`1 postWriteAction, CommandResponseHandling responseHandling, IBsonSerializer`1 resultSerializer, MessageEncoderSettings messageEncoderSettings, CancellationToken cancellationToken)
   at MongoDB.Driver.Core.Operations.CommandOperationBase`1.ExecuteProtocol(IChannelHandle channel, ICoreSessionHandle session, ReadPreference readPreference, CancellationToken cancellationToken)
   at MongoDB.Driver.Core.Operations.ReadCommandOperation`1.ExecuteAttempt(RetryableReadContext context, Int32 attempt, Nullable`1 transactionNumber, CancellationToken cancellationToken)
   at MongoDB.Driver.Core.Operations.RetryableReadOperationExecutor.Execute[TResult](IRetryableReadOperation`1 operation, RetryableReadContext context, CancellationToken cancellationToken)
   at MongoDB.Driver.Core.Operations.ReadCommandOperation`1.Execute(RetryableReadContext context, CancellationToken cancellationToken)
   at MongoDB.Driver.Core.Operations.AggregateOperation`1.Execute(RetryableReadContext context, CancellationToken cancellationToken)
   at MongoDB.Driver.Core.Operations.AggregateOperation`1.Execute(IReadBinding binding, CancellationToken cancellationToken)
   at MongoDB.Driver.OperationExecutor.ExecuteReadOperation[TResult](IReadBinding binding, IReadOperation`1 operation, CancellationToken cancellationToken)
   at MongoDB.Driver.MongoCollectionImpl`1.ExecuteReadOperation[TResult](IClientSessionHandle session, IReadOperation`1 operation, ReadPreference readPreference, CancellationToken cancellationToken)
   at MongoDB.Driver.MongoCollectionImpl`1.ExecuteReadOperation[TResult](IClientSessionHandle session, IReadOperation`1 operation, CancellationToken cancellationToken)
   at MongoDB.Driver.MongoCollectionImpl`1.Aggregate[TResult](IClientSessionHandle session, PipelineDefinition`2 pipeline, AggregateOptions options, CancellationToken cancellationToken)
   at MongoDB.Driver.MongoCollectionImpl`1.<>c__DisplayClass23_0`1.<Aggregate>b__0(IClientSessionHandle session)
   at MongoDB.Driver.MongoCollectionImpl`1.UsingImplicitSession[TResult](Func`2 func, CancellationToken cancellationToken)
   at MongoDB.Driver.MongoCollectionImpl`1.Aggregate[TResult](PipelineDefinition`2 pipeline, AggregateOptions options, CancellationToken cancellationToken)
   at YetenekYonetimAPI.Services.ApplicationService.GetApplicationsWithAdayDetailsAsync(String jobPostingId) in /app/Services/ApplicationService.cs:line 58
   at YetenekYonetimAPI.Controllers.JobPostingController.GetApplicationsWithDetailsByJobPostingId(String id) in /app/Controllers/JobPostingController.cs:line 79
   at lambda_method22(Closure, Object)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ActionMethodExecutor.AwaitableObjectResultExecutor.Execute(ActionContext actionContext, IActionResultTypeMapper mapper, ObjectMethodExecutor executor, Object controller, Object[] arguments)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeActionMethodAsync>g__Awaited|12_0(ControllerActionInvoker invoker, ValueTask`1 actionResultValueTask)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeNextActionFilterAsync>g__Awaited|10_0(ControllerActionInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Rethrow(ActionExecutedContextSealed context)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeInnerFilterAsync>g__Awaited|13_0(ControllerActionInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeFilterPipelineAsync>g__Awaited|20_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
   at Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIMiddleware.Invoke(HttpContext httpContext)
   at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)
   at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.Authentication.AuthenticationMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddlewareImpl.Invoke(HttpContext context)

HEADERS
=======
Accept: text/plain
Connection: keep-alive
Host: localhost:5005
User-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36
Accept-Encoding: gzip, deflate, br, zstd
Accept-Language: tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7
Referer: http://localhost:5005/swagger/index.html
sec-ch-ua-platform: "macOS"
sec-ch-ua: "Not)A;Brand";v="8", "Chromium";v="138", "Google Chrome";v="138"
sec-ch-ua-mobile: ?0
Sec-Fetch-Site: same-origin
Sec-Fetch-Mode: cors
Sec-Fetch-Dest: empty