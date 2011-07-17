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
        private readonly Dictionary<int, RuleBinding> _stateRules;
        private readonly Dictionary<int, RuleBinding> _contextRules;

        public Network(IEnumerable<RuleBinding> rules, Dictionary<int, RuleBinding> stateRules)
        {
            Contract.Requires<ArgumentNullException>(rules != null, "rules");

            _rules = new List<RuleBinding>(rules);

            //Dictionary<int, string> stateRules = new Dictionary<int, string>();
            //foreach (var rule in _rules)
            //{
            //    stateRules[rule.StartState.Id] = rule.Name;
            //    stateRules[rule.EndState.Id] = rule.Name;
            //}

            HashSet<State> states = new HashSet<State>();
            HashSet<Transition> transitions = new HashSet<Transition>();
            Dictionary<int, RuleBinding> contextRules = new Dictionary<int, RuleBinding>();

            foreach (var rule in _rules)
            {
                ExtractStatesAndTransitions(rule, rule.StartState, states, transitions, stateRules, contextRules);
                //ExtractStatesAndTransitions(rule.Name, rule.EndState, states, transitions, stateRules, contextRules);
            }

            _states = states.ToDictionary(i => i.Id);
            _transitions = new List<Transition>(transitions);
            _stateRules = stateRules;
            _contextRules = contextRules;
        }

        private static void ExtractStatesAndTransitions(RuleBinding currentRule, State currentState, HashSet<State> states, HashSet<Transition> transitions, Dictionary<int, RuleBinding> stateRules, Dictionary<int, RuleBinding> contextRules)
        {
            if (!states.Add(currentState))
                return;

            currentRule = currentRule ?? stateRules[currentState.Id];

            foreach (var transition in currentState.OutgoingTransitions)
            {
                if (transitions.Add(transition))
                {
                    if (transition.IsContext)
                    {
                        PushContextTransition pushContext = transition as PushContextTransition;
                        if (pushContext != null)
                        {
                            contextRules[pushContext.ContextIdentifiers[0]] = currentRule;

                            foreach (var popTransition in pushContext.PopTransitions)
                            {
                                // make sure this pop transition ends in this rule
                                if (popTransition.ContextIdentifiers.Last() != pushContext.ContextIdentifiers.First())
                                    throw new InvalidOperationException();

                                ExtractStatesAndTransitions(currentRule, popTransition.TargetState, states, transitions, stateRules, contextRules);
                            }

                            ExtractStatesAndTransitions(null, transition.TargetState, states, transitions, stateRules, contextRules);
                            continue;
                        }

                        PopContextTransition popContext = transition as PopContextTransition;
                        if (popContext != null)
                        {
                            continue;
                        }

                        throw new InvalidOperationException("Unrecognized context transition.");
                    }
                    else
                    {
                        ExtractStatesAndTransitions(currentRule, transition.TargetState, states, transitions, stateRules, contextRules);
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

        public Dictionary<int, RuleBinding> StateRules
        {
            get
            {
                return _stateRules;
            }
        }

        public Dictionary<int, RuleBinding> ContextRules
        {
            get
            {
                return _contextRules;
            }
        }

        public RuleBinding GetRule(string name)
        {
            return _rules.Single(i => string.Equals(i.Name, name));
        }
    }
}
