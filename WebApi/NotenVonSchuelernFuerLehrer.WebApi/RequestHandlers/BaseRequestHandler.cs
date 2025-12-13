namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public abstract class BaseRequestHandler<TRequest, TResponse> : IRequestHandler
    where TRequest : IRequest 
    where TResponse : IResponse
{
    public bool CanHandle(IRequest request)
    {
        return request is TRequest;
    }

    public async Task<IResponse> HandleInternalAsync(IRequest request)
    {
        if (request is not TRequest typedRequest)
        {
            throw new InvalidOperationException($"Wrong request type. Expected {typeof(TRequest).Name}, but got {request.GetType().Name}");
        }

        return await HandleAsync(typedRequest);
    }

    protected abstract Task<TResponse> HandleAsync(TRequest request);
}

public interface IRequestHandler
{
    public bool CanHandle(IRequest request);
    Task<IResponse> HandleInternalAsync(IRequest request);
}
public interface IRequest;
public interface IResponse;
public interface IEmptyResponse : IResponse;