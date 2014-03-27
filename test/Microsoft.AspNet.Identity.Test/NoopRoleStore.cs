using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity.Test
{
    public class NoopRoleStore : IRoleStore<TestRole, string>
    {
        public Task Create(TestRole role)
        {
            return Task.FromResult(0);
        }

        public Task Update(TestRole role)
        {
            return Task.FromResult(0);
        }

        public Task<TestRole> FindById(string id)
        {
            return Task.FromResult<TestRole>(null);
        }

        public Task<TestRole> FindByName(string name)
        {
            return Task.FromResult<TestRole>(null);
        }

        public void Dispose()
        {
        }

        public Task Delete(TestRole role)
        {
            return Task.FromResult(0);
        }
    }
}