using System;
using System.Collections.Generic;

namespace ChargifyNET
{
    internal sealed class ExceptionHandler
    {
        IDictionary<Func<ChargifyException, bool>, Action<ChargifyException>> ActionsToPerformUponFailure = new Dictionary<Func<ChargifyException, bool>, Action<ChargifyException>>();

        public ExceptionHandler Add(Func<ChargifyException, bool> @if,
            Action<ChargifyException> then)
        {
            ActionsToPerformUponFailure.Add(@if, then);
            return this;
        }

        public bool Evaluate(ChargifyException e)
        {
            foreach (var action in ActionsToPerformUponFailure)
            {
                if (action.Key(e))
                {
                    action.Value(e);
                    return false;
                }
            }

            return true;
        }
    }
}