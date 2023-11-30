namespace HA.Kostal;

public interface IKostalClient
{
    KostalClientResult readPage();
    Task<KostalClientResult> readPageAsync();
}