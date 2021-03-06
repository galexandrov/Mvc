﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.ModelBinding;
using Xunit;

namespace Microsoft.AspNet.Mvc.IntegrationTests
{
    public class ActionParameterIntegrationTest
    {
        private class Address
        {
            public string Street { get; set; }
        }

        private class Person3
        {
            public Person3()
            {
                Address = new List<Address>();
            }

            public List<Address> Address { get; }
        }

        [Fact(Skip = "Extra entries in model state dictionary. #2466")]
        public async Task ActionParameter_NonSettableCollectionModel_EmptyPrefix_GetsBound()
        {
            // Arrange
            var argumentBinder = ModelBindingTestHelper.GetArgumentBinder();
            var parameter = new ParameterDescriptor()
            {
                Name = "Address",
                ParameterType = typeof(Person3)
            };

            var operationContext = ModelBindingTestHelper.GetOperationBindingContext(request =>
            {
                request.QueryString = QueryString.Create("[0].Street", "SomeStreet");
            });

            var modelState = new ModelStateDictionary();
            var model = new Person3();

            // Act
            var modelBindingResult = await argumentBinder.BindModelAsync(parameter, modelState, operationContext);

            // Assert
            Assert.NotNull(modelBindingResult);
            Assert.True(modelBindingResult.IsModelSet);

            // Model
            Assert.NotNull(modelBindingResult.Model);
            var boundModel = Assert.IsType<Person3>(modelBindingResult.Model);
            Assert.Equal(1, boundModel.Address.Count);
            Assert.Equal("SomeStreet", boundModel.Address[0].Street);

            // ModelState
            Assert.True(modelState.IsValid);

            Assert.Equal(1, modelState.Keys.Count);
            var key = Assert.Single(modelState.Keys, k => k == "[0].Street");
            Assert.NotNull(modelState[key].Value);
            Assert.Equal("SomeStreet", modelState[key].Value.AttemptedValue);
            Assert.Equal("SomeStreet", modelState[key].Value.RawValue);
            Assert.Empty(modelState[key].Errors);
            Assert.Equal(ModelValidationState.Valid, modelState[key].ValidationState);
        }

        private class Person6
        {
            public CustomReadOnlyCollection<Address> Address { get; set; }
        }

        [Fact(Skip = "Extra entries in model state dictionary. #2466")]
        public async Task ActionParameter_ReadOnlyCollectionModel_EmptyPrefix_DoesNotGetBound()
        {
            // Arrange
            var argumentBinder = ModelBindingTestHelper.GetArgumentBinder();
            var parameter = new ParameterDescriptor()
            {
                Name = "Address",
                ParameterType = typeof(Person6)
            };
            var operationContext = ModelBindingTestHelper.GetOperationBindingContext(request =>
            {
                request.QueryString = QueryString.Create("[0].Street", "SomeStreet");
            });

            var modelState = new ModelStateDictionary();

            // Act
            var modelBindingResult = await argumentBinder.BindModelAsync(parameter, modelState, operationContext);

            // Assert
            Assert.NotNull(modelBindingResult);
            Assert.True(modelBindingResult.IsModelSet);

            // Model
            var boundModel = Assert.IsType<Person6>(modelBindingResult.Model);
            Assert.NotNull(boundModel);
            Assert.NotNull(boundModel.Address);

            // Arrays should not be updated.
            Assert.Equal(0, boundModel.Address.Count());

            // ModelState
            Assert.True(modelState.IsValid);
            Assert.Empty(modelState.Keys);
        }

        private class Person4
        {
            public Address[] Address { get; set; }
        }

        [Fact(Skip = "Extra entries in model state dictionary. #2466")]
        public async Task ActionParameter_SettableArrayModel_EmptyPrefix_GetsBound()
        {
            // Arrange
            var argumentBinder = ModelBindingTestHelper.GetArgumentBinder();
            var parameter = new ParameterDescriptor()
            {
                Name = "Address",
                ParameterType = typeof(Person4)
            };

            var operationContext = ModelBindingTestHelper.GetOperationBindingContext(request =>
            {
                request.QueryString = QueryString.Create("[0].Street", "SomeStreet");
            });

            var modelState = new ModelStateDictionary();
            var model = new Person4();
            // Act
            var modelBindingResult = await argumentBinder.BindModelAsync(parameter, modelState, operationContext);

            // Assert
            Assert.NotNull(modelBindingResult);
            Assert.True(modelBindingResult.IsModelSet);

            // Model
            Assert.NotNull(modelBindingResult.Model);
            var boundModel = Assert.IsType<Person4>(modelBindingResult.Model);
            Assert.Equal(1, boundModel.Address.Count());
            Assert.Equal("SomeStreet", boundModel.Address[0].Street);

            // ModelState
            Assert.True(modelState.IsValid);

            Assert.Equal(1, modelState.Keys.Count);
            var key = Assert.Single(modelState.Keys, k => k == "[0].Street");
            Assert.NotNull(modelState[key].Value);
            Assert.Equal("SomeStreet", modelState[key].Value.AttemptedValue);
            Assert.Equal("SomeStreet", modelState[key].Value.RawValue);
            Assert.Empty(modelState[key].Errors);
            Assert.Equal(ModelValidationState.Valid, modelState[key].ValidationState);
        }

