namespace WebServiceMock.Core.Models
{
    public enum HostMode
    {
        /// <summary>The OWIN middleware will be setup for a self-host solution.</summary>
        Self,

        /// <summary>The OWIN middleware will be setup to be hosted is IIS.</summary>
        InternetInformationServices
    }
}