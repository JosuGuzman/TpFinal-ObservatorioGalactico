namespace WatchTower.Core.Exceptions;

public class BusinessRuleException : WatchTowerException
{
    public BusinessRuleException(string message) 
        : base(message, "BUSINESS_RULE_VIOLATION", 422)
    {
    }
}