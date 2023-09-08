namespace JsonStringAttempt.Test
{
    [TestClass]
    public class JsonConversion
    {
        [TestMethod]
        public void JsonConverter_SerializeBasicObject_True()
        {
            Product product = new Product()
            {
                Id = 1,
                Name = "Test",
                Description = "Test",
                Price = 1,
            };

            string expected = "{\"Id\":1,\"Name\":\"Test\",\"Description\":\"Test\",\"Price\":1}";

            string result = JsonConverter.Serialize(product);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void JsonConverter_SerializeNestedObject_True()
        {
            User user = new User()
            {
                Username = "John",
                Password = "Test",
                Age = 1,
                Product = new Product()
                {
                    Id = 1,
                    Name = "Test",
                    Description = "Test",
                    Price = 1,
                }
            };

            string expected = "{\"Username\":\"John\",\"Password\":\"Test\",\"Age\":1,\"Product\":{\"Id\":1,\"Name\":\"Test\",\"Description\":\"Test\",\"Price\":1}}";

            string result = JsonConverter.Serialize(user);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void JsonConverter_SerializeArrayBasic_True()
        {
            Product[] product = new[]
            {
                new Product()
                {
                    Id = 1,
                    Name = "Test",
                    Description = "Test",
                    Price = 1,
                },
                new Product()
                {
                    Id = 1,
                    Name = "Test",
                    Description = "Test",
                    Price = 1,
                }
            };

            string expected = "[{\"Id\":1,\"Name\":\"Test\",\"Description\":\"Test\",\"Price\":1},{\"Id\":1,\"Name\":\"Test\",\"Description\":\"Test\",\"Price\":1}]";

            string result = JsonConverter.Serialize(product);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void JsonConverter_SerializeArrayInObject_True()
        {
            Product[] product = new[]
            {
                new Product()
                {
                    Id = 1,
                    Name = "Test",
                    Description = "Test",
                    Price = 1,
                },
                new Product()
                {
                    Id = 1,
                    Name = "Test",
                    Description = "Test",
                    Price = 1,
                }
            };

            Cart cart = new Cart() { Name = "Test", Products = product };

            string expected = "{\"Name\":\"Test\",\"Products\":[{\"Id\":1,\"Name\":\"Test\",\"Description\":\"Test\",\"Price\":1},{\"Id\":1,\"Name\":\"Test\",\"Description\":\"Test\",\"Price\":1}]}";

            string result = JsonConverter.Serialize(cart);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DictionaryDeserialize_DeserializeBasicObject_True()
        {
            string input = "{\"Id\":1,\"Name\":\"Test\",\"Description\":\"Test\",\"Price\":1}";

            var result = JsonConverter.DictionaryDeserialize(input);

            Assert.IsNotNull(result);
            Assert.AreEqual("Test", result["Name"]);
        }

        [TestMethod]
        public void DictionaryDeserialize_DeserializeNestedObject_True()
        {
            string testString = "{\"Username\":\"John\",\"Password\":\"Test\",\"Age\":1,\"Product\":{\"Id\":1,\"Name\":\"Test\",\"Description\":\"Test\",\"Price\":1}}";

            var result = JsonConverter.DictionaryDeserialize(testString);

            Assert.IsNotNull(result);
            Assert.AreEqual("John", result["Username"]);
            Assert.AreEqual(1, result["Age"]);
            Assert.IsInstanceOfType(result["Product"], typeof(Dictionary<string, object>));
            Assert.AreEqual("Test", (result["Product"] as Dictionary<string, object>)["Name"]);
            Assert.AreEqual(1, (result["Product"] as Dictionary<string, object>)["Price"]);
        }

        [TestMethod]
        public void ListDeserialize_DeserializeStringArray_True()
        {
            string array = "[\"One\",\"Two\",\"Three\"]";

            var result = JsonConverter.ListDeserialize(array);

            Assert.IsNotNull(result);
            Assert.AreEqual("One", result.First());
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void ListDeserialize_DeserializeIntArray_True()
        {
            string array = "[1,2,3]";

            var result = JsonConverter.ListDeserialize(array);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.First());
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void ListDeserialize_DeserializeObjectArray_True()
        {
            string array = "[{\"Id\":1,\"Name\":\"Test\",\"Description\":\"Test\",\"Price\":1},{\"Id\":1,\"Name\":\"Test\",\"Description\":\"Test\",\"Price\":1}]";

            var result = JsonConverter.ListDeserialize(array);


            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.First(), typeof(Dictionary<string, object>));
            Assert.AreEqual(2, result.Count);

            if (result.First() is Dictionary<string, object>)
            {
                string testCase = (result.First() as Dictionary<string, object>)["Name"] as string;
                Assert.AreEqual("Test", testCase);
            }
        }

        [TestMethod]
        public void JsonConverter_DeserializeBasicObject_True()
        {
            string input = "{\"Id\":1,\"Name\":\"Test\",\"Description\":\"Test\",\"Price\":1}";

            Product product = JsonConverter.Deserialize<Product>(input);

            Assert.IsNotNull(product);
            Assert.IsInstanceOfType(product, typeof(Product));
            Assert.AreEqual(product.Id, 1);
        }

        [TestMethod]
        public void JsonConverter_DeserializeNestedObject_True()
        {
            string testString = "{\"Username\":\"John\",\"Password\":\"Test\",\"Age\":1,\"Product\":{\"Id\":1,\"Name\":\"Test\",\"Description\":\"Test\",\"Price\":1}}";

            User user = JsonConverter.Deserialize<User>(testString);

            Assert.IsNotNull(user);
            Assert.IsInstanceOfType(user, typeof(User));
            Assert.AreEqual("John", user.Username);
            Assert.IsInstanceOfType(user.Product, typeof(Product));
        }

        [TestMethod]
        public void JsonConverter_DeserializeNestedObjectHaveValues_True()
        {
            string testString = "{\"Username\":\"John\",\"Password\":\"Test\",\"Age\":1,\"Product\":{\"Id\":1,\"Name\":\"Test\",\"Description\":\"Test\",\"Price\":1}}";

            User user = JsonConverter.Deserialize<User>(testString);

            Assert.IsNotNull(user);
            Assert.IsInstanceOfType(user, typeof(User));
            Assert.AreEqual("John", user.Username);
            Assert.AreEqual("Test", user.Product.Name);
        }

        [TestMethod]
        public void JsonConverter_DeserializeObject_NoArgumentException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => JsonConverter.Deserialize<Product>(""));
        }

        [TestMethod]
        public void JsonConverter_DeserializeObject_FormatException()
        {
            Assert.ThrowsException<FormatException>(() => JsonConverter.Deserialize<Product>("\"Id\":1,\"Name\":\"Test\",\"Description\":\"Test\",\"Price\":1"));
        }
    }


    internal class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        public Product Product { get; set; }
    }

    internal class Cart
    {
        public string Name { get; set; }
        public Product[] Products { get; set; }
    }

    internal class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}
