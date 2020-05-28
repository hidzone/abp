﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.BlobStoring.TestObjects;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Xunit;

namespace Volo.Abp.BlobStoring
{
    public abstract class BlobContainer_Tests<TStartupModule> : AbpIntegratedTest<TStartupModule>
        where TStartupModule : IAbpModule
    {
        protected IBlobContainer<TestContainer1> Container { get; }

        protected BlobContainer_Tests()
        {
            Container = GetRequiredService<IBlobContainer<TestContainer1>>();
        }
        
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }

        [Theory]
        [InlineData("test-blob-1")]
        [InlineData("test-blob-1.txt")]
        [InlineData("test-folder/test-blob-1")]
        public async Task Should_Save_And_Get_Blobs(string blobName)
        {
            var testContent = "test content".GetBytes();
            await Container.SaveAsync(blobName, testContent);

            var result = await Container.GetAllBytesAsync(blobName);
            result.SequenceEqual(testContent).ShouldBeTrue();
        }
        
        [Theory]
        [InlineData("test-blob-1")]
        [InlineData("test-blob-1.txt")]
        [InlineData("test-folder/test-blob-1")]
        public async Task Should_Delete_Saved_Blobs(string blobName)
        {
            await Container.SaveAsync(blobName, "test content".GetBytes());
            (await Container.GetAllBytesAsync(blobName)).ShouldNotBeNull();

            await Container.DeleteAsync(blobName);
            (await Container.GetAllBytesOrNullAsync(blobName)).ShouldBeNull();
        }

        [Theory]
        [InlineData("test-blob-1")]
        [InlineData("test-blob-1.txt")]
        [InlineData("test-folder/test-blob-1")]
        public async Task Saved_Blobs_Should_Exists(string blobName)
        {
            await Container.SaveAsync(blobName, "test content".GetBytes());
            (await Container.ExistsAsync(blobName)).ShouldBeTrue();

            await Container.DeleteAsync(blobName);
            (await Container.ExistsAsync(blobName)).ShouldBeFalse();
        }
        
        [Theory]
        [InlineData("test-blob-1")]
        [InlineData("test-blob-1.txt")]
        [InlineData("test-folder/test-blob-1")]
        public async Task Unknown_Blobs_Should_Not_Exists(string blobName)
        {
            await Container.DeleteAsync(blobName);
            (await Container.ExistsAsync(blobName)).ShouldBeFalse();
        }
    }
}