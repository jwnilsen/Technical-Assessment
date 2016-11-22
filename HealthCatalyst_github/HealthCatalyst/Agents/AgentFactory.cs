
namespace HealthCatalyst.Agents
{
    public class AgentFactory
    {
        private PersonAgent _personAgent;

        public AgentFactory()
        {
        }

        public PersonAgent personAgent
        {
            get
            {
                if (_personAgent == null)
                    _personAgent = new PersonAgent();

                return _personAgent;
            }
        }
    }
}
