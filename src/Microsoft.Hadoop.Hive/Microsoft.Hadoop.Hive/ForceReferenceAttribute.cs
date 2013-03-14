using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Hadoop.Hive
{
   [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
   public sealed class ForceReferenceAttribute : Attribute
   {
      public ForceReferenceAttribute(Type type)
      {
         this.Type = type;
      }

      public Type Type { get; private set; }
   }
}
