using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Testing;
using Xunit;

namespace Microsoft.AspNet.Identity.InMemory.Test
{
    public class InMemoryStoreTest
    {
        [Fact]
        public async Task CanDeleteUser()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("Delete");
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            IdentityResultAssert.IsSuccess(await manager.Delete(user));
            Assert.Null(await manager.FindById(user.Id));
        }

        [Fact]
        public async Task CanUpdateUserName()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("Update");
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            Assert.Null(await manager.FindByName("New"));
            user.UserName = "New";
            IdentityResultAssert.IsSuccess(await manager.Update(user));
            Assert.NotNull(await manager.FindByName("New"));
            Assert.Null(await manager.FindByName("Update"));
        }

        [Fact]
        public async Task UserValidatorCanBlockCreate()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("CreateBlocked");
            manager.UserValidator = new AlwaysBadValidator();
            IdentityResultAssert.IsFailure(await manager.Create(user), AlwaysBadValidator.ErrorMessage);
        }

        [Fact]
        public async Task UserValidatorCanBlockUpdate()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("UpdateBlocked");
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            manager.UserValidator = new AlwaysBadValidator();
            IdentityResultAssert.IsFailure(await manager.Update(user), AlwaysBadValidator.ErrorMessage);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task UserValidatorBlocksShortEmailsWhenRequiresUniqueEmail(string email)
        {
            var manager = CreateManager();
            var user = new InMemoryUser("UpdateBlocked") { Email = email };
            manager.UserValidator = new UserValidator<InMemoryUser, string> { RequireUniqueEmail = true };
            IdentityResultAssert.IsFailure(await manager.Create(user), "Email cannot be null or empty.");
        }

#if NET45
        [Theory]
        [InlineData("@@afd")]
        [InlineData("bogus")]
        public async Task UserValidatorBlocksInvalidEmailsWhenRequiresUniqueEmail(string email)
        {
            var manager = CreateManager();
            var user = new InMemoryUser("UpdateBlocked") { Email = email };
            manager.UserValidator = new UserValidator<InMemoryUser, string> { RequireUniqueEmail = true };
            IdentityResultAssert.IsFailure(await manager.Create(user), "Email '"+email+"' is invalid.");
        }
