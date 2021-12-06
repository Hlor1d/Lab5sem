using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.IO;

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

    abstract class V2Data : IEnumerable<DataItem>
    {
        public string str { get;protected set; }
        public DateTime mydate { get; protected set; }
        public abstract IEnumerator<DataItem> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
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

    class V2DataListEnumerator : IEnumerator<DataItem>
    {
        List<DataItem> Mas = new List<DataItem>();
        int curIndex;
        DataItem curdat;
        public V2DataListEnumerator(List<DataItem> Mas1)
        {
            Mas = Mas1;
            curIndex = -1;
            curdat = default(DataItem);
        }

        public bool MoveNext()
        {
            if (++curIndex >= Mas.Count)
            {
                return false;
            }
            else
            {
                curdat = Mas[curIndex];
            }
            return true;
        }

        public void Reset() { curIndex = -1; }

        void IDisposable.Dispose() { }

        public DataItem Current
        {
            get { return curdat; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }

    class   V2DataList : V2Data
    {
        public override IEnumerator<DataItem> GetEnumerator()
        {
            return new V2DataListEnumerator(MAS);
        }
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

        public bool SaveAsText(string filename)
        {
            bool mybl = true;
            try
            {
                using (StreamWriter sw = new StreamWriter(filename, true, System.Text.Encoding.Default))
                {
                    sw.WriteLine(MAS.Count);
                    for (int i = 0; i < MAS.Count;i++)
                    {
                        sw.WriteLine(MAS[i].val.Real);
                        sw.WriteLine(MAS[i].val.Imaginary);
                        sw.WriteLine(MAS[i].vec.X);
                        sw.WriteLine(MAS[i].vec.Y);
                    }
                }
            }
            catch
            {
                mybl = false;
            }
            return mybl;
        }

        public bool LoadAsText(string filename)
        {
            bool mybl = true;
            try
            {
                using (StreamReader sr = new StreamReader(filename, System.Text.Encoding.Default))
                {
                    string line;
                    line = sr.ReadLine();
                    var cnt = Int32.Parse(line);
                    List<DataItem> MAS = new List<DataItem>();
                    for (int i = 0; i < cnt; i++)
                    {
                        line = sr.ReadLine();
                        var vr = float.Parse(line);
                        line = sr.ReadLine();
                        var vi = float.Parse(line);
                        line = sr.ReadLine();
                        var vx = float.Parse(line);
                        line = sr.ReadLine();
                        var vy = float.Parse(line);
                        var compl = new Complex(vr, vi);
                        var vecccc = new Vector2(vx, vy);
                        var dtitem = new DataItem(vecccc, compl);
                        MAS.Add(dtitem);
                    }

                }
            }
            catch
            {
                mybl = false;
            }
            return mybl;
        }
    }

    class V2DataArrayEnumerator : IEnumerator<DataItem>
    {
        List<DataItem> Mas = new List<DataItem>();
        int curIndex;
        DataItem curdat;
        public V2DataArrayEnumerator(int ox1, int oy1, Vector2 step1, Fv2Complex F)
        {
            for (int i = 0; i < ox1; i++)
            {
                for (int j = 0; j < oy1; j++)
                {
                    Vector2 vecc = new Vector2(step1.X * i, step1.Y * j);
                    DataItem datt = new DataItem(vecc, F(vecc));
                    Mas.Add(datt);
                }
            }
            curIndex = -1;
            curdat = default(DataItem);
        }
        public bool MoveNext()
        {
            if (++curIndex >= Mas.Count)
            {
                return false;
            }
            else
            {
                curdat = Mas[curIndex];
            }
            return true;
        }

        public void Reset() { curIndex = -1; }

        void IDisposable.Dispose() { }

        public DataItem Current
        {
            get { return curdat; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }

    class V2DataArray : V2Data
    {
        Fv2Complex F;
        public int ox { get;private set; }
        public int oy { get; private set; }
        public Vector2 step { get; private set; }
        public Complex[,] val { get; private set; }
        public override IEnumerator<DataItem> GetEnumerator()
        {
            return new V2DataArrayEnumerator(ox, oy, step, F);
        }
        public V2DataArray(string a1, DateTime b1) : base(a1, b1)
        {
            this.mydate = b1;
            this.str = a1;
            this.val = new Complex[0, 0];
        }
        public V2DataArray(string a1, DateTime b1, int ox1, int oy1, Vector2 step1, Fv2Complex F1) : base(a1, b1)
        {
            F = F1;
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
        public bool SaveBinary (string filename)
        {
            bool mybl = true;
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate)))
                {
                    writer.Write(ox);
                    writer.Write(oy);
                    writer.Write(step.X);
                    writer.Write(step.Y);
                    for (int i = 0; i < ox; i++)
                    {
                        for (int j = 0; i < oy; j++)
                        {
                            writer.Write(val[i, j].Real);
                            writer.Write(val[i, j].Imaginary);
                        }
                    }
                }
            }
            catch
            {
                mybl = false;
            }
            return mybl;
        }
        public bool LoadBinary(string filename)
        {
            bool mybl = true;
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
                {
                    ox = reader.ReadInt32();
                    oy = reader.ReadInt32();
                    float myx = reader.ReadSingle();
                    float myy = reader.ReadSingle();
                    step = new Vector2(myx, myy);
                    val = new Complex[ox,oy];
                    for (int i = 0; i < ox; i++)
                    {
                        for (int j = 0; i < oy; j++)
                        {
                            float myyx= reader.ReadSingle();
                            float myyy = reader.ReadSingle();
                            val[i, j] = new Complex(myyx, myyy);
                        }
                    }
                }
            }
            catch
            {
                mybl = false;
            }
            return mybl;
        }
    }
    class V2MainCollectionEnumerator : IEnumerator<V2Data>
    {
        List<V2Data> Mas = new List<V2Data>();
        int curIndex;
        V2Data curdat;
        public V2MainCollectionEnumerator(List<V2Data> Mas1)
        {
            this.Mas = Mas1;
            this.curIndex = -1;
            this.curdat = default(V2Data);
        }

        public bool MoveNext()
        {
            if (++curIndex >= Mas.Count)
            {
                return false;
            }
            else
            {
                curdat = Mas[curIndex];
            }
            return true;
        }

        public void Reset() { curIndex = -1; }

        public V2Data Current
        {
            get { return curdat; }
        }

        void IDisposable.Dispose() { }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }

    class V2MainCollection : IEnumerable<V2Data>
    {
        public IEnumerator<V2Data> GetEnumerator()
        {
            return new V2MainCollectionEnumerator(MAS);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
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
        public float MaxDistance
        {
            get
            {
                float maxxx = float.NaN;
                var comb = from s in MAS
                           from s1 in s
                           from s2 in s
                           select new { p1 = s1, p2 = s2 };
                maxxx = (from s in comb select Vector2.Distance(s.p1.vec, s.p2.vec)).Max();
                return maxxx;
            }
        }

        public IEnumerable<Vector2> uniq
        {
            get
            {
                IEnumerable<Vector2> comb = null;
                comb = (from s in MAS
                        from s1 in s
                        select s1.vec).Distinct();
                return comb;
            }
        }

        public IEnumerable<V2DataList> wthzero
        {
            get
            {
                var comb1 = from s in MAS.OfType<V2DataList>()
                            where (!(from s1 in s select s1.val.Imaginary).Contains(0))
                            select s;
                return comb1;
            }
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
        public static Complex myfunc3(Vector2 v)
        {
            Complex c = new Complex(v.X+10, v.Y+10);
            return c;
        }
    }

    class Program
    {
        static void test0()
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
            col.Add(lst2);
            col.Add(arr2);
            Console.WriteLine(col.ToLongString("F2"));
            Console.WriteLine("____________________________");
            float a = col.MaxDistance;
            IEnumerable<Vector2> a1 = col.uniq;
            var a2 = col.wthzero;
        }

        static void test1()
        {
            DateTime date1 = DateTime.Now;
            Fv2Complex F = Forfunc.myfunc1;
            Vector2 v = new Vector2(1, 1);

            V2DataArray arr1 = new V2DataArray("ID01", date1, 3, 3, v, F);
            arr1.SaveBinary(@"C:\ctest\arr1.dat");
            V2DataArray arr2 = new V2DataArray("ID01", date1);
            arr2.LoadBinary(@"C:\ctest\arr1.dat");

            Console.WriteLine(arr1.ToLongString("F2"));
            Console.WriteLine(arr2.ToLongString("F2"));


            V2DataList lst1 = (V2DataList)arr1;
            lst1.SaveAsText(@"C:\ctest\lst1.txt");
            V2DataList lst2 = new V2DataList("ID01", date1);
            lst2.LoadAsText(@"C:\ctest\lst1.txt");

            Console.WriteLine(lst1.ToLongString("F2"));
            Console.WriteLine(lst1.ToLongString("F2"));

        }

        static void test2()
        {
            DateTime date1 = DateTime.Now;
            Fv2Complex F = Forfunc.myfunc1;
            Vector2 v = new Vector2(1, 1);
            V2DataArray arr1 = new V2DataArray("ID01", date1, 4, 4, v, F);
            V2MainCollection col = new V2MainCollection();
            col.Add(arr1);

            Fv2Complex F1 = Forfunc.myfunc2;
            Vector2 v1 = new Vector2(1, 2);
            V2DataArray arr2 = new V2DataArray("ID02", date1, 3, 3, v1, F1);
            col.Add(arr2);

            Fv2Complex F2 = Forfunc.myfunc3;
            V2DataArray arr3 = new V2DataArray("ID02", date1, 4, 4, v, F2);
            V2DataList lst3 = (V2DataList)arr3;
            col.Add(lst3);


            Console.WriteLine(col.ToLongString("F2"));

            float a = col.MaxDistance;
            IEnumerable<Vector2> a1 = col.uniq;
            var a2 = col.wthzero;
        }
        static void Main(string[] args)
        {
            test1();
            Console.WriteLine("__________________________");
            test2();
        }
    }
}
