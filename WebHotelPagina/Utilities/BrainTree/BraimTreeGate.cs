using Braintree;
using Microsoft.Extensions.Options; 

namespace SlnWeb.Utilities.BrainTree
{
    public class BraimTreeGate : IBrainTreeGate
    {
        public Configuracion _options { get; set; }
        public IBraintreeGateway brain { get; set; }
        public BraimTreeGate(IOptions<Configuracion> options)
        {
            _options = options.Value;
        }

        public IBraintreeGateway CreateGateWay()
        {
            return new BraintreeGateway(_options.Environment,_options.MerchanId,
                                        _options.PublicKey,_options.PrivateKey); 
        }

        public IBraintreeGateway GetGateWay()
        {
            if (brain==null) 
            { 
                brain=CreateGateWay();  
            }
            return brain;
        }
    }
}
