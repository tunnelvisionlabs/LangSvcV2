namespace Tvl.VisualStudio.Language.Parsing.Experimental.Atn
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public class Network
    {
        private readonly List<RuleBinding> _rules;
        private readonly Dictionary<int, State> _states;
        private readonly List<Transition> _transitions;
        private readonly Dictionary<int, string> _stateRules;

        public Network(IEnumerable<RuleBinding> rules)
        {
            Contract.Requires<ArgumentNullException>(rules != null, "rules");

            _rules = new List<RuleBinding>(rules);

            Dictionary<int, string> stateRules = new Dictionary<int, string>();
            foreach (var rule in _rules)
            {
                stateRules[rule.StartState.Id] = rule.Name;
                stateRules[rule.EndState.Id] = rule.Name;
            }

            HashSet<State> states = new HashSet<State>();
            HashSet<Transition> transitions = new HashSet<Transition>();

            foreach (var rule in _rules)
            {
                ExtractStatesAndTransitions(rule.Name, rule.StartState, states, transitions, stateRules);
                ExtractStatesAndTransitions(rule.Name, rule.EndState, states, transitions, stateRules);
            }

            _states = states.ToDictionary(i => i.Id);
            _transitions = new List<Transition>(transitions);
            _stateRules = stateRules;
        }

        private static void ExtractStatesAndTransitions(string currentRule, State currentState, HashSet<State> states, HashSet<Transition> transitions, Dictionary<int, string> stateRules)
        {
            if (!states.Add(currentState))
                return;

            stateRules[currentState.Id] = currentRule;

            foreach (var transition in currentState.OutgoingTransitions)
            {
                if (transitions.Add(transition))
                {
                    if (transition.IsContext)
                    {
                        PushContextTransition pushContext = transition as PushContextTransition;
                        if (pushContext != null)
                        {
                            ExtractStatesAndTransitions(currentRule, pushContext.PopTransition.TargetState, states, transitions, stateRules);
                            continue;
                        }

                        PopContextTransition popContext = transition as PopContextTransition;
                        if (popContext != null)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        ExtractStatesAndTransitions(currentRule, transition.TargetState, states, transitions, stateRules);
                    }
                }
            }
        }

        public IDictionary<int, State> States
        {
            get
            {
                return _states;
            }
        }

        public ReadOnlyCollection<Transition> Transitions
        {
            get
            {
                return _transitions.AsReadOnly();
            }
        }

        public ReadOnlyCollection<RuleBinding> Rules
        {
            get
            {
                return _rules.AsReadOnly();
            }
        }

        public Dictionary<int, string> StateRules
        {
            get
            {
                return _stateRules;
            }
        }

        public RuleBinding GetRule(string name)
        {
            return _rules.Single(i => string.Equals(i.Name, name));
        }
    }
}
