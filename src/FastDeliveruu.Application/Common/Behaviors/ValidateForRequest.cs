using System.Text.RegularExpressions;

namespace FastDeliveruu.Application.Common.Behaviors;

public static class ValidateForRequest
{
    public static bool ValidPhoneNumber(string phoneNumber)
    {
        string pattern = @"^\+(?:[0-9] ?){6,14}[0-9]$";
        return Regex.IsMatch(phoneNumber, pattern);
    }
}