#endif

        [Fact]
        public async Task PasswordValidatorCanBlockAddPassword()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("AddPasswordBlocked");
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            manager.PasswordValidator = new AlwaysBadValidator();
            IdentityResultAssert.IsFailure(await manager.AddPassword(user.Id, "password"), AlwaysBadValidator.ErrorMessage);
        }

        [Fact]
        public async Task PasswordValidatorCanBlockChangePassword()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("ChangePasswordBlocked");
            IdentityResultAssert.IsSuccess(await manager.Create(user, "password"));
            manager.PasswordValidator = new AlwaysBadValidator();
            IdentityResultAssert.IsFailure(await manager.ChangePassword(user.Id, "password", "new"), AlwaysBadValidator.ErrorMessage);
        }

        [Fact]
        public async Task CanCreateUserNoPassword()
        {
            var manager = CreateManager();
            IdentityResultAssert.IsSuccess(await manager.Create(new InMemoryUser("CreateUserTest")));
            var user = await manager.FindByName("CreateUserTest");
            Assert.NotNull(user);
            Assert.Null(user.PasswordHash);
            var logins = await manager.GetLogins(user.Id);
            Assert.NotNull(logins);
            Assert.Equal(0, logins.Count());
        }

        [Fact]
        public async Task CanCreateUserAddLogin()
        {
            var manager = CreateManager();
            const string userName = "CreateExternalUserTest";
            const string provider = "ZzAuth";
            const string providerKey = "HaoKey";
            IdentityResultAssert.IsSuccess(await manager.Create(new InMemoryUser(userName)));
            var user = await manager.FindByName(userName);
            var login = new UserLoginInfo(provider, providerKey);
            IdentityResultAssert.IsSuccess(await manager.AddLogin(user.Id, login));
            var logins = await manager.GetLogins(user.Id);
            Assert.NotNull(logins);
            Assert.Equal(1, logins.Count());
            Assert.Equal(provider, logins.First().LoginProvider);
            Assert.Equal(providerKey, logins.First().ProviderKey);
        }

        [Fact]
        public async Task CanCreateUserLoginAndAddPassword()
        {
            var manager = CreateManager();
            var login = new UserLoginInfo("Provider", "key");
            var user = new InMemoryUser("CreateUserLoginAddPasswordTest");
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            IdentityResultAssert.IsSuccess(await manager.AddLogin(user.Id, login));
            Assert.False(await manager.HasPassword(user.Id));
            IdentityResultAssert.IsSuccess(await manager.AddPassword(user.Id, "password"));
            Assert.True(await manager.HasPassword(user.Id));
            var logins = await manager.GetLogins(user.Id);
            Assert.NotNull(logins);
            Assert.Equal(1, logins.Count());
            Assert.Equal(user, await manager.Find(login));
            Assert.Equal(user, await manager.Find(user.UserName, "password"));
        }

        [Fact]
        public async Task AddPasswordFailsIfAlreadyHave()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("CannotAddAnotherPassword");
            IdentityResultAssert.IsSuccess(await manager.Create(user, "Password"));
            Assert.True(await manager.HasPassword(user.Id));
            IdentityResultAssert.IsFailure(await manager.AddPassword(user.Id, "password"), "User already has a password set.");
        }

        [Fact]
        public async Task CanCreateUserAddRemoveLogin()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("CreateUserAddRemoveLoginTest");
            var login = new UserLoginInfo("Provider", "key");
            var result = await manager.Create(user);
            Assert.NotNull(user);
            IdentityResultAssert.IsSuccess(result);
            IdentityResultAssert.IsSuccess(await manager.AddLogin(user.Id, login));
            Assert.Equal(user, await manager.Find(login));
            var logins = await manager.GetLogins(user.Id);
            Assert.NotNull(logins);
            Assert.Equal(1, logins.Count());
            Assert.Equal(login.LoginProvider, logins.Last().LoginProvider);
            Assert.Equal(login.ProviderKey, logins.Last().ProviderKey);
            var stamp = user.SecurityStamp;
            IdentityResultAssert.IsSuccess(await manager.RemoveLogin(user.Id, login));
            Assert.Null(await manager.Find(login));
            logins = await manager.GetLogins(user.Id);
            Assert.NotNull(logins);
            Assert.Equal(0, logins.Count());
            Assert.NotEqual(stamp, user.SecurityStamp);
        }

        [Fact]
        public async Task CanRemovePassword()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("RemovePasswordTest");
            const string password = "password";
            IdentityResultAssert.IsSuccess(await manager.Create(user, password));
            var stamp = user.SecurityStamp;
            IdentityResultAssert.IsSuccess(await manager.RemovePassword(user.Id));
            var u = await manager.FindByName(user.UserName);
            Assert.NotNull(u);
            Assert.Null(u.PasswordHash);
            Assert.NotEqual(stamp, user.SecurityStamp);
        }

        [Fact]
        public async Task CanChangePassword()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("ChangePasswordTest");
            const string password = "password";
            const string newPassword = "newpassword";
            IdentityResultAssert.IsSuccess(await manager.Create(user, password));
            var stamp = user.SecurityStamp;
            Assert.NotNull(stamp);
            IdentityResultAssert.IsSuccess(await manager.ChangePassword(user.Id, password, newPassword));
            Assert.Null(await manager.Find(user.UserName, password));
            Assert.Equal(user, await manager.Find(user.UserName, newPassword));
            Assert.NotEqual(stamp, user.SecurityStamp);
        }

        [Fact]
        public async Task CanAddRemoveUserClaim()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("ClaimsAddRemove");
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            Claim[] claims = { new Claim("c", "v"), new Claim("c2", "v2"), new Claim("c2", "v3") };
            foreach (Claim c in claims)
            {
                IdentityResultAssert.IsSuccess(await manager.AddClaim(user.Id, c));
            }
            var userClaims = await manager.GetClaims(user.Id);
            Assert.Equal(3, userClaims.Count);
            IdentityResultAssert.IsSuccess(await manager.RemoveClaim(user.Id, claims[0]));
            userClaims = await manager.GetClaims(user.Id);
            Assert.Equal(2, userClaims.Count);
            IdentityResultAssert.IsSuccess(await manager.RemoveClaim(user.Id, claims[1]));
            userClaims = await manager.GetClaims(user.Id);
            Assert.Equal(1, userClaims.Count);
            IdentityResultAssert.IsSuccess(await manager.RemoveClaim(user.Id, claims[2]));
            userClaims = await manager.GetClaims(user.Id);
            Assert.Equal(0, userClaims.Count);
        }

        [Fact]
        public async Task ChangePasswordFallsIfPasswordWrong()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("user");
            IdentityResultAssert.IsSuccess(await manager.Create(user, "password"));
            var result = await manager.ChangePassword(user.Id, "bogus", "newpassword");
            IdentityResultAssert.IsFailure(result, "Incorrect password.");
        }

        [Fact]
        public async Task AddDupeUserNameFails()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("dupe");
            var user2 = new InMemoryUser("dupe");
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            IdentityResultAssert.IsFailure(await manager.Create(user2), "Name dupe is already taken.");
        }

        [Fact]
        public async Task AddDupeEmailAllowedByDefault()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("dupe") {Email = "yup@yup.com"};
            var user2 = new InMemoryUser("dupeEmail") { Email = "yup@yup.com" };
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            IdentityResultAssert.IsSuccess(await manager.Create(user2));
        }

        [Fact]
        public async Task AddDupeEmailFallsWhenUniqueEmailRequired()
        {
            var manager = CreateManager();
            manager.UserValidator = new UserValidator<InMemoryUser, string> { RequireUniqueEmail = true };
            var user = new InMemoryUser("dupe") { Email = "yup@yup.com" };
            var user2 = new InMemoryUser("dupeEmail") { Email = "yup@yup.com" };
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            IdentityResultAssert.IsFailure(await manager.Create(user2), "Email 'yup@yup.com' is already taken.");
        }

        [Fact]
        public async Task UpdateSecurityStampActuallyChanges()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("stampMe");
            Assert.Null(user.SecurityStamp);
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            var stamp = user.SecurityStamp;
            Assert.NotNull(stamp);
            IdentityResultAssert.IsSuccess(await manager.UpdateSecurityStamp(user.Id));
            Assert.NotEqual(stamp, user.SecurityStamp);
        }

        [Fact]
        public async Task AddDupeLoginFails()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("DupeLogin");
            var login = new UserLoginInfo("provder", "key");
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            IdentityResultAssert.IsSuccess(await manager.AddLogin(user.Id, login));
            var result = await manager.AddLogin(user.Id, login);
            IdentityResultAssert.IsFailure(result, "A user with that external login already exists.");
        }

        // Email tests
        [Fact]
        public async Task CanFindByEmail()
        {
            var manager = CreateManager();
            const string userName = "EmailTest";
            const string email = "email@test.com";
            var user = new InMemoryUser(userName) { Email = email };
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            var fetch = await manager.FindByEmail(email);
            Assert.Equal(user, fetch);
        }

        [Fact]
        public async Task CanFindUsersViaUserQuerable()
        {
            var mgr = CreateManager();
            var users = new[]
            {
                new InMemoryUser("user1"),
                new InMemoryUser("user2"),
                new InMemoryUser("user3")
            };
            foreach (InMemoryUser u in users)
            {
                IdentityResultAssert.IsSuccess(await mgr.Create(u));
            }
            var usersQ = mgr.Users;
            Assert.Equal(3, usersQ.Count());
            Assert.NotNull(usersQ.FirstOrDefault(u => u.UserName == "user1"));
            Assert.NotNull(usersQ.FirstOrDefault(u => u.UserName == "user2"));
            Assert.NotNull(usersQ.FirstOrDefault(u => u.UserName == "user3"));
            Assert.Null(usersQ.FirstOrDefault(u => u.UserName == "bogus"));
        }

        [Fact]
        public async Task ConfirmEmailFalseByDefaultTest()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("test");
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            Assert.False(await manager.IsEmailConfirmed(user.Id));
        }

        // TODO: No token provider implementations yet
        private class StaticTokenProvider : IUserTokenProvider<InMemoryUser, string>
        {
            private static string MakeToken(string purpose, IUser<string> user)
            {
                return string.Join(":", user.Id, purpose, "ImmaToken");
            }

            public Task<string> Generate(string purpose, UserManager<InMemoryUser, string> manager,
                InMemoryUser user)
            {
                return Task.FromResult(MakeToken(purpose, user));
            }

            public Task<bool> Validate(string purpose, string token, UserManager<InMemoryUser, string> manager,
                InMemoryUser user)
            {
                return Task.FromResult(token == MakeToken(purpose, user));
            }

            public Task Notify(string token, UserManager<InMemoryUser, string> manager, InMemoryUser user)
            {
                return Task.FromResult(0);
            }

            public Task<bool> IsValidProviderForUser(UserManager<InMemoryUser, string> manager, InMemoryUser user)
            {
                return Task.FromResult(true);
            }
        }

        [Fact]
        public async Task CanResetPasswordWithStaticTokenProvider()
        {
            var manager = CreateManager();
            manager.UserTokenProvider = new StaticTokenProvider();
            var user = new InMemoryUser("ResetPasswordTest");
            const string password = "password";
            const string newPassword = "newpassword";
            IdentityResultAssert.IsSuccess(await manager.Create(user, password));
            var stamp = user.SecurityStamp;
            Assert.NotNull(stamp);
            var token = await manager.GeneratePasswordResetToken(user.Id);
            Assert.NotNull(token);
            IdentityResultAssert.IsSuccess(await manager.ResetPassword(user.Id, token, newPassword));
            Assert.Null(await manager.Find(user.UserName, password));
            Assert.Equal(user, await manager.Find(user.UserName, newPassword));
            Assert.NotEqual(stamp, user.SecurityStamp);
        }

        [Fact]
        public async Task PasswordValidatorCanBlockResetPasswordWithStaticTokenProvider()
        {
            var manager = CreateManager();
            manager.UserTokenProvider = new StaticTokenProvider();
            var user = new InMemoryUser("ResetPasswordTest");
            const string password = "password";
            const string newPassword = "newpassword";
            IdentityResultAssert.IsSuccess(await manager.Create(user, password));
            var stamp = user.SecurityStamp;
            Assert.NotNull(stamp);
            var token = await manager.GeneratePasswordResetToken(user.Id);
            Assert.NotNull(token);
            manager.PasswordValidator = new AlwaysBadValidator();
            IdentityResultAssert.IsFailure(await manager.ResetPassword(user.Id, token, newPassword), AlwaysBadValidator.ErrorMessage);
            Assert.NotNull(await manager.Find(user.UserName, password));
            Assert.Equal(user, await manager.Find(user.UserName, password));
            Assert.Equal(stamp, user.SecurityStamp);
        }

        [Fact]
        public async Task ResetPasswordWithStaticTokenProviderFailsWithWrongToken()
        {
            var manager = CreateManager();
            manager.UserTokenProvider = new StaticTokenProvider();
            var user = new InMemoryUser("ResetPasswordTest");
            const string password = "password";
            const string newPassword = "newpassword";
            IdentityResultAssert.IsSuccess(await manager.Create(user, password));
            var stamp = user.SecurityStamp;
            Assert.NotNull(stamp);
            IdentityResultAssert.IsFailure(await manager.ResetPassword(user.Id, "bogus", newPassword), "Invalid token.");
            Assert.NotNull(await manager.Find(user.UserName, password));
            Assert.Equal(user, await manager.Find(user.UserName, password));
            Assert.Equal(stamp, user.SecurityStamp);
        }

        [Fact]
        public async Task CanGenerateAndVerifyUserTokenWithStaticTokenProvider()
        {
            var manager = CreateManager();
            manager.UserTokenProvider = new StaticTokenProvider();
            var user = new InMemoryUser("UserTokenTest");
            var user2 = new InMemoryUser("UserTokenTest2");
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            IdentityResultAssert.IsSuccess(await manager.Create(user2));
            var token = await manager.GenerateUserToken("test", user.Id);
            Assert.True(await manager.VerifyUserToken(user.Id, "test", token));
            Assert.False(await manager.VerifyUserToken(user.Id, "test2", token));
            Assert.False(await manager.VerifyUserToken(user.Id, "test", token + "a"));
            Assert.False(await manager.VerifyUserToken(user2.Id, "test", token));
        }

        [Fact]
        public async Task CanConfirmEmailWithStaticToken()
        {
            var manager = CreateManager();
            manager.UserTokenProvider = new StaticTokenProvider();
            var user = new InMemoryUser("test");
            Assert.False(user.EmailConfirmed);
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            var token = await manager.GenerateEmailConfirmationToken(user.Id);
            Assert.NotNull(token);
            IdentityResultAssert.IsSuccess(await manager.ConfirmEmail(user.Id, token));
            Assert.True(await manager.IsEmailConfirmed(user.Id));
            IdentityResultAssert.IsSuccess(await manager.SetEmail(user.Id, null));
            Assert.False(await manager.IsEmailConfirmed(user.Id));
        }

        [Fact]
        public async Task ConfirmEmailWithStaticTokenFailsWithWrongToken()
        {
            var manager = CreateManager();
            manager.UserTokenProvider = new StaticTokenProvider();
            var user = new InMemoryUser("test");
            Assert.False(user.EmailConfirmed);
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            IdentityResultAssert.IsFailure(await manager.ConfirmEmail(user.Id, "bogus"), "Invalid token.");
            Assert.False(await manager.IsEmailConfirmed(user.Id));
        }

        // TODO: Can't reenable til we have a SecurityStamp linked token provider
        //[Fact]
        //public async Task ConfirmTokenFailsAfterPasswordChange()
        //{
        //    var manager = CreateManager();
        //    var user = new InMemoryUser("test");
        //    Assert.False(user.EmailConfirmed);
        //    IdentityResultAssert.IsSuccess(await manager.Create(user, "password"));
        //    var token = await manager.GenerateEmailConfirmationToken(user.Id);
        //    Assert.NotNull(token);
        //    IdentityResultAssert.IsSuccess(await manager.ChangePassword(user.Id, "password", "newpassword"));
        //    IdentityResultAssert.IsFailure(await manager.ConfirmEmail(user.Id, token), "Invalid token.");
        //    Assert.False(await manager.IsEmailConfirmed(user.Id));
        //}

        // Lockout tests

        [Fact]
        public async Task SingleFailureLockout()
        {
            var mgr = CreateManager();
            mgr.DefaultAccountLockoutTimeSpan = TimeSpan.FromHours(1);
            mgr.UserLockoutEnabledByDefault = true;
            var user = new InMemoryUser("fastLockout");
            IdentityResultAssert.IsSuccess(await mgr.Create(user));
            Assert.True(await mgr.GetLockoutEnabled(user.Id));
            Assert.True(user.LockoutEnabled);
            Assert.False(await mgr.IsLockedOut(user.Id));
            IdentityResultAssert.IsSuccess(await mgr.AccessFailed(user.Id));
            Assert.True(await mgr.IsLockedOut(user.Id));
            Assert.True(await mgr.GetLockoutEndDate(user.Id) > DateTimeOffset.UtcNow.AddMinutes(55));
            Assert.Equal(0, await mgr.GetAccessFailedCount(user.Id));
        }

        [Fact]
        public async Task TwoFailureLockout()
        {
            var mgr = CreateManager();
            mgr.DefaultAccountLockoutTimeSpan = TimeSpan.FromHours(1);
            mgr.UserLockoutEnabledByDefault = true;
            mgr.MaxFailedAccessAttemptsBeforeLockout = 2;
            var user = new InMemoryUser("twoFailureLockout");
            IdentityResultAssert.IsSuccess(await mgr.Create(user));
            Assert.True(await mgr.GetLockoutEnabled(user.Id));
            Assert.True(user.LockoutEnabled);
            Assert.False(await mgr.IsLockedOut(user.Id));
            IdentityResultAssert.IsSuccess(await mgr.AccessFailed(user.Id));
            Assert.False(await mgr.IsLockedOut(user.Id));
            Assert.False(await mgr.GetLockoutEndDate(user.Id) > DateTimeOffset.UtcNow.AddMinutes(55));
            Assert.Equal(1, await mgr.GetAccessFailedCount(user.Id));
            IdentityResultAssert.IsSuccess(await mgr.AccessFailed(user.Id));
            Assert.True(await mgr.IsLockedOut(user.Id));
            Assert.True(await mgr.GetLockoutEndDate(user.Id) > DateTimeOffset.UtcNow.AddMinutes(55));
            Assert.Equal(0, await mgr.GetAccessFailedCount(user.Id));
        }

        [Fact]
        public async Task ResetAccessCountPreventsLockout()
        {
            var mgr = CreateManager();
            mgr.DefaultAccountLockoutTimeSpan = TimeSpan.FromHours(1);
            mgr.UserLockoutEnabledByDefault = true;
            mgr.MaxFailedAccessAttemptsBeforeLockout = 2;
            var user = new InMemoryUser("resetLockout");
            IdentityResultAssert.IsSuccess(await mgr.Create(user));
            Assert.True(await mgr.GetLockoutEnabled(user.Id));
            Assert.True(user.LockoutEnabled);
            Assert.False(await mgr.IsLockedOut(user.Id));
            IdentityResultAssert.IsSuccess(await mgr.AccessFailed(user.Id));
            Assert.False(await mgr.IsLockedOut(user.Id));
            Assert.False(await mgr.GetLockoutEndDate(user.Id) > DateTimeOffset.UtcNow.AddMinutes(55));
            Assert.Equal(1, await mgr.GetAccessFailedCount(user.Id));
            IdentityResultAssert.IsSuccess(await mgr.ResetAccessFailedCount(user.Id));
            Assert.Equal(0, await mgr.GetAccessFailedCount(user.Id));
            Assert.False(await mgr.IsLockedOut(user.Id));
            Assert.False(await mgr.GetLockoutEndDate(user.Id) > DateTimeOffset.UtcNow.AddMinutes(55));
            IdentityResultAssert.IsSuccess(await mgr.AccessFailed(user.Id));
            Assert.False(await mgr.IsLockedOut(user.Id));
            Assert.False(await mgr.GetLockoutEndDate(user.Id) > DateTimeOffset.UtcNow.AddMinutes(55));
            Assert.Equal(1, await mgr.GetAccessFailedCount(user.Id));
        }

        [Fact]
        public async Task CanEnableLockoutManuallyAndLockout()
        {
            var mgr = CreateManager();
            mgr.DefaultAccountLockoutTimeSpan = TimeSpan.FromHours(1);
            mgr.MaxFailedAccessAttemptsBeforeLockout = 2;
            var user = new InMemoryUser("manualLockout");
            IdentityResultAssert.IsSuccess(await mgr.Create(user));
            Assert.False(await mgr.GetLockoutEnabled(user.Id));
            Assert.False(user.LockoutEnabled);
            IdentityResultAssert.IsSuccess(await mgr.SetLockoutEnabled(user.Id, true));
            Assert.True(await mgr.GetLockoutEnabled(user.Id));
            Assert.True(user.LockoutEnabled);
            Assert.False(await mgr.IsLockedOut(user.Id));
            IdentityResultAssert.IsSuccess(await mgr.AccessFailed(user.Id));
            Assert.False(await mgr.IsLockedOut(user.Id));
            Assert.False(await mgr.GetLockoutEndDate(user.Id) > DateTimeOffset.UtcNow.AddMinutes(55));
            Assert.Equal(1, await mgr.GetAccessFailedCount(user.Id));
            IdentityResultAssert.IsSuccess(await mgr.AccessFailed(user.Id));
            Assert.True(await mgr.IsLockedOut(user.Id));
            Assert.True(await mgr.GetLockoutEndDate(user.Id) > DateTimeOffset.UtcNow.AddMinutes(55));
            Assert.Equal(0, await mgr.GetAccessFailedCount(user.Id));
        }

        [Fact]
        public async Task UserNotLockedOutWithNullDateTimeAndIsSetToNullDate()
        {
            var mgr = CreateManager();
            mgr.UserLockoutEnabledByDefault = true;
            var user = new InMemoryUser("LockoutTest");
            IdentityResultAssert.IsSuccess(await mgr.Create(user));
            Assert.True(await mgr.GetLockoutEnabled(user.Id));
            Assert.True(user.LockoutEnabled);
            IdentityResultAssert.IsSuccess(await mgr.SetLockoutEndDate(user.Id, new DateTimeOffset()));
            Assert.False(await mgr.IsLockedOut(user.Id));
            Assert.Equal(new DateTimeOffset(), await mgr.GetLockoutEndDate(user.Id));
            Assert.Equal(new DateTimeOffset(), user.LockoutEnd);
        }

        [Fact]
        public async Task LockoutFailsIfNotEnabled()
        {
            var mgr = CreateManager();
            var user = new InMemoryUser("LockoutNotEnabledTest");
            IdentityResultAssert.IsSuccess(await mgr.Create(user));
            Assert.False(await mgr.GetLockoutEnabled(user.Id));
            Assert.False(user.LockoutEnabled);
            IdentityResultAssert.IsFailure(await mgr.SetLockoutEndDate(user.Id, new DateTimeOffset()), "Lockout is not enabled for this user.");
            Assert.False(await mgr.IsLockedOut(user.Id));
        }

        [Fact]
        public async Task LockoutEndToUtcNowMinus1SecInUserShouldNotBeLockedOut()
        {
            var mgr = CreateManager();
            mgr.UserLockoutEnabledByDefault = true;
            var user = new InMemoryUser("LockoutUtcNowTest") { LockoutEnd = DateTimeOffset.UtcNow.AddSeconds(-1) };
            IdentityResultAssert.IsSuccess(await mgr.Create(user));
            Assert.True(await mgr.GetLockoutEnabled(user.Id));
            Assert.True(user.LockoutEnabled);
            Assert.False(await mgr.IsLockedOut(user.Id));
        }

        [Fact]
        public async Task LockoutEndToUtcNowSubOneSecondWithManagerShouldNotBeLockedOut()
        {
            var mgr = CreateManager();
            mgr.UserLockoutEnabledByDefault = true;
            var user = new InMemoryUser("LockoutUtcNowTest");
            IdentityResultAssert.IsSuccess(await mgr.Create(user));
            Assert.True(await mgr.GetLockoutEnabled(user.Id));
            Assert.True(user.LockoutEnabled);
            IdentityResultAssert.IsSuccess(await mgr.SetLockoutEndDate(user.Id, DateTimeOffset.UtcNow.AddSeconds(-1)));
            Assert.False(await mgr.IsLockedOut(user.Id));
        }

        [Fact]
        public async Task LockoutEndToUtcNowPlus5ShouldBeLockedOut()
        {
            var mgr = CreateManager();
            mgr.UserLockoutEnabledByDefault = true;
            var user = new InMemoryUser("LockoutUtcNowTest") { LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(5) };
            IdentityResultAssert.IsSuccess(await mgr.Create(user));
            Assert.True(await mgr.GetLockoutEnabled(user.Id));
            Assert.True(user.LockoutEnabled);
            Assert.True(await mgr.IsLockedOut(user.Id));
        }

        [Fact]
        public async Task UserLockedOutWithDateTimeLocalKindNowPlus30()
        {
            var mgr = CreateManager();
            mgr.UserLockoutEnabledByDefault = true;
            var user = new InMemoryUser("LockoutTest");
            IdentityResultAssert.IsSuccess(await mgr.Create(user));
            Assert.True(await mgr.GetLockoutEnabled(user.Id));
            Assert.True(user.LockoutEnabled);
            var lockoutEnd = new DateTimeOffset(DateTime.Now.AddMinutes(30).ToLocalTime());
            IdentityResultAssert.IsSuccess(await mgr.SetLockoutEndDate(user.Id, lockoutEnd));
            Assert.True(await mgr.IsLockedOut(user.Id));
            var end = await mgr.GetLockoutEndDate(user.Id);
            Assert.Equal(lockoutEnd, end);
        }

        // Role Tests
        [Fact]
        public async Task CanCreateRoleTest()
        {
            var manager = CreateRoleManager();
            var role = new InMemoryRole("create");
            Assert.False(await manager.RoleExists(role.Name));
            IdentityResultAssert.IsSuccess(await manager.Create(role));
            Assert.True(await manager.RoleExists(role.Name));
        }

        private class AlwaysBadValidator : IUserValidator<InMemoryUser, string>, IRoleValidator<InMemoryRole, string>, IPasswordValidator
        {
            public const string ErrorMessage = "I'm Bad.";

            public Task<IdentityResult> Validate(UserManager<InMemoryUser, string> manager, InMemoryUser user)
            {
                return Task.FromResult(IdentityResult.Failed(ErrorMessage));
            }

            public Task<IdentityResult> Validate(RoleManager<InMemoryRole, string> manager, InMemoryRole role)
            {
                return Task.FromResult(IdentityResult.Failed(ErrorMessage));
            }

            public Task<IdentityResult> Validate(string password)
            {
                return Task.FromResult(IdentityResult.Failed(ErrorMessage));
            }
        }

        [Fact]
        public async Task BadValidatorBlocksCreateRole()
        {
            var manager = CreateRoleManager();
            manager.RoleValidator = new AlwaysBadValidator();
            IdentityResultAssert.IsFailure(await manager.Create(new InMemoryRole("blocked")),
                AlwaysBadValidator.ErrorMessage);
        }

        [Fact]
        public async Task BadValidatorBlocksRoleUpdate()
        {
            var manager = CreateRoleManager();
            var role = new InMemoryRole("poorguy");
            IdentityResultAssert.IsSuccess(await manager.Create(role));
            var error = AlwaysBadValidator.ErrorMessage;
            manager.RoleValidator = new AlwaysBadValidator();
            IdentityResultAssert.IsFailure(await manager.Update(role), error);
        }

        [Fact]
        public async Task CanDeleteRoleTest()
        {
            var manager = CreateRoleManager();
            var role = new InMemoryRole("delete");
            Assert.False(await manager.RoleExists(role.Name));
            IdentityResultAssert.IsSuccess(await manager.Create(role));
            IdentityResultAssert.IsSuccess(await manager.Delete(role));
            Assert.False(await manager.RoleExists(role.Name));
        }

        [Fact]
        public async Task CanRoleFindByIdTest()
        {
            var manager = CreateRoleManager();
            var role = new InMemoryRole("FindById");
            Assert.Null(await manager.FindById(role.Id));
            IdentityResultAssert.IsSuccess(await manager.Create(role));
            Assert.Equal(role, await manager.FindById(role.Id));
        }

        [Fact]
        public async Task CanRoleFindByName()
        {
            var manager = CreateRoleManager();
            var role = new InMemoryRole("FindByName");
            Assert.Null(await manager.FindByName(role.Name));
            Assert.False(await manager.RoleExists(role.Name));
            IdentityResultAssert.IsSuccess(await manager.Create(role));
            Assert.Equal(role, await manager.FindByName(role.Name));
        }

        [Fact]
        public async Task CanUpdateRoleNameTest()
        {
            var manager = CreateRoleManager();
            var role = new InMemoryRole("update");
            Assert.False(await manager.RoleExists(role.Name));
            IdentityResultAssert.IsSuccess(await manager.Create(role));
            Assert.True(await manager.RoleExists(role.Name));
            role.Name = "Changed";
            IdentityResultAssert.IsSuccess(await manager.Update(role));
            Assert.False(await manager.RoleExists("update"));
            Assert.Equal(role, await manager.FindByName(role.Name));
        }

        [Fact]
        public async Task CanQuerableRolesTest()
        {
            var manager = CreateRoleManager();
            InMemoryRole[] roles =
            {
                new InMemoryRole("r1"), new InMemoryRole("r2"), new InMemoryRole("r3"),
                new InMemoryRole("r4")
            };
            foreach (var r in roles)
            {
                IdentityResultAssert.IsSuccess(await manager.Create(r));
            }
            Assert.Equal(roles.Length, manager.Roles.Count());
            var r1 = manager.Roles.FirstOrDefault(r => r.Name == "r1");
            Assert.Equal(roles[0], r1);
        }

        //[Fact]
        //public async Task DeleteRoleNonEmptySucceedsTest()
        //{
        //    // Need fail if not empty?
        //    var userMgr = CreateManager();
        //    var roleMgr = CreateRoleManager();
        //    var role = new InMemoryRole("deleteNonEmpty");
        //    Assert.False(await roleMgr.RoleExists(role.Name));
        //    IdentityResultAssert.IsSuccess(await roleMgr.Create(role));
        //    var user = new InMemoryUser("t");
        //    IdentityResultAssert.IsSuccess(await userMgr.Create(user));
        //    IdentityResultAssert.IsSuccess(await userMgr.AddToRole(user.Id, role.Name));
        //    IdentityResultAssert.IsSuccess(await roleMgr.Delete(role));
        //    Assert.Null(await roleMgr.FindByName(role.Name));
        //    Assert.False(await roleMgr.RoleExists(role.Name));
        //    // REVIEW: We should throw if deleteing a non empty role?
        //    var roles = await userMgr.GetRoles(user.Id);

        //    // In memory this doesn't work since there's no concept of cascading deletes
        //    //Assert.Equal(0, roles.Count());
        //}

        ////[Fact]
        ////public async Task DeleteUserRemovesFromRoleTest()
        ////{
        ////    // Need fail if not empty?
        ////    var userMgr = CreateManager();
        ////    var roleMgr = CreateRoleManager();
        ////    var role = new InMemoryRole("deleteNonEmpty");
        ////    Assert.False(await roleMgr.RoleExists(role.Name));
        ////    IdentityResultAssert.IsSuccess(await roleMgr.Create(role));
        ////    var user = new InMemoryUser("t");
        ////    IdentityResultAssert.IsSuccess(await userMgr.Create(user));
        ////    IdentityResultAssert.IsSuccess(await userMgr.AddToRole(user.Id, role.Name));
        ////    IdentityResultAssert.IsSuccess(await userMgr.Delete(user));
        ////    role = roleMgr.FindById(role.Id);
        ////}

        [Fact]
        public async Task CreateRoleFailsIfExists()
        {
            var manager = CreateRoleManager();
            var role = new InMemoryRole("dupeRole");
            Assert.False(await manager.RoleExists(role.Name));
            IdentityResultAssert.IsSuccess(await manager.Create(role));
            Assert.True(await manager.RoleExists(role.Name));
            var role2 = new InMemoryRole("dupeRole");
            IdentityResultAssert.IsFailure(await manager.Create(role2));
        }

        [Fact]
        public async Task CanAddUsersToRole()
        {
            var manager = CreateManager();
            var roleManager = CreateRoleManager();
            var role = new InMemoryRole("addUserTest");
            IdentityResultAssert.IsSuccess(await roleManager.Create(role));
            InMemoryUser[] users =
            {
                new InMemoryUser("1"), new InMemoryUser("2"), new InMemoryUser("3"),
                new InMemoryUser("4")
            };
            foreach (var u in users)
            {
                IdentityResultAssert.IsSuccess(await manager.Create(u));
                IdentityResultAssert.IsSuccess(await manager.AddToRole(u.Id, role.Name));
                Assert.True(await manager.IsInRole(u.Id, role.Name));
            }
        }

        [Fact]
        public async Task CanGetRolesForUser()
        {
            var userManager = CreateManager();
            var roleManager = CreateRoleManager();
            InMemoryUser[] users =
            {
                new InMemoryUser("u1"), new InMemoryUser("u2"), new InMemoryUser("u3"),
                new InMemoryUser("u4")
            };
            InMemoryRole[] roles =
            {
                new InMemoryRole("r1"), new InMemoryRole("r2"), new InMemoryRole("r3"),
                new InMemoryRole("r4")
            };
            foreach (var u in users)
            {
                IdentityResultAssert.IsSuccess(await userManager.Create(u));
            }
            foreach (var r in roles)
            {
                IdentityResultAssert.IsSuccess(await roleManager.Create(r));
                foreach (var u in users)
                {
                    IdentityResultAssert.IsSuccess(await userManager.AddToRole(u.Id, r.Name));
                    Assert.True(await userManager.IsInRole(u.Id, r.Name));
                }
            }

            foreach (var u in users)
            {
                var rs = await userManager.GetRoles(u.Id);
                Assert.Equal(roles.Length, rs.Count);
                foreach (var r in roles)
                {
                    Assert.True(rs.Any(role => role == r.Name));
                }
            }
        }


        [Fact]
        public async Task RemoveUserFromRoleWithMultipleRoles()
        {
            var userManager = CreateManager();
            var roleManager = CreateRoleManager();
            var user = new InMemoryUser("MultiRoleUser");
            IdentityResultAssert.IsSuccess(await userManager.Create(user));
            InMemoryRole[] roles =
            {
                new InMemoryRole("r1"), new InMemoryRole("r2"), new InMemoryRole("r3"),
                new InMemoryRole("r4")
            };
            foreach (var r in roles)
            {
                IdentityResultAssert.IsSuccess(await roleManager.Create(r));
                IdentityResultAssert.IsSuccess(await userManager.AddToRole(user.Id, r.Name));
                Assert.True(await userManager.IsInRole(user.Id, r.Name));
            }
            IdentityResultAssert.IsSuccess(await userManager.RemoveFromRole(user.Id, roles[2].Name));
            Assert.False(await userManager.IsInRole(user.Id, roles[2].Name));
        }

        [Fact]
        public async Task CanRemoveUsersFromRole()
        {
            var userManager = CreateManager();
            var roleManager = CreateRoleManager();
            InMemoryUser[] users =
            {
                new InMemoryUser("1"), new InMemoryUser("2"), new InMemoryUser("3"),
                new InMemoryUser("4")
            };
            foreach (var u in users)
            {
                IdentityResultAssert.IsSuccess(await userManager.Create(u));
            }
            var r = new InMemoryRole("r1");
            IdentityResultAssert.IsSuccess(await roleManager.Create(r));
            foreach (var u in users)
            {
                IdentityResultAssert.IsSuccess(await userManager.AddToRole(u.Id, r.Name));
                Assert.True(await userManager.IsInRole(u.Id, r.Name));
            }
            foreach (var u in users)
            {
                IdentityResultAssert.IsSuccess(await userManager.RemoveFromRole(u.Id, r.Name));
                Assert.False(await userManager.IsInRole(u.Id, r.Name));
            }
        }

        [Fact]
        public async Task RemoveUserNotInRoleFails()
        {
            var userMgr = CreateManager();
            var roleMgr = CreateRoleManager();
            var role = new InMemoryRole("addUserDupeTest");
            var user = new InMemoryUser("user1");
            IdentityResultAssert.IsSuccess(await userMgr.Create(user));
            IdentityResultAssert.IsSuccess(await roleMgr.Create(role));
            var result = await userMgr.RemoveFromRole(user.Id, role.Name);
            IdentityResultAssert.IsFailure(result, "User is not in role.");
        }

        [Fact]
        public async Task AddUserToRoleFailsIfAlreadyInRole()
        {
            var userMgr = CreateManager();
            var roleMgr = CreateRoleManager();
            var role = new InMemoryRole("addUserDupeTest");
            var user = new InMemoryUser("user1");
            IdentityResultAssert.IsSuccess(await userMgr.Create(user));
            IdentityResultAssert.IsSuccess(await roleMgr.Create(role));
            IdentityResultAssert.IsSuccess(await userMgr.AddToRole(user.Id, role.Name));
            Assert.True(await userMgr.IsInRole(user.Id, role.Name));
            IdentityResultAssert.IsFailure(await userMgr.AddToRole(user.Id, role.Name), "User already in role.");
        }

        [Fact]
        public async Task CanFindRoleByNameWithManager()
        {
            var roleMgr = CreateRoleManager();
            var role = new InMemoryRole("findRoleByNameTest");
            IdentityResultAssert.IsSuccess(await roleMgr.Create(role));
            Assert.Equal(role.Id, (await roleMgr.FindByName(role.Name)).Id);
        }

        [Fact]
        public async Task CanFindRoleWithManager()
        {
            var roleMgr = CreateRoleManager();
            var role = new InMemoryRole("findRoleTest");
            IdentityResultAssert.IsSuccess(await roleMgr.Create(role));
            Assert.Equal(role.Name, (await roleMgr.FindById(role.Id)).Name);
        }

        [Fact]
        public async Task SetPhoneNumberTest()
        {
            var manager = CreateManager();
            var userName = "PhoneTest";
            var user = new InMemoryUser(userName);
            user.PhoneNumber = "123-456-7890";
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            var stamp = await manager.GetSecurityStamp(user.Id);
            Assert.Equal(await manager.GetPhoneNumber(user.Id), "123-456-7890");
            IdentityResultAssert.IsSuccess(await manager.SetPhoneNumber(user.Id, "111-111-1111"));
            Assert.Equal(await manager.GetPhoneNumber(user.Id), "111-111-1111");
            Assert.NotEqual(stamp, user.SecurityStamp);
        }

        [Fact]
        public async Task CanChangePhoneNumber()
        {
            var manager = CreateManager();
            const string userName = "PhoneTest";
            var user = new InMemoryUser(userName) {PhoneNumber = "123-456-7890"};
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            Assert.False(await manager.IsPhoneNumberConfirmed(user.Id));
            var stamp = await manager.GetSecurityStamp(user.Id);
            var token1 = await manager.GenerateChangePhoneNumberToken(user.Id, "111-111-1111");
            IdentityResultAssert.IsSuccess(await manager.ChangePhoneNumber(user.Id, "111-111-1111", token1));
            Assert.True(await manager.IsPhoneNumberConfirmed(user.Id));
            Assert.Equal(await manager.GetPhoneNumber(user.Id), "111-111-1111");
            Assert.NotEqual(stamp, user.SecurityStamp);
        }

        [Fact]
        public async Task ChangePhoneNumberFailsWithWrongToken()
        {
            var manager = CreateManager();
            const string userName = "PhoneTest";
            var user = new InMemoryUser(userName) {PhoneNumber = "123-456-7890"};
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            Assert.False(await manager.IsPhoneNumberConfirmed(user.Id));
            var stamp = await manager.GetSecurityStamp(user.Id);
            IdentityResultAssert.IsFailure(await manager.ChangePhoneNumber(user.Id, "111-111-1111", "bogus"),
                "Invalid token.");
            Assert.False(await manager.IsPhoneNumberConfirmed(user.Id));
            Assert.Equal(await manager.GetPhoneNumber(user.Id), "123-456-7890");
            Assert.Equal(stamp, user.SecurityStamp);
        }

        [Fact]
        public async Task CanVerifyPhoneNumber()
        {
            var manager = CreateManager();
            const string userName = "VerifyPhoneTest";
            var user = new InMemoryUser(userName);
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            const string num1 = "111-123-4567";
            const string num2 = "111-111-1111";
            var token1 = await manager.GenerateChangePhoneNumberToken(user.Id, num1);
            var token2 = await manager.GenerateChangePhoneNumberToken(user.Id, num2);
            Assert.NotEqual(token1, token2);
            Assert.True(await manager.VerifyChangePhoneNumberToken(user.Id, token1, num1));
            Assert.True(await manager.VerifyChangePhoneNumberToken(user.Id, token2, num2));
            Assert.False(await manager.VerifyChangePhoneNumberToken(user.Id, token2, num1));
            Assert.False(await manager.VerifyChangePhoneNumberToken(user.Id, token1, num2));
        }

        private class EmailTokenProvider : IUserTokenProvider<InMemoryUser, string>
        {
            private static string MakeToken(string purpose)
            {
                return "email:" + purpose;
            }

            public Task<string> Generate(string purpose, UserManager<InMemoryUser, string> manager, InMemoryUser user)
            {
                return Task.FromResult(MakeToken(purpose));
            }

            public Task<bool> Validate(string purpose, string token, UserManager<InMemoryUser, string> manager, InMemoryUser user)
            {
                return Task.FromResult(token == MakeToken(purpose));
            }

            public Task Notify(string token, UserManager<InMemoryUser, string> manager, InMemoryUser user)
            {
                return manager.SendEmail(user.Id, token, token);
            }

            public async Task<bool> IsValidProviderForUser(UserManager<InMemoryUser, string> manager, InMemoryUser user)
            {
                return !string.IsNullOrEmpty(await manager.GetEmail(user.Id));
            }
        }

        private class SmsTokenProvider : IUserTokenProvider<InMemoryUser, string>
        {
            private static string MakeToken(string purpose)
            {
                return "sms:" + purpose;
            }

            public Task<string> Generate(string purpose, UserManager<InMemoryUser, string> manager, InMemoryUser user)
            {
                return Task.FromResult(MakeToken(purpose));
            }

            public Task<bool> Validate(string purpose, string token, UserManager<InMemoryUser, string> manager, InMemoryUser user)
            {
                return Task.FromResult(token == MakeToken(purpose));
            }

            public Task Notify(string token, UserManager<InMemoryUser, string> manager, InMemoryUser user)
            {
                return manager.SendSms(user.Id, token);
            }

            public async Task<bool> IsValidProviderForUser(UserManager<InMemoryUser, string> manager, InMemoryUser user)
            {
                return !string.IsNullOrEmpty(await manager.GetPhoneNumber(user.Id));
            }
        }

        [Fact]
        public async Task CanEmailTwoFactorToken()
        {
            var manager = CreateManager();
            var messageService = new TestMessageService();
            manager.EmailService = messageService;
            const string factorId = "EmailCode";
            manager.RegisterTwoFactorProvider(factorId, new EmailTokenProvider());
            var user = new InMemoryUser("EmailCodeTest") { Email = "foo@foo.com" };
            const string password = "password";
            IdentityResultAssert.IsSuccess(await manager.Create(user, password));
            var stamp = user.SecurityStamp;
            Assert.NotNull(stamp);
            var token = await manager.GenerateTwoFactorToken(user.Id, factorId);
            Assert.NotNull(token);
            Assert.Null(messageService.Message);
            IdentityResultAssert.IsSuccess(await manager.NotifyTwoFactorToken(user.Id, factorId, token));
            Assert.NotNull(messageService.Message);
            Assert.Equal(token, messageService.Message.Subject);
            Assert.Equal(token, messageService.Message.Body);
            Assert.True(await manager.VerifyTwoFactorToken(user.Id, factorId, token));
        }

        [Fact]
        public async Task NotifyWithUnknownProviderFails()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("NotifyFail");
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            await ExceptionAssert.ThrowsAsync<NotSupportedException>(async () => await manager.NotifyTwoFactorToken(user.Id, "Bogus", "token"), "No IUserTwoFactorProvider for 'Bogus' is registered.");
        }


        //[Fact]
        //public async Task EmailTokenFactorWithFormatTest()
        //{
        //    var manager = CreateManager();
        //    var messageService = new TestMessageService();
        //    manager.EmailService = messageService;
        //    const string factorId = "EmailCode";
        //    manager.RegisterTwoFactorProvider(factorId, new EmailTokenProvider<InMemoryUser>
        //    {
        //        Subject = "Security Code",
        //        BodyFormat = "Your code is: {0}"
        //    });
        //    var user = new InMemoryUser("EmailCodeTest") { Email = "foo@foo.com" };
        //    const string password = "password";
        //    IdentityResultAssert.IsSuccess(await manager.Create(user, password));
        //    var stamp = user.SecurityStamp;
        //    Assert.NotNull(stamp);
        //    var token = await manager.GenerateTwoFactorToken(user.Id, factorId);
        //    Assert.NotNull(token);
        //    Assert.Null(messageService.Message);
        //    IdentityResultAssert.IsSuccess(await manager.NotifyTwoFactorToken(user.Id, factorId, token));
        //    Assert.NotNull(messageService.Message);
        //    Assert.Equal("Security Code", messageService.Message.Subject);
        //    Assert.Equal("Your code is: " + token, messageService.Message.Body);
        //    Assert.True(await manager.VerifyTwoFactorToken(user.Id, factorId, token));
        //}

        //[Fact]
        //public async Task EmailFactorFailsAfterSecurityStampChangeTest()
        //{
        //    var manager = CreateManager();
        //    const string factorId = "EmailCode";
        //    manager.RegisterTwoFactorProvider(factorId, new EmailTokenProvider<InMemoryUser>());
        //    var user = new InMemoryUser("EmailCodeTest") { Email = "foo@foo.com" };
        //    IdentityResultAssert.IsSuccess(await manager.Create(user));
        //    var stamp = user.SecurityStamp;
        //    Assert.NotNull(stamp);
        //    var token = await manager.GenerateTwoFactorToken(user.Id, factorId);
        //    Assert.NotNull(token);
        //    IdentityResultAssert.IsSuccess(await manager.UpdateSecurityStamp(user.Id));
        //    Assert.False(await manager.VerifyTwoFactorToken(user.Id, factorId, token));
        //}

        [Fact]
        public async Task EnableTwoFactorChangesSecurityStamp()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("TwoFactorEnabledTest");
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            var stamp = user.SecurityStamp;
            Assert.NotNull(stamp);
            IdentityResultAssert.IsSuccess(await manager.SetTwoFactorEnabled(user.Id, true));
            Assert.NotEqual(stamp, await manager.GetSecurityStamp(user.Id));
            Assert.True(await manager.GetTwoFactorEnabled(user.Id));
        }

        [Fact]
        public async Task CanSendSms()
        {
            var manager = CreateManager();
            var messageService = new TestMessageService();
            manager.SmsService = messageService;
            var user = new InMemoryUser("SmsTest") { PhoneNumber = "4251234567" };
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            await manager.SendSms(user.Id, "Hi");
            Assert.NotNull(messageService.Message);
            Assert.Equal("Hi", messageService.Message.Body);
        }

        [Fact]
        public async Task CanSendEmail()
        {
            var manager = CreateManager();
            var messageService = new TestMessageService();
            manager.EmailService = messageService;
            var user = new InMemoryUser("EmailTest") { Email = "foo@foo.com" };
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            await manager.SendEmail(user.Id, "Hi", "Body");
            Assert.NotNull(messageService.Message);
            Assert.Equal("Hi", messageService.Message.Subject);
            Assert.Equal("Body", messageService.Message.Body);
        }

        [Fact]
        public async Task CanSmsTwoFactorToken()
        {
            var manager = CreateManager();
            var messageService = new TestMessageService();
            manager.SmsService = messageService;
            const string factorId = "PhoneCode";
            manager.RegisterTwoFactorProvider(factorId, new SmsTokenProvider());
            var user = new InMemoryUser("PhoneCodeTest") { PhoneNumber = "4251234567" };
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            var stamp = user.SecurityStamp;
            Assert.NotNull(stamp);
            var token = await manager.GenerateTwoFactorToken(user.Id, factorId);
            Assert.NotNull(token);
            Assert.Null(messageService.Message);
            IdentityResultAssert.IsSuccess(await manager.NotifyTwoFactorToken(user.Id, factorId, token));
            Assert.NotNull(messageService.Message);
            Assert.Equal(token, messageService.Message.Body);
            Assert.True(await manager.VerifyTwoFactorToken(user.Id, factorId, token));
        }

        //[Fact]
        //public async Task PhoneTokenFactorFormatTest()
        //{
        //    var manager = CreateManager();
        //    var messageService = new TestMessageService();
        //    manager.SmsService = messageService;
        //    const string factorId = "PhoneCode";
        //    manager.RegisterTwoFactorProvider(factorId, new PhoneNumberTokenProvider<InMemoryUser>
        //    {
        //        MessageFormat = "Your code is: {0}"
        //    });
        //    var user = new InMemoryUser("PhoneCodeTest") { PhoneNumber = "4251234567" };
        //    IdentityResultAssert.IsSuccess(await manager.Create(user));
        //    var stamp = user.SecurityStamp;
        //    Assert.NotNull(stamp);
        //    var token = await manager.GenerateTwoFactorToken(user.Id, factorId);
        //    Assert.NotNull(token);
        //    Assert.Null(messageService.Message);
        //    IdentityResultAssert.IsSuccess(await manager.NotifyTwoFactorToken(user.Id, factorId, token));
        //    Assert.NotNull(messageService.Message);
        //    Assert.Equal("Your code is: " + token, messageService.Message.Body);
        //    Assert.True(await manager.VerifyTwoFactorToken(user.Id, factorId, token));
        //}

        [Fact]
        public async Task GenerateTwoFactorWithUnknownFactorProviderWillThrow()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("PhoneCodeTest");
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            const string error = "No IUserTwoFactorProvider for 'bogus' is registered.";
            await ExceptionAssert.ThrowsAsync<NotSupportedException>(() => manager.GenerateTwoFactorToken(user.Id, "bogus"), error);
            await ExceptionAssert.ThrowsAsync<NotSupportedException>(
                () => manager.VerifyTwoFactorToken(user.Id, "bogus", "bogus"), error);
        }

        [Fact]
        public async Task GetValidTwoFactorTestEmptyWithNoProviders()
        {
            var manager = CreateManager();
            var user = new InMemoryUser("test");
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            var factors = await manager.GetValidTwoFactorProviders(user.Id);
            Assert.NotNull(factors);
            Assert.True(!factors.Any());
        }

        [Fact]
        public async Task GetValidTwoFactorTest()
        {
            var manager = CreateManager();
            manager.RegisterTwoFactorProvider("phone", new SmsTokenProvider());
            manager.RegisterTwoFactorProvider("email", new EmailTokenProvider());
            var user = new InMemoryUser("test");
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            var factors = await manager.GetValidTwoFactorProviders(user.Id);
            Assert.NotNull(factors);
            Assert.True(!factors.Any());
            IdentityResultAssert.IsSuccess(await manager.SetPhoneNumber(user.Id, "111-111-1111"));
            factors = await manager.GetValidTwoFactorProviders(user.Id);
            Assert.NotNull(factors);
            Assert.True(factors.Count() == 1);
            Assert.Equal("phone", factors[0]);
            IdentityResultAssert.IsSuccess(await manager.SetEmail(user.Id, "test@test.com"));
            factors = await manager.GetValidTwoFactorProviders(user.Id);
            Assert.NotNull(factors);
            Assert.True(factors.Count() == 2);
            IdentityResultAssert.IsSuccess(await manager.SetEmail(user.Id, null));
            factors = await manager.GetValidTwoFactorProviders(user.Id);
            Assert.NotNull(factors);
            Assert.True(factors.Count() == 1);
            Assert.Equal("phone", factors[0]);
        }

        //[Fact]
        //public async Task PhoneFactorFailsAfterSecurityStampChangeTest()
        //{
        //    var manager = CreateManager();
        //    var factorId = "PhoneCode";
        //    manager.RegisterTwoFactorProvider(factorId, new PhoneNumberTokenProvider<InMemoryUser>());
        //    var user = new InMemoryUser("PhoneCodeTest");
        //    user.PhoneNumber = "4251234567";
        //    IdentityResultAssert.IsSuccess(await manager.Create(user));
        //    var stamp = user.SecurityStamp;
        //    Assert.NotNull(stamp);
        //    var token = await manager.GenerateTwoFactorToken(user.Id, factorId);
        //    Assert.NotNull(token);
        //    IdentityResultAssert.IsSuccess(await manager.UpdateSecurityStamp(user.Id));
        //    Assert.False(await manager.VerifyTwoFactorToken(user.Id, factorId, token));
        //}

        [Fact]
        public async Task VerifyTokenFromWrongTokenProviderFails()
        {
            var manager = CreateManager();
            manager.RegisterTwoFactorProvider("PhoneCode", new SmsTokenProvider());
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider());
            var user = new InMemoryUser("WrongTokenProviderTest") {PhoneNumber = "4251234567"};
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            var token = await manager.GenerateTwoFactorToken(user.Id, "PhoneCode");
            Assert.NotNull(token);
            Assert.False(await manager.VerifyTwoFactorToken(user.Id, "EmailCode", token));
        }

        [Fact]
        public async Task VerifyWithWrongSmsTokenFails()
        {
            var manager = CreateManager();
            const string factorId = "PhoneCode";
            manager.RegisterTwoFactorProvider(factorId, new SmsTokenProvider());
            var user = new InMemoryUser("PhoneCodeTest") {PhoneNumber = "4251234567"};
            IdentityResultAssert.IsSuccess(await manager.Create(user));
            Assert.False(await manager.VerifyTwoFactorToken(user.Id, factorId, "bogus"));
        }

        private static UserManager<InMemoryUser, string> CreateManager()
        {
            return new UserManager<InMemoryUser, string>(new InMemoryUserStore<InMemoryUser>());
        }

        private static RoleManager<InMemoryRole> CreateRoleManager()
        {
            return new RoleManager<InMemoryRole>(new InMemoryRoleStore());
        }

        public class TestMessageService : IIdentityMessageService
        {
            public IdentityMessage Message { get; set; }

            public Task Send(IdentityMessage message)
            {
                Message = message;
                return Task.FromResult(0);
            }
        }
    }
}