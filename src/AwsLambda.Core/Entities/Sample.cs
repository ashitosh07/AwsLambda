namespace AwsLambda.Core.Entities;

public class Sample
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }

    public Sample(
        string name,
        int age,
        string email)
    {
        Name = name;
        Age = age;
        Email = email;
    }
}
