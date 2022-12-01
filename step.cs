using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame
{
    internal class step
    {
        public step(long sec, float x, float y)
        {
            this.sec = sec;
            this.x = x;
            this.y = y;
        }
        public long sec { get; set; }    
        public float x { get; set; }
        public float y { get; set; }
        public string tostring()
        {
            return sec.ToString()+";"+x.ToString()+";"+y.ToString();
        }
        public int CompareTo(step other)
        {
            if (null == other)
            {
                return 1;
            }
            //return this.Id.CompareTo(other.Id);//growing
            return other.sec.CompareTo(this.sec);//descending
        }
    }
}
