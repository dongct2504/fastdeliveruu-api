using FastDeliveruu.Application.Common.Constants;
using System.Text.RegularExpressions;

namespace FastDeliveruu.Application.Common.Behaviors;

public static class ValidateForRequest
{
    public static bool BeValidPhoneNumber(string phoneNumber)
    {
        string pattern = @"^\+(?:[0-9] ?){6,14}[0-9]$";
        return Regex.IsMatch(phoneNumber, pattern);
    }

    public static bool BeValidRole(string? role)
    {
        if (role == null)
        {
            return true;
        }

        if (role == RoleConstants.Customer ||
            role == RoleConstants.Staff ||
            role == RoleConstants.Admin)
        {
            return true;
        }

        return false;
    }

    public static bool BeValidPaymentMethod(string paymentMethod)
    {
        return paymentMethod == PaymentMethods.Cash ||
            paymentMethod == PaymentMethods.Vnpay ||
            paymentMethod == PaymentMethods.Momo;
    }
}