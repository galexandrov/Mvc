﻿using System;
using Xunit;

namespace Microsoft.AspNet.Mvc.ModelBinding.Test
{
    public class ModelBindingContextTest
    {
        [Fact]
        public void CopyConstructor()
        {
            // Arrange
            ModelBindingContext originalBindingContext = new ModelBindingContext
            {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(object)),
                ModelName = "theName",
                ModelState = new ModelStateDictionary(),
                ValueProvider = new SimpleHttpValueProvider()
            };

            // Act
            ModelBindingContext newBindingContext = new ModelBindingContext(originalBindingContext);

            // Assert
            Assert.Null(newBindingContext.ModelMetadata);
            Assert.Equal("", newBindingContext.ModelName);
            Assert.Equal(originalBindingContext.ModelState, newBindingContext.ModelState);
            Assert.Equal(originalBindingContext.ValueProvider, newBindingContext.ValueProvider);
        }

        [Fact]
        public void ModelProperty_ThrowsIfModelMetadataDoesNotExist()
        {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext();

            // Act & assert
            ExceptionAssert.Throws<InvalidOperationException>(
                () => bindingContext.Model = null,
                "The ModelMetadata property must be set before accessing this property.");
        }

        [Fact]
        public void ModelAndModelTypeAreFedFromModelMetadata()
        {
            // Act
            ModelBindingContext bindingContext = new ModelBindingContext
            {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => 42, typeof(int))
            };

            // Assert
            Assert.Equal(42, bindingContext.Model);
            Assert.Equal(typeof(int), bindingContext.ModelType);
        }

        // TODO: Validation
        //[Fact]
        //public void ValidationNodeProperty()
        //{
        //    // Act
        //    ModelBindingContext bindingContext = new ModelBindingContext
        //    {
        //        ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => 42, typeof(int))
        //    };

        //    // Act & assert
        //    MemberHelper.TestPropertyWithDefaultInstance(bindingContext, "ValidationNode", new ModelValidationNode(bindingContext.ModelMetadata, "someName"));
        //}

        // TODO: Validation
        //[Fact]
        //public void ValidationNodeProperty_DefaultValues()
        //{
        //    // Act
        //    ModelBindingContext bindingContext = new ModelBindingContext
        //    {
        //        ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => 42, typeof(int)),
        //        ModelName = "theInt"
        //    };

        //    // Act
        //    ModelValidationNode validationNode = bindingContext.ValidationNode;

        //    // Assert
        //    Assert.NotNull(validationNode);
        //    Assert.Equal(bindingContext.ModelMetadata, validationNode.ModelMetadata);
        //    Assert.Equal(bindingContext.ModelName, validationNode.ModelStateKey);
        //}
    }
}