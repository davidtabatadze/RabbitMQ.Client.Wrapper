using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Wrapper.Test
{
    public class ModelPublisher : RabbitPublisher<Model>
    {
        private bool _overrideLogs { get; set; }
        private bool _hideLogs { get; set; }
        public ModelPublisher(RabbitPublisherConfiguration configuration, ILoggerFactory logger = null, bool overrideLogs = false, bool hideLogs = false)
            : base(configuration, logger?.CreateLogger(typeof(ModelPublisher))) { _overrideLogs = overrideLogs; _hideLogs = hideLogs; }


    }
}
