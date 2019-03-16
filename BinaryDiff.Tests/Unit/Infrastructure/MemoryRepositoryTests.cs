using BinaryDiff.Infrastructure.Repositories;
using BinaryDiff.Infrastructure.Repositories.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BinaryDiff.Tests.Unit.Infrastructure
{
    /// <summary>
    /// Tests for MemoryRepository
    /// </summary>
    public class MemoryRepositoryTests
    {
        private readonly IMemoryRepository<Guid, object> _repository;

        /// <summary>
        /// Initializes repository with seed data to use on tests
        /// </summary>
        public MemoryRepositoryTests()
        {
            _repository = new MemoryRepository<Guid, object>(_seedData);
        }

        /// <summary>
        /// Tests expected exception in case key provided is null
        /// </summary>
        [Fact]
        public void Find_ThrowsInvalidOperationException_IfKeyIsNull()
        {
            Guid? key = null;

            Assert.Throws<InvalidOperationException>(() => _repository.Find(key.Value));
        }

        /// <summary>
        /// Tests result for a key that doesn't exist on repository
        /// </summary>
        [Fact]
        public void Find_ReturnsDefaultTypeValue_IfKeyNotFound()
        {
            var expected = default(object);

            var result = _repository.Find(Guid.NewGuid());

            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Tests result for a given key that exists on repository
        /// </summary>
        [Fact]
        public void Find_ReturnsValueForGivenKey()
        {
            var expected = _seedData.First();

            var result = _repository.Find(expected.Key);

            Assert.Equal(expected.Value, result);
        }

        /// <summary>
        /// Tests expected exception in case key provided is null
        /// </summary>
        [Fact]
        public void Save_ThrowsInvalidOperationException_IfKeyIsNull()
        {
            Guid? key = null;

            Assert.Throws<InvalidOperationException>(() => _repository.Save(key.Value, new { }));
        }

        /// <summary>
        /// Tests expected exception in case value provided is null
        /// </summary>
        [Fact]
        public void Save_ThrowsInvalidOperationException_IfValueIsNull()
        {
            Assert.Throws<InvalidOperationException>(() => _repository.Save(Guid.NewGuid(), null));
        }

        /// <summary>
        /// Tests if item is saved on repository
        /// </summary>
        [Fact]
        public void Save_AddsKeyValuePairToRepository()
        {
            var newObject = new { key = Guid.NewGuid(), field = "value" };

            _repository.Save(newObject.key, newObject);

            var existing = _repository.Find(newObject.key);

            Assert.Equal(newObject, existing);
        }

        private static IDictionary<Guid, object> _seedData => new Dictionary<Guid, object>
        {
            { new Guid("bc0d9ca3-7d10-47f2-9b64-b88425e55149"), new { value = "foo" } },
            { new Guid("07087409-02ee-429c-a563-fe89b062dd87"), new { value = "bar" } }
        };
    }
}
