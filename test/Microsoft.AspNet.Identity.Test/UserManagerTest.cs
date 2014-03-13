using System.Collections.Generic;
using System.Security.Claims;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Identity.Test
{
    public class UserManagerTest
    {
        [Fact]
        public void ServiceProviderWireupTest()
        {
            var manager = new UserManager<TestUser, string>(TestServices.DefaultServiceProvider<TestUser, string>());
            Assert.NotNull(manager.PasswordHasher);
            Assert.NotNull(manager.PasswordValidator);
            Assert.NotNull(manager.UserValidator);
        }

         //TODO: Mock fails in K (this works fine in net45)
        //[Fact]
        //public async Task CreateTest()
        //{
        //    // Setup
        //    var store = new Mock<IUserStore<TestUser, string>>();
        //    var user = new TestUser();
        //    store.Setup(s => s.Create(user)).Verifiable();
        //    var userManager = new UserManager<TestUser, string>(store.Object);

        //    // Act
        //    var result = await userManager.Create(user);

        //    // Assert
        //    Assert.True(result.Succeeded);
        //    store.VerifyAll();
        //}

        [Fact]
        public void UsersQueryableFailWhenStoreNotImplementedTest()
        {
            var manager = new UserManager<TestUser, string>(new NoopUserStore());
            Assert.False(manager.SupportsQueryableUsers);
            Assert.Throws<NotSupportedException>(() => manager.Users.Count());
        }

        [Fact]
        public async Task UsersEmailMethodsFailWhenStoreNotImplementedTest()
        {
            var manager = new UserManager<TestUser, string>(new NoopUserStore());
            Assert.False(manager.SupportsUserEmail);
            await Assert.ThrowsAsync<NotSupportedException>(() => manager.FindByEmail(null));
            await Assert.ThrowsAsync<NotSupportedException>(() => manager.SetEmail(null, null));
            await Assert.ThrowsAsync<NotSupportedException>(() => manager.GetEmail(null));
            await Assert.ThrowsAsync<NotSupportedException>(() => manager.IsEmailConfirmed(null));
            await Assert.ThrowsAsync<NotSupportedException>(() => manager.ConfirmEmail(null, null));
        }

        [Fact]
        public async Task UsersPhoneNumberMethodsFailWhenStoreNotImplementedTest()
        {
            var manager = new UserManager<TestUser, string>(new NoopUserStore());
            Assert.False(manager.SupportsUserPhoneNumber);
            await Assert.ThrowsAsync<NotSupportedException>(async () => await manager.SetPhoneNumber(null, null));
            await Assert.ThrowsAsync<NotSupportedException>(async () => await manager.SetPhoneNumber(null, null));
            await Assert.ThrowsAsync<NotSupportedException>(async () => await manager.GetPhoneNumber(null));
        }

        [Fact]
        public async Task TokenMethodsThrowWithNoTokenProviderTest()
        {
            var manager = new UserManager<TestUser, string>(new NoopUserStore());
            await Assert.ThrowsAsync<NotSupportedException>(
                async () => await manager.GenerateUserToken(null, null));
            await Assert.ThrowsAsync<NotSupportedException>(
                async () => await manager.VerifyUserToken(null, null, null));
        }

        [Fact]
        public async Task PasswordMethodsFailWhenStoreNotImplementedTest()
        {
            var manager = new UserManager<TestUser, string>(new NoopUserStore());
            Assert.False(manager.SupportsUserPassword);
            await Assert.ThrowsAsync<NotSupportedException>(() => manager.Create(null, null));
            await Assert.ThrowsAsync<NotSupportedException>(() => manager.ChangePassword(null, null, null));
            await Assert.ThrowsAsync<NotSupportedException>(() => manager.AddPassword(null, null));
            await Assert.ThrowsAsync<NotSupportedException>(() => manager.RemovePassword(null));
            await Assert.ThrowsAsync<NotSupportedException>(() => manager.CheckPassword(null, null));
            await Assert.ThrowsAsync<NotSupportedException>(() => manager.HasPassword(null));
        }

        [Fact]
        public async Task SecurityStampMethodsFailWhenStoreNotImplementedTest()
        {
            var manager = new UserManager<TestUser, string>(new NoopUserStore());
            Assert.False(manager.SupportsUserSecurityStamp);
            await Assert.ThrowsAsync<NotSupportedException>(() => manager.UpdateSecurityStamp("bogus"));
            await Assert.ThrowsAsync<NotSupportedException>(() => manager.GetSecurityStamp("bogus"));
            await Assert.ThrowsAsync<NotSupportedException>(() => manager.VerifyChangePhoneNumberToken("bogus", "1", "111-111-1111"));
            await Assert.ThrowsAsync<NotSupportedException>(() => manager.GenerateChangePhoneNumberToken("bogus", "111-111-1111"));
        }

        [Fact]
        public async Task LoginMethodsFailWhenStoreNotImplementedTest()
        {
            var manager = new UserManager<TestUser, string>(new NoopUserStore());
            Assert.False(manager.SupportsUserLogin);
            await Assert.ThrowsAsync<NotSupportedException>(async () => await manager.AddLogin("bogus", null));
            await Assert.ThrowsAsync<NotSupportedException>(async () => await manager.RemoveLogin("bogus", null));
            await Assert.ThrowsAsync<NotSupportedException>(async () => await manager.GetLogins("bogus"));
            await Assert.ThrowsAsync<NotSupportedException>(async () => await manager.Find(null));
        }

        [Fact]
        public async Task ClaimMethodsFailWhenStoreNotImplementedTest()
        {
            var manager = new UserManager<TestUser, string>(new NoopUserStore());
            Assert.False(manager.SupportsUserClaim);
            await Assert.ThrowsAsync<NotSupportedException>(async () => await manager.AddClaim("bogus", null));
            await Assert.ThrowsAsync<NotSupportedException>(async () => await manager.RemoveClaim("bogus", null));
            await Assert.ThrowsAsync<NotSupportedException>(async () => await manager.GetClaims("bogus"));
        }

        [Fact]
        public async Task TwoFactorStoreMethodsFailWhenStoreNotImplementedTest()
        {
            var manager = new UserManager<TestUser, string>(new NoopUserStore());
            Assert.False(manager.SupportsUserTwoFactor);
            await Assert.ThrowsAsync<NotSupportedException>(async () => await manager.GetTwoFactorEnabled("bogus"));
            await Assert.ThrowsAsync<NotSupportedException>(async () => await manager.SetTwoFactorEnabled("bogus", true));
        }

        [Fact]
        public async Task RoleMethodsFailWhenStoreNotImplementedTest()
        {
            var manager = new UserManager<TestUser, string>(new NoopUserStore());
            Assert.False(manager.SupportsUserRole);
            await Assert.ThrowsAsync<NotSupportedException>(async () => await manager.AddToRole("bogus", null));
            await Assert.ThrowsAsync<NotSupportedException>(async () => await manager.GetRoles("bogus"));
            await Assert.ThrowsAsync<NotSupportedException>(async () => await manager.RemoveFromRole("bogus", null));
            await Assert.ThrowsAsync<NotSupportedException>(async () => await manager.IsInRole("bogus", "bogus"));
        }

        [Fact]
        public void DisposeAfterDisposeWorksTest()
        {
            var manager = new UserManager<TestUser, string>(new NoopUserStore());
            manager.Dispose();
            manager.Dispose();
        }

        [Fact]
        public async Task ManagerPublicNullCheckTest()
        {
            Assert.Throws<ArgumentNullException>(() => new UserManager<TestUser, string>((IUserStore<TestUser, string>)null));
            var manager = new UserManager<TestUser, string>(new NotImplementedStore());
            Assert.Throws<ArgumentNullException>(() => manager.ClaimsIdentityFactory = null);
            Assert.Throws<ArgumentNullException>(() => manager.PasswordHasher = null);
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await manager.CreateIdentity(null, "whatever"));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await manager.Create(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await manager.Create(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await manager.Create(new TestUser(), null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await manager.Update(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await manager.Delete(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await manager.AddClaim("bogus", null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await manager.FindByName(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await manager.Find(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await manager.AddLogin("bogus", null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await manager.RemoveLogin("bogus", null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await manager.FindByEmail(null));
            Assert.Throws<ArgumentNullException>(() => manager.RegisterTwoFactorProvider(null, null));
            Assert.Throws<ArgumentNullException>(() => manager.RegisterTwoFactorProvider("bogus", null));
        }

        //[Fact]
        //public void MethodsFailWithUnknownUserTest()
        //{
        //    var db = IdentityResultExtensions.CreateDefaultDb();
        //    var manager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(db));
        //    manager.UserTokenProvider = new NoOpTokenProvider();
        //    var error = "UserId not found.";
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.AddClaimAsync(null, new Claim("a", "b"))), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.AddLoginAsync(null, new UserLoginInfo("", ""))), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.AddPasswordAsync(null, null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.AddToRoleAsync(null, null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.ChangePasswordAsync(null, null, null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.GetClaimsAsync(null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.GetLoginsAsync(null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.GetRolesAsync(null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.IsInRoleAsync(null, null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.RemoveClaimAsync(null, new Claim("a", "b"))), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.RemoveLoginAsync(null, new UserLoginInfo("", ""))), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.RemovePasswordAsync(null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.RemoveFromRoleAsync(null, null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.UpdateSecurityStampAsync(null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.GetSecurityStampAsync(null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.HasPasswordAsync(null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.GeneratePasswordResetTokenAsync(null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.ResetPasswordAsync(null, null, null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.IsEmailConfirmedAsync(null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.GenerateEmailConfirmationTokenAsync(null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.ConfirmEmailAsync(null, null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.GetEmailAsync(null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.SetEmailAsync(null, null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.IsPhoneNumberConfirmedAsync(null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.ChangePhoneNumberAsync(null, null, null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.VerifyChangePhoneNumberTokenAsync(null, null, null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.GetPhoneNumberAsync(null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.SetPhoneNumberAsync(null, null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.GetTwoFactorEnabledAsync(null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.SetTwoFactorEnabledAsync(null, true)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.GenerateTwoFactorTokenAsync(null, null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.VerifyTwoFactorTokenAsync(null, null, null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.NotifyTwoFactorTokenAsync(null, null, null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.GetValidTwoFactorProvidersAsync(null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.VerifyUserTokenAsync(null, null, null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.AccessFailedAsync(null)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.SetLockoutEnabledAsync(null, false)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.SetLockoutEndDateAsync(null, DateTimeOffset.UtcNow)), error);
        //    ExceptionHelper.ThrowsWithError<InvalidOperationException>(
        //        () => AsyncHelper.RunSync(() => manager.IsLockedOutAsync(null)), error);
        //}

        [Fact]
        public async Task MethodsThrowWhenDisposedTest()
        {
            var manager = new UserManager<TestUser, string>(new NoopUserStore());
            manager.Dispose();
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.AddClaim("bogus", null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.AddLogin("bogus", null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.AddPassword("bogus", null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.AddToRole("bogus", null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.ChangePassword("bogus", null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.GetClaims("bogus"));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.GetLogins("bogus"));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.GetRoles("bogus"));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.IsInRole("bogus", null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.RemoveClaim("bogus", null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.RemoveLogin("bogus", null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.RemovePassword("bogus"));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.RemoveFromRole("bogus", null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.RemoveClaim("bogus", null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.Find("bogus", null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.Find(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.FindById(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.FindByName(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.Create(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.Create(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.CreateIdentity(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.Update(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.Delete(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.UpdateSecurityStamp(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.GetSecurityStamp(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.GeneratePasswordResetToken(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.ResetPassword(null, null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.GenerateEmailConfirmationToken(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.IsEmailConfirmed(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.ConfirmEmail(null, null));
        }

        private class NotImplementedStore : 
            IUserPasswordStore<TestUser, string>, 
            IUserClaimStore<TestUser, string>,
            IUserLoginStore<TestUser, string>,
            IUserEmailStore<TestUser, string>,
            IUserPhoneNumberStore<TestUser, string>,
            IUserLockoutStore<TestUser, string>,
            IUserTwoFactorStore<TestUser, string>
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public Task Create(TestUser user)
            {
                throw new NotImplementedException();
            }

            public Task Update(TestUser user)
            {
                throw new NotImplementedException();
            }

            public Task Delete(TestUser user)
            {
                throw new NotImplementedException();
            }

            public Task<TestUser> FindById(string userId)
            {
                throw new NotImplementedException();
            }

            public Task<TestUser> FindByName(string userName)
            {
                throw new NotImplementedException();
            }

            public Task SetPasswordHash(TestUser user, string passwordHash)
            {
                throw new NotImplementedException();
            }

            public Task<string> GetPasswordHash(TestUser user)
            {
                throw new NotImplementedException();
            }

            public Task<bool> HasPassword(TestUser user)
            {
                throw new NotImplementedException();
            }

            public Task<IList<Claim>> GetClaims(TestUser user)
            {
                throw new NotImplementedException();
            }

            public Task AddClaim(TestUser user, Claim claim)
            {
                throw new NotImplementedException();
            }

            public Task RemoveClaim(TestUser user, Claim claim)
            {
                throw new NotImplementedException();
            }

            public Task AddLogin(TestUser user, UserLoginInfo login)
            {
                throw new NotImplementedException();
            }

            public Task RemoveLogin(TestUser user, UserLoginInfo login)
            {
                throw new NotImplementedException();
            }

            public Task<IList<UserLoginInfo>> GetLogins(TestUser user)
            {
                throw new NotImplementedException();
            }

            public Task<TestUser> Find(UserLoginInfo login)
            {
                throw new NotImplementedException();
            }

            public Task SetEmail(TestUser user, string email)
            {
                throw new NotImplementedException();
            }

            public Task<string> GetEmail(TestUser user)
            {
                throw new NotImplementedException();
            }

            public Task<bool> GetEmailConfirmed(TestUser user)
            {
                throw new NotImplementedException();
            }

            public Task SetEmailConfirmed(TestUser user, bool confirmed)
            {
                throw new NotImplementedException();
            }

            public Task<TestUser> FindByEmail(string email)
            {
                throw new NotImplementedException();
            }

            public Task SetPhoneNumber(TestUser user, string phoneNumber)
            {
                throw new NotImplementedException();
            }

            public Task<string> GetPhoneNumber(TestUser user)
            {
                throw new NotImplementedException();
            }

            public Task<bool> GetPhoneNumberConfirmed(TestUser user)
            {
                throw new NotImplementedException();
            }

            public Task SetPhoneNumberConfirmed(TestUser user, bool confirmed)
            {
                throw new NotImplementedException();
            }

            public Task<DateTimeOffset> GetLockoutEndDate(TestUser user)
            {
                throw new NotImplementedException();
            }

            public Task SetLockoutEndDate(TestUser user, DateTimeOffset lockoutEnd)
            {
                throw new NotImplementedException();
            }

            public Task<int> IncrementAccessFailedCount(TestUser user)
            {
                throw new NotImplementedException();
            }

            public Task ResetAccessFailedCount(TestUser user)
            {
                throw new NotImplementedException();
            }

            public Task<int> GetAccessFailedCount(TestUser user)
            {
                throw new NotImplementedException();
            }

            public Task<bool> GetLockoutEnabled(TestUser user)
            {
                throw new NotImplementedException();
            }

            public Task SetLockoutEnabled(TestUser user, bool enabled)
            {
                throw new NotImplementedException();
            }

            public Task SetTwoFactorEnabled(TestUser user, bool enabled)
            {
                throw new NotImplementedException();
            }

            public Task<bool> GetTwoFactorEnabled(TestUser user)
            {
                throw new NotImplementedException();
            }
        }
    }
}