        private class Person5
        {
            public Address[] Address { get; } = new Address[] { };
        }

        [Fact(Skip = "Extra entries in model state dictionary. #2466")]
        public async Task ActionParameter_NonSettableArrayModel_EmptyPrefix_DoesNotGetBound()
        {
            // Arrange
            var argumentBinder = ModelBindingTestHelper.GetArgumentBinder();
            var parameter = new ParameterDescriptor()
            {
                Name = "Address",
                ParameterType = typeof(Person5)
            };

            var operationContext = ModelBindingTestHelper.GetOperationBindingContext(request =>
            {
                request.QueryString = QueryString.Create("[0].Street", "SomeStreet");
            });

            var modelState = new ModelStateDictionary();
            // Act
            var modelBindingResult = await argumentBinder.BindModelAsync(parameter, modelState, operationContext);

            // Assert
            Assert.NotNull(modelBindingResult);
            Assert.True(modelBindingResult.IsModelSet);

            // Model
            Assert.NotNull(modelBindingResult.Model);
            var boundModel = Assert.IsType<Person5>(modelBindingResult.Model);
            Assert.NotNull(boundModel.Address);

            // Arrays should not be updated.
            Assert.Equal(0, boundModel.Address.Count());

            // ModelState
            Assert.True(modelState.IsValid);
            Assert.Empty(modelState.Keys);
        }

        [Fact(Skip = "Extra entries in model state dictionary. #2466")]
        public async Task ActionParameter_NonSettableCollectionModel_WithPrefix_GetsBound()
        {
            // Arrange
            var argumentBinder = ModelBindingTestHelper.GetArgumentBinder();
            var parameter = new ParameterDescriptor()
            {
                Name = "Address",
                BindingInfo = new BindingInfo()
                {
                    BinderModelName = "prefix"
                },
                ParameterType = typeof(Person3)
            };

            var operationContext = ModelBindingTestHelper.GetOperationBindingContext(request =>
            {
                request.QueryString = QueryString.Create("prefix.Address[0].Street", "SomeStreet");
            });

            var modelState = new ModelStateDictionary();

            // Act
            var modelBindingResult = await argumentBinder.BindModelAsync(parameter, modelState, operationContext);

            // Assert
            Assert.NotNull(modelBindingResult);
            Assert.True(modelBindingResult.IsModelSet);

            // Model
            Assert.NotNull(modelBindingResult.Model);
            var boundModel = Assert.IsType<Person3>(modelBindingResult.Model);
            Assert.Equal(1, boundModel.Address.Count);
            Assert.Equal("SomeStreet", boundModel.Address[0].Street);

            // ModelState
            Assert.True(modelState.IsValid);

            Assert.Equal(1, modelState.Keys.Count);
            var key = Assert.Single(modelState.Keys, k => k == "prefix.Address[0].Street");
            Assert.NotNull(modelState[key].Value);
            Assert.Equal("SomeStreet", modelState[key].Value.AttemptedValue);
            Assert.Equal("SomeStreet", modelState[key].Value.RawValue);
            Assert.Empty(modelState[key].Errors);
            Assert.Equal(ModelValidationState.Valid, modelState[key].ValidationState);
        }

