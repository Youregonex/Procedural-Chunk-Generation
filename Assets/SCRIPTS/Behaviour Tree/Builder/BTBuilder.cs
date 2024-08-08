
namespace Youregone.BehaviourTrees
{
    public class BTBuilder
    {
        private Composite _compositeNode;

        public BTBuilder StartBuildingSelector()
        {
            _compositeNode = new Selector();

            return this;
        }

        public BTBuilder StartBuildingSequence()
        {
            _compositeNode = new Sequence();

            return this;
        }

        public BTBuilder WithCondition(ConditionNode conditionNode)
        {
            _compositeNode.AddChildNode(conditionNode);

            return this;
        }

        public BTBuilder WithBehaviour(BehaviourNode behaviourNode)
        {
            _compositeNode.AddChildNode(behaviourNode);

            return this;
        }

        public BTBuilder WithInverter(Inverter inverter)
        {
            _compositeNode.AddChildNode(inverter);

            return this;
        }

        public BTBuilder WithSequence(Composite sequence)
        {
            _compositeNode.AddChildNode(sequence);

            return this;
        }

        public BTBuilder WithSelector(Composite selector)
        {
            _compositeNode.AddChildNode(selector);

            return this;
        }

        public Composite Build()
        {
            Composite compositeNode = _compositeNode;
            _compositeNode = new Composite();

            return compositeNode;
        }
    }
}