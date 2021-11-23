using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CSprac1
{
    struct DataItem
    {
        public Vector2 vec { get; set; }
        public Complex val { get; set; }
        public DataItem(Vector2 a, Complex b)
        {
            this.val = b;
            this.vec = a;
        }
        public string ToLongString(string format)
        {
            return vec + " " + " " + val + " " + val.Magnitude.ToString(format);
        }
        public override string ToString()
        {
            return vec.X.ToString() + " " + vec.X.ToString() + " " + val.ToString();
        }

    }

    abstract class V2Data
    {
        public string str { get; set; }
        public DateTime mydate { get; set; }
        public V2Data(string a, DateTime b)
        {
            this.mydate = b;
            this.str = a;
        }
        public abstract int Count { get; }
        public abstract float MinDistance { get; }
        public abstract string ToLongString(string format);
        public override string ToString()
        {
            return str.ToString() + " " + mydate.ToString();
        }

    }

    delegate Complex Fv2Complex(Vector2 v2);

    class V2DataList : V2Data
    {
        List<DataItem> MAS = new List<DataItem>();
        public List<DataItem> mygget { get; }
        public V2DataList(string a1, DateTime b1) : base(a1, b1)
        {
            this.mydate = b1;
            this.str = a1;
        }
        public bool Add(DataItem newItem)
        {
            bool a = false;
            if (MAS != null) { a = MAS.Contains(newItem); }
            if (!a) MAS.Add(newItem);
            return !a;
        }
        public int AddDefaults(int nItems, Fv2Complex F)
        {
            int a = 0;
            for (int i = 0; i < nItems; i++)
            {
                Vector2 v = new Vector2(i, i);
                Complex c = F(v);
                DataItem d = new DataItem(v, c);
                if (Add(d)) a++;
            }
            return a;
        }
        public override int Count
        {
            get
            {
                return MAS.Count();
            }
        }
        public override float MinDistance
        {
            get
            {
                float minn = float.MaxValue;
                for (int i = 0; i < Count - 1; i++)
                {
                    for (int j = i + 1; j < Count; j++)
                    {
                        if ((MAS[i].vec - MAS[j].vec).Length() < minn)
                        {
                            minn = (MAS[i].vec - MAS[j].vec).Length();
                        }
                    }
                }
                return minn;
            }
        }
        public override string ToString()
        {
            return MAS.GetType() + " " + this.mydate + " " + this.str + " " + Count.ToString();
        }
        public override string ToLongString(string format)
        {
            string mys = MAS.GetType() + " " + this.mydate + " " + this.str + " " + Count.ToString();
            for (int i = 0; i < Count; i++)
            {
                mys += "\n" + MAS[i].vec + " " + MAS[i].val + " " + MAS[i].val.Magnitude.ToString(format);
            }
            return mys;
        }
    }

    class V2DataArray : V2Data
    {
        public int ox { get; set; }
        public int oy { get; set; }
        public Vector2 step { get; set; }
        public Complex[,] val { get; set; }
        public V2DataArray(string a1, DateTime b1) : base(a1, b1)
        {
            this.mydate = b1;
            this.str = a1;
            this.val = new Complex[0, 0];
        }
        public V2DataArray(string a1, DateTime b1, int ox1, int oy1, Vector2 step1, Fv2Complex F) : base(a1, b1)
        {
            this.mydate = b1;
            this.str = a1;
            ox = ox1;
            oy = oy1;
            this.step = step1;
            this.val = new Complex[ox1, oy1];
            for (int i = 0; i < ox1; i++)
            {
                for (int j = 0; j < oy1; j++)
                {
                    Vector2 vecc = new Vector2(step1.X * i, step1.Y * j);
                    val[i, j] = F(vecc);
                }
            }
        }
        public override int Count
        {
            get
            {
                return val.Length;
            }
        }
        public override float MinDistance
        {
            get
            {
                return Math.Min(step.X, step.Y);
            }
        }
        public override string ToString()
        {
            string mys = this.GetType() + " " + this.mydate + " " + this.str + " " + this.ox + " " + this.oy + " " + this.step + " ";
            return mys;
        }
        public override string ToLongString(string format)
        {
            string mys = this.GetType() + " " + this.mydate + " " + this.str + " " + this.ox + " " + this.oy + " " + this.step + "\n";
            Console.WriteLine(ox);
            for (int i = 0; i < this.ox; i++)
            {
                for (int j = 0; j < this.oy; j++)
                {
                    Vector2 vecc = new Vector2(step.X * i, step.Y * j);
                    mys += vecc.ToString() + " " + val[i, j].ToString() + " " + this.val[i, j].Magnitude.ToString(format) + "\n";
                }
            }
            return mys;
        }
        public static implicit operator V2DataList(V2DataArray arr)
        {
            V2DataList rez = new V2DataList(arr.str, arr.mydate);
            for (int i = 0; i < arr.ox; i++)
            {
                for (int j = 0; j < arr.oy; j++)
                {
                    Vector2 vecc = new Vector2(arr.step.X * i, arr.step.Y * j);
                    DataItem a1 = new DataItem(vecc, arr.val[i, j]);
                    rez.Add(a1);
                }
            }
            return rez;
        }
    }

    class V2MainCollection
    {
        List<V2Data> MAS = new List<V2Data>();
        public int Count
        {
            get
            {
                return MAS.Count();
            }
        }
        public V2Data this[int index]
        {
            get
            {
                return MAS[index];
            }
        }
        public bool Contains(string ID)
        {
            bool rez = false;
            for (int i = 0; i < MAS.Count; i++)
            {
                if (MAS[i].str == ID) rez = true;
            }
            return rez;
        }
        public bool Add(V2Data v2Data)
        {
            bool rez = this.Contains(v2Data.str);
            if (!rez) MAS.Add(v2Data);
            return !rez;
        }
        public string ToLongString(string format)
        {
            string mys = "";
            for (int i = 0; i < this.Count; i++)
            {
                mys += this[i].ToLongString(format) + "\n\n";
            }
            return mys;
        }
        public override string ToString()
        {
            string mys = "";
            for (int i = 0; i < this.Count; i++)
            {
                mys += this[i].ToString() + "\n\n";
            }
            return mys;
        }
    }
    static class Forfunc
    {
        public static Complex myfunc1(Vector2 v)
        {
            Complex c = new Complex(v.X, v.Y);
            return c;
        }
        public static Complex myfunc2(Vector2 v)
        {
            Complex c = new Complex(v.X * v.X, v.Y + 10);
            return c;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            DateTime date1 = DateTime.Now;
            Fv2Complex F = Forfunc.myfunc1;
            Vector2 v = new Vector2(1, 1);
            V2DataArray arr1 = new V2DataArray("ID01", date1, 3, 3, v, F);
            Console.WriteLine(arr1.ToLongString("F2"));
            V2DataList lst1 = (V2DataList)arr1;
            Console.WriteLine(lst1.ToLongString("F2"));
            Console.WriteLine("Count\n" + arr1.Count + "\n" + lst1.Count);
            Console.WriteLine("MinDistance\n" + arr1.MinDistance + "\n" + lst1.MinDistance);
            V2MainCollection col = new V2MainCollection();
            col.Add(arr1);
            col.Add(lst1);
            Fv2Complex F1 = Forfunc.myfunc2;
            Vector2 v1 = new Vector2(1, 2);
            V2DataArray arr2 = new V2DataArray("ID02", date1, 2, 2, v1, F1);
            V2DataList lst2 = (V2DataList)arr2;
            col.Add(arr2);
            col.Add(lst2);
            Console.WriteLine(col.ToLongString("F2"));
            Console.ReadLine();
        }
    }
}