        [Fact(Skip = "Extra entries in model state dictionary. #2466")]
        public async Task ActionParameter_ReadOnlyCollectionModel_WithPrefix_DoesNotGetBound()
        {
            // Arrange
            var argumentBinder = ModelBindingTestHelper.GetArgumentBinder();
            var parameter = new ParameterDescriptor()
            {
                Name = "Address",
                BindingInfo = new BindingInfo
                {
                    BinderModelName = "prefix"
                },
                ParameterType = typeof(Person6)
            };
            var operationContext = ModelBindingTestHelper.GetOperationBindingContext(request =>
            {
                request.QueryString = QueryString.Create("prefix.Address[0].Street", "SomeStreet");
            });

            var modelState = new ModelStateDictionary();

            // Act
            var modelBindingResult = await argumentBinder.BindModelAsync(parameter, modelState, operationContext);

            // Assert
            Assert.NotNull(modelBindingResult);
            Assert.True(modelBindingResult.IsModelSet);

            // Model
            var boundModel = Assert.IsType<Person6>(modelBindingResult.Model);
            Assert.NotNull(boundModel);
            Assert.NotNull(boundModel.Address);

            // Arrays should not be updated.
            Assert.Equal(0, boundModel.Address.Count());

            // ModelState
            Assert.True(modelState.IsValid);
            Assert.Empty(modelState.Keys);
        }

        [Fact(Skip = "Extra entries in model state dictionary. #2466")]
        public async Task ActionParameter_SettableArrayModel_WithPrefix_GetsBound()
        {
            // Arrange
            var argumentBinder = ModelBindingTestHelper.GetArgumentBinder();
            var parameter = new ParameterDescriptor()
            {
                Name = "Address",
                BindingInfo = new BindingInfo()
                {
                    BinderModelName = "prefix"
                },
                ParameterType = typeof(Person4)
            };

            var operationContext = ModelBindingTestHelper.GetOperationBindingContext(request =>
            {
                request.QueryString = QueryString.Create("prefix.Address[0].Street", "SomeStreet");
            });

            var modelState = new ModelStateDictionary();

            // Act
            var modelBindingResult = await argumentBinder.BindModelAsync(parameter, modelState, operationContext);

            // Assert
            Assert.NotNull(modelBindingResult);
            Assert.True(modelBindingResult.IsModelSet);

            // Model
            Assert.NotNull(modelBindingResult.Model);
            var boundModel = Assert.IsType<Person4>(modelBindingResult.Model);
            Assert.Equal(1, boundModel.Address.Count());
            Assert.Equal("SomeStreet", boundModel.Address[0].Street);

            // ModelState
            Assert.True(modelState.IsValid);

            Assert.Equal(1, modelState.Keys.Count);
            var key = Assert.Single(modelState.Keys, k => k == "prefix.Address[0].Street");
            Assert.NotNull(modelState[key].Value);
            Assert.Equal("SomeStreet", modelState[key].Value.AttemptedValue);
            Assert.Equal("SomeStreet", modelState[key].Value.RawValue);
            Assert.Empty(modelState[key].Errors);
            Assert.Equal(ModelValidationState.Valid, modelState[key].ValidationState);
        }

        [Fact(Skip = "Extra entries in model state dictionary. #2466")]
        public async Task ActionParameter_NonSettableArrayModel_WithPrefix_DoesNotGetBound()
        {
            // Arrange
            var argumentBinder = ModelBindingTestHelper.GetArgumentBinder();
            var parameter = new ParameterDescriptor()
            {
                Name = "Address",
                BindingInfo = new BindingInfo()
                {
                    BinderModelName = "prefix"
                },
                ParameterType = typeof(Person5)
            };

            var operationContext = ModelBindingTestHelper.GetOperationBindingContext(request =>
            {
                request.QueryString = QueryString.Create("prefix.Address[0].Street", "SomeStreet");
            });

            var modelState = new ModelStateDictionary();

            // Act
            var modelBindingResult = await argumentBinder.BindModelAsync(parameter, modelState, operationContext);

            Assert.NotNull(modelBindingResult);
            Assert.True(modelBindingResult.IsModelSet);

            // Model
            Assert.NotNull(modelBindingResult.Model);
            var boundModel = Assert.IsType<Person4>(modelBindingResult.Model);

            // Arrays should not be updated.
            Assert.Equal(0, boundModel.Address.Count());

            // ModelState
            Assert.True(modelState.IsValid);
            Assert.Empty(modelState.Keys);
        }

        private class CustomReadOnlyCollection<T> : ICollection<T>, IReadOnlyCollection<T>
        {
            private ICollection<T> _original;

            public CustomReadOnlyCollection() : this(new List<T>())
            {
            }

            public CustomReadOnlyCollection(ICollection<T> original)
            {
                _original = original;
            }

            public int Count
            {
                get { return _original.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public void Add(T item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(T item)
            {
                return _original.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                _original.CopyTo(array, arrayIndex);
            }

            public bool Remove(T item)
            {
                throw new NotSupportedException();
            }

            public IEnumerator<T> GetEnumerator()
            {
                foreach (T t in _original)
                {
                    yield return t;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}