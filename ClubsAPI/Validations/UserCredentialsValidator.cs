using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubsAPI.Data;
using ClubsAPI.DTOs;
using FluentValidation;

namespace ClubsAPI.Validations
{
    public class UserCredentialsValidator : AbstractValidator<UserCredentials>
    {
      public UserCredentialsValidator(ApplicationDataContext dbContext)
      {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);

        RuleFor(x => x.Email)
          .Custom((value, context) =>
          {
            var loginInUse = dbContext.Users.Any(u => u.Email == value);
            if (loginInUse)
            {
              context.AddFailure("Email", "That Email is taken");
            }
          });
      }
    }
}
