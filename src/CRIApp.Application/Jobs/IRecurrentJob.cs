namespace CRIApp.Application.Jobs;

public interface IRecurrentJob
{
    Task Run();
}