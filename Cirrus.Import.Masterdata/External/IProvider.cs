namespace Cirrus.Import.Masterdata.External
{
    interface IProvider
    {
        bool Enabled { get; }

        string Key { get; }
    }
}
