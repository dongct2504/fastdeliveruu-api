using FastDeliveruu.Common.Constants;
using FastDeliveruu.Common.Enums;
using System.Text.RegularExpressions;

namespace FastDeliveruu.Application.Common.Behaviors;

public static class ValidateForRequest
{
    public static bool BeValidPhoneNumber(string? phoneNumber)
    {
        if (phoneNumber == null) return true;

        Regex regex = new Regex(@"^\+84\d{6,15}$");
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

    public static bool BeValidPaymentMethod(PaymentMethodsEnum paymentMethod)
    {
        return paymentMethod == PaymentMethodsEnum.Cash ||
            paymentMethod == PaymentMethodsEnum.Vnpay ||
            paymentMethod == PaymentMethodsEnum.Paypal ||
            paymentMethod == PaymentMethodsEnum.Momo;
    }
}