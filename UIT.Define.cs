using System;
using System.Runtime.InteropServices;

namespace TVS
{
    //数据转换使用
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public partial class UIT
    {
        [FieldOffset(0)]
        private Byte b0;
        [FieldOffset(1)]
        private Byte b1;
        [FieldOffset(2)]
        private Byte b2;
        [FieldOffset(3)]
        private Byte b3;

        [FieldOffset(0)]
        private Int32 i;

        [FieldOffset(0)]
        private float f;

        //
        #region set and get
        //

        public Byte B0
        {
            set
            {
                b0 = value;
            }
            get
            {
                return b0;
            }
        }
        public Byte B1
        {
            set
            {
                b1 = value;
            }
            get
            {
                return b1;
            }
        }
        public Byte B2
        {
            set
            {
                b2 = value;
            }
            get
            {
                return b2;
            }
        }
        public Byte B3
        {
            set
            {
                b3 = value;
            }
            get
            {
                return b3;
            }
        }
        public Int32 I
        {
            set
            {
                i = value;
            }
            get
            {
                return i;
            }
        }
        public float F
        {
            set
            {
                f = value;
            }
            get
            {
                return f;
            }
        }

        //
        #endregion
        //

    }
}

//end

