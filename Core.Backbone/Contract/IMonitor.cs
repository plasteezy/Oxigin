using System.Threading.Tasks;
using Global.Models;

namespace Core.Backbone.Contract
{
    public interface IMonitor
    {
        Listen Listen { get; set; }

        Task InitMessenger();

        void Start(string consumer);

        void Kill();

        Task SubscribeToChannel(Listen listen);
    }
}