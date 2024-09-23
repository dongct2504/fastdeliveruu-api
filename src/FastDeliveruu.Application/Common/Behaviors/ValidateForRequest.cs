using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Enums;
using System.Text.RegularExpressions;

namespace FastDeliveruu.Application.Common.Behaviors;

public static class ValidateForRequest
{
    public static bool BeValidPhoneNumber(string phoneNumber)
    {
        Regex regex = new Regex(@"^\d{6,15}$");
        return regex.IsMatch(phoneNumber);
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

    public static bool BeValidPaymentMethod(PaymentMethods paymentMethod)
    {
        return paymentMethod == PaymentMethods.Cash ||
            paymentMethod == PaymentMethods.Vnpay ||
            paymentMethod == PaymentMethods.Momo;
    }
}