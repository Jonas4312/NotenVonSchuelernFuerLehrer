namespace NotenVonSchuelernFuerLehrer.WebApi.Exceptions; 

public class ValidationException : Exception
{
    public ValidationException(params string[] validationErrors)
    {
        ValidationErrors = validationErrors.ToList();
    }
    
    public ValidationException(List<string> validationErrors)
    {
        ValidationErrors = validationErrors;
    }
    
    public readonly List<string> ValidationErrors;
}