namespace AwsLambda.Lambda.StepFun;


public class State
{
    public string? Name { get; set; }
    public string? Message { get; set; }
    public int WaitInSeconds { get; set; }
}