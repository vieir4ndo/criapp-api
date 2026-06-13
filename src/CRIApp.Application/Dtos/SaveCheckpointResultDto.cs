namespace CRIApp.Application.Dtos;

public class SaveCheckpointResultDto
{
    public bool Assimilated { get; }

    public SaveCheckpointResultDto(bool assimilated)
    {
        Assimilated = assimilated;
    }
}
