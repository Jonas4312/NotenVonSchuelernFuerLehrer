namespace NotenVonSchuelernFuerLehrer.WebApi.Exceptions; 

public class ValidationException : Exception
{
    public ValidationException(params List<string> validationErrors)
    {
        ValidationErrors = validationErrors;
    }
    
    public readonly List<string> ValidationErrors;
}