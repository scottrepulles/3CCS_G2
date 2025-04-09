namespace DHK.Blazor.Module.Interfaces;

public interface ILocalFileService
{
    string SaveExcelTemplate(byte[] content, string fileName);
}
