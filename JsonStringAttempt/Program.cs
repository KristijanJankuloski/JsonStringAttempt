using JsonStringAttempt;

User user = new User()
{
    Username = "john1",
    Password = "heresjohnny",
    Age = 22,
    Product = new Product()
    {
        Id = 1,
        Name = "IPhone",
        Description = "an iphone",
        Price = 1999.99
    }
};

string array = "[{\"Id\":1,\"Name\":\"Test\",\"Description\":\"Test\",\"Price\":1},{\"Id\":1,\"Name\":\"Test\",\"Description\":\"Test\",\"Price\":1}]";

var result = JsonConverter.ListDeserialize(array);


string serializedData = JsonConverter.Serialize(user);
Console.WriteLine(serializedData);

User deserializedUser = JsonConverter.Deserialize<User>(serializedData);
Console.WriteLine($"Username: {deserializedUser.Username}");
Console.WriteLine($"Password: {deserializedUser.Password}");
Console.WriteLine($"Age: {deserializedUser.Age}");
Console.WriteLine($"Product: {deserializedUser.Product.Name} - {deserializedUser.Product.Description} - {deserializedUser.Product.Price}");

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int Age { get; set; }
    public Product Product { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
}