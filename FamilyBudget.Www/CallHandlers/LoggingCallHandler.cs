using FamilyBudget.Www.App_Utils;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyBudget.Www.CallHandlers
{
    public class LoggingCallHandler : ICallHandler
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            // Before invoking the method on the original target    
            WriteInfo(String.Format("{0}... - {1}", input.MethodBase, DateTime.Now.ToLongTimeString()));
            
            // Invoke the next handler in the chain    
            var result = getNext().Invoke(input, getNext);    
            
            // After invoking the method on the original target    
            if (result.Exception != null)
            {
                WriteError(String.Format("{0} threw exception {1} at {2}", input.MethodBase, result.Exception.Message, DateTime.Now.ToLongTimeString()));
            }
            else
            {
                WriteInfo(String.Format("{0} OK - {2}", input.MethodBase, result.ReturnValue, DateTime.Now.ToLongTimeString()));
            }

            return result;
        }

        public int Order { get; set; }

        private void WriteInfo(string message)
        {
            Logger.Info(message);
        }

        private void WriteError(string message)
        {
            Logger.Error(message);
        }
    }
}