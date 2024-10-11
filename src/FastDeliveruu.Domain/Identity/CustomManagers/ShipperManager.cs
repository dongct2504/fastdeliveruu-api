using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FastDeliveruu.Domain.Identity.CustomManagers;

public class ShipperManager : UserManager<Shipper>
{
    public ShipperManager(
        IUserStore<Shipper> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<Shipper> passwordHasher,
        IEnumerable<IUserValidator<Shipper>> userValidators,
        IEnumerable<IPasswordValidator<Shipper>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<Shipper>> logger)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        RegisterTokenProvider(TokenOptions.DefaultProvider, new EmailTokenProvider<Shipper>());
    }
}
