using Es2al.DataAccess.Context;
using Es2al.DataAccess.Repositories;
using Es2al.Models.Entites;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
namespace Es2al.Tests.Repository
{
    public class BaseRepositoryTests
    {
        private AppDbContext CreateNewContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }
        [Fact]
        public async Task AddAsync_PassingEntityToAdd_ShouldSaveEntity()
        {
            //Arrange
            var entity = new AppUser { UserName = "User #1", PasswordHash = "Password #1", Id = 1 };
            using var context = CreateNewContext();
            var baseRepository = new BaseRepository<AppUser>(context);

            //Act
            await baseRepository.AddAsync(entity);
            var result=await baseRepository.GetAll().ToListAsync();

            //Assert
            result.First().UserName.Should().Be(entity.UserName);
        }
        [Fact]
        public async Task UpdateAsync_PassingAnEntityToUpdate_ShouldUpdateEntity()
        {
            //Arrange
            var entity = new AppUser { UserName = "User #1", PasswordHash = "Password #1", Id = 1 };
            using var context = CreateNewContext();
            var baseRepository = new BaseRepository<AppUser>(context);
            await baseRepository.AddAsync(entity);

            //Act
            entity.UserName = "User #2";
            await baseRepository.UpdateAsync(entity);
            var expected = await baseRepository.FindAsync(entity.Id);

            //Assert
            expected?.UserName.Should().Be("User #2");
        }
        [Fact]
        public async Task RemoveAsync_PassingEntityToDelete_ShouldDeleteEntity()
        {
            //Arrange
            var entity = new AppUser { UserName = "User #1", PasswordHash = "Password #1", Id = 1 };
            using var context = CreateNewContext();
            var baseRepository = new BaseRepository<AppUser>(context);
            await baseRepository.AddAsync(entity);

            //Act
            await baseRepository.RemoveAsync(entity);
            var expected = await baseRepository.FindAsync(1);

            //Assert
            expected.Should().BeNull();
        }
        [Fact]
        public async Task RemoveAsync_PassingPersistedEntityIdToDelete_ShouldDeleteEntity()
        {
            //Arrange
            var entity = new AppUser { UserName = "User #1", PasswordHash = "Password #1", Id = 1 };
            using var context = CreateNewContext();
            var baseRepository = new BaseRepository<AppUser>(context);
            await baseRepository.AddAsync(entity);

            //Act
            await baseRepository.RemoveAsync(1);
            var expected = await baseRepository.FindAsync(entity.Id);

            //Assert
            expected.Should().BeNull();
        }
        [Fact]
        public async Task RemoveAsync_PassingNotPersistedEntityIdToDelete_ThrowKeyNotFoundException()
        {
            //Arrange
            using var context = CreateNewContext();
            var baseRepository = new BaseRepository<AppUser>(context);

            //Act
            Func<Task> func= async ()=> await baseRepository.RemoveAsync(1);

            //Assert
            await func.Should().ThrowAsync<KeyNotFoundException>();
        }
        [Fact]
        public async Task FindAsync_PassingPersistedEntityIdToFind_ReturnFoundEntity()
        {
            //Arrange
            var entity = new AppUser { UserName = "User #1", PasswordHash = "Password #1",Id= 1 };
            using var context = CreateNewContext();
            var baseRepository = new BaseRepository<AppUser>(context);
            await baseRepository.AddAsync(entity);

            //Act
            var expected = await baseRepository.FindAsync(1);

            //Assert
            expected.Should().BeEquivalentTo(entity);
        }
        [Fact]
        public async Task FindAsync_PassingNotPersistedEntityIdToFind_ReturnNull()
        {
            //Arrange
            using var context = CreateNewContext();
            var baseRepository = new BaseRepository<AppUser>(context);
            //Act
            var expected = await baseRepository.FindAsync(3);
            //Assert
            expected.Should().BeNull();
        }

    }
}
