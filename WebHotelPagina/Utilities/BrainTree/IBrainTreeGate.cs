using Braintree;

namespace SlnWeb.Utilities.BrainTree
{
    public interface IBrainTreeGate
    {
        IBraintreeGateway CreateGateWay();
        IBraintreeGateway GetGateWay();

    }
